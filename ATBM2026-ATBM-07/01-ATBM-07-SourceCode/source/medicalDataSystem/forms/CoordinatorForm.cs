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
    private readonly ComboBox _recordIdComboBox = new() { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _serviceComboBox = new() { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
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
            AutoSize = true,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(225, 233, 251),
            ForeColor = Color.FromArgb(35, 65, 130),
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        refreshButton.FlatAppearance.BorderSize = 0;
        refreshButton.Click += (_, _) => RefreshPatients();

        var editButton = new Button
        {
            Text = "Cập nhật",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(245, 158, 11),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        editButton.FlatAppearance.BorderSize = 0;
        editButton.Click += (_, _) => EditPatient();

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
        _recordIdComboBox.Dock = DockStyle.Fill;
        _serviceComboBox.Dock = DockStyle.Fill;
        _doctorComboBox.Dock = DockStyle.Fill;
        _doctorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _doctorComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        _doctorComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        _technicianComboBox.Dock = DockStyle.Fill;
        _technicianComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _technicianComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        _technicianComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        _serviceDatePicker.Dock = DockStyle.Fill;
        _serviceDatePicker.Format = DateTimePickerFormat.Short;
        _serviceDatePicker.Enabled = false;

        _serviceComboBox.SelectedIndexChanged += (_, _) =>
        {
            if (_serviceComboBox.SelectedItem != null)
            {
                var propInfo = _serviceComboBox.SelectedItem.GetType().GetProperty("NGAYDV");
                if (propInfo != null)
                {
                    _serviceDatePicker.Value = (DateTime)propInfo.GetValue(_serviceComboBox.SelectedItem, null)!;
                }
            }
        };

        _cccdSearchTextBox.Dock = DockStyle.Left;
        _cccdSearchTextBox.PlaceholderText = "Nhập CCCD để lọc";
        _cccdSearchTextBox.TextChanged += (_, _) => ApplyPatientFilter();

        _patientsGrid.DataSource = _patientsBindingSource;
        _patientsGrid.DataBindingComplete += (_, _) => ApplyVietnameseHeaders(_patientsGrid);
        _patientsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _patientsGrid.CellClick += PatientsGrid_CellClick;

        _recordIdComboBox.SelectedIndexChanged += (_, _) => LoadUnassignedServices();

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
        intakeLayout.Controls.Add(editButton, 3, 3);

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
        refreshButton.Margin = new Padding(8, 0, 0, 0);
        cccdSearchPanel.Controls.Add(refreshButton);
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
        assignmentLayout.Controls.Add(_recordIdComboBox, 3, 1);
        assignmentLayout.Controls.Add(_serviceComboBox, 4, 1);
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

        var filteredList = string.IsNullOrWhiteSpace(keyword)
            ? _allPatients
            : _allPatients.Where(p => (p.CCCD ?? string.Empty).Trim().Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

        _patientsGrid.DataSource = null; // Bắt buộc DataGrid gỡ kết nối cũ
        _patientsBindingSource.DataSource = new System.ComponentModel.BindingList<Patient>(filteredList);
        _patientsGrid.DataSource = _patientsBindingSource; // Gắn lại kết nối mới
        ApplyVietnameseHeaders(_patientsGrid);
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
            MessageBox.Show(this, "Đã thêm bệnh nhân thành công.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearPatientInputs();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ClearPatientInputs()
    {
        _patientIdTextBox.Clear();
        _nameTextBox.Clear();
        _cccdTextBox.Clear();
        _addressTextBox.Clear();
        _medicalHistoryTextBox.Clear();
        _familyHistoryTextBox.Clear();
        _allergyTextBox.Clear();
    }

    private void EditPatient()
    {
        try
        {
            string patientId = _patientIdTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(patientId))
            {
                MessageBox.Show(this, "Vui lòng click chọn một bệnh nhân ở danh sách bên dưới để cập nhật.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] addressParts = _addressTextBox.Text.Split(',', StringSplitOptions.TrimEntries);
            var patient = new Patient
            {
                MABN = patientId,
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

            _coordinatorService.EditPatient(patient);
            RefreshPatients();
            MessageBox.Show(this, "Đã cập nhật bệnh nhân thành công.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearPatientInputs();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PatientsGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.RowIndex < _patientsGrid.Rows.Count)
        {
            var row = _patientsGrid.Rows[e.RowIndex];
            _patientIdTextBox.Text = row.Cells["MABN"]?.Value?.ToString() ?? string.Empty;
            
            _nameTextBox.Text = row.Cells["TENBN"]?.Value?.ToString() ?? string.Empty;
            _cccdTextBox.Text = row.Cells["CCCD"]?.Value?.ToString() ?? string.Empty;
            
            string sonha = row.Cells["SONHA"]?.Value?.ToString() ?? string.Empty;
            string tenduong = row.Cells["TENDUONG"]?.Value?.ToString() ?? string.Empty;
            string quanhuyen = row.Cells["QUANHUYEN"]?.Value?.ToString() ?? string.Empty;
            string tinhtp = row.Cells["TINHTP"]?.Value?.ToString() ?? string.Empty;
            string[] parts = new[] { sonha, tenduong, quanhuyen, tinhtp }.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            
            _addressTextBox.Text = string.Join(", ", parts);
            _medicalHistoryTextBox.Text = row.Cells["TIENSUBENH"]?.Value?.ToString() ?? string.Empty;
            _familyHistoryTextBox.Text = row.Cells["TIENSUBENHGD"]?.Value?.ToString() ?? string.Empty;
            _allergyTextBox.Text = row.Cells["DIUNGTHUOC"]?.Value?.ToString() ?? string.Empty;

            LoadPatientRecords(_patientIdTextBox.Text);
        }
    }

    private void LoadPatientRecords(string patientId)
    {
        if (string.IsNullOrWhiteSpace(patientId))
        {
            _recordIdComboBox.DataSource = null;
            return;
        }
        try
        {
            _recordIdComboBox.DataSource = _coordinatorService.GetMedicalRecordsByPatient(patientId);
        }
        catch { /* Bỏ qua nếu lỗi kết nối hoặc phân quyền */ }
    }

    private void LoadUnassignedServices()
    {
        if (_recordIdComboBox.SelectedItem is null || !int.TryParse(_recordIdComboBox.SelectedItem.ToString(), out int recordId))
        {
            _serviceComboBox.DataSource = null;
            return;
        }
        try
        {
            var services = _coordinatorService.GetUnassignedServices(recordId);
            _serviceComboBox.DataSource = services.Select(s => new { s.LOAIDV, s.NGAYDV, Display = $"{s.LOAIDV} ({s.NGAYDV:dd/MM/yyyy})" }).ToList();
            _serviceComboBox.DisplayMember = "Display";
            _serviceComboBox.ValueMember = "LOAIDV";
        }
        catch { /* Bỏ qua nếu lỗi */ }
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
            
            // Tự động tìm MÃ HSBA vừa tạo để hiển thị cho UI
            int newRecordId = 0;
            try
            {
                var oracleSvc = new OracleConnectionService(_session.ConnectionString);
                newRecordId = oracleSvc.Execute(conn =>
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT MAX(MAHSBA) FROM HOSPITAL_ADMIN.HSBA WHERE MABN = :mabn";
                    var param = cmd.CreateParameter();
                    param.ParameterName = "mabn";
                    param.Value = patientId;
                    cmd.Parameters.Add(param);
                    var res = cmd.ExecuteScalar();
                    return res != null && res != DBNull.Value ? Convert.ToInt32(res) : 0;
                });
            }
            catch { /* Bỏ qua nếu lỗi */ }

            if (newRecordId > 0)
            {
                LoadPatientRecords(patientId);
                _recordIdComboBox.SelectedItem = newRecordId;
                MessageBox.Show(this, $"Đã tạo HSBA thành công!\n👉 Mã hồ sơ (MÃ HSBA) vừa tạo là: {newRecordId}\n\nLưu ý quy trình:\n1. Bác sĩ sẽ tiến hành khám bệnh và chỉ định Dịch vụ.\n2. Sau khi có Dịch vụ, Điều phối viên mới lấy mã này và Loại Dịch vụ để phân công Kỹ thuật viên.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, "Đã tạo HSBA thành công!\n\nLưu ý quy trình:\n1. Bác sĩ (Role Bác sĩ) sẽ tiến hành khám bệnh và chỉ định Dịch vụ.\n2. Sau khi có Dịch vụ, Điều phối viên mới lấy Mã HSBA và Loại Dịch vụ đó điền vào form này để phân công Kỹ thuật viên.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
            if (_recordIdComboBox.SelectedItem is null || !int.TryParse(_recordIdComboBox.SelectedItem.ToString(), out int recordId))
            {
                MessageBox.Show(this, "Vui lòng chọn mã hồ sơ hợp lệ từ danh sách.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_technicianComboBox.SelectedValue is null)
            {
                MessageBox.Show(this, "Vui lòng chọn kỹ thuật viên từ danh sách.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_serviceComboBox.SelectedValue is null)
            {
                MessageBox.Show(this, "Vui lòng chọn một dịch vụ cần phân công từ danh sách.\n\nNếu danh sách trống, nghĩa là hồ sơ này chưa có dịch vụ nào đang chờ phân công.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string technicianId = _technicianComboBox.SelectedValue?.ToString() ?? string.Empty;
            string serviceType = _serviceComboBox.SelectedValue.ToString() ?? string.Empty;
            DateTime serviceDate = DateTime.Today;
            if (_serviceComboBox.SelectedItem != null)
            {
                var propInfo = _serviceComboBox.SelectedItem.GetType().GetProperty("NGAYDV");
                if (propInfo != null)
                {
                    serviceDate = (DateTime)propInfo.GetValue(_serviceComboBox.SelectedItem, null)!;
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"[UI] Đang gọi AssignTechnician - MAHSBA: {recordId}, LOAIDV: {serviceType}, NGAYDV: {serviceDate:yyyy-MM-dd}, MAKTV: {technicianId}");
            
            _coordinatorService.AssignTechnician(recordId, serviceType, serviceDate, technicianId);
            MessageBox.Show(this, "Đã phân công kỹ thuật viên thành công.", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadUnassignedServices();
        }
        catch (Exception ex)
        {
            string errorDetails = $"Lỗi: {ex.Message}\nStack Trace: {ex.StackTrace}";
            System.Diagnostics.Debug.WriteLine($"[UI Error] {errorDetails}");
            
            if (ex.Message.Contains("ORA-12537") || ex.Message.Contains("connection closed"))
            {
                MessageBox.Show(this, $"Lỗi ORA-12537: Đã mất kết nối tới Database.\n\nChi tiết:\n{ex.Message}\n\n👉 Giải pháp: Tắt ứng dụng, mở tab Output (Debug) trong Visual Studio để xem dòng log cuối cùng trước khi crash, sau đó gửi cho mình nhé.", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(this, $"{ex.Message}\n\n(Xem thêm chi tiết trong Output Debug)", "Điều phối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
