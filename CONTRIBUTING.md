# Contributing Guidelines

Thank you for contributing to the CSC12001 project.

## Ground Rule

If documentation conflicts, [docs/designs/Requirements.md](docs/designs/Requirements.md) is the source of truth.

## Before You Change Anything

Read these first:

1. [README.md](README.md)
2. [docs/designs/SETUP_GUIDE.md](docs/designs/SETUP_GUIDE.md)
3. [docs/designs/ARCHITECTURE.md](docs/designs/ARCHITECTURE.md)

## Workflow

1. Create a branch.
2. Make focused changes.
3. Test what you changed.
4. Update documentation if setup, behavior, or script order changed.
5. Open a pull request with a clear summary.

## Commit Message Format

Use:

```text
<type>(<scope>): <subject>
```

Examples:

- `feat(subsystem1): add role grant action`
- `fix(subsystem2): authenticate patients via self view`
- `docs(setup): rewrite clone and run guide`

## Coding Notes

- No hardcoded secrets beyond the intentionally documented demo credentials already used by the checked-in Oracle scripts
- Prefer parameterized database access
- Prefer Oracle-enforced security over UI-only checks
- Keep table and column names aligned with the assignment
- For this repository, assume the working Oracle service is `XEPDB1` unless the local environment proves otherwise

## Documentation Notes

- Keep setup instructions aligned with the real script order that works
- Mention the two-pass OLS setup when editing OLS docs
- Do not describe a custom admin-account database if Oracle itself is the source of truth
- Document unfinished areas honestly, especially Requirement 4 backup and recovery
