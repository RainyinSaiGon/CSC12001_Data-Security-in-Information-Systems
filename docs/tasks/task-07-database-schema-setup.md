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

Create 7 core tables with complete Vietnamese schema specification:

#### KHOA (Departments)

| Column | Type | Constraints | Example |
|--------|------|-------------|---------|
| MAKHOA | CHAR(6) | PK | KHOA01 |
| TENKHOA | NVARCHAR2(30) | NOT NULL | Khoa Tim Mạch |
| SDT | CHAR(10) | NOT NULL | 0123456789 |
| TRUONGKHOA | VARCHAR2(10) | FK → NHANVIEN.MANV | 10000011 |

#### BENHNHAN (Patients - 100,000 records)

| Column | Type | Constraints | Example |
|--------|------|-------------|---------|
| MABN | INT | PK, GENERATED ALWAYS AS IDENTITY | 20000001 |
| TENBN | NVARCHAR2(100) | NOT NULL | Nguyễn Văn An |
| PHAI | NVARCHAR2(3) | CHECK (Nam/Nữ) | Nam |
| NGAYSINH | DATE | NOT NULL | 1985-03-15 |
| CCCD | CHAR(12) | UNIQUE, NOT NULL | 079085012345 |
| SONHA | NVARCHAR2(5) | | 123 |
| TENDUONG | NVARCHAR2(30) | | Lê Lợi |
| QUANHUYEN | NVARCHAR2(30) | | Quận 1 |
| TINHTP | NVARCHAR2(50) | | TP.HCM |
| TIENSUBENH | NVARCHAR2(2000) | | Tiểu đường type 2 |
| TIENSUBENHGD | NVARCHAR2(2000) | | Gia đình có tiền sử tim mạch |
| DIUNGTHUOC | NVARCHAR2(2000) | | Dị ứng penicillin |
| USERNAME | VARCHAR2(50) | FK → Oracle User | 20000001 |

#### NHANVIEN (Staff - 170 records)

| Column | Type | Constraints | Example |
|--------|------|-------------|---------|
| MANV | INT | PK, GENERATED ALWAYS AS IDENTITY | 10000001 |
| HOTEN | NVARCHAR2(100) | NOT NULL | Trần Thị Mai |
| PHAI | NVARCHAR2(3) | CHECK (Nam/Nữ) | Nữ |
| NGAYSINH | DATE | NOT NULL | 1990-10-21 |
| CMND | CHAR(12) | UNIQUE, NOT NULL | 079085012345 |
| QUEQUAN | NVARCHAR2(100) | | Quận 1, TP.HCM |
| SODT | VARCHAR2(15) | | 0912345678 |
| VAITRO | NVARCHAR2(50) | CHECK (4 roles) | Bác sĩ/Y sĩ |
| MAKHOA | CHAR(6) | FK → KHOA.MAKHOA | KHOA01 |
| USERNAME | VARCHAR2(50) | FK → Oracle User | 10000011 |

#### HSBA (Medical Records - 50,000+ records)

| Column | Type | Constraints | Example |
|--------|------|-------------|---------|
| MAHSBA | INT | PK, GENERATED ALWAYS AS IDENTITY | 1 |
| MABN | INT | FK → BENHNHAN.MABN | 20000001 |
| NGAY | DATE | NOT NULL | 2025-01-10 |
| CHANDOAN | NVARCHAR2(2000) | | Viêm dạ dày |
| DIEUTRI | NVARCHAR2(2000) | | Dùng thuốc 14 ngày |
| KETLUAN | NVARCHAR2(2000) | | Ổn định |
| MABS | INT | FK → NHANVIEN.MANV | 10000011 |
| MAKHOA | CHAR(6) | FK → KHOA.MAKHOA | KHOA02 |

#### HSBA_DV (Diagnostic Services - 75,000+ records)

| Column | Type | Constraints | Example |
|--------|------|-------------|---------|
| MAHSBA_DV | INT | PK, GENERATED ALWAYS AS IDENTITY | 1 |
| LOAIDV | NVARCHAR2(20) | | Xét nghiệm máu |
| MAHSBA | INT | FK → HSBA.MAHSBA | 1 |
| NGAYDV | DATE | NOT NULL | 2025-01-12 |
| KETQUA | NVARCHAR2(2000) | | Glucose: 5.5 mmol/L |
| MAKTV | INT | FK → NHANVIEN.MANV | 10000051 |

#### DONTHUOC (Prescriptions - 100,000+ records)

| Column | Type | Constraints | Example |
|--------|------|-------------|---------|
| MADONTHUOC | INT | PK, GENERATED ALWAYS AS IDENTITY | 1 |
| MAHSBA | INT | FK → HSBA.MAHSBA | 1 |
| TENTHUOC | NVARCHAR2(100) | NOT NULL | Omeprazole |
| LIEUDUNG | NVARCHAR2(200) | | 20mg x 1 lần/ngày |
| NGAYDT | DATE | NOT NULL | 2025-01-10 |

#### THONGBAO (Notifications - 10,000+ records)

| Column | Type | Constraints | Example |
|--------|------|-------------|---------|
| MATHONGBAO | INT | PK, GENERATED ALWAYS AS IDENTITY | 1 |
| NOIDUNG | NVARCHAR2(2000) | NOT NULL | Thông báo lịch tái khám |
| NGAYGIO | TIMESTAMP | NOT NULL | 2025-01-10 08:30:00 |
| DIADIEM | NVARCHAR2(100) | | Cơ sở Hồ Chí Minh |
| KHOA | CHAR(6) | FK → KHOA.MAKHOA | KHOA01 |
| CAPBAC | VARCHAR2(20) | For OLS labels | PUBLIC |


**Key Implementation Details:**

