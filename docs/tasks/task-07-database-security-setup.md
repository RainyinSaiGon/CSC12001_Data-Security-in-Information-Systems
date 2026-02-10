# Task 07: Database Security Setup - RBAC, VPD, OLS, Users

**Assigned to:** Ngọc, Vũ (Part B)  
**Type:** Database Administration  
**Duration:** 10 hours  
**Priority:** Critical (blocks Task 04)  
**Timeline:** Feb 17 - Feb 21, 2026

---

## Overview

Configure all access control mechanisms in Oracle database:

- Create 4 distinct database roles (Coordinator, Doctor, Technician, Patient)
- Configure role-based access control (RBAC) with permissions
- Implement Virtual Private Database (VPD) for row-level security
- Create Oracle Label Security (OLS) with 3-level hierarchy
- Create test users with proper role assignments

## Deliverables

### 04_Users_Creation.sql

Create 8 test database users with role assignments:

| User | Role | Department | Location | Purpose |
|------|------|------------|----------|---------|
| user_dir_001 | Coordinator | All | All | Director level access |
| user_dir_002 | Coordinator | All | All | Director level access |
| user_dh_001 | Doctor | Cardiology | Ho Chi Minh | Department head |
| user_dh_002 | Technician | Gastroenterology | Hai Phong | Department head |
| user_staff_001 | Doctor | Cardiology | Ho Chi Minh | Staff level |
| user_staff_002 | Doctor | Neurology | Ha Noi | Staff level |
| user_staff_003 | Technician | Gastroenterology | Hai Phong | Staff level |
| user_staff_004 | Patient | - | - | Patient access |

Requirements:

- Use CREATE USER statements
- Grant appropriate roles
- Assign OLS labels (from task 05)
- Default tablespace assignment
- Enable all accounts
- Never hardcode actual passwords

### 05_RBAC_Setup.sql

Create 4 roles with specific permissions:

**COORDINATOR Role:**

```
GRANT SELECT, INSERT, UPDATE ON BENHNHAN TO COORDINATOR
GRANT SELECT, INSERT, UPDATE ON HSBA TO COORDINATOR
GRANT SELECT ON NHANVIEN TO COORDINATOR
GRANT SELECT ON HSBA_DV TO COORDINATOR
```

Purpose: Manage patients, records, assign doctors/technicians

**DOCTOR Role:**

```
GRANT SELECT ON BENHNHAN TO DOCTOR (VPD filtered)
GRANT SELECT, INSERT, UPDATE ON HSBA TO DOCTOR (VPD filtered)
GRANT INSERT, UPDATE ON DONTHUOC TO DOCTOR
GRANT INSERT ON HSBA_DV TO DOCTOR
GRANT SELECT ON NHANVIEN TO DOCTOR
```

Purpose: Manage patient care, see only assigned patients

**TECHNICIAN Role:**

```
GRANT SELECT ON HSBA_DV TO TECHNICIAN (VPD filtered)
GRANT UPDATE ON HSBA_DV TO TECHNICIAN (VPD filtered)
GRANT SELECT ON BENHNHAN TO TECHNICIAN (for info)
GRANT SELECT ON HSBA TO TECHNICIAN
```

Purpose: Update service results, see only assigned services

**PATIENT Role:**

```
GRANT SELECT ON BENHNHAN TO PATIENT (single row own record)
GRANT UPDATE ON BENHNHAN TO PATIENT (own contact info only)
GRANT SELECT ON HSBA TO PATIENT (own records only)
GRANT SELECT ON DONTHUOC TO PATIENT (own prescriptions)
```

Purpose: View own medical information, update contact

### 06_VPD_Setup.sql

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
- Technicians: WHERE MANV = current_user
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

### 07_OLS_Setup.sql

Configure Oracle Label Security:

**Label Hierarchy (3 Levels):**

Level 1 - DEPARTMENTS:

- Cardiology
- Gastroenterology
- Neurology

Level 2 - LOCATIONS:

- Ho Chi Minh
- Hai Phong
- Ha Noi

Level 3 - CLASSIFICATIONS:

- Staff (lowest)
- DepartmentHead (medium)
- Director (highest)

**Implementation:**

```
Create OLS policy using DBMS_MACADM:
1. Create level components
2. Create level values
3. Apply policy to Notification table
4. Assign user labels
5. Assign notification labels
6. Configure label hierarchy rules
```

**Label Examples:**

- Staff level: "Cardiology:HoChiMinh:Staff"
- Dept head: "Cardiology:HoChiMinh:DepartmentHead"
- Director: "Cardiology:*:Director" (all locations)

**Access Rules:**

- User can access notification if user_label >= notification_label
- Must satisfy ALL 3 dimensions simultaneously
- Director (highest) can expand to all departments as needed

## Dependencies

- **Requires:** Task 06 tables completed (Fri Feb 14)
- **Blocks:** Task 04 security services
- **Blocks:** Task 03 medical forms (after implementation)

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

- **Wed Feb 19:** 04_Users_Creation.sql + 05_RBAC_Setup.sql
  - Unblocks Phôn to start AuthenticationService
  - Allows testing of role-based access

- **Thu Feb 20:** 06_VPD_Setup.sql + 07_OLS_Setup.sql
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
| `04_Users_Creation.sql` — 8 test users with role assignments | Required | Mon, Feb 17 |
| `05_RBAC_Setup.sql` — 4 roles with specific permissions | Critical | Wed, Feb 19 |

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
| `06_VPD_Setup.sql` — VPD policies for row-level security | Critical | Thu, Feb 20 |

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
| `07_OLS_Setup.sql` — 3-level label hierarchy, user label assignments | Required | Fri, Feb 21 |

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

- Task 06: Provides table foundation
- Task 04: Depends on users and roles existing
- Task 03: Depends on security mechanisms working
- Task 08: Audit logging complements security

---

**Critical Timeline: Thu Feb 20 for full security implementation**
