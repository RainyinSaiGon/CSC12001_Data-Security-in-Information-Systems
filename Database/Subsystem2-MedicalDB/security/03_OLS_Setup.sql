SET SERVEROUTPUT ON;
WHENEVER SQLERROR EXIT SQL.SQLCODE;

PROMPT === Requirement 2 / OLS setup ===
PROMPT Pass 1 - create THONGBAO_OLS if it does not exist, then reconnect.
PROMPT Pass 2 - reconnect as the same user and rerun this script to finish labels/data setup.

-- =============================================================================
-- KHỐI 1: KIỂM TRA TIỀN ĐIỀU KIỆN — Tạo OLS Policy hoặc xác nhận đã tồn tại
-- Mục đích: Script này cần chạy 2 lần (2-pass):
--   Pass 1: Tạo policy THONGBAO_OLS lần đầu tiên, rồi báo lỗi yêu cầu reconnect.
--           Oracle cần một session mới để kích hoạt role THONGBAO_OLS_DBA.
--   Pass 2: Sau khi reconnect, kiểm tra role đã được kích hoạt chưa rồi tiếp tục.
-- =============================================================================
DECLARE
    v_role_enabled NUMBER := 0;

    -- -------------------------------------------------------------------------
    -- Hàm nội bộ: policy_exists
    -- Mục đích: Kiểm tra xem OLS policy có tên p_policy_name đã tồn tại chưa
    --           bằng cách truy vấn DBA_SA_POLICIES (nếu có quyền) hoặc fallback
    --           sang ALL_SA_POLICIES.
    -- Trả về:   1 nếu policy đã tồn tại, 0 nếu chưa.
    -- -------------------------------------------------------------------------
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

    -- -------------------------------------------------------------------------
    -- Thủ tục nội bộ: cleanup_policy
    -- Mục đích: Dọn sạch hoàn toàn một OLS policy cũ trước khi tạo lại.
    --           Thực hiện theo thứ tự: xóa khỏi bảng → disable → drop policy.
    --           Mỗi bước bọc trong EXCEPTION để bỏ qua nếu đã không tồn tại.
    -- Dùng khi: Script được chạy lại sau khi đã có policy cũ bị lỗi hoặc cần reset.
    -- -------------------------------------------------------------------------
    PROCEDURE cleanup_policy(p_policy_name IN VARCHAR2) IS
    BEGIN
        -- Bước 1: Gỡ policy khỏi bảng THONGBAO và xóa cột OLS_LABEL khỏi bảng
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

        -- Bước 2: Disable policy trên toàn bộ hệ thống OLS
        BEGIN
            SA_SYSDBA.DISABLE_POLICY(policy_name => p_policy_name);
        EXCEPTION
            WHEN OTHERS THEN NULL;
        END;

        -- Bước 3: Xóa hẳn policy khỏi Oracle OLS
        BEGIN
            SA_SYSDBA.DROP_POLICY(policy_name => p_policy_name, drop_column => TRUE);
        EXCEPTION
            WHEN OTHERS THEN NULL;
        END;
    END;

BEGIN
    IF policy_exists('THONGBAO_OLS') = 0 THEN
        -- Policy chưa tồn tại: dọn dẹp policy cũ có tên khác (phòng conflict)
        cleanup_policy('THONGBAO_OLS');
        cleanup_policy('HOS_OLS_POL');

        -- Xóa cột OLS_LABEL thừa nếu còn sót từ lần chạy trước
        BEGIN
            EXECUTE IMMEDIATE 'ALTER TABLE THONGBAO DROP COLUMN OLS_LABEL';
        EXCEPTION
            WHEN OTHERS THEN NULL;
        END;

        -- Tạo OLS policy mới với tên THONGBAO_OLS
        -- READ_CONTROL: chỉ áp dụng kiểm soát đọc (SELECT), không chặn ghi
        SA_SYSDBA.CREATE_POLICY(
            policy_name     => 'THONGBAO_OLS',
            column_name     => 'OLS_LABEL',
            default_options => 'READ_CONTROL'
        );

        -- Báo lỗi yêu cầu reconnect: Oracle chỉ kích hoạt role THONGBAO_OLS_DBA
        -- trong session MỚI sau khi policy được tạo.
        RAISE_APPLICATION_ERROR(
            -20032,
            'THONGBAO_OLS policy created. Disconnect and reconnect as ' || USER ||
            ', then rerun 03_OLS_Setup.sql so THONGBAO_OLS_DBA is enabled in the new session.'
        );
    END IF;

    -- Kiểm tra role THONGBAO_OLS_DBA đã được kích hoạt trong session hiện tại chưa
    -- Nếu chưa → yêu cầu reconnect (Pass 1 chưa hoàn tất)
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
        -- ORA-12458: Oracle Label Security chưa được cài đặt/kích hoạt trong database
        -- → Hướng dẫn DBA kích hoạt OLS trước khi chạy script này
        IF SQLCODE = -12458 THEN
            RAISE_APPLICATION_ERROR(
                -20034,
                'Oracle Label Security is not enabled in this database. Connect as SYSDBA to the project PDB, run LBACSYS.CONFIGURE_OLS and LBACSYS.OLS_ENFORCEMENT.ENABLE_OLS, restart the database, rerun Create_HOSPITAL_ADMIN.sql, reconnect as HOSPITAL_ADMIN, then rerun 03_OLS_Setup.sql.'
            );
        END IF;
        RAISE;
