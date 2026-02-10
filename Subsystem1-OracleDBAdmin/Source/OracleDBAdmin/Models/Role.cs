namespace OracleDBAdmin.Models;

// Oracle database role entity
public class Role
{
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
