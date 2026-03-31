namespace OracleDBAdmin;

using OracleDBAdmin.Services;

public partial class Form1 : Form
{
    private readonly ValidationService _validationService = new();
    private OracleConnectionService? _connectionService;
    private UserService? _userService;
    private RoleService? _roleService;
    private PermissionService? _permissionService;
    private PrivilegeService? _privilegeService;

    private readonly TextBox _dataSourceTextBox = new() { Width = 220, Text = "localhost:1521/XE" };
    private readonly TextBox _adminUserTextBox = new() { Width = 160 };
    private readonly TextBox _adminPasswordTextBox = new() { Width = 160, UseSystemPasswordChar = true };
    private readonly Label _statusLabel = new() { AutoSize = true };

    private readonly DataGridView _usersGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly TextBox _newUserTextBox = new() { Width = 140 };
    private readonly TextBox _newUserPasswordTextBox = new() { Width = 140 };

    private readonly DataGridView _rolesGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly TextBox _roleNameTextBox = new() { Width = 140 };

    private readonly TextBox _grantRoleNameTextBox = new() { Width = 120 };
    private readonly TextBox _granteeTextBox = new() { Width = 120 };
    private readonly TextBox _objectOwnerTextBox = new() { Width = 100 };
    private readonly TextBox _objectNameTextBox = new() { Width = 120 };
    private readonly ComboBox _privilegeComboBox = new() { Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _columnListTextBox = new() { Width = 160 };
    private readonly CheckBox _grantOptionCheckBox = new() { Text = "WITH GRANT OPTION", AutoSize = true };

    private readonly DataGridView _privilegesGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly TextBox _viewerGranteeTextBox = new() { Width = 140 };

    public Form1()
    {
        InitializeComponent();
        BuildUi();
    }

    private void BuildUi()
    {
        Text = "Oracle DB Admin";

        _privilegeComboBox.Items.AddRange(new object[] { "SELECT", "INSERT", "UPDATE", "DELETE", "EXECUTE" });
        _privilegeComboBox.SelectedIndex = 0;

        var connectionPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 70,
            AutoScroll = true,
            Padding = new Padding(12)
        };

        var connectButton = new Button { Text = "Connect", AutoSize = true };
        connectButton.Click += (_, _) => ConnectAdmin();

        connectionPanel.Controls.Add(new Label { Text = "Data Source", AutoSize = true });
        connectionPanel.Controls.Add(_dataSourceTextBox);
        connectionPanel.Controls.Add(new Label { Text = "Admin User", AutoSize = true });
        connectionPanel.Controls.Add(_adminUserTextBox);
        connectionPanel.Controls.Add(new Label { Text = "Password", AutoSize = true });
        connectionPanel.Controls.Add(_adminPasswordTextBox);
        connectionPanel.Controls.Add(connectButton);
        connectionPanel.Controls.Add(_statusLabel);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildUsersTab());
        tabs.TabPages.Add(BuildRolesTab());
        tabs.TabPages.Add(BuildPermissionsTab());
        tabs.TabPages.Add(BuildViewerTab());

