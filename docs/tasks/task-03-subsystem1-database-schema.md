# Task 03: Subsystem 1 - Admin Database Schema & Data Dictionary

**Assigned to:** Duyên, Triết  
**Type:** Database Design & Administration  
**Duration:** 10 hours  
**Priority:** High (required for OracleDBAdmin application)  
**Timeline:** Feb 12 - Feb 20, 2026

---

## 1. Overview

Design and create the admin database schema for Subsystem 1 (OracleDBAdmin) with 7 SQL script deliverables:

* **01_CreateAdminTables.sql** — Admin users, roles, user-role assignments, operation audit trail
* **02_CreateDatabaseObjectTables.sql** — Database object catalog, columns, permissions tracking, grantee hierarchy
* **03_CreateAdminViews.sql** — User/role/object permissions views, grant options, audit trails, column-level permissions
* **04_CreateAdminStoredProcedures.sql** — User/role/permission management, validation, audit logging
* **05_CreateAdminFunctions.sql** — Permission logic functions (column-level grants, permission checking)
* **06_CreateAdminIndexes.sql** — Performance indexes on user, role, object, permission, and audit tables
* **07_InsertSampleData.sql** — 4 admin users, 4 admin roles, role assignments, 15+ permissions, sample audit logs

## 2. Requirements & Constraints

* Support all oracle object types: TABLE, VIEW, STORED PROCEDURE, FUNCTION
* Column-level grants: SELECT and UPDATE only; object-level only for INSERT/DELETE
* WITH GRANT OPTION tracking and enforcement for delegated administration
* Permission verification via views and stored procedures
* Audit trail with immutable logging of all admin operations
* Catalog real Subsystem 2 database objects (medical tables)
* Prevent invalid grants via validation in stored procedures

## 3. Deliverables Summary

### Tables (ADMIN_USERS, ADMIN_ROLES, ADMIN_USER_ROLES, ADMIN_OPERATION_AUDIT, DB_OBJECTS, DB_OBJECT_COLUMNS, PERMISSIONS_CATALOG, PERMISSION_ASSIGNMENTS, GRANTEE_HIERARCHY)

* ADMIN_USERS — Admin user accounts (ADMIN_USER_ID PK, TENTAIKHOAN unique, MATKHAU hashed, HOTEN, EMAIL, SODT, ACTIVE)
* ADMIN_ROLES — Admin role definitions (ADMIN_ROLE_ID PK, TENROLEVAITRO, MOTA, CREATED_DATE)
* ADMIN_USER_ROLES — User-Role assignments (PK: ADMIN_USER_ID, ADMIN_ROLE_ID)
* ADMIN_OPERATION_AUDIT — Operation audit trail (AUDIT_ID PK, ADMIN_USER_ID FK, OPERATION_TYPE, OBJECT_TYPE, TIMESTAMP, SOURCE_IP, SUCCESS)
* DB_OBJECTS — Database object catalog (OBJECT_ID PK, OBJECT_NAME, OBJECT_TYPE TABLE|VIEW|PROCEDURE|FUNCTION, OBJECT_OWNER, CREATED_DATE)
* DB_OBJECT_COLUMNS — Column catalog for column-level grants (COLUMN_ID PK, OBJECT_ID FK, COLUMN_NAME, DATA_TYPE, NULLABLE)
* PERMISSIONS_CATALOG — Available permissions per object type (PERMISSION_ID PK, PERMISSION_NAME SELECT|INSERT|UPDATE|DELETE|EXECUTE, APPLICABLE_TO)
* PERMISSION_ASSIGNMENTS — Granted permissions tracking (GRANT_ID PK, USER_OR_ROLE_NAME, USER_OR_ROLE_TYPE USER|ROLE, OBJECT_ID FK, PERMISSION_ID FK, WITH_GRANT_OPTION Y/N, GRANTED_BY, GRANTED_DATE)
* GRANTEE_HIERARCHY — Permission delegation chains (HIERARCHY_ID PK, ORIGINAL_GRANTEE, DELEGATED_TO, DELEGATION_DEPTH, CREATED_DATE)

### Views (6 required)

