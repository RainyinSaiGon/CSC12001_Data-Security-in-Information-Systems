# Database Scripts

This folder contains the Oracle setup assets for the project.

If anything here conflicts with [docs/designs/Requirements.md](../docs/designs/Requirements.md), the assignment document wins.

## Main Folder To Use

For the working hospital demo, the important folder is:

- [database/Subsystem2-MedicalDB](Subsystem2-MedicalDB)

That folder contains:

- schema scripts
- RBAC setup
- VPD setup
- OLS setup
- audit setup
- reset and report scripts

## Working Script Order

Use the project PDB service:

```text
localhost:1521/XEPDB1
```

### As SYS

Run:

```sql
@database/Subsystem2-MedicalDB/Create_HOSPITAL_ADMIN.sql
```

### As HOSPITAL_ADMIN

Run:

```sql
@database/Subsystem2-MedicalDB/Reset.sql

@database/Subsystem2-MedicalDB/schema/01_CreateTables.sql
@database/Subsystem2-MedicalDB/schema/02_CreateIndexes.sql
@database/Subsystem2-MedicalDB/schema/03_InsertSampleData.sql

@database/Subsystem2-MedicalDB/security/01_RBAC_Setup.sql
@database/Subsystem2-MedicalDB/security/02_VPD_Setup.sql
```

Then run OLS in two passes:

```sql
@database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql
```

Reconnect as `HOSPITAL_ADMIN`, then run the same file again:

```sql
@database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql
```

Then finish audit:

```sql
@database/Subsystem2-MedicalDB/audit/01_StandardAudit_Setup.sql
@database/Subsystem2-MedicalDB/audit/02_FGA_Setup.sql
@database/Subsystem2-MedicalDB/Report.sql
```

Optional:

```sql
@database/Subsystem2-MedicalDB/audit/03_ReadAuditLogs.sql
```

## Important Current Notes

- `Create_HOSPITAL_ADMIN.sql` creates or unlocks `HOSPITAL_ADMIN` with password `12345678`
- `01_RBAC_Setup.sql` creates runtime Oracle users with password is the same to username
- `03_OLS_Setup.sql` is intentionally two-pass
- old `HOS_OLS_POL` cleanup is already handled in reset and OLS setup

## Folder Status

Ready now:

- `schema/`
- `security/`
- `audit/`
- `Reset.sql`
- `Report.sql`
- `recovery/`

The `recovery/` folder is checked in and documented; it still needs to be exercised in the target Oracle environment before the final demo.
