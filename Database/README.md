# Database Administration & Scripts

Two separate Oracle database systems organized by subsystem functionality. Each subsystem has its own independent database schema.

## Overview

This directory contains SQL scripts for two separate database systems:

1. **Subsystem 1 (OracleDBAdmin):** Administrative database for managing users, roles, and permissions
2. **Subsystem 2 (MedicalDataManagement):** Medical data database with patient records, staff, prescriptions, etc.

## Directory Structure

```
Database/
├── Subsystem1-AdminDB/                            # Admin Tool Database
│   ├── schema/                                    (Create these files)
│   │   ├── 01_CreateTables.sql                   # Admin users, roles, permissions
│   │   ├── 02_CreateIndexes.sql                  # Performance indexes
│   │   └── 03_InsertSampleData.sql               # Sample admin users
│   ├── security/                                  (Create these files)
│   │   ├── 01_AdminUsers_Creation.sql            # Create admin database users
│   │   ├── 02_AdminRBAC_Setup.sql                # Admin role-based access control
│   │   └── 03_AdminAudit_Setup.sql               # Audit admin operations
│   ├── audit/                                     (Create these files)
│   │   └── 01_AdminOperationAudit.sql            # Audit trail for admin actions
│   └── README.md                                  # Subsystem 1 documentation
│
├── Subsystem2-MedicalDB/                         # Medical Data Database
│   ├── schema/                                    (Create these files)
│   │   ├── 01_CreateTables.sql                   # 7 medical data tables
│   │   ├── 02_CreateIndexes.sql                  # Performance indexes
│   │   └── 03_InsertSampleData.sql               # Sample data (100,000 patients, 170 staff)
│   ├── security/                                  (Create these files)
│   │   ├── 01_Users_Creation.sql                 # Create 4 role users
│   │   ├── 02_RBAC_Setup.sql                     # Role-Based Access Control
│   │   ├── 03_VPD_Setup.sql                      # Virtual Private Database (row-level)
│   │   └── 04_OLS_Setup.sql                      # Oracle Label Security
│   ├── audit/                                     (Create these files)
│   │   ├── 01_StandardAudit_Setup.sql
│   │   ├── 02_FineGrainedAudit_Setup.sql
│   │   ├── 03_UnifiedAudit_Setup.sql
│   │   └── ReadAuditLogs.sql
│   └── README.md                                  # Subsystem 2 documentation
│
├── Audit/                                         # Legacy audit scripts
│   └── ReadAuditLogs.sql
│
└── README.md                                      # This file
```

## Execution Order for Both Subsystems

**Warning:** Replace `<SYS_PASSWORD>` with your actual SYS account password. Never commit or share real credentials in documentation.

### Phase 1: Subsystem 1 Admin Database Setup

#### 1.1 Create Admin Schema (Run First)

```sql
-- For Oracle 21c XE:
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba
-- Or: sqlplus / as sysdba

@Subsystem1-AdminDB/schema/01_CreateTables.sql
@Subsystem1-AdminDB/schema/02_CreateIndexes.sql
@Subsystem1-AdminDB/schema/03_InsertSampleData.sql
```

#### 1.2 Configure Admin Security (Run Second - CRITICAL ORDER)

**IMPORTANT: Execute security scripts in this EXACT order!**

- Admin users MUST be created FIRST before roles can be assigned

```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

-- Step 1: CREATE ADMIN USERS FIRST
@Subsystem1-AdminDB/security/01_AdminUsers_Creation.sql

-- Step 2: Create and assign admin roles with permissions
@Subsystem1-AdminDB/security/02_AdminRBAC_Setup.sql

-- Step 3: Enable audit for admin operations
@Subsystem1-AdminDB/security/03_AdminAudit_Setup.sql
```

#### 1.3 Setup Admin Audit (Run Third)

```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

@Subsystem1-AdminDB/audit/01_AdminOperationAudit.sql
```

### Phase 2: Subsystem 2 Medical Database Setup

#### 2.1 Create Medical Schema (Run First)

```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

@Subsystem2-MedicalDB/schema/01_CreateTables.sql
@Subsystem2-MedicalDB/schema/02_CreateIndexes.sql
@Subsystem2-MedicalDB/schema/03_InsertSampleData.sql
```

#### 2.2 Security Configuration (Run Second - CRITICAL ORDER)

**IMPORTANT: Execute security scripts in this EXACT order!**

- Users MUST be created FIRST before roles can be assigned
- Then RBAC roles can be created and assigned to existing users

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

#### 2.3 Audit Configuration (Run Third)

Create and execute audit scripts:

```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

@Subsystem2-MedicalDB/audit/01_StandardAudit_Setup.sql
@Subsystem2-MedicalDB/audit/02_FineGrainedAudit_Setup.sql
@Subsystem2-MedicalDB/audit/03_UnifiedAudit_Setup.sql
```

