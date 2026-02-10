# TC#3: VPD Implementation

**Related Requirement:** Req 1 — Access Control & Interface  
**Priority:** Critical  
**Prerequisite:** TC#2 (RBAC) passed

---

## Description

Verify that Virtual Private Database (VPD) policies transparently filter data at the database level, ensuring doctors see only their assigned patients, coordinators see assigned records, and technicians see only their assigned services.

---

## Prerequisites

- TC#2 passed (RBAC configured)
- `06_VPD_Setup.sql` executed successfully
- VPD policies active on relevant tables
- Sample data with multiple doctors and patient assignments

---

## Test Steps

### Step 1: Verify VPD Policies Exist

```sql
-- Check active VPD policies
SELECT object_owner, object_name, policy_name, function, enable
FROM dba_policies
WHERE object_owner = 'HOSPITAL_ADMIN'
ORDER BY object_name;
```

**Expected Result:** Policies active on BENHNHAN, HSBA, HSBA_DV tables

### Step 2: Doctor Data Isolation — HSBA Table

```sql
-- Connect as DOCTOR001
CONNECT DOCTOR001/[password]@XE;

-- Should return ONLY patients assigned to DOCTOR001
SELECT MAHSBA, MABENHNHAN, MABACSI FROM HSBA;

-- Verify: all rows should have MABACSI matching DOCTOR001's staff ID
SELECT COUNT(*) AS total_rows FROM HSBA;
SELECT COUNT(*) AS own_rows FROM HSBA WHERE MABACSI = [DOCTOR001_staff_id];
-- These two counts should be EQUAL
```

**Expected Result:** DOCTOR001 sees only records where MABACSI matches their staff ID

### Step 3: Doctor Data Isolation — Cross-Doctor Check

```sql
-- Connect as DOCTOR002
CONNECT DOCTOR002/[password]@XE;

-- Should return DIFFERENT set of patients
SELECT MAHSBA, MABENHNHAN, MABACSI FROM HSBA;

-- DOCTOR002 should NOT see DOCTOR001's patients
SELECT COUNT(*) FROM HSBA WHERE MABACSI = [DOCTOR001_staff_id];
-- Should return 0
```

**Expected Result:** DOCTOR002 sees zero of DOCTOR001's records

### Step 4: Coordinator Record Assignment

```sql
-- Connect as COORDINATOR001
CONNECT COORDINATOR001/[password]@XE;

-- Coordinator should see patients in their department/assignment
SELECT COUNT(*) FROM BENHNHAN;

-- Verify data scope based on coordinator's assignment
SELECT MABENHNHAN, HOTEN FROM BENHNHAN ORDER BY MABENHNHAN;
```

**Expected Result:** Coordinator sees patients in their assigned scope

### Step 5: Technician Service Filtering

```sql
-- Connect as TECHNICIAN001
CONNECT TECHNICIAN001/[password]@XE;

-- Should see only assigned diagnostic services
SELECT MADICHVU, TENDICHVU, KETQUA FROM HSBA_DV;

-- Verify: all returned services are assigned to TECHNICIAN001
SELECT MADICHVU FROM HSBA_DV
WHERE MAKYTHUATVIEN != [TECHNICIAN001_staff_id];
-- Should return 0 rows (VPD filters them out)
```

**Expected Result:** Technician sees only services assigned to them

### Step 6: VPD Transparency Check

```sql
-- Connect as DOCTOR001
CONNECT DOCTOR001/[password]@XE;

-- This query should work WITHOUT any WHERE clause for the doctor
-- VPD adds the filter transparently
EXPLAIN PLAN FOR SELECT * FROM HSBA;
SELECT * FROM TABLE(DBMS_XPLAN.DISPLAY);
-- Look for filter predicate added by VPD
```

**Expected Result:** Execution plan shows VPD-added predicate as a filter

### Step 7: Performance Overhead Check

```sql
-- Compare query time with and without VPD (as SYS vs normal user)
-- As SYS (no VPD):
SET TIMING ON;
SELECT COUNT(*) FROM HOSPITAL_ADMIN.HSBA;

-- As DOCTOR001 (with VPD):
CONNECT DOCTOR001/[password]@XE;
SET TIMING ON;
SELECT COUNT(*) FROM HSBA;
```

**Expected Result:** VPD overhead < 10% of base query time

---

## Pass Criteria

- [ ] VPD policies active on BENHNHAN, HSBA, HSBA_DV
- [ ] Doctor sees only assigned patients (HSBA filtered by MABACSI)
- [ ] Two doctors see completely disjoint patient sets
- [ ] Coordinator sees records in assigned scope
- [ ] Technician sees only assigned services
- [ ] VPD filtering is transparent (no explicit WHERE needed)
- [ ] Performance overhead < 10%

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
| 7 | | | ☐ Pass / ☐ Fail | |

**Overall:** ☐ Pass / ☐ Fail  
**Tested By:** ________________  
**Date:** ________________
