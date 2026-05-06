namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public class CoordinatorService
{
    private readonly OracleConnectionService _connectionService;

    public CoordinatorService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public List<Patient> GetAllPatients()
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT MABN, TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN,
                       TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC, USERNAME
                FROM BENHNHAN
                ORDER BY MABN DESC
                """;

            using var reader = command.ExecuteReader();
            var items = new List<Patient>();
            while (reader.Read())
            {
                items.Add(MapPatient(reader));
            }

            return items;
        });
    }

    public bool AddPatient(Patient patient)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                BEGIN
                    HOSPITAL_ADMIN.SP_ADD_PATIENT(
                        :tenbn, :phai, TRUNC(:ngaysinh), :cccd, :sonha, :tenduong, 
                        :quanhuyen, :tinhtp, :tiensubenh, :tiensubenhgd, :diungthuoc
                    );
                END;
                """;
            string normalizedCccd = patient.CCCD.Trim();
            command.Parameters.Add(new OracleParameter("tenbn", patient.TENBN));
            command.Parameters.Add(new OracleParameter("phai", patient.PHAI));
            command.Parameters.Add(new OracleParameter("ngaysinh", patient.NGAYSINH));
            command.Parameters.Add(new OracleParameter("cccd", normalizedCccd));
            command.Parameters.Add(new OracleParameter("sonha", patient.SONHA));
            command.Parameters.Add(new OracleParameter("tenduong", patient.TENDUONG));
            command.Parameters.Add(new OracleParameter("quanhuyen", patient.QUANHUYEN));
            command.Parameters.Add(new OracleParameter("tinhtp", patient.TINHTP));
            command.Parameters.Add(new OracleParameter("tiensubenh", patient.TIENSUBENH));
            command.Parameters.Add(new OracleParameter("tiensubenhgd", patient.TIENSUBENHGD));
            command.Parameters.Add(new OracleParameter("diungthuoc", patient.DIUNGTHUOC));
            command.ExecuteNonQuery();
            return true;
        });
    }

    public bool EditPatient(Patient patient)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE BENHNHAN
                SET TENBN = :tenbn,
                    PHAI = :phai,
                    NGAYSINH = TRUNC(:ngaysinh),
                    CCCD = :cccd,
                    USERNAME = :username,
                    SONHA = :sonha,
                    TENDUONG = :tenduong,
                    QUANHUYEN = :quanhuyen,
                    TINHTP = :tinhtp,
                    TIENSUBENH = :tiensubenh,
                    TIENSUBENHGD = :tiensubenhgd,
                    DIUNGTHUOC = :diungthuoc
                WHERE MABN = :mabn
                """;
            command.Parameters.Add(new OracleParameter("tenbn", patient.TENBN));
            command.Parameters.Add(new OracleParameter("phai", patient.PHAI));
            command.Parameters.Add(new OracleParameter("ngaysinh", patient.NGAYSINH));
            command.Parameters.Add(new OracleParameter("cccd", patient.CCCD));
            command.Parameters.Add(new OracleParameter("username", patient.CCCD));
            command.Parameters.Add(new OracleParameter("sonha", patient.SONHA));
            command.Parameters.Add(new OracleParameter("tenduong", patient.TENDUONG));
            command.Parameters.Add(new OracleParameter("quanhuyen", patient.QUANHUYEN));
            command.Parameters.Add(new OracleParameter("tinhtp", patient.TINHTP));
            command.Parameters.Add(new OracleParameter("tiensubenh", patient.TIENSUBENH));
            command.Parameters.Add(new OracleParameter("tiensubenhgd", patient.TIENSUBENHGD));
            command.Parameters.Add(new OracleParameter("diungthuoc", patient.DIUNGTHUOC));
            command.Parameters.Add(new OracleParameter("mabn", patient.MABN));
            return command.ExecuteNonQuery() == 1;
        });
    }

    public bool AssignDoctorToPatient(string doctorId, string patientId)
    {
        return CreateMedicalRecord(patientId, doctorId, string.Empty);
    }

    public bool AssignTechnicianToService(string technicianId, string serviceId)
    {
        string[] parts = serviceId.Split('|', StringSplitOptions.TrimEntries);
        if (parts.Length != 3)
        {
            return false;
        }

        return AssignTechnician(int.Parse(parts[0]), parts[1], DateTime.Parse(parts[2]), technicianId);
    }

    public bool CreateMedicalRecord(string patientId, string doctorId, string departmentCode)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO HSBA (MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA)
                VALUES (:mabn, SYSDATE, NULL, NULL, NULL, :mabs, :makhoa)
                """;
            command.Parameters.Add(new OracleParameter("mabn", patientId));
            command.Parameters.Add(new OracleParameter("mabs", doctorId));
            command.Parameters.Add(new OracleParameter("makhoa", string.IsNullOrWhiteSpace(departmentCode) ? DBNull.Value : departmentCode));
            return command.ExecuteNonQuery() == 1;
        });
    }

    public bool AssignTechnician(int medicalRecordId, string serviceType, DateTime serviceDate, string technicianId)
    {
        return _connectionService.Execute(connection =>
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[AssignTechnician] Bắt đầu. MAHSBA: {medicalRecordId}, LOAIDV: {serviceType}, NGAYDV: {serviceDate:yyyy-MM-dd}, MAKTV: {technicianId}");

                using var cmdUpdate = connection.CreateCommand();
                cmdUpdate.BindByName = true;
                cmdUpdate.CommandText = """
                    UPDATE HSBA_DV 
                    SET MAKTV = :maktv 
                    WHERE MAHSBA = :mahsba AND LOAIDV = :loaidv AND TRUNC(NGAYDV) = TRUNC(:ngaydv)
                    """;
                cmdUpdate.Parameters.Add(new OracleParameter("maktv", OracleDbType.Varchar2, technicianId, System.Data.ParameterDirection.Input));
                cmdUpdate.Parameters.Add(new OracleParameter("mahsba", OracleDbType.Int32, medicalRecordId, System.Data.ParameterDirection.Input));
                cmdUpdate.Parameters.Add(new OracleParameter("loaidv", OracleDbType.NVarchar2, serviceType, System.Data.ParameterDirection.Input));
                cmdUpdate.Parameters.Add(new OracleParameter("ngaydv", OracleDbType.Date, serviceDate, System.Data.ParameterDirection.Input));
                
                System.Diagnostics.Debug.WriteLine("[AssignTechnician] Đang thực thi lệnh UPDATE...");
                int updated = cmdUpdate.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine($"[AssignTechnician] UPDATE thành công. Số dòng ảnh hưởng: {updated}");

                if (updated == 0)
                {
                    throw new Exception("Dịch vụ này chưa tồn tại trong Hồ sơ bệnh án!\n\n💡 LƯU Ý QUY TRÌNH (TC#2 & TC#3):\nĐiều phối viên KHÔNG có quyền tự tạo Dịch vụ. Bác sĩ phải là người 'Chỉ định Dịch vụ' trước, sau đó Điều phối viên mới được phép quay lại form này để cập nhật phân công Kỹ thuật viên.");
                }
                return true;
            }
            catch (OracleException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AssignTechnician] OracleException lỗi số ({ex.Number}): {ex.Message}");
                throw new Exception($"Lỗi CSDL Oracle ({ex.Number}): {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AssignTechnician] Exception chung: {ex.Message}");
                throw;
            }
        });
    }

    public List<DiagnosticService> GetUnassignedServices(int medicalRecordId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV
                FROM HSBA_DV
                WHERE MAHSBA = :mahsba AND MAKTV IS NULL
                ORDER BY NGAYDV DESC
                """;
            command.Parameters.Add(new OracleParameter("mahsba", medicalRecordId));

            using var reader = command.ExecuteReader();
            var list = new List<DiagnosticService>();
            while (reader.Read())
            {
                list.Add(new DiagnosticService
                {
                    MAHSBA = reader.GetInt32(0),
                    LOAIDV = reader.GetString(1),
                    NGAYDV = reader.GetDateTime(2),
                    KETQUA = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    MAKTV = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                });
            }
            return list;
        });
    }

    public List<Staff> GetDoctors()
    {
        return GetStaffByAppRole("DOCTOR");
    }

    public List<Staff> GetTechnicians()
    {
        return GetStaffByAppRole("TECHNICIAN");
    }

    public List<int> GetMedicalRecordsByPatient(string patientId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT MAHSBA FROM HSBA WHERE MABN = :mabn ORDER BY MAHSBA DESC
                """;
            command.Parameters.Add(new OracleParameter("mabn", patientId));

            using var reader = command.ExecuteReader();
            var list = new List<int>();
            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            return list;
        });
    }

    public string GetRecordStatus(string recordId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT CASE
                    WHEN EXISTS (SELECT 1 FROM HSBA_DV WHERE MAHSBA = :mahsba AND MAKTV IS NOT NULL) THEN 'Assigned to technician'
                    WHEN EXISTS (SELECT 1 FROM HSBA WHERE MAHSBA = :mahsba AND MABS IS NOT NULL) THEN 'Assigned to doctor'
                    ELSE 'Created'
                END AS STATUS
                FROM dual
                """;
            command.Parameters.Add(new OracleParameter("mahsba", int.Parse(recordId)));
            return command.ExecuteScalar()?.ToString() ?? string.Empty;
        });
    }

    private List<Staff> GetStaffByAppRole(string appRole)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT MANV, HOTEN, PHAI, NGAYSINH, CCCD, QUEQUAN, SODT, VAITRO, CHUYENKHOA, USERNAME
                FROM NHANVIEN
                WHERE CASE
                    WHEN VAITRO = N'Điều phối viên' THEN 'COORDINATOR'
                    WHEN VAITRO = N'Bác sĩ/Y sĩ' THEN 'DOCTOR'
                    WHEN VAITRO = N'Kỹ thuật viên' THEN 'TECHNICIAN'
                    WHEN VAITRO = N'Bệnh nhân' THEN 'PATIENT'
                    ELSE 'STAFF'
                END = :approle
                ORDER BY MANV
                """;
            command.Parameters.Add(new OracleParameter("approle", appRole));

            using var reader = command.ExecuteReader();
            var items = new List<Staff>();
            while (reader.Read())
            {
                items.Add(new Staff
                {
                    MANV = reader.GetString(0),
                    HOTEN = reader.GetString(1),
                    PHAI = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    NGAYSINH = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
                    CCCD = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    QUEQUAN = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    SODT = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    VAITRO = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    CHUYENKHOA = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    USERNAME = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
                });
            }

            return items;
        });
    }

    private static Patient MapPatient(OracleDataReader reader)
    {
        return new Patient
        {
            MABN = reader.GetString(0),
            TENBN = reader.GetString(1),
            PHAI = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
            NGAYSINH = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
            CCCD = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
            SONHA = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
            TENDUONG = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
            QUANHUYEN = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
            TINHTP = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
            TIENSUBENH = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
            TIENSUBENHGD = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
            DIUNGTHUOC = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
            USERNAME = reader.IsDBNull(12) ? string.Empty : reader.GetString(12)
        };
    }
}