END;
/

-- =============================================================================
-- KHỐI 2: TẠO CÁC THÀNH PHẦN OLS VÀ ÁP DỤNG POLICY VÀO BẢNG THONGBAO
-- Mục đích: Định nghĩa toàn bộ cấu trúc nhãn bảo mật (label) theo 3 chiều:
--   - Level (mức độ bảo mật): phân cấp theo chức danh
--   - Compartment (ngăn chuyên môn): phân theo chuyên khoa
--   - Group (nhóm địa lý): phân theo chi nhánh/vùng địa lý
--   Sau đó gắn policy vào bảng THONGBAO để mỗi dòng có nhãn riêng.
-- =============================================================================
DECLARE
    -- -------------------------------------------------------------------------
    -- Hàm nội bộ: table_policy_applied
    -- Mục đích: Kiểm tra policy THONGBAO_OLS đã được gắn vào bảng THONGBAO chưa.
    -- Trả về:   1 nếu đã áp dụng, 0 nếu chưa.
    -- -------------------------------------------------------------------------
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

    -- -------------------------------------------------------------------------
    -- Thủ tục nội bộ: safe_create_level
    -- Mục đích: Tạo một Level (mức độ bảo mật) trong policy THONGBAO_OLS.
    --           Level có thứ bậc: số càng cao → quyền xem càng rộng (dominance).
    --           User có Level cao hơn được đọc dữ liệu có Level thấp hơn.
    -- Bỏ qua:  ORA-12419 (đã tồn tại) và ORA-1 (unique constraint) để idempotent.
    -- -------------------------------------------------------------------------
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

    -- -------------------------------------------------------------------------
    -- Thủ tục nội bộ: safe_create_compartment
    -- Mục đích: Tạo một Compartment (ngăn chuyên môn) trong policy THONGBAO_OLS.
    --           Compartment kiểm soát theo chiều ngang (không có thứ bậc): user
    --           phải có đúng compartment trong label mới đọc được dòng đó.
    -- Bỏ qua:  ORA-12420 (đã tồn tại) để idempotent.
    -- -------------------------------------------------------------------------
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

    -- -------------------------------------------------------------------------
    -- Thủ tục nội bộ: safe_create_group
    -- Mục đích: Tạo một Group (nhóm địa lý/tổ chức) trong policy THONGBAO_OLS.
    --           Group hỗ trợ phân cấp cha-con: user thuộc group cha có thể đọc
    --           dữ liệu của group con (nếu cấu hình parent_group).
    -- Bỏ qua:  ORA-12421 (đã tồn tại) để idempotent.
    -- -------------------------------------------------------------------------
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

    -- -------------------------------------------------------------------------
    -- Thủ tục nội bộ: safe_create_label
    -- Mục đích: Tạo một Label hợp lệ (tổ hợp Level:Compartment:Group) dùng để
    --           gán vào từng dòng dữ liệu trong bảng THONGBAO.
    --           p_tag là số định danh nội bộ Oracle, p_label là chuỗi đọc được.
    -- Bỏ qua:  ORA-1 (unique) và ORA-12453 (label đã tồn tại) để idempotent.
    -- -------------------------------------------------------------------------
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
    -- Level: phân cấp chức danh (số lớn = quyền đọc rộng hơn)
    -- L3_GD (30): Ban Giám đốc — cấp cao nhất, đọc được tất cả level ≤ 30
    -- L2_LD (20): Lãnh đạo Khoa — đọc được level ≤ 20
    -- L1_NV (10): Nhân viên thường — chỉ đọc level ≤ 10
    safe_create_level(30, 'L3_GD', 'Ban Giam Doc');
    safe_create_level(20, 'L2_LD', 'Lanh Dao Khoa');
    safe_create_level(10, 'L1_NV', 'Nhan Vien');

    -- Compartment: phân theo chuyên khoa bệnh viện
    -- User phải có compartment tương ứng mới đọc được thông báo của khoa đó
    safe_create_compartment(100, 'C_TIEU', 'Tieu Hoa');
    safe_create_compartment(110, 'C_THAN', 'Than Kinh');
    safe_create_compartment(120, 'C_TIM', 'Tim Mach');

    -- Group: phân theo chi nhánh địa lý
    -- User thuộc group nào chỉ đọc được thông báo của group đó
    safe_create_group(10, 'G_HN', 'Ha Noi');
    safe_create_group(20, 'G_HP', 'Hai Phong');
    safe_create_group(30, 'G_HCM', 'Ho Chi Minh');

    -- Label tổng hợp: kết hợp Level + Compartment + Group
    -- Cú pháp: 'Level[:Compartment1,Compartment2[:Group1,Group2]]'
    -- Label chỉ Level: thông báo toàn hệ thống không giới hạn chuyên khoa/vùng
    safe_create_label(1000, 'L1_NV');
    safe_create_label(2000, 'L2_LD');
    safe_create_label(3000, 'L3_GD');
    -- Label Level + Compartment: thông báo theo chức danh + chuyên khoa
    safe_create_label(2100, 'L2_LD:C_TIEU');
    -- Label đầy đủ Level + Compartment + Group: thông báo cụ thể nhất
    safe_create_label(1130, 'L1_NV:C_TIEU:G_HCM');
    safe_create_label(1110, 'L1_NV:C_TIEU:G_HN');
    safe_create_label(2220, 'L2_LD:C_TIEU,C_THAN:G_HP');

    -- Gắn policy vào bảng THONGBAO nếu chưa được áp dụng
    -- READ_CONTROL: Oracle sẽ tự động lọc SELECT dựa trên cột OLS_LABEL
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

