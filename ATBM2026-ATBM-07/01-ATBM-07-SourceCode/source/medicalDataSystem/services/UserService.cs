namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public sealed class UserService
{
    private readonly OracleConnectionService _connectionService;
    private readonly RBACService _rbacService;
    private readonly VPDService _vpdService;
    private static readonly string[] RevocableRoles =
    {
        "DIEU_PHOI_VIEN",
        "BAC_SI_Y_SI",
        "KY_THUAT_VIEN",
        "BENH_NHAN"
    };
    public string LastErrorMessage { get; private set; } = string.Empty;
    public string LastRoleOperation { get; private set; } = string.Empty;

    public UserService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
        _rbacService = new RBACService(connectionService);
        _vpdService = new VPDService(connectionService);
    }

    public bool CreateUser(CreateUserRequest request)
    {
        LastErrorMessage = string.Empty;

        if (!ValidateCreateUserRequest(request, out string validationMessage))
        {
            LastErrorMessage = validationMessage;
            return false;
        }

        string safeUsername;
        try
        {
            safeUsername = NormalizeAndValidateOracleUsername(request.Username);
        }
        catch (ArgumentException ex)
        {
            LastErrorMessage = ex.Message;
            return false;
        }

        using var connection = _connectionService.GetConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            if (UsernameExists(connection, transaction, safeUsername))
            {
                LastErrorMessage = $"Username {safeUsername} already exists.";
                transaction.Rollback();
                return false;
            }

            if (string.Equals(request.UserType, "STAFF", StringComparison.OrdinalIgnoreCase))
            {
                InsertStaff(connection, transaction, request, safeUsername);
            }
            else
            {
                InsertPatient(connection, transaction, request, safeUsername);
            }

            EnsureOracleUser(connection, transaction, safeUsername);
            ExecuteNonQuery(connection, transaction, $"GRANT CREATE SESSION TO {Q(safeUsername)}");

            if (string.Equals(request.UserType, "STAFF", StringComparison.OrdinalIgnoreCase))
            {
                string roleName = MapStaffRoleToDbRole(request.Role);
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    LastErrorMessage = "Unsupported staff role.";
                    transaction.Rollback();
                    return false;
                }

                ExecuteNonQuery(connection, transaction, $"GRANT {roleName} TO {Q(safeUsername)}");
            }
            else
            {
                ExecuteNonQuery(connection, transaction, $"GRANT BENH_NHAN TO {Q(safeUsername)}");
            }

            transaction.Commit();
            return true;
        }
        catch (OracleException ex)
        {
            TryRollback(transaction);
            LastErrorMessage = ex.Number == -1920
                ? $"Oracle user {safeUsername} already exists and could not be updated."
                : $"Oracle error {ex.Number}: {ex.Message}";
            return false;
        }
        catch (Exception ex)
        {
            TryRollback(transaction);
            LastErrorMessage = ex.Message;
            return false;
        }
    }

    public List<PatientAccountItem> GetPatientUsersByCccd(string cccdKeyword)
    {
        return _rbacService.GetPatientUsersByCccd(cccdKeyword);
    }

    public List<PatientUserDisplayItem> GetPatientUserDisplayByCccd(string cccdKeyword)
    {
        return _rbacService.GetPatientUserDisplayByCccd(cccdKeyword);
    }

    public List<DepartmentOption> GetDepartments()
    {
        const string sql = """
            SELECT MAKHOA, TENKHOA
            FROM KHOA
            ORDER BY MAKHOA
            """;

        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var reader = command.ExecuteReader();
            var departments = new List<DepartmentOption>();
            while (reader.Read())
            {
                string maKhoa = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                string tenKhoa = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                departments.Add(new DepartmentOption
                {
                    MAKHOA = maKhoa,
                    TENKHOA = tenKhoa
                });
            }

            return departments;
        });
    }

    public List<UserAccountItem> GetStaffUsersByCmnd(string cmndKeyword)
    {
        return _rbacService.GetStaffUsersByCmnd(cmndKeyword);
    }

    public List<StaffUserDisplayItem> GetStaffUserDisplayByCmnd(string cmndKeyword)
    {
        return _rbacService.GetStaffUserDisplayByCmnd(cmndKeyword);
    }

    public List<VpdPolicyItem> GetVpdPolicies()
    {
        return _vpdService.GetVpdPolicies();
    }

    public bool SetVpdPolicyEnabled(string objectName, string policyName, bool enable)
    {
        bool ok = _vpdService.SetVpdPolicyEnabled(objectName, policyName, enable);
        LastErrorMessage = _vpdService.LastErrorMessage;
        return ok;
    }

    public string NormalizeIdText(string input)
    {
        return _rbacService.NormalizeIdText(input);
    }

    public UserSecurityProfileItem? GetStaffProfileWithRolesByCmnd(string cmndKeyword)
    {
        return _rbacService.GetStaffProfileWithRolesByCmnd(cmndKeyword);
    }

    public UserSecurityProfileItem? GetPatientProfileWithRolesByCccd(string cccdKeyword)
    {
        return _rbacService.GetPatientProfileWithRolesByCccd(cccdKeyword);
    }

    public bool GrantRoleToStaff(string username, string roleName)
    {
        bool ok = _rbacService.GrantRoleToStaff(username, roleName);
        LastErrorMessage = _rbacService.LastErrorMessage;
        LastRoleOperation = _rbacService.LastRoleOperation;
        return ok;
    }

    public bool RevokeRoleFromUser(string username, string roleName)
    {
        bool ok = _rbacService.RevokeRoleFromUser(username, roleName);
        LastErrorMessage = _rbacService.LastErrorMessage;
        LastRoleOperation = _rbacService.LastRoleOperation;
        return ok;
    }

    public bool RevokeUserAccess(string username)
    {
        LastErrorMessage = string.Empty;

        string safeUsername;
        try
        {
            safeUsername = NormalizeAndValidateOracleUsername(username);
        }
        catch (ArgumentException ex)
        {
            LastErrorMessage = ex.Message;
            return false;
        }

        using var connection = _connectionService.GetConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            if (!UsernameExists(connection, transaction, safeUsername))
            {
                LastErrorMessage = $"Username {safeUsername} does not exist.";
                transaction.Rollback();
                return false;
            }

            foreach (string roleName in RevocableRoles)
            {
                if (HasRoleGrant(connection, transaction, safeUsername, roleName))
                {
                    ExecuteNonQuery(connection, transaction, $"REVOKE {roleName} FROM {Q(safeUsername)}");
                }
            }

            if (HasSystemPrivilege(connection, transaction, safeUsername, "CREATE SESSION"))
            {
                ExecuteNonQuery(connection, transaction, $"REVOKE CREATE SESSION FROM {Q(safeUsername)}");
            }

            ExecuteNonQuery(connection, transaction, $"ALTER USER {Q(safeUsername)} ACCOUNT LOCK");

            transaction.Commit();
            LastRoleOperation = "USER_REVOKED";
            return true;
        }
        catch (OracleException ex)
        {
            TryRollback(transaction);
            LastErrorMessage = $"Oracle error {ex.Number}: {ex.Message}";
            return false;
        }
        catch (Exception ex)
        {
            TryRollback(transaction);
            LastErrorMessage = ex.Message;
            return false;
        }
    }

    private List<UserAccountItem> SearchUsers(string searchText, string sql)
    {
        string normalizedKeyword = NormalizeIdKeyword(searchText);
        if (string.IsNullOrWhiteSpace(normalizedKeyword))
        {
            return new List<UserAccountItem>();
        }

        string searchPattern = $"%{normalizedKeyword}%";

        return _connectionService.Execute(connection =>
        {
            using var dataCommand = connection.CreateCommand();
            dataCommand.CommandText = sql;
            dataCommand.Parameters.Add(new OracleParameter("searchTerm", searchPattern));

            using var reader = dataCommand.ExecuteReader();
            var users = new List<UserAccountItem>();
            while (reader.Read())
            {
                users.Add(new UserAccountItem
                {
                    UserId = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    FullName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Username = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    AccountStatus = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    CreatedDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    ExpiryDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
                });
            }

            return users;
        });
    }

    private static string NormalizeIdKeyword(string searchText)
    {
        return new string((searchText ?? string.Empty).Where(char.IsDigit).ToArray());
    }

    private static List<string> GetCurrentRolesByUsername(OracleConnection connection, string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return new List<string>();
        }

        const string sql = """
            SELECT rp.GRANTED_ROLE
            FROM DBA_ROLE_PRIVS rp
            WHERE rp.GRANTEE = :grantee
            ORDER BY rp.GRANTED_ROLE
            """;

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("grantee", username.Trim().ToUpperInvariant()));

        using var reader = command.ExecuteReader();
        var roles = new List<string>();
        while (reader.Read())
        {
            roles.Add(reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
        }

        return roles.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
    }

    private static bool ValidateCreateUserRequest(CreateUserRequest request, out string message)
    {
        message = string.Empty;

        if (request is null)
        {
            message = "Request is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.UserType) ||
            (!string.Equals(request.UserType, "STAFF", StringComparison.OrdinalIgnoreCase) &&
             !string.Equals(request.UserType, "PATIENT", StringComparison.OrdinalIgnoreCase)))
        {
            message = "UserType must be STAFF or PATIENT.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            message = "Username is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Gender) ||
            string.IsNullOrWhiteSpace(request.IDNumber))
        {
            message = "FullName, Gender, and IDNumber are required.";
            return false;
        }

        if (request.BirthDate == default)
        {
            message = "BirthDate is required.";
            return false;
        }

        if (string.Equals(request.UserType, "STAFF", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(request.Address) ||
                string.IsNullOrWhiteSpace(request.Phone) ||
                string.IsNullOrWhiteSpace(request.Role) ||
                string.IsNullOrWhiteSpace(request.Department))
            {
                message = "Address, Phone, Role, and Department are required for STAFF.";
                return false;
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.SONHA) ||
                string.IsNullOrWhiteSpace(request.TENDUONG) ||
                string.IsNullOrWhiteSpace(request.QUANHUYEN) ||
                string.IsNullOrWhiteSpace(request.TINHTP))
            {
                message = "SONHA, TENDUONG, QUANHUYEN, and TINHTP are required for PATIENT.";
                return false;
            }
        }

        return true;
    }

    private static string NormalizeAndValidateGrantRole(string roleName)
    {
        string normalized = (roleName ?? string.Empty).Trim().ToUpperInvariant();
        return normalized switch
        {
            "DIEU_PHOI_VIEN" => "DIEU_PHOI_VIEN",
            "BAC_SI_Y_SI" => "BAC_SI_Y_SI",
            "KY_THUAT_VIEN" => "KY_THUAT_VIEN",
            "BENH_NHAN" => "BENH_NHAN",
            _ => string.Empty
        };
    }

    private static bool IsStaffUsername(OracleConnection connection, OracleTransaction transaction, string username)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM NHANVIEN n
            WHERE UPPER(n.USERNAME) = :username
            """;

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("username", username));
        int count = Convert.ToInt32(command.ExecuteScalar());
        return count > 0;
    }

    private static bool HasRoleGrant(OracleConnection connection, OracleTransaction transaction, string username, string roleName)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM DBA_ROLE_PRIVS rp
            WHERE rp.GRANTEE = :grantee
              AND rp.GRANTED_ROLE = :roleName
            """;

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("grantee", username.Trim().ToUpperInvariant()));
        command.Parameters.Add(new OracleParameter("roleName", roleName.Trim().ToUpperInvariant()));
        int count = Convert.ToInt32(command.ExecuteScalar());
        return count > 0;
    }

    private static bool HasSystemPrivilege(OracleConnection connection, OracleTransaction transaction, string username, string privilege)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM DBA_SYS_PRIVS sp
            WHERE sp.GRANTEE = :grantee
              AND sp.PRIVILEGE = :privilege
            """;

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("grantee", username.Trim().ToUpperInvariant()));
        command.Parameters.Add(new OracleParameter("privilege", privilege.Trim().ToUpperInvariant()));
        int count = Convert.ToInt32(command.ExecuteScalar());
        return count > 0;
    }

    private static string NormalizeAndValidateOracleUsername(string username)
    {
        string normalized = (username ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Username is required.");
        }

        if (normalized.Length > 30)
        {
            throw new ArgumentException("Username must be 30 characters or fewer.");
        }

        bool isValid = normalized.All(ch =>
            (ch >= 'A' && ch <= 'Z') ||
            (ch >= '0' && ch <= '9') ||
            ch == '_' || ch == '$' || ch == '#');

        if (!isValid)
        {
            throw new ArgumentException("Username contains invalid Oracle identifier characters.");
        }

        return normalized;
    }

    private static string Q(string identifier) =>
        '"' + identifier.Replace("\"", "\"\"") + '"';

    private static bool UsernameExists(OracleConnection connection, OracleTransaction transaction, string username)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM (
                SELECT UPPER(USERNAME) AS USERNAME FROM NHANVIEN WHERE USERNAME IS NOT NULL
                UNION ALL
                SELECT UPPER(USERNAME) AS USERNAME FROM BENHNHAN WHERE USERNAME IS NOT NULL
                UNION ALL
                SELECT USERNAME FROM ALL_USERS
            ) u
            WHERE u.USERNAME = :username
            """;

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("username", username));
        int count = Convert.ToInt32(command.ExecuteScalar());
        return count > 0;
    }

    private static void InsertStaff(OracleConnection connection, OracleTransaction transaction, CreateUserRequest request, string username)
    {
        const string sql = """
            INSERT INTO NHANVIEN (
                HOTEN, PHAI, NGAYSINH, CCCD,
                QUEQUAN, SODT, VAITRO, CHUYENKHOA, USERNAME
            )
            VALUES (
                :fullName, :gender, :birthDate, :idNumber,
                :address, :phone, :role, :department, :username
            )
            """;

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("fullName", request.FullName.Trim()));
        command.Parameters.Add(new OracleParameter("gender", request.Gender.Trim()));
        command.Parameters.Add(new OracleParameter("birthDate", request.BirthDate));
        command.Parameters.Add(new OracleParameter("idNumber", request.IDNumber.Trim()));
        command.Parameters.Add(new OracleParameter("address", request.Address.Trim()));
        command.Parameters.Add(new OracleParameter("phone", request.Phone.Trim()));
        command.Parameters.Add(new OracleParameter("role", request.Role.Trim()));
        command.Parameters.Add(new OracleParameter("department", request.Department.Trim()));
        command.Parameters.Add(new OracleParameter("username", username));
        command.ExecuteNonQuery();
    }

    private static void InsertPatient(OracleConnection connection, OracleTransaction transaction, CreateUserRequest request, string username)
    {
        const string sql = """
            INSERT INTO BENHNHAN (
                TENBN, PHAI, NGAYSINH, CCCD,
                SONHA, TENDUONG, QUANHUYEN, TINHTP,
                TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC, USERNAME
            )
            VALUES (
                :fullName, :gender, :birthDate, :idNumber,
                :sonha, :tenduong, :quanhuyen, :tinhtp,
                :tiensuBenh, :tiensuBenhGd, :diungThuoc, :username
            )
            """;

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("fullName", request.FullName.Trim()));
        command.Parameters.Add(new OracleParameter("gender", request.Gender.Trim()));
        command.Parameters.Add(new OracleParameter("birthDate", request.BirthDate));
        command.Parameters.Add(new OracleParameter("idNumber", request.IDNumber.Trim()));
        command.Parameters.Add(new OracleParameter("sonha", request.SONHA.Trim()));
        command.Parameters.Add(new OracleParameter("tenduong", request.TENDUONG.Trim()));
        command.Parameters.Add(new OracleParameter("quanhuyen", request.QUANHUYEN.Trim()));
        command.Parameters.Add(new OracleParameter("tinhtp", request.TINHTP.Trim()));
        command.Parameters.Add(new OracleParameter("tiensuBenh", request.TIENSUBENH.Trim()));
        command.Parameters.Add(new OracleParameter("tiensuBenhGd", request.TIENSUBENHGD.Trim()));
        command.Parameters.Add(new OracleParameter("diungThuoc", request.DIUNGTHUOC.Trim()));
        command.Parameters.Add(new OracleParameter("username", username));
        command.ExecuteNonQuery();
    }

    private static void EnsureOracleUser(OracleConnection connection, OracleTransaction transaction, string username)
    {
        try
        {
            ExecuteNonQuery(connection, transaction, $"CREATE USER {Q(username)} IDENTIFIED BY {Q(username)}");
        }
        catch (OracleException ex) when (ex.Number == -1920)
        {
            ExecuteNonQuery(connection, transaction, $"ALTER USER {Q(username)} IDENTIFIED BY {Q(username)} ACCOUNT UNLOCK");
        }
    }

    private static void ExecuteNonQuery(OracleConnection connection, OracleTransaction transaction, string sql)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static string MapStaffRoleToDbRole(string role)
    {
        string normalizedRole = (role ?? string.Empty).Trim();
        return normalizedRole switch
        {
            "Điều phối viên" => "DIEU_PHOI_VIEN",
            "Dieu phoi vien" => "DIEU_PHOI_VIEN",
            "Bác sĩ/Y sĩ" => "BAC_SI_Y_SI",
            "Bac si/Y si" => "BAC_SI_Y_SI",
            "Kỹ thuật viên" => "KY_THUAT_VIEN",
            "Ky thuat vien" => "KY_THUAT_VIEN",
            _ => string.Empty
        };
    }

    private void TryRollback(OracleTransaction transaction)
    {
        try
        {
            transaction.Rollback();
        }
        catch
        {
            // Ignore rollback failures to preserve original error.
        }
    }
}

