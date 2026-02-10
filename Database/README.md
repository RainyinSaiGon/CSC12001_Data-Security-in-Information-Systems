# Database Administration & Scripts

Complete Oracle database setup including schema definition, security configuration, audit setup, and backup/recovery procedures.

## Overview

This directory will contain all SQL scripts organized by functionality. Currently, only this README exists—SQL scripts must be created following the structure below:

- **Schema/**: Table creation and sample data scripts (to create)
- **Security/**: RBAC, VPD, OLS configuration scripts (to create)
- **Audit/**: Audit trail setup scripts (to create)
- **BackupRestore/**: Backup and recovery scripts (to create)

## Planned Directory Structure

The following structure shows where SQL scripts should be created:

```
Database/
├── Schema/                            (Create these files)
│   ├── 01_CreateTables.sql           # Create all tables
│   ├── 02_CreateIndexes.sql          # Create indexes for performance
│   └── 03_InsertSampleData.sql       # Sample data for testing
├── Security/                          (Create these files)
│   ├── 01_RBAC_Setup.sql             # Role-Based Access Control
│   ├── 02_VPD_Setup.sql              # Virtual Private Database
│   ├── 03_OLS_Setup.sql              # Oracle Label Security
│   └── 04_Users_Creation.sql         # Create database users
├── Audit/                             (Create these files)
│   ├── 01_StandardAudit_Setup.sql
│   ├── 02_FineGrainedAudit_Setup.sql
│   ├── 03_UnifiedAudit_Setup.sql
│   └── ReadAuditLogs.sql
├── BackupRestore/                     (Create these files)
│   ├── 01_BackupStrategy.sql
│   ├── 02_AutomaticBackup.sql
│   └── 03_RecoveryScripts.sql
└── README.md                          (This file)
```

## Execution Order (When Scripts are Created)

**Warning:** Replace `<SYS_PASSWORD>` with your actual SYS account password. Never commit or share real credentials in documentation.

### 1. Initial Setup (Run First)

Create and execute schema scripts:

```sql
-- For Oracle 21c XE:
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba
-- Or: sqlplus / as sysdba

@Schema/01_CreateTables.sql
@Schema/02_CreateIndexes.sql
@Schema/03_InsertSampleData.sql
```

### 2. Security Configuration (Run Second)

Create and execute security scripts:

```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

@Security/01_RBAC_Setup.sql
@Security/02_VPD_Setup.sql
@Security/03_OLS_Setup.sql
@Security/04_Users_Creation.sql
```

### 3. Audit Configuration (Run Third)

Create and execute audit scripts:

```sql
sqlplus sys/<SYS_PASSWORD>@localhost:1521/XE as sysdba

@Audit/01_StandardAudit_Setup.sql
@Audit/02_FineGrainedAudit_Setup.sql
@Audit/03_UnifiedAudit_Setup.sql
```

### 4. Backup/Recovery Setup (Optional)

Create and execute backup scripts:

```sql
@BackupRestore/01_BackupStrategy.sql
@BackupRestore/02_AutomaticBackup.sql
```

## Schema Details

### Core Tables

**BENHNHAN** (Patient)

```sql
CREATE TABLE BENHNHAN (
    MABENHNHAN NUMBER PRIMARY KEY,
    HOTEN VARCHAR2(100) NOT NULL,
    PHAI CHAR(1),
    NGAYSINH DATE,
    CCCD VARCHAR2(20) UNIQUE,
    DIENTHOAI VARCHAR2(15),
    DIACHI VARCHAR2(200),
    DIUNG CLOB
);
```

**NHANVIEN** (Staff)

```sql
CREATE TABLE NHANVIEN (
    MANV NUMBER PRIMARY KEY,
    HOTEN VARCHAR2(100) NOT NULL,
    VAITRO VARCHAR2(50), -- COORDINATOR, DOCTOR, TECHNICIAN
    CHUYENKHOA VARCHAR2(50)
);
```

**HSBA** (Medical Record)

```sql
CREATE TABLE HSBA (
    MAHSBA NUMBER PRIMARY KEY,
    MABENHNHAN NUMBER REFERENCES BENHNHAN(MABENHNHAN),
    NGAYTAO DATE,
    CHANDOAN CLOB,
    DIEUTRI CLOB,
    KETLUAN CLOB,
    MABACSI NUMBER REFERENCES NHANVIEN(MANV) -- Doctor assigned
);
```

**HSBA_DV** (Diagnostic Service)

```sql
CREATE TABLE HSBA_DV (
    MADICHVU NUMBER PRIMARY KEY,
    MAHSBA NUMBER REFERENCES HSBA(MAHSBA),
    TENDICHVU VARCHAR2(100),
    NGAY DATE,
    KETQUA CLOB,
    HOANTHANH NUMBER(1) DEFAULT 0,
    MAKYTHUATVIEN NUMBER REFERENCES NHANVIEN(MANV) -- Technician performing
);
```

**DONTHUOC** (Prescription)

```sql
CREATE TABLE DONTHUOC (
    MADONTHUOC NUMBER PRIMARY KEY,
    MAHSBA NUMBER REFERENCES HSBA(MAHSBA),
    TENTHUOC VARCHAR2(100),
    LIEUDUNG VARCHAR2(200),
    HUONGDAN VARCHAR2(200),
    NGAYDANGKY DATE
);
```

**THONGBAO** (Notification - required for OLS)

```sql
CREATE TABLE THONGBAO (
    MATHONG NUMBER PRIMARY KEY,
    NOIDUNG CLOB,
    NGAYGIO TIMESTAMP,
    DIADIEM VARCHAR2(100)
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
SELECT role FROM dba_roles WHERE role LIKE '%ROLE';

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
