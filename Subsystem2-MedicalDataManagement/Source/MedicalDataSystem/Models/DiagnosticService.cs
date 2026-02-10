namespace MedicalDataSystem.Models;

// Diagnostic service entity from HSBA_DV table
public class DiagnosticService
{
    public string MADV { get; set; } = string.Empty;
    public string MAHSBA { get; set; } = string.Empty;
    public string TenDichVu { get; set; } = string.Empty;
    public DateTime Ngay { get; set; }
    public string KETQUA { get; set; } = string.Empty;
    public bool HoanThanh { get; set; }
}
