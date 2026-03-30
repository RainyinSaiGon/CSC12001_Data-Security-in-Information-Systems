namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class NotificationForm : Form
{
    private readonly UserSession _session;
    private readonly OLSService _olsService;
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };

    public NotificationForm(UserSession session)
    {
        _session = session;
        _olsService = new OLSService(new OracleConnectionService(session.ConnectionString));
        InitializeComponent();
        BuildUi();
        LoadNotifications();
    }

    private void BuildUi()
    {
        var header = new Label
        {
            Dock = DockStyle.Top,
            Height = 36,
            Text = $"Accessible notifications for {_session.FullName} ({_session.Role})",
            TextAlign = ContentAlignment.MiddleLeft
        };

        Controls.Add(_grid);
        Controls.Add(header);
    }

    private void LoadNotifications()
    {
        try
        {
            List<Notification> notifications = _olsService.GetAccessibleNotificationsDetailed();
            _grid.DataSource = notifications;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Notifications", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
