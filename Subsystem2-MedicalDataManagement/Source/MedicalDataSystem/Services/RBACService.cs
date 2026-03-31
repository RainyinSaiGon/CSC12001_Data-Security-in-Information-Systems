namespace MedicalDataSystem.Services;

using Oracle.ManagedDataAccess.Client;

public class RBACService
{
    private static readonly Dictionary<string, List<string>> RoleActions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["COORDINATOR"] = new() { "ViewPatients", "AddPatient", "EditPatient", "CreateMedicalRecord", "AssignDoctor", "AssignTechnician" },
        ["DOCTOR"] = new() { "ViewAssignedPatients", "UpdateMedicalRecord", "OrderDiagnosticService", "DeleteDiagnosticService", "ManagePrescription", "EditPatientHistory" },
        ["TECHNICIAN"] = new() { "ViewAssignedServices", "UpdateServiceResult" },
        ["PATIENT"] = new() { "ViewSelf", "EditSelf", "ViewNotifications" }
    };

    private readonly OracleConnectionService _connectionService;

    public RBACService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public string? CheckUserRole(string username)
    {
        return _connectionService.Execute(connection =>
        {
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
                    SELECT 'PATIENT'
                    FROM BENHNHAN
                    WHERE UPPER(USERNAME) = UPPER(:username)
                )
                FETCH FIRST 1 ROWS ONLY
                """;
            command.Parameters.Add(new OracleParameter("username", username));
            return command.ExecuteScalar()?.ToString();
        });
    }

    public bool CheckPermission(string username, string action)
    {
        string? role = CheckUserRole(username);
        if (role is null || !RoleActions.TryGetValue(role, out var actions))
        {
            return false;
        }

        return actions.Contains(action, StringComparer.OrdinalIgnoreCase);
    }

    public List<string> GetAvailableActions(string username)
    {
        string? role = CheckUserRole(username);
        if (role is null || !RoleActions.TryGetValue(role, out var actions))
        {
            return new List<string>();
        }

        return actions.ToList();
    }
}
