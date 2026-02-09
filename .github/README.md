# GitHub Configuration

GitHub-specific configuration files for CI/CD, issue management, and contribution workflows.

## Directory Structure

```
.github/
├── workflows/
│   ├── subsystem1-ci.yml          # Subsystem 1 build & test automation
│   ├── subsystem2-ci.yml          # Subsystem 2 build & test automation
│   └── database-ci.yml            # Database scripts validation
├── ISSUE_TEMPLATE/
│   ├── bug_report.md              # Bug report template
│   ├── feature_request.md         # Feature request template
│   └── config.yml                 # Issue template configuration
├── pull_request_template.md       # Pull request template
└── README.md                      # This file
```

## Workflows (CI/CD Automation)

### Subsystem 1 CI/CD (`subsystem1-ci.yml`)
Automated build and test pipeline for Oracle Database Administration application.

**Triggers**: 
- Pull request to `main` or `develop` branches
- Changes to files in `Subsystem1-OracleDBAdmin/` directory

**Jobs**:
- **build-and-test**: Builds .NET project, runs unit tests (Windows)
- **code-quality**: Code analysis using CodeQL

**Matrix**: .NET versions 6.0.x, 7.0.x

### Subsystem 2 CI/CD (`subsystem2-ci.yml`)
Automated build and test pipeline for Medical Data Management System.

**Triggers**: 
- Pull request to `main` or `develop` branches
- Changes to files in `Subsystem2-MedicalDataManagement/` directory

**Jobs**:
- **build-and-test**: Builds .NET project, runs unit tests (Windows)
- **code-quality**: Code analysis using CodeQL

**Matrix**: .NET versions 6.0.x, 7.0.x

### Database CI/CD (`database-ci.yml`)
Validation pipeline for SQL scripts and database schemas.

**Triggers**: 
- Pull request to `main` or `develop` branches
- Changes to files in `Database/` directory

**Jobs**:
- **validate-scripts**: Checks SQL syntax and script structure
- **lint**: Lints SQL files using SQLFluff with Oracle dialect

**Features**:
- Verifies script execution order
- Checks for documentation/comments
- Validates SQL syntax

## Issue Templates

### Bug Report (`bug_report.md`)
Used when reporting bugs or issues.

**Fields**:
- Bug description
- System/environment information
- Steps to reproduce
- Current vs expected behavior
- Logs and screenshots
- Additional context
- Severity level

**Labels**: `bug`

### Feature Request (`feature_request.md`)
Used when suggesting new features or improvements.

**Fields**:
- Feature summary
- Problem statement
- Proposed solution
- Related requirements
- Use cases
- Acceptance criteria
- Task breakdown
- Complexity estimation

**Labels**: `enhancement`

### Issue Template Configuration (`config.yml`)
Customizes the issue creation experience.

**Settings**:
- Disables blank issues (users must use a template)
- Links to Discussions for general questions

## Pull Request Template (`pull_request_template.md`)

Standard template for all pull requests.

**Sections**:
- Description of changes
- Type of change (bug/feature/breaking/docs)
- Related issues
- Detailed changes list
- Components affected
- Comprehensive checklist
- Testing performed
- Screenshots (if applicable)
- Code review responses
- Additional notes

## Usage

### Creating Issues
1. Click "New Issue" on GitHub
2. Select either "Bug Report" or "Feature Request"
3. Fill out all required fields
4. Submit the issue

### Creating Pull Requests
1. Create your branch: `git checkout -b feature/your-feature`
2. Make your changes
3. Push to remote: `git push origin feature/your-feature`
4. Open a Pull Request on GitHub
5. The template will automatically populate
6. Complete all sections and submit

## Workflow Execution

When you create a pull request:
1. **Relevant workflow triggers** based on changed files
2. **Automated checks run** (build, test, lint)
3. **Results appear** as status checks on the PR
4. **All checks must pass** before merging is allowed

### Workflow Status Checks

- ✅ **Success**: All checks passed, safe to merge
- ⏳ **In Progress**: Checks still running
- ❌ **Failed**: One or more checks failed, review logs

## Local Development

### Before Pushing
1. Run tests locally: `dotnet test`
2. Build the solution: `dotnet build --configuration Release`
3. Fix any issues before pushing
4. Create a descriptive commit message following the format in [CONTRIBUTING.md](../CONTRIBUTING.md)

### Common Workflow Issues

**Workflow Not Running**
- Check file path matches workflow trigger paths
- Verify branch is `main` or `develop`
- Check workflow is enabled in Actions tab

**Build Failures**
- Ensure dependencies are installed
- Check .NET version compatibility
- Verify connection strings and configurations
- Review workflow logs for detailed errors

**SQL Syntax Errors**
- Use Oracle-compatible SQL syntax
- Add comments explaining complex logic
- Test scripts locally first
- Check SQLFluff output in workflow logs

## References

- [GitHub Workflow Documentation](https://docs.github.com/en/actions/using-workflows)
- [Issue Templates Guide](https://docs.github.com/en/communities/using-templates-to-encourage-useful-issues-and-pull-requests)
- [Pull Request Templates](https://docs.github.com/en/communities/using-templates-to-encourage-useful-issues-and-pull-requests/creating-a-pull-request-template-for-your-repository)

## Team Guidelines

### When Creating Issues
- Search for existing issues first
- Use bug report for actual problems
- Use feature request for improvements
- Provide as much detail as possible
- Include relevant logs or screenshots

### When Creating Pull Requests
- One feature/fix per PR (keep them focused)
- Reference related issues
- Complete all checklist items
- Respond to review feedback promptly
- Keep commit history clean

## Contributing

See [CONTRIBUTING.md](../CONTRIBUTING.md) for complete contribution guidelines.
