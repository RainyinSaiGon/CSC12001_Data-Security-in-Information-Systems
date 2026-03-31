# Database Scripts

This directory contains Oracle scripts and placeholders used by the project. The assignment still requires one integrated system with two modules, even if some folder names here come from an older draft.

## Important Rule

If a database note conflicts with [docs/designs/Requirements.md](../docs/designs/Requirements.md), the assignment document wins.

## What This Folder Means

- `Subsystem1-AdminDB/` is a historical folder name for Subsystem 1 work.
- It should not be interpreted as a requirement to build a separate custom admin database for users and roles.
- Subsystem 1 must manage real Oracle users, roles, and privileges.
- Subsystem 2 stores the medical data and the Oracle-enforced security mechanisms required by the assignment.

## Current Checked-In Assets

### `Subsystem2-MedicalDB`

Currently checked in:

- `schema/01_CreateTables.sql`
- `schema/02_CreateIndexes.sql`
- `schema/03_InsertSampleData.sql`
- `security/01_RBAC_Setup.sql`
- `security/02_VPD_Setup.sql`
- `security/03_OLS_Setup.sql`
- `audit/01_StandardAudit_Setup.sql`
- `audit/02_FGA_Setup.sql`
- `audit/03_ReadAuditLogs.sql`
- `Reset.sql`
- `Report.sql`

Currently present but not yet implemented as complete project assets:

- `recovery/`

### `Subsystem1-AdminDB`

The folder exists, but it should be used for Subsystem 1 support scripts only. Those scripts should help the admin UI work with:

- Oracle users
- Oracle roles
- system privileges
- object privileges
- column privileges
- Oracle data dictionary views

They should not define replacement account tables such as `ADMIN_USERS` or `ADMIN_ROLES`.

## Requirements-Aligned Schema Summary

The core relations required by the assignment are:

- `BENHNHAN`
- `NHANVIEN`
- `HSBA`
- `HSBA_DV`
- `DONTHUOC`
- `THONGBAO` for the OLS part

Recommended design rules:

- Preserve the required column names from the assignment.
- Prefer composite keys for `HSBA_DV` and `DONTHUOC`.
- If you add helper columns such as `USERNAME`, document them as extensions.
- Keep Oracle as the source of truth for accounts and roles.

## Execution Order

For the scripts currently in the repo:

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

@database/Subsystem2-MedicalDB/Report.sql
@database/Subsystem2-MedicalDB/audit/03_ReadAuditLogs.sql
```

Audit scripts for Requirement 3 are checked in. Recovery scripts for Requirement 4 are still pending.

## Security Mapping

### Subsystem 1

Use Oracle catalog views and DDL for:

- create, alter, drop user
- create, drop role
- grant and revoke role
- grant and revoke system privilege
- grant and revoke object privilege
- column-level `SELECT` and `UPDATE`
- privilege inspection

### Subsystem 2

Use Oracle features according to the assignment:

- RBAC for technician and patient cases
- VPD for coordinator and doctor cases
- OLS for `THONGBAO`
- Standard Audit plus FGA or Unified Audit for the specified audit cases
- Oracle backup and recovery tools for Requirement 4

## Current Gap Summary

Not yet present as finished checked-in scripts:

- completed recovery scripts under `Subsystem2-MedicalDB/recovery/`
- finalized Subsystem 1 Oracle support scripts aligned with the assignment
