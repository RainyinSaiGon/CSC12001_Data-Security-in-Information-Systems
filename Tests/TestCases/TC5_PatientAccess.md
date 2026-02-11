# TC#5: Patient Self-Service Access

**Related Requirement:** Req 1 — Access Control & Interface  
**Priority:** High  
**Prerequisite:** TC#2 (RBAC) and TC#3 (VPD) passed

---

## Description

Verify that patients can view only their own data, update personal contact information, view their medical history (read-only), and cannot access other patients' records or modify medical data.

---

## Prerequisites

- TC#2 and TC#3 passed (RBAC + VPD configured)
- User 20000001 and 20000002 created with distinct medical records
- Sample data includes medical history for both patients

---

## Test Steps

### Step 1: View Own Data Only

```sql
CONNECT 20000001/[password]@XE;

-- Should return ONLY 20000001's record
SELECT MABENHNHAN, HOTEN, NGAYSINH, DIENTHOAI
FROM BENHNHAN;

-- Count should be exactly 1
SELECT COUNT(*) AS my_records FROM BENHNHAN;
```

**Expected Result:** Exactly 1 row returned — 20000001's own record

### Step 2: Cross-Patient Isolation

```sql
-- 20000001 should NOT see 20000002's data
CONNECT 20000001/[password]@XE;
SELECT COUNT(*) FROM BENHNHAN WHERE MABENHNHAN = 20000002;
-- Should return 0

-- Verify by connecting as 20000002
CONNECT 20000002/[password]@XE;
SELECT MABENHNHAN FROM BENHNHAN;
-- Should return only PATIENT002's ID
```

**Expected Result:** Each patient sees only their own record

### Step 3: Update Personal Information

```sql
CONNECT PATIENT001/[password]@XE;

-- Should succeed: Update own contact information
UPDATE BENHNHAN
SET DIENTHOAI = '0901234567'
WHERE MABENHNHAN = [own_id];

-- Verify the update
SELECT DIENTHOAI FROM BENHNHAN;
```

**Expected Result:** Contact info update succeeds

### Step 4: View Medical History (Read-Only)

```sql
CONNECT PATIENT001/[password]@XE;

-- Should succeed: Read own medical records
SELECT MAHSBA, NGAYTAO, CHANDOAN FROM HSBA;

-- Should succeed: Read own prescriptions
SELECT * FROM DONTHUOC dt
JOIN HSBA h ON dt.MAHSBA = h.MAHSBA;

-- Should succeed: Read own diagnostic services
SELECT * FROM HSBA_DV dv
JOIN HSBA h ON dv.MAHSBA = h.MAHSBA;
```

**Expected Result:** All reads return only PATIENT001's history — all within VPD scope

### Step 5: Negative Test — Modify Medical Data

```sql
CONNECT PATIENT001/[password]@XE;

-- Should FAIL: Cannot edit diagnosis
UPDATE HSBA SET CHANDOAN = 'Self-Diagnosis'
WHERE MAHSBA = [own_record_id];

-- Should FAIL: Cannot edit prescriptions
UPDATE DONTHUOC SET LIEUDUNG = '999mg'
WHERE MAHSBA = [own_record_id];

-- Should FAIL: Cannot insert medical records
INSERT INTO HSBA (MAHSBA, MABENHNHAN, CHANDOAN)
VALUES (99999, [own_id], 'Fake Record');

-- Should FAIL: Cannot delete records
DELETE FROM BENHNHAN WHERE MABENHNHAN = [own_id];
```

**Expected Result:** All modifications denied with `ORA-01031` insufficient privileges

### Step 6: Negative Test — Access Other Patients

```sql
CONNECT PATIENT001/[password]@XE;

-- Direct ID lookup — VPD should block
SELECT * FROM BENHNHAN WHERE MABENHNHAN = [PATIENT002_id];
-- Should return 0 rows

-- Wildcard search — VPD should still filter
SELECT * FROM BENHNHAN WHERE HOTEN LIKE '%Nguyen%';
-- Should return only PATIENT001 if name matches
```

**Expected Result:** VPD prevents any access to other patients' data

---

## Pass Criteria

- [ ] Patient sees exactly 1 record (own data only)
- [ ] Cannot see other patients' records
- [ ] Can update own contact information (phone, address)
- [ ] Can read own medical history (HSBA, prescriptions, services)
- [ ] Cannot modify medical records (diagnosis, prescriptions)
- [ ] Cannot INSERT or DELETE any records
- [ ] VPD data isolation between PATIENT001 and PATIENT002

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
