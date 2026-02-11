# Task 06: Subsystem 2 - Medical Business Logic Services

**Assigned to:** Phôn (Part A)  
**Type:** Backend Services  
**Duration:** 20 hours  
**Priority:** High  
**Timeline:** Feb 21 - Feb 28, 2026

---

## Overview

Implement 5 business logic services for medical operations:

- Patient data access with row-level security
- Doctor clinical operations (diagnoses, prescriptions)
- Coordinator patient management
- Technician service workflows
- Audit logging for compliance

## Deliverables

| Service | Purpose | Key Operations |
|---------|---------|-----------------|
| PatientService | Patient data access | GetPatient, GetMyRecords, UpdateContactInfo |
| DoctorService | Clinical operations | GetAssignedPatients, CreateDiagnosis, OrderService |
| CoordinatorService | Full patient management | GetAllPatients, AddPatient, AssignDoctor, AssignTechnician |
| TechnicianService | Service workflows | GetAssignedServices, UpdateResults, CompleteService |
| AuditService | Compliance logging | LogUserAction, GetAuditLogs, LogSensitiveAccess |

## Requirements

- Implement 5 service classes with complete method signatures
- Use OracleConnectionService for database access
- Parameterized queries (prevent SQL injection)
- Proper exception handling
- Audit logging for sensitive operations
- Support row-level security in PatientService and DoctorService (via VPD)
- **Column-level restrictions:** Patient can only edit contact fields, reject all other field edits
- VPD filtering transparent through VPD policy application

## Column-Level Edit Restrictions (TC#5)

**Patient CANNOT Edit (Read-Only - Reject Attempts):**
- MABN (patient ID)
- TENBN (name)
- PHAI (gender)
- NGAYSINH (birth date)
- CCCD (national ID)
- TIENSUABENH (personal medical history)
- TIENSUABENHGD (family medical history)
- DIUNGTHUOC (drug allergies)

**Patient CAN ONLY Edit:**
- SODT (phone number)
- SONHA (house number)
- TENDUONG (street name)
- QUANHUYEM (district)
- TINHTP (province/city)

**Implementation:**
- PatientService.UpdatePatientInfo() must validate submitted fields
- Reject any attempt to modify read-only fields with ValidationException
- Log all edit attempts (successful and rejected) for audit trail
- Application blocks UI updates to read-only fields as well

## Dependencies

- **Requires:** Ngọc, Vũ's database tables (Fri Feb 14)
- **Requires:** Task 05's security services for validation
- **Unblocks:** Task 04 (forms need these services)

## Success Criteria

✓ All 5 services fully implemented  
✓ Data queries return correct results  
✓ Row-level security enforced in PatientService  
✓ Audit logging captures all sensitive operations  
✓ Services work with Task 05 security services  
✓ No hardcoded data or connection strings  
✓ Comprehensive error handling

## Service Details

### PatientService

- **GetPatient(patientId): Patient** — **VPD filters to own record only** (WHERE MABN = patientId)
- **UpdatePatientInfo(patient): bool** — validates and rejects read-only field edits
  - Allowed fields: SODT, SONHA, TENDUONG, QUANHUYEM, TINHTP
  - Throws ValidationException if read-only fields modified (MABN, TENBN, PHAI, NGAYSINH, CCCD, TIENSUABENH, TIENSUABENHGD, DIUNGTHUOC)
  - Logs all attempts (accepted/rejected) for audit
- GetMyMedicalRecords(patientId): List<MedicalRecord> — **VPD pre-filters to own records** (transparent filtering)
- GetMyPrescriptions(patientId): List<Prescription> — linked to own medical records via MAHSBA FK
- **ValidateEditableFields(fieldDict): bool** — enforces column-level restrictions

### DoctorService

