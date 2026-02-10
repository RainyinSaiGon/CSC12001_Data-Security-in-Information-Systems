# Task Assignment Summary - Organized by Deliverable

**Document Purpose:** Map task files (docs/tasks/) to original TASK_ASSIGNMENT.md assignments  
**Created:** February 10, 2026  
**Status:** Complete

---

## Task Assignments Overview

| Task File | Deliverable | Assigned To | Hours | Priority | Due Date |
|-----------|-------------|-------------|-------|----------|----------|
| task-01 | Subsystem 1 Database Admin UI Forms | Person 1 | 20-25 | High | Feb 28 |
| task-02 | Subsystem 1 Business Logic Services | Person 2 | 25-30 | Critical | Feb 21 |
| task-03 | Subsystem 2 Medical UI Forms | Person 3 | 25-30 | High | Feb 28 |
| task-04 | Subsystem 2 Security Services | Person 4 | 30-35 | Critical | Feb 28 |
| task-05 | Subsystem 2 Medical Business Services | Person 5 | 20 | High | Feb 28 |
| task-06 | Database Schema Setup (Tables/Indexes/Data) | Person 5 | 8 | **CRITICAL** | **Feb 14** |
| task-07 | Database Security Setup (RBAC/VPD/OLS) | Person 5 | 10 | Critical | Feb 21 |
| task-08 | Database Audit Setup (Logging/Backup Docs) | Person 5 | 7 | Medium-High | Feb 28 |

**Total Project Hours:** 155-175 hours across 5 team members over 4 weeks

---

## Cross-Reference: TASK_ASSIGNMENT.md → Task Files

### Person 1: Subsystem 1 - Database Administrator UI

**Original Assignment (TASK_ASSIGNMENT.md, Line 11):**

- Focus: Forms and User Interface Design
- Estimated Hours: 20-25 hours
- Priority Level: High

**Task File:** [task-01-subsystem1-database-admin-ui.md](task-01-subsystem1-database-admin-ui.md)

- Forms to implement: 5 (MainForm, UserManagementForm, RoleManagementForm, PermissionForm, PrivilegeViewerForm)
- Timeline: Feb 18 - Feb 28
- Depends on: Person 5 database (Fri 2/14), Person 2 services (Wed 2/19)

---

### Person 2: Subsystem 1 - Business Logic Services

**Original Assignment (TASK_ASSIGNMENT.md, Line 138):**

- Focus: Service Layer Implementation and Database Integration
- Estimated Hours: 25-30 hours
- Priority Level: Critical

**Task File:** [task-02-subsystem1-business-services.md](task-02-subsystem1-business-services.md)

- Services to implement: 6 (OracleConnectionService, ValidationService, UserService, RoleService, PermissionService, PrivilegeService)
- Timeline: Feb 10 - Feb 21
- Critical item: OracleConnectionService (implement first - blocks all others)
- Depends on: Person 5 database setup (Fri 2/14)

---

### Person 3: Subsystem 2 - Medical Data Management UI Forms

**Original Assignment (TASK_ASSIGNMENT.md, Line 340):**

- Focus: Role-Based User Interface Implementation and Data Display
- Estimated Hours: 25-30 hours
- Priority Level: High

**Task File:** [task-03-subsystem2-medical-ui-forms.md](task-03-subsystem2-medical-ui-forms.md)

- Forms to implement: 7 (LoginForm, CoordinatorForm, DoctorForm, TechnicianForm, PatientForm, NotificationForm)
- Timeline: Feb 18 - Feb 28
- Critical: LoginForm (entry point, implement first)
- Depends on: Person 4 security services (Fri 2/21), Person 5 business services (Fri 2/21)

---

### Person 4: Subsystem 2 - Security Services

**Original Assignment (TASK_ASSIGNMENT.md, Line 645):**

- Focus: Security Mechanisms and Access Control Implementation
- Estimated Hours: 30-35 hours
- Priority Level: Critical

**Task File:** [task-04-subsystem2-security-services.md](task-04-subsystem2-security-services.md)

- Services to implement: 6 (AuthenticationService, OracleConnectionService, RBACService, VPDService, OLSService, ValidationService)
- Timeline: Feb 19 - Feb 28
- Critical: AuthenticationService (implement first - foundation for all security)
- Implements: RBAC (role-based access), VPD (row-level security), OLS (label-based security)
- Depends on: Person 5 database users and security setup (Wed 2/19 RBAC, Thu 2/20 VPD/OLS)

---

### Person 5: Subsystem 2 - Business Services and Database Setup

**Original Assignment (TASK_ASSIGNMENT.md, Line 924):**

- Focus: Database Administration and Business Logic Service Implementation
- Estimated Hours: 35-40 hours
- Priority Level: Critical

**Task File(s):**

#### Part A: Business Services

[task-05-subsystem2-business-services.md](task-05-subsystem2-business-services.md)

- Services to implement: 5 (PatientService, DoctorService, CoordinatorService, TechnicianService, AuditService)
- Hours: 20
- Timeline: Feb 21 - Feb 28

#### Part B: Database Setup

[task-06-database-schema-setup.md](task-06-database-schema-setup.md)

- SQL Scripts: 3 (CreateTables, CreateIndexes, InsertSampleData)
- Hours: 8
- Timeline: Feb 10 - Feb 14 (MUST BE COMPLETE BY FRI 2/14 - BLOCKS EVERYONE)
- **Critical Deadline:** Friday February 14 EOD (non-negotiable)

[task-07-database-security-setup.md](task-07-database-security-setup.md)

