# Traceability Matrix: Test Cases → Person → Deliverables

This document maps each test case to the responsible team member and required deliverables, ensuring complete coverage of requirements and clear accountability.

---

## Requirement 1: Access Control & Interface (5 points)

### TC#1: User Account Setup

**Test Objective:** Verify system can create, store, and manage user accounts with proper validation.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | Person 5 (Database), Person 2 (Service), Person 1 (Form) |
| **Test Timeline** | End of Week 2 (after database and services ready) |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `01_CreateTables.sql` - NHÂNVIÊN table | Critical | Week 1 - End Friday |
| Person 5 | `03_InsertSampleData.sql` - 170 staff records | Critical | Week 1 - End Friday |
| Person 2 | `OracleConnectionService.cs` | Critical | Week 1 - Mid Week |
| Person 2 | `ValidationService.cs` - ValidateUsername, ValidatePassword | Required | Week 2 - Early |
| Person 2 | `UserService.cs` - CreateUser(), ListUsers(), DeleteUser(), ModifyUser() | Required | Week 2 - Early |
| Person 1 | `Forms/UserManagementForm.cs` | Required | Week 3 |

**Pass Criteria:**
- ✓ Database tables created with no errors
- ✓ Sample user data inserted successfully (170 staff members)
- ✓ OracleConnectionService successfully connects to database
- ✓ ValidationService validates username format (3-30 chars, alphanumeric + underscore, no reserved keywords)
- ✓ ValidationService validates password strength (8+ chars, mixed case, at least 1 number)
- ✓ UserService.CreateUser() creates user in NHÂNVIÊN table
- ✓ UserService.ListUsers() retrieves all users with complete details
- ✓ UserService.ModifyUser() updates user properties
- ✓ UserService.DeleteUser() removes user from database
- ✓ UserManagementForm displays all users in DataGrid
- ✓ UserManagementForm input validation prevents invalid data
- ✓ UserManagementForm shows error messages for failed operations
- ✓ No security vulnerabilities (no hardcoded credentials)
- ✓ Performance acceptable (user list loads within 2 seconds)

**Evidence Tracking:**
- Database script execution log
- Service unit test results
- Form functional testing checklist

---

### TC#2: RBAC Configuration

**Test Objective:** Verify role-based access control restricts actions per role assignment.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | Person 5 (Database), Person 4 (Service), Person 3 & 1 (Forms) |
| **Test Timeline** | End of Week 2 (after RBAC setup) |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `04_Users_Creation.sql` - Create test users with role assignments | Critical | Week 2 - Early |
| Person 5 | `05_RBAC_Setup.sql` - CREATE ROLE, GRANT statements | Critical | Week 2 - Early |
| Person 4 | `Services/OracleConnectionService.cs` | Prerequisite | Week 1 |
| Person 4 | `Services/AuthenticationService.cs` - Login(), ValidateUserRole() | Critical | Week 2 - Early |
| Person 4 | `Services/RBACService.cs` - CheckUserRole(), CheckPermission(), GetAvailableActions() | Required | Week 2 |
| Person 1 | `Forms/MainForm.cs` (menu/button enablement based on role) | Required | Week 3 |
| Person 3 | `Forms/LoginForm.cs` (redirect to role-specific forms) | Required | Week 3 |

**Pass Criteria:**
- ✓ Four roles created in database: COORDINATOR, DOCTOR, TECHNICIAN, PATIENT
- ✓ Appropriate permissions granted to each role
- ✓ Test users created and assigned to roles correctly
- ✓ AuthenticationService.Login() returns correct role for valid credentials
- ✓ AuthenticationService.ValidateUserRole() correctly verifies role assignments
- ✓ RBACService.CheckUserRole() returns user's role from database
- ✓ RBACService.CheckPermission() verifies user has action permission (whitelist check)
- ✓ RBACService.GetAvailableActions() returns complete list for user's role
- ✓ Coordinator can perform coordinator actions, not doctor actions
- ✓ Doctor can perform doctor actions, not technician actions
- ✓ Technician cannot access coordinator or doctor functions
- ✓ Patient cannot access staff functions
- ✓ MainForm enables/disables buttons based on user's role
- ✓ LoginForm opens correct role-specific form (Coordinator/Doctor/Technician/Patient)
- ✓ All RBAC checks complete in < 100ms

---

### TC#3: VPD Implementation

