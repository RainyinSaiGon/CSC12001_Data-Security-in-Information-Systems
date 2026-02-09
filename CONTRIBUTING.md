# Contributing Guidelines

Thank you for contributing to CSC12001 Data Security Project! This document outlines how to contribute code, documentation, and feedback.

## Code of Conduct

- Be respectful and professional
- Provide constructive feedback
- Help fellow team members
- Report issues promptly
- Follow project standards

## Getting Started

1. **Clone the Repository**
    ```bash
    git clone https://github.com/dinhdaivu/CSC12001_Data-Security-in-Information-Systems.git
    cd CSC12001_Data-Security-in-Information-Systems
    ```

2. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b fix/issue-name
   ```

3. **Make Your Changes**
   - Follow code standards (see below)
   - Write tests for new features
   - Update documentation
   - Commit frequently with descriptive messages

4. **Push to Remote**
   ```bash
   git push origin feature/your-feature-name
   ```

5. **Create a Pull Request**
   - Describe changes clearly
   - Reference related issues
   - Wait for review

## Commit Message Format

Format: `<type>(<scope>): <subject>`

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Test additions or modifications
- `chore`: Build, dependencies, etc.

Scope (optional):
- `subsystem1`: Oracle DB Admin changes
- `subsystem2`: Medical system changes
- `database`: Database scripts
- `docs`: Documentation

Examples:
```
feat(subsystem1): add user creation form
fix(subsystem2): resolve VPD policy issue
docs(database): update schema documentation
refactor(subsystem2): improve service layer
test(audit): add audit verification tests
```

## C# Code Standards

### Naming Conventions
```csharp
// Classes and public methods
public class UserManagementForm { }
public void CreateUser() { }

// Private members
private string _connectionString;
private void InitializeComponent() { }

// Constants
private const string DATABASE_OWNER = "project_admin";
public static readonly string DEFAULT_ROLE = "user_role";

// Local variables
string userName = GetUserName();
int userId = 123;
```

### Code Style
```csharp
// .NET 10.0 with Oracle 21c XE
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

// Proper spacing and formatting
public void GrantPermission(string userName, string objectName, string permissionType)
{
    if (string.IsNullOrEmpty(userName))
    {
        throw new ArgumentNullException(nameof(userName));
    }

    // Connection string from configuration
    var config = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();
    
    string connectionString = $"Data Source={config["OracleDbConnection:DataSource"]};" +
                             $"User Id={config["OracleDbConnection:UserId"]};" +
                             $"Password={config["OracleDbConnection:Password"]};";

    // Comment explaining logic
    var permission = new Permission 
    { 
        UserName = userName, 
        ObjectName = objectName, 
        PermissionType = permissionType 
    };

    try
    {
        using var connection = new OracleConnection(connectionString);
        connection.Open();
        _permissionService.Grant(permission);
    }
    catch (OracleException ex)
    {
        MessageBox.Show($"Database error: {ex.Message}");
        Logger.LogError(ex);
    }
}
```

### XML Documentation
```csharp
/// <summary>
/// Grants permission to a database user.
/// </summary>
/// <param name="userName">The username to grant permission to</param>
/// <param name="objectName">The database object name</param>
/// <param name="permissionType">Type of permission (SELECT, INSERT, UPDATE, DELETE)</param>
/// <returns>True if successful, false otherwise</returns>
/// <exception cref="ArgumentNullException">Thrown when userName is null</exception>
public bool GrantPermission(string userName, string objectName, string permissionType)
{
    // Implementation
}
```

## Database Script Standards

### SQL Script Rules
1. Always include transaction control
2. Add comments explaining complex logic
3. Use consistent naming conventions
4. Include error handling where appropriate
5. Test all scripts before committing

```sql
-- Create roles for database security
-- Script: 01_RBAC_Setup.sql
-- Date: February 2026

-- COORDINATOR_ROLE: Can manage patient records and assign doctors
BEGIN
    BEGIN
        EXECUTE IMMEDIATE 'DROP ROLE coordinator_role';
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE != -01921 THEN RAISE; END IF;
    END;
    
    EXECUTE IMMEDIATE 'CREATE ROLE coordinator_role';
    
    -- Grant permissions
    EXECUTE IMMEDIATE 'GRANT SELECT ON patient_table TO coordinator_role';
    EXECUTE IMMEDIATE 'GRANT INSERT, UPDATE ON patient_table TO coordinator_role';
    
    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END;
