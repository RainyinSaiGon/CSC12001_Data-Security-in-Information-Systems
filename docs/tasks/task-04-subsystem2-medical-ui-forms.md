# Task 04: Subsystem 2 - Medical Data Management UI Forms

**Assigned to:** Duyên  
**Type:** Front-end Implementation  
**Duration:** 25-30 hours  
**Priority:** High  
**Timeline:** Feb 18 - Feb 28, 2026

---

## 1. Objective

Implement 6 role-specific Windows Forms with automatic security filtering:

* **LoginForm** — Authentication, role detection
* **CoordinatorForm** — Patient/record management, full access
* **DoctorForm** — VPD-filtered patient list showing only assigned patients
* **TechnicianForm** — VPD-filtered service list showing only assigned services
* **PatientForm** — Row-level security; own records/prescriptions read-only
* **NotificationForm** — OLS label-based filtering

## 2. Scope of Work

* `LoginForm.cs` — User authentication, role determination
* `CoordinatorForm.cs` — Patient/record management with RBAC control
* `DoctorForm.cs` — VPD-filtered patient care (diagnoses, prescriptions)
* `TechnicianForm.cs` — VPD-filtered service management with result entry
* `PatientForm.cs` — Own records/prescriptions view with contact info edit
* `NotificationForm.cs` — OLS-filtered role-specific notifications

## 3. Requirements & Dependencies

* Windows Forms (C#) with role-based interfaces
* Integrate security services (AuthenticationService, RBACService, VPDService, OLSService)
* Automatic role detection and form redirection after login
* DataGrids for patient/record/service lists with database-enforced filtering
* Medical data read-only where appropriate (no patient editing of diagnoses)
* Dependencies: Task 07-09 (database), Task 05 (security services), Task 06 (business services)

## 4. Acceptance Criteria

* [ ] LoginForm authenticates correctly and determines role
* [ ] Coordinator sees all patients; Doctor sees only assigned; Technician sees only assigned services
* [ ] VPD filtering transparent (database enforced, no application changes needed)
* [ ] OLS filtering on notifications by user labels
* [ ] Row-level security enforced (patients see own records only)
* [ ] Medical data read-only where required; contact info editable
* [ ] Professional UI with clear status displays