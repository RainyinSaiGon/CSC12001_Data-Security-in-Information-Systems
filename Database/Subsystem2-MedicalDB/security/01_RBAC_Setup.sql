SET SERVEROUTPUT ON;

PROMPT === Requirement 1 / RBAC setup ===

-- Xóa các view và trigger cũ trước khi tạo lại để tránh lỗi khi chạy lặp lại
BEGIN EXECUTE IMMEDIATE 'DROP VIEW V_SELF_NHANVIEN'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP VIEW V_SELF_BENHNHAN'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP VIEW V_TECHNICIAN_HSBA_DV'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP VIEW V_PATIENT_HSBA'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP VIEW V_PATIENT_DONTHUOC'; EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN EXECUTE IMMEDIATE 'DROP TRIGGER TRG_HSBADV_TECH_ONLY'; EXCEPTION WHEN OTHERS THEN NULL; END;
/

-- Procedure ensure_role: Tạo Oracle Role nếu chưa tồn tại.
-- Kiểm tra xung đột tên với user hiện có để tránh lỗi khi role trùng tên user.
-- Nếu role đã tồn tại thì tái sử dụng, không tạo lại.
DECLARE
    PROCEDURE ensure_role(p_role_name IN VARCHAR2) IS
        v_role_count NUMBER := 0;
        v_user_count NUMBER := 0;
    BEGIN
        BEGIN
            SELECT COUNT(*)
            INTO v_role_count
            FROM DBA_ROLES
            WHERE ROLE = p_role_name;
        EXCEPTION
            WHEN OTHERS THEN
                v_role_count := 0;
        END;

        BEGIN
            SELECT COUNT(*)
            INTO v_user_count
            FROM DBA_USERS
            WHERE USERNAME = p_role_name;
        EXCEPTION
            WHEN OTHERS THEN
                v_user_count := 0;
        END;

        IF v_user_count > 0 THEN
            RAISE_APPLICATION_ERROR(-20021, 'Name conflict: user ' || p_role_name || ' already exists.');
        ELSIF v_role_count = 0 THEN
            EXECUTE IMMEDIATE 'CREATE ROLE ' || p_role_name;
            DBMS_OUTPUT.PUT_LINE('Created role ' || p_role_name || '.');
        ELSE
            DBMS_OUTPUT.PUT_LINE('Role ' || p_role_name || ' already exists. Reusing it.');
        END IF;
    END;
BEGIN
    -- Tạo 4 role tương ứng với 4 nhóm người dùng trong hệ thống
    ensure_role('DIEU_PHOI_VIEN'); -- Điều phối viên: quản lý bệnh nhân và phân công
    ensure_role('BAC_SI_Y_SI');    -- Bác sĩ / Y sĩ: xem và cập nhật hồ sơ bệnh án
    ensure_role('KY_THUAT_VIEN'); -- Kỹ thuật viên: cập nhật kết quả dịch vụ kỹ thuật
    ensure_role('BENH_NHAN');     -- Bệnh nhân: chỉ xem thông tin của bản thân
END;
/

-- View V_SELF_NHANVIEN: Cho phép nhân viên xem và cập nhật đúng dòng thông tin của chính mình
-- trong bảng NHANVIEN, dựa theo USERNAME của phiên Oracle đang đăng nhập.
-- Dùng cho role: DIEU_PHOI_VIEN, BAC_SI_Y_SI, KY_THUAT_VIEN (cập nhật thông tin cá nhân)
CREATE OR REPLACE VIEW V_SELF_NHANVIEN AS
SELECT MANV, HOTEN, PHAI, NGAYSINH, CCCD, QUEQUAN, SODT, VAITRO, CHUYENKHOA, USERNAME, PASSWORD_HASH
FROM NHANVIEN
WHERE UPPER(USERNAME) = UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER'));

-- View V_SELF_BENHNHAN: Cho phép bệnh nhân xem và cập nhật đúng dòng thông tin của chính mình
-- trong bảng BENHNHAN, dựa theo USERNAME của phiên Oracle đang đăng nhập.
-- Dùng cho role: BENH_NHAN
CREATE OR REPLACE VIEW V_SELF_BENHNHAN AS
SELECT MABN, TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN,
    TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC, USERNAME, PASSWORD_HASH
