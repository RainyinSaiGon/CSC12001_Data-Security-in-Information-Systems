namespace MedicalDataSystem.Models;

public sealed class CreateUserRequest
{
    public string UserType { get; set; } = string.Empty; // STAFF or PATIENT
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string IDNumber { get; set; } = string.Empty; // CMND/CCCD

    // STAFF fields
    public string Address { get; set; } = string.Empty; // QUEQUAN
    public string Phone { get; set; } = string.Empty; // SODT
    public string Role { get; set; } = string.Empty; // VAITRO
    public string Department { get; set; } = string.Empty; // CHUYENKHOA

    // PATIENT fields
    public string SONHA { get; set; } = string.Empty;
    public string TENDUONG { get; set; } = string.Empty;
    public string QUANHUYEN { get; set; } = string.Empty;
    public string TINHTP { get; set; } = string.Empty;
    public string TIENSUBENH { get; set; } = string.Empty;
    public string TIENSUBENHGD { get; set; } = string.Empty;
    public string DIUNGTHUOC { get; set; } = string.Empty;
}
