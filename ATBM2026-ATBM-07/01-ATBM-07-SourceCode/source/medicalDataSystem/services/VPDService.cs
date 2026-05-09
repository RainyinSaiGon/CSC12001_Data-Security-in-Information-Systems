namespace MedicalDataSystem.Services;

using Oracle.ManagedDataAccess.Client;

public class VPDService
{
    private readonly OracleConnectionService _connectionService;
    public string LastErrorMessage { get; private set; } = string.Empty;

    public VPDService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public List<string> GetVisiblePatients(string doctorId)
    {
        var doctorService = new DoctorService(_connectionService, this);
        return doctorService.GetAssignedPatients(doctorId).Select(patient => $"{patient.MABN} - {patient.TENBN}").ToList();
    }

    public List<string> GetVisibleRecords(string staffId, string role)
    {
        if (string.Equals(role, "DOCTOR", StringComparison.OrdinalIgnoreCase))
        {
            var doctorService = new DoctorService(_connectionService, this);
            return doctorService.GetAssignedMedicalRecords(staffId)
                .Select(record => $"{record.MAHSBA} - {record.CHANDOAN}")
                .ToList();
        }

        return new List<string>();
    }

    public List<string> GetVisibleServices(string technicianId)
    {
        var technicianService = new TechnicianService(_connectionService, this);
        return technicianService.GetAssignedServices(technicianId)
            .Select(service => $"{service.MAHSBA} | {service.LOAIDV} | {service.NGAYDV:yyyy-MM-dd}")
            .ToList();
    }

    public List<VpdPolicyItem> GetVpdPolicies()
    {
        const string sql = """
            SELECT
                OBJECT_NAME,
                POLICY_NAME,
                PF_OWNER,
                FUNCTION,
                SEL,
                INS,
                UPD,
                DEL,
                ENABLE
            FROM USER_POLICIES
            ORDER BY OBJECT_NAME, POLICY_NAME
            """;

        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var reader = command.ExecuteReader();
            var result = new List<VpdPolicyItem>();
            while (reader.Read())
            {
                result.Add(new VpdPolicyItem
                {
                    ObjectName = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    PolicyName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    FunctionOwner = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    PolicyFunction = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    SelectEnabled = reader.IsDBNull(4) ? "NO" : reader.GetString(4),
                    InsertEnabled = reader.IsDBNull(5) ? "NO" : reader.GetString(5),
                    UpdateEnabled = reader.IsDBNull(6) ? "NO" : reader.GetString(6),
                    DeleteEnabled = reader.IsDBNull(7) ? "NO" : reader.GetString(7),
                    IsEnabled = reader.IsDBNull(8) ? "NO" : reader.GetString(8)
                });
            }

            return result;
        });
    }

    public bool SetVpdPolicyEnabled(string objectName, string policyName, bool enable)
    {
        LastErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(objectName) || string.IsNullOrWhiteSpace(policyName))
        {
            LastErrorMessage = "Policy name and object name are required.";
            return false;
        }

        try
        {
            _connectionService.Execute(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = $"""
                    BEGIN
                        DBMS_RLS.ENABLE_POLICY(
                            object_schema => 'HOSPITAL_ADMIN',
                            object_name   => :object_name,
                            policy_name   => :policy_name,
                            enable        => {(enable ? "TRUE" : "FALSE")}
                        );
                    END;
                    """;
                command.Parameters.Add("object_name", OracleDbType.Varchar2, objectName, System.Data.ParameterDirection.Input);
                command.Parameters.Add("policy_name", OracleDbType.Varchar2, policyName, System.Data.ParameterDirection.Input);
                command.ExecuteNonQuery();
            });

            return true;
        }
        catch (OracleException ex)
        {
            LastErrorMessage = $"Oracle error {ex.Number}: {ex.Message}";
            return false;
        }
        catch (Exception ex)
        {
            LastErrorMessage = ex.Message;
            return false;
        }
    }
}
