SET SERVEROUTPUT ON;

PROMPT === Requirement 4 / Logical recovery preparation (real schema) ===
PROMPT Run as SYS in XEPDB1 before importing into HOSPITAL_ADMIN.
PROMPT This script saves a pre-restore row-count snapshot for quick comparison.

ALTER SESSION SET CONTAINER = XEPDB1;

BEGIN
    BEGIN
        EXECUTE IMMEDIATE 'DROP TABLE HOSPITAL_ADMIN.RECOVERY_ROWCOUNT_SNAPSHOT PURGE';
        DBMS_OUTPUT.PUT_LINE('Dropped old snapshot table HOSPITAL_ADMIN.RECOVERY_ROWCOUNT_SNAPSHOT.');
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE != -942 THEN
                RAISE;
            END IF;
    END;

    EXECUTE IMMEDIATE q'[
        CREATE TABLE HOSPITAL_ADMIN.RECOVERY_ROWCOUNT_SNAPSHOT AS
        SELECT TABLE_NAME,
               NUM_ROWS AS SOURCE_ROWS,
               SYSTIMESTAMP AS SNAPSHOT_TIME
        FROM DBA_TABLES
        WHERE OWNER = 'HOSPITAL_ADMIN'
          AND TABLE_NAME IN (
              'NHANVIEN',
              'BENHNHAN',
              'HSBA',
              'HSBA_DV',
              'DONTHUOC',
              'THONGBAO',
              'KHOA'
          )
    ]';

    DBMS_OUTPUT.PUT_LINE('Created HOSPITAL_ADMIN.RECOVERY_ROWCOUNT_SNAPSHOT.');
END;
/

PROMPT Next step (PowerShell, real-schema restore):
PROMPT impdp system/1104@localhost:1521/xepdb1 directory=MEDICAL_BACKUP_DIR dumpfile=MANUAL_LOGICAL_YYYYMMDDHH24MISS.DMP schemas=HOSPITAL_ADMIN table_exists_action=replace exclude=PROCACT_INSTANCE nologfile=yes
