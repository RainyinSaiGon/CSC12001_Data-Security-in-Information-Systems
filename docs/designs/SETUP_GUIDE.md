# Setup Guide

This guide is aligned to [Requirements.md](./Requirements.md). It describes how to prepare the Oracle environment and the current repository snapshot for the course project.

## Current Repository Snapshot

The repository currently contains:

- `docs/designs/Requirements.md` as the authoritative assignment text
- database scripts under `database/`
- checked-in source for `subsystem2-medicalDataManagement`
- checked-in source for `Subsystem1-OracleDBAdmin`
- task briefs for both subsystems

This guide therefore separates:

- required Oracle/database setup for the full project
- what can already be built from the current repo snapshot

## Core Assumptions

- Windows 10 or Windows 11
- Visual Studio 2022 or newer
- .NET 10 SDK
- Oracle Database 21c environment
- SQL*Plus or SQL Developer
- A `SYSDBA` account for installation

Before starting Requirement 2, verify that the Oracle environment you will use for the final demo supports Oracle Label Security. The project requirement depends on OLS.

## Recommended Oracle Identities

Use distinct identities for installation and runtime:

- `SYS` or another `SYSDBA` account: one-time installation and privileged setup
- schema owner, for example `HOSPITAL_ADMIN`: owns project tables, policies, and supporting objects
- runtime admin account: used by Subsystem 1 to perform Oracle administration tasks
- runtime staff and patient accounts: used by Subsystem 2 so Oracle can enforce RBAC, VPD, and OLS per real user

Do not use a single all-powerful application account as the normal runtime identity for every user.

## Step 1: Verify Prerequisites

```powershell
& "C:\Program Files\dotnet\dotnet.exe" --version
sc query OracleServiceXE
lsnrctl status
sqlplus -version
```

Expected outcome:

- `.NET 10.x` is installed
- Oracle service is running
- listener is active on port `1521`
- SQL*Plus is available

## Step 2: Create the Schema Owner

Connect as `SYSDBA` and create a dedicated owner account. Replace the placeholder password.

```sql
sqlplus / as sysdba

CREATE USER hospital_admin IDENTIFIED BY "<STRONG_PASSWORD>";

GRANT CONNECT, RESOURCE TO hospital_admin;
GRANT CREATE VIEW, CREATE PROCEDURE, CREATE SEQUENCE TO hospital_admin;
GRANT CREATE TRIGGER, CREATE TYPE, CREATE SYNONYM TO hospital_admin;
GRANT UNLIMITED TABLESPACE TO hospital_admin;

GRANT EXECUTE ON DBMS_RLS TO hospital_admin;
GRANT AUDIT SYSTEM TO hospital_admin;
GRANT SELECT_CATALOG_ROLE TO hospital_admin;
```

If you decide to let the schema owner create helper views over data dictionary information for Subsystem 1, grant only the additional privileges that are truly needed.

## Step 3: Install the Medical Schema

The checked-in scripts under `database/Subsystem2-MedicalDB` are the concrete setup assets currently present in the repo.

Run them in this order:

```sql
sqlplus hospital_admin/<STRONG_PASSWORD>@localhost:1521/XE

@database/Subsystem2-MedicalDB/Reset.sql

@database/Subsystem2-MedicalDB/schema/01_CreateTables.sql
@database/Subsystem2-MedicalDB/schema/02_CreateIndexes.sql
@database/Subsystem2-MedicalDB/schema/03_InsertSampleData.sql
```

Notes:

- Keep the required assignment relations intact: `BENHNHAN`, `NHANVIEN`, `HSBA`, `HSBA_DV`, `DONTHUOC`.
- `THONGBAO` is required for the OLS part.
- If you add helper columns such as `USERNAME`, document them as schema extensions, not replacements for the required columns.

## Step 4: Configure Security in Oracle

Run the security scripts that exist in the repo:

```sql
sqlplus hospital_admin/<STRONG_PASSWORD>@localhost:1521/XE

@database/Subsystem2-MedicalDB/security/01_RBAC_Setup.sql
@database/Subsystem2-MedicalDB/security/02_VPD_Setup.sql
@database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql
```

Required interpretation:

