# Task 08: Database Audit Setup - Standard, Fine-Grained, Unified Audit

**Assigned to:** Ngọc, Vũ (Part B)  
**Type:** Database Administration  
**Duration:** 7 hours  
**Priority:** Medium-High  
**Timeline:** Feb 21 - Feb 28, 2026

---

## Overview

Implement comprehensive audit logging to track all database operations for compliance, security monitoring, and troubleshooting:

- Standard Oracle auditing for basic operations
- Fine-grained auditing (FGA) for sensitive data
- Unified auditing for comprehensive tracking
- Audit log query utilities for reports

## Deliverables

### 08_StandardAudit_Setup.sql

Enable Oracle standard auditing:

**Audit Statements:**

```sql
-- Log all user activity
AUDIT ALL STATEMENTS BY users;

-- Log login/logout
AUDIT CONNECT;
AUDIT DISCONNECT;

-- Log sensitive table operations
AUDIT SELECT, INSERT, UPDATE, DELETE ON HSBA;
AUDIT INSERT, UPDATE, DELETE ON BENHNHAN;
AUDIT INSERT, UPDATE, DELETE ON DONTHUOC;

-- Log privilege changes
AUDIT GRANT ON SYSTEM;
```

**Features:**

- Records IN the DBA_AUDIT_TRAIL view
- Captures: user, timestamp, action type, SQL text
- Useful for compliance baseline auditing
- Storage: SYS.AUD$ table
- Regular maintenance: archive and purge old records

**Configuration:**

- Set AUDIT_TRAIL = DB (audit to database)
- Alternative: AUDIT_TRAIL = OS (audit to OS files)
- Restart database after enabling

### 09_FineGrainedAudit_Setup.sql

Implement fine-grained auditing for sensitive columns:

**FGA Policies:**

Policy on HSBA (Medical Records):

```
Table: HSBA
Monitored Operations: INSERT, UPDATE, DELETE
Audit: All operations on medical records
Alert: If someone deletes a record
```

Using DBMS_FGA.ADD_POLICY:

- Policy name: FGA_HSBA_POLICY
- Object: HSBA table
- Audit condition: None (always audit)
- Enable: TRUE
- Log to: FGA_LOG$ table

Policy on DONTHUOC (Prescriptions):

```
Table: DONTHUOC
Monitored Operations: All (SELECT, INSERT, UPDATE, DELETE)
Audit: Complete prescription management
Column Focus: TENTHUOC (drug name), LIEUDUNG (dosage)
```

Policy on BENHNHAN - Sensitive Columns:

```
Table: BENHNHAN
Monitored Operations: SELECT (CCCD = ID #), UPDATE, DELETE
Columns: CCCD (National ID), Phone, Address
Monitor SELECT: Track who views sensitive patient data
```

**Implementation:**

```
-- Add each policy with DBMS_FGA.ADD_POLICY:
DBMS_FGA.ADD_POLICY(
  object_schema => 'PROJECT_ADMIN',
  object_name => 'HSBA',
  policy_name => 'FGA_HSBA_POLICY',
  audit_condition => NULL,  -- Always audit
  audit_column => NULL,
  handler_schema => NULL,
  handler_module => NULL,
  enable => TRUE,
  statement_types => 'INSERT,UPDATE,DELETE'
);
-- Repeat for DONTHUOC and BENHNHAN
```

### 10_UnifiedAudit_Setup.sql

Implement modern Unified Auditing:

**Purpose:** Provide integrated auditing across all security mechanisms

**Audit Policies:**

Policy 1: RBAC Violations Audit

```
CREATE AUDIT POLICY rbac_violations_policy
  ACTIONS:
    - ROLE granted/revoked
    - System privilege granted/revoked
  Audit When: Non-privileged user attempts unauthorized action
  Target: All users
```

Policy 2: VPD Policy Violations

```
CREATE AUDIT POLICY vpd_violations_policy
  ACTIONS:
    - Query on HSBA (VPD-protected)
    - Query on HSBA_DV (VPD-protected)
  Audit When: Violation occurs
  Target: All users
```

