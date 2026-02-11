# Documentation

Complete documentation for CSC12001 - Data Security in Information Systems project.

## Quick Links

| Document | Purpose |
|----------|---------|
| [SETUP_GUIDE.md](SETUP_GUIDE.md) | **Complete setup guide for .NET 10.0 & Oracle 21c XE** |
| [ARCHITECTURE.md](ARCHITECTURE.md) | System design and technical architecture |
| [tasks/README.md](tasks/README.md) | Task assignments (10 tasks), requirements, & progress tracking |
| [tasks/task-03](tasks/task-03-subsystem1-database-schema.md) | Subsystem 1: Database schema design (NEW) |
| [tasks/task-07 to task-10](tasks/task-07-database-schema-setup.md) | Subsystem 2: Database, security, audit, backup |

## Overview

This documentation directory contains the following consolidated files:

### ARCHITECTURE.md

- System design diagrams and structure
- Entity relationship diagrams (Subsystem 1 Admin DB + Subsystem 2 Medical DB)
- Sequence diagrams for key operations
- Class diagrams
- Database schema design for both subsystems

### tasks/ (Organized Task Files - 10 Tasks)

- **Tasks 1-3**: Subsystem 1 - Database Admin Tool (Duyên, Triết)
  - task-01: Admin UI Forms
  - task-02: Business Services
  - task-03: Database Schema Design (NEW)
- **Tasks 4-10**: Subsystem 2 - Medical Data Management
  - task-04 to task-06: Application (Duyên, Phôn)
  - task-07 to task-10: Database setup (Ngọc, Vũ)
- Requirements, implementation guides, and test criteria
- Progress tracking, compliance checklists, and performance targets

## Main Topics

## 1. Database Design

### Oracle Database Express 21c (XE)

**Connection Details:**

- Service: localhost:1521/XE
- SQL*Plus: `sqlplus user/password@localhost:1521/XE`
- ODP.NET: `Data Source=localhost:1521/XE;User Id=user;Password=password;`

### Core Entities

- BENHNHAN (Patient)
- NHANVIEN (Staff)
- HSBA (Medical Record)
- HSBA_DV (Diagnostic Service)
- DONTHUOC (Prescription)
- THONGBAO (Notification)

### Key Relationships

```
BENHNHAN
  ├── HSBA (1:M - patient has multiple records)
  │   ├── HSBA_DV (1:M - record has multiple services)
  │   └── DONTHUOC (1:M - record has multiple prescriptions)
  │
NHANVIEN
  ├── HSBA (doctor treating records)
  ├── HSBA_DV (technician performing services)
```

## 2. Security Architecture

### RBAC (Role-Based Access Control)

Four main roles with hierarchical permissions:

1. **Coordinator** (20 users): Patient and record management
2. **Doctor/Nurse** (100 users): Clinical decision making
3. **Technician** (50 users): Service execution
4. **Patient** (100,000 users): Self-service portal

### VPD (Virtual Private Database)

- Doctor sees only their patients
- Coordinator sees assigned records
- Technician sees assigned services
- Row-level filtering at database

### OLS (Oracle Label Security)

- 3 hierarchy levels: Director > Department Head > Staff
- 3 departments: Cardiology, Gastroenterology, Neurology
- 3 hospital locations: HCM, Hai Phong, Ha Noi
- Multi-component labels on notification data

### Audit & Compliance

- Standard audit: User actions at system level
- Fine-grained audit: Sensitive field changes
- Unified audit: Consolidated compliance view
- Immutable audit trail

## 3. Application Architecture

### Subsystem 1: Oracle DB Admin

- **Framework**: .NET 10.0 WinForm
- **Purpose**: Database administration and security management
- **Users**: DBA and database administrators
- **Key Functions**: User/role management, permission control
- **Package**: Oracle.ManagedDataAccess.Core 23.26.100

### Subsystem 2: Medical System

- **Framework**: .NET 10.0 WinForm
- **Purpose**: Medical record management
- **Users**: Coordinators, doctors, technicians, patients
- **Key Functions**: Patient records, consultations, diagnostics, prescriptions
- **Package**: Oracle.ManagedDataAccess.Core 23.26.100

