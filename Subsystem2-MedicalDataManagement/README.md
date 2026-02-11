# Subsystem 2: Medical Data Management System

WinForm-based medical data management system with role-based access control, virtual private database, and Oracle Label Security for a hospital management platform.

## Overview

A comprehensive HIPAA-compliant medical management system with:

- Patient record management
- Medical history tracking
- Doctor consultation records
- Diagnostic service management
- Prescription management
- Multi-level access control (RBAC, VPD, OLS)
- Comprehensive audit trails

## Architecture

### Intended Project Structure

The following architecture outlines the planned directory structure for this application. Create these files and folders as you implement features:

```
subsystem2-medicalDataManagement/source/medicalDataSystem/
├── forms/                                # [CREATE] UI Forms & Windows
│   ├── LoginForm.cs                     # User authentication
│   ├── LoginForm.Designer.cs
│   ├── MainForm.cs                      # Main application window
│   ├── MainForm.Designer.cs
│   ├── CoordinatorForm.cs               # Coordinator UI (RBAC)
│   ├── CoordinatorForm.Designer.cs
│   ├── DoctorForm.cs                    # Doctor UI (VPD)
│   ├── DoctorForm.Designer.cs
│   ├── TechnicianForm.cs                # Technician UI (RBAC)
│   ├── TechnicianForm.Designer.cs
│   ├── PatientForm.cs                   # Patient UI (Row-level)
│   ├── PatientForm.Designer.cs
│   └── NotificationForm.cs              # OLS notification viewer
│
├── models/                               # [CREATE] Entity Models
│   ├── Patient.cs
│   ├── Staff.cs
│   ├── MedicalRecord.cs
│   ├── DiagnosticService.cs
│   ├── Prescription.cs
│   └── Notification.cs
│
├── services/                             # [CREATE] Business Logic & Database Access
│   ├── AuthenticationService.cs         # Login & session management
│   ├── OracleConnectionService.cs       # Database connection management
│   ├── RBACService.cs                   # Role-based access control
│   ├── VPDService.cs                    # Virtual private database filtering
│   ├── OLSService.cs                    # Label security management
│   ├── PatientService.cs                # Patient operations
│   ├── DoctorService.cs                 # Doctor operations
│   ├── CoordinatorService.cs            # Coordinator operations
│   ├── AuditService.cs                  # Audit logging
│   └── ValidationService.cs             # Input validation & error handling
│
├── Program.cs                            # Application entry point
├── App.config                            # Application configuration
└── MedicalDataSystem.csproj             # Project file
```

### File Creation Guide

When implementing features in order:

1. **Start with Models** — Define data structures and entity classes
2. **Create Services** — Implement business logic and Oracle database access
3. **Build Forms** — Create UI forms that use services
4. **Add Program.cs** — Main entry point and application initialization

