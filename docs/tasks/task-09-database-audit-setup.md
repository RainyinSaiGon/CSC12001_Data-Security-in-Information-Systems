# Task 09: Database Audit Setup

**Assigned to:** Ngoc, Vu  
**Priority:** High  

## Objective

Implement Requirement 3 using Oracle auditing features and prepare queries that read the resulting audit logs. No UI is required for this task.

## Required Audit Coverage

### Standard Audit

Enable system auditing and choose five concrete audit scenarios involving:

- specific users
- specific objects
- success and failure cases
- a mix of tables, views, procedures, or functions

### FGA or Unified Audit

Implement the required assignment scenarios:

1. Updates on `DONTHUOC` attributes `MAHSBA`, `NGAYDT`, `TENTHUOC`, `LIEUDUNG` after the prescription already exists
2. Successful doctor updates on `HSBA.CHANDOAN`, `HSBA.DIEUTRI`, `HSBA.KETLUAN`
3. Illegal updates on `HSBA.CHANDOAN`, `HSBA.DIEUTRI`, `HSBA.KETLUAN`
4. Illegal insert, update, or delete on `HSBA_DV`

## Output Expectations

Place finished scripts in `database/Subsystem2-MedicalDB/audit/`.

Suggested outputs:

- standard audit setup script
- FGA or Unified Audit setup script
- audit log reading/reporting script

## Acceptance Criteria

- Five standard-audit contexts are demonstrated and readable.
- Required valid and invalid `HSBA` operations are captured.
- Required `DONTHUOC` post-creation updates are captured.
- Required illegal `HSBA_DV` operations are captured.
- Log-reading queries clearly show who did what, when, and whether it succeeded.
