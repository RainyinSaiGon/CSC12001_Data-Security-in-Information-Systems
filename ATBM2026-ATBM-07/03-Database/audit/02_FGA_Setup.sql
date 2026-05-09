SET SERVEROUTPUT ON;

PROMPT === Requirement 3 / FGA setup ===

BEGIN DBMS_FGA.DROP_POLICY(USER, 'DONTHUOC', 'FGA_DONTHUOC_AFTER_CREATE'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN DBMS_FGA.DROP_POLICY(USER, 'HSBA', 'FGA_HSBA_VALID_UPDATE'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN DBMS_FGA.DROP_POLICY(USER, 'HSBA', 'FGA_HSBA_INVALID_UPDATE'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN DBMS_FGA.DROP_POLICY(USER, 'HSBA_DV', 'FGA_HSBA_DV_ILLEGAL_DML'); EXCEPTION WHEN OTHERS THEN NULL; END;
/

CREATE OR REPLACE FUNCTION CHECK_IS_OWNING_DOCTOR(p_mabs IN VARCHAR2) RETURN NUMBER IS
    v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count
    FROM NHANVIEN
    WHERE MANV = p_mabs
      AND UPPER(USERNAME) = UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER'))
      AND VAITRO = N'Bác sĩ/Y sĩ';
    RETURN v_count;
EXCEPTION
    WHEN OTHERS THEN RETURN 0;
END;
/

CREATE OR REPLACE FUNCTION CHECK_ILLEGAL_HSBA_DV(p_mahsba IN NUMBER, p_maktv IN VARCHAR2) RETURN NUMBER IS
    v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count
    FROM NHANVIEN NV
    WHERE UPPER(NV.USERNAME) = UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER'))
      AND (
          NV.VAITRO = N'Điều phối viên' 
          OR (NV.VAITRO = N'Bác sĩ/Y sĩ' AND EXISTS (SELECT 1 FROM HSBA H WHERE H.MAHSBA = p_mahsba AND H.MABS = NV.MANV)) 
          OR (NV.VAITRO = N'Kỹ thuật viên' AND NV.MANV = p_maktv)
      );
      
    IF v_count > 0 THEN
        RETURN 0;
    ELSE
        RETURN 1;
    END IF;
EXCEPTION
    WHEN OTHERS THEN RETURN 1;
END;
/

BEGIN
    DBMS_FGA.ADD_POLICY(
        object_schema   => USER,
        object_name     => 'DONTHUOC',
        policy_name     => 'FGA_DONTHUOC_AFTER_CREATE',
        audit_column    => 'MAHSBA,NGAYDT,TENTHUOC,LIEUDUNG',
        statement_types => 'UPDATE',
        enable          => TRUE
    );

    DBMS_FGA.ADD_POLICY(
        object_schema    => USER,
        object_name      => 'HSBA',
        policy_name      => 'FGA_HSBA_VALID_UPDATE',
        audit_condition  => 'CHECK_IS_OWNING_DOCTOR(MABS) = 1',
        audit_column     => 'CHANDOAN,DIEUTRI,KETLUAN',
        statement_types  => 'UPDATE',
        enable           => TRUE
    );

    DBMS_FGA.ADD_POLICY(
        object_schema    => USER,
        object_name      => 'HSBA',
        policy_name      => 'FGA_HSBA_INVALID_UPDATE',
        audit_condition  => 'CHECK_IS_OWNING_DOCTOR(MABS) = 0',
        audit_column     => 'CHANDOAN,DIEUTRI,KETLUAN',
        statement_types  => 'UPDATE',
        enable           => TRUE
    );

    DBMS_FGA.ADD_POLICY(
        object_schema    => USER,
        object_name      => 'HSBA_DV',
        policy_name      => 'FGA_HSBA_DV_ILLEGAL_DML',
        audit_condition  => 'CHECK_ILLEGAL_HSBA_DV(MAHSBA, MAKTV) = 1',
        statement_types  => 'INSERT,UPDATE,DELETE',
        enable           => TRUE
    );
END;
/

PROMPT FGA policies created for:
PROMPT - DONTHUOC post-creation updates
PROMPT - valid HSBA updates
PROMPT - invalid HSBA updates
PROMPT - illegal HSBA_DV DML
