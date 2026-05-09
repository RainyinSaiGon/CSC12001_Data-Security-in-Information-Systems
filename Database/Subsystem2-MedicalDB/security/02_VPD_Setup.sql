SET SERVEROUTPUT ON;

PROMPT === Requirement 1 / VPD setup ===

-- =============================================================================
-- DỌN DẸP: Xóa toàn bộ policy VPD cũ trước khi tạo lại
-- Mục đích: Tránh lỗi ORA-28101 "policy already exists" khi chạy lại script.
-- Dùng EXCEPTION WHEN OTHERS THEN NULL để bỏ qua lỗi nếu policy chưa tồn tại.
-- =============================================================================
BEGIN DBMS_RLS.DROP_POLICY(USER, 'HSBA', 'HSBA_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN DBMS_RLS.DROP_POLICY(USER, 'BENHNHAN', 'BENHNHAN_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN DBMS_RLS.DROP_POLICY(USER, 'HSBA_DV', 'HSBA_DV_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN DBMS_RLS.DROP_POLICY(USER, 'DONTHUOC', 'DONTHUOC_VPD'); EXCEPTION WHEN OTHERS THEN NULL; END;
/

-- =============================================================================
-- DỌN DẸP: Xóa toàn bộ function VPD cũ trước khi tạo lại
-- Mục đích: Đảm bảo script idempotent — chạy nhiều lần vẫn cho kết quả đúng.
-- =============================================================================
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

-- =============================================================================
-- HÀM TRỢ GIÚP: APP_CURRENT_MANV
-- Mục đích: Tra cứu MANV (mã nhân viên) của user Oracle đang đăng nhập từ bảng
--           NHANVIEN, dựa vào cột USERNAME so sánh với SESSION_USER.
-- Dùng bởi: Các hàm VPD policy để biết nhân viên nào đang truy cập, từ đó lọc
--           đúng dữ liệu (VD: bác sĩ chỉ thấy hồ sơ bệnh nhân của mình).
-- Trả về:   MANV dạng VARCHAR2 nếu tìm thấy, NULL nếu user không phải nhân viên.
-- =============================================================================
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

-- =============================================================================
-- HÀM TRỢ GIÚP: APP_CURRENT_ROLE
-- Mục đích: Tra cứu VAITRO (vai trò nghiệp vụ) của user Oracle đang đăng nhập
--           từ bảng NHANVIEN.
-- Dùng bởi: Các hàm VPD policy để phân nhánh logic lọc theo vai trò:
--           'Điều phối viên' → thấy tất cả, 'Bác sĩ/Y sĩ' → thấy bệnh nhân
--           mình phụ trách, các role khác → không thấy gì (1=0).
-- Trả về:   Chuỗi VAITRO (VD: N'Bác sĩ/Y sĩ'), NULL nếu không phải nhân viên.
-- =============================================================================
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

-- =============================================================================
-- HÀM VPD POLICY: VPD_HSBA_FN — Lọc bảng HSBA (Hồ Sơ Bệnh Án)
-- Mục đích: Oracle gọi hàm này tự động mỗi khi có câu SELECT/UPDATE/DELETE/INSERT
--           trên bảng HSBA. Hàm trả về mệnh đề WHERE bổ sung, được Oracle nối vào
--           câu query gốc để giới hạn dữ liệu mà user thấy/tác động được.
--
-- Logic lọc theo vai trò:
--   - Schema owner (SYS/HOSPITAL_ADMIN):  1=1  → thấy toàn bộ (không bị lọc)
--   - Điều phối viên:                     1=1  → thấy toàn bộ hồ sơ
--   - Bác sĩ/Y sĩ:          MABS = '<MANV>'   → chỉ thấy hồ sơ mình phụ trách
--   - Tất cả vai trò khác:               1=0  → không thấy dòng nào
--
-- Tham số p_schema, p_object: Oracle truyền tự động (tên schema và tên bảng).
-- =============================================================================
CREATE OR REPLACE FUNCTION VPD_HSBA_FN(p_schema VARCHAR2, p_object VARCHAR2)
RETURN VARCHAR2
AS
    v_role NVARCHAR2(50);
    v_manv NHANVIEN.MANV%TYPE;
BEGIN
    -- Schema owner luôn thấy tất cả (cần để admin và VPD engine hoạt động đúng)
    IF UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER')) = UPPER(USER) THEN
        RETURN '1=1';
    END IF;

    v_role := APP_CURRENT_ROLE();
    v_manv := APP_CURRENT_MANV();

    IF v_role = N'Điều phối viên' THEN
        -- Điều phối viên quản lý toàn hệ thống, cần thấy tất cả hồ sơ
        RETURN '1=1';
    ELSIF v_role = N'Bác sĩ/Y sĩ' THEN
        IF v_manv IS NULL THEN
            RETURN '1=0';
        END IF;
        -- Bác sĩ chỉ thấy hồ sơ của bệnh nhân mình phụ trách (MABS = MANV của bác sĩ đó)
        RETURN 'MABS = ''' || REPLACE(v_manv, '''', '''''') || '''';
    END IF;

    -- Kỹ thuật viên, bệnh nhân, và các role khác không truy cập trực tiếp bảng HSBA
    RETURN '1=0';
END;
/


-- =============================================================================
-- HÀM VPD POLICY: VPD_BENHNHAN_FN — Lọc bảng BENHNHAN (Thông tin bệnh nhân)
-- Mục đích: Kiểm soát hàng nào trong bảng BENHNHAN mà mỗi user được phép thấy,
--           đảm bảo bí mật thông tin cá nhân của bệnh nhân.
--
-- Logic lọc theo vai trò:
--   - Schema owner:          1=1  → thấy toàn bộ
--   - Điều phối viên:        1=1  → thấy toàn bộ danh sách bệnh nhân
--   - Bác sĩ/Y sĩ:          MABN IN (SELECT MABN FROM HSBA WHERE MABS=...)
--                                  → chỉ thấy bệnh nhân mình đang điều trị
--   - Bệnh nhân (và khác):   UPPER(USERNAME) = UPPER(SESSION_USER)
--                                  → chỉ thấy dòng của chính mình
--
-- Lưu ý: Bệnh nhân truy cập qua view V_SELF_BENHNHAN (RBAC) và VPD là lớp backup.
-- =============================================================================
CREATE OR REPLACE FUNCTION VPD_BENHNHAN_FN(p_schema VARCHAR2, p_object VARCHAR2)
RETURN VARCHAR2
AS
    v_role NVARCHAR2(50);
    v_manv NHANVIEN.MANV%TYPE;
BEGIN
    -- Schema owner luôn thấy tất cả
    IF UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER')) = UPPER(USER) THEN
        RETURN '1=1';
    END IF;

    v_role := APP_CURRENT_ROLE();
    v_manv := APP_CURRENT_MANV();

    IF v_role = N'Điều phối viên' THEN
        -- Điều phối viên cần thấy toàn bộ để phân công và quản lý
        RETURN '1=1';
    ELSIF v_role = N'Bác sĩ/Y sĩ' THEN
        IF v_manv IS NULL THEN
            RETURN '1=0';
        END IF;
        -- Bác sĩ chỉ thấy bệnh nhân có hồ sơ do mình phụ trách (subquery qua bảng HSBA)
        RETURN 'MABN IN (SELECT MABN FROM HSBA WHERE MABS = ''' || REPLACE(v_manv, '''', '''''') || ''')';
    END IF;

    -- Bệnh nhân và vai trò không xác định: chỉ thấy dòng của chính mình
    RETURN 'UPPER(USERNAME) = UPPER(SYS_CONTEXT(''USERENV'', ''SESSION_USER''))';
END;
/

-- =============================================================================
-- HÀM VPD POLICY: VPD_HSBA_DV_FN — Lọc bảng HSBA_DV (Dịch vụ trong hồ sơ)
-- Mục đích: Kiểm soát kỹ thuật viên và bác sĩ chỉ thấy các dịch vụ/xét nghiệm
--           thuộc hồ sơ bệnh án mình được phân công, tránh lộ thông tin chéo.
--
-- Logic lọc theo vai trò:
--   - Schema owner:          1=1  → thấy toàn bộ
--   - Điều phối viên:        1=1  → thấy toàn bộ để quản lý phân công
--   - Bác sĩ/Y sĩ:          MAHSBA IN (SELECT MAHSBA FROM HSBA WHERE MABS=...)
--                                  → chỉ thấy dịch vụ thuộc hồ sơ mình phụ trách
--   - Tất cả vai trò khác:   1=0  → không có quyền xem (kỹ thuật viên dùng view riêng)
--
-- Lưu ý: Kỹ thuật viên truy cập qua view V_TECHNICIAN_HSBA_DV (RBAC),
--        view đó đã lọc theo MANV_KTV, VPD chặn truy cập thẳng bảng gốc.
-- =============================================================================
CREATE OR REPLACE FUNCTION VPD_HSBA_DV_FN(p_schema VARCHAR2, p_object VARCHAR2)
RETURN VARCHAR2
AS
    v_role NVARCHAR2(50);
    v_manv NHANVIEN.MANV%TYPE;
BEGIN
    -- Schema owner luôn thấy tất cả
    IF UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER')) = UPPER(USER) THEN
        RETURN '1=1';
    END IF;

    v_role := APP_CURRENT_ROLE();
    v_manv := APP_CURRENT_MANV();

    IF v_role = N'Điều phối viên' THEN
        -- Điều phối viên cần thấy tất cả dịch vụ để điều phối kỹ thuật viên
        RETURN '1=1';
    ELSIF v_role = N'Bác sĩ/Y sĩ' THEN
        IF v_manv IS NULL THEN
            RETURN '1=0';
        END IF;
        -- Bác sĩ chỉ thấy dịch vụ trong hồ sơ của bệnh nhân mình phụ trách
        RETURN 'MAHSBA IN (SELECT MAHSBA FROM HSBA WHERE MABS = ''' || REPLACE(v_manv, '''', '''''') || ''')';
    END IF;

    -- Kỹ thuật viên, bệnh nhân, và các role khác bị chặn hoàn toàn
    RETURN '1=0';
END;
/

-- =============================================================================
-- HÀM VPD POLICY: VPD_DONTHUOC_FN — Lọc bảng DONTHUOC (Đơn thuốc)
-- Mục đích: Đảm bảo bác sĩ chỉ thấy đơn thuốc thuộc hồ sơ bệnh nhân mình kê,
--           tránh bác sĩ này đọc hoặc sửa đơn thuốc của bác sĩ khác.
--
-- Logic lọc theo vai trò (giống VPD_HSBA_DV_FN vì quan hệ qua MAHSBA):
--   - Schema owner:          1=1  → thấy toàn bộ
--   - Điều phối viên:        1=1  → thấy toàn bộ để giám sát
--   - Bác sĩ/Y sĩ:          MAHSBA IN (SELECT MAHSBA FROM HSBA WHERE MABS=...)
--                                  → chỉ thấy đơn thuốc trong hồ sơ mình phụ trách
--   - Tất cả vai trò khác:   1=0  → không có quyền xem đơn thuốc
-- =============================================================================
CREATE OR REPLACE FUNCTION VPD_DONTHUOC_FN(p_schema VARCHAR2, p_object VARCHAR2)
RETURN VARCHAR2
AS
    v_role NVARCHAR2(50);
    v_manv NHANVIEN.MANV%TYPE;
BEGIN
    -- Schema owner luôn thấy tất cả
    IF UPPER(SYS_CONTEXT('USERENV', 'SESSION_USER')) = UPPER(USER) THEN
        RETURN '1=1';
    END IF;

    v_role := APP_CURRENT_ROLE();
    v_manv := APP_CURRENT_MANV();

    IF v_role = N'Điều phối viên' THEN
        -- Điều phối viên có thể cần xem đơn thuốc để kiểm tra và điều phối
        RETURN '1=1';
    ELSIF v_role = N'Bác sĩ/Y sĩ' THEN
        IF v_manv IS NULL THEN
            RETURN '1=0';
        END IF;
        -- Bác sĩ chỉ thấy đơn thuốc thuộc hồ sơ do mình kê (liên kết qua bảng HSBA)
        RETURN 'MAHSBA IN (SELECT MAHSBA FROM HSBA WHERE MABS = ''' || REPLACE(v_manv, '''', '''''') || ''')';
    END IF;

    -- Kỹ thuật viên, bệnh nhân không được phép xem đơn thuốc của người khác
    RETURN '1=0';
END;
/

-- =============================================================================
-- ĐĂNG KÝ VPD POLICY: Gắn các hàm lọc vào bảng qua Oracle RLS (Row Level Security)
-- Mục đích: Sau khi tạo hàm, phải đăng ký với DBMS_RLS.ADD_POLICY để Oracle tự
--           động gọi hàm mỗi khi có câu lệnh DML trên bảng tương ứng.
--
-- Tham số quan trọng:
--   object_name:      Bảng áp dụng policy
--   policy_name:      Tên định danh duy nhất của policy trong Oracle
--   policy_function:  Hàm trả về mệnh đề WHERE lọc dữ liệu
--   statement_types:  Loại câu lệnh bị kiểm soát (SELECT, UPDATE, DELETE, INSERT)
--   update_check:     TRUE → áp dụng lọc cả khi UPDATE/INSERT để chặn ghi chéo dữ liệu
-- =============================================================================
BEGIN
    -- Policy cho bảng HSBA: lọc hồ sơ bệnh án theo vai trò nhân viên
    DBMS_RLS.ADD_POLICY(
        object_schema   => USER,
        object_name     => 'HSBA',
        policy_name     => 'HSBA_VPD',
        function_schema => USER,
        policy_function => 'VPD_HSBA_FN',
        statement_types => 'SELECT,UPDATE,DELETE,INSERT',
        update_check    => TRUE
    );

    -- Policy cho bảng BENHNHAN: lọc thông tin bệnh nhân theo vai trò
    DBMS_RLS.ADD_POLICY(
        object_schema   => USER,
        object_name     => 'BENHNHAN',
        policy_name     => 'BENHNHAN_VPD',
        function_schema => USER,
        policy_function => 'VPD_BENHNHAN_FN',
        statement_types => 'SELECT,UPDATE,DELETE,INSERT',
        update_check    => TRUE
    );

    -- Policy cho bảng HSBA_DV: lọc dịch vụ xét nghiệm theo vai trò
    DBMS_RLS.ADD_POLICY(
        object_schema   => USER,
        object_name     => 'HSBA_DV',
        policy_name     => 'HSBA_DV_VPD',
        function_schema => USER,
        policy_function => 'VPD_HSBA_DV_FN',
        statement_types => 'SELECT,UPDATE,DELETE,INSERT',
        update_check    => TRUE
    );

    -- Policy cho bảng DONTHUOC: lọc đơn thuốc theo vai trò
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

-- =============================================================================
-- CẤP QUYỀN EXECUTE: Cho phép tất cả user Oracle gọi các hàm VPD
-- Mục đích: Oracle gọi hàm VPD với quyền của người dùng truy vấn (không phải
--           schema owner), nên cần GRANT EXECUTE TO PUBLIC để policy hoạt động
--           đúng khi mọi user thực hiện SELECT trên các bảng được bảo vệ.
-- =============================================================================
GRANT EXECUTE ON APP_CURRENT_MANV TO PUBLIC;
GRANT EXECUTE ON APP_CURRENT_ROLE TO PUBLIC;
GRANT EXECUTE ON VPD_HSBA_FN TO PUBLIC;
GRANT EXECUTE ON VPD_BENHNHAN_FN TO PUBLIC;
GRANT EXECUTE ON VPD_HSBA_DV_FN TO PUBLIC;
GRANT EXECUTE ON VPD_DONTHUOC_FN TO PUBLIC;

COMMIT;

PROMPT === VPD setup completed ===
