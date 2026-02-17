# Subsystem 1: Oracle Database Administration Application

WinForm-based database administration tool for managing Oracle users, roles, and permissions with column-level security support.

## Overview

Comprehensive Oracle DBA interface providing:

* User and role management
* Permission granting/revoking with WITH GRANT OPTION delegation
* Column-level security for SELECT/UPDATE operations
* Enterprise privilege tracking and administration

## Features

* **User Management** - Create, modify, delete Oracle users
* **Role Management** - Create and manage roles with hierarchy
* **Permission Control** - Grant/revoke with delegation options
* **Column-level Security** - Fine-grained control on specific columns
* **Privilege Viewer** - Track permissions across users and roles
* **Audit Support** - Full audit trail of all operations

## Project Structure

```
subsystem1-oracleDBAdmin/source/oracleDBAdmin/
├── forms/
│   ├── MainForm.cs                     # Application window
│   ├── UserManagementForm.cs           # User CRUD operations
│   ├── RoleManagementForm.cs           # Role management
│   ├── PermissionForm.cs               # Permission management
│   └── PrivilegeViewerForm.cs          # Privilege viewer
├── models/
│   ├── User.cs
│   ├── Role.cs
│   ├── Permission.cs
│   └── OracleObject.cs
├── services/
│   ├── OracleConnectionService.cs
│   ├── UserService.cs
│   ├── RoleService.cs
│   ├── PermissionService.cs
│   ├── PrivilegeService.cs
│   └── ValidationService.cs
├── Program.cs
└── OracleDBAdmin.csproj
```

## Getting Started

**Prerequisites:**

* .NET 10.0 SDK or higher
* Visual Studio 2022 or later
* Oracle Data Provider for .NET Core (ODP.NET Core)
* Oracle Database Express 21c (XE)

**Setup:**

1. Open the solution in Visual Studio 2022
2. Install the required NuGet package:

   ```bash
   dotnet add package Oracle.ManagedDataAccess.Core
   ```

3. Configure database connection (see SETUP_GUIDE.md)
4. Build and run the application

**Security Notice:**

Never commit credentials to version control. Use User Secrets, environment variables, or configuration files. See [CONTRIBUTING.md](../../CONTRIBUTING.md#security-checklist) for security guidelines.

## Usage

**Create User:**

1. Open Main Form → User Management
2. Enter username and password
3. Click Create User

**Grant Permission:**

1. Open Permission Management
2. Select user/role and target object
3. Choose permission type (SELECT, INSERT, UPDATE, DELETE)
4. For SELECT/UPDATE: optionally specify columns for column-level security
5. Check "WITH GRANT OPTION" for delegation
6. Click Grant

**View Privileges:**

1. Open Privilege Viewer
2. Select user or role
3. View all granted permissions and objects

## Development

**Code Standards:**

* Follow Microsoft C# coding guidelines
* Use meaningful variable names
* Add XML documentation to public methods
* Handle exceptions appropriately

**Build:**

```bash
dotnet build OracleDBAdmin.slnx
```

**Testing:**

* Test user creation with various usernames
* Verify permission granting/revoking
* Test column-level security permissions
* Validate WITH GRANT OPTION delegation
* Test privilege viewer accuracy

## Troubleshooting

**Connection Failed:**

* Verify Oracle listener is running
* Check TNS alias configuration in tnsnames.ora
* Validate connection string credentials

**Permission Errors:**

* Ensure DBA user has proper system privileges
* Verify target object exists before granting
* Check that role exists for role-based grants

## References

* [Oracle Database Security](https://docs.oracle.com/database/121/DBSEG/toc.htm)
* [Using WinForms with Oracle](https://docs.oracle.com/cd/E11882_01/windows.112/e10927/)
* [ODP.NET Core Documentation](https://docs.oracle.com/en-us/iaas/Content/API/SDKDocs/odp-net-core.htm)
