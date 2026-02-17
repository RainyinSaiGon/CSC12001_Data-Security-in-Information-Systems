# Data Security in Information Systems

Medical and database administration platform implementing comprehensive security mechanisms including Role-Based Access Control (RBAC), Virtual Private Database (VPD), and Oracle Label Security (OLS).

## Overview

A complete data security solution for hospital management consisting of:

* Subsystem 1: Oracle Database Administration WinForm application for user/role/permission management
* Subsystem 2: Medical Data Management system with patient records, consultations, and diagnostics
* Security Framework: RBAC, VPD, OLS, and comprehensive audit trails
* Database: Oracle Database Express 21c (XE)
* Technology Stack: .NET 10.0, WinForms, Oracle ODP.NET

## Project Status

Current Release: February 18, 2026

## Quick Start

### Prerequisites

* Oracle Database Express 21c (XE)
* Visual Studio 2022 or later
* .NET 10.0 SDK or higher
* Oracle Data Provider for .NET (ODP.NET)
* Git for version control

### Installation

1. Clone the repository

```bash
git clone https://github.com/dinhdaivu/CSC12001_Data-Security-in-Information-Systems.git
cd CSC12001_Data-Security-in-Information-Systems
```

2. Install dependencies

```bash
dotnet add package Oracle.ManagedDataAccess.Core
```

3. Setup Oracle Database (see database/README.md for detailed instructions)

4. Configure connection strings securely

```bash
dotnet user-secrets init
dotnet user-secrets set "OracleDbConnection:Password" "<STRONG_PASSWORD>"
```

5. Build and run applications

```bash
dotnet build
dotnet run
```

## Project Structure

```
CSC12001_Data-Security-in-Information-Systems/
├── subsystem1-oracleDBAdmin/           # Database Admin Application
│   ├── source/
│   │   └── oracleDBAdmin/
│   │       ├── forms/                  # UI Forms (MainForm, UserManagementForm, etc.)
│   │       ├── models/                 # Data models (User, Role, Permission, etc.)
│   │       ├── services/               # Business logic services
│   │       └── Program.cs
│   └── README.md
│
├── subsystem2-medicalDataManagement/   # Medical Data Management System
│   ├── source/
│   │   └── medicalDataSystem/
│   │       ├── forms/                  # UI Forms (LoginForm, CoordinatorForm, DoctorForm, etc.)
│   │       ├── models/                 # Entity models (Patient, Staff, MedicalRecord, etc.)
│   │       ├── services/               # Business logic services
│   │       └── Program.cs
│   └── README.md
│
├── database/                           # Database setup scripts
│   ├── Subsystem1-AdminDB/             # Admin Database
│   │   ├── schema/                     # Tables, views, procedures (01-07_**.sql)
│   │   ├── security/                   # RBAC, user creation (01-03_**.sql)
│   │   ├── audit/                      # Audit setup
│   │   └── README.md
│   │
│   ├── Subsystem2-MedicalDB/           # Medical Database
│   │   ├── schema/                     # Tables, indexes, sample data (01-03_**.sql)
│   │   ├── security/                   # Users, RBAC, VPD, OLS (01-04_**.sql)
│   │   ├── audit/                      # Standard, FGA, Unified audit setup
│   │   └── README.md
│   │
│   └── README.md
│
├── docs/                               # Documentation
│   ├── ARCHITECTURE.md                 # System design and architecture
│   ├── SETUP_GUIDE.md                  # Detailed setup instructions
│   ├── CHANGELOG_2026.md               # Technology stack updates
│   └── tasks/                          # Task specifications (10 files)
│       ├── README.md                   # Task assignment summary
│       ├── task-01 to task-10/         # Individual task specifications
│
├── tests/                              # Test cases and scenarios
│   ├── testCases/                      # Test case documentation (TC1-TC5)
│   └── auditTestScenarios/             # SQL audit test scenarios
│
├── README.md                           # This file
├── CONTRIBUTING.md                     # Development guidelines
├── LICENSE                             # License information
└── .gitignore                          # Git ignore rules
```

## Getting Started

**Prerequisites:**

* Oracle Database Express 21c (XE)
* Visual Studio 2022 or later
* .NET 10.0 SDK or higher
* Oracle Data Provider for .NET (ODP.NET)
* Git for version control

**Quick Setup:**

1. Clone and install dependencies:

   ```bash
   git clone https://github.com/dinhdaivu/CSC12001_Data-Security-in-Information-Systems.git
   cd CSC12001_Data-Security-in-Information-Systems
   dotnet add package Oracle.ManagedDataAccess.Core
   ```

