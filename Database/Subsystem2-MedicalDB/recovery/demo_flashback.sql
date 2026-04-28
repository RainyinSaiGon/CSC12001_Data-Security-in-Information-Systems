-- demo_flashback.sql
-- Demo Flashback Database trên bảng BENHNHAN
-- Chạy từng bước theo hướng dẫn bên dưới

-- 1. Kiểm tra Flashback đã bật chưa
SELECT flashback_on FROM v$database;

-- 2. Tạo restore point (mốc phục hồi)
CREATE RESTORE POINT before_flashback_demo GUARANTEE FLASHBACK DATABASE;

-- 3. Xem dữ liệu gốc
SELECT * FROM BENHNHAN;

-- 4. Thực hiện thao tác gây lỗi (xóa dữ liệu)
DELETE FROM BENHNHAN;
COMMIT;

-- 5. Kiểm tra dữ liệu đã bị xóa
SELECT * FROM BENHNHAN;

-- 6. Đưa database về trạng thái MOUNT (chạy ngoài SQL*Plus/SQLcl):
-- SHUTDOWN IMMEDIATE;
-- STARTUP MOUNT;

-- 7. Thực hiện flashback về restore point
FLASHBACK DATABASE TO RESTORE POINT before_flashback_demo;

-- 8. Mở lại database
ALTER DATABASE OPEN RESETLOGS;

-- 9. Kiểm tra dữ liệu đã phục hồi
SELECT * FROM BENHNHAN;

-- 10. Xóa restore point nếu không cần nữa
DROP RESTORE POINT before_flashback_demo;
