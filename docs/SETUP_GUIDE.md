# Complete Setup Guide - .NET 10.0 & Oracle 21c XE

## Quick Reference

| Component | Version | Status |
|-----------|---------|--------|
| .NET SDK | 10.0.102 | ? Ready |
| Oracle Database | Express 21c (21.3.0.0.0) | ? Ready |
| ODP.NET Package | 23.26.100 | ? Available |
| Visual Studio | 2022 | Required |
| Windows Desktop Runtime | 10.0.2 | ? Installed |

## Connection Details

### Oracle 21c XE
- **Host**: localhost
- **Port**: 1521
- **Service Name**: XE
- **Connection String**: `Data Source=localhost:1521/XE;User Id=username;Password=password;`
- **SQL*Plus**: `sqlplus username/password@localhost:1521/XE`

### .NET 10.0
- **Target Framework**: net10.0
- **Package**: Oracle.ManagedDataAccess.Core
- **Package Version**: 23.26.100
- **Project Type**: Windows Forms Application

## Step-by-Step Setup

### 1. Verify Prerequisites

```powershell
# Check .NET version
& "C:\Program Files\dotnet\dotnet.exe" --version
# Should show: 10.0.102

# Check Oracle service
sc query OracleServiceXE
# Should show: STATE = RUNNING

# Check Oracle listener
lsnrctl status
# Should show: Listening on port 1521

# Check SQL*Plus
sqlplus -version
# Should show: SQL*Plus: Release 21.0.0.0.0
```

### 2. Create Database User

```sql
-- Connect as SYSDBA
sqlplus / as sysdba

-- Create project_admin user
CREATE USER project_admin IDENTIFIED BY YourStrongPassword123!;

-- Grant basic privileges
GRANT CONNECT, RESOURCE TO project_admin;
GRANT CREATE VIEW, CREATE PROCEDURE, CREATE SEQUENCE TO project_admin;
GRANT CREATE TRIGGER, CREATE TYPE, CREATE SYNONYM TO project_admin;
GRANT UNLIMITED TABLESPACE TO project_admin;

-- Grant advanced security privileges
GRANT CREATE USER, ALTER USER, DROP USER TO project_admin;
GRANT CREATE ROLE, DROP ANY ROLE, GRANT ANY ROLE TO project_admin;
GRANT GRANT ANY PRIVILEGE TO project_admin;
GRANT EXECUTE ON DBMS_RLS TO project_admin;
GRANT AUDIT SYSTEM TO project_admin;
GRANT SELECT ON SYS.DBA_AUDIT_TRAIL TO project_admin;
GRANT SELECT ON SYS.DBA_FGA_AUDIT_TRAIL TO project_admin;

-- Verify
SELECT username, account_status, default_tablespace 
FROM dba_users 
WHERE username = 'PROJECT_ADMIN';

EXIT;
```

### 3. Test Connection

```sql
-- Test connection
sqlplus project_admin/YourStrongPassword123!@localhost:1521/XE

-- Run test query
SELECT 'Connection successful!' AS status FROM dual;

-- Check privileges
SELECT * FROM session_privs ORDER BY privilege;

EXIT;
```

### 4. Create WinForm Projects

#### Subsystem 1 - OracleDBAdmin

```powershell
# Navigate to project directory
cd Subsystem1-OracleDBAdmin
mkdir Source
cd Source

# Create WinForm project for .NET 10.0
& "C:\Program Files\dotnet\dotnet.exe" new winforms -n OracleDBAdmin -f net10.0

# Navigate to project
cd OracleDBAdmin

# Add Oracle package
& "C:\Program Files\dotnet\dotnet.exe" add package Oracle.ManagedDataAccess.Core

# Add configuration package
& "C:\Program Files\dotnet\dotnet.exe" add package Microsoft.Extensions.Configuration
& "C:\Program Files\dotnet\dotnet.exe" add package Microsoft.Extensions.Configuration.UserSecrets

# Verify packages
& "C:\Program Files\dotnet\dotnet.exe" list package

# Initialize user secrets
& "C:\Program Files\dotnet\dotnet.exe" user-secrets init

# Set connection credentials
& "C:\Program Files\dotnet\dotnet.exe" user-secrets set "OracleDbConnection:UserId" "project_admin"
& "C:\Program Files\dotnet\dotnet.exe" user-secrets set "OracleDbConnection:Password" "YourStrongPassword123!"
& "C:\Program Files\dotnet\dotnet.exe" user-secrets set "OracleDbConnection:DataSource" "localhost:1521/XE"

# Build project
& "C:\Program Files\dotnet\dotnet.exe" build
```

