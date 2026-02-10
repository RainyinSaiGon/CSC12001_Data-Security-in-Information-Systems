namespace MedicalDataSystem.Models;

// Prescription entity from ĐƠNTHUỐC table
public class Prescription
{
    public string MÃĐƠN { get; set; } = string.Empty;
    public string MÃHSBA { get; set; } = string.Empty;
    public string TÊNHÓA { get; set; } = string.Empty;
    public string LIỀU { get; set; } = string.Empty;
    public string HƯỚNGDẪN { get; set; } = string.Empty;
    public DateTime NgayDangKy { get; set; }
}
