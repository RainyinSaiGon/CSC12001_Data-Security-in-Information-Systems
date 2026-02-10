namespace MedicalDataSystem.Models;

// Notification entity for OLS (Oracle Label Security)
public class Notification
{
    public int NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Classification { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
