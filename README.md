# Data Security in Information Systems

Oracle security course project for CSC12001. The authoritative assignment text is [docs/designs/Requirements.md](docs/designs/Requirements.md).

## Important Rule

If this README conflicts with the assignment, use [docs/designs/Requirements.md](docs/designs/Requirements.md).

## Project Scope

The project must deliver one application with two modules:

- Subsystem 1: Oracle database administration
- Subsystem 2: medical data management

Security mechanisms required by the assignment:

- RBAC
- VPD
- OLS
- Standard Audit plus FGA or Unified Audit
- Backup and recovery

## Current Repository State

What is currently checked in:

- design documents in `docs/designs/`
- task briefs in `docs/tasks/`
- database folders in `database/`
- application source for `subsystem2-medicalDataManagement`
- application source for `Subsystem1-OracleDBAdmin`
- database scripts for schema, security, audit, reset, and reporting under `database/Subsystem2-MedicalDB`

## Repository Structure

```text
CSC12001_Data-Security-in-Information-Systems/
|-- README.md
|-- CONTRIBUTING.md
|-- docs/
|   |-- designs/
|   |   |-- Requirements.md
|   |   |-- ARCHITECTURE.md
|   |   `-- SETUP_GUIDE.md
|   `-- tasks/
|-- database/
|   |-- README.md
|   |-- Subsystem1-AdminDB/
|   |-- Subsystem2-MedicalDB/
|   `-- audit/
|-- Subsystem1-OracleDBAdmin/
`-- subsystem2-medicalDataManagement/
    |-- README.md
    `-- source/
```

## Architecture Summary

- Oracle remains the source of truth for users, roles, and privileges.
- Subsystem 1 should operate on Oracle users, roles, privileges, and data dictionary views.
- Subsystem 2 should preserve the required relations: `BENHNHAN`, `NHANVIEN`, `HSBA`, `HSBA_DV`, `DONTHUOC`, and `THONGBAO`.
- The project should not replace Oracle account management with custom admin tables.

## Recommended Reading Order

1. [docs/designs/Requirements.md](docs/designs/Requirements.md)
2. [docs/designs/ARCHITECTURE.md](docs/designs/ARCHITECTURE.md)
3. [docs/designs/SETUP_GUIDE.md](docs/designs/SETUP_GUIDE.md)
4. [database/README.md](database/README.md)

## Build Status

Subsystem 2:

```powershell
cd subsystem2-medicalDataManagement\source
& "C:\Program Files\dotnet\dotnet.exe" restore
& "C:\Program Files\dotnet\dotnet.exe" build MedicalDataSystem.csproj
```

Subsystem 1:

```powershell
cd Subsystem1-OracleDBAdmin\Source\OracleDBAdmin
& "C:\Program Files\dotnet\dotnet.exe" restore
& "C:\Program Files\dotnet\dotnet.exe" build OracleDBAdmin.csproj
```

## Notes

- Some older files in the repository use historical names such as `Subsystem1-AdminDB`. Treat those as folder names, not proof that the final design must use a separate admin database.
- `Reset.sql` and `Report.sql` under `database/Subsystem2-MedicalDB` now match the current VPD, OLS, FGA, and Oracle-user setup.
