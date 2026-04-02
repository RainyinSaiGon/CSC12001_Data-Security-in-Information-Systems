# Setup Guide

This guide is for teammates who clone the repository and want the project running with the least confusion.

If this guide conflicts with [Requirements.md](Requirements.md), the assignment document wins.

## 1. What You Need

- Windows 10 or Windows 11
- Oracle Database XE or another Oracle installation that exposes a PDB service such as `XEPDB1`
- Oracle listener on port `1521`
- SQL Developer or SQL*Plus
- a `SYSDBA` login
- .NET SDK installed
- this repository cloned locally

Important:

- Use the project PDB service, usually `XEPDB1`
- Do not run the project in `CDB$ROOT`
- Oracle Label Security must be available for Requirement 2

## 2. Clone The Repository

```powershell
git clone <YOUR_GITHUB_URL>
cd CSC12001_Data-Security-in-Information-Systems
```

## 3. Verify Oracle First

Check that Oracle and the listener are running:

```powershell
lsnrctl status
sqlplus -version
```

Then connect as `SYS` and confirm the container:

```sql
SELECT SYS_CONTEXT('USERENV', 'CON_NAME') FROM dual;
```

Expected result:

- a PDB such as `XEPDB1`
- not `CDB$ROOT`

## 4. Create The Schema Owner

Run [Create_HOSPITAL_ADMIN.sql](../../database/Subsystem2-MedicalDB/Create_HOSPITAL_ADMIN.sql) as `SYS` in the project PDB.

Example:

```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XEPDB1 as sysdba

@database/Subsystem2-MedicalDB/Create_HOSPITAL_ADMIN.sql
```

This script:

- creates or unlocks `HOSPITAL_ADMIN`
- sets password to `12345678`
- grants the Oracle privileges needed by the checked-in scripts
- grants OLS packages where available

## 5. Run The Database Scripts

Reconnect as `HOSPITAL_ADMIN`:

```sql
sqlplus HOSPITAL_ADMIN/12345678@localhost:1521/XEPDB1
```

Run the scripts in this exact order.

### 5.1 Reset

```sql
@database/Subsystem2-MedicalDB/Reset.sql
```

### 5.2 Schema

```sql
@database/Subsystem2-MedicalDB/schema/01_CreateTables.sql
@database/Subsystem2-MedicalDB/schema/02_CreateIndexes.sql
@database/Subsystem2-MedicalDB/schema/03_InsertSampleData.sql
```

### 5.3 Security

```sql
@database/Subsystem2-MedicalDB/security/01_RBAC_Setup.sql
@database/Subsystem2-MedicalDB/security/02_VPD_Setup.sql
```

### 5.4 OLS

Run [03_OLS_Setup.sql](../../database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql) in two passes.

Pass 1:

```sql
@database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql
```

Expected result:

- the script creates `THONGBAO_OLS`
- it stops with a reconnect message on purpose

Then disconnect and reconnect as the same user:

```sql
CONNECT HOSPITAL_ADMIN/12345678@localhost:1521/XEPDB1
```

Pass 2:

```sql
@database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql
```

This second run should:

- finish OLS components
- create labels
- apply the table policy
- insert `t1` to `t7` notifications
- assign OLS labels to the demo users

### 5.5 Audit

```sql
@database/Subsystem2-MedicalDB/audit/01_StandardAudit_Setup.sql
@database/Subsystem2-MedicalDB/audit/02_FGA_Setup.sql
```

### 5.6 Verification

```sql
@database/Subsystem2-MedicalDB/Report.sql
```

Optional audit log readback:

```sql
@database/Subsystem2-MedicalDB/audit/03_ReadAuditLogs.sql
```

## 6. Quick Verification Queries

Use these after setup and before opening the apps.

### Roles

```sql
SELECT role
FROM dba_roles
WHERE role IN ('DIEU_PHOI_VIEN', 'BAC_SI_Y_SI', 'KY_THUAT_VIEN', 'BENH_NHAN')
ORDER BY role;
```

### Sample Oracle users

```sql
SELECT username
FROM dba_users
WHERE username IN ('NV000001', 'NV000021', 'NV000121', 'BN000000001')
ORDER BY username;
```

### Role grants for sample users

```sql
SELECT grantee, granted_role
FROM dba_role_privs
WHERE grantee IN ('NV000001', 'NV000021', 'NV000121', 'BN000000001')
ORDER BY grantee, granted_role;
```

### VPD policies

```sql
SELECT object_name, policy_name, enable
FROM user_policies
ORDER BY object_name, policy_name;
```

