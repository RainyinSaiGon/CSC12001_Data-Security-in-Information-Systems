SELECT flashback_on FROM v$database;
alter database flashback on;

-- Set the flash recovery area (if not already set)
ALTER SYSTEM SET db_recovery_file_dest = 'D:/oracle-backup/fra';

-- Set the size for the flash recovery area
ALTER SYSTEM SET db_recovery_file_dest_size = 10G;

-- Enable flashback
SHUTDOWN IMMEDIATE;
STARTUP MOUNT;
ALTER DATABASE FLASHBACK ON;
ALTER DATABASE OPEN;


-- dang nhap bang KTV
select * from hospital_admin.HSBA ;

INSERT INTO hospital_admin.HSBA (MABN, NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS, MAKHOA)
VALUES (4, TO_DATE('01-01-2026', 'DD-MM-YYYY'), 'TEST RECOVERY', 'Test', 'test', 21, 'KHOA02');
commit;

select * from hospital_admin.HSBA where MABN = 4;

ALTER TABLE hospital_admin.HSBA ENABLE ROW MOVEMENT;
-- Thực hiện flashback để khôi phục dữ liệu về trạng thái trước khi thực hiện INSERT
FLASHBACK TABLE hospital_admin.HSBA TO TIMESTAMP TO_TIMESTAMP('2026-04-14 21:06:00', 'YYYY-MM-DD HH24:MI:SS');

ALTER PLUGGABLE DATABASE XEPDB1 CLOSE IMMEDIATE;
