# Audit Logs

Sample audit log files and analysis from security testing.

## Audit Log Samples

### DBA_AUDIT_TRAIL Views

Sample queries to extract audit data:

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

## Fine-Grained Audit Examples

Examples of fine-grained audit logs:

```
Audit Type: DML on PRESCRIPTION
Timestamp: 2026-02-09 14:30:45
User: DOCTOR001
Action: UPDATE
Columns Modified: 
  - LIỀUDÙNG: 'Previous Value' -> 'New Value'
Table: ĐƠNTHUỐC
Record ID: 12345
Success: Y
```

## Analysis Templates

### Audit Log Analysis Report

Date: ________
Period: ________ to ________

#### Summary
- Total Audit Events: ____
- Failed Authentication: ____
- Privilege Changes: ____
- Data Modifications: ____

#### Suspicious Activities
1. [Activity Description]
2. [Activity Description]

#### Compliance Status
- [ ] All audit events logged
- [ ] No gaps in audit trail
- [ ] Timestamps accurate
- [ ] User identification complete

## Audit Test Scenarios

### Scenario 1: Unauthorized Access Attempt
- User: Invalid_User
- Target: BỆNHNHÂN table
- Result: Access Denied (logged)

### Scenario 2: Privilege Escalation Attempt
- User: TECHNICIAN001
- Attempted Action: Write to HSBA table
- Result: Denied (logged)

### Scenario 3: Data Modification by Authorized User
- User: DOCTOR001
- Table: HSBA
- Columns Modified: CHẨNĐOÁN, ĐIỀUTRỊ
- Result: Success (logged with details)

### Scenario 4: Prescription Update
- User: DOCTOR001
- Table: ĐƠNTHUỐC
- Columns Modified: LIỀUDÙNG
- Result: Success (logged)

### Scenario 5: Patient Data Access
- User: PATIENT001
- Query: Own medical records
- Result: Success (logged)

## Log File Locations

### Oracle Audit Trail
```
Database: project_admin tablespace
Table: aud$
Query: SELECT * FROM aud$ ORDER BY ntimestamp# DESC;
```

### Alert Logs
```
Location: $ORACLE_BASE/diag/rdbms/[db_name]/alert_[db_name].log
Contains: Errors, warnings, audit events
```

## Audit Log Retention Policy

- **Retention Period**: 1 year
- **Archive Frequency**: Monthly
- **Backup**: Weekly to tape
- **Purge Policy**: Delete after 1 year retention

## Audit Dashboard Metrics

```
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

## References

- See [IMPLEMENTATION.md](IMPLEMENTATION.md) for audit setup requirements
- See [Database README](../Database/README.md) for audit scripts
- Check [Database/Audit/ReadAuditLogs.sql](../Database/Audit/ReadAuditLogs.sql) for sample queries
