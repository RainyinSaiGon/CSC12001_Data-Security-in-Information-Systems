SET SERVEROUTPUT ON;
SET DEFINE OFF;

PROMPT === INSERT SAMPLE DATA ===

BEGIN
    DBMS_OUTPUT.PUT_LINE('Cleaning existing sample rows...');

    BEGIN EXECUTE IMMEDIATE 'UPDATE KHOA SET TRUONGKHOA = NULL'; EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN EXECUTE IMMEDIATE 'DELETE FROM HSBA_DV'; EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN EXECUTE IMMEDIATE 'DELETE FROM DONTHUOC'; EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN EXECUTE IMMEDIATE 'DELETE FROM HSBA'; EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN EXECUTE IMMEDIATE 'DELETE FROM THONGBAO'; EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN EXECUTE IMMEDIATE 'DELETE FROM NHANVIEN'; EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN EXECUTE IMMEDIATE 'DELETE FROM BENHNHAN'; EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN EXECUTE IMMEDIATE 'DELETE FROM KHOA'; EXCEPTION WHEN OTHERS THEN NULL; END;
END;
/

COMMIT;

-- INSERT SAMPLE DATA

INSERT INTO KHOA VALUES ('KHOA01', N'Khoa tiêu hóa', '0900000001', NULL);
INSERT INTO KHOA VALUES ('KHOA02', N'Khoa thần kinh', '0900000002', NULL);
INSERT INTO KHOA VALUES ('KHOA03', N'Khoa tim mạch', '0900000003', NULL);
COMMIT;

/* ==========================================================================
   BƯỚC 3: TẠO 170 NHÂN VIÊN (TÊN THẬT & ID RIÊNG BIỆT)
    Range CCCD: Bắt đầu bằng 99... (990000000001 -> 990000000170)
   ========================================================================== */
DECLARE
    TYPE t_arr IS TABLE OF NVARCHAR2(50);
    v_ho      t_arr := t_arr(N'Nguyễn', N'Trần', N'Lê', N'Phạm', N'Huỳnh', N'Hoàng', N'Phan', N'Vũ', N'Võ', N'Đặng', N'Bùi', N'Đỗ', N'Hồ', N'Ngô', N'Dương', N'Lý');
    v_lot_nam t_arr := t_arr(N'Văn', N'Hữu', N'Đức', N'Thành', N'Công', N'Minh', N'Quốc', N'Thế', N'Gia', N'Mạnh', N'Hải');
    v_ten_nam t_arr := t_arr(N'Hùng', N'Cường', N'Tuấn', N'Dũng', N'Minh', N'Hiếu', N'Nhân', N'Trí', N'Tín', N'Phúc', N'Khang', N'Bảo', N'Lâm', N'Sơn', N'Tùng', N'Thịnh');
    v_lot_nu  t_arr := t_arr(N'Thị', N'Diệu', N'Ánh', N'Ngọc', N'Thanh', N'Phương', N'Hồng', N'Mai', N'Thảo', N'Thu', N'Kim');
    v_ten_nu  t_arr := t_arr(N'Hoa', N'Lan', N'Huệ', N'Cúc', N'Dung', N'Hạnh', N'Trang', N'Huyền', N'Thư', N'Thảo', N'Linh', N'Vy', N'Nhi', N'Hân', N'Quỳnh', N'Yến');

    v_hoten NVARCHAR2(150);
    v_phai NVARCHAR2(10);
    v_vaitro NVARCHAR2(100);
    v_khoa VARCHAR2(10);
    v_cmnd_nv VARCHAR2(20);
    v_username_nv VARCHAR2(30);
