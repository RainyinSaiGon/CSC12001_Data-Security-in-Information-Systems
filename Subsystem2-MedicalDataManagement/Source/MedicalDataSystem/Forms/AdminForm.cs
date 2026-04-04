namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public class AdminForm : Form
{
    private readonly UserSession _session;
    private readonly OracleConnectionService _connectionService;

    public AdminForm(UserSession session)
    {
        _session = session;
        _connectionService = new OracleConnectionService(session.ConnectionString);
        BuildUi();
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

        tabControl.TabPages.Add(new TabPage("Users"));
        tabControl.TabPages.Add(new TabPage("RBAC"));
        tabControl.TabPages.Add(new TabPage("VPD Policies"));
        tabControl.TabPages.Add(new TabPage("OLS Labels"));
        tabControl.TabPages.Add(new TabPage("Audit Log"));

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
}
