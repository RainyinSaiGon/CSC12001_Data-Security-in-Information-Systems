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
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    };
    private readonly DataGridView _staffUsersGrid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    };

    private readonly Label _patientPageInfoLabel = new() { AutoSize = true, Padding = new Padding(6, 8, 6, 0) };
    private readonly Button _patientPrevButton = new() { Text = "Trang trước", Width = 110, Height = 30 };
    private readonly Button _patientNextButton = new() { Text = "Trang sau", Width = 100, Height = 30 };
    private readonly Button _patientRefreshButton = new() { Text = "Làm mới", Width = 90, Height = 30 };

    private readonly Label _staffPageInfoLabel = new() { AutoSize = true, Padding = new Padding(6, 8, 6, 0) };
    private readonly Button _staffPrevButton = new() { Text = "Trang trước", Width = 110, Height = 30 };
    private readonly Button _staffNextButton = new() { Text = "Trang sau", Width = 100, Height = 30 };
    private readonly Button _staffRefreshButton = new() { Text = "Làm mới", Width = 90, Height = 30 };

    private const int PageSize = 20;
    private int _patientCurrentPage = 1;
    private int _patientTotalPages = 1;
    private int _patientTotalUsers;

    private int _staffCurrentPage = 1;
    private int _staffTotalPages = 1;
    private int _staffTotalUsers;

    public AdminForm(UserSession session)
    {
        _session = session;
        _connectionService = new OracleConnectionService(session.ConnectionString);
        _userService = new UserService(_connectionService);
        BuildUi();
        RefreshPatientUsersPage();
        RefreshStaffUsersPage();
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
                RefreshPatientUsersPage();
                RefreshStaffUsersPage();
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

        var pagingPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            Padding = new Padding(8, 4, 8, 4),
            WrapContents = false
        };

        _patientPrevButton.Click += (_, _) =>
        {
            if (_patientCurrentPage <= 1)
            {
                return;
            }

            _patientCurrentPage--;
            RefreshPatientUsersPage();
        };

        _patientNextButton.Click += (_, _) =>
        {
            if (_patientCurrentPage >= _patientTotalPages)
            {
                return;
            }

            _patientCurrentPage++;
            RefreshPatientUsersPage();
        };

        _patientRefreshButton.Click += (_, _) => RefreshPatientUsersPage();

        pagingPanel.Controls.Add(_patientPrevButton);
        pagingPanel.Controls.Add(_patientNextButton);
        pagingPanel.Controls.Add(_patientRefreshButton);
        pagingPanel.Controls.Add(_patientPageInfoLabel);

        tab.Controls.Add(_patientUsersGrid);
        tab.Controls.Add(pagingPanel);
        return tab;
    }

    private TabPage BuildStaffUsersSubTab()
    {
        var tab = new TabPage("NV");

        var pagingPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            Padding = new Padding(8, 4, 8, 4),
            WrapContents = false
        };

        _staffPrevButton.Click += (_, _) =>
        {
            if (_staffCurrentPage <= 1)
            {
                return;
            }

            _staffCurrentPage--;
            RefreshStaffUsersPage();
        };

        _staffNextButton.Click += (_, _) =>
        {
            if (_staffCurrentPage >= _staffTotalPages)
            {
                return;
            }

            _staffCurrentPage++;
            RefreshStaffUsersPage();
        };

        _staffRefreshButton.Click += (_, _) => RefreshStaffUsersPage();

        pagingPanel.Controls.Add(_staffPrevButton);
        pagingPanel.Controls.Add(_staffNextButton);
        pagingPanel.Controls.Add(_staffRefreshButton);
        pagingPanel.Controls.Add(_staffPageInfoLabel);

        tab.Controls.Add(_staffUsersGrid);
        tab.Controls.Add(pagingPanel);
        return tab;
    }

    private void RefreshPatientUsersPage()
    {
        try
        {
            (List<UserAccountItem> users, int totalCount) = _userService.GetPatientUsersPage(_patientCurrentPage, PageSize);
            _patientTotalUsers = totalCount;
            _patientTotalPages = Math.Max(1, (int)Math.Ceiling(_patientTotalUsers / (double)PageSize));

            if (_patientCurrentPage > _patientTotalPages)
            {
                _patientCurrentPage = _patientTotalPages;
                (users, totalCount) = _userService.GetPatientUsersPage(_patientCurrentPage, PageSize);
                _patientTotalUsers = totalCount;
            }

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

            _patientPageInfoLabel.Text = $"BN: Trang {_patientCurrentPage}/{_patientTotalPages} - Tổng: {_patientTotalUsers} tài khoản";
            _patientPrevButton.Enabled = _patientCurrentPage > 1;
            _patientNextButton.Enabled = _patientCurrentPage < _patientTotalPages;
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
            (List<UserAccountItem> users, int totalCount) = _userService.GetStaffUsersPage(_staffCurrentPage, PageSize);
            _staffTotalUsers = totalCount;
            _staffTotalPages = Math.Max(1, (int)Math.Ceiling(_staffTotalUsers / (double)PageSize));

            if (_staffCurrentPage > _staffTotalPages)
            {
                _staffCurrentPage = _staffTotalPages;
                (users, totalCount) = _userService.GetStaffUsersPage(_staffCurrentPage, PageSize);
                _staffTotalUsers = totalCount;
            }

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

            _staffPageInfoLabel.Text = $"NV: Trang {_staffCurrentPage}/{_staffTotalPages} - Tổng: {_staffTotalUsers} tài khoản";
            _staffPrevButton.Enabled = _staffCurrentPage > 1;
            _staffNextButton.Enabled = _staffCurrentPage < _staffTotalPages;
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