#### Subsystem 2 - MedicalDataSystem

```powershell
# Navigate to project directory
cd Subsystem2-MedicalDataManagement
mkdir Source
cd Source

# Create WinForm project for .NET 10.0
& "C:\Program Files\dotnet\dotnet.exe" new winforms -n MedicalDataSystem -f net10.0

# Navigate to project
cd MedicalDataSystem

# Add Oracle package
& "C:\Program Files\dotnet\dotnet.exe" add package Oracle.ManagedDataAccess.Core

# Add configuration packages
& "C:\Program Files\dotnet\dotnet.exe" add package Microsoft.Extensions.Configuration
& "C:\Program Files\dotnet\dotnet.exe" add package Microsoft.Extensions.Configuration.UserSecrets

# Initialize user secrets
& "C:\Program Files\dotnet\dotnet.exe" user-secrets init

# Set connection credentials
& "C:\Program Files\dotnet\dotnet.exe" user-secrets set "OracleDbConnection:UserId" "project_admin"
& "C:\Program Files\dotnet\dotnet.exe" user-secrets set "OracleDbConnection:Password" "YourStrongPassword123!"
& "C:\Program Files\dotnet\dotnet.exe" user-secrets set "OracleDbConnection:DataSource" "localhost:1521/XE"

# Build project
& "C:\Program Files\dotnet\dotnet.exe" build
```

### 5. Test Oracle Connection in C#

Create a test file `TestConnection.cs`:

```csharp
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

class TestConnection
{
    static void Main()
    {
        try
        {
            // Load configuration from user secrets
            var config = new ConfigurationBuilder()
                .AddUserSecrets<TestConnection>()
                .Build();

            string userId = config["OracleDbConnection:UserId"];
            string password = config["OracleDbConnection:Password"];
            string dataSource = config["OracleDbConnection:DataSource"];

            string connectionString = $"Data Source={dataSource};User Id={userId};Password={password};";

            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("? Successfully connected to Oracle 21c XE!");
                Console.WriteLine($"Oracle Version: {connection.ServerVersion}");
                Console.WriteLine($"Database: {connection.Database}");
                
                // Test query
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT 'Hello from Oracle!' AS message FROM dual";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine($"Test Query: {reader.GetString(0)}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Connection failed: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}
```

Run the test:
```powershell
& "C:\Program Files\dotnet\dotnet.exe" run
```

## Configuration Templates

### appsettings.json (Committed)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "OracleDbConnection": {
    "DataSource": "localhost:1521/XE"
  }
}
```

### appsettings.local.json (NOT Committed - Add to .gitignore)

```json
{
  "OracleDbConnection": {
    "UserId": "project_admin",
    "Password": "YourStrongPassword123!",
    "DataSource": "localhost:1521/XE"
  }
}
```

### .gitignore Entry

```
# Sensitive configuration files
appsettings.local.json
appsettings.*.local.json
**/appsettings.local.json
**/appsettings.*.local.json
```

## Common Commands

### Oracle Service Management

```powershell
# Check service status
sc query OracleServiceXE

# Start service
net start OracleServiceXE

# Stop service
net stop OracleServiceXE

# Restart service
net stop OracleServiceXE && net start OracleServiceXE
```

### Oracle Listener Management

```powershell
# Check listener status
lsnrctl status

# Start listener
lsnrctl start

# Stop listener
lsnrctl stop

