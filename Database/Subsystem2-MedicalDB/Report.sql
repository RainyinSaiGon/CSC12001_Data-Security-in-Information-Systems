-- =====================================================
-- VERIFICATION REPORT 
-- =====================================================

SET SERVEROUTPUT ON SIZE UNLIMITED;
SET DEFINE OFF;
SET VERIFY OFF;
SET FEEDBACK OFF;
SET LINESIZE 150;
SET PAGESIZE 0;

DECLARE
    v_role_count         NUMBER;
    v_staff_users        NUMBER := 0;
    v_patient_users      NUMBER := 0;
    v_staff_with_roles   NUMBER := 0;
    v_patient_with_roles NUMBER := 0;
    v_policies_active    NUMBER;
    v_sql                VARCHAR2(300);
    v_current_count      NUMBER;
    v_total_records      NUMBER := 0;
BEGIN
    DBMS_OUTPUT.PUT_LINE(RPAD('=', 90, '='));
    DBMS_OUTPUT.PUT_LINE('     SECURITY AND DATA VERIFICATION REPORT');
    DBMS_OUTPUT.PUT_LINE('         MEDICAL DATABASE');
    DBMS_OUTPUT.PUT_LINE(RPAD('=', 90, '='));

    -- 1. ROLES
    SELECT COUNT(*) INTO v_role_count 
    FROM DBA_ROLES 
    WHERE ROLE IN ('DIEU_PHOI_VIEN','BAC_SI_Y_SI','KY_THUAT_VIEN','BENH_NHAN');

    IF v_role_count = 4 THEN
        DBMS_OUTPUT.PUT_LINE(CHR(10) || '[OK] All 4 security roles exist.');
    ELSE
        DBMS_OUTPUT.PUT_LINE(CHR(10) || '[FAIL] Only ' || v_role_count || '/4 roles found.');
    END IF;

    -- 2. USERS & ROLES 
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '--- USER ACCOUNTS ---');

    -- Staff Users
    BEGIN
        SELECT COUNT(*) INTO v_staff_users 
        FROM DBA_USERS WHERE USERNAME IN (SELECT USERNAME FROM NHANVIEN);
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -942 THEN  -- ORA-00942: table or view does not exist
                DBMS_OUTPUT.PUT_LINE('[WARN] NHANVIEN table missing or inaccessible - Staff users set to 0.');
            ELSE
                RAISE;
            END IF;
    END;

    -- Staff with Roles
    BEGIN
        SELECT COUNT(DISTINCT GRANTEE) INTO v_staff_with_roles
        FROM DBA_ROLE_PRIVS
        WHERE GRANTEE IN (SELECT USERNAME FROM NHANVIEN)
          AND GRANTED_ROLE IN ('DIEU_PHOI_VIEN','BAC_SI_Y_SI','KY_THUAT_VIEN');
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -942 THEN
                DBMS_OUTPUT.PUT_LINE('[WARN] NHANVIEN table missing or inaccessible - Staff roles set to 0.');
            ELSE
                RAISE;
            END IF;
    END;

    -- Patient Users
    BEGIN
        SELECT COUNT(*) INTO v_patient_users 
        FROM DBA_USERS WHERE USERNAME IN (SELECT USERNAME FROM BENHNHAN);
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -942 THEN
                DBMS_OUTPUT.PUT_LINE('[WARN] BENHNHAN table missing or inaccessible - Patient users set to 0.');
            ELSE
                RAISE;
            END IF;
    END;

    -- Patient with Roles
    BEGIN
        SELECT COUNT(DISTINCT GRANTEE) INTO v_patient_with_roles
        FROM DBA_ROLE_PRIVS
        WHERE GRANTEE IN (SELECT USERNAME FROM BENHNHAN)
          AND GRANTED_ROLE = 'BENH_NHAN';
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -942 THEN
                DBMS_OUTPUT.PUT_LINE('[WARN] BENHNHAN table missing or inaccessible - Patient roles set to 0.');
            ELSE
                RAISE;
            END IF;
    END;

    DBMS_OUTPUT.PUT_LINE('BENHNHAN (Patients)   : ' || v_patient_users || '   (with role: ' || v_patient_with_roles || ')');
    DBMS_OUTPUT.PUT_LINE('NHANVIEN (Staff)      : ' || v_staff_users || '   (with roles: ' || v_staff_with_roles || ')');

    -- 3. VPD
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '--- VPD POLICIES ---');
    FOR p IN (
        SELECT object_name, policy_name, enable 
        FROM user_policies 
        WHERE policy_name IN ('HSBA_DOCTOR_VPD', 'BENHNHAN_DOCTOR_VPD')
    ) LOOP
        DBMS_OUTPUT.PUT_LINE('Table: ' || RPAD(p.object_name,15) || 
                             ' | Policy: ' || RPAD(p.policy_name,25) || 
                             ' | Enabled: ' || p.enable);
    END LOOP;

    SELECT COUNT(*) INTO v_policies_active 
    FROM user_policies 
    WHERE policy_name IN ('HSBA_DOCTOR_VPD','BENHNHAN_DOCTOR_VPD') 
      AND enable = 'YES';

    IF v_policies_active = 2 THEN
        DBMS_OUTPUT.PUT_LINE('[OK] Both VPD policies are active.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('[WARN] VPD issue: ' || v_policies_active || '/2');
    END IF;

    -- 4. TABLE COUNTS
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '--- TABLE RECORD COUNTS ---');
    DBMS_OUTPUT.PUT_LINE(RPAD('Table Name', 32) || ' | Row Count');
    DBMS_OUTPUT.PUT_LINE(RPAD('-', 65, '-'));

    FOR t IN (
        SELECT table_name 
        FROM user_tables 
        WHERE table_name NOT LIKE 'BIN$%' 
        ORDER BY table_name
    ) LOOP
        BEGIN
            EXECUTE IMMEDIATE 'SELECT COUNT(*) FROM ' || t.table_name INTO v_current_count;
            DBMS_OUTPUT.PUT_LINE(RPAD(t.table_name,32) || ' | ' || 
                                 TRIM(TO_CHAR(v_current_count,'999,999,999')));
            v_total_records := v_total_records + v_current_count;
        EXCEPTION WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE(RPAD(t.table_name,32) || ' | [Error]');
        END;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE(RPAD('-',65,'-'));
    DBMS_OUTPUT.PUT_LINE(RPAD('TOTAL RECORDS',32) || ' | ' || 
                         TRIM(TO_CHAR(v_total_records,'999,999,999')));
    DBMS_OUTPUT.PUT_LINE(RPAD('=',90,'='));
    DBMS_OUTPUT.PUT_LINE('Verification completed at ' || TO_CHAR(SYSDATE,'DD-MON-YYYY HH24:MI:SS'));
END;
/