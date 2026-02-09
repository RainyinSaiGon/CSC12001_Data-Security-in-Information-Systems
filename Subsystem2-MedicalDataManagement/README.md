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
Subsystem2-MedicalDataManagement/Source/MedicalDataSystem/
├── Forms/                                # [CREATE] UI Forms & Windows
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
├── Models/                               # [CREATE] Entity Models
│   ├── Patient.cs
│   ├── Staff.cs
│   ├── MedicalRecord.cs
│   ├── DiagnosticService.cs
│   ├── Prescription.cs
│   └── Notification.cs
│
├── Services/                             # [CREATE] Business Logic & Database Access
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

### BỆNHNHÂN (Patient)
- MÃBN: Patient ID
- TÊNBN: Patient Name
- PHÁI: Gender
- NGÀYSINH: Birth Date
- CCCD: ID Number
- Address fields, Medical history, Drug allergies

### NHÂNVIÊN (Staff)
- MÃNV: Staff ID
- HỌTÊN: Full Name
- VAITRÒ: Role (Coordinator, Doctor/Nurse, Technician)
- CHUYÊNKHOA: Specialty

### HSBA (Medical Record)
- MÃHSBA: Record ID
- MÃBN: Patient Reference
- CHẨNĐOÁN: Diagnosis
- ĐIỀUTRỊ: Treatment
- KẾTLUẬN: Conclusion

### HSBA_DV (Diagnostic Service)
- Service type, Date, Results

### ĐƠNTHUỐC (Prescription)
- Drug name, Dosage, Instructions

## Security Implementation

### 1. RBAC (Role-Based Access Control)
Roles: Coordinator, Doctor/Nurse, Technician, Patient

### 2. VPD (Virtual Private Database)
- Doctors see only their patients' records
- Coordinators see assigned records
- Technicians see assigned services

### 3. OLS (Oracle Label Security)
- Hospital locations: Hồ Chí Minh, Hải Phòng, Hà Nội
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
- .NET Framework 4.7.2+
- Visual Studio 2019+
- Oracle ODP.NET
- Oracle Database with configured RBAC, VPD, OLS

### Setup

1. Run database schema scripts
2. Execute security setup scripts
3. Open solution in Visual Studio
4. Install ODP.NET: `Install-Package Oracle.ManagedDataAccess`
5. Build and run

### Database Configuration
```bash
cd Database/Schema
sqlplus project_admin@orcl @01_CreateTables.sql
sqlplus project_admin@orcl @02_CreateIndexes.sql
sqlplus project_admin@orcl @03_InsertSampleData.sql

cd ../Security
sqlplus project_admin@orcl @01_RBAC_Setup.sql
sqlplus project_admin@orcl @02_VPD_Setup.sql
sqlplus project_admin@orcl @03_OLS_Setup.sql
sqlplus project_admin@orcl @04_Users_Creation.sql
```

### Configure Connection Strings

⚠️ **Security Warning**: Never commit passwords or credentials to version control. See [CONTRIBUTING.md](../../CONTRIBUTING.md#security-checklist) security guidelines.

Use one of these methods to securely provide database credentials:

**Option 1: User Secrets (Development)**
```bash
dotnet user-secrets init
dotnet user-secrets set "OracleDbConnection:UserId" "project_admin"
dotnet user-secrets set "OracleDbConnection:Password" "your_secure_password"
```

**Option 2: Environment Variables (Production)**
```bash
# Windows
set ORACLE_USERID=project_admin
set ORACLE_PASSWORD=your_secure_password

# Linux/macOS
export ORACLE_USERID=project_admin
export ORACLE_PASSWORD=your_secure_password
```

**Option 3: Local Config File (Development Only)**
Create `appsettings.local.json` (add to `.gitignore`) with credentials.

See [Subsystem1-OracleDBAdmin/README.md](../Subsystem1-OracleDBAdmin/README.md#database-connection) for detailed configuration examples.

## Features by Role

### Coordinator Dashboard
- [ ] Patient management (add/edit)
- [ ] Medical record assignment
- [ ] Doctor & technician coordination
- [ ] Record status tracking

### Doctor Dashboard
- [ ] Patient lookup (assigned)
- [ ] Medical history review
- [ ] Diagnosis & treatment input
- [ ] Prescription management
- [ ] Service ordering

### Technician Dashboard
- [ ] Service list for assignment
- [ ] Result data entry
- [ ] Service completion

### Patient Portal
- [ ] Personal information view/edit
- [ ] Medical history review
- [ ] Appointment tracking
- [ ] Notification viewing

## Testing Scenarios

### Test Case 1: User Setup
- [ ] Users created per role
- [ ] Accounts accessible
- [ ] Password policies enforced

### Test Case 2: RBAC
- [ ] Technician can't view doctors' functions
- [ ] Patient can only see own data
- [ ] Coordinator can assign records

### Test Case 3: VPD
- [ ] Doctor A can't see Doctor B's patients
- [ ] Transparent filtering works
- [ ] Performance acceptable

### Test Case 4: Audit
- [ ] Sensitive operations logged
- [ ] Unauthorized access blocked
- [ ] Audit trails readable

## Development

### Code Standards
- Follow Microsoft C# guidelines
- XML documentation on public methods
- Proper exception handling
- Input validation for all forms

### Building
```bash
dotnet build MedicalDataSystem.sln
```

### Testing
- Test each role's functionality separately
- Verify data isolation between users
- Check audit logging
- Performance test with 100k patients

## Troubleshooting

### Permission Denied Errors
- Check user role in NHÂNVIÊN table
- Verify VPD policies applied
- Check RBAC grants

### Data Not Displaying
- Verify VPD filters
- Check audit trail for access denials
- Confirm user's organization level

### Performance Issues
- Check index creation
- Verify VPD policy efficiency
- Monitor query execution

## References

- [Oracle RBAC Documentation](https://docs.oracle.com/database/121/DBSEG/authorization.htm)
- [Oracle VPD Guide](https://docs.oracle.com/database/121/DBSEG/vpd.htm)
- [Oracle Label Security](https://docs.oracle.com/database/121/DBSEG/label_security.htm)
- [HIPAA Compliance](https://www.hhs.gov/hipaa/)