**Test Objective:** Verify Virtual Private Database row-level security filtering at database level.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | Person 5 (Database), Person 4 (Service), Person 3 (Form) |
| **Test Timeline** | End of Week 2 (after VPD setup) |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `01_CreateTables.sql` - BỆNHNHÂN, HSBA, HSBA_DV tables | Critical | Week 1 |
| Person 5 | `02_CreateIndexes.sql` - Performance optimization | Important | Week 1 |
| Person 5 | `03_InsertSampleData.sql` - 100 patients, doctor-patient assignments | Critical | Week 1 |
| Person 5 | `06_VPD_Setup.sql` - VPD policies on HSBA, HSBA_DV with WHERE conditions | Critical | Week 2 - Early |
| Person 4 | `Services/OracleConnectionService.cs` | Prerequisite | Week 1 |
| Person 4 | `Services/AuthenticationService.cs` | Prerequisite | Week 2 |
| Person 4 | `Services/VPDService.cs` - GetVisiblePatients(), GetVisibleRecords(), GetVisibleServices() | Required | Week 2 |
| Person 4 | `Services/RBACService.cs` | Prerequisite | Week 2 |
| Person 3 | `Forms/DoctorForm.cs` - Display only assigned patients (VPD filtered) | Required | Week 3 |
| Person 3 | `Forms/TechnicianForm.cs` - Display only assigned services (VPD filtered) | Required | Week 3 |

**Pass Criteria:**
- ✓ VPD policies created and attached to HSBA and HSBA_DV tables
- ✓ VPD policy functions return proper WHERE clause conditions
- ✓ Database testing: Doctor query returns only assigned patients
- ✓ Database testing: Technician query returns only assigned services
- ✓ VPDService.GetVisiblePatients() returns only doctor's assigned patients
- ✓ VPDService.GetVisibleRecords() returns only staff's authorized records
- ✓ VPDService.GetVisibleServices() returns only technician's assigned services
- ✓ DoctorForm displays only assigned patients in DataGrid (0 excluded patients visible)
- ✓ TechnicianForm displays only assigned services in DataGrid
- ✓ Doctor cannot access patient records via direct SQL query (VPD enforced)
- ✓ VPD filtering transparent to application (no changes to form code needed)
- ✓ VPD overhead < 10% performance impact
- ✓ Filtering works correctly with NULL assignments (returns no records if no assignment)

---

### TC#4: Technician Access

**Test Objective:** Verify technician role isolation prevents access to unassigned services.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | Person 5 (Database, Service), Person 3 (Form) |
| **Test Timeline** | End of Week 2-3 (after VPD and services ready) |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `01_CreateTables.sql` - HSBA_DV (diagnostic services table) | Prerequisite | Week 1 |
| Person 5 | `03_InsertSampleData.sql` - 50 technicians, service assignments | Prerequisite | Week 1 |
| Person 5 | `06_VPD_Setup.sql` - VPD policy on HSBA_DV for technician filtering | Critical | Week 2 |
| Person 5 | `Services/TechnicianService.cs` - GetAssignedServices(), UpdateServiceResult(), CompleteService() | Required | Week 2 |
| Person 4 | `Services/AuthenticationService.cs` - Authenticate technician users | Prerequisite | Week 2 |
| Person 4 | `Services/VPDService.cs` - GetVisibleServices() | Prerequisite | Week 2 |
| Person 3 | `Forms/TechnicianForm.cs` - Display assigned services, update results, mark complete | Required | Week 3 |

**Pass Criteria:**
- ✓ Technician role created with appropriate permissions
- ✓ 50 sample technicians created in database
- ✓ Service assignments created linking technicians to HSBA_DV records
- ✓ VPD policy on HSBA_DV filters results by MÃNV (technician ID)
- ✓ TechnicianService.GetAssignedServices() returns only assigned services
- ✓ TechnicianService cannot update results for services not assigned
- ✓ TechnicianForm displays technician's assigned services only
- ✓ TechnicianForm cannot view/edit services assigned to other technicians
- ✓ TechnicianForm UpdateResults() validates technician has permission
- ✓ TechnicianForm CompleteService() marks service as complete
- ✓ Cannot access other technician's services via form or direct service method
- ✓ Service status updates properly recorded with timestamp
- ✓ Audit trail records who updated what service

---

### TC#5: Patient Self-Service Access

