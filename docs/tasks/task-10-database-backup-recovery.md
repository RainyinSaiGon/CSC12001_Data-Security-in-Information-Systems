# Task 10: Subsystem 2 Database Backup & Recovery

**Assigned to:** Ngọc, Vũ (Part C)
**Duration:** 8 hours
**Priority:** Medium-High
**Timeline:** Feb 28 - Mar 7, 2026

---

## 1. Objective

Design, implement, and validate a comprehensive backup and recovery strategy for the Medical Data Management System. Ensure data durability for 100,000+ patient records using Oracle RMAN, Flashback, and audit trail integration.

## 2. Scope of Work

### 2.1. Methodology Research & Strategy (Requirement 4.1, 4.2)

* **Research:** Analyze pros/cons/use-cases for four Oracle backup methods:
  * RMAN (Recovery Manager) — Primary strategy
  * Hot Backup — Online backup while DB is active
  * Cold Backup — Offline backup
  * Data Pump — Logical backup for specific tables

* **Configuration:** Enable ARCHIVELOG mode for point-in-time recovery

### 2.2. Automated Backup Implementation

* **Tooling:** Configure Oracle RMAN with 30-day retention policy
* **Scheduling:** Implement DBMS_SCHEDULER jobs:
  * **Daily:** Incremental backups (23:00)
  * **Weekly:** Full database backups (Sunday 00:00)
  * **Maintenance:** Automatic obsolete archive log deletion

### 2.3. Incident Recovery Scenarios (Requirement 4.3)

Execute and validate recovery procedures using Audit Logs (Task 09) to identify failure timestamps:

* **Scenario A (Data Corruption):** Recover erroneous HSBA row update (Method: Flashback Table)
* **Scenario B (Data Loss):** Recover deleted HSBA_DV record (Method: Point-in-Time Recovery)
* **Scenario C (Disaster Recovery):** Restore entire database after failure (Method: RMAN Full Restore)
* **Scenario D (Table Restoration):** Restore single table from backup (Method: RMAN/Data Pump)

## 3. Deliverables & Execution Order

### `01_Backup_Strategy.sql`

* SQL commands to enable ARCHIVELOG
* RMAN configuration scripts (retention policy, channels, optimization)
* Comparison documentation of 4 backup methods

### `02_AutomaticBackup.sql`

* PL/SQL scripts creating DBMS_SCHEDULER jobs for daily/weekly backups
* Archive log deletion policy configuration
* Queries to monitor backup status in V$RMAN_BACKUP_JOB_DETAILS

### `03_RecoveryScenarios.sql`

* Executable scripts demonstrating:
  * FLASHBACK TABLE operations
  * RMAN RESTORE DATABASE commands
  * Audit log timestamp integration with recovery

## 4. Acceptance Criteria

* [ ] **Automation:** Daily and weekly backup jobs scheduled and visible
* [ ] **Retention:** RMAN confirms 30-day recovery window
* [ ] **Data Integrity:** Post-recovery row counts match baseline:
  * BENHNHAN: ~100,000
  * NHANVIEN: 170
  * HSBA: ~140,000+
  * DONTHUOC: ~280,000+
* [ ] **Precision Recovery:** Successfully recovered specific row using audit log timestamp
* [ ] **Performance:** Full recovery completed within RTO < 2 hours

