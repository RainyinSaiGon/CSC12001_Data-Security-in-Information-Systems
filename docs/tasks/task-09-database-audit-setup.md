# Task 09: Subsystem 2 Database Audit & Backup/Recovery Setup

**Assigned to:** Ngọc, Vũ (Part B)
**Duration:** 7 hours
**Priority:** Medium-High
**Timeline:** Feb 21 - Feb 28, 2026

---

## 1. Objective

Implement comprehensive auditing using Standard, Fine-Grained (FGA), and Unified Auditing to monitor user activities on sensitive medical data. Establish backup and recovery strategies leveraging audit logs to restore data integrity after incidents.

## 2. Scope of Work

### 2.1. Standard Auditing (Requirement 3.2)

* **Activation:** Enable system-wide auditing
* **Scope:** Monitor specific behaviors on key database objects (Tables, Views, Stored Procedures, Functions)
* **Scenarios:** Implement 5 distinct audit contexts: Failed login attempts, unauthorized table access, privilege changes, schema modifications, bulk data deletion attempts

### 2.2. Advanced Auditing - FGA/Unified Audit (Requirement 3.3)

* **Scenario A (Prescription Updates):**
  * Target: DONTHUOC (Prescriptions)
  * Action: Updates to drug name, dosage, date after creation
  * Condition: Modifications by Doctors/Pharmacists

* **Scenario B (Medical Record Updates):**
  * Target: HSBA (Medical Records)
  * Action: Updates to diagnosis, treatment, conclusion
  * User: Assigned doctors only

* **Scenario C (Illegal Medical Record Updates):**
  * Target: HSBA
  * Action: Unauthorized update attempts on diagnosis, treatment, conclusion

* **Scenario D (Service Record Tampering):**
  * Target: HSBA_DV (Service Records)
  * Action: Illegal INSERT, DELETE, or UPDATE operations

### 2.3. Backup & Recovery (Requirement 4)

* **Mechanism:** Research and implement Oracle DBMS backup/recovery solutions
* **Implementation:**
  * Configure manual and automatic backup schedules
  * Perform data recovery using audit logs to identify and restore compromised data
* **Evaluation:** Analyze advantages and disadvantages of implemented methods

## 3. Deliverables & Execution Order

### `01_Standard_Audit.sql`

* Activates standard auditing
* Configures 5 required audit contexts
* Includes queries to verify DBA_AUDIT_TRAIL

### `02_FGA_Unified_Audit.sql`

* Creates policies for Scenarios A, B, C, and D
* Captures SQL text and bind variables
* Tracks post-prescription modifications on DONTHUOC
* Monitors valid and invalid updates on HSBA
* Monitors integrity of HSBA_DV

### `03_Audit_Reporting.sql`

* Scripts to extract and read audit logs
* Generates compliance reports showing Who, When, What, and How

## 4. Acceptance Criteria

* [ ] **Standard Audit:** 5 distinct scenarios logged successfully
* [ ] **Prescription Audit:** Updates to DONTHUOC logged with context
* [ ] **Medical Record Integrity:** Authorized and unauthorized changes captured
* [ ] **Service Tampering:** Any INSERT/UPDATE/DELETE on HSBA_DV triggers audit
