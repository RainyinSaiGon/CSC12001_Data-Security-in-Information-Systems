namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public class RBACService
{
    private static readonly Dictionary<string, List<string>> RoleActions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["COORDINATOR"] = new() { "ViewPatients", "AddPatient", "EditPatient", "CreateMedicalRecord", "AssignDoctor", "AssignTechnician" },
        ["DOCTOR"] = new() { "ViewAssignedPatients", "UpdateMedicalRecord", "OrderDiagnosticService", "DeleteDiagnosticService", "ManagePrescription", "EditPatientHistory" },
        ["TECHNICIAN"] = new() { "ViewAssignedServices", "UpdateServiceResult" },
        ["PATIENT"] = new() { "ViewSelf", "EditSelf", "ViewNotifications" }
    };

    private readonly OracleConnectionService _connectionService;
    public string LastErrorMessage { get; private set; } = string.Empty;
    public string LastRoleOperation { get; private set; } = string.Empty;

    public RBACService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public string NormalizeIdText(string input)
    {
        return new string((input ?? string.Empty).Where(char.IsDigit).ToArray());
    }

    public List<PatientAccountItem> GetPatientUsersByCccd(string cccdKeyword)
    {
        string normalizedKeyword = NormalizeIdText(cccdKeyword);
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
                    MABN = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
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

    public List<PatientUserDisplayItem> GetPatientUserDisplayByCccd(string cccdKeyword)
    {
        return GetPatientUsersByCccd(cccdKeyword)
            .Select(u => new PatientUserDisplayItem
            {
                MaBN = u.MABN,
                HoTen = u.TENBN,
                GioiTinh = u.PHAI,
                NgaySinh = u.NGAYSINH?.ToString("dd/MM/yyyy") ?? "—",
                CCCD = u.CCCD,
                SoNha = u.SONHA,
                TenDuong = u.TENDUONG,
                QuanHuyen = u.QUANHUYEN,
                TinhTP = u.TINHTP,
                TienSuBenh = u.TIENSUBENH,
                TienSuBenhGiaDinh = u.TIENSUBENHGD,
                DiUngThuoc = u.DIUNGTHUOC,
                Username = u.Username,
                TrangThaiTaiKhoan = u.AccountStatus,
                TaoLuc = u.CreatedDate?.ToString("dd/MM/yyyy") ?? "—",
                HetHan = u.ExpiryDate?.ToString("dd/MM/yyyy") ?? "—"
            })
            .ToList();
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
            WHERE n.CCCD LIKE :searchTerm
            ORDER BY u.USERNAME
            """);
    }

    public List<StaffUserDisplayItem> GetStaffUserDisplayByCmnd(string cmndKeyword)
    {
        return GetStaffUsersByCmnd(cmndKeyword)
            .Select(u => new StaffUserDisplayItem
            {
                Id = u.UserId,
                FullName = u.FullName,
                Username = u.Username,
                AccountStatus = u.AccountStatus,
                Created = u.CreatedDate?.ToString("dd/MM/yyyy") ?? "—",
                Expiry = u.ExpiryDate?.ToString("dd/MM/yyyy") ?? "—"
            })
            .ToList();
    }

    public UserSecurityProfileItem? GetStaffProfileWithRolesByCmnd(string cmndKeyword)
    {
        string normalizedKeyword = NormalizeIdText(cmndKeyword);
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
                n.CCCD,
                n.USERNAME,
                n.QUEQUAN,
                n.SODT,
                n.VAITRO,
                n.CHUYENKHOA
            FROM NHANVIEN n
            WHERE n.CCCD = :idNumber
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
                UserId = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
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
        string normalizedKeyword = NormalizeIdText(cccdKeyword);
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
                UserId = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
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
        LastRoleOperation = string.Empty;

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

            if (HasRoleGrant(connection, transaction, safeUsername, normalizedRole))
            {
                LastErrorMessage = $"Role {normalizedRole} is already granted to {safeUsername}.";
                transaction.Rollback();
                return false;
            }

            ExecuteNonQuery(connection, transaction, $"GRANT {normalizedRole} TO {Q(safeUsername)}");
            LastRoleOperation = "GRANTED";
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

    public bool RevokeRoleFromUser(string username, string roleName)
    {
        LastErrorMessage = string.Empty;
        LastRoleOperation = string.Empty;

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
            LastErrorMessage = "Unsupported role for revoke.";
            return false;
        }

        using var connection = _connectionService.GetConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            if (!HasRoleGrant(connection, transaction, safeUsername, normalizedRole))
            {
                LastErrorMessage = $"Role {normalizedRole} is not granted to {safeUsername}.";
                transaction.Rollback();
                return false;
            }

            string mandatoryRole = GetMandatoryRoleForUser(connection, transaction, safeUsername);
            if (!string.IsNullOrWhiteSpace(mandatoryRole) && string.Equals(mandatoryRole, normalizedRole, StringComparison.OrdinalIgnoreCase))
            {
                LastErrorMessage = $"Cannot revoke base role {normalizedRole} of user {safeUsername}.";
                transaction.Rollback();
                return false;
            }

            ExecuteNonQuery(connection, transaction, $"REVOKE {normalizedRole} FROM {Q(safeUsername)}");
            LastRoleOperation = "REVOKED";
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

    public string? CheckUserRole(string username)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT APP_ROLE FROM (
                    SELECT CASE
                        WHEN VAITRO = N'Điều phối viên' THEN 'COORDINATOR'
                        WHEN VAITRO = N'Bác sĩ/Y sĩ' THEN 'DOCTOR'
                        WHEN VAITRO = N'Kỹ thuật viên' THEN 'TECHNICIAN'
                        WHEN VAITRO = N'Bệnh nhân' THEN 'PATIENT'
                        ELSE 'STAFF'
                    END AS APP_ROLE
                    FROM NHANVIEN
                    WHERE UPPER(USERNAME) = UPPER(:username)
                    UNION ALL
                    SELECT 'PATIENT'
                    FROM BENHNHAN
                    WHERE UPPER(USERNAME) = UPPER(:username)
                )
                FETCH FIRST 1 ROWS ONLY
                """;
            command.Parameters.Add(new OracleParameter("username", username));
            return command.ExecuteScalar()?.ToString();
        });
    }

    public bool CheckPermission(string username, string action)
    {
        string? role = CheckUserRole(username);
        if (role is null || !RoleActions.TryGetValue(role, out var actions))
        {
            return false;
        }

        return actions.Contains(action, StringComparer.OrdinalIgnoreCase);
    }

    public List<string> GetAvailableActions(string username)
    {
        string? role = CheckUserRole(username);
        if (role is null || !RoleActions.TryGetValue(role, out var actions))
        {
            return new List<string>();
        }

        return actions.ToList();
    }

    private List<UserAccountItem> SearchUsers(string searchText, string sql)
    {
        string normalizedKeyword = NormalizeIdText(searchText);
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

    private static string GetMandatoryRoleForUser(OracleConnection connection, OracleTransaction transaction, string username)
    {
        const string staffSql = """
            SELECT n.VAITRO
            FROM NHANVIEN n
            WHERE UPPER(n.USERNAME) = :username
            FETCH FIRST 1 ROWS ONLY
            """;

        using (var staffCommand = connection.CreateCommand())
        {
            staffCommand.Transaction = transaction;
            staffCommand.CommandText = staffSql;
            staffCommand.Parameters.Add(new OracleParameter("username", username));
            object? staffRole = staffCommand.ExecuteScalar();
            if (staffRole is not null && staffRole != DBNull.Value)
            {
                return MapStaffBusinessRoleToOracleRole(staffRole.ToString() ?? string.Empty);
            }
        }

        const string patientSql = """
            SELECT COUNT(*)
            FROM BENHNHAN b
            WHERE UPPER(b.USERNAME) = :username
            """;

        using var patientCommand = connection.CreateCommand();
        patientCommand.Transaction = transaction;
        patientCommand.CommandText = patientSql;
        patientCommand.Parameters.Add(new OracleParameter("username", username));
        int patientCount = Convert.ToInt32(patientCommand.ExecuteScalar());
        return patientCount > 0 ? "BENH_NHAN" : string.Empty;
    }

    private static string MapStaffBusinessRoleToOracleRole(string businessRole)
    {
        string normalized = (businessRole ?? string.Empty).Trim().ToUpperInvariant();

        if (normalized.Contains("ĐIỀU PHỐI") || normalized.Contains("DIEU PHOI") || normalized.Replace(" ", string.Empty).Contains("DIEUPHOI"))
        {
            return "DIEU_PHOI_VIEN";
        }

        if (normalized.Contains("BÁC SĨ") || normalized.Contains("Y SĨ") || normalized.Contains("BAC SI") || normalized.Contains("Y SI") || normalized.Contains("BACSI") || normalized.Contains("YSI"))
        {
            return "BAC_SI_Y_SI";
        }

        if (normalized.Contains("KỸ THUẬT") || normalized.Contains("KY THUAT") || normalized.Replace(" ", string.Empty).Contains("KYTHUAT"))
        {
            return "KY_THUAT_VIEN";
        }

        if (normalized.Contains("BỆNH NHÂN") || normalized.Contains("BENH NHAN") || normalized.Replace(" ", string.Empty).Contains("BENHNHAN"))
        {
            return "BENH_NHAN";
        }

        return string.Empty;
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

    private static void ExecuteNonQuery(OracleConnection connection, OracleTransaction transaction, string sql)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static void TryRollback(OracleTransaction transaction)
    {
        try
        {
            transaction.Rollback();
        }
        catch
        {
        }
    }
}
