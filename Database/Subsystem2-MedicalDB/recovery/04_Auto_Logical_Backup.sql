SET SERVEROUTPUT ON;

PROMPT === Requirement 4 / Automatic logical backup ===
PROMPT Prerequisite: DBA should create directory object MEDICAL_BACKUP_DIR pointing to D:/oracle-backup/logical.

CREATE OR REPLACE PROCEDURE RUN_AUTO_LOGICAL_BACKUP AS
    l_handle    NUMBER;
    l_job_state VARCHAR2(30);
    l_file_suffix VARCHAR2(14);
    l_dump_file VARCHAR2(128);
BEGIN
    l_file_suffix := TO_CHAR(SYSTIMESTAMP, 'YYYYMMDDHH24MISS');
    l_dump_file := 'auto_logical_' || l_file_suffix || '.dmp';

    l_handle := DBMS_DATAPUMP.OPEN(
        operation => 'EXPORT',
        job_mode   => 'SCHEMA'
    );

    DBMS_DATAPUMP.ADD_FILE(
        handle    => l_handle,
        filename  => l_dump_file,
        directory => 'MEDICAL_BACKUP_DIR',
        filetype  => DBMS_DATAPUMP.KU$_FILE_TYPE_DUMP_FILE,
        reusefile => 1
    );

    DBMS_DATAPUMP.METADATA_FILTER(
        handle => l_handle,
        name   => 'SCHEMA_EXPR',
        value  => 'IN (''HOSPITAL_ADMIN'')'
    );

    DBMS_DATAPUMP.START_JOB(l_handle);
    DBMS_DATAPUMP.WAIT_FOR_JOB(l_handle, l_job_state);
    DBMS_DATAPUMP.DETACH(l_handle);

    DBMS_OUTPUT.PUT_LINE('Auto logical export finished with state: ' || l_job_state);
    DBMS_OUTPUT.PUT_LINE('Dump file: ' || l_dump_file);
END;
/

BEGIN
    BEGIN
        DBMS_SCHEDULER.DROP_JOB(
            job_name => 'MED_AUTO_LOGICAL_BACKUP_JOB',
            force    => TRUE
        );
    EXCEPTION
        WHEN OTHERS THEN
            NULL;
    END;

    DBMS_SCHEDULER.CREATE_JOB(
        job_name        => 'MED_AUTO_LOGICAL_BACKUP_JOB',
        job_type        => 'STORED_PROCEDURE',
        job_action      => 'RUN_AUTO_LOGICAL_BACKUP',
        start_date      => SYSTIMESTAMP,
        repeat_interval => 'FREQ=DAILY;BYHOUR=2;BYMINUTE=0;BYSECOND=0',
        enabled         => TRUE,
        auto_drop       => FALSE,
        comments        => 'Daily Data Pump logical backup for HOSPITAL_ADMIN'
    );

    DBMS_OUTPUT.PUT_LINE('Created scheduler job MED_AUTO_LOGICAL_BACKUP_JOB.');
END;
/
