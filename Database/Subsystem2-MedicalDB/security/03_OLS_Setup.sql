/* ==========================================================================
   OLS SETUP 
   ========================================================================= */

-- 1. CLEANUP (Drop old policy)
BEGIN
    SA_SYSDBA.DISABLE_POLICY(policy_name => 'THONGBAO_OLS');
    SA_SYSDBA.DROP_POLICY(policy_name => 'THONGBAO_OLS', drop_column => FALSE); -- FALSE = Keep OLS_LABEL column if it exists
EXCEPTION WHEN OTHERS THEN NULL; END;
/

-- ==========================================================================
-- STEP 1: DEFINE POLICY & COMPONENTS 
-- ==========================================================================
BEGIN
    -- 1. Create Policy
    SA_SYSDBA.CREATE_POLICY(
        policy_name => 'THONGBAO_OLS',
        column_name => 'OLS_LABEL',
        default_options => 'READ_CONTROL, WRITE_CONTROL'
    );

    -- 2. Define Levels (Cấp bậc)
    SA_COMPONENTS.CREATE_LEVEL('THONGBAO_OLS', 30, 'L3_GD', 'Ban Giam Doc');
    SA_COMPONENTS.CREATE_LEVEL('THONGBAO_OLS', 20, 'L2_LD', 'Lanh Dao Khoa');
    SA_COMPONENTS.CREATE_LEVEL('THONGBAO_OLS', 10, 'L1_NV', 'Nhan Vien');

    -- 3. Define Compartments (Khoa)
    SA_COMPONENTS.CREATE_COMPARTMENT('THONGBAO_OLS', 100, 'C_TIEU', 'Khoa Tieu Hoa');
    SA_COMPONENTS.CREATE_COMPARTMENT('THONGBAO_OLS', 110, 'C_THAN', 'Khoa Than Kinh');
    SA_COMPONENTS.CREATE_COMPARTMENT('THONGBAO_OLS', 120, 'C_TIM',  'Khoa Tim Mach');

    -- 4. Define Groups (Cơ sở)
    SA_COMPONENTS.CREATE_GROUP('THONGBAO_OLS', 10, 'G_HN',  'Ha Noi');
    SA_COMPONENTS.CREATE_GROUP('THONGBAO_OLS', 20, 'G_HP',  'Hai Phong');
    SA_COMPONENTS.CREATE_GROUP('THONGBAO_OLS', 30, 'G_HCM', 'Ho Chi Minh');
END;
/

-- ==========================================================================
-- STEP 2: DEFINE DATA LABELS 
-- ==========================================================================
BEGIN
    -- Standard Hierarchy Labels
    SA_LABEL_ADMIN.CREATE_LABEL('THONGBAO_OLS', 1000, 'L1_NV');
    SA_LABEL_ADMIN.CREATE_LABEL('THONGBAO_OLS', 2000, 'L2_LD');
    SA_LABEL_ADMIN.CREATE_LABEL('THONGBAO_OLS', 3000, 'L3_GD');
    
    -- Specific Test Case Labels (t4-t7)
    SA_LABEL_ADMIN.CREATE_LABEL('THONGBAO_OLS', 2100, 'L2_LD:C_TIEU');
    SA_LABEL_ADMIN.CREATE_LABEL('THONGBAO_OLS', 1130, 'L1_NV:C_TIEU:G_HCM');
    SA_LABEL_ADMIN.CREATE_LABEL('THONGBAO_OLS', 1110, 'L1_NV:C_TIEU:G_HN');
    SA_LABEL_ADMIN.CREATE_LABEL('THONGBAO_OLS', 2220, 'L2_LD:C_TIEU,C_THAN:G_HP');
END;
/

-- ==========================================================================
-- STEP 3: APPLY POLICY TO TABLE 
-- ==========================================================================
BEGIN
    SA_POLICY_ADMIN.APPLY_TABLE_POLICY(
        policy_name   => 'THONGBAO_OLS',
        schema_name   => USER, -- Applies to Current User's table (Change schema if needed)
        table_name    => 'THONGBAO',
        table_options => 'READ_CONTROL, WRITE_CONTROL'
    );
END;
/

-- ==========================================================================
-- STEP 4: UPDATE EXISTING 12,000 ROWS (Random Security Assignment)
-- ==========================================================================
-- We randomly assign labels to the existing data so they are visible/hidden 
-- according to the new rules.
DECLARE
    v_random NUMBER;
    v_label_str VARCHAR2(100);
BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> UPDATING SECURITY LABELS FOR 12,000 EXISTING NOTIFICATIONS...');
    
    -- Loop through rows that have NULL label
    -- (Using a single UPDATE for performance instead of a loop)
    
    -- 1. Assign 70% to General Staff (L1_NV) - Visible to everyone
    UPDATE THONGBAO 
    SET OLS_LABEL = CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV')
    WHERE OLS_LABEL IS NULL AND ROWNUM <= (SELECT COUNT(*) * 0.7 FROM THONGBAO);
    
    -- 2. Assign 20% to Dept Leaders (L2_LD)
    UPDATE THONGBAO 
    SET OLS_LABEL = CHAR_TO_LABEL('THONGBAO_OLS', 'L2_LD')
    WHERE OLS_LABEL IS NULL AND ROWNUM <= (SELECT COUNT(*) * 0.2 FROM THONGBAO);
    
    -- 3. Assign 10% to Directors (L3_GD) - Highly Confidential
    UPDATE THONGBAO 
    SET OLS_LABEL = CHAR_TO_LABEL('THONGBAO_OLS', 'L3_GD')
    WHERE OLS_LABEL IS NULL;
    
    COMMIT;
    DBMS_OUTPUT.PUT_LINE('>>> EXISTING DATA SECURED.');
END;
/

-- ==========================================================================
-- STEP 5: INSERT REQUIRED TEST DATA (t1 - t7) 
-- ==========================================================================
BEGIN
    -- t1: Gửi đến toàn bộ nhân viên
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL) 
    VALUES (N't1: Gửi đến toàn bộ nhân viên', SYSDATE, N'Online', CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV'));

    -- t2: Gửi đến toàn bộ Ban giám đốc
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL) 
    VALUES (N't2: Gửi đến toàn bộ Ban giám đốc', SYSDATE, N'Phòng Giám đốc', CHAR_TO_LABEL('THONGBAO_OLS', 'L3_GD'));

    -- t3: Gửi đến các lãnh đạo khoa
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL) 
    VALUES (N't3: Gửi đến các lãnh đạo khoa', SYSDATE, N'Hội trường chính', CHAR_TO_LABEL('THONGBAO_OLS', 'L2_LD'));

    -- t4: Gửi đến lãnh đạo Khoa tiêu hóa
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL) 
    VALUES (N't4: Gửi đến lãnh đạo Khoa tiêu hóa', SYSDATE, N'Khoa Tiêu Hóa', CHAR_TO_LABEL('THONGBAO_OLS', 'L2_LD:C_TIEU'));

    -- t5: Gửi đến nhân viên Khoa tiêu hóa ở Hồ Chí Minh
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL) 
    VALUES (N't5: Gửi đến nhân viên Khoa tiêu hóa ở Hồ Chí Minh', SYSDATE, N'HCM Phòng 1', CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV:C_TIEU:G_HCM'));

    -- t6: Gửi đến nhân viên Khoa tiêu hóa ở Hà Nội
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL) 
    VALUES (N't6: Gửi đến nhân viên Khoa tiêu hóa ở Hà Nội', SYSDATE, N'HN Phòng 1', CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV:C_TIEU:G_HN'));

    -- t7: Gửi đến lãnh đạo Khoa tiêu hóa và Khoa thần kinh tại Hải Phòng
    INSERT INTO THONGBAO (NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL) 
    VALUES (N't7: Gửi đến lãnh đạo Khoa tiêu hóa và Khoa thần kinh tại Hải Phòng', SYSDATE, N'HP Phòng họp', CHAR_TO_LABEL('THONGBAO_OLS', 'L2_LD:C_TIEU,C_THAN:G_HP'));
    
    COMMIT;
END;
/

-- ==========================================================================
-- STEP 6: ASSIGN LABELS TO USERS 
-- ==========================================================================
BEGIN
    -- u1 (NV000001): Director
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000001', 'L3_GD:C_TIEU,C_THAN,C_TIM:G_HN,G_HP,G_HCM');

    -- u2 (NV000090): Cardio Leader HCM
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000090', 'L2_LD:C_TIM:G_HCM');

    -- u3 (NV000060): Neuro Leader HN
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000060', 'L2_LD:C_THAN:G_HN');

    -- u4 (NV000061): Neuro Staff HCM
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000061', 'L1_NV:C_THAN:G_HCM');

    -- u5 (NV000091): Cardio Staff HCM
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000091', 'L1_NV:C_TIM:G_HCM');

    -- u6 (NV000002): Room Leader reading Cardio HCM
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000002', 'L2_LD:C_TIM:G_HCM');

    -- u7 (NV000003): Room Leader reading ALL Leaders
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000003', 'L2_LD:C_TIEU,C_THAN,C_TIM:G_HN,G_HP,G_HCM');

    -- u8 (NV000030): Gastro Staff HN
    SA_USER_ADMIN.SET_USER_LABELS('THONGBAO_OLS', 'NV000030', 'L1_NV:C_TIEU:G_HN');
END;
/

-- Final Status Report
SELECT 'Total Notifications' AS ITEM, COUNT(*) AS CNT FROM THONGBAO
UNION ALL
SELECT 'Labeled L1_NV (Public)', COUNT(*) FROM THONGBAO WHERE OLS_LABEL = CHAR_TO_LABEL('THONGBAO_OLS', 'L1_NV')
UNION ALL
SELECT 'Labeled L3_GD (Confidential)', COUNT(*) FROM THONGBAO WHERE OLS_LABEL = CHAR_TO_LABEL('THONGBAO_OLS', 'L3_GD');