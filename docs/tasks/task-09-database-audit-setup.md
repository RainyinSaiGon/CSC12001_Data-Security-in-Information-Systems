# Task 09: Subsystem 2 Database Audit Setup - Standard, Fine-Grained, Unified Audit

**Assigned to:** Ngọc, Vũ (Part B)  
**Type:** Database Administration  
**Duration:** 7 hours  
**Priority:** Medium-High  
**Timeline:** Feb 21 - Feb 28, 2026

---

## Overview

Implement comprehensive audit logging to track all Subsystem 2 database operations for compliance, security monitoring, and troubleshooting per test case requirements:

- **Create AUDITLOG table** (deferred from Task 07) for custom audit trail
- Standard Oracle auditing for basic operations on 7 tables (BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, KHOA, THONGBAO)
- Fine-grained auditing (FGA) for sensitive data access and column-specific updates
- Unified auditing for comprehensive tracking
- Audit log query utilities for reports and compliance

## Audit Logging Requirements by Test Case

**TC#1 - DBA Account Creation:**
- AUDIT: User account creation actions (metadata)

**TC#2 - Coordinator Operations:**
- AUDIT: BENHNHAN INSERT, UPDATE (patient creation/modification)
- AUDIT: HSBA INSERT (new medical record creation)
- AUDIT: Role assignments (MABS, MAKHOA updates)

**TC#3 - Doctor Operations (Column-Specific):**
- AUDIT COLUMNS CRITICAL: HSBA CHANDOAN, DIEUTRI, KETLUAN (diagnosis/treatment/conclusion updates - must record old/new values)
- AUDIT COLUMNS CRITICAL: BENHNHAN TIENSUABENH, TIENSUABENHGD, DIUNGTHUOC (medical history updates - must record old/new values)
- AUDIT COLUMNS CRITICAL: DONTHUOC TENTHUOC, LIEUUNG (prescription changes AFTER creation - must record old/new values)
- AUDIT: HSBA_DV INSERT, DELETE (service ordering)
- AUDIT: SELECT on patient-related tables for compliance trail

**TC#4 - Technician Operations (Column-Specific):**
- AUDIT COLUMN CRITICAL: HSBA_DV KETQUA (service results - must record old/new values)
- AUDIT: SELECT on assigned services

**TC#5 - Patient Operations (Column-Specific + Rejection):**
- AUDIT UPDATE: BENHNHAN contact fields (SODT, SONHA, TENDUONG, QUANHUYEM, TINHTP)
- AUDIT REJECTION: Attempts to modify read-only fields (MABN, TENBN, PHAI, NGAYSINH, CCCD, etc.)
- AUDIT: SELECT on own records for compliance trail

**All Roles:**
- AUDIT: Failed authentication attempts
- AUDIT: Privilege escalation attempts
- AUDIT: VPD/OLS policy violations (if detectable)

### Implementation Guidance: Security Event Auditing

**Failed Authentication Auditing:**
- Application layer: Log all login attempts with username, timestamp, result (success/failure), IP address
- Database level: Enable `AUDIT CONNECT` to track connection attempts
- Log to dedicated AUDIT_LOGIN table (application-side custom logging)
- Capture: UserID, AttemptTime, Result (Success/Failure), AttemptSource, ErrorReason
- Alert threshold: 5+ failed attempts in 15 minutes triggers security alert
- Audit log: Never delete failed authentication attempts

**Privilege Escalation Detection:**
- Monitor role/privilege changes: Compare user's current roles with baseline roles
- Application check: Before any elevated operation, validate user role hasn't changed
- Database audit: `AUDIT GRANT ON SYSTEM` (all privilege grants)
- Log to AUDIT_PRIVILEGE table: GrantorID, GranteeID, PrivilegeGranted, Timestamp
- Cross-check: User requesting operation vs. recorded privileges in NHANVIEN.VAITRO
- Flag: Any operation where user's session role doesn't match NHANVIEN.VAITRO value
- Implementation: ValidateUserPrivileges() in Task 05 Services validates against NHANVIEN

