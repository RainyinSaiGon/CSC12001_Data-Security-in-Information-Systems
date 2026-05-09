

SET SERVEROUTPUT ON SIZE UNLIMITED;
SET DEFINE OFF;
SET VERIFY OFF;
SET FEEDBACK OFF;
SET LINESIZE 180;
SET PAGESIZE 0;

PROMPT ====================================================================================================
PROMPT SECURITY AND DATA VERIFICATION REPORT
PROMPT ====================================================================================================

PROMPT [1/7] Roles
DECLARE
    v_role_count NUMBER := 0;
BEGIN
    BEGIN
        EXECUTE IMMEDIATE q'[
            SELECT COUNT(*)
            FROM DBA_ROLES
            WHERE ROLE IN ('DIEU_PHOI_VIEN', 'BAC_SI_Y_SI', 'KY_THUAT_VIEN', 'BENH_NHAN')
        ]'
        INTO v_role_count;

        IF v_role_count = 4 THEN
            DBMS_OUTPUT.PUT_LINE('[OK] All 4 application roles exist.');
        ELSE
            DBMS_OUTPUT.PUT_LINE('[WARN] Only ' || v_role_count || '/4 application roles exist.');
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('[WARN] Could not query DBA_ROLES in this session.');
    END;
END;
/

PROMPT [2/7] User accounts and role grants
DECLARE
    v_staff_users        NUMBER := 0;
    v_patient_users      NUMBER := 0;
    v_staff_with_roles   NUMBER := 0;
    v_patient_with_roles NUMBER := 0;
BEGIN
    BEGIN
        EXECUTE IMMEDIATE q'[
            SELECT COUNT(*)
            FROM DBA_USERS
            WHERE USERNAME IN (SELECT USERNAME FROM NHANVIEN WHERE USERNAME IS NOT NULL)
        ]'
        INTO v_staff_users;
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('[WARN] Could not count NHANVIEN mapped Oracle users.');
    END;

    BEGIN
        EXECUTE IMMEDIATE q'[
            SELECT COUNT(DISTINCT GRANTEE)
            FROM DBA_ROLE_PRIVS
            WHERE GRANTEE IN (SELECT USERNAME FROM NHANVIEN WHERE USERNAME IS NOT NULL)
              AND GRANTED_ROLE IN ('DIEU_PHOI_VIEN', 'BAC_SI_Y_SI', 'KY_THUAT_VIEN')
        ]'
        INTO v_staff_with_roles;
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('[WARN] Could not count NHANVIEN role grants.');
    END;

    BEGIN
        EXECUTE IMMEDIATE q'[
            SELECT COUNT(*)
            FROM DBA_USERS
            WHERE USERNAME IN (SELECT USERNAME FROM BENHNHAN WHERE USERNAME IS NOT NULL)
        ]'
        INTO v_patient_users;
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('[WARN] Could not count BENHNHAN mapped Oracle users.');
    END;

    BEGIN
        EXECUTE IMMEDIATE q'[
            SELECT COUNT(DISTINCT GRANTEE)
            FROM DBA_ROLE_PRIVS
            WHERE GRANTEE IN (SELECT USERNAME FROM BENHNHAN WHERE USERNAME IS NOT NULL)
              AND GRANTED_ROLE = 'BENH_NHAN'
        ]'
        INTO v_patient_with_roles;
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('[WARN] Could not count BENHNHAN role grants.');
    END;

    DBMS_OUTPUT.PUT_LINE('NHANVIEN mapped Oracle users : ' || v_staff_users || ' (with role grants: ' || v_staff_with_roles || ')');
    DBMS_OUTPUT.PUT_LINE('BENHNHAN mapped Oracle users : ' || v_patient_users || ' (with role grants: ' || v_patient_with_roles || ')');
END;
/

PROMPT [3/7] VPD policies
DECLARE
    v_vpd_policy_count NUMBER := 0;
    v_object_name      VARCHAR2(128);
    v_policy_name      VARCHAR2(128);
    v_enabled          VARCHAR2(10);
    c                  SYS_REFCURSOR;
