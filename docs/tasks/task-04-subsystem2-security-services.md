# Task 04: Subsystem 2 - Security Services Implementation

**Assigned to:** Person 4  
**Type:** Backend Security Services  
**Duration:** 30-35 hours  
**Priority:** Critical (blocks Task 03)  
**Timeline:** Feb 19 - Feb 28, 2026

---

## Overview

Implement 6 critical security services implementing all access control mechanisms:

- User authentication with role determination
- Role-based access control (RBAC) with action verification
- Virtual Private Database (VPD) for row-level filtering
- Oracle Label Security (OLS) for label-based access
- Validation services for data integrity

## Deliverables

| Service | Purpose | Security Mechanism |
|---------|---------|-------------------|
| AuthenticationService | User login and role detection | Basic auth → role assignment |
| OracleConnectionService | Connection management | Secure connections |
| RBACService | Action authorization | Role → Available actions |
| VPDService | Row-level security | Transparent filtering in DB |
| OLSService | Label-based access | 3-level label hierarchy |
| ValidationService | Input validation | Data integrity |

## Requirements

- Implement 4 distinct roles: Coordinator, Doctor, Technician, Patient
- Support RBAC with role-action mapping
- Implement VPD policies (transparent to forms)
- Support OLS with 3-level hierarchy:
  - Level 1: Department (Cardiology, Gastroenterology, Neurology)
  - Level 2: Location (Ho Chi Minh, Hai Phong, Ha Noi)
  - Level 3: Classification (Staff, DepartmentHead, Director)
- All security enforced at database level
- Proper error handling and logging

## Dependencies

- **Requires:** Person 5's database users and security setup (Wed Feb 19 for RBAC, Thu Feb 20 for VPD/OLS)
- **Blocks:** Task 03 (forms need these services)
- **Uses:** OracleConnectionService from this or Person 2

## Success Criteria

✓ AuthenticationService authenticates all 4 user types correctly  
✓ RBAC prevents unauthorized actions  
✓ VPD filtering works at database level  
✓ OLS label filtering enforces label hierarchy  
✓ All services work transparently with forms  
✓ Comprehensive error handling  
✓ Security enforced (database level, not app level)

## Role Definitions

**Coordinator** (20 staff):

- Available Actions: ViewAllPatients, AddPatient, EditPatient, DeletePatient, CreateMedicalRecord, AssignDoctor, AssignTechnician, ViewAllRecords, ViewAllStaff

**Doctor/Nurse** (100 staff):

- Available Actions: ViewAssignedPatients, ViewOwnPatientHistory, CreateDiagnosis, UpdateTreatment, CreatePrescription, UpdatePrescription, OrderDiagnosticService, ViewPatientAllergies

**Technician** (50 staff):

- Available Actions: ViewAssignedServices, UpdateServiceResults, MarkServiceComplete, ViewRelatedPatientInfo

**Patient** (100,000+):

- Available Actions: ViewOwnRecords, ViewOwnPrescriptions, ViewAppointmentHistory, UpdateOwnContactInfo

## Critical Implementation Order

1. **OracleConnectionService** (first - all others depend)
2. **AuthenticationService** (immediately after)
3. **RBACService** (after auth works)
4. **VPDService** (depends on DB setup)
5. **OLSService** (depends on DB setup)
6. **ValidationService** (independant, can be last)

## VPD Setup

Policies must be configured at database level by Person 5:

- HSBA policy (medical records filtering)
- HSBA_DV policy (diagnostic service filtering)
- Service methods query pre-filtered data

## OLS 3-Level Hierarchy

Label examples:

- "Cardiology:HoChiMinh:Staff" - Staff level
- "Cardiology:HoChiMinh:DepartmentHead" - Department head level
- "Cardiology:*:Director" - Director (all locations)

Users compare labels hierarchically - must meet all 3 dimensions.

## Traceability Matrix

### TC#2: RBAC Configuration (Service Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Test Timeline** | End of Week 2 |

**Person 4 Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `Services/OracleConnectionService.cs` | Prerequisite | Week 1 |
| `Services/AuthenticationService.cs` — Login(), ValidateUserRole() | Critical | Week 2 - Early |
| `Services/RBACService.cs` — CheckUserRole(), CheckPermission(), GetAvailableActions() | Required | Week 2 |

**Pass Criteria:**

- ✓ AuthenticationService.Login() returns correct role for valid credentials
- ✓ AuthenticationService.ValidateUserRole() correctly verifies role assignments
- ✓ RBACService.CheckUserRole() returns user's role from database
- ✓ RBACService.CheckPermission() verifies user has action permission (whitelist check)
- ✓ RBACService.GetAvailableActions() returns complete list for user's role
- ✓ Coordinator can perform coordinator actions, not doctor actions
- ✓ Doctor can perform doctor actions, not technician actions
- ✓ Technician cannot access coordinator or doctor functions
- ✓ Patient cannot access staff functions
- ✓ All RBAC checks complete in < 100ms

---

### TC#3: VPD Implementation (Service Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Test Timeline** | End of Week 2 |

**Person 4 Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `Services/OracleConnectionService.cs` | Prerequisite | Week 1 |
| `Services/AuthenticationService.cs` | Prerequisite | Week 2 |
| `Services/VPDService.cs` — GetVisiblePatients(), GetVisibleRecords(), GetVisibleServices() | Required | Week 2 |
| `Services/RBACService.cs` | Prerequisite | Week 2 |

**Pass Criteria:**

- ✓ VPDService.GetVisiblePatients() returns only doctor's assigned patients
- ✓ VPDService.GetVisibleRecords() returns only staff's authorized records
- ✓ VPDService.GetVisibleServices() returns only technician's assigned services
- ✓ Doctor cannot access patient records via direct SQL query (VPD enforced)
- ✓ VPD filtering transparent to application (no changes to form code needed)
- ✓ VPD overhead < 10% performance impact

---

### OLS#2: User Label Assignment (Service Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 2: OLS Notification System |
| **Test Timeline** | End of Week 3 |

**Person 4 Deliverables:**

| Deliverable | Status | Completion Date |
|-------------|--------|-----------------|
| `Services/OracleConnectionService.cs` | Prerequisite | Week 1 |
| `Services/AuthenticationService.cs` | Prerequisite | Week 2 |
| `Services/OLSService.cs` — GetUserLabels(), CanAccessNotification(), GetAccessibleNotifications() | Required | Week 2 |

**Pass Criteria:**

- ✓ OLSService.GetUserLabels() retrieves correct labels for each user
- ✓ OLSService.CanAccessNotification() verifies label compatibility correctly
- ✓ Director can access notifications at any label level (all 15 notifications)
- ✓ Department Head can access own department notifications + lower classifications
- ✓ Staff can access only notifications matching their exact labels
- ✓ OLSService.GetAccessibleNotifications() returns only accessible notification IDs

---

## Related Tasks

- Task 03: Depends on these services
- Task 05: Complements business services
- Task 06-08: Database must provide VPD/OLS infrastructure

---


