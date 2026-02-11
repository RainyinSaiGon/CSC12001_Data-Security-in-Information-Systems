# Task Assignment Summary - Organized by Deliverable

**Document Purpose:** Map task files (docs/tasks/) to original TASK_ASSIGNMENT.md assignments  
**Created:** February 10, 2026  
**Status:** Complete

---

## Task Assignments Overview

| Task File | Deliverable | Assigned To | Hours | Priority | Due Date |
|---|---|---|---|---|---|
| **SUBSYSTEM 1: Database Admin Tool** | | | | | |
| task-01 | Subsystem 1 Database Admin UI Forms | Duyên, Triết | 20-25 | High | Mar 21 |
| task-02 | Subsystem 1 Business Logic Services | Duyên, Triết | 25-30 | Critical | Mar 21 |
| task-03 | **Subsystem 1 Database Schema Design** | **Duyên, Triết** | **10** | **High** | **Feb 20** |
| **SUBSYSTEM 2: Medical Data Management** | | | | | |
| task-04 | Subsystem 2 Medical UI Forms | Duyên | 25-30 | High | Mar 28 |
| task-05 | Subsystem 2 Security Services | Phôn | 30-35 | Critical | Mar 28 |
| task-06 | Subsystem 2 Medical Business Services | Phôn | 20 | High | Mar 28 |
| task-07 | Subsystem 2 Database Schema Setup | Ngọc, Vũ | 8 | **CRITICAL** | **Feb 14** |
| task-08 | Subsystem 2 Database Security Setup | Ngọc, Vũ | 10 | Critical | Feb 21 |
| task-09 | Subsystem 2 Database Audit Setup | Ngọc, Vũ | 7 | Medium-High | Feb 28 |
| task-10 | Subsystem 2 Database Backup & Recovery | Ngọc, Vũ | 8 | Medium-High | Mar 7 |

**Total Project Hours:** 194-213 hours across 5 team members over 4 weeks
**Key Difference:** Subsystem 1 now has its own separate database (Task 3) distinct from Subsystem 2 (Tasks 7-10)

---

## Cross-Reference: TASK_ASSIGNMENT.md → Task Files

### Duyên, Triết: Subsystem 1 - Database Administrator UI & Services

**Assignments:**

- **Task 01:** [task-01-subsystem1-database-admin-ui.md](task-01-subsystem1-database-admin-ui.md)
  - Forms: MainForm, UserManagementForm, RoleManagementForm, PermissionForm, PrivilegeViewerForm
  - **Due: Mar 21**

- **Task 02:** [task-02-subsystem1-business-services.md](task-02-subsystem1-business-services.md)
  - Services: OracleConnectionService, ValidationService, UserService, RoleService, PermissionService, PrivilegeService
  - **Due: Mar 21**

- **Task 03:** [task-03-subsystem1-database-schema.md](task-03-subsystem1-database-schema.md)
  - Database Schema Design: Tables, Views, Stored Procedures, Functions
  - Support permissions on TABLE, VIEW, STORED PROCEDURE, FUNCTION
  - Column-level grants (SELECT, UPDATE) + Object-level grants (INSERT, DELETE)
  - WITH GRANT OPTION delegation support
  - **Due: Feb 20**

---

### Duyên: Subsystem 2 - Medical Data Management UI Forms

**Assignment:**

- **Task 04:** [task-04-subsystem2-medical-ui-forms.md](task-04-subsystem2-medical-ui-forms.md)
  - Forms: LoginForm, CoordinatorForm, DoctorForm, TechnicianForm, PatientForm, NotificationForm
  - **Due: Mar 28**

---

### Phôn: Subsystem 2 - Security & Business Services

**Assignments:**

- **Task 05:** [task-05-subsystem2-security-services.md](task-05-subsystem2-security-services.md)
  - Security Services: AuthenticationService, RBACService, VPDService, OLSService
  - **Due: Mar 28**

- **Task 06:** [task-06-subsystem2-business-services.md](task-06-subsystem2-business-services.md)
  - Business Services: PatientService, DoctorService, CoordinatorService, TechnicianService, AuditService
  - **Due: Mar 28**

---

### Ngọc, Vũ: Subsystem 2 - Medical Database Setup & Security

**Assignments:**

- **Task 07:** [task-07-database-schema-setup.md](task-07-database-schema-setup.md)
  - Schema, Indexes, Sample Data
  - **Due: Feb 14**

- **Task 08:** [task-08-database-security-setup.md](task-08-database-security-setup.md)
  - RBAC, VPD, OLS Setup
  - **Due: Feb 21**

- **Task 09:** [task-09-database-audit-setup.md](task-09-database-audit-setup.md)
  - Standard, FGA, Unified Audit
  - **Due: Feb 28**