**Test Objective:** Verify patient role isolation restricts access to own records only.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | Person 5 (Database, Service), Person 3 (Form) |
| **Test Timeline** | End of Week 3 (after all services ready) |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `01_CreateTables.sql` - BỆNHNHÂN, HSBA, ĐƠNTHUỐC tables | Prerequisite | Week 1 |
| Person 5 | `03_InsertSampleData.sql` - 100 patients, medical records, prescriptions | Prerequisite | Week 1 |
| Person 5 | `Services/PatientService.cs` - GetPatient(), GetMyMedicalRecords(), GetMyPrescriptions(), UpdatePatientInfo() | Required | Week 2 |
| Person 4 | `Services/AuthenticationService.cs` - Authenticate patient users | Prerequisite | Week 2 |
| Person 3 | `Forms/LoginForm.cs` - Authenticate patients | Prerequisite | Week 3 |
| Person 3 | `Forms/PatientForm.cs` - Display own records, prescriptions, appointments; edit contact info | Required | Week 3 |

**Pass Criteria:**
- ✓ Patient role created with SELECT on own records only
- ✓ 100 sample patients created with medical records and prescriptions
- ✓ PatientService.GetPatient() returns patient's own information
- ✓ PatientService.GetMyMedicalRecords() returns only authenticated patient's records
- ✓ PatientService.GetMyPrescriptions() returns only patient's prescriptions
- ✓ PatientService.UpdatePatientInfo() updates only contact info (address, phone, email)
- ✓ Patient cannot update medical data (diagnosis, treatment, prescription)
- ✓ PatientForm displays authenticated patient's name, ID, contact info
- ✓ PatientForm displays patient's medical records in read-only DataGrid
- ✓ PatientForm displays patient's prescriptions in read-only DataGrid
- ✓ PatientForm displays patient's appointment history
- ✓ Patient cannot access other patient's records
- ✓ Contact info edit functionality works (saves to database)
- ✓ Medical data fields are read-only (cannot be modified)
- ✓ Row-level security enforced at database level (not just application)

---

## Requirement 2: Notification System with OLS (5 points)

### OLS#1: Label Hierarchy Configuration

**Test Objective:** Verify Oracle Label Security 3-level label hierarchy created and functional.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 2: OLS Notification System |
| **Primary Owner** | Person 5 (Database) |
| **Test Timeline** | End of Week 2 (after OLS setup) |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `01_CreateTables.sql` - Notification table with label fields | Required | Week 1 |
| Person 5 | `07_OLS_Setup.sql` - Create label hierarchy (Departments, Locations, Classifications) | Critical | Week 2 |

**Pass Criteria:**
- ✓ OLS policy created using DBMS_MACADM package
- ✓ Level 1 component created: DEPARTMENTS with 3 values (Cardiology, Gastroenterology, Neurology)
- ✓ Level 2 component created: LOCATIONS with 3 values (Hồ Chí Minh, Hải Phòng, Hà Nội)
- ✓ Level 3 component created: CLASSIFICATIONS with 3 values (Staff, DepartmentHead, Director)
- ✓ Label hierarchy properly defined with correct component orders
- ✓ Policy attached to Notification table
- ✓ Notification table has label columns: Department, Location, Classification
- ✓ OLS policies are active and enforced
- ✓ Script executes without errors
- ✓ Labels are retrievable via database queries

---

### OLS#2: User Label Assignment

**Test Objective:** Verify 8 test users assigned appropriate OLS labels with label-based access control.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 2: OLS Notification System |
| **Primary Owner** | Person 5 (Database), Person 4 (Service), Person 3 (Form) |
| **Test Timeline** | End of Week 3 (after OLS service and form ready) |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `04_Users_Creation.sql` - Create 8 test users with various roles | Required | Week 2 |
| Person 5 | `07_OLS_Setup.sql` - User label assignment (2 Directors, 2 Dept Heads, 4 Staff) | Critical | Week 2 |
| Person 5 | `03_InsertSampleData.sql` - 15 notifications with varied labels | Required | Week 1 |
| Person 4 | `Services/OracleConnectionService.cs` | Prerequisite | Week 1 |
| Person 4 | `Services/AuthenticationService.cs` | Prerequisite | Week 2 |
| Person 4 | `Services/OLSService.cs` - GetUserLabels(), CanAccessNotification(), GetAccessibleNotifications() | Required | Week 2 |
| Person 3 | `Forms/NotificationForm.cs` - Display OLS-filtered notifications | Required | Week 3 |

