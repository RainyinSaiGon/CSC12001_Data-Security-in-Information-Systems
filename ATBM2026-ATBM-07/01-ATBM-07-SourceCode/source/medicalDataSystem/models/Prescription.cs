namespace MedicalDataSystem.Models;

public class Prescription
{
    public int MAHSBA { get; set; }
    public DateTime NGAYDT { get; set; }
    public string TENTHUOC { get; set; } = string.Empty;
    public string LIEUDUNG { get; set; } = string.Empty;
}