2. Configure database connection:

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "OracleDbConnection:Password" "<PASSWORD>"
   ```

3. Build and run:

   ```bash
   dotnet build
   dotnet run
   ```

See [SETUP_GUIDE.md](docs/SETUP_GUIDE.md) for complete configuration and [database/README.md](database/README.md) for database setup.

## Documentation

* [SETUP_GUIDE.md](docs/SETUP_GUIDE.md) - Complete setup and configuration guide
* [ARCHITECTURE.md](docs/ARCHITECTURE.md) - System design and architecture
* [database/README.md](database/README.md) - Database schema and security setup
* [subsystem1-oracleDBAdmin/README.md](subsystem1-oracleDBAdmin/README.md) - Database admin application
* [subsystem2-medicalDataManagement/README.md](subsystem2-medicalDataManagement/README.md) - Medical data system
* [docs/tasks/README.md](docs/tasks/README.md) - Task assignments and specifications
* [CONTRIBUTING.md](CONTRIBUTING.md) - Development guidelines

## Security Features

* Role-Based Access Control (RBAC) - User roles with specific permissions
* Virtual Private Database (VPD) - Transparent row-level security filtering
* Oracle Label Security (OLS) - Multi-level classification and access control
* Comprehensive Audit Trails - Standard, Fine-Grained, and Unified Audit logging
* Secure Credential Management - User secrets and environment variables
* Input Validation and Sanitization - Application-level security controls
* Parameterized Queries - Protection against SQL injection
## Team & Contributors

* Duyên, Triết - Database Administration UI and Services (Subsystem 1)
* Phôn - Medical System Security and Business Services (Subsystem 2)
* Ngọc, Vũ - Medical Database Setup and Integration

## Security Features

* Role-Based Access Control (RBAC) - User and permission management
* Virtual Private Database (VPD) - Row-level security filtering
* Oracle Label Security (OLS) - Multi-level access control
* Comprehensive Audit Trails - Standard, fine-grained, and unified auditing
* Secure Credential Management - User secrets and environment variables
* Input Validation and Sanitization - Application-level security controls
* Parameterized Queries - SQL injection prevention

## Key Features

**Subsystem 1 - Oracle Database Administration:**

* User and role management with create/modify/delete operations
* Permission granting/revoking with WITH GRANT OPTION delegation
* Column-level security on SELECT/UPDATE operations
* Privilege viewer and administration interface

**Subsystem 2 - Medical Data Management:**

* Patient record management with medical history tracking
* Doctor consultation and prescription management
* Diagnostic service ordering and result tracking
* Role-based access control (Coordinator, Doctor, Technician, Patient)
* Virtual Private Database for row-level filtering
* Oracle Label Security for location and department restrictions
* Complete audit trail of all sensitive operations

## Development

**Build Applications:**

```bash
# Subsystem 1
cd subsystem1-oracleDBAdmin/source
dotnet build oracleDBAdmin.slnx

# Subsystem 2
cd subsystem2-medicalDataManagement/source
dotnet build medicalDataSystem.slnx
```

**Development Workflow:**
1. Create feature branch: `git checkout -b feature/name`
2. Make changes and test thoroughly
3. Commit with descriptive message: `git commit -m "type: description"`
4. Push and create pull request

See [CONTRIBUTING.md](CONTRIBUTING.md) for development standards and guidelines.

## License

MIT License - See [LICENSE](LICENSE) file for details

## Troubleshooting

**Oracle Connection Issues:**

* Verify Oracle 21c XE service is running: `net start OracleServiceXE`
* Check listener status: `lsnrctl status`
* Verify TNS_ADMIN environment variable is set
* Test with SQL*Plus before debugging application connection

**Database Objects Not Found:**

* Verify scripts executed without errors
* Check user has proper privileges
* Run `SELECT * FROM user_tables;` to verify tables exist

**Application Won't Connect:**

* Check connection string in configuration files
* Verify TNS alias exists in tnsnames.ora
* Ensure firewall allows Oracle port (1521)
* Test with SQL*Plus first

**ODP.NET Issues:**

* Use `dotnet add package Oracle.ManagedDataAccess.Core` for .NET 10.0
* Verify NuGet package version matches .NET target framework

## Course Information

* **Course**: CSC12001 - Data Security in Information Systems
* **Institution**: University of Science - Faculty of Information Technology
* **Academic Year**: 2025-2026
* **Assignment Type**: Team project (2 subsystems)

## Testing Requirements

* All security mechanisms (RBAC, VPD, OLS) properly enforced
* Audit trails record all sensitive operations
* Role-based access control prevents unauthorized actions
* Virtual private database filters data correctly
* Label security restricts notifications by user level
* Backup and recovery procedures tested and validated

## GitHub Workflows

Three CI/CD pipelines validate code automatically (currently disabled):

* **Subsystem 1 Build** - Validates .NET project for subsystem1-oracleDBAdmin
* **Subsystem 2 Build** - Validates .NET project for subsystem2-medicalDataManagement
* **Database Validation** - Validates SQL scripts and execution order

Workflows activate automatically when triggered on pull requests to main/develop branches.
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

# Build and commit
dotnet build --configuration Release
git commit -m "type: description"
```

---

**Ready to start?** Follow the Getting Started section above!

For questions or contributions, refer to [CONTRIBUTING.md](CONTRIBUTING.md).
