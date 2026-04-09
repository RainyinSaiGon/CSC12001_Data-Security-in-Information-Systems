namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;
public class AdminForm : Form

{
    private readonly UserSession _session;
    private readonly OracleConnectionService _connectionService;
    private readonly UserService _userService;
    private readonly RBACService _rbacService;
    private readonly VPDService _vpdService;
    private readonly OLSService _olsService;
    private readonly TabControl _mainTabControl = new() { Dock = DockStyle.Fill };
    private readonly TabControl _usersSubTabs = new() { Dock = DockStyle.Fill };
    private readonly Label _footerStatusLabel = new() { Dock = DockStyle.Fill, AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft };
    private readonly DataGridView _vpdPoliciesGrid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
        RowTemplate = { Height = 28 },
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
        ScrollBars = ScrollBars.Both
    };
    private readonly Label _vpdPoliciesInfoLabel = new() { AutoSize = true, Padding = new Padding(6, 8, 6, 0) };
    private readonly Button _vpdRefreshButton = new() { Text = "Refresh VPD", Width = 110, Height = 30 };
    private readonly Button _vpdEnableButton = new() { Text = "Enable", Width = 90, Height = 30, Enabled = false };
    private readonly Button _vpdDisableButton = new() { Text = "Disable", Width = 90, Height = 30, Enabled = false };
    private readonly ComboBox _olsUserCombo = new() { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Button _olsLoadLabelButton = new() { Text = "Load User Labels", Width = 130, Height = 30 };
    private readonly Button _olsPreviewButton = new() { Text = "Preview Accessible Notifications", Width = 210, Height = 30 };
    private readonly Label _olsLabelSummaryLabel = new() { AutoSize = true, Padding = new Padding(6, 8, 6, 0), Text = "User label: (not loaded)" };
    private readonly DataGridView _olsPreviewGrid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
        RowTemplate = { Height = 28 },
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
        ScrollBars = ScrollBars.Both
    };
    private readonly TextBox _olsHierarchyTextBox = new()
    {
        Multiline = true,
        ReadOnly = true,
        Dock = DockStyle.Fill,
        ScrollBars = ScrollBars.Vertical
    };

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
    private readonly DataGridView _roleGrid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
        RowTemplate = { Height = 28 },
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
    };
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
        _rbacService = new RBACService(_connectionService);
        _vpdService = new VPDService(_connectionService);
        _olsService = new OLSService(_connectionService);
        InitializeComponent();
        SetSearchHints();
    }

    private void InitializeComponent()
    {
        Text = $"Hospital Admin Dashboard — {_session.FullName}";
        Width = 1000;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;
        Padding = new Padding(8);

        _mainTabControl.TabPages.Clear();
        _mainTabControl.TabPages.Add(BuildUsersTab());
        _mainTabControl.TabPages.Add(BuildVpdPoliciesTab());
        _mainTabControl.TabPages.Add(BuildOlsVisualizationTab());
        _mainTabControl.TabPages.Add(new TabPage("Audit Log"));

        _mainTabControl.SelectedIndexChanged += (_, _) =>
        {
            if (_mainTabControl.SelectedIndex == 0)
            {
                SetSearchHints();
            }
            else if (_mainTabControl.SelectedIndex == 1)
            {
                RefreshVpdPolicies();
            }

            UpdateFooterStatus("Ready");
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
            Margin = new Padding(0),
            Anchor = AnchorStyles.Right | AnchorStyles.Top
        };
        logoutButton.Click += (_, _) => Logout();

        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(8, 8, 8, 8)
        };
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        headerLayout.Controls.Add(statusLabel, 0, 0);
        headerLayout.Controls.Add(logoutButton, 1, 0);

        var headerPanel = new Panel
        {
            Dock = DockStyle.Fill
        };
        headerPanel.Controls.Add(headerLayout);

        var footerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 1,
            Padding = new Padding(8, 6, 8, 6)
        };
        footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        footerLayout.Controls.Add(_footerStatusLabel, 0, 0);

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        rootLayout.Controls.Add(headerPanel, 0, 0);
        rootLayout.Controls.Add(_mainTabControl, 0, 1);
        rootLayout.Controls.Add(footerLayout, 0, 2);

        Controls.Clear();
        Controls.Add(rootLayout);
        UpdateFooterStatus("Ready");

        _ = _connectionService;
    }

    private void UpdateFooterStatus(string message)
    {
        _footerStatusLabel.Text = message;
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

        _usersSubTabs.TabPages.Clear();
        _usersSubTabs.TabPages.Add(BuildPatientUsersSubTab());
        _usersSubTabs.TabPages.Add(BuildStaffUsersSubTab());
        _usersSubTabs.TabPages.Add(BuildCreateUserSubTab());
        _usersSubTabs.SelectedIndexChanged += (_, _) =>
        {
            if (_usersSubTabs.SelectedIndex == 0)
            {
                RefreshPatientUsersPage();
            }
            else if (_usersSubTabs.SelectedIndex == 1)
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
            RowCount = 3
        };
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
        profileLayout.Controls.Add(grantPanel, 0, 0);
        profileLayout.Controls.Add(_securityProfileTextBox, 0, 1);
        profileLayout.Controls.Add(_roleGrid, 0, 2);

        if (_roleGrid.Columns.Count == 0)
        {
            _roleGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "RoleName",
                HeaderText = "Role"
            });

            _roleGrid.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "RevokeRole",
                HeaderText = "Action",
                Text = "Revoke",
                UseColumnTextForButtonValue = true
            });

            _roleGrid.CellContentClick -= HandleRoleGridCellContentClick;
            _roleGrid.CellContentClick += HandleRoleGridCellContentClick;
        }

        profileGroup.Controls.Add(profileLayout);

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
        rootLayout.Controls.Add(_usersSubTabs, 0, 0);
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

        var commonLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 3,
            Padding = new Padding(8),
            AutoSize = true
        };
        commonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        commonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        commonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        commonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        commonLayout.Controls.Add(new Label { Text = "UserType", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        commonLayout.Controls.Add(_createUserTypeCombo, 1, 0);
        commonLayout.Controls.Add(new Label { Text = "Username", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 0);
        commonLayout.Controls.Add(_createUsernameTextBox, 3, 0);

        commonLayout.Controls.Add(new Label { Text = "FullName", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        commonLayout.Controls.Add(_createFullNameTextBox, 1, 1);
        commonLayout.Controls.Add(new Label { Text = "Gender", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 1);
        commonLayout.Controls.Add(_createGenderCombo, 3, 1);

        commonLayout.Controls.Add(new Label { Text = "BirthDate", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        commonLayout.Controls.Add(_createBirthDatePicker, 1, 2);
        commonLayout.Controls.Add(new Label { Text = "IDNumber", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 2);
        commonLayout.Controls.Add(_createIdNumberTextBox, 3, 2);

        var commonGroup = new GroupBox
        {
            Text = "Common Information",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        commonGroup.Controls.Add(commonLayout);

        var staffGroup = new GroupBox
        {
            Text = "Staff Details",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        _createStaffPanel.Dock = DockStyle.Fill;
        staffGroup.Controls.Add(_createStaffPanel);

        var patientGroup = new GroupBox
        {
            Text = "Patient Details",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        _createPatientPanel.Dock = DockStyle.Fill;
        patientGroup.Controls.Add(_createPatientPanel);

        var actionPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        _createUserButton.Anchor = AnchorStyles.Left;
        actionPanel.Controls.Add(_createUserButton);

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
            RowCount = 5,
            Padding = new Padding(8)
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.Controls.Add(commonGroup, 0, 0);
        rootLayout.Controls.Add(staffGroup, 0, 1);
        rootLayout.Controls.Add(patientGroup, 0, 2);
        rootLayout.Controls.Add(actionPanel, 0, 3);
        rootLayout.Controls.Add(flowGroup, 0, 4);

        tab.Controls.Add(rootLayout);

        ToggleCreateUserPanels();
        SetCreateFlowStep(1);
        return tab;
    }

    private TabPage BuildVpdPoliciesTab()
    {
        var tab = new TabPage("VPD Policies");

        _vpdRefreshButton.Click -= HandleVpdRefreshClick;
        _vpdRefreshButton.Click += HandleVpdRefreshClick;
        _vpdEnableButton.Click -= HandleVpdEnableClick;
        _vpdEnableButton.Click += HandleVpdEnableClick;
        _vpdDisableButton.Click -= HandleVpdDisableClick;
        _vpdDisableButton.Click += HandleVpdDisableClick;
        _vpdPoliciesGrid.SelectionChanged -= HandleVpdPolicySelectionChanged;
        _vpdPoliciesGrid.SelectionChanged += HandleVpdPolicySelectionChanged;

        var topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8, 4, 8, 4),
            WrapContents = false
        };
        topPanel.Controls.Add(_vpdRefreshButton);
        topPanel.Controls.Add(_vpdEnableButton);
        topPanel.Controls.Add(_vpdDisableButton);
        topPanel.Controls.Add(_vpdPoliciesInfoLabel);

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.Controls.Add(topPanel, 0, 0);
        rootLayout.Controls.Add(_vpdPoliciesGrid, 0, 1);

        tab.Controls.Add(rootLayout);
        RefreshVpdPolicies();
        return tab;
    }

    private TabPage BuildOlsVisualizationTab()
    {
        var tab = new TabPage("OLS Labels");

        _olsLoadLabelButton.Click -= HandleOlsLoadLabelClick;
        _olsLoadLabelButton.Click += HandleOlsLoadLabelClick;
        _olsPreviewButton.Click -= HandleOlsPreviewClick;
        _olsPreviewButton.Click += HandleOlsPreviewClick;
        _olsPreviewGrid.DataBindingComplete -= HandleOlsPreviewDataBindingComplete;
        _olsPreviewGrid.DataBindingComplete += HandleOlsPreviewDataBindingComplete;

        var topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8, 4, 8, 4),
            WrapContents = false
        };
        topPanel.Controls.Add(new Label { Text = "User", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        topPanel.Controls.Add(_olsUserCombo);
        topPanel.Controls.Add(_olsLoadLabelButton);
        topPanel.Controls.Add(_olsPreviewButton);
        topPanel.Controls.Add(_olsLabelSummaryLabel);

        var hierarchyGroup = new GroupBox
        {
            Text = "OLS Label Hierarchy",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        hierarchyGroup.Controls.Add(_olsHierarchyTextBox);

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
        rootLayout.Controls.Add(topPanel, 0, 0);
        rootLayout.Controls.Add(_olsPreviewGrid, 0, 1);
        rootLayout.Controls.Add(hierarchyGroup, 0, 2);

        tab.Controls.Add(rootLayout);
        LoadOlsUserChoices();
        _olsPreviewGrid.DataSource = new List<OlsNotificationAccessPreviewItem>();
        return tab;
    }

    private void LoadOlsUserChoices()
    {
        try
        {
            List<string> usernames = _olsService.GetAvailableUsernames();

            _olsUserCombo.BeginUpdate();
            _olsUserCombo.Items.Clear();
            foreach (string user in usernames)
            {
                _olsUserCombo.Items.Add(user);
            }

            if (_olsUserCombo.Items.Count > 0)
            {
                _olsUserCombo.SelectedIndex = 0;
            }

            _olsUserCombo.EndUpdate();
            UpdateFooterStatus($"OLS: Loaded {usernames.Count} users.");
        }
        catch (Exception ex)
        {
            _olsUserCombo.Items.Clear();
            _olsLabelSummaryLabel.Text = "User label: load users failed";
            _olsHierarchyTextBox.Text = ex.Message;
            UpdateFooterStatus("OLS: Failed to load user list.");
        }
    }

    private void HandleOlsLoadLabelClick(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;

        string username = _olsUserCombo.SelectedItem?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(username))
        {
            MessageBox.Show(this, "Please select a user.", "OLS Labels", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            string userLabel = _olsService.GetUserLabel(username);
            if (string.IsNullOrWhiteSpace(userLabel))
            {
                _olsLabelSummaryLabel.Text = $"User label: {username} has no OLS metadata";
                _olsHierarchyTextBox.Text = "No OLS label found in ALL_SA_USERS/DBA_SA_USERS for this user.";
                UpdateFooterStatus($"OLS: No label metadata for {username}.");
                return;
            }

            _olsLabelSummaryLabel.Text = $"User label: {userLabel}";
            RenderUserHierarchy(userLabel);
            UpdateFooterStatus($"OLS: Loaded label for {username}.");
        }
        catch (Exception ex)
        {
            _olsLabelSummaryLabel.Text = "User label: failed";
            _olsHierarchyTextBox.Text = ex.Message;
            UpdateFooterStatus("OLS: Failed to load label.");
            MessageBox.Show(this, ex.Message, "OLS Labels", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void HandleOlsPreviewClick(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;

        string username = _olsUserCombo.SelectedItem?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(username))
        {
            MessageBox.Show(this, "Please select a user.", "OLS Labels", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            List<OlsNotificationAccessPreviewItem> previewRows = _olsService.BuildNotificationAccessPreview(username);
            _olsPreviewGrid.DataSource = previewRows;

            int allowedCount = previewRows.Count(row => row.CanAccess);
            int deniedCount = previewRows.Count - allowedCount;
            UpdateFooterStatus($"OLS: {username} -> YES={allowedCount}, NO={deniedCount}");

            if (previewRows.Count > 0)
            {
                _olsLabelSummaryLabel.Text = $"User label: {previewRows[0].UserLabel}";
                RenderUserHierarchy(previewRows[0].UserLabel);
            }
        }
        catch (Exception ex)
        {
            _olsPreviewGrid.DataSource = new List<OlsNotificationAccessPreviewItem>();
            UpdateFooterStatus("OLS: Preview failed.");
            MessageBox.Show(this, ex.Message, "OLS Labels", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void HandleOlsPreviewDataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        _ = sender;
        _ = e;

        foreach (DataGridViewRow row in _olsPreviewGrid.Rows)
        {
            if (row.DataBoundItem is not OlsNotificationAccessPreviewItem item)
            {
                continue;
            }

            if (item.CanAccess)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(224, 247, 224);
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(252, 228, 228);
            }
        }
    }

    private void RenderUserHierarchy(string userLabel)
    {
        OlsParsedLabel parsed = _olsService.ParseLabel(userLabel);

        string compartments = parsed.Compartments.Count == 0
            ? "(none)"
            : string.Join(", ", parsed.Compartments.OrderBy(x => x));
        string groups = parsed.Groups.Count == 0
            ? "(none)"
            : string.Join(", ", parsed.Groups.OrderBy(x => x));

        _olsHierarchyTextBox.Text = string.Join(Environment.NewLine,
            "OLS User Label Tree",
            "|- Policy: THONGBAO_OLS",
            $"|- Level: {parsed.LevelCode} (rank={parsed.LevelRank})",
            $"|- Compartments: {compartments}",
            $"|- Groups: {groups}",
            "",
            "Access rule simulation:",
            "- Level dominance: user level >= row level",
            "- Compartment inclusion: row compartments subset of user compartments",
            "- Group inclusion: row groups subset of user groups");
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

    private void HandleVpdRefreshClick(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;
        RefreshVpdPolicies();
    }

    private void HandleVpdEnableClick(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;
        ToggleSelectedVpdPolicy(true);
    }

    private void HandleVpdDisableClick(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;
        ToggleSelectedVpdPolicy(false);
    }

    private void HandleVpdPolicySelectionChanged(object? sender, EventArgs e)
    {
        _ = sender;
        _ = e;
        UpdateVpdActionButtons();
    }

    private void UpdateVpdActionButtons()
    {
        VpdPolicyItem? selectedPolicy = GetSelectedVpdPolicy();
        if (selectedPolicy is null)
        {
            _vpdEnableButton.Enabled = false;
            _vpdDisableButton.Enabled = false;
            return;
        }

        bool isEnabled = string.Equals(selectedPolicy.IsEnabled, "YES", StringComparison.OrdinalIgnoreCase);
        _vpdEnableButton.Enabled = !isEnabled;
        _vpdDisableButton.Enabled = isEnabled;
    }

    private VpdPolicyItem? GetSelectedVpdPolicy()
    {
        if (_vpdPoliciesGrid.CurrentRow?.DataBoundItem is VpdPolicyItem selected)
        {
            return selected;
        }

        return null;
    }

    private void ToggleSelectedVpdPolicy(bool enable)
    {
        VpdPolicyItem? selectedPolicy = GetSelectedVpdPolicy();
        if (selectedPolicy is null)
        {
            MessageBox.Show(this, "Please select one policy first.", "VPD Policies", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        bool currentlyEnabled = string.Equals(selectedPolicy.IsEnabled, "YES", StringComparison.OrdinalIgnoreCase);
        if (currentlyEnabled == enable)
        {
            UpdateFooterStatus($"VPD: Policy {selectedPolicy.PolicyName} is already {(enable ? "enabled" : "disabled")}.");
            UpdateVpdActionButtons();
            return;
        }

        string actionText = enable ? "enable" : "disable";
        if (MessageBox.Show(
                this,
                $"Do you want to {actionText} policy {selectedPolicy.PolicyName} on {selectedPolicy.ObjectName}?",
                "VPD Policies",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        bool updated = _vpdService.SetVpdPolicyEnabled(selectedPolicy.ObjectName, selectedPolicy.PolicyName, enable);
        if (!updated)
        {
            UpdateFooterStatus("VPD: Failed to change policy state.");
            string detail = string.IsNullOrWhiteSpace(_vpdService.LastErrorMessage)
                ? "Failed to change VPD policy state."
                : _vpdService.LastErrorMessage;
            MessageBox.Show(this, detail, "VPD Policies", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        UpdateFooterStatus($"VPD: {(enable ? "Enabled" : "Disabled")} {selectedPolicy.PolicyName}.");
        RefreshVpdPolicies();
    }

    private void RefreshVpdPolicies()
    {
        try
        {
            List<VpdPolicyItem> policies = _vpdService.GetVpdPolicies();

            _vpdPoliciesGrid.DataSource = policies;
            _vpdPoliciesInfoLabel.Text = $"VPD: {policies.Count} policy";
            UpdateFooterStatus($"VPD: Loaded {policies.Count} policy records.");
            UpdateVpdActionButtons();
        }
        catch (Exception ex)
        {
            _vpdPoliciesGrid.DataSource = new List<VpdPolicyItem>();
            _vpdPoliciesInfoLabel.Text = "VPD: load failed";
            UpdateFooterStatus("VPD: Load failed.");
            UpdateVpdActionButtons();
            MessageBox.Show(this, ex.Message, "VPD Policies", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RefreshPatientUsersPage()
    {
        try
        {
            string cccdKeyword = _rbacService.NormalizeIdText(_patientSearchBox.Text);
            if (string.IsNullOrWhiteSpace(cccdKeyword))
            {
                _patientUsersGrid.DataSource = new List<object>();
                _patientPageInfoLabel.Text = "BN: Nhap CCCD de tim";
                UpdateFooterStatus("BN: Nhap CCCD de tim.");
                ShowProfileResult(null, "Chua co CCCD de tra cuu.");
                return;
            }

            if (cccdKeyword.Length != 12)
            {
                _patientUsersGrid.DataSource = new List<object>();
                _patientPageInfoLabel.Text = "BN: CCCD phai dung 12 so";
                UpdateFooterStatus("BN: CCCD phai dung 12 so.");
                ShowProfileResult(null, "CCCD phai dung 12 so moi xem duoc profile + role.");
                return;
            }

            List<PatientUserDisplayItem> users = _rbacService.GetPatientUserDisplayByCccd(cccdKeyword);
            _patientUsersGrid.DataSource = users;

            _patientPageInfoLabel.Text = $"BN: Tim thay {users.Count} tai khoan (toi da 100 dong)";
            UpdateFooterStatus($"BN: Tim thay {users.Count} tai khoan.");

            UserSecurityProfileItem? profile = _rbacService.GetPatientProfileWithRolesByCccd(cccdKeyword);
            ShowProfileResult(profile, $"Tra cuu theo CCCD: {cccdKeyword}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Users - BN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateFooterStatus("BN: Loi tra cuu.");
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
                UpdateFooterStatus("NV: Nhap CMND de tim.");
                ShowProfileResult(null, "Chua co CMND de tra cuu.");
                return;
            }

            List<StaffUserDisplayItem> users = _rbacService.GetStaffUserDisplayByCmnd(cmndKeyword);
            _staffUsersGrid.DataSource = users;

            _staffPageInfoLabel.Text = $"NV: Tim thay {users.Count} tai khoan";
            UpdateFooterStatus($"NV: Tim thay {users.Count} tai khoan.");

            string normalizedCmnd = _rbacService.NormalizeIdText(cmndKeyword);
            if (normalizedCmnd.Length == 12)
            {
                UserSecurityProfileItem? profile = _rbacService.GetStaffProfileWithRolesByCmnd(normalizedCmnd);
                ShowProfileResult(profile, $"Tra cuu theo CMND: {normalizedCmnd}");
            }
            else
            {
                ShowProfileResult(null, "CMND chua du 12 so, chi hien danh sach tim gan dung.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Users - NV", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateFooterStatus("NV: Loi tra cuu.");
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
            RefreshRoleGrid(null);
            _securityProfileTextBox.Text = $"{note}{Environment.NewLine}{Environment.NewLine}Khong tim thay profile hoac chua du dieu kien tra cuu.";
            return;
        }

        RefreshRoleGrid(profile);

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

        bool granted = _rbacService.GrantRoleToStaff(_selectedProfile.Username, roleName);
        if (!granted)
        {
            string detail = string.IsNullOrWhiteSpace(_rbacService.LastErrorMessage) ? "Grant role failed." : _rbacService.LastErrorMessage;
            MessageBox.Show(this, detail, "Grant Role", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        MessageBox.Show(this, $"Granted {roleName} to {_selectedProfile.Username}.", "Grant Role", MessageBoxButtons.OK, MessageBoxIcon.Information);
        ReloadSelectedProfile($"Cap nhat role cho user: {_selectedProfile.Username} (GRANTED)");
    }

    private void HandleRoleGridCellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        _ = sender;
        if (e.RowIndex < 0)
        {
            return;
        }

        if (_selectedProfile is null || string.IsNullOrWhiteSpace(_selectedProfile.Username))
        {
            return;
        }

        if (!string.Equals(_selectedProfile.UserType, "STAFF", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(this, "Chi cho phep revoke role voi profile STAFF.", "Revoke Role", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DataGridViewColumn? revokeColumn = _roleGrid.Columns["RevokeRole"];
        if (revokeColumn is null)
        {
            return;
        }

        int revokeColumnIndex = revokeColumn.Index;
        if (e.ColumnIndex != revokeColumnIndex)
        {
            return;
        }

        string roleName = _roleGrid.Rows[e.RowIndex].Cells["RoleName"].Value?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return;
        }

        if (MessageBox.Show(this, $"Revoke role {roleName} from {_selectedProfile.Username}?", "Revoke Role", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        bool revoked = _rbacService.RevokeRoleFromUser(_selectedProfile.Username, roleName);
        if (!revoked)
        {
            string detail = string.IsNullOrWhiteSpace(_rbacService.LastErrorMessage) ? "Revoke role failed." : _rbacService.LastErrorMessage;
            MessageBox.Show(this, detail, "Revoke Role", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        MessageBox.Show(this, $"Revoked {roleName} from {_selectedProfile.Username}.", "Revoke Role", MessageBoxButtons.OK, MessageBoxIcon.Information);
        ReloadSelectedProfile($"Cap nhat role cho user: {_selectedProfile.Username} (REVOKED)");
    }

    private void RefreshRoleGrid(UserSecurityProfileItem? profile)
    {
        _roleGrid.Rows.Clear();

        if (profile is null)
        {
            return;
        }

        foreach (string role in profile.CurrentOracleRoles)
        {
            _roleGrid.Rows.Add(role);
        }
    }

    private void ReloadSelectedProfile(string note)
    {
        if (_selectedProfile is null || string.IsNullOrWhiteSpace(_selectedProfile.IdNumber))
        {
            return;
        }

        UserSecurityProfileItem? refreshed = string.Equals(_selectedProfile.UserType, "STAFF", StringComparison.OrdinalIgnoreCase)
            ? _rbacService.GetStaffProfileWithRolesByCmnd(_selectedProfile.IdNumber)
            : _rbacService.GetPatientProfileWithRolesByCccd(_selectedProfile.IdNumber);

        ShowProfileResult(refreshed, note);
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
