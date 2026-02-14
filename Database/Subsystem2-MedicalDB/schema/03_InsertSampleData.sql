--gen data
--KHOA

INSERT INTO KHOA VALUES ('KHOA01', N'Khoa tiêu hóa', '0900000001', NULL);
INSERT INTO KHOA VALUES ('KHOA02', N'Khoa thần kinh', '0900000002', NULL);
INSERT INTO KHOA VALUES ('KHOA03', N'Khoa tim mạch', '0900000003', NULL);
COMMIT;


/* ==========================================================================
   BƯỚC 3: TẠO 170 NHÂN VIÊN (TÊN THẬT & ID RIÊNG BIỆT)
   Range CMND: Bắt đầu bằng 99... (990000000001 -> 990000000170)
   ========================================================================== */
DECLARE
    TYPE t_arr IS TABLE OF NVARCHAR2(50);
    v_ho      t_arr := t_arr(N'Nguyễn', N'Trần', N'Lê', N'Phạm', N'Huỳnh', N'Hoàng', N'Phan', N'Vũ', N'Võ', N'Đặng', N'Bùi', N'Đỗ', N'Hồ', N'Ngô', N'Dương', N'Lý');
    v_lot_nam t_arr := t_arr(N'Văn', N'Hữu', N'Đức', N'Thành', N'Công', N'Minh', N'Quốc', N'Thế', N'Gia', N'Mạnh', N'Hải');
    v_ten_nam t_arr := t_arr(N'Hùng', N'Cường', N'Tuấn', N'Dũng', N'Minh', N'Hiếu', N'Nhân', N'Trí', N'Tín', N'Phúc', N'Khang', N'Bảo', N'Lâm', N'Sơn', N'Tùng', N'Thịnh');
    v_lot_nu  t_arr := t_arr(N'Thị', N'Diệu', N'Ánh', N'Ngọc', N'Thanh', N'Phương', N'Hồng', N'Mai', N'Thảo', N'Thu', N'Kim');
    v_ten_nu  t_arr := t_arr(N'Hoa', N'Lan', N'Huệ', N'Cúc', N'Dung', N'Hạnh', N'Trang', N'Huyền', N'Thư', N'Thảo', N'Linh', N'Vy', N'Nhi', N'Hân', N'Quỳnh', N'Yến');

    v_hoten NVARCHAR2(100);
    v_phai NVARCHAR2(3);
    v_vaitro NVARCHAR2(50);
    v_khoa CHAR(6);
    v_cmnd_nv CHAR(12); -- Biến lưu CMND nhân viên
BEGIN
    FOR i IN 1..170 LOOP
        -- 1. Random Tên
        IF DBMS_RANDOM.VALUE > 0.4 THEN 
            v_phai := N'Nữ';
            v_hoten := v_ho(TRUNC(DBMS_RANDOM.VALUE(1, v_ho.COUNT + 1))) || ' ' || v_lot_nu(TRUNC(DBMS_RANDOM.VALUE(1, v_lot_nu.COUNT + 1))) || ' ' || v_ten_nu(TRUNC(DBMS_RANDOM.VALUE(1, v_ten_nu.COUNT + 1)));
        ELSE
            v_phai := N'Nam';
            v_hoten := v_ho(TRUNC(DBMS_RANDOM.VALUE(1, v_ho.COUNT + 1))) || ' ' || v_lot_nam(TRUNC(DBMS_RANDOM.VALUE(1, v_lot_nam.COUNT + 1))) || ' ' || v_ten_nam(TRUNC(DBMS_RANDOM.VALUE(1, v_ten_nam.COUNT + 1)));
        END IF;

        -- 2. Logic Vai trò & Khoa
        IF i <= 20 THEN 
            v_vaitro := N'Điều phối viên';
            IF MOD(i,3)=0 THEN v_khoa:='KHOA01'; ELSIF MOD(i,3)=1 THEN v_khoa:='KHOA02'; ELSE v_khoa:='KHOA03'; END IF;
        ELSIF i <= 120 THEN 
            v_vaitro := N'Bác sĩ/Y sĩ';
            IF i BETWEEN 21 AND 53 THEN v_khoa := 'KHOA01';
            ELSIF i BETWEEN 54 AND 86 THEN v_khoa := 'KHOA02';
            ELSE v_khoa := 'KHOA03';
            END IF;
        ELSE 
            v_vaitro := N'Kỹ thuật viên';
            IF MOD(i,3)=0 THEN v_khoa:='KHOA01'; ELSIF MOD(i,3)=1 THEN v_khoa:='KHOA02'; ELSE v_khoa:='KHOA03'; END IF;
        END IF;

        -- 3. TẠO CMND NHÂN VIÊN (ĐẦU SỐ 99 ĐỂ KHÔNG TRÙNG)
        -- Kết quả: 990000000001, 990000000002...
        v_cmnd_nv := '99' || LPAD(i, 10, '0');

        INSERT INTO NHANVIEN (HOTEN, PHAI, NGAYSINH, CMND, QUEQUAN, SODT, VAITRO, CHUYENKHOA, USERNAME)
        VALUES (v_hoten, v_phai, DATE '1980-01-01' + TRUNC(DBMS_RANDOM.VALUE(0, 8000)), v_cmnd_nv, N'TP. Hồ Chí Minh', '09' || LPAD(i, 8, '0'), v_vaitro, v_khoa, 'NV' || LPAD(i, 6, '0'));
    END LOOP;
    COMMIT;
