namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class CoordinatorForm : BaseMedicalForm
{
    private readonly UserSession _session;
    private readonly CoordinatorService _coordinatorService;
    private readonly BindingSource _patientsBindingSource = new();
    private readonly TextBox _cccdSearchTextBox = new() { Width = 220 };
    private List<Patient> _allPatients = new();
    private readonly DataGridView _patientsGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly ComboBox _doctorComboBox = new() { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _technicianComboBox = new() { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _patientIdTextBox = new() { Width = 260 };
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
        Text = $"Màn hình điều phối - {_session.FullName}";
        BackColor = Color.FromArgb(241, 244, 249);
        MinimumSize = new Size(1040, 680);

        var notificationsButton = new Button
        {
            Text = "Thông báo",
            AutoSize = true,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(12, 4, 12, 4),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        notificationsButton.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        notificationsButton.Click += (_, _) => new NotificationForm(_session).ShowDialog(this);
        var logoutButton = new Button { Text = "Log out", AutoSize = true };
        logoutButton.Click += (_, _) => Logout();
        topPanel.Controls.Add(addButton);
        topPanel.Controls.Add(refreshButton);
        topPanel.Controls.Add(notificationsButton);
        topPanel.Controls.Add(logoutButton);

        var addButton = new Button
        {
            Text = "Thêm bệnh nhân",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        addButton.FlatAppearance.BorderSize = 0;
        addButton.Click += (_, _) => AddPatient();

        var refreshButton = new Button
        {
            Text = "Làm mới",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(225, 233, 251),
            ForeColor = Color.FromArgb(35, 65, 130),
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        refreshButton.FlatAppearance.BorderSize = 0;
        refreshButton.Click += (_, _) => RefreshPatients();

        var assignDoctorButton = new Button
        {
            Text = "Tạo hồ sơ + phân bác sĩ",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        assignDoctorButton.FlatAppearance.BorderSize = 0;
        assignDoctorButton.Click += (_, _) => CreateRecord();

        var assignTechnicianButton = new Button
        {
            Text = "Phân kỹ thuật viên",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        assignTechnicianButton.FlatAppearance.BorderSize = 0;
        assignTechnicianButton.Click += (_, _) => AssignTechnician();

        _nameTextBox.Dock = DockStyle.Fill;
        _nameTextBox.PlaceholderText = "Nhập họ tên";
        _cccdTextBox.Dock = DockStyle.Fill;
        _cccdTextBox.PlaceholderText = "Nhập CCCD";
        _addressTextBox.Dock = DockStyle.Fill;
        _addressTextBox.PlaceholderText = "Số nhà, Đường, Quận/Huyện, Tỉnh/TP";
        _medicalHistoryTextBox.Dock = DockStyle.Fill;
        _medicalHistoryTextBox.PlaceholderText = "Tiền sử bệnh";
        _familyHistoryTextBox.Dock = DockStyle.Fill;
        _familyHistoryTextBox.PlaceholderText = "Tiền sử bệnh gia đình";
        _allergyTextBox.Dock = DockStyle.Fill;
        _allergyTextBox.PlaceholderText = "Dị ứng thuốc";
        _patientIdTextBox.Dock = DockStyle.Fill;
        _patientIdTextBox.PlaceholderText = "Mã bệnh nhân";
        _recordIdTextBox.Dock = DockStyle.Fill;
        _recordIdTextBox.PlaceholderText = "Mã hồ sơ";
        _serviceTypeTextBox.Dock = DockStyle.Fill;
        _serviceTypeTextBox.PlaceholderText = "Loại dịch vụ";
        _doctorComboBox.Dock = DockStyle.Fill;
        _doctorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _doctorComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        _doctorComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        _technicianComboBox.Dock = DockStyle.Fill;
        _technicianComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _technicianComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        _technicianComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        _serviceDatePicker.Dock = DockStyle.Fill;
        _serviceDatePicker.Format = DateTimePickerFormat.Short;

        _cccdSearchTextBox.Dock = DockStyle.Left;
        _cccdSearchTextBox.PlaceholderText = "Nhập CCCD để lọc";
        _cccdSearchTextBox.TextChanged += (_, _) => ApplyPatientFilter();

        _patientsGrid.DataSource = _patientsBindingSource;
        _patientsGrid.DataBindingComplete += (_, _) => ApplyVietnameseHeaders(_patientsGrid);

        _patientsGrid.BackgroundColor = Color.White;
        _patientsGrid.BorderStyle = BorderStyle.None;
        _patientsGrid.EnableHeadersVisualStyles = false;
        _patientsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
        _patientsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
        _patientsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
        _patientsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 234, 255);
        _patientsGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
        _patientsGrid.RowTemplate.Height = 26;

        var scrollHost = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            AutoScrollMinSize = new Size(0, 760),
            BackColor = Color.FromArgb(241, 244, 249)
        };

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(12, 12, 12, 56),
            BackColor = Color.FromArgb(241, 244, 249)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var headerCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = 78,
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
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 82));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18));

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
            Text = "Bảng điều phối",
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Margin = Padding.Empty
        });

        headerTextPanel.Controls.Add(new Label
        {
            Text = "Quản lý tiếp nhận bệnh nhân và phân công nhân sự",
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(100, 116, 139),
            Margin = new Padding(0, 2, 0, 0)
        });

        headerLayout.Controls.Add(headerTextPanel, 0, 0);
        headerLayout.Controls.Add(notificationsButton, 1, 0);
        headerCard.Controls.Add(headerLayout);

        var intakeCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = 124,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 0, 10)
        };

        var intakeLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 4,
            Margin = Padding.Empty
        };
        intakeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        intakeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        intakeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        intakeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        intakeLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        intakeLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        intakeLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        intakeLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        intakeLayout.Controls.Add(new Label { Text = "Họ tên", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 0);
        intakeLayout.Controls.Add(new Label { Text = "CCCD", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 0);
        intakeLayout.Controls.Add(new Label { Text = "Địa chỉ", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 2, 0);
        intakeLayout.Controls.Add(new Label { Text = "Tiền sử bệnh", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 3, 0);
        intakeLayout.Controls.Add(_nameTextBox, 0, 1);
        intakeLayout.Controls.Add(_cccdTextBox, 1, 1);
        intakeLayout.Controls.Add(_addressTextBox, 2, 1);
        intakeLayout.Controls.Add(_medicalHistoryTextBox, 3, 1);

        intakeLayout.Controls.Add(new Label { Text = "Tiền sử bệnh GĐ", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 2);
        intakeLayout.Controls.Add(new Label { Text = "Dị ứng", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 2);
        intakeLayout.Controls.Add(_familyHistoryTextBox, 0, 3);
        intakeLayout.Controls.Add(_allergyTextBox, 1, 3);
        intakeLayout.Controls.Add(addButton, 2, 3);
        intakeLayout.Controls.Add(refreshButton, 3, 3);

        intakeCard.Controls.Add(intakeLayout);

        var listCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = 300,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 0, 10)
        };

        var listLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Margin = Padding.Empty
        };
        listLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        listLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        listLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        listLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        listLayout.Controls.Add(new Label
        {
            Text = "Danh sách bệnh nhân",
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        }, 0, 0);

        listLayout.Controls.Add(new Label
        {
            Text = "Xem và quản lý bệnh nhân theo CCCD",
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(100, 116, 139)
        }, 0, 1);

        var cccdSearchPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        cccdSearchPanel.Controls.Add(new Label
        {
            Text = "Tìm CCCD:",
            AutoSize = true,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            Margin = new Padding(0, 8, 8, 0)
        });
        cccdSearchPanel.Controls.Add(_cccdSearchTextBox);
        listLayout.Controls.Add(cccdSearchPanel, 0, 2);

        listLayout.Controls.Add(_patientsGrid, 0, 3);
        listCard.Controls.Add(listLayout);

        var assignmentCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = 128,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(10),
            Margin = Padding.Empty
        };

        var assignmentLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 4,
            Margin = Padding.Empty
        };
        assignmentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12));
        assignmentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
        assignmentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 2));
        assignmentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12));
        assignmentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18));
        assignmentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        assignmentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
        assignmentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        assignmentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
        assignmentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        assignmentLayout.Controls.Add(new Label { Text = "Mã bệnh nhân", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 0);
        assignmentLayout.Controls.Add(new Label { Text = "Bác sĩ (gõ để gợi ý)", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 0);
        assignmentLayout.Controls.Add(new Label { Text = "Mã hồ sơ", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 3, 0);
        assignmentLayout.Controls.Add(new Label { Text = "Loại dịch vụ", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 4, 0);
        assignmentLayout.Controls.Add(_patientIdTextBox, 0, 1);
        assignmentLayout.Controls.Add(_doctorComboBox, 1, 1);
        assignmentLayout.Controls.Add(_recordIdTextBox, 3, 1);
        assignmentLayout.Controls.Add(_serviceTypeTextBox, 4, 1);
        assignmentLayout.Controls.Add(assignDoctorButton, 5, 1);

        assignmentLayout.Controls.Add(new Label { Text = "Ngày", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 2);
        assignmentLayout.Controls.Add(new Label { Text = "KTV (gõ để gợi ý)", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 2);
        assignmentLayout.Controls.Add(_serviceDatePicker, 0, 3);
        assignmentLayout.Controls.Add(_technicianComboBox, 1, 3);
        assignmentLayout.Controls.Add(assignTechnicianButton, 5, 3);

        assignmentCard.Controls.Add(assignmentLayout);

        root.Controls.Add(headerCard, 0, 0);
        root.Controls.Add(intakeCard, 0, 1);
        root.Controls.Add(listCard, 0, 2);
        root.Controls.Add(assignmentCard, 0, 3);

        scrollHost.Controls.Add(root);
        Controls.Add(scrollHost);
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
        _allPatients = _coordinatorService.GetAllPatients();
        ApplyPatientFilter();
    }

    private void ApplyPatientFilter()
    {
        string keyword = _cccdSearchTextBox.Text.Trim();

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
            ["USERNAME"] = "Tên Đăng Nhập"
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
            MessageBox.Show(this, ex.Message, "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CreateRecord()
    {
        try
        {
            string patientId = _patientIdTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(patientId))
            {
                MessageBox.Show(this, "Vui lòng nhập mã bệnh nhân.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_doctorComboBox.SelectedValue is null)
            {
                MessageBox.Show(this, "Vui lòng chọn bác sĩ từ danh sách.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string doctorId = _doctorComboBox.SelectedValue?.ToString() ?? string.Empty;
            _coordinatorService.CreateMedicalRecord(patientId, doctorId, string.Empty);
            MessageBox.Show(this, "Đã tạo hồ sơ và phân công bác sĩ.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AssignTechnician()
    {
        try
        {
            if (!int.TryParse(_recordIdTextBox.Text, out int recordId))
            {
                MessageBox.Show(this, "Mã hồ sơ không hợp lệ.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_technicianComboBox.SelectedValue is null)
            {
                MessageBox.Show(this, "Vui lòng chọn kỹ thuật viên từ danh sách.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string technicianId = _technicianComboBox.SelectedValue?.ToString() ?? string.Empty;
            _coordinatorService.AssignTechnician(recordId, _serviceTypeTextBox.Text.Trim(), _serviceDatePicker.Value, technicianId);
            MessageBox.Show(this, "Đã phân công kỹ thuật viên.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
