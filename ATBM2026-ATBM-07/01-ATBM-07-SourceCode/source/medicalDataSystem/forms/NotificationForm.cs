namespace MedicalDataSystem.Forms;

using MedicalDataSystem.Models;
using MedicalDataSystem.Services;

public partial class NotificationForm : Form
{
    private readonly UserSession _session;
    private readonly OLSService _olsService;
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };

    public NotificationForm(UserSession session)
    {
        _session = session;
        _olsService = new OLSService(new OracleConnectionService(session.ConnectionString));
        InitializeComponent();
        BuildUi();
        LoadNotifications();
    }

    private void BuildUi()
    {
        var header = new Label
        {
            Dock = DockStyle.Top,
            Height = 36,
            Text = $"Accessible notifications for {_session.FullName} ({_session.Role})",
            TextAlign = ContentAlignment.MiddleLeft
        };

        _grid.DataBindingComplete += (_, _) => ApplyVietnameseHeaders(_grid);

        Controls.Add(_grid);
        Controls.Add(header);
    }

    private void LoadNotifications()
    {
        try
        {
            List<Notification> notifications = _olsService.GetAccessibleNotificationsDetailed();
            _grid.DataSource = notifications;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Notifications", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void ApplyVietnameseHeaders(DataGridView grid)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["MATHONGBAO"] = "Mã Thông Báo",
            ["NOIDUNG"] = "Nội Dung",
            ["NGAYGIO"] = "Ngày Giờ",
            ["DIADIEM"] = "Địa Điểm"
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
}