# Reload listener
lsnrctl reload
```

### .NET Commands

```powershell
# Check .NET version
& "C:\Program Files\dotnet\dotnet.exe" --version

# List installed SDKs
& "C:\Program Files\dotnet\dotnet.exe" --list-sdks

# List installed runtimes
& "C:\Program Files\dotnet\dotnet.exe" --list-runtimes

# Create new WinForm project
& "C:\Program Files\dotnet\dotnet.exe" new winforms -n ProjectName -f net10.0

# Add NuGet package
& "C:\Program Files\dotnet\dotnet.exe" add package PackageName

# List packages
& "C:\Program Files\dotnet\dotnet.exe" list package

# Build project
& "C:\Program Files\dotnet\dotnet.exe" build

# Run project
& "C:\Program Files\dotnet\dotnet.exe" run

# Clean build artifacts
& "C:\Program Files\dotnet\dotnet.exe" clean
```

### User Secrets Management

```powershell
# Initialize user secrets
& "C:\Program Files\dotnet\dotnet.exe" user-secrets init

# Set a secret
& "C:\Program Files\dotnet\dotnet.exe" user-secrets set "Key" "Value"

# List all secrets
& "C:\Program Files\dotnet\dotnet.exe" user-secrets list

# Remove a secret
& "C:\Program Files\dotnet\dotnet.exe" user-secrets remove "Key"

# Clear all secrets
& "C:\Program Files\dotnet\dotnet.exe" user-secrets clear
```

## Troubleshooting

### Oracle Connection Issues

**Problem**: ORA-12154: TNS:could not resolve the connect identifier
```powershell
# Solution: Use direct connection string
Data Source=localhost:1521/XE
# Instead of: Data Source=XE
```

**Problem**: ORA-01017: invalid username/password
```powershell
# Solution: Verify credentials
sqlplus project_admin/password@localhost:1521/XE
```

**Problem**: ORA-12541: TNS:no listener
```powershell
# Solution: Start the listener
lsnrctl start
```

**Problem**: Service not running
```powershell
# Solution: Start Oracle service
net start OracleServiceXE
```

### .NET Issues

**Problem**: 'dotnet' is not recognized
```powershell
# Solution: Use full path or add to PATH
& "C:\Program Files\dotnet\dotnet.exe" --version

# Or add to PATH permanently
$env:Path += ";C:\Program Files\dotnet"
```

**Problem**: Package restore failed
```powershell
# Solution: Clear NuGet cache and restore
& "C:\Program Files\dotnet\dotnet.exe" nuget locals all --clear
& "C:\Program Files\dotnet\dotnet.exe" restore
```

### Connection String Issues

**Problem**: Cannot read user secrets
```powershell
# Solution: Initialize user secrets first
& "C:\Program Files\dotnet\dotnet.exe" user-secrets init
& "C:\Program Files\dotnet\dotnet.exe" user-secrets list
```

**Problem**: Connection string format error
```csharp
// ? Correct format for Oracle 21c XE
"Data Source=localhost:1521/XE;User Id=project_admin;Password=password;"

// ? Wrong formats
"Data Source=XE;..." // Missing host and port
"Data Source=orcl;..." // Wrong service name
```

## Next Steps

1. ? Prerequisites verified
2. ? Database user created
3. ? Connection tested
4. ?? Create database tables (Schema scripts)
5. ?? Configure security (RBAC, VPD, OLS)
6. ?? Setup audit mechanisms
7. ?? Build WinForm applications
8. ?? Test all features

## Resources

- [.NET 10.0 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Oracle 21c XE Documentation](https://docs.oracle.com/en/database/oracle/oracle-database/21/)
- [ODP.NET Core Documentation](https://www.oracle.com/database/technologies/appdev/dotnet/odp.html)
- [Visual Studio 2022 Download](https://visualstudio.microsoft.com/vs/)

---

**Last Updated**: February 2026  
**Project**: CSC12001 - Data Security in Information Systems  
**Institution**: University of Science - Faculty of Information Technology
