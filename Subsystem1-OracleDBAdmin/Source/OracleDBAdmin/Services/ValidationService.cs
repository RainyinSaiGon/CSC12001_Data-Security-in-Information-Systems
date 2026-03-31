namespace OracleDBAdmin.Services;

using System.Text.RegularExpressions;

public class ValidationService
{
    public bool ValidateIdentifier(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && Regex.IsMatch(value, "^[A-Za-z][A-Za-z0-9_$#]{0,29}$");
    }

    public bool ValidatePassword(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && value.Length >= 3;
    }

    public string QuoteIdentifier(string value)
    {
        return value.Trim().ToUpperInvariant();
    }
}
