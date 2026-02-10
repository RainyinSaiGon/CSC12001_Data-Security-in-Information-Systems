# Implementation Guides & Requirements

Complete assignment specification with step-by-step implementation guides for each requirement.

## Requirement 1: Access Control & Interface (5 points)

**Description:** Implement role-based access control and application interface for multiple user types.

### Requirements
- [ ] TC#1: User Setup & Account Creation
- [ ] TC#2: RBAC for Coordinator & Patient roles
- [ ] TC#3: VPD for Doctor/Nurse role
- [ ] TC#4: RBAC for Technician role
- [ ] TC#5: Patient Self-Service Access
- [ ] Application Interface Implementation

### Implementation Guide
- [ ] Database Setup for user roles and permissions
- [ ] RBAC Implementation (role definitions, role assignments)
- [ ] VPD Implementation (row-level security policies)
- [ ] Application UI Guide (forms and interfaces for each role)

---

## Requirement 2: Notification System with OLS (5 points)

**Description:** Implement Oracle Label Security (OLS) notification system with 3-level label hierarchy.

### Requirements
- [ ] OLS Label Hierarchy (3 levels: Department, Location, Classification)
- [ ] Label Components (Departments, Locations)
- [ ] User Label Assignment (8 users with appropriate labels)
- [ ] Notification Table with Labels
- [ ] Notification UI Interface
- [ ] Access Control Verification

### Implementation Guide
- [ ] OLS Label Hierarchy Setup (configure 3-level structure)
- [ ] User Label Assignment (assign labels to 8 test users)
- [ ] Notification System UI (create forms for notification display)
- [ ] Label-based access control configuration

---

## Requirement 3: Audit & Monitoring (5 points)

**Description:** Implement comprehensive audit mechanisms using Oracle's standard, fine-grained, and unified audit trails.

### Requirements
- [ ] Standard Audit Configuration
- [ ] Fine-Grained / Unified Audit Setup
- [ ] 5+ Audit Test Scenarios
  - [ ] Scenario 1: Unauthorized access attempt
  - [ ] Scenario 2: Privilege escalation attempt
  - [ ] Scenario 3: Data modification by authorized user
  - [ ] Scenario 4: Prescription update
  - [ ] Scenario 5: Patient data access
- [ ] Audit Log Analysis

### Implementation Guide
- [ ] Standard Audit Setup (enable database auditing for specific users/objects)
- [ ] Fine-Grained Audit Setup (configure object-level audit triggers)
- [ ] Unified Audit Setup (modern unified audit trail configuration)
- [ ] Test Scenario Documentation (create and execute 5+ test scenarios)
- [ ] Audit Log Analysis (process and analyze audit results)

---

## Requirement 4: Backup & Recovery (5 points)

**Description:** Implement and document backup and recovery procedures using multiple strategies.

### Requirements
- [ ] Backup Strategy Research & Documentation
- [ ] RMAN Backup Implementation
- [ ] Export/Datapump Backup
- [ ] Recovery Procedures
- [ ] Strategy Evaluation
- [ ] Testing Documentation

### Implementation Guide
- [ ] Backup Strategy Documentation (research and document 2+ strategies)
- [ ] RMAN Configuration (set up Recovery Manager)
- [ ] Recovery Procedures (document recovery steps)
- [ ] Testing Documentation (test recovery procedures)

---

## Test Cases Summary with Pass Criteria

| Test Case | Description | Related Req | Status | Pass Criteria |
|-----------|-------------|------------|--------|---------------|
| TC#1 | User account setup | Req 1 | | UserService creates users, Form displays/validates data, DB stores 170 staff |
| TC#2 | RBAC configuration | Req 1 | | Roles created, users can only perform authorized actions, role restrictions enforced |
| TC#3 | VPD implementation | Req 1 | | Doctor sees only assigned patients, VPD policies transparent, filtering at DB level |
| TC#4 | Technician access | Req 1 | | Technician sees only assigned services, cannot access other technician services |
| TC#5 | Patient self-service | Req 1 | | Patient sees only own records, cannot edit medical data, row-level security enforced |
| OLS#1 | Label hierarchy | Req 2 | | 3-level hierarchy created (Dept, Location, Classification), 3 values each, OLS active |
| OLS#2 | User label assignment | Req 2 | | 8 users labeled, label-based filtering works, notifications filtered by labels |
| AUD#1 | Standard audit | Req 3 | | DBA_AUDIT_TRAIL populated, user logins/logouts logged, table operations tracked |
| AUD#2 | Fine-grained audit | Req 3 | | FGA_LOG$ tracks sensitive field access, prescription/medical record modifications logged |
| BAK#1 | RMAN backup | Req 4 | | Backup executes successfully, incremental strategy configured, backups created |
| BAK#2 | Recovery testing | Req 4 | | Full/point-in-time recovery tested, RTO < 30 min, data integrity verified |

