# Subsystem 1: Oracle Database Administration Application

WinForm-based database administration tool for managing Oracle users, roles, and permissions with support for column-level security.

## Overview

This application provides a comprehensive interface for DBA operations including:
- User and role management
- Permission granting/revoking with WITH GRANT OPTION support
- Column-level security for SELECT/UPDATE operations
- Permission viewing and administration

## Architecture

```
OracleDBAdmin/
├── Forms/
│   ├── MainForm.cs                 # Main application window
│   ├── UserManagementForm.cs       # User CRUD operations
│   ├── RoleManagementForm.cs       # Role CRUD operations
│   ├── PermissionForm.cs           # Permission management UI
│   └── PrivilegeViewerForm.cs      # View user/role privileges
├── Models/
│   ├── User.cs
│   ├── Role.cs
│   ├── Permission.cs
│   └── OracleObject.cs
├── Services/
│   ├── OracleConnectionService.cs  # Database connection management
│   ├── UserService.cs              # User operations
│   ├── RoleService.cs              # Role operations
│   ├── PermissionService.cs        # Permission operations
│   └── PrivilegeService.cs         # Privilege querying
├── OracleDBAdmin.csproj
└── Program.cs
```

## Features

- **User Management**: Create, modify, delete Oracle users
- **Role Management**: Create and manage roles
- **Permission Control**: Grant/revoke permissions with options
- **Column-level Security**: Specify permissions on specific columns
- **Grant Option**: WITH GRANT OPTION support for permission delegation
- **Privilege Viewer**: View permissions for users and roles

## Getting Started

### Prerequisites
- .NET Framework 4.7.2+
- Visual Studio 2019+
- Oracle ODP.NET NuGet package
- Oracle Database 11g+

### Setup

1. Open the solution in Visual Studio
2. Install NuGet package: `Install-Package Oracle.ManagedDataAccess`
3. Update connection string in `app.config`
4. Build and run

## Database Connection

Update `app.config`:
```xml
<connectionStrings>
    <add name="OracleDbConnection" 
         connectionString="Data Source=orcl;User Id=project_admin;Password=project_admin123;" 
         providerName="Oracle.ManagedDataAccess.Client" />
</connectionStrings>
```

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
