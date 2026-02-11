# Data Security in Information Systems - Medical & Database Administration Platform

A comprehensive data security project implementing two subsystems: Oracle Database Administration WinForm application and Medical Data Management System with advanced security mechanisms.

## Quick Navigation

| Section | Description |
|---------|-------------|
| [Getting Started](#getting-started) | Complete setup guide |
| [docs/SETUP_GUIDE.md](docs/SETUP_GUIDE.md) | Detailed setup guide for .NET 10.0 & Oracle 21c XE |
| [subsystem1-oracleDBAdmin/README.md](subsystem1-oracleDBAdmin/README.md) | Database admin application setup |
| [subsystem2-medicalDataManagement/README.md](subsystem2-medicalDataManagement/README.md) | Medical system setup |
| [database/README.md](database/README.md) | Database schema & scripts |
| [docs/README.md](docs/README.md) | Documentation index |
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
| **Subsystem 1** | WinForm, .NET 10.0, Oracle ODP.NET |
| **Subsystem 2** | WinForm, .NET 10.0, Oracle ODP.NET |
| **Database** | Oracle Database Express 21c (XE) |
| **Security** | Oracle RBAC, VPD, OLS, Audit mechanisms |
| **Version Control** | Git/GitHub |
| **Documentation** | Markdown, MS Word |

## Project Structure

Currently existing: Documentation files (README.md, CONTRIBUTING.md, docs/), GitHub workflows, issue templates.

To be created: Database SQL scripts, application source code, test files, utility scripts.

```
CSC12001_Data-Security-in-Information-Systems/
│
├── subsystem1-oracleDBAdmin/              # Database Administration Application
│   ├── source/                            # (To be created)
│   │   ├── oracleDBAdmin.slnx
│   │   └── oracleDBAdmin/
│   │       ├── forms/                    # UI Forms
│   │       ├── models/                   # Data models
│   │       ├── services/                 # Business logic
│   │       └── Program.cs
│   └── README.md
│
├── subsystem2-medicalDataManagement/      # Medical Data Management System
│   ├── source/                            # (To be created)
│   │   ├── medicalDataSystem.slnx
│   │   └── medicalDataSystem/
│   │       ├── forms/                    # UI Forms (RBAC, VPD, OLS)
│   │       ├── models/                   # Entity models
│   │       ├── services/                 # Business logic
│   │       └── Program.cs
│   └── README.md
│
├── database/                              # Database setup scripts (separated by subsystem)
│   │
│   ├── Subsystem1-AdminDB/                # Admin Database for OracleDBAdmin tool
│   │   ├── schema/                        # Admin tables, views, procedures
│   │   │   ├── 01_CreateAdminTables.sql
│   │   │   ├── 02_CreateDatabaseObjectTables.sql
│   │   │   ├── 03_CreateAdminViews.sql
│   │   │   ├── 04_CreateAdminStoredProcedures.sql
│   │   │   ├── 05_CreateAdminFunctions.sql
│   │   │   ├── 06_CreateAdminIndexes.sql
│   │   │   └── 07_InsertSampleData.sql
│   │   ├── security/                      # Admin database security
│   │   │   ├── 01_AdminUsers_Creation.sql
│   │   │   ├── 02_AdminRBAC_Setup.sql
│   │   │   └── 03_AdminAudit_Setup.sql
│   │   ├── audit/                         # Admin audit setup
│   │   │   └── 01_AdminOperationAudit.sql
│   │   └── README.md
│   │
│   ├── Subsystem2-MedicalDB/              # Medical Database for Medical System
│   │   ├── schema/                        # Medical data tables
│   │   │   ├── 01_CreateTables.sql
│   │   │   ├── 02_CreateIndexes.sql
│   │   │   └── 03_InsertSampleData.sql
│   │   ├── security/                      # Medical database security
│   │   │   ├── 01_Users_Creation.sql
│   │   │   ├── 02_RBAC_Setup.sql
│   │   │   ├── 03_VPD_Setup.sql
│   │   │   └── 04_OLS_Setup.sql
│   │   ├── audit/                         # Medical audit configuration
│   │   │   ├── 01_StandardAudit_Setup.sql
│   │   │   ├── 02_FineGrainedAudit_Setup.sql
│   │   │   ├── 03_UnifiedAudit_Setup.sql
│   │   │   └── ReadAuditLogs.sql
│   │   └── README.md
│   │
│   ├── Audit/                             # Legacy audit scripts
│   │   └── ReadAuditLogs.sql
│   │
│   └── README.md
│
├── docs/                                  # Documentation (consolidated)
│   ├── README.md                          # Documentation index
│   ├── ARCHITECTURE.md                    # System design & architecture
│   ├── SETUP_GUIDE.md                     # .NET & Oracle setup guide
│   ├── CHANGELOG_2026.md                  # Technology stack updates
│   └── tasks/                             # Organized task files with requirements
│       ├── README.md                      # Task assignment summary
│       └── task-01 to task-08             # Per-deliverable task specs
│
├── tests/                                 # Testing (To be created)
│   ├── testCases/                         # Test case documentation
│   │   ├── TC1_UserSetup.md              # Test Case #1
│   │   ├── TC2_RBAC.md                   # Test Case #2
│   │   ├── TC3_VPD.md                    # Test Case #3
│   │   ├── TC4_Technician.md             # Test Case #4
│   │   └── TC5_PatientAccess.md          # Test Case #5
│   └── auditTestScenarios/               # Audit test scripts
│       ├── AuditTest_01_UnauthorizedAccess.sql
│       ├── AuditTest_02_PrivilegeEscalation.sql
│       └── ...
│
├── utils/                                 # Utility scripts (To be created)
│   ├── scripts/                           # SQL utility scripts
│   │   ├── CreateAllUsers.sql
│   │   └── GrantAllPermissions.sql
│   └── Config/
│       └── appsettings.template.json     # Connection string template
│
├── README.md                              # This file
├── CONTRIBUTING.md                        # Development guidelines
├── LICENSE
└── .gitignore
```

## Getting Started

### Prerequisites

- Oracle Database Express 21c (XE) installed and running
- Visual Studio 2022 or later
- .NET 10.0 SDK or higher
- Oracle Data Provider for .NET (ODP.NET) for .NET Core/10+
- SQL*Plus or Oracle SQL Developer (for database scripts)
- Git for version control

### Step 1: Clone Repository & Install Dependencies

```bash
git clone https://github.com/dinhdaivu/CSC12001_Data-Security-in-Information-Systems.git
cd CSC12001_Data-Security-in-Information-Systems

# Install ODP.NET NuGet packages for .NET 10.0
# Package Manager: Install-Package Oracle.ManagedDataAccess.Core
# Or use dotnet CLI:
dotnet add package Oracle.ManagedDataAccess.Core
```

### Step 2: Setup Oracle Database

**2.1 Create Oracle User for Project**

**IMPORTANT: Use a strong, randomly-generated password (minimum 12 characters, mixed case, numbers, symbols). Set it via secrets manager or local config, never commit to repository.**

```sql
-- Connect to Oracle 21c XE as SYSTEM
-- For XE: sqlplus system/your_password@localhost:1521/XE
-- Or: sqlplus / as sysdba

-- Replace <STRONG_PASSWORD> with a secure password (e.g., generate with: openssl rand -base64 12)
CREATE USER project_admin IDENTIFIED BY <STRONG_PASSWORD>;

-- Grant basic privileges
GRANT CONNECT, RESOURCE TO project_admin;
GRANT CREATE VIEW, CREATE PROCEDURE, CREATE SEQUENCE TO project_admin;
GRANT CREATE TRIGGER, CREATE TYPE, CREATE SYNONYM TO project_admin;
GRANT UNLIMITED TABLESPACE TO project_admin;

-- For advanced security features (RBAC, VPD, OLS)
GRANT CREATE USER, ALTER USER, DROP USER TO project_admin;
GRANT CREATE ROLE, DROP ANY ROLE, GRANT ANY ROLE TO project_admin;
GRANT GRANT ANY PRIVILEGE TO project_admin;
GRANT EXECUTE ON DBMS_RLS TO project_admin;
GRANT AUDIT SYSTEM TO project_admin;
GRANT SELECT ON SYS.DBA_AUDIT_TRAIL TO project_admin;
GRANT SELECT ON SYS.DBA_FGA_AUDIT_TRAIL TO project_admin;

-- Verify user creation
SELECT username, account_status FROM dba_users WHERE username = 'PROJECT_ADMIN';
```

**After user creation:**

1. Store the password in your secure credential manager:
   - **Development**: `dotnet user-secrets set "OracleDbConnection:Password" "<STRONG_PASSWORD>"`
   - **Production**: Set environment variable or use secrets manager (AWS Secrets Manager, Azure Key Vault, etc.)
2. **Rotate the password** immediately after initial setup and regularly (at least quarterly)
3. Never commit the plaintext password to the repository
4. Use different passwords for different environments (dev, staging, production)

**2.2 Setup Subsystem 1 Admin Database** (Task 3 - Schema Design by Duyên, Triết; SQL Implementation by Ngọc, Vũ)

This database manages user/role administration and permissions:

```bash
# For Oracle 21c XE, connect using:
sqlplus project_admin/your_password@localhost:1521/XE

-- Schema creation (Subsystem 1 Admin DB)
@database/Subsystem1-AdminDB/schema/01_CreateAdminTables.sql
@database/Subsystem1-AdminDB/schema/02_CreateDatabaseObjectTables.sql
@database/Subsystem1-AdminDB/schema/03_CreateAdminViews.sql
@database/Subsystem1-AdminDB/schema/04_CreateAdminStoredProcedures.sql
@database/Subsystem1-AdminDB/schema/05_CreateAdminFunctions.sql
@database/Subsystem1-AdminDB/schema/06_CreateAdminIndexes.sql
@database/Subsystem1-AdminDB/schema/07_InsertSampleData.sql

-- Security setup
@database/Subsystem1-AdminDB/security/01_AdminUsers_Creation.sql
@database/Subsystem1-AdminDB/security/02_AdminRBAC_Setup.sql
@database/Subsystem1-AdminDB/security/03_AdminAudit_Setup.sql
```

**2.3 Setup Subsystem 2 Medical Database** (Tasks 7-10 - Ngọc, Vũ)

This database contains patient records with RBAC, VPD, and OLS security:

```bash
# Schema creation (Subsystem 2 Medical DB)
sqlplus project_admin/your_password@localhost:1521/XE

-- Execute in order:
@database/Subsystem2-MedicalDB/schema/01_CreateTables.sql
@database/Subsystem2-MedicalDB/schema/02_CreateIndexes.sql
@database/Subsystem2-MedicalDB/schema/03_InsertSampleData.sql

-- Security setup (RBAC, VPD, OLS)
@database/Subsystem2-MedicalDB/security/01_Users_Creation.sql
@database/Subsystem2-MedicalDB/security/02_RBAC_Setup.sql
@database/Subsystem2-MedicalDB/security/03_VPD_Setup.sql
@database/Subsystem2-MedicalDB/security/04_OLS_Setup.sql

-- Audit configuration
@database/Subsystem2-MedicalDB/audit/01_StandardAudit_Setup.sql
@database/Subsystem2-MedicalDB/audit/02_FineGrainedAudit_Setup.sql
@database/Subsystem2-MedicalDB/audit/03_UnifiedAudit_Setup.sql
```

See [database/README.md](database/README.md) for complete database documentation and detailed task specifications in [docs/tasks/](docs/tasks/).

See [database/README.md](database/README.md) for security mechanism details.

**2.4 Setup Audit** (To be created)

Create and execute audit configuration scripts:

```bash
sqlplus project_admin/your_password@localhost:1521/XE

@database/audit/01_StandardAudit_Setup.sql
@database/audit/02_FineGrainedAudit_Setup.sql
@database/audit/03_UnifiedAudit_Setup.sql
```

See [database/README.md](database/README.md) and [docs/tasks/task-08-database-audit-setup.md](docs/tasks/task-08-database-audit-setup.md) for audit setup guidance.

### Step 3: Configure Connection Strings

**IMPORTANT: Never commit plaintext passwords to the repository!**

Use one of these secure credential management approaches:

#### Option A: User Secrets (Development - Recommended)

```bash
# Initialize user secrets (run in your project directory)
dotnet user-secrets init

# Set connection credentials for Oracle 21c XE
dotnet user-secrets set "OracleDbConnection:UserId" "project_admin"
dotnet user-secrets set "OracleDbConnection:Password" "your_secure_password_here"
dotnet user-secrets set "OracleDbConnection:DataSource" "localhost:1521/XE"
```

In your C# code:

```csharp
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

// Load configuration from user secrets
var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string userId = config["OracleDbConnection:UserId"];
string password = config["OracleDbConnection:Password"];
string dataSource = config["OracleDbConnection:DataSource"];

string connectionString = $"Data Source={dataSource};User Id={userId};Password={password};";
```

#### Option B: Environment Variables (Production)

Set environment variables before running the application:

```powershell
# PowerShell
$env:ORACLE_USERID = "project_admin"
$env:ORACLE_PASSWORD = "your_secure_password_here"
$env:ORACLE_DATASOURCE = "localhost:1521/XE"
```

```bash
# Bash/Linux
export ORACLE_USERID="project_admin"
export ORACLE_PASSWORD="your_secure_password_here"
export ORACLE_DATASOURCE="localhost:1521/XE"
```

Read from environment in code:

```csharp
using Oracle.ManagedDataAccess.Client;

string userId = Environment.GetEnvironmentVariable("ORACLE_USERID");
string password = Environment.GetEnvironmentVariable("ORACLE_PASSWORD");
string dataSource = Environment.GetEnvironmentVariable("ORACLE_DATASOURCE");

string connectionString = $"Data Source={dataSource};User Id={userId};Password={password};";

using (var connection = new OracleConnection(connectionString))
{
    connection.Open();
    // Your code here
}
```

#### Option C: Local Configuration File (Development - Not Committed)

Create `appsettings.local.json` (add to `.gitignore`):

```json
{
  "OracleDbConnection": {
    "UserId": "project_admin",
    "Password": "your_secure_password_here",
    "DataSource": "localhost:1521/XE"
  }
}
```

Add to `.gitignore`:

``` text
appsettings.local.json
appsettings.*.local.json
```

Load in C# code:

```csharp
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

string userId = config["OracleDbConnection:UserId"];
string password = config["OracleDbConnection:Password"];
string dataSource = config["OracleDbConnection:DataSource"];

string connectionString = $"Data Source={dataSource};User Id={userId};Password={password};";
```

**Select the approach that matches your deployment environment and update both Subsystem1 and Subsystem2 configuration files accordingly.**

### Step 4: Build & Run Applications

**Subsystem 1 - Database Administration**

```bash
cd subsystem1-oracleDBAdmin/source
dotnet build oracleDBAdmin.slnx
# Or open in Visual Studio and Build > Build Solution
# Run the application
```

**Subsystem 2 - Medical Data Management**

```bash
cd subsystem2-medicalDataManagement/source
dotnet build medicalDataSystem.slnx
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
# For Oracle 21c XE:
# Schema setup
sqlplus project_admin/password@localhost:1521/XE @database/schema/01_CreateTables.sql

# Security setup
sqlplus project_admin/password@localhost:1521/XE @database/security/01_RBAC_Setup.sql

# Audit setup
sqlplus project_admin/password@localhost:1521/XE @database/audit/01_StandardAudit_Setup.sql

# Read audit logs
sqlplus project_admin/password@localhost:1521/XE @database/audit/ReadAuditLogs.sql
```

### Visual Studio

```text
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
- [subsystem1-oracleDBAdmin/README.md](subsystem1-oracleDBAdmin/README.md) - DB admin app documentation
- [subsystem2-medicalDataManagement/README.md](subsystem2-medicalDataManagement/README.md) - Medical system documentation
- [database/README.md](database/README.md) - Database schema and security setup
- [CONTRIBUTING.md](CONTRIBUTING.md) - Development guidelines and standards
- [tests/](tests/) - Test cases and scenarios

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
# Verify Oracle 21c XE is running
sqlplus /nolog
SQL> CONNECT system/password@localhost:1521/XE

# Or connect as SYSDBA
SQL> CONNECT / AS SYSDBA

# Check Oracle service status
sc query OracleServiceXE

# Check listener status
lsnrctl status

# Start listener if needed
lsnrctl start

# Start Oracle service if stopped
net start OracleServiceXE
```

### Database Objects Not Found

- Verify scripts executed without errors
- Check user has proper privileges
- Run `SELECT * FROM user_tables;` to verify tables exist

### ODP.NET Installation Issues

```bash
# For .NET 10.0 projects, use:
dotnet add package Oracle.ManagedDataAccess.Core
# Or in Package Manager Console:
Install-Package Oracle.ManagedDataAccess.Core
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

## GitHub Configuration

### CI/CD Workflows

Three automated pipelines validate code and documentation:

**Subsystem 1 CI/CD** (`subsystem1-ci.yml`)

- Triggers on PR to main/develop with changes to `subsystem1-oracleDBAdmin/`
- Builds .NET project and runs tests (Windows, .NET 8.0.x & 10.0.x)
- Code quality analysis using CodeQL
- **Status**: Currently disabled (`if: false`) — activate when source code ready

**Subsystem 2 CI/CD** (`subsystem2-ci.yml`)

- Triggers on PR to main/develop with changes to `subsystem2-medicalDataManagement/`
- Builds .NET project and runs tests (Windows, .NET 8.0.x & 10.0.x)
- Code quality analysis using CodeQL
- **Status**: Currently disabled (`if: false`) — activate when source code ready

**Database CI/CD** (`database-ci.yml`)

- Triggers on PR to main/develop with changes to `Database/`
- Validates SQL syntax and script structure
- Verifies execution order of database scripts
- **Status**: Currently disabled (`if: false`) — activate when SQL scripts ready

### Issue Templates

**Bug Report** (`bug_report.md`)

- Use for reporting bugs or issues
- Includes system info, reproduction steps, severity levels
- Auto-labeled as `bug`

**Feature Request** (`feature_request.md`)

- Use for suggesting new features or improvements
- Includes problem statement, proposed solution, acceptance criteria
- Auto-labeled as `enhancement`

### Pull Request Template

**Template** (`pull_request_template.md`)

- Standard format for all PRs
- Includes description, type, related issues, testing info, checklist
- Ensures consistent PR quality and information

### Using Workflows

When you're ready to activate CI/CD checks:

1. Remove `if: false` from job definitions in workflow files
2. Push changes to a feature branch
3. Create a PR to main/develop
4. Workflows automatically trigger based on changed files
5. Review status checks on PR before merging

### Local Development

Before pushing:

```bash
# Run tests locally
dotnet test

# Build the project
dotnet build --configuration Release

# Create descriptive commits
git commit -m "type(scope): description"
```

## License

MIT License - See [LICENSE](LICENSE) file

---

**Ready to start?** Follow the Getting Started section above!

For questions or contributions, refer to [CONTRIBUTING.md](CONTRIBUTING.md).
