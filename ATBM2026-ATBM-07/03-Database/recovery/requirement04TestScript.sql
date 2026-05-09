CONNECT SYS/tg13jzmat@localhost:1521/XEPDB1 AS SYSDBA;

--@Create_Medical_Backup_Directory.sql

-- Chạy PowerShell với quyền Admin
-- rman target / cmdfile="D:\Sẻn\HK2-25-26\AT&BM_HTTT\CSC12001_Data-Security-in-Information-Systems\Database\Subsystem2-MedicalDB\recovery\00_Manual_Physical_Full_Backup.rman" log="D:\oracle-backup\physical\logs\MED_MANUAL_PHY_FULL.log"
-- rman target / cmdfile="D:\Sẻn\HK2-25-26\AT&BM_HTTT\CSC12001_Data-Security-in-Information-Systems\Database\Subsystem2-MedicalDB\recovery\01_Manual_Physical_Incremental_Backup.rman" log="D:\oracle-backup\physical\logs\MED_MANUAL_PHY_INCR.log"

@02_Auto_Physical_Incremental_Backup.sql
@03_Manual_Logical_Backup.sql
@04_Auto_Logical_Backup.sql
@05_Prepare_Logical_Recovery_Test.sql

-- Chạy PowerShell với quyền Admin
--impdp system/tg13jzmat@localhost:1521/xepdb1 directory=MEDICAL_BACKUP_DIR dumpfile=MANUAL_LOGICAL_20260506115607.DMP schemas=HOSPITAL_ADMIN table_exists_action=replace exclude=PROCACT_INSTANCE nologfile=yes

@06_Verify_Logical_Recovery_Test.sql
@07_Recovery_Audit_Timestamp_Anchor.sql

-- Chạy PowerShell với quyền Admin
-- rman target / cmdfile="D:\Sẻn\HK2-25-26\AT&BM_HTTT\CSC12001_Data-Security-in-Information-Systems\Database\Subsystem2-MedicalDB\recovery\08_Manual_Physical_Recovery_PITR.rman" log="D:\oracle-backup\physical\logs\MED_MANUAL_PITR.log"

-- Optional: --@demo_flashback.sql
