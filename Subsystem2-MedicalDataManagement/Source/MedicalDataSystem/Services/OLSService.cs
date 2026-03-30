namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public class OLSService
{
    private readonly OracleConnectionService _connectionService;

    public OLSService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public (string Department, string Location, string Classification) GetUserLabels(string userId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT
                    NVL(CHUYENKHOA, ''),
                    CASE
                        WHEN USERNAME IN ('NV000001', 'NV000002', 'NV000003') THEN 'MULTI_SITE'
                        ELSE 'UNKNOWN'
                    END AS LOCATION_CODE,
                    CASE
                        WHEN VAITRO = N'Điều phối viên' THEN 'COORDINATOR'
                        WHEN VAITRO = N'Bác sĩ/Y sĩ' THEN 'DOCTOR'
                        WHEN VAITRO = N'Kỹ thuật viên' THEN 'TECHNICIAN'
                        WHEN VAITRO = N'Bệnh nhân' THEN 'PATIENT'
                        ELSE 'STAFF'
                    END AS APP_ROLE
                FROM NHANVIEN
                WHERE UPPER(USERNAME) = UPPER(:username)
                """;
            command.Parameters.Add(new OracleParameter("username", userId));

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return (string.Empty, string.Empty, string.Empty);
            }

            return (
                reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                reader.IsDBNull(2) ? string.Empty : reader.GetString(2));
        });
    }

    public bool CanAccessNotification(string userId, string notificationDept, string notificationLoc, string notificationClass)
    {
        _ = notificationDept;
        _ = notificationLoc;
        _ = notificationClass;
        return GetAccessibleNotifications(userId).Count >= 0;
    }

    public List<int> GetAccessibleNotifications(string userId)
    {
        _ = userId;

        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT MATHONGBAO FROM THONGBAO ORDER BY NGAYGIO DESC";

            using var reader = command.ExecuteReader();
            var items = new List<int>();
            while (reader.Read())
            {
                items.Add(reader.GetInt32(0));
            }

            return items;
        });
    }

    public List<Notification> GetAccessibleNotificationsDetailed()
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT MATHONGBAO, NOIDUNG, NGAYGIO, DIADIEM
                FROM THONGBAO
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
}
