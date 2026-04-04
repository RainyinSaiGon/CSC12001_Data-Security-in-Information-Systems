# Requirement 4 - Backup and Recovery

This folder contains the scripts used to demonstrate backup and recovery for Task 10.

## Script List
- `Create_Medical_Backup_Directory.sql`: creates `MEDICAL_BACKUP_DIR` for logical dump output.
- `00_Manual_Physical_Full_Backup.rman`: manual RMAN physical full baseline backup.
- `01_Manual_Physical_Incremental_Backup.rman`: manual RMAN physical incremental backup.
- `02_Auto_Physical_Incremental_Backup.sql`: creates scheduler job `MED_AUTO_PHY_INC_BACKUP_JOB` (daily 01:00).
- `03_Manual_Logical_Backup.sql`: manual Data Pump schema export (dump-only).
- `04_Auto_Logical_Backup.sql`: creates procedure `RUN_AUTO_LOGICAL_BACKUP` and scheduler job `MED_AUTO_LOGICAL_BACKUP_JOB` (daily 02:00).
- `05_Prepare_Logical_Recovery_Test.sql`: prepares real-schema recovery by storing pre-restore row-count snapshot.
- `06_Verify_Logical_Recovery_Test.sql`: verifies restored `HOSPITAL_ADMIN` and compares with pre-restore snapshot.
- `07_Recovery_Audit_Timestamp_Anchor.sql`: gets incident timestamp anchor from audit/FGA logs.
- `08_Manual_Physical_Recovery_PITR.rman`: PITR template using `SET UNTIL TIME`.
- `09_Method_Comparison_And_Conclusion.md`: report-ready method comparison and conclusion.

## Important Implementation Notes
- Run physical `.rman` files with RMAN, not SQLPlus.
- In PowerShell, RMAN executable must be invoked with `&`.
- Logical backup scripts are intentionally dump-only (no Data Pump `.log` registration) for compatibility in this XE environment.
- The first run of incremental backup can be promoted by RMAN to level 0 if no valid incremental parent exists. This is expected behavior.

## End-to-End Run Order

### 1) Logical directory setup (one-time)
Run as SYS (or DBA):

```sql
@D:/atbmhttt/lab/CSC12001_Data-Security-in-Information-Systems/Database/Subsystem2-MedicalDB/recovery/Create_Medical_Backup_Directory.sql
```

Expected:
- `PL/SQL procedure successfully completed.`
- `MEDICAL_BACKUP_DIR` points to `D:/oracle-backup/logical`.
- On this Windows machine, `NT SERVICE\OracleServiceXE` must have NTFS write permission on `D:/oracle-backup/logical`.

### 2) Manual physical full baseline
Run in PowerShell:

```powershell
& "C:\app\LENOVO\product\21c\dbhomeXE\bin\rman.exe" target / cmdfile="D:\atbmhttt\lab\CSC12001_Data-Security-in-Information-Systems\Database\Subsystem2-MedicalDB\recovery\00_Manual_Physical_Full_Backup.rman" log="D:\oracle-backup\physical\logs\MED_MANUAL_PHY_FULL.log"
```

Expected:
- RMAN creates `FULL_*.BKP` files under `D:\oracle-backup\physical`.
- Log ends with `Recovery Manager complete.`

### 3) Manual physical incremental
Run in PowerShell:

```powershell
& "C:\app\LENOVO\product\21c\dbhomeXE\bin\rman.exe" target / cmdfile="D:\atbmhttt\lab\CSC12001_Data-Security-in-Information-Systems\Database\Subsystem2-MedicalDB\recovery\01_Manual_Physical_Incremental_Backup.rman" log="D:\oracle-backup\physical\logs\MED_MANUAL_PHY_INC_L1.log"
```

Expected:
- RMAN creates `MANUAL_INC_*.BKP` files under `D:\oracle-backup\physical`.
- Log ends with `Recovery Manager complete.`

### 4) Auto physical scheduler setup
Run as SYS:

```sql
@D:/atbmhttt/lab/CSC12001_Data-Security-in-Information-Systems/Database/Subsystem2-MedicalDB/recovery/02_Auto_Physical_Incremental_Backup.sql
```

