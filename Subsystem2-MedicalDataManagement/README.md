# Subsystem 2: Medical Data Management System

WinForm-based HIPAA-compliant medical data management system with role-based access control (RBAC), virtual private database (VPD), and Oracle Label Security (OLS) for hospital operations.

## Overview

Comprehensive medical management platform providing:

* Patient record and medical history management
* Doctor consultation and diagnosis tracking
* Diagnostic service and prescription management
* Multi-tier access control (RBAC, VPD, OLS)
* Comprehensive audit trails and compliance logging
* 100,000+ patient capacity with 170+ staff support

## Features

* **Patient Management** - Create, update, view patient records and medical history
* **Medical Records** - Complete diagnosis, treatment, and consultation tracking
* **Diagnostic Services** - Order and manage laboratory and diagnostic tests
* **Prescriptions** - Create and track medications with dosage instructions
* **Role-Based Access** - Coordinator, Doctor/Nurse, Technician, Patient roles
* **Virtual Private Database** - Row-level filtering by user assignment
* **Label Security** - Location and department-based access control
* **Audit Trail** - Standard, fine-grained, and unified audit logging

## Project Structure

```
subsystem2-medicalDataManagement/source/medicalDataSystem/
├── forms/
│   ├── LoginForm.cs                    # User authentication
│   ├── MainForm.cs                     # Application window
│   ├── CoordinatorForm.cs              # Coordinator UI
│   ├── DoctorForm.cs                   # Doctor UI
│   ├── TechnicianForm.cs               # Technician UI
│   ├── PatientForm.cs                  # Patient UI
│   └── NotificationForm.cs             # OLS notification viewer
├── models/
│   ├── Patient.cs
│   ├── Staff.cs
│   ├── MedicalRecord.cs
│   ├── DiagnosticService.cs
│   ├── Prescription.cs
│   └── Notification.cs
├── services/
│   ├── AuthenticationService.cs
│   ├── OracleConnectionService.cs
│   ├── RBACService.cs
│   ├── VPDService.cs
│   ├── OLSService.cs
│   ├── PatientService.cs
│   ├── DoctorService.cs
│   ├── CoordinatorService.cs
│   ├── AuditService.cs
│   └── ValidationService.cs
├── Program.cs
└── MedicalDataSystem.csproj
```

## Database Schema

**BENHNHAN (Patient):** Patient ID, name, gender, birth date, national ID, address, medical history, allergies, linked Oracle user

**NHANVIEN (Staff):** Staff ID, full name, gender, birth date, ID number, phone, role, department, linked Oracle user

**HSBA (Medical Record):** Record ID, patient reference, record date, diagnosis, treatment, conclusion, assigned doctor, department

**HSBA_DV (Diagnostic Service):** Composite key (MAHSBA, LOAIDV, NGAYDV), service type, test result, assigned technician

**DONTHUOC (Prescription):** Composite key (MAHSBA, TENTHUOC, NGAYDT), drug name, dosage and instructions

## Security Implementation

**RBAC (Role-Based Access Control):**

* Coordinator (20 staff) - Patient management and record assignment
* Doctor/Nurse (100 staff) - Diagnosis, treatment, and prescription management
* Technician (50 staff) - Diagnostic service execution and results
* Patient (100,000 users) - Self-service medical records access

**VPD (Virtual Private Database):**

* Doctors see only assigned patient records
* Coordinators see assigned medical records
* Technicians see assigned service requests

**OLS (Oracle Label Security):**

* Hospital locations: Ho Chi Minh, Hai Phong, Ha Noi
* Departments: Cardiology, Gastroenterology, Neurology
* Hierarchy: Director > Department Head > Staff

**Audit Mechanisms:**

* Standard audit for user actions
* Fine-grained audit for sensitive fields
* Unified audit for compliance reporting

## Getting Started

**Prerequisites:**

* .NET 10.0 SDK or higher
* Visual Studio 2022 or later
* Oracle Data Provider for .NET Core (ODP.NET Core)
* Oracle Database Express 21c (XE) with proper configuration

**Database Setup:**

