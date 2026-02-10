namespace MedicalDataSystem.Models;

// Medical record entity from HSBA table
public class MedicalRecord
{
    public string MÃHSBA { get; set; } = string.Empty;
    public string MÃBN { get; set; } = string.Empty;
    public string CHẨNĐOÁN { get; set; } = string.Empty;
    public string ĐIỀUTRỊ { get; set; } = string.Empty;
    public string KẾTLUẬN { get; set; } = string.Empty;
    public DateTime TaoBanGhi { get; set; }
}
