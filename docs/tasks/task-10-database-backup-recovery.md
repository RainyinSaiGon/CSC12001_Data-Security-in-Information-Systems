# Task 10: Subsystem 2 Database Backup & Recovery - Yêu cầu 4

**Assigned to:** Ngọc, Vũ (Part C)  
**Type:** Database Administration  
**Duration:** 8 hours  
**Priority:** Medium-High  
**Timeline:** Feb 28 - Mar 7, 2026

---

## Overview

Implement comprehensive backup and recovery mechanisms for Subsystem 2 Oracle database data protection and disaster recovery:

- Research Oracle backup methodologies (RMAN, export/import, hot backup)
- Configure both automatic and manual backup strategies
- Implement recovery procedures based on audit logs (Task 09)
- Test recovery scenarios to ensure data restoration capability for 7 tables (KHOA, BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO)
- Production-scale data: 100K patients, 170 staff, ~140K-210K medical records, ~280K-420K prescriptions, ~140K-210K diagnostic services, 12K notifications

## Requirement Mapping

**Requirement 4: Backup and Recovery**

- Requirement 4.1: Understand backup mechanisms from DBMS
- Requirement 4.2: Implement backup methods on Oracle
- Requirement 4.3: Evaluate advantages/disadvantages of methods
- Requirement 4.4: Provide conclusions

## Deliverables

### 01_Backup_Strategy.sql

Document and implement backup policy (Pros/Cons and Use Cases):

**Backup Methods Research:**

1. **RMAN (Recovery Manager) Backup**

2. **Hot Backup (Online Backup)**

3. **Cold Backup (Offline Backup)**

4. **Export/Import (Data Pump)**

**Implementation Plan:**

```sql
-- Enable ARCHIVELOG for recovery
ALTER SYSTEM SET ARCHIVE_LOG_DEST_10 = 'LOCATION=/archive_logs';
ALTER DATABASE ARCHIVELOG;

-- Configure RMAN
CONFIGURE RETENTION POLICY TO RECOVERY WINDOW OF 30 DAYS;
CONFIGURE BACKUP OPTIMIZATION ON;
CONFIGURE CHANNEL 1 DEVICE TYPE DISK
  FORMAT '/backup/rman_%d_%T_%s_%p';

-- Schedule automatic backups (via database scheduler)
CREATE OR REPLACE PROCEDURE DAILY_BACKUP AS
BEGIN
  -- RMAN backup script
  -- Runs nightly via DBMS_SCHEDULER
END;
```

**Backup Schedule:**

- Daily incremental backups (11 PM)
- Weekly full backup (Sunday midnight)
- Monthly archive backup to offline storage
- Retention: 30-day rolling window

---

### 02_AutomaticBackup.sql

Configure automatic backup mechanisms:

**RMAN Automated Backup:**

```sql
-- Enable RMAN backup automation
CONFIGURE BACKUP RETENTION POLICY TO RECOVERY WINDOW OF 30 DAYS;
CONFIGURE DEFAULT DEVICE TYPE TO DISK;

-- Create backup job
BEGIN
  DBMS_SCHEDULER.CREATE_JOB(
    job_name   => 'DAILY_BACKUP_JOB',
    job_type   => 'EXECUTABLE',
    job_action => '/scripts/rman_backup.sh',
    start_date => SYSDATE,
    repeat_interval => 'FREQ=DAILY;BYHOUR=23',
    enabled    => TRUE,
    comments   => 'Daily RMAN backup at 11 PM'
  );
END;
/

-- Create archive log backup job
DBMS_SCHEDULER.CREATE_JOB(
  job_name   => 'ARCHIVE_BACKUP_JOB',
  job_type   => 'EXECUTABLE',
  job_action => '/scripts/archive_backup.sh',
  repeat_interval => 'FREQ=WEEKLY;BYDAY=SUN;BYHOUR=00',
  enabled   => TRUE
);
```

**Automatic Archive Log Management:**

```sql
-- Enable archive log deletion after backup
RMAN> CONFIGURE ARCHIVELOG DELETION POLICY TO BACKED UP 2 TIMES TO DISK;

-- Compress backups to save space
RMAN> CONFIGURE COMPRESSION ALGORITHM 'BASIC';
```

**Monitoring Backup Status:**

```sql
-- Query backup history
SELECT RECID, STAMP, TYPE, COMPLETION_TIME, STATUS
FROM V$BACKUP_SET
ORDER BY COMPLETION_TIME DESC;

-- Check backup failures
SELECT OUTPUT
FROM V$RMAN_BACKUP_JOB_DETAILS
WHERE STATUS = 'FAILED'
ORDER BY END_TIME DESC;
```

---

### 03_RecoveryScenarios.sql

