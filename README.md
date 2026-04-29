# Data Security in Information Systems

Course project for `CSC12001 - Data Security in Information Systems`.

The authoritative assignment text is [docs/designs/Requirements.md](docs/designs/Requirements.md). If any note in this repository conflicts with that file, the assignment document wins.

## What This Repository Contains

- `Subsystem1-OracleDBAdmin`
  Oracle administration client for users, roles, and privileges.
- `subsystem2-medicalDataManagement`
  Medical-data client for coordinator, doctor, technician, and patient workflows.
- `database/Subsystem2-MedicalDB`
  Schema, security, audit, reset, and report scripts for the hospital scenario.
- `docs/designs`
  Setup and architecture guides.

## Quick Start

If your friend just cloned the repository and wants the project running, use this order:

1. Read [docs/designs/SETUP_GUIDE.md](docs/designs/SETUP_GUIDE.md).
2. Connect to the Oracle PDB service, usually `XEPDB1`, not `XE`.
3. Run [database/Subsystem2-MedicalDB/Create_HOSPITAL_ADMIN.sql](database/Subsystem2-MedicalDB/Create_HOSPITAL_ADMIN.sql) as `SYS`.
4. Reconnect as `HOSPITAL_ADMIN` and run the schema, RBAC, VPD, OLS, and audit scripts in the order shown in the setup guide.
5. Build and run the WinForms apps.
6. For the medical app, type `localhost:1521/XEPDB1` in the login form if the textbox still shows `XE`.

## Working Oracle Flow

The setup that currently works in this repository is:

- schema owner: `HOSPITAL_ADMIN`
- schema owner password: `12345678`
- generated employee and patient password: `123`
- Oracle service for the project: `localhost:1521/XEPDB1`
- OLS setup: two-pass flow
  first run creates `THONGBAO_OLS`, then reconnect, then run the same script again

## Sample Runtime Accounts

Use these for the medical app after setup:

- coordinator: `NV000001 / 123`
- doctor: `NV000021 / 123`
- technician: `NV000121 / 123`
- patient: `BN000000001 / 123`

Do not use `HOSPITAL_ADMIN` as an application user. It is the schema owner and setup account, not a mapped business user.

## Recommended Reading Order

1. [docs/designs/SETUP_GUIDE.md](docs/designs/SETUP_GUIDE.md)
2. [docs/designs/ARCHITECTURE.md](docs/designs/ARCHITECTURE.md)
3. [database/README.md](database/README.md)
4. [subsystem2-medicalDataManagement/README.md](subsystem2-medicalDataManagement/README.md)

## Build

Subsystem 2:

```powershell
dotnet restore "subsystem2-medicalDataManagement/source/medicalDataSystem/MedicalDataSystem.csproj"
dotnet build "subsystem2-medicalDataManagement/source/medicalDataSystem/MedicalDataSystem.csproj"
```

Subsystem 1:

```powershell
dotnet restore "Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/OracleDBAdmin.csproj"
dotnet build "Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/OracleDBAdmin.csproj"
```

## Current Project Status

What is ready in the repository:

- schema scripts
- RBAC setup
- VPD setup
- OLS setup
- Standard Audit and FGA setup
- reset and reporting scripts
- Requirement 4 backup and recovery scripts under `database/Subsystem2-MedicalDB/recovery`
- both WinForms projects

What still needs environment-specific validation before a live demo:

- Run and verify the Requirement 4 backup/recovery scripts against the target Oracle installation and OS paths

## Notes

- The medical app now relies on Oracle users and the schema owner `HOSPITAL_ADMIN` through `CURRENT_SCHEMA`, so role accounts such as `NV000021` and `BN000000001` can work against the shared objects.
- Some older folder names remain for historical reasons. They are repository names, not design requirements.
