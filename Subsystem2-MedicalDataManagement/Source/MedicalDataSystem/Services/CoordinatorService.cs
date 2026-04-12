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
                ORDER BY MABN
                FETCH FIRST 200 ROWS ONLY
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
                INSERT INTO BENHNHAN (
                    TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN,
                    TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC, USERNAME, PASSWORD_HASH
                ) VALUES (
                    :tenbn, :phai, TRUNC(:ngaysinh), :cccd, :sonha, :tenduong, :quanhuyen,
                    :tinhtp, :tiensubenh, :tiensubenhgd, :diungthuoc, :username, :password_hash
                )
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
            command.Parameters.Add(new OracleParameter("username", normalizedCccd));
            command.Parameters.Add(new OracleParameter("password_hash", BCrypt.Net.BCrypt.HashPassword(normalizedCccd)));
            return command.ExecuteNonQuery() == 1;
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
            using var command = connection.CreateCommand();
            command.CommandText = """
                MERGE INTO HSBA_DV target
                USING (
                    SELECT :mahsba AS MAHSBA, :loaidv AS LOAIDV, :ngaydv AS NGAYDV FROM dual
                ) source
                ON (
                    target.MAHSBA = source.MAHSBA AND
                    target.LOAIDV = source.LOAIDV AND
                    target.NGAYDV = source.NGAYDV
                )
                WHEN MATCHED THEN
                    UPDATE SET MAKTV = :maktv
                WHEN NOT MATCHED THEN
                    INSERT (MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV)
                    VALUES (:mahsba, :loaidv, :ngaydv, NULL, :maktv)
                """;
            command.Parameters.Add(new OracleParameter("mahsba", medicalRecordId));
            command.Parameters.Add(new OracleParameter("loaidv", serviceType));
            command.Parameters.Add(new OracleParameter("ngaydv", serviceDate));
            command.Parameters.Add(new OracleParameter("maktv", technicianId));
            return command.ExecuteNonQuery() > 0;
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
