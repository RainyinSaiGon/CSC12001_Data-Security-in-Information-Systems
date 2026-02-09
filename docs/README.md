# Documentation

Complete documentation for CSC12001 - Data Security in Information Systems project.

## Quick Links

| Document | Purpose |
|----------|---------|
| [SETUP_GUIDE.md](SETUP_GUIDE.md) | **Complete setup guide for .NET 10.0 & Oracle 21c XE** |
| [CHANGELOG_2026.md](CHANGELOG_2026.md) | **Technology stack updates and migration guide** |
| [ARCHITECTURE.md](ARCHITECTURE.md) | System design and technical architecture |
| [IMPLEMENTATION.md](IMPLEMENTATION.md) | Requirements, implementation guides, & test results |
| [AUDITLOGS.md](AUDITLOGS.md) | Sample audit log files and analysis |

## Overview

This documentation directory contains the following consolidated files:

### ARCHITECTURE.md
- System design diagrams and structure
- Entity relationship diagrams
- Sequence diagrams for key operations
- Class diagrams
- Database schema design

### IMPLEMENTATION.md
- Complete assignment specification with 4 requirements
- Step-by-step implementation guides for each requirement
- Test case descriptions and acceptance criteria
- Security policy definitions and requirements
- User role definitions and permissions model
- Team member contributions and effort tracking
- Progress tracking for each requirement
- Test execution results and performance analysis
- Security assessment and final summary

### AUDITLOGS.md
- Sample audit trail outputs and analysis
- Audit query examples and templates
- Log analysis samples and interpretations
- Violation detection samples and patterns
- Log analysis samples
- Violation detection samples

## Main Topics

## 1. Database Design

### Oracle Database Express 21c (XE)

**Connection Details:**
- Service: localhost:1521/XE
- SQL*Plus: `sqlplus user/password@localhost:1521/XE`
- ODP.NET: `Data Source=localhost:1521/XE;User Id=user;Password=password;`

### Core Entities
- BỆNHNHÂN (Patient)
- NHÂNVIÊN (Staff)
- HSBA (Medical Record)
- HSBA_DV (Diagnostic Service)
- ĐƠNTHUỐC (Prescription)
- THÔNGBÁO (Notification)

### Key Relationships
```
BỆNHNHÂN
  ├── HSBA (1:M - patient has multiple records)
  │   ├── HSBA_DV (1:M - record has multiple services)
  │   └── ĐƠNTHUỐC (1:M - record has multiple prescriptions)
  │
NHÂNVIÊN
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
- Create users per NHÂNVIÊN records
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

### Requirement 1: Access Control & Interface
- [ ] User account setup (TC#1)
- [ ] RBAC configuration (TC#2, TC#4, TC#5)
- [ ] VPD implementation (TC#3)
- [ ] Application interfaces

### Requirement 2: Notification System
- [ ] OLS label hierarchy (3 levels)
- [ ] Multi-component labels
- [ ] User label assignment
- [ ] Notification interface

### Requirement 3: Audit & Monitoring
- [ ] Standard audit setup
- [ ] Fine-grained audit configuration
- [ ] Unified audit implementation
- [ ] 5+ audit test scenarios

### Requirement 4: Backup & Recovery
- [ ] Backup strategy documentation
- [ ] Backup implementation (RMAN, Export)
- [ ] Recovery procedures
- [ ] Strategy evaluation

## 7. Critical Files

### Documentation (Ready)
```
README.md                               ← Start here
CONTRIBUTING.md                         ← Development standards
docs/README.md                          ← This file
docs/IMPLEMENTATION.md                  ← Requirements, guides, & test results
docs/AUDITLOGS.md                       ← Audit query examples
```

### Database Structure (To Create)
```
Database/
├── Schema/                (Create SQL files)
│   ├── 01_CreateTables.sql
│   ├── 02_CreateIndexes.sql
│   └── 03_InsertSampleData.sql
├── Security/              (Create SQL files)
│   ├── 01_RBAC_Setup.sql
│   ├── 02_VPD_Setup.sql
│   ├── 03_OLS_Setup.sql
│   └── 04_Users_Creation.sql
├── Audit/                 (Create SQL files)
│   ├── 01_StandardAudit_Setup.sql
│   ├── 02_FineGrainedAudit_Setup.sql
│   ├── 03_UnifiedAudit_Setup.sql
│   └── ReadAuditLogs.sql
└── BackupRestore/         (Create SQL files)
    ├── 01_BackupStrategy.sql
    ├── 02_AutomaticBackup.sql
    └── 03_RecoveryScripts.sql
```

### Application Source Code (To Create)
```
Subsystem1-OracleDBAdmin/
└── Source/
    ├── OracleDBAdmin/
    │   ├── Forms/        → Create UI forms
    │   ├── Models/       → Create data models
    │   ├── Services/     → Create business logic
    │   └── Program.cs
    └── OracleDBAdmin.sln

Subsystem2-MedicalDataManagement/
└── Source/
    ├── MedicalDataSystem/
    │   ├── Forms/        → Create UI forms
    │   ├── Models/       → Create entity models
    │   ├── Services/     → Create business logic
    │   └── Program.cs
    └── MedicalDataSystem.sln
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
- Database setup: See [Database/README.md](../Database/README.md)
- Subsystem 1: See [Subsystem1-OracleDBAdmin/README.md](../Subsystem1-OracleDBAdmin/README.md)
- Subsystem 2: See [Subsystem2-MedicalDataManagement/README.md](../Subsystem2-MedicalDataManagement/README.md)
- Development: See [CONTRIBUTING.md](../CONTRIBUTING.md)

## 10. References

### Oracle Documentation
- [Oracle Database Security Guide](https://docs.oracle.com/database/121/DBSEG/)
- [Oracle Virtual Private Database](https://docs.oracle.com/database/121/DBSEG/vpd.htm)
- [Oracle Label Security](https://docs.oracle.com/database/121/DBSEG/label_security.htm)
- [Oracle Auditing](https://docs.oracle.com/database/121/DBSEG/audit.htm)

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