public sealed class UserAccountItem
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public sealed class PatientAccountItem
{
    public string MABN { get; set; } = string.Empty;
    public string TENBN { get; set; } = string.Empty;
    public string PHAI { get; set; } = string.Empty;
    public DateTime? NGAYSINH { get; set; }
    public string CCCD { get; set; } = string.Empty;
    public string SONHA { get; set; } = string.Empty;
    public string TENDUONG { get; set; } = string.Empty;
    public string QUANHUYEN { get; set; } = string.Empty;
    public string TINHTP { get; set; } = string.Empty;
    public string TIENSUBENH { get; set; } = string.Empty;
    public string TIENSUBENHGD { get; set; } = string.Empty;
    public string DIUNGTHUOC { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public sealed class DepartmentOption
{
    public string MAKHOA { get; set; } = string.Empty;
    public string TENKHOA { get; set; } = string.Empty;
    public string DisplayText => string.IsNullOrWhiteSpace(TENKHOA) ? MAKHOA : $"{MAKHOA} - {TENKHOA}";

    public override string ToString()
    {
        return DisplayText;
    }
}

public sealed class PatientUserDisplayItem
{
    public string MaBN { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public string NgaySinh { get; set; } = string.Empty;
    public string CCCD { get; set; } = string.Empty;
    public string SoNha { get; set; } = string.Empty;
    public string TenDuong { get; set; } = string.Empty;
    public string QuanHuyen { get; set; } = string.Empty;
    public string TinhTP { get; set; } = string.Empty;
    public string TienSuBenh { get; set; } = string.Empty;
    public string TienSuBenhGiaDinh { get; set; } = string.Empty;
    public string DiUngThuoc { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string TrangThaiTaiKhoan { get; set; } = string.Empty;
    public string TaoLuc { get; set; } = string.Empty;
    public string HetHan { get; set; } = string.Empty;
}

public sealed class StaffUserDisplayItem
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string Expiry { get; set; } = string.Empty;
}

public sealed class VpdPolicyItem
{
    public string ObjectName { get; set; } = string.Empty;
    public string PolicyName { get; set; } = string.Empty;
    public string FunctionOwner { get; set; } = string.Empty;
    public string PolicyFunction { get; set; } = string.Empty;
    public string SelectEnabled { get; set; } = string.Empty;
    public string InsertEnabled { get; set; } = string.Empty;
    public string UpdateEnabled { get; set; } = string.Empty;
    public string DeleteEnabled { get; set; } = string.Empty;
    public string IsEnabled { get; set; } = string.Empty;
}

public sealed class UserSecurityProfileItem
{
    public string UserType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string IdNumber { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    // Staff info
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string BusinessRole { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;

    // Patient info
    public string SoNha { get; set; } = string.Empty;
    public string TenDuong { get; set; } = string.Empty;
    public string QuanHuyen { get; set; } = string.Empty;
    public string TinhTp { get; set; } = string.Empty;
    public string TienSuBenh { get; set; } = string.Empty;
    public string TienSuBenhGiaDinh { get; set; } = string.Empty;
    public string DiUngThuoc { get; set; } = string.Empty;

    public List<string> CurrentOracleRoles { get; set; } = new();
}
