namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class DoctorForm : BaseMedicalForm
{
    private readonly UserSession _session;
    private readonly DoctorService _doctorService;
    private readonly BindingSource _patientsBindingSource = new();
    private readonly BindingSource _recordsBindingSource = new();
    private readonly TextBox _patientCccdSearchTextBox = new() { Width = 220 };
    private readonly TextBox _recordMabnSearchTextBox = new() { Width = 220 };
    private List<Patient> _allPatients = new();
    private List<MedicalRecord> _allRecords = new();
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
        Text = $"Bảng bác sĩ - {_session.FullName}";
        BackColor = Color.FromArgb(241, 244, 249);
        MinimumSize = new Size(1040, 700);

        _recordIdTextBox.Dock = DockStyle.Fill;
        _recordIdTextBox.PlaceholderText = "Mã hồ sơ";
        _diagnosisTextBox.Dock = DockStyle.Fill;
        _diagnosisTextBox.PlaceholderText = "Chẩn đoán";
        _treatmentTextBox.Dock = DockStyle.Fill;
        _treatmentTextBox.PlaceholderText = "Phác đồ điều trị";
        _conclusionTextBox.Dock = DockStyle.Fill;
        _conclusionTextBox.PlaceholderText = "Kết luận";
        _serviceTypeTextBox.Dock = DockStyle.Fill;
        _serviceTypeTextBox.PlaceholderText = "Loại dịch vụ";
        _serviceDatePicker.Dock = DockStyle.Fill;
        _serviceDatePicker.Format = DateTimePickerFormat.Short;
        _prescriptionNameTextBox.Dock = DockStyle.Fill;
        _prescriptionNameTextBox.PlaceholderText = "Tên thuốc";
        _prescriptionDoseTextBox.Dock = DockStyle.Fill;
        _prescriptionDoseTextBox.PlaceholderText = "Liều dùng";
        _prescriptionDatePicker.Dock = DockStyle.Fill;
        _prescriptionDatePicker.Format = DateTimePickerFormat.Short;

        _patientCccdSearchTextBox.PlaceholderText = "Tìm theo CCCD";
        _patientCccdSearchTextBox.TextChanged += (_, _) => ApplyPatientFilter();
        _recordMabnSearchTextBox.PlaceholderText = "Tìm theo MABN";
        _recordMabnSearchTextBox.TextChanged += (_, _) => ApplyRecordFilter();

        _patientsGrid.DataSource = _patientsBindingSource;
        _recordsGrid.DataSource = _recordsBindingSource;
        _patientsGrid.DataBindingComplete += (_, _) => ApplyVietnameseHeaders(_patientsGrid);
        _recordsGrid.DataBindingComplete += (_, _) => ApplyVietnameseHeaders(_recordsGrid);

        var updateRecordButton = new Button
        {
            Text = "Cập nhật hồ sơ",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        updateRecordButton.FlatAppearance.BorderSize = 0;
        updateRecordButton.Click += (_, _) => UpdateRecord();

        var addServiceButton = new Button
        {
            Text = "Chỉ định dịch vụ",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        addServiceButton.FlatAppearance.BorderSize = 0;
        addServiceButton.Click += (_, _) => AddService();

        var savePrescriptionButton = new Button
        {
            Text = "Lưu toa thuốc",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        savePrescriptionButton.FlatAppearance.BorderSize = 0;
        savePrescriptionButton.Click += (_, _) => SavePrescription();

        var notificationsButton = new Button
        {
            Text = "Thông báo",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(248, 250, 252),
            Font = new Font("Segoe UI", 8f)
        };
        notificationsButton.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        notificationsButton.Click += (_, _) => new NotificationForm(_session).ShowDialog(this);

        ConfigureGrid(_patientsGrid);
        ConfigureGrid(_recordsGrid);

        var scrollHost = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            AutoScrollMinSize = new Size(0, 860),
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
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var actionCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = 176,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 0, 10)
        };

        var actionLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 7,
            Margin = Padding.Empty
        };
        actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18));
        actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
        actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
        actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
        actionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));

        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        actionLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));

        actionLayout.Controls.Add(new Label { Text = "Record ID", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 0);
        actionLayout.Controls.Add(new Label { Text = "Diagnosis", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 0);
        actionLayout.Controls.Add(new Label { Text = "Treatment", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 2, 0);
        actionLayout.Controls.Add(new Label { Text = "Conclusion", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 3, 0);
        actionLayout.Controls.Add(_recordIdTextBox, 0, 1);
        actionLayout.Controls.Add(_diagnosisTextBox, 1, 1);
        actionLayout.Controls.Add(_treatmentTextBox, 2, 1);
        actionLayout.Controls.Add(_conclusionTextBox, 3, 1);
        actionLayout.Controls.Add(updateRecordButton, 4, 1);

        actionLayout.Controls.Add(new Label { Text = "Service type", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 2);
        actionLayout.Controls.Add(new Label { Text = "Service date", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 2);
        actionLayout.Controls.Add(_serviceTypeTextBox, 0, 3);
        actionLayout.Controls.Add(_serviceDatePicker, 1, 3);
        actionLayout.Controls.Add(addServiceButton, 4, 3);

        actionLayout.Controls.Add(new Label { Text = "Drug", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 4);
        actionLayout.Controls.Add(new Label { Text = "Dose", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 4);
        actionLayout.Controls.Add(new Label { Text = "Date", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 2, 4);
        actionLayout.Controls.Add(_prescriptionNameTextBox, 0, 5);
        actionLayout.Controls.Add(_prescriptionDoseTextBox, 1, 5);
        actionLayout.Controls.Add(_prescriptionDatePicker, 2, 5);
        actionLayout.Controls.Add(savePrescriptionButton, 4, 5);
        actionLayout.Controls.Add(notificationsButton, 4, 6);

        actionCard.Controls.Add(actionLayout);

        var patientsCard = CreateGridCard(
            "Danh sách bệnh nhân",
            "Tìm bệnh nhân theo CCCD",
            _patientCccdSearchTextBox,
            _patientsGrid,
            174);

        var recordsCard = CreateGridCard(
            "Hồ sơ bệnh án",
            "Tìm hồ sơ theo MABN",
            _recordMabnSearchTextBox,
            _recordsGrid,
            174);

        root.Controls.Add(actionCard, 0, 0);
        root.Controls.Add(patientsCard, 0, 1);
        root.Controls.Add(recordsCard, 0, 2);

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

    private static Panel CreateGridCard(string title, string subtitle, TextBox searchBox, DataGridView grid, int height)
    {
        var card = new Panel
        {
            Dock = DockStyle.Top,
            Height = height,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 0, 10)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Margin = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label
        {
            Text = title,
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        }, 0, 0);

        layout.Controls.Add(new Label
        {
            Text = subtitle,
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(100, 116, 139)
        }, 0, 1);

        var searchPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        searchPanel.Controls.Add(new Label
        {
            Text = "Tìm kiếm:",
            AutoSize = true,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            Margin = new Padding(0, 8, 8, 0)
        });
        searchPanel.Controls.Add(searchBox);
        layout.Controls.Add(searchPanel, 0, 2);

        layout.Controls.Add(grid, 0, 3);
        card.Controls.Add(layout);

        return card;
    }

    private void RefreshData()
    {
        if (string.IsNullOrWhiteSpace(_session.StaffId))
        {
            return;
        }

        string doctorId = _session.StaffId.Value.ToString();
        _allPatients = _doctorService.GetAssignedPatients(doctorId);
        _allRecords = _doctorService.GetAssignedMedicalRecords(doctorId);
        ApplyPatientFilter();
        ApplyRecordFilter();
    }

    private void ApplyPatientFilter()
    {
        string keyword = _patientCccdSearchTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            _patientsBindingSource.DataSource = _allPatients;
            return;
        }

        _patientsBindingSource.DataSource = _allPatients
            .Where(p => (p.CCCD ?? string.Empty).Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static void ApplyVietnameseHeaders(DataGridView grid)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["MABN"] = "Mã Bệnh Nhân",
            ["TENBN"] = "Tên Bệnh Nhân",
            ["PHAI"] = "Giới Tính",
            ["NGAYSINH"] = "Ngày Sinh",
            ["CCCD"] = "CCCD",
            ["SONHA"] = "Số Nhà",
            ["TENDUONG"] = "Tên Đường",
            ["QUANHUYEN"] = "Quận/Huyện",
            ["TINHTP"] = "Tỉnh/TP",
            ["TIENSUBENH"] = "Tiền Sử Bệnh",
            ["TIENSUBENHGD"] = "Tiền Sử Bệnh GĐ",
            ["DIUNGTHUOC"] = "Dị Ứng Thuốc",
            ["USERNAME"] = "Tên Đăng Nhập",
            ["MAHSBA"] = "Mã Hồ Sơ",
            ["NGAY"] = "Ngày Khám",
            ["CHANDOAN"] = "Chẩn Đoán",
            ["DIEUTRI"] = "Điều Trị",
            ["KETLUAN"] = "Kết Luận",
            ["MABS"] = "Mã Bác Sĩ",
            ["MAKHOA"] = "Mã Khoa"
        };

        foreach (DataGridViewColumn column in grid.Columns)
        {
            string key = string.IsNullOrWhiteSpace(column.DataPropertyName) ? column.Name : column.DataPropertyName;
            if (headers.TryGetValue(key, out string? text))
            {
                column.HeaderText = text;
            }
        }
    }

    private void ApplyRecordFilter()
    {
        string keyword = _recordMabnSearchTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            _recordsBindingSource.DataSource = _allRecords;
            return;
        }

        _recordsBindingSource.DataSource = _allRecords
            .Where(r => r.MABN.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
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
            MessageBox.Show(this, ex.Message, "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show(this, ex.Message, "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show(this, "Đã lưu toa thuốc.", "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
