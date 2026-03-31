# Task 03: Subsystem 1 - Oracle Admin Support Queries

**Suggested owner:** same developer as Task 01  
**Priority:** Medium  
**Focus:** verify the SQL support used by the admin app

## Goal

Keep the Oracle-side support for the admin app simple and requirement-aligned.

## Important Rule

- do not create replacement account tables
- Oracle itself is the source of truth for users, roles, and privileges

## What To Verify

- user queries use Oracle catalog views
- role queries use Oracle catalog views
- granted privilege queries are clear enough for the UI
- any helper SQL still matches the current app behavior

## Useful Oracle Views

- `DBA_USERS`
- `DBA_ROLES`
- `DBA_SYS_PRIVS`
- `DBA_TAB_PRIVS`
- `DBA_COL_PRIVS`
- `DBA_ROLE_PRIVS`
- `DBA_OBJECTS`
- `DBA_TAB_COLUMNS`

## Acceptance Criteria

- no fake account-management schema is introduced
- admin UI data comes from real Oracle dictionary sources
- support SQL matches the current app features