Expected:
- Job `MED_AUTO_PHY_INC_BACKUP_JOB` created and enabled.

### 5) Manual logical backup
Run as SYS (or user with required Data Pump rights):

```sql
@D:/atbmhttt/lab/CSC12001_Data-Security-in-Information-Systems/Database/Subsystem2-MedicalDB/recovery/03_Manual_Logical_Backup.sql
```

Expected:
- Output contains `Manual logical export finished with state: COMPLETED`.
- Output contains generated dump filename.
- A new `manual_logical_*.dmp` appears in `D:\oracle-backup\logical`.

### 6) Auto logical scheduler setup
Run as SYS:

```sql
@D:/atbmhttt/lab/CSC12001_Data-Security-in-Information-Systems/Database/Subsystem2-MedicalDB/recovery/04_Auto_Logical_Backup.sql
```

Expected:
- Procedure `RUN_AUTO_LOGICAL_BACKUP` replaced/created.
- Job `MED_AUTO_LOGICAL_BACKUP_JOB` created and enabled.

### 7) Validate a logical dump file (recommended)
Use this flow to verify a `.dmp` is correct before recovery drills.

#### A) Check dump file exists on OS
Run in PowerShell:

```powershell
Get-ChildItem "D:\oracle-backup\logical" -File | Sort-Object LastWriteTime -Descending | Select-Object -First 10 Name,Length,LastWriteTime
```

Expected:
- Target dump file is present.
- File size is non-zero.

#### B) Validate Oracle can read the dump metadata
Run in SQL*Plus/SQLcl as SYS (or DBA):

```sql
SET SERVEROUTPUT ON;
DECLARE
	l_info     SYS.KU$_DUMPFILE_INFO;
	l_filetype NUMBER;
BEGIN
	DBMS_DATAPUMP.GET_DUMPFILE_INFO(
		filename   => 'MANUAL_LOGICAL_YYYYMMDDHH24MISS.DMP',
		directory  => 'MEDICAL_BACKUP_DIR',
		info_table => l_info,
		filetype   => l_filetype
	);

	DBMS_OUTPUT.PUT_LINE('OK: dump file is readable. filetype=' || l_filetype);
END;
/
```

Expected:
- `PL/SQL procedure successfully completed.`
- `OK: dump file is readable. filetype=...`

#### C) Convert dump metadata to SQL for inspection
Run in PowerShell:

```powershell
impdp system/1104@localhost:1521/xepdb1 directory=MEDICAL_BACKUP_DIR dumpfile=MANUAL_LOGICAL_YYYYMMDDHH24MISS.DMP sqlfile=DATA_PUMP_DIR:kiem_tra_dump.sql content=metadata_only nologfile=yes
```

Expected:
- Data Pump job completes successfully.
- `kiem_tra_dump.sql` is generated under `DATA_PUMP_DIR`.

Note:
- In this XE setup, writing SQLFILE to `MEDICAL_BACKUP_DIR` can fail with `ORA-29283`.
- Prefer writing SQLFILE to `DATA_PUMP_DIR` and keep reading dump from `MEDICAL_BACKUP_DIR`.

### 8) Logical recovery walkthrough (real schema)

#### A) Prepare pre-restore snapshot
Run as SYS:

```sql
@D:/atbmhttt/lab/CSC12001_Data-Security-in-Information-Systems/Database/Subsystem2-MedicalDB/recovery/05_Prepare_Logical_Recovery_Test.sql
```

#### B) Import dump to real schema
Run in PowerShell (replace dump filename):

```powershell
impdp system/1104@localhost:1521/xepdb1 directory=MEDICAL_BACKUP_DIR dumpfile=MANUAL_LOGICAL_YYYYMMDDHH24MISS.DMP schemas=HOSPITAL_ADMIN table_exists_action=replace exclude=PROCACT_INSTANCE nologfile=yes
```

#### C) Verify restored schema
Run as SYS:

```sql
@D:/atbmhttt/lab/CSC12001_Data-Security-in-Information-Systems/Database/Subsystem2-MedicalDB/recovery/06_Verify_Logical_Recovery_Test.sql
```

