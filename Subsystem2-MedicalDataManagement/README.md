# Subsystem 2: Medical Data Management

This folder contains the checked-in source for the medical-data module of the CSC12001 project.

## Scope

Subsystem 2 supports the hospital scenario in the assignment:

- patient records
- medical records
- diagnostic services
- prescriptions
- notifications
- Oracle-enforced security

Required security mapping from the assignment:

- RBAC for technician and patient cases
- VPD for coordinator and doctor cases
- OLS for `THONGBAO`
- auditing for sensitive operations

## Current Source Layout

```text
subsystem2-medicalDataManagement/source/medicalDataSystem/
|-- forms/
|-- models/
|-- services/
|-- Program.cs
`-- MedicalDataSystem.csproj
```

## Important Notes

- The assignment requires one integrated application with both subsystem modules.
- The repository now also contains the Subsystem 1 administration client in `Subsystem1-OracleDBAdmin`.
- Security must ultimately be enforced by Oracle, not only by UI logic.

## Requirements-Aligned Data Model

Keep the required assignment relations recognizable:

- `BENHNHAN`
- `NHANVIEN`
- `HSBA`
- `HSBA_DV`
- `DONTHUOC`
- `THONGBAO`

Avoid drifting to renamed keys or replacement tables that no longer match the assignment text.

## Build

```powershell
cd subsystem2-medicalDataManagement\source
& "C:\Program Files\dotnet\dotnet.exe" restore
& "C:\Program Files\dotnet\dotnet.exe" build MedicalDataSystem.csproj
```

## Database Setup

Use the scripts that currently exist under `database/Subsystem2-MedicalDB`:

```sql
sqlplus hospital_admin/<STRONG_PASSWORD>@localhost:1521/XE

@database/Subsystem2-MedicalDB/Reset.sql

@database/Subsystem2-MedicalDB/schema/01_CreateTables.sql
@database/Subsystem2-MedicalDB/schema/02_CreateIndexes.sql
@database/Subsystem2-MedicalDB/schema/03_InsertSampleData.sql

@database/Subsystem2-MedicalDB/security/01_RBAC_Setup.sql
@database/Subsystem2-MedicalDB/security/02_VPD_Setup.sql
@database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql

@database/Subsystem2-MedicalDB/audit/01_StandardAudit_Setup.sql
@database/Subsystem2-MedicalDB/audit/02_FGA_Setup.sql
```

Audit scripts for Requirement 3 are checked in under `database/Subsystem2-MedicalDB/audit/`. Recovery scripts for Requirement 4 are still pending.

## Implementation Reminder

For final behavior, the application should authenticate and test permissions in a way that lets Oracle enforce:

- the active Oracle user identity
- Oracle roles and grants
- VPD predicates
- OLS labels
- audit policies

If the application always connects as one high-privilege account, the required Oracle security behavior will not be demonstrated correctly.