**Pass Criteria:**
- ✓ 8 test users created: 2 Directors, 2 Department Heads, 4 Staff
- ✓ Directors assigned highest classification level (Director)
- ✓ Department Heads assigned medium classification level (DepartmentHead)
- ✓ Staff users assigned minimum classification level (Staff)
- ✓ All users assigned to all three label components (Department, Location, Classification)
- ✓ OLSService.GetUserLabels() retrieves correct labels for each user
- ✓ OLSService.CanAccessNotification() verifies label compatibility correctly
- ✓ Director can access notifications at any label level (all 15 notifications)
- ✓ Department Head can access own department notifications + lower classifications
- ✓ Staff can access only notifications matching their exactly matching labels
- ✓ OLSService.GetAccessibleNotifications() returns only accessible notification IDs
- ✓ NotificationForm displays only filtered notifications per user's labels
- ✓ Notification content shows title, content, and label information
- ✓ Label-based filtering transparent to application form (database enforced)

---

## Requirement 3: Audit & Monitoring (5 points)

### AUD#1: Standard Audit Configuration

**Test Objective:** Verify Oracle standard audit trails record user actions for compliance.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 3: Audit & Monitoring |
| **Primary Owner** | Person 5 (Database), Person 5 (Service) |
| **Test Timeline** | End of Week 2 (after audit setup) |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `08_StandardAudit_Setup.sql` - Enable auditing on tables and user connections | Critical | Week 2 |
| Person 5 | `Services/AuditService.cs` - LogUserAction(), GetAuditLogs(), LogSensitiveAccess() | Required | Week 2 |
| Person 5 | `Database/Audit/ReadAuditLogs.sql` - Sample queries for audit log analysis | Required | Week 2 |

**Pass Criteria:**
- ✓ Standard audit enabled at database level (AUDIT ALL STATEMENTS)
- ✓ AUDIT CONNECT logs user logins
- ✓ AUDIT DISCONNECT logs user logouts
- ✓ AUDIT SELECT, INSERT, UPDATE, DELETE ON HSBA logs medical record operations
- ✓ AUDIT INSERT, UPDATE, DELETE ON BỆNHNHÂN logs patient data modifications
- ✓ AUDIT INSERT, UPDATE, DELETE ON ĐƠNTHUỐC logs prescription changes
- ✓ DBA_AUDIT_TRAIL table populated with audit records
- ✓ AuditService.LogUserAction() inserts custom audit records
- ✓ AuditService.GetAuditLogs() retrieves audit trail with date range filtering
- ✓ Audit records include: UserId, ActionTime, Action, TableName, RecordId, Details, IPAddress
- ✓ Audit logs queryable by user, date range, and operation type
- ✓ Audit records immutable (cannot be modified after insertion)
- ✓ Performance impact minimal (< 5% overhead)
- ✓ Audit trail sufficient for compliance verification

---

### AUD#2: Fine-Grained Audit Configuration

**Test Objective:** Verify fine-grained audit tracks sensitive data access and modifications.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 3: Audit & Monitoring |
| **Primary Owner** | Person 5 (Database) |
| **Test Timeline** | End of Week 2 (after FGA setup) |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `09_FineGrainedAudit_Setup.sql` - Create FGA policies for sensitive operations | Critical | Week 2 |
| Person 5 | `Database/Audit/ReadAuditLogs.sql` - Queries to analyze FGA_LOG$ | Required | Week 2 |

**Pass Criteria:**
- ✓ FGA policy created on HSBA for INSERT, UPDATE, DELETE operations
- ✓ FGA policy created on ĐƠNTHUỐC for all operations (ALL)
- ✓ FGA policy created on BỆNHNHÂN for SELECT operations on sensitive columns
- ✓ FGA policies use DBMS_FGA.ADD_POLICY correctly
- ✓ FGA_LOG$ table populated with policy violation events
- ✓ FGA records include: DBMS_SESSION, statement, timestamp, sensitive data identifier
- ✓ Sensitive field changes logged in FGA_LOG$ with before/after values
- ✓ Prescription INSERT/UPDATE/DELETE operations tracked in FGA
- ✓ Medical record modifications tracked in FGA
- ✓ Patient data access logged (SELECT on phone, address fields)
- ✓ Can query FGA logs to identify access patterns
- ✓ FGA policies do not significantly impact performance

---

## Requirement 4: Backup & Recovery (5 points)

### BAK#1: RMAN Backup Implementation

**Test Objective:** Verify RMAN backup configuration and successful backup creation.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 4: Backup & Recovery |
| **Primary Owner** | Person 5 (Database Admin) |
| **Test Timeline** | Week 3-4 |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `Database/BackupRestore/Backup_Recovery_Documentation.md` - RMAN strategy section | Required | Week 3 |

