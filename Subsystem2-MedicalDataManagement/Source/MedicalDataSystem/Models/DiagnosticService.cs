namespace MedicalDataSystem.Models;

// Diagnostic service entity from HSBA_DV table
public class DiagnosticService
{
    public string MÃDV { get; set; } = string.Empty;
    public string MÃHSBA { get; set; } = string.Empty;
    public string TenDichVu { get; set; } = string.Empty;
    public DateTime Ngay { get; set; }
    public string KẾTQUẢ { get; set; } = string.Empty;
    public bool HoanThanh { get; set; }
}
