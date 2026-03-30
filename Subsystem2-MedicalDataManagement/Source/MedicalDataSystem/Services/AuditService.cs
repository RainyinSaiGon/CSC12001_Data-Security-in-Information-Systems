namespace MedicalDataSystem.Services;

using Oracle.ManagedDataAccess.Client;

public class AuditService
{
    private readonly OracleConnectionService _connectionService;

    public AuditService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public bool LogUserAction(string userId, string action, string details)
    {
        _ = userId;
        _ = action;
        _ = details;
        return true;
    }

    public List<(string User, DateTime Time, string Action, string Details)> GetAuditLogs(
        DateTime startDate,
        DateTime endDate,
        string? specificUser = null)
    {
        return _connectionService.Execute(connection =>
        {
            var sqlCandidates = new[]
            {
                new
                {
                    Sql = """
                        SELECT DBUSERNAME, EVENT_TIMESTAMP, ACTION_NAME,
                               NVL(SQL_TEXT, OBJECT_SCHEMA || '.' || OBJECT_NAME)
                        FROM UNIFIED_AUDIT_TRAIL
                        WHERE EVENT_TIMESTAMP BETWEEN :startDate AND :endDate
                        """,
                    UserColumn = "DBUSERNAME"
                },
                new
                {
                    Sql = """
                        SELECT USERNAME, NTIMESTAMP#, ACTION_NAME,
                               NVL(OBJ_NAME, PRIV_USED)
                        FROM DBA_AUDIT_TRAIL
                        WHERE NTIMESTAMP# BETWEEN :startDate AND :endDate
                        """,
                    UserColumn = "USERNAME"
                }
            };

            foreach (var candidate in sqlCandidates)
            {
                try
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = candidate.Sql +
                        (string.IsNullOrWhiteSpace(specificUser) ? string.Empty : $" AND UPPER({candidate.UserColumn}) = UPPER(:specificUser)");
                    command.Parameters.Add(new OracleParameter("startDate", startDate));
                    command.Parameters.Add(new OracleParameter("endDate", endDate));

                    if (!string.IsNullOrWhiteSpace(specificUser))
                    {
                        command.Parameters.Add(new OracleParameter("specificUser", specificUser));
                    }

                    using var reader = command.ExecuteReader();
                    var items = new List<(string, DateTime, string, string)>();
                    while (reader.Read())
                    {
                        items.Add((
                            reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                            reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1),
                            reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            reader.IsDBNull(3) ? string.Empty : reader.GetString(3)));
                    }

                    return items;
                }
                catch (OracleException)
                {
                    // Try the next audit source.
                }
            }

            return new List<(string, DateTime, string, string)>();
        });
    }

    public bool LogSensitiveAccess(string userId, string dataType, string recordId)
    {
        _ = userId;
        _ = dataType;
        _ = recordId;
        return true;
    }
}