Implement recovery procedures based on audit logs:

**Scenario 1: Recover a Deleted Row**

Based on audit trail showing deletion time:

```sql
-- Recover row from within 6 hours of deletion
-- Use point-in-time recovery approach
FLASHBACK TABLE HSBA TO TIMESTAMP (SYSTIMESTAMP - INTERVAL '2' HOUR);
```

**Scenario 2: Recover Accidentally Modified Data**

When audit log shows unauthorized UPDATE:

```sql
-- Identify bad transaction from FGA_LOG$
SELECT * FROM FGA_LOG$
WHERE DB_USER = 'user_staff_001'
  AND SQL_BIND LIKE '%UPDATE HSBA%'
  AND TIMESTAMP >= to_date('2026-02-25 14:30:00', 'yyyy-mm-dd hh24:mi:ss')
  AND TIMESTAMP <= to_date('2026-02-25 14:45:00', 'yyyy-mm-dd hh24:mi:ss');

-- Recover to point before update
FLASHBACK TABLE HSBA TO TIMESTAMP 
  (to_timestamp('2026-02-25 14:29:00', 'yyyy-mm-dd hh24:mi:ss'));
```

**Scenario 3: Recover Entire Database**

In case of major corruption (all 7 tables: KHOA, BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO):

```sql
-- Restore from last known good backup
RMAN> RESTORE DATABASE;
RMAN> RECOVER DATABASE UNTIL TIME;

-- Verify recovery of all tables
SQL> SELECT COUNT(*) AS khoa_count FROM KHOA;         -- Expected: 3
SQL> SELECT COUNT(*) AS benhnhan_count FROM BENHNHAN; -- Expected: 100,000
SQL> SELECT COUNT(*) AS nhanvien_count FROM NHANVIEN; -- Expected: 170
SQL> SELECT COUNT(*) AS hsba_count FROM HSBA;         -- Expected: ~140K-210K
SQL> SELECT COUNT(*) AS hsbadv_count FROM HSBA_DV;    -- Expected: ~140K-210K
SQL> SELECT COUNT(*) AS donthuoc_count FROM DONTHUOC; -- Expected: ~280K-420K
SQL> SELECT COUNT(*) AS thongbao_count FROM THONGBAO; -- Expected: 12,000
```

**Scenario 4: Selective Data Recovery**

Recover specific tables from backup:

```sql
-- Export specific table from backup
RMAN> RESTORE TABLE HSBA;

-- Import recovered table
Data Pump import with REMAP_TABLE option
```

---

## Dependencies

- **Requires:** Task 07 tables completed (Fri Feb 14 - COMPLETED)
- **Requires:** Task 08 security setup (users/roles must exist to test authentication-based recovery)
- **Requires:** Task 09 audit logs completed (Fri Feb 28 - for audit-driven recovery point identification)
- **Supports:** Disaster recovery planning for entire project
- **Timeline:** Runs Feb 28 - Mar 7 (after Tasks 07-09 complete)

## Success Criteria

✓ All 4 backup methods documented with pros/cons and use cases
✓ RMAN configured with 30-day retention policy
✓ Automatic daily incremental backup job created and tested
✓ Weekly full backup scheduled and verified
✓ Archive log management configured with automatic deletion after backup
✓ Recovery scenarios tested successfully for all 7 tables (KHOA, BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO)
✓ Audit logs (Task 09) used to identify and verify recovery points
✓ Database successfully recovered from backup with data integrity verified
✓ Point-in-time recovery tested (within 1-hour recovery window from audit logs)
✓ Flashback recovery tested for recent changes (< 6 hours)
✓ Performance impact of backup < 15%
✓ Recovery time objectives (RTO) met: < 2 hours for full database recovery
✓ Row count verification: BENHNHAN (100K), NHANVIEN (170), HSBA (~140K-210K), HSBA_DV (~140K-210K), DONTHUOC (~280K-420K), THONGBAO (12K)

## Evaluation Framework

### Advantages Analysis:

**RMAN:**

**Hot Backup:**

**Cold Backup:**

**Export/Import:**


### Disadvantages Analysis:

**RMAN:**

**Hot Backup:**

**Cold Backup:**

**Export/Import:**


## Implementation Recommendations

**For This Project:**
1. Use **RMAN** as primary method (production-grade, flexible recovery)
2. Implement **automatic daily incremental** backups (efficient, reliable)
3. Weekly **full backup to offline storage** (disaster recovery)
4. **Flashback** for quick recovery of recent changes (audit-driven)
5. **Data Pump export** for logical backup of sensitive tables

**Recovery Plan:**
- Point-in-time recovery using archive logs
- Flashback for data corruption scenarios
- Full restore from RMAN backups for catastrophic failure
- Audit logs guide recovery to specific transaction points

