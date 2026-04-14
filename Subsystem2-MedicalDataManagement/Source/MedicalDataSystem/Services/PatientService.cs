namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public class PatientService
{
    private readonly OracleConnectionService _connectionService;

    public PatientService(OracleConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public Patient? GetPatient(string patientId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT MABN, TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN,
                       TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC, USERNAME
                FROM V_SELF_BENHNHAN
                """;
            _ = patientId;

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

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
        });
    }

    public bool UpdatePatientInfo(Patient patient)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE V_SELF_BENHNHAN
                SET SONHA = :sonha,
                    TENDUONG = :tenduong,
                    QUANHUYEN = :quanhuyen,
                    TINHTP = :tinhtp,
                    TIENSUBENH = :tiensubenh,
                    TIENSUBENHGD = :tiensubenhgd,
                    DIUNGTHUOC = :diungthuoc
                """;
            command.Parameters.Add(new OracleParameter("sonha", patient.SONHA));
            command.Parameters.Add(new OracleParameter("tenduong", patient.TENDUONG));
            command.Parameters.Add(new OracleParameter("quanhuyen", patient.QUANHUYEN));
            command.Parameters.Add(new OracleParameter("tinhtp", patient.TINHTP));
            command.Parameters.Add(new OracleParameter("tiensubenh", patient.TIENSUBENH));
            command.Parameters.Add(new OracleParameter("tiensubenhgd", patient.TIENSUBENHGD));
            command.Parameters.Add(new OracleParameter("diungthuoc", patient.DIUNGTHUOC));
            return command.ExecuteNonQuery() == 1;
        });
    }

    public List<MedicalRecord> GetMyMedicalRecords(string patientId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT MAHSBA, MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA
                FROM V_PATIENT_HSBA
                ORDER BY NGAY DESC, MAHSBA DESC
                """;
            _ = patientId;

            using var reader = command.ExecuteReader();
            var items = new List<MedicalRecord>();
            while (reader.Read())
            {
                items.Add(new MedicalRecord
                {
                    MAHSBA = reader.GetInt32(0),
                    MABN = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    NGAY = reader.GetDateTime(2),
                    CHANDOAN = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    DIEUTRI = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    KETLUAN = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    MABS = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    MAKHOA = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                });
            }

            return items;
        });
    }

    public List<Prescription> GetMyPrescriptions(string patientId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT d.MAHSBA, d.NGAYDT, d.TENTHUOC, d.LIEUDUNG
                FROM V_PATIENT_DONTHUOC d
                ORDER BY d.NGAYDT DESC, d.MAHSBA DESC
                """;
            _ = patientId;

            using var reader = command.ExecuteReader();
            var items = new List<Prescription>();
            while (reader.Read())
            {
                items.Add(new Prescription
                {
                    MAHSBA = reader.GetInt32(0),
                    NGAYDT = reader.GetDateTime(1),
                    TENTHUOC = reader.GetString(2),
                    LIEUDUNG = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                });
            }

            return items;
        });
    }
}