FROM BENHNHAN
WHERE UPPER(USERNAME) = UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER'));

-- View V_TECHNICIAN_HSBA_DV: Lọc bảng HSBA_DV chỉ trả về các dòng mà kỹ thuật viên
-- hiện tại được phân công (MAKTV = MANV của user đang đăng nhập).
-- Dùng cho role: KY_THUAT_VIEN (xem và cập nhật kết quả dịch vụ kỹ thuật của mình)
CREATE OR REPLACE VIEW V_TECHNICIAN_HSBA_DV AS
SELECT d.MAHSBA, d.LOAIDV, d.NGAYDV, d.KETQUA, d.MAKTV
FROM HSBA_DV d
WHERE d.MAKTV = (
    SELECT n.MANV
    FROM NHANVIEN n
    WHERE UPPER(n.USERNAME) = UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER'))
);

-- View V_PATIENT_HSBA: Lọc bảng HSBA chỉ trả về các hồ sơ bệnh án thuộc về
-- bệnh nhân hiện tại đang đăng nhập (MABN khớp với bệnh nhân của phiên hiện tại).
-- Dùng cho role: BENH_NHAN (xem lịch sử khám bệnh của bản thân)
CREATE OR REPLACE VIEW V_PATIENT_HSBA AS
SELECT h.MAHSBA, h.MABN, h.NGAY, h.CHANDOAN, h.DIEUTRI, h.MABS, h.MAKHOA, h.KETLUAN
FROM HSBA h
WHERE h.MABN = (
    SELECT b.MABN
    FROM BENHNHAN b
    WHERE UPPER(b.USERNAME) = UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER'))
);

-- View V_PATIENT_DONTHUOC: Lọc bảng DONTHUOC chỉ trả về đơn thuốc thuộc về
-- các hồ sơ bệnh án của bệnh nhân hiện tại đang đăng nhập.
-- Dùng cho role: BENH_NHAN (xem đơn thuốc của bản thân)
CREATE OR REPLACE VIEW V_PATIENT_DONTHUOC AS
SELECT d.MAHSBA, d.NGAYDT, d.TENTHUOC, d.LIEUDUNG
FROM DONTHUOC d
JOIN HSBA h ON h.MAHSBA = d.MAHSBA
WHERE h.MABN = (
    SELECT b.MABN
    FROM BENHNHAN b
    WHERE UPPER(b.USERNAME) = UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER'))
);

-- Trigger TRG_HSBADV_TECH_ONLY: Ngăn kỹ thuật viên cập nhật cột KETQUA
-- trên các dòng HSBA_DV không được phân công cho họ (MAKTV khác MANV của họ).
-- Đây là lớp bảo vệ thứ hai sau view, đảm bảo tính toàn vẹn khi UPDATE trực tiếp vào bảng.
CREATE OR REPLACE TRIGGER TRG_HSBADV_TECH_ONLY
BEFORE UPDATE OF KETQUA ON HSBA_DV
FOR EACH ROW
DECLARE
    v_manv NHANVIEN.MANV%TYPE;
BEGIN
    SELECT MANV
    INTO v_manv
    FROM NHANVIEN
    WHERE UPPER(USERNAME) = UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER'));

    IF :OLD.MAKTV != v_manv THEN
        RAISE_APPLICATION_ERROR(-20010, 'Technician can update only assigned HSBA_DV rows.');
    END IF;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20011, 'Current Oracle user is not mapped to a technician row.');
END;
/

-- ============================================================
-- PHÂN QUYỀN TRÊN BẢNG THỰC (raw tables) CHO TỪNG ROLE
-- ============================================================

-- DIEU_PHOI_VIEN: Quyền quản lý toàn diện bệnh nhân, hồ sơ, dịch vụ kỹ thuật.
-- VPD sẽ trả về 1=1 cho role này → thấy tất cả dữ liệu.
GRANT SELECT, INSERT, UPDATE ON BENHNHAN TO DIEU_PHOI_VIEN;
GRANT SELECT, INSERT ON HSBA TO DIEU_PHOI_VIEN;
GRANT UPDATE (MABS, MAKHOA) ON HSBA TO DIEU_PHOI_VIEN;         -- Chỉ được đổi bác sĩ và khoa phụ trách
GRANT SELECT ON NHANVIEN TO DIEU_PHOI_VIEN;
GRANT SELECT, INSERT ON HSBA_DV TO DIEU_PHOI_VIEN;
GRANT UPDATE (MAKTV) ON HSBA_DV TO DIEU_PHOI_VIEN;             -- Chỉ được phân công lại kỹ thuật viên
GRANT EXECUTE ON SP_ADD_PATIENT TO DIEU_PHOI_VIEN;             -- Thủ tục thêm bệnh nhân mới

