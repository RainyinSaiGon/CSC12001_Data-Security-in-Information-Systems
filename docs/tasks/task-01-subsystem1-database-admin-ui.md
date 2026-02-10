# Task 01: Subsystem 1 - Database Admin UI Forms

**Assigned to:** Person 1  
**Type:** Front-end Implementation  
**Duration:** 20-25 hours  
**Priority:** High  
**Timeline:** Feb 18 - Feb 28, 2026

---

## Overview

Implement 5 Windows Forms for Oracle database administration:

- Main application window with navigation
- User management (CRUD operations)
- Role management
- Permission granting and management
- Privilege viewer and reporting

## Deliverables

| Form | Purpose | Key Features |
|------|---------|--------------|
| MainForm.cs | Navigation hub | Menu bar, status bar, buttons to all forms |
| UserManagementForm.cs | User CRUD | Create/Edit/Delete users, search, grid display |
| RoleManagementForm.cs | Role management | Create/Delete roles, view privileges |
| PermissionForm.cs | Permission grants | RBAC, column-level security, grant options |
| PrivilegeViewerForm.cs | Privilege reports | Display privileges, filter, export CSV |

## Requirements

- Professional Windows Forms UI (C#)
- Integrate with Person 2's business services (UserService, RoleService, etc.)
- Form-level input validation
- DataGrid display with sorting/filtering
- Clear error handling and user feedback
- Status bar showing connection info

## Dependencies

- **Requires:** Person 5's database completion (Fri Feb 14)
- **Requires:** Person 2's services (available Wed Feb 19)
- **Ready to start:** With UI design and framework

## Success Criteria

✓ All 5 forms fully functional  
✓ CRUD operations work correctly  
✓ Data displays in grids properly  
✓ Services integrate seamlessly  
✓ No unhandled exceptions  
✓ Professional UI appearance  
✓ All validations working

## Detailed Form Specifications

### MainForm.cs [PRIMARY FORM]

- MenuStrip: File, Tools, Help menus
- Buttons: User Management, Role Management, Permission Management, View Privileges
- StatusStrip: Connection status, current user display, timestamp
- Implement proper window closing and application exit procedures

### UserManagementForm.cs

- TextBox: Username input, Password input (masked)
- Buttons: Create, Update, Delete, Clear, Refresh
- DataGrid: User list (Username, CreatedDate, Status)
- SearchBox: Username filter
- Form-level validation before database operations

### RoleManagementForm.cs

- TextBox: Role name, Description input
- Buttons: Create, Delete, Clear, Refresh
- DataGrid: Role list (RoleName, Description, CreatedDate)
- SearchBox: Role name filter

### PermissionForm.cs

- ComboBox: User/Role selection, Object selection
- CheckBox: SELECT, INSERT, UPDATE, DELETE, WITH GRANT OPTION
- Buttons: Grant, Revoke, Clear, Refresh
- DataGrid: Existing permissions (grantee, object, type, columns, grant option)
- TextBox: Column list for column-level security

### PrivilegeViewerForm.cs

- ComboBox: User or Role selection
- DataGrid: Privilege list (Object Name, Type, Permission, Grant Option, Date)
- RadioButtons: System Privileges vs Object Privileges filter
- Buttons: Refresh, Export, Filter

### Required Services Integration

- UserService.CreateUser(), ListUsers(), DeleteUser(), ModifyUser()
- RoleService.CreateRole(), ListRoles(), DeleteRole()
- PermissionService.GrantPermission(), RevokePermission(), GrantColumnPermission()
- PrivilegeService.GetUserPrivileges(), GetRolePrivileges(), GetObjectPermissions()

---

## Traceability Matrix

### TC#1: User Account Setup (Form Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | Person 5 (Database), Person 2 (Service), **Person 1 (Form)** |
| **Test Timeline** | End of Week 2 (after database and services ready) |

**Person 1 Deliverables:**

| Deliverable | Status | Completion Date |
|------------|--------|-----------------|
| `Forms/UserManagementForm.cs` | Required | Week 3 |

**Pass Criteria (Person 1):**

- ✓ UserManagementForm displays all users in DataGrid
- ✓ UserManagementForm input validation prevents invalid data
- ✓ UserManagementForm shows error messages for failed operations
- ✓ Performance acceptable (user list loads within 2 seconds)

---

### TC#2: RBAC Configuration (Form Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | Person 5 (Database), Person 4 (Service), Person 3 & **Person 1** (Forms) |
| **Test Timeline** | End of Week 2 (after RBAC setup) |

**Person 1 Deliverables:**

| Deliverable | Status | Completion Date |
|------------|--------|-----------------|
| `Forms/MainForm.cs` (menu/button enablement based on role) | Required | Week 3 |

**Pass Criteria (Person 1):**

- ✓ MainForm enables/disables buttons based on user's role

---

## Related Tasks

- Task 02: Person 2 implements the services used by these forms
- Task 03-05: Medical UI forms (similar architecture for Subsystem 2)

---
