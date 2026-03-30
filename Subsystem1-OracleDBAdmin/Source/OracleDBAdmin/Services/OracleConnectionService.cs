namespace OracleDBAdmin.Services;

using Oracle.ManagedDataAccess.Client;

public class OracleConnectionService
{
    private readonly string _connectionString;

    public OracleConnectionService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static string BuildConnectionString(string dataSource, string userId, string password)
    {
        return $"Data Source={dataSource};User Id={userId};Password={password};Pooling=true;";
    }

    public OracleConnection GetConnection()
    {
        var connection = new OracleConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public bool TestConnection()
    {
        using var connection = GetConnection();
        return connection.State == System.Data.ConnectionState.Open;
    }

    public T Execute<T>(Func<OracleConnection, T> action)
    {
        using var connection = GetConnection();
        return action(connection);
    }

    public void Execute(Action<OracleConnection> action)
    {
        using var connection = GetConnection();
        action(connection);
    }
}
