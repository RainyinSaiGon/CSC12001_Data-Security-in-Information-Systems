# Task 02: Subsystem 1 - Business Logic Services

**Assigned to:** Duyên, Triết  
**Type:** Backend Services  
**Duration:** 25-30 hours  
**Priority:** Critical (blocks Task 01)  
**Timeline:** Feb 10 - Feb 21, 2026

---

## 1. Overview & Deliverables

Implement 6 foundational services for database user/role/permission management:

* **OracleConnectionService** — Connection pooling, lifecycle management, error handling (implement FIRST)
* **ValidationService** — Username/password/object validation with Oracle keyword checking
* **UserService** — User CRUD (CreateUser, ModifyUser, DeleteUser, ListUsers, GrantRole)
* **RoleService** — Role operations (CreateRole, DeleteRole, ListRoles, GetRolePrivileges)
* **PermissionService** — Grant/revoke with column-level security (SupportsColumnGrant, ValidateColumnPermission)
* **PrivilegeService** — Privilege queries (GetUserPrivileges, GetRolePrivileges, GetObjectPermissions)

## 2. Requirements & Dependencies

* Use Oracle.ManagedDataAccess.Core; parameterized queries to prevent SQL injection
* Exception handling, configuration-based connection strings, no hardcoded credentials
* Dependencies: Task 03 (Oracle catalog queries and admin SQL support), Task 01 (UI forms consume these)
* Models required: User.cs, Role.cs, Permission.cs, OracleObject.cs

## 3. Method Specifications

* **OracleConnectionService** — TestConnection(), GetConnection(), CloseConnection() with pooling support
* **ValidationService** — ValidateUsername (3-30 chars, alphanumeric+underscore, no keywords), ValidatePassword (8+ chars, mixed case, 1+ number), CheckObjectExists()
* **UserService** — CreateUser(), ModifyUser(), DeleteUser(), ListUsers(), GrantRole()
* **RoleService** — CreateRole(), DeleteRole(), ListRoles(), GetRolePrivileges()
* **PermissionService** — GrantPermission(), RevokePermission(), GrantColumnPermission() with validation that only SELECT/UPDATE allow column-level, SupportsColumnGrant(), ValidateColumnPermission(), GetObjectPermissions()
* **PrivilegeService** — GetUserPrivileges(), GetRolePrivileges(), GetObjectPermissions(), HasPrivilege()

---

## 4. Acceptance Criteria

* [ ] OracleConnectionService connects successfully with proper error handling
* [ ] ValidationService enforces username (3-30 chars, alphanumeric+underscore, no keywords) and password (8+ chars, mixed case, 1+ number) rules
* [ ] UserService successfully creates/modifies/deletes/lists users
* [ ] RoleService creates/deletes roles and retrieves privileges
* [ ] PermissionService validates column-level restrictions (SELECT/UPDATE only), rejects INSERT/DELETE on columns
* [ ] PrivilegeService queries user/role/object permissions correctly
* [ ] All queries parameterized; no hardcoded credentials
* [ ] Services integrate seamlessly with Task 01 UI forms
