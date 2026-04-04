namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class DoctorForm : Form
{
    private readonly UserSession _session;
    private readonly DoctorService _doctorService;
    private readonly DataGridView _patientsGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly DataGridView _recordsGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly TextBox _recordIdTextBox = new() { Width = 80 };
    private readonly TextBox _diagnosisTextBox = new() { Width = 200 };
    private readonly TextBox _treatmentTextBox = new() { Width = 200 };
    private readonly TextBox _conclusionTextBox = new() { Width = 200 };
    private readonly TextBox _serviceTypeTextBox = new() { Width = 180 };
    private readonly DateTimePicker _serviceDatePicker = new() { Width = 150 };
    private readonly TextBox _prescriptionNameTextBox = new() { Width = 160 };
    private readonly TextBox _prescriptionDoseTextBox = new() { Width = 160 };
    private readonly DateTimePicker _prescriptionDatePicker = new() { Width = 150 };

    public DoctorForm(UserSession session)
    {
        _session = session;
        _doctorService = new DoctorService(new OracleConnectionService(session.ConnectionString), new VPDService(new OracleConnectionService(session.ConnectionString)));
        InitializeComponent();
        BuildUi();
        RefreshData();
    }

    private void BuildUi()
    {
        Text = $"Doctor Dashboard - {_session.FullName}";

        var topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 140,
            AutoScroll = true,
            Padding = new Padding(12)
        };

        topPanel.Controls.Add(new Label { Text = "Record ID", AutoSize = true });
        topPanel.Controls.Add(_recordIdTextBox);
        topPanel.Controls.Add(new Label { Text = "Diagnosis", AutoSize = true });
        topPanel.Controls.Add(_diagnosisTextBox);
        topPanel.Controls.Add(new Label { Text = "Treatment", AutoSize = true });
        topPanel.Controls.Add(_treatmentTextBox);
        topPanel.Controls.Add(new Label { Text = "Conclusion", AutoSize = true });
        topPanel.Controls.Add(_conclusionTextBox);
        var updateRecordButton = new Button { Text = "Update record", AutoSize = true };
        updateRecordButton.Click += (_, _) => UpdateRecord();
        topPanel.Controls.Add(updateRecordButton);

        topPanel.Controls.Add(new Label { Text = "Service type", AutoSize = true });
        topPanel.Controls.Add(_serviceTypeTextBox);
        topPanel.Controls.Add(new Label { Text = "Service date", AutoSize = true });
        topPanel.Controls.Add(_serviceDatePicker);
        var addServiceButton = new Button { Text = "Order service", AutoSize = true };
        addServiceButton.Click += (_, _) => AddService();
        topPanel.Controls.Add(addServiceButton);

        topPanel.Controls.Add(new Label { Text = "Drug", AutoSize = true });
        topPanel.Controls.Add(_prescriptionNameTextBox);
        topPanel.Controls.Add(new Label { Text = "Dose", AutoSize = true });
        topPanel.Controls.Add(_prescriptionDoseTextBox);
        topPanel.Controls.Add(new Label { Text = "Date", AutoSize = true });
        topPanel.Controls.Add(_prescriptionDatePicker);
        var savePrescriptionButton = new Button { Text = "Save prescription", AutoSize = true };
        savePrescriptionButton.Click += (_, _) => SavePrescription();
        topPanel.Controls.Add(savePrescriptionButton);

        var notificationsButton = new Button { Text = "Notifications", AutoSize = true };
        notificationsButton.Click += (_, _) => new NotificationForm(_session).ShowDialog(this);
        topPanel.Controls.Add(notificationsButton);

        var logoutButton = new Button { Text = "Log out", AutoSize = true };
        logoutButton.Click += (_, _) => Logout();
        topPanel.Controls.Add(logoutButton);

        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
        split.Panel1.Controls.Add(_patientsGrid);
        split.Panel2.Controls.Add(_recordsGrid);

        Controls.Add(split);
        Controls.Add(topPanel);
    }

    private void RefreshData()
    {
        if (_session.StaffId is null)
        {
            return;
        }

        string doctorId = _session.StaffId.Value.ToString();
        _patientsGrid.DataSource = _doctorService.GetAssignedPatients(doctorId);
        _recordsGrid.DataSource = _doctorService.GetAssignedMedicalRecords(doctorId);
    }

    private void UpdateRecord()
    {
        try
        {
            var record = new MedicalRecord
            {
                MAHSBA = int.Parse(_recordIdTextBox.Text),
                CHANDOAN = _diagnosisTextBox.Text.Trim(),
                DIEUTRI = _treatmentTextBox.Text.Trim(),
                KETLUAN = _conclusionTextBox.Text.Trim()
            };

            _doctorService.UpdateMedicalRecord(record);
            RefreshData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Doctor", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AddService()
    {
        try
        {
            var service = new DiagnosticService
            {
                MAHSBA = int.Parse(_recordIdTextBox.Text),
                LOAIDV = _serviceTypeTextBox.Text.Trim(),
                NGAYDV = _serviceDatePicker.Value
            };

            _doctorService.OrderDiagnosticService(service);
            RefreshData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Doctor", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SavePrescription()
    {
        try
        {
            var prescription = new Prescription
            {
                MAHSBA = int.Parse(_recordIdTextBox.Text),
                TENTHUOC = _prescriptionNameTextBox.Text.Trim(),
                LIEUDUNG = _prescriptionDoseTextBox.Text.Trim(),
                NGAYDT = _prescriptionDatePicker.Value
            };

            _doctorService.UpdatePrescription(prescription);
            MessageBox.Show(this, "Prescription saved.", "Doctor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Doctor", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Logout()
    {
        if (MessageBox.Show(this, "Are you sure you want to log out?", "Log out", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        DialogResult = DialogResult.Retry;
        Close();
    }
}
