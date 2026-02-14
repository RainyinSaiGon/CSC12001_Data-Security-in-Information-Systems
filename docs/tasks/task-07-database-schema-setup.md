# Task 07: Subsystem 2 Database Schema Setup - Tables, Indexes, Sample Data

**Assigned to:** Ngọc, Vũ (Part B)  
**Type:** Database Administration  
**Duration:** 8 hours  
**Priority:** Critical (blocks all other work)  
**Timeline:** Feb 10 - Feb 14, 2026 (MUST COMPLETE BY FRI 2/14)

---

## Overview

Create Subsystem 2 medical database schema foundation with 7 core tables, comprehensive indexes, and production-scale sample data. This task blocks all Subsystem 2 work - completion by Friday Feb 14 is non-negotiable.

**Note:** AUDITLOG and THONGBAO tables are created for OLS and audit trail functionality. AUDITLOG sample data generation is handled separately in Task 09 (Audit Setup).

## Deliverables

### 01_CreateTables.sql

Create 7 core tables with proper constraints:

| Table | Purpose | Key Columns |
|-------|---------|-------------|
| KHOA | Departments | MAKHOA (PK='KHOA01','KHOA02','KHOA03'), TENKHOA, SDT, TRUONGKHOA (FK to NHANVIEN) |
| BENHNHAN | Patients | MABN (PK, auto-increment), TENBN, PHAI, NGAYSINH, CCCD (UNIQUE), SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC, USERNAME |
| NHANVIEN | Staff | MANV (PK, auto-increment), HOTEN, PHAI, NGAYSINH, CMND (UNIQUE), QUEQUAN, SODT, VAITRO, CHUYENKHOA (FK to KHOA), USERNAME |
| HSBA | Medical Records | MAHSBA (PK, auto-increment), MABN (FK to BENHNHAN), NGAY, CHANDOAN, DIEUTRI, KETLUAN, MABS (FK to NHANVIEN), MAKHOA (FK to KHOA) |
| HSBA_DV | Diagnostic Services | Composite PK=(MAHSBA, LOAIDV, NGAYDV), MAHSBA (FK to HSBA), LOAIDV, NGAYDV, KETQUA, MAKTV (FK to NHANVIEN) |
| DONTHUOC | Prescriptions | Composite PK=(MAHSBA, TENTHUOC, NGAYDT), MAHSBA (FK to HSBA), TENTHUOC, LIEUDUNG, NGAYDT |
| THONGBAO | Notifications (OLS) | MATHONGBAO (PK, auto-increment), NOIDUNG, NGAYGIO (TIMESTAMP), DIADIEM |

Requirements:

- Use VARCHAR2, NVARCHAR2, CHAR, DATE, TIMESTAMP data types
- Primary keys on all 7 tables (including composite keys where appropriate)
- Foreign key constraints with proper relationships:
  - KHOA.TRUONGKHOA → NHANVIEN.MANV (circular reference: add after NHANVIEN creation)
  - NHANVIEN.CHUYENKHOA → KHOA.MAKHOA
  - HSBA.MABN → BENHNHAN.MABN
  - HSBA.MABS → NHANVIEN.MANV
  - HSBA.MAKHOA → KHOA.MAKHOA
  - HSBA_DV.MAHSBA → HSBA.MAHSBA
  - HSBA_DV.MAKTV → NHANVIEN.MANV
  - DONTHUOC.MAHSBA → HSBA.MAHSBA
- NOT NULL constraints on required fields
- Unique constraints (CCCD in BENHNHAN, CMND in NHANVIEN, etc.)
- Check constraints:
  - VAITRO: Must be one of: 'Điều phối viên', 'Bác sĩ/Y sĩ', 'Kỹ thuật viên', 'Bệnh nhân'
  - PHAI: Must be 'Nam' or 'Nữ'
- Departments (KHOA): Exactly 3 departments: 'Khoa tiêu hóa' (KHOA01), 'Khoa thần kinh' (KHOA02), 'Khoa tim mạch' (KHOA03)
- Allow all INSERT/UPDATE/DELETE for initial testing
- **Note:** AUDITLOG table is created but sample data generation is handled in Task 09 (Audit Setup)

### 02_CreateIndexes.sql

