namespace MedicalDataSystem.Models;

// Staff/Employee entity from NHÂNVIÊN table
public class Staff
{
    public string MÃNV { get; set; } = string.Empty;
    public string HỌTÊN { get; set; } = string.Empty;
    public string VAITRÒ { get; set; } = string.Empty; // Coordinator, Doctor/Nurse, Technician
    public string CHUYÊNKHOA { get; set; } = string.Empty;
}
