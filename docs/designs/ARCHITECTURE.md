# Architecture & Design

This document follows [Requirements.md](./Requirements.md) as the source of truth. If any other note, README, or task brief conflicts with the assignment text, `Requirements.md` wins.

## Design Principles

- The project is one application with two functional modules, not two unrelated systems.
- Oracle accounts, roles, and privileges are managed by Oracle itself.
- Subsystem 1 must not introduce custom tables that replace Oracle user or role management.
- Security controls must be enforced in the database first, with the WinForms UI acting as an operator interface.
- Any schema extension is allowed only when it supports the assignment without changing the required meaning of the original relations.

## System Overview

```text
CSC12001 Application
|
+-- Module 1: Oracle DB Administration
|   +-- Create, alter, drop users and roles
|   +-- Grant and revoke system, role, object, and column privileges
|   +-- Inspect effective privileges
|
+-- Module 2: Medical Data Management
|   +-- Coordinator workflow
|   +-- Doctor workflow
|   +-- Technician workflow
|   +-- Patient self-service workflow
|   +-- Notification viewer for OLS
|
+-- Oracle Database
    +-- Oracle users and roles
    +-- BENHNHAN
    +-- NHANVIEN
    +-- HSBA
    +-- HSBA_DV
    +-- DONTHUOC
    +-- THONGBAO
    +-- RBAC, VPD, OLS, Audit, Backup/Recovery
```

## Module Boundaries

### Module 1: Oracle DB Administration

The first module is an administration UI for real Oracle security objects. It should work with Oracle data dictionary views and Oracle DDL, for example:

- `DBA_USERS`, `ALL_USERS`
- `DBA_ROLES`
- `DBA_SYS_PRIVS`
- `DBA_TAB_PRIVS`
- `DBA_COL_PRIVS`
- `ROLE_ROLE_PRIVS`
- `DBA_OBJECTS`
- `DBA_PROCEDURES`
- `DBA_TAB_COLUMNS`

Required capabilities from the assignment:

1. Create, edit, and drop users or roles.
2. List users and roles in Oracle.
3. Grant privileges to users and roles, including role-to-user assignment.
4. Support `WITH GRANT OPTION` where applicable.
5. Support object privilege management on tables, views, stored procedures, and functions.
6. Support column-level `SELECT` and `UPDATE`.
7. Revoke privileges from users or roles.
8. Display privileges already granted on database objects.

Important constraint:

- Do not model Oracle accounts in custom tables such as `ADMIN_USERS` or `ADMIN_ROLES`. Those ideas were part of an older draft and are not valid for this project.

### Module 2: Medical Data Management

The second module implements the hospital scenario from the assignment and must enforce the required policies on Oracle:

- Technicians and patients: RBAC-based implementation for the required actions.
- Coordinators and doctors: VPD-based implementation for the required actions.
- Notifications: OLS-based implementation on `THONGBAO`.
- Audit: Standard Audit plus FGA or Unified Audit for the required scenarios.
- Backup and recovery: Oracle-native backup and recovery procedures, no UI required.

Suggested forms in the combined WinForms application:

- `LoginForm`
- `AdminMainForm`
- `UserManagementForm`
- `RoleManagementForm`
- `PermissionForm`
- `PrivilegeViewerForm`
- `CoordinatorForm`
- `DoctorForm`
- `TechnicianForm`
- `PatientForm`
- `NotificationForm`

## Required Data Model

The base schema should preserve the assignment relations and names.

### BENHNHAN

Required columns:

- `MABN`
- `TENBN`
- `PHAI`
- `NGAYSINH`
- `CCCD`
- `SONHA`
- `TENDUONG`
- `QUANHUYEN`
- `TINHTP`
- `TIENSUBENH`
- `TIENSUBENHGD`
- `DIUNGTHUOC`

### NHANVIEN

Required columns:

- `MANV`
- `HOTEN`
- `PHAI`
- `NGAYSINH`
- `CMND`
- `QUEQUAN`
- `SODT`
- `VAITRO`
- `CHUYENKHOA`

Allowed role values are the ones stated in the assignment:

- `Dieu phoi vien`
- `Bac si/Y si`
- `Ky thuat vien`
- `Benh nhan`

### HSBA

Required columns:

- `MAHSBA`
- `MABN`
- `NGAY`
- `CHANDOAN`
- `DIEUTRI`
- `MABS`
- `MAKHOA`
- `KETLUAN`

### HSBA_DV

Required columns:

- `MAHSBA`
- `LOAIDV`
- `NGAYDV`
- `MAKTV`
- `KETQUA`

Recommended key:

- Composite primary key on `MAHSBA, LOAIDV, NGAYDV`

### DONTHUOC

Required columns:

