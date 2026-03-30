# Task 07: Subsystem 2 Database Schema Setup

**Assigned to:** Ngoc, Vu  
**Type:** Database Administration  
**Priority:** Critical  

## Objective

Create the medical database schema so it matches the assignment data model first, then add only the minimum extensions needed for Oracle security and application lookup.

## Required Base Relations

### BENHNHAN

Required attributes:

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

Required attributes:

- `MANV`
- `HOTEN`
- `PHAI`
- `NGAYSINH`
- `CMND`
- `QUEQUAN`
- `SODT`
- `VAITRO`
- `CHUYENKHOA`

### HSBA

Required attributes:

- `MAHSBA`
- `MABN`
- `NGAY`
- `CHANDOAN`
- `DIEUTRI`
- `MABS`
- `MAKHOA`
- `KETLUAN`

### HSBA_DV

Required attributes:

- `MAHSBA`
- `LOAIDV`
- `NGAYDV`
- `MAKTV`
- `KETQUA`

Recommended key:

- composite key on `MAHSBA, LOAIDV, NGAYDV`

### DONTHUOC

Required attributes:

- `MAHSBA`
- `NGAYDT`
- `TENTHUOC`
- `LIEUDUNG`

Recommended key:

- composite key on `MAHSBA, NGAYDT, TENTHUOC`

### THONGBAO

Needed for Requirement 2:

- `NOIDUNG`
- `NGAYGIO`
- `DIADIEM`

An Oracle-managed OLS label column may be added when implementing OLS.

## Allowed Extensions

These are acceptable if documented clearly:

- `USERNAME` in `BENHNHAN`
- `USERNAME` in `NHANVIEN`
- helper views for account lookup
- optional reference tables for departments or locations

Do not let helper tables replace the required assignment relations.

## Current Checked-In Files

The repo already contains these schema files:

- `database/Subsystem2-MedicalDB/schema/01_CreateTables.sql`
- `database/Subsystem2-MedicalDB/schema/02_CreateIndexes.sql`
- `database/Subsystem2-MedicalDB/schema/03_InsertSampleData.sql`

This task should keep those files aligned with the assignment.

## Data Volume Targets from the Assignment

- approximately `100000` patient users
- `20` coordinators
- `100` doctors
- `50` technicians

Other row counts may be chosen by the team, but they should support the required security and audit demonstrations.

## Acceptance Criteria

- The required assignment relations are present and recognizable.
- Column names stay aligned with the assignment text.
- `HSBA_DV` and `DONTHUOC` do not drift into unrelated surrogate-key-only designs.
- Any `USERNAME` mapping is documented as an extension, not as a replacement for Oracle user management.
- Sample data supports all role scenarios required by TC#1 through TC#5.
