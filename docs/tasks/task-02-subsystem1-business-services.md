# Task 02: Subsystem 1 - Business Logic Services

**Assigned to:** Duyên, Triết  
**Type:** Backend Services  
**Duration:** 25-30 hours  
**Priority:** Critical (blocks Task 01)  
**Timeline:** Feb 10 - Feb 21, 2026

---

## Overview

Implement 6 foundational services for database user/role/permission management:

- Connection pooling and management
- User operations (create, list, delete, modify)
- Role management
- Permission granting/revocation
- Privilege querying

## Deliverables

| Service | Purpose | Key Responsibility |
|---------|---------|-------------------|
| OracleConnectionService | Connection management | Connection pooling, lifecycle, error handling |
| ValidationService | Input validation | Username, password, object validation |
| UserService | User CRUD | Create/modify/delete users, grant roles |
| RoleService | Role operations | Create/delete roles, list privileges |
| PermissionService | Permission management | Grant/revoke, column-level security |
| PrivilegeService | Privilege queries | Query data dictionary for privilege info |

## Requirements

- Use Oracle.ManagedDataAccess.Core for database connectivity
- Parameterized queries (prevent SQL injection)
- Proper exception handling and logging
- No hardcoded credentials
- Connection string from configuration
- All services tested with sample data

## Dependencies

- **Requires:** Ngọc, Vũ's database setup (Fri Feb 14)
- **Blocks:** Task 01 (Duyên, Triết needs these services)
- **Models needed:** User.cs, Role.cs, Permission.cs, OracleObject.cs

## Success Criteria

✓ All 6 services implemented  
✓ OracleConnectionService provides working connections  
✓ All CRUD operations functional  
✓ SQL queries correct and efficient  
✓ Exception handling comprehensive  
✓ Services work with Duyên, Triết's forms  
✓ No hardcoded data or credentials

## Critical: OracleConnectionService

Must implement FIRST - all other services depend on it:

- TestConnection(): bool
- GetConnection(): OracleConnection
- CloseConnection(OracleConnection): void
- Connection pooling
- Error handling for connection failures

## Detailed Method Specifications

### OracleConnectionService.cs [IMPLEMENT FIRST]

- Constructor(string connectionString): Initialize with connection details
- TestConnection(): bool — Verify Oracle connection using ODP.NET
- GetConnection(): OracleConnection — Return new or pooled connection
- CloseConnection(OracleConnection): void — Properly close and dispose
- Tech: Oracle.ManagedDataAccess.Core, connection pooling, appsettings.json

### ValidationService.cs

- ValidateUsername(string): bool — 3-30 chars, alphanumeric + underscore, no reserved keywords
- ValidatePassword(string): bool — 8+ chars, mixed case, at least 1 number
- CheckObjectExists(string): bool — Query DBA_OBJECTS

### UserService.cs

- CreateUser(User): bool — CREATE USER, handle duplicates
- ModifyUser(User): bool — ALTER USER, handle not-found
- DeleteUser(string): bool — DROP USER, cascade
- ListUsers(): List\<User\> — Query DBA_USERS
- GrantRole(string, string): bool — GRANT role TO user

### RoleService.cs

- CreateRole(Role): bool — CREATE ROLE
- DeleteRole(string): bool — DROP ROLE, handle dependencies
- ListRoles(): List\<Role\> — Query DBA_ROLES
- GetRolePrivileges(string): List\<Permission\> — Query ROLE_TAB_PRIVS, ROLE_SYS_PRIVS

### PermissionService.cs

- GrantPermission(Permission): bool — GRANT ON TO [WITH GRANT OPTION]
- RevokePermission(Permission): bool — REVOKE
- GrantColumnPermission(string, string, List\<string\>, string): bool — Column-level GRANT
- GetObjectPermissions(string): List\<Permission\> — Query TABLE_PRIVS

### PrivilegeService.cs

- GetUserPrivileges(string): List\<Permission\> — USER_TAB_PRIVS + USER_SYS_PRIVS
- GetRolePrivileges(string): List\<Permission\> — ROLE_TAB_PRIVS + ROLE_SYS_PRIVS
- GetObjectPermissions(string): List\<Permission\> — ALL_TAB_PRIVS
- HasPrivilege(string, string, string): bool — Check single privilege

### Required Models

- User.cs, Role.cs, Permission.cs, OracleObject.cs

---

## Traceability Matrix

### TC#1: User Account Setup (Service Deliverables)

| Aspect | Details |
|--------|---------|
| **Related Requirement** | Req 1: Access Control & Interface |
| **Primary Owner** | Ngọc, Vũ (Database), **Duyên, Triết (Service)**, Duyên, Triết (Form) |
| **Test Timeline** | End of Week 2 |

**Duyên, Triết Deliverables:**

| Deliverable | Status | Completion Date |
|------------|--------|-----------------|
| `OracleConnectionService.cs` | Critical | Week 1 - Mid Week |
| `ValidationService.cs` — ValidateUsername, ValidatePassword | Required | Week 2 - Early |
| `UserService.cs` — CreateUser(), ListUsers(), DeleteUser(), ModifyUser() | Required | Week 2 - Early |

**Pass Criteria (Duyên, Triết):**

- ✓ OracleConnectionService successfully connects to database
- ✓ ValidationService validates username format (3-30 chars, alphanumeric + underscore, no reserved keywords)
- ✓ ValidationService validates password strength (8+ chars, mixed case, at least 1 number)
- ✓ UserService.CreateUser() creates user in NHANVIEN table
- ✓ UserService.ListUsers() retrieves all users with complete details
- ✓ UserService.ModifyUser() updates user properties
- ✓ UserService.DeleteUser() removes user from database
- ✓ No security vulnerabilities (no hardcoded credentials)

---

### TC#2: RBAC Configuration (Service Prerequisites)

| Deliverable | Status | Completion Date |
|------------|--------|-----------------|
| `Services/OracleConnectionService.cs` | Prerequisite | Week 1 |

---

## Related Tasks

- Task 01: Depends on these services for UI functionality
- Task 04: Security services use similar connection approach

---