- `MAHSBA`
- `NGAYDT`
- `TENTHUOC`
- `LIEUDUNG`

Recommended key:

- Composite primary key on `MAHSBA, NGAYDT, TENTHUOC`

### THONGBAO

Requirement 2 explicitly introduces `THONGBAO` with at least:

- `NOIDUNG`
- `NGAYGIO`
- `DIADIEM`

For OLS, it is acceptable to add:

- A hidden Oracle label column managed by OLS
- Optional helper columns for seed data only, if they do not replace the OLS label as the enforcement mechanism

## Allowed Supporting Extensions

The assignment allows adjusting the model when needed. The following extensions are consistent with the requirements:

- Add `USERNAME` to `NHANVIEN` and `BENHNHAN` to link each Oracle account to exactly one row.
- Create a `UNION ALL` view such as `V_SYSTEM_USERS` so the application can resolve the current session user from one logical object instead of scattering lookup logic.
- Add lookup tables for departments or locations if useful, but do not replace the required relations above.
- Add audit helper views or packaged procedures.

## Account-to-Row Mapping Strategy

TC#1 requires Oracle-managed accounts without a custom account-management table. A practical design is:

1. Oracle stores the actual login accounts.
2. `NHANVIEN.USERNAME` links staff accounts to staff rows.
3. `BENHNHAN.USERNAME` links patient accounts to patient rows.
4. A helper view presents both sources as one logical user directory for the application.
5. Policies use `SYS_CONTEXT('USERENV', 'SESSION_USER')` to identify the active Oracle account.

This satisfies the requirement more closely than the older design that authenticated only through `NHANVIEN`.

## Security Architecture

### RBAC Scope

Use Oracle roles and grants to implement the role-based cases required in Requirement 1, Question 2:

- Technician permissions on assigned service work
- Patient self-view and self-update permissions

RBAC can be combined with:

- Views
- Stored procedures
- Column-specific grants

This is acceptable as long as Oracle roles remain the primary authorization mechanism for these cases.

### VPD Scope

Use VPD for the roles explicitly required in Requirement 1, Question 3:

- Coordinators
- Doctors

Typical policy targets:

- `HSBA`
- `HSBA_DV`
- Coordinator-visible patient workflow objects
- Doctor-visible patient workflow objects

VPD predicates should follow the real schema. For example, doctor visibility should be derived from `HSBA.MABS`, not from a nonexistent `BENHNHAN.MANV`.

### OLS Scope

Apply OLS to `THONGBAO` using three label components required by the assignment:

- Level: `Ban giam doc > Lanh dao khoa > Nhan vien`
- Compartment: department such as Tim mach, Than kinh, Tieu hoa
- Group: location such as Ho Chi Minh, Hai Phong, Ha Noi

The design must support all assignment examples `u1` to `u8` and data labels `t1` to `t7`, including messages that target multiple departments at one location.

### Audit Scope

Requirement 3 needs:

- Standard Audit for five chosen scenarios
- FGA or Unified Audit for:
  - Prescription updates after creation
  - Valid doctor updates on `HSBA`
  - Invalid updates on `HSBA`
  - Invalid insert, update, delete on `HSBA_DV`

### Backup and Recovery Scope

Requirement 4 needs:

- Research of Oracle backup and recovery methods
- Manual and automatic backup
- Recovery based on incident timing visible in audit logs
- Evaluation of pros and cons

## Logical Access Matrix

### Coordinator

- View, add, and edit `BENHNHAN`
- Create `HSBA`
- Assign `MABS`
- Coordinate `MAKTV` for `HSBA_DV`
- Enforced with VPD for coordinator scenarios

### Doctor

- View only `HSBA` rows assigned to that doctor
- Insert and delete related `HSBA_DV`
- Update `CHANDOAN`, `DIEUTRI`, `KETLUAN`
- View related patients
- Update `TIENSUBENH`, `TIENSUBENHGD`, `DIUNGTHUOC` of treated patients
- Add, delete, and update `DONTHUOC`
- Enforced with VPD and audited where required

### Technician

- View only assigned `HSBA_DV` rows
- Update only `KETQUA`
- Enforced with RBAC-oriented design plus role-specific filtering objects

### Patient

- View only personal row in `BENHNHAN`
- Update only allowed personal fields
- Enforced with RBAC-oriented design plus self-service filtering objects

## Current Repository Note

This repository snapshot currently contains:

- Design and task documentation
- Database folders for both subsystem tracks
- Checked-in application source for `subsystem2-medicalDataManagement`
- Checked-in application source for `Subsystem1-OracleDBAdmin`
- Checked-in audit, reset, and verification scripts under `database/Subsystem2-MedicalDB`

Requirement 4 backup and recovery assets are still incomplete, so README and setup documents should continue to describe that gap honestly.
