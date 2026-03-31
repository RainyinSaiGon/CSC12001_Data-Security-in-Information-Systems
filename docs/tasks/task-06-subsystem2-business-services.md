# Task 06: Subsystem 2 - Business Service Verification

**Suggested owner:** shared with app developer  
**Priority:** Medium  
**Focus:** make sure form actions match Oracle permissions

## Goal

Verify the business services behind the medical app so each role can do only the required actions.

## Main Files

- `subsystem2-medicalDataManagement/source/medicalDataSystem/services/CoordinatorService.cs`
- `subsystem2-medicalDataManagement/source/medicalDataSystem/services/DoctorService.cs`
- `subsystem2-medicalDataManagement/source/medicalDataSystem/services/TechnicianService.cs`
- `subsystem2-medicalDataManagement/source/medicalDataSystem/services/PatientService.cs`

## What To Check

- coordinator can add patients and assign doctor or technician
- doctor can work only on assigned records
- technician can update only assigned service results
- patient can update only allowed personal fields
- service methods respect Oracle grants and views

## Acceptance Criteria

- no role can perform a forbidden action through the app
- patient updates go through self-service objects
- code builds cleanly
