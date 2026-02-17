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
CREATE ROLE Dieu_phoi_vien;
CREATE ROLE Bac_si_Y_si;
CREATE ROLE Ky_thuat_vien;
CREATE ROLE Benh_nhan;

-- 3. Grant Permissions to Roles
-- Technician
GRANT SELECT ON HSBA_DV TO Ky_thuat_vien;
GRANT UPDATE (KETQUA) ON HSBA_DV TO Ky_thuat_vien;
GRANT SELECT ON BENHNHAN TO Ky_thuat_vien;
GRANT SELECT ON HSBA TO Ky_thuat_vien;

-- Patient
GRANT SELECT ON BENHNHAN TO Benh_nhan;
GRANT UPDATE (SONHA, TENDUONG, QUANHUYEN, TINHTP) ON BENHNHAN TO Benh_nhan;
GRANT SELECT ON HSBA TO Benh_nhan;
GRANT SELECT ON DONTHUOC TO Benh_nhan;
GRANT SELECT ON THONGBAO TO Benh_nhan;


BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> STEP 1 COMPLETE: Roles and Permissions defined.');
END;
/


-- =====================================================
-- STEP 2: CREATE USERS & ASSIGN ROLES (The Execution Loop)
-- =====================================================
DECLARE
    v_role_name VARCHAR2(30);
BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> STEP 2 START: Creating Users and Assigning Roles...');

    -- A. STAFF Loop (Create User + Assign Staff Role)
    FOR u IN (
        SELECT USERNAME, 
               CASE VAITRO 
                   WHEN N'Điều phối viên' THEN 'Dieu_phoi_vien'
                   WHEN N'Bác sĩ/Y sĩ' THEN 'Bac_si_Y_si'
                   WHEN N'Kỹ thuật viên' THEN 'Ky_thuat_vien'
               END AS ROLE_NAME
        FROM NHANVIEN 
        WHERE USERNAME IS NOT NULL
    ) LOOP
        BEGIN
            -- 1. Create User
            EXECUTE IMMEDIATE 'CREATE USER ' || u.USERNAME || ' IDENTIFIED BY "123"';
            EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || u.USERNAME;
            
            -- 2. Assign Role Immediately
            IF u.ROLE_NAME IS NOT NULL THEN
                EXECUTE IMMEDIATE 'GRANT ' || u.ROLE_NAME || ' TO ' || u.USERNAME;
            END IF;
            
        EXCEPTION 
            WHEN OTHERS THEN NULL; -- Skip if user already exists
        END;
    END LOOP;
    DBMS_OUTPUT.PUT_LINE('>>> STAFF ACCOUNTS CREATED.');

    -- B. PATIENT Loop (Create User + Assign Benh_nhan Role)
    -- This handles 100,000+ users in one efficient pass
    FOR u IN (SELECT USERNAME FROM BENHNHAN WHERE USERNAME IS NOT NULL) LOOP
        BEGIN
            -- 1. Create User
            EXECUTE IMMEDIATE 'CREATE USER ' || u.USERNAME || ' IDENTIFIED BY "123"';
            EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || u.USERNAME;
            
            -- 2. Assign Role Immediately
            EXECUTE IMMEDIATE 'GRANT Benh_nhan TO ' || u.USERNAME;
            
        EXCEPTION 
            WHEN OTHERS THEN NULL; -- Skip if user already exists
        END;
    END LOOP;
    
    DBMS_OUTPUT.PUT_LINE('>>> PATIENT ACCOUNTS CREATED.');
    DBMS_OUTPUT.PUT_LINE('>>> ALL DONE.');
END;
/

COMMIT;