namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class TechnicianForm : BaseMedicalForm
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
        BackColor = Color.FromArgb(241, 244, 249);
        MinimumSize = new Size(920, 620);

        var scrollHost = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            AutoScrollMinSize = new Size(0, 640),
            BackColor = Color.FromArgb(241, 244, 249)
        };

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(12, 12, 12, 56),
            BackColor = Color.FromArgb(241, 244, 249)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 138));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var headerCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(12, 10, 12, 10),
            Margin = new Padding(0, 0, 0, 10)
        };

        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty
        };
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

        var headerTextPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        headerTextPanel.Controls.Add(new Label
        {
            Text = "Technician Dashboard",
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Margin = Padding.Empty
        });

        headerTextPanel.Controls.Add(new Label
        {
            Text = $"Welcome back, {_session.FullName}. Manage assigned services and update results",
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(100, 116, 139),
            Margin = new Padding(0, 2, 0, 0)
        });

        var notificationsButton = new Button
        {
            Text = "Notifications",
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(248, 250, 252),
            Margin = new Padding(0, 0, 0, 0),
            Padding = new Padding(10, 4, 10, 4)
        };
        notificationsButton.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);

        headerLayout.Controls.Add(headerTextPanel, 0, 0);
        headerLayout.Controls.Add(notificationsButton, 1, 0);
        headerCard.Controls.Add(headerLayout);

        var actionCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(12, 10, 12, 10),
            Margin = new Padding(0, 0, 0, 10)
        };

        var actionLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 4,
            Margin = Padding.Empty
        };
        actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
        actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
        actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

        actionLayout.Controls.Add(new Label { Text = "Record ID", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 0);
        actionLayout.Controls.Add(new Label { Text = "Service type", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 0);
        actionLayout.Controls.Add(new Label { Text = "Date", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 2, 0);
        actionLayout.Controls.Add(_recordIdTextBox, 0, 1);
        actionLayout.Controls.Add(_serviceTypeTextBox, 1, 1);
        actionLayout.Controls.Add(_serviceDatePicker, 2, 1);
        actionLayout.Controls.Add(new Label { Text = "Result", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 2);

        _recordIdTextBox.Dock = DockStyle.Fill;
        _recordIdTextBox.Margin = new Padding(0, 0, 10, 0);
        _recordIdTextBox.PlaceholderText = "Enter record ID";
        _serviceTypeTextBox.Dock = DockStyle.Fill;
        _serviceTypeTextBox.Margin = new Padding(0, 0, 10, 0);
        _serviceTypeTextBox.PlaceholderText = "Enter service type";
        _serviceDatePicker.Dock = DockStyle.Fill;
        _serviceDatePicker.Margin = new Padding(0, 0, 10, 0);
        _resultTextBox.Dock = DockStyle.Fill;
        _resultTextBox.Margin = new Padding(0, 0, 10, 0);
        _resultTextBox.PlaceholderText = "Enter test result or observations";

        var saveButton = new Button
        {
            Text = "Save result",
            Dock = DockStyle.Fill,
            MinimumSize = new Size(0, 36),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += (_, _) => SaveResult();
        notificationsButton.Click += (_, _) => new NotificationForm(_session).ShowDialog(this);

        actionLayout.Controls.Add(_resultTextBox, 0, 3);
        actionLayout.SetColumnSpan(_resultTextBox, 3);
        actionLayout.Controls.Add(saveButton, 3, 3);
        actionCard.Controls.Add(actionLayout);

        var gridCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(12, 10, 12, 12),
            Margin = Padding.Empty
        };

        var gridCardLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty
        };
        gridCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        gridCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        gridCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        gridCardLayout.Controls.Add(new Label
        {
            Text = "Assigned Services",
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        }, 0, 0);

        gridCardLayout.Controls.Add(new Label
        {
            Text = "View and manage all services assigned to you",
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(100, 116, 139)
        }, 0, 1);

        _servicesGrid.BackgroundColor = Color.White;
        _servicesGrid.BorderStyle = BorderStyle.None;
        _servicesGrid.EnableHeadersVisualStyles = false;
        _servicesGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
        _servicesGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
        _servicesGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
        _servicesGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 234, 255);
        _servicesGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
        _servicesGrid.RowTemplate.Height = 26;

        gridCardLayout.Controls.Add(_servicesGrid, 0, 2);
        gridCard.Controls.Add(gridCardLayout);

        root.Controls.Add(headerCard, 0, 0);
        root.Controls.Add(actionCard, 0, 1);
        root.Controls.Add(gridCard, 0, 2);

        scrollHost.Controls.Add(root);
        Controls.Add(scrollHost);
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
