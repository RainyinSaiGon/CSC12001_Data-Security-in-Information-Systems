# Task 04: Subsystem 2 - UI Polish

**Suggested owner:** 1 developer  
**Priority:** Medium  
**Focus:** improve the medical desktop UI without changing the security design

## Goal

Polish the WinForms UI in `subsystem2-medicalDataManagement` so the demo feels cleaner and easier to use.

## Current State

The UI is functional again and should stay stable. This task is for refinement, not major redesign.

## Good Targets

- login form layout
- coordinator dashboard readability
- doctor form readability
- technician form readability
- patient form readability
- notifications form readability
- labels, spacing, and button naming

## Important Constraints

- do not break the working login flow
- keep `localhost:1521/XEPDB1` visible in instructions
- do not replace Oracle-enforced behavior with UI-only checks

## Main Files

- `subsystem2-medicalDataManagement/source/medicalDataSystem/forms/LoginForm.cs`
- `subsystem2-medicalDataManagement/source/medicalDataSystem/forms/CoordinatorForm.cs`
- `subsystem2-medicalDataManagement/source/medicalDataSystem/forms/DoctorForm.cs`
- `subsystem2-medicalDataManagement/source/medicalDataSystem/forms/TechnicianForm.cs`
- `subsystem2-medicalDataManagement/source/medicalDataSystem/forms/PatientForm.cs`
- `subsystem2-medicalDataManagement/source/medicalDataSystem/forms/NotificationForm.cs`

## Acceptance Criteria

- forms are easier to read and use
- login still works for sample users
- no security behavior is moved from Oracle into the UI
- project builds cleanly
