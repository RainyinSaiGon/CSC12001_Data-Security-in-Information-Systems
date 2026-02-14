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

- Use **NVARCHAR2** for all Vietnamese text (patient names, diagnoses, etc.)
- Use **GENERATED ALWAYS AS IDENTITY** for auto-increment numeric PKs
- Foreign key constraints: KHOA → NHANVIEN → HSBA chain
- USERNAME column links to Oracle database users (no separate user table)
- VAITRO values: 'Điều phối viên', 'Bác sĩ/Y sĩ', 'Kỹ thuật viên'
- Departments: KHOA01 (Tim Mạch), KHOA02 (Thần Kinh), KHOA03 (Tiêu Hóa)
- Locations: 'Cơ sở Hồ Chí Minh', 'Cơ sở Hải Phòng', 'Cơ sở Hà Nội'
- Allow all INSERT/UPDATE/DELETE operations for testing phase

### 02_CreateIndexes.sql

**Create table-specific indexes for query optimization (7 indexes):**

**Index Summary:**
- **HSBA**: 2 indexes (MABN, MABS) for patient and doctor record lookups
- **HSBA_DV**: 2 indexes (LOAIDV, NGAYDV) for service type and date filtering
- **THONGBAO**: 2 indexes (NOIDUNG, DIADIEM) for content and location searches
- **DONTHUOC**: 1 index (TENTHUOC) for drug name lookups

### 03_InsertSampleData.sql

