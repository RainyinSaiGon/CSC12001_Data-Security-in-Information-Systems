-- =============================================================================
-- AuditTest_01: Unauthorized Access Attempt
-- =============================================================================
-- Scenario: An invalid/unauthorized user attempts to access the BENHNHAN table.
-- Expected: Access denied and the attempt is logged in the audit trail.
-- Related: Req 3 - Audit & Monitoring, task-08 Scenario 1
-- =============================================================================

-- =====================
-- STEP 1: Attempt unauthorized access
-- =====================
-- Connect as a user without BENHNHAN access (or use invalid credentials)
-- Option A: Use a user with no privileges
CONNECT UNAUTHORIZED_USER/test_password@XE;

-- Attempt to query patient table
SELECT * FROM HOSPITAL_ADMIN.BENHNHAN;
-- Expected: ORA-00942 table or view does not exist
--       OR: ORA-01017 invalid username/password (if user doesn't exist)

-- =====================
-- STEP 2: Verify audit trail captures the denial
-- =====================
-- Connect as DBA to check audit logs
CONNECT SYS/[password]@XE AS SYSDBA;

-- Check standard audit trail for failed access
SELECT
    username,
    action_name,
    obj_name,
    returncode,
    TO_CHAR(timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time,
    os_username,
    terminal
FROM dba_audit_trail
WHERE returncode != 0
  AND obj_name = 'BENHNHAN'
ORDER BY timestamp DESC
FETCH FIRST 10 ROWS ONLY;

-- =====================
-- STEP 3: Verify unified audit trail (if unified audit enabled)
-- =====================
SELECT
    dbusername,
    action_name,
    object_name,
    return_code,
    TO_CHAR(event_timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time,
    authentication_type
FROM unified_audit_trail
WHERE return_code != 0
  AND object_name = 'BENHNHAN'
ORDER BY event_timestamp DESC
FETCH FIRST 10 ROWS ONLY;

-- =====================
-- PASS CRITERIA:
-- =====================
-- [ ] Unauthorized access attempt is denied
-- [ ] Audit trail contains a record of the denial
-- [ ] Record includes: username, timestamp, object name, return code
-- [ ] OS username and terminal info captured