Policy 3: Sensitive Data Access

```
CREATE AUDIT POLICY sensitive_data_policy
  ACTIONS:
    - SELECT on HSBA (medical records)
    - SELECT on DONTHUOC (prescriptions)
    - SELECT CCCD on BENHNHAN (patient IDs)
  Audit When: Access occurs
  Extended Audit: Log exact columns accessed
```

**Implementation:**

```
CREATE AUDIT POLICY [policy_name] ...
-- Define what to audit
-- Specify conditions
-- Enable policy
AUDIT POLICY [policy_name];
-- Results stored in UNIFIED_AUDIT_TRAIL
```

### 11_ReadAuditLogs.sql

Provide queries for audit log analysis:

**Query 1: All activities by user (date range)**

```sql
SELECT * FROM DBA_AUDIT_TRAIL
WHERE USERNAME = 'user_name'
  AND TIMESTAMP >= to_date('2026-02-10', 'yyyy-mm-dd')
  AND TIMESTAMP <= to_date('2026-02-28', 'yyyy-mm-dd')
ORDER BY TIMESTAMP DESC;
```

**Query 2: Sensitive data access (FGA)**

```sql
SELECT * FROM FGA_LOG$
WHERE DB_USER IN ('user_name')
  AND TIMESTAMP >= to_date('2026-02-10', 'yyyy-mm-dd')
ORDER BY TIMESTAMP DESC;
```

**Query 3: Failed login attempts**

```sql
SELECT * FROM DBA_AUDIT_TRAIL
WHERE RETURNCODE != 0  -- Non-zero = failure
  AND PRIV_USED = 'CREATE SESSION'
ORDER BY TIMESTAMP DESC;
```

**Query 4: Data modifications (INSERT/UPDATE/DELETE)**

```sql
SELECT USERNAME, ACTION_NAME, OBJ_NAME, TIMESTAMP
FROM DBA_AUDIT_TRAIL
WHERE ACTION_NAME IN ('INSERT', 'UPDATE', 'DELETE')
  AND OBJ_NAME IN ('BENHNHAN', 'HSBA', 'DONTHUOC')
ORDER BY TIMESTAMP DESC;
```

**Query 5: Privilege grants/revokes**

```sql
SELECT USERNAME, ACTION_NAME, PRIV_USED, TIMESTAMP
FROM DBA_AUDIT_TRAIL
WHERE ACTION_NAME IN ('GRANT ROLE', 'GRANT SYSTEM')
ORDER BY TIMESTAMP DESC;
```

**Query 6: Unified Audit Trail (comprehensive)**

```sql
SELECT USERID, ACTION_NAME, OBJ_NAME, SQL_TEXT, EVENT_TIMESTAMP
FROM UNIFIED_AUDIT_TRAIL
WHERE EVENT_TIMESTAMP >= trunc(sysdate)
ORDER BY EVENT_TIMESTAMP DESC;
```

### 12_BackupAndRecovery_Documentation.md

Document backup and recovery procedures:

**Content Required:**

Backup Strategy 1: RMAN (Recovery Manager)

- Full backup procedures
- Incremental backup strategy
- Retention policies (e.g., keep 30 days)
- Recovery procedures
- Testing recovery process

Backup Strategy 2: Export/Data Pump

- Schema export: expdp command
- Data export procedures
- Full dump file export
- Import procedures
- Selective object recovery

Disaster Recovery

- Point-in-time recovery steps
- Full database recovery from backup
- Selective object recovery
- Recovery time objectives (RTO): < 4 hours for full recovery
- Recovery point objectives (RPO): < 1 hour data loss acceptable

Testing & Validation

- Procedures to test backup completeness
- How to verify recovery works
- Test schedule (monthly minimum)
- Documentation of test results

Maintenance

- Regular backup verification
- Log file archival
- Backup media management
- Monitoring backup jobs

## Dependencies

