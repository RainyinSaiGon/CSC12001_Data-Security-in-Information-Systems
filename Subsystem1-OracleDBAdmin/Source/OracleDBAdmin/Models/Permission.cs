namespace OracleDBAdmin.Models;

// Oracle permission/privilege entity
public class Permission
{
    public string GrantedTo { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;
    public string PermissionType { get; set; } = string.Empty; // SELECT, INSERT, UPDATE, DELETE
    public List<string> Columns { get; set; } = new();
    public bool WithGrantOption { get; set; }
    public DateTime GrantedDate { get; set; }
}
