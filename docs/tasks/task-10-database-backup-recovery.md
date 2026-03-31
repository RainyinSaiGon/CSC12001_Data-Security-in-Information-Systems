# Task 10: Requirement 4 - Backup And Recovery

**Suggested owner:** 1 developer  
**Priority:** High  
**Focus:** complete the unfinished Requirement 4 work

## Goal

Implement the backup and recovery part that is still missing from the repository.

## Current State

This is the biggest clearly unfinished requirement in the repo.

The folder exists:

- `database/Subsystem2-MedicalDB/recovery`

But the final checked-in scripts and documentation are still incomplete.

## Expected Deliverables

- one manual backup flow
- one automatic backup flow
- one recovery walkthrough
- tie the recovery scenario to audit timestamps from Requirement 3
- short comparison of methods
- short conclusion for the report

## Good Oracle Options To Compare

- RMAN full backup
- RMAN incremental backup
- Data Pump export/import
- Flashback features if available in the local Oracle environment

## Suggested Output Files

- backup notes
- backup commands
- scheduled backup script or instructions
- recovery walkthrough
- comparison summary

## Acceptance Criteria

- Requirement 4 has real checked-in assets under `database/Subsystem2-MedicalDB/recovery`
- backup can be demonstrated
- recovery can be explained and repeated
- documentation is clear enough for the final report
