# Contributing Guidelines

Thank you for contributing to the CSC12001 project.

## Ground Rule

When documentation conflicts, [docs/designs/Requirements.md](docs/designs/Requirements.md) is the source of truth.

## Workflow

1. Create a branch.
2. Make focused changes.
3. Test what you changed.
4. Update documentation if behavior or setup changed.
5. Open a pull request with a clear summary.

## Commit Message Format

Use:

```text
<type>(<scope>): <subject>
```

Examples:

- `feat(subsystem1): add privilege viewer query`
- `fix(subsystem2): correct doctor VPD filter`
- `docs(database): align setup guide to requirements`

## Coding Notes

- No hardcoded credentials
- Parameterize database access
- Prefer Oracle-enforced security over UI-only checks
- Keep table and column names aligned with the assignment

Example constant naming:

```csharp
private const string SchemaOwner = "HOSPITAL_ADMIN";
```

## Documentation Notes

- Do not describe a custom admin-account database if Oracle itself is the source of truth.
- Do not assume Subsystem 1 source exists in the repo unless it is actually checked in.
- If you add helper schema objects, document them as extensions, not replacements for the required relations.