-- BAC_SI_Y_SI: Quyền xem và cập nhật hồ sơ bệnh án thuộc phạm vi điều trị của mình.
-- VPD sẽ lọc chỉ trả về dữ liệu của bệnh nhân mà bác sĩ này phụ trách.
GRANT SELECT ON HSBA TO BAC_SI_Y_SI;
GRANT SELECT ON BENHNHAN TO BAC_SI_Y_SI;
GRANT UPDATE (CHANDOAN, DIEUTRI, KETLUAN) ON HSBA TO BAC_SI_Y_SI;              -- Chỉ cập nhật nội dung y tế
GRANT UPDATE (TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC) ON BENHNHAN TO BAC_SI_Y_SI; -- Cập nhật tiền sử bệnh
GRANT SELECT ON HSBA_DV TO BAC_SI_Y_SI;
GRANT INSERT, DELETE ON HSBA_DV TO BAC_SI_Y_SI;                -- Tạo / xóa yêu cầu dịch vụ kỹ thuật
GRANT SELECT ON DONTHUOC TO BAC_SI_Y_SI;
GRANT INSERT, UPDATE, DELETE ON DONTHUOC TO BAC_SI_Y_SI;       -- Quản lý đơn thuốc

-- KY_THUAT_VIEN: Chỉ được thao tác qua VIEW, không có quyền trên bảng thực.
-- View đã lọc sẵn các dòng được phân công, trigger bảo vệ tầng dưới.
GRANT SELECT ON V_TECHNICIAN_HSBA_DV TO KY_THUAT_VIEN;         -- Xem dịch vụ kỹ thuật được phân công
GRANT UPDATE (KETQUA) ON V_TECHNICIAN_HSBA_DV TO KY_THUAT_VIEN; -- Cập nhật kết quả dịch vụ
GRANT SELECT ON V_SELF_NHANVIEN TO KY_THUAT_VIEN;              -- Xem thông tin cá nhân
GRANT UPDATE (QUEQUAN, SODT, PASSWORD_HASH) ON V_SELF_NHANVIEN TO KY_THUAT_VIEN; -- Cập nhật thông tin cá nhân

-- BENH_NHAN: Chỉ được thao tác qua VIEW, không có quyền trên bảng thực.
-- Mỗi view đã giới hạn dữ liệu của chính bệnh nhân đó.
GRANT SELECT ON V_SELF_BENHNHAN TO BENH_NHAN;                   -- Xem thông tin cá nhân
GRANT UPDATE (SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC, PASSWORD_HASH) ON V_SELF_BENHNHAN TO BENH_NHAN; -- Cập nhật thông tin cá nhân
GRANT SELECT ON V_PATIENT_HSBA TO BENH_NHAN;                   -- Xem hồ sơ bệnh án của bản thân
GRANT SELECT ON V_PATIENT_DONTHUOC TO BENH_NHAN;               -- Xem đơn thuốc của bản thân

-- Cho phép tất cả nhân viên (không phân biệt role) xem và cập nhật thông tin cá nhân của mình
GRANT SELECT ON V_SELF_NHANVIEN TO DIEU_PHOI_VIEN;
GRANT UPDATE (QUEQUAN, SODT, PASSWORD_HASH) ON V_SELF_NHANVIEN TO DIEU_PHOI_VIEN;
GRANT SELECT ON V_SELF_NHANVIEN TO BAC_SI_Y_SI;
GRANT UPDATE (QUEQUAN, SODT, PASSWORD_HASH) ON V_SELF_NHANVIEN TO BAC_SI_Y_SI;

