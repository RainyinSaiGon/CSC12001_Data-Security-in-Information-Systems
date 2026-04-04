# Requirement 4 - Method Comparison and Conclusion

## Compared Methods

### RMAN full backup
- Scope: full physical database files, control file, SPFILE.
- Strengths: strong disaster recovery base; required for reliable PITR.
- Limits: larger backup size and longer runtime.

### RMAN incremental level 1 cumulative
- Scope: changed blocks since last level 0/full baseline.
- Strengths: smaller daily backup window and storage footprint.
- Limits: restore chain depends on baseline plus incrementals.

### Data Pump export/import (schema mode)
- Scope: logical objects/data for selected schema (`HOSPITAL_ADMIN`).
- Strengths: easy schema-level restore/migration; easy object inspection via `sqlfile`.
- Limits: not a full physical disaster recovery substitute.

## Recovery Strategy Used For Task 10
- Operational accident (logical issue): use Data Pump remap import into `RECOVERY_TEST` for verification and selective restore.
- Severe data corruption or PITR need: use RMAN restore/recover with `SET UNTIL TIME` anchored from audit timestamps.

## Tie-In With Requirement 3 Audit
- Audit trails (`DBA_AUDIT_TRAIL`, `DBA_FGA_AUDIT_TRAIL`) provide incident timestamps.
- `07_Recovery_Audit_Timestamp_Anchor.sql` computes a suggested restore timestamp (30 seconds before latest incident event).
- That timestamp is used in `08_Manual_Physical_Recovery_PITR.rman`.

## Conclusion
- The repository now includes:
  - manual + automatic backup flows (physical and logical),
  - repeatable logical recovery validation scripts,
  - a physical PITR walkthrough template,
  - audit-linked recovery timestamp selection guidance.
- This satisfies Requirement 4 deliverables with runnable scripts and report-ready documentation.