- `01_RBAC_Setup.sql` should cover Oracle user creation and role grants for the RBAC cases in the assignment.
- `02_VPD_Setup.sql` should implement coordinator and doctor policies.
- `03_OLS_Setup.sql` should implement Requirement 2 on `THONGBAO`.

Audit scripts are now checked in under `database/Subsystem2-MedicalDB/audit/`. Recovery scripts are still pending for Requirement 4.

For Requirement 3 setup and verification, run:

```sql
@database/Subsystem2-MedicalDB/audit/01_StandardAudit_Setup.sql
@database/Subsystem2-MedicalDB/audit/02_FGA_Setup.sql
@database/Subsystem2-MedicalDB/Report.sql
@database/Subsystem2-MedicalDB/audit/03_ReadAuditLogs.sql
```

## Step 5: Provision Runtime Oracle Accounts

The assignment requires Oracle-managed accounts for staff and patients.

Recommended pattern:

1. Add `USERNAME` to `NHANVIEN` and `BENHNHAN`, or create an equivalent supported mapping strategy.
2. Create Oracle users for:
   - coordinators
   - doctors
   - technicians
   - test patients
3. Grant Oracle roles according to the assignment.
4. For policy logic, resolve the active row via `SYS_CONTEXT('USERENV', 'SESSION_USER')`.

Do not create a custom account-management table for this purpose.

## Step 6: Build the Checked-In Applications

```powershell
cd subsystem2-medicalDataManagement\source
& "C:\Program Files\dotnet\dotnet.exe" restore
& "C:\Program Files\dotnet\dotnet.exe" build MedicalDataSystem.csproj
```

```powershell
cd Subsystem1-OracleDBAdmin\Source\OracleDBAdmin
& "C:\Program Files\dotnet\dotnet.exe" restore
& "C:\Program Files\dotnet\dotnet.exe" build OracleDBAdmin.csproj
```

## Step 7: Configure Application Secrets

Use secrets or a local non-committed configuration file. Avoid hardcoded credentials.

```powershell
& "C:\Program Files\dotnet\dotnet.exe" user-secrets init
& "C:\Program Files\dotnet\dotnet.exe" user-secrets set "OracleDbConnection:DataSource" "localhost:1521/XE"
& "C:\Program Files\dotnet\dotnet.exe" user-secrets set "OracleDbConnection:UserId" "<RUNTIME_USER>"
& "C:\Program Files\dotnet\dotnet.exe" user-secrets set "OracleDbConnection:Password" "<RUNTIME_PASSWORD>"
```

For security testing, connect as the actual Oracle end user whose privileges you want to verify, not always as the schema owner.

## Suggested Validation Checklist

### Schema Validation

```sql
SELECT table_name
FROM user_tables
WHERE table_name IN ('BENHNHAN', 'NHANVIEN', 'HSBA', 'HSBA_DV', 'DONTHUOC', 'THONGBAO');
```

### Role and User Validation

```sql
SELECT username FROM dba_users;
SELECT role FROM dba_roles;
```

### VPD Validation

```sql
SELECT object_owner, object_name, policy_name
FROM dba_policies
WHERE object_owner = 'HOSPITAL_ADMIN';
```

### Audit Validation

Use the audit views appropriate to the auditing mode you configure later in Requirement 3, for example:

- `DBA_AUDIT_TRAIL`
- `DBA_FGA_AUDIT_TRAIL`
- `UNIFIED_AUDIT_TRAIL`

## Troubleshooting

### `ORA-12154`

Use a full connect descriptor such as:

```text
Data Source=localhost:1521/XE
```

### `ORA-01017`

Verify the exact Oracle account and password:

```sql
sqlplus some_user/some_password@localhost:1521/XE
```

### `ORA-12541`

Start the listener:

```powershell
lsnrctl start
```

### Missing OLS Features

If the Oracle environment used on your machine does not expose OLS features needed for Requirement 2, do not hide that fact in the docs. Document the limitation and move the OLS demo to a compatible Oracle environment for the final evaluation.

## What Is Still Missing in the Repo

The following items are still required by the assignment but are not fully checked in as finished assets in the current snapshot:

- backup and recovery scripts for Requirement 4

That is a project-status fact, not a requirements change.
