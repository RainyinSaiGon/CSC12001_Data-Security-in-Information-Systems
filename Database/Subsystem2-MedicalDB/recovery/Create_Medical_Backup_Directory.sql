SET SERVEROUTPUT ON;

PROMPT === Requirement 4 / Create Data Pump directory object ===
PROMPT This script must be run by a DBA user.
PROMPT Make sure the OS folder D:/oracle-backup/logical already exists.

BEGIN
    BEGIN
        EXECUTE IMMEDIATE 'DROP DIRECTORY MEDICAL_BACKUP_DIR';
    EXCEPTION
        WHEN OTHERS THEN
            NULL;
    END;

    EXECUTE IMMEDIATE q'[CREATE DIRECTORY MEDICAL_BACKUP_DIR AS 'D:\oracle-backup\logical']';
    EXECUTE IMMEDIATE 'GRANT READ, WRITE ON DIRECTORY MEDICAL_BACKUP_DIR TO HOSPITAL_ADMIN';

    DBMS_OUTPUT.PUT_LINE('Created directory object MEDICAL_BACKUP_DIR and granted READ/WRITE to HOSPITAL_ADMIN.');
END;
/
