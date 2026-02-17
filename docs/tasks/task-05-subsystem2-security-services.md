# Task 05: Subsystem 2 - Security Services Implementation

**Assigned to:** Phôn
**Type:** Backend Security Services
**Duration:** 30-35 hours
**Priority:** Critical (Blocks Task 04)
**Timeline:** Feb 19 - Feb 28, 2026

---

## 1. Objective

Develop the centralized security layer for Subsystem 2, acting as the bridge between the application UI (Forms) and the Oracle Database security mechanisms. This task involves implementing 6 core services that handle authentication, connection management, and the enforcement of RBAC, VPD, and OLS policies.

## 2. Scope of Work

* **OracleConnectionService:** Manage secure, pooled connections with transaction support
* **AuthenticationService:** Handle login, password verification, and session initialization with role detection (Coordinator, Doctor, Technician, Patient)
* **ValidationService:** Sanitize inputs and enforce data integrity before database interaction
* **RBACService:** Verify user authorization for role-specific actions (Technician, Patient roles)
* **VPDService:** Wrapper for VPD-enabled queries ensuring transparent filtering for Doctor/Coordinator roles
* **OLSService:** Compare user labels against notification (`THONGBAO`) labels using 3-component hierarchy (Levels: Director > Dept Head > Staff; Compartments: Cardiology, Gastroenterology, Neurology; Groups: Ho Chi Minh, Hai Phong, Ha Noi)



## 3. Deliverables

* `OracleConnectionService.cs` — Connection pooling and transaction management
* `AuthenticationService.cs` — Login, logout, role detection via `GetCurrentUserRole()`
* `RBACService.cs` — Authorization verification with `CheckPermission()`, `GetAvailableActions()`
* `VPDService.cs` — VPD context validation and exception handling
* `OLSService.cs` — Label comparison via `GetUserLabels()`, `CanAccessNotification()`
* `ValidationService.cs` — Input sanitization and data integrity enforcement

## 4. Acceptance Criteria

* [ ] **Authentication:** System correctly identifies all 4 user roles upon login.
* [ ] **RBAC Enforcement:** Technicians are blocked from accessing Doctor-specific features.
* [ ] **VPD Transparency:** Doctor forms display *only* assigned patients; Coordinators see the full list (or Dept list).
* [ ] **OLS Logic:** The `OLSService` correctly determines that a "Cardiology Staff" cannot view a "Director" level notification.
* [ ] **Integration:** All services function seamlessly with the UI Forms (Task 04).