* V_USER_PERMISSIONS — All permissions assigned to specific user across all objects
* V_ROLE_PERMISSIONS — All permissions granted to specific role
* V_OBJECT_PERMISSIONS — Who has permissions on specific database object
* V_USER_GRANT_OPTIONS — Users/roles with WITH GRANT OPTION
* V_PERMISSION_AUDIT_TRAIL — Detailed audit trail with permission change history
* V_COLUMN_LEVEL_PERMISSIONS — Column-level SELECT and UPDATE grants

### Stored Procedures (16 required)

**User Management:** SP_CREATE_USER, SP_MODIFY_USER, SP_DELETE_USER  
**Role Management:** SP_CREATE_ROLE, SP_DELETE_ROLE, SP_ASSIGN_ROLE_TO_USER  
**Permission Grant/Revoke:** SP_GRANT_PERMISSION, SP_REVOKE_PERMISSION, SP_VALIDATE_COLUMN_LEVEL_PERMISSION  
**Permission Tracking:** SP_GET_USER_PERMISSIONS, SP_GET_ROLE_PERMISSIONS, SP_GET_OBJECT_GRANTEES, SP_CHECK_GRANT_OPTION  
**Audit & Logging:** SP_LOG_ADMIN_OPERATION, SP_GET_PERMISSION_AUDIT_TRAIL

### Functions (6 required)

* F_CAN_GRANT_COLUMN_LEVEL(permission_type) — Returns 1 for SELECT/UPDATE, 0 otherwise
* F_CAN_GRANT_OBJECT_LEVEL(permission_type, object_type) — Returns 1 if valid permission for object type
* F_HAS_PERMISSION(grantee, object_id, permission_id, column_id) — Returns 1 if granted
* F_HAS_GRANT_OPTION(grantee, object_id, permission_id) — Returns 1 if can grant to others
* F_GET_PERMISSION_NAME(permission_id) — Return permission name
* F_GET_OBJECT_NAME_BY_ID(object_id) — Return object name

### Indexes (8 required)

* ADMIN_USERS(TENTAIKHOAN, ACTIVE) — Login lookup
* ADMIN_ROLES(TENROLEVAITRO) — Role lookup
* DB_OBJECTS(OBJECT_NAME, OBJECT_TYPE) — Object search
* PERMISSION_ASSIGNMENTS(USER_OR_ROLE_NAME, OBJECT_ID) — Permission lookup
* PERMISSION_ASSIGNMENTS(ADMIN_USER_ID) — Audit by user
* ADMIN_OPERATION_AUDIT(TIMESTAMP) — Audit by date
* DB_OBJECT_COLUMNS(OBJECT_ID) — Column lookup

### Sample Data (Initial)

* **Admin Users (4):** 1 DBA (full), 1 Security Admin (roles/perms), 1 User Admin (users/roles), 1 Regular Admin (example)
* **Admin Roles (4):** DBA (all permissions), Security Admin (role/perm mgmt), User Admin (user/role mgmt), Audit Admin (audit view)
* **Permissions (15+):** CREATE_USER, DROP_USER, MODIFY_USER, CREATE_ROLE, DROP_ROLE, GRANT_PERMISSION, REVOKE_PERMISSION, VIEW_AUDIT, etc.
* **Database Objects:** Subsystem 2 medical tables (BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO)
* **Column-Level Permissions:** SELECT/UPDATE on restricted columns (CCCD, HOTEN, medical data)
* **Audit Logs:** Initial creation of users, roles, and permissions  

## 4. Acceptance Criteria

* [ ] All 9 tables created with correct constraints (PK, FK, NOT NULL, UNIQUE, CHECK)
* [ ] All 6 views functional and queryable
* [ ] All 16 stored procedures created and executable
* [ ] All 6 functions working correctly
* [ ] All 8 indexes created and functional
* [ ] Sample data inserted: 4 users, 4 roles, 15+ permissions, audit logs
* [ ] Permission rules enforced (column-level for SELECT/UPDATE only)
* [ ] WITH GRANT OPTION tracked and accessible
* [ ] Audit trail immutable and comprehensive
* [ ] OracleDBAdmin application successfully queries schema