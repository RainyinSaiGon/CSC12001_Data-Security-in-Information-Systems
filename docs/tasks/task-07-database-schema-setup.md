# Task 07: Database Schema And Sample Data Verification

**Suggested owner:** database maintainer  
**Priority:** Medium  
**Focus:** keep schema and sample data consistent for demos

## Goal

Verify that the schema and sample data scripts remain stable and support the security demos.

## Main Files

- `database/Subsystem2-MedicalDB/schema/01_CreateTables.sql`
- `database/Subsystem2-MedicalDB/schema/02_CreateIndexes.sql`
- `database/Subsystem2-MedicalDB/schema/03_InsertSampleData.sql`

## What To Check

- required relations still exist
- helper columns such as `USERNAME` are still present
- sample data still supports:
  - coordinator
  - doctor
  - technician
  - patient
  - OLS demo users

## Acceptance Criteria

- schema scripts run from a clean reset
- sample users needed by the demo exist
- OLS demo users such as `NV000090` and `NV000060` still exist
