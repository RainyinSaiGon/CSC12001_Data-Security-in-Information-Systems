-- ReadAuditLogs.sql
-- Sample queries to read and analyze Oracle audit logs
-- Part of CSC12001 Data Security in Information Systems

-- ============================================
-- 1. VIEW ALL AUDITED ACTIONS
-- ============================================
-- General audit trail for all actions by PROJECT_ADMIN user
SELECT 
    username, 
    action_name, 
    TO_CHAR(timestamp#, 'YYYY-MM-DD HH24:MI:SS') as action_time, 
    returncode,
    obj_name,
    new_owner
FROM dba_audit_trail 
WHERE owner='PROJECT_ADMIN' 
ORDER BY timestamp# DESC;


-- ============================================
-- 2. FIND FAILED LOGIN ATTEMPTS
-- ============================================
-- Identify failed authentication attempts (action codes 1, 3, 13)
-- returncode != 0 indicates failure
SELECT 
    username, 
    action_name, 
    TO_CHAR(timestamp#, 'YYYY-MM-DD HH24:MI:SS') as failed_time, 
    returncode,
    terminal,
    priv_used
FROM dba_audit_trail 
WHERE action IN (1, 3, 13) 
    AND returncode != 0
ORDER BY timestamp# DESC;


-- ============================================
-- 3. FIND PRIVILEGE GRANTS/REVOKES
-- ============================================
-- Track all privilege grant and revoke operations
-- Action codes: 14-20 relate to system privilege and role changes
SELECT 
    username as granted_by, 
    action_name, 
    new_owner as grantee,
    obj_name as privilege_or_role,
    TO_CHAR(timestamp#, 'YYYY-MM-DD HH24:MI:SS') as grant_time,
    returncode
FROM dba_audit_trail
WHERE action IN (14, 15, 16, 17, 18, 19, 20)
ORDER BY timestamp# DESC;


-- ============================================
-- 4. FIND DATA MODIFICATIONS (DML)
-- ============================================
-- Track INSERT, UPDATE, DELETE, SELECT operations
-- Action codes: 2 (INSERT), 3 (DELETE), 6 (UPDATE), etc.
SELECT 
    username, 
    action_name, 
    obj_name as table_name,
    TO_CHAR(timestamp#, 'YYYY-MM-DD HH24:MI:SS') as modification_time,
    ses_actions,
    returncode
FROM dba_audit_trail
WHERE action IN (2, 3, 6, 7, 8, 9, 11, 12)
    AND obj_name IN ('BENHNHAN', 'NHANVIEN', 'HSBA', 'DONTHUOC')
ORDER BY timestamp# DESC;


-- ============================================
-- 5. AUDIT TRAIL FOR SPECIFIC TABLE
-- ============================================
-- Get all audit records for a specific table (e.g., BENHNHAN - Patient records)
SELECT 
    username, 
    action_name, 
    TO_CHAR(timestamp#, 'YYYY-MM-DD HH24:MI:SS') as action_time,
    obj_name,
    priv_used,
    returncode
FROM dba_audit_trail
WHERE obj_name = 'BENHNHAN'
ORDER BY timestamp# DESC;


-- ============================================
-- 6. AUDIT COUNTS BY USER (LAST 7 DAYS)
-- ============================================
-- Summary of audit events by user in the last 7 days
SELECT 
    username,
    COUNT(*) as total_actions,
    SUM(CASE WHEN returncode != 0 THEN 1 ELSE 0 END) as failed_actions,
    MAX(timestamp#) as last_action_time
FROM dba_audit_trail
WHERE timestamp# >= TRUNC(SYSDATE) - 7
GROUP BY username
ORDER BY total_actions DESC;


-- ============================================
-- 7. AUDIT COUNTS BY ACTION TYPE (LAST 7 DAYS)
-- ============================================
-- Summary of audit events by action type
SELECT 
    action_name,
    COUNT(*) as total_count,
    SUM(CASE WHEN returncode != 0 THEN 1 ELSE 0 END) as failed_count
FROM dba_audit_trail
WHERE timestamp# >= TRUNC(SYSDATE) - 7
GROUP BY action_name
ORDER BY total_count DESC;


-- ============================================
-- 8. SESSION AUDIT TRAIL
-- ============================================
-- View all actions in a specific session
SELECT 
    username,
    sessionid,
    action_name,
    TO_CHAR(timestamp#, 'YYYY-MM-DD HH24:MI:SS') as action_time,
    obj_name,
    returncode
FROM dba_audit_trail
WHERE sessionid = &session_id
ORDER BY timestamp# ASC;


-- ============================================
-- 9. FINE-GRAINED AUDIT (FGA) EVENTS
-- ============================================
-- View fine-grained audit trail for column-level tracking
-- This table contains detailed audit logs for sensitive columns
SELECT 
    dbusername as db_user,
    event_timestamp,
    event_type,
    object_schema,
    object_name,
    policy_name,
    sql_text,
    return_code
FROM fga_log$
ORDER BY event_timestamp DESC;


-- ============================================
-- 10. AUDIT STATISTICS (LAST 30 DAYS)
-- ============================================
-- Overall audit statistics for the last month
SELECT 
    TO_CHAR(TRUNC(timestamp#), 'YYYY-MM-DD') as audit_date,
    COUNT(*) as total_audit_events,
    COUNT(DISTINCT username) as unique_users,
    SUM(CASE WHEN returncode != 0 THEN 1 ELSE 0 END) as failed_attempts
FROM dba_audit_trail
WHERE timestamp# >= TRUNC(SYSDATE) - 30
GROUP BY TRUNC(timestamp#)
ORDER BY audit_date DESC;


-- ============================================
-- 11. EMERGENCY: SUSPICIOUS ACTIVITY CHECK
-- ============================================
-- Identify potential security incidents
SELECT 
    username,
    COUNT(*) as failed_attempts,
    MAX(timestamp#) as last_attempt_time,
    MIN(timestamp#) as first_attempt_time
FROM dba_audit_trail
WHERE returncode != 0 
    AND timestamp# >= TRUNC(SYSDATE) - 1  -- Last 24 hours
GROUP BY username
HAVING COUNT(*) > 5  -- More than 5 failures in 24 hours
ORDER BY failed_attempts DESC;


-- ============================================
-- 12. PRIVILEGED USER ACTIONS
-- ============================================
-- Monitor actions by privileged accounts (DBA, SYSTEM)
SELECT 
    username,
    action_name,
    obj_name,
    TO_CHAR(timestamp#, 'YYYY-MM-DD HH24:MI:SS') as action_time,
    priv_used,
    sess_actions
FROM dba_audit_trail
WHERE username IN ('SYS', 'SYSTEM', 'PROJECT_ADMIN')
    AND timestamp# >= TRUNC(SYSDATE) - 1
ORDER BY timestamp# DESC;


-- ============================================
-- NOTES
-- ============================================
-- To run these queries:
-- 1. Connect as a user with AUDIT_ADMIN or DBA role
-- 2. Ensure the AUD$ table exists (standard Oracle installation)
-- 3. For FGA queries, ensure fine-grained audit has been enabled
-- 4. Adjust WHERE clause conditions as needed for your audit period
--
-- Common Return Codes:
-- 0 = Success
-- 1004 = User already exists
-- 1005 = Permission already granted
-- Other non-zero = Failure/Error
