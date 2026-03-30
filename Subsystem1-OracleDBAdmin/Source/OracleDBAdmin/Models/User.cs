namespace OracleDBAdmin.Models;

public class User
{
    public string Username { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
    public DateTime? Created { get; set; }
    public string DefaultTablespace { get; set; } = string.Empty;
}
