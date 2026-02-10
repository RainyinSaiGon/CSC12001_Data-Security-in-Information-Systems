namespace MedicalDataSystem.Models;

// Patient entity from BỆNHNHÂN table
public class Patient
{
    public string MÃBN { get; set; } = string.Empty;
    public string TÊNBN { get; set; } = string.Empty;
    public string PHÁI { get; set; } = string.Empty;
    public DateTime NGÀYSINH { get; set; }
    public string CCCD { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string DiUng { get; set; } = string.Empty;
}
