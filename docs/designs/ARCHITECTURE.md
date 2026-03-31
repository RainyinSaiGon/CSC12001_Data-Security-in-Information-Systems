# Architecture & Design

This document summarizes the intended design and the current working implementation path in the repository.

If anything here conflicts with [Requirements.md](Requirements.md), the assignment document wins.

## 1. System Shape

The project is one system with two modules:

- Module 1
  Oracle administration
- Module 2
  Medical data management

Both modules rely on Oracle as the real security engine.

## 2. Core Design Principles

- Oracle remains the source of truth for accounts, roles, privileges, and row filtering.
- Subsystem 1 must manage real Oracle users and roles, not custom replacement account tables.
- Subsystem 2 must use the required hospital relations from the assignment.
- Database security comes first. The WinForms apps are operator interfaces on top of Oracle behavior.

## 3. Working Repository Architecture

### Database owner

- schema owner: `HOSPITAL_ADMIN`
- setup password: `12345678`
- created by [Create_HOSPITAL_ADMIN.sql](../../database/Subsystem2-MedicalDB/Create_HOSPITAL_ADMIN.sql)

### Oracle runtime service

- expected service: `localhost:1521/XEPDB1`
- do not use `CDB$ROOT`

### Runtime Oracle users

The checked-in RBAC script creates Oracle users for employees and patients.

Important mapping:

- `NHANVIEN.USERNAME`
- `BENHNHAN.USERNAME`
- `SYS_CONTEXT('USERENV', 'SESSION_USER')`

That mapping is the basis for:

- self views
- VPD predicates
- OLS user labels
- audit context

## 4. Module 1

Subsystem 1 is the Oracle admin client.

Expected responsibilities:

- create or drop users
- create or drop roles
- grant and revoke object privileges
- grant roles to users
- inspect granted privileges

Important constraint:

- it should work with Oracle objects directly
- it should not invent a custom user database

## 5. Module 2

Subsystem 2 is the medical client.

Working role split:

- coordinator
  VPD-controlled workflow over patients and record assignment
- doctor
  VPD-controlled access to owned treatment records
- technician
  RBAC plus restricted objects for assigned service work
- patient
  RBAC plus self views for personal data and prescriptions

## 6. Data Model

The project keeps these recognizable relations:

- `BENHNHAN`
- `NHANVIEN`
- `HSBA`
- `HSBA_DV`
- `DONTHUOC`
- `THONGBAO`

Working extensions that are acceptable:

- `USERNAME` in `NHANVIEN`
- `USERNAME` in `BENHNHAN`
- Oracle OLS label column on `THONGBAO`
- helper views such as `V_SELF_NHANVIEN`, `V_SELF_BENHNHAN`, and other security-facing views

## 7. Security Design

### RBAC

Used for:

- technician permissions
- patient self-service permissions

Implemented with:

- Oracle roles
- grants
- restricted views
- column-specific updates where appropriate

### VPD

Used for:

- coordinator cases
- doctor cases

Implemented over the medical tables so Oracle filters rows automatically.

### OLS

Used for:

- `THONGBAO`

Current practical note:

- [03_OLS_Setup.sql](../../database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql) is intentionally two-pass
- first pass creates the policy
- second pass after reconnect completes components, labels, table policy, data, and user labels

### Audit

Used for:

- Standard Audit
- Fine-Grained Audit

to capture the required operations from Requirement 3.

## 8. Application Runtime Detail

The medical app uses runtime Oracle users such as `NV000021` and `BN000000001`, but it resolves shared objects through the `HOSPITAL_ADMIN` schema at session level.

That allows:

- real Oracle-user authentication
- Oracle-enforced RBAC, VPD, OLS, and audit behavior
- access to schema-owned tables and views without changing object ownership

## 9. Honest Status Note

The repository is in good shape for:

- schema setup
- RBAC
- VPD
- OLS
- Standard Audit
- FGA
- both WinForms apps

Still incomplete as checked-in project assets:

- backup and recovery scripts for Requirement 4
