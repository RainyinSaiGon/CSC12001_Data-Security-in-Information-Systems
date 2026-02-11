# Subsystem 2: Medical Data Management Database

**Purpose:** Core medical and healthcare data for the Medical Data Management system.

This database stores all patient records, staff information, medical histories, prescriptions, and diagnostic services.

## Overview

The Subsystem 2 Medical Database contains:
- Patient information (demographics, medical history)
- Staff/employee records with role assignments
- Medical records and examination history
- Diagnostic services and results
- Prescriptions and medication information
- Notifications (with Oracle Label Security for multi-level access)
- Audit trail of all data access and modifications

## Database Features

### Security Mechanisms
1. **RBAC (Role-Based Access Control):** 4 roles - Coordinator, Doctor/Nurse, Technician, Patient
2. **VPD (Virtual Private Database):** Row-level filtering for sensitive data
3. **OLS (Oracle Label Security):** Classification-based access control for notifications
4. **Audit Trail:** Comprehensive audit logging of all data modifications

### Tables

| Table | Purpose | Security Method |
|-------|---------|-----------------|
| BENHNHAN | Patient demographics | RBAC + VPD (Patients see own data) |
| NHANVIEN | Staff information | RBAC |
| HSBA | Medical records (exams) | RBAC + VPD (filtered by assignment) |
| HSBA_DV | Diagnostic services | RBAC + VPD |
| DONTHUOC | Prescriptions | RBAC + VPD |
| THONGBAO | Notifications | RBAC + OLS (label-based) |
| AUDITLOG | Audit trail | RBAC (DBA/Audit only) |

## Planned Directory Structure

```
Subsystem2-MedicalDB/
├── schema/                            
│   ├── 01_CreateTables.sql           # Create all 7 medical data tables
│   ├── 02_CreateIndexes.sql          # Performance indexes
│   └── 03_InsertSampleData.sql       # 100 patients, 170 staff, sample records
├── security/                          
│   ├── 01_Users_Creation.sql         # Create 4 role users
│   ├── 02_RBAC_Setup.sql             # Define 4 roles with permissions
│   ├── 03_VPD_Setup.sql              # Row-level policies
│   └── 04_OLS_Setup.sql              # Label security for notifications
├── audit/                             
│   ├── 01_StandardAudit_Setup.sql    # System audit
│   ├── 02_FineGrainedAudit_Setup.sql # Fine-grained audit (specific operations)
│   ├── 03_UnifiedAudit_Setup.sql     # Unified audit trail
│   └── ReadAuditLogs.sql             # Query audit logs
└── README.md                          (This file)
```

## Test Cases

Each role is validated with specific test cases:

| Test Case | Role | Security Type | Validation |
|-----------|------|---------------|-----------|
| TC#1 | User Setup | N/A | 170 staff created with correct roles |
| TC#2 | COORDINATOR | RBAC | Can view all patients, assign roles |
| TC#3 | DOCTOR/NURSE | VPD + RBAC | Can only see assigned patients |
| TC#4 | TECHNICIAN | RBAC + VPD | Can view services for assigned records |
| TC#5 | PATIENT | VPD + RBAC | Can only see own medical record |

## Execution Order

### 1. Create Medical Schema (Run First)
```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

@Subsystem2-MedicalDB/schema/01_CreateTables.sql
@Subsystem2-MedicalDB/schema/02_CreateIndexes.sql
@Subsystem2-MedicalDB/schema/03_InsertSampleData.sql
```

### 2. Configure Medical Security (Run Second - CRITICAL ORDER)
```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

-- Step 1: CREATE USERS FIRST
@Subsystem2-MedicalDB/security/01_Users_Creation.sql

-- Step 2: Create and assign roles
@Subsystem2-MedicalDB/security/02_RBAC_Setup.sql

-- Step 3: Configure row-level security policies
@Subsystem2-MedicalDB/security/03_VPD_Setup.sql

-- Step 4: Configure label-based security
@Subsystem2-MedicalDB/security/04_OLS_Setup.sql
```

### 3. Setup Audit Logging (Run Third)
```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

@Subsystem2-MedicalDB/audit/01_StandardAudit_Setup.sql
@Subsystem2-MedicalDB/audit/02_FineGrainedAudit_Setup.sql
@Subsystem2-MedicalDB/audit/03_UnifiedAudit_Setup.sql
```

## Connection String (MedicalDataSystem Application)

```csharp
// Connection to Subsystem 2 Medical Database
string connectionString = "Data Source=(DESCRIPTION="
                        + "(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))"
                        + "(CONNECT_DATA=(SERVICE_NAME=XE)));"
                        + "User Id=COORDINATOR001;Password=<password>;";
```

## Security Overview

### Four Roles: 'Điều phối viên', 'Bác sĩ/Y sĩ', 'Kỹ thuật viên', 'Bệnh nhân'

#### 'Điều phối viên' (Coordinator) - RBAC Only
- View all patients
- Assign doctors to patients
- Assign technicians to services
- Access all medical records
- Issue notifications
- No row-level restrictions

#### 'Bác sĩ/Y sĩ' (Doctor/Nurse) - RBAC + VPD
- View only assigned patients (VPD: filter by doctor assignments)
- Create/update medical records for own patients
- View diagnostic services for own patients
- Issue prescriptions
- Cannot see other doctors' patients

#### 'Kỹ thuật viên' (Technician) - RBAC + VPD
- View diagnostic services for assigned records
- Update service results
- Cannot view patient demographics
- Cannot view prescriptions
- Limited to assigned services

#### 'Bệnh nhân' (Patient) - VPD Only
- View only own medical record (VPD: filter by MABN)
- View own prescriptions
- View own diagnostic services
- Cannot view other patients' data
- Cannot access staff or notification data

## Next Steps

1. Create schema files in `schema/` directory
2. Create security configuration files in `security/` directory
3. Create audit configuration files in `audit/` directory
4. Migrate existing database content to this structure
5. Update MedicalDataSystem application connection strings

## Related Documentation

- Architecture: See [ARCHITECTURE.md](../../docs/ARCHITECTURE.md)
- Configuration: See [SETUP_GUIDE.md](../../docs/SETUP_GUIDE.md)
- Tasks: See [tasks/](../../docs/tasks/)