- **Requires:** Task 06 tables completed
- **Requires:** Task 07 security setup
- **Complements:** Task 05 AuditService (uses audit tables)
- **Optional for:** Week 4 compliance testing

## Success Criteria

✓ Standard auditing captures basic operations  
✓ FGA logs sensitive data access attempts  
✓ Unified audit captures all important events  
✓ Audit log queries work and return data  
✓ AuditService can read logs effectively  
✓ Backup procedures documented  
✓ Audit data persists correctly  
✓ No performance impact from auditing

## Storage and Maintenance

**Audit Table Sizes:**

- DBA_AUDIT_TRAIL grows ~10-50 MB/day depending on activity
- FGA_LOG$ smaller but critical for compliance
- UNIFIED_AUDIT_TRAIL comprehensive
- Plan for: 50-100 GB/year storage

**Purge Old Records:**

```sql
-- Example: purge audit older than 90 days
DELETE FROM AUD$ WHERE TIMESTAMP < sysdate - 90;
COMMIT;
```

**Archival:**

- Export old records to files monthly
- Keep 1 year in database
- Archive older records to secure storage
- Restore if needed for investigation

## Compliance Notes

- GDPR: Medical records access must be traceable
- HIPAA: Requires comprehensive audit trail
- Local regulations: Medical data highly sensitive
- Non-repudiation: Auditing proves who did what

## Timeline

- **Fri Feb 21:** Basic auditing (Tasks 08, 09, 10)
- **Mon Feb 24:** Verify audit logs working
- **Fri Feb 28:** Backup documentation complete
- **Week 4:** Backup/recovery testing

## Traceability Matrix

### AUD#1: Standard Audit Configuration

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 3: Audit & Monitoring |
| **Test Timeline** | End of Week 2 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `08_Standard_Audit.sql` — Standard audit policies | Required | Week 2 |
| `09_FGA_Setup.sql` — Fine-Grained Audit policies | Required | Week 2 |

**Pass Criteria:**

- ✓ Standard audit enabled for CREATE USER, DROP USER, ALTER USER
- ✓ Standard audit enabled for GRANT, REVOKE operations
- ✓ Audit trail records include: username, timestamp, action, object
- ✓ DBA_AUDIT_TRAIL populated after test operations
- ✓ Audit retention policy configured (90 days minimum)

---

### AUD#2: Fine-Grained Audit (FGA)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 3: Audit & Monitoring |
| **Test Timeline** | End of Week 2 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `09_FGA_Setup.sql` — FGA policies for sensitive data | Required | Week 2 |

**Pass Criteria:**

- ✓ FGA policy on BENHNHAN: logs access to CCCD (national ID), patient details
- ✓ FGA policy on HSBA: logs access to diagnosis and treatment details
- ✓ FGA policy on DONTH: logs access to prescriptions
- ✓ DBA_FGA_AUDIT_TRAIL populated after SELECT on sensitive columns
- ✓ FGA handler sends notification on suspicious access patterns
- ✓ FGA overhead < 5% on normal query performance

**Evidence Tracking:**

- Query DBA_FGA_AUDIT_TRAIL after test SELECT operations
- Performance comparison (FGA enabled vs disabled)

---

### BAK#1: RMAN Backup Configuration

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 4: Backup & Recovery |
| **Test Timeline** | End of Week 3 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `10_Backup_Config.sql` — RMAN backup scripts | Required | Week 3 |
| Backup documentation | Required | Week 3 |

**Pass Criteria:**

- ✓ Full backup script executes without errors
- ✓ Incremental backup script executes without errors
- ✓ Backup verification (RMAN VALIDATE) passes
- ✓ Backup catalog updated with latest backup information
- ✓ Backup retention period configured (7 days minimum)

---

### BAK#2: Recovery Testing

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 4: Backup & Recovery |
| **Test Timeline** | End of Week 3 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| Recovery test documentation | Required | Week 3 |
| Point-in-time recovery demonstration | Required | Week 3 |

