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
- Implement recovery procedures based on audit logs
- Test recovery scenarios to ensure data restoration capability

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

In case of major corruption:

```sql
-- Restore from last known good backup
RMAN> RESTORE DATABASE;
RMAN> RECOVER DATABASE UNTIL TIME;

-- Verify recovery
SQL> SELECT COUNT(*) FROM BENHNHAN;
SQL> SELECT COUNT(*) FROM HSBA;
SQL> SELECT COUNT(*) FROM DONTHUOC;
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

- **Requires:** Task 07 tables completed (Fri Feb 14)
- **Requires:** Task 09 audit logs (Fri Feb 28)
- **Supports:** Disaster recovery planning for entire project
- **No blocks:** Complements other work

## Success Criteria

✓ All 4 backup methods documented with pros/cons  
✓ RMAN configured with retention policy  
✓ Automatic daily backup job created  
✓ Weekly full backup scheduled  
✓ Archive log management configured  
✓ Recovery scenarios tested successfully  
✓ Audit logs used to identify recovery points  
✓ Database successfully recovered from backup  
✓ Performance impact of backup < 15%  
✓ Recovery time objectives (RTO) met: < 2 hours

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

- ✓ All 4 backup methods evaluated (RMAN, Hot, Cold, Data Pump)
- ✓ RMAN configured with 30-day retention policy
- ✓ Automatic daily incremental backups running
- ✓ Weekly full backups completing successfully
- ✓ Archive log management working (automatic deletion after backup)
- ✓ Database recovery from backup tested and working
- ✓ Point-in-time recovery using audit logs verified
- ✓ Flashback table recovery tested for recent changes
- ✓ Recovery Time Objective (RTO) < 2 hours achieved
- ✓ Document advantages/disadvantages of each method
- ✓ Recovery procedures documented for operations team

**Evidence Tracking:**

- RMAN configuration output
- Backup job logs showing daily success
- V$BACKUP_SET query results showing backup history
- Successful restore test results
- Archive log count and deletion proof
- Recovery completion time metrics
- Database integrity check results post-recovery

---

## Related Tasks

- Task 07: Provides database foundation
- Task 08: Security setup supports audit-based recovery
- Task 09: Audit logs guide recovery point identification
- All tasks: Depend on reliable backup strategy

---

## Conclusion Guidance

Summarize findings:
1. Best backup method for hospital system requirements
2. Recovery capabilities meeting business continuity needs
3. Integration with audit logging for compliance
4. Operational cost-benefit analysis
5. Recommendations for production deployment

**Critical: RTO < 2 hours is non-negotiable for hospital operations**

