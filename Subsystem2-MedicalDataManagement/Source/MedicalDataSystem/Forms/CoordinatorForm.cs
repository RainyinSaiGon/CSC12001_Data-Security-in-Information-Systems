namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class CoordinatorForm : BaseMedicalForm
{
    private readonly UserSession _session;
    private readonly CoordinatorService _coordinatorService;
    private readonly DataGridView _patientsGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly ComboBox _doctorComboBox = new() { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _technicianComboBox = new() { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _patientIdTextBox = new() { Width = 80 };
    private readonly TextBox _recordIdTextBox = new() { Width = 80 };
    private readonly TextBox _serviceTypeTextBox = new() { Width = 180 };
    private readonly DateTimePicker _serviceDatePicker = new() { Width = 160 };
    private readonly TextBox _nameTextBox = new() { Width = 180 };
    private readonly TextBox _cccdTextBox = new() { Width = 140 };
    private readonly TextBox _addressTextBox = new() { Width = 260 };
    private readonly TextBox _medicalHistoryTextBox = new() { Width = 200 };
    private readonly TextBox _familyHistoryTextBox = new() { Width = 200 };
    private readonly TextBox _allergyTextBox = new() { Width = 180 };

    public CoordinatorForm(UserSession session)
    {
        _session = session;
        _coordinatorService = new CoordinatorService(new OracleConnectionService(session.ConnectionString));
        InitializeComponent();
        BuildUi();
        LoadReferenceData();
        RefreshPatients();
    }

    private void BuildUi()
    {
        Text = $"Coordinator Dashboard - {_session.FullName}";

        var topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 170,
            AutoScroll = true,
            Padding = new Padding(12)
        };

        topPanel.Controls.Add(new Label { Text = "Name", AutoSize = true });
        topPanel.Controls.Add(_nameTextBox);
        topPanel.Controls.Add(new Label { Text = "CCCD", AutoSize = true });
        topPanel.Controls.Add(_cccdTextBox);
        topPanel.Controls.Add(new Label { Text = "Address", AutoSize = true });
        topPanel.Controls.Add(_addressTextBox);
        topPanel.Controls.Add(new Label { Text = "Medical history", AutoSize = true });
        topPanel.Controls.Add(_medicalHistoryTextBox);
        topPanel.Controls.Add(new Label { Text = "Family history", AutoSize = true });
        topPanel.Controls.Add(_familyHistoryTextBox);
        topPanel.Controls.Add(new Label { Text = "Allergy", AutoSize = true });
        topPanel.Controls.Add(_allergyTextBox);

        var addButton = new Button { Text = "Add patient", AutoSize = true };
        addButton.Click += (_, _) => AddPatient();
        var refreshButton = new Button { Text = "Refresh", AutoSize = true };
        refreshButton.Click += (_, _) => RefreshPatients();
        var notificationsButton = new Button { Text = "Notifications", AutoSize = true };
        notificationsButton.Click += (_, _) => new NotificationForm(_session).ShowDialog(this);
        topPanel.Controls.Add(addButton);
        topPanel.Controls.Add(refreshButton);
        topPanel.Controls.Add(notificationsButton);

        var assignmentPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 120,
            AutoScroll = true,
            Padding = new Padding(12)
        };

        assignmentPanel.Controls.Add(new Label { Text = "Patient ID", AutoSize = true });
        assignmentPanel.Controls.Add(_patientIdTextBox);
        assignmentPanel.Controls.Add(new Label { Text = "Doctor", AutoSize = true });
        assignmentPanel.Controls.Add(_doctorComboBox);
        var assignDoctorButton = new Button { Text = "Create record + assign doctor", AutoSize = true };
        assignDoctorButton.Click += (_, _) => CreateRecord();
        assignmentPanel.Controls.Add(assignDoctorButton);

        assignmentPanel.Controls.Add(new Label { Text = "Record ID", AutoSize = true });
        assignmentPanel.Controls.Add(_recordIdTextBox);
        assignmentPanel.Controls.Add(new Label { Text = "Service type", AutoSize = true });
        assignmentPanel.Controls.Add(_serviceTypeTextBox);
        assignmentPanel.Controls.Add(new Label { Text = "Date", AutoSize = true });
        assignmentPanel.Controls.Add(_serviceDatePicker);
        assignmentPanel.Controls.Add(new Label { Text = "Technician", AutoSize = true });
        assignmentPanel.Controls.Add(_technicianComboBox);
        var assignTechnicianButton = new Button { Text = "Assign technician", AutoSize = true };
        assignTechnicianButton.Click += (_, _) => AssignTechnician();
        assignmentPanel.Controls.Add(assignTechnicianButton);

        Controls.Add(_patientsGrid);
        Controls.Add(assignmentPanel);
        Controls.Add(topPanel);
    }

    private void LoadReferenceData()
    {
        _doctorComboBox.DataSource = _coordinatorService.GetDoctors();
        _doctorComboBox.DisplayMember = "HOTEN";
        _doctorComboBox.ValueMember = "MANV";

        _technicianComboBox.DataSource = _coordinatorService.GetTechnicians();
        _technicianComboBox.DisplayMember = "HOTEN";
        _technicianComboBox.ValueMember = "MANV";
    }

    private void RefreshPatients()
    {
        _patientsGrid.DataSource = _coordinatorService.GetAllPatients();
    }

    private void AddPatient()
    {
        try
        {
            string[] addressParts = _addressTextBox.Text.Split(',', StringSplitOptions.TrimEntries);
            var patient = new Patient
            {
                TENBN = _nameTextBox.Text.Trim(),
                PHAI = "Nam",
                NGAYSINH = DateTime.Today.AddYears(-30),
                CCCD = _cccdTextBox.Text.Trim(),
                SONHA = addressParts.ElementAtOrDefault(0) ?? string.Empty,
                TENDUONG = addressParts.ElementAtOrDefault(1) ?? string.Empty,
                QUANHUYEN = addressParts.ElementAtOrDefault(2) ?? string.Empty,
                TINHTP = addressParts.ElementAtOrDefault(3) ?? "TP.HCM",
                TIENSUBENH = _medicalHistoryTextBox.Text.Trim(),
                TIENSUBENHGD = _familyHistoryTextBox.Text.Trim(),
                DIUNGTHUOC = _allergyTextBox.Text.Trim()
            };

            _coordinatorService.AddPatient(patient);
            RefreshPatients();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Coordinator", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CreateRecord()
    {
        try
        {
            int patientId = int.Parse(_patientIdTextBox.Text);
            int doctorId = Convert.ToInt32(_doctorComboBox.SelectedValue);
            _coordinatorService.CreateMedicalRecord(patientId, doctorId, string.Empty);
            MessageBox.Show(this, "Medical record created and assigned.", "Coordinator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Coordinator", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AssignTechnician()
    {
        try
        {
            int recordId = int.Parse(_recordIdTextBox.Text);
            int technicianId = Convert.ToInt32(_technicianComboBox.SelectedValue);
            _coordinatorService.AssignTechnician(recordId, _serviceTypeTextBox.Text.Trim(), _serviceDatePicker.Value, technicianId);
            MessageBox.Show(this, "Technician assigned.", "Coordinator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Coordinator", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