- **Task 10:** [task-10-database-backup-recovery.md](task-10-database-backup-recovery.md)
  - Backup & Recovery Procedures
  - **Due: Mar 7**

---

## Critical Dependencies and Timeline

```
Week 1 (Feb 10-14): Foundation Phase
├─ Duyên, Triết: Database schema design (CRITICAL FOUNDATION)
│  └─ Task 03: Schema, tables, views, procedures, functions
│     Deadline: WED 2/20 EOD (design review)
├─ Ngọc, Vũ: Medical database setup (CRITICAL BLOCKER)
│  └─ Task 07: Schema, indexes, sample data
│     Deadline: FRI 2/14 EOD (blocks security/audit setup)
└─ Duyên, Triết, Phôn: Can begin parallel work

Week 2 (Feb 17-21): Services Implementation
├─ Ngọc, Vũ: Database security setup
│  ├─ Task 08a: RBAC + Users (Wed 2/19)
│  │  └─ Unblocks Phôn's AuthenticationService
│  ├─ Task 08b: VPD + OLS (Thu 2/20)
│  │  └─ Unblocks Phôn's remaining services + Duyên's forms
│  └─ Task 09: Audit setup (Fri 2/21)
│     └─ Completes database security layer
│
├─ Duyên, Triết: Business services (Subsystem 1)
│  ├─ Task 02: All 6 services by Fri 2/21
│  │  └─ Unblocks Duyên, Triết form integration
│  
├─ Phôn: Security & business services (Subsystem 2)
│  ├─ Task 05: AuthenticationService, RBACService, VPDService, OLSService (Wed 2/19-20)
│  │  └─ Integrates with database security layer
│  └─ Task 06: Business services by Fri 2/21
│
└─ Duyên: Can start LoginForm design (Fri 2/21 ready to integrate)

Week 3 (Feb 24-28): UI Implementation & Testing
├─ Duyên, Triết: All Subsystem 1 forms implemented
│  └─ Task 01: 5 admin forms complete by Fri 2/28
├─ Duyên: All Subsystem 2 medical forms implemented
│  └─ Task 04: 7 medical forms complete by Fri 2/28
└─ Ngọc, Vũ: Backup & recovery setup
   └─ Task 10: Backup & recovery procedures by Fri 3/7

Week 4 (Mar 3-7): Integration and System Testing
├─ All: End-to-end testing
├─ All: Security verification (RBAC, VPD, OLS)
├─ All: Audit trail validation
└─ All: Final fixes and documentation
```

---

## Files in docs/tasks/ Directory

All task files follow consistent structure:

```
docs/tasks/
├── task-01-subsystem1-database-admin-ui.md
├── task-02-subsystem1-business-services.md
├── task-03-subsystem1-database-schema.md              (NEW)
├── task-04-subsystem2-medical-ui-forms.md
├── task-05-subsystem2-security-services.md
├── task-06-subsystem2-business-services.md
├── task-07-database-schema-setup.md
├── task-08-database-security-setup.md
├── task-09-database-audit-setup.md
└── task-10-database-backup-recovery.md
```

Each file contains:

- **Overview:** What deliverable and why
- **Deliverables table:** Specific items to create
- **Requirements:** Technical specifications
- **Dependencies:** Blocking items and prerequisites
- **Success criteria:** Checkboxes for completion
- **Timeline:** Start/end dates
- **Related tasks:** Cross-references

---

## Verification Checklist

### Task File Coverage

- [ ] Task 01: Duyên, Triết - Subsystem 1 Admin UI Forms complete  
- [ ] Task 02: Duyên, Triết - Subsystem 1 Business Services complete  
- [ ] Task 03: Duyên, Triết - Subsystem 1 Database Schema Design complete  
- [ ] Task 04: Duyên - Subsystem 2 Medical UI Forms complete  
- [ ] Task 05: Phôn - Subsystem 2 Security Services complete  
- [ ] Task 06: Phôn - Subsystem 2 Business Services complete  
- [ ] Task 07: Ngọc, Vũ - Subsystem 2 Database Schema Setup complete  
- [ ] Task 08: Ngọc, Vũ - Subsystem 2 Database Security Setup complete  
- [ ] Task 09: Ngọc, Vũ - Subsystem 2 Database Audit Setup complete  
- [ ] Task 10: Ngọc, Vũ - Subsystem 2 Database Backup & Recovery complete  

### Assignment Verification

- [ ] All 5 people have clear task assignments  
- [ ] Hours estimated for each task  
- [ ] Priority levels defined  
- [ ] Deadlines specified  
- [ ] Dependencies documented  
- [ ] Success criteria provided  
- [ ] Cross-references added  

### Consistency Check

- [ ] Task file assignments match TASK_ASSIGNMENT.md  
- [ ] All original deliverables covered  
- [ ] Task-based organization (not person-based)  
- [ ] Reduced file size per task (concise, focused)  
- [ ] Clear relationships between tasks  
- [ ] Critical path identified and documented  

