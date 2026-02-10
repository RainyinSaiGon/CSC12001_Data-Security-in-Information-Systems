namespace MedicalDataSystem.Models;

// Staff/Employee entity from NHANVIEN table
public class Staff
{
    public string MANV { get; set; } = string.Empty;
    public string HOTEN { get; set; } = string.Empty;
    public string VAITRO { get; set; } = string.Empty; // Coordinator, Doctor/Nurse, Technician
    public string CHUYENKHOA { get; set; } = string.Empty;
}
