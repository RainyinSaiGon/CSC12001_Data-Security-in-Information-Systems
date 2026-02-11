# Task 03: Subsystem 1 - Admin Database Schema & Data Dictionary

**Assigned to:** Duyên, Triết  
**Type:** Database Design & Administration  
**Duration:** 10 hours  
**Priority:** High (required for OracleDBAdmin application)  
**Timeline:** Feb 12 - Feb 20, 2026

---

## Overview

Design and create the admin database schema for Subsystem 1 (OracleDBAdmin). This database stores:
1. **Administrative User & Role Management** - Track admin users, roles, and role-to-user assignments
2. **Permission Tracking** - Record permissions granted on all database objects (tables, views, stored procedures, functions)
3. **Database Object Catalog** - Catalog of database objects from both Subsystem 1 and Subsystem 2 that can be managed
4. **Permission Grant Options** - Track WITH GRANT OPTION for delegated administration
5. **Audit Trail** - Complete audit log of all administrative operations

## Requirements Context (Vietnamese Specification)

Per Subsystem 1 specification:
- Manage users and roles
- Grant/revoke permissions WITH GRANT OPTION support
- **Support permissions on: TABLE, VIEW, STORED PROCEDURE, FUNCTION**
- **SELECT, UPDATE**: Can grant at column level
- **INSERT, DELETE**: Cannot grant at column level
- View permissions for each user/role on database objects

## Deliverables

### Part 1: Admin Tables (Management Infrastructure)

#### 01_CreateAdminTables.sql

Create tables for admin user and role management:

| Table | Purpose | Key Columns |
|-------|---------|-------------|
| ADMIN_USERS | Admin user accounts | ADMIN_USER_ID (PK), TENTAIKHOAN (unique), MATKHAU (hashed), HOTEN, EMAIL, SODT, ACTIVE |
| ADMIN_ROLES | Admin role definitions | ADMIN_ROLE_ID (PK), TENROLEVAITRO, MOTA, CREATED_DATE |
| ADMIN_USER_ROLES | User-Role assignment | (ADMIN_USER_ID, ADMIN_ROLE_ID) - PK, ASSIGNED_DATE, ASSIGNED_BY |
| ADMIN_OPERATION_AUDIT | Operation audit trail | AUDIT_ID (PK), ADMIN_USER_ID (FK), OPERATION_TYPE, OBJECT_TYPE, OBJECT_NAME, TIMESTAMP, SOURCE_IP, SUCCESS, DETAILS |

### Part 2: Database Object Catalog & Permission Tracking

#### 02_CreateDatabaseObjectTables.sql

Create tables to catalog and track permissions on database objects:

| Table | Purpose | Key Columns |
|-------|---------|-------------|
| DB_OBJECTS | Catalog all database objects | OBJECT_ID (PK), OBJECT_NAME, OBJECT_TYPE (TABLE\|VIEW\|PROCEDURE\|FUNCTION), OBJECT_OWNER, CREATED_DATE, description |
| DB_OBJECT_COLUMNS | Column catalog (for column-level grants) | COLUMN_ID (PK), OBJECT_ID (FK), COLUMN_NAME, DATA_TYPE, NULLABLE |
| PERMISSIONS_CATALOG | Available permissions per object type | PERMISSION_ID (PK), PERMISSION_NAME (SELECT\|INSERT\|UPDATE\|DELETE\|EXECUTE), APPLICABLE_TO (comma-separated object types) |
| PERMISSION_ASSIGNMENTS | Track all granted permissions | GRANT_ID (PK), USER_OR_ROLE_NAME, USER_OR_ROLE_TYPE (USER\|ROLE), OBJECT_ID (FK), PERMISSION_ID (FK), COLUMN_ID (FK - NULL for object-level), WITH_GRANT_OPTION (Y/N), GRANTED_BY, GRANTED_DATE |
| GRANTEE_HIERARCHY | Track permission delegation chains | HIERARCHY_ID (PK), ORIGINAL_GRANTEE, DELEGATED_TO, PERMISSION_GRANT_ID (FK), DELEGATION_DEPTH, CREATED_DATE |

### Part 3: Views for OracleDBAdmin Application

#### 03_CreateAdminViews.sql

Create views to support the OracleDBAdmin WinForm forms:

| View | Purpose |
|------|---------|
| V_USER_PERMISSIONS | Shows all permissions assigned to a specific user across all objects |
| V_ROLE_PERMISSIONS | Shows all permissions granted to a specific role |
| V_OBJECT_PERMISSIONS | Shows who has permissions on a specific database object |
| V_USER_GRANT_OPTIONS | Identifies which users/roles can grant permissions (have WITH GRANT OPTION) |
| V_PERMISSION_AUDIT_TRAIL | Detailed audit trail with permission change history |
| V_COLUMN_LEVEL_PERMISSIONS | Identifies column-level SELECT and UPDATE grants |

### Part 4: Stored Procedures for Permission Management Operations

#### 04_CreateAdminStoredProcedures.sql

