namespace OracleDBAdmin.Services;

using Oracle.ManagedDataAccess.Client;
using OracleDBAdmin.Models;

public class UserService
{
    private readonly OracleConnectionService _connectionService;
    private readonly ValidationService _validationService;

    public UserService(OracleConnectionService connectionService, ValidationService validationService)
    {
        _connectionService = connectionService;
        _validationService = validationService;
    }

    public List<User> ListUsers()
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT USERNAME, ACCOUNT_STATUS, CREATED, DEFAULT_TABLESPACE
                FROM DBA_USERS
                ORDER BY USERNAME
                """;
            using var reader = command.ExecuteReader();
            var items = new List<User>();
            while (reader.Read())
            {
                items.Add(new User
                {
                    Username = reader.GetString(0),
                    AccountStatus = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Created = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                    DefaultTablespace = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                });
            }

            return items;
        });
    }

    public void CreateUser(string username, string password)
    {
        if (!_validationService.ValidateIdentifier(username) || !_validationService.ValidatePassword(password))
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            string safeUsername = _validationService.QuoteIdentifier(username);
            command.CommandText = $"CREATE USER {safeUsername} IDENTIFIED BY \"{password}\"";
            command.ExecuteNonQuery();

            using var grantCommand = connection.CreateCommand();
            grantCommand.CommandText = $"GRANT CREATE SESSION TO {safeUsername}";
            grantCommand.ExecuteNonQuery();
        });
    }

    public void ResetPassword(string username, string password)
    {
        if (!_validationService.ValidateIdentifier(username) || !_validationService.ValidatePassword(password))
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"ALTER USER {_validationService.QuoteIdentifier(username)} IDENTIFIED BY \"{password}\"";
            command.ExecuteNonQuery();
        });
    }

    public void DropUser(string username)
    {
        if (!_validationService.ValidateIdentifier(username))
        {
            throw new InvalidOperationException("Invalid username.");
        }

        _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"DROP USER {_validationService.QuoteIdentifier(username)} CASCADE";
            command.ExecuteNonQuery();
        });
    }
}