**Pass Criteria:**
- ✓ RMAN configured with proper backup destination
- ✓ Incremental backup strategy documented (Level 0 full backup + Level 1 incremental)
- ✓ Retention policies defined (RMAN retention policy set)
- ✓ Full backup can be executed successfully
- ✓ Backup files created and verified
- ✓ Backup metadata recorded in RMAN catalog/control file
- ✓ Backup size and duration acceptable
- ✓ Multiple backup copies maintained (redundancy)
- ✓ Archive logs included in backup strategy
- ✓ Documentation includes commands for: CREATE BACKUP, BACKUP RECOVERY OPTIONS, LIST BACKUPS

---

### BAK#2: Recovery Testing

**Test Objective:** Verify recovery procedures restore database to known states successfully.

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 4: Backup & Recovery |
| **Primary Owner** | Person 5 (Database Admin) |
| **Test Timeline** | Week 4 (final integration testing) |

**Required Deliverables:**

| Person | Deliverable | Status | Completion Date |
|--------|------------|--------|-----------------|
| Person 5 | `Database/BackupRestore/Backup_Recovery_Documentation.md` - Complete recovery procedures | Required | Week 3 |

**Pass Criteria:**
- ✓ Recovery procedures documented: Full recovery, Point-in-time recovery, Selective object recovery
- ✓ Full recovery tested: Database recovered from backup to full operational state
- ✓ Point-in-time recovery tested: Database recovered to specific timestamp
- ✓ Selective object recovery tested: Specific table recovered without full restore
- ✓ Recovered data verified to match backup state
- ✓ Integrity checks (DBVERIFY) pass after recovery
- ✓ Recovered database accessible to applications
- ✓ RTO (Recovery Time Objective) measured and documented (target: < 30 minutes)
- ✓ RPO (Recovery Point Objective) measured and documented (target: < 1 hour)
- ✓ Recovery procedures tested from cold backup (database completely down)
- ✓ Test results documented with timestamps and data integrity verification

---

## Summary: Test Case Dependency Order

### Phase 1: Foundation (Week 1 - Must Complete Before Phase 2)
- TC#1 Foundation: Database tables and sample data created

### Phase 2: Security Configuration (Week 2 - Dependent on Phase 1)
- TC#1 Services: UserService and ValidationService implemented
- TC#2 Prerequisites: Roles created, RBAC setup, authentication service
- TC#3 Prerequisites: VPD policies created
- OLS#1: Label hierarchy created
- AUD#1: Standard audit configured
- AUD#2: Fine-grained audit configured
- BAK#1: RMAN backup configured

### Phase 3: Application Implementation (Week 3 - Dependent on Phase 2)
- TC#1 Complete: UserManagementForm implemented
- TC#2 Complete: RBAC verified through forms
- TC#3 Complete: VPD filtering verified in DoctorForm, TechnicianForm
- TC#4 Complete: TechnicianForm tests technician access
- TC#5 Complete: PatientForm tests patient access
- OLS#2 Complete: NotificationForm tests OLS filtering
- BAK#2 Prep: Recovery procedures tested

### Phase 4: Testing & Verification (Week 4)
- All test cases verified end-to-end
- Integration testing across subsystems
- Security testing (cannot bypass VPD, RBAC, OLS)
- Audit trail verification
- Backup recovery testing
- Performance benchmarking
- Final bug fixes

---

## Person Responsibility Summary

| Person | Test Cases Owned | Deliverables Count | Critical Path |
|--------|------------------|-------------------|----------------|
| Person 1 | TC#1, TC#2 | 5 forms | Depends on Person 2 (Week 2) |
| Person 2 | TC#1, TC#2 | 6 services | Depends on Person 5 (Week 1) |
| Person 3 | TC#3, TC#4, TC#5, OLS#2 | 7 forms | Depends on Persons 4, 5 (Week 2) |
| Person 4 | TC#2, TC#3, OLS#1, OLS#2, AUD | 6 services | Depends on Person 5 (Week 1-2) |
| Person 5 | All (critical blocker) | 21 total | **CRITICAL PATH - Must deliver Week 1** |

**Critical Success Factor:** Person 5's Week 1 deliverables are the single biggest blocker. All other work waits on:
- `01_CreateTables.sql` completion
- `02_CreateIndexes.sql` completion  
- `03_InsertSampleData.sql` completion
- Connection testing verification

Without these three files complete by **Friday, February 13, 2026 (End of Week 1)**, the entire team schedule slips.

