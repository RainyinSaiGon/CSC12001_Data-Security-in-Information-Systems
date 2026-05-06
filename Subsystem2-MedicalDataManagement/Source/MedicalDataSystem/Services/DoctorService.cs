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
            command.Parameters.Add(new OracleParameter("mabs", doctorId));

            using var reader = command.ExecuteReader();
            var items = new List<Patient>();
            while (reader.Read())
            {
                items.Add(new Patient
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
            command.Parameters.Add(new OracleParameter("mabs", doctorId));

            using var reader = command.ExecuteReader();
            var items = new List<MedicalRecord>();
            while (reader.Read())
            {
                items.Add(new MedicalRecord
                {
                    MAHSBA = reader.GetInt32(0),
                    MABN = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    NGAY = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2),
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

    public bool CreateDiagnosis(MedicalRecord record)
    {
        return UpdateMedicalRecord(record);
    }

    public bool UpdateMedicalRecord(MedicalRecord record)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.BindByName = true;
            command.CommandText = """
                DECLARE
                    v_mahsba INT := :mahsba;
                    v_chandoan NVARCHAR2(2000) := :chandoan;
                    v_dieutri NVARCHAR2(2000) := :dieutri;
                    v_ketluan NVARCHAR2(2000) := :ketluan;
                BEGIN
                    UPDATE HSBA
                    SET CHANDOAN = v_chandoan,
                        DIEUTRI = v_dieutri,
                        KETLUAN = v_ketluan
                    WHERE MAHSBA = v_mahsba;
                END;
                """;
            command.Parameters.Add(new OracleParameter("chandoan", record.CHANDOAN ?? ""));
            command.Parameters.Add(new OracleParameter("dieutri", record.DIEUTRI ?? ""));
            command.Parameters.Add(new OracleParameter("ketluan", record.KETLUAN ?? ""));
            command.Parameters.Add(new OracleParameter("mahsba", record.MAHSBA));
            command.ExecuteNonQuery();
            return true;
        });
    }

    public bool UpdatePatientHistory(Patient patient)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.BindByName = true;
            command.CommandText = """
                DECLARE
                    v_mabn VARCHAR2(32) := :mabn;
                    v_tiensubenh NVARCHAR2(2000) := :tiensubenh;
                    v_tiensubenhgd NVARCHAR2(2000) := :tiensubenhgd;
                    v_diungthuoc NVARCHAR2(2000) := :diungthuoc;
                BEGIN
                    UPDATE BENHNHAN
                    SET TIENSUBENH = v_tiensubenh,
                        TIENSUBENHGD = v_tiensubenhgd,
                        DIUNGTHUOC = v_diungthuoc
                    WHERE MABN = v_mabn;
                END;
                """;
            command.Parameters.Add(new OracleParameter("tiensubenh", patient.TIENSUBENH ?? ""));
            command.Parameters.Add(new OracleParameter("tiensubenhgd", patient.TIENSUBENHGD ?? ""));
            command.Parameters.Add(new OracleParameter("diungthuoc", patient.DIUNGTHUOC ?? ""));
            command.Parameters.Add(new OracleParameter("mabn", patient.MABN ?? ""));
            command.ExecuteNonQuery();
            return true;
        });
    }

    public bool UpdatePrescription(Prescription prescription)
    {
        return _connectionService.Execute(connection =>
        {
            using var cmdUpdate = connection.CreateCommand();
            cmdUpdate.BindByName = true;
            cmdUpdate.CommandText = """
                DECLARE
                    v_mahsba INT := :mahsba;
                    v_tenthuoc NVARCHAR2(100) := :tenthuoc;
                    v_lieudung NVARCHAR2(200) := :lieudung;
                    v_ngaydt DATE := TO_DATE(:ngaydt_str, 'YYYY-MM-DD');
                BEGIN
                    UPDATE DONTHUOC 
                    SET LIEUDUNG = v_lieudung 
                    WHERE MAHSBA = v_mahsba AND TENTHUOC = v_tenthuoc 
                      AND NGAYDT >= v_ngaydt AND NGAYDT < v_ngaydt + 1;
                      
                    IF SQL%ROWCOUNT = 0 THEN
                        INSERT INTO DONTHUOC (MAHSBA, TENTHUOC, LIEUDUNG, NGAYDT)
                        VALUES (v_mahsba, v_tenthuoc, v_lieudung, v_ngaydt);
                    END IF;
                END;
                """;
            cmdUpdate.Parameters.Add(new OracleParameter("lieudung", prescription.LIEUDUNG ?? ""));
            cmdUpdate.Parameters.Add(new OracleParameter("mahsba", prescription.MAHSBA));
            cmdUpdate.Parameters.Add(new OracleParameter("tenthuoc", prescription.TENTHUOC ?? ""));
            cmdUpdate.Parameters.Add(new OracleParameter("ngaydt_str", prescription.NGAYDT.ToString("yyyy-MM-dd")));
            cmdUpdate.ExecuteNonQuery();
            return true;
        });
    }

    public bool DeletePrescription(Prescription prescription)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.BindByName = true;
            command.CommandText = """
                DECLARE
                    v_mahsba INT := :mahsba;
                    v_tenthuoc NVARCHAR2(100) := :tenthuoc;
                    v_ngaydt DATE := TO_DATE(:ngaydt_str, 'YYYY-MM-DD');
                BEGIN
                    DELETE FROM DONTHUOC
                    WHERE MAHSBA = v_mahsba AND TENTHUOC = v_tenthuoc 
                      AND NGAYDT >= v_ngaydt AND NGAYDT < v_ngaydt + 1;
                END;
                """;
            command.Parameters.Add(new OracleParameter("mahsba", prescription.MAHSBA));
            command.Parameters.Add(new OracleParameter("tenthuoc", prescription.TENTHUOC ?? ""));
            command.Parameters.Add(new OracleParameter("ngaydt_str", prescription.NGAYDT.ToString("yyyy-MM-dd")));
            command.ExecuteNonQuery();
            return true;
        });
    }

    public bool OrderDiagnosticService(DiagnosticService service)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.BindByName = true;
            command.CommandText = """
                INSERT INTO HSBA_DV (MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV)
                VALUES (:mahsba, :loaidv, TO_DATE(:ngaydv_str, 'YYYY-MM-DD'), NULL, NULL)
                """;
            command.Parameters.Add(new OracleParameter("mahsba", OracleDbType.Int32, service.MAHSBA, System.Data.ParameterDirection.Input));
            command.Parameters.Add(new OracleParameter("loaidv", OracleDbType.NVarchar2, service.LOAIDV, System.Data.ParameterDirection.Input));
            command.Parameters.Add(new OracleParameter("ngaydv_str", OracleDbType.Varchar2, service.NGAYDV.ToString("yyyy-MM-dd"), System.Data.ParameterDirection.Input));
            return command.ExecuteNonQuery() == 1;
        });
    }

    public bool DeleteDiagnosticService(DiagnosticService service)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.BindByName = true;
            command.CommandText = """
                DECLARE
                    v_mahsba INT := :mahsba;
                    v_loaidv NVARCHAR2(100) := :loaidv;
                    v_ngaydv DATE := TO_DATE(:ngaydv_str, 'YYYY-MM-DD');
                BEGIN
                    DELETE FROM HSBA_DV
                    WHERE MAHSBA = v_mahsba AND LOAIDV = v_loaidv 
                      AND NGAYDV >= v_ngaydv 
                      AND NGAYDV < v_ngaydv + 1;
                END;
                """;
            command.Parameters.Add(new OracleParameter("mahsba", service.MAHSBA));
            command.Parameters.Add(new OracleParameter("loaidv", service.LOAIDV ?? ""));
            command.Parameters.Add(new OracleParameter("ngaydv_str", service.NGAYDV.ToString("yyyy-MM-dd")));
            command.ExecuteNonQuery();
            return true;
        });
    }

    public List<Prescription> GetPrescriptions(int recordId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.BindByName = true;
            command.CommandText = "SELECT MAHSBA, TENTHUOC, LIEUDUNG, NGAYDT FROM DONTHUOC WHERE MAHSBA = :mahsba ORDER BY NGAYDT DESC";
            command.Parameters.Add(new OracleParameter("mahsba", OracleDbType.Int32, recordId, System.Data.ParameterDirection.Input));
            using var reader = command.ExecuteReader();
            var list = new List<Prescription>();
            while (reader.Read())
            {
                list.Add(new Prescription
                {
                    MAHSBA = reader.GetInt32(0),
                    TENTHUOC = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    LIEUDUNG = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    NGAYDT = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3)
                });
            }
            return list;
        });
    }

    public List<DiagnosticService> GetDiagnosticServices(int recordId)
    {
        return _connectionService.Execute(connection =>
        {
            using var command = connection.CreateCommand();
            command.BindByName = true;
            command.CommandText = "SELECT MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV FROM HSBA_DV WHERE MAHSBA = :mahsba ORDER BY NGAYDV DESC";
            command.Parameters.Add(new OracleParameter("mahsba", OracleDbType.Int32, recordId, System.Data.ParameterDirection.Input));
            using var reader = command.ExecuteReader();
            var list = new List<DiagnosticService>();
            while (reader.Read())
            {
                list.Add(new DiagnosticService
                {
                    MAHSBA = reader.GetInt32(0),
                    LOAIDV = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    NGAYDV = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2),
                    KETQUA = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    MAKTV = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                });
            }
            return list;
        });
    }
}
