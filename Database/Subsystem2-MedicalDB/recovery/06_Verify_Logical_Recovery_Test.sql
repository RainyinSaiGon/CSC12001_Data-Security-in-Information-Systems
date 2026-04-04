-- Run as SYS in XEPDB1 after restoring into HOSPITAL_ADMIN.

SELECT object_type, COUNT(*) object_count
FROM dba_objects
WHERE owner = 'HOSPITAL_ADMIN'
GROUP BY object_type
ORDER BY object_type;

SELECT table_name
FROM dba_tables
WHERE owner = 'HOSPITAL_ADMIN'
  AND table_name IN (
    'NHANVIEN',
    'BENHNHAN',
    'HSBA',
    'HSBA_DV',
    'DONTHUOC',
    'THONGBAO',
      'KHOA',
      'RECOVERY_ROWCOUNT_SNAPSHOT'
  )
ORDER BY table_name;

SELECT s.table_name,
       s.source_rows,
       t.num_rows recovered_rows,
       CASE WHEN s.source_rows = t.num_rows THEN 'MATCH' ELSE 'DIFF' END comparison_result,
       s.snapshot_time
FROM HOSPITAL_ADMIN.RECOVERY_ROWCOUNT_SNAPSHOT s
JOIN dba_tables t
  ON t.owner = 'HOSPITAL_ADMIN'
 AND t.table_name = s.table_name
WHERE s.table_name IN (
    'NHANVIEN',
    'BENHNHAN',
    'HSBA',
    'HSBA_DV',
    'DONTHUOC',
    'THONGBAO',
    'KHOA'
  )
ORDER BY s.table_name;

-- Note: NUM_ROWS may be stale if statistics are not refreshed.
