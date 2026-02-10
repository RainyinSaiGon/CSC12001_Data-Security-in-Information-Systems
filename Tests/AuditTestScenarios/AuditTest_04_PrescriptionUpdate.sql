-- =============================================================================
-- AuditTest_04: Prescription Update
-- =============================================================================
-- Scenario: DOCTOR001 updates the LIEUDUNG (dosage) column in DONTHUOC
--           (prescription) table.
-- Expected: Modification succeeds and is logged in audit trail with details.
-- Related: Req 3 - Audit & Monitoring, task-08 Scenario 4
-- =============================================================================

-- =====================
-- STEP 1: Record prescription baseline
-- =====================
CONNECT DOCTOR001/[password]@XE;

-- View current prescriptions for assigned patients
SELECT
    dt.MAHSBA,
    dt.MATHUOC,
    dt.LIEUDUNG,
    dt.SOLUONG
FROM DONTHUOC dt
JOIN HSBA h ON dt.MAHSBA = h.MAHSBA
ORDER BY h.NGAY DESC
FETCH FIRST 10 ROWS ONLY;

-- =====================
-- STEP 2: Update prescription dosage
-- =====================
-- Update dosage for a specific prescription
UPDATE DONTHUOC
SET LIEUDUNG = N'500mg x 3 lan/ngay (sau an)'
WHERE MAHSBA = (SELECT MIN(MAHSBA) FROM HSBA)
  AND MATHUOC = (
      SELECT MIN(dt.MATHUOC)
      FROM DONTHUOC dt
      WHERE dt.MAHSBA = (SELECT MIN(MAHSBA) FROM HSBA)
  );

COMMIT;

-- Verify the update
SELECT MAHSBA, MATHUOC, LIEUDUNG
FROM DONTHUOC
WHERE LIEUDUNG = N'500mg x 3 lan/ngay (sau an)';

-- =====================
-- STEP 3: Verify audit trail captures prescription change
-- =====================
CONNECT SYS/[password]@XE AS SYSDBA;

-- Standard audit
SELECT
    username,
    action_name,
    obj_name,
    returncode,
    TO_CHAR(timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time
FROM dba_audit_trail
WHERE username = 'DOCTOR001'
  AND obj_name = 'DONTHUOC'
ORDER BY timestamp DESC
FETCH FIRST 5 ROWS ONLY;

-- =====================
-- STEP 4: Verify FGA captures dosage-level changes
-- =====================
SELECT
    db_user,
    object_name,
    policy_name,
    sql_text,
    TO_CHAR(timestamp, 'YYYY-MM-DD HH24:MI:SS') AS event_time
FROM dba_fga_audit_trail
WHERE db_user = 'DOCTOR001'
  AND object_name = 'DONTHUOC'
ORDER BY timestamp DESC
FETCH FIRST 5 ROWS ONLY;

-- =====================
-- PASS CRITERIA:
-- =====================
-- [ ] Prescription update succeeds for DOCTOR001
-- [ ] Dosage (LIEUDUNG) column correctly updated
-- [ ] Standard audit records the UPDATE on DONTHUOC
-- [ ] FGA captures the prescription change with SQL text
-- [ ] Audit trail preserves the original and new dosage values
