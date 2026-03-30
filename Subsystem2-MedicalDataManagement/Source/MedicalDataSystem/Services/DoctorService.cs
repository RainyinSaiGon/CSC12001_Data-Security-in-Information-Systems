namespace MedicalDataSystem.Services;

using MedicalDataSystem.Models;
using Oracle.ManagedDataAccess.Client;

public class DoctorService
{
    private readonly OracleConnectionService _connectionService;

    public DoctorService(OracleConnectionService connectionService, VPDService vpdService)
    {
        _connectionService = connectionService;
        _ = vpdService;
    }

    public List<Patient> GetAssignedPatients(string doctorId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT DISTINCT
                    b.MABN, b.TENBN, b.PHAI, b.NGAYSINH, b.CCCD, b.SONHA, b.TENDUONG,
                    b.QUANHUYEN, b.TINHTP, b.TIENSUBENH, b.TIENSUBENHGD, b.DIUNGTHUOC, b.USERNAME
                FROM BENHNHAN b
                JOIN HSBA h ON h.MABN = b.MABN
                WHERE h.MABS = :mabs
                ORDER BY b.MABN
                """;
            command.Parameters.Add(new OracleParameter("mabs", int.Parse(doctorId)));

            using var reader = command.ExecuteReader();
            var items = new List<Patient>();
            while (reader.Read())
            {
                items.Add(new Patient
                {
                    MABN = reader.GetInt32(0),
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
                });
            }

            return items;
        });
    }

    public List<MedicalRecord> GetAssignedMedicalRecords(string doctorId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT MAHSBA, MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA
                FROM HSBA
                WHERE MABS = :mabs
                ORDER BY NGAY DESC, MAHSBA DESC
                """;
            command.Parameters.Add(new OracleParameter("mabs", int.Parse(doctorId)));

            using var reader = command.ExecuteReader();
            var items = new List<MedicalRecord>();
            while (reader.Read())
            {
                items.Add(new MedicalRecord
                {
                    MAHSBA = reader.GetInt32(0),
                    MABN = reader.GetInt32(1),
                    NGAY = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2),
                    CHANDOAN = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    DIEUTRI = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    KETLUAN = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    MABS = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                    MAKHOA = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                });
            }

            return items;
        });
    }

    public bool CreateDiagnosis(MedicalRecord record)
    {
        return UpdateMedicalRecord(record);
    }

    public bool UpdateMedicalRecord(MedicalRecord record)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE HSBA
                SET CHANDOAN = :chandoan,
                    DIEUTRI = :dieutri,
                    KETLUAN = :ketluan
                WHERE MAHSBA = :mahsba
                """;
            command.Parameters.Add(new OracleParameter("chandoan", record.CHANDOAN));
            command.Parameters.Add(new OracleParameter("dieutri", record.DIEUTRI));
            command.Parameters.Add(new OracleParameter("ketluan", record.KETLUAN));
            command.Parameters.Add(new OracleParameter("mahsba", record.MAHSBA));
            return command.ExecuteNonQuery() == 1;
        });
    }

    public bool UpdatePatientHistory(Patient patient)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE BENHNHAN
                SET TIENSUBENH = :tiensubenh,
                    TIENSUBENHGD = :tiensubenhgd,
                    DIUNGTHUOC = :diungthuoc
                WHERE MABN = :mabn
                """;
            command.Parameters.Add(new OracleParameter("tiensubenh", patient.TIENSUBENH));
            command.Parameters.Add(new OracleParameter("tiensubenhgd", patient.TIENSUBENHGD));
            command.Parameters.Add(new OracleParameter("diungthuoc", patient.DIUNGTHUOC));
            command.Parameters.Add(new OracleParameter("mabn", patient.MABN));
            return command.ExecuteNonQuery() == 1;
        });
    }

    public bool UpdatePrescription(Prescription prescription)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                MERGE INTO DONTHUOC target
                USING (
                    SELECT :mahsba AS MAHSBA, :tenthuoc AS TENTHUOC, :ngaydt AS NGAYDT FROM dual
                ) source
                ON (
                    target.MAHSBA = source.MAHSBA AND
                    target.TENTHUOC = source.TENTHUOC AND
                    target.NGAYDT = source.NGAYDT
                )
                WHEN MATCHED THEN
                    UPDATE SET LIEUDUNG = :lieudung
                WHEN NOT MATCHED THEN
                    INSERT (MAHSBA, TENTHUOC, LIEUDUNG, NGAYDT)
                    VALUES (:mahsba, :tenthuoc, :lieudung, :ngaydt)
                """;
            command.Parameters.Add(new OracleParameter("mahsba", prescription.MAHSBA));
            command.Parameters.Add(new OracleParameter("tenthuoc", prescription.TENTHUOC));
            command.Parameters.Add(new OracleParameter("ngaydt", prescription.NGAYDT));
            command.Parameters.Add(new OracleParameter("lieudung", prescription.LIEUDUNG));
            return command.ExecuteNonQuery() > 0;
        });
    }

    public bool DeletePrescription(Prescription prescription)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM DONTHUOC
                WHERE MAHSBA = :mahsba AND TENTHUOC = :tenthuoc AND NGAYDT = :ngaydt
                """;
            command.Parameters.Add(new OracleParameter("mahsba", prescription.MAHSBA));
            command.Parameters.Add(new OracleParameter("tenthuoc", prescription.TENTHUOC));
            command.Parameters.Add(new OracleParameter("ngaydt", prescription.NGAYDT));
            return command.ExecuteNonQuery() == 1;
        });
    }

    public bool OrderDiagnosticService(DiagnosticService service)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO HSBA_DV (MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV)
                VALUES (:mahsba, :loaidv, :ngaydv, :ketqua, :maktv)
                """;
            command.Parameters.Add(new OracleParameter("mahsba", service.MAHSBA));
            command.Parameters.Add(new OracleParameter("loaidv", service.LOAIDV));
            command.Parameters.Add(new OracleParameter("ngaydv", service.NGAYDV));
            command.Parameters.Add(new OracleParameter("ketqua", string.IsNullOrWhiteSpace(service.KETQUA) ? DBNull.Value : service.KETQUA));
            command.Parameters.Add(new OracleParameter("maktv", service.MAKTV == 0 ? DBNull.Value : service.MAKTV));
            return command.ExecuteNonQuery() == 1;
        });
    }

    public bool DeleteDiagnosticService(DiagnosticService service)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM HSBA_DV
                WHERE MAHSBA = :mahsba AND LOAIDV = :loaidv AND NGAYDV = :ngaydv
                """;
            command.Parameters.Add(new OracleParameter("mahsba", service.MAHSBA));
            command.Parameters.Add(new OracleParameter("loaidv", service.LOAIDV));
            command.Parameters.Add(new OracleParameter("ngaydv", service.NGAYDV));
            return command.ExecuteNonQuery() == 1;
        });
    }
}