BEGIN
    BEGIN
        OPEN c FOR q'[
            SELECT OBJECT_NAME, POLICY_NAME, ENABLE
            FROM USER_POLICIES
            WHERE POLICY_NAME IN ('HSBA_VPD', 'BENHNHAN_VPD', 'HSBA_DV_VPD', 'DONTHUOC_VPD')
            ORDER BY OBJECT_NAME, POLICY_NAME
        ]';

        LOOP
            FETCH c INTO v_object_name, v_policy_name, v_enabled;
            EXIT WHEN c%NOTFOUND;
            DBMS_OUTPUT.PUT_LINE(RPAD(v_object_name, 15) || ' | ' || RPAD(v_policy_name, 18) || ' | ENABLED=' || v_enabled);
        END LOOP;
        CLOSE c;

        EXECUTE IMMEDIATE q'[
            SELECT COUNT(*)
            FROM USER_POLICIES
            WHERE POLICY_NAME IN ('HSBA_VPD', 'BENHNHAN_VPD', 'HSBA_DV_VPD', 'DONTHUOC_VPD')
              AND ENABLE = 'YES'
        ]'
        INTO v_vpd_policy_count;

        IF v_vpd_policy_count = 4 THEN
            DBMS_OUTPUT.PUT_LINE('[OK] All 4 VPD policies are active.');
        ELSE
            DBMS_OUTPUT.PUT_LINE('[WARN] Active VPD policies: ' || v_vpd_policy_count || '/4.');
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('[WARN] Could not verify USER_POLICIES in this session.');
    END;
END;
/

PROMPT [4/7] OLS check
DECLARE
    v_has_ols_label_column NUMBER := 0;
BEGIN
    BEGIN
        EXECUTE IMMEDIATE q'[
            SELECT COUNT(*)
            FROM USER_TAB_COLUMNS
            WHERE TABLE_NAME = 'THONGBAO'
              AND COLUMN_NAME = 'OLS_LABEL'
        ]'
        INTO v_has_ols_label_column;

        IF v_has_ols_label_column = 1 THEN
            DBMS_OUTPUT.PUT_LINE('[OK] THONGBAO contains OLS_LABEL column.');
        ELSE
            DBMS_OUTPUT.PUT_LINE('[WARN] THONGBAO does not contain OLS_LABEL column.');
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('[WARN] Could not verify OLS label column.');
    END;
END;
/

PROMPT [5/7] FGA policies
DECLARE
    v_fga_policy_count NUMBER := 0;
    v_object_name      VARCHAR2(128);
    v_policy_name      VARCHAR2(128);
    v_enabled          VARCHAR2(10);
    c                  SYS_REFCURSOR;
BEGIN
    BEGIN
        OPEN c FOR q'[
            SELECT OBJECT_NAME, POLICY_NAME, ENABLED
            FROM USER_AUDIT_POLICIES
            WHERE POLICY_NAME IN (
                'FGA_DONTHUOC_AFTER_CREATE',
                'FGA_HSBA_VALID_UPDATE',
                'FGA_HSBA_INVALID_UPDATE',
                'FGA_HSBA_DV_ILLEGAL_DML'
            )
            ORDER BY OBJECT_NAME, POLICY_NAME
        ]';

        LOOP
            FETCH c INTO v_object_name, v_policy_name, v_enabled;
            EXIT WHEN c%NOTFOUND;
            DBMS_OUTPUT.PUT_LINE(RPAD(v_object_name, 15) || ' | ' || RPAD(v_policy_name, 28) || ' | ENABLED=' || v_enabled);
        END LOOP;
        CLOSE c;

        EXECUTE IMMEDIATE q'[
            SELECT COUNT(*)
            FROM USER_AUDIT_POLICIES
            WHERE POLICY_NAME IN (
                'FGA_DONTHUOC_AFTER_CREATE',
                'FGA_HSBA_VALID_UPDATE',
                'FGA_HSBA_INVALID_UPDATE',
                'FGA_HSBA_DV_ILLEGAL_DML'
            )
              AND ENABLED = 'YES'
        ]'
        INTO v_fga_policy_count;

        IF v_fga_policy_count = 4 THEN
            DBMS_OUTPUT.PUT_LINE('[OK] All 4 FGA policies are active.');
        ELSE
            DBMS_OUTPUT.PUT_LINE('[WARN] Active FGA policies: ' || v_fga_policy_count || '/4.');
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('[WARN] USER_AUDIT_POLICIES is not accessible in this session.');
    END;
