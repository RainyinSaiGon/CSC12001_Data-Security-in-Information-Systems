SET SERVEROUTPUT ON;
ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

DECLARE
    TYPE t_usernames IS TABLE OF VARCHAR2(30);
    v_staff_users   t_usernames;
    v_patient_users t_usernames;
    v_count         NUMBER := 0;
BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> STARTING FULL SYSTEM RESET...');

    -- 1. COLLECT ALL USERNAMES FIRST 
    DBMS_OUTPUT.PUT_LINE('Collecting usernames...');
    BEGIN
        SELECT USERNAME BULK COLLECT INTO v_staff_users 
        FROM NHANVIEN WHERE USERNAME IS NOT NULL;
        
        SELECT USERNAME BULK COLLECT INTO v_patient_users 
        FROM BENHNHAN WHERE USERNAME IS NOT NULL;
    EXCEPTION 
        WHEN OTHERS THEN 
            DBMS_OUTPUT.PUT_LINE('Warning: Could not read usernames (tables may not exist yet).');
    END;

    -- 2. DROP VPD POLICIES & FUNCTIONS
    DBMS_OUTPUT.PUT_LINE('Dropping VPD policies and functions...');
    BEGIN DBMS_RLS.DROP_POLICY(USER, 'HSBA', 'HSBA_DOCTOR_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN DBMS_RLS.DROP_POLICY(USER, 'BENHNHAN', 'BENHNHAN_DOCTOR_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN EXECUTE IMMEDIATE 'DROP FUNCTION VPD_HSBA_FUNCTION'; EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN EXECUTE IMMEDIATE 'DROP FUNCTION VPD_BENHNHAN_FUNCTION'; EXCEPTION WHEN OTHERS THEN NULL; END;

    -- 3. DROP USERS 
    DBMS_OUTPUT.PUT_LINE('Dropping staff users (' || v_staff_users.COUNT || ')...');
    FOR i IN 1..v_staff_users.COUNT LOOP
        BEGIN
            EXECUTE IMMEDIATE 'DROP USER ' || v_staff_users(i) || ' CASCADE';
        EXCEPTION WHEN OTHERS THEN NULL; END;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('Dropping patient users (' || v_patient_users.COUNT || ')...');
    FOR i IN 1..v_patient_users.COUNT LOOP
        BEGIN
            EXECUTE IMMEDIATE 'DROP USER ' || v_patient_users(i) || ' CASCADE';
            v_count := v_count + 1;
        EXCEPTION WHEN OTHERS THEN NULL; END;
    END LOOP;

    -- 4. DROP ROLES
    DBMS_OUTPUT.PUT_LINE('Dropping roles...');
    FOR r IN (SELECT role FROM dba_roles 
              WHERE role IN ('DIEU_PHOI_VIEN','BAC_SI_Y_SI','KY_THUAT_VIEN','BENH_NHAN')) LOOP
        BEGIN
            EXECUTE IMMEDIATE 'DROP ROLE ' || r.role;
            DBMS_OUTPUT.PUT_LINE('  Dropped role: ' || r.role);
        EXCEPTION WHEN OTHERS THEN NULL; END;
    END LOOP;

    -- 5. DROP ALL TABLES + SEQUENCES (with PURGE)
    DBMS_OUTPUT.PUT_LINE('Dropping tables and sequences...');
    FOR t IN (SELECT table_name FROM user_tables WHERE table_name NOT LIKE 'BIN$%') LOOP
        BEGIN
            EXECUTE IMMEDIATE 'DROP TABLE ' || t.table_name || ' CASCADE CONSTRAINTS PURGE';
        EXCEPTION WHEN OTHERS THEN NULL; END;
    END LOOP;

    FOR s IN (SELECT sequence_name FROM user_sequences) LOOP
        BEGIN
            EXECUTE IMMEDIATE 'DROP SEQUENCE ' || s.sequence_name;
        EXCEPTION WHEN OTHERS THEN NULL; END;
    END LOOP;

    COMMIT;
    DBMS_OUTPUT.PUT_LINE('=== FULL SYSTEM RESET COMPLETED SUCCESSFULLY ===');
    DBMS_OUTPUT.PUT_LINE('Dropped ' || v_staff_users.COUNT || ' staff + ' || v_count || ' patient users.');
    DBMS_OUTPUT.PUT_LINE('All tables, roles, VPD, sequences removed.');
END;
/