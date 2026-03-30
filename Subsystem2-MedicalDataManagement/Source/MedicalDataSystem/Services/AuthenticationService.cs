namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public class AuthenticationService
{
    private readonly OracleConnectionService? _connectionService;

    public AuthenticationService()
    {
    }

    public AuthenticationService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public UserSession Authenticate(string username, string password, string dataSource)
    {
        string connectionString = OracleConnectionService.BuildConnectionString(dataSource, username, password);
        var connectionService = new OracleConnectionService(connectionString);

        using var connection = connectionService.GetConnection();

        using var staffCommand = connection.CreateCommand();
        staffCommand.CommandText = """
            SELECT
                MANV,
                HOTEN,
                CHUYENKHOA,
                CASE
                    WHEN VAITRO = N'Điều phối viên' THEN 'COORDINATOR'
                    WHEN VAITRO = N'Bác sĩ/Y sĩ' THEN 'DOCTOR'
                    WHEN VAITRO = N'Kỹ thuật viên' THEN 'TECHNICIAN'
                    WHEN VAITRO = N'Bệnh nhân' THEN 'PATIENT'
                    ELSE 'STAFF'
                END AS APP_ROLE
            FROM NHANVIEN
            WHERE UPPER(USERNAME) = UPPER(:username)
            """;
        staffCommand.Parameters.Add(new OracleParameter("username", username));

        using (var reader = staffCommand.ExecuteReader())
        {
            if (reader.Read())
            {
                return new UserSession
                {
                    Username = username.ToUpperInvariant(),
                    FullName = reader.GetString(1),
                    Role = reader.GetString(3),
                    StaffId = reader.GetInt32(0),
                    DepartmentCode = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ConnectionString = connectionString,
                    DataSource = dataSource
                };
            }
        }

        using var patientCommand = connection.CreateCommand();
        patientCommand.CommandText = """
            SELECT MABN, TENBN
            FROM BENHNHAN
            WHERE UPPER(USERNAME) = UPPER(:username)
            """;
        patientCommand.Parameters.Add(new OracleParameter("username", username));

        using var patientReader = patientCommand.ExecuteReader();
        if (patientReader.Read())
        {
            return new UserSession
            {
                Username = username.ToUpperInvariant(),
                FullName = patientReader.GetString(1),
                Role = "PATIENT",
                PatientId = patientReader.GetInt32(0),
                ConnectionString = connectionString,
                DataSource = dataSource
            };
        }

        throw new InvalidOperationException("Authenticated Oracle user is not mapped to NHANVIEN or BENHNHAN.");
    }

    public string? Login(string username, string password)
    {
        if (_connectionService is null)
        {
            throw new InvalidOperationException("A connection service is required for this overload.");
        }

        _ = password;

        using var connection = _connectionService.GetConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT APP_ROLE FROM (
                SELECT CASE
                    WHEN VAITRO = N'Điều phối viên' THEN 'COORDINATOR'
                    WHEN VAITRO = N'Bác sĩ/Y sĩ' THEN 'DOCTOR'
                    WHEN VAITRO = N'Kỹ thuật viên' THEN 'TECHNICIAN'
                    WHEN VAITRO = N'Bệnh nhân' THEN 'PATIENT'
                    ELSE 'STAFF'
                END AS APP_ROLE
                FROM NHANVIEN
                WHERE UPPER(USERNAME) = UPPER(:username)
                UNION ALL
                SELECT 'PATIENT' AS APP_ROLE
                FROM BENHNHAN
                WHERE UPPER(USERNAME) = UPPER(:username)
            )
            FETCH FIRST 1 ROWS ONLY
            """;
        command.Parameters.Add(new OracleParameter("username", username));
        return command.ExecuteScalar()?.ToString();
    }

    public bool ValidateUserRole(string username, string expectedRole)
    {
        string? currentRole = Login(username, string.Empty);
        return string.Equals(currentRole, expectedRole, StringComparison.OrdinalIgnoreCase);
    }

    public void Logout(string username)
    {
        _ = username;
    }
}