/
```

## Testing Guidelines

### Unit Testing
- Write tests for business logic
- Use Assert statements effectively
- Test both success and failure cases
- Mock external dependencies

### Integration Testing
- Test with actual Oracle database
- Verify security policies work correctly
- Test multi-user scenarios
- Validate audit logging

### Test Case Documentation
```markdown
## Test Case: TC#1 - User Setup and Account Creation

### Objective
Verify that DBA can create user accounts linked to NHÂNVIÊN records

### Prerequisites
- Oracle database initialized
- DBA user connected

### Steps
1. Create 5 test users
2. Connect as each user
3. Query DBA_USERS for verification

### Expected Results
- All users created successfully
- All users can authenticate
- User accounts appear in DBA_USERS

### Notes
- Passwords follow security policy
- Account linked via EXTERNAL_NAME attribute
```

## Documentation Standards

### README Files
- Clear, concise descriptions
- Easy-to-follow setup steps
- Troubleshooting sections
- Links to relevant resources

### Code Comments
```csharp
// Use single-line comments for brief explanations
int userCount = 0; // Track number of users created

/*
 * Use multi-line comments for detailed explanations
 * explaining complex algorithmic decisions or
 * business logic that might not be obvious
 */
foreach (var role in roles)
{
    // Single line comment
    ProcessRole(role);
}
```

## Pull Request Process

1. **Before Creating PR**
   - [ ] Code follows standards
   - [ ] Tests pass locally
   - [ ] No merge conflicts
   - [ ] Documentation updated

2. **PR Description Template**
   ```markdown
   ## Description
   Brief description of changes

   ## Type of Change
   - [ ] Bug fix (non-breaking)
   - [ ] New feature (non-breaking)
   - [ ] Breaking change
   - [ ] Documentation update

   ## Changes Made
   - List of specific changes
   - Updated files

   ## Testing Done
   - How you tested the changes
   - Test cases executed

   ## Related Issues
   Closes #123

   ## Screenshots (if applicable)
   [Add screenshots for UI changes]
   ```

3. **Review Process**
   - At least one reviewer approval required
   - All comments must be resolved
   - CI/CD checks must pass

## Issue Reporting

### Bug Report Template
```markdown
## Description
[Clear description of the bug]

## Steps to Reproduce
1. [First step]
2. [Second step]
3. ...

## Expected Behavior
[What should happen]

## Actual Behavior
[What actually happens]

## Screenshots/Logs
[Attach if applicable]

## Environment
- Windows/Linux/Mac
- Visual Studio version
- Oracle version
- .NET version
```

### Feature Request Template
```markdown
## Feature Description
[Clear description of requested feature]

## Use Case
[Why is this feature needed?]

## Proposed Solution
[How should it work?]

## Related Issues
[Link any related issues]
```

## Code Review Checklist

Reviewers should verify:
- [ ] Code follows project standards
- [ ] Tests are included and pass
- [ ] Documentation is updated
- [ ] No hardcoded credentials or sensitive data
- [ ] Error handling is appropriate
- [ ] Security best practices followed
- [ ] Performance implications considered
- [ ] No duplicate code

## Security Checklist

Before committing sensitive code:
- [ ] No credentials in code
- [ ] SQL injection prevention implemented
- [ ] Input validation present
- [ ] Authentication/authorization enforced
- [ ] Sensitive data is encrypted
- [ ] Audit logging configured
- [ ] Error messages don't leak information

## Deployment Checklist

Before merging to main:
- [ ] All tests pass
- [ ] Code review approved
- [ ] Database migrations tested
- [ ] Security review completed
- [ ] Documentation updated
- [ ] Changelog updated
- [ ] Release notes prepared

## Tools & Resources

### Development Tools
- Visual Studio 2019+
- Oracle SQL Developer
- Git for version control
- Postman (for API testing if applicable)

### Documentation
- [Microsoft C# Coding Guidelines](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Oracle Database Documentation](https://docs.oracle.com/database/)
- [Git Workflow Guide](https://guides.github.com/introduction/flow/)

## Questions?

If you have questions:
1. Check existing issues/discussions
2. Ask in team meetings
3. Create an issue with `[QUESTION]` tag
4. Contact team leads

## License

By contributing, you agree that your contribution will be licensed under the same MIT License as the project.

Thank you for your contributions!