Create procedures to implement Subsystem 1 requirements:

**User Management:**
- `SP_CREATE_USER(username, password, fullname, email, phone, active)` - Create admin user
- `SP_MODIFY_USER(user_id, fullname, email, phone, active)` - Modify user details
- `SP_DELETE_USER(user_id, created_by)` - Delete user with audit

**Role Management:**
- `SP_CREATE_ROLE(role_name, description, created_by)` - Create admin role
- `SP_DELETE_ROLE(role_id, created_by)` - Delete admin role
- `SP_ASSIGN_ROLE_TO_USER(user_id, role_id, assigned_by)` - Assign role to user

**Permission Grant/Revoke:**
- `SP_GRANT_PERMISSION(grantee, grantee_type, object_id, permission_id, column_id, with_grant_option, granted_by)` - Grant permission (table/view/procedure/function)
- `SP_REVOKE_PERMISSION(grantee, grantee_type, object_id, permission_id, column_id, granted_by)` - Revoke permission
- `SP_VALIDATE_COLUMN_LEVEL_PERMISSION(permission_id, object_type)` - Validate column-level grant rules

**Permission Tracking:**
- `SP_GET_USER_PERMISSIONS(user_id)` - Get all permissions for a user
- `SP_GET_ROLE_PERMISSIONS(role_id)` - Get all permissions for a role
- `SP_GET_OBJECT_GRANTEES(object_id)` - Get all users/roles with permissions on object
- `SP_CHECK_GRANT_OPTION(grantee, object_id, permission_id)` - Check if grantee can grant this permission

**Audit & Tracking:**
- `SP_LOG_ADMIN_OPERATION(user_id, operation_type, object_type, object_name, success, details, error_msg)` - Log admin operation
- `SP_GET_PERMISSION_AUDIT_TRAIL(filter_criteria, start_date, end_date)` - Query audit logs

### Part 5: Functions for Permission Logic

#### 05_CreateAdminFunctions.sql

Create functions for permission checking:

| Function | Purpose |
|----------|---------|
| F_CAN_GRANT_COLUMN_LEVEL(permission_type) | Returns 1 if permission can be granted at column level (SELECT, UPDATE), 0 otherwise |
| F_CAN_GRANT_OBJECT_LEVEL(permission_type, object_type) | Returns 1 if permission is valid for object type |
| F_HAS_PERMISSION(grantee, object_id, permission_id, column_id) | Returns 1 if grantee has permission |
| F_HAS_GRANT_OPTION(grantee, object_id, permission_id) | Returns 1 if grantee can grant this permission to others |
| F_GET_PERMISSION_NAME(permission_id) | Return permission name |
| F_GET_OBJECT_NAME_BY_ID(object_id) | Return object name |

### Part 6: Indexes for Performance

#### 06_CreateAdminIndexes.sql

| Table | Columns | Purpose |
|-------|---------|---------|
| ADMIN_USERS | TENTAIKHOAN | Login lookup |
| ADMIN_USERS | ACTIVE | Query active users |
| ADMIN_ROLES | TENROLEVAITRO | Role lookup |
| DB_OBJECTS | OBJECT_NAME, OBJECT_TYPE | Object search |
| PERMISSION_ASSIGNMENTS | (USER_OR_ROLE_NAME, OBJECT_ID) | Permission lookup |
| PERMISSION_ASSIGNMENTS | ADMIN_USER_ID | Audit by user |
| ADMIN_OPERATION_AUDIT | TIMESTAMP | Audit by date |
| DB_OBJECT_COLUMNS | OBJECT_ID | Column lookup |

### Part 7: Sample Data

#### 07_InsertSampleData.sql

Create initial data:

**Database Objects Catalog:**
- Sample tables, views, stored procedures, functions from both Subsystem 1 and Subsystem 2
- Include Subsystem 2 medical database objects (BENHNHAN, NHANVIEN, HSBA, etc.)

**Admin Users (4 users):**
- 1 DBA with full permissions
- 1 Security Admin (manage roles & permissions)
- 1 User Admin (create/modify users, manage role assignment)
- 1 Regular Admin (example user)

**Admin Roles (4 roles):**
- DBA: All permissions
- Security Admin: CREATE_ROLE, DROP_ROLE, GRANT_PERMISSION, REVOKE_PERMISSION
- User Admin: CREATE_USER, DROP_USER, MODIFY_USER, ASSIGN_ROLE, REVOKE_ROLE
- Audit Admin: VIEW_AUDIT, VIEW_PERMISSIONS

**Permission Examples:**
- Sample permissions on Subsystem 2 tables (BENHNHAN, NHANVIEN, HSBA, etc.)
- Include column-level permissions (SELECT, UPDATE on specific columns like CCCD, HOTEN)
- Include object-level permissions (INSERT, DELETE on tables)
- Include stored procedure and function execute permissions

## Requirements & Constraints