END;
/

/* ==========================================================================
   BƯỚC 4: TẠO 100,000 BỆNH NHÂN (TÊN THẬT & CCCD RIÊNG BIỆT)
   Range CCCD: 000... (000000000001 -> 000000100000)
   ========================================================================== */
DECLARE
    TYPE t_arr IS TABLE OF NVARCHAR2(100);
    v_ho      t_arr := t_arr(N'Nguyễn', N'Trần', N'Lê', N'Phạm', N'Huỳnh', N'Hoàng', N'Phan', N'Vũ', N'Võ', N'Đặng', N'Bùi', N'Đỗ', N'Hồ', N'Ngô', N'Dương', N'Lý', N'Đinh', N'Đoàn', N'Lâm', N'Trịnh', N'Mai', N'Đào', N'Cao');
    v_lot_nam t_arr := t_arr(N'Văn', N'Hữu', N'Đức', N'Thành', N'Công', N'Minh', N'Quốc', N'Thế', N'Gia', N'Mạnh', N'Hải', N'Chí', N'Tuấn', N'Anh', N'Nguyên', N'Bá', N'Xuân');
    v_ten_nam t_arr := t_arr(N'Hùng', N'Cường', N'Tuấn', N'Dũng', N'Minh', N'Hiếu', N'Nhân', N'Trí', N'Tín', N'Phúc', N'Khang', N'Bảo', N'Lâm', N'Sơn', N'Tùng', N'Thịnh', N'Kiên', N'Long', N'Huy', N'Hoàng', N'Vinh');
    v_lot_nu  t_arr := t_arr(N'Thị', N'Diệu', N'Ánh', N'Ngọc', N'Thanh', N'Phương', N'Hồng', N'Mai', N'Thảo', N'Thu', N'Kim', N'Mỹ', N'Nhã', N'Bảo', N'Tuyết', N'Lan');
    v_ten_nu  t_arr := t_arr(N'Hoa', N'Lan', N'Huệ', N'Cúc', N'Dung', N'Hạnh', N'Trang', N'Huyền', N'Thư', N'Thảo', N'Linh', N'Vy', N'Nhi', N'Hân', N'Quỳnh', N'Yến', N'Trâm', N'Tú', N'Châu', N'Nga', N'Vân');
    v_duong   t_arr := t_arr(N'Nguyễn Huệ', N'Lê Lợi', N'Pasteur', N'Hai Bà Trưng', N'Lê Duẩn', N'Đồng Khởi', N'Nam Kỳ Khởi Nghĩa', N'Điện Biên Phủ', N'Nguyễn Thị Minh Khai', N'CMT8', N'Võ Văn Kiệt', N'Phạm Văn Đồng', N'Hoàng Diệu', N'Nguyễn Văn Linh', N'Lý Thường Kiệt', N'3 Tháng 2');
    v_quan    t_arr := t_arr(N'Quận 1', N'Quận 3', N'Quận 4', N'Quận 5', N'Quận 6', N'Quận 7', N'Quận 8', N'Quận 10', N'Quận 11', N'Quận 12', N'Bình Thạnh', N'Phú Nhuận', N'Tân Bình', N'Gò Vấp', N'Thủ Đức');

    v_hoten NVARCHAR2(100);
    v_phai NVARCHAR2(3);
    v_sonha NVARCHAR2(20);
    v_tenduong NVARCHAR2(50);
    v_quanhuyen NVARCHAR2(30);
    v_cccd_bn CHAR(12); -- Biến lưu CCCD bệnh nhân
