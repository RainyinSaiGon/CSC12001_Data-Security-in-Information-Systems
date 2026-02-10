namespace MedicalDataSystem.Models;

// Prescription entity from DONTHUOC table
public class Prescription
{
    public string MADON { get; set; } = string.Empty;
    public string MAHSBA { get; set; } = string.Empty;
    public string TENHOA { get; set; } = string.Empty;
    public string LIEU { get; set; } = string.Empty;
    public string HUONGDAN { get; set; } = string.Empty;
    public DateTime NgayDangKy { get; set; }
}
