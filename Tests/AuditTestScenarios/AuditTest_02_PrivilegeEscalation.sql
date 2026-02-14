-- =============================================================================
-- AuditTest_02: Privilege Escalation Attempt
-- =============================================================================
-- Scenario: TECHNICIAN001 attempts to write to the HSBA (medical records) table,
--           which is outside their role permissions.
-- Expected: Action denied and logged in audit trail.
-- Related: Req 3 - Audit & Monitoring, task-08 Scenario 2
-- =============================================================================

-- =====================
-- STEP 1: Attempt privilege escalation
-- =====================
CONNECT TECHNICIAN001/[password]@XE;

-- Attempt to INSERT into HSBA (unauthorized for Technician role)
INSERT INTO HSBA (MAHSBA, MABN, MABS, NGAY, CHANDOAN)
VALUES ('FAKE_HSBA', 'BN001', 'BS001', SYSDATE, 'Unauthorized Entry');
-- Expected: ORA-01031 insufficient privileges or ORA-00942

-- Attempt to UPDATE HSBA (also unauthorized)
UPDATE HSBA SET CHANDOAN = 'Tampered Diagnosis' WHERE MAHSBA = 'HSBA001';
-- Expected: Denied

-- Attempt to DELETE from HSBA
DELETE FROM HSBA WHERE MAHSBA = 'HSBA001';
-- Expected: Denied

-- Attempt to GRANT role to self (extreme escalation)
GRANT 'Bác sĩ/Y sĩ' TO TECHNICIAN001;
-- Expected: ORA-01031 insufficient privileges

-- =====================
-- STEP 2: Verify audit trail captures the escalation attempts
-- =====================
CONNECT SYS/[password]@XE AS SYSDBA;

-- Check for failed operations by TECHNICIAN001
SELECT
    username,
    action_name,
    obj_name,
    returncode,
    TO_CHAR(timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time,
    priv_used,
    sql_text
FROM dba_audit_trail
WHERE username = 'TECHNICIAN001'
  AND returncode != 0
ORDER BY timestamp DESC
FETCH FIRST 20 ROWS ONLY;

-- =====================
-- STEP 3: Verify unified audit for privilege violations
-- =====================
SELECT
    dbusername,
    action_name,
    object_name,
    return_code,
    sql_text,
    TO_CHAR(event_timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time
FROM unified_audit_trail
WHERE dbusername = 'TECHNICIAN001'
  AND return_code != 0
ORDER BY event_timestamp DESC
FETCH FIRST 20 ROWS ONLY;

-- =====================
-- PASS CRITERIA:
-- =====================
-- [ ] All 4 escalation attempts are denied
-- [ ] Each denial is recorded in audit trail
-- [ ] Audit record includes: username, action, object, return code, SQL text
-- [ ] GRANT attempt specifically captured as privilege violation
