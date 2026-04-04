namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public sealed class UserService
{
    private readonly OracleConnectionService _connectionService;
    public string LastErrorMessage { get; private set; } = string.Empty;

    public UserService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
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
            ExecuteNonQuery(connection, transaction, $"GRANT CREATE SESSION TO {safeUsername}");

            if (string.Equals(request.UserType, "STAFF", StringComparison.OrdinalIgnoreCase))
            {
                string roleName = MapStaffRoleToDbRole(request.Role);
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    LastErrorMessage = "Unsupported staff role.";
                    transaction.Rollback();
                    return false;
                }

                ExecuteNonQuery(connection, transaction, $"GRANT {roleName} TO {safeUsername}");
            }
            else
            {
                ExecuteNonQuery(connection, transaction, $"GRANT BENH_NHAN TO {safeUsername}");
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
        string normalizedKeyword = NormalizeIdKeyword(cccdKeyword);
        if (normalizedKeyword.Length != 12)
        {
            return new List<PatientAccountItem>();
        }

        const string sql = """
            SELECT
                NVL(b.MABN, 0) AS MABN,
                NVL(b.TENBN, '(Khong tim thay benh nhan)') AS TENBN,
                NVL(b.PHAI, '') AS PHAI,
                b.NGAYSINH,
                NVL(b.CCCD, '') AS CCCD,
                NVL(b.SONHA, '') AS SONHA,
                NVL(b.TENDUONG, '') AS TENDUONG,
                NVL(b.QUANHUYEN, '') AS QUANHUYEN,
                NVL(b.TINHTP, '') AS TINHTP,
                NVL(b.TIENSUBENH, '') AS TIENSUBENH,
                NVL(b.TIENSUBENHGD, '') AS TIENSUBENHGD,
                NVL(b.DIUNGTHUOC, '') AS DIUNGTHUOC,
                u.USERNAME,
                NVL(d.ACCOUNT_STATUS, 'N/A') AS ACCOUNT_STATUS,
                d.CREATED,
                d.EXPIRY_DATE
            FROM (
                SELECT DISTINCT rp.GRANTEE AS USERNAME
                FROM DBA_ROLE_PRIVS rp
                WHERE rp.GRANTED_ROLE = 'BENH_NHAN'
            ) u
            LEFT JOIN BENHNHAN b ON b.USERNAME = u.USERNAME
            LEFT JOIN DBA_USERS d ON d.USERNAME = u.USERNAME
            WHERE b.CCCD = :cccdExact
            ORDER BY u.USERNAME
            FETCH FIRST 100 ROWS ONLY
            """;

        return _connectionService.Execute(connection =>
        {
            using var dataCommand = connection.CreateCommand();
            dataCommand.CommandText = sql;
            dataCommand.Parameters.Add(new OracleParameter("cccdExact", normalizedKeyword));

            using var reader = dataCommand.ExecuteReader();
            var users = new List<PatientAccountItem>();
            while (reader.Read())
            {
                users.Add(new PatientAccountItem
                {
                    MABN = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    TENBN = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    PHAI = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    NGAYSINH = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    CCCD = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    SONHA = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    TENDUONG = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    QUANHUYEN = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    TINHTP = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    TIENSUBENH = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                    TIENSUBENHGD = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                    DIUNGTHUOC = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                    Username = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                    AccountStatus = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                    CreatedDate = reader.IsDBNull(14) ? null : reader.GetDateTime(14),
                    ExpiryDate = reader.IsDBNull(15) ? null : reader.GetDateTime(15)
                });
            }

            return users;
        });
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
        return SearchUsers(
            cmndKeyword,
            """
            SELECT
                NVL(n.MANV, 0) AS USER_ID,
                NVL(n.HOTEN, '(Khong tim thay nhan vien)') AS FULL_NAME,
                u.USERNAME,
                NVL(d.ACCOUNT_STATUS, 'N/A') AS ACCOUNT_STATUS,
                d.CREATED,
                d.EXPIRY_DATE
            FROM (
                SELECT DISTINCT rp.GRANTEE AS USERNAME
                FROM DBA_ROLE_PRIVS rp
                WHERE rp.GRANTED_ROLE IN ('DIEU_PHOI_VIEN', 'BAC_SI_Y_SI', 'KY_THUAT_VIEN')
            ) u
            LEFT JOIN NHANVIEN n ON UPPER(n.USERNAME) = UPPER(u.USERNAME)
            LEFT JOIN DBA_USERS d ON UPPER(d.USERNAME) = UPPER(u.USERNAME)
            WHERE n.CMND LIKE :searchTerm
            ORDER BY u.USERNAME
            """);
    }

    public UserSecurityProfileItem? GetStaffProfileWithRolesByCmnd(string cmndKeyword)
    {
        string normalizedKeyword = NormalizeIdKeyword(cmndKeyword);
        if (normalizedKeyword.Length != 12)
        {
            return null;
        }

        const string sql = """
            SELECT
                n.MANV,
                n.HOTEN,
                n.PHAI,
                n.NGAYSINH,
                n.CMND,
                n.USERNAME,
                n.QUEQUAN,
                n.SODT,
                n.VAITRO,
                n.CHUYENKHOA
            FROM NHANVIEN n
            WHERE n.CMND = :idNumber
            FETCH FIRST 1 ROWS ONLY
            """;

        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.Add(new OracleParameter("idNumber", normalizedKeyword));

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            string username = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
            var roles = GetCurrentRolesByUsername(connection, username);

            return new UserSecurityProfileItem
            {
                UserType = "STAFF",
                UserId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                FullName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                Gender = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                BirthDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                IdNumber = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Username = username,
                Address = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                Phone = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                BusinessRole = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                Department = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                CurrentOracleRoles = roles
            };
        });
    }

    public UserSecurityProfileItem? GetPatientProfileWithRolesByCccd(string cccdKeyword)
    {
        string normalizedKeyword = NormalizeIdKeyword(cccdKeyword);
        if (normalizedKeyword.Length != 12)
        {
            return null;
        }

        const string sql = """
            SELECT
                b.MABN,
                b.TENBN,
                b.PHAI,
                b.NGAYSINH,
                b.CCCD,
                b.USERNAME,
                b.SONHA,
                b.TENDUONG,
                b.QUANHUYEN,
                b.TINHTP,
                b.TIENSUBENH,
                b.TIENSUBENHGD,
                b.DIUNGTHUOC
            FROM BENHNHAN b
            WHERE b.CCCD = :idNumber
            FETCH FIRST 1 ROWS ONLY
            """;

        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.Add(new OracleParameter("idNumber", normalizedKeyword));

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            string username = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
            var roles = GetCurrentRolesByUsername(connection, username);

            return new UserSecurityProfileItem
            {
                UserType = "PATIENT",
                UserId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                FullName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                Gender = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                BirthDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                IdNumber = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Username = username,
                SoNha = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                TenDuong = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                QuanHuyen = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                TinhTp = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                TienSuBenh = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                TienSuBenhGiaDinh = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                DiUngThuoc = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                CurrentOracleRoles = roles
            };
        });
    }

    public bool GrantRoleToStaff(string username, string roleName)
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

        string normalizedRole = NormalizeAndValidateGrantRole(roleName);
        if (string.IsNullOrWhiteSpace(normalizedRole))
        {
            LastErrorMessage = "Unsupported role for staff grant.";
            return false;
        }

        using var connection = _connectionService.GetConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            if (!IsStaffUsername(connection, transaction, safeUsername))
            {
                LastErrorMessage = "Selected account is not mapped to NHANVIEN.";
                transaction.Rollback();
                return false;
            }

            ExecuteNonQuery(connection, transaction, $"GRANT {normalizedRole} TO {safeUsername}");
            transaction.Commit();
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
                    UserId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
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

        if (!isValid || !(normalized[0] >= 'A' && normalized[0] <= 'Z'))
        {
            throw new ArgumentException("Username contains invalid Oracle identifier characters.");
        }

        return normalized;
    }

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
                HOTEN, PHAI, NGAYSINH, CMND,
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
            ExecuteNonQuery(connection, transaction, $"CREATE USER {username} IDENTIFIED BY \"123\"");
        }
        catch (OracleException ex) when (ex.Number == -1920)
        {
            ExecuteNonQuery(connection, transaction, $"ALTER USER {username} IDENTIFIED BY \"123\" ACCOUNT UNLOCK");
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
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public sealed class PatientAccountItem
{
    public int MABN { get; set; }
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

public sealed class UserSecurityProfileItem
{
    public string UserType { get; set; } = string.Empty;
    public int UserId { get; set; }
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