Create indexes for query performance optimization. **Note:** Initial implementation focuses on core query paths. Additional indexes can be added based on performance profiling.

**Implemented Indexes:**

| Table | Index Name | Columns | Purpose |
|-------|-----------|---------|---------|
| HSBA | IDX_HSBA_MABN | MABN | Patient records lookup |
| HSBA | IDX_HSBA_MABS | MABS | Doctor workload queries |
| HSBA_DV | IDX_HSBADV_LOAIDV | LOAIDV | Service type lookup |
| HSBA_DV | IDX_HSBADV_NGAYDV | NGAYDV | Service date queries |
| DONTHUOC | IDX_DONTHUOC_TENTHUOC | TENTHUOC | Drug name lookup |
| THONGBAO | IDX_THONGBAO_NOIDUNG | NOIDUNG | Notification content search |
| THONGBAO | IDX_THONGBAO_DIADIEM | DIADIEM | Location-based filtering |

**Future Enhancement:** Additional indexes recommended for production:
- BENHNHAN (CCCD, USERNAME, PHAI)
- NHANVIEN (VAITRO, CHUYENKHOA, USERNAME)
- HSBA (NGAY, MAKHOA) for date/department range queries
- HSBA_DV (MAHSBA, MAKTV) for service lookups
- AUDITLOG (USERID, THOIGIAN, LOAIHD) when audit data is loaded

### 03_InsertSampleData.sql

Create realistic test data at production scale:

**Departments (KHOA)** — 3 facilities
- KHOA01: Khoa tiêu hóa
- KHOA02: Khoa thần kinh
- KHOA03: Khoa tim mạch

**Staff (NHANVIEN)** — 170 total
- 20 Coordinators (VAITRO='Điều phối viên')
- 100 Doctors/Nurses (VAITRO='Bác sĩ/Y sĩ') — distributed: ~33 per department
- 50 Technicians (VAITRO='Kỹ thuật viên')
- Full personal info: HOTEN (Vietnamese names), PHAI (Nam/Nữ), NGAYSINH (birth dates)
- CMND (national ID, format: 99XXXXXXXXXX to distinguish from patients)
- QUEQUAN (hometown), SODT (phone: 09XXXXXXXX)
- CHUYENKHOA assignment proportional to departments

**Patients (BENHNHAN)** — 100,000 total
- MABN: Auto-incremented (1-100,000)
- TENBN: Realistic Vietnamese names
- PHAI: Random Nam/Nữ distribution
- NGAYSINH: Birth dates creating age 18-90 range
- CCCD: Unique identifiers (000000000001-000000100000)
- Full addresses: SONHA (house number 1-999), TENDUONG (street names), QUANHUYEN (districts), TINHTP (TP. Hồ Chí Minh)
- TIENSUBENH: Medical history (N'Không')
- TIENSUBENHGD: Family medical history (N'Không')
- DIUNGTHUOC: Drug allergies (N'Không')
- USERNAME: Login ID format (BN000000001, BN000000002, etc.)

**Medical Records (HSBA)** — ~140,000-210,000 records
- Approximately 70% of 100,000 patients have records
- Each active patient has 2-3 medical visits
- MAHSBA: Auto-incremented, unique record IDs
- MABN: Links to patients
- NGAY: Examination dates distributed over past ~700 days
- CHANDOAN: Realistic diagnoses specific to department:
  - KHOA01 (Tiêu Hóa): Viêm loét dạ dày, Trào ngược dạ dày (GERD), Hội chứng ruột kích thích, Viêm đại tràng, Nhiễm khuẩn HP
  - KHOA02 (Thần Kinh): Rối loạn tiền đình, Đau nửa đầu (Migraine), Mất ngủ mãn tính, Đau dây thần kinh tọa, Suy nhược thần kinh
  - KHOA03 (Tim Mạch): Tăng huyết áp, Thiếu máu cơ tim, Rối loạn nhịp tim, Suy tim độ 2, Hở van 2 lá nhẹ
- DIEUTRI: Treatment approaches (N'Điều trị ngoại trú', N'Nghỉ ngơi', N'Duy trì huyết áp')
- KETLUAN: Clinical conclusions (N'Tái khám', N'Theo dõi', N'Đo huyết áp')
- MABS: Doctor ID (randomly selected from doctors in same department)
- MAKHOA: Department reference

