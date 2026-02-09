# Subsystem 1: Oracle Database Administration Application

WinForm-based database administration tool for managing Oracle users, roles, and permissions with support for column-level security.

## Overview

This application provides a comprehensive interface for DBA operations including:
- User and role management
- Permission granting/revoking with WITH GRANT OPTION support
- Column-level security for SELECT/UPDATE operations
- Permission viewing and administration

## Architecture

### Intended Project Structure

The following architecture outlines the planned directory structure for this application. Create these files and folders as you implement features:

```
Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/
├── Forms/                              # [CREATE] UI Forms & Windows
│   ├── MainForm.cs                    # Main application window
│   ├── MainForm.Designer.cs
│   ├── UserManagementForm.cs          # User CRUD operations
│   ├── UserManagementForm.Designer.cs
│   ├── RoleManagementForm.cs          # Role CRUD operations
│   ├── RoleManagementForm.Designer.cs
│   ├── PermissionForm.cs              # Permission management UI
│   ├── PermissionForm.Designer.cs
│   └── PrivilegeViewerForm.cs         # View user/role privileges
│
├── Models/                             # [CREATE] Data models & entities
│   ├── User.cs
│   ├── Role.cs
│   ├── Permission.cs
│   ├── OracleObject.cs
│   └── PrivilegeInfo.cs
│
├── Services/                           # [CREATE] Business logic & database access
│   ├── OracleConnectionService.cs     # Database connection management
│   ├── UserService.cs                 # User operations (CRUD)
│   ├── RoleService.cs                 # Role operations (CRUD)
│   ├── PermissionService.cs           # Permission operations (Grant/Revoke)
│   ├── PrivilegeService.cs            # Privilege querying
│   └── ValidationService.cs           # Input validation & error handling
│
├── Program.cs                          # Application entry point
├── App.config                          # Application configuration
└── OracleDBAdmin.csproj               # Project file
```

### File Creation Guide

When implementing features:

1. **Start with Models** - Define data structures first
2. **Create Services** - Implement business logic and Oracle interactions
3. **Build Forms** - Create UI forms that use services
4. **Add Program.cs** - Main entry point and initialization

See [Development](#development) section below for implementation details.

## Features

- **User Management**: Create, modify, delete Oracle users
- **Role Management**: Create and manage roles
- **Permission Control**: Grant/revoke permissions with options
- **Column-level Security**: Specify permissions on specific columns
- **Grant Option**: WITH GRANT OPTION support for permission delegation
- **Privilege Viewer**: View permissions for users and roles

## Getting Started

### Prerequisites
- .NET 10.0 SDK or higher
- Visual Studio 2022 or later
- Oracle Data Provider for .NET Core (ODP.NET Core)
- Oracle Database Express 21c (XE)

### Setup

1. Open the solution in Visual Studio 2022
2. Install NuGet package for .NET 10.0:
   ```bash
   dotnet add package Oracle.ManagedDataAccess.Core
   # Or in Package Manager Console:
   Install-Package Oracle.ManagedDataAccess.Core
   ```
3. Configure database credentials (see Database Connection section below)
4. Build and run

## Database Connection

**Security Warning**: Never commit passwords or credentials to version control. See [CONTRIBUTING.md](../../CONTRIBUTING.md#security-checklist) security guidelines.

For detailed setup instructions on configuring connection strings (User Secrets, Environment Variables, Local Config), see [docs/SETUP_GUIDE.md](../../docs/SETUP_GUIDE.md#step-3-configure-connection-strings).

## Usage Guide

### Creating a User
1. Open Main Form
2. Click "User Management"
3. Enter username and password
4. Click "Create User"

### Granting Permissions
1. Open "Permission Management"
2. Select user/role and object (table, view, etc.)
3. Choose permission type (SELECT, INSERT, UPDATE, DELETE)
4. For SELECT/UPDATE: optionally specify columns
5. Check "WITH GRANT OPTION" if delegation needed
6. Click "Grant"

### Viewing Permissions
1. Open "Privilege Viewer"
2. Select user or role
3. View all granted permissions and objects

## Development

### Code Standards
- Follow Microsoft C# coding guidelines
- Use meaningful variable names
- Add XML documentation to public methods
- Handle exceptions appropriately

### Building
```bash
dotnet build OracleDBAdmin.sln
```

### Testing
- Test user creation with various usernames
- Verify permission granting/revoking
- Test column-level security
- Test WITH GRANT OPTION functionality

## Troubleshooting

### Connection Failed
- Check Oracle listener is running
- Verify TNS alias exists
- Check connection string credentials

### Permission Errors
- Ensure DBA user has proper privileges
- Check if object exists before granting
- Verify role exists for role-based grants

## References

- [Oracle WinForm Guide](https://docs.oracle.com/cd/E11882_01/windows.112/e10927/toc.htm)
- [Oracle Database Security](https://docs.oracle.com/database/121/DBSEG/toc.htm)
