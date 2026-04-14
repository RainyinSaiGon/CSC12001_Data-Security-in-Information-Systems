-- Đóng PDB trước khi phục hồi
ALTER PLUGGABLE DATABASE XEPDB1 CLOSE IMMEDIATE;

-- RMAN script (lưu thành file: pitr_recover_rman.rman)
-- Chạy trong RMAN:
-- rman target /
-- RMAN> @pitr_recover_rman.rman

RUN {
  SET UNTIL TIME "TO_DATE('2026-04-14 21:06:00', 'YYYY-MM-DD HH24:MI:SS')";
  RESTORE PLUGGABLE DATABASE XEPDB1;
  RECOVER PLUGGABLE DATABASE XEPDB1;
}

-- Sau khi recover xong, mở lại PDB
ALTER PLUGGABLE DATABASE XEPDB1 OPEN RESETLOGS;