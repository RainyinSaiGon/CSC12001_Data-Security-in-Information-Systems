namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;

// Service for patient-related operations
public class PatientService
{
    private readonly OracleConnectionService _connectionService;

    public PatientService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    // Get patient details (row-level security applied)
    public Patient? GetPatient(string patientId)
    {
        // TODO: Query patient from BENHNHAN table with row-level security
        return null;
    }

    // Update patient contact information
    public bool UpdatePatientInfo(Patient patient)
    {
        // TODO: Update patient record in Oracle
        return true;
    }

    // Get medical records for authenticated patient
    public List<MedicalRecord> GetMyMedicalRecords(string patientId)
    {
        // TODO: Return only authenticated patient's records
        // Row-level security ensures patient can only see own records
        return new List<MedicalRecord>();
    }

    // Get prescriptions for authenticated patient
    public List<Prescription> GetMyPrescriptions(string patientId)
    {
        // TODO: Return only authenticated patient's prescriptions
        return new List<Prescription>();
    }
}
