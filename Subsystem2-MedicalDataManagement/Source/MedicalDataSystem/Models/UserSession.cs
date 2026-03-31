namespace MedicalDataSystem.Models;

public class UserSession
{
    public string Username { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public int? StaffId { get; init; }
    public int? PatientId { get; init; }
    public string? DepartmentCode { get; init; }
    public string ConnectionString { get; init; } = string.Empty;
    public string DataSource { get; init; } = string.Empty;
}