END;
/

PROMPT [6/7] Table record counts
DECLARE
    v_current_count NUMBER := 0;
    v_total_records NUMBER := 0;
BEGIN
    DBMS_OUTPUT.PUT_LINE(RPAD('TABLE NAME', 32) || ' | ROW COUNT');
    DBMS_OUTPUT.PUT_LINE(RPAD('-', 60, '-'));

    FOR t IN (
        SELECT TABLE_NAME
        FROM USER_TABLES
        WHERE TABLE_NAME NOT LIKE 'BIN$%'
        ORDER BY TABLE_NAME
    ) LOOP
        BEGIN
            EXECUTE IMMEDIATE 'SELECT COUNT(*) FROM ' || t.TABLE_NAME INTO v_current_count;
            DBMS_OUTPUT.PUT_LINE(RPAD(t.TABLE_NAME, 32) || ' | ' || TRIM(TO_CHAR(v_current_count, '999,999,999')));
            v_total_records := v_total_records + v_current_count;
        EXCEPTION
            WHEN OTHERS THEN
                DBMS_OUTPUT.PUT_LINE(RPAD(t.TABLE_NAME, 32) || ' | [ERROR]');
        END;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE(RPAD('-', 60, '-'));
    DBMS_OUTPUT.PUT_LINE(RPAD('TOTAL RECORDS', 32) || ' | ' || TRIM(TO_CHAR(v_total_records, '999,999,999')));
END;
/

PROMPT [7/7] Audit trail counts
DECLARE
    v_current_count NUMBER := 0;
BEGIN
    BEGIN
        EXECUTE IMMEDIATE q'[
            SELECT COUNT(*)
            FROM DBA_AUDIT_TRAIL
            WHERE OBJ_NAME IN ('BENHNHAN', 'HSBA', 'HSBA_DV', 'DONTHUOC')
        ]'
        INTO v_current_count;

        DBMS_OUTPUT.PUT_LINE('DBA_AUDIT_TRAIL relevant rows     : ' || v_current_count);
        IF v_current_count = 0 THEN
            DBMS_OUTPUT.PUT_LINE('[INFO] No standard-audit rows yet. Run some demo actions first, then read the logs again.');
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('[WARN] DBA_AUDIT_TRAIL is not accessible in this session.');
    END;

    BEGIN
        EXECUTE IMMEDIATE q'[
            SELECT COUNT(*)
            FROM DBA_FGA_AUDIT_TRAIL
            WHERE OBJECT_NAME IN ('HSBA', 'HSBA_DV', 'DONTHUOC')
        ]'
        INTO v_current_count;

        DBMS_OUTPUT.PUT_LINE('DBA_FGA_AUDIT_TRAIL relevant rows : ' || v_current_count);
        IF v_current_count = 0 THEN
            DBMS_OUTPUT.PUT_LINE('[INFO] No FGA rows yet. Run the audited doctor/technician actions first, then read the logs again.');
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('[WARN] DBA_FGA_AUDIT_TRAIL is not accessible in this session.');
    END;
END;
/

PROMPT ====================================================================================================
PROMPT Verification completed.
PROMPT ====================================================================================================



SELECT * FROM BENHNHAN WHERE USERNAME = '999999990001';