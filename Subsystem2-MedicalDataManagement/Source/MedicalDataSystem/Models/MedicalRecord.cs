namespace MedicalDataSystem.Models;

public class MedicalRecord
{
    public int MAHSBA { get; set; }
    public string MABN { get; set; } = string.Empty;
    public DateTime NGAY { get; set; }
    public string CHANDOAN { get; set; } = string.Empty;
    public string DIEUTRI { get; set; } = string.Empty;
    public string KETLUAN { get; set; } = string.Empty;
    public string MABS { get; set; } = string.Empty;
    public string MAKHOA { get; set; } = string.Empty;
}
