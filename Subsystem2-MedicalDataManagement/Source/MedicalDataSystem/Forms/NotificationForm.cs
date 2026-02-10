namespace MedicalDataSystem.Forms;

/// <summary>
/// Notification viewer with OLS (Oracle Label Security)
/// Display notifications based on user's label permissions
/// </summary>
public partial class NotificationForm : Form
{
    public NotificationForm()
    {
        InitializeComponent();
    }

    // TODO: Implement notification viewer
    // - Display notifications filtered by OLS labels
    // - Only show notifications user has label access to
    // - Test OLS 3-level hierarchy: Department, Location, Classification
}
