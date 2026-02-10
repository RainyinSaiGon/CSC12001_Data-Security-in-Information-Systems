namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;

// Service for coordinator operations
// Manages patient records and staff assignments (RBAC-based)
public class CoordinatorService
{
    private readonly OracleConnectionService _connectionService;

    public CoordinatorService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    // Get all patients (coordinators can see all)
    public List<Patient> GetAllPatients()
    {
        // TODO: Query all patients from BENHNHAN table
        return new List<Patient>();
    }

    // Add new patient
    public bool AddPatient(Patient patient)
    {
        // TODO: Insert new patient into BENHNHAN table
        return true;
    }

    // Edit patient information
    public bool EditPatient(Patient patient)
    {
        // TODO: Update patient record in BENHNHAN table
        return true;
    }

    // Assign doctor to patient
    public bool AssignDoctorToPatient(string doctorId, string patientId)
    {
        // TODO: Create assignment record in database
        return true;
    }

    // Assign technician to service
    public bool AssignTechnicianToService(string technicianId, string serviceId)
    {
        // TODO: Assign technician to diagnostic service
        return true;
    }

    // Get record status for tracking
    public string GetRecordStatus(string recordId)
    {
        // TODO: Query record status from database
        return string.Empty;
    }
}
