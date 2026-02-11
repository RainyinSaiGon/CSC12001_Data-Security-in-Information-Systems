# Task 08: Subsystem 2 Database Security Setup - RBAC, VPD, OLS, Users

**Assigned to:** Ngọc, Vũ (Part B)  
**Type:** Database Administration  
**Duration:** 10 hours  
**Priority:** Critical (blocks Task 05)  
**Timeline:** Feb 17 - Feb 21, 2026

---

## Overview

Configure all Subsystem 2 access control mechanisms per Vietnamese specification requirements:

- **TC#1:** Create user accounts, link to NHANVIEN/BENHNHAN rows via primary key matching
- **TC#2 (Coordinator) & TC#5 (Patient):** Use **RBAC** role-based access control
- **TC#3 (Doctor) & TC#4 (Technician):** Use **VPD** (Virtual Private Database) for transparent row-level filtering
- Create 4 database roles with specific permission mappings
- Implement Oracle Label Security (OLS) with 3-level hierarchy for notifications
- Create test users with proper role assignments

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
- Create Oracle user for each NHANVIEN record with username = MANV (employee ID)
- Example: Employee with MANV='NV001' gets Oracle account 'NV001'
- Link: Oracle username automatically matches NHANVIEN.MANV (primary key)
- Role assignment: Determined by NHANVIEN.VAITRO value
  - VAITRO='Điều phối viên' → 'Điều phối viên' role
  - VAITRO='Bác sĩ/Y sĩ' → 'Bác sĩ/Y sĩ' role
  - VAITRO='Kỹ thuật viên' → 'Kỹ thuật viên' role

**Patient Account Strategy:**
- Create Oracle user for each BENHNHAN record with username = MABN (patient ID)
- Example: Patient with MABN='BN001' gets Oracle account 'BN001'
- Link: Oracle username automatically matches BENHNHAN.MABN (primary key)
- All patients assigned 'Bệnh nhân' role automatically
- VPD/RLS automatically filters to patient's own records via WHERE MABN = SYS_CONTEXT('USERENV','SESSION_USER')

**Test User Sample (8 representative accounts):**

| Account | Type | Data Row | Key Used | Assigned Role | Purpose |
|---------|------|----------|----------|---------------|---------|
| NV001 | Staff | NHANVIEN | MANV='NV001' | 'Điều phối viên' | Coordinator staff |
| NV005 | Staff | NHANVIEN | MANV='NV005' | 'Điều phối viên' | Coordinator staff |
| NV010 | Staff | NHANVIEN | MANV='NV010' | 'Bác sĩ/Y sĩ' | Doctor staff |
| NV020 | Staff | NHANVIEN | MANV='NV020' | 'Bác sĩ/Y sĩ' | Doctor staff |
| NV050 | Staff | NHANVIEN | MANV='NV050' | 'Kỹ thuật viên' | Technician staff |
| NV055 | Staff | NHANVIEN | MANV='NV055' | 'Kỹ thuật viên' | Technician staff |
| BN001 | Patient | BENHNHAN | MABN='BN001' | 'Bệnh nhân' | Patient access |
| BN002 | Patient | BENHNHAN | MABN='BN002' | 'Bệnh nhân' | Patient access |

**Production Scale:**
- Create 20 Coordinator accounts (NHANVIEN with VAITRO='Điều phối viên')
- Create 100 Doctor accounts (NHANVIEN with VAITRO='Bác sĩ/Y sĩ')
- Create 50 Technician accounts (NHANVIEN with VAITRO='Kỹ thuật viên')
- Create ~100,000 Patient accounts (BENHNHAN records automatically)

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
- Use CREATE USER for each MANV/MABN value (use chosen strategy above for MABN)
- Assign temporary password = username (change at first login)
- GRANT CONNECT role as minimum
- Grant specific role based on VAITRO/type
- VPD policies use: SYS_CONTEXT('USERENV','SESSION_USER') in WHERE clauses to match primary keys
- No additional user table required (Oracle manages authentication)
- For high-volume patient registration: Consider connection pooling to handle 100,000+ concurrent potential users

### 02_RBAC_Setup.sql

**Security Mechanisms Summary:**
- **RBAC (Action-based):** 'Điều phối viên', 'Kỹ thuật viên', 'Bệnh nhân' - defined role with allowed actions
- **VPD (Transparent Filtering):** 'Bác sĩ/Y sĩ', 'Kỹ thuật viên' - database pre-filters rows per user context

Create 4 roles with specific permissions:

