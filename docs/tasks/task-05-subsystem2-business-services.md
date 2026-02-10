# Task 05: Subsystem 2 - Medical Business Logic Services

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
- Support row-level security in PatientService
- VPD filtering transparent through queries

## Dependencies

- **Requires:** Ngọc, Vũ's database tables (Fri Feb 14)
- **Requires:** Task 04's security services for validation
- **Unblocks:** Task 03 (forms need these services)

## Success Criteria

✓ All 5 services fully implemented  
✓ Data queries return correct results  
✓ Row-level security enforced in PatientService  
✓ Audit logging captures all sensitive operations  
✓ Services work with Task 04 security services  
✓ No hardcoded data or connection strings  
✓ Comprehensive error handling

## Service Details

### PatientService

- GetPatient(patientId): Patient
- UpdatePatientInfo(patient): bool
- GetMyMedicalRecords(patientId): List<MedicalRecord>
- GetMyPrescriptions(patientId): List<Prescription>

### DoctorService

- GetAssignedPatients(doctorId): List<Patient>
- CreateDiagnosis(record): bool
- UpdatePrescription(prescription): bool
- OrderDiagnosticService(service): bool

### CoordinatorService

- GetAllPatients(): List<Patient>
- AddPatient(patient): bool
- EditPatient(patient): bool
- AssignDoctorToPatient(doctorId, patientId): bool
- AssignTechnicianToService(technicianId, serviceId): bool
- GetRecordStatus(recordId): string

### TechnicianService

- GetAssignedServices(technicianId): List<DiagnosticService>
- UpdateServiceResult(serviceId, result): bool
- CompleteService(serviceId): bool

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
| `Services/TechnicianService.cs` — GetAssignedServices(), UpdateServiceResult(), CompleteService() | Required | Week 2 |

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
| `Services/PatientService.cs` — GetPatient(), GetMyMedicalRecords(), GetMyPrescriptions(), UpdatePatientInfo() | Required | Week 2 |

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
| `Services/AuditService.cs` — LogUserAction(), GetAuditLogs(), LogSensitiveAccess() | Required | Week 2 |

**Pass Criteria:**

- ✓ AuditService.LogUserAction() inserts custom audit records
- ✓ AuditService.GetAuditLogs() retrieves audit trail with date range filtering
- ✓ Audit records include: UserId, ActionTime, Action, TableName, RecordId, Details, IPAddress
- ✓ Audit logs queryable by user, date range, and operation type
- ✓ Audit records immutable (cannot be modified after insertion)

---

## Related Tasks

- Task 03: Forms use these services
- Task 04: Security services validate operations
- Task 06-08: Database provides underlying tables

---
