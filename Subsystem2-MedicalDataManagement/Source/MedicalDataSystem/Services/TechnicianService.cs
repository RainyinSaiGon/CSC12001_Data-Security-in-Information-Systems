namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;

// Service for technician operations
// Manage diagnostic services (VPD-based filtering)
public class TechnicianService
{
    private readonly OracleConnectionService _connectionService;
    private readonly VPDService _vpdService;

    public TechnicianService(OracleConnectionService connectionService, VPDService vpdService)
    {
        _connectionService = connectionService;
        _vpdService = vpdService;
    }

    // Get assigned services for technician (VPD filtered)
    public List<DiagnosticService> GetAssignedServices(string technicianId)
    {
        // TODO: Apply VPD policy to return only assigned services
        return new List<DiagnosticService>();
    }

    // Update service results after testing
    public bool UpdateServiceResult(string serviceId, string result)
    {
        // TODO: Update KẾTQUẢ in HSBA_DV table with audit logging
        return true;
    }

    // Mark service as complete
    public bool CompleteService(string serviceId)
    {
        // TODO: Update service completion status
        return true;
    }
}
