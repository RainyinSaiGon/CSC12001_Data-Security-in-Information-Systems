namespace MedicalDataSystem.Models;

public sealed class SessionAuditLog
{
    public string Username { get; set; } = string.Empty;
    public string UserHost { get; set; } = string.Empty;
    public string Terminal { get; set; } = string.Empty;
    public int ReturnCode { get; set; }
    public DateTime LogonTime { get; set; }
    public DateTime? LogoffTime { get; set; }
    public long SessionId { get; set; }
}