### OLS policy

```sql
SELECT policy_name, column_name
FROM dba_sa_policies
WHERE policy_name = 'THONGBAO_OLS';
```

### Notifications

Reconnect once more as `HOSPITAL_ADMIN` after pass 2 if needed, then run:

```sql
SELECT noidung
FROM thongbao
WHERE REGEXP_LIKE(noidung, '^t[1-7]:')
ORDER BY noidung;
```

## 7. Practical Demo Check

If you only want to confirm the project is presentation-ready, use this short flow:

1. open the admin app and show Oracle users, roles, and privileges
2. log in to the medical app as coordinator: `NV000001 / 123`
3. log in as doctor: `NV000021 / 123`
4. log in as technician: `NV000121 / 123`
5. log in as patient: `BN000000001 / 123`
6. open notifications for one or more labeled users such as `NV000001`, `NV000090`, or `NV000060`
7. perform a few audited actions, then read the logs with:

```sql
@database/Subsystem2-MedicalDB/audit/03_ReadAuditLogs.sql
```

Good OLS demo users:

- `NV000001`
- `NV000090`
- `NV000060`
- `NV000061`
- `NV000030`

## 8. Build The Applications

### Subsystem 2

```powershell
dotnet restore "subsystem2-medicalDataManagement/source/medicalDataSystem/MedicalDataSystem.csproj"
dotnet build "subsystem2-medicalDataManagement/source/medicalDataSystem/MedicalDataSystem.csproj"
```

Run:

```powershell
dotnet run --project "subsystem2-medicalDataManagement/source/medicalDataSystem/MedicalDataSystem.csproj"
```

### Subsystem 1

```powershell
dotnet restore "Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/OracleDBAdmin.csproj"
dotnet build "Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/OracleDBAdmin.csproj"
dotnet run --project "Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/OracleDBAdmin.csproj"
```

## 9. App Login Notes

For the medical app:

- schema owner account is only for setup
- do not log in as `HOSPITAL_ADMIN`
- use the generated business accounts instead

Sample users:

- coordinator: `NV000001 / 123`
- doctor: `NV000021 / 123`
- technician: `NV000121 / 123`
- patient: `BN000000001 / 123`

Important:

- if the login textbox still shows `localhost:1521/XE`, replace it with `localhost:1521/XEPDB1`
- if the app was already open during a rebuild, close it before running again so the executable is not locked

## 10. Common Problems

### ORA-01017

Wrong username or password.

Check:

```sql
sqlplus NV000021/123@localhost:1521/XEPDB1
```

### ORA-12154 or ORA-12514

Wrong service name.

Use:

```text
localhost:1521/XEPDB1
```

not:

```text
localhost:1521/XE
```

unless your Oracle installation truly uses `XE` as the working PDB service.

### OLS first run stops

That is expected. Reconnect and run [03_OLS_Setup.sql](../../database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql) again.

### OLS shows ORA-12458

That is not the normal two-pass stop.

It means Oracle Label Security is not enabled in that database or PDB yet.

Connect as `SYSDBA` to the project PDB such as `XEPDB1`, then enable OLS:

```sql
EXEC LBACSYS.CONFIGURE_OLS;
EXEC LBACSYS.OLS_ENFORCEMENT.ENABLE_OLS;
```

After that:

- restart the Oracle database
- rerun [Create_HOSPITAL_ADMIN.sql](../../database/Subsystem2-MedicalDB/Create_HOSPITAL_ADMIN.sql) as `SYS`
- reconnect as `HOSPITAL_ADMIN`
- rerun [03_OLS_Setup.sql](../../database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql)

If those `LBACSYS` calls do not exist, that Oracle installation does not currently have OLS available, so Requirement 2 cannot run there as-is.

### Medical app shows ORA-00942

Usually one of these:

- setup scripts were not run in the correct order
- the user logged into the wrong Oracle service
- the app was not restarted after a build

### Notifications are empty

Usually one of these:

- `03_OLS_Setup.sql` was run only once
- you did not reconnect after the second OLS pass
- the current user was not assigned an OLS label in the setup script

### A role logs in but sees no useful rows

Usually one of these:

- sample data was not loaded correctly
- the chosen user has no related rows for that workflow
- setup was rerun partially instead of from reset

## 11. Honest Project Status

The repository is ready for:

- schema setup
- RBAC
- VPD
- OLS
- Standard Audit
- FGA
- both WinForms apps

Still incomplete as checked-in project assets:

- Requirement 4 backup and recovery scripts in `database/Subsystem2-MedicalDB/recovery`
