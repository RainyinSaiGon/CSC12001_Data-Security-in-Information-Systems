# Task 07: Subsystem 2 Database Schema Setup - Tables, Indexes, Sample Data

**Assigned to:** Ngọc, Vũ (Part B)  
**Type:** Database Administration  
**Duration:** 8 hours  
**Priority:** Critical (blocks all other work)  
**Timeline:** Feb 10 - Feb 14, 2026 (MUST COMPLETE BY FRI 2/14)

---

## Overview

Create Subsystem 2 medical database schema foundation with 7 tables, performance indexes, and representative sample data. This task blocks all Subsystem 2 work - completion by Friday Feb 14 is non-negotiable.

## Deliverables

### 01_CreateTables.sql

Create 7 core tables with proper constraints:

| Table | Purpose | Key Columns |
|-------|---------|-------------|
| KHOA | Departments | MAKHOA (PK), TENKHOA, SDT, TRUONGKHOA (FK) |
| BENHNHAN | Patients | MABENHNHAN (PK), HOTEN, PHAI, NGAYSINH, CCCD, DIENTHOAI, SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC, USERNAME |
| NHANVIEN | Staff | MANV (PK), HOTEN, PHAI, NGAYSINH, CMND, QUEQUAN, SODT, VAITRO, MAKHOA (FK), USERNAME |
| HSBA | Medical Records | MAHSBA (PK), MABENHNHAN (FK), NGAYTAO, CHANDOAN, DIEUTRI, KETLUAN, MABACSI (FK), MAKHOA (FK) |
| HSBA_DV | Diagnostic Services | MADICHVU (PK), MAHSBA (FK), TENDICHVU, NGAY, KETQUA, HOANTHANH, MAKYTHUATVIEN (FK) |
| DONTHUOC | Prescriptions | MADONTHUOC (PK), MAHSBA (FK), TENTHUOC, LIEUDUNG, HUONGDAN, NGAYDANGKY |
| THONGBAO | Notifications (OLS) | MATHONGBAO (PK), NOIDUNG, NGAYGIO, DIADIEM |
| AUDITLOG | Audit Trail | AUDITID (PK), USERID, THOIGIAN, LOAIHD, TENTABLE, MARECORD |

Requirements:

- Use VARCHAR2, DATE, TIMESTAMP data types
- Primary keys on all tables (including composite keys where appropriate)
- Foreign key constraints with proper relationships
- NOT NULL constraints on required fields
- Unique constraints (CCCD in BENHNHAN, CMND in NHANVIEN, etc.)
- VAITRO constraint: Check values are one of: 'Điều phối viên', 'Bác sĩ/Y sĩ', 'Kỹ thuật viên', 'Bệnh nhân'
- Departments (KHOA): Only 3 departments: 'Khoa Tiêu Hóa', 'Khoa Thần Kinh', 'Khoa Tim Mạch'
- Locations: 3 facilities: 'Cơ sở Hồ Chí Minh', 'Cơ sở Hải Phòng', 'Cơ sở Hà Nội'
- Allow all INSERT/UPDATE/DELETE for initial testing

### 02_CreateIndexes.sql

Create 10+ indexes for query performance:

| Table | Columns | Purpose |
|-------|---------|---------|
| BENHNHAN | MABN | Patient lookup |
| BENHNHAN | CCCD | ID-based search |
| NHANVIEN | MANV | Staff lookup |
| HSBA | MABN | Patient records query |
| HSBA | MAHSBA | Record lookup |
| HSBA_DV | MAHSBA | Service lookup |
| DONTHUOC | MAHSBA | Prescription query |
| AUDITLOG | USERID | Audit by user |
| AUDITLOG | THOIGIAN | Audit by date |
| AUDITLOG | USERID, THOIGIAN | Combined query |
| THONGBAO | (KHOA, DIADIEM, CAPBAC) | OLS filtering |

### 03_InsertSampleData.sql

Create realistic test data (scaled for production):

