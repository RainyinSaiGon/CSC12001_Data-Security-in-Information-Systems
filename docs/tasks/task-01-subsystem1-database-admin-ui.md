# Task 01: Subsystem 1 - Database Admin UI Forms

**Assigned to:** Duyên, Triết  
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
- Integrate with Duyên, Triết's business services (UserService, RoleService, etc.)
- Form-level input validation
- DataGrid display with sorting/filtering
- Clear error handling and user feedback
- Status bar showing connection info

## Dependencies

- **Requires:** Ngọc, Vũ's database completion (Fri Feb 14)
- **Requires:** Duyên, Triết's services (available Wed Feb 19)
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

- ComboBox: User/Role selection, Object selection, **Object Type selection** (Table, View, Procedure, Function)
- CheckBox: SELECT, INSERT, UPDATE, DELETE, WITH GRANT OPTION
  - **IMPORTANT:** Column-level permissions (below) only available for SELECT and UPDATE
  - Column list DISABLED for INSERT and DELETE (Oracle limitation)
- Buttons: Grant, Revoke, Clear, Refresh
- DataGrid: Existing permissions (grantee, object, type, columns, grant option)
- TextBox: Column list for column-level security
  - **Restrictions:** Only enabled when SELECT or UPDATE is checked
  - Must be empty/disabled for INSERT, DELETE, or Procedure/Function objects

**Permission Matrix by Object Type:**

| Object Type | SELECT | INSERT | UPDATE | DELETE | Grantable to Cols? |
|-------------|--------|--------|--------|--------|--------------------|
| Table | ✓ | ✓ | ✓ | ✓ | SELECT/UPDATE only |
| View | ✓ | ✗ | ✗ | ✗ | SELECT only |
| Procedure | EXECUTE | - | - | - | No |
| Function | EXECUTE | - | - | - | No |

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
| **Primary Owner** | Ngọc, Vũ (Database), Duyên, Triết (Service), **Duyên, Triết (Form)** |
| **Test Timeline** | End of Week 2 (after database and services ready) |

**Duyên, Triết Deliverables:**

| Deliverable | Status | Completion Date |
|------------|--------|-----------------|
| `forms/UserManagementForm.cs` | Required | Week 3 |

**Pass Criteria (Duyên, Triết):**

- ✓ UserManagementForm displays all users in DataGrid
- ✓ UserManagementForm input validation prevents invalid data
- ✓ UserManagementForm shows error messages for failed operations
- ✓ Performance acceptable (user list loads within 2 seconds)

---

### TC#2: RBAC Configuration (Form Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | Ngọc, Vũ (Database), Phôn (Service), Duyên & **Duyên, Triết** (Forms) |
| **Test Timeline** | End of Week 2 (after RBAC setup) |

**Duyên, Triết Deliverables:**

| Deliverable | Status | Completion Date |
|------------|--------|-----------------|
| `forms/MainForm.cs` (menu/button enablement based on role) | Required | Week 3 |

**Pass Criteria (Duyên, Triết):**

- ✓ MainForm enables/disables buttons based on user's role

---

## Related Tasks

- Task 02: Duyên, Triết implements the services used by these forms
- Task 03-05: Medical UI forms (similar architecture for Subsystem 2)

---