**Pass Criteria:**

- ✓ Full database restore from backup completes successfully
- ✓ Point-in-time recovery (PITR) restores to specific timestamp
- ✓ Tablespace recovery isolates and restores individual tablespace
- ✓ All data verified after recovery (row counts match)
- ✓ Recovery time documented (RTO metrics)

**Evidence Tracking:**

- RMAN restore/recover log output
- Before/after row count comparison
- RTO measurement documentation

---

## Audit Query Samples

### DBA_AUDIT_TRAIL Views

```sql
-- View all audited actions
SELECT username, action_name, timestamp#, returncode 
FROM dba_audit_trail 
WHERE owner='PROJECT_ADMIN' 
ORDER BY timestamp# DESC;

-- Find failed login attempts
SELECT username, action_name, timestamp#, returncode 
FROM dba_audit_trail 
WHERE action IN (1,3,13) AND returncode != 0;

-- Find privilege grants
SELECT username, action_name, new_owner, objectname, timestamp#
FROM dba_audit_trail
WHERE action IN (14, 15, 16, 17, 18, 19, 20);

-- Find data modifications
SELECT username, action_name, timestamp#
FROM dba_audit_trail
WHERE action IN (2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
```

### Fine-Grained Audit Log Example

```text
Audit Type: DML on PRESCRIPTION
Timestamp: 2026-02-09 14:30:45
User: DOCTOR001
Action: UPDATE
Columns Modified: 
  - LIEUDUNG: 'Previous Value' -> 'New Value'
Table: DONTHUOC
Record ID: 12345
Success: Y
```

---

## Audit Test Scenarios

### Scenario 1: Unauthorized Access Attempt

- **User:** Invalid_User
- **Target:** BENHNHAN table
- **Result:** Access Denied (logged)

### Scenario 2: Privilege Escalation Attempt

- **User:** TECHNICIAN001
- **Attempted Action:** Write to HSBA table
- **Result:** Denied (logged)

### Scenario 3: Data Modification by Authorized User

- **User:** DOCTOR001
- **Table:** HSBA
- **Columns Modified:** CHANDOAN, DIEUTRI
- **Result:** Success (logged with details)

### Scenario 4: Prescription Update

- **User:** DOCTOR001
- **Table:** DONTHUOC
- **Columns Modified:** LIEUDUNG
- **Result:** Success (logged)

### Scenario 5: Patient Data Access

- **User:** PATIENT001
- **Query:** Own medical records
- **Result:** Success (logged)

---

## Audit Analysis Template

**Date:** ________  
**Period:** ________ to ________

### Summary

- Total Audit Events: ____
- Failed Authentication: ____
- Privilege Changes: ____
- Data Modifications: ____

### Suspicious Activities

1. [Activity Description]
2. [Activity Description]

### Compliance Status

- [ ] All audit events logged
- [ ] No gaps in audit trail
- [ ] Timestamps accurate
- [ ] User identification complete

---

## Audit Infrastructure

### Log File Locations

| Location | Purpose |
|----------|---------|
| `SYS.AUD$` table | Standard audit trail |
| `FGA_LOG$` table | Fine-grained audit trail |
| `$ORACLE_BASE/diag/rdbms/[db_name]/alert_[db_name].log` | Alert logs (errors, warnings) |

### Retention Policy

| Parameter | Value |
|-----------|-------|
| Retention Period | 1 year |
| Archive Frequency | Monthly |
| Backup | Weekly to tape |
| Purge Policy | Delete after 1 year retention |

### Dashboard Metrics (to populate during testing)

```text
Last 24 Hours:
- Login Attempts: __
- Failed Logins: __
- DML Operations: __
- DDL Operations: __
- Access Violations: __

Last 7 Days:
- Total Audit Events: __
- Unique Users: __
- Tables Modified: __
- Security Incidents: __
```

---

## Related Tasks

- Task 06: Provides audit table
- Task 05: AuditService reads these logs
- All other tasks: Audited by these mechanisms

---