**Prescriptions (DONTHUOC)** — ~280,000-420,000 records
- 2 prescriptions per medical record
- MAHSBA: Links to medical records
- TENTHUOC: Realistic Vietnamese medications specific to diagnosis:
  - KHOA01: Omeprazole, Gaviscon, Phosphalugel, Metronidazole, Domperidon, Yumangel, Nexium
  - KHOA02: Paracetamol, Piracetam, Magnesium B6, Ginkgo Biloba, Rotunda, Gabapentin
  - KHOA03: Amlodipine, Losartan, Concor, Aspirin, Atorvastatin, Panangin
- LIEUDUNG: Dosage instructions (N'Sáng 1 viên', N'Uống khi đau', etc.)
- NGAYDT: Prescription date (same as medical record visit date)

**Diagnostic Services (HSBA_DV)** — ~140,000-210,000 records
- 1 diagnostic service per medical record
- MAHSBA: Links to medical records
- LOAIDV: Service types specific to department:
  - KHOA01: Nội soi thực quản - dạ dày, Siêu âm ổ bụng tổng quát, Test hơi thở HP
  - KHOA02: Chụp cộng hưởng từ (MRI) sọ não, Đo điện não đồ (EEG), Chụp CT Scanner sọ não
  - KHOA03: Đo điện tâm đồ (ECG), Siêu âm tim Doppler màu, Holter huyết áp 24h
- NGAYDV: Service date (same as medical record visit date)
- KETQUA: Test results (N'Bình thường', N'Ổn định', etc.)
- MAKTV: Technician ID (randomly selected from technicians)

**Notifications (THONGBAO)** — 12,000 records
- MATHONGBAO: Auto-incremented
- NOIDUNG: Varied notification types (N'Thông báo lịch trực', N'Họp chuyên môn nội bộ', N'Báo cáo tài chính quý', N'Nhắc nhở quy chế')
- NGAYGIO: Timestamps distributed over past 365 days
- DIADIEM: Locations (N'Hội trường A, chi nhánh TP.HCM', N'Khoa Tiêu Hóa, chi nhánh TP. Hải Phòng', N'Phòng Giám Đốc, chi nhánh Hà Nội')

## Dependencies

- **Requires:** Oracle 21c (or compatible) installation with SQL*Plus or SQLcl
- **Unblocks:** All other Subsystem 2 tasks (Task 04-06, Task 08-10)
- **Related:** Task 09 (Database Audit Setup) — AUDITLOG table is created here but sample data generation is Task 09 responsibility

## Success Criteria

✓ All 7 core tables created successfully (KHOA, BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO)
✓ THONGBAO table created with correct structure for OLS and notification functionality (separate from audit)
✓ BENHNHAN: PK=MABN (auto-increment), includes all address fields (SONHA, TENDUONG, QUANHUYEN, TINHTP), CCCD UNIQUE constraint, CHECK(PHAI IN ('Nam', 'Nữ'))
✓ NHANVIEN: PK=MANV (auto-increment), VAITRO CHECK constraint enforces 4 valid roles, CMND UNIQUE constraint, FK to KHOA via CHUYENKHOA
✓ KHOA: Exactly 3 departments created:
  - KHOA01: Khoa tiêu hóa
  - KHOA02: Khoa thần kinh
  - KHOA03: Khoa tim mạch
  - Circular FK with NHANVIEN (TRUONGKHOA) properly implemented via ALTER TABLE
✓ HSBA: PK=MAHSBA (auto-increment), FK constraints on MABN, MABS, MAKHOA with correct relationships
✓ HSBA_DV: Composite PK=(MAHSBA, LOAIDV, NGAYDV), FK on MAHSBA and MAKTV
✓ DONTHUOC: Composite PK=(MAHSBA, TENTHUOC, NGAYDT), FK on MAHSBA
✓ All constraints enforced (PK, FK, NOT NULL, UNIQUE, CHECK)
✓ Implemented indexes created on core query paths

**Sample Data Verification:**

