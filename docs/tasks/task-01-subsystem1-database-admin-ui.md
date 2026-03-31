# Task 01: Subsystem 1 - Admin App Completion

**Suggested owner:** 1 developer  
**Priority:** High  
**Focus:** finish the Oracle admin application for Requirement 1

## Goal

Complete the checked-in admin app in `Subsystem1-OracleDBAdmin` so it clearly demonstrates:

- listing Oracle users
- listing Oracle roles
- creating and dropping users
- creating and dropping roles
- granting privileges
- revoking privileges
- viewing granted privileges

## Current State

The project already contains a working Subsystem 1 codebase, but it still needs a cleanup pass against the assignment wording.

Most likely remaining gaps:

- polish the user and role management flow
- verify revoke flows are complete
- verify role-to-user grant and revoke flow
- verify object privilege inspection is clear enough for the demo

## Main Files

- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Form1.cs`
- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Services/UserService.cs`
- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Services/RoleService.cs`
- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Services/PermissionService.cs`
- `Subsystem1-OracleDBAdmin/Source/OracleDBAdmin/Services/PrivilegeService.cs`

## Expected Deliverables

- working admin UI for the assignment demo
- clean create, drop, grant, revoke flows
- clear privilege display for users and roles
- short demo notes or screenshots if needed

## Acceptance Criteria

- user list comes from Oracle, not custom account tables
- role list comes from Oracle
- create and drop user works
- create and drop role works
- grant role to user works
- revoke role from user works or is clearly covered
- grant and revoke object privilege works
- privilege viewer shows real Oracle grants
