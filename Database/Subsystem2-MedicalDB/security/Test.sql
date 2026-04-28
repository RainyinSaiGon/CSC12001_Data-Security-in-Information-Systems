SET SERVEROUTPUT ON;

DECLARE
    TYPE t_varchar_list IS TABLE OF VARCHAR2(100);

    v_last_names   t_varchar_list := t_varchar_list('Nguyen', 'Tran', 'Le', 'Pham', 'Ho', 'Vo');
    v_middle_names t_varchar_list := t_varchar_list('Minh', 'Thanh', 'Gia', 'Quoc', 'Bao', 'Anh');
    v_male_names   t_varchar_list := t_varchar_list('An', 'Binh', 'Khanh', 'Long', 'Nam', 'Phuc');
    v_female_names t_varchar_list := t_varchar_list('Anh', 'Ha', 'Lan', 'Linh', 'My', 'Trang');
    v_streets      t_varchar_list := t_varchar_list('Le Loi', 'Nguyen Hue', 'Tran Hung Dao', 'Vo Van Tan', 'Hai Ba Trung');
    v_districts    t_varchar_list := t_varchar_list('District 1', 'District 3', 'District 5', 'Binh Thanh', 'Phu Nhuan');

    v_phai      NVARCHAR2(3);
    v_tenbn     NVARCHAR2(100);
    v_cccd      CHAR(12);
    v_username  VARCHAR2(50);
    v_sonha     NVARCHAR2(30);
    v_tenduong  NVARCHAR2(30);
    v_quanhuyen NVARCHAR2(30);
    v_ngaysinh  DATE;
    v_mabn      BENHNHAN.MABN%TYPE;
    v_exists    NUMBER;

    FUNCTION pick_item(p_list t_varchar_list) RETURN VARCHAR2 IS
    BEGIN
        RETURN p_list(TRUNC(DBMS_RANDOM.VALUE(1, p_list.COUNT + 1)));
    END;
BEGIN
    DBMS_RANDOM.SEED(TO_CHAR(SYSTIMESTAMP, 'YYYYMMDDHH24MISSFF'));

    IF DBMS_RANDOM.VALUE(0, 1) < 0.5 THEN
        v_phai := N'Nam';
        v_tenbn := TO_NCHAR(pick_item(v_last_names) || ' ' || pick_item(v_middle_names) || ' ' || pick_item(v_male_names));
    ELSE
        v_phai := UNISTR('N\1EEF');
        v_tenbn := TO_NCHAR(pick_item(v_last_names) || ' ' || pick_item(v_middle_names) || ' ' || pick_item(v_female_names));
    END IF;

    LOOP
        v_cccd := LPAD(TRUNC(DBMS_RANDOM.VALUE(10000000, 99999999)), 8, '0')
                  || LPAD(TRUNC(DBMS_RANDOM.VALUE(1000, 9999)), 4, '0');

        SELECT COUNT(*)
        INTO v_exists
        FROM BENHNHAN
        WHERE CCCD = v_cccd;

        EXIT WHEN v_exists = 0;
    END LOOP;

    v_username := v_cccd;
    v_sonha := TO_NCHAR(TO_CHAR(TRUNC(DBMS_RANDOM.VALUE(1, 300))));
    v_tenduong := TO_NCHAR(pick_item(v_streets));
    v_quanhuyen := TO_NCHAR(pick_item(v_districts));
    v_ngaysinh := ADD_MONTHS(TRUNC(SYSDATE), -1 * TRUNC(DBMS_RANDOM.VALUE(18 * 12, 75 * 12)));

    INSERT INTO BENHNHAN (
        TENBN,
        PHAI,
        NGAYSINH,
        CCCD,
        SONHA,
        TENDUONG,
        QUANHUYEN,
        TINHTP,
        TIENSUBENH,
        TIENSUBENHGD,
        DIUNGTHUOC,
        USERNAME,
        PASSWORD_HASH
    )
    VALUES (
        v_tenbn,
        v_phai,
        v_ngaysinh,
        v_cccd,
        v_sonha,
        v_tenduong,
        v_quanhuyen,
        N'TP HCM',
        N'Khong',
        N'Khong',
        N'Khong',
        v_username,
        NULL
    )
    RETURNING MABN INTO v_mabn;

    COMMIT;

    DBMS_OUTPUT.PUT_LINE('Inserted BENHNHAN successfully');
    DBMS_OUTPUT.PUT_LINE('MABN     : ' || v_mabn);
    DBMS_OUTPUT.PUT_LINE('TENBN    : ' || v_tenbn);
    DBMS_OUTPUT.PUT_LINE('PHAI     : ' || v_phai);
    DBMS_OUTPUT.PUT_LINE('CCCD     : ' || v_cccd);
    DBMS_OUTPUT.PUT_LINE('USERNAME : ' || v_username);
END;
/

SELECT *
FROM BENHNHAN
WHERE USERNAME = CCCD
ORDER BY MABN DESC;


SHOW USER;
SELECT USER, SYS_CONTEXT('USERENV', 'SESSION_USER'), SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA')
FROM dual;
