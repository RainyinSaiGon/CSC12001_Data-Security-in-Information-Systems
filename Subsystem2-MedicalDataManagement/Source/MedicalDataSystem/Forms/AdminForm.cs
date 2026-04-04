namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;
using Oracle.ManagedDataAccess.Client;

public class AdminForm : Form
{
    private readonly UserSession _session;
    private readonly OracleConnectionService _connectionService;
    private readonly UserService _userService;

    private readonly DataGridView _patientUsersGrid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells,
        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
        RowTemplate = { Height = 28 },
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
        ScrollBars = ScrollBars.Both
    };
    private readonly DataGridView _staffUsersGrid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
        RowTemplate = { Height = 28 },
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
        ScrollBars = ScrollBars.Both
    };
    private readonly TextBox _patientSearchBox = new() { Width = 240, PlaceholderText = "Nhap CCCD benh nhan" };
    private readonly TextBox _staffSearchBox = new() { Width = 240, PlaceholderText = "Nhap CMND nhan vien" };
    private readonly Button _patientSearchButton = new() { Text = "Tim", Width = 70, Height = 30 };
    private readonly Button _staffSearchButton = new() { Text = "Tim", Width = 70, Height = 30 };

    private readonly Label _patientPageInfoLabel = new() { AutoSize = true, Padding = new Padding(6, 8, 6, 0) };
    private readonly Button _patientRefreshButton = new() { Text = "Tim lai", Width = 90, Height = 30 };

    private readonly Label _staffPageInfoLabel = new() { AutoSize = true, Padding = new Padding(6, 8, 6, 0) };
    private readonly Button _staffRefreshButton = new() { Text = "Tim lai", Width = 90, Height = 30 };
    private readonly ComboBox _grantRoleCombo = new() { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Button _grantRoleButton = new() { Text = "Grant Role", Width = 110, Height = 30, Enabled = false };
    private readonly Label _grantRoleHintLabel = new() { AutoSize = true, Padding = new Padding(6, 8, 6, 0), Text = "Chon nhan vien de grant role." };
    private readonly TextBox _securityProfileTextBox = new()
    {
        Multiline = true,
        ReadOnly = true,
        Dock = DockStyle.Fill,
        ScrollBars = ScrollBars.Vertical
    };
    private UserSecurityProfileItem? _selectedProfile;

    private readonly ComboBox _createUserTypeCombo = new() { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _createUsernameTextBox = new() { Width = 180 };
    private readonly TextBox _createFullNameTextBox = new() { Width = 220 };
    private readonly ComboBox _createGenderCombo = new() { Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _createBirthDatePicker = new() { Width = 150, Format = DateTimePickerFormat.Short };
    private readonly TextBox _createIdNumberTextBox = new() { Width = 180 };

    private readonly TextBox _createStaffAddressTextBox = new() { Width = 220 };
    private readonly TextBox _createStaffPhoneTextBox = new() { Width = 140 };
    private readonly ComboBox _createStaffRoleCombo = new() { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _createStaffDepartmentCombo = new() { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };

    private readonly TextBox _createPatientSoNhaTextBox = new() { Width = 120 };
    private readonly TextBox _createPatientTenDuongTextBox = new() { Width = 180 };
    private readonly TextBox _createPatientQuanHuyenTextBox = new() { Width = 160 };
    private readonly TextBox _createPatientTinhTpTextBox = new() { Width = 160 };
    private readonly TextBox _createPatientTienSuBenhTextBox = new() { Width = 220 };
    private readonly TextBox _createPatientTienSuGdTextBox = new() { Width = 220 };
    private readonly TextBox _createPatientDiUngTextBox = new() { Width = 220 };

    private readonly FlowLayoutPanel _createStaffPanel = new() { AutoSize = true, WrapContents = true, Dock = DockStyle.Top };
    private readonly FlowLayoutPanel _createPatientPanel = new() { AutoSize = true, WrapContents = true, Dock = DockStyle.Top };
    private readonly Button _createUserButton = new() { Text = "Create User", Width = 120, Height = 32 };
    private readonly TextBox _createFlowTextBox = new()
    {
        Multiline = true,
        ReadOnly = true,
        Dock = DockStyle.Fill,
        ScrollBars = ScrollBars.Vertical
    };

    public AdminForm(UserSession session)
    {
        _session = session;
        _connectionService = new OracleConnectionService(session.ConnectionString);
        _userService = new UserService(_connectionService);
        BuildUi();
        SetSearchHints();
    }

    private void BuildUi()
    {
        Text = $"Hospital Admin Dashboard — {_session.FullName}";
        Width = 1000;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;

        var tabControl = new TabControl
        {
            Dock = DockStyle.Fill
        };

        tabControl.TabPages.Add(BuildUsersTab());
        tabControl.TabPages.Add(new TabPage("VPD Policies"));
        tabControl.TabPages.Add(new TabPage("OLS Labels"));
        tabControl.TabPages.Add(new TabPage("Audit Log"));

        tabControl.SelectedIndexChanged += (_, _) =>
        {
            if (tabControl.SelectedIndex == 0)
            {
                SetSearchHints();
            }
        };

        var statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0),
            Text = $"Logged in as: {_session.Username}"
        };

        var logoutButton = new Button
        {
            Text = "Log out",
            AutoSize = true,
            Margin = new Padding(0, 6, 0, 0)
        };
        logoutButton.Click += (_, _) => Logout();

        var headerActions = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 130,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(8, 0, 8, 0)
        };
        headerActions.Controls.Add(logoutButton);

        var headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 42
        };
        headerPanel.Controls.Add(statusLabel);
        headerPanel.Controls.Add(headerActions);

        Controls.Add(tabControl);
        Controls.Add(headerPanel);

        _ = _connectionService;
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

    private TabPage BuildUsersTab()
    {
        var tab = new TabPage("Users");

        var usersSubTabs = new TabControl
        {
            Dock = DockStyle.Fill
        };

        usersSubTabs.TabPages.Add(BuildPatientUsersSubTab());
        usersSubTabs.TabPages.Add(BuildStaffUsersSubTab());
        usersSubTabs.TabPages.Add(BuildCreateUserSubTab());
        usersSubTabs.SelectedIndexChanged += (_, _) =>
        {
            if (usersSubTabs.SelectedIndex == 0)
            {
                RefreshPatientUsersPage();
            }
            else if (usersSubTabs.SelectedIndex == 1)
            {
                RefreshStaffUsersPage();
            }
        };

        var profileGroup = new GroupBox
        {
            Text = "Thong tin nguoi dung va quyen hien tai",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };

        _grantRoleCombo.Items.Clear();
        _grantRoleCombo.Items.AddRange(new object[]
        {
            "DIEU_PHOI_VIEN",
            "BAC_SI_Y_SI",
            "KY_THUAT_VIEN",
            "BENH_NHAN"
        });
        _grantRoleCombo.SelectedIndex = 0;

        _grantRoleButton.Click -= HandleGrantRoleClick;
        _grantRoleButton.Click += HandleGrantRoleClick;

        var grantPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 42,
            WrapContents = false,
            Padding = new Padding(0)
        };
        grantPanel.Controls.Add(new Label { Text = "Grant role:", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        grantPanel.Controls.Add(_grantRoleCombo);
        grantPanel.Controls.Add(_grantRoleButton);
        grantPanel.Controls.Add(_grantRoleHintLabel);

        var profileLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        profileLayout.Controls.Add(grantPanel, 0, 0);
        profileLayout.Controls.Add(_securityProfileTextBox, 0, 1);

        profileGroup.Controls.Add(profileLayout);

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
        rootLayout.Controls.Add(usersSubTabs, 0, 0);
        rootLayout.Controls.Add(profileGroup, 0, 1);

        tab.Controls.Add(rootLayout);
        ShowProfileResult(null, "Nhap CCCD/CMND va bam Tim de hien thong tin + RBAC role.");
        return tab;
    }

    private TabPage BuildPatientUsersSubTab()
    {
        var tab = new TabPage("BN");

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

        var searchPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8, 4, 8, 4),
            WrapContents = false
        };

        searchPanel.Controls.Add(new Label { Text = "CCCD:", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        searchPanel.Controls.Add(_patientSearchBox);
        searchPanel.Controls.Add(_patientSearchButton);

        var pagingPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8, 4, 8, 4),
            WrapContents = false
        };

        _patientSearchButton.Click += (_, _) =>
        {
            RefreshPatientUsersPage();
        };

        _patientSearchBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                RefreshPatientUsersPage();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        };

        _patientRefreshButton.Click += (_, _) => RefreshPatientUsersPage();

        pagingPanel.Controls.Add(_patientRefreshButton);
        pagingPanel.Controls.Add(_patientPageInfoLabel);

        rootLayout.Controls.Add(searchPanel, 0, 0);
        rootLayout.Controls.Add(_patientUsersGrid, 0, 1);
        rootLayout.Controls.Add(pagingPanel, 0, 2);
        tab.Controls.Add(rootLayout);
        return tab;
    }

    private TabPage BuildStaffUsersSubTab()
    {
        var tab = new TabPage("NV");

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

        var searchPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8, 4, 8, 4),
            WrapContents = false
        };

        searchPanel.Controls.Add(new Label { Text = "CMND:", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        searchPanel.Controls.Add(_staffSearchBox);
        searchPanel.Controls.Add(_staffSearchButton);

        var pagingPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8, 4, 8, 4),
            WrapContents = false
        };

        _staffSearchButton.Click += (_, _) =>
        {
            RefreshStaffUsersPage();
        };

        _staffSearchBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                RefreshStaffUsersPage();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        };

        _staffRefreshButton.Click += (_, _) => RefreshStaffUsersPage();

        pagingPanel.Controls.Add(_staffRefreshButton);
        pagingPanel.Controls.Add(_staffPageInfoLabel);

        rootLayout.Controls.Add(searchPanel, 0, 0);
        rootLayout.Controls.Add(_staffUsersGrid, 0, 1);
        rootLayout.Controls.Add(pagingPanel, 0, 2);
        tab.Controls.Add(rootLayout);
        return tab;
    }

    private TabPage BuildCreateUserSubTab()
    {
        var tab = new TabPage("Create User");

        _createUserTypeCombo.Items.Clear();
        _createUserTypeCombo.Items.AddRange(new object[] { "STAFF", "PATIENT" });
        _createUserTypeCombo.SelectedIndex = 0;
        _createUserTypeCombo.SelectedIndexChanged += (_, _) => ToggleCreateUserPanels();

        _createGenderCombo.Items.Clear();
        _createGenderCombo.Items.AddRange(new object[] { "Nam", "Nữ" });
        _createGenderCombo.SelectedIndex = 0;

        _createStaffRoleCombo.Items.Clear();
        _createStaffRoleCombo.Items.AddRange(new object[] { "Điều phối viên", "Bác sĩ/Y sĩ", "Kỹ thuật viên" });
        _createStaffRoleCombo.SelectedIndex = 0;

        _createStaffPanel.Controls.Clear();
        _createStaffPanel.Controls.Add(new Label { Text = "Address (QUEQUAN)", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createStaffPanel.Controls.Add(_createStaffAddressTextBox);
        _createStaffPanel.Controls.Add(new Label { Text = "Phone", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createStaffPanel.Controls.Add(_createStaffPhoneTextBox);
        _createStaffPanel.Controls.Add(new Label { Text = "Role", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createStaffPanel.Controls.Add(_createStaffRoleCombo);
        _createStaffPanel.Controls.Add(new Label { Text = "Department", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createStaffPanel.Controls.Add(_createStaffDepartmentCombo);

        LoadDepartmentOptions();

        _createPatientPanel.Controls.Clear();
        _createPatientPanel.Controls.Add(new Label { Text = "So nha", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createPatientPanel.Controls.Add(_createPatientSoNhaTextBox);
        _createPatientPanel.Controls.Add(new Label { Text = "Ten duong", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createPatientPanel.Controls.Add(_createPatientTenDuongTextBox);
        _createPatientPanel.Controls.Add(new Label { Text = "Quan/Huyen", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createPatientPanel.Controls.Add(_createPatientQuanHuyenTextBox);
        _createPatientPanel.Controls.Add(new Label { Text = "Tinh/TP", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createPatientPanel.Controls.Add(_createPatientTinhTpTextBox);
        _createPatientPanel.Controls.Add(new Label { Text = "Tien su benh", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createPatientPanel.Controls.Add(_createPatientTienSuBenhTextBox);
        _createPatientPanel.Controls.Add(new Label { Text = "Tien su benh GD", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createPatientPanel.Controls.Add(_createPatientTienSuGdTextBox);
        _createPatientPanel.Controls.Add(new Label { Text = "Di ung thuoc", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        _createPatientPanel.Controls.Add(_createPatientDiUngTextBox);

        _createUserButton.Click -= HandleCreateUserClick;
        _createUserButton.Click += HandleCreateUserClick;

        var formPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(12),
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight
        };

        formPanel.Controls.Add(new Label { Text = "UserType", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        formPanel.Controls.Add(_createUserTypeCombo);
        formPanel.Controls.Add(new Label { Text = "Username", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        formPanel.Controls.Add(_createUsernameTextBox);
        formPanel.Controls.Add(new Label { Text = "FullName", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        formPanel.Controls.Add(_createFullNameTextBox);
        formPanel.Controls.Add(new Label { Text = "Gender", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        formPanel.Controls.Add(_createGenderCombo);
        formPanel.Controls.Add(new Label { Text = "BirthDate", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        formPanel.Controls.Add(_createBirthDatePicker);
        formPanel.Controls.Add(new Label { Text = "IDNumber", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        formPanel.Controls.Add(_createIdNumberTextBox);

        formPanel.SetFlowBreak(_createIdNumberTextBox, true);
        formPanel.Controls.Add(_createStaffPanel);
        formPanel.SetFlowBreak(_createStaffPanel, true);
        formPanel.Controls.Add(_createPatientPanel);
        formPanel.SetFlowBreak(_createPatientPanel, true);
        formPanel.Controls.Add(_createUserButton);

        var flowGroup = new GroupBox
        {
            Text = "Create User Flow",
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };
        flowGroup.Controls.Add(_createFlowTextBox);

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
        rootLayout.Controls.Add(formPanel, 0, 0);
        rootLayout.Controls.Add(flowGroup, 0, 1);

        tab.Controls.Add(rootLayout);

        ToggleCreateUserPanels();
        SetCreateFlowStep(1);
        return tab;
    }

    private void SetSearchHints()
    {
        if (string.IsNullOrWhiteSpace(_patientSearchBox.Text))
        {
            _patientUsersGrid.DataSource = new List<object>();
            _patientPageInfoLabel.Text = "BN: Nhap CCCD de tim";
        }

        if (string.IsNullOrWhiteSpace(_staffSearchBox.Text))
        {
            _staffUsersGrid.DataSource = new List<object>();
            _staffPageInfoLabel.Text = "NV: Nhap CMND de tim";
        }
    }

    private void RefreshPatientUsersPage()
    {
        try
        {
            string cccdKeyword = new string(_patientSearchBox.Text.Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(cccdKeyword))
            {
                _patientUsersGrid.DataSource = new List<object>();
                _patientPageInfoLabel.Text = "BN: Nhap CCCD de tim";
                ShowProfileResult(null, "Chua co CCCD de tra cuu.");
                return;
            }

            if (cccdKeyword.Length != 12)
            {
                _patientUsersGrid.DataSource = new List<object>();
                _patientPageInfoLabel.Text = "BN: CCCD phai dung 12 so";
                ShowProfileResult(null, "CCCD phai dung 12 so moi xem duoc profile + role.");
                return;
            }

            List<PatientAccountItem> users = _userService.GetPatientUsersByCccd(cccdKeyword);

            _patientUsersGrid.DataSource = users
                .Select(u => new
                {
                    MaBN = u.MABN,
                    HoTen = u.TENBN,
                    GioiTinh = u.PHAI,
                    NgaySinh = u.NGAYSINH?.ToString("dd/MM/yyyy") ?? "—",
                    CCCD = u.CCCD,
                    SoNha = u.SONHA,
                    TenDuong = u.TENDUONG,
                    QuanHuyen = u.QUANHUYEN,
                    TinhTP = u.TINHTP,
                    TienSuBenh = u.TIENSUBENH,
                    TienSuBenhGiaDinh = u.TIENSUBENHGD,
                    DiUngThuoc = u.DIUNGTHUOC,
                    Username = u.Username,
                    TrangThaiTaiKhoan = u.AccountStatus,
                    TaoLuc = u.CreatedDate?.ToString("dd/MM/yyyy") ?? "—",
                    HetHan = u.ExpiryDate?.ToString("dd/MM/yyyy") ?? "—"
                })
                .ToList();

            _patientPageInfoLabel.Text = $"BN: Tim thay {users.Count} tai khoan (toi da 100 dong)";

            UserSecurityProfileItem? profile = _userService.GetPatientProfileWithRolesByCccd(cccdKeyword);
            ShowProfileResult(profile, $"Tra cuu theo CCCD: {cccdKeyword}");
        }
        catch (OracleException ex)
        {
            MessageBox.Show(this, $"Oracle error {ex.Number}: {ex.Message}", "Users - BN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ShowProfileResult(null, $"Loi Oracle khi tra cuu BN: {ex.Number}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Users - BN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ShowProfileResult(null, "Loi khi tra cuu BN.");
        }
    }

    private void RefreshStaffUsersPage()
    {
        try
        {
            string cmndKeyword = _staffSearchBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(cmndKeyword))
            {
                _staffUsersGrid.DataSource = new List<object>();
                _staffPageInfoLabel.Text = "NV: Nhap CMND de tim";
                ShowProfileResult(null, "Chua co CMND de tra cuu.");
                return;
            }

            List<UserAccountItem> users = _userService.GetStaffUsersByCmnd(cmndKeyword);

            _staffUsersGrid.DataSource = users
                .Select(u => new
                {
                    Id = u.UserId,
                    FullName = u.FullName,
                    Username = u.Username,
                    AccountStatus = u.AccountStatus,
                    Created = u.CreatedDate?.ToString("dd/MM/yyyy") ?? "—",
                    Expiry = u.ExpiryDate?.ToString("dd/MM/yyyy") ?? "—"
                })
                .ToList();

            _staffPageInfoLabel.Text = $"NV: Tim thay {users.Count} tai khoan";

            string normalizedCmnd = new string(cmndKeyword.Where(char.IsDigit).ToArray());
            if (normalizedCmnd.Length == 12)
            {
                UserSecurityProfileItem? profile = _userService.GetStaffProfileWithRolesByCmnd(normalizedCmnd);
                ShowProfileResult(profile, $"Tra cuu theo CMND: {normalizedCmnd}");
            }
            else
            {
                ShowProfileResult(null, "CMND chua du 12 so, chi hien danh sach tim gan dung.");
            }
        }
        catch (OracleException ex)
        {
            MessageBox.Show(this, $"Oracle error {ex.Number}: {ex.Message}", "Users - NV", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ShowProfileResult(null, $"Loi Oracle khi tra cuu NV: {ex.Number}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Users - NV", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ShowProfileResult(null, "Loi khi tra cuu NV.");
        }
    }

    private void ShowProfileResult(UserSecurityProfileItem? profile, string note)
    {
        _selectedProfile = profile;
        bool canGrant = profile is not null && string.Equals(profile.UserType, "STAFF", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(profile.Username);
        _grantRoleButton.Enabled = canGrant;
        _grantRoleHintLabel.Text = canGrant
            ? $"Dang chon: {profile!.Username}"
            : "Chi grant role khi dang hien profile STAFF.";

        if (profile is null)
        {
            _securityProfileTextBox.Text = $"{note}{Environment.NewLine}{Environment.NewLine}Khong tim thay profile hoac chua du dieu kien tra cuu.";
            return;
        }

        string[] roleLines = profile.CurrentOracleRoles.Count == 0
            ? new[] { "(Khong co role)" }
            : profile.CurrentOracleRoles.ToArray();

        if (string.Equals(profile.UserType, "STAFF", StringComparison.OrdinalIgnoreCase))
        {
            _securityProfileTextBox.Text = string.Join(Environment.NewLine,
                note,
                string.Empty,
                $"Loai: {profile.UserType}",
                $"ID: {profile.UserId}",
                $"Ho ten: {profile.FullName}",
                $"Gioi tinh: {profile.Gender}",
                $"Ngay sinh: {profile.BirthDate:dd/MM/yyyy}",
                $"CMND: {profile.IdNumber}",
                $"Username: {profile.Username}",
                $"Que quan: {profile.Address}",
                $"So DT: {profile.Phone}",
                $"Vai tro nghiep vu: {profile.BusinessRole}",
                $"Khoa: {profile.Department}",
                "Role Oracle hien tai:",
                string.Join(Environment.NewLine, roleLines.Select(r => $"- {r}")));
            return;
        }

        _securityProfileTextBox.Text = string.Join(Environment.NewLine,
            note,
            string.Empty,
            $"Loai: {profile.UserType}",
            $"ID: {profile.UserId}",
            $"Ho ten: {profile.FullName}",
            $"Gioi tinh: {profile.Gender}",
            $"Ngay sinh: {profile.BirthDate:dd/MM/yyyy}",
            $"CCCD: {profile.IdNumber}",
            $"Username: {profile.Username}",
            $"Dia chi: {profile.SoNha}, {profile.TenDuong}, {profile.QuanHuyen}, {profile.TinhTp}",
            $"Tien su benh: {profile.TienSuBenh}",
            $"Tien su benh GD: {profile.TienSuBenhGiaDinh}",
            $"Di ung thuoc: {profile.DiUngThuoc}",
            "Role Oracle hien tai:",
            string.Join(Environment.NewLine, roleLines.Select(r => $"- {r}")));
    }

    private void HandleGrantRoleClick(object? sender, EventArgs e)
    {
        _ = sender;

        if (_selectedProfile is null || !string.Equals(_selectedProfile.UserType, "STAFF", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(this, "Hay tim nhan vien truoc khi grant role.", "Grant Role", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string roleName = _grantRoleCombo.SelectedItem?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(roleName))
        {
            MessageBox.Show(this, "Role khong hop le.", "Grant Role", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        bool granted = _userService.GrantRoleToStaff(_selectedProfile.Username, roleName);
        if (!granted)
        {
            string detail = string.IsNullOrWhiteSpace(_userService.LastErrorMessage) ? "Grant role failed." : _userService.LastErrorMessage;
            MessageBox.Show(this, detail, "Grant Role", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        MessageBox.Show(this, $"Granted {roleName} to {_selectedProfile.Username}.", "Grant Role", MessageBoxButtons.OK, MessageBoxIcon.Information);

        if (!string.IsNullOrWhiteSpace(_selectedProfile.IdNumber))
        {
            UserSecurityProfileItem? refreshed = _userService.GetStaffProfileWithRolesByCmnd(_selectedProfile.IdNumber);
            ShowProfileResult(refreshed, $"Cap nhat role cho user: {_selectedProfile.Username}");
        }
        else
        {
            ShowProfileResult(_selectedProfile, $"Da grant role {roleName}.");
        }
    }

    private void ToggleCreateUserPanels()
    {
        bool isStaff = string.Equals(_createUserTypeCombo.SelectedItem?.ToString(), "STAFF", StringComparison.OrdinalIgnoreCase);
        _createStaffPanel.Visible = isStaff;
        _createPatientPanel.Visible = !isStaff;
    }

    private void HandleCreateUserClick(object? sender, EventArgs e)
    {
        _ = sender;
        SetCreateFlowStep(1);

        if (!TryBuildCreateUserRequest(out CreateUserRequest? request, out string validationMessage) || request is null)
        {
            MessageBox.Show(this, validationMessage, "Create User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SetCreateFlowStep(2);
        Application.DoEvents();
        SetCreateFlowStep(3);
        Application.DoEvents();

        bool created = _userService.CreateUser(request);
        if (!created)
        {
            string detail = string.IsNullOrWhiteSpace(_userService.LastErrorMessage) ? "Create user failed." : _userService.LastErrorMessage;
            MessageBox.Show(this, detail, "Create User", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        SetCreateFlowStep(4);
        Application.DoEvents();
        SetCreateFlowStep(5);

        MessageBox.Show(this, "User created successfully.", "Create User", MessageBoxButtons.OK, MessageBoxIcon.Information);

        if (string.Equals(request.UserType, "STAFF", StringComparison.OrdinalIgnoreCase))
        {
            _staffSearchBox.Text = request.IDNumber;
            RefreshStaffUsersPage();
        }
        else
        {
            _patientSearchBox.Text = request.IDNumber;
            RefreshPatientUsersPage();
        }
    }

    private bool TryBuildCreateUserRequest(out CreateUserRequest? request, out string message)
    {
        request = null;
        message = string.Empty;

        string userType = _createUserTypeCombo.SelectedItem?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(userType))
        {
            message = "UserType is required.";
            return false;
        }

        string idNumber = new string((_createIdNumberTextBox.Text ?? string.Empty).Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(_createUsernameTextBox.Text) ||
            string.IsNullOrWhiteSpace(_createFullNameTextBox.Text) ||
            string.IsNullOrWhiteSpace(idNumber))
        {
            message = "Username, FullName, and IDNumber are required.";
            return false;
        }

        var built = new CreateUserRequest
        {
            UserType = userType,
            Username = _createUsernameTextBox.Text.Trim(),
            FullName = _createFullNameTextBox.Text.Trim(),
            Gender = _createGenderCombo.SelectedItem?.ToString() ?? "Nam",
            BirthDate = _createBirthDatePicker.Value.Date,
            IDNumber = idNumber,
            Address = _createStaffAddressTextBox.Text.Trim(),
            Phone = _createStaffPhoneTextBox.Text.Trim(),
            Role = _createStaffRoleCombo.SelectedItem?.ToString() ?? string.Empty,
            Department = (_createStaffDepartmentCombo.SelectedItem as DepartmentOption)?.MAKHOA ?? string.Empty,
            SONHA = _createPatientSoNhaTextBox.Text.Trim(),
            TENDUONG = _createPatientTenDuongTextBox.Text.Trim(),
            QUANHUYEN = _createPatientQuanHuyenTextBox.Text.Trim(),
            TINHTP = _createPatientTinhTpTextBox.Text.Trim(),
            TIENSUBENH = _createPatientTienSuBenhTextBox.Text.Trim(),
            TIENSUBENHGD = _createPatientTienSuGdTextBox.Text.Trim(),
            DIUNGTHUOC = _createPatientDiUngTextBox.Text.Trim()
        };

        if (string.Equals(userType, "STAFF", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(built.Address) ||
                string.IsNullOrWhiteSpace(built.Phone) ||
                string.IsNullOrWhiteSpace(built.Role) ||
                string.IsNullOrWhiteSpace(built.Department))
            {
                message = "Address, Phone, Role, and Department are required for STAFF.";
                return false;
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(built.SONHA) ||
                string.IsNullOrWhiteSpace(built.TENDUONG) ||
                string.IsNullOrWhiteSpace(built.QUANHUYEN) ||
                string.IsNullOrWhiteSpace(built.TINHTP))
            {
                message = "SONHA, TENDUONG, QUANHUYEN, and TINHTP are required for PATIENT.";
                return false;
            }
        }

        request = built;
        return true;
    }

    private void LoadDepartmentOptions()
    {
        _createStaffDepartmentCombo.BeginUpdate();
        try
        {
            List<DepartmentOption> departments = _userService.GetDepartments();
            _createStaffDepartmentCombo.Items.Clear();
            foreach (DepartmentOption department in departments)
            {
                _createStaffDepartmentCombo.Items.Add(department);
            }

            if (_createStaffDepartmentCombo.Items.Count > 0)
            {
                _createStaffDepartmentCombo.SelectedIndex = 0;
            }

        }
        catch (Exception ex)
        {
            _createStaffDepartmentCombo.Items.Clear();
            MessageBox.Show(this, $"Unable to load KHOA list: {ex.Message}", "Create User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            _createStaffDepartmentCombo.EndUpdate();
        }
    }

    private void SetCreateFlowStep(int currentStep)
    {
        string[] steps =
        {
            "Step 1: Admin nhap thong tin",
            "Step 2: Insert vao NHANVIEN / BENHNHAN",
            "Step 3: CREATE USER trong Oracle",
            "Step 4: GRANT ROLE",
            "Step 5: Hoan tat"
        };

        for (int i = 0; i < steps.Length; i++)
        {
            bool isCurrent = (i + 1) == currentStep;
            steps[i] = isCurrent ? $">> {steps[i]}" : $"   {steps[i]}";
        }

        _createFlowTextBox.Text = string.Join(Environment.NewLine, steps);
    }
}