-- =============================================================================
-- KHỐI 3: GÁN NHÃN DỮ LIỆU VÀ CẤU HÌNH NHÃN USER
-- Mục đích: (1) Gán nhãn OLS cho tất cả dòng THONGBAO chưa có nhãn (mặc định L1_NV).
--           (2) Xóa và tạo lại các thông báo mẫu t1-t7 với nhãn khác nhau để minh hoạ
--               cơ chế lọc theo cấp độ/chuyên khoa/vùng địa lý.
--           (3) Gán nhãn tối đa (max label) cho từng user cụ thể — xác định phạm vi
--               dữ liệu tối đa mà user đó được phép đọc.
-- =============================================================================
BEGIN
    -- Gán nhãn mặc định L1_NV cho tất cả dòng cũ chưa có nhãn OLS
    -- Đảm bảo mọi dòng đều nằm trong policy, không có dòng "vô nhãn" lọt qua
    UPDATE THONGBAO
    SET OLS_LABEL = CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV')
    WHERE OLS_LABEL IS NULL;

    -- Xóa dữ liệu mẫu cũ (nếu có) để tránh trùng lặp khi chạy lại script
    DELETE FROM THONGBAO
    WHERE NOIDUNG LIKE 't1:%'
       OR NOIDUNG LIKE 't2:%'
       OR NOIDUNG LIKE 't3:%'
       OR NOIDUNG LIKE 't4:%'
       OR NOIDUNG LIKE 't5:%'
       OR NOIDUNG LIKE 't6:%'
       OR NOIDUNG LIKE 't7:%';

    -- t1: Nhãn L1_NV (level 10) — mọi nhân viên đều đọc được
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t1: Gui den toan bo nhan vien', SYSTIMESTAMP, 'Online', CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV'));

    -- t2: Nhãn L3_GD (level 30) — chỉ Ban Giám đốc (level ≥ 30) đọc được
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t2: Gui den toan bo Ban giam doc', SYSTIMESTAMP, 'Phong hop Giam doc', CHAR_TO_LABEL('THONGBAO_OLS', 'L3_GD'));

    -- t3: Nhãn L2_LD (level 20) — Lãnh đạo Khoa trở lên (level ≥ 20) đọc được
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t3: Gui den cac lanh dao khoa', SYSTIMESTAMP, 'Hoi truong', CHAR_TO_LABEL('THONGBAO_OLS', 'L2_LD'));

    -- t4: Nhãn L2_LD:C_TIEU — Lãnh đạo Khoa Tiêu Hóa (cần đủ level ≥ 20 VÀ compartment C_TIEU)
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t4: Gui den lanh dao Khoa tieu hoa', SYSTIMESTAMP, 'Khoa tieu hoa', CHAR_TO_LABEL('THONGBAO_OLS', 'L2_LD:C_TIEU'));

    -- t5: Nhãn L1_NV:C_TIEU:G_HCM — Nhân viên Khoa Tiêu Hóa ở Hồ Chí Minh
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t5: Gui den nhan vien Khoa tieu hoa o Ho Chi Minh', SYSTIMESTAMP, 'Ho Chi Minh', CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV:C_TIEU:G_HCM'));

    -- t6: Nhãn L1_NV:C_TIEU:G_HN — Nhân viên Khoa Tiêu Hóa ở Hà Nội
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t6: Gui den nhan vien Khoa tieu hoa o Ha Noi', SYSTIMESTAMP, 'Ha Noi', CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV:C_TIEU:G_HN'));

    -- t7: Nhãn L2_LD:C_TIEU,C_THAN:G_HP — Lãnh đạo Khoa Tiêu Hóa & Thần Kinh tại Hải Phòng
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES ('t7: Gui den lanh dao Khoa tieu hoa va Khoa than kinh tai Hai Phong', SYSTIMESTAMP, 'Hai Phong', CHAR_TO_LABEL('THONGBAO_OLS', 'L2_LD:C_TIEU,C_THAN:G_HP'));

    -- ==========================================================================
    -- GÁN NHÃN TỐI ĐA CHO TỪNG USER (SET_USER_LABELS)
    -- Cú pháp nhãn: 'MaxLevel:Compartments:Groups'
    -- MaxLevel: mức tối đa user có thể đọc (dominance — đọc được mọi level ≤ MaxLevel)
    -- Compartments: danh sách chuyên khoa user có quyền đọc (ngăn cách bằng dấu phẩy)
    -- Groups: danh sách chi nhánh user thuộc về
    -- ==========================================================================

    -- HOSPITAL_ADMIN: quyền cao nhất — L3_GD, toàn bộ chuyên khoa, toàn bộ vùng
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'HOSPITAL_ADMIN', 'L3_GD:C_TIEU,C_THAN,C_TIM:G_HN,G_HP,G_HCM');

    -- 990000000001: Ban Giám đốc — toàn quyền đọc (tương đương HOSPITAL_ADMIN)
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', '"990000000001"', 'L3_GD:C_TIEU,C_THAN,C_TIM:G_HN,G_HP,G_HCM');

    -- 990000000090: Lãnh đạo Khoa Tim Mạch ở Hồ Chí Minh
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', '"990000000090"', 'L2_LD:C_TIM:G_HCM');

    -- 990000000060: Lãnh đạo Khoa Thần Kinh ở Hà Nội
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', '"990000000060"', 'L2_LD:C_THAN:G_HN');

    -- 990000000061: Nhân viên Khoa Thần Kinh ở Hồ Chí Minh
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', '"990000000061"', 'L1_NV:C_THAN:G_HCM');

    -- 990000000091: Nhân viên Khoa Tim Mạch ở Hồ Chí Minh
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', '"990000000091"', 'L1_NV:C_TIM:G_HCM');

    -- 990000000002: Lãnh đạo Khoa Tim Mạch ở Hồ Chí Minh (thêm mẫu lãnh đạo khoa)
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', '"990000000002"', 'L2_LD:C_TIM:G_HCM');

    -- 990000000003: Lãnh đạo quyền truy cập rộng — toàn bộ chuyên khoa, toàn bộ vùng, level L2
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', '"990000000003"', 'L2_LD:C_TIEU,C_THAN,C_TIM:G_HN,G_HP,G_HCM');

    -- 990000000030: Nhân viên Khoa Tiêu Hóa ở Hà Nội
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', '"990000000030"', 'L1_NV:C_TIEU:G_HN');
END;
/

-- =============================================================================
-- CẤP QUYỀN SELECT TRÊN BẢNG THONGBAO CHO CÁC ORACLE ROLE
-- Mục đích: Tất cả nhân viên (Điều phối viên, Bác sĩ, Kỹ thuật viên) và bệnh nhân
--           đều được phép SELECT bảng THONGBAO. Tuy nhiên OLS sẽ tự động lọc dòng
--           theo nhãn của từng user — họ chỉ thấy thông báo phù hợp với nhãn của mình.
-- =============================================================================
GRANT SELECT ON THONGBAO TO DIEU_PHOI_VIEN;
GRANT SELECT ON THONGBAO TO BAC_SI_Y_SI;
GRANT SELECT ON THONGBAO TO KY_THUAT_VIEN;
GRANT SELECT ON THONGBAO TO BENH_NHAN;

-- SP_ADD_PATIENT: thủ tục đặc biệt cho phép Điều phối viên tạo tài khoản bệnh nhân mới
GRANT EXECUTE ON SP_ADD_PATIENT TO DIEU_PHOI_VIEN;

COMMIT;

PROMPT === OLS setup completed ===
WHENEVER SQLERROR CONTINUE;
