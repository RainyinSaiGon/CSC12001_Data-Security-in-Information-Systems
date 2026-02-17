/* ==========================================================================
   SYSTEM VERIFICATION REPORT
   ========================================================================== */
SET SERVEROUTPUT ON SIZE UNLIMITED;
SET LINESIZE 200;

DECLARE
    v_role_count NUMBER;
    v_staff_users NUMBER;
    v_patient_users NUMBER;
    v_staff_with_roles NUMBER;
    v_patient_with_roles NUMBER;
    v_policies_active NUMBER;
BEGIN
    DBMS_OUTPUT.PUT_LINE(RPAD('=', 60, '='));
    DBMS_OUTPUT.PUT_LINE(' SECURITY SETUP VERIFICATION REPORT');
    DBMS_OUTPUT.PUT_LINE(RPAD('=', 60, '='));

    -- =====================================================
    -- 1. CHECK ROLES EXIST
    -- =====================================================
    SELECT COUNT(*) INTO v_role_count 
    FROM DBA_ROLES 
    WHERE ROLE IN ('DIEU_PHOI_VIEN', 'BAC_SI_Y_SI', 'KY_THUAT_VIEN', 'BENH_NHAN');

    IF v_role_count = 4 THEN
        DBMS_OUTPUT.PUT_LINE('[OK] All 4 Security Roles exist.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('[FAIL] Only ' || v_role_count || '/4 Roles found.');
    END IF;

    -- =====================================================
    -- 2. CHECK USER ACCOUNTS & ASSIGNMENTS
    -- =====================================================
    -- Count total Staff users in DB
    SELECT COUNT(*) INTO v_staff_users 
    FROM DBA_USERS 
    WHERE USERNAME IN (SELECT USERNAME FROM NHANVIEN);

    -- Count Staff with valid Roles
    SELECT COUNT(DISTINCT GRANTEE) INTO v_staff_with_roles
    FROM DBA_ROLE_PRIVS
    WHERE GRANTEE IN (SELECT USERNAME FROM NHANVIEN)
      AND GRANTED_ROLE IN ('DIEU_PHOI_VIEN', 'BAC_SI_Y_SI', 'KY_THUAT_VIEN');

    -- Count total Patient users in DB
    SELECT COUNT(*) INTO v_patient_users 
    FROM DBA_USERS 
    WHERE USERNAME IN (SELECT USERNAME FROM BENHNHAN);

    -- Count Patients with 'BENH_NHAN' Role
    SELECT COUNT(DISTINCT GRANTEE) INTO v_patient_with_roles
    FROM DBA_ROLE_PRIVS
    WHERE GRANTED_ROLE = 'BENH_NHAN';

    DBMS_OUTPUT.PUT_LINE(CHR(10) || '--- USER ACCOUNTS ---');
    DBMS_OUTPUT.PUT_LINE('Staff Accounts Created:   ' || v_staff_users);
    DBMS_OUTPUT.PUT_LINE('Staff with Roles:         ' || v_staff_with_roles);
    
    DBMS_OUTPUT.PUT_LINE('Patient Accounts Created: ' || v_patient_users);
    DBMS_OUTPUT.PUT_LINE('Patients with Roles:      ' || v_patient_with_roles);

    IF v_staff_users > 0 AND v_staff_users = v_staff_with_roles THEN
        DBMS_OUTPUT.PUT_LINE('[OK] All Staff have roles.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('[WARN] Mismatch in Staff roles.');
    END IF;

    -- =====================================================
    -- 3. CHECK VPD POLICIES
    -- =====================================================
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '--- VPD POLICIES ---');
    
    FOR p IN (
        SELECT object_name, policy_name, enable 
        FROM user_policies 
        WHERE policy_name IN ('HSBA_DOCTOR_VPD', 'BENHNHAN_DOCTOR_VPD')
    ) LOOP
        DBMS_OUTPUT.PUT_LINE('Table: ' || RPAD(p.object_name, 10) || 
                             ' | Policy: ' || RPAD(p.policy_name, 20) || 
                             ' | Active: ' || p.enable);
    END LOOP;

    SELECT COUNT(*) INTO v_policies_active 
    FROM user_policies 
    WHERE policy_name IN ('HSBA_DOCTOR_VPD', 'BENHNHAN_DOCTOR_VPD') 
      AND enable = 'YES';

    IF v_policies_active = 2 THEN
        DBMS_OUTPUT.PUT_LINE('[OK] All VPD Policies are Active.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('[FAIL] VPD Policies missing or disabled.');
    END IF;

    DBMS_OUTPUT.PUT_LINE(RPAD('=', 60, '='));
END;
/