-- ============================================================
-- TẠO ORACLE USER VÀ CẤP ROLE CHO TOÀN BỘ NHÂN VIÊN & BỆNH NHÂN
-- ============================================================
DECLARE
    -- staff_cursor: Duyệt tất cả nhân viên có USERNAME, ánh xạ VAITRO sang tên Oracle Role tương ứng
    CURSOR staff_cursor IS
        SELECT USERNAME,
               CCCD AS USER_PASSWORD,
               CASE
                   WHEN VAITRO = N'Điều phối viên' THEN 'DIEU_PHOI_VIEN'
                   WHEN VAITRO = N'Bác sĩ/Y sĩ' THEN 'BAC_SI_Y_SI'
                   WHEN VAITRO = N'Kỹ thuật viên' THEN 'KY_THUAT_VIEN'
               END AS ROLE_NAME
        FROM NHANVIEN
        WHERE USERNAME IS NOT NULL;

    -- patient_cursor: Duyệt tất cả bệnh nhân có USERNAME, tất cả đều nhận role BENH_NHAN
    CURSOR patient_cursor IS
        SELECT USERNAME,
               CCCD AS USER_PASSWORD
        FROM BENHNHAN
        WHERE USERNAME IS NOT NULL;

    -- q_ident: Bọc tên định danh Oracle trong dấu ngoặc kép để hỗ trợ username bắt đầu bằng số (CCCD)
    FUNCTION q_ident(p_value IN VARCHAR2) RETURN VARCHAR2 IS
    BEGIN
        RETURN '"' || REPLACE(p_value, '"', '""') || '"';
    END;

    -- q_literal: Bọc giá trị chuỗi trong dấu nháy đơn, escape ký tự đặc biệt cho SQL động
    FUNCTION q_literal(p_value IN VARCHAR2) RETURN VARCHAR2 IS
    BEGIN
        RETURN '''' || REPLACE(p_value, '''', '''''') || '''';
    END;
BEGIN
    -- Tạo Oracle user cho từng nhân viên, mật khẩu mặc định = CCCD
    -- Nếu user đã tồn tại thì mở khóa và đặt lại mật khẩu thay vì báo lỗi
    FOR r IN staff_cursor LOOP
        BEGIN
            EXECUTE IMMEDIATE 'CREATE USER ' || q_ident(r.USERNAME) || ' IDENTIFIED BY ' || q_ident(r.USER_PASSWORD);
        EXCEPTION
            WHEN OTHERS THEN
                IF SQLCODE = -1920 THEN
                    EXECUTE IMMEDIATE 'ALTER USER ' || q_ident(r.USERNAME) || ' IDENTIFIED BY ' || q_ident(r.USER_PASSWORD) || ' ACCOUNT UNLOCK';
                ELSE
                    RAISE;
                END IF;
        END;

        EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || q_ident(r.USERNAME); -- Cho phép đăng nhập
        IF r.ROLE_NAME IS NOT NULL THEN
            EXECUTE IMMEDIATE 'GRANT ' || r.ROLE_NAME || ' TO ' || q_ident(r.USERNAME); -- Cấp role theo vaitro
        END IF;
    END LOOP;

    -- Tạo Oracle user cho từng bệnh nhân, mật khẩu mặc định = CCCD
    -- Tất cả bệnh nhân đều nhận role BENH_NHAN
    FOR r IN patient_cursor LOOP
        BEGIN
            EXECUTE IMMEDIATE 'CREATE USER ' || q_ident(r.USERNAME) || ' IDENTIFIED BY ' || q_ident(r.USER_PASSWORD);
        EXCEPTION
            WHEN OTHERS THEN
                IF SQLCODE = -1920 THEN
                    EXECUTE IMMEDIATE 'ALTER USER ' || q_ident(r.USERNAME) || ' IDENTIFIED BY ' || q_ident(r.USER_PASSWORD) || ' ACCOUNT UNLOCK';
                ELSE
                    RAISE;
                END IF;
        END;

        EXECUTE IMMEDIATE 'GRANT CREATE SESSION TO ' || q_ident(r.USERNAME); -- Cho phép đăng nhập
        EXECUTE IMMEDIATE 'GRANT BENH_NHAN TO ' || q_ident(r.USERNAME);      -- Cấp role bệnh nhân
    END LOOP;
END;
/

COMMIT;

PROMPT === RBAC setup completed ===
