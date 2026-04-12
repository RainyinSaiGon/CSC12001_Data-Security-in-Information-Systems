namespace MedicalDataSystem.Services;

using Oracle.ManagedDataAccess.Client;

public class OracleConnectionService
{
    private const string AppSchema = "HOSPITAL_ADMIN";
    private readonly string _connectionString;

    public OracleConnectionService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string ConnectionString => _connectionString;

    public static string BuildConnectionString(string dataSource, string userId, string password)
    {
        string escapedUserId = userId.Replace("\"", "\"\"");
        string escapedPassword = password.Replace("\"", "\"\"");
        return $"Data Source={dataSource};User Id=\"{escapedUserId}\";Password=\"{escapedPassword}\";Pooling=true;";
    }

    public bool TestConnection()
    {
        using var connection = new OracleConnection(_connectionString);
        connection.Open();
        return connection.State == System.Data.ConnectionState.Open;
    }

    public OracleConnection GetConnection()
    {
        var connection = new OracleConnection(_connectionString);
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"ALTER SESSION SET CURRENT_SCHEMA = {AppSchema}";
            command.ExecuteNonQuery();
        }

        return connection;
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
