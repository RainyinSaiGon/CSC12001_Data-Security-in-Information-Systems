namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class DoctorForm : BaseMedicalForm
{
    private readonly UserSession _session;
    private readonly DoctorService _doctorService;
    private readonly BindingSource _patientsBindingSource = new();
    private readonly BindingSource _recordsBindingSource = new();
    private readonly BindingSource _servicesBindingSource = new();
    private readonly BindingSource _prescriptionsBindingSource = new();

    private readonly TextBox _patientCccdSearchTextBox = new() { Width = 220 };
    private List<Patient> _allPatients = new();
    private List<MedicalRecord> _allRecords = new();
    
    private readonly DataGridView _patientsGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly DataGridView _recordsGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly DataGridView _servicesGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly DataGridView _prescriptionsGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };

    private readonly TextBox _patientIdTextBox = new() { Dock = DockStyle.Fill, ReadOnly = true };
    private readonly TextBox _medicalHistoryTextBox = new() { Dock = DockStyle.Fill };
    private readonly TextBox _familyHistoryTextBox = new() { Dock = DockStyle.Fill };
    private readonly TextBox _allergyTextBox = new() { Dock = DockStyle.Fill };

    private readonly TextBox _recordIdTextBox = new() { Dock = DockStyle.Fill, ReadOnly = true };
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

        _patientsGrid.DataSource = _patientsBindingSource;
        _recordsGrid.DataSource = _recordsBindingSource;
        _servicesGrid.DataSource = _servicesBindingSource;
        _prescriptionsGrid.DataSource = _prescriptionsBindingSource;

        _patientsGrid.DataBindingComplete += (_, _) => ApplyVietnameseHeaders(_patientsGrid);
        _recordsGrid.DataBindingComplete += (_, _) => ApplyVietnameseHeaders(_recordsGrid);
        _servicesGrid.DataBindingComplete += (_, _) => ApplyVietnameseHeaders(_servicesGrid);
        _prescriptionsGrid.DataBindingComplete += (_, _) => ApplyVietnameseHeaders(_prescriptionsGrid);

        _patientsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _patientsGrid.CellClick += PatientsGrid_CellClick;
        
        _recordsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _recordsGrid.CellClick += RecordsGrid_CellClick;
        
        _servicesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _servicesGrid.CellClick += ServicesGrid_CellClick;
        
        _prescriptionsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _prescriptionsGrid.CellClick += PrescriptionsGrid_CellClick;
        
        ConfigureGrid(_patientsGrid);
        ConfigureGrid(_recordsGrid);
        ConfigureGrid(_servicesGrid);
        ConfigureGrid(_prescriptionsGrid);

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

        var scrollHost = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            AutoScrollMinSize = new Size(0, 900),
            BackColor = Color.FromArgb(241, 244, 249)
        };

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 7,
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
            Text = "Bảng bác sĩ",
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Margin = Padding.Empty
        });

        headerTextPanel.Controls.Add(new Label
        {
            Text = $"Xin chào, {_session.FullName}. Quản lý hồ sơ, chỉ định dịch vụ và toa thuốc",
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(100, 116, 139),
            Margin = new Padding(0, 2, 0, 0)
        });

        headerLayout.Controls.Add(headerTextPanel, 0, 0);
        headerLayout.Controls.Add(notificationsButton, 1, 0);
        headerCard.Controls.Add(headerLayout);

        Font boldFont = new Font("Segoe UI", 8, FontStyle.Bold);

        var patientsCard = CreateGridCard(
            "Danh sách bệnh nhân",
            "Tìm bệnh nhân theo CCCD",
            _patientCccdSearchTextBox,
            _patientsGrid,
            174);
            
        var patientInfoCard = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(10), Margin = new Padding(0, 0, 0, 10) };
        var pLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 2, Margin = Padding.Empty };
        pLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        pLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        pLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        pLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        pLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        pLayout.Controls.Add(new Label { Text = "Mã Bệnh nhân", AutoSize = true, Font = boldFont }, 0, 0);
        pLayout.Controls.Add(new Label { Text = "Tiền sử bệnh", AutoSize = true, Font = boldFont }, 1, 0);
        pLayout.Controls.Add(new Label { Text = "Tiền sử GĐ", AutoSize = true, Font = boldFont }, 2, 0);
        pLayout.Controls.Add(new Label { Text = "Dị ứng", AutoSize = true, Font = boldFont }, 3, 0);
        pLayout.Controls.Add(_patientIdTextBox, 0, 1);
        pLayout.Controls.Add(_medicalHistoryTextBox, 1, 1);
        pLayout.Controls.Add(_familyHistoryTextBox, 2, 1);
        pLayout.Controls.Add(_allergyTextBox, 3, 1);
        var updatePatientBtn = CreateButton("Lưu BN", Color.FromArgb(37, 99, 235));
        updatePatientBtn.Click += (_, _) => UpdatePatient();
        pLayout.Controls.Add(updatePatientBtn, 4, 1);
        patientInfoCard.Controls.Add(pLayout);

        var recordsCard = CreateSimpleGridCard(
            "Hồ sơ bệnh án (Click chọn Bệnh nhân ở trên để xem HSBA)",
            _recordsGrid,
            174);
            
        var recordInfoCard = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(10), Margin = new Padding(0, 0, 0, 10) };
        var rLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 2, Margin = Padding.Empty };
        rLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        rLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        rLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        rLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        rLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        rLayout.Controls.Add(new Label { Text = "Mã HSBA", AutoSize = true, Font = boldFont }, 0, 0);
        rLayout.Controls.Add(new Label { Text = "Chẩn đoán", AutoSize = true, Font = boldFont }, 1, 0);
        rLayout.Controls.Add(new Label { Text = "Điều trị", AutoSize = true, Font = boldFont }, 2, 0);
        rLayout.Controls.Add(new Label { Text = "Kết luận", AutoSize = true, Font = boldFont }, 3, 0);
        rLayout.Controls.Add(_recordIdTextBox, 0, 1);
        rLayout.Controls.Add(_diagnosisTextBox, 1, 1);
        rLayout.Controls.Add(_treatmentTextBox, 2, 1);
        rLayout.Controls.Add(_conclusionTextBox, 3, 1);
        var updateRecordBtn = CreateButton("Lưu HSBA", Color.FromArgb(37, 99, 235));
        updateRecordBtn.Click += (_, _) => UpdateRecord();
        rLayout.Controls.Add(updateRecordBtn, 4, 1);
        recordInfoCard.Controls.Add(rLayout);
        
        var servicesCard = new Panel { Dock = DockStyle.Top, Height = 220, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(10), Margin = new Padding(0, 0, 0, 10) };
        var sLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = Padding.Empty };
        var sInputPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 2, Margin = Padding.Empty };
        sInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        sInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        sInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        sInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        sInputPanel.Controls.Add(new Label { Text = "Loại dịch vụ (Chỉ định cận lâm sàng)", AutoSize = true, Font = boldFont }, 0, 0);
        sInputPanel.Controls.Add(new Label { Text = "Ngày dịch vụ", AutoSize = true, Font = boldFont }, 1, 0);
        sInputPanel.Controls.Add(_serviceTypeTextBox, 0, 1);
        sInputPanel.Controls.Add(_serviceDatePicker, 1, 1);
        var addServiceBtn = CreateButton("Chỉ định", Color.FromArgb(37, 99, 235));
        addServiceBtn.Click += (_, _) => AddService();
        var delServiceBtn = CreateButton("Xóa DV", Color.FromArgb(220, 38, 38));
        delServiceBtn.Click += (_, _) => DeleteService();
        sInputPanel.Controls.Add(addServiceBtn, 2, 1);
        sInputPanel.Controls.Add(delServiceBtn, 3, 1);
        sLayout.Controls.Add(sInputPanel, 0, 0);
        sLayout.Controls.Add(_servicesGrid, 0, 1);
        servicesCard.Controls.Add(sLayout);

        var prescriptionsCard = new Panel { Dock = DockStyle.Top, Height = 220, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(10), Margin = new Padding(0, 0, 0, 10) };
        var p2Layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = Padding.Empty };
        var pInputPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 2, Margin = Padding.Empty };
        pInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        pInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        pInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        pInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
        pInputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
        pInputPanel.Controls.Add(new Label { Text = "Tên thuốc", AutoSize = true, Font = boldFont }, 0, 0);
        pInputPanel.Controls.Add(new Label { Text = "Liều dùng", AutoSize = true, Font = boldFont }, 1, 0);
        pInputPanel.Controls.Add(new Label { Text = "Ngày kê đơn", AutoSize = true, Font = boldFont }, 2, 0);
        pInputPanel.Controls.Add(_prescriptionNameTextBox, 0, 1);
        pInputPanel.Controls.Add(_prescriptionDoseTextBox, 1, 1);
        pInputPanel.Controls.Add(_prescriptionDatePicker, 2, 1);
        var savePrescriptionBtn = CreateButton("Lưu Toa", Color.FromArgb(37, 99, 235));
        savePrescriptionBtn.Click += (_, _) => SavePrescription();
        var delPrescriptionBtn = CreateButton("Xóa", Color.FromArgb(220, 38, 38));
        delPrescriptionBtn.Click += (_, _) => DeletePrescription();
        pInputPanel.Controls.Add(savePrescriptionBtn, 3, 1);
        pInputPanel.Controls.Add(delPrescriptionBtn, 4, 1);
        p2Layout.Controls.Add(pInputPanel, 0, 0);
        p2Layout.Controls.Add(_prescriptionsGrid, 0, 1);
        prescriptionsCard.Controls.Add(p2Layout);

        root.Controls.Add(headerCard, 0, 0);
        root.Controls.Add(patientsCard, 0, 1);
        root.Controls.Add(patientInfoCard, 0, 2);
        root.Controls.Add(recordsCard, 0, 3);
        root.Controls.Add(recordInfoCard, 0, 4);
        root.Controls.Add(servicesCard, 0, 5);
        root.Controls.Add(prescriptionsCard, 0, 6);

        scrollHost.Controls.Add(root);
        Controls.Add(scrollHost);
    }
    
    private Button CreateButton(string text, Color backColor)
    {
        var btn = new Button
        {
            Text = text,
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = backColor,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        btn.FlatAppearance.BorderSize = 0;
        return btn;
    }

    private static Panel CreateSimpleGridCard(string title, DataGridView grid, int height)
    {
        var card = new Panel { Dock = DockStyle.Top, Height = height, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(10), Margin = new Padding(0, 0, 0, 10) };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = Padding.Empty };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(new Label { Text = title, AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42) }, 0, 0);
        layout.Controls.Add(grid, 0, 1);
        card.Controls.Add(layout);
        return card;
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
        string? doctorId = _session.StaffId;
        if (string.IsNullOrWhiteSpace(doctorId))
        {
            return;
        }

        _allPatients = _doctorService.GetAssignedPatients(doctorId);
        _allRecords = _doctorService.GetAssignedMedicalRecords(doctorId);
        ApplyPatientFilter();
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
            ["MAKHOA"] = "Mã Khoa",
            ["LOAIDV"] = "Loại Dịch Vụ",
            ["NGAYDV"] = "Ngày Thực Hiện",
            ["KETQUA"] = "Kết Quả",
            ["MAKTV"] = "Mã KTV",
            ["TENTHUOC"] = "Tên Thuốc",
            ["LIEUDUNG"] = "Liều Dùng",
            ["NGAYDT"] = "Ngày Kê Đơn"
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

    private void PatientsGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.RowIndex < _patientsGrid.Rows.Count)
        {
            var row = _patientsGrid.Rows[e.RowIndex];
            string patientId = row.Cells["MABN"]?.Value?.ToString() ?? string.Empty;
            _patientIdTextBox.Text = patientId;
            _medicalHistoryTextBox.Text = row.Cells["TIENSUBENH"]?.Value?.ToString() ?? string.Empty;
            _familyHistoryTextBox.Text = row.Cells["TIENSUBENHGD"]?.Value?.ToString() ?? string.Empty;
            _allergyTextBox.Text = row.Cells["DIUNGTHUOC"]?.Value?.ToString() ?? string.Empty;

            _recordsBindingSource.DataSource = _allRecords.Where(r => r.MABN == patientId).ToList();
            _servicesBindingSource.DataSource = null;
            _prescriptionsBindingSource.DataSource = null;
        }
    }

    private void RecordsGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.RowIndex < _recordsGrid.Rows.Count)
        {
            var row = _recordsGrid.Rows[e.RowIndex];
            string recordIdStr = row.Cells["MAHSBA"]?.Value?.ToString() ?? string.Empty;
            _recordIdTextBox.Text = recordIdStr;
            _diagnosisTextBox.Text = row.Cells["CHANDOAN"]?.Value?.ToString() ?? string.Empty;
            _treatmentTextBox.Text = row.Cells["DIEUTRI"]?.Value?.ToString() ?? string.Empty;
            _conclusionTextBox.Text = row.Cells["KETLUAN"]?.Value?.ToString() ?? string.Empty;

            LoadServicesAndPrescriptions(recordIdStr);
        }
    }

    private void ServicesGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.RowIndex < _servicesGrid.Rows.Count)
        {
            var row = _servicesGrid.Rows[e.RowIndex];
            _serviceTypeTextBox.Text = row.Cells["LOAIDV"]?.Value?.ToString() ?? string.Empty;
            if (DateTime.TryParse(row.Cells["NGAYDV"]?.Value?.ToString(), out var date))
                _serviceDatePicker.Value = date;
        }
    }

    private void PrescriptionsGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.RowIndex < _prescriptionsGrid.Rows.Count)
        {
            var row = _prescriptionsGrid.Rows[e.RowIndex];
            _prescriptionNameTextBox.Text = row.Cells["TENTHUOC"]?.Value?.ToString() ?? string.Empty;
            _prescriptionDoseTextBox.Text = row.Cells["LIEUDUNG"]?.Value?.ToString() ?? string.Empty;
            if (DateTime.TryParse(row.Cells["NGAYDT"]?.Value?.ToString(), out var date))
                _prescriptionDatePicker.Value = date;
        }
    }

    private void LoadServicesAndPrescriptions(string recordIdStr)
    {
        if (!int.TryParse(recordIdStr, out int recordId))
        {
            _servicesBindingSource.DataSource = null;
            _prescriptionsBindingSource.DataSource = null;
            return;
        }
        try
        {
            _servicesBindingSource.DataSource = _doctorService.GetDiagnosticServices(recordId);
            _prescriptionsBindingSource.DataSource = _doctorService.GetPrescriptions(recordId);
        }
        catch { /* Bỏ qua nếu lỗi */ }
    }

    private void UpdatePatient()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_patientIdTextBox.Text)) return;
            var p = new Patient
            {
                MABN = _patientIdTextBox.Text,
                TIENSUBENH = _medicalHistoryTextBox.Text.Trim(),
                TIENSUBENHGD = _familyHistoryTextBox.Text.Trim(),
                DIUNGTHUOC = _allergyTextBox.Text.Trim()
            };
            _doctorService.UpdatePatientHistory(p);
            MessageBox.Show(this, "Đã cập nhật thông tin bệnh nhân.", "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshData();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void UpdateRecord()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_recordIdTextBox.Text)) return;
            var record = new MedicalRecord
            {
                MAHSBA = int.Parse(_recordIdTextBox.Text),
                CHANDOAN = _diagnosisTextBox.Text.Trim(),
                DIEUTRI = _treatmentTextBox.Text.Trim(),
                KETLUAN = _conclusionTextBox.Text.Trim()
            };

            _doctorService.UpdateMedicalRecord(record);
            MessageBox.Show(this, "Đã cập nhật thông tin hồ sơ bệnh án.", "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (string.IsNullOrWhiteSpace(_recordIdTextBox.Text)) return;
            var service = new DiagnosticService
            {
                MAHSBA = int.Parse(_recordIdTextBox.Text),
                LOAIDV = _serviceTypeTextBox.Text.Trim(),
                NGAYDV = _serviceDatePicker.Value
            };

            _doctorService.OrderDiagnosticService(service);
            MessageBox.Show(this, "Đã chỉ định dịch vụ.", "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadServicesAndPrescriptions(_recordIdTextBox.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteService()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_recordIdTextBox.Text)) return;
            var service = new DiagnosticService
            {
                MAHSBA = int.Parse(_recordIdTextBox.Text),
                LOAIDV = _serviceTypeTextBox.Text.Trim(),
                NGAYDV = _serviceDatePicker.Value
            };
            _doctorService.DeleteDiagnosticService(service);
            MessageBox.Show(this, "Đã xóa dịch vụ.", "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadServicesAndPrescriptions(_recordIdTextBox.Text);
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
            if (string.IsNullOrWhiteSpace(_recordIdTextBox.Text)) return;
            var prescription = new Prescription
            {
                MAHSBA = int.Parse(_recordIdTextBox.Text),
                TENTHUOC = _prescriptionNameTextBox.Text.Trim(),
                LIEUDUNG = _prescriptionDoseTextBox.Text.Trim(),
                NGAYDT = _prescriptionDatePicker.Value
            };

            _doctorService.UpdatePrescription(prescription);
            MessageBox.Show(this, "Đã lưu/cập nhật toa thuốc.", "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadServicesAndPrescriptions(_recordIdTextBox.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeletePrescription()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_recordIdTextBox.Text)) return;
            var prescription = new Prescription
            {
                MAHSBA = int.Parse(_recordIdTextBox.Text),
                TENTHUOC = _prescriptionNameTextBox.Text.Trim(),
                NGAYDT = _prescriptionDatePicker.Value
            };
            _doctorService.DeletePrescription(prescription);
            MessageBox.Show(this, "Đã xóa toa thuốc.", "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadServicesAndPrescriptions(_recordIdTextBox.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Bác sĩ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
