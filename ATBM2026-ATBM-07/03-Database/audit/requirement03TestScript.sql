SET DEFINE OFF;
SET LINESIZE 300;
SET PAGESIZE 100;
COL USERNAME FORMAT A15;
COL OBJECT_NAME FORMAT A15;
COL ACTION_NAME FORMAT A15;
COL POLICY_NAME FORMAT A30;
COL SQL_TEXT FORMAT A60;

PROMPT =====================================================================
PROMPT THUC HIEN CAC HANH VI DE TAO LOG (STANDARD AUDIT & FGA)
PROMPT =====================================================================

PROMPT ---> KET NOI: KY THUAT VIEN (990000000121)
CONNECT "990000000121"/990000000121@localhost:1521/XEPDB1;

PROMPT [Checklist] Failed SELECT on BENHNHAN
-- Kỹ thuật viên không có quyền SELECT toàn bộ bảng BENHNHAN
SELECT * FROM HOSPITAL_ADMIN.BENHNHAN FETCH FIRST 5 ROWS ONLY;
-- Ky vong: Loi ORA-01031 (Insufficient privileges)


PROMPT ---> KET NOI: BENH NHAN (000000000001)
CONNECT "000000000001"/000000000001@localhost:1521/XEPDB1;

PROMPT [Checklist] Failed DML on HSBA_DV (Standard) & Illegal HSBA_DV DML (FGA)
-- Bệnh nhân cố tình xóa và sửa kết quả dịch vụ trái phép
DELETE FROM HOSPITAL_ADMIN.HSBA_DV WHERE MAHSBA = 1;
UPDATE HOSPITAL_ADMIN.HSBA_DV SET KETQUA = 'Âm tính giả' WHERE MAHSBA = 1;
-- Ky vong: Loi ORA-01031


PROMPT ---> KET NOI: BAC SI 21 (990000000021)
CONNECT "990000000021"/990000000021@localhost:1521/XEPDB1;

PROMPT [Checklist] Successful login/logout
-- Lệnh CONNECT phía trên và phía dưới sẽ tự sinh Audit log Session (LOGON/LOGOFF).

PROMPT [Checklist] VPD function execution
-- Gọi lệnh SELECT sẽ kích hoạt chính sách VPD_HSBA_FN bên dưới
SELECT * FROM HOSPITAL_ADMIN.HSBA FETCH FIRST 5 ROWS ONLY;

PROMPT [Checklist] DML on HSBA (Standard) & Valid doctor update on HSBA (FGA)
-- Sửa lại: Lấy MABS (GUID) thông qua V_SELF_NHANVIEN
UPDATE HOSPITAL_ADMIN.HSBA 
SET CHANDOAN = 'Viêm họng hạt cấp', KETLUAN = 'Tái khám sau 1 tuần' 
WHERE MABS = (SELECT MANV FROM HOSPITAL_ADMIN.V_SELF_NHANVIEN);
COMMIT;

PROMPT [Checklist] DML on DONTHUOC (Standard) & Update DONTHUOC after creation (FGA)
-- Sửa lại: Update đơn thuốc thuộc hồ sơ do chính Bác sĩ này phụ trách
UPDATE HOSPITAL_ADMIN.DONTHUOC 
SET LIEUDUNG = 'Sáng 2 viên, tối 1 viên (cập nhật mới)' 
WHERE MAHSBA IN (SELECT MAHSBA FROM HOSPITAL_ADMIN.HSBA WHERE MABS = (SELECT MANV FROM HOSPITAL_ADMIN.V_SELF_NHANVIEN));
COMMIT;

PROMPT [Checklist] Invalid doctor update on HSBA (FGA)
-- Bác sĩ cố tình sửa thông tin bệnh án của Bác sĩ 22
UPDATE HOSPITAL_ADMIN.HSBA 
SET CHANDOAN = 'Hack bệnh án của bác sĩ khác' 
WHERE MABS = '990000000022';
-- Ky vong: 0 rows updated (bị che bởi VPD), nhưng câu lệnh FGA sẽ bắt lại được nguyên văn!

PROMPT =====================================================================
PROMPT THUC THI XEM NHAT KY KIEM TOAN (03_ReadAuditLogs.sql)
PROMPT =====================================================================
PROMPT ---> KET NOI: ADMIN (HOSPITAL_ADMIN)
CONNECT "HOSPITAL_ADMIN"/12345678@localhost:1521/XEPDB1;

@03_ReadAuditLogs.sql