## 4. Implementation Steps

### Phase 1: Database Setup

1. Create tables (Schema setup)
2. Insert sample data
3. Create indexes for performance

### Phase 2: Security Configuration

1. Create roles and users
2. Implement RBAC policies
3. Setup VPD policies
4. Configure OLS labels

### Phase 3: Audit Setup

1. Enable standard audit
2. Configure fine-grained audit
3. Setup unified audit
4. Test audit logging

### Phase 4: Application Development

1. Build Subsystem 1 (DB Admin)
2. Build Subsystem 2 (Medical System)
3. Integration testing
4. Security testing

### Phase 5: Backup & Recovery

1. Document backup strategy
2. Configure RMAN
3. Test recovery procedures
4. Document recovery steps

## 5. Test Cases

### TC#1: User Setup & Account Creation

- Create users per NHANVIEN records
- Link accounts to staff/patients
- Verify authentication

### TC#2: RBAC Configuration

- Coordinator: Patient management
- Technician: Service execution
- Patient: Self-service access

### TC#3: VPD Implementation

- Doctor data isolation
- Coordinator record assignment
- Technician service filtering

### TC#4: Technician Role Management

- View assigned services only
- Update service results
- Transparent filtering

### TC#5: Patient Self-Service

- View own data only
- Update personal information
- View medical history

## 6. Requirements Mapping

### Requirement 1: Access Control & Interface (5 points)

| Task | Deliverable | Owner | Status |
|------|-------------|-------|--------|
| Task 03 | Database schema with permission tables | Duyên, Triết | In Progress |
| Task 01 | Admin UI forms for user/role management | Duyên, Triết | In Progress |
| Task 02 | Business logic services | Duyên, Triết | In Progress |
| Task 04 | Medical UI forms | Duyên | Pending |
| Task 07 | Medical database schema | Ngọc, Vũ | Blocked (DB not started) |
| Task 08 | RBAC + VPD + OLS policies | Ngọc, Vũ | Blocked (Waiting Task 07) |

### Requirement 2: Notification System (5 points)

| Task | Deliverable | Owner | Status |
|------|-------------|-------|--------|
| Task 08 | OLS label hierarchy (3 levels) | Ngọc, Vũ | Blocked |
| Task 05 | OLS service implementation | Phôn | Pending |
| Task 04 | Notification UI form | Duyên | Pending |

### Requirement 3: Audit & Monitoring (5 points)

| Task | Deliverable | Owner | Status |
|------|-------------|-------|--------|
| Task 09 | Standard, FGA, Unified audit | Ngọc, Vũ | Blocked (Waiting Task 08) |
| Task 06 | Audit service (read logs) | Phôn | Pending |

### Requirement 4: Backup & Recovery (5 points)

| Task | Deliverable | Owner | Status |
|------|-------------|-------|--------|
| Task 10 | RMAN backup + recovery procedures | Ngọc, Vũ | Blocked (Waiting Task 09) |

## 7. Critical Files

### Documentation (Ready)

```text
README.md                               ← Start here
CONTRIBUTING.md                         ← Development standards
docs/README.md                          ← This file
docs/tasks/README.md                    ← Task assignments & progress tracking
docs/tasks/task-01 to task-08           ← Per-deliverable specifications
```

### Database Structure (Two Separate Databases)

```
Database/
├── Subsystem1-AdminDB/
│   ├── README.md
│   ├── schema/                (Create SQL files)
│   │   ├── 01_CreateTables.sql
│   │   ├── 02_CreateViews.sql
│   │   ├── 03_CreateProcedures.sql
│   │   ├── 04_CreateFunctions.sql
│   │   ├── 05_CreateIndexes.sql
│   │   ├── 06_CreateTriggers.sql
│   │   └── 07_InsertSampleData.sql
│   ├── security/
│   │   └── ADMIN_RBAC_Setup.sql
│   └── audit/
│       └── ADMIN_Audit_Setup.sql
│
└── Subsystem2-MedicalDB/
    ├── README.md
    ├── schema/                (Create SQL files)
    │   ├── 01_CreateTables.sql
    │   ├── 02_CreateIndexes.sql
    │   └── 03_InsertSampleData.sql
    ├── security/              (Create SQL files)
    │   ├── 01_RBAC_Setup.sql
    │   ├── 02_VPD_Setup.sql
    │   ├── 03_OLS_Setup.sql
    │   └── 04_Users_Creation.sql
    ├── audit/                 (Create SQL files)
    │   ├── 01_StandardAudit_Setup.sql
    │   ├── 02_FineGrainedAudit_Setup.sql
    │   ├── 03_UnifiedAudit_Setup.sql
    │   └── ReadAuditLogs.sql
    └── backupRestore/         (Create SQL files)
        ├── 01_BackupStrategy.sql
        ├── 02_AutomaticBackup.sql
        └── 03_RecoveryScripts.sql
```

