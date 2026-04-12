# Subsystem 2: Medical Data Management

This folder contains the medical WinForms client for the hospital scenario.

## What This App Does

The app supports these business users:

- coordinator
- doctor
- technician
- patient

Security is intended to be enforced by Oracle:

- RBAC for technician and patient cases
- VPD for coordinator and doctor cases
- OLS for `THONGBAO`
- audit for sensitive operations

## Build

```powershell
dotnet restore "subsystem2-medicalDataManagement/source/medicalDataSystem/MedicalDataSystem.csproj"
dotnet build "subsystem2-medicalDataManagement/source/medicalDataSystem/MedicalDataSystem.csproj"
```

Run:

```powershell
dotnet run --project "subsystem2-medicalDataManagement/source/medicalDataSystem/MedicalDataSystem.csproj"
```

## Database Requirements

Before running the app, complete the Oracle setup from [docs/designs/SETUP_GUIDE.md](../docs/designs/SETUP_GUIDE.md).

Minimum required scripts:

- [Create_HOSPITAL_ADMIN.sql](../database/Subsystem2-MedicalDB/Create_HOSPITAL_ADMIN.sql)
- [01_CreateTables.sql](../database/Subsystem2-MedicalDB/schema/01_CreateTables.sql)
- [02_CreateIndexes.sql](../database/Subsystem2-MedicalDB/schema/02_CreateIndexes.sql)
- [03_InsertSampleData.sql](../database/Subsystem2-MedicalDB/schema/03_InsertSampleData.sql)
- [01_RBAC_Setup.sql](../database/Subsystem2-MedicalDB/security/01_RBAC_Setup.sql)
- [02_VPD_Setup.sql](../database/Subsystem2-MedicalDB/security/02_VPD_Setup.sql)
- [03_OLS_Setup.sql](../database/Subsystem2-MedicalDB/security/03_OLS_Setup.sql)

## Login Notes

Use:

```text
localhost:1521/XEPDB1
```

The current login textbox may still default to `localhost:1521/XE`. Replace it manually with `localhost:1521/XEPDB1`.

Do not log in as `HOSPITAL_ADMIN`. Use the business accounts created by the RBAC script:

- `990000000001 / 990000000001`
- `990000000021 / 990000000021`
- `990000000121 / 990000000121`
- `000000000001 / 000000000001`

`USERNAME` is now mapped to CCCD for both staff and patients. Passwords use CCCD as the initial input and are lazily stored as bcrypt hashes in `PASSWORD_HASH` on first successful login.

## Implementation Note

The app resolves shared objects through the `HOSPITAL_ADMIN` schema at session level, so runtime Oracle users can work with the schema-owned tables, views, policies, and packages.

That is why the medical app must connect with real Oracle user accounts instead of one shared admin account.
