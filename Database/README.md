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
│   │   └── 03_InsertSampleData.sql               # Sample data (100 patients, 170 staff)
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
    MAKHOA VARCHAR2(10) PRIMARY KEY,
    TENKHOA VARCHAR2(100), -- 'Khoa Tiêu Hóa', 'Khoa Thần Kinh', 'Khoa Tim Mạch'
    SDT VARCHAR2(20),
    TRUONGKHOA VARCHAR2(10) REFERENCES NHANVIEN(MANV)
);
```

**BENHNHAN** (Patient)

```sql
CREATE TABLE BENHNHAN (
    MABENHNHAN VARCHAR2(10) PRIMARY KEY,
    HOTEN VARCHAR2(100) NOT NULL,
    PHAI CHAR(1),
    NGAYSINH DATE,
    CCCD VARCHAR2(20) UNIQUE,
    DIENTHOAI VARCHAR2(15),
    SONHA VARCHAR2(50),
    TENDUONG VARCHAR2(50),
    QUANHUYEN VARCHAR2(50),
    TINHTP VARCHAR2(50),
    TIENSUBENH CLOB, -- Previously TIENSUABENH
    TIENSUBENHGD CLOB,
    DIUNGTHUOC CLOB,
    USERNAME VARCHAR2(50) -- Map to Oracle User
);
```

**NHANVIEN** (Staff)

```sql
CREATE TABLE NHANVIEN (
    MANV VARCHAR2(10) PRIMARY KEY,
    HOTEN VARCHAR2(100) NOT NULL,
    PHAI CHAR(1),
    NGAYSINH DATE,
    CMND VARCHAR2(20) UNIQUE,
    QUEQUAN VARCHAR2(100),
    SODT VARCHAR2(15),
    VAITRO VARCHAR2(50) CHECK (VAITRO IN ('Điều phối viên', 'Bác sĩ/Y sĩ', 'Kỹ thuật viên', 'Bệnh nhân')),
    MAKHOA VARCHAR2(10) REFERENCES KHOA(MAKHOA),
    USERNAME VARCHAR2(50) -- Map to Oracle User
);
```

**HSBA** (Medical Record)

```sql
CREATE TABLE HSBA (
    MAHSBA VARCHAR2(10) PRIMARY KEY,
    MABENHNHAN VARCHAR2(10) REFERENCES BENHNHAN(MABENHNHAN),
    NGAYTAO DATE,
    CHANDOAN CLOB,
    DIEUTRI CLOB,
    KETLUAN CLOB,
    MABACSI VARCHAR2(10) REFERENCES NHANVIEN(MANV), -- Doctor assigned
    MAKHOA VARCHAR2(10) REFERENCES KHOA(MAKHOA)
);
```

**HSBA_DV** (Diagnostic Service)

```sql
CREATE TABLE HSBA_DV (
    MADICHVU VARCHAR2(10) PRIMARY KEY, -- Changed to VARCHAR2 based on feedback
    MAHSBA VARCHAR2(10) REFERENCES HSBA(MAHSBA),
    TENDICHVU VARCHAR2(100),
    NGAY DATE,
    KETQUA CLOB,
    HOANTHANH NUMBER(1) DEFAULT 0,
    MAKYTHUATVIEN VARCHAR2(10) REFERENCES NHANVIEN(MANV) -- Technician performing
);
```

**DONTHUOC** (Prescription)

```sql
CREATE TABLE DONTHUOC (
    MADONTHUOC VARCHAR2(10) PRIMARY KEY,
    MAHSBA VARCHAR2(10) REFERENCES HSBA(MAHSBA),
    TENTHUOC VARCHAR2(100),
    LIEUDUNG VARCHAR2(200), -- Previously LIEUUNG
    HUONGDAN VARCHAR2(200),
    NGAYDANGKY DATE
);
```

**THONGBAO** (Notification - required for OLS)

```sql
CREATE TABLE THONGBAO (
    MATHONGBAO NUMBER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    NOIDUNG CLOB,
    NGAYGIO TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    DIADIEM VARCHAR2(100)
);
```

**AUDITLOG** (Custom Audit Trail)

```sql
CREATE TABLE AUDITLOG (
    AUDITID NUMBER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    USERID VARCHAR2(50),
    THOIGIAN TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    LOAIHD VARCHAR2(50),
    TENTABLE VARCHAR2(50),
    MARECORD VARCHAR2(50)
);
```

## Security Mechanisms

### RBAC (Role-Based Access Control)

- **COORDINATOR**: Access to patient data, record assignment
- **DOCTOR**: Access to assigned patient records, diagnoses
- **TECHNICIAN**: Access to diagnostic services
- **PATIENT**: Access to own medical records

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
-- Check tables
SELECT * FROM user_tables;

-- Check users
SELECT username FROM dba_users WHERE username LIKE 'DOCTOR%' OR username LIKE 'PATIENT%';

-- Check roles
SELECT role FROM dba_roles WHERE role IN ('Điều phối viên', 'Bác sĩ/Y sĩ', 'Kỹ thuật viên', 'Bệnh nhân');

-- Check VPD policies
SELECT object_owner, object_name, policy_name FROM dba_policies;

-- Check OLS configuration
SELECT * FROM lbacsys.lbacsys_labeling WHERE owner='PROJECT_ADMIN';

-- Check audit trail
SELECT * FROM aud$ ORDER BY ntimestamp# DESC;
```

## References

- [Oracle Database Security Guide](https://docs.oracle.com/database/121/DBSEG/)
- [Oracle Virtual Private Database](https://docs.oracle.com/database/121/DBSEG/vpd.htm)
- [Oracle Label Security](https://docs.oracle.com/database/121/DBSEG/label_security.htm)
- [Oracle Auditing](https://docs.oracle.com/database/121/DBSEG/audit.htm)
- [Oracle RMAN Backup](https://docs.oracle.com/database/121/RCMRF/)
