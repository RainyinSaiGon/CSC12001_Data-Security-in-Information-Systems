
SET SERVEROUTPUT ON;
WHENEVER SQLERROR EXIT SQL.SQLCODE;

PROMPT === Requirement 2 / OLS setup ===
PROMPT Pass 1 - create THONGBAO_OLS if it does not exist, then reconnect.
PROMPT Pass 2 - reconnect as the same user and rerun this script to finish labels/data setup.

DECLARE
    v_role_enabled NUMBER := 0;

    FUNCTION policy_exists(p_policy_name IN VARCHAR2) RETURN NUMBER IS
        v_count NUMBER := 0;
    BEGIN
        BEGIN
            SELECT COUNT(*)
            INTO v_count
            FROM DBA_SA_POLICIES
            WHERE POLICY_NAME = p_policy_name;
        EXCEPTION
            WHEN OTHERS THEN
                SELECT COUNT(*)
                INTO v_count
                FROM ALL_SA_POLICIES
                WHERE POLICY_NAME = p_policy_name;
        END;

        RETURN v_count;
    END;

    PROCEDURE cleanup_policy(p_policy_name IN VARCHAR2) IS
    BEGIN
        BEGIN
            SA_POLICY_ADMIN.REMOVE_TABLE_POLICY(
                policy_name => p_policy_name,
                schema_name => USER,
                table_name  => 'THONGBAO',
                drop_column => TRUE
            );
        EXCEPTION
            WHEN OTHERS THEN NULL;
        END;

        BEGIN
            SA_SYSDBA.DISABLE_POLICY(policy_name => p_policy_name);
        EXCEPTION
            WHEN OTHERS THEN NULL;
        END;

        BEGIN
            SA_SYSDBA.DROP_POLICY(policy_name => p_policy_name, drop_column => TRUE);
        EXCEPTION
            WHEN OTHERS THEN NULL;
        END;
    END;

BEGIN
    IF policy_exists('THONGBAO_OLS') = 0 THEN
        cleanup_policy('THONGBAO_OLS');
        cleanup_policy('HOS_OLS_POL');

        BEGIN
            EXECUTE IMMEDIATE 'ALTER TABLE THONGBAO DROP COLUMN OLS_LABEL';
        EXCEPTION
            WHEN OTHERS THEN NULL;
        END;

        SA_SYSDBA.CREATE_POLICY(
            policy_name     => 'THONGBAO_OLS',
            column_name     => 'OLS_LABEL',
            default_options => 'READ_CONTROL'
        );

        RAISE_APPLICATION_ERROR(
            -20032,
            'THONGBAO_OLS policy created. Disconnect and reconnect as ' || USER ||
            ', then rerun 03_OLS_Setup.sql so THONGBAO_OLS_DBA is enabled in the new session.'
        );
    END IF;

    SELECT COUNT(*)
    INTO v_role_enabled
    FROM SESSION_ROLES
    WHERE ROLE = 'THONGBAO_OLS_DBA';

    IF v_role_enabled = 0 THEN
        RAISE_APPLICATION_ERROR(
            -20033,
            'THONGBAO_OLS_DBA is not enabled in this session. Reconnect and rerun 03_OLS_Setup.sql.'
        );
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE = -12458 THEN
            RAISE_APPLICATION_ERROR(
                -20034,
                'Oracle Label Security is not enabled in this database. Connect as SYSDBA to the project PDB, run LBACSYS.CONFIGURE_OLS and LBACSYS.OLS_ENFORCEMENT.ENABLE_OLS, restart the database, rerun Create_HOSPITAL_ADMIN.sql, reconnect as HOSPITAL_ADMIN, then rerun 03_OLS_Setup.sql.'
            );
        END IF;
        RAISE;
END;
/

DECLARE
    FUNCTION table_policy_applied RETURN NUMBER IS
        v_count NUMBER := 0;
    BEGIN
        BEGIN
            SELECT COUNT(*)
            INTO v_count
            FROM DBA_SA_TABLE_POLICIES
            WHERE POLICY_NAME = 'THONGBAO_OLS'
              AND SCHEMA_NAME = USER
              AND TABLE_NAME = 'THONGBAO';
        EXCEPTION
            WHEN OTHERS THEN
                SELECT COUNT(*)
                INTO v_count
                FROM ALL_SA_TABLE_POLICIES
                WHERE POLICY_NAME = 'THONGBAO_OLS'
                  AND SCHEMA_NAME = USER
                  AND TABLE_NAME = 'THONGBAO';
        END;

        RETURN v_count;
    END;

    PROCEDURE safe_create_level(
        p_number IN NUMBER,
        p_short_name IN VARCHAR2,
        p_long_name IN VARCHAR2
    ) IS
    BEGIN
        SA_COMPONENTS.CREATE_LEVEL('THONGBAO_OLS', p_number, p_short_name, p_long_name);
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE NOT IN (-12419, -1) THEN
                DBMS_OUTPUT.PUT_LINE('Creating level ' || p_short_name || ' failed: ' || SQLERRM);
                RAISE;
            END IF;
    END;

    PROCEDURE safe_create_compartment(
        p_number IN NUMBER,
        p_short_name IN VARCHAR2,
        p_long_name IN VARCHAR2
    ) IS
    BEGIN
        SA_COMPONENTS.CREATE_COMPARTMENT('THONGBAO_OLS', p_number, p_short_name, p_long_name);
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE NOT IN (-12420, -1) THEN
                DBMS_OUTPUT.PUT_LINE('Creating compartment ' || p_short_name || ' failed: ' || SQLERRM);
                RAISE;
            END IF;
    END;

    PROCEDURE safe_create_group(
        p_number IN NUMBER,
        p_short_name IN VARCHAR2,
        p_long_name IN VARCHAR2
    ) IS
    BEGIN
        SA_COMPONENTS.CREATE_GROUP('THONGBAO_OLS', p_number, p_short_name, p_long_name);
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE NOT IN (-12421, -1) THEN
                DBMS_OUTPUT.PUT_LINE('Creating group ' || p_short_name || ' failed: ' || SQLERRM);
                RAISE;
            END IF;
    END;

    PROCEDURE safe_create_label(
        p_tag IN NUMBER,
        p_label IN VARCHAR2
    ) IS
    BEGIN
        SA_LABEL_ADMIN.CREATE_LABEL('THONGBAO_OLS', p_tag, p_label);
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE NOT IN (-1, -12453) THEN
                DBMS_OUTPUT.PUT_LINE('Creating label ' || p_label || ' failed: ' || SQLERRM);
                RAISE;
            END IF;
    END;
