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
        audit_condition  => 'EXISTS (SELECT 1 FROM NHANVIEN NV WHERE NV.MANV = HSBA.MABS AND UPPER(NV.USERNAME) = UPPER(SYS_CONTEXT(''USERENV'',''SESSION_USER'')) AND NV.VAITRO = N''Bác sĩ/Y sĩ'')',
        audit_column     => 'CHANDOAN,DIEUTRI,KETLUAN',
        statement_types  => 'UPDATE',
        enable           => TRUE
    );

    DBMS_FGA.ADD_POLICY(
        object_schema    => USER,
        object_name      => 'HSBA',
        policy_name      => 'FGA_HSBA_INVALID_UPDATE',
        audit_condition  => 'NOT EXISTS (SELECT 1 FROM NHANVIEN NV WHERE NV.MANV = HSBA.MABS AND UPPER(NV.USERNAME) = UPPER(SYS_CONTEXT(''USERENV'',''SESSION_USER'')) AND NV.VAITRO = N''Bác sĩ/Y sĩ'')',
        audit_column     => 'CHANDOAN,DIEUTRI,KETLUAN',
        statement_types  => 'UPDATE',
        enable           => TRUE
    );

    DBMS_FGA.ADD_POLICY(
        object_schema    => USER,
        object_name      => 'HSBA_DV',
        policy_name      => 'FGA_HSBA_DV_ILLEGAL_DML',
        audit_condition  => 'NOT EXISTS (SELECT 1 FROM NHANVIEN NV WHERE UPPER(NV.USERNAME) = UPPER(SYS_CONTEXT(''USERENV'',''SESSION_USER'')) AND (NV.VAITRO = N''Điều phối viên'' OR (NV.VAITRO = N''Bác sĩ/Y sĩ'' AND EXISTS (SELECT 1 FROM HSBA H WHERE H.MAHSBA = HSBA_DV.MAHSBA AND H.MABS = NV.MANV)) OR (NV.VAITRO = N''Kỹ thuật viên'' AND NV.MANV = HSBA_DV.MAKTV)))',
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
