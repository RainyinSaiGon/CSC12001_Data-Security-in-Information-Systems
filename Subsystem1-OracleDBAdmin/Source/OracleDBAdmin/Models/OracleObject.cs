namespace OracleDBAdmin.Models;

// Oracle database object (table, view, procedure, etc.)
public class OracleObject
{
    public string ObjectName { get; set; } = string.Empty;
    public string ObjectType { get; set; } = string.Empty; // TABLE, VIEW, PROCEDURE, FUNCTION, PACKAGE
    public string Owner { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
