-- =============================================================================
-- AuditTest_03: Data Modification by Authorized User
-- =============================================================================
-- Scenario: DOCTOR001 modifies CHANDOAN (diagnosis) and DIEUTRI (treatment)
--           columns in HSBA table — an authorized action.
-- Expected: Modification succeeds and Fine-Grained Audit captures the details.
-- Related: Req 3 - Audit & Monitoring, task-08 Scenario 3
-- =============================================================================

-- =====================
-- STEP 1: Record baseline state
-- =====================
CONNECT SYS/[password]@XE AS SYSDBA;

-- Get current count of FGA records
SELECT COUNT(*) AS fga_count_before FROM fga_log$;

-- =====================
-- STEP 2: Perform authorized modification
-- =====================
CONNECT DOCTOR001/[password]@XE;

-- Update diagnosis (authorized for Doctor role)
UPDATE HSBA
SET CHANDOAN = N'Viem phoi cap tinh',
    DIEUTRI  = N'Khang sinh + nghi ngoi'
WHERE MAHSBA = (SELECT MIN(MAHSBA) FROM HSBA);  -- Own patient via VPD

COMMIT;

-- Verify the update was applied
SELECT MAHSBA, CHANDOAN, DIEUTRI
FROM HSBA
WHERE CHANDOAN = N'Viem phoi cap tinh';

-- =====================
-- STEP 3: Verify Standard Audit logs the modification
-- =====================
CONNECT SYS/[password]@XE AS SYSDBA;

SELECT
    username,
    action_name,
    obj_name,
    returncode,
    TO_CHAR(timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time
FROM dba_audit_trail
WHERE username = 'DOCTOR001'
  AND obj_name = 'HSBA'
  AND action_name = 'UPDATE'
ORDER BY timestamp DESC
FETCH FIRST 5 ROWS ONLY;

-- =====================
-- STEP 4: Verify Fine-Grained Audit captures column-level details
-- =====================
SELECT
    db_user,
    object_name,
    policy_name,
    sql_text,
    TO_CHAR(timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time
FROM dba_fga_audit_trail
WHERE db_user = 'DOCTOR001'
  AND object_name = 'HSBA'
ORDER BY timestamp DESC
FETCH FIRST 5 ROWS ONLY;

-- Verify FGA count increased
SELECT COUNT(*) AS fga_count_after FROM fga_log$;
-- Should be greater than fga_count_before

-- =====================
-- PASS CRITERIA:
-- =====================
-- [ ] UPDATE succeeds for DOCTOR001 on own patient's HSBA record
-- [ ] Standard audit trail records the UPDATE operation
-- [ ] FGA captures the column-level modifications (CHANDOAN, DIEUTRI)
-- [ ] FGA record includes SQL text showing the exact changes
-- [ ] FGA event count increases after the modification