---

## Compliance Checklist

- [ ] Both subsystems implemented
- [ ] All security mechanisms configured
- [ ] All test cases passed
- [ ] Complete documentation
- [ ] Source code committed
- [ ] Database scripts functional
- [ ] All team members contribute

---

## Implementation Requirements for Pass Criteria (Detailed)

### Requirement 1: Access Control & Interface (5 points)

**TC#1 - User Account Setup Requirements:**
- Person 5 Database: Create NHÂNVIÊN table with MÃNV, HỌTÊN, VAITRÒ, CHUYÊNKHOA columns  
- Person 5 Sample Data: Insert 170 staff members (20 Coordinators, 100 Doctors, 50 Technicians)
- Person 2 Services: Implement UserService with CreateUser(), ListUsers(), ModifyUser(), DeleteUser()
- Person 1 Form: UserManagementForm displays all 170 users; validation checks username/password format
- Pass Criteria: Database has samples, services can CRUD users, form displays without errors

**TC#2 - RBAC Configuration Requirements:**
- Person 5 Database: Create 4 roles with CREATE ROLE; grant permissions via GRANT statements
- Person 4 Services: RBACService.CheckPermission() enforces role-action whitelist authorization
- Person 1/3 Forms: Menu/button enablement based on user role (deny unauthorized actions)
- Pass Criteria: Coordinator cannot access Doctor functions, Doctor cannot access Technician functions

**TC#3 - VPD Implementation Requirements:**
- Person 5 Database: VPD policies on HSBA/HSBA_DV using WHERE conditions to filter by user ID
- Person 5 Sample Data: 100 patients assigned to 100 doctors (doctor-patient relationships)
- Person 4 Service: VPDService retrieves already-filtered data from database
- Person 3 Forms: DoctorForm displays filtered patient list (no code changes for filtering)
- Pass Criteria: Doctor sees ≤100 patients, VPD enforces at database, filtering transparent

**TC#4 - Technician Access Requirements:**
- Person 5 Database: 50 technicians, services assigned to technicians, VPD on HSBA_DV
- Person 5 Service: TechnicianService.GetAssignedServices() returns assigned services only
- Person 3 Form: TechnicianForm displays assigned services with update/complete options
- Pass Criteria: Technician sees only assigned services, cannot access other technician data

**TC#5 - Patient Self-Service Access Requirements:**
- Person 5 Service: PatientService filters queries to authenticated patient's records only
- Person 3 Form: PatientForm displays patient's BỆNHNHÂN, HSBA, ĐƠNTHUỐC (read-only for medical)
- editability: Contact info (address, phone) editable; diagnoses/treatment/prescriptions read-only
- Pass Criteria: Patient sees own records only, cannot edit medical data, row-level security enforced

### Requirement 2: Notification System with OLS (5 points)

**OLS#1 - Label Hierarchy Configuration Requirements:**
- Person 5 Database: Create Notification table with Department, Location, Classification columns
- Person 5 OLS Setup: Use DBMS_MACADM to create 3-level label hierarchy
  - Level 1: Departments (Cardiology, Gastroenterology, Neurology)
  - Level 2: Locations (Hồ Chí Minh, Hải Phòng, Hà Nội)
  - Level 3: Classifications (Staff, DepartmentHead, Director)
- Person 5 Labels: OLS policy attached to Notification table, enforces label-based access
- Pass Criteria: 3 levels with 3 values each, OLS policy active, labels retrievable

