namespace OracleDBAdmin.Services;

using OracleDBAdmin.Models;

public class PrivilegeService
{
    private readonly OracleConnectionService _connectionService;
    private readonly ValidationService _validationService;

    public PrivilegeService(OracleConnectionService connectionService, ValidationService validationService)
    {
        _connectionService = connectionService;
        _validationService = validationService;
    }

    public List<Permission> GetPrivileges(string grantee)
    {
        if (!_validationService.ValidateIdentifier(grantee))
        {
            throw new InvalidOperationException("Invalid grantee.");
        }

        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT GRANTEE, PRIVILEGE, OWNER, TABLE_NAME, COLUMN_NAME, GRANTABLE
                FROM DBA_COL_PRIVS
                WHERE GRANTEE = :grantee
                UNION ALL
                SELECT GRANTEE, PRIVILEGE, OWNER, TABLE_NAME, CAST(NULL AS VARCHAR2(30)), GRANTABLE
                FROM DBA_TAB_PRIVS
                WHERE GRANTEE = :grantee
                UNION ALL
                SELECT GRANTEE, GRANTED_ROLE, CAST(NULL AS VARCHAR2(30)), CAST(NULL AS VARCHAR2(30)), CAST(NULL AS VARCHAR2(30)), ADMIN_OPTION
                FROM DBA_ROLE_PRIVS
                WHERE GRANTEE = :grantee
                ORDER BY 1, 2, 3, 4, 5
                """;
            command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("grantee", grantee.Trim().ToUpperInvariant()));

            using var reader = command.ExecuteReader();
            var items = new List<Permission>();
            while (reader.Read())
            {
                items.Add(new Permission
                {
                    Grantee = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    PrivilegeType = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    ObjectOwner = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    ObjectName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    ColumnName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    Grantable = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                });
            }

            return items;
        });
    }
}
