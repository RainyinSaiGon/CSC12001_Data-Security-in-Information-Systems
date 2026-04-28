namespace MedicalDataSystem.Models;

public sealed class FgaAuditLog
{
    public string DbUser { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;
    public string PolicyName { get; set; } = string.Empty;
    public string StatementType { get; set; } = string.Empty;
    public DateTime ActionTime { get; set; }
    public string SqlText { get; set; } = string.Empty;
}
