# Task 05: Subsystem 2 - Authentication And Security Integration

**Suggested owner:** shared with UI developer or main app maintainer  
**Priority:** High  
**Focus:** keep the medical app login and role routing stable

## Goal

Review the security-related services in the medical app and make sure all four user types work correctly.

## Current State

The app now supports:

- coordinator login
- doctor login
- technician login
- patient login

But this area is sensitive and should be kept stable while UI changes continue.

## Main Files

- `subsystem2-medicalDataManagement/source/medicalDataSystem/services/OracleConnectionService.cs`
- `subsystem2-medicalDataManagement/source/medicalDataSystem/services/AuthenticationService.cs`
- `subsystem2-medicalDataManagement/source/medicalDataSystem/services/ValidationService.cs`

## What To Verify

- app uses real Oracle users
- current schema behavior still works
- doctor, technician, patient, and coordinator all log in correctly
- patient path uses self views correctly
- error messages are understandable

## Acceptance Criteria

- `NV000001 / 123` works
- `NV000021 / 123` works
- `NV000121 / 123` works
- `BN000000001 / 123` works
- app uses `XEPDB1` successfully at runtime
