# Task 02: Subsystem 1 - Service Layer Hardening

**Suggested owner:** same developer as Task 01  
**Priority:** High  
**Focus:** make the admin app service layer match the final UI flow

## Goal

Review and finish the backend services that support the Oracle admin app.

## Current State

The repository already includes the main services, but they should be reviewed against the final Requirement 1 demo.

Likely checks:

- role revoke support
- privilege revoke support
- object and column privilege coverage
- clear validation and error messages

## Main Files

- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Services/OracleConnectionService.cs`
- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Services/ValidationService.cs`
- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Services/UserService.cs`
- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Services/RoleService.cs`
- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Services/PermissionService.cs`
- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Services/PrivilegeService.cs`

## Deliverables

- service methods aligned with the admin UI
- no missing grant or revoke path needed by the demo
- safe parameterized SQL wherever possible
- updated error handling if any admin action is confusing

## Acceptance Criteria

- services support the final admin UI without manual SQL workarounds
- role and privilege revoke cases are covered
- object privilege queries show correct results
- code builds cleanly