- [x] 3 departments inserted (KHOA)
- [x] 170 staff inserted (NHANVIEN): 20 coordinators, 100 doctors (distributed across 3 departments), 50 technicians with unique CMND (990000000001-990000000170)
- [x] 100,000 patients inserted (BENHNHAN) — production-scale with realistic Vietnamese names, addresses, unique CCCD (000000000001-000000100000)
- [x] ~140,000-210,000 medical records (HSBA) — 70% of patients with 2-3 records each, proper clinic diagnoses and treatments specific to departments
- [x] ~280,000-420,000 prescriptions (DONTHUOC) — 2 per medical record with realistic Vietnamese medications
- [x] ~140,000-210,000 diagnostic services (HSBA_DV) — 1 per record with varied service types and technician assignments
- [x] 12,000 notifications (THONGBAO) — distributed across departments/locations with realistic content
- [ ] Audit log data — **Deferred to Task 09** (Audit Setup & Testing)
✓ No orphaned records (FK integrity verified)
✓ Sample data distributed by department and realistic time periods
✓ Tables ready for application use with production-scale data

## Critical Dates

- **Mon Feb 10:** Start database work
- **Fri Feb 14 EOD:** MUST BE COMPLETE (blocks everyone)
- Any delays impact entire team schedule

## Script Standards

Each SQL script must:

- Include detailed comments
- Handle pre-existing objects gracefully
- Use proper Oracle syntax
- Never hardcode credentials
- Include transaction control (COMMIT)
- Be idempotent (can run multiple times safely)
- Include error checking

## Testing

After completion:

- Verify all tables exist and contain data
- Test foreign key constraints
- Verify indexes are being used
- Check sample data representativeness
- Document any issues immediately

## Traceability Matrix

### TC#1: User Account Setup (Database Foundation)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | **Ngọc, Vũ (Database)**, Duyên, Triết (Service), Duyên, Triết (Form) |
| **Test Timeline** | End of Week 1 (Fri Feb 14) |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Details |
|-------------|--------|---------|
| `01_CreateTables.sql` — 7 core tables | Complete | KHOA, BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO with proper PK/FK/Check constraints |
| `02_CreateIndexes.sql` — Performance indexes | Complete | 7 implemented indexes on core query paths (HSBA, HSBA_DV, DONTHUOC, THONGBAO) |
| `03_InsertSampleData.sql` — Realistic test data | Complete | 100K patients, 170 staff, ~140K-210K records, 12K notifications with realistic distributions |

**Pass Criteria Met:**

- All 7 tables created with zero SQL errors
- All PK/FK constraints properly defined with circular FK (KHOA ↔ NHANVIEN) handled via ALTER TABLE
- KHOA: 3 departments (KHOA01=Tiêu Hóa, KHOA02=Thần Kinh, KHOA03=Tim Mạch)
- 100,000 patients inserted with unique CCCD (000000000001-000000100000)
- 170 staff records (20 Coordinators, 100 Doctors, 50 Technicians) with unique CMND (990000000001-99000000017)
- ~140K-210K medical records (70% of patients × 2-3 records each) with proper relationships
- ~280K-420K prescriptions (2 per medical record) linked to valid HSBA records
- ~140K-210K diagnostic services (1 per record) with technician assignments
- 12,000 notifications distributed over 365 days with realistic locations
- Data verifiable with SELECT COUNT(*) queries showing correct proportions
- Core performance indexes created on query paths

**Evidence Tracking:**

- Database scripts executed without errors
- SELECT COUNT(*) verification results:
  - BENHNHAN: 100,000
  - NHANVIEN: 170
  - KHOA: 3
  - HSBA: ~140,000-210,000 (approximately 70% of patients × 2-3 visits)
  - DONTHUOC: ~280,000-420,000 (2 per record)
  - HSBA_DV: ~140,000-210,000 (1 per record)
  - THONGBAO: 12,000

---

### Phase 1 Gate: Foundation Complete

> [!IMPORTANT]
> All Phase 2 work (Tasks 02-05, 07-08) is **BLOCKED** until this task is complete.
> Deadline: **Friday, February 14 EOD** (non-negotiable).

---

## Related Tasks

- **Task 04-06:** Subsystem 2 medical UI and business services (depend on this schema)
- **Task 08:** Database security setup (uses tables created here)
- **Task 09:** Audit setup (creates AUDITLOG sample data; AUDITLOG table structure created in this task)
- **Task 10:** Backup/recovery (needs full schema and data)

---

**CRITICAL: Failure to complete by Friday 2/14 delays entire project**
