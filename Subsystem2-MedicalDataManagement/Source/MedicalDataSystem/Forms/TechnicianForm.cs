namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class TechnicianForm : Form
{
    private readonly UserSession _session;
    private readonly TechnicianService _technicianService;
    private readonly DataGridView _servicesGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly TextBox _recordIdTextBox = new() { Width = 80 };
    private readonly TextBox _serviceTypeTextBox = new() { Width = 180 };
    private readonly DateTimePicker _serviceDatePicker = new() { Width = 150 };
    private readonly TextBox _resultTextBox = new() { Width = 220 };

    public TechnicianForm(UserSession session)
    {
        _session = session;
        _technicianService = new TechnicianService(new OracleConnectionService(session.ConnectionString), new VPDService(new OracleConnectionService(session.ConnectionString)));
        InitializeComponent();
        BuildUi();
        RefreshData();
    }

    private void BuildUi()
    {
        Text = $"Technician Dashboard - {_session.FullName}";

        var topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 100,
            AutoScroll = true,
            Padding = new Padding(12)
        };

        topPanel.Controls.Add(new Label { Text = "Record ID", AutoSize = true });
        topPanel.Controls.Add(_recordIdTextBox);
        topPanel.Controls.Add(new Label { Text = "Service type", AutoSize = true });
        topPanel.Controls.Add(_serviceTypeTextBox);
        topPanel.Controls.Add(new Label { Text = "Date", AutoSize = true });
        topPanel.Controls.Add(_serviceDatePicker);
        topPanel.Controls.Add(new Label { Text = "Result", AutoSize = true });
        topPanel.Controls.Add(_resultTextBox);

        var saveButton = new Button { Text = "Save result", AutoSize = true };
        saveButton.Click += (_, _) => SaveResult();
        var notificationsButton = new Button { Text = "Notifications", AutoSize = true };
        notificationsButton.Click += (_, _) => new NotificationForm(_session).ShowDialog(this);
        topPanel.Controls.Add(saveButton);
        topPanel.Controls.Add(notificationsButton);

        Controls.Add(_servicesGrid);
        Controls.Add(topPanel);
    }

    private void RefreshData()
    {
        if (_session.StaffId is null)
        {
            return;
        }

        _servicesGrid.DataSource = _technicianService.GetAssignedServices(_session.StaffId.Value.ToString());
    }

    private void SaveResult()
    {
        try
        {
            _technicianService.UpdateServiceResult(
                int.Parse(_recordIdTextBox.Text),
                _serviceTypeTextBox.Text.Trim(),
                _serviceDatePicker.Value,
                _resultTextBox.Text.Trim());

            RefreshData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Technician", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
