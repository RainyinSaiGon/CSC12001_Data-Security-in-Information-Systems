/* ==========================================================================
   USER CREATION  +  RBAC SETUP 
   ========================================================================== */
SET SERVEROUTPUT ON;
ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

-- =====================================================
-- STEP 1: DEFINE ROLES & PERMISSIONS (The Foundation)
-- =====================================================
BEGIN
    -- 1. Clean up old roles (if they exist)
    FOR r IN (SELECT role FROM dba_roles WHERE role IN ('DIEU_PHOI_VIEN','BAC_SI_Y_SI','KY_THUAT_VIEN','BENH_NHAN')) LOOP
        EXECUTE IMMEDIATE 'DROP ROLE ' || r.role;
    END LOOP;
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

-- 2. Create the Roles
CREATE ROLE DIEU_PHOI_VIEN;
CREATE ROLE BAC_SI_Y_SI;
CREATE ROLE KY_THUAT_VIEN;
CREATE ROLE BENH_NHAN;

-- 3. Grant Permissions to Roles
-- Technician
GRANT SELECT ON HSBA_DV TO KY_THUAT_VIEN;
GRANT UPDATE (KETQUA) ON HSBA_DV TO KY_THUAT_VIEN;
GRANT SELECT ON BENHNHAN TO KY_THUAT_VIEN;
GRANT SELECT ON HSBA TO KY_THUAT_VIEN;

-- Patient
GRANT SELECT ON BENHNHAN TO BENH_NHAN;
GRANT UPDATE (SONHA, TENDUONG, QUANHUYEN, TINHTP) ON BENHNHAN TO BENH_NHAN;
GRANT SELECT ON HSBA TO BENH_NHAN;
GRANT SELECT ON DONTHUOC TO BENH_NHAN;
GRANT SELECT ON THONGBAO TO BENH_NHAN;


BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> STEP 1 COMPLETE: Roles and Permissions defined.');
END;
/


-- =====================================================
-- STEP 2: CREATE USERS & ASSIGN ROLES (The Execution Loop)
-- =====================================================
DECLARE
    TYPE t_staff IS TABLE OF ROWID;
    TYPE t_patient IS TABLE OF ROWID;
    TYPE t_usernames IS TABLE OF VARCHAR2(30);
    TYPE t_roles IS TABLE OF VARCHAR2(30);
    
    v_staff_usernames t_usernames;
    v_staff_roles     t_roles;
    v_patient_usernames t_usernames;
    
    v_skipped_staff    NUMBER := 0;
    v_created_staff   NUMBER := 0;
    v_skipped_patients NUMBER := 0;
    v_created_patients NUMBER := 0;
    v_error_count      NUMBER := 0;  -- New: Count errors instead of printing each
BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> STEP 2 START: Creating Users and Assigning Roles...');

    -- A. COLLECT STAFF DATA (Handle missing table)
    BEGIN
        SELECT USERNAME, 
               CASE VAITRO 
                   WHEN N'Điều phối viên' THEN 'DIEU_PHOI_VIEN'
                   WHEN N'Bác sĩ/Y sĩ' THEN 'BAC_SI_Y_SI'
                   WHEN N'Kỹ thuật viên' THEN 'KY_THUAT_VIEN'
               END AS ROLE_NAME
        BULK COLLECT INTO v_staff_usernames, v_staff_roles
        FROM NHANVIEN 
        WHERE USERNAME IS NOT NULL;
        
        DBMS_OUTPUT.PUT_LINE('Collected ' || v_staff_usernames.COUNT || ' staff records.');
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -942 THEN  -- ORA-00942: table or view does not exist
                DBMS_OUTPUT.PUT_LINE('[WARN] NHANVIEN table missing or inaccessible - Skipping staff creation.');
            ELSE
                RAISE;
            END IF;
    END;

    -- B. STAFF Loop (Create User + Assign Staff Role)
    IF v_staff_usernames.COUNT > 0 THEN
        FOR i IN 1..v_staff_usernames.COUNT LOOP
            BEGIN
                -- 1. Create User
                EXECUTE IMMEDIATE 'CREATE USER ' || v_staff_usernames(i) || ' IDENTIFIED BY "123"';
                EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || v_staff_usernames(i);
                
                -- 2. Assign Role Immediately
                IF v_staff_roles(i) IS NOT NULL THEN
                    EXECUTE IMMEDIATE 'GRANT ' || v_staff_roles(i) || ' TO ' || v_staff_usernames(i);
                END IF;
                
                v_created_staff := v_created_staff + 1;
            EXCEPTION 
                WHEN OTHERS THEN
                    IF SQLCODE IN (-1920, -1) THEN  -- ORA-01920: user/role conflict, ORA-00001: unique constraint
                        v_skipped_staff := v_skipped_staff + 1;
                        -- Optionally re-grant role and session if needed
                        BEGIN
                            EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || v_staff_usernames(i);
                            IF v_staff_roles(i) IS NOT NULL THEN
                                EXECUTE IMMEDIATE 'GRANT ' || v_staff_roles(i) || ' TO ' || v_staff_usernames(i);
                            END IF;
                        EXCEPTION WHEN OTHERS THEN NULL; END;
                    ELSE
                        v_error_count := v_error_count + 1;  -- Count unexpected errors
                    END IF;
            END;
        END LOOP;
        DBMS_OUTPUT.PUT_LINE('>>> STAFF ACCOUNTS: Created ' || v_created_staff || ', Skipped ' || v_skipped_staff || '.');
    END IF;

    -- C. COLLECT PATIENT DATA (Handle missing table)
    BEGIN
        SELECT USERNAME
        BULK COLLECT INTO v_patient_usernames
        FROM BENHNHAN 
        WHERE USERNAME IS NOT NULL;
        
        DBMS_OUTPUT.PUT_LINE('Collected ' || v_patient_usernames.COUNT || ' patient records.');
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE = -942 THEN
                DBMS_OUTPUT.PUT_LINE('[WARN] BENHNHAN table missing or inaccessible - Skipping patient creation.');
            ELSE
                RAISE;
            END IF;
    END;

    -- D. PATIENT Loop (Create User + Assign Benh_nhan Role)
    IF v_patient_usernames.COUNT > 0 THEN
        FOR i IN 1..v_patient_usernames.COUNT LOOP
            BEGIN
                -- 1. Create User
                EXECUTE IMMEDIATE 'CREATE USER ' || v_patient_usernames(i) || ' IDENTIFIED BY "123"';
                EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || v_patient_usernames(i);
                
                -- 2. Assign Role Immediately
                EXECUTE IMMEDIATE 'GRANT BENH_NHAN TO ' || v_patient_usernames(i);
                
                v_created_patients := v_created_patients + 1;
                
                -- Batch commit every 2000 to avoid resource issues
                IF MOD(i, 2000) = 0 THEN 
                    COMMIT; 
                    DBMS_OUTPUT.PUT_LINE('Committed patient batch at ' || i || ' (Created so far: ' || v_created_patients || ')');
                END IF;
            EXCEPTION 
                WHEN OTHERS THEN
                    IF SQLCODE IN (-1920, -1) THEN
                        v_skipped_patients := v_skipped_patients + 1;
                        -- Optionally re-grant role and session
                        BEGIN
                            EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || v_patient_usernames(i);
                            EXECUTE IMMEDIATE 'GRANT BENH_NHAN TO ' || v_patient_usernames(i);
                        EXCEPTION WHEN OTHERS THEN NULL; END;
                    ELSE
                        v_error_count := v_error_count + 1;  -- Count unexpected errors
                    END IF;
            END;
        END LOOP;
        COMMIT;
        DBMS_OUTPUT.PUT_LINE('>>> PATIENT ACCOUNTS: Created ' || v_created_patients || ', Skipped ' || v_skipped_patients || '.');
    END IF;

    DBMS_OUTPUT.PUT_LINE('Total unexpected errors: ' || v_error_count);
    DBMS_OUTPUT.PUT_LINE('>>> ALL DONE.');
END;
/