BEGIN
    safe_create_level(30, 'L3_GD', 'Ban Giam Doc');
    safe_create_level(20, 'L2_LD', 'Lanh Dao Khoa');
    safe_create_level(10, 'L1_NV', 'Nhan Vien');

    safe_create_compartment(100, 'C_TIEU', 'Tieu Hoa');
    safe_create_compartment(110, 'C_THAN', 'Than Kinh');
    safe_create_compartment(120, 'C_TIM', 'Tim Mach');

    safe_create_group(10, 'G_HN', 'Ha Noi');
    safe_create_group(20, 'G_HP', 'Hai Phong');
    safe_create_group(30, 'G_HCM', 'Ho Chi Minh');

    safe_create_label(1000, 'L1_NV');
    safe_create_label(2000, 'L2_LD');
    safe_create_label(3000, 'L3_GD');
    safe_create_label(2100, 'L2_LD:C_TIEU');
    safe_create_label(1130, 'L1_NV:C_TIEU:G_HCM');
    safe_create_label(1110, 'L1_NV:C_TIEU:G_HN');
    safe_create_label(2220, 'L2_LD:C_TIEU,C_THAN:G_HP');

    IF table_policy_applied = 0 THEN
        SA_POLICY_ADMIN.APPLY_TABLE_POLICY(
            policy_name   => 'THONGBAO_OLS',
            schema_name   => USER,
            table_name    => 'THONGBAO',
            table_options => 'READ_CONTROL'
        );
    END IF;
END;
/

BEGIN
    UPDATE THONGBAO
    SET OLS_LABEL = CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV')
    WHERE OLS_LABEL IS NULL;

    DELETE FROM THONGBAO
    WHERE NOIDUNG LIKE 't1:%'
       OR NOIDUNG LIKE 't2:%'
       OR NOIDUNG LIKE 't3:%'
       OR NOIDUNG LIKE 't4:%'
       OR NOIDUNG LIKE 't5:%'
       OR NOIDUNG LIKE 't6:%'
       OR NOIDUNG LIKE 't7:%';

    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t1: Gui den toan bo nhan vien', SYSTIMESTAMP, 'Online', CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV'));

    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t2: Gui den toan bo Ban giam doc', SYSTIMESTAMP, 'Phong hop Giam doc', CHAR_TO_LABEL('THONGBAO_OLS', 'L3_GD'));

    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t3: Gui den cac lanh dao khoa', SYSTIMESTAMP, 'Hoi truong', CHAR_TO_LABEL('THONGBAO_OLS', 'L2_LD'));

    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t4: Gui den lanh dao Khoa tieu hoa', SYSTIMESTAMP, 'Khoa tieu hoa', CHAR_TO_LABEL('THONGBAO_OLS', 'L2_LD:C_TIEU'));

    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t5: Gui den nhan vien Khoa tieu hoa o Ho Chi Minh', SYSTIMESTAMP, 'Ho Chi Minh', CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV:C_TIEU:G_HCM'));

    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t6: Gui den nhan vien Khoa tieu hoa o Ha Noi', SYSTIMESTAMP, 'Ha Noi', CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV:C_TIEU:G_HN'));

    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t7: Gui den lanh dao Khoa tieu hoa va Khoa than kinh tai Hai Phong', SYSTIMESTAMP, 'Hai Phong', CHAR_TO_LABEL('THONGBAO_OLS', 'L2_LD:C_TIEU,C_THAN:G_HP'));

    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'HOSPITAL_ADMIN', 'L3_GD:C_TIEU,C_THAN,C_TIM:G_HN,G_HP,G_HCM');
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000001', 'L3_GD:C_TIEU,C_THAN,C_TIM:G_HN,G_HP,G_HCM');
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000090', 'L2_LD:C_TIM:G_HCM');
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000060', 'L2_LD:C_THAN:G_HN');
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000061', 'L1_NV:C_THAN:G_HCM');
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000091', 'L1_NV:C_TIM:G_HCM');
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000002', 'L2_LD:C_TIM:G_HCM');
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000003', 'L2_LD:C_TIEU,C_THAN,C_TIM:G_HN,G_HP,G_HCM');
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000030', 'L1_NV:C_TIEU:G_HN');
END;
/

GRANT SELECT ON THONGBAO TO DIEU_PHOI_VIEN;
GRANT SELECT ON THONGBAO TO BAC_SI_Y_SI;
GRANT SELECT ON THONGBAO TO KY_THUAT_VIEN;
GRANT SELECT ON THONGBAO TO BENH_NHAN;

COMMIT;

PROMPT === OLS setup completed ===
WHENEVER SQLERROR CONTINUE;
