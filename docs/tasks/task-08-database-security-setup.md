# Task 08: Subsystem 2 Database Security Setup - RBAC, VPD, OLS, Users

**Assigned to:** Ngọc, Vũ (Part B)  
**Type:** Database Administration  
**Duration:** 10 hours  
**Priority:** Critical (blocks Task 05)  
**Timeline:** Feb 17 - Feb 21, 2026

---

## Overview

Configure all Subsystem 2 access control mechanisms per Vietnamese specification requirements:

- **TC#1:** Create user accounts, link to NHANVIEN/BENHNHAN rows via USERNAME column matching
- **Câu 2 (TC#4 Technician & TC#5 Patient):** Use **RBAC** role-based access control with application-level filtering
- **Câu 3 (TC#2 Coordinator & TC#3 Doctor):** Use **VPD** (Virtual Private Database) for transparent row-level filtering
- Create 2 database roles with specific permission mappings 
- Implement Oracle Label Security (OLS) with 3-level hierarchy for THONGBAO notifications
- Create test users and production-scale accounts with proper role assignments
- Tables secured: BENHNHAN (100K patients), NHANVIEN (170 staff), HSBA (~140K-210K records), HSBA_DV (~140K-210K services), DONTHUOC (~280K-420K prescriptions), THONGBAO (12K notifications)

## Deliverables

Execute scripts in this order:

1. **01_Users_Creation.sql** (Create users FIRST)
2. **02_RBAC_Setup.sql** (Grant roles)
3. **03_VPD_Setup.sql** (Configure VPD policies)
4. **04_OLS_Setup.sql** (Configure OLS labels)

### 01_Users_Creation.sql

**TC#1: Account Creation and User-Data Linking**

DBA creates Oracle user accounts linked to database records without separate user management table:

**Staff Account Strategy:**

- Create Oracle user for each NHANVIEN record with username from USERNAME column
- Example: Employee with MANV=1, USERNAME='NV000001' gets Oracle account 'NV000001'
- Link: Oracle username ('NV000001') matches NHANVIEN.USERNAME column value
- VPD filtering uses: WHERE MANV = TO_NUMBER(SYS_CONTEXT('USERENV','SESSION_USER')) to link Oracle user to staff record
- Role assignment: Determined by NHANVIEN.VAITRO value
  - VAITRO='Điều phối viên' → 'Điều phối viên' role
  - VAITRO='Bác sĩ/Y sĩ' → 'Bác sĩ/Y sĩ' role
  - VAITRO='Kỹ thuật viên' → 'Kỹ thuật viên' role

**Patient Account Strategy:**

- Create Oracle user for each BENHNHAN record with username from USERNAME column
- Example: Patient with MABN=1234, USERNAME='BN000001234' gets Oracle account 'BN000001234'
- Link: Oracle username ('BN000001234') matches BENHNHAN.USERNAME column value
- RBAC filtering in application layer: Application enforces WHERE MABN = TO_NUMBER(SYS_CONTEXT('USERENV','SESSION_USER'))
- All patients assigned 'Bệnh nhân' role automatically
- Patient can only view/edit their own records via application-layer validation

**Test User Sample (8-12 representative accounts):**

Note: NHANVIEN.MANV uses GENERATED ALWAYS AS IDENTITY (auto-increment from 1), so actual staff IDs are 1-170. Usernames follow format: 'NV' + LPAD(MANV, 6, '0') for staff, 'BN' + LPAD(MABN, 9, '0') for patients.

| MANV/MABN | Type | Data Row | Username Generated | Link Method | Assigned Role | Purpose |
|---------|------|----------|---------|---------|---------------|----------|
| 1 | Coordinator | NHANVIEN.MANV=1 | NV000001 | Oracle user 'NV000001', query NHANVIEN WHERE USERNAME='NV000001' | Điều phối viên | Coordinator #1 |
| 5 | Coordinator | NHANVIEN.MANV=5 | NV000005 | Oracle user 'NV000005', query NHANVIEN WHERE USERNAME='NV000005' | Điều phối viên | Coordinator #5 |
| 25 | Doctor | NHANVIEN.MANV=25 | NV000025 | Oracle user 'NV000025', query NHANVIEN WHERE USERNAME='NV000025' | Bác sĩ/Y sĩ | Doctor (KHOA01) |
| 75 | Doctor | NHANVIEN.MANV=75 | NV000075 | Oracle user 'NV000075', query NHANVIEN WHERE USERNAME='NV000075' | Bác sĩ/Y sĩ | Doctor (KHOA02) |
| 130 | Technician | NHANVIEN.MANV=130 | NV000130 | Oracle user 'NV000130', query NHANVIEN WHERE USERNAME='NV000130' | Kỹ thuật viên | Technician #1 |
| 160 | Technician | NHANVIEN.MANV=160 | NV000160 | Oracle user 'NV000160', query NHANVIEN WHERE USERNAME='NV000160' | Kỹ thuật viên | Technician #2 |
| 1234 | Patient | BENHNHAN.MABN=1234 | BN000001234 | Oracle user 'BN000001234', query BENHNHAN WHERE USERNAME='BN000001234' | Bệnh nhân | Patient #1234 |
| 50000 | Patient | BENHNHAN.MABN=50000 | BN000050000 | Oracle user 'BN000050000', query BENHNHAN WHERE USERNAME='BN000050000' | Bệnh nhân | Patient #50000

**Production Scale:**

- Create 20 Coordinator accounts (NHANVIEN MANV 1-20 with VAITRO='Điều phối viên')
- Create 100 Doctor accounts (NHANVIEN MANV 21-120 with VAITRO='Bác sĩ/Y sĩ')
- Create 50 Technician accounts (NHANVIEN MANV 121-170 with VAITRO='Kỹ thuật viên')
- Create ~100,000 Patient accounts (BENHNHAN MABN 1-100,000 representing patient population)

**Bulk Patient Account Creation Strategy (~100,000 Scale):**

For patient accounts, use one of these strategies:

1. **Immediate Creation (High Resource):** Create Oracle user for each BENHNHAN row on patient registration
   - Pros: Immediate authentication available
   - Cons: High CPU/memory cost, must manage 100,000 users in Oracle
   - Recommended: For smaller patient bases (<10,000 users)

2. **Deferred Creation (Recommended):** Create Oracle user on first login attempt
   - Pros: Reduces upfront resource cost, lazy account provisioning
   - Cons: Slight delay on first login while account creates
   - Recommended: For large patient bases (>10,000 users) like this project
   - Implementation:
     - Pre-register in BENHNHAN table with MABN
     - On login attempt: Check if MABN Oracle user exists
     - If not: Create user with password = MABN (force change at first login)
     - Grant PATIENT role and VPD policies
     - Record account creation in AUDITLOG

3. **Batch Creation (Moderate):** Nightly batch job creates users for new registrations
   - Pros: Controlled resource usage, scheduled processing
   - Cons: Account not available immediately after patient registration
   - Implementation:
     - Identify new BENHNHAN rows with CREATED_DATE = today
     - Run batch script: `CREATE USER MABN IDENTIFIED BY mabn QUOTA UNLIMITED ON users`
     - Grant PATIENT role to each new user
     - Apply VPD policies
     - Log batch creation summary in AUDITLOG

**Recommended Implementation (Deferred + Batch Hybrid):**

- Coordinator registers patient → Insert BENHNHAN row (no user creation)
- Patient provided temporary access code
- On first login attempt: CreateUserIfNotExists(MABN)
  - Create user asynchronously in background
  - Return "Account initializing, please try again in 30 seconds"
  - Or create synchronously (takes 1-2 seconds per user)
- Nightly batch: Create users for any remaining uncreated patients
- Store account creation timestamp in BENHNHAN or dedicated PATIENT_ACCOUNTS table

**Implementation:**

- Use CREATE USER for each USERNAME value from NHANVIEN and BENHNHAN tables (01_Users_Creation.sql does this automatically)
- Username format: 'NV' + LPAD(MANV, 6, '0') for staff (NV000001..NV000170), 'BN' + LPAD(MABN, 9, '0') for patients (BN000000001..BN000100000)
- Assign temporary password = '123' (insecure for production - MUST change on first login)
- GRANT CREATE SESSION role as minimum
- Grant specific role based on VAITRO (staff) or automatically assign 'Bệnh nhân' (patients)
- VPD policies use: WHERE MANV = TO_NUMBER(SYS_CONTEXT('USERENV','SESSION_USER')) for staff lookup
- RBAC filtering uses: WHERE MABN = TO_NUMBER(SYS_CONTEXT('USERENV','SESSION_USER')) for patient lookup (application-layer enforcement)
- No additional user table required (Oracle manages authentication, USERNAME column in NHANVIEN/BENHNHAN provides linking)
- For high-volume patient registration: Consider connection pooling to handle 100,000+ concurrent potential users

### 02_RBAC_Setup.sql

**Security Mechanisms Summary (Per Original Requirements):**

- **Câu 2 - RBAC (Role-Based Access Control):** 'Kỹ thuật viên', 'Bệnh nhân' - defined roles with allowed actions, filtering in application layer
- **Câu 3 - VPD (Virtual Private Database):** 'Điều phối viên', 'Bác sĩ/Y sĩ' - database transparently pre-filters rows per user context

Create 4 roles with specific permissions:

**'Điều phối viên' Role (TC#2):**

```
GRANT SELECT, INSERT, UPDATE ON BENHNHAN TO 'Điều phối viên'
GRANT SELECT, INSERT, UPDATE ON HSBA TO 'Điều phối viên'
GRANT SELECT ON NHANVIEN TO 'Điều phối viên'
GRANT SELECT, UPDATE (MAKTV) ON HSBA_DV TO 'Điều phối viên'
```

VPD Filtering (Câu 3 Requirement):

- VPD policy may filter by department/location assignment (implementation-specific)
- Or allow full access (1=1) if coordinators manage all patients
- Can update MAKHOA (department) in HSBA via coordinator assignment
- Can update MABS (doctor) in HSBA via doctor assignment  
- Can update MAKTV (technician) in HSBA_DV via technician assignment

Purpose: Manage patients, records, assign doctors/technicians (VPD-filtered access)

**'Bác sĩ/Y sĩ' Role (TC#3 - VPD Filtered):**

```
GRANT SELECT ON BENHNHAN TO 'Bác sĩ/Y sĩ'
GRANT SELECT, INSERT, UPDATE ON HSBA TO 'Bác sĩ/Y sĩ'
GRANT INSERT, UPDATE, DELETE ON DONTHUOC TO 'Bác sĩ/Y sĩ'
GRANT INSERT, DELETE ON HSBA_DV TO 'Bác sĩ/Y sĩ'
GRANT SELECT ON NHANVIEN TO 'Bác sĩ/Y sĩ'
GRANT UPDATE (TIENSUABENH, TIENSUABENHGD, DIUNGTHUOC) ON BENHNHAN TO 'Bác sĩ/Y sĩ'
```

Special Permissions (VPD Filtered by WHERE MABS = current_user):

- Can DELETE rows from HSBA_DV: remove unnecessary diagnostic services
- Can UPDATE patient medical history fields: TIENSUABENH, TIENSUABENHGD, DIUNGTHUOC
- All updates to CHANDOAN, DIEUTRI, KETLUAN, TENTHUOC, LIEUUNG are audit-logged (TC#3.c)
- VPD transparent filtering: only sees/updates assigned patients' records

Purpose: Manage patient care, see and update only assigned patients

**'Kỹ thuật viên' Role (TC#4 - RBAC per Câu 2, NO VPD):**

```
GRANT SELECT ON HSBA_DV TO 'Kỹ thuật viên'
GRANT UPDATE (KETQUA) ON HSBA_DV TO 'Kỹ thuật viên'
GRANT SELECT ON BENHNHAN TO 'Kỹ thuật viên' (context only)
GRANT SELECT ON HSBA TO 'Kỹ thuật viên'
```

RBAC Implementation (Câu 2 Requirement):

- **NO VPD!** Filtering done in application layer (TechnicianService.cs)
- Application enforces: WHERE MAKTV = current_user (show only assigned services)
- Allowed: ViewAssignedServices, UpdateServiceResults (KETQUA)
- Denied: DeleteServices, ViewOtherTechnicianServices, UpdateOtherFields
- All KETQUA updates are audit-logged (TC#4)

Purpose: Update diagnostic service results, application filters to show only own assigned services

**'Bệnh nhân' Role (TC#5 - RBAC per Câu 2, NO VPD):**

```
GRANT SELECT ON BENHNHAN TO 'Bệnh nhân'
GRANT UPDATE (SONHA, TENDUONG, QUANHUYEN, TINHTP) ON BENHNHAN TO 'Bệnh nhân'
GRANT SELECT ON HSBA TO 'Bệnh nhân'
GRANT SELECT ON DONTHUOC TO 'Bệnh nhân'
GRANT SELECT ON THONGBAO TO 'Bệnh nhân'
```

RBAC Implementation (Câu 2 Requirement):

- **NO VPD!** Filtering done in application layer (PatientService.cs)
- Application enforces: WHERE MABN = current_user (see only own records)
- **Never Editable:** MABN, TENBN, PHAI, NGAYSINH, CCCD, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC
- **Always Editable:** SONHA, TENDUONG, QUANHUYEN, TINHTP (contact info only)
- Application must validate and reject read-only field update attempts
- All edit attempts (successful or rejected) are audit-logged (TC#5)

Purpose: View own medical information, update only contact information (application-filtered)

### 03_VPD_Setup.sql

Implement Virtual Private Database policies (Câu 3: Coordinator & Doctor ONLY):

**HSBA Table Policy (Medical Records Filtering for Doctors & Coordinators):**

```
Policy Function Logic:
- Doctors (TC#3): WHERE MABS = current_user_MANV
- Coordinators (TC#2): WHERE 1=1 (or department-based filtering)
- Others: No VPD (Technician/Patient use application filtering)
```

Implementation:

- Use DBMS_RLS package
- Create policy function returning WHERE clause based on role
- Policy name: HSBA_DOCTOR_COORDINATOR_VPD
- Apply to HSBA table for Doctors and Coordinators only
- Enable context-aware filtering

**BENHNHAN Table Policy (Patient Filtering for Doctors & Coordinators):**

```
Policy Function Logic:
- Doctors (TC#3): WHERE MABN IN (SELECT MABN FROM HSBA WHERE MABS = current_user_MANV)
- Coordinators (TC#2): WHERE 1=1 (full access or department-filtered)
- Others: No VPD (Patient uses application filtering)
```

**IMPORTANT - NO VPD for Technician or Patient:**

- Technician (TC#4): Uses RBAC + application filtering in TechnicianService.cs
- Patient (TC#5): Uses RBAC + application filtering in PatientService.cs
- Per Câu 2 requirement: These roles use RBAC, NOT VPD

Requirements:

- VPD policies apply ONLY to Coordinator and Doctor roles (Câu 3)
- Policies must be transparent to application
- VPD filtering occurs automatically for Coordinator/Doctor queries
- No changes needed to service layer for VPD-enabled roles
- Performance impact should be minimal
- Test that unauthorized rows are hidden for Doctors/Coordinators

### 04_OLS_Setup.sql

Configure Oracle Label Security:

**Label Hierarchy (3 Levels):**

Level 1 - DEPARTMENTS (KHOA):

- **Tim mạch** (Cardiology)
- **Tiêu hóa** (Gastroenterology)
- **Thần kinh** (Neurology)

Level 2 - LOCATIONS (ĐỊA ĐIỂM):

- **Hồ Chí Minh** (Ho Chi Minh)
- **Hải Phòng** (Hai Phong)
- **Hà Nội** (Ha Noi)

Level 3 - CLASSIFICATIONS (CẤP BẬC):

- **Nhân viên** (Staff) - lowest
- **Lãnh đạo khoa** (DepartmentHead) - medium
- **Ban Giám đốc** (Director) - highest

**Implementation:**

```
Create OLS policy using DBMS_MACADM:
1. Create level components
2. Create level values
3. Apply policy to THONGBAO (Notification) table
4. Assign user labels
5. Assign notification labels 
6. Configure label hierarchy rules
```

**Label Examples:**

- Staff level: "Tim mạch:Hồ Chí Minh:Nhân viên"
- Dept head: "Tim mạch:Hồ Chí Minh:Lãnh đạo khoa"
- Director: "Tim mạch:*:Ban Giám đốc" (all locations)
- Table Name: **THONGBAO** (Notifications) with fields: MATHONGBAO, NOIDUNG, NGAYGIO, DIADIEM

**Access Rules:**

- User can access notification if user_label >= notification_label
- Must satisfy ALL 3 dimensions simultaneously
- Director (highest) can expand to all departments as needed

## Dependencies

- **Requires:** Task 07 tables completed (Fri Feb 14)
- **Blocks:** Task 05 security services
- **Blocks:** Task 04 medical forms (after implementation)

## Success Criteria

✓ All 4 roles created with correct permissions  
✓ 8 test users created and functional  
✓ RBAC prevents unauthorized operations  
✓ VPD policies transparently filter data  
✓ OLS label hierarchy enforces access rules  
✓ Users cannot bypass security mechanisms  
✓ Services see pre-filtered data automatically  
✓ All policies enabled and tested

## Implementation Schedule

- **Mon Feb 17:** 01_Users_Creation.sql
  - Creates all test and production-scale users (170 staff, ~100K patients concept)
  - Creates 8 test users for functional testing
  - Must run FIRST before any role assignments
  - Unblocks tasks that need authentication

- **Wed Feb 19:** 02_RBAC_Setup.sql
  - Creates 4 roles (Điều phối viên, Bác sĩ/Y sĩ, Kỹ thuật viên, Bệnh nhân)
  - Assigns roles to test users
  - Unblocks Phôn to start AuthenticationService
  - Allows testing of role-based access control

- **Thu Feb 20:** 03_VPD_Setup.sql + 04_OLS_Setup.sql
  - Implements row-level filtering for HSBA (doctors) and HSBA_DV (technicians)
  - Implements 3-level label hierarchy on THONGBAO table
  - Assigns user labels based on roles
  - Unblocks Phôn VPDService and OLSService
  - Unblocks Duyên medical forms

**Note:** Task 08 must complete before Task 09 (Audit Setup) can properly audit user actions.

## Testing Checklist

After implementation:

- Verify users can authenticate with roles
- Test RBAC: confirm Doctor cannot execute Coordinator actions
- Test VPD: confirm Doctor sees only assigned patients
- Test VPD: confirm Technician sees only assigned services
- Test OLS: confirm notifications filtered by labels
- Test that ALL users together see correct data sets
- Document any configuration issues

## Traceability Matrix

### TC#2: RBAC Configuration (Database Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Test Timeline** | Wed Feb 19 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `01_Users_Creation.sql` — 8 test users created in system | Required | Mon, Feb 17 |
| `02_RBAC_Setup.sql` — 4 roles with specific permissions | Critical | Wed, Feb 19 |

**Pass Criteria:**

- ✓ 4 database roles created (COORDINATOR, DOCTOR, TECHNICIAN, PATIENT)
- ✓ 8 test users created with correct role assignments
- ✓ COORDINATOR has full access to patient tables
- ✓ DOCTOR has limited access (own patients only via VPD)
- ✓ TECHNICIAN has access only to diagnostic services
- ✓ PATIENT has read-only access to own records
- ✓ Role switching test: user cannot escalate privileges

**Evidence Tracking:**

- Role creation scripts output log
- Query DBA_ROLE_PRIVS to verify assignments
- Manual privilege escalation attempt (must fail)

---

### TC#3: VPD Implementation (Database Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Test Timeline** | Thu Feb 20 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `03_VPD_Setup.sql` — VPD policies for row-level security | Critical | Thu, Feb 20 |

**Pass Criteria:**

- ✓ VPD policy on HSBA table: Doctors see only assigned patient records
- ✓ VPD policy on DICHVU table: Technicians see only assigned services
- ✓ VPD policy on BENHNHAN table: Patients see only own record
- ✓ VPD transparent: SELECT * FROM HSBA returns filtered results per session user
- ✓ VPD performance: query overhead < 10%

**Evidence Tracking:**

- VPD policy creation log
- SELECT tests from multiple user sessions showing different row counts
- EXPLAIN PLAN comparison (with/without VPD)

---

### OLS#1: OLS Hierarchy Setup

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 2: OLS Notification System |
| **Test Timeline** | Thu-Fri Feb 20-21 |

**Ngọc, Vũ Database Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `04_OLS_Setup.sql` — 3-level label hierarchy, user label assignments | Required | Fri, Feb 21 |

**Pass Criteria:**

- ✓ OLS policy created with 3 label dimensions (Department, Location, Classification)
- ✓ Labels assigned to all 8 test users based on their roles
- ✓ 15 sample notifications created with different label levels
- ✓ Director can see all 15 notifications
- ✓ Department Head sees only own department notifications
- ✓ Regular staff sees only matching-label notifications
- ✓ OLS enforcement verified through SELECT from different sessions

**Evidence Tracking:**

- OLS setup script output log
- SELECT THONGBAO from multiple user sessions with different label clearances
- Row count comparison across user sessions

---

## Related Tasks

- Task 07: Provides table foundation (COMPLETED)
- **Task 09:** Database audit setup (depends on users/roles created here)
- Task 05: Depends on users and roles existing
- Task 04: Depends on security mechanisms working
- Task 10: Backup/recovery complements security

---

**Critical Timeline: Thu Feb 20 for full security implementation**