**Create realistic test data scaled for production (100,000 patients - TC#5 requirement):**

#### KHOA (3 departments)

- KHOA01, Khoa Tim Mạch (Cardiology), Trưởng: MANV=10000001
- KHOA02, Khoa Thần Kinh (Neurology), Trưởng: MANV=10000011
- KHOA03, Khoa Tiêu Hóa (Gastroenterology), Trưởng: MANV=10000021

#### BENHNHAN (100,000 patients) — **PRODUCTION-SCALE per TC#5**

Sample records:
- MABN=20000001: Nguyễn Văn An, Nam, DOB 1985-03-15, CCCD=079085012345, 123 Lê Lợi, Quận 1, TP.HCM
- MABN=20000002: Trần Thị Hoa, Nữ, DOB 1992-07-22, CCCD=079092007222, 456 Nguyễn Hue, Quận 3, TP.HCM
- MABN=20000003: Lê Minh Phúc, Nam, DOB 1978-11-10, CCCD=079078101010, 789 Tôn Đức Thắng, Quận 7, TP.HCM
- Generated script should create 100,000 records with:
  - Diverse Vietnamese names
  - CCCD: 12-digit unique national ID numbers
  - Birth dates creating realistic age distribution (18-80 years old)
  - Medical history: Common conditions (Tiểu đường, Cao huyết áp, Bệnh tim mạch)
  - Drug allergies: Penicillin, Aspirin, various others
  - Distributed across 3 locations (TP.HCM, Hải Phòng, Hà Nội)

#### NHANVIEN (170 staff)

Distribution by role and location:

**Coordinators (20 total):** Điều phối viên
- 10 TP.HCM: MANV 10000001-10000010
- 5 Hải Phòng: MANV 10000062-10000066
- 5 Hà Nội: MANV 10000117-10000121

**Doctors/Nurses (100 total):** Bác sĩ/Y sĩ
- 60 TP.HCM: MANV 10000011-10000070 (distributed: 20 KHOA01, 20 KHOA02, 20 KHOA03)
- 20 Hải Phòng: MANV 10000067-10000086
- 20 Hà Nội: MANV 10000122-10000141

**Technicians (50 total):** Kỹ thuật viên
- 30 TP.HCM: MANV 10000087-10000116 (Xét nghiệm, Chẩn đoán hình ảnh, EKG)
- 10 Hải Phòng: MANV 10000142-10000151
- 10 Hà Nội: MANV 10000152-10000161

Sample staff records:
- MANV=10000001: Trần Thị Mai, Nữ, DOB 1990-10-21, CMND=079090102122, VAITRO='Điều phối viên', KHOA='KHOA01'
- MANV=10000011: Phạm Văn Hùng, Nam, DOB 1985-05-30, CMND=079085053030, VAITRO='Bác sĩ/Y sĩ', KHOA='KHOA02'
- MANV=10000051: Võ Thị Lan, Nữ, DOB 1988-12-15, CMND=079088121515, VAITRO='Kỹ thuật viên', KHOA='KHOA01'

#### HSBA (50,000+ medical records) — **0.5 records/patient avg**

Distribution across 100,000 patients:
- 50,000 patients with 1 active record
- 40,000 patients with 0 records (outpatient follow-up patients)
- 10,000 patients with 2-4 records (chronic conditions needing multiple visits)

Sample records:
- MAHSBA=1: MABN=20000001, NGAY=2025-01-10, CHANDOAN='Viêm dạ dày', DIEUTRI='Dùng thuốc 14 ngày + kiêng cơm nóng', MABS=10000011, KHOA='KHOA03'
- MAHSBA=2: MABN=20000002, NGAY=2025-01-12, CHANDOAN='Đau đầu migraine', DIEUTRI='Kê đơn giảm đau kết hợp chuyên khoa', MABS=10000021, KHOA='KHOA02'
- MAHSBA=3: MABN=20000003, NGAY=2025-01-08, CHANDOAN='Khám tái khám bệnh tim', DIEUTRI='Tiếp tục điều trị hiện tại', MABS=10000051, KHOA='KHOA01'

#### HSBA_DV (75,000+ diagnostic services) — **1.5 services/record avg**

Service distribution by type:
- Xét nghiệm máu: 30% (blood tests, hematology)
- Chẩn đoán hình ảnh: 25% (X-ray, ultrasound, CT scans)
- EKG: 15% (electrocardiograms)
- Nội soi: 15% (endoscopy/colonoscopy)
- Siêu âm: 15% (ultrasound specialized)

Sample records:
- MAHSBA_DV=1: LOAIDV='Xét nghiệm máu', MAHSBA=1, NGAYDV=2025-01-11, KETQUA='Hb: 13.5, WBC: 6.8, Glucose: 5.5 mmol/L', MAKTV=10000051
- MAHSBA_DV=2: LOAIDV='Chẩn đoán hình ảnh', MAHSBA=2, NGAYDV=2025-01-13, KETQUA='Đầu: Bình thường, Không tấn công', MAKTV=10000061
- MAHSBA_DV=3: LOAIDV='EKG', MAHSBA=3, NGAYDV=2025-01-10, KETQUA='Nhịp đều, Không có bất thường', MAKTV=10000071

#### DONTHUOC (100,000+ prescriptions) — **2+ drugs per record avg**

Sample prescriptions:

| MAHSBA | TENTHUOC | LIEUDUNG | NGAYDT |
|--------|----------|----------|--------|
| 1 | Omeprazole | 20mg x 1 lần/ngày, sau bữa ăn | 2025-01-10 |
| 1 | Domperidone | 10mg x 3 lần/ngày, trước bữa ăn | 2025-01-10 |
| 2 | Sumatriptan | 50mg x 1 lần khi đau | 2025-01-12 |
| 2 | Paracetamol | 500mg x 2 lần/ngày | 2025-01-12 |
| 3 | Atorvastatin | 20mg x 1 lần/tối | 2025-01-08 |
| 3 | Aspirin | 100mg x 1 lần/sáng | 2025-01-08 |

Common medications in dataset:
- Đường huyết: Metformin, Glibenclamide
- Tim mạch: Lisinopril, Atorvastatin, Aspirin
- Tiêu hóa: Omeprazole, Domperidone, Aluminium hydroxide
- Thần kinh: Paracetamol, Ibuprofen, Diazepam
- Kháng sinh: Amoxicillin, Azithromycin, Cephalexin

#### THONGBAO (10,000+ notifications) — **0.2 notifications/record avg**

Notification types:
- **Nhắc lịch tái khám** (30%): "Quý khách được mời tái khám vào [ngày]. Vui lòng liên hệ để xác nhận."
- **Kết quả xét nghiệm** (25%): "Kết quả xét nghiệm của quý khách đã sẵn sàng. Vui lòng liên hệ khoa để nhận kết quả."
- **Lịch tiêm chủng** (20%): "Quý khách hãy đến tiêm chủng vào [ngày]. Địa chỉ [cơ sở]."
- **Thông báo dược sĩ** (15%): "Cần kiểm tra k lại liều dùng thuốc với dược sĩ."
- **Khác** (10%): Various health reminders

Sample notifications:
- MATHONGBAO=1: NOIDUNG='Nhắc lịch tái khám bệnh tim vào 2025-02-15', NGAYGIO=2025-01-15 09:00, DIADIEM='Cơ sở Hồ Chí Minh', KHOA='KHOA01'
- MATHONGBAO=2: NOIDUNG='Kết quả xét nghiệm của quý khách đã sẵn sàng', NGAYGIO=2025-01-14 14:30, DIADIEM='Cơ sở Hải Phòng', KHOA='KHOA03'

## Dependencies

- **Requires:** Oracle 21c (or compatible) installation
- **Unblocks:** All other Subsystem 2 tasks (Task 04-06, Task 08-10)

## Success Criteria

[ ] All 7 tables created with correct Vietnamese column names (KHOA, BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO)
[ ] KHOA: PK=MAKHOA (CHAR 6), 3 departments created  
[ ] BENHNHAN: PK=MABN (GENERATED ALWAYS AS IDENTITY), includes all address fields (SONHA, TENDUONG, QUANHUYEN, TINHTP), CCCD unique constraint
[ ] NHANVIEN: PK=MANV (GENERATED ALWAYS AS IDENTITY), VAITRO constraint enforces 3 valid roles
[ ] HSBA: PK=MAHSBA (GENERATED ALWAYS AS IDENTITY), FK constraints on MABN, MABS, MAKHOA  
[ ] HSBA_DV: PK=MAHSBA_DV (GENERATED ALWAYS AS IDENTITY), FK constraints on MAHSBA, MAKTV
[ ] DONTHUOC: PK=MADONTHUOC (GENERATED ALWAYS AS IDENTITY), FK on MAHSBA
[ ] THONGBAO: PK=MATHONGBAO (GENERATED ALWAYS AS IDENTITY), includes KHOA and CAPBAC for OLS
[ ] All constraints enforced (PK, FK, NOT NULL, UNIQUE, CHECK)  
[ ] All 7 indexes created as specified (IDX_HSBA_MABN, IDX_HSBA_MABS, IDX_HSBADV_LOAIDV, IDX_HSBADV_NGAYDV, IDX_THONGBAO_NOIDUNG, IDX_THONGBAO_DIADIEM, IDX_DONTHUOC_TENTHUOC)  
[ ] 100,000 patients inserted (BENHNHAN) — production-scale per TC#5 requirement
[ ] 170 staff inserted (20 coordinators, 100 doctors, 50 technicians) with realistic distribution
[ ] 50,000+ medical records inserted (HSBA) — proportional to patient count
[ ] 100,000+ prescriptions inserted (DONTHUOC) — 2+ per medical record  
[ ] 75,000+ diagnostic services inserted (HSBA_DV) — 1.5+ per medical record
[ ] 10,000+ notifications inserted (THONGBAO) — proportional to records
[ ] Data matches Vietnamese specification exactly (names, addresses, diagnoses, medications)
[ ] No orphaned records — all FK relationships maintain referential integrity  
[ ] Tables ready for application use and OLS/RBAC testing

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