**'Điều phối viên' Role (TC#2):**

```
GRANT SELECT, INSERT, UPDATE ON BENHNHAN TO 'Điều phối viên'
GRANT SELECT, INSERT, UPDATE ON HSBA TO 'Điều phối viên'
GRANT SELECT ON NHANVIEN TO 'Điều phối viên'
GRANT SELECT ON HSBA_DV TO 'Điều phối viên'
```

Special Permissions (via application logic):
- Can update MAKHOA (department) in HSBA via coordinator assignment
- Can update MABS (doctor) in HSBA via doctor assignment  
- Can update MAKTV (technician) in HSBA_DV via technician assignment

Purpose: Manage patients, records, assign doctors/technicians

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

**'Kỹ thuật viên' Role (TC#4 - RBAC Action Control + VPD Filtering):**

```
GRANT SELECT ON HSBA_DV TO 'Kỹ thuật viên'
GRANT UPDATE ON HSBA_DV TO 'Kỹ thuật viên'
GRANT SELECT ON BENHNHAN TO 'Kỹ thuật viên' (context only)
GRANT SELECT ON HSBA TO 'Kỹ thuật viên'
```

RBAC Action Permissions:
- Allowed: ViewAssignedServices, UpdateServiceResults (KETQUA), MarkServiceComplete
- Denied: DeleteServices, ViewOtherTechnicianServices
- Database VPD enforces: WHERE MAKTV = current_user (show only assigned services)
- All KETQUA updates are audit-logged (TC#4)

Purpose: Update diagnostic service results, see only own assigned/conducted services

**'Bệnh nhân' Role (TC#5 - RBAC Column-Level Restrictions):**

```
GRANT SELECT ON BENHNHAN TO 'Bệnh nhân'
GRANT UPDATE (SODT, SONHA, TENDUONG, QUANHUYEM, TINHTP) ON BENHNHAN TO 'Bệnh nhân'
GRANT SELECT ON HSBA TO 'Bệnh nhân'
GRANT SELECT ON DONTHUOC TO 'Bệnh nhân'
GRANT SELECT ON THONGBAO TO 'Bệnh nhân'
```

RBAC Column-Level Restrictions:
- **Never Editable:** MABN, TENBN, PHAI, NGAYSINH, CCCD, TIENSUABENH, TIENSUABENHGD, DIUNGTHUOC
- **Always Editable:** SODT, SONHA, TENDUONG, QUANHUYEM, TINHTP (contact info only)
- Database VPD enforces: WHERE MABN = current_user (see only own records)
- Application must validate and reject read-only field update attempts
- All edit attempts (successful or rejected) are audit-logged (TC#5)

Purpose: View own medical information, update only contact information

### 03_VPD_Setup.sql

Implement Virtual Private Database policies:

**HSBA Table Policy (Medical Records Filtering):**

```
Policy Function Logic:
- Doctors: WHERE MANV = current_user
- Coordinators: WHERE assigned_to_coordinator = current_user
- Others: No direct HSBA access
```

Implementation:

- Use DBMS_RLS package
- Create policy function returning WHERE clause
- Policy name: HSBA_VPD_Policy
- Apply to HSBA table
- Enable on all users

**HSBA_DV Table Policy (Service Filtering):**

```
Policy Function Logic:
- Technicians (TC#4): WHERE MAKTV = current_user  (MAKTV = technician ID)
- Others: No direct HSBA_DV access
```

Implementation:

- Create policy function
- Policy name: HSBA_DV_VPD_Policy
- Apply to HSBA_DV table
- Enable on all users

Requirements:

- Policies must be transparent to application
- VPD filtering occurs automatically for all queries
- No changes needed to service layer
- Performance impact should be minimal
- Test that unauthorized rows are hidden

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
- Table Name: **THONGBAO** (Notifications) with fields: NỘIDUNG, NGÀYGIỜ, ĐỊAĐIỂM

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
  - Creates all test users and assigns to RESOURCE role
  - Must run FIRST before any role assignments

- **Wed Feb 19:** 02_RBAC_Setup.sql
  - Creates 4 roles ('Điều phối viên', 'Bác sĩ/Y sĩ', 'Kỹ thuật viên', 'Bệnh nhân')
  - Assigns roles to users
  - Unblocks Phôn to start AuthenticationService
  - Allows testing of role-based access

- **Thu Feb 20:** 03_VPD_Setup.sql + 04_OLS_Setup.sql
  - Unblocks Phôn VPDService and OLSService
  - Unblocks Duyên medical forms

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

- Task 07: Provides table foundation
- Task 05: Depends on users and roles existing
- Task 04: Depends on security mechanisms working
- Task 09: Audit logging complements security

---

**Critical Timeline: Thu Feb 20 for full security implementation**
