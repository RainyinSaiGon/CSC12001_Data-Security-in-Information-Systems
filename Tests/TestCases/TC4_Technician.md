# TC#4: Technician Role Management

**Related Requirement:** Req 1 — Access Control & Interface  
**Priority:** High  
**Prerequisite:** TC#2 (RBAC) and TC#3 (VPD) passed

---

## Description

Verify that technicians can only view their assigned diagnostic services, update service results, and that all data filtering is transparent. Technicians must NOT be able to access patient records, other technicians' services, or modify data outside their scope.

---

## Prerequisites

- TC#2 and TC#3 passed (RBAC + VPD configured)
- 10000051 and 10000052 created with distinct service assignments
- Sample diagnostic service data loaded

---

## Test Steps

### Step 1: View Assigned Services Only

```sql
CONNECT 10000051/[password]@XE;

-- Should return ONLY services assigned to 10000051
SELECT MAHSBA_DV, LOAIDV, MAHSBA, NGAYDV, KETQUA
FROM HSBA_DV
ORDER BY NGAYDV DESC;

-- Count verification
SELECT COUNT(*) AS my_services FROM HSBA_DV;
```

**Expected Result:** Only 10000051's assigned services returned

### Step 2: Cross-Technician Isolation

```sql
-- 10000051 should NOT see 10000052's services
CONNECT 10000051/[password]@XE;
SELECT COUNT(*) FROM HSBA_DV WHERE MAKTV = 10000052;
-- Should return 0 (VPD filters them out)

-- Verify by connecting as 10000052
CONNECT 10000052/[password]@XE;
SELECT COUNT(*) AS tech2_services FROM HSBA_DV;
-- Should return different count from 10000051
```

**Expected Result:** Each technician sees a disjoint set of services

### Step 3: Update Service Results

```sql
CONNECT 10000051/[password]@XE;

-- Should succeed: Update result of assigned service
UPDATE HSBA_DV
SET KETQUA = 'Ket qua binh thuong'
WHERE MAHSBA_DV = [assigned_service_id];

-- Verify the update
SELECT MAHSBA_DV, KETQUA FROM HSBA_DV WHERE MAHSBA_DV = [assigned_service_id];
```

**Expected Result:** UPDATE succeeds, result stored correctly

### Step 4: Negative Test — Access Patient Records

```sql
CONNECT TECHNICIAN001/[password]@XE;

-- Should FAIL: Cannot read patient details
SELECT * FROM BENHNHAN;

-- Should FAIL: Cannot read medical records
SELECT * FROM HSBA;

-- Should FAIL: Cannot read prescriptions
SELECT * FROM DONTHUOC;
```

**Expected Result:** All queries return `ORA-00942: table or view does not exist` or empty results via VPD

### Step 5: Negative Test — Modify Non-Assigned Data

```sql
CONNECT TECHNICIAN001/[password]@XE;

-- Should FAIL: Cannot update another technician's service
UPDATE HSBA_DV
SET KETQUA = 'Tampered'
WHERE MAHSBA_DV = [other_tech_service_id];
-- VPD should make this update 0 rows

-- Should FAIL: Cannot insert new services
INSERT INTO HSBA_DV (LOAIDV, MAHSBA, NGAYDV, KETQUA, MAKTV)
VALUES ('Fake Service', 999, SYSDATE, 'Fake Result', 10000051);

-- Should FAIL: Cannot delete services
DELETE FROM HSBA_DV WHERE MAHSBA_DV = [any_service_id];
```

**Expected Result:** Update impacts 0 rows; INSERT/DELETE denied

### Step 6: Transparent Filtering Verification

```sql
CONNECT TECHNICIAN001/[password]@XE;

-- Simple query without WHERE — VPD adds filter automatically
SELECT * FROM HSBA_DV;

-- Technician doesn't need to know about the filter
-- The application code should work identically for all technicians
```

**Expected Result:** Data automatically filtered; no WHERE clause needed in application code

---

## Pass Criteria

- [ ] Technician sees only assigned diagnostic services
- [ ] Cannot access other technicians' services (VPD isolation)
- [ ] Can update service results for assigned services
- [ ] Cannot access BENHNHAN, HSBA, or DONTHUOC tables
- [ ] Cannot INSERT/DELETE service records
- [ ] Cannot UPDATE non-assigned services (0 rows affected)
- [ ] Filtering is transparent at application level

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
