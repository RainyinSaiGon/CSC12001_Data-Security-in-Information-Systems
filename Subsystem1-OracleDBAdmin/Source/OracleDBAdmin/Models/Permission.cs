namespace OracleDBAdmin.Models;

public class Permission
{
    public string Grantee { get; set; } = string.Empty;
    public string PrivilegeType { get; set; } = string.Empty;
    public string ObjectOwner { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string Grantable { get; set; } = string.Empty;
}
