# TC#2: RBAC Configuration

**Related Requirement:** Req 1 — Access Control & Interface  
**Priority:** Critical  
**Prerequisite:** TC#1 (User Setup) passed

---

## Description

Verify that Role-Based Access Control is correctly configured with 4 roles (COORDINATOR, DOCTOR, TECHNICIAN, PATIENT), each having specific permissions. Users should only be able to perform authorized actions.

---

## Prerequisites

- TC#1 passed (8 test users created)
- `05_RBAC_Setup.sql` executed successfully
- All 4 database roles created

---

## Test Steps

### Step 1: Verify Role Creation

```sql
-- Check all 4 roles exist
SELECT role FROM dba_roles
WHERE role IN ('ROLE_COORDINATOR', 'ROLE_DOCTOR', 'ROLE_TECHNICIAN', 'ROLE_PATIENT');
```

**Expected Result:** 4 roles returned

### Step 2: Verify COORDINATOR Permissions

```sql
-- Connect as COORDINATOR001
CONNECT COORDINATOR001/[password]@XE;

-- Should succeed: Full access to patient management
SELECT COUNT(*) FROM BENHNHAN;
INSERT INTO BENHNHAN (MABENHNHAN, HOTEN) VALUES ('TEST_P', 'Test Patient');
UPDATE BENHNHAN SET HOTEN = 'Updated' WHERE MABENHNHAN = 'TEST_P';
DELETE FROM BENHNHAN WHERE MABENHNHAN = 'TEST_P';

-- Should succeed: Access to appointment scheduling
SELECT COUNT(*) FROM LICHKHAM;
```

**Expected Result:** All operations succeed for COORDINATOR

### Step 3: Verify DOCTOR Permissions

```sql
-- Connect as DOCTOR001
CONNECT DOCTOR001/[password]@XE;

-- Should succeed: Read patient data (VPD-filtered)
SELECT COUNT(*) FROM BENHNHAN;

-- Should succeed: Update medical records
UPDATE HSBA SET CHANDOAN = 'Test Diagnosis'
WHERE MAHSBA = [assigned_record_id];

-- Should FAIL: Cannot manage staff
INSERT INTO NHANVIEN (MANV, HOTEN) VALUES ('TEST', 'Test');
```

**Expected Result:** SELECT/UPDATE on medical tables succeeds; INSERT to NHANVIEN denied

### Step 4: Verify TECHNICIAN Permissions

```sql
-- Connect as TECHNICIAN001
CONNECT TECHNICIAN001/[password]@XE;

-- Should succeed: View assigned diagnostic services
SELECT * FROM DICHVU WHERE assigned_tech = 'TECHNICIAN001';

-- Should succeed: Update service results
UPDATE DICHVU SET KETQUA = 'Normal' WHERE MADV = [assigned_service];

-- Should FAIL: Cannot access patient records
SELECT * FROM HSBA;

-- Should FAIL: Cannot modify patient data
UPDATE BENHNHAN SET HOTEN = 'Hacked' WHERE MABENHNHAN = 'BN001';
```

**Expected Result:** Service operations succeed; patient data access denied

### Step 5: Verify PATIENT Permissions

```sql
-- Connect as PATIENT001
CONNECT PATIENT001/[password]@XE;

-- Should succeed: Read own records
SELECT * FROM BENHNHAN WHERE MABENHNHAN = [own_id];

-- Should FAIL: Cannot modify medical records
UPDATE HSBA SET CHANDOAN = 'Self-diagnosis' WHERE MAHSBA = [any_record];

-- Should FAIL: Cannot view other patients
SELECT * FROM BENHNHAN WHERE MABENHNHAN != [own_id];
```

**Expected Result:** Own record read succeeds; all modifications and other patient access denied

### Step 6: Cross-Role Negative Tests

```sql
-- DOCTOR should NOT be able to do COORDINATOR actions
CONNECT DOCTOR001/[password]@XE;
DELETE FROM BENHNHAN WHERE MABENHNHAN = 'BN001';  -- Should FAIL

-- TECHNICIAN should NOT be able to do DOCTOR actions
CONNECT TECHNICIAN001/[password]@XE;
UPDATE HSBA SET CHANDOAN = 'Fake' WHERE MAHSBA = 'HSBA001';  -- Should FAIL

-- PATIENT should NOT be able to do TECHNICIAN actions
CONNECT PATIENT001/[password]@XE;
UPDATE DICHVU SET KETQUA = 'Fake' WHERE MADV = 'DV001';  -- Should FAIL
```

**Expected Result:** All cross-role actions denied with `ORA-01031` or `ORA-00942`

---

## Pass Criteria

- [ ] 4 database roles created (COORDINATOR, DOCTOR, TECHNICIAN, PATIENT)
- [ ] 8 test users created with correct role assignments
- [ ] COORDINATOR has full access to patient management tables
- [ ] DOCTOR has limited access (own patients only via VPD in TC#3)
- [ ] TECHNICIAN has access only to diagnostic services
- [ ] PATIENT has read-only access to own records
- [ ] All cross-role unauthorized actions are denied

---

## Test Execution Log

| Step | Tester | Date | Result | Notes |
|------|--------|------|--------|-------|
| 1 | | | ☐ Pass / ☐ Fail | |
| 2 | | | ☐ Pass / ☐ Fail | |
| 3 | | | ☐ Pass / ☐ Fail | |
| 4 | | | ☐ Pass / ☐ Fail | |
| 5 | | | ☐ Pass / ☐ Fail | |
| 6 | | | ☐ Pass / ☐ Fail | |

**Overall:** ☐ Pass / ☐ Fail  
**Tested By:** ________________  
**Date:** ________________
