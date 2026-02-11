# Subsystem 1: Oracle Database Administration Database

**Purpose:** Support the Oracle Database Admin UI tool for managing users, roles, and permissions across Oracle database instances.

This database stores administrative metadata and audit logs for the OracleDBAdmin application.

## Overview

The Subsystem 1 Admin Database contains:
- Administrative user accounts and credentials
- Role hierarchy and management data
- Permission mappings and relationships
- Audit trail of all administrative operations (user creation, role assignment, permission grants/revokes)

## Planned Directory Structure

```
Subsystem1-AdminDB/
├── schema/                            
│   ├── 01_CreateTables.sql           # Admin user, role, permission tables
│   ├── 02_CreateIndexes.sql          # Performance indexes
│   └── 03_InsertSampleData.sql       # Test admin users and roles
├── security/                          
│   ├── 01_AdminUsers_Creation.sql    # Create admin database users
│   ├── 02_AdminRBAC_Setup.sql        # Role-based access for admin operations
│   └── 03_AdminAudit_Setup.sql       # Audit admin operations
├── audit/                             
│   └── 01_AdminOperationAudit.sql    # Audit logs for admin actions
└── README.md                          (This file)
```

## Table Structure

### ADMIN_USERS
Stores administrative user accounts
```
ADMIN_USER_ID (PK)
TENTAIKHOAN (Username)
MATKHAU (Password - hashed)
HOTEN (Full name)
EMAIL
SODT (Phone)
QUYHAN (Permission level: DBA, Security Admin, User Admin, Audit Admin)
CREATED_DATE
LAST_LOGIN
ACTIVE (Y/N)
```

### ADMIN_ROLES
Stores defined admin roles
```
ADMIN_ROLE_ID (PK)
TENROLEVAITRO (Role name)
MOTA (Description)
CREATED_DATE
```

### ADMIN_PERMISSIONS
Stores individual permissions available in admin tool
```
PERMISSION_ID (PK)
TENQUYEN (Permission name: CREATE_USER, DROP_USER, GRANT_ROLE, REVOKE_PERMISSION, VIEW_AUDIT, etc.)
MOTA (Description)
LOAIQUYEN (Type: USER_MGT, ROLE_MGT, PERMISSION_MGT, AUDIT_VIEW)
```

### ADMIN_ROLE_PERMISSIONS
Maps roles to permissions (many-to-many)
```
ADMIN_ROLE_ID (FK)
PERMISSION_ID (FK)
GRANTED_DATE
GRANTED_BY (ADMIN_USER_ID - who assigned this permission)
WITH_GRANT_OPTION (Y/N)
PRIMARY KEY (ADMIN_ROLE_ID, PERMISSION_ID)
```

### ADMIN_USER_ROLES
Assigns admin roles to admin users
```
ADMIN_USER_ID (FK)
ADMIN_ROLE_ID (FK)
ASSIGNED_DATE
ASSIGNED_BY (ADMIN_USER_ID)
PRIMARY KEY (ADMIN_USER_ID, ADMIN_ROLE_ID)
```

### ADMIN_OPERATION_AUDIT
Audit trail of all admin operations
```
AUDIT_ID (PK)
ADMIN_USER_ID (FK - who performed operation)
OPERATION_TYPE (CREATE_USER, DROP_USER, GRANT_ROLE, etc.)
OBJECT_TYPE (User, Role, Permission, System)
OBJECT_NAME (Target user/role name)
TIMESTAMP
IP_ADDRESS
SUCCESS (Y/N)
DETAILS (JSON - what changed)
ERROR_MESSAGE
```

### ADMIN_SESSION_Log
Track admin login sessions
```
SESSION_ID (PK)
ADMIN_USER_ID (FK)
LOGIN_TIME
LOGOUT_TIME
IP_ADDRESS
SESSION_STATUS (Active, Closed, Timeout)
```

## Execution Order

### 1. Create Admin Schema (Run First)
```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

@Subsystem1-AdminDB/schema/01_CreateTables.sql
@Subsystem1-AdminDB/schema/02_CreateIndexes.sql
@Subsystem1-AdminDB/schema/03_InsertSampleData.sql
```

### 2. Configure Admin Security (Run Second)
```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

-- Create admin database users with minimum required privileges
@Subsystem1-AdminDB/security/01_AdminUsers_Creation.sql

-- Setup RBAC for admin operations
@Subsystem1-AdminDB/security/02_AdminRBAC_Setup.sql

-- Enable audit for admin operations
@Subsystem1-AdminDB/security/03_AdminAudit_Setup.sql
```

### 3. Setup Admin Audit (Run Third)
```sql
@Subsystem1-AdminDB/audit/01_AdminOperationAudit.sql
```

## Connection String (OracleDBAdmin Application)

```csharp
// Connection to Subsystem 1 Admin Database
string connectionString = "Data Source=(DESCRIPTION="
                        + "(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))"
                        + "(CONNECT_DATA=(SERVICE_NAME=XE)));"
                        + "User Id=ADMIN_APP;Password=<password>;";
```

## Security Considerations

1. **Separate Database Instance:** Subsystem 1 uses separate database from Subsystem 2 for admin isolation
2. **Restricted Access:** Only authenticated admin users can connect
3. **Audit All Operations:** Every admin action is logged with timestamp, user ID, and details
4. **Password Hashing:** Admin passwords stored as hashed values (SHA-256 or Oracle DBMS_CRYPTO)
5. **Session Tracking:** All admin sessions logged for accountability

## Next Steps

1. Create schema files in `schema/` directory
2. Create security configuration files in `security/` directory
3. Create audit configuration files in `audit/` directory
4. Update OracleDBAdmin application to connect to this database
5. Configure connection pooling for performance
