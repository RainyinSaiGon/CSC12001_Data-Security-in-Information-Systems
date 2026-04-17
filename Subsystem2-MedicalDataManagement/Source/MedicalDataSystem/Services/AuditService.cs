namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public class AuditService
{
    private readonly OracleConnectionService _connectionService;
    private const string AuditTimeFormat = "yyyy-MM-dd HH:mm:ss";

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

    public List<AuditLog> GetStandardAuditLogs()
    {
        var sqlCandidates = new[]
        {
            """
            SELECT
                USERNAME,
                ACTION_NAME,
                OBJ_NAME,
                RETURNCODE,
                TO_CHAR(EXTENDED_TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS ACTION_TIME
            FROM DBA_AUDIT_TRAIL
            WHERE OBJ_NAME IN ('BENHNHAN', 'HSBA', 'HSBA_DV', 'DONTHUOC')
            ORDER BY EXTENDED_TIMESTAMP DESC
            """,
            """
            SELECT
                USERNAME,
                ACTION_NAME,
                OBJ_NAME,
                RETURNCODE,
                TO_CHAR(TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS ACTION_TIME
            FROM DBA_AUDIT_TRAIL
            WHERE OBJ_NAME IN ('BENHNHAN', 'HSBA', 'HSBA_DV', 'DONTHUOC')
            ORDER BY TIMESTAMP DESC
            """
        };

        return _connectionService.Execute(connection =>
        {
            foreach (string sql in sqlCandidates)
            {
                try
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = sql;

                    using var reader = command.ExecuteReader();
                    var logs = new List<AuditLog>();

                    while (reader.Read())
                    {
                        string actionTimeRaw = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                        logs.Add(new AuditLog
                        {
                            Username = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                            ActionName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            ObjectName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            ReturnCode = reader.IsDBNull(3) ? -1 : Convert.ToInt32(reader.GetDecimal(3)),
                            ActionTime = ParseAuditTime(actionTimeRaw)
                        });
                    }

                    return logs;
                }
                catch (OracleException ex) when (ex.Number == 904)
                {
                    // Try the fallback timestamp column in this Oracle edition.
                }
            }

            return new List<AuditLog>();
        });
    }

    public List<FgaAuditLog> GetFgaAuditLogs()
    {
        const string sql = """
            SELECT
                DB_USER,
                OBJECT_NAME,
                POLICY_NAME,
                STATEMENT_TYPE,
                TIMESTAMP,
                SQL_TEXT
            FROM DBA_FGA_AUDIT_TRAIL
            WHERE OBJECT_NAME IN ('HSBA', 'HSBA_DV', 'DONTHUOC')
            ORDER BY TIMESTAMP DESC
            """;

        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var reader = command.ExecuteReader();
            var logs = new List<FgaAuditLog>();

            while (reader.Read())
            {
                logs.Add(new FgaAuditLog
                {
                    DbUser = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    ObjectName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    PolicyName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    StatementType = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    ActionTime = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                    SqlText = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                });
            }

            return logs;
        });
    }

    public List<SessionAuditLog> GetSessionAuditLogs()
    {
        const string sql = """
            SELECT
                USERNAME,
                USERHOST,
                TERMINAL,
                RETURNCODE,
                TO_CHAR(TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS LOGON_TIME,
                TO_CHAR(LOGOFF_TIME, 'YYYY-MM-DD HH24:MI:SS') AS LOGOFF_TIME,
                SESSIONID
            FROM DBA_AUDIT_SESSION
            WHERE RETURNCODE = 0
            ORDER BY TIMESTAMP DESC
            """;

        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var reader = command.ExecuteReader();
            var logs = new List<SessionAuditLog>();

            while (reader.Read())
            {
                string logonTimeRaw = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                string logoffTimeRaw = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);

                DateTime parsedLogonTime = ParseAuditTime(logonTimeRaw);
                DateTime parsedLogoffTime = ParseAuditTime(logoffTimeRaw);

                logs.Add(new SessionAuditLog
                {
                    Username = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    UserHost = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Terminal = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    ReturnCode = reader.IsDBNull(3) ? -1 : Convert.ToInt32(reader.GetDecimal(3)),
                    LogonTime = parsedLogonTime,
                    LogoffTime = parsedLogoffTime == DateTime.MinValue ? null : parsedLogoffTime,
                    SessionId = reader.IsDBNull(6) ? 0 : Convert.ToInt64(reader.GetDecimal(6))
                });
            }

            return logs;
        });
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
                        SELECT USERNAME, EXTENDED_TIMESTAMP, ACTION_NAME,
                               NVL(OBJ_NAME, PRIV_USED)
                        FROM DBA_AUDIT_TRAIL
                        WHERE EXTENDED_TIMESTAMP BETWEEN :startDate AND :endDate
                        """,
                    UserColumn = "USERNAME"
                },
                new
                {
                    Sql = """
                        SELECT USERNAME, TIMESTAMP, ACTION_NAME,
                               NVL(OBJ_NAME, PRIV_USED)
                        FROM DBA_AUDIT_TRAIL
                        WHERE TIMESTAMP BETWEEN :startDate AND :endDate
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
                catch (OracleException ex) when (ex.Number == 904)
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

    private static DateTime ParseAuditTime(string value)
    {
        return DateTime.TryParseExact(value, AuditTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsed)
            ? parsed
            : DateTime.MinValue;
    }
}
