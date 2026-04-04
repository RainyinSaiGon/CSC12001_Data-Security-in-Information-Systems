namespace MedicalDataSystem.Services;

using Oracle.ManagedDataAccess.Client;

public sealed class UserService
{
    private readonly OracleConnectionService _connectionService;

    public UserService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
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
