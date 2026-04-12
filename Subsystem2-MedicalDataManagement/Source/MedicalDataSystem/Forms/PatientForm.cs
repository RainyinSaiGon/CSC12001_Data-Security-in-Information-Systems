namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class PatientForm : Form
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
        Text = $"Patient Portal - {_session.FullName}";

        var profileLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 160,
            AutoScroll = true,
            Padding = new Padding(12)
        };

        profileLayout.Controls.Add(_identityLabel);
        profileLayout.Controls.Add(new Label { Text = "So nha", AutoSize = true });
        profileLayout.Controls.Add(_sonhaTextBox);
        profileLayout.Controls.Add(new Label { Text = "Ten duong", AutoSize = true });
        profileLayout.Controls.Add(_tenduongTextBox);
        profileLayout.Controls.Add(new Label { Text = "Quan/Huyen", AutoSize = true });
        profileLayout.Controls.Add(_quanhuyenTextBox);
        profileLayout.Controls.Add(new Label { Text = "Tinh/TP", AutoSize = true });
        profileLayout.Controls.Add(_tinhtpTextBox);
        profileLayout.Controls.Add(new Label { Text = "Tien su benh", AutoSize = true });
        profileLayout.Controls.Add(_tiensuTextBox);
        profileLayout.Controls.Add(new Label { Text = "Tien su benh GD", AutoSize = true });
        profileLayout.Controls.Add(_tiensuGiaDinhTextBox);
        profileLayout.Controls.Add(new Label { Text = "Di ung thuoc", AutoSize = true });
        profileLayout.Controls.Add(_diungTextBox);

        var saveButton = new Button { Text = "Save profile", AutoSize = true };
        saveButton.Click += (_, _) => SaveProfile();
        var notificationsButton = new Button { Text = "Notifications", AutoSize = true };
        notificationsButton.Click += (_, _) => new NotificationForm(_session).ShowDialog(this);
        profileLayout.Controls.Add(saveButton);
        profileLayout.Controls.Add(notificationsButton);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(new TabPage("Medical Records") { Controls = { _recordsGrid } });
        tabs.TabPages.Add(new TabPage("Prescriptions") { Controls = { _prescriptionsGrid } });

        Controls.Add(tabs);
        Controls.Add(profileLayout);
    }

    private void LoadData()
    {
        if (string.IsNullOrWhiteSpace(_session.PatientId))
        {
            return;
        }

        Patient? patient = _patientService.GetPatient(_session.PatientId);
        if (patient is null)
        {
            return;
        }

        _identityLabel.Text = $"Patient #{patient.MABN} - {patient.TENBN} - {patient.CCCD}";
        _sonhaTextBox.Text = patient.SONHA;
        _tenduongTextBox.Text = patient.TENDUONG;
        _quanhuyenTextBox.Text = patient.QUANHUYEN;
        _tinhtpTextBox.Text = patient.TINHTP;
        _tiensuTextBox.Text = patient.TIENSUBENH;
        _tiensuGiaDinhTextBox.Text = patient.TIENSUBENHGD;
        _diungTextBox.Text = patient.DIUNGTHUOC;

        _recordsGrid.DataSource = _patientService.GetMyMedicalRecords(_session.PatientId);
        _prescriptionsGrid.DataSource = _patientService.GetMyPrescriptions(_session.PatientId);
    }

    private void SaveProfile()
    {
        if (string.IsNullOrWhiteSpace(_session.PatientId))
        {
            return;
        }

        var patient = new Patient
        {
            MABN = _session.PatientId,
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
            MessageBox.Show(this, "Profile updated.", "Patient", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Patient", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
