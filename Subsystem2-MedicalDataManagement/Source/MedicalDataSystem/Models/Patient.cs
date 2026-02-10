namespace MedicalDataSystem.Models;

// Patient entity from BENHNHAN table
public class Patient
{
    public string MABN { get; set; } = string.Empty;
    public string TENBN { get; set; } = string.Empty;
    public string PHAI { get; set; } = string.Empty;
    public DateTime NGAYSINH { get; set; }
    public string CCCD { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string DiUng { get; set; } = string.Empty;
}
