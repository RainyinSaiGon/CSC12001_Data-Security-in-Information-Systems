# Task 06: Database Schema Setup - Tables, Indexes, Sample Data

**Assigned to:** Person 5 (Part B)  
**Type:** Database Administration  
**Duration:** 8 hours  
**Priority:** Critical (blocks all other work)  
**Timeline:** Feb 10 - Feb 14, 2026 (MUST COMPLETE BY FRI 2/14)

---

## Overview

Create database schema foundation with 7 tables, performance indexes, and representative sample data. This task blocks all other work - completion by Friday Feb 14 is non-negotiable.

## Deliverables

### 01_CreateTables.sql

Create 7 core tables with proper constraints:

| Table | Purpose | Key Columns |
|-------|---------|-------------|
| BENHNHAN | Patients | MABN (PK), TENBN, PHAI, NGAYSINH, CCCD, email, phone |
| NHANVIEN | Staff | MANV (PK), HOTEN, VAITRO, CHUYENKHOA |
| HSBA | Medical Records | MAHSBA (PK), MABN (FK), MANV (FK), CHANDOAN, DIEUTRI, KETLUAN, Status |
| HSBA_DV | Diagnostic Services | MADV (PK), MAHSBA (FK), MANV (FK), TenDichVu, KETQUA, Status |
| DONTHUOC | Prescriptions | MADON (PK), MAHSBA (FK), TENHOA, LIEU, HUONGDAN |
| Notification | Notifications (OLS) | NotificationId (PK), Title, Content, Department, Location, Classification |
| AuditLog | Audit Trail | AuditId (PK), UserId, ActionTime, Action, TableName, RecordId |

Requirements:

- Use VARCHAR2, DATE, TIMESTAMP data types
- Primary keys on all tables
- Foreign key constraints with proper relationships
- NOT NULL constraints on required fields
- Unique constraints (CCCD in BENHNHAN, etc.)
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
| AuditLog | UserId | Audit by user |
| AuditLog | ActionTime | Audit by date |
| AuditLog | UserId, ActionTime | Combined query |
| Notification | (Dept, Loc, Class) | OLS filtering |

### 03_InsertSampleData.sql

Create realistic test data:

- **100 patients** (BENHNHAN)
  - Vietnamese names, realistic IDs
  - Birth dates creating age 18-80 range
  - Diverse addresses and phone numbers
  - Some with drug allergies (DiUng)

- **170 staff** (NHANVIEN)
  - 20 Coordinators (VAITRO='Coordinator')
  - 100 Doctors/Nurses (VAITRO='Doctor')
  - 50 Technicians (VAITRO='Technician')
  - Realistic names and specialties

- **20 medical records** (HSBA)
  - Link to patients and doctors
  - Realistic diagnoses and treatments
  - Date range from 3 months ago to current
  - Status: Pending, Active, Completed, Archived

- **50 prescriptions** (DONTHUOC)
  - Link to medical records
  - Realistic drug names (aspirin, amoxicillin, etc.)
  - Dosages: tablets, ml, injections
  - Expiration dates

- **30 diagnostic services** (HSBA_DV)
  - Types: X-ray, Lab test, Ultrasound, CT scan, ECG
  - Mix of statuses: Pending, InProgress, Completed
  - Some with results, some without

- **15 notifications** (Notification)
  - Varied departments and locations
  - OLS labels for testing
  - Realistic content

## Dependencies

- **Requires:** Oracle 21c (or compatible) installation
- **Unblocks:** All other tasks (Task 01-05, Task 07-08)

## Success Criteria

✓ All 7 tables created successfully  
✓ All constraints enforced  
✓ All 10+ indexes created  
✓ 100+ sample records inserted  
✓ Data is realistic and representative  
✓ Queries execute efficiently  
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
| **Primary Owner** | **Person 5 (Database)**, Person 2 (Service), Person 1 (Form) |
| **Test Timeline** | End of Week 1 (Fri Feb 14) |

**Person 5 Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `01_CreateTables.sql` — 7 core tables | Critical | Mon-Wed, Feb 10-12 |
| `02_CreateIndexes.sql` — Performance indexes | Required | Thu, Feb 13 |
| `03_InsertSampleData.sql` — Realistic test data | Required | Fri, Feb 14 |

**Pass Criteria:**

- ✓ All 7 tables created with zero SQL errors
- ✓ All PK/FK constraints properly defined
- ✓ 100 patients inserted with realistic Vietnamese names
- ✓ 20 staff records in NHANVIEN table
- ✓ 50+ medical records (HSBA) with proper relationships
- ✓ 30+ prescriptions linked to valid HSBA records
- ✓ 25+ diagnostic services with status tracking
- ✓ Sample data verifiable with SELECT COUNT(*) queries

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

- Task 07: Database security setup (needs these tables)
- Task 08: Audit setup (needs AuditLog table)
- All other tasks: Depend on this schema

---

**CRITICAL: Failure to complete by Friday 2/14 delays entire project**
