namespace OracleDBAdmin.Models;

// Oracle database user entity
public class User
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public List<string> AssignedRoles { get; set; } = new();
    public List<Permission> DirectPermissions { get; set; } = new();
}