## Testing Checklist

After implementation:

- [ ] Backup job runs daily without errors
- [ ] Full backups complete successfully weekly
- [ ] Archive logs backed up and deleted properly
- [ ] Restore from backup successful
- [ ] Point-in-time recovery works correctly
- [ ] Flashback functionality confirmed
- [ ] Recovery time < 2 hours
- [ ] Audit logs accurately track all changes
- [ ] Recovery procedures documented for operations team
- [ ] Recovery plan validated with sample data

## Traceability Matrix

### Requirement 4: Backup & Recovery 

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 4: Backup & Recovery|
| **Test Timeline** | End of Week 4 (Mar 7) |
| **User Facing?** | No (backend only) |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `01_Backup_Strategy.sql` — Strategy documentation and implementation | Required | Feb 28 |
| `02_AutomaticBackup.sql` — Automated backup jobs and monitoring | Required | Feb 28 |
| `03_RecoveryScenarios.sql` — Recovery procedures and testing | Required | Mar 7 |

**Pass Criteria:**

- ✓ All 4 backup methods evaluated (RMAN, Hot, Cold, Data Pump) with written advantages/disadvantages
- ✓ RMAN configured with 30-day retention policy and automatic job scheduling
- ✓ Automatic daily incremental backups running successfully
- ✓ Weekly full backups completing successfully with verification
- ✓ Archive log management working (automatic deletion after backed up 2 times)
- ✓ Database recovery from backup tested - all 7 tables restored with correct row counts:
  - KHOA: 3
  - BENHNHAN: 100,000
  - NHANVIEN: 170
  - HSBA: ~140,000-210,000
  - HSBA_DV: ~140,000-210,000
  - DONTHUOC: ~280,000-420,000
  - THONGBAO: 12,000
- ✓ Point-in-time recovery using audit logs (Task 09) verified and working
- ✓ Flashback table recovery tested for recent changes (< 6 hours)
- ✓ Recovery Time Objective (RTO) < 2 hours achieved
- ✓ Recovery procedures documented for operations team
- ✓ Audit integrity verified post-recovery (audit logs persistent)

**Evidence Tracking:**

- RMAN configuration output (retention policy verified)
- Backup job logs showing daily successful runs
- V$BACKUP_SET query results showing backup history
- Successful restore test results with before/after row count comparison
- Audit logs (AUDITLOG from Task 09) showing recovery point identification
- Archive log deletion proof (count decrease after backup)
- Recovery completion time metrics (RTO demonstration < 2 hours)
- Database integrity check results post-recovery (DBMS_REPAIR or equivalent)
- SELECT COUNT(*) verification for all 7 tables matching expected volumes

---

## Related Tasks

- **Task 07:** Provides database foundation (7 tables: KHOA, BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO) - COMPLETED
- **Task 08:** Security setup provides user authentication context for recovery testing
- **Task 09:** Audit logs (AUDITLOG table) guide recovery point identification - MUST COMPLETE FIRST
- All other tasks: Depend on reliable backup and recovery strategy for data protection

---

## Conclusion Guidance

Summarize findings and recommendations in written report:

1. **Best Backup Method for Hospital System:**
   - Primary recommendation: RMAN (most reliable, flexible recovery options)
   - Justification: supports 7-table medical database (BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, KHOA, THONGBAO)
   - Handles 100K+ patient records and 300K+ transaction records

2. **Recovery Capabilities:**
   - Full database recovery: < 2 hours (meets hospital RTO)
   - Point-in-time recovery: within 1-hour window using audit logs
   - Flashback recovery: < 6 hours for recent changes
   - Row-level recovery: supported via audit trail

3. **Integration with Audit Logging (Task 09):**
   - AUDITLOG table enables transaction-level recovery point identification
   - Audit timestamps guide point-in-time recovery precision
   - Compliance: audit logs persist across recovery cycles

4. **Operational Cost-Benefit Analysis:**
   - Resource cost: disk storage for 30-day backup window (~50-100 GB)
   - Personnel time: automated backups minimize manual intervention
   - Recovery benefit: avoid medical data loss (critical for patient safety)
   - Regulatory benefit: supports GDPR/HIPAA compliance requirements

5. **Recommendations for Production Deployment:**
   - Implement RMAN with 30-day rolling retention window
   - Automatic daily incremental backups (11 PM)
   - Weekly full backup to offline storage (Sunday midnight)
   - Test recovery procedures monthly
   - Maintain audit logs for compliance (1-year retention)
   - Document RTO/RPO for operational team

**Critical: Medical data loss is unacceptable - RTO < 2 hours is non-negotiable for hospital operations. Patient safety depends on reliable backup and recovery.**