**OLS#2 - User Label Assignment Requirements:**
- Person 5 Database: Assign labels to 8 test users (2 Directors, 2 Dept Heads, 4 Staff)
- Person 4 Service: OLSService.GetAccessibleNotifications() returns filtered notification IDs
- Person 3 Form: NotificationForm displays only accessible notifications based on user labels
- Access Rule: User can access notification if user_level >= notification_level in ALL 3 dimensions
- Pass Criteria: Directors see all, Dept Heads see own + lower, Staff see exactly matching labels

### Requirement 3: Audit & Monitoring (5 points)

**AUD#1 - Standard Audit Configuration Requirements:**
- Person 5 Database: Enable auditing via AUDIT ALL STATEMENTS, AUDIT CONNECT/DISCONNECT
- Audit Targets: All INSERT/UPDATE/DELETE on HSBA, BỆNHNHÂN, ĐƠNTHUỐC
- Storage: DBA_AUDIT_TRAIL populated with timestamp, user, object, operation
- Person 5 Service: AuditService.LogUserAction() logs custom events to AuditLog table
- Person 5 Queries: ReadAuditLogs.sql provides sample queries for compliance analysis
- Pass Criteria: Audit records created for all user actions, immutable audit trail

**AUD#2 - Fine-Grained Audit Configuration Requirements:**
- Person 5 Database: Enable fine-grained auditing using DBMS_FGA.ADD_POLICY
- FGA Targets: All operations on HSBA, ĐƠNTHUỐC; SELECT on BỆNHNHÂN sensitive columns
- Storage: FGA_LOG$ records who, what, when for sensitive operations
- Tracking: Capture details of prescription/diagnosis/patient modifications for compliance
- Pass Criteria: FGA_LOG$ has records, sensitive field changes traceable to user

### Requirement 4: Backup & Recovery (5 points)

**BAK#1 - RMAN Backup Implementation Requirements:**
- Person 5 Database: Configure RMAN with backup destination
- Strategy: Level 0 (full backup) + Level 1 (incremental) with retention policies (7+ days)
- Documentation: Backup_Recovery_Documentation.md documents RMAN configuration, commands, strategy
- Verification: Execute backup, verify backups created successfully
- Pass Criteria: Backup runs successfully, incremental strategy works, backups verified

**BAK#2 - Recovery Testing Requirements:**
- Person 5 Database: Document recovery procedures for 3 scenarios (full, point-in-time, selective)
- Testing: Execute recovery procedures, verify data integrity (DBVERIFY)
- Performance: Measure RTO (< 30 minutes) and RPO (< 1 hour)
- Documentation: Backup_Recovery_Documentation.md includes tested recovery procedures
- Pass Criteria: Full recovery works, point-in-time recovery works, data integrity verified

---

# Progress & Test Results

## Team Members & Contributions

| Member | Role | Responsibility | Status |
|--------|------|-----------------|--------|
| To be assigned | Person 1 | Subsystem 1 Forms (5 forms) | Not started |
| To be assigned | Person 2 | Subsystem 1 Services (6 services) | Not started |
| To be assigned | Person 3 | Subsystem 2 Forms (7 forms) | Not started |
| To be assigned | Person 4 | Subsystem 2 Security Services (6 services) | Not started |
| To be assigned | Person 5 | Database & Business Services (11 services + 12 SQL files) | Not started |

## Progress Tracking

### Requirement 1: Access Control & Interface (5 points)
**Current Status**: Not Started

**Milestone Targets:**
- Week 1: Database setup complete (Person 5)
- Week 2: All services implemented (Persons 2, 4, 5)
- Week 3: All forms completed (Persons 1, 3)
- Week 4: Integration testing and security verification

**Dependencies:**
- Blocks: Person 1 form development (waits for Person 2 services)
- Blocks: Person 3 form development (waits for Person 4 authentication)
- Blocks: Person 2 service development (waits for Person 5 database)

### Requirement 2: OLS Notification System (5 points)
**Current Status**: Not Started

**Milestone Targets:**
- Week 2: OLS label hierarchy created (Person 5)
- Week 2: User labels assigned (Person 5)
- Week 3: OLS service implemented (Person 4)
- Week 3: Notification form implemented (Person 3)

### Requirement 3: Audit & Monitoring (5 points)
**Current Status**: Not Started

**Milestone Targets:**
- Week 2: Standard audit configured (Person 5)
- Week 2: Fine-grained audit setup (Person 5)
- Week 3: Audit service implemented (Person 5)
- Week 4: 5+ test scenarios executed and documented

