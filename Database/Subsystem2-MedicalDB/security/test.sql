-- Check NHANVIEN USERNAME column
SELECT 
    COUNT(*) AS total_staff,
    COUNT(USERNAME) AS staff_with_username,
    COUNT(*) - COUNT(USERNAME) AS staff_without_username
FROM NHANVIEN;

-- Check BENHNHAN USERNAME column
SELECT 
    COUNT(*) AS total_patients,
    COUNT(USERNAME) AS patients_with_username,
    COUNT(*) - COUNT(USERNAME) AS patients_without_username
FROM BENHNHAN;

-- Sample NHANVIEN data
SELECT MANV, HOTEN, VAITRO, USERNAME
FROM NHANVIEN
WHERE ROWNUM <= 10
ORDER BY MANV;

-- Sample BENHNHAN data
SELECT MABN, TENBN, USERNAME
FROM BENHNHAN
WHERE ROWNUM <= 10
ORDER BY MABN;