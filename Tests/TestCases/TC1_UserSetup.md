# TC#1: User Setup & Account Creation

**Related Requirement:** Req 1 — Access Control & Interface  
**Priority:** Critical  
**Prerequisite:** Task 06 (Database Schema) completed

---

## Description

Verify that user accounts can be created based on NHANVIEN (staff) records, linked to corresponding staff/patient entities, and authenticated successfully through the application.

---

## Prerequisites

- Oracle 21c XE instance running
- Schema tables created (including `USERNAME` column in `NHANVIEN`/`BENHNHAN`)
- `04_Users_Creation.sql` executed successfully
- Sample data inserted via `03_InsertSampleData.sql`

---

## Test Steps

### Step 1: Verify User Creation

```sql
-- Check all 8 test users exist
SELECT username, account_status, created
FROM dba_users
WHERE username IN (
    '10000001', '10000002', -- Coordinators
    '10000011', '10000012', -- Doctors
    '10000051', '10000052', -- Technicians
    '20000001', '20000002'  -- Patients
);
```

**Expected Result:** 8 rows returned, all with `ACCOUNT_STATUS = 'OPEN'`

### Step 2: Verify Role Assignments

```sql
-- Check role grants for each user
SELECT grantee, granted_role
FROM dba_role_privs
WHERE grantee IN (
    '10000001', '10000002',
    '10000011', '10000012',
    '10000051', '10000052',
    '20000001', '20000002'
)
ORDER BY grantee;
```

**Expected Result:** Each user has exactly one role assigned matching their prefix

### Step 3: Verify Staff Linkage

```sql
-- Verify users are linked to NHANVIEN records
```sql
-- Verify users are linked in NHANVIEN table (No joins needed)
SELECT MANV, HOTEN, VAITRO, USERNAME
FROM NHANVIEN
WHERE USERNAME IS NOT NULL
ORDER BY USERNAME;

-- Verify users are linked in BENHNHAN table
SELECT MABN, TENBN, USERNAME
FROM BENHNHAN
WHERE USERNAME IS NOT NULL
ORDER BY USERNAME;
```

**Expected Result:** Each user maps to a corresponding staff record

### Step 4: Authentication Test

```sql
-- Test authentication (connect as each user)
CONNECT DOCTOR001/[password]@XE;
SELECT USER FROM dual;
```

**Expected Result:** Connection succeeds, returns `DOCTOR001`

### Step 5: Verify Application Login

1. Launch the WinForms/Medical application
2. Enter credentials for each test user
3. Verify the login form accepts valid credentials
4. Verify invalid passwords are rejected

**Expected Result:** All 8 users can log in; invalid credentials denied

---

## Pass Criteria

- [ ] UserService creates all 8 users successfully
- [ ] Login form displays and validates credentials
- [ ] Database stores all 170 staff records from sample data
- [ ] Each user maps to correct NHANVIEN record
- [ ] Authentication works for all 4 role types
- [ ] Invalid credentials are rejected with appropriate error

---

## Test Execution Log

| Step | Tester | Date | Result | Notes |
|------|--------|------|--------|-------|
| 1 | | | ☐ Pass / ☐ Fail | |
| 2 | | | ☐ Pass / ☐ Fail | |
| 3 | | | ☐ Pass / ☐ Fail | |
| 4 | | | ☐ Pass / ☐ Fail | |
| 5 | | | ☐ Pass / ☐ Fail | |

**Overall:** ☐ Pass / ☐ Fail  
**Tested By:** ________________  
**Date:** ________________
