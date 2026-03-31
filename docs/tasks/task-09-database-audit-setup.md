# Task 09: Audit Setup And Demo Evidence

**Suggested owner:** database maintainer or tester  
**Priority:** Medium  
**Focus:** verify Requirement 3 end to end

## Goal

Make sure the audit scripts work and prepare the exact demo evidence for Requirement 3.

## Main Files

- `database/Subsystem2-MedicalDB/audit/01_StandardAudit_Setup.sql`
- `database/Subsystem2-MedicalDB/audit/02_FGA_Setup.sql`
- `database/Subsystem2-MedicalDB/audit/03_ReadAuditLogs.sql`
- `database/Subsystem2-MedicalDB/Report.sql`

## What To Do

- rerun audit setup
- perform valid and invalid business actions
- confirm logs appear in the expected Oracle audit views
- prepare short proof queries or screenshots for the demo

## Acceptance Criteria

- Standard Audit works
- FGA works
- audit logs can be read back after demo actions
- demo script steps are verified against real output
