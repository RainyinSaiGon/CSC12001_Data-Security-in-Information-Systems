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

        UserSession? staffSession = TryReadStaffSession(connection, username, password, verifyPasswordHash: true, connectionString, dataSource);
        if (staffSession is not null)
        {
            return staffSession;
        }

        UserSession? patientSession = TryReadPatientSession(connection, username, password, verifyPasswordHash: true, connectionString, dataSource);
        if (patientSession is not null)
        {
            return patientSession;
        }

        throw new InvalidOperationException("Authenticated Oracle user is not mapped to NHANVIEN or BENHNHAN.");
    }

    public string? Login(string username, string password)
    {
        if (_connectionService is null)
        {
            throw new InvalidOperationException("A connection service is required for this overload.");
        }

        using var connection = _connectionService.GetConnection();

        UserSession? staffSession = TryReadStaffSession(connection, username, password, verifyPasswordHash: false, _connectionService.ConnectionString, string.Empty);
        if (staffSession is not null)
        {
            return staffSession.Role;
        }

        UserSession? patientSession = TryReadPatientSession(connection, username, password, verifyPasswordHash: false, _connectionService.ConnectionString, string.Empty);
        return patientSession?.Role;
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

    private static UserSession? TryReadStaffSession(OracleConnection connection, string username, string password, bool verifyPasswordHash, string connectionString, string dataSource)
    {
        try
        {
            using var staffCommand = connection.CreateCommand();
            staffCommand.CommandText = """
                SELECT
                    MANV,
                    HOTEN,
                    CHUYENKHOA,
                    PASSWORD_HASH,
                    CASE
                        WHEN VAITRO = N'Điều phối viên' THEN 'COORDINATOR'
                        WHEN VAITRO = N'Bác sĩ/Y sĩ' THEN 'DOCTOR'
                        WHEN VAITRO = N'Kỹ thuật viên' THEN 'TECHNICIAN'
                        WHEN VAITRO = N'Bệnh nhân' THEN 'PATIENT'
                        ELSE 'STAFF'
                    END AS APP_ROLE
                FROM V_SELF_NHANVIEN
                """;

            using var reader = staffCommand.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            string? storedHash = reader.IsDBNull(3) ? null : reader.GetString(3);
            if (verifyPasswordHash && !EnsurePasswordHash(connection, updateSelfStaff: true, storedHash, password))
            {
                throw new InvalidOperationException("Invalid username or password.");
            }

            return new UserSession
            {
                Username = username.ToUpperInvariant(),
                FullName = reader.GetString(1),
                Role = reader.GetString(4),
                StaffId = reader.GetString(0),
                DepartmentCode = reader.IsDBNull(2) ? null : reader.GetString(2),
                ConnectionString = connectionString,
                DataSource = dataSource
            };
        }
        catch (OracleException ex) when (ex.Number == 942 || ex.Number == 904)
        {
            return null;
        }
    }

    private static UserSession? TryReadPatientSession(OracleConnection connection, string username, string password, bool verifyPasswordHash, string connectionString, string dataSource)
    {
        try
        {
            using var patientCommand = connection.CreateCommand();
            patientCommand.CommandText = """
                SELECT MABN, TENBN, PASSWORD_HASH
                FROM V_SELF_BENHNHAN
                """;

            using var patientReader = patientCommand.ExecuteReader();
            if (!patientReader.Read())
            {
                return null;
            }

            string? storedHash = patientReader.IsDBNull(2) ? null : patientReader.GetString(2);
            if (verifyPasswordHash && !EnsurePasswordHash(connection, updateSelfStaff: false, storedHash, password))
            {
                throw new InvalidOperationException("Invalid username or password.");
            }

            return new UserSession
            {
                Username = username.ToUpperInvariant(),
                FullName = patientReader.GetString(1),
                Role = "PATIENT",
                PatientId = patientReader.GetString(0),
                ConnectionString = connectionString,
                DataSource = dataSource
            };
        }
        catch (OracleException ex) when (ex.Number == 942 || ex.Number == 904)
        {
            return null;
        }
    }

    private static bool EnsurePasswordHash(OracleConnection connection, bool updateSelfStaff, string? storedHash, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(storedHash))
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        string newHash = BCrypt.Net.BCrypt.HashPassword(password);

        using var updateCommand = connection.CreateCommand();
        updateCommand.CommandText = updateSelfStaff
            ? "UPDATE V_SELF_NHANVIEN SET PASSWORD_HASH = :password_hash"
            : "UPDATE V_SELF_BENHNHAN SET PASSWORD_HASH = :password_hash";
        updateCommand.Parameters.Add(new OracleParameter("password_hash", newHash));
        updateCommand.ExecuteNonQuery();

        return true;
    }
}