```bash
# Run database schema scripts
cd database/Subsystem2-MedicalDB/schema
sqlplus project_admin/password@localhost:1521/XE @01_CreateTables.sql
sqlplus project_admin/password@localhost:1521/XE @02_CreateIndexes.sql
sqlplus project_admin/password@localhost:1521/XE @03_InsertSampleData.sql

# Run security setup scripts
cd ../security
sqlplus project_admin/password@localhost:1521/XE @01_Users_Creation.sql
sqlplus project_admin/password@localhost:1521/XE @02_RBAC_Setup.sql
sqlplus project_admin/password@localhost:1521/XE @03_VPD_Setup.sql
sqlplus project_admin/password@localhost:1521/XE @04_OLS_Setup.sql
```

**Application Setup:**

1. Open solution in Visual Studio 2022
2. Install Oracle data provider: `dotnet add package Oracle.ManagedDataAccess.Core`
3. Configure connection string (see SETUP_GUIDE.md)
4. Build and run

**Security Notice:**

Never commit credentials to version control. Use User Secrets, environment variables, or configuration files. See [CONTRIBUTING.md](../../CONTRIBUTING.md#security-checklist) for guidelines.

## User Interfaces

**Coordinator Dashboard:**

* Patient management (add/edit/delete)
* Medical record creation and assignment
* Doctor and technician coordination
* Record status tracking and workflow
* Comprehensive patient database access

**Doctor Dashboard:**

* Patient lookup (assigned patients only via VPD)
* Medical history review and drug allergy tracking
* Diagnosis and treatment input
* Prescription creation and management
* Diagnostic service ordering
* Consultation tracking

**Technician Dashboard:**

* Service list (assigned services only via VPD)
* Result data entry and tracking
* Service completion marking
* Test result management
* Resource and equipment tracking

**Patient Portal:**

* Personal information view and edit
* Medical history review (read-only)
* Appointment tracking and scheduling
* Prescription access and tracking
* Notification system with label-based filtering

## Development

**Code Standards:**

* Follow Microsoft C# coding guidelines
* Use meaningful variable and method names
* Add XML documentation to public methods
* Implement exception handling with specific exception types
* Validate all user inputs
* Implement proper null checking and defensive programming

**Building:**

```bash
cd subsystem2-medicalDataManagement/source
dotnet build MedicalDataSystem.slnx
dotnet run --project MedicalDataSystem/MedicalDataSystem.csproj
```

**Testing Strategy:**

* Unit tests for individual services with mock data
* Integration tests for form-service interactions
* Security validation (RBAC, VPD, OLS enforcement)
* Performance testing with realistic data volumes (100,000+ patients)
* Query response time verification (< 500ms target)

## Troubleshooting

**Authentication Issues:**

* Invalid username/password → Verify user exists in NHANVIEN table and check password
* Role not determined → Verify VAITRO column populated and role is valid (Coordinator/Doctor/Technician/Patient)

**Permission Denied Errors:**

* User lacks access → Check user role in NHANVIEN and verify RBAC database grants
* Access denied viewing data → Verify VPD policies applied and doctor-patient assignments exist

**Data Not Displaying:**

* No data in forms → Verify VPD filtering and check if user has assigned patients/services
* OLS labels not filtering → Verify user labels assigned in database and label hierarchy configured

**Performance Issues:**

* Slow data loading → Verify indexes created and check VPD policy efficiency
* Form freezes → Consider implementing async/await for database calls and add progress indicators

## References

**Oracle Database:**

* [Oracle Role-Based Access Control](https://docs.oracle.com/database/121/DBSEG/authorization.htm)
* [Oracle Virtual Private Database](https://docs.oracle.com/database/121/DBSEG/vpd.htm)
* [Oracle Label Security](https://docs.oracle.com/database/121/DBSEG/label_security.htm)
* [Oracle Audit and Compliance](https://docs.oracle.com/database/121/DBSEG/audit.htm)

**Healthcare Compliance:**

* [HIPAA Security Rule](https://www.hhs.gov/hipaa/for-professionals/security/)
* [HIPAA Privacy Rule](https://www.hhs.gov/hipaa/for-professionals/privacy/)
* [GDPR Data Protection](https://gdpr-info.eu/)
* [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)

**Security Standards:**

* [OWASP Top 10](https://owasp.org/www-project-top-ten/)
* [CIS Database Security Benchmarks](https://www.cisecurity.org/benchmarks/databases)
* [SQL Injection Prevention](https://owasp.org/www-community/attacks/SQL_Injection)
