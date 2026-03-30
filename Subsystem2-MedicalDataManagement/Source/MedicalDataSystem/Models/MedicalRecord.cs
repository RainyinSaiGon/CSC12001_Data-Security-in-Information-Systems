namespace MedicalDataSystem.Models;

public class MedicalRecord
{
    public int MAHSBA { get; set; }
    public int MABN { get; set; }
    public DateTime NGAY { get; set; }
    public string CHANDOAN { get; set; } = string.Empty;
    public string DIEUTRI { get; set; } = string.Empty;
    public string KETLUAN { get; set; } = string.Empty;
    public int MABS { get; set; }
    public string MAKHOA { get; set; } = string.Empty;
}
