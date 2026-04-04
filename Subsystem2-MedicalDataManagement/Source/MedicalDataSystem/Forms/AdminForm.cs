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
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
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
        tabControl.TabPages.Add(new TabPage("RBAC"));
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
            Dock = DockStyle.Bottom,
            Height = 32,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0),
            Text = $"Logged in as: {_session.Username}"
        };

        Controls.Add(tabControl);
        Controls.Add(statusLabel);

        _ = _connectionService;
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

        tab.Controls.Add(usersSubTabs);
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
            string cccdKeyword = _patientSearchBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(cccdKeyword))
            {
                _patientUsersGrid.DataSource = new List<object>();
                _patientPageInfoLabel.Text = "BN: Nhap CCCD de tim";
                return;
            }

            List<UserAccountItem> users = _userService.GetPatientUsersByCccd(cccdKeyword);

            _patientUsersGrid.DataSource = users
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

            _patientPageInfoLabel.Text = $"BN: Tim thay {users.Count} tai khoan";
        }
        catch (OracleException ex)
        {
            MessageBox.Show(this, $"Oracle error {ex.Number}: {ex.Message}", "Users - BN", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Users - BN", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }
        catch (OracleException ex)
        {
            MessageBox.Show(this, $"Oracle error {ex.Number}: {ex.Message}", "Users - NV", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Users - NV", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
