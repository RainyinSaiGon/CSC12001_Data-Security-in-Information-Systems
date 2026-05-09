
SHOW CON_NAME;
SELECT STATUS FROM V$INSTANCE;
SELECT OPEN_MODE FROM V$DATABASE;
-- xóa 1 dòng trong bảng HSBA
DELETE FROM hospital_admin.HSBA WHERE MAHSBA =1;
commit;

select * from hospital_admin.HSBA;

-- dang nhap bang KTV
select * from hospital_admin.HSBA ;

INSERT INTO hospital_admin.HSBA (MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA)
VALUES (4, TO_DATE('01-01-2026', 'DD-MM-YYYY'), 'TEST RECOVERY', 'Test', 'test', 21, 'KHOA02');
commit;

select * from hospital_admin.HSBA where MABN = 4;
