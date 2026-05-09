namespace MedicalDataSystem.Services;

using System.Text.RegularExpressions;

public class ValidationService
{
    private static readonly Regex CccdRegex = new("^[0-9]{12}$", RegexOptions.Compiled);
    private static readonly Regex UuidLikeRegex = new("^[A-Fa-f0-9]{32}$|^[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}$", RegexOptions.Compiled);

    public bool ValidateUsername(string username)
    {
        return !string.IsNullOrWhiteSpace(username) && CccdRegex.IsMatch(username);
    }

    public bool ValidatePassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && CccdRegex.IsMatch(password);
    }

    public bool ValidatePatientId(string patientId)
    {
        return !string.IsNullOrWhiteSpace(patientId) && UuidLikeRegex.IsMatch(patientId);
    }

    public bool ValidateMedicalRecord(string diagnosis, string treatment, string conclusion)
    {
        return !string.IsNullOrWhiteSpace(diagnosis)
            || !string.IsNullOrWhiteSpace(treatment)
            || !string.IsNullOrWhiteSpace(conclusion);
    }
}