        Controls.Add(tabs);
        Controls.Add(connectionPanel);
    }

    private TabPage BuildUsersTab()
    {
        var tab = new TabPage("Users");
        var commands = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, AutoScroll = true, Padding = new Padding(12) };
        commands.Controls.Add(new Label { Text = "Username", AutoSize = true });
        commands.Controls.Add(_newUserTextBox);
        commands.Controls.Add(new Label { Text = "Password", AutoSize = true });
        commands.Controls.Add(_newUserPasswordTextBox);

        var createButton = new Button { Text = "Create", AutoSize = true };
        createButton.Click += (_, _) => ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _userService!.CreateUser(_newUserTextBox.Text.Trim(), _newUserPasswordTextBox.Text);
            RefreshUsers();
        });

        var resetButton = new Button { Text = "Reset Password", AutoSize = true };
        resetButton.Click += (_, _) => ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _userService!.ResetPassword(_newUserTextBox.Text.Trim(), _newUserPasswordTextBox.Text);
            RefreshUsers();
        });

        var dropButton = new Button { Text = "Drop", AutoSize = true };
        dropButton.Click += (_, _) => ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _userService!.DropUser(_newUserTextBox.Text.Trim());
            RefreshUsers();
        });

        var refreshButton = new Button { Text = "Refresh", AutoSize = true };
        refreshButton.Click += (_, _) => RefreshUsers();

        commands.Controls.Add(createButton);
        commands.Controls.Add(resetButton);
        commands.Controls.Add(dropButton);
        commands.Controls.Add(refreshButton);

        tab.Controls.Add(_usersGrid);
        tab.Controls.Add(commands);
        return tab;
    }

    private TabPage BuildRolesTab()
    {
        var tab = new TabPage("Roles");
        var commands = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, AutoScroll = true, Padding = new Padding(12) };
        commands.Controls.Add(new Label { Text = "Role", AutoSize = true });
        commands.Controls.Add(_roleNameTextBox);

        var createButton = new Button { Text = "Create", AutoSize = true };
        createButton.Click += (_, _) => ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _roleService!.CreateRole(_roleNameTextBox.Text.Trim());
            RefreshRoles();
        });

        var dropButton = new Button { Text = "Drop", AutoSize = true };
        dropButton.Click += (_, _) => ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _roleService!.DropRole(_roleNameTextBox.Text.Trim());
            RefreshRoles();
        });

        var refreshButton = new Button { Text = "Refresh", AutoSize = true };
        refreshButton.Click += (_, _) => RefreshRoles();

        commands.Controls.Add(createButton);
        commands.Controls.Add(dropButton);
        commands.Controls.Add(refreshButton);

        tab.Controls.Add(_rolesGrid);
        tab.Controls.Add(commands);
        return tab;
    }

    private TabPage BuildPermissionsTab()
    {
        var tab = new TabPage("Permissions");
        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(12)
        };

        layout.Controls.Add(new Label { Text = "Grantee", AutoSize = true });
        layout.Controls.Add(_granteeTextBox);
        layout.Controls.Add(new Label { Text = "Role", AutoSize = true });
        layout.Controls.Add(_grantRoleNameTextBox);

        var grantRoleButton = new Button { Text = "Grant Role", AutoSize = true };
        grantRoleButton.Click += (_, _) => ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _permissionService!.GrantRoleToUser(_grantRoleNameTextBox.Text.Trim(), _granteeTextBox.Text.Trim());
        });
        layout.Controls.Add(grantRoleButton);

        layout.Controls.Add(new Label { Text = "Owner", AutoSize = true });
        layout.Controls.Add(_objectOwnerTextBox);
        layout.Controls.Add(new Label { Text = "Object", AutoSize = true });
        layout.Controls.Add(_objectNameTextBox);
        layout.Controls.Add(new Label { Text = "Privilege", AutoSize = true });
        layout.Controls.Add(_privilegeComboBox);
        layout.Controls.Add(new Label { Text = "Columns", AutoSize = true });
        layout.Controls.Add(_columnListTextBox);
        layout.Controls.Add(_grantOptionCheckBox);

        var grantPrivilegeButton = new Button { Text = "Grant Privilege", AutoSize = true };
        grantPrivilegeButton.Click += (_, _) => ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _permissionService!.GrantObjectPrivilege(
                _granteeTextBox.Text.Trim(),
                _objectOwnerTextBox.Text.Trim(),
                _objectNameTextBox.Text.Trim(),
                _privilegeComboBox.SelectedItem?.ToString() ?? "SELECT",
                _columnListTextBox.Text.Trim(),
                _grantOptionCheckBox.Checked);
        });

        var revokePrivilegeButton = new Button { Text = "Revoke Privilege", AutoSize = true };
        revokePrivilegeButton.Click += (_, _) => ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _permissionService!.RevokeObjectPrivilege(
                _granteeTextBox.Text.Trim(),
                _objectOwnerTextBox.Text.Trim(),
                _objectNameTextBox.Text.Trim(),
                _privilegeComboBox.SelectedItem?.ToString() ?? "SELECT",
                _columnListTextBox.Text.Trim());
        });

        layout.Controls.Add(grantPrivilegeButton);
        layout.Controls.Add(revokePrivilegeButton);
        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage BuildViewerTab()
    {
        var tab = new TabPage("Privilege Viewer");
        var commands = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, AutoScroll = true, Padding = new Padding(12) };
        commands.Controls.Add(new Label { Text = "Grantee", AutoSize = true });
        commands.Controls.Add(_viewerGranteeTextBox);

        var loadButton = new Button { Text = "Load Privileges", AutoSize = true };
        loadButton.Click += (_, _) => ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _privilegesGrid.DataSource = _privilegeService!.GetPrivileges(_viewerGranteeTextBox.Text.Trim());
        });

        commands.Controls.Add(loadButton);

        tab.Controls.Add(_privilegesGrid);
        tab.Controls.Add(commands);
        return tab;
    }

    private void ConnectAdmin()
    {
        try
        {
            string connectionString = OracleConnectionService.BuildConnectionString(
                _dataSourceTextBox.Text.Trim(),
                _adminUserTextBox.Text.Trim(),
                _adminPasswordTextBox.Text);

            _connectionService = new OracleConnectionService(connectionString);
            _connectionService.TestConnection();

            _userService = new UserService(_connectionService, _validationService);
            _roleService = new RoleService(_connectionService, _validationService);
            _permissionService = new PermissionService(_connectionService, _validationService);
            _privilegeService = new PrivilegeService(_connectionService, _validationService);

            _statusLabel.Text = "Connected.";
            RefreshUsers();
            RefreshRoles();
        }
        catch (Exception ex)
        {
            _statusLabel.Text = ex.Message;
        }
    }

    private void RefreshUsers()
    {
        ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _usersGrid.DataSource = _userService!.ListUsers();
        });
    }

    private void RefreshRoles()
    {
        ExecuteAdminAction(() =>
        {
            EnsureConnected();
            _rolesGrid.DataSource = _roleService!.ListRoles();
        });
    }

    private void EnsureConnected()
    {
        if (_connectionService is null || _userService is null || _roleService is null || _permissionService is null || _privilegeService is null)
        {
            throw new InvalidOperationException("Connect to Oracle first.");
        }
    }

    private void ExecuteAdminAction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Oracle DB Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