### Application Source Code (Two Separate Subsystems)

```
Subsystem1-OracleDBAdmin/
└── Source/
    ├── OracleDBAdmin/
    │   ├── Forms/                → Admin UI forms
    │   │   ├── MainForm.cs
    │   │   ├── UserManagementForm.cs
    │   │   ├── RoleManagementForm.cs
    │   │   └── PermissionForm.cs
    │   ├── Models/               → Data models
    │   │   ├── User.cs
    │   │   ├── Role.cs
    │   │   ├── Permission.cs
    │   │   └── OracleObject.cs
    │   ├── Services/             → Business logic for permissions
    │   │   ├── UserService.cs
    │   │   ├── PermissionService.cs
    │   │   └── ValidationService.cs
    │   └── Program.cs
    └── OracleDBAdmin.slnx

Subsystem2-MedicalDataManagement/
└── Source/
    ├── MedicalDataSystem/
    │   ├── Forms/                → Medical UI forms
    │   │   ├── LoginForm.cs
    │   │   ├── DoctorForm.cs
    │   │   ├── PatientForm.cs
    │   │   ├── CoordinatorForm.cs
    │   │   ├── TechnicianForm.cs
    │   │   └── NotificationForm.cs
    │   ├── Models/               → Entity models
    │   │   ├── Patient.cs
    │   │   ├── MedicalRecord.cs
    │   │   ├── Prescription.cs
    │   │   └── Notification.cs
    │   ├── Services/             → Business logic
    │   │   ├── AuthenticationService.cs
    │   │   ├── RBACService.cs
    │   │   ├── VPDService.cs
    │   │   ├── AuditService.cs
    │   │   └── PatientService.cs
    │   └── Program.cs
    └── MedicalDataSystem.slnx
```

## 8. Key Concepts

### Data Security Layers

1. **Network**: Encrypted connections (SSL/TLS)
2. **Authentication**: Username/password + role-based
3. **Authorization**: RBAC + VPD policies
4. **Classification**: OLS labels
5. **Auditing**: Complete action tracking
6. **Encryption**: Sensitive data at rest

### Separation of Duties

- Coordinator: Management decisions
- Doctor: Clinical decisions
- Technician: Service execution
- Patient: Own data access
- DBA: Infrastructure management

### Privacy by Design

- Minimal data access
- Role-based filtering
- Transparent row filtering
- Audit on sensitive operations
- Encrypted sensitive data

## 9. Getting Help

For detailed guidance on specific topics:

- Database setup: See [database/README.md](../database/README.md)
- Subsystem 1: See [subsystem1-oracleDBAdmin/README.md](../subsystem1-oracleDBAdmin/README.md)
- Subsystem 2: See [subsystem2-medicalDataManagement/README.md](../subsystem2-medicalDataManagement/README.md)
- Development: See [CONTRIBUTING.md](../CONTRIBUTING.md)

## 10. References

### Oracle Documentation

- [Oracle Database Security Guide](https://docs.oracle.com/en/database/oracle/oracle-database/26/dbseg/index.html)


### Security Best Practices

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)
- [CIS Controls](https://www.cisecurity.org/cis-controls)

### Healthcare Compliance

- [HIPAA Security Rule](https://www.hhs.gov/hipaa/for-professionals/security/)
- [GDPR Data Protection](https://gdpr-info.eu/)

---

**Last Updated**: February 2026  
**Course**: CSC12001 - Data Security in Information Systems  
**Institution**: University of Science - Faculty of Information Technology
