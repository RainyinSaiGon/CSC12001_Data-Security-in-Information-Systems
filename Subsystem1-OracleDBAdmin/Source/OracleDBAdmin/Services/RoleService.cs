namespace OracleDBAdmin.Services;

using OracleDBAdmin.Models;

public class RoleService
{
    private readonly OracleConnectionService _connectionService;
    private readonly ValidationService _validationService;

    public RoleService(OracleConnectionService connectionService, ValidationService validationService)
    {
        _connectionService = connectionService;
        _validationService = validationService;
    }

    public List<Role> ListRoles()
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT ROLE, AUTHENTICATION_TYPE
                FROM DBA_ROLES
                ORDER BY ROLE
                """;
            using var reader = command.ExecuteReader();
            var items = new List<Role>();
            while (reader.Read())
            {
                items.Add(new Role
                {
                    Name = reader.GetString(0),
                    AuthenticationType = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
                });
            }

            return items;
        });
    }

    public void CreateRole(string roleName)
    {
        if (!_validationService.ValidateIdentifier(roleName))
        {
            throw new InvalidOperationException("Invalid role name.");
        }

        _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"CREATE ROLE {_validationService.QuoteIdentifier(roleName)}";
            command.ExecuteNonQuery();
        });
    }

    public void DropRole(string roleName)
    {
        if (!_validationService.ValidateIdentifier(roleName))
        {
            throw new InvalidOperationException("Invalid role name.");
        }

        _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"DROP ROLE {_validationService.QuoteIdentifier(roleName)}";
            command.ExecuteNonQuery();
        });
    }
}