See [Development](#development) section below for implementation details.

## Entities & Schema

### BENHNHAN (Patient)

- MABENHNHAN: Patient ID
- HOTEN: Patient Name
- PHAI: Gender
- NGAYSINH: Birth Date
- CCCD: ID Number
- Address fields (DIACHI), Medical history (TIENSUBENH), Drug allergies (DIUNG)

### NHANVIEN (Staff)

- MANV: Staff ID
- HOTEN: Full Name
- VAITRO: Role ('Điều phối viên', 'Bác sĩ/Y sĩ', 'Kỹ thuật viên')
- CHUYENKHOA: Specialty

### HSBA (Medical Record)

- MAHSBA: Record ID
- MABENHNHAN: Patient Reference
- CHANDOAN: Diagnosis
- DIEUTRI: Treatment
- KETLUAN: Conclusion
- NGAYTAO: Creation Date
- MABACSI: Doctor Assigned

### HSBA_DV (Diagnostic Service)

- MADICHVU: Service ID
- MAHSBA: Record Reference
- TENDICHVU: Service Name
- NGAY: Date
- KETQUA: Result
- HOANTHANH: Status
- MAKYTHUATVIEN: Technician Performing

### DONTHUOC (Prescription)

- MADONTHUOC: Prescription ID
- MAHSBA: Record Reference
- TENTHUOC: Drug Name
- LIEUDUNG: Dosage
- HUONGDAN: Instructions
- NGAYDANGKY: Date

## Security Implementation

### 1. RBAC (Role-Based Access Control)

Roles: Coordinator, Doctor/Nurse, Technician, Patient

### 2. VPD (Virtual Private Database)

- Doctors see only their patients' records
- Coordinators see assigned records
- Technicians see assigned services

### 3. OLS (Oracle Label Security)

- Hospital locations: Ho Chi Minh, Hai Phong, Ha Noi
- Departments: Cardiology, Gastroenterology, Neurology
- Hierarchy: Director > Department Head > Staff

### 4. Audit Mechanisms

- Standard audit for user actions
- Fine-grained audit for sensitive fields
- Unified audit for compliance

## User Roles

### Coordinator (20 staff)

- View/add/edit patients
- Create medical records
- Assign doctors and technicians
- Manage record assignments

### Doctor/Nurse (100 staff)

- View patient history & allergies
- Create/modify diagnoses & treatments
- Order diagnostic services
- Manage prescriptions
- Update patient medical history

### Technician (50 staff)

- View assigned services
- Update diagnostic results
- Track service completion

### Patient (100,000 users)

- View own medical records
- Update own contact information
- View prescriptions
- View appointment history

## Getting Started

### Prerequisites

- .NET 10.0 SDK or higher
- Visual Studio 2022 or later
- Oracle Data Provider for .NET Core (ODP.NET Core)
- Oracle Database Express 21c (XE) with configured RBAC, VPD, OLS

### Setup

1. Run database schema scripts
2. Execute security setup scripts
3. Open solution in Visual Studio 2022
4. Install ODP.NET Core:

   ```bash
   dotnet add package Oracle.ManagedDataAccess.Core
   # Or in Package Manager Console:
   Install-Package Oracle.ManagedDataAccess.Core
   ```

5. Build and run

### Database Configuration

```bash
# For Oracle 21c XE:
cd database/schema
sqlplus project_admin/your_password@localhost:1521/XE @01_CreateTables.sql
sqlplus project_admin/your_password@localhost:1521/XE @02_CreateIndexes.sql
sqlplus project_admin/your_password@localhost:1521/XE @03_InsertSampleData.sql

cd ../security
sqlplus project_admin/your_password@localhost:1521/XE @01_RBAC_Setup.sql
sqlplus project_admin/your_password@localhost:1521/XE @02_VPD_Setup.sql
sqlplus project_admin/your_password@localhost:1521/XE @03_OLS_Setup.sql
sqlplus project_admin/your_password@localhost:1521/XE @04_Users_Creation.sql
```

### Configure Connection Strings

**Security Warning**: Never commit passwords or credentials to version control. See [CONTRIBUTING.md](../../CONTRIBUTING.md#security-checklist) security guidelines.

For detailed setup instructions on configuring connection strings (User Secrets, Environment Variables, Local Config), see [docs/SETUP_GUIDE.md](../../docs/SETUP_GUIDE.md#step-3-configure-connection-strings).

## Features by Role

### Coordinator Dashboard

- Patient management (add/edit/delete)
- Medical record creation and assignment
- Doctor & technician coordination
- Record status tracking and workflow
- Comprehensive patient database access

### Doctor Dashboard  

- Patient lookup (assigned patients only - VPD filtered)
- Medical history review and allergies
- Diagnosis & treatment input and management
- Prescription creation and updates
- Diagnostic service ordering
- Patient consultation tracking

### Technician Dashboard

- Service list (assigned services only - VPD filtered)
- Result data entry and tracking
- Service completion marking
- Test result management
- Equipment/resource tracking

### Patient Portal

- Personal information view and edit
- Medical history review (read-only)
- Appointment tracking and scheduling
- Notification viewing (OLS label filtered)
- Prescription access

### Notification System

- Label-based filtering (Department, Location, Classification)
- Department-wide notifications
- Location-specific messages
- Classification-level access control

## Development

### Code Standards

- Follow Microsoft C# coding guidelines
- Named parameters for clarity
- Meaningful variable names and method names
- Exception handling with specific exception types
- Input validation on all user inputs
- XML documentation on public methods
- Proper null checking and defensive programming

### Building and Running

```bash
# Build the solution
cd subsystem2-medicalDataManagement/source
dotnet build MedicalDataSystem.slnx

# Run the application
dotnet run --project MedicalDataSystem/MedicalDataSystem.csproj

# Run as release build
dotnet run --project MedicalDataSystem/MedicalDataSystem.csproj --configuration Release
```

### Testing Strategy

**Unit Testing:** Test individual services with mock data

- `AuthenticationService`: Test login with valid/invalid credentials
- `RBACService`: Test role permissions
- `PatientService`: Test patient data retrieval

**Integration Testing:** Test form-service interactions

- LoginForm with AuthenticationService
- CoordinatorForm with CoordinatorService
- DoctorForm with VPD filtering

**Security Testing:** Verify access control mechanisms

- RBAC: User can only perform role-appropriate actions
- VPD: Users see only authorized rows (transparent filtering)
- OLS: Notifications filtered by user labels
- Audit: All sensitive operations logged

**Performance Testing:** Test with realistic data volumes

- 100,000+ patient records
- 170+ staff members
- 1M+ medical records and services
- Verify query response times < 500ms

## Troubleshooting

### Authentication Issues

**Problem:** "Invalid username/password" error

- **Solution**: Verify user exists in NHANVIEN table
- **Solution**: Check password is correct
- **Solution**: Ensure user account is enabled in database

**Problem:** "Role not determined after login"

- **Solution**: Verify VAITRO column populated in NHANVIEN
- **Solution**: Check AuthenticationService retrieves role correctly
- **Solution**: Ensure user has valid role (Coordinator/Doctor/Technician/Patient)

### Permission Denied Errors

**Problem:** "User does not have permission for this action"

- **Solution**: Check user role in NHANVIEN table
- **Solution**: Verify RBAC roles created in database
- **Solution**: Check RBAC grants for user's role

**Problem:** "Access denied" error when viewing data

- **Solution**: Verify VPD policies applied to HSBA, HSBA_DV tables
- **Solution**: Check user-doctor assignment table exists
- **Solution**: Verify database session context properly configured

### Data Not Displaying

**Problem:** Forms show no data even when data exists

- **Solution**: Verify VPD filters (doctor should see assigned patients)
- **Solution**: Check audit trail for access denials
- **Solution**: Confirm user's organization level and assignments
- **Solution**: Verify connection string and database connectivity

**Problem:** OLS labels not filtering notifications properly

- **Solution**: Verify user labels assigned in database
- **Solution**: Check notification labels match user levels
- **Solution**: Verify label hierarchy configured correctly
- **Solution**: Test with direct SQL query to verify filtering

### Performance Issues

**Problem:** Slow data loading or timeouts

- **Solution**: Check if indexes created (database/schema/02_CreateIndexes.sql)
- **Solution**: Verify VPD policy efficiency (may need index on MANV in VPD policies)
- **Solution**: Monitor database query plans
- **Solution**: Consider table statistics updates

**Problem:** Form freezes during data retrieval

- **Solution**: Implement async/await for database calls
- **Solution**: Add progress indicators for long operations
- **Solution**: Consider paging for large result sets
- **Solution**: Check for blocking locks in database

## References

### Oracle Documentation

- [Oracle Role-Based Access Control](https://docs.oracle.com/database/121/DBSEG/authorization.htm)
- [Oracle Virtual Private Database](https://docs.oracle.com/database/121/DBSEG/vpd.htm)
- [Oracle Label Security](https://docs.oracle.com/database/121/DBSEG/label_security.htm)
- [Oracle Audit & Compliance](https://docs.oracle.com/database/121/DBSEG/audit.htm)
- [Oracle ODP.NET Documentation](https://www.oracle.com/database/technologies/appdev/dotnet.html)

### Healthcare & Compliance

- [HIPAA Security Rule](https://www.hhs.gov/hipaa/for-professionals/security/)
- [HIPAA Minimum Necessary Principle](https://www.hhs.gov/hipaa/for-professionals/privacy/)
- [GDPR Data Protection](https://gdpr-info.eu/)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)

### Security Best Practices

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [CIS Database Security Best Practices](https://www.cisecurity.org/benchmarks/databases)
- [SQL Injection Prevention](https://owasp.org/www-community/attacks/SQL_Injection)

## Support & Contact

For questions or issues related to this subsystem:

1. Check [Troubleshooting](#troubleshooting) section above
2. Review [Subsystem Architecture](#architecture)
3. Consult [docs/SETUP_GUIDE.md](../../docs/SETUP_GUIDE.md)
4. Contact project lead for complex issues