BEGIN
    FOR i IN 1..170 LOOP
        IF DBMS_RANDOM.VALUE > 0.4 THEN
            v_phai := N'Nữ';
            v_hoten := v_ho(TRUNC(DBMS_RANDOM.VALUE(1, v_ho.COUNT + 1))) || ' ' ||
                       v_lot_nu(TRUNC(DBMS_RANDOM.VALUE(1, v_lot_nu.COUNT + 1))) || ' ' ||
                       v_ten_nu(TRUNC(DBMS_RANDOM.VALUE(1, v_ten_nu.COUNT + 1)));
        ELSE
            v_phai := N'Nam';
            v_hoten := v_ho(TRUNC(DBMS_RANDOM.VALUE(1, v_ho.COUNT + 1))) || ' ' ||
                       v_lot_nam(TRUNC(DBMS_RANDOM.VALUE(1, v_lot_nam.COUNT + 1))) || ' ' ||
                       v_ten_nam(TRUNC(DBMS_RANDOM.VALUE(1, v_ten_nam.COUNT + 1)));
        END IF;

        IF i <= 20 THEN
            v_vaitro := N'Điều phối viên';
            IF MOD(i, 3) = 0 THEN
                v_khoa := 'KHOA01';
            ELSIF MOD(i, 3) = 1 THEN
                v_khoa := 'KHOA02';
            ELSE
                v_khoa := 'KHOA03';
            END IF;
        ELSIF i <= 120 THEN
            v_vaitro := N'Bác sĩ/Y sĩ';
            IF i BETWEEN 21 AND 53 THEN
                v_khoa := 'KHOA01';
            ELSIF i BETWEEN 54 AND 86 THEN
                v_khoa := 'KHOA02';
            ELSE
                v_khoa := 'KHOA03';
            END IF;
        ELSE
            v_vaitro := N'Kỹ thuật viên';
            IF MOD(i, 3) = 0 THEN
                v_khoa := 'KHOA01';
            ELSIF MOD(i, 3) = 1 THEN
                v_khoa := 'KHOA02';
            ELSE
                v_khoa := 'KHOA03';
            END IF;
        END IF;

        v_cmnd_nv := '99' || LPAD(i, 10, '0');

        INSERT INTO NHANVIEN (HOTEN, PHAI, NGAYSINH, CCCD, QUEQUAN, SODT, VAITRO, CHUYENKHOA, USERNAME, PASSWORD_HASH)
        VALUES (
            v_hoten,
            v_phai,
            DATE '1980-01-01' + TRUNC(DBMS_RANDOM.VALUE(0, 8000)),
            v_cmnd_nv,
            N'TP. Hồ Chí Minh',
            '09' || LPAD(i, 8, '0'),
            v_vaitro,
            v_khoa,
            v_cmnd_nv,
            NULL
        );
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

    v_hoten NVARCHAR2(150);
    v_phai NVARCHAR2(10);
    v_sonha NVARCHAR2(50);
    v_tenduong NVARCHAR2(100);
    v_quanhuyen NVARCHAR2(100);
    v_cccd_bn VARCHAR2(20);
    v_username_bn VARCHAR2(30);
    v_skipped NUMBER := 0;
BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> BẮT ĐẦU TẠO BỆNH NHÂN...');
    FOR i IN 1..100000 LOOP
        IF DBMS_RANDOM.VALUE > 0.5 THEN
            v_phai := N'Nữ';
            v_hoten := v_ho(TRUNC(DBMS_RANDOM.VALUE(1, v_ho.COUNT + 1))) || ' ' ||
                       v_lot_nu(TRUNC(DBMS_RANDOM.VALUE(1, v_lot_nu.COUNT + 1))) || ' ' ||
                       v_ten_nu(TRUNC(DBMS_RANDOM.VALUE(1, v_ten_nu.COUNT + 1)));
        ELSE
            v_phai := N'Nam';
            v_hoten := v_ho(TRUNC(DBMS_RANDOM.VALUE(1, v_ho.COUNT + 1))) || ' ' ||
                       v_lot_nam(TRUNC(DBMS_RANDOM.VALUE(1, v_lot_nam.COUNT + 1))) || ' ' ||
                       v_ten_nam(TRUNC(DBMS_RANDOM.VALUE(1, v_ten_nam.COUNT + 1)));
        END IF;

        v_sonha := TO_CHAR(TRUNC(DBMS_RANDOM.VALUE(1, 999)));
        v_tenduong := v_duong(TRUNC(DBMS_RANDOM.VALUE(1, v_duong.COUNT + 1)));
        v_quanhuyen := v_quan(TRUNC(DBMS_RANDOM.VALUE(1, v_quan.COUNT + 1)));
        v_cccd_bn := LPAD(i, 12, '0');

        BEGIN
            INSERT INTO BENHNHAN (
                TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN,
                TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC, USERNAME, PASSWORD_HASH
            )
            VALUES (
                v_hoten,
                v_phai,
                ADD_MONTHS(TRUNC(SYSDATE), -1 * TRUNC(DBMS_RANDOM.VALUE(18 * 12, 90 * 12))),
                v_cccd_bn,
                v_sonha,
                v_tenduong,
                v_quanhuyen,
                N'TP. Hồ Chí Minh',
                N'Không',
                N'Không',
                N'Không',
                v_cccd_bn,
                NULL
            );
        EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
                v_skipped := v_skipped + 1;
            WHEN OTHERS THEN
                RAISE;
        END;

        IF MOD(i, 2000) = 0 THEN
            COMMIT;
        END IF;
    END LOOP;
    COMMIT;
    DBMS_OUTPUT.PUT_LINE('=== HOÀN TẤT TẠO BỆNH NHÂN');
