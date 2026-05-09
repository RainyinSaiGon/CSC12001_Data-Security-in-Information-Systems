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

    public bool UpdatePatientInfo(Patient patient, bool attemptRestrictedUpdate = false)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.BindByName = true;
            
            if (attemptRestrictedUpdate)
            {
                command.CommandText = """
                    DECLARE
                        v_mabn VARCHAR2(32) := :mabn;
                        v_tenbn NVARCHAR2(100) := :tenbn;
                        v_cccd CHAR(12) := :cccd;
                    BEGIN
                        -- Lệnh này sẽ bị Oracle chặn (ORA-01031) do Bệnh nhân không có quyền UPDATE trên cột TENBN, CCCD
                        UPDATE V_SELF_BENHNHAN
                        SET TENBN = v_tenbn,
                            CCCD = v_cccd
                        WHERE MABN = v_mabn;
                    END;
                    """;
                command.Parameters.Add(new OracleParameter("tenbn", patient.TENBN ?? ""));
                command.Parameters.Add(new OracleParameter("cccd", patient.CCCD ?? ""));
                command.Parameters.Add(new OracleParameter("mabn", patient.MABN));
                command.ExecuteNonQuery();
                return true;
            }
            else
            {
                command.CommandText = """
                    DECLARE
                        v_mabn VARCHAR2(32) := :mabn;
                        v_sonha NVARCHAR2(30) := :sonha;
                        v_tenduong NVARCHAR2(30) := :tenduong;
                        v_quanhuyen NVARCHAR2(30) := :quanhuyen;
                        v_tinhtp NVARCHAR2(50) := :tinhtp;
                        v_tiensubenh NVARCHAR2(2000) := :tiensubenh;
                        v_tiensubenhgd NVARCHAR2(2000) := :tiensubenhgd;
                        v_diungthuoc NVARCHAR2(2000) := :diungthuoc;
                    BEGIN
                        UPDATE V_SELF_BENHNHAN
                        SET SONHA = v_sonha,
                            TENDUONG = v_tenduong,
                            QUANHUYEN = v_quanhuyen,
                            TINHTP = v_tinhtp,
                            TIENSUBENH = v_tiensubenh,
                            TIENSUBENHGD = v_tiensubenhgd,
                            DIUNGTHUOC = v_diungthuoc
                        WHERE MABN = v_mabn;
                    END;
                    """;
                command.Parameters.Add(new OracleParameter("sonha", patient.SONHA ?? ""));
                command.Parameters.Add(new OracleParameter("tenduong", patient.TENDUONG ?? ""));
                command.Parameters.Add(new OracleParameter("quanhuyen", patient.QUANHUYEN ?? ""));
                command.Parameters.Add(new OracleParameter("tinhtp", patient.TINHTP ?? ""));
                command.Parameters.Add(new OracleParameter("tiensubenh", patient.TIENSUBENH ?? ""));
                command.Parameters.Add(new OracleParameter("tiensubenhgd", patient.TIENSUBENHGD ?? ""));
                command.Parameters.Add(new OracleParameter("diungthuoc", patient.DIUNGTHUOC ?? ""));
                command.Parameters.Add(new OracleParameter("mabn", patient.MABN));
                command.ExecuteNonQuery();
                return true;
            }
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
