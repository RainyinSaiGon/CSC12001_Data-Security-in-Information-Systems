namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class PatientForm : BaseMedicalForm
{
    private readonly UserSession _session;
    private readonly PatientService _patientService;
    private readonly TextBox _sonhaTextBox = new() { Width = 140 };
    private readonly TextBox _tenduongTextBox = new() { Width = 180 };
    private readonly TextBox _quanhuyenTextBox = new() { Width = 160 };
    private readonly TextBox _tinhtpTextBox = new() { Width = 160 };
    private readonly TextBox _tiensuTextBox = new() { Width = 220 };
    private readonly TextBox _tiensuGiaDinhTextBox = new() { Width = 220 };
    private readonly TextBox _diungTextBox = new() { Width = 220 };
    private readonly Label _identityLabel = new() { AutoSize = true };
    private readonly DataGridView _recordsGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly DataGridView _prescriptionsGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };

    public PatientForm(UserSession session)
    {
        _session = session;
        _patientService = new PatientService(new OracleConnectionService(session.ConnectionString));
        InitializeComponent();
        BuildUi();
        LoadData();
    }

    private void BuildUi()
    {
        Text = $"Cổng thông tin bệnh nhân - {_session.FullName}";
        BackColor = Color.FromArgb(241, 244, 249);
        MinimumSize = new Size(980, 680);

        _sonhaTextBox.Dock = DockStyle.Fill;
        _sonhaTextBox.PlaceholderText = "Số nhà";
        _tenduongTextBox.Dock = DockStyle.Fill;
        _tenduongTextBox.PlaceholderText = "Tên đường";
        _quanhuyenTextBox.Dock = DockStyle.Fill;
        _quanhuyenTextBox.PlaceholderText = "Quận/Huyện";
        _tinhtpTextBox.Dock = DockStyle.Fill;
        _tinhtpTextBox.PlaceholderText = "Tỉnh/TP";
        _tiensuTextBox.Dock = DockStyle.Fill;
        _tiensuTextBox.PlaceholderText = "Tiền sử bệnh";
        _tiensuGiaDinhTextBox.Dock = DockStyle.Fill;
        _tiensuGiaDinhTextBox.PlaceholderText = "Tiền sử bệnh gia đình";
        _diungTextBox.Dock = DockStyle.Fill;
        _diungTextBox.PlaceholderText = "Dị ứng thuốc";

        ConfigureReadOnlyPersonalField(_sonhaTextBox);
        ConfigureReadOnlyPersonalField(_tenduongTextBox);
        ConfigureReadOnlyPersonalField(_quanhuyenTextBox);
        ConfigureReadOnlyPersonalField(_tinhtpTextBox);
        ConfigureReadOnlyPersonalField(_tiensuTextBox);
        ConfigureReadOnlyPersonalField(_tiensuGiaDinhTextBox);
        ConfigureReadOnlyPersonalField(_diungTextBox);

        ConfigureGrid(_recordsGrid);
        ConfigureGrid(_prescriptionsGrid);

        var saveButton = new Button
        {
            Text = "Lưu hồ sơ",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            Padding = Padding.Empty
        };
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += (_, _) => SaveProfile();

        var notificationsButton = new Button
        {
            Text = "Thông báo",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(248, 250, 252),
            Font = new Font("Segoe UI", 8),
            Padding = Padding.Empty
        };
        notificationsButton.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        notificationsButton.Click += (_, _) => new NotificationForm(_session).ShowDialog(this);

        _identityLabel.AutoSize = true;
        _identityLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        _identityLabel.ForeColor = Color.FromArgb(15, 23, 42);

        var scrollHost = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            AutoScrollMinSize = new Size(0, 700),
            BackColor = Color.FromArgb(241, 244, 249)
        };

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(12, 12, 12, 56),
            BackColor = Color.FromArgb(241, 244, 249)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var profileCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = 190,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(12),
            Margin = new Padding(0, 0, 0, 10)
        };

        var profileLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 5,
            Margin = Padding.Empty
        };
        for (int i = 0; i < 6; i++)
        {
            profileLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66f));
        }
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));

        profileLayout.Controls.Add(_identityLabel, 0, 0);
        profileLayout.SetColumnSpan(_identityLabel, 6);

        var editHintLabel = new Label
        {
            Text = "(Nhấp đúp vào ô để chỉnh sửa)",
            AutoSize = true,
            Font = new Font("Segoe UI", 7.5f, FontStyle.Italic),
            ForeColor = Color.FromArgb(100, 116, 139),
            Margin = new Padding(0, 0, 0, 0)
        };
        profileLayout.Controls.Add(editHintLabel, 0, 1);
        profileLayout.SetColumnSpan(editHintLabel, 6);

        profileLayout.Controls.Add(new Label { Text = "Số nhà", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 2);
        profileLayout.Controls.Add(new Label { Text = "Tên đường", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 2);
        profileLayout.Controls.Add(new Label { Text = "Quận/Huyện", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 2, 2);
        profileLayout.Controls.Add(new Label { Text = "Tỉnh/TP", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 3, 2);
        profileLayout.Controls.Add(new Label { Text = "Tiền sử bệnh", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 4, 2);

        profileLayout.Controls.Add(_sonhaTextBox, 0, 3);
        profileLayout.Controls.Add(_tenduongTextBox, 1, 3);
        profileLayout.Controls.Add(_quanhuyenTextBox, 2, 3);
        profileLayout.Controls.Add(_tinhtpTextBox, 3, 3);
        profileLayout.Controls.Add(_tiensuTextBox, 4, 3);

        profileLayout.Controls.Add(new Label { Text = "Tiền sử bệnh GD", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 4);
        profileLayout.Controls.Add(new Label { Text = "Dị ứng thuốc", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 4);

        profileLayout.Controls.Add(_tiensuGiaDinhTextBox, 0, 5);
        profileLayout.Controls.Add(_diungTextBox, 1, 5);
        profileLayout.Controls.Add(saveButton, 4, 5);
        profileLayout.Controls.Add(notificationsButton, 5, 5);

        profileCard.Controls.Add(profileLayout);

        var dataCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = 360,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(10),
            Margin = Padding.Empty
        };

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9)
        };
        tabs.TabPages.Add(new TabPage("Hồ sơ bệnh án") { Controls = { _recordsGrid } });
        tabs.TabPages.Add(new TabPage("Đơn thuốc") { Controls = { _prescriptionsGrid } });

        dataCard.Controls.Add(tabs);

        root.Controls.Add(profileCard, 0, 0);
        root.Controls.Add(dataCard, 0, 1);

        scrollHost.Controls.Add(root);
        Controls.Add(scrollHost);
    }

    private static void ConfigureGrid(DataGridView grid)
    {
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 234, 255);
        grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
        grid.RowTemplate.Height = 26;
    }

    private static void ConfigureReadOnlyPersonalField(TextBox textBox)
    {
        textBox.ReadOnly = true;
        textBox.BackColor = Color.FromArgb(242, 244, 248);
        textBox.DoubleClick += (_, _) =>
        {
            textBox.ReadOnly = false;
            textBox.BackColor = Color.White;
            textBox.Focus();
            textBox.SelectAll();
        };
        textBox.Leave += (_, _) =>
        {
            textBox.ReadOnly = true;
            textBox.BackColor = Color.FromArgb(242, 244, 248);
        };
    }

    private void LoadData()
    {
        if (_session.PatientId is null)
        {
            return;
        }

        Patient? patient = _patientService.GetPatient(_session.PatientId.Value.ToString());
        if (patient is null)
        {
            return;
        }

        _identityLabel.Text = $"Bệnh nhân #{patient.MABN} - {patient.TENBN} - {patient.CCCD}";
        _sonhaTextBox.Text = patient.SONHA;
        _tenduongTextBox.Text = patient.TENDUONG;
        _quanhuyenTextBox.Text = patient.QUANHUYEN;
        _tinhtpTextBox.Text = patient.TINHTP;
        _tiensuTextBox.Text = patient.TIENSUBENH;
        _tiensuGiaDinhTextBox.Text = patient.TIENSUBENHGD;
        _diungTextBox.Text = patient.DIUNGTHUOC;

        _recordsGrid.DataSource = _patientService.GetMyMedicalRecords(_session.PatientId.Value.ToString());
        _prescriptionsGrid.DataSource = _patientService.GetMyPrescriptions(_session.PatientId.Value.ToString());
    }

    private void SaveProfile()
    {
        if (_session.PatientId is null)
        {
            return;
        }

        var patient = new Patient
        {
            MABN = _session.PatientId.Value,
            SONHA = _sonhaTextBox.Text.Trim(),
            TENDUONG = _tenduongTextBox.Text.Trim(),
            QUANHUYEN = _quanhuyenTextBox.Text.Trim(),
            TINHTP = _tinhtpTextBox.Text.Trim(),
            TIENSUBENH = _tiensuTextBox.Text.Trim(),
            TIENSUBENHGD = _tiensuGiaDinhTextBox.Text.Trim(),
            DIUNGTHUOC = _diungTextBox.Text.Trim()
        };

        try
        {
            _patientService.UpdatePatientInfo(patient);
            MessageBox.Show(this, "Đã cập nhật hồ sơ.", "Bệnh nhân", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Bệnh nhân", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
