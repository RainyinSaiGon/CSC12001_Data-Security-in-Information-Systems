


-- Enable flashback
-- SHUTDOWN IMMEDIATE;
-- STARTUP MOUNT;
-- ALTER DATABASE FLASHBACK ON;
-- ALTER DATABASE OPEN;
-- Set the flash recovery area (if not already set)
--ALTER SYSTEM SET db_recovery_file_dest = 'D:/oracle-backup/fra';
-- Set the size for the flash recovery area
--ALTER SYSTEM SET db_recovery_file_dest_size = 10G;
-- ALTER SESSION SET CONTAINER = XEPDB1;
-- ALTER TABLE hospital_admin.HSBA ENABLE ROW MOVEMENT;

-- Thực hiện flashback để khôi phục dữ liệu về trạng thái trước khi thực hiện INSERT
FLASHBACK TABLE hospital_admin.HSBA TO TIMESTAMP (SYSTIMESTAMP - INTERVAL '5' MINUTE);