- **100,000 patients** (BENHNHAN) — **Production-scale dataset as per TC#5**
  - Vietnamese names, realistic IDs
  - Birth dates creating age 18-80 range
  - Full addresses: SONHA (house number), TENDUONG (street), QUANHUYEM (district), TINHTP (province)
  - TIENSUABENH (patient's medical fund)
  - TIENSUABENHGD (family's medical fund)
  - DIUNGTHUOC (drug allergies/intolerances)
  - Performance consideration: May require script optimization or batched inserts

- **170 staff** (NHANVIEN)
  - 20 Coordinators (VAITRO='Điều phối viên')
  - 100 Doctors/Nurses (VAITRO='Bác sĩ/Y sĩ')
  - 50 Technicians (VAITRO='Kỹ thuật viên')
  - Full personal info: HOTEN (full name), PHAI (gender), NGAYSINH (birth date)
  - CMND (national ID), QUEQUAN (hometown), SODT (phone number)
  - CHUYENKHOA (specialty/department)

- **50,000+ medical records** (HSBA) — **Proportional to 100K patients (avg 0.5 records/patient)**
  - MAHSBA: Record ID, MABN: Patient reference, NGAY: Examination date
  - CHANDOAN: Diagnosis, DIEUTRI: Treatment provided
  - MABS: Doctor ID (prescribing physician), MAKHOA: Department ID
  - KETLUAN: Conclusion/remarks
  - Mix of completed and archived records
  - Realistic distribution: Active patients have 1-3 records, inactive have 0-1

- **100,000+ prescriptions** (DONTHUOC) — **Proportional to HSBA (avg 2 prescriptions/record)**
  - MAHSBA: Link to medical records (FK)
  - NGAYDT: Prescription date
  - TENTHUOC: Drug/medication name
  - LIEUUNG: Dosage instructions
  - Composite key: (MAHSBA, NGAYDT, TENTHUOC) allows multiple drugs per prescription date
  - Average 2+ drugs per prescription date

- **75,000+ diagnostic services** (HSBA_DV) — **Proportional to HSBA (avg 1.5 services/record)**
  - MAHSBA: Link to medical records
  - LOAIDV: Service type (X-ray, Lab test, Ultrasound, CT scan, ECG, etc.)
  - NGAYDV: Service date/time
  - MAKTV: Technician ID (KTV - Kỹ Thuật Viên performing the service)
  - KETQUA: Test results (some completed, some pending)

- **10,000+ notifications** (THONGBAO) — **Proportional to HSBA (avg 0.2 notifications/record)**
  - Varied departments and locations
  - OLS labels for testing
  - Realistic content

## Dependencies

- **Requires:** Oracle 21c (or compatible) installation
- **Unblocks:** All other Subsystem 2 tasks (Task 04-06, Task 08-10)

## Success Criteria

✓ All 7 tables created with correct Vietnamese column names  
✓ BENHNHAN: PK=MABN, includes all address fields (SONHA, TENDUONG, QUANHUYEM, TINHTP)
✓ NHANVIEN: PK=MANV, VAITRO constraint enforces 4 valid roles
✓ HSBA: PK=MAHSBA, FK constraints on MABN, MABS, MAKHOA
✓ HSBA_DV: Composite PK=(MAHSBA, LOAIDV, NGAYDV), FK on MAHSBA, MAKTV  
✓ DONTHUOC: Composite PK=(MAHSBA, NGAYDT, TENTHUOC), FK on MAHSBA
✓ THONGBAO & AUDITLOG: Created with correct structures for OLS and audit
✓ All constraints enforced (PK, FK, NOT NULL, UNIQUE)  
✓ All 10+ indexes created  

- [x] 100,000 patients inserted (BENHNHAN) — production-scale per TC#5
- [x] 170 staff (20 coordinators, 100 doctors, 50 technicians) inserted (NHANVIEN)
✓ Data matches Vietnamese specification exactly  
✓ No orphaned records (FK integrity)  
✓ Tables ready for application use

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

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `01_CreateTables.sql` — 7 core tables | Critical | Mon-Wed, Feb 10-12 |
| `02_CreateIndexes.sql` — Performance indexes | Required | Thu, Feb 13 |
| `03_InsertSampleData.sql` — Realistic test data | Required | Fri, Feb 14 |

**Pass Criteria:**

- ✓ All 7 tables created with zero SQL errors
- ✓ All PK/FK constraints properly defined
- ✓ 100,000 patients inserted with realistic Vietnamese names (production-scale per TC#5)
- ✓ 8 test users (Staff: 10xxxxxx, Patients: 20xxxxxx) created for TC verification
- ✓ 170 staff records in NHANVIEN table (20 Coordinators, 100 Doctors, 50 Technicians)
- ✓ 50,000+ medical records (HSBA) with proper relationships — proportional to patient volume
- ✓ 100,000+ prescriptions linked to valid HSBA records — avg 2+ per record
- ✓ 75,000+ diagnostic services with status tracking — avg 1.5 per record
- ✓ 10,000+ notifications with OLS labels — realistic event distribution
- ✓ All data verifiable with SELECT COUNT(*) queries showing correct proportions

**Evidence Tracking:**

- Database script execution log (sqlplus output)
- SELECT COUNT(*) query results for each table
- DESCRIBE output for each table showing constraints

---

### Phase 1 Gate: Foundation Complete

> [!IMPORTANT]
> All Phase 2 work (Tasks 02-05, 07-08) is **BLOCKED** until this task is complete.
> Deadline: **Friday, February 14 EOD** (non-negotiable).

---

## Related Tasks

- Task 08: Database security setup (needs these tables)
- Task 09: Audit setup (needs AuditLog table)
- Task 10: Backup/recovery (needs full schema)
- All other tasks: Depend on this schema

---

**CRITICAL: Failure to complete by Friday 2/14 delays entire project**
