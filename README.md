# Data Security in Information Systems - Medical & Database Administration Platform

A comprehensive data security project implementing two subsystems: Oracle Database Administration WinForm application and Medical Data Management System with advanced security mechanisms.

## Quick Navigation

| Section | Description |
|---------|-------------|
| [Getting Started](#getting-started) | Complete setup guide |
| [Subsystem1-OracleDBAdmin/README.md](Subsystem1-OracleDBAdmin/README.md) | Database admin application setup |
| [Subsystem2-MedicalDataManagement/README.md](Subsystem2-MedicalDataManagement/README.md) | Medical system setup |
| [Database/README.md](Database/README.md) | Database schema & scripts |
| [docs/README.md](docs/README.md) | Architecture & security documentation |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Development guidelines |

## Overview

This project implements a complete data security solution for a hospital management system, featuring:

- **Subsystem 1**: Oracle Database Administration WinForm application for user/role management and permission control
- **Subsystem 2**: Medical Data Management system with patient records, doctor consultations, and diagnostic services
- **Security Mechanisms**: RBAC (Role-Based Access Control), VPD (Virtual Private Database), OLS (Oracle Label Security)
- **Audit & Monitoring**: Standard audit, Fine-grained audit, and Unified audit trails
- **Backup & Recovery**: Data backup strategies and recovery procedures

## Technology Stack

| Component | Technology |
|-----------|-----------|
| **Subsystem 1** | WinForm, .NET Framework, Oracle ODP.NET |
| **Subsystem 2** | WinForm, .NET Framework, Oracle ODP.NET |
| **Database** | Oracle Database 11g/12c+ |
| **Security** | Oracle RBAC, VPD, OLS, Audit mechanisms |
| **Version Control** | Git/GitHub |
| **Documentation** | Markdown, MS Word |

## Project Structure

```
CSC12001_Data-Security-in-Information-Systems/
│
├── Subsystem1-OracleDBAdmin/              # Database Administration Application
│   ├── Source/
│   │   ├── OracleDBAdmin.sln
│   │   └── OracleDBAdmin/
│   │       ├── Forms/                    # UI Forms
│   │       ├── Models/                   # Data models
│   │       ├── Services/                 # Business logic
│   │       └── Program.cs
│   └── README.md
│
├── Subsystem2-MedicalDataManagement/      # Medical Data Management System
│   ├── Source/
│   │   ├── MedicalDataSystem.sln
│   │   └── MedicalDataSystem/
│   │       ├── Forms/                    # UI Forms (RBAC, VPD, OLS)
│   │       ├── Models/                   # Entity models
│   │       ├── Services/                 # Business logic
│   │       └── Program.cs
│   └── README.md
│
├── Database/                              # Database setup scripts
│   ├── Schema/                            # Data modeling
│   │   ├── 01_CreateTables.sql
│   │   ├── 02_CreateIndexes.sql
│   │   └── 03_InsertSampleData.sql
│   ├── Security/                          # Security configuration
│   │   ├── 01_RBAC_Setup.sql
│   │   ├── 02_VPD_Setup.sql
│   │   ├── 03_OLS_Setup.sql
│   │   └── 04_Users_Creation.sql
│   ├── Audit/                             # Audit configuration
│   │   ├── 01_StandardAudit_Setup.sql
│   │   ├── 02_FineGrainedAudit_Setup.sql
│   │   ├── 03_UnifiedAudit_Setup.sql
│   │   └── ReadAuditLogs.sql
│   ├── BackupRestore/                     # Backup & recovery scripts
│   │   ├── 01_BackupStrategy.sql
│   │   ├── 02_AutomaticBackup.sql
│   │   └── 03_RecoveryScripts.sql
│   └── README.md
│
├── docs/                                  # Documentation
│   ├── README.md                          # Architecture & design docs
│   ├── Requirements/                      # Assignment requirements
│   ├── Architecture/                      # System architecture
│   ├── Implementation/                    # Implementation guides
│   ├── Reports/                           # Test results & reports
│   └── AuditLogs/                         # Audit log samples
│
├── Tests/                                 # Testing
│   ├── TestCases/
│   │   ├── TC1_UserSetup.md
│   │   ├── TC2_RBAC.md
│   │   ├── TC3_VPD.md
│   │   ├── TC4_Technician.md
│   │   └── TC5_PatientAccess.md
│   └── AuditTestScenarios/
│       ├── AuditTest_01.sql
│       ├── AuditTest_02.sql
│       └── ...
│
├── Utils/                                 # Utility scripts
│   ├── Scripts/
│   │   ├── CreateAllUsers.sql
│   │   └── GrantAllPermissions.sql
│   └── ConnectionStrings.config
│
├── README.md                              # This file
├── CONTRIBUTING.md                        # Development guidelines
├── LICENSE
└── .gitignore
```

## Getting Started

### Prerequisites

- Oracle Database 11g or 12c+ installed and running
- Visual Studio 2019 or later
- .NET Framework 4.7.2 or higher
- Oracle Data Provider for .NET (ODP.NET)
- SQL*Plus or Oracle SQL Developer (for database scripts)
- Git for version control

### Step 1: Clone Repository & Install Dependencies

```bash
git clone https://github.com/dinhdaivu/CSC12001_Data-Security-in-Information-Systems.git
cd CSC12001_Data-Security-in-Information-Systems

# Install ODP.NET NuGet packages (in Visual Studio)
# Package Manager: Install-Package Oracle.ManagedDataAccess
```

### Step 2: Setup Oracle Database

**2.1 Create Oracle User for Project**
```sql
-- Connect as SYSTEM or DBA
CREATE USER project_admin IDENTIFIED BY project_admin123;
GRANT CONNECT, RESOURCE, CREATE VIEW, CREATE PROCEDURE TO project_admin;
```

**2.2 Run Database Schema Scripts**
```bash
sqlplus project_admin@orcl

-- Execute in order:
@Database/Schema/01_CreateTables.sql
@Database/Schema/02_CreateIndexes.sql
@Database/Schema/03_InsertSampleData.sql
```

**2.3 Setup Security (RBAC, VPD, OLS)**
```bash
sqlplus project_admin@orcl

@Database/Security/01_RBAC_Setup.sql
@Database/Security/02_VPD_Setup.sql
@Database/Security/03_OLS_Setup.sql
@Database/Security/04_Users_Creation.sql
```

**2.4 Setup Audit**
```bash
sqlplus project_admin@orcl

@Database/Audit/01_StandardAudit_Setup.sql
@Database/Audit/02_FineGrainedAudit_Setup.sql
@Database/Audit/03_UnifiedAudit_Setup.sql
```

### Step 3: Configure Connection Strings

Edit connection strings in both subsystems:

```xml
<!-- Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/app.config -->
<connectionStrings>
    <add name="OracleDbConnection" 
         connectionString="Data Source=orcl;User Id=project_admin;Password=project_admin123;" 
         providerName="Oracle.ManagedDataAccess.Client" />
</connectionStrings>
```

Same for Subsystem2.

### Step 4: Build & Run Applications

**Subsystem 1 - Database Administration**
```bash
cd Subsystem1-OracleDBAdmin/Source
dotnet build OracleDBAdmin.sln
# Or open in Visual Studio and Build > Build Solution
# Run the application
```

**Subsystem 2 - Medical Data Management**
```bash
cd Subsystem2-MedicalDataManagement/Source
dotnet build MedicalDataSystem.sln
# Or open in Visual Studio and Build > Build Solution
# Run the application
```

### Step 5: Verify Setup

- [ ] Both applications start without connection errors
- [ ] Database tables are created successfully
- [ ] Sample data is inserted
- [ ] Users can log in with different roles
- [ ] Security policies are enforced

## Key Features

### Subsystem 1: Oracle Database Administration
- Create, modify, delete users and roles
- Grant/revoke permissions on database objects
- View user privileges and role assignments
- WITH GRANT OPTION support for permission delegation
- Column-level security on SELECT/UPDATE operations

### Subsystem 2: Medical Data Management
- **RBAC**: Role-based access control (Coordinator, Doctor, Technician, Patient)
- **VPD**: Virtual Private Database for data filtering
- **OLS**: Oracle Label Security for multi-level notifications
- **Audit**: Track all sensitive operations (diagnoses, treatments, prescriptions)
- **Patient Privacy**: Each user sees only their own information

### Security Mechanisms
- **RBAC**: Different roles with different permissions
- **VPD**: Transparent row-level security
- **OLS**: Multi-level classification and segregation
- **Standard Audit**: Track user actions
- **Fine-grained Audit**: Detailed operation tracking
- **Backup & Recovery**: Data protection mechanisms

## Available Commands

### Database Scripts

```bash
# Schema setup
sqlplus project_admin@orcl @Database/Schema/01_CreateTables.sql

# Security setup
sqlplus project_admin@orcl @Database/Security/01_RBAC_Setup.sql

# Audit setup
sqlplus project_admin@orcl @Database/Audit/01_StandardAudit_Setup.sql

# Read audit logs
sqlplus project_admin@orcl @Database/Audit/ReadAuditLogs.sql
```

### Visual Studio
```
Build > Build Solution           # Compile project
Debug > Start Debugging          # Run with debugger
Build > Clean Solution           # Remove build artifacts
```

## Development Workflow

1. **Create Branch**
   ```bash
   git checkout -b feature/your-feature
   ```

2. **Make Changes**
   - Follow [CONTRIBUTING.md](CONTRIBUTING.md) standards
   - Test your changes thoroughly
   - Write/update documentation

3. **Commit Changes**
   ```bash
   git commit -m "type: description"
   # Examples: feat: add RBAC support, fix: connection string issue
   ```

4. **Push & Create Pull Request**
   ```bash
   git push origin feature/your-feature
   ```

## Documentation

- [docs/README.md](docs/README.md) - System architecture and technical design
- [Subsystem1-OracleDBAdmin/README.md](Subsystem1-OracleDBAdmin/README.md) - DB admin app documentation
- [Subsystem2-MedicalDataManagement/README.md](Subsystem2-MedicalDataManagement/README.md) - Medical system documentation
- [Database/README.md](Database/README.md) - Database schema and security setup
- [CONTRIBUTING.md](CONTRIBUTING.md) - Development guidelines and standards
- [Tests/](Tests/) - Test cases and scenarios

## Requirements Implementation

### Requirement 1: Access Control & Interface
- [ ] Database setup & user account creation (TC#1)
- [ ] RBAC implementation (TC#2, TC#4, TC#5)
- [ ] VPD implementation (TC#3)
- [ ] User interface for all roles

### Requirement 2: Notification System with OLS
- [ ] OLS label hierarchy setup
- [ ] Multi-level notification classification
- [ ] User label assignment by organization
- [ ] Notification interface

### Requirement 3: Audit & Monitoring
- [ ] Standard audit setup
- [ ] Fine-grained audit configuration
- [ ] Unified audit setup
- [ ] Test scenarios and verification

### Requirement 4: Backup & Recovery
- [ ] Backup strategy documentation
- [ ] Automatic backup implementation
- [ ] Recovery procedures
- [ ] Testing and evaluation

## Troubleshooting

### Oracle Connection Issues
```bash
# Verify Oracle is running
sqlplus /nolog
SQL> CONNECT system/password@orcl

# Check listener status
lsnrctl status

# Start listener if needed
lsnrctl start
```

### Database Objects Not Found
- Verify scripts executed without errors
- Check user has proper privileges
- Run `SELECT * FROM user_tables;` to verify tables exist

### ODP.NET Installation Issues
```
Package Manager Console: Install-Package Oracle.ManagedDataAccess
```

### Application Won't Connect
- Check connection string in app.config
- Verify TNS_ADMIN environment variable is set
- Ensure firewall allows Oracle port (1521)
- Test connection with SQL*Plus first

## Course Information

- **Course**: CSC12001 - Data Security in Information Systems
- **Institution**: University of Science - Faculty of Information Technology
- **Academic Year**: 2025-2026
- **Assignment Type**: Team project (2 subsystems)

## Submission Requirements

1. **Project Structure**: Proper folder organization as specified
2. **Source Code**: Well-commented, follows C# conventions
3. **Database Scripts**: Fully functional SQL scripts
4. **Documentation**: Complete theoretical explanation and implementation guide
5. **Team Contribution**: Clear assignment of work to team members
6. **Test Results**: Evidence of all test cases passing

**Submission Format**: 
- Folder name: `ATBM-2026-[GroupCode]` (when ready to submit)
- Include: Source code, SQL scripts, documentation, reports

## License

MIT License - See [LICENSE](LICENSE) file

---

**Ready to start?** Follow the Getting Started section above!

For questions or contributions, refer to [CONTRIBUTING.md](CONTRIBUTING.md).
