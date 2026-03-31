# Demo Script

This file is a live demo guide for `Yeu cau 1` to `Yeu cau 3` in the project.

Source of truth:
- Requirement document: `docs/designs/Requirements.md`
- Database scripts: `database/Subsystem2-MedicalDB`
- Subsystem 1 UI: `Subsystem1-OracleDBAdmin`
- Subsystem 2 UI: `subsystem2-medicalDataManagement`

## 1. Demo Goal

Show that the system supports:
- Oracle account creation and account-to-row mapping
- RBAC for `Ky thuat vien` and `Benh nhan`
- VPD for `Dieu phoi vien` and `Bac si / Y si`
- OLS-based notification distribution
- Standard Audit and Fine-Grained Audit

## 2. Before The Demo

Prepare these tools:
- Oracle SQL Developer or SQL*Plus
- The `OracleDBAdmin` WinForms app
- The `MedicalDataSystem` WinForms app

Recommended Oracle accounts:
- One DBA or schema owner account for setup
- Sample business accounts created by the scripts

## 3. Exact SQL Execution Order

Run these scripts in order.

### 3.1 Database schema

1. `database/Subsystem2-MedicalDB/schema/01_CreateTables.sql`
2. `database/Subsystem2-MedicalDB/schema/02_CreateIndexes.sql`
3. `database/Subsystem2-MedicalDB/schema/03_InsertSampleData.sql`

### 3.2 Security setup

4. `database/Subsystem2-MedicalDB/security/01_RBAC_Setup.sql`
5. `database/Subsystem2-MedicalDB/security/02_VPD_Setup.sql`
6. `database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql`
   Run this one in 2 passes:
   First run creates `THONGBAO_OLS` and intentionally stops with a reconnect message.
   Reconnect as the same Oracle user, then run it a second time to finish components, labels, table policy, and sample notifications.

### 3.3 Audit setup

7. `database/Subsystem2-MedicalDB/audit/01_StandardAudit_Setup.sql`
8. `database/Subsystem2-MedicalDB/audit/02_FGA_Setup.sql`

### 3.4 Audit reading

9. `database/Subsystem2-MedicalDB/audit/03_ReadAuditLogs.sql`

Notes:
- Password created for generated users in `01_RBAC_Setup.sql` is `123`.
- `Reset.sql` and `Report.sql` are now aligned with the current VPD, OLS, FGA, and Oracle-user setup.
- For a clean rerun before the demo, start with `database/Subsystem2-MedicalDB/Reset.sql`, then run the ordered setup scripts below.

## 4. Sample User Queries

Run these queries after setup to pick accounts with useful demo data.

```sql
SELECT username
FROM nhanvien
WHERE vaitro = N'Điều phối viên'
FETCH FIRST 3 ROWS ONLY;
```

```sql
SELECT username
FROM nhanvien n
WHERE vaitro = N'Bác sĩ/Y sĩ'
  AND EXISTS (
      SELECT 1
      FROM hsba h
      WHERE h.mabs = n.manv
  )
FETCH FIRST 3 ROWS ONLY;
```

```sql
SELECT username
FROM nhanvien n
WHERE vaitro = N'Kỹ thuật viên'
  AND EXISTS (
      SELECT 1
      FROM hsba_dv d
      WHERE d.maktv = n.manv
  )
FETCH FIRST 3 ROWS ONLY;
```

```sql
SELECT username
FROM benhnhan b
WHERE EXISTS (
    SELECT 1
    FROM hsba h
    WHERE h.mabn = b.mabn
)
FETCH FIRST 3 ROWS ONLY;
```

Typical ranges from sample data:
- Coordinators: `NV000001` to `NV000020`
- Doctors: `NV000021` to `NV000120`
- Technicians: `NV000121` to `NV000170`
- Patients: `BN000000001` upward

## 5. Quick Verification Queries

Use these to confirm setup before starting the UI demo.

### 5.1 Roles exist

```sql
SELECT role
FROM dba_roles
WHERE role IN ('DIEU_PHOI_VIEN', 'BAC_SI_Y_SI', 'KY_THUAT_VIEN', 'BENH_NHAN')
ORDER BY role;
```

### 5.2 Generated Oracle users exist

```sql
SELECT username
FROM dba_users
WHERE username IN ('NV000001', 'NV000021', 'NV000121', 'BN000000001')
ORDER BY username;
```

### 5.3 Role grants exist

```sql
SELECT grantee, granted_role
FROM dba_role_privs
WHERE grantee IN ('NV000001', 'NV000021', 'NV000121', 'BN000000001')
ORDER BY grantee, granted_role;
```

### 5.4 VPD policies exist

```sql
SELECT object_name, policy_name, enable
FROM user_policies
WHERE policy_name IN ('HSBA_VPD', 'BENHNHAN_VPD', 'HSBA_DV_VPD', 'DONTHUOC_VPD')
ORDER BY object_name, policy_name;
```

## 6. Live Demo Flow

This order is presentation-friendly.

### Step 1. Show setup completed

Say:
"I already ran reset, schema, RBAC, VPD, OLS, and audit setup scripts in the required order."

Show:
- role query
- Oracle user query
- VPD policy query

Expected result:
- 4 roles exist
- sample users exist
- VPD policies are enabled

### Step 2. Yeu cau 1, Cau 1: Oracle account management

Open `OracleDBAdmin`.

Show:
- connection panel
- Users tab
- Roles tab
- Permissions tab
- Privilege Viewer tab

What to do:
1. Connect using the DBA or schema owner account.
2. Show list of users.
3. Show list of roles.
4. Optionally create a test user, then reset password, then drop the user.