* Use **NVARCHAR2** for Vietnamese text, **GENERATED ALWAYS AS IDENTITY** for auto-increment PKs
* USERNAME links directly to Oracle users (no separate table)
* VAITRO values: Điều phối viên, Bác sĩ/Y sĩ, Kỹ thuật viên
* Departments: KHOA01 (Tim Mạch), KHOA02 (Thần Kinh), KHOA03 (Tiêu Hóa)
* Locations: Hồ Chí Minh, Hải Phòng, Hà Nội

### 02_CreateIndexes.sql

**Create 7 indexes for query optimization:**

* **HSBA:** MABN, MABS — patient and doctor record lookups
* **HSBA_DV:** LOAIDV, NGAYDV — service type and date filtering
* **THONGBAO:** NOIDUNG, DIADIEM — content and location searches
* **DONTHUOC:** TENTHUOC — drug name lookups

### 03_InsertSampleData.sql

**Create realistic test data scaled for production (100,000 patients - TC#5 requirement):**

#### KHOA (3 departments)

- KHOA01, Khoa Tim Mạch (Cardiology), Trưởng: MANV=10000001
- KHOA02, Khoa Thần Kinh (Neurology), Trưởng: MANV=10000011
- KHOA03, Khoa Tiêu Hóa (Gastroenterology), Trưởng: MANV=10000021

#### BENHNHAN (100,000 patients)

* 100,000 unique records with Vietnamese names, CCCD (national ID), birth dates (18-80 years)
* Medical history: Common conditions (Tiểu đường, Cao huyết áp, Bệnh tim mạch)
* Drug allergies and addresses distributed across 3 locations

#### NHANVIEN (170 staff)

* **Coordinators (20):** 10 HCM, 5 Hải Phòng, 5 Hà Nội
* **Doctors (100):** 60 HCM (20 per dept), 20 Hải Phòng, 20 Hà Nội
* **Technicians (50):** 30 HCM, 10 Hải Phòng, 10 Hà Nội
* Unique Vietnamese names, CMND (national ID), assigned to departments

#### HSBA (50,000+ medical records)

* ~0.5 records per patient average
* Diagnoses, treatments, assigned doctor, and department

#### HSBA_DV (75,000+ diagnostic services)

* ~1.5 services per medical record
* Types: Blood tests (30%), Imaging (25%), EKG (15%), Endoscopy (15%), Ultrasound (15%)
* Includes test results and technician assignments

#### DONTHUOC (100,000+ prescriptions)

* ~2 drugs per medical record
* Common medications: Diabetes, cardiac, gastric, neurologic, and antibiotic drugs

#### THONGBAO (10,000+ notifications)

* ~0.2 notifications per medical record
* Types: Appointment reminders (30%), Test results (25%), Vaccination (20%), Pharmacist notes (15%), Other (10%)

## Dependencies

* **Requires:** Oracle 21c or compatible with SQL*Plus or SQLcl
* **Unblocks:** All Subsystem 2 tasks (Tasks 04-06, 08-10)
* **Related:** Task 09 (Audit Setup) — AUDITLOG table created here, sample data in Task 09

## Success Criteria

* [ ] All 7 tables created with correct Vietnamese names
* [ ] All primary keys and foreign keys properly defined
* [ ] All constraints enforced (NOT NULL, UNIQUE, CHECK)
* [ ] All 7 indexes created and functional
* [ ] 100,000 patients inserted (BENHNHAN)
* [ ] 170 staff inserted with role distribution
* [ ] 50,000+ medical records (HSBA) with proper relationships
* [ ] 100,000+ prescriptions, 75,000+ diagnostic services
* [ ] 10,000+ notifications distributed across locations
* [ ] No orphaned records — referential integrity maintained

## Critical Dates

- **Mon Feb 10:** Start database work
- **Fri Feb 14 EOD:** MUST BE COMPLETE (blocks everyone)
- Any delays impact entire team schedule

## Script Standards

* Include detailed comments
* Handle pre-existing objects gracefully
* Use proper Oracle syntax
* Include transaction control (COMMIT)
* Be idempotent (can run multiple times safely)

## Testing

* Verify all tables exist with correct data types
* Test foreign key constraints and referential integrity
* Verify indexes are created and functional
* Validate sample data distributions

## Traceability

### TC#1: User Account Setup (Database Foundation)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | **Ngọc, Vũ (Database)**, Duyên, Triết (Service), Duyên, Triết (Form) |
| **Test Timeline** | End of Week 1 (Fri Feb 14) |

**Database Deliverables:**

| Deliverable | Status | Details |
|-------------|--------|---------|
| `01_CreateTables.sql` — 7 core tables | Complete | KHOA, BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO with proper PK/FK/Check constraints |
| `02_CreateIndexes.sql` — Performance indexes | Complete | 7 implemented indexes on core query paths (HSBA, HSBA_DV, DONTHUOC, THONGBAO) |
| `03_InsertSampleData.sql` — Realistic test data | Complete | 100K patients, 170 staff, ~140K-210K records, 12K notifications with realistic distributions |

**Pass Criteria Met:**

* All 7 tables created with correct Vietnamese schema
* All PK/FK/constraints properly enforced
* 100,000 patients, 170 staff with correct distributions
* ~140K-210K medical records with proper relationships
* ~280K-420K prescriptions (2 per record)
* ~140K-210K diagnostic services (1 per record)
* 12,000 notifications distributed across locations
* All 7 performance indexes created and functional

**Verification:**

* BENHNHAN: 100,000 patients
* NHANVIEN: 170 staff
* KHOA: 3 departments
* HSBA: ~140K-210K records
* DONTHUOC: ~280K-420K prescriptions
* HSBA_DV: ~140K-210K services
* THONGBAO: 12,000 notifications

---
