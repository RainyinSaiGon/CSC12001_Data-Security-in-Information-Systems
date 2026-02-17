# Task 06: Subsystem 2 - Medical Business Logic Services

**Assigned to:** Phôn (Part A)
**Type:** Backend Services
**Duration:** 20 hours
**Priority:** High
**Timeline:** Feb 21 - Feb 28, 2026

---

## 1. Objective

Develop the core business logic layer for the Medical Data Management System (Subsystem 2). This involves creating 5 distinct service classes that handle data operations for Patients, Doctors, Coordinators, Technicians, and Audit logging, ensuring strict adherence to the project's security and compliance requirements.

## 2. Scope of Work

### Patient Service (TC#5)

* Data access with `GetPatient()`, `GetMyRecords()` with data isolation (patients see only their records)
* `UpdateContactInfo()` allows contact fields (`SODT`, `SONHA`, `TENDUONG`, `QUANHUYEM`, `TINHTP`) only; rejects identity/medical fields
* All updates validated against allowed column list with logging of rejected attempts

### Doctor Service (TC#3)

* `GetAssignedPatients()` returns only assigned patients via database VPD
* Clinical operations: `CreateDiagnosis()` (HSBA), `OrderService()` (HSBA_DV), `DeleteService()`, `UpdatePrescription()` (DONTHUOC)
* Medical history updates restricted to assigned patients only

### Coordinator Service (TC#2)

* `GetAllPatients()` views entire patient registry without VPD restrictions
* Patient management: `AddPatient()`, `EditPatient()` for demographics
* Staff assignments: `AssignDoctor()` (link to HSBA), `AssignTechnician()` (link to HSBA_DV)

### Technician Service (TC#4)

* `GetAssignedServices()` shows only assigned diagnostic services
* `UpdateServiceResult()` modifies KETQUA field in HSBA_DV
* `CompleteService()` marks services as completed

### Audit Service (Requirement 3)

* `LogUserAction()` records application-level events with filtering (Date Range, User, Action Type)
* `LogSensitiveAccess()` specifically logs access to medical data columns

## 3. Deliverables

* `PatientService.cs` — Data access with contact-only edit restrictions and validation
* `DoctorService.cs` — Clinical operations (diagnosis, services, prescriptions) for assigned patients
* `CoordinatorService.cs` — Patient management and staff assignments
* `TechnicianService.cs` — Service workflow and result entry
* `AuditService.cs` — Event logging and audit trail generation

## 4. Acceptance Criteria

* [ ] Patient restrictions enforced: `TENBN`/`NGAYSINH` updates throw exception and log warning
* [ ] Doctor isolation: `GetAssignedPatients()` returns only assigned patients
* [ ] Technician limits: Can only update `KETQUA` for assigned services
* [ ] Coordinator access: Successfully assign doctors and technicians to records
* [ ] Audit trail: Critical actions (diagnosis updates, sensitive access, rejected edits) logged
* [ ] Security: All queries use parameterized statements to prevent injection
