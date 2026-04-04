namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class LoginForm : Form
{
    private readonly ValidationService _validationService = new();
    private readonly AuthenticationService _authenticationService = new();
    private readonly TextBox _usernameTextBox = new() { Width = 220 };
    private readonly TextBox _passwordTextBox = new() { Width = 220, UseSystemPasswordChar = true };
    private readonly TextBox _dataSourceTextBox = new() { Width = 220 };
    private readonly Button _loginButton = new() { Text = "Login", AutoSize = true };
    private readonly Label _statusLabel = new() { AutoSize = true };

    public LoginForm()
    {
        InitializeComponent();
        BuildUi();
    }

    private void BuildUi()
    {
        _dataSourceTextBox.Text = Environment.GetEnvironmentVariable("ORACLE_DATA_SOURCE") ?? "localhost:1521/XE";
        _loginButton.Click += HandleLogin;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(24),
            AutoSize = true
        };

        layout.Controls.Add(new Label { Text = "Oracle Username", AutoSize = true }, 0, 0);
        layout.Controls.Add(_usernameTextBox, 1, 0);
        layout.Controls.Add(new Label { Text = "Password", AutoSize = true }, 0, 1);
        layout.Controls.Add(_passwordTextBox, 1, 1);
        layout.Controls.Add(new Label { Text = "Data Source", AutoSize = true }, 0, 2);
        layout.Controls.Add(_dataSourceTextBox, 1, 2);
        layout.Controls.Add(_loginButton, 1, 3);
        layout.Controls.Add(_statusLabel, 1, 4);

        Controls.Add(layout);
    }

    private void HandleLogin(object? sender, EventArgs e)
    {
        _ = sender;

        string username = _usernameTextBox.Text.Trim();
        string password = _passwordTextBox.Text;
        string dataSource = _dataSourceTextBox.Text.Trim();

        if (!_validationService.ValidateUsername(username))
        {
            ShowStatus("Enter a valid Oracle username.");
            return;
        }

        if (!_validationService.ValidatePassword(password))
        {
            ShowStatus("Enter the Oracle password.");
            return;
        }

        try
        {
            UserSession session = _authenticationService.Authenticate(username, password, dataSource);
            Form nextForm = session.Role switch
            {
                "ADMIN" => new AdminForm(session),
                "COORDINATOR" => new CoordinatorForm(session),
                "DOCTOR" => new DoctorForm(session),
                "TECHNICIAN" => new TechnicianForm(session),
                "PATIENT" => new PatientForm(session),
                _ => throw new InvalidOperationException($"Unsupported role: {session.Role}")
            };

            Hide();
            nextForm.FormClosed += (_, _) => Close();
            nextForm.Show();
        }
        catch (Exception ex)
        {
            ShowStatus(ex.Message);
        }
    }

    private void ShowStatus(string message)
    {
        _statusLabel.Text = message;
    }
}