BEGIN
    FOR i IN 1..100000 LOOP
        IF DBMS_RANDOM.VALUE > 0.5 THEN 
            v_phai := N'Nữ';
            v_hoten := v_ho(TRUNC(DBMS_RANDOM.VALUE(1, v_ho.COUNT + 1))) || ' ' || v_lot_nu(TRUNC(DBMS_RANDOM.VALUE(1, v_lot_nu.COUNT + 1))) || ' ' || v_ten_nu(TRUNC(DBMS_RANDOM.VALUE(1, v_ten_nu.COUNT + 1)));
        ELSE
            v_phai := N'Nam';
            v_hoten := v_ho(TRUNC(DBMS_RANDOM.VALUE(1, v_ho.COUNT + 1))) || ' ' || v_lot_nam(TRUNC(DBMS_RANDOM.VALUE(1, v_lot_nam.COUNT + 1))) || ' ' || v_ten_nam(TRUNC(DBMS_RANDOM.VALUE(1, v_ten_nam.COUNT + 1)));
        END IF;

        v_sonha := TO_CHAR(TRUNC(DBMS_RANDOM.VALUE(1, 999)));
        v_tenduong := v_duong(TRUNC(DBMS_RANDOM.VALUE(1, v_duong.COUNT + 1)));
        v_quanhuyen := v_quan(TRUNC(DBMS_RANDOM.VALUE(1, v_quan.COUNT + 1)));
        
        -- 4. TẠO CCCD BỆNH NHÂN (ĐẦU SỐ 0 NHƯ CŨ)
        -- Kết quả: 000000000001...
        v_cccd_bn := LPAD(i, 12, '0');

        INSERT INTO BENHNHAN (TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC, USERNAME)
        VALUES (v_hoten, v_phai, ADD_MONTHS(SYSDATE, -1 * TRUNC(DBMS_RANDOM.VALUE(18*12, 90*12))), v_cccd_bn, v_sonha, v_tenduong, v_quanhuyen, N'TP. Hồ Chí Minh', N'Không', N'Không', N'Không', 'BN' || LPAD(i, 9, '0'));

        IF MOD(i, 2000) = 0 THEN COMMIT; END IF;
    END LOOP;
    COMMIT;
END;
/
-- =====

SET DEFINE OFF;
SET SERVEROUTPUT ON;

-- 2. TỰ ĐỘNG SỬA LỖI ĐỘ RỘNG CỘT
-- Chạy lệnh này để đảm bảo cột chứa đủ tên dịch vụ dài
BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE HSBA_DV MODIFY LOAIDV NVARCHAR2(100)';
    DBMS_OUTPUT.PUT_LINE('--- ĐÃ MỞ RỘNG CỘT LOAIDV THÀNH CÔNG ---');
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('--- Cột LOAIDV đã đủ rộng hoặc có lỗi khác (Bỏ qua) ---');
END;
/

-- 3. RESET DATA (Xóa dữ liệu cũ để chạy lại sạch sẽ)
DELETE FROM THONGBAO;
DELETE FROM DONTHUOC;
DELETE FROM HSBA_DV;
DELETE FROM HSBA;
COMMIT;

