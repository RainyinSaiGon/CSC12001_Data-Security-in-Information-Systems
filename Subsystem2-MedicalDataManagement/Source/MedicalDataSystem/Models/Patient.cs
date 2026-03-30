namespace MedicalDataSystem.Models;

public class Patient
{
    public int MABN { get; set; }
    public string TENBN { get; set; } = string.Empty;
    public string PHAI { get; set; } = string.Empty;
    public DateTime NGAYSINH { get; set; }
    public string CCCD { get; set; } = string.Empty;
    public string SONHA { get; set; } = string.Empty;
    public string TENDUONG { get; set; } = string.Empty;
    public string QUANHUYEN { get; set; } = string.Empty;
    public string TINHTP { get; set; } = string.Empty;
    public string TIENSUBENH { get; set; } = string.Empty;
    public string TIENSUBENHGD { get; set; } = string.Empty;
    public string DIUNGTHUOC { get; set; } = string.Empty;
    public string USERNAME { get; set; } = string.Empty;
}
