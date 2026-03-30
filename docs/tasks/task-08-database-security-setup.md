# Task 08: Subsystem 2 Database Security Setup

**Assigned to:** Ngoc, Vu  
**Priority:** Critical  

## Objective

Configure Oracle security for the medical module exactly as required by the assignment:

- RBAC for technician and patient cases
- VPD for coordinator and doctor cases
- OLS for `THONGBAO`

## Requirement Mapping

### TC#1: Oracle Account Provisioning

- Create Oracle accounts for staff and the patient sample set used for testing.
- Link each Oracle account to exactly one row in `NHANVIEN` or `BENHNHAN`.
- Do not create a custom account-management table.
- Recommended extension: `USERNAME` columns plus a helper view for lookup.

### RBAC Cases

Use Oracle roles and grants for the cases explicitly assigned to RBAC:

#### Technician

- view only assigned `HSBA_DV` rows
- update only `KETQUA`
- audit updates to `KETQUA`

#### Patient

- view only own information
- update only allowed personal fields
- must not update protected identity fields

### VPD Cases

Use VPD for the cases explicitly assigned to VPD:

#### Coordinator

- view, add, and edit `BENHNHAN`
- create `HSBA`
- assign `MABS`
- coordinate `MAKTV`

#### Doctor

- view only `HSBA` rows where that doctor is responsible
- add and delete `HSBA_DV` rows for treated cases
- update `CHANDOAN`, `DIEUTRI`, `KETLUAN`
- view related patients
- update `TIENSUBENH`, `TIENSUBENHGD`, `DIUNGTHUOC`
- add, delete, and update `DONTHUOC`

### OLS Case

Configure `THONGBAO` with three components:

- levels: `Ban giam doc > Lanh dao khoa > Nhan vien`
- compartments: `Tim mach`, `Than kinh`, `Tieu hoa`
- groups: `Ho Chi Minh`, `Hai Phong`, `Ha Noi`

The setup must support all example users `u1` to `u8` and all example messages `t1` to `t7`.

## Current Checked-In Script Names

The repository currently contains:

- `database/Subsystem2-MedicalDB/security/01_RBAC_Setup.sql`
- `database/Subsystem2-MedicalDB/security/02_VPD_Setup.sql`
- `database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql`

Keep those scripts aligned with the requirement mapping above.

## Acceptance Criteria

- Oracle accounts are mapped to rows without a custom account table.
- Technician and patient cases are implemented through Oracle-role-based design.
- Coordinator and doctor cases are implemented through VPD.
- `THONGBAO` is protected with a three-component OLS model.
- The design remains traceable to Requirement 1 and Requirement 2 without using the older separate-admin-database idea.