- SQL Scripts: 3 (Users_Creation, RBAC_Setup, VPD_Setup, OLS_Setup)
- Hours: 10
- Timeline: Feb 17 - Feb 21
- Milestone 1: Wed 2/19 (RBAC) - Unblocks Person 4 AuthenticationService
- Milestone 2: Thu 2/20 (VPD/OLS) - Unblocks Person 4 remaining services and Person 3 forms

[task-08-database-audit-setup.md](task-08-database-audit-setup.md)

- SQL Scripts: 4 (StandardAudit, FineGrainedAudit, UnifiedAudit, ReadAuditLogs)
- Documentation: 1 (Backup & Recovery guide)
- Hours: 7
- Timeline: Feb 21 - Feb 28

**Total for Person 5:** 35-40 hours across 3 tasks/4 SQL script groups

---

## Critical Dependencies and Timeline

```
Week 1 (Feb 10-14): Foundation Phase
├─ Person 5: Database setup (CRITICAL BLOCKER)
│  └─ Task 06: Schema, indexes, sample data
│     Deadline: FRI 2/14 EOD (blocks all others)
└─ Person 1-4: Can begin design/framework while waiting

Week 2 (Feb 17-21): Services Implementation
├─ Person 5: Security setup
│  ├─ Task 07a: RBAC + Users (Wed 2/19)
│  │  └─ Unblocks Person 4 AuthenticationService
│  ├─ Task 07b: VPD + OLS (Thu 2/20)
│  │  └─ Unblocks Person 4 remaining services + Person 3 forms
│  └─ Task 05: Business services (Fri 2/21)
│     └─ Unblocks Forms testing
│
├─ Person 2: Business services
│  ├─ Task 02: All 6 services by Fri 2/21
│  │  └─ Unblocks Person 1 form integration
│  
├─ Person 4: Security services
│  ├─ AuthenticationService (Wed 2/19)
│  ├─ RBACService (Wed 2/19)
│  ├─ VPDService (Thu 2/20)
│  └─ OLSService (Thu 2/20)
│
└─ Person 3: Can start LoginForm design (Fri 2/21 ready to integrate)

Week 3 (Feb 24-28): UI Implementation & Testing
├─ Person 1: All forms implemented
│  └─ Task 01: 5 forms complete by Fri 2/28
├─ Person 3: All forms implemented
│  └─ Task 03: 7 forms complete by Fri 2/28
└─ Person 4: Services complete
   └─ Task 04: All 6 services complete by Fri 2/28

Week 4 (Mar 3-7): Integration and System Testing
├─ All: End-to-end testing
├─ All: Security verification (RBAC, VPD, OLS)
├─ All: Audit and backup testing
└─ All: Final fixes and documentation
```

---

## Files in docs/tasks/ Directory

All task files follow consistent structure:

```
docs/tasks/
├── task-01-subsystem1-database-admin-ui.md
├── task-02-subsystem1-business-services.md
├── task-03-subsystem2-medical-ui-forms.md
├── task-04-subsystem2-security-services.md
├── task-05-subsystem2-business-services.md
├── task-06-database-schema-setup.md
├── task-07-database-security-setup.md
└── task-08-database-audit-setup.md
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

✓ Task 01: Person 1 assignments complete  
✓ Task 02: Person 2 assignments complete  
✓ Task 03: Person 3 assignments complete  
✓ Task 04: Person 4 assignments complete  
✓ Task 05: Person 5 Part A (Business Services) complete  
✓ Task 06: Person 5 Part B (Database Schema) complete  
✓ Task 07: Person 5 Part B (Database Security) complete  
✓ Task 08: Person 5 Part B (Database Audit) complete  

### Assignment Verification

✓ All 5 people have clear task assignments  
✓ Hours estimated for each task  
✓ Priority levels defined  
✓ Deadlines specified  
✓ Dependencies documented  
✓ Success criteria provided  
✓ Cross-references added  

### Consistency Check

✓ Task file assignments match TASK_ASSIGNMENT.md  
✓ All original deliverables covered  
✓ Task-based organization (not person-based)  
✓ Reduced file size per task (concise, focused)  
✓ Clear relationships between tasks  
✓ Critical path identified and documented  

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
2. Monitor critical blockers (Person 5 Fri 2/14)
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

## Summary

The original `TASK_ASSIGNMENT.md` (1529 lines), `TRACEABILITY_MATRIX.md`, `IMPLEMENTATION.md`, and `AUDITLOGS.md` have been reorganized into 8 focused task files:

- **Cleaner structure:** Each file covers one major deliverable
- **Easier assignment:** Team members can focus on their specific tasks
- **Better tracking:** Concrete success criteria for each task
- **Reduced complexity:** Concise scope without verbose repetition
- **Cross-referenced:** Clear dependencies and relationships

All assignments remain unchanged from original - just better organized for execution.

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

- Week 1: Database setup complete (Person 5)
- Week 2: All services implemented (Persons 2, 4, 5)
- Week 3: All forms completed (Persons 1, 3)
- Week 4: Integration testing and security verification

### Requirement 2: OLS Notification System (5 points)

**Milestone Targets:**

- Week 2: OLS label hierarchy + user labels (Person 5)
- Week 3: OLS service (Person 4) + Notification form (Person 3)

### Requirement 3: Audit & Monitoring (5 points)

**Milestone Targets:**

- Week 2: Standard + fine-grained audit configured (Person 5)
- Week 3: Audit service implemented (Person 5)
- Week 4: 5+ test scenarios executed and documented

### Requirement 4: Backup & Recovery (5 points)

**Milestone Targets:**

- Week 2: Backup strategy documented (Person 5)
- Week 3: RMAN backup configured (Person 5)
- Week 4: Recovery procedures tested (Person 5)

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
