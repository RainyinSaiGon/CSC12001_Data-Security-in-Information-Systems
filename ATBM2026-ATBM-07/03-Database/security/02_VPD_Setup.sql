SET SERVEROUTPUT ON;

PROMPT === Requirement 1 / VPD setup ===

BEGIN DBMS_RLS.DROP_POLICY(USER, 'HSBA', 'HSBA_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN DBMS_RLS.DROP_POLICY(USER, 'BENHNHAN', 'BENHNHAN_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN DBMS_RLS.DROP_POLICY(USER, 'HSBA_DV', 'HSBA_DV_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN DBMS_RLS.DROP_POLICY(USER, 'DONTHUOC', 'DONTHUOC_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
/

BEGIN EXECUTE IMMEDIATE 'DROP FUNCTION APP_CURRENT_MANV'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP FUNCTION APP_CURRENT_ROLE'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP FUNCTION VPD_HSBA_FN'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP FUNCTION VPD_BENHNHAN_FN'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP FUNCTION VPD_HSBA_DV_FN'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP FUNCTION VPD_DONTHUOC_FN'; EXCEPTION WHEN OTHERS THEN NULL; END;
/

CREATE OR REPLACE FUNCTION APP_CURRENT_MANV
RETURN VARCHAR2
AS
    v_manv NHANVIEN.MANV%TYPE;
BEGIN
    SELECT MANV
    INTO v_manv
    FROM NHANVIEN
    WHERE UPPER(USERNAME) = UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER'));

    RETURN v_manv;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RETURN NULL;
END;
/

CREATE OR REPLACE FUNCTION APP_CURRENT_ROLE
RETURN VARCHAR2
AS
    v_role NVARCHAR2(50);
BEGIN
    SELECT VAITRO
    INTO v_role
    FROM NHANVIEN
    WHERE UPPER(USERNAME) = UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER'));

    RETURN v_role;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RETURN NULL;
END;
/

CREATE OR REPLACE FUNCTION VPD_HSBA_FN(p_schema VARCHAR2, p_object VARCHAR2)
RETURN VARCHAR2
AS
    v_role NVARCHAR2(50);
    v_manv NHANVIEN.MANV%TYPE;
BEGIN
    IF UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER')) = UPPER(USER) THEN
        RETURN '1=1';
    END IF;

    v_role := APP_CURRENT_ROLE();
    v_manv := APP_CURRENT_MANV();

    IF v_role = N'Điều phối viên' THEN
        RETURN '1=1';
    ELSIF v_role = N'Bác sĩ/Y sĩ' THEN
        IF v_manv IS NULL THEN
            RETURN '1=0';
        END IF;
        RETURN 'MABS = ''' || REPLACE(v_manv, '''', '''''') || '''';
    END IF;

    RETURN '1=0';
END;
/


CREATE OR REPLACE FUNCTION VPD_BENHNHAN_FN(p_schema VARCHAR2, p_object VARCHAR2)
RETURN VARCHAR2
AS
    v_role NVARCHAR2(50);
    v_manv NHANVIEN.MANV%TYPE;
BEGIN
    IF UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER')) = UPPER(USER) THEN
        RETURN '1=1';
    END IF;

    v_role := APP_CURRENT_ROLE();
    v_manv := APP_CURRENT_MANV();

    IF v_role = N'Điều phối viên' THEN
        RETURN '1=1';
    ELSIF v_role = N'Bác sĩ/Y sĩ' THEN
        IF v_manv IS NULL THEN
            RETURN '1=0';
        END IF;
        RETURN 'MABN IN (SELECT MABN FROM HSBA WHERE MABS = ''' || REPLACE(v_manv, '''', '''''') || ''')';
    END IF;

    -- Patient self-row only
    RETURN 'UPPER(USERNAME) = UPPER(SYS_CONTEXT(''USERENV'', ''SESSION_USER''))';
END;
/

CREATE OR REPLACE FUNCTION VPD_HSBA_DV_FN(p_schema VARCHAR2, p_object VARCHAR2)
RETURN VARCHAR2
AS
    v_role NVARCHAR2(50);
    v_manv NHANVIEN.MANV%TYPE;
BEGIN
    IF UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER')) = UPPER(USER) THEN
        RETURN '1=1';
    END IF;

    v_role := APP_CURRENT_ROLE();
    v_manv := APP_CURRENT_MANV();

    IF v_role = N'Điều phối viên' THEN
        RETURN '1=1';
    ELSIF v_role = N'Bác sĩ/Y sĩ' THEN
        IF v_manv IS NULL THEN
            RETURN '1=0';
        END IF;
        RETURN 'MAHSBA IN (SELECT MAHSBA FROM HSBA WHERE MABS = ''' || REPLACE(v_manv, '''', '''''') || ''')';
    END IF;

    RETURN '1=0'; -- No access for others
END;
/

CREATE OR REPLACE FUNCTION VPD_DONTHUOC_FN(p_schema VARCHAR2, p_object VARCHAR2)
RETURN VARCHAR2
AS
    v_role NVARCHAR2(50);
    v_manv NHANVIEN.MANV%TYPE;
BEGIN
    IF UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER')) = UPPER(USER) THEN
        RETURN '1=1';
    END IF;

    v_role := APP_CURRENT_ROLE();
    v_manv := APP_CURRENT_MANV();

    IF v_role = N'Điều phối viên' THEN
        RETURN '1=1';
    ELSIF v_role = N'Bác sĩ/Y sĩ' THEN
        IF v_manv IS NULL THEN
            RETURN '1=0';
        END IF;
        RETURN 'MAHSBA IN (SELECT MAHSBA FROM HSBA WHERE MABS = ''' || REPLACE(v_manv, '''', '''''') || ''')';
    END IF;

    RETURN '1=0';
END;
/

BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => USER,
        object_name     => 'HSBA',
        policy_name     => 'HSBA_VPD',
        function_schema => USER,
        policy_function => 'VPD_HSBA_FN',
        statement_types => 'SELECT,UPDATE,DELETE,INSERT',
        update_check    => TRUE
    );

    DBMS_RLS.ADD_POLICY(
        object_schema   => USER,
        object_name     => 'BENHNHAN',
        policy_name     => 'BENHNHAN_VPD',
        function_schema => USER,
        policy_function => 'VPD_BENHNHAN_FN',
        statement_types => 'SELECT,UPDATE,DELETE,INSERT',
        update_check    => TRUE
    );

    DBMS_RLS.ADD_POLICY(
        object_schema   => USER,
        object_name     => 'HSBA_DV',
        policy_name     => 'HSBA_DV_VPD',
        function_schema => USER,
        policy_function => 'VPD_HSBA_DV_FN',
        statement_types => 'SELECT,UPDATE,DELETE,INSERT',
        update_check    => TRUE
    );

    DBMS_RLS.ADD_POLICY(
        object_schema   => USER,
        object_name     => 'DONTHUOC',
        policy_name     => 'DONTHUOC_VPD',
        function_schema => USER,
        policy_function => 'VPD_DONTHUOC_FN',
        statement_types => 'SELECT,UPDATE,DELETE,INSERT',
        update_check    => TRUE
    );
END;
/

GRANT EXECUTE ON APP_CURRENT_MANV TO PUBLIC;
GRANT EXECUTE ON APP_CURRENT_ROLE TO PUBLIC;
GRANT EXECUTE ON VPD_HSBA_FN TO PUBLIC;
GRANT EXECUTE ON VPD_BENHNHAN_FN TO PUBLIC;
GRANT EXECUTE ON VPD_HSBA_DV_FN TO PUBLIC;
GRANT EXECUTE ON VPD_DONTHUOC_FN TO PUBLIC;

COMMIT;

PROMPT === VPD setup completed ===