Say:
"Subsystem 1 uses Oracle-managed accounts and privileges directly. It does not use custom account tables."

### Step 3. Yeu cau 1, Cau 2: Coordinator via VPD and patient/technician via RBAC

Open `MedicalDataSystem`.

#### 3.1 Coordinator demo

Login with a coordinator account such as `NV000001` and password `123`.

Show:
- `Coordinator Dashboard`
- patient grid
- add patient controls
- create record and assign doctor
- assign technician

Do:
1. Add one patient.
2. Create one `HSBA` for a patient and assign a doctor.
3. Assign one technician for a service.

Expected:
- coordinator can work with patient records and orchestration actions

#### 3.2 Technician demo

Logout and login with a technician account returned by the helper query.

Show:
- `Technician Dashboard`
- assigned service list

Do:
1. Open only assigned services.
2. Update `KETQUA`.

Expected:
- technician only sees assigned rows
- result update succeeds for own assignment

Optional proof query:

```sql
SELECT *
FROM v_technician_hsba_dv;
```

#### 3.3 Patient demo

Logout and login with a patient account returned by the helper query.

Show:
- `Patient Portal`
- self profile
- medical records tab
- prescriptions tab

Do:
1. Update allowed self fields such as address or medical history.
2. Show that personal records and prescriptions are visible.

Expected:
- patient sees only personal data
- patient can update only allowed profile fields

### Step 4. Yeu cau 1, Cau 3: Doctor with VPD

Login with a doctor account that has existing records.

Show:
- `Doctor Dashboard`
- assigned patients grid
- assigned records grid

Do:
1. Update `CHANDOAN`, `DIEUTRI`, `KETLUAN`.
2. Add one diagnostic service.
3. Add or update one prescription.

Expected:
- doctor only sees own treatment records
- updates on own records succeed

Optional proof query from another session:

```sql
SELECT mahsba, mabs, chandoan, dieutri, ketluan
FROM hsba
ORDER BY mahsba;
```

Say:
"The application uses the Oracle account of the logged-in user, and VPD filters the doctor-visible rows automatically."

### Step 5. Yeu cau 2: OLS notifications

Use one or more of the sample labeled users in `03_OLS_Setup.sql`.

Recommended:
- `NV000001`
- `NV000090`
- `NV000060`
- `NV000061`
- `NV000030`

From any role form, click `Notifications`.

Show:
- notification grid
- different visible messages for different users

What to verify:
- `t1` goes to all employees
- `t2` goes only to board-level users
- `t3` goes to department leaders
- `t4` goes to leaders of gastroenterology
- `t5` goes to gastro employees in Ho Chi Minh
- `t6` goes to gastro employees in Ha Noi
- `t7` goes to leaders of gastro and neurology in Hai Phong

Suggested query:

```sql
SELECT mathongbao, noidung, ngaygio, diadiem
FROM thongbao
ORDER BY ngaygio DESC;
```

Say:
"The same table is queried, but Oracle Label Security automatically limits what each user can read."

### Step 6. Yeu cau 3: Audit

Perform these actions first:
- doctor updates one valid `HSBA`
- doctor attempts an invalid `HSBA` update on another doctor's row
- doctor updates `DONTHUOC` after it already exists
- unauthorized user attempts illegal `HSBA_DV` change

Then run:
- `database/Subsystem2-MedicalDB/audit/03_ReadAuditLogs.sql`

What to show:
- standard audit rows in `DBA_AUDIT_TRAIL`
- FGA rows in `DBA_FGA_AUDIT_TRAIL`

Say:
"Standard Audit captures the general access scenarios, and FGA captures the sensitive business cases required by the assignment."

## 7. Testcase Checklist

### TC#1

- Accounts exist for employees and patients
- Oracle account maps to exactly one `NHANVIEN` or `BENHNHAN` row
- Login succeeds using generated Oracle users

### TC#2

- Coordinator can view patients
- Coordinator can add patient
- Coordinator can create `HSBA`
- Coordinator can assign doctor
- Coordinator can assign technician

### TC#3

- Doctor sees only own `HSBA`
- Doctor can insert and delete `HSBA_DV`
- Doctor can update `CHANDOAN`, `DIEUTRI`, `KETLUAN`
- Doctor can update patient history for own patients
- Doctor can insert, update, delete `DONTHUOC`

### TC#4

- Technician sees only assigned `HSBA_DV`
- Technician can update only `KETQUA`
- Illegal changes are blocked

### TC#5

- Staff sees only own row in `NHANVIEN`
- Patient sees only own row in `BENHNHAN`
- Allowed self-update fields work
- Identity and protected fields are not editable

### Yeu cau 2

- OLS policy created successfully
- Labels exist
- Demo users have labels
- Users see only allowed notifications

### Yeu cau 3

- Standard Audit enabled
- 5 standard audit contexts exist
- FGA policies exist
- Valid and invalid actions produce audit rows
- Audit logs can be read back successfully

## 8. Fast Recovery If Something Goes Wrong During Demo

If login fails:
- confirm the user exists in `DBA_USERS`
- confirm role grants in `DBA_ROLE_PRIVS`
- confirm password is `123`

If a role sees no data:
- rerun the helper query to choose a user that actually has related rows
- confirm sample data was inserted before security setup

If notifications are empty:
- rerun `03_OLS_Setup.sql`
- make sure the testing username is one of the users labeled in that script

If audit log is empty:
- perform the audited actions again
- rerun `03_ReadAuditLogs.sql` using a DBA-capable account

## 9. Short Demo Closing

Suggested closing sentence:

"The system satisfies the assignment by combining Oracle-managed accounts, RBAC, VPD, OLS, and auditing, with separate UIs for administration and medical workflows."
