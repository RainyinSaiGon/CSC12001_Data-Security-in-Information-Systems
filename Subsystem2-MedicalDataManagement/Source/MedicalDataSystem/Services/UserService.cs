namespace MedicalDataSystem.Services;

using Oracle.ManagedDataAccess.Client;

public sealed class UserService
{
    private readonly OracleConnectionService _connectionService;

    public UserService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public List<UserAccountItem> GetPatientUsersByCccd(string cccdKeyword)
    {
        return SearchUsers(
            cccdKeyword,
            """
            SELECT
                NVL(b.MABN, 0) AS USER_ID,
                NVL(b.TENBN, '(Khong tim thay benh nhan)') AS FULL_NAME,
                u.USERNAME,
                NVL(d.ACCOUNT_STATUS, 'N/A') AS ACCOUNT_STATUS,
                d.CREATED,
                d.EXPIRY_DATE
            FROM (
                SELECT DISTINCT rp.GRANTEE AS USERNAME
                FROM DBA_ROLE_PRIVS rp
                WHERE rp.GRANTED_ROLE = 'BENH_NHAN'
            ) u
            LEFT JOIN BENHNHAN b ON UPPER(b.USERNAME) = UPPER(u.USERNAME)
            LEFT JOIN DBA_USERS d ON UPPER(d.USERNAME) = UPPER(u.USERNAME)
            WHERE REPLACE(NVL(TRIM(b.CCCD), ''), ' ', '') LIKE :searchTerm
            ORDER BY u.USERNAME
            """);
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
            WHERE REPLACE(NVL(TRIM(n.CMND), ''), ' ', '') LIKE :searchTerm
            ORDER BY u.USERNAME
            """);
    }

    private List<UserAccountItem> SearchUsers(string searchText, string sql)
    {
        string normalizedKeyword = new string(searchText.Where(char.IsLetterOrDigit).ToArray());
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