---

## How to Use These Task Files

**For Team Members:**

1. Find your assigned task file(s)
2. Read the Overview and understand scope
3. Review Dependencies - what must complete first
4. Review the Traceability Matrix section in your task file for pass criteria
5. Use Success Criteria checklist for validation
6. Report blockers/issues immediately

**For Project Manager:**

1. Track task completion using success criteria
2. Monitor critical blockers (Ngọc, Vũ Fri 2/14)
3. Use Timeline section for schedule verification
4. Cross-reference tasks to ensure dependencies met
5. Weekly status: Check which milestones completed

**For Technical Lead:**

1. Review task specifications
2. Verify technical requirements are clear
3. Assess resource requirements
4. Identify integration points between tasks
5. Plan code review ceremonies

---


## Test Cases Summary

| TC | Description | Related Req | Pass Criteria |
|----|-------------|-------------|---------------|
| TC#1 | User account setup | Req 1 | UserService creates users, Form displays/validates data, DB stores 170 staff |
| TC#2 | RBAC configuration | Req 1 | Roles created, users can only perform authorized actions |
| TC#3 | VPD implementation | Req 1 | Doctor sees only assigned patients, VPD transparent at DB level |
| TC#4 | Technician access | Req 1 | Technician sees only assigned services, cannot access other data |
| TC#5 | Patient self-service | Req 1 | Patient sees only own records, cannot edit medical data |
| OLS#1 | Label hierarchy | Req 2 | 3-level hierarchy (Dept, Location, Classification), OLS active |
| OLS#2 | User label assignment | Req 2 | 8 users labeled, label-based filtering works |
| AUD#1 | Standard audit | Req 3 | DBA_AUDIT_TRAIL populated, logins/operations tracked |
| AUD#2 | Fine-grained audit | Req 3 | FGA_LOG$ tracks sensitive field access |
| BAK#1 | RMAN backup | Req 4 | Backup executes, incremental strategy configured |
| BAK#2 | Recovery testing | Req 4 | Full/point-in-time recovery tested, RTO < 30 min |

---

## Compliance Checklist

- [ ] Both subsystems implemented
- [ ] All security mechanisms configured (RBAC, VPD, OLS)
- [ ] All 11 test cases passed
- [ ] Complete documentation
- [ ] Source code committed
- [ ] Database scripts functional
- [ ] All team members contribute

---

## Performance Targets

**Database Performance:**

| Metric | Target |
|--------|--------|
| Query response time | < 500 ms |
| Audit logging overhead | < 5% |
| VPD filtering overhead | < 10% |

**Application Performance:**

| Metric | Target |
|--------|--------|
| Login time | < 2 seconds |
| Data loading | < 3 seconds |
| Record search | < 5 seconds |

> [!NOTE]
> Actual measurements will be recorded during Week 4 testing.
> Performance issues will be logged as blockers if thresholds exceeded.

---

## Progress Tracking

### Requirement 1: Access Control & Interface (5 points)

**Milestone Targets:**

- Week 1: Database schemas designed (Task 03 - Duyên, Triết) & database setup (Task 07 - Ngọc, Vũ)
- Week 2: All services implemented (Task 02 - Duyên/Triết; Task 05-06 - Phôn; Task 08 - Ngọc/Vũ)
- Week 3: All forms completed (Task 01 - Duyên/Triết; Task 04 - Duyên)
- Week 4: Integration testing and security verification

### Requirement 2: OLS Notification System (5 points)

**Milestone Targets:**

- Week 2: OLS label hierarchy + user labels (Ngọc, Vũ)
- Week 3: OLS service (Phôn) + Notification form (Duyên)

### Requirement 3: Audit & Monitoring (5 points)

**Milestone Targets:**

- Week 2: Standard + fine-grained audit configured (Task 09 - Ngọc, Vũ)
- Week 3: Audit service implemented (Task 06 - Phôn)
- Week 4: 5+ audit test scenarios executed and documented

### Requirement 4: Backup & Recovery (5 points)

**Milestone Targets:**

- Week 2: Backup strategy documented (Task 10 - Ngọc, Vũ)
- Week 3: RMAN backup configured (Task 10 - Ngọc, Vũ)
- Week 4: Recovery procedures tested + RTO verification (Task 10 - Ngọc, Vũ)

---

## Security Assessment Checklist

- [ ] RBAC properly implemented
- [ ] VPD transparent filtering works
- [ ] OLS labels correctly assigned
- [ ] Audit trails immutable
- [ ] No security vulnerabilities found
- [ ] Data encryption implemented

---

**Document Status:** Ready for Project Execution  
**Last Updated:** February 10, 2026  
**Next Review:** After Week 1 completion (Feb 14)