DECLARE
    -- ================= KHAI BÁO KHO DỮ LIỆU =================
    TYPE t_list IS TABLE OF NVARCHAR2(200);

    -- KHOA TIÊU HÓA (KHOA01)
    g_benh_th  t_list := t_list(N'Viêm loét dạ dày tá tràng', N'Trào ngược dạ dày (GERD)', N'Hội chứng ruột kích thích', N'Viêm đại tràng', N'Nhiễm khuẩn HP');
    g_thuoc_th t_list := t_list(N'Omeprazole 20mg', N'Gaviscon', N'Phosphalugel', N'Metronidazole 250mg', N'Domperidon 10mg', N'Yumangel', N'Nexium 40mg');
    g_dv_th    t_list := t_list(N'Nội soi thực quản - dạ dày', N'Siêu âm ổ bụng tổng quát', N'Test hơi thở HP');

    -- KHOA THẦN KINH (KHOA02)
    g_benh_tk  t_list := t_list(N'Rối loạn tiền đình', N'Đau nửa đầu Migraine', N'Mất ngủ mãn tính', N'Đau dây thần kinh tọa', N'Suy nhược thần kinh');
    g_thuoc_tk t_list := t_list(N'Paracetamol 500mg', N'Piracetam 800mg', N'Magnesium B6', N'Ginkgo Biloba', N'Rotunda 30mg', N'Gabapentin');
    g_dv_tk    t_list := t_list(N'Chụp cộng hưởng từ (MRI) sọ não', N'Đo điện não đồ (EEG)', N'Chụp CT Scanner sọ não');

    -- KHOA TIM MẠCH (KHOA03)
    g_benh_tm  t_list := t_list(N'Tăng huyết áp', N'Thiếu máu cơ tim', N'Rối loạn nhịp tim', N'Suy tim độ 2', N'Hở van 2 lá nhẹ');
    g_thuoc_tm t_list := t_list(N'Amlodipine 5mg', N'Losartan 50mg', N'Concor 2.5mg', N'Aspirin 81mg', N'Atorvastatin 10mg', N'Panangin');
    g_dv_tm    t_list := t_list(N'Đo điện tâm đồ (ECG)', N'Siêu âm tim Doppler màu', N'Holter huyết áp 24h');

    -- ================= BIẾN HỆ THỐNG =================
    TYPE t_num_array IS TABLE OF NUMBER;
    bs_th   t_num_array; bs_tk   t_num_array; bs_tm   t_num_array; ktv_list t_num_array;

    -- Biến trung gian (Fix lỗi PLS-00425)
    v_cur_benh NVARCHAR2(200); v_cur_thuoc NVARCHAR2(200); v_cur_dv NVARCHAR2(200);
    v_cur_bs NUMBER; v_cur_ktv NUMBER;

    -- Biến Loop
    v_mabn NUMBER; v_new_mahsba NUMBER; v_ngaykham DATE;
    v_scenario INT; v_num_hsba INT; 
    v_cnt_hsba INT := 0;

BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> BẮT ĐẦU GENERATE DỮ LIỆU KHỐI LƯỢNG LỚN...');

    -- 1. CACHE ID NHÂN VIÊN
    SELECT MANV BULK COLLECT INTO bs_th FROM NHANVIEN WHERE CHUYENKHOA = 'KHOA01' AND (VAITRO LIKE N'%Bác sĩ%' OR VAITRO LIKE N'%Y sĩ%');
    SELECT MANV BULK COLLECT INTO bs_tk FROM NHANVIEN WHERE CHUYENKHOA = 'KHOA02' AND (VAITRO LIKE N'%Bác sĩ%' OR VAITRO LIKE N'%Y sĩ%');
    SELECT MANV BULK COLLECT INTO bs_tm FROM NHANVIEN WHERE CHUYENKHOA = 'KHOA03' AND (VAITRO LIKE N'%Bác sĩ%' OR VAITRO LIKE N'%Y sĩ%');
    SELECT MANV BULK COLLECT INTO ktv_list FROM NHANVIEN WHERE VAITRO LIKE N'%Kỹ thuật viên%';

    -- 2. LOOP 100,000 BỆNH NHÂN
    FOR r IN (SELECT MABN FROM BENHNHAN) LOOP
        v_mabn := r.MABN;

        -- LOGIC: 70% Bệnh nhân sẽ có hồ sơ (Active)
        IF DBMS_RANDOM.VALUE < 0.7 THEN 
            
            -- Mỗi bệnh nhân Active có từ 2 ĐẾN 3 hồ sơ
            v_num_hsba := TRUNC(DBMS_RANDOM.VALUE(2, 4)); 

            FOR h IN 1..v_num_hsba LOOP
                v_scenario := TRUNC(DBMS_RANDOM.VALUE(1, 4)); -- 1:TH, 2:TK, 3:TM
                v_ngaykham := SYSDATE - TRUNC(DBMS_RANDOM.VALUE(1, 700));

                -- >>> XỬ LÝ THEO KHOA <<<
                IF v_scenario = 1 AND bs_th.COUNT > 0 THEN -- TIÊU HÓA
                    v_cur_bs := bs_th(TRUNC(DBMS_RANDOM.VALUE(1, bs_th.COUNT + 1)));
                    v_cur_benh := g_benh_th(TRUNC(DBMS_RANDOM.VALUE(1, g_benh_th.COUNT + 1)));
                    
                    INSERT INTO HSBA (MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA)
                    VALUES (v_mabn, v_ngaykham, v_cur_benh, N'Điều trị ngoại trú', N'Tái khám', v_cur_bs, 'KHOA01')
                    RETURNING MAHSBA INTO v_new_mahsba;

                    -- 2 Thuốc
                    FOR d IN 1..2 LOOP
                        v_cur_thuoc := g_thuoc_th(TRUNC(DBMS_RANDOM.VALUE(1, g_thuoc_th.COUNT + 1)));
                        BEGIN INSERT INTO DONTHUOC (MAHSBA, TENTHUOC, LIEUDUNG, NGAYDT) VALUES (v_new_mahsba, v_cur_thuoc, N'Sáng 1 viên', v_ngaykham);
                        EXCEPTION WHEN DUP_VAL_ON_INDEX THEN NULL; END;
                    END LOOP;

                    -- 1 Dịch vụ
                    v_cur_dv := g_dv_th(TRUNC(DBMS_RANDOM.VALUE(1, g_dv_th.COUNT + 1)));
                    v_cur_ktv := ktv_list(TRUNC(DBMS_RANDOM.VALUE(1, ktv_list.COUNT + 1)));
                    BEGIN INSERT INTO HSBA_DV (MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV) VALUES (v_new_mahsba, v_cur_dv, v_ngaykham, N'Bình thường', v_cur_ktv);
                    EXCEPTION WHEN DUP_VAL_ON_INDEX THEN NULL; END;

                ELSIF v_scenario = 2 AND bs_tk.COUNT > 0 THEN -- THẦN KINH
                    v_cur_bs := bs_tk(TRUNC(DBMS_RANDOM.VALUE(1, bs_tk.COUNT + 1)));
                    v_cur_benh := g_benh_tk(TRUNC(DBMS_RANDOM.VALUE(1, g_benh_tk.COUNT + 1)));
                    
                    INSERT INTO HSBA (MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA)
                    VALUES (v_mabn, v_ngaykham, v_cur_benh, N'Nghỉ ngơi', N'Theo dõi', v_cur_bs, 'KHOA02')
                    RETURNING MAHSBA INTO v_new_mahsba;

                    -- 2 Thuốc
                    FOR d IN 1..2 LOOP
                        v_cur_thuoc := g_thuoc_tk(TRUNC(DBMS_RANDOM.VALUE(1, g_thuoc_tk.COUNT + 1)));
                        BEGIN INSERT INTO DONTHUOC (MAHSBA, TENTHUOC, LIEUDUNG, NGAYDT) VALUES (v_new_mahsba, v_cur_thuoc, N'Uống khi đau', v_ngaykham);
                        EXCEPTION WHEN DUP_VAL_ON_INDEX THEN NULL; END;
                    END LOOP;

                    -- 1 Dịch vụ
                    v_cur_dv := g_dv_tk(TRUNC(DBMS_RANDOM.VALUE(1, g_dv_tk.COUNT + 1)));
                    v_cur_ktv := ktv_list(TRUNC(DBMS_RANDOM.VALUE(1, ktv_list.COUNT + 1)));
                    BEGIN INSERT INTO HSBA_DV (MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV) VALUES (v_new_mahsba, v_cur_dv, v_ngaykham, N'Ổn định', v_cur_ktv);
                    EXCEPTION WHEN DUP_VAL_ON_INDEX THEN NULL; END;

                ELSIF v_scenario = 3 AND bs_tm.COUNT > 0 THEN -- TIM MẠCH
                    v_cur_bs := bs_tm(TRUNC(DBMS_RANDOM.VALUE(1, bs_tm.COUNT + 1)));
                    v_cur_benh := g_benh_tm(TRUNC(DBMS_RANDOM.VALUE(1, g_benh_tm.COUNT + 1)));
                    
                    INSERT INTO HSBA (MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA)
                    VALUES (v_mabn, v_ngaykham, v_cur_benh, N'Duy trì huyết áp', N'Đo huyết áp', v_cur_bs, 'KHOA03')
                    RETURNING MAHSBA INTO v_new_mahsba;

                    -- 2 Thuốc
                    FOR d IN 1..2 LOOP
                        v_cur_thuoc := g_thuoc_tm(TRUNC(DBMS_RANDOM.VALUE(1, g_thuoc_tm.COUNT + 1)));
                        BEGIN INSERT INTO DONTHUOC (MAHSBA, TENTHUOC, LIEUDUNG, NGAYDT) VALUES (v_new_mahsba, v_cur_thuoc, N'Sáng 1 viên', v_ngaykham);
                        EXCEPTION WHEN DUP_VAL_ON_INDEX THEN NULL; END;
                    END LOOP;

                    -- 1 Dịch vụ
                    v_cur_dv := g_dv_tm(TRUNC(DBMS_RANDOM.VALUE(1, g_dv_tm.COUNT + 1)));
                    v_cur_ktv := ktv_list(TRUNC(DBMS_RANDOM.VALUE(1, ktv_list.COUNT + 1)));
                    BEGIN INSERT INTO HSBA_DV (MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV) VALUES (v_new_mahsba, v_cur_dv, v_ngaykham, N'Nhịp xoang đều', v_cur_ktv);
                    EXCEPTION WHEN DUP_VAL_ON_INDEX THEN NULL; END;

                END IF; -- End Kịch bản
                
                v_cnt_hsba := v_cnt_hsba + 1;
            END LOOP; -- End HSBA Loop
        END IF;

        IF MOD(v_mabn, 2000) = 0 THEN COMMIT; END IF;
    END LOOP; -- End Patient Loop
    COMMIT;
    DBMS_OUTPUT.PUT_LINE('=== HOÀN TẤT SINH HSBA: ' || v_cnt_hsba || ' hồ sơ ===');
