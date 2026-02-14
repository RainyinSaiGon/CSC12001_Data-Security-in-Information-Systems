-- =============================================================================
-- AuditTest_05: Patient Data Access
-- =============================================================================
-- Scenario: PATIENT001 queries their own medical records through the system.
-- Expected: Access succeeds (read-only) and is logged in the audit trail.
-- Related: Req 3 - Audit & Monitoring, task-08 Scenario 5
-- =============================================================================

-- =====================
-- STEP 1: Patient queries own medical records
-- =====================
CONNECT PATIENT001/[password]@XE;

-- Query own patient details
SELECT MABN, TENBN, NGAYSINH, SONHA, TENDUONG, QUANHUYEN, TINHTP
FROM BENHNHAN;

-- Query own medical history
SELECT
    h.MAHSBA,
    h.NGAY,
    h.CHANDOAN,
    h.DIEUTRI
FROM HSBA h
ORDER BY h.NGAY DESC;

-- Query own prescriptions
SELECT
    dt.MAHSBA,
    dt.TENTHUOC,
    dt.LIEUDUNG
FROM DONTHUOC dt
JOIN HSBA h ON dt.MAHSBA = h.MAHSBA;

-- Query own diagnostic services
SELECT
    dv.MAHSBA_DV,
    dv.LOAIDV,
    dv.NGAYDV,
    dv.KETQUA
FROM HSBA_DV dv
JOIN HSBA h ON dv.MAHSBA = h.MAHSBA;

-- =====================
-- STEP 2: Verify patient sees only own data (VPD cross-check)
-- =====================
-- All above queries should be scoped to PATIENT001 only
-- Verify row counts are consistent with patient's actual records
SELECT 'BENHNHAN' AS tbl, COUNT(*) AS cnt FROM BENHNHAN
UNION ALL
SELECT 'HSBA', COUNT(*) FROM HSBA
UNION ALL
SELECT 'DONTHUOC', COUNT(*) FROM DONTHUOC dt JOIN HSBA h ON dt.MAHSBA = h.MAHSBA;

-- =====================
-- STEP 3: Verify audit trail captures patient access
-- =====================
CONNECT SYS/[password]@XE AS SYSDBA;

-- Standard audit - all SELECT operations by PATIENT001
SELECT
    username,
    action_name,
    obj_name,
    returncode,
    TO_CHAR(timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time
FROM dba_audit_trail
WHERE username = 'PATIENT001'
  AND action_name = 'SELECT'
ORDER BY timestamp DESC
FETCH FIRST 20 ROWS ONLY;

-- =====================
-- STEP 4: Verify FGA captures sensitive data access
-- =====================
SELECT
    db_user,
    object_name,
    policy_name,
    sql_text,
    TO_CHAR(timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time
FROM dba_fga_audit_trail
WHERE db_user = 'PATIENT001'
ORDER BY timestamp DESC
FETCH FIRST 20 ROWS ONLY;

-- =====================
-- STEP 5: Verify unified audit (comprehensive)
-- =====================
SELECT
    dbusername,
    action_name,
    object_name,
    return_code,
    TO_CHAR(event_timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time,
    sql_text
FROM unified_audit_trail
WHERE dbusername = 'PATIENT001'
ORDER BY event_timestamp DESC
FETCH FIRST 20 ROWS ONLY;

-- =====================
-- PASS CRITERIA:
-- =====================
-- [ ] PATIENT001 successfully reads own medical records
-- [ ] VPD ensures only own data is returned (row count matches actual records)
-- [ ] Standard audit captures all SELECT operations by PATIENT001
-- [ ] FGA captures access to sensitive medical fields
-- [ ] All audit records include: username, timestamp, object, SQL text
-- [ ] Audit trail shows read-only access pattern (no writes)
