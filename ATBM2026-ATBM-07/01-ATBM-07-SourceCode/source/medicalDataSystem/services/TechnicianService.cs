namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public class TechnicianService
{
    private readonly OracleConnectionService _connectionService;

    public TechnicianService(OracleConnectionService connectionService, VPDService vpdService)
    {
        _connectionService = connectionService;
        _ = vpdService;
    }

    public List<DiagnosticService> GetAssignedServices(string technicianId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV
                FROM V_TECHNICIAN_HSBA_DV
                ORDER BY NGAYDV DESC, MAHSBA DESC
                """;
            _ = technicianId;

            using var reader = command.ExecuteReader();
            var items = new List<DiagnosticService>();
            while (reader.Read())
            {
                items.Add(new DiagnosticService
                {
                    MAHSBA = reader.GetInt32(0),
                    LOAIDV = reader.GetString(1),
                    NGAYDV = reader.GetDateTime(2),
                    KETQUA = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    MAKTV = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                });
            }

            return items;
        });
    }

    public bool UpdateServiceResult(string serviceId, string result)
    {
        string[] parts = serviceId.Split('|', StringSplitOptions.TrimEntries);
        if (parts.Length != 3)
        {
            return false;
        }

        return UpdateServiceResult(int.Parse(parts[0]), parts[1], DateTime.Parse(parts[2]), result);
    }

    public bool UpdateServiceResult(int medicalRecordId, string serviceType, DateTime serviceDate, string result)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.BindByName = true;
            command.CommandText = """
                DECLARE
                    v_mahsba INT := :mahsba;
                    v_loaidv NVARCHAR2(100) := :loaidv;
                    v_ngaydv DATE := TO_DATE(:ngaydv_str, 'YYYY-MM-DD');
                    v_ketqua NVARCHAR2(2000) := :ketqua;
                BEGIN
                    UPDATE V_TECHNICIAN_HSBA_DV
                    SET KETQUA = v_ketqua
                    WHERE MAHSBA = v_mahsba AND LOAIDV = v_loaidv 
                      AND NGAYDV >= v_ngaydv AND NGAYDV < v_ngaydv + 1;
                END;
                """;
            command.Parameters.Add(new OracleParameter("ketqua", result ?? ""));
            command.Parameters.Add(new OracleParameter("mahsba", medicalRecordId));
            command.Parameters.Add(new OracleParameter("loaidv", serviceType ?? ""));
            command.Parameters.Add(new OracleParameter("ngaydv_str", serviceDate.ToString("yyyy-MM-dd")));
            command.ExecuteNonQuery();
            return true;
        });
    }

    public bool CompleteService(string serviceId)
    {
        return UpdateServiceResult(serviceId, "Completed");
    }
}
