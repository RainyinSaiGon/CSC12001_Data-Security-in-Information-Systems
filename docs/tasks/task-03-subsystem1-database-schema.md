# Task 03: Subsystem 1 - Oracle Catalog Queries and Admin Support SQL

**Assigned to:** Duyen, Triet  
**Type:** Database Administration Support  
**Priority:** High  

## Objective

Prepare the Oracle-side support needed for the Subsystem 1 admin UI without inventing custom account-management tables.

This task exists to support the assignment requirements for:

- create, edit, delete user or role
- list users and roles
- grant and revoke privileges
- support `WITH GRANT OPTION`
- inspect granted privileges

## Source-of-Truth Constraints

- Oracle users and roles are managed by Oracle itself.
- Do not create replacement tables such as `ADMIN_USERS` or `ADMIN_ROLES`.
- Use Oracle data dictionary views for inspection.
- Use real Oracle DDL and privilege statements for administration.

## Expected Deliverables

### 1. Data Dictionary Query Set

Queries or views for:

- users
- roles
- system privileges
- object privileges
- column privileges
- role grants
- database objects eligible for permission assignment

Typical sources:

- `DBA_USERS`
- `DBA_ROLES`
- `DBA_SYS_PRIVS`
- `DBA_TAB_PRIVS`
- `DBA_COL_PRIVS`
- `ROLE_ROLE_PRIVS`
- `DBA_OBJECTS`
- `DBA_TAB_COLUMNS`

### 2. Admin Operation SQL

Reusable SQL or PL/SQL wrappers for:

- create user
- alter user
- drop user
- create role
- drop role
- grant role to user
- grant object privilege
- revoke object privilege
- grant column-level `SELECT`
- grant column-level `UPDATE`

### 3. Demo Objects for Permission Testing

Prepare representative objects so the admin UI can demonstrate grants on:

- table
- view
- stored procedure
- function

### 4. Privilege-Inspection Output

Support the UI screens that show:

- what a user has
- what a role has
- who has privileges on a given object
- whether `WITH GRANT OPTION` was used

## Acceptance Criteria

- No custom table is used as the source of truth for Oracle accounts or roles.
- User and role lists come from Oracle dictionary views.
- Column-level grants are available only for `SELECT` and `UPDATE`.
- Procedure and function permissions are handled as executable object permissions.
- Revoke workflows can be demonstrated from the same Oracle-managed data.
- The SQL support layer matches the assignment more closely than the older custom-admin-schema draft.