Expected:
- Core tables exist in `HOSPITAL_ADMIN`.
- Object counts are non-zero.
- Pre-restore snapshot and post-restore row-count statistics can be compared quickly.

### 9) Physical recovery walkthrough (PITR anchored by audit)

#### A) Get incident anchor timestamp from Requirement 3 logs
Run as SYS:

```sql
@D:/atbmhttt/lab/CSC12001_Data-Security-in-Information-Systems/Database/Subsystem2-MedicalDB/recovery/07_Recovery_Audit_Timestamp_Anchor.sql
```

Take `suggested_restore_time` from output.

#### B) Apply PITR in RMAN test environment
Edit timestamp in `08_Manual_Physical_Recovery_PITR.rman`, then run in PowerShell:

```powershell
& "C:\app\LENOVO\product\21c\dbhomeXE\bin\rman.exe" target / cmdfile="D:\atbmhttt\lab\CSC12001_Data-Security-in-Information-Systems\Database\Subsystem2-MedicalDB\recovery\08_Manual_Physical_Recovery_PITR.rman" log="D:\oracle-backup\physical\logs\MED_MANUAL_PITR.log"
```

Warning:
- PITR must be executed on a dedicated recovery test copy/environment.
- `OPEN RESETLOGS` changes database incarnation.

## How To Test Again

### A) Check scheduler jobs
```sql
SELECT owner, job_name, enabled, state, repeat_interval, next_run_date
FROM dba_scheduler_jobs
WHERE job_name IN ('MED_AUTO_PHY_INC_BACKUP_JOB', 'MED_AUTO_LOGICAL_BACKUP_JOB');
```

Expected:
- Physical: daily 01:00.
- Logical: daily 02:00.
- `ENABLED = TRUE`.

### B) Trigger immediate test run (without waiting schedule)
```sql
BEGIN
	DBMS_SCHEDULER.RUN_JOB('MED_AUTO_PHY_INC_BACKUP_JOB', use_current_session => FALSE);
	DBMS_SCHEDULER.RUN_JOB('MED_AUTO_LOGICAL_BACKUP_JOB', use_current_session => FALSE);
END;
/
```

### C) Check run history
```sql
SELECT job_name, status, actual_start_date, run_duration, additional_info
FROM dba_scheduler_job_run_details
WHERE job_name IN ('MED_AUTO_PHY_INC_BACKUP_JOB', 'MED_AUTO_LOGICAL_BACKUP_JOB')
ORDER BY log_date DESC;
```

Expected:
- Latest rows show `STATUS = SUCCEEDED`.

### D) Check output artifacts
- Physical files:
	- `D:\oracle-backup\physical\*.BKP`
	- `D:\oracle-backup\physical\logs\MED_AUTO_PHY_INC_BACKUP.log`
- Logical files:
	- `D:\oracle-backup\logical\manual_logical_*.dmp`
	- `D:\oracle-backup\logical\auto_logical_*.dmp`

## Optional Cleanup For Re-Test
If you need a clean scheduler state before re-running scripts:

```sql
BEGIN
	BEGIN DBMS_SCHEDULER.STOP_JOB('MED_AUTO_PHY_INC_BACKUP_JOB', force => TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
	BEGIN DBMS_SCHEDULER.STOP_JOB('MED_AUTO_LOGICAL_BACKUP_JOB', force => TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
	BEGIN DBMS_SCHEDULER.DROP_JOB('MED_AUTO_PHY_INC_BACKUP_JOB', force => TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
	BEGIN DBMS_SCHEDULER.DROP_JOB('MED_AUTO_LOGICAL_BACKUP_JOB', force => TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
END;
/
```

## Deliverables Mapping (Task 10)
- Manual backup flow: `00_Manual_Physical_Full_Backup.rman`, `01_Manual_Physical_Incremental_Backup.rman`, `03_Manual_Logical_Backup.sql`.
- Automatic backup flow: `02_Auto_Physical_Incremental_Backup.sql`, `04_Auto_Logical_Backup.sql`.
- Recovery walkthrough: sections 8 and 9 with scripts `05` to `08`.
- Audit timestamp tie-in: `07_Recovery_Audit_Timestamp_Anchor.sql`.
- Comparison and conclusion: `09_Method_Comparison_And_Conclusion.md`.