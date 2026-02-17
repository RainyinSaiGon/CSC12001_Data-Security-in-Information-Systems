# Task 01: Subsystem 1 - Database Admin UI Forms

**Assigned to:** Duyên, Triết  
**Type:** Front-end Implementation  
**Duration:** 20-25 hours  
**Priority:** High  
**Timeline:** Feb 18 - Feb 28, 2026

---

## 1. Objective

Implement 5 Windows Forms for Oracle database administration:

* **MainForm** — Navigation hub with menu bar, status bar, button routing to all forms
* **UserManagementForm** — User CRUD (create/edit/delete), search, grid display
* **RoleManagementForm** — Role creation/deletion, view privileges
* **PermissionForm** — RBAC permission grants with column-level security support
* **PrivilegeViewerForm** — Privilege reports with filtering and CSV export

## 2. Deliverables & Requirements

* Professional Windows Forms UI (C#) with role-based navigation
* Integrate with OracleConnectionService, ValidationService, UserService, RoleService, PermissionService, PrivilegeService
* Form-level input validation with DataGrid sorting/filtering
* Clear error handling and user feedback with connection status display
* Dependencies: Task 03 (database schema), Task 02 (services)

## 3. Form Specifications

* **MainForm** — MenuStrip (File/Tools/Help), navigation buttons, status bar (connection/user/timestamp)
* **UserManagementForm** — Username/password inputs, CRUD buttons, DataGrid (Username, CreatedDate, Status), search filter
* **RoleManagementForm** — Role name/description inputs, create/delete buttons, DataGrid (RoleName, Description, CreatedDate)
* **PermissionForm** — User/Role/Object selection, permission checkboxes (SELECT/INSERT/UPDATE/DELETE), column-level security for SELECT/UPDATE only, grant option toggle
  * Column permissions disabled for INSERT/DELETE and Procedure/Function objects
  * Permission Matrix: Table (all 4 perms + columns), View (SELECT only + columns), Procedure/Function (EXECUTE only)
* **PrivilegeViewerForm** — User/Role selection, DataGrid (Object, Type, Permission, Grant Option), System/Object privilege filter, export to CSV

---

## 4. Acceptance Criteria

* [ ] All 5 forms fully functional with no unhandled exceptions
* [ ] UserManagementForm displays/filters users; validation prevents invalid data
* [ ] RoleManagementForm creates/deletes roles with grid updates
* [ ] PermissionForm grants/revokes with column-level restrictions enforced
* [ ] PrivilegeViewerForm displays privileges with filtering and export
* [ ] MainForm enables/disables buttons based on user role
* [ ] Professional UI appearance; all grids refresh properly