- **GetAssignedPatients(doctorId): List<Patient>** — **VPD transparent filter:** WHERE MABS = doctorId
- **CreateDiagnosis(record): bool** — INSERT/UPDATE CHANDOAN, DIEUTRI, KETLUAN (audit-logged per TC#3.c)
- **UpdatePrescription(prescription): bool** — UPDATE TENTHUOC, LIEUUNG (audit-logged post-creation per TC#3.e)
- OrderDiagnosticService(service): bool — INSERT HSBA_DV (diagnostic test/service)
- **DeleteDiagnosticService(serviceId): bool** — DELETE HSBA_DV (remove unnecessary services per TC#3.b)
- **UpdatePatientHistory(patientId, history): bool** — UPDATE TIENSUABENH, TIENSUABENHGD, DIUNGTHUOC on **assigned patients only** (subject to VPD)

### CoordinatorService

- **GetAllPatients(): List<Patient>** — SELECT * FROM BENHNHAN (**unrestricted, no VPD for Coordinator per TC#2**)
- AddPatient(patient): bool — INSERT BENHNHAN
- EditPatient(patient): bool — UPDATE BENHNHAN
- **AssignDoctorToPatient(doctorId, patientId): bool** — UPDATE HSBA.MABS (assign treating physician)
- **AssignTechnicianToService(technicianId, serviceId): bool** — UPDATE HSBA_DV.MAKTV (assign service performer)
- GetRecordStatus(recordId): string — SELECT from HSBA

### TechnicianService

- **GetAssignedServices(technicianId): List<DiagnosticService>** — **VPD transparent filter:** WHERE MAKTV = technicianId
- **UpdateServiceResult(serviceId, result): bool** — UPDATE HSBA_DV.KETQUA (audit-logged per TC#4)
- CompleteService(serviceId): bool — mark service complete
- **ValidateAssignment(serviceId, technicianId): bool** — ensure technician only accesses own assigned services

### AuditService

- LogUserAction(userId, action, details): bool
- GetAuditLogs(startDate, endDate, user?): List<AuditLogEntry>
- LogSensitiveAccess(userId, dataType, recordId): bool

## Traceability Matrix

### TC#4: Technician Access (Service Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Test Timeline** | End of Week 2-3 |

**Phôn Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `services/TechnicianService.cs` — GetAssignedServices(), UpdateServiceResult(), CompleteService() | Required | Week 2 |

**Pass Criteria:**

- ✓ TechnicianService.GetAssignedServices() returns only assigned services
- ✓ TechnicianService cannot update results for services not assigned
- ✓ Cannot access other technician's services via direct service method
- ✓ Service status updates properly recorded with timestamp
- ✓ Audit trail records who updated what service

---

### TC#5: Patient Self-Service Access (Service Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Test Timeline** | End of Week 3 |

**Phôn Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `services/PatientService.cs` — GetPatient(), GetMyMedicalRecords(), GetMyPrescriptions(), UpdatePatientInfo() | Required | Week 2 |

**Pass Criteria:**

- ✓ PatientService.GetPatient() returns patient's own information
- ✓ PatientService.GetMyMedicalRecords() returns only authenticated patient's records
- ✓ PatientService.GetMyPrescriptions() returns only patient's prescriptions
- ✓ PatientService.UpdatePatientInfo() updates only contact info (address, phone, email)
- ✓ Patient cannot update medical data (diagnosis, treatment, prescription)
- ✓ Row-level security enforced at database level (not just application)

---

### AUD#1: Standard Audit Configuration (Service Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 3: Audit & Monitoring |
| **Test Timeline** | End of Week 2 |

**Phôn Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `services/AuditService.cs` — LogUserAction(), GetAuditLogs(), LogSensitiveAccess() | Required | Week 2 |

**Pass Criteria:**

- ✓ AuditService.LogUserAction() inserts custom audit records
- ✓ AuditService.GetAuditLogs() retrieves audit trail with date range filtering
- ✓ Audit records include: UserId, ActionTime, Action, TableName, RecordId, Details, IPAddress
- ✓ Audit logs queryable by user, date range, and operation type
- ✓ Audit records immutable (cannot be modified after insertion)

---

## Related Tasks

- Task 04: Forms use these services
- Task 05: Security services validate operations
- Task 07-09: Database provides underlying tables

---
