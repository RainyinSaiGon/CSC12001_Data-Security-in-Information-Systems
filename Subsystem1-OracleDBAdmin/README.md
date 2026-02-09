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
- .NET Framework 4.7.2+
- Visual Studio 2019+
- Oracle ODP.NET NuGet package
- Oracle Database 11g+

### Setup

1. Open the solution in Visual Studio
2. Install NuGet package: `Install-Package Oracle.ManagedDataAccess`
3. Configure database credentials (see Database Connection section below)
4. Build and run

## Database Connection

### Configuration Methods

**Security Warning**: Never commit passwords or credentials to version control. See [CONTRIBUTING.md](../../CONTRIBUTING.md#security-checklist) security guidelines.

#### Option 1: User Secrets (Development)
Recommended for local development. Credentials are encrypted and stored outside the project.

```bash
# Initialize user secrets for the project
cd Subsystem1-OracleDBAdmin
dotnet user-secrets init

# Set Oracle connection credentials
dotnet user-secrets set "OracleDbConnection:UserId" "project_admin"
dotnet user-secrets set "OracleDbConnection:Password" "your_secure_password"
```

Update `app.config` to use placeholder:
```xml
<connectionStrings>
    <add name="OracleDbConnection" 
         connectionString="Data Source=orcl;User Id=[SET_VIA_USER_SECRETS];Password=[SET_VIA_USER_SECRETS];" 
         providerName="Oracle.ManagedDataAccess.Client" />
</connectionStrings>
```

In `Program.cs`, load from user secrets:
```csharp
var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string connectionString = config.GetConnectionString("OracleDbConnection");
```

#### Option 2: Environment Variables (Production)
For production or CI/CD environments.

```bash
# Set environment variables (Windows)
set ORACLE_USERID=project_admin
set ORACLE_PASSWORD=your_secure_password
set ORACLE_DATA_SOURCE=orcl

# Set environment variables (Linux/macOS)
export ORACLE_USERID=project_admin
export ORACLE_PASSWORD=your_secure_password
export ORACLE_DATA_SOURCE=orcl
```

In `Program.cs`:
```csharp
var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

string userId = config["ORACLE_USERID"];
string password = config["ORACLE_PASSWORD"];
string dataSource = config["ORACLE_DATA_SOURCE"];

string connectionString = $"Data Source={dataSource};User Id={userId};Password={password};";
```

#### Option 3: Local Configuration File (Development Only)
Create an uncommitted local config file:

1. Create `appsettings.local.json` (add to `.gitignore`)
```json
{
  "OracleDbConnection": {
    "DataSource": "orcl",
    "UserId": "project_admin",
    "Password": "your_secure_password"
  }
}
```

2. Load in `Program.cs`:
```csharp
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.local.json", optional: true)
    .Build();
```

### Secure Connection String Template

Use this template in `app.config` (credentials NOT embedded):
```xml
<connectionStrings>
    <add name="OracleDbConnection" 
         connectionString="Data Source=[DATA_SOURCE];User Id=[USER_ID];Password=[PASSWORD];" 
         providerName="Oracle.ManagedDataAccess.Client" />
</connectionStrings>
```

Replace placeholders via:
- User secrets (development)
- Environment variables (production)
- Configuration files (local only, not committed)

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
