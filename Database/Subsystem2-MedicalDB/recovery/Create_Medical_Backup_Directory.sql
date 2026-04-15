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


-- SET SERVEROUTPUT ON;
-- DECLARE
--     l_info     SYS.KU$_DUMPFILE_INFO;
--     l_filetype NUMBER;
-- BEGIN
--     DBMS_DATAPUMP.GET_DUMPFILE_INFO(
--         filename   => 'MANUAL_LOGICAL_20260404164825.DMP',
--         directory  => 'MEDICAL_BACKUP_DIR',
--         info_table => l_info,
--         filetype   => l_filetype
--     );

--     DBMS_OUTPUT.PUT_LINE('OK: dump file is readable. filetype=' || l_filetype);

--     FOR i IN 1 .. l_info.COUNT LOOP
--         DBMS_OUTPUT.PUT_LINE(
--             'item_code=' || l_info(i).item_code || ', value=' || l_info(i).value
--         );
--     END LOOP;
-- END;
-- /