## Additional Information

- **Subsystem 1 Admin Database:** See [Subsystem1-AdminDB/README.md](Subsystem1-AdminDB/README.md)
- **Subsystem 2 Medical Database:** See [Subsystem2-MedicalDB/README.md](Subsystem2-MedicalDB/README.md)
- **Test Cases:** See [../Tests/](../Tests/) directory

## Schema Details

### Core Tables

**KHOA** (Department)

```sql
CREATE TABLE KHOA (
    MAKHOA CHAR(6) PRIMARY KEY,
    TENKHOA NVARCHAR2(30) NOT NULL,
    SDT CHAR(10) NOT NULL,
    TRUONGKHOA VARCHAR2(10) REFERENCES NHANVIEN(MANV)
);
```

**BENHNHAN** (Patient - 100,000 records per TC#5)

```sql
CREATE TABLE BENHNHAN (
    MABN INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    TENBN NVARCHAR2(100) NOT NULL,
    PHAI NVARCHAR2(3) CHECK (PHAI IN ('Nam', 'Nữ')),
    NGAYSINH DATE,
    CCCD CHAR(12) UNIQUE NOT NULL,
    SONHA NVARCHAR2(5),
    TENDUONG NVARCHAR2(30),
    QUANHUYEN NVARCHAR2(30),
    TINHTP NVARCHAR2(50),
    TIENSUBENH NVARCHAR2(2000),
    TIENSUBENHGD NVARCHAR2(2000),
    DIUNGTHUOC NVARCHAR2(2000),
    USERNAME VARCHAR2(50) -- Map to Oracle User
);
```

**NHANVIEN** (Staff - 170 records)

```sql
CREATE TABLE NHANVIEN (
    MANV INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    HOTEN NVARCHAR2(100) NOT NULL,
    PHAI NVARCHAR2(3) CHECK (PHAI IN ('Nam', 'Nữ')),
    NGAYSINH DATE,
    CMND CHAR(12) UNIQUE NOT NULL,
    QUEQUAN NVARCHAR2(100),
    SODT VARCHAR2(15),
    VAITRO NVARCHAR2(50) CHECK (VAITRO IN ('Điều phối viên', 'Bác sĩ/Y sĩ', 'Kỹ thuật viên')),
    MAKHOA CHAR(6) REFERENCES KHOA(MAKHOA),
    USERNAME VARCHAR2(50) -- Map to Oracle User
);
```

**KHOA** (Department)

```sql
CREATE TABLE KHOA (
    MAKHOA CHAR(6) PRIMARY KEY,
    TENKHOA NVARCHAR2(30) NOT NULL,
    SDT CHAR(10) NOT NULL,
    TRUONGKHOA VARCHAR2(10) REFERENCES NHANVIEN(MANV)
);
```

**HSBA** (Medical Record - 50,000+ records)

```sql
CREATE TABLE HSBA (
    MAHSBA INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    MABN INT REFERENCES BENHNHAN(MABN),
    NGAY DATE NOT NULL,
    CHANDOAN NVARCHAR2(2000),
    DIEUTRI NVARCHAR2(2000),
    KETLUAN NVARCHAR2(2000),
    MABS INT REFERENCES NHANVIEN(MANV),
    MAKHOA CHAR(6) REFERENCES KHOA(MAKHOA)
);
```

**HSBA_DV** (Diagnostic Service - 75,000+ records)

```sql
CREATE TABLE HSBA_DV (
    MAHSBA_DV INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    LOAIDV NVARCHAR2(20),
    MAHSBA INT REFERENCES HSBA(MAHSBA),
    NGAYDV DATE NOT NULL,
    KETQUA NVARCHAR2(2000),
    MAKTV INT REFERENCES NHANVIEN(MANV)
);
```

**DONTHUOC** (Prescription - 100,000+ records)

```sql
CREATE TABLE DONTHUOC (
    MADONTHUOC INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    MAHSBA INT REFERENCES HSBA(MAHSBA),
    TENTHUOC NVARCHAR2(100) NOT NULL,
    LIEUDUNG NVARCHAR2(200),
    NGAYDT DATE NOT NULL
);
```

**THONGBAO** (Notification - 10,000+ records)

```sql
CREATE TABLE THONGBAO (
    MATHONGBAO INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    NOIDUNG NVARCHAR2(2000) NOT NULL,
    NGAYGIO TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    DIADIEM NVARCHAR2(100),
    KHOA CHAR(6) REFERENCES KHOA(MAKHOA),
    CAPBAC VARCHAR2(20)
);
```

## Security Mechanisms

### RBAC (Role-Based Access Control)

- **COORDINATOR**: Access to patient data, record assignment
- **DOCTOR**: Access to assigned patient records, diagnoses  
- **TECHNICIAN**: Access to diagnostic services
- **PATIENT**: Read-only access to own medical records

### VPD (Virtual Private Database)

- Doctors see only their patients
- Coordinators see records they assigned
- Technicians see assigned services
- Transparent row filtering

### OLS (Oracle Label Security)

- 3 hierarchy levels: Director, Department Head, Staff
- 3 departments: Cardiology, Gastroenterology, Neurology
- 3 locations: HCM, Hai Phong, Ha Noi
- Multi-component labels for notifications

## Audit Setup

### Standard Audit

Tracks user logins, object access, administrative actions

### Fine-Grained Audit

Detailed logging of specific operations:

- Diagnosis/treatment updates with timestamps
- Prescription modifications
- User ID for all actions

### Unified Audit

Consolidated audit trail combining multiple audit sources

## Backup & Recovery

### Backup Methods

1. **RMAN**: Recovery Manager for full/incremental backups
2. **Export/Datapump**: Logical backups for portability
3. **OS-level**: Operating system file backups

### Recovery Procedures

1. Point-in-time recovery
2. Full database recovery
3. Table-level recovery
4. Flashback database

## Prerequisites

- Oracle Database Express 21c (XE)
- SQL*Plus or Oracle SQL Developer
- DBA privileges (SYS or SYSTEM account)
- Sufficient tablespace (> 1GB recommended)

## Important Notes

1. **Execute as DBA**: Most scripts require SYSDBA privilege
2. **Test First**: Run on test database before production
3. **Backup Before**: Create full database backup before schema changes
4. **User Passwords**: Change default passwords after setup
5. **Table Space**: Ensure sufficient tablespace before creating tables
6. **Tablespace Name**: Scripts use USERS tablespace; adjust if needed

## Troubleshooting

### "ORA-01920: user name already exists"

Drop the user first:

```sql
DROP USER project_admin CASCADE;
```

### "ORA-00959: tablespace is not online"

Check available tablespaces:

```sql
SELECT tablespace_name FROM user_tablespaces;
```

### "ORA-04043: object does not exist"

Verify script executed successfully by checking:

```sql
SELECT * FROM user_tables;
SELECT * FROM dba_tables WHERE owner='PROJECT_ADMIN';
```

### "ORA-01031: insufficient privileges"

Ensure commands run as SYSDBA or user with proper grants

## Verification Commands

After setup, verify everything is configured:

```sql
-- Check tables created (should be 7)
SELECT table_name FROM user_tables 
WHERE table_name IN ('KHOA', 'BENHNHAN', 'NHANVIEN', 'HSBA', 'HSBA_DV', 'DONTHUOC', 'THONGBAO');

-- Check data volumes
SELECT 'BENHNHAN' AS tbl, COUNT(*) AS cnt FROM BENHNHAN
UNION ALL SELECT 'NHANVIEN', COUNT(*) FROM NHANVIEN
UNION ALL SELECT 'HSBA', COUNT(*) FROM HSBA
UNION ALL SELECT 'HSBA_DV', COUNT(*) FROM HSBA_DV
UNION ALL SELECT 'DONTHUOC', COUNT(*) FROM DONTHUOC
UNION ALL SELECT 'THONGBAO', COUNT(*) FROM THONGBAO;

-- Check users (should have test users)
SELECT username FROM dba_users WHERE username LIKE '1000%' OR username LIKE '2000%';

-- Check roles (should have 3 clinical roles)
SELECT role FROM dba_roles WHERE role IN ('Điều phối viên', 'Bác sĩ/Y sĩ', 'Kỹ thuật viên');

-- Check VPD policies (should have active policies)
SELECT object_owner, object_name, policy_name FROM dba_policies WHERE object_owner = 'HOSPITAL_ADMIN';

-- Check OLS configuration
SELECT * FROM lbacsys.lbacsys_labeling WHERE owner = 'HOSPITAL_ADMIN';

-- Check audit trail (recent activity)
SELECT * FROM aud$ ORDER BY ntimestamp# DESC FETCH FIRST 20 ROWS ONLY;
```

## References

- [Oracle Database Security Guide](https://docs.oracle.com/database/121/DBSEG/)
- [Oracle Virtual Private Database](https://docs.oracle.com/database/121/DBSEG/vpd.htm)
- [Oracle Label Security](https://docs.oracle.com/database/121/DBSEG/label_security.htm)
- [Oracle Auditing](https://docs.oracle.com/database/121/DBSEG/audit.htm)
- [Oracle RMAN Backup](https://docs.oracle.com/database/121/RCMRF/)