END;
/

/* ==========================================================================
   PHẦN 3: SINH 12,000 THÔNG BÁO (THONGBAO)
   ========================================================================== */
DECLARE
    v_noidung NVARCHAR2(2000);
    v_diadiem NVARCHAR2(100);
BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> ĐANG TẠO THÔNG BÁO...');
    FOR i IN 1..12000 LOOP
        -- Giả lập nội dung
        CASE TRUNC(DBMS_RANDOM.VALUE(1,5))
            WHEN 1 THEN v_noidung := N'Thông báo lịch trực'; 
            WHEN 2 THEN v_noidung := N'Họp chuyên môn nội bộ'; 
            WHEN 3 THEN v_noidung := N'Báo cáo tài chính quý'; 
            ELSE        v_noidung := N'Nhắc nhở quy chế'; 
        END CASE;

        CASE TRUNC(DBMS_RANDOM.VALUE(1,4))
            WHEN 1 THEN v_diadiem := N'Hội trường A, chi nhánh TP.HCM';
            WHEN 2 THEN v_diadiem := N'Khoa Tiêu Hóa, chi nhánh TP. Hải Phòng';
            ELSE        v_diadiem := N'Phòng Giám Đốc, chi nhánh Hà Nội';
        END CASE;

        INSERT INTO THONGBAO(NOIDUNG, NGAYGIO, DIADIEM) 
        VALUES (v_noidung, SYSDATE - TRUNC(DBMS_RANDOM.VALUE(0,365)), v_diadiem);
        
        IF MOD(i, 2000) = 0 THEN COMMIT; END IF;
    END LOOP;
    COMMIT;
    DBMS_OUTPUT.PUT_LINE('=== HOÀN TẤT 12,000 THÔNG BÁO ===');
END;
/

