namespace MedicalDataSystem.Services;

using System.Text.RegularExpressions;

public class ValidationService
{
    public bool ValidateUsername(string username)
    {
        return !string.IsNullOrWhiteSpace(username)
            && Regex.IsMatch(username, "^[A-Za-z0-9_]{3,30}$");
    }

    public bool ValidatePassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && password.Length >= 3;
    }

    public bool ValidatePatientId(string patientId)
    {
        return int.TryParse(patientId, out int value) && value > 0;
    }

    public bool ValidateMedicalRecord(string diagnosis, string treatment, string conclusion)
    {
        return !string.IsNullOrWhiteSpace(diagnosis)
            || !string.IsNullOrWhiteSpace(treatment)
            || !string.IsNullOrWhiteSpace(conclusion);
    }
}
