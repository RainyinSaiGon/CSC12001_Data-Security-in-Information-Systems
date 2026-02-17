/* ==========================================================================
    VPD SETUP (Virtual Private Database)
   ========================================================================== */
ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

-- =====================================================
-- 1. CREATE POLICY FUNCTION: MEDICAL RECORDS (HSBA)
-- =====================================================
CREATE OR REPLACE FUNCTION VPD_HSBA_FUNCTION(
    p_schema_name VARCHAR2,
    p_object_name VARCHAR2
) RETURN VARCHAR2 AS
    v_user VARCHAR2(30) := SYS_CONTEXT('USERENV', 'SESSION_USER');
    v_role VARCHAR2(50);
    v_manv NUMBER;
BEGIN
    -- If user is not staff (e.g., ADMIN or Patient), allow access (filtered elsewhere)
    IF v_user NOT LIKE 'NV%' THEN RETURN '1=1'; END IF;

    -- Extract Employee ID from Username (e.g., 'NV001' -> 1)
    v_manv := TO_NUMBER(REGEXP_SUBSTR(v_user, '\d+'));

    -- Get Role
    BEGIN
        SELECT VAITRO INTO v_role FROM NHANVIEN WHERE MANV = v_manv;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN '1=1'; -- Fallback
    END;

    -- POLICY LOGIC
    IF v_role = N'Điều phối viên' THEN
        RETURN '1=1'; -- Coordinator sees all
    ELSIF v_role = N'Bác sĩ/Y sĩ' THEN
        RETURN 'MABS = ' || v_manv; -- Doctor sees only assigned records
    ELSE
        RETURN '1=1'; -- Default for others
    END IF;
END;
/

-- =====================================================
-- 2. CREATE POLICY FUNCTION: PATIENTS (BENHNHAN)
-- =====================================================
CREATE OR REPLACE FUNCTION VPD_BENHNHAN_FUNCTION(
    p_schema_name VARCHAR2,
    p_object_name VARCHAR2
) RETURN VARCHAR2 AS
    v_user VARCHAR2(30) := SYS_CONTEXT('USERENV', 'SESSION_USER');
    v_role VARCHAR2(50);
    v_manv NUMBER;
BEGIN
    IF v_user NOT LIKE 'NV%' THEN RETURN '1=1'; END IF;

    v_manv := TO_NUMBER(REGEXP_SUBSTR(v_user, '\d+'));

    BEGIN
        SELECT VAITRO INTO v_role FROM NHANVIEN WHERE MANV = v_manv;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN '1=1';
    END;

    -- POLICY LOGIC
    IF v_role = N'Điều phối viên' THEN
        RETURN '1=1'; -- Coordinator sees all
    ELSIF v_role = N'Bác sĩ/Y sĩ' THEN
        -- Doctor sees only patients they have treated in HSBA
        RETURN 'MABN IN (SELECT DISTINCT MABN FROM HSBA WHERE MABS = ' || v_manv || ')';
    ELSE
        RETURN '1=1';
    END IF;
END;
/

-- =====================================================
-- 3. APPLY POLICIES (DROP & ADD)
-- =====================================================
DECLARE
    v_schema VARCHAR2(30) := USER; -- Automatically uses current schema
BEGIN
    -- 1. HSBA POLICY
    BEGIN DBMS_RLS.DROP_POLICY(v_schema, 'HSBA', 'HSBA_DOCTOR_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
    
    DBMS_RLS.ADD_POLICY(
        object_schema   => v_schema,
        object_name     => 'HSBA',
        policy_name     => 'HSBA_DOCTOR_VPD',
        function_schema => v_schema,
        policy_function => 'VPD_HSBA_FUNCTION',
        statement_types => 'SELECT,INSERT,UPDATE,DELETE',
        update_check    => TRUE
    );

    -- 2. BENHNHAN POLICY
    BEGIN DBMS_RLS.DROP_POLICY(v_schema, 'BENHNHAN', 'BENHNHAN_DOCTOR_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;

    DBMS_RLS.ADD_POLICY(
        object_schema   => v_schema,
        object_name     => 'BENHNHAN',
        policy_name     => 'BENHNHAN_DOCTOR_VPD',
        function_schema => v_schema,
        policy_function => 'VPD_BENHNHAN_FUNCTION',
        statement_types => 'SELECT,INSERT,UPDATE,DELETE',
        update_check    => TRUE
    );
END;
/

-- =====================================================
-- 4. GRANT PERMISSIONS
-- =====================================================
GRANT EXECUTE ON VPD_HSBA_FUNCTION TO PUBLIC;
GRANT EXECUTE ON VPD_BENHNHAN_FUNCTION TO PUBLIC;

COMMIT;

