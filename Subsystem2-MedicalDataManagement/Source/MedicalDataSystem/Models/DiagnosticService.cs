namespace MedicalDataSystem.Models;

// Diagnostic service entity from HSBA_DV table
public class DiagnosticService
{
    public string MADICHVU { get; set; } = string.Empty;
    public string MAHSBA { get; set; } = string.Empty;
    public string TENDICHVU { get; set; } = string.Empty;
    public DateTime NGAY { get; set; }
    public string KETQUA { get; set; } = string.Empty;
    public bool HOANTHANH { get; set; }
    public string MAKYTHUATVIEN { get; set; } = string.Empty;
}
