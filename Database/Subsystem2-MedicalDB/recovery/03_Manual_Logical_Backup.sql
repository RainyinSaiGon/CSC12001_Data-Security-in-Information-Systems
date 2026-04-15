SET SERVEROUTPUT ON;

PROMPT === Requirement 4 / Manual logical backup ===
PROMPT Prerequisite: DBA should create directory object MEDICAL_BACKUP_DIR pointing to D:/oracle-backup/logical.

DECLARE
    l_handle    NUMBER;
    l_job_state VARCHAR2(30);
    l_file_suffix VARCHAR2(14);
    l_dump_file VARCHAR2(128);
BEGIN
    l_file_suffix := TO_CHAR(SYSTIMESTAMP, 'YYYYMMDDHH24MISS');
    l_dump_file := 'manual_logical_' || l_file_suffix || '.dmp';

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

    DBMS_OUTPUT.PUT_LINE('Manual logical export finished with state: ' || l_job_state);
    DBMS_OUTPUT.PUT_LINE('Dump file: ' || l_dump_file);
END;
/

