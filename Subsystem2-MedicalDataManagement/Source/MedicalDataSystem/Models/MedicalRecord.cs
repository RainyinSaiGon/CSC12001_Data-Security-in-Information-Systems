namespace MedicalDataSystem.Models;

// Medical record entity from HSBA table
public class MedicalRecord
{
    public string MAHSBA { get; set; } = string.Empty;
    public string MABN { get; set; } = string.Empty;
    public string CHANDOAN { get; set; } = string.Empty;
    public string DIEUTRI { get; set; } = string.Empty;
    public string KETLUAN { get; set; } = string.Empty;
    public DateTime TaoBanGhi { get; set; }
}