**Support all database object types:** TABLE, VIEW, STORED PROCEDURE, FUNCTION  
**Column-level grants:** SELECT and UPDATE can be granted at column level  
**Object-level grants:** INSERT and DELETE at object level only  
**WITH GRANT OPTION:** Track and enforce delegation of permissions  
**Permission verification:** Views and procedures validate permission rules  
**Audit trail:** All admin operations logged with timestamps and user IDs  
**Support Oracle native objects:** Catalog real Subsystem 2 database objects  
**Prevent invalid grants:** Stored procedures validate permission assignment rules  

## Test Validation (Task 02 & UI Integration)

Will verify:
1. All tables, views, procedures, functions created
2. Permission rules enforced (column-level for SELECT/UPDATE, object-level for INSERT/DELETE)
3. WITH GRANT OPTION tracked and enforced
4. Audit trail captures all operations
5. Sample data represents realistic permission scenarios
6. OracleDBAdmin application can:
   - View users and roles
   - Grant permissions with column-level options
   - Revoke permissions
   - Query permission assignments
   - Audit all administrative actions

## Related Files

- Subsystem 1 UI: [task-01-subsystem1-database-admin-ui.md](task-01-subsystem1-database-admin-ui.md)
- Subsystem 1 Services: [task-02-subsystem1-business-services.md](task-02-subsystem1-business-services.md)
- Architecture: See [ARCHITECTURE.md](../ARCHITECTURE.md#subsystem-1-admin-database-erd)
- Database Setup: Will be implemented by Ngọc, Vũ in separate SQL creation task

## Completion Criteria

Database schema design complete (7 artifact files planned)  
All tables, views, procedures, functions documented  
Permission logic defined and validated  
Sample data representing all scenarios prepared  
Audit trail infrastructure established  
Ready for SQL script implementation (Database team)  
OracleDBAdmin application ready to use database (UI development can proceed)- Security Admin: Create/manage roles and permissions
- User Admin: Create/modify/delete users, assign roles
- Audit Admin: Read-only audit logs

**Permissions (15+ permissions):**
- USER_MGT: CREATE_USER, DROP_USER, MODIFY_USER, RESET_PASSWORD
- ROLE_MGT: CREATE_ROLE, DROP_ROLE, MODIFY_ROLE, ASSIGN_ROLE, REVOKE_ROLE
- PERMISSION_MGT: GRANT_PERMISSION, REVOKE_PERMISSION, CREATE_PERMISSION, DROP_PERMISSION
- AUDIT_VIEW: VIEW_AUDIT, EXPORT_AUDIT, DELETE_AUDIT

**Role-Permission Assignments:**
- DBA: All permissions
- Security Admin: All ROLE_MGT + PERMISSION_MGT
- User Admin: All USER_MGT + ASSIGN_ROLE
- Audit Admin: VIEW_AUDIT only

**Initial Audit Log:**
- Log of admin users and roles creation

## Requirements & Constraints

- All table names in UPPERCASE (ADMIN_USERS, ADMIN_ROLES, etc.)
- All column names in UPPERCASE
- English naming convention for admin database (separate from Vietnamese medical data)
- Support hashed passwords (no plaintext storage)
- WITH GRANT_OPTION support for delegated administration
- Audit trail must be immutable (logging use only, no UPDATE/DELETE on audit records in normal operations)
- Foreign key constraints with CASCADE delete where appropriate

## Test Validation (Task 4 - Security Setup)

Will verify:
1. All 6 tables created successfully
2. Primary and foreign key constraints working
3. Check constraints enforcing QUYHAN and LOAIQUYEN values
4. Indexes created and performing efficiently
5. Sample data inserted correctly (10 users, 4 roles, 15+ permissions)

## Related Files

- Schema scripts: `Database/Subsystem1-AdminDB/schema/`
- Security setup: `Database/Subsystem1-AdminDB/security/`
- Audit setup: `Database/Subsystem1-AdminDB/audit/`
- Architecture: See [ARCHITECTURE.md](../ARCHITECTURE.md#subsystem-1-admin-database-erd)

## Database Connection

```
Host: localhost
Port: 1521
Service: XE
Username: ADMIN_APP (to be created in Task 4)
Database: Oracle 21c XE
```

## Code Example

Once completed, OracleDBAdmin application will connect via:

```csharp
string connectionString = "Data Source=(DESCRIPTION="
                        + "(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))"
                        + "(CONNECT_DATA=(SERVICE_NAME=XE)));"
                        + "User Id=ADMIN_APP;Password=<admin_password>;";

using (OracleConnection conn = new OracleConnection(connectionString))
{
    conn.Open();
    // Query ADMIN_USERS, ADMIN_ROLES, ADMIN_PERMISSIONS
    // Log operations to ADMIN_OPERATION_AUDIT
}
```

## Completion Criteria

All 6 admin tables created  
All constraints and indexes in place  
Sample data (10 users, 4 roles, 15+ permissions) inserted  
Foreign key relationships validated  
Audit logging functional  
Ready for Task 4 (Security Setup)
