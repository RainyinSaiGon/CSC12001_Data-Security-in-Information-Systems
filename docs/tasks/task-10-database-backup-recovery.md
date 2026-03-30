# Task 10: Database Backup and Recovery

**Assigned to:** Ngoc, Vu  
**Priority:** High  

## Objective

Implement Requirement 4 using Oracle-native backup and recovery methods. No UI is required.

## Required Scope

1. Research Oracle backup and recovery methods
2. Implement manual backup
3. Implement automatic backup
4. Recover from a problem using information obtained from the audit logs in Requirement 3
5. Evaluate advantages and disadvantages of the methods tested
6. Write a short conclusion

## Recommended Oracle Methods to Compare

- RMAN full backup
- RMAN incremental backup
- Data Pump export/import
- Flashback features where available

## Output Expectations

Place finished recovery assets in `database/Subsystem2-MedicalDB/recovery/`.

Suggested contents:

- backup strategy notes
- manual backup commands
- automatic backup scheduling scripts
- recovery walkthrough scripts
- comparison summary

## Acceptance Criteria

- At least one manual backup flow is demonstrated.
- At least one automatic backup flow is demonstrated.
- A recovery scenario is tied to an incident time visible in the audit logs.
- The documentation clearly states the tradeoffs of each tested method.
- The deliverables remain traceable to Requirement 4.
