# Task 03: Subsystem 2 - Medical Data Management UI Forms

**Assigned to:** Duyên  
**Type:** Front-end Implementation  
**Duration:** 25-30 hours  
**Priority:** High  
**Timeline:** Feb 18 - Feb 28, 2026

---

## Overview

Implement 7 role-specific Windows Forms for medical data management with automatic security filtering:

- Login form (authentication entry point)
- Coordinator interface (manage all patients/records)
- Doctor interface (see only assigned patients - VPD filtered)
- Technician interface (see only assigned services - VPD filtered)
- Patient interface (see own records only - row-level security)
- Notification viewer (OLS label-based filtering)

## Deliverables

| Form | Role | Purpose | Security |
|------|------|---------|----------|
| LoginForm.cs | All | User authentication, role determination | Basic auth |
| CoordinatorForm.cs | Coordinator | Patient/record management, role assignment | RBAC controlled |
| DoctorForm.cs | Doctor | Patient care, diagnoses, prescriptions | VPD filtered |
| TechnicianForm.cs | Technician | Service management, results | VPD filtered |
| PatientForm.cs | Patient | View own records/prescriptions | Row-level security |
| NotificationForm.cs | All | Role-specific notifications | OLS label filtered |

## Requirements

- Windows Forms (C#) with role-based interfaces
- Integrate with Phôn's security services (AuthenticationService, RBACService, VPDService, OLSService)
- Forms display data pre-filtered by database security policies
- Automatic role detection after login
- DataGrids for patient/record/service lists
- Read-only medical data (no patient editing of diagnoses)
- Professional UI with clear status displays

## Dependencies

- **Requires:** Ngọc, Vũ's database + security setup (Fri Feb 21)
- **Requires:** Phôn's security services (available Fri Feb 21)
- **Requires:** Phôn's business services for data access
- **Blocks:** System testing (Week 4)

## Success Criteria

✓ Login authenticates correctly and determines role  
✓ Each role sees only authorized data  
✓ VPD filtering works (doctors see only assigned patients)  
✓ OLS filtering works (notifications by label)  
✓ Row-level security enforced (patients see own records)  
✓ Medical data is read-only where appropriate  
✓ Forms handle all security mechanisms transparently  
✓ Professional appearance, intuitive workflow

## Critical: LoginForm

Entry point for entire system:

- Username/password input
- Validate credentials via AuthenticationService
- Return user role (Coordinator, Doctor, Technician, Patient)
- Open appropriate role-specific form
- Handle failed authentications gracefully
- Close login form after transition

## Security Implementation

All forms leverage Phôn security services:

- AuthenticationService.Login() for credential validation
- RBACService.CheckPermission() for action authorization
- VPDService queries return pre-filtered data
- OLSService filters notifications by user labels
- No additional filtering needed in forms (database enforces)

## Traceability Matrix

### TC#2: RBAC Configuration (LoginForm)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Test Timeline** | End of Week 2 |

**Duyên Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `Forms/LoginForm.cs` (redirect to role-specific forms) | Required | Week 3 |

**Pass Criteria:**

- ✓ LoginForm opens correct role-specific form (Coordinator/Doctor/Technician/Patient)

---

### TC#3: VPD Implementation (DoctorForm, TechnicianForm)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Test Timeline** | End of Week 2 |

**Duyên Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `Forms/DoctorForm.cs` — Display only assigned patients (VPD filtered) | Required | Week 3 |
| `Forms/TechnicianForm.cs` — Display only assigned services (VPD filtered) | Required | Week 3 |

**Pass Criteria:**

- ✓ DoctorForm displays only assigned patients in DataGrid (0 excluded patients visible)
- ✓ TechnicianForm displays only assigned services in DataGrid
- ✓ VPD filtering transparent to application (no changes to form code needed)

---

### TC#4: Technician Access

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Test Timeline** | End of Week 2-3 |

**Duyên Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `Forms/TechnicianForm.cs` — Display assigned services, update results, mark complete | Required | Week 3 |

**Pass Criteria:**

- ✓ TechnicianForm displays technician's assigned services only
- ✓ TechnicianForm cannot view/edit services assigned to other technicians
- ✓ TechnicianForm UpdateResults() validates technician has permission
- ✓ TechnicianForm CompleteService() marks service as complete

---

### TC#5: Patient Self-Service Access

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Test Timeline** | End of Week 3 |

**Duyên Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `Forms/LoginForm.cs` — Authenticate patients | Prerequisite | Week 3 |
| `Forms/PatientForm.cs` — Display own records, prescriptions, appointments; edit contact info | Required | Week 3 |

**Pass Criteria:**

- ✓ PatientForm displays authenticated patient's name, ID, contact info
- ✓ PatientForm displays patient's medical records in read-only DataGrid
- ✓ PatientForm displays patient's prescriptions in read-only DataGrid
- ✓ PatientForm displays patient's appointment history
- ✓ Patient cannot access other patient's records
- ✓ Contact info edit functionality works (saves to database)
- ✓ Medical data fields are read-only (cannot be modified)

---

### OLS#2: User Label Assignment (NotificationForm)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 2: OLS Notification System |
| **Test Timeline** | End of Week 3 |

**Duyên Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `Forms/NotificationForm.cs` — Display OLS-filtered notifications | Required | Week 3 |

**Pass Criteria:**

- ✓ NotificationForm displays only filtered notifications per user's labels
- ✓ Notification content shows title, content, and label information
- ✓ Label-based filtering transparent to application form (database enforced)

---

## Related Tasks

- Task 04: Security services (must complete first)
- Task 05: Business services (provide data queries)
- Task 06-08: Database setup (enables all security)

---
