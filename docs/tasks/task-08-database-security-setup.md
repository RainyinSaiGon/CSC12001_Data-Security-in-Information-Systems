# Task 08: Database Security Setup Verification

**Suggested owner:** database maintainer  
**Priority:** High  
**Focus:** keep RBAC, VPD, and OLS scripts rerunnable and demo-ready

## Goal

Maintain the security setup scripts so a teammate can clone the repo and run them in the documented order without manual debugging.

## Main Files

- `database/Subsystem2-MedicalDB/security/01_RBAC_Setup.sql`
- `database/Subsystem2-MedicalDB/security/02_VPD_Setup.sql`
- `database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql`
- `database/Subsystem2-MedicalDB/Reset.sql`
- `database/Subsystem2-MedicalDB/Create_HOSPITAL_ADMIN.sql`

## Current Important Reality

- RBAC script creates Oracle users with password `123`
- OLS script is intentionally two-pass
- reset should clean both current and stale OLS policy states

## Acceptance Criteria

- setup works with `XEPDB1`
- RBAC script creates usable runtime accounts
- VPD script enables expected policies
- OLS script works on first run, reconnect, second run
- reset is safe for reruns