**VPD/OLS Policy Violation Detection:**
- VPD violations: If query returns rows where row predicate should exclude them (application-level check)
- OLS violations: If user accesses THONGBAO row with classification level above their clearance
- Log: All attempted access violations with user, table, record ID, violation reason
- Database-side: FGA policy on THONGBAO SELECT operations to capture classification-level mismatches
- Application-side: AuditService.LogVPDViolation(userId, tableId, rowId, expectedLabel, userLabel)

## Deliverables

### 01_CreateAuditLog_Table.sql

**Create AUDITLOG table for custom audit trail (deferred from Task 07):**

Table Structure:
```sql
CREATE TABLE AUDITLOG (
    AUDITID INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    USERID VARCHAR2(50),          -- User who performed action (matches NHANVIEN.MANV or BENHNHAN.MABN)
    THOIGIAN TIMESTAMP,           -- When action occurred
    LOAIHD VARCHAR2(50),          -- Type of action (INSERT, UPDATE, DELETE, SELECT, etc.)
    TENTABLE VARCHAR2(50),        -- Table affected
    MARECORD VARCHAR2(100),       -- Record identifier (table:id format)
    MABN INT,                     -- Patient ID if applicable
    MANV INT,                     -- Staff ID if applicable
    OLD_VALUES CLOB,              -- Previous values (for updates)
    NEW_VALUES CLOB,              -- New values (for updates/inserts)
    CONSTRAINT FK_AUDIT_BN FOREIGN KEY (MABN) REFERENCES BENHNHAN(MABN),
    CONSTRAINT FK_AUDIT_NV FOREIGN KEY (MANV) REFERENCES NHANVIEN(MANV)
);

CREATE INDEX IDX_AUDIT_USERID ON AUDITLOG(USERID);
CREATE INDEX IDX_AUDIT_THOIGIAN ON AUDITLOG(THOIGIAN);
CREATE INDEX IDX_AUDIT_LOAIHD ON AUDITLOG(LOAIHD);
CREATE INDEX IDX_AUDIT_TENTABLE ON AUDITLOG(TENTABLE);
```

Purpose:
- Store custom application-level audit events
- Track operations across all 7 tables (KHOA, BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO)
- Track operations from all 4 user roles (Điều phối viên, Bác sĩ/Y sĩ, Kỹ thuật viên, Bệnh nhân)
- Complement standard Oracle audit trail
- Support detailed old/new value comparison for compliance
- Store 50,000+ audit entries over 2-week period

### 02_StandardAudit_Setup.sql

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

### 03_FineGrainedAudit_Setup.sql

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

### 04_UnifiedAudit_Setup.sql

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

### 05_ReadAuditLogs.sql

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

### 06_BackupAndRecovery_Documentation.md

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

- **Requires:** Task 07 tables completed (Fri Feb 14 - COMPLETED)
- **Requires:** Task 08 security setup (users and roles must exist before auditing them)
- **Complements:** Task 06 AuditService (uses audit tables for reporting)
- **Timeline-dependent:** Task 09 runs Feb 21 - Feb 28 (after Task 08 completes)

## Success Criteria

✓ AUDITLOG table created with proper structure and FK constraints (deferred deliverable from Task 07)
✓ AUDITLOG indexes created on USERID, THOIGIAN, LOAIHD, TENTABLE for query performance
✓ Standard auditing captures basic operations on all 7 tables
✓ FGA logs sensitive data access attempts on BENHNHAN, HSBA, DONTHUOC
✓ Unified audit captures all important events with context
✓ Audit log queries work and return data from DBA_AUDIT_TRAIL, FGA_LOG$, UNIFIED_AUDIT_TRAIL
✓ AuditService can read logs and generate reports effectively
✓ Backup procedures documented for disaster recovery
✓ Audit data persists correctly across user sessions
✓ No significant performance impact from auditing (< 5% overhead)

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

- **Fri Feb 21:** Create AUDITLOG table + Basic auditing (Tasks 01-02)
- **Mon Feb 24:** Fine-Grained auditing setup (Task 03) + Verify audit logs working
- **Wed Feb 26:** Unified auditing setup (Task 04) + Query utilities (Task 05)
- **Fri Feb 28:** Backup documentation complete (Task 06) + Final testing

## Traceability Matrix

