/* ==========================================================================
   FULL SYSTEM RESET: DROP VPD, ROLES, AND USERS
   ========================================================================== */
SET SERVEROUTPUT ON;
-- Enable script mode for CDB environments
ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

DECLARE
    v_count NUMBER := 0;
BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> STARTING SYSTEM CLEANUP...');

    -- =====================================================
    -- 1. DROP VPD POLICIES (Must be done first)
    -- =====================================================
    -- Drop HSBA Policy
    BEGIN
        DBMS_RLS.DROP_POLICY(USER, 'HSBA', 'HSBA_DOCTOR_VPD');
        DBMS_OUTPUT.PUT_LINE('Dropped Policy: HSBA_DOCTOR_VPD');
    EXCEPTION WHEN OTHERS THEN NULL; END;

    -- Drop BENHNHAN Policy
    BEGIN
        DBMS_RLS.DROP_POLICY(USER, 'BENHNHAN', 'BENHNHAN_DOCTOR_VPD');
        DBMS_OUTPUT.PUT_LINE('Dropped Policy: BENHNHAN_DOCTOR_VPD');
    EXCEPTION WHEN OTHERS THEN NULL; END;

    -- =====================================================
    -- 2. DROP POLICY FUNCTIONS
    -- =====================================================
    BEGIN
        EXECUTE IMMEDIATE 'DROP FUNCTION VPD_HSBA_FUNCTION';
        DBMS_OUTPUT.PUT_LINE('Dropped Function: VPD_HSBA_FUNCTION');
    EXCEPTION WHEN OTHERS THEN NULL; END;

    BEGIN
        EXECUTE IMMEDIATE 'DROP FUNCTION VPD_BENHNHAN_FUNCTION';
        DBMS_OUTPUT.PUT_LINE('Dropped Function: VPD_BENHNHAN_FUNCTION');
    EXCEPTION WHEN OTHERS THEN NULL; END;

    -- =====================================================
    -- 3. DROP ROLES
    -- =====================================================
    FOR r IN (SELECT role FROM dba_roles WHERE role IN ('DIEU_PHOI_VIEN','BAC_SI_Y_SI','KY_THUAT_VIEN','BENH_NHAN')) LOOP
        BEGIN
            EXECUTE IMMEDIATE 'DROP ROLE ' || r.role;
            DBMS_OUTPUT.PUT_LINE('Dropped Role: ' || r.role);
        EXCEPTION WHEN OTHERS THEN NULL; END;
    END LOOP;

    -- =====================================================
    -- 4. DROP USER ACCOUNTS (Staff & Patients)
    -- =====================================================
    DBMS_OUTPUT.PUT_LINE('>>> DROPPING USERS (This may take time)...');

    -- Drop Staff
    FOR u IN (SELECT USERNAME FROM NHANVIEN WHERE USERNAME IS NOT NULL) LOOP
        BEGIN
            EXECUTE IMMEDIATE 'DROP USER ' || u.USERNAME || ' CASCADE';
        EXCEPTION WHEN OTHERS THEN NULL; END;
    END LOOP;
    DBMS_OUTPUT.PUT_LINE('Dropped all Staff users.');

    -- Drop Patients
    FOR u IN (SELECT USERNAME FROM BENHNHAN WHERE USERNAME IS NOT NULL) LOOP
        BEGIN
            EXECUTE IMMEDIATE 'DROP USER ' || u.USERNAME || ' CASCADE';
            v_count := v_count + 1;
        EXCEPTION WHEN OTHERS THEN NULL; END;
    END LOOP;
    
    DBMS_OUTPUT.PUT_LINE('Dropped ' || v_count || ' Patient users.');
    DBMS_OUTPUT.PUT_LINE('>>> SYSTEM CLEANUP COMPLETE.');
END;
/