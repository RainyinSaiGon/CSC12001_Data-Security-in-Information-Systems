namespace MedicalDataSystem.Services;

public class VPDService
{
    private readonly OracleConnectionService _connectionService;

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
}
