namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;

// Service for doctor/nurse operations
// Implements VPD-based filtering for patient records
public class DoctorService
{
    private readonly OracleConnectionService _connectionService;
    private readonly VPDService _vpdService;

    public DoctorService(OracleConnectionService connectionService, VPDService vpdService)
    {
        _connectionService = connectionService;
        _vpdService = vpdService;
    }

    // Get patients assigned to doctor (VPD filtered)
    public List<Patient> GetAssignedPatients(string doctorId)
    {
        // TODO: Apply VPD policy to return only doctor's assigned patients
        return new List<Patient>();
    }

    // Create diagnosis for patient
    public bool CreateDiagnosis(MedicalRecord record)
    {
        // TODO: Insert diagnosis into HSBA table
        return true;
    }

    // Update prescription
    public bool UpdatePrescription(Prescription prescription)
    {
        // TODO: Update prescription in ĐƠNTHUỐC table with audit logging
        return true;
    }

    // Order diagnostic service for patient
    public bool OrderDiagnosticService(DiagnosticService service)
    {
        // TODO: Insert service order into HSBA_DV table
        return true;
    }
}