/* ==========================================================================
--   CODE KIỂM TRA LẠI (VERIFICATION)
--   ========================================================================== */
--PROMPT ========================================================
--PROMPT BÁO CÁO TỔNG HỢP
--PROMPT ========================================================
--SELECT 
--    'Tong so Benh Nhan' AS METRIC, COUNT(*) AS VAL FROM BENHNHAN
--UNION ALL
--SELECT 'Tong so HSBA', COUNT(*) FROM HSBA
--UNION ALL
--SELECT 'Tong so Thong Bao', COUNT(*) FROM THONGBAO
--UNION ALL
--SELECT 'Tong so Don Thuoc', COUNT(*) FROM DONTHUOC
--UNION ALL
--SELECT 'Tong so Dich Vu', COUNT(*) FROM HSBA_DV;
----
--SET SERVEROUTPUT ON SIZE 1000000;
--SET LINESIZE 200;
--
--DECLARE
--    v_mabn_demo NUMBER;
--    v_tenbn NVARCHAR2(100);
--    v_namsinh DATE;
--    v_diachi NVARCHAR2(200);
--    
--    -- Biến tạm để in thông tin
--    v_tenkhoa NVARCHAR2(100);
--    v_tenbs NVARCHAR2(100);
--    v_tenktv NVARCHAR2(100);
--BEGIN
--    -- 1. CHỌN 1 BỆNH NHÂN NGẪU NHIÊN CÓ NHIỀU HỒ SƠ ĐỂ TEST
--    BEGIN
--        SELECT MABN, TENBN, NGAYSINH, SONHA||' '||TENDUONG||', '||QUANHUYEN
--        INTO v_mabn_demo, v_tenbn, v_namsinh, v_diachi
--        FROM (
--            SELECT b.MABN, b.TENBN, b.NGAYSINH, b.SONHA, b.TENDUONG, b.QUANHUYEN
--            FROM BENHNHAN b
--            JOIN HSBA h ON b.MABN = h.MABN
--            GROUP BY b.MABN, b.TENBN, b.NGAYSINH, b.SONHA, b.TENDUONG, b.QUANHUYEN
--            HAVING COUNT(h.MAHSBA) >= 2 -- Chỉ lấy người đi khám nhiều lần
--            ORDER BY DBMS_RANDOM.VALUE
--        )
--        WHERE ROWNUM = 1;
--    EXCEPTION 
--        WHEN NO_DATA_FOUND THEN
--            DBMS_OUTPUT.PUT_LINE('Không tìm thấy bệnh nhân nào có trên 2 hồ sơ. Hãy chạy lại Script Generator!');
--            RETURN;
--    END;
--
--    -- 2. IN THÔNG TIN HÀNH CHÍNH
--    DBMS_OUTPUT.PUT_LINE('================================================================');
--    DBMS_OUTPUT.PUT_LINE('                 HỒ SƠ BỆNH ÁN ĐIỆN TỬ (DEMO)                   ');
--    DBMS_OUTPUT.PUT_LINE('================================================================');
--    DBMS_OUTPUT.PUT_LINE('Mã BN    : ' || v_mabn_demo);
--    DBMS_OUTPUT.PUT_LINE('Họ tên   : ' || v_tenbn);
--    DBMS_OUTPUT.PUT_LINE('Ngày sinh: ' || TO_CHAR(v_namsinh, 'DD/MM/YYYY'));
--    DBMS_OUTPUT.PUT_LINE('Địa chỉ  : ' || v_diachi);
--    DBMS_OUTPUT.PUT_LINE('----------------------------------------------------------------');
--    
--    -- 3. DUYỆT QUA LỊCH SỬ KHÁM BỆNH (Sắp xếp theo ngày mới nhất trước)
--    FOR r_hsba IN (
--        SELECT h.MAHSBA, h.NGAY, h.MAKHOA, h.CHANDOAN, h.KETLUAN, h.MABS
--        FROM HSBA h
--        WHERE h.MABN = v_mabn_demo
--        ORDER BY h.NGAY DESC
--    ) LOOP
--        -- Lấy tên Khoa và Bác sĩ
--        SELECT TENKHOA INTO v_tenkhoa FROM KHOA WHERE MAKHOA = r_hsba.MAKHOA;
--        SELECT HOTEN INTO v_tenbs FROM NHANVIEN WHERE MANV = r_hsba.MABS;
--
--        DBMS_OUTPUT.PUT_LINE(' ');
--        DBMS_OUTPUT.PUT_LINE('>>> NGÀY KHÁM: ' || TO_CHAR(r_hsba.NGAY, 'DD/MM/YYYY') || '  |  Mã HSBA: ' || r_hsba.MAHSBA);
--        DBMS_OUTPUT.PUT_LINE('    Khoa      : ' || v_tenkhoa || ' (' || r_hsba.MAKHOA || ')');
--        DBMS_OUTPUT.PUT_LINE('    Bác sĩ    : ' || v_tenbs || ' (ID: ' || r_hsba.MABS || ')');
--        DBMS_OUTPUT.PUT_LINE('    Chẩn đoán : ' || r_hsba.CHANDOAN);
--        DBMS_OUTPUT.PUT_LINE('    Kết luận  : ' || r_hsba.KETLUAN);
--        
--        -- 4. IN DỊCH VỤ CẬN LÂM SÀNG
--        DBMS_OUTPUT.PUT_LINE('    --- DỊCH VỤ ĐÃ THỰC HIỆN ---');
--        FOR r_dv IN (SELECT LOAIDV, KETQUA, MAKTV FROM HSBA_DV WHERE MAHSBA = r_hsba.MAHSBA) LOOP
--            SELECT HOTEN INTO v_tenktv FROM NHANVIEN WHERE MANV = r_dv.MAKTV;
--            DBMS_OUTPUT.PUT_LINE('    + ' || RPAD(r_dv.LOAIDV, 35) || ' | KTV: ' || v_tenktv || ' | KQ: ' || r_dv.KETQUA);
--        END LOOP;
--
--        -- 5. IN ĐƠN THUỐC
--        DBMS_OUTPUT.PUT_LINE('    --- ĐƠN THUỐC ---');
--        FOR r_thuoc IN (SELECT TENTHUOC, LIEUDUNG FROM DONTHUOC WHERE MAHSBA = r_hsba.MAHSBA) LOOP
--            DBMS_OUTPUT.PUT_LINE('    o ' || RPAD(r_thuoc.TENTHUOC, 30) || ' : ' || r_thuoc.LIEUDUNG);
--        END LOOP;
--        
--        DBMS_OUTPUT.PUT_LINE('    --------------------------------------------------------');
--    END LOOP;
--    
--    DBMS_OUTPUT.PUT_LINE('===================== HẾT BỆNH ÁN ==============================');
--END;
--/
--
--SET LINESIZE 200;
--SET PAGESIZE 100;
--COLUMN VAITRO FORMAT A20;
--COLUMN TENKHOA FORMAT A25;
--COLUMN HOTEN FORMAT A25;
--COLUMN MAKHOA FORMAT A10;
--
--PROMPT ========================================================
--PROMPT BÁO CÁO 1: TỔNG HỢP SỐ LƯỢNG THEO VAI TRÒ
--PROMPT (Target: ~20 Điều phối, ~100 Bác sĩ, ~50 KTV)
--PROMPT ========================================================
--
--SELECT 
--    VAITRO, 
--    COUNT(*) AS SO_LUONG,
--    ROUND(RATIO_TO_REPORT(COUNT(*)) OVER () * 100, 1) || '%' AS TY_LE
--FROM NHANVIEN
--GROUP BY VAITRO
--ORDER BY SO_LUONG;
--
--PROMPT ========================================================
--PROMPT BÁO CÁO 2: PHÂN BỔ NHÂN SỰ VÀO CÁC KHOA
--PROMPT (Yêu cầu: Bác sĩ phải chia đều vào 3 khoa KHOA01, KHOA02, KHOA03)
--PROMPT ========================================================
--
--SELECT 
--    NV.VAITRO,
--    NV.CHUYENKHOA AS MA_KHOA,
--    CASE 
--        WHEN K.TENKHOA IS NULL THEN N'--- (Không thuộc khoa) ---' -- Thêm chữ N ở đây
--        ELSE K.TENKHOA 
--    END AS TEN_KHOA,
--    COUNT(*) AS SO_LUONG_NV
--FROM NHANVIEN NV
--LEFT JOIN KHOA K ON NV.CHUYENKHOA = K.MAKHOA
--GROUP BY NV.VAITRO, NV.CHUYENKHOA, K.TENKHOA
--ORDER BY NV.VAITRO, NV.CHUYENKHOA;
--PROMPT ========================================================
--PROMPT BÁO CÁO 3: KIỂM TRA MẪU DỮ LIỆU (TÊN THẬT & ID)
--PROMPT (Lấy ngẫu nhiên 2 người mỗi nhóm để soi)
--PROMPT ========================================================
--
--SELECT * FROM (
--    SELECT MANV, HOTEN, VAITRO, CHUYENKHOA, CMND 
--    FROM NHANVIEN 
--    WHERE VAITRO LIKE N'%Điều phối%' 
--    ORDER BY DBMS_RANDOM.VALUE
--) WHERE ROWNUM <= 2
--UNION ALL
--SELECT * FROM (
--    SELECT MANV, HOTEN, VAITRO, CHUYENKHOA, CMND 
--    FROM NHANVIEN 
--    WHERE VAITRO LIKE N'%Bác sĩ%' 
--    ORDER BY DBMS_RANDOM.VALUE
--) WHERE ROWNUM <= 2
--UNION ALL
--SELECT * FROM (
--    SELECT MANV, HOTEN, VAITRO, CHUYENKHOA, CMND 
--    FROM NHANVIEN 
--    WHERE VAITRO LIKE N'%Kỹ thuật%' 
--    ORDER BY DBMS_RANDOM.VALUE
--) WHERE ROWNUM <= 2;
--
--
