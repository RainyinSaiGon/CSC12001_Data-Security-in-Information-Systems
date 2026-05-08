namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public class OLSService
{
    private const string OlsPolicyName = "THONGBAO_OLS";
    private readonly OracleConnectionService _connectionService;
    public string LastErrorMessage { get; private set; } = string.Empty;

    public OLSService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public List<OlsLabelOption> GetSupportedLabelOptions()
    {
        return new List<OlsLabelOption>
        {
            new() { LabelCode = string.Empty, LabelTag = null, DisplayText = "(Tat ca nhan)" },
            new() { LabelCode = "L1_NV", LabelTag = 1000, DisplayText = "Gui den toan bo Nhan Vien" },
            new() { LabelCode = "L2_LD", LabelTag = 2000, DisplayText = "Gui den cac Lanh Dao Khoa" },
            new() { LabelCode = "L3_GD", LabelTag = 3000, DisplayText = "Gui den Ban Giam Doc" },
            new() { LabelCode = "L2_LD:C_TIEU", LabelTag = 2100, DisplayText = "Gui den Lanh Dao Khoa tieu hoa" },
            new() { LabelCode = "L1_NV:C_TIEU:G_HCM", LabelTag = 1130, DisplayText = "Gui den Nhan Vien Khoa tieu hoa tai Ho Chi Minh" },
            new() { LabelCode = "L1_NV:C_TIEU:G_HN", LabelTag = 1110, DisplayText = "Gui den Nhan Vien Khoa tieu hoa tai Ha Noi" },
            new() { LabelCode = "L2_LD:C_TIEU,C_THAN:G_HP", LabelTag = 2220, DisplayText = "Gui den Lanh Dao Khoa tieu hoa va than kinh tai Hai Phong" }
        };
    }

    public List<OlsNotificationDisplayItem> GetAllNotificationsForAdminDisplay(int? filterLabelTag = null)
    {
        LastErrorMessage = string.Empty;
        try
        {
            return _connectionService.Execute(connection =>
            {
                using var command = connection.CreateCommand();
                string sql = """
                    SELECT
                        t.MATHONGBAO,
                        t.NOIDUNG,
                        t.NGAYGIO,
                        t.DIADIEM,
                        t.OLS_LABEL AS OLS_LABEL_TAG,
                        CASE
                            WHEN t.OLS_LABEL = 1000 THEN 'L1_NV'
                            WHEN t.OLS_LABEL = 2000 THEN 'L2_LD'
                            WHEN t.OLS_LABEL = 3000 THEN 'L3_GD'
                            WHEN t.OLS_LABEL = 2100 THEN 'L2_LD:C_TIEU'
                            WHEN t.OLS_LABEL = 1130 THEN 'L1_NV:C_TIEU:G_HCM'
                            WHEN t.OLS_LABEL = 1110 THEN 'L1_NV:C_TIEU:G_HN'
                            WHEN t.OLS_LABEL = 2220 THEN 'L2_LD:C_TIEU,C_THAN:G_HP'
                            ELSE 'UNKNOWN'
                        END AS OLS_LABEL_TEXT
                    FROM HOSPITAL_ADMIN.THONGBAO t
                    """;

                if (filterLabelTag.HasValue)
                {
                    sql += " WHERE t.OLS_LABEL = :filterLabelTag ";
                }

                sql += " ORDER BY t.NGAYGIO DESC, t.MATHONGBAO DESC";

                command.CommandText = sql;
                if (filterLabelTag.HasValue)
                {
                    command.Parameters.Add(new OracleParameter("filterLabelTag", filterLabelTag.Value));
                }

                using var reader = command.ExecuteReader();
                var items = new List<OlsNotificationDisplayItem>();
                while (reader.Read())
                {
                    string labelCode = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                    items.Add(new OlsNotificationDisplayItem
                    {
                        MaThongBao = reader.GetInt32(0),
                        NoiDung = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        NgayGio = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2),
                        DiaDiem = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        OlsLabel = BuildLabelCodeWithTag(labelCode, reader.IsDBNull(4) ? null : (int?)reader.GetDecimal(4)),
                        DoiTuongDuocThongBao = TranslateLabelToVietnamese(labelCode)
                    });
                }

                return items;
            });
        }
        catch (OracleException ex)
        {
            LastErrorMessage = $"Oracle error {ex.Number}: {ex.Message}";
            return new List<OlsNotificationDisplayItem>();
        }
        catch (Exception ex)
        {
            LastErrorMessage = ex.Message;
            return new List<OlsNotificationDisplayItem>();
        }
    }

    public bool CreateNotificationAsAdmin(string noiDung, DateTime ngayGio, string diaDiem, string labelCode)
    {
        LastErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(noiDung) || string.IsNullOrWhiteSpace(diaDiem) || string.IsNullOrWhiteSpace(labelCode))
        {
            LastErrorMessage = "Noi dung, dia diem va OLS label la bat buoc.";
            return false;
        }

        try
        {
            return _connectionService.Execute(connection =>
            {
                if (!IsHospitalAdminSession(connection))
                {
                    LastErrorMessage = "Chi HOSPITAL_ADMIN moi duoc tao thong bao trong OLS tab.";
                    return false;
                }

                using var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO HOSPITAL_ADMIN.THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
                    VALUES (
                        :noiDung,
                        :ngayGio,
                        :diaDiem,
                        CHAR_TO_LABEL(:policyName, :labelCode)
                    )
                    """;
                command.Parameters.Add(new OracleParameter("noiDung", noiDung.Trim()));
                command.Parameters.Add(new OracleParameter("ngayGio", ngayGio));
                command.Parameters.Add(new OracleParameter("diaDiem", diaDiem.Trim()));
                command.Parameters.Add(new OracleParameter("policyName", OlsPolicyName));
                command.Parameters.Add(new OracleParameter("labelCode", labelCode.Trim().ToUpperInvariant()));

                return command.ExecuteNonQuery() == 1;
            });
        }
        catch (OracleException ex)
        {
            LastErrorMessage = $"Oracle error {ex.Number}: {ex.Message}";
            return false;
        }
        catch (Exception ex)
        {
            LastErrorMessage = ex.Message;
            return false;
        }
    }

    public List<Notification> GetAccessibleNotificationsDetailed()
    {
        LastErrorMessage = string.Empty;
        try
        {
            return _connectionService.Execute(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT MATHONGBAO, NOIDUNG, NGAYGIO, DIADIEM
                    FROM HOSPITAL_ADMIN.THONGBAO
                    ORDER BY NGAYGIO DESC
                    FETCH FIRST 200 ROWS ONLY
                    """;

                using var reader = command.ExecuteReader();
                var items = new List<Notification>();
                while (reader.Read())
                {
                    items.Add(new Notification
                    {
                        MATHONGBAO = reader.GetInt32(0),
                        NOIDUNG = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        NGAYGIO = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2),
                        DIADIEM = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                    });
                }

                return items;
            });
        }
        catch (OracleException ex)
        {
            LastErrorMessage = $"Oracle error {ex.Number}: {ex.Message}";
            return new List<Notification>();
        }
        catch (Exception ex)
        {
            LastErrorMessage = ex.Message;
            return new List<Notification>();
        }
    }

    private static bool IsHospitalAdminSession(OracleConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT UPPER(SYS_CONTEXT('USERENV','SESSION_USER')) FROM DUAL";
        string sessionUser = command.ExecuteScalar()?.ToString() ?? string.Empty;
        return string.Equals(sessionUser, "HOSPITAL_ADMIN", StringComparison.OrdinalIgnoreCase);
    }

    private static string TranslateLabelToVietnamese(string labelCode)
    {
        string normalized = (labelCode ?? string.Empty).Trim().ToUpperInvariant();
        return normalized switch
        {
            "L1_NV" => "Gui den toan bo nhan vien",
            "L2_LD" => "Gui den cac lanh dao khoa",
            "L3_GD" => "Gui den toan bo Ban giam doc",
            "L2_LD:C_TIEU" => "Gui den lanh dao Khoa tieu hoa",
            "L1_NV:C_TIEU:G_HCM" => "Gui den nhan vien Khoa tieu hoa o Ho Chi Minh",
            "L1_NV:C_TIEU:G_HN" => "Gui den nhan vien Khoa tieu hoa o Ha Noi",
            "L2_LD:C_TIEU,C_THAN:G_HP" => "Gui den lanh dao Khoa tieu hoa va Khoa than kinh tai Hai Phong",
            _ => "Nhan khac / chua dinh nghia trong mau"
        };
    }

    private static string BuildLabelCodeWithTag(string labelCode, int? labelTag)
    {
        string normalizedCode = (labelCode ?? string.Empty).Trim().ToUpperInvariant();
        if (!labelTag.HasValue)
        {
            return normalizedCode;
        }

        if (string.IsNullOrWhiteSpace(normalizedCode) || normalizedCode == "UNKNOWN")
        {
            return $"UNKNOWN - {labelTag.Value}";
        }

        return $"{labelTag.Value} - {normalizedCode}";
    }
}

public sealed class OlsNotificationDisplayItem
{
    public int MaThongBao { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public DateTime NgayGio { get; set; }
    public string DiaDiem { get; set; } = string.Empty;
    public string OlsLabel { get; set; } = string.Empty;
    public string DoiTuongDuocThongBao { get; set; } = string.Empty;
}

public sealed class OlsLabelOption
{
    public string LabelCode { get; set; } = string.Empty;
    public int? LabelTag { get; set; }
    public string DisplayText { get; set; } = string.Empty;

    public override string ToString()
    {
        return DisplayText;
    }
}