### AUD#1: AUDITLOG Table Creation (Deferred from Task 07)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 3: Audit & Monitoring |
| **Test Timeline** | Fri Feb 21 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `01_CreateAuditLog_Table.sql` — AUDITLOG with FKs and indexes | Required | Fri, Feb 21 |

**Pass Criteria:**

- ✓ AUDITLOG table created with 9 columns (AUDITID, USERID, THOIGIAN, LOAIHD, TENTABLE, MARECORD, MABN, MANV, OLD_VALUES, NEW_VALUES)
- ✓ Primary key on AUDITID (auto-increment)
- ✓ Foreign keys to BENHNHAN.MABN and NHANVIEN.MANV (both optional)
- ✓ 4 indexes created for query performance (USERID, THOIGIAN, LOAIHD, TENTABLE)
- ✓ Table ready to store 50,000+ audit events

---

### AUD#2: Standard Audit Configuration

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 3: Audit & Monitoring |
| **Test Timeline** | End of Week 2 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `02_StandardAudit_Setup.sql` — Standard audit policies | Required | Fri, Feb 21 |

**Pass Criteria:**

- ✓ Standard audit enabled for CREATE USER, DROP USER, ALTER USER operations
- ✓ Standard audit enabled for GRANT and REVOKE operations (all privileges)
- ✓ Standard audit enabled for INSERT, UPDATE, DELETE on 7 main tables (BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, KHOA, THONGBAO)
- ✓ Audit trail records include: username, timestamp, action, object
- ✓ DBA_AUDIT_TRAIL populated after test operations
- ✓ Audit retention policy configured (90 days minimum)

---

### AUD#3: Fine-Grained Audit (FGA)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 3: Audit & Monitoring |
| **Test Timeline** | End of Week 2 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `03_FineGrainedAudit_Setup.sql` — FGA policies for sensitive data | Required | Mon, Feb 24 |

**Pass Criteria:**

- ✓ FGA policy on BENHNHAN: logs access to CCCD (national ID), patient details, medical history fields
- ✓ FGA policy on HSBA: logs access to diagnosis (CHANDOAN), treatment (DIEUTRI), conclusion (KETLUAN)
- ✓ FGA policy on DONTHUOC: logs access to prescriptions (TENTHUOC, LIEUDUNG)
- ✓ FGA policies configured with DBMS_FGA.ADD_POLICY for affected tables
- ✓ DBA_FGA_AUDIT_TRAIL populated after SELECT on sensitive columns
- ✓ FGA overhead < 5% on normal query performance

---

### BAK#1: RMAN Backup Configuration

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 4: Backup & Recovery |
| **Test Timeline** | End of Week 3 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `06_BackupAndRecovery_Documentation.md` — RMAN backup scripts and procedures | Required | Fri, Feb 28 |

**Pass Criteria:**

- ✓ Full backup script for entire database
- ✓ Incremental backup scripts for daily backups
- ✓ Backup catalog configuration
- ✓ Retention policy documented (7 days minimum)
- ✓ Recovery procedures documented (full, point-in-time, tablespace-level)
- ✓ Recovery time objectives (RTO): < 4 hours for full recovery
- ✓ Recovery point objectives (RPO): < 1 hour acceptable data loss

---

### BAK#2: Recovery Testing

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 4: Backup & Recovery |
| **Test Timeline** | End of Week 3 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| Recovery test results and documentation | Required | Fri, Feb 28 |

**Pass Criteria:**

- ✓ Full database restore from backup completes successfully
- ✓ Point-in-time recovery (PITR) restores to specific timestamp (within 1-hour window)
- ✓ Tablespace recovery isolates and restores individual tablespace
- ✓ All data verified after recovery (row counts match baseline: 100K patients, 170 staff, ~140K-210K records, etc.)
- ✓ Recovery time documented and meets RTO targets

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

- **Task 07:** Provides table foundation (COMPLETED - Feb 14)
- **Task 08:** Creates users and roles to be audited (Feb 17-20) — MUST COMPLETE BEFORE TASK 09
- Task 06: AuditService reads audit logs and generates reports
- Task 10: Backup/recovery procedures include audit data protection

---