END;
/

DECLARE
    TYPE t_list IS TABLE OF NVARCHAR2(200);

    g_benh_th  t_list := t_list(N'Viêm loét dạ dày tá tràng', N'Trào ngược dạ dày (GERD)', N'Hội chứng ruột kích thích', N'Viêm đại tràng', N'Nhiễm khuẩn HP');
    g_thuoc_th t_list := t_list(N'Omeprazole 20mg', N'Gaviscon', N'Phosphalugel', N'Metronidazole 250mg', N'Domperidon 10mg', N'Yumangel', N'Nexium 40mg');
    g_dv_th    t_list := t_list(N'Nội soi thực quản - dạ dày', N'Siêu âm ổ bụng tổng quát', N'Test hơi thở HP');

    g_benh_tk  t_list := t_list(N'Rối loạn tiền đình', N'Đau nửa đầu Migraine', N'Mất ngủ mãn tính', N'Đau dây thần kinh tọa', N'Suy nhược thần kinh');
    g_thuoc_tk t_list := t_list(N'Paracetamol 500mg', N'Piracetam 800mg', N'Magnesium B6', N'Ginkgo Biloba', N'Rotunda 30mg', N'Gabapentin');
    g_dv_tk    t_list := t_list(N'Chụp cộng hưởng từ (MRI) sọ não', N'Đo điện não đồ (EEG)', N'Chụp CT Scanner sọ não');

    g_benh_tm  t_list := t_list(N'Tăng huyết áp', N'Thiếu máu cơ tim', N'Rối loạn nhịp tim', N'Suy tim độ 2', N'Hở van 2 lá nhẹ');
    g_thuoc_tm t_list := t_list(N'Amlodipine 5mg', N'Losartan 50mg', N'Concor 2.5mg', N'Aspirin 81mg', N'Atorvastatin 10mg', N'Panangin');
    g_dv_tm    t_list := t_list(N'Đo điện tâm đồ (ECG)', N'Siêu âm tim Doppler màu', N'Holter huyết áp 24h');

    TYPE t_id_array IS TABLE OF VARCHAR2(32);
    bs_th t_id_array;
    bs_tk t_id_array;
    bs_tm t_id_array;
    ktv_list t_id_array;

    v_cur_benh NVARCHAR2(200);
    v_cur_thuoc NVARCHAR2(200);
    v_cur_dv NVARCHAR2(200);
    v_cur_bs VARCHAR2(32);
    v_cur_ktv VARCHAR2(32);

    v_mabn VARCHAR2(32);
    v_new_mahsba NUMBER;
    v_ngaykham DATE;
    v_scenario INT;
    v_num_hsba INT;
    v_cnt_hsba INT := 0;
    v_patient_counter INT := 0;
BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> BẮT ĐẦU GENERATE DỮ LIỆU KHỐI LƯỢNG LỚN...');

    SELECT MANV BULK COLLECT INTO bs_th
    FROM NHANVIEN
    WHERE CHUYENKHOA = 'KHOA01' AND (VAITRO LIKE N'%Bác sĩ%' OR VAITRO LIKE N'%Y sĩ%');

    SELECT MANV BULK COLLECT INTO bs_tk
    FROM NHANVIEN
    WHERE CHUYENKHOA = 'KHOA02' AND (VAITRO LIKE N'%Bác sĩ%' OR VAITRO LIKE N'%Y sĩ%');

    SELECT MANV BULK COLLECT INTO bs_tm
    FROM NHANVIEN
    WHERE CHUYENKHOA = 'KHOA03' AND (VAITRO LIKE N'%Bác sĩ%' OR VAITRO LIKE N'%Y sĩ%');

    SELECT MANV BULK COLLECT INTO ktv_list
    FROM NHANVIEN
    WHERE VAITRO LIKE N'%Kỹ thuật viên%';

    FOR r IN (SELECT MABN FROM BENHNHAN) LOOP
        v_patient_counter := v_patient_counter + 1;
        v_mabn := r.MABN;

        IF DBMS_RANDOM.VALUE < 0.7 THEN
            v_num_hsba := TRUNC(DBMS_RANDOM.VALUE(2, 4));

            FOR h IN 1..v_num_hsba LOOP
                v_scenario := TRUNC(DBMS_RANDOM.VALUE(1, 4));
                v_ngaykham := SYSDATE - TRUNC(DBMS_RANDOM.VALUE(1, 700));

                IF v_scenario = 1 AND bs_th.COUNT > 0 THEN
                    v_cur_bs := bs_th(TRUNC(DBMS_RANDOM.VALUE(1, bs_th.COUNT + 1)));
                    v_cur_benh := g_benh_th(TRUNC(DBMS_RANDOM.VALUE(1, g_benh_th.COUNT + 1)));

                    INSERT INTO HSBA (MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA)
                    VALUES (v_mabn, v_ngaykham, v_cur_benh, N'Điều trị ngoại trú', N'Tái khám', v_cur_bs, 'KHOA01')
                    RETURNING MAHSBA INTO v_new_mahsba;

                    FOR d IN 1..2 LOOP
                        v_cur_thuoc := g_thuoc_th(TRUNC(DBMS_RANDOM.VALUE(1, g_thuoc_th.COUNT + 1)));
                        BEGIN
                            INSERT INTO DONTHUOC (MAHSBA, TENTHUOC, LIEUDUNG, NGAYDT)
                            VALUES (v_new_mahsba, v_cur_thuoc, N'Sáng 1 viên', v_ngaykham);
                        EXCEPTION
                            WHEN DUP_VAL_ON_INDEX THEN NULL;
                        END;
                    END LOOP;

                    v_cur_dv := g_dv_th(TRUNC(DBMS_RANDOM.VALUE(1, g_dv_th.COUNT + 1)));
                    v_cur_ktv := ktv_list(TRUNC(DBMS_RANDOM.VALUE(1, ktv_list.COUNT + 1)));
                    BEGIN
                        INSERT INTO HSBA_DV (MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV)
                        VALUES (v_new_mahsba, v_cur_dv, v_ngaykham, N'Bình thường', v_cur_ktv);
                    EXCEPTION
                        WHEN DUP_VAL_ON_INDEX THEN NULL;
                    END;

                ELSIF v_scenario = 2 AND bs_tk.COUNT > 0 THEN
                    v_cur_bs := bs_tk(TRUNC(DBMS_RANDOM.VALUE(1, bs_tk.COUNT + 1)));
                    v_cur_benh := g_benh_tk(TRUNC(DBMS_RANDOM.VALUE(1, g_benh_tk.COUNT + 1)));

                    INSERT INTO HSBA (MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA)
                    VALUES (v_mabn, v_ngaykham, v_cur_benh, N'Nghỉ ngơi', N'Theo dõi', v_cur_bs, 'KHOA02')
                    RETURNING MAHSBA INTO v_new_mahsba;

                    FOR d IN 1..2 LOOP
                        v_cur_thuoc := g_thuoc_tk(TRUNC(DBMS_RANDOM.VALUE(1, g_thuoc_tk.COUNT + 1)));
                        BEGIN
                            INSERT INTO DONTHUOC (MAHSBA, TENTHUOC, LIEUDUNG, NGAYDT)
                            VALUES (v_new_mahsba, v_cur_thuoc, N'Uống khi đau', v_ngaykham);
                        EXCEPTION
                            WHEN DUP_VAL_ON_INDEX THEN NULL;
                        END;
                    END LOOP;

                    v_cur_dv := g_dv_tk(TRUNC(DBMS_RANDOM.VALUE(1, g_dv_tk.COUNT + 1)));
                    v_cur_ktv := ktv_list(TRUNC(DBMS_RANDOM.VALUE(1, ktv_list.COUNT + 1)));
                    BEGIN
                        INSERT INTO HSBA_DV (MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV)
                        VALUES (v_new_mahsba, v_cur_dv, v_ngaykham, N'Ổn định', v_cur_ktv);
                    EXCEPTION
                        WHEN DUP_VAL_ON_INDEX THEN NULL;
                    END;

                ELSIF v_scenario = 3 AND bs_tm.COUNT > 0 THEN
                    v_cur_bs := bs_tm(TRUNC(DBMS_RANDOM.VALUE(1, bs_tm.COUNT + 1)));
                    v_cur_benh := g_benh_tm(TRUNC(DBMS_RANDOM.VALUE(1, g_benh_tm.COUNT + 1)));

                    INSERT INTO HSBA (MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA)
                    VALUES (v_mabn, v_ngaykham, v_cur_benh, N'Duy trì huyết áp', N'Đo huyết áp', v_cur_bs, 'KHOA03')
                    RETURNING MAHSBA INTO v_new_mahsba;

                    FOR d IN 1..2 LOOP
                        v_cur_thuoc := g_thuoc_tm(TRUNC(DBMS_RANDOM.VALUE(1, g_thuoc_tm.COUNT + 1)));
                        BEGIN
                            INSERT INTO DONTHUOC (MAHSBA, TENTHUOC, LIEUDUNG, NGAYDT)
                            VALUES (v_new_mahsba, v_cur_thuoc, N'Sáng 1 viên', v_ngaykham);
                        EXCEPTION
                            WHEN DUP_VAL_ON_INDEX THEN NULL;
                        END;
                    END LOOP;

                    v_cur_dv := g_dv_tm(TRUNC(DBMS_RANDOM.VALUE(1, g_dv_tm.COUNT + 1)));
                    v_cur_ktv := ktv_list(TRUNC(DBMS_RANDOM.VALUE(1, ktv_list.COUNT + 1)));
                    BEGIN
                        INSERT INTO HSBA_DV (MAHSBA, LOAIDV, NGAYDV, KETQUA, MAKTV)
                        VALUES (v_new_mahsba, v_cur_dv, v_ngaykham, N'Nhịp xoang đều', v_cur_ktv);
                    EXCEPTION
                        WHEN DUP_VAL_ON_INDEX THEN NULL;
                    END;
                END IF;

                v_cnt_hsba := v_cnt_hsba + 1;
            END LOOP;
        END IF;

        IF MOD(v_patient_counter, 2000) = 0 THEN
            COMMIT;
        END IF;
    END LOOP;

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
        CASE TRUNC(DBMS_RANDOM.VALUE(1, 5))
            WHEN 1 THEN v_noidung := N'Thông báo lịch trực';
            WHEN 2 THEN v_noidung := N'Họp chuyên môn nội bộ';
            WHEN 3 THEN v_noidung := N'Báo cáo tài chính quý';
            ELSE v_noidung := N'Nhắc nhở quy chế';
        END CASE;

        CASE TRUNC(DBMS_RANDOM.VALUE(1, 4))
            WHEN 1 THEN v_diadiem := N'Hội trường A, chi nhánh TP.HCM';
            WHEN 2 THEN v_diadiem := N'Khoa Tiêu Hóa, chi nhánh TP. Hải Phòng';
            ELSE v_diadiem := N'Phòng Giám Đốc, chi nhánh Hà Nội';
        END CASE;

        INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM)
        VALUES (v_noidung, SYSDATE - TRUNC(DBMS_RANDOM.VALUE(0, 365)), v_diadiem);

        IF MOD(i, 2000) = 0 THEN
            COMMIT;
        END IF;
    END LOOP;

    COMMIT;
    DBMS_OUTPUT.PUT_LINE('=== HOÀN TẤT 12,000 THÔNG BÁO ===');
END;
/

PROMPT === SAMPLE DATA COMPLETED ===
