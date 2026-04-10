namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;
using System.ComponentModel;


public partial class LoginForm : Form
{
    private readonly ValidationService _validationService = new();
    private readonly AuthenticationService _authenticationService = new();
    private readonly TextBox _usernameTextBox = new();
    private readonly TextBox _passwordTextBox = new() { UseSystemPasswordChar = true };
    private readonly TextBox _dataSourceTextBox = new();
    private readonly Button _loginButton = new() { Text = "Login" };
    private readonly Label _statusLabel = new() { AutoSize = true };
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Form? TargetForm { get; private set; }


    public LoginForm()
    {
        InitializeComponent();
        BuildUi();
    }

    private void BuildUi()
    {
        SuspendLayout();

        _dataSourceTextBox.Text = Environment.GetEnvironmentVariable("ORACLE_DATA_SOURCE") ?? "localhost:11521/xepdb1";
        _loginButton.Click += HandleLogin;
        AcceptButton = _loginButton;

        Font = new Font("Segoe UI", 9);
        BackColor = Color.FromArgb(236, 245, 250);
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(980, 600);
        MinimumSize = new Size(820, 560);

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(0),
            ColumnCount = 2,
            RowCount = 1
        };
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37f));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 63f));

        var leftPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(70, 140, 220),
            Padding = new Padding(36, 48, 36, 48)
        };

        var logoPanel = new Panel
        {
            Size = new Size(130, 130),
            BackColor = Color.FromArgb(108, 167, 232)
        };

        var logoLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = "+",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Arial", 88, FontStyle.Bold, GraphicsUnit.Pixel),
            ForeColor = Color.FromArgb(238, 247, 255)
        };
        logoPanel.Controls.Add(logoLabel);

        var leftTitle = new Label
        {
            Text = "Medical Data",
            AutoSize = true,
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.White,
            Margin = new Padding(0, 18, 0, 0)
        };

        var leftSubtitle = new Label
        {
            Text = "Secure Access Portal",
            AutoSize = true,
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.FromArgb(232, 242, 255),
            Margin = new Padding(0, 8, 0, 24)
        };

        var leftPoints = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            WrapContents = false,
            Margin = Padding.Empty
        };
        leftPoints.Controls.Add(new Label { Text = "•  Encrypted Connection", AutoSize = true, Font = new Font("Segoe UI", 12), ForeColor = Color.FromArgb(233, 243, 255), Margin = new Padding(0, 10, 0, 0) });
        leftPoints.Controls.Add(new Label { Text = "•  HIPAA Compliant", AutoSize = true, Font = new Font("Segoe UI", 12), ForeColor = Color.FromArgb(233, 243, 255), Margin = new Padding(0, 10, 0, 0) });
        leftPoints.Controls.Add(new Label { Text = "•  24/7 Access", AutoSize = true, Font = new Font("Segoe UI", 12), ForeColor = Color.FromArgb(233, 243, 255), Margin = new Padding(0, 10, 0, 0) });

        var leftContent = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(0, 20, 0, 0),
            ColumnCount = 1,
            RowCount = 4
        };
        leftContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        leftContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        leftContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        leftContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        leftContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        logoPanel.Anchor = AnchorStyles.None;
        leftTitle.Anchor = AnchorStyles.None;
        leftSubtitle.Anchor = AnchorStyles.None;
        leftPoints.Anchor = AnchorStyles.None;

        leftTitle.TextAlign = ContentAlignment.MiddleCenter;
        leftSubtitle.TextAlign = ContentAlignment.MiddleCenter;

        leftContent.Controls.Add(logoPanel);
        leftContent.Controls.Add(leftTitle);
        leftContent.Controls.Add(leftSubtitle);
        leftContent.Controls.Add(leftPoints);
        leftPanel.Controls.Add(leftContent);

        var rightPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(246, 246, 247),
            Padding = new Padding(22, 18, 22, 16),
            AutoScroll = true
        };

        var rightCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(26, 24, 26, 20)
        };

        var rightLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 12,
            Margin = Padding.Empty,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        rightLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
        rightLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var heading = new Label
        {
            Text = "Hospital Management System",
            AutoSize = true,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = Color.FromArgb(16, 34, 58),
            Margin = Padding.Empty,
            Anchor = AnchorStyles.None
        };

        var subHeading = new Label
        {
            Text = "Sign in to access patient records and medical data",
            AutoSize = true,
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.FromArgb(86, 96, 112),
            Margin = new Padding(0, 2, 0, 10),
            Anchor = AnchorStyles.None
        };

        var usernameLabel = new Label { Text = "Oracle Username", AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(35, 47, 65), Margin = Padding.Empty };
        var passwordLabel = new Label { Text = "Password", AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(35, 47, 65), Margin = Padding.Empty };
        var dataSourceLabel = new Label { Text = "Data Source", AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(35, 47, 65), Margin = Padding.Empty };

        _usernameTextBox.Dock = DockStyle.Fill;
        _usernameTextBox.Font = new Font("Segoe UI", 11);
        _usernameTextBox.Margin = Padding.Empty;
        _usernameTextBox.PlaceholderText = "Enter your username";

        _passwordTextBox.Dock = DockStyle.Fill;
        _passwordTextBox.Font = new Font("Segoe UI", 11);
        _passwordTextBox.Margin = Padding.Empty;
        _passwordTextBox.PlaceholderText = "Enter your password";

        var togglePasswordButton = new Button
        {
            Text = "👁",
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            Margin = Padding.Empty,
            BackColor = Color.FromArgb(248, 250, 252),
            Font = new Font("Segoe UI Emoji", 10)
        };
        togglePasswordButton.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        togglePasswordButton.Click += (_, _) =>
        {
            _passwordTextBox.UseSystemPasswordChar = !_passwordTextBox.UseSystemPasswordChar;
            togglePasswordButton.Text = _passwordTextBox.UseSystemPasswordChar ? "👁" : "🙈";
        };

        var passwordPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        passwordPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        passwordPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));
        passwordPanel.Controls.Add(_passwordTextBox, 0, 0);
        passwordPanel.Controls.Add(togglePasswordButton, 1, 0);

        _dataSourceTextBox.Dock = DockStyle.Fill;
        _dataSourceTextBox.Font = new Font("Segoe UI", 11);
        _dataSourceTextBox.Margin = Padding.Empty;
        _dataSourceTextBox.PlaceholderText = "localhost:11521/xepdb1";

        _loginButton.Dock = DockStyle.Fill;
        _loginButton.Height = 44;
        _loginButton.Margin = new Padding(0, 10, 0, 0);
        _loginButton.FlatStyle = FlatStyle.Flat;
        _loginButton.FlatAppearance.BorderSize = 0;
        _loginButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        _loginButton.BackColor = Color.FromArgb(71, 136, 219);
        _loginButton.ForeColor = Color.White;
        _loginButton.Cursor = Cursors.Hand;

        _statusLabel.AutoSize = true;
        _statusLabel.Font = new Font("Segoe UI", 10);
        _statusLabel.ForeColor = Color.FromArgb(184, 49, 47);
        _statusLabel.Margin = new Padding(0, 5, 0, 0);
        _statusLabel.MaximumSize = new Size(540, 0);

        var footer = new Label
        {
            Text = "Protected by enterprise-grade encryption.\nFor assistance, contact IT Support at ext. 4000",
            AutoSize = true,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(95, 103, 118),
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.Top,
            Margin = new Padding(0, 10, 0, 0)
        };

        var separator = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 1,
            BackColor = Color.FromArgb(215, 216, 222),
            Margin = new Padding(0, 16, 0, 10)
        };

        rightLayout.Controls.Add(heading, 0, 0);
        rightLayout.Controls.Add(subHeading, 0, 1);
        rightLayout.Controls.Add(usernameLabel, 0, 2);
        rightLayout.Controls.Add(_usernameTextBox, 0, 3);
        rightLayout.Controls.Add(passwordLabel, 0, 4);
        rightLayout.Controls.Add(passwordPanel, 0, 5);
        rightLayout.Controls.Add(dataSourceLabel, 0, 6);
        rightLayout.Controls.Add(_dataSourceTextBox, 0, 7);
        rightLayout.Controls.Add(_loginButton, 0, 8);
        rightLayout.Controls.Add(_statusLabel, 0, 9);
        rightLayout.Controls.Add(separator, 0, 10);
        rightLayout.Controls.Add(footer, 0, 11);

        rightCard.Controls.Add(rightLayout);
        rightPanel.Controls.Add(rightCard);

        rootLayout.Controls.Add(leftPanel, 0, 0);
        rootLayout.Controls.Add(rightPanel, 1, 0);

        Controls.Add(rootLayout);
        ResumeLayout();
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
                "COORDINATOR" => new CoordinatorForm(session),
                "DOCTOR" => new DoctorForm(session),
                "TECHNICIAN" => new TechnicianForm(session),
                "PATIENT" => new PatientForm(session),
                _ => throw new InvalidOperationException($"Unsupported role: {session.Role}")
            };

            TargetForm = nextForm;
            DialogResult = DialogResult.OK;
            Close();
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
