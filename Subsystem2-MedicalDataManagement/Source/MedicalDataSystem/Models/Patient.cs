namespace MedicalDataSystem.Models;

// Patient entity from BENHNHAN table
public class Patient
{
    public string MABENHNHAN { get; set; } = string.Empty;
    public string HOTEN { get; set; } = string.Empty;
    public string PHAI { get; set; } = string.Empty;
    public DateTime NGAYSINH { get; set; }
    public string CCCD { get; set; } = string.Empty;
    public string DIENTHOAI { get; set; } = string.Empty;
    public string DIACHI { get; set; } = string.Empty;
    public string DIUNG { get; set; } = string.Empty;
}
