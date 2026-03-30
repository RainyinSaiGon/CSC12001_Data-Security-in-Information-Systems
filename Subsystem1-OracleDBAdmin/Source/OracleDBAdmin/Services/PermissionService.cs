namespace OracleDBAdmin.Services;

public class PermissionService
{
    private readonly OracleConnectionService _connectionService;
    private readonly ValidationService _validationService;

    public PermissionService(OracleConnectionService connectionService, ValidationService validationService)
    {
        _connectionService = connectionService;
        _validationService = validationService;
    }

    public void GrantRoleToUser(string roleName, string username)
    {
        if (!_validationService.ValidateIdentifier(roleName) || !_validationService.ValidateIdentifier(username))
        {
            throw new InvalidOperationException("Invalid role or user.");
        }

        _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"GRANT {_validationService.QuoteIdentifier(roleName)} TO {_validationService.QuoteIdentifier(username)}";
            command.ExecuteNonQuery();
        });
    }

    public void GrantObjectPrivilege(string grantee, string owner, string objectName, string privilege, string? columns, bool withGrantOption)
    {
        ExecuteObjectPrivilege(grantee, owner, objectName, privilege, columns, withGrantOption, grant: true);
    }

    public void RevokeObjectPrivilege(string grantee, string owner, string objectName, string privilege, string? columns)
    {
        ExecuteObjectPrivilege(grantee, owner, objectName, privilege, columns, withGrantOption: false, grant: false);
    }

    private void ExecuteObjectPrivilege(string grantee, string owner, string objectName, string privilege, string? columns, bool withGrantOption, bool grant)
    {
        if (!_validationService.ValidateIdentifier(grantee)
            || !_validationService.ValidateIdentifier(owner)
            || !_validationService.ValidateIdentifier(objectName)
            || !_validationService.ValidateIdentifier(privilege))
        {
            throw new InvalidOperationException("Invalid privilege statement.");
        }

        string privilegeSql = privilege.Trim().ToUpperInvariant();
        string objectSql = $"{_validationService.QuoteIdentifier(owner)}.{_validationService.QuoteIdentifier(objectName)}";
        string columnSql = string.Empty;

        if (!string.IsNullOrWhiteSpace(columns))
        {
            string[] safeColumns = columns
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(_validationService.QuoteIdentifier)
                .ToArray();

            columnSql = $" ({string.Join(", ", safeColumns)})";
        }

        string sql = grant
            ? $"GRANT {privilegeSql}{columnSql} ON {objectSql} TO {_validationService.QuoteIdentifier(grantee)}{(withGrantOption ? " WITH GRANT OPTION" : string.Empty)}"
            : $"REVOKE {privilegeSql}{columnSql} ON {objectSql} FROM {_validationService.QuoteIdentifier(grantee)}";

        _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        });
    }
}
