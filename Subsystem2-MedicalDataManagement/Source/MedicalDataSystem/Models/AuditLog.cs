namespace MedicalDataSystem.Models;

public sealed class AuditLog
{
    public string Username { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;
    public DateTime ActionTime { get; set; }
    public int ReturnCode { get; set; }
}