### Requirement 4: Backup & Recovery (5 points)
**Current Status**: Not Started

**Milestone Targets:**
- Week 2: Backup strategy documented (Person 5)
- Week 3: RMAN backup configured (Person 5)
- Week 4: Recovery procedures tested (Person 5)

## Test Execution Results

### Test Case Tracking with Pass Criteria

| TC | Description | Purpose | Status | Pass Criteria | Notes |
|----:|------------|---------|--------|---------------|-------|
| TC#1 | User account setup | Verify user creation | Pending | UserService works + Form functions + 170 staff in DB | Requires DB tables + services |
| TC#2 | RBAC configuration | Verify role-based access | Pending | Roles created + permissions enforced + users cannot do unauthorized actions | Requires RBAC roles + AuthService |
| TC#3 | VPD implementation | Verify row-level filtering | Pending | Doctor sees ≤ 100 patients, VPD transparent, VPD filters at DB not app | Requires VPD policies |
| TC#4 | Technician access | Verify technician isolation | Pending | Technician sees only assigned services, cannot access other technician data | Requires VPD on services |
| TC#5 | Patient self-service | Verify patient data isolation | Pending | Patient sees own records only, no medical data editing, row-level security enforced | Requires row-level security |
| OLS#1 | Label hierarchy | Verify label structure | Pending | 3 levels created (Dept/Location/Class), 3 values each, OLS policy active on Notification table | Requires OLS setup |
| OLS#2 | User label assignment | Verify label access control | Pending | 8 users labeled correctly, label-based filtering works, accessible notifications filtered | Requires user labels |
| AUD#1 | Standard audit | Verify audit logging | Pending | DBA_AUDIT_TRAIL has records, logins/logouts logged, HSBA/BỆNHNHÂN operations tracked | Requires audit configuration |
| AUD#2 | Fine-grained audit | Verify sensitive field audit | Pending | FGA_LOG$ has records, prescription/diagnosis changes logged with details | Requires fine-grained policies |
| BAK#1 | RMAN backup | Verify backup functionality | Pending | Backup executes successfully, backups created and verified, incremental strategy works | Requires RMAN setup |
| BAK#2 | Recovery testing | Verify recovery procedures | Pending | Full recovery < 30 min, point-in-time recovery works, data integrity verified | Requires backup verification |

## Performance Analysis

### Baseline Targets (to be measured during testing)

**Database Performance Targets:**
- Query response time: < 500 ms
- Audit logging overhead: < 5%
- VPD filtering overhead: < 10%

**Application Performance Targets:**
- Login time: < 2 seconds
- Data loading: < 3 seconds  
- Record search: < 5 seconds

### Notes
- Actual measurements will be recorded during Week 4 testing
- Performance issues will be logged as blockers if thresholds exceeded
- Index optimization may be required if targets not met

## Security Assessment

- [ ] RBAC properly implemented
- [ ] VPD transparent filtering works
- [ ] OLS labels correctly assigned
- [ ] Audit trails immutable
- [ ] No security vulnerabilities found
- [ ] Data encryption implemented

## Issues & Resolutions

| Issue ID | Component | Description | Status | Priority | Resolution |
|----------|-----------|-------------|--------|----------|-----------|
| To be logged | TBD | To be determined during development | Not started | - | TBD |

**Note:** Issues will be recorded in GitHub Issues and tracked during implementation.

## Final Summary

**Project Status:** Not Started  
**Estimated Total Effort:** 160-175 hours (5 persons × 4 weeks)

**Next Steps:**
1. Assign team members to roles (Person 1-5)
2. Create detailed project plan with milestones
3. Setup database development environment
4. Begin implementing in parallel per TASK_ASSIGNMENT.md
5. Conduct weekly sync meetings for progress tracking

**Key Success Factors:**
- Strict adherence to Phase 0 architecture (Forms/Models/Services)
- Timely completion of Person 5's database setup
- Regular team communication on blockers/dependencies
- Comprehensive security testing in Week 4
- Complete documentation of all procedures

**Report Date:** February 10, 2026  
**Last Updated:** February 10, 2026  
**Next Review Date:** To be scheduled after team assignment
