namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class TechnicianForm : BaseMedicalForm
{
    private readonly UserSession _session;
    private readonly TechnicianService _technicianService;
    private readonly BindingSource _servicesBindingSource = new();
    private readonly ComboBox _servicesSearchFieldComboBox = new() { Width = 170, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _servicesSearchTextBox = new() { Width = 260 };
    private List<DiagnosticService> _allServices = new();
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
        Text = $"Màn hình kỹ thuật viên - {_session.FullName}";
        BackColor = Color.FromArgb(241, 244, 249);
        MinimumSize = new Size(920, 620);

        _recordIdTextBox.PlaceholderText = "Mã hồ sơ";
        _serviceTypeTextBox.PlaceholderText = "Loại dịch vụ";
        _resultTextBox.PlaceholderText = "Nhập kết quả/chẩn đoán cận lâm sàng";
        _serviceDatePicker.Format = DateTimePickerFormat.Short;

        _servicesSearchTextBox.PlaceholderText = "Tìm kiếm trong danh sách dịch vụ";
        _servicesSearchTextBox.TextChanged += (_, _) => ApplyServicesFilter();
        _servicesSearchFieldComboBox.Items.AddRange([
            "Mã hồ sơ",
            "Loại dịch vụ",
            "Ngày dịch vụ",
            "Kết quả",
            "Mã kỹ thuật viên"
        ]);
        _servicesSearchFieldComboBox.SelectedIndex = 0;
        _servicesSearchFieldComboBox.SelectedIndexChanged += (_, _) => ApplyServicesFilter();

        _servicesGrid.DataSource = _servicesBindingSource;
        _servicesGrid.DataBindingComplete += (_, _) => ApplyVietnameseHeaders(_servicesGrid);

        var scrollHost = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            AutoScrollMinSize = new Size(0, 640),
            BackColor = Color.FromArgb(241, 244, 249)
        };

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
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
            Text = "Bảng kỹ thuật viên",
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Margin = Padding.Empty
        });

        headerTextPanel.Controls.Add(new Label
        {
            Text = $"Xin chào, {_session.FullName}. Quản lý dịch vụ được phân công và cập nhật kết quả",
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(100, 116, 139),
            Margin = new Padding(0, 2, 0, 0)
        });

        var notificationsButton = new Button
        {
            Text = "Thông báo",
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

        actionLayout.Controls.Add(new Label { Text = "Mã hồ sơ", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 0);
        actionLayout.Controls.Add(new Label { Text = "Loại dịch vụ", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 1, 0);
        actionLayout.Controls.Add(new Label { Text = "Ngày", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 2, 0);
        actionLayout.Controls.Add(_recordIdTextBox, 0, 1);
        actionLayout.Controls.Add(_serviceTypeTextBox, 1, 1);
        actionLayout.Controls.Add(_serviceDatePicker, 2, 1);
        actionLayout.Controls.Add(new Label { Text = "Kết quả", AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 2);

        _recordIdTextBox.Dock = DockStyle.Fill;
        _recordIdTextBox.Margin = new Padding(0, 0, 10, 0);
        _serviceTypeTextBox.Dock = DockStyle.Fill;
        _serviceTypeTextBox.Margin = new Padding(0, 0, 10, 0);
        _serviceDatePicker.Dock = DockStyle.Fill;
        _serviceDatePicker.Margin = new Padding(0, 0, 10, 0);
        _resultTextBox.Dock = DockStyle.Fill;
        _resultTextBox.Margin = new Padding(0, 0, 10, 0);

        var saveButton = new Button
        {
            Text = "Lưu kết quả",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(0)
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
            RowCount = 4,
            Margin = Padding.Empty
        };
        gridCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        gridCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        gridCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        gridCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        gridCardLayout.Controls.Add(new Label
        {
            Text = "Dịch vụ được phân công",
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        }, 0, 0);

        gridCardLayout.Controls.Add(new Label
        {
            Text = "Theo dõi và cập nhật các dịch vụ bạn phụ trách",
            AutoSize = true,
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(100, 116, 139)
        }, 0, 1);

        var searchPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 8),
            Padding = Padding.Empty,
            AutoSize = true
        };
        searchPanel.Controls.Add(new Label
        {
            Text = "Tìm kiếm:",
            AutoSize = true,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            Margin = new Padding(0, 8, 8, 0)
        });
        searchPanel.Controls.Add(_servicesSearchFieldComboBox);
        searchPanel.Controls.Add(_servicesSearchTextBox);

        _servicesGrid.BackgroundColor = Color.White;
        _servicesGrid.BorderStyle = BorderStyle.None;
        _servicesGrid.EnableHeadersVisualStyles = false;
        _servicesGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
        _servicesGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
        _servicesGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
        _servicesGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 234, 255);
        _servicesGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
        _servicesGrid.RowTemplate.Height = 26;

        gridCardLayout.Controls.Add(searchPanel, 0, 2);
        gridCardLayout.Controls.Add(_servicesGrid, 0, 3);
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

        _allServices = _technicianService.GetAssignedServices(_session.StaffId.Value.ToString());
        ApplyServicesFilter();
    }

    private void ApplyServicesFilter()
    {
        string keyword = _servicesSearchTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            _servicesBindingSource.DataSource = _allServices;
            return;
        }

        string selectedField = _servicesSearchFieldComboBox.SelectedItem?.ToString() ?? "Mã hồ sơ";

        _servicesBindingSource.DataSource = _allServices
            .Where(s => selectedField switch
            {
                "Mã hồ sơ" => s.MAHSBA.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase),
                "Loại dịch vụ" => s.LOAIDV.Contains(keyword, StringComparison.OrdinalIgnoreCase),
                "Ngày dịch vụ" => s.NGAYDV.ToString("dd/MM/yyyy HH:mm").Contains(keyword, StringComparison.OrdinalIgnoreCase),
                "Kết quả" => s.KETQUA.Contains(keyword, StringComparison.OrdinalIgnoreCase),
                "Mã kỹ thuật viên" => s.MAKTV.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase),
                _ => true
            })
            .ToList();
    }

    private static void ApplyVietnameseHeaders(DataGridView grid)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["MAHSBA"] = "Mã Hồ Sơ",
            ["LOAIDV"] = "Loại Dịch Vụ",
            ["NGAYDV"] = "Ngày Dịch Vụ",
            ["KETQUA"] = "Kết Quả",
            ["MAKTV"] = "Mã Kỹ Thuật Viên"
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
            MessageBox.Show(this, ex.Message, "Kỹ thuật viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
