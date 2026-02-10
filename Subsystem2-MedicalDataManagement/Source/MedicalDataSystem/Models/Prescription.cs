namespace MedicalDataSystem.Models;

// Prescription entity from DONTHUOC table
public class Prescription
{
    public string MADONTHUOC { get; set; } = string.Empty;
    public string MAHSBA { get; set; } = string.Empty;
    public string TENTHUOC { get; set; } = string.Empty;
    public string LIEUDUNG { get; set; } = string.Empty;
    public string HUONGDAN { get; set; } = string.Empty;
    public DateTime NGAYDANGKY { get; set; }
}
