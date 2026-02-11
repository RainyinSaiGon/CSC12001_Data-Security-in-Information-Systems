# Architecture & Design

Complete system architecture and technical design documentation for the Data Security in Information Systems project.

## System Overview Diagram

```
┌──────────────────────────────────────────────────────────────────────┐
│                     Medical Hospital System                          │
│                    (Data Security Project)                           │
└──────────────────────────────────────────────────────────────────────┘
                              │
           ┌──────────────────┼───────────────────┐
           │                  │                   │
    ┌──────▼────────┐  ┌──────▼─────────┐  ┌─────▼──────────────────┐
    │ Subsystem 1   │  │ Subsystem 2    │  │  Oracle 21c XE         │
    │ DB Admin      │  │ Medical System │  │  Separate Databases    │
    │ WinForm       │  │ WinForm        │  │  + Security Features   │
    └──────┬────────┘  └──────┬─────────┘  └─────┬──────────────────┘
           │                  │                   │
       .NET 10        .NET 10                 RBAC,VPD
       ODP.NET        ODP.NET                 OLS,Audit
           │                  │                   │
    ┌──────▼────────┐  ┌──────▼─────────┐       │
    │  Admin DB     │  │  Medical DB    │ ◄─────┘
    │               │  │                │
    │ Users/Roles   │  │ Patients       │
    │ Permissions   │  │ Staff          │
    │ Audit Logs    │  │ Medical Rec.   │
    │ (Independent) │  │ Prescriptions  │
    └───────────────┘  └────────────────┘
```

## Database Separation

**Key Change:** Subsystem 1 and Subsystem 2 use **separate Oracle databases** for clear architectural separation:

1. **Subsystem 1 Admin Database:** Stores administrative metadata (users, roles, permissions, admin audit logs)
2. **Subsystem 2 Medical Database:** Stores patient and medical data (with RBAC, VPD, OLS, data audit)

## Architecture Layers

### 1. Presentation Layer (UI Forms)

**Subsystem 1 - Oracle DB Admin:**
```
MainForm (Navigation Hub)
├── UserManagementForm
├── RoleManagementForm
├── PermissionForm
└── PrivilegeViewerForm
```

**Subsystem 2 - Medical System:**
```
LoginForm (Authentication)
├── CoordinatorForm (Patient Management)
├── DoctorForm (Clinical Records)
├── TechnicianForm (Service Management)
├── PatientForm (Self-Service Portal)
└── NotificationForm (OLS Label-based)
```

### 2. Business Logic Layer (Services)

**Subsystem 1 Services:**
- `OracleConnectionService`: Database connection pooling
- `UserService`: User CRUD operations
- `RoleService`: Role management
- `PermissionService`: Permission grant/revoke
- `PrivilegeService`: Privilege querying
- `ValidationService`: Input validation

**Subsystem 2 Services:**
- `AuthenticationService`: User login & role determination
- `OracleConnectionService`: Connection management
- `RBACService`: Role-based access control
- `VPDService`: Virtual private database filtering
- `OLSService`: Oracle label security management
- `PatientService`: Patient operations
- `DoctorService`: Doctor operations
- `CoordinatorService`: Coordinator operations
- `TechnicianService`: Technician operations
- `AuditService`: Audit logging
- `ValidationService`: Input validation

### 3. Data Access Layer (Models)

**Subsystem 1 Models:**
- `User`: Database user representation
- `Role`: Database role representation
- `Permission`: Permission with grant options
- `OracleObject`: Database object metadata

**Subsystem 2 Models:**
- `Patient`: BENHNHAN table mapping
- `Staff`: NHANVIEN table mapping
- `MedicalRecord`: HSBA table mapping
- `DiagnosticService`: HSBA_DV table mapping
- `Prescription`: DONTHUOC table mapping
- `Notification`: THONGBAO table mapping

### 4. Database Layer (Oracle 21c XE)

**Two separate Oracle 21c XE databases with independent schemas:**

#### Database 1: Subsystem 1 Admin Database
- **Connection:** `localhost:1521/XE` (Admin DB credentials)
- **Purpose:** Administrative metadata and operations
- **Tables:** ADMIN_USERS, ADMIN_ROLES, ADMIN_PERMISSIONS, ADMIN_ROLE_PERMISSIONS, ADMIN_USER_ROLES, ADMIN_OPERATION_AUDIT
- **Security:** RBAC for admin operations
- **Audit:** Complete audit trail of all admin actions

#### Database 2: Subsystem 2 Medical Database
- **Connection:** `localhost:1521/XE` (Medical DB credentials)
- **Purpose:** Patient and medical data
- **Tables:** BENHNHAN, NHANVIEN, HSBA, HSBA_DV, DONTHUOC, THONGBAO, AUDITLOG
- **Security:** RBAC + VPD (row-level) + OLS (label-based)
- **Audit:** Comprehensive data access and modification audit trail

## Entity Relationship Diagram (ERD)

### Subsystem 1 Admin Database (OracleDBAdmin)

```
ADMIN_USERS (Administrative Users)
├── ADMIN_USER_ID (PK)
├── TENTAIKHOAN (Username)
├── HOTEN (Full Name)
├── EMAIL
├── QUYHAN (Permission Level)
├── ACTIVE (Y/N)
└─── Has Multiple ──► ADMIN_USER_ROLES
                     ├── ADMIN_USER_ID (FK)
                     └── ADMIN_ROLE_ID (FK)
                     
                     ◄─── References ADMIN_ROLES
                          ├── ADMIN_ROLE_ID (PK)
                          ├── TENROLEVAITRO (Role Name)
                          └── MOTA (Description)
                          
                          Related To ──► ADMIN_ROLE_PERMISSIONS
                                        ├── ADMIN_ROLE_ID (FK)
                                        ├── PERMISSION_ID (FK)
                                        └── WITH_GRANT_OPTION

                                        ◄─── References ADMIN_PERMISSIONS
                                             ├── PERMISSION_ID (PK)
                                             ├── TENQUYEN (Permission Name)
                                             └── LOAIQUYEN (Type)

ADMIN_OPERATION_AUDIT (Audit Trail)
├── AUDIT_ID (PK)
├── ADMIN_USER_ID (FK)
├── OPERATION_TYPE
├── OBJECT_TYPE
├── OBJECT_NAME
├── TIMESTAMP
├── SUCCESS (Y/N)
└── DETAILS (JSON)
```

### Subsystem 2 Medical Database (MedicalDataSystem)

```
BENHNHAN (Patient)
├── MABN (PK)
├── TENBN
├── PHAI
├── NGAYSINH
├── CCCD (UNIQUE)
├── DiaChi
└── DiUng
    │
    └─── Has Multiple ──► HSBA (Medical Record)
                         ├── MAHSBA (PK)
                         ├── MABN (FK to Patient)
                         ├── MANV (FK to Doctor)
                         ├── CHANDOAN
                         ├── DIEUTRI
                         ├── KETLUAN
                         └─── Related To ──► HSBA_DV (Diagnostic Service)
                                            ├── MADV (PK)
                                            ├── MAHSBA (FK)
                                            ├── MANV_Technician (FK)
                                            ├── TenDichVu
                                            ├── Ngay
                                            └── KETQUA
                                            
                                 Related To ──► DONTHUOC (Prescription)
                                                ├── MADON (PK)
                                                ├── MAHSBA (FK)
                                                ├── TENHOA
                                                ├── LIEU
                                                └── HUONGDAN

NHANVIEN (Staff)
├── MANV (PK)
├── HOTEN
├── VAITRO ['Điều phối viên'|'Bác sĩ/Y sĩ'|'Kỹ thuật viên'|'Bệnh nhân']
├── CHUYENKHOA
└── Email
    │
    └─── Assigned As ──► Doctor in HSBA
    └─── Assigned As ──► Technician in HSBA_DV
    └─── Can Create ──► HSBA (Medical Records)
    └─── Can Create ──► DONTHUOC (Prescriptions)

THONGBAO (Notification) - OLS Label Security
├── MATHONG (PK)
├── Title
├── Content
├── Ngay  
├── Department (Label Component 1)
├── Location (Label Component 2)
└── Classification (Label Component 3)
```

## Security Architecture

### Subsystem 1: Admin Database Security

```
┌──────────────────────────────────────────────────────┐
│  OracleDBAdmin Application (WinForm)        [S1]    │
│  ├─ User Login (Admin Authentication)                │
│  ├─ Role-Based Feature Access                        │
│  └─ Operation Permission Checking                    │
├──────────────────────────────────────────────────────┤
│  Business Logic Layer (Admin Services)      [S1]    │
│  ├─ User Management Service                          │
│  ├─ Role Management Service                          │
│  ├─ Permission Grant/Revoke Service                  │
│  └─ Audit Service (Log all operations)               │
├──────────────────────────────────────────────────────┤
│  Admin Database Security Layer              [S1]    │
│  ├─ RBAC (Admin roles for DBA, SecAdmin, etc.)       │
│  └─ Audit Trails (Admin operation history)           │
├──────────────────────────────────────────────────────┤
│  Oracle Database 21c XE (Admin)                      │
│  ├─ ADMIN_USERS table                                │
│  ├─ ADMIN_ROLES, ADMIN_PERMISSIONS tables            │
│  └─ ADMIN_OPERATION_AUDIT table                      │
└──────────────────────────────────────────────────────┘
```

### Subsystem 2: Medical Database Security

```
┌──────────────────────────────────────────────────────┐
│  MedicalDataSystem Application (WinForm)    [S2]    │
│  ├─ Authentication Check (LoginForm)                 │
│  ├─ Role-Based Menu Display                          │
│  └─ Patient Data Access Control                      │
├──────────────────────────────────────────────────────┤
│  Business Logic Layer (Medical Services)    [S2]    │
│  ├─ RBAC Service (Role verification)                 │
│  └─ VPD Service (Row filtering)                      │
├──────────────────────────────────────────────────────┤
│  Database Security Layer (Oracle)            [S2]    │
│  ├─ VPD Policies (Transparent filtering)             │
│  ├─ OLS Labels (Multi-component hierarchy)           │
│  └─ Audit Trails (Immutable logging)                 │
├──────────────────────────────────────────────────────┤
│  Oracle Database 21c XE (Medical)                    │
│  ├─ RBAC Roles & Privileges                          │
│  ├─ Row-Level Security (VPD)                         │
│  ├─ Column-Level Security                            │
│  └─ Comprehensive Audit Trail (AUDITLOG)             │
└──────────────────────────────────────────────────────┘

Legend: [S1] = Subsystem 1  [S2] = Subsystem 2
```

### Access Control Layers (Subsystem 2)

```
┌──────────────────────────────────────────────────────┐
│  Application Layer (Forms)                   [S2]    │
│  ├─ Authentication Check (LoginForm)                 │
│  └─ Role-Based Menu Display                          │
├──────────────────────────────────────────────────────┤
│  Business Logic Layer (Services)             [S2]    │
│  ├─ RBAC Service (Role verification)                 │
│  └─ VPD Service (Row filtering)                      │
├──────────────────────────────────────────────────────┤
│  Database Security Layer (Oracle)            [S2]    │
│  ├─ VPD Policies (Transparent filtering)             │
│  ├─ OLS Labels (Multi-component hierarchy)           │
│  └─ Audit Trails (Immutable logging)                 │
├──────────────────────────────────────────────────────┤
│  Oracle Database 21c XE                              │
│  ├─ RBAC Roles & Privileges                          │
│  ├─ Row-Level Security (VPD)                         │
│  ├─ Column-Level Security                            │
│  └─ Comprehensive Audit Trail                        │
└──────────────────────────────────────────────────────┘
```

### Role-Based Isolation

```
Coordinator Access:
├─ SELECT, INSERT, UPDATE on BENHNHAN
├─ SELECT, INSERT, UPDATE on HSBA
├─ SELECT on NHANVIEN
└─ VIEW: All Patients, Assignment Records

Doctor Access (VPD Filtered):
├─ SELECT on BENHNHAN (only assigned patients)
├─ SELECT, INSERT, UPDATE on HSBA (own patients)
├─ INSERT, UPDATE on DONTHUOC
├─ INSERT on HSBA_DV
└─ VIEW: Assigned Patients Only (transparent filtering)

Technician Access (VPD Filtered):
├─ SELECT on HSBA_DV (assigned services only)
├─ UPDATE on HSBA_DV
├─ SELECT on BENHNHAN (for reference)
└─ VIEW: Assigned Services Only

Patient Access (Row-Level Security):
├─ SELECT on BENHNHAN (self only)
├─ UPDATE on BENHNHAN (contact info only)
├─ SELECT on HSBA (own records)
├─ SELECT on DONTHUOC (own prescriptions)
└─ VIEW: Own Medical Records Only (read-only)
```

## Data Flow Diagrams

### Login & Authentication Flow

```
User Input (Username/Password)
        │
        ▼
   LoginForm
        │
        ├─► AuthenticationService.Login()
        │        │
        │        ▼
        │   Query NHANVIEN table
        │        │
        │        ├─► Valid? ───► Get VAITRO (Role)
        │        │                    │
        │        │                    ▼
        │        │            Return Role String
        │        │                    │
        │        │   Invalid? ───────► Return null
        │
        ▼
  Role Determined
        │
        ├─ Coordinator ──► Open CoordinatorForm
        ├─ Doctor ───────► Open DoctorForm
        ├─ Technician ──► Open TechnicianForm
        └─ Patient ─────► Open PatientForm
```

### VPD Filtering Flow

```
Doctor Queries Patient Records
        │
        ▼
   DoctorService.GetAssignedPatients()
        │
        ├─► VPDService.GetVisiblePatients()
        │        │
        │        ▼
        │   Executes: SELECT * FROM BENHNHAN
        │            WHERE MANV = SYS_CONTEXT(...)
        │        │
        │        ▼
        │   VPD Policy Applied Automatically
        │   (Database level enforcement)
        │        │
        │        ▼
        │   Only Doctor's Patients Returned
        │        │
        ▼
  Results to Doctor Form
  (Cannot see other doctors' patients)
```

### OLS Label Security Flow

```
User Accesses Notification System
        │
        ▼
   NotificationForm
        │
        ├─► OLSService.GetUserLabels()
        │        │
        │        ▼
        │   Return: (Department, Location, Classification)
        │
        ├─► OLSService.GetAccessibleNotifications()
        │        │
        │        ▼
        │   For Each Notification:
        │   CanAccessNotification(user, notif_labels)?
        │        │
        │        ├─► Compare Labels in Hierarchy
        │        ├─ user_label >= notification_label?
        │        │
        │        ├─ YES ──► Include in Results
        │        └─ NO  ──► Exclude from Results
        │        │
        ▼
  Display Accessible Notifications Only
```

## Class Diagram (Core Services)

```
Subsystem 2 Service Architecture:

┌─────────────────────────┐
│  AuthenticationService  │
├─────────────────────────┤
│ - _connectionString     │
├─────────────────────────┤
│ + Login(user, pwd)      │
│ + ValidateUserRole()    │
│ + Logout(user)          │
└─────────────────────────┘
           △
           │ Uses
           │
     ┌─────┴──────┐
     │            │
┌────▼────────┐  ┌┴──────────────┐
│  RBACService│  │  VPDService   │
├─────────────┤  ├───────────────┤
│ Role defs   │  │ Row filtering │
├─────────────┤  ├───────────────┤
│ CheckRole() │  │ GetVisible()  │
│ GetActions()│  │ Rows filtered │
└─────────────┘  │ at DB level   │
                 └───────────────┘
                       △
                       │
     ┌─────────────────┼──────────── ┐
     │                 │             │
┌────▼─────┐   ┌───────▼──┐  ┌───────▼──┐
│  Patient  │  │  Doctor  │  │Technician│
│ Service   │  │ Service  │  │ Service  │
├───────────┤  ├──────────┤  ├──────────┤
│GetPatient │  │GetAssign │  │GetAssign │
│GetRecords │  │Diagnosis │  │Services  │
│UpdateInfo │  │PrescRxpt │  │UpdateRst │
└───────────┘  └──────────┘  └──────────┘
     △              △              △
     │              │              │
     └──────────────┼──────────────┘
                    │
            ┌───────▼────────┐
            │  OracleConnection
            │  Service        │
            ├─────────────────┤
            │- connString     │
            │- connPool       │
            ├─────────────────┤
            │+ GetConnection()│
            │+ TestConnection│
            └─────────────────┘
```

## Sequence Diagram - Patient Data Access

```
Patient    PatientForm    PatientService    OracleConn    Database
  │            │              │                │            │
  ├─ Login ────►│              │                │            │
  │            │─ Authenticate ────────────────●            │
  │            │◄─ Role: Patient ───────────────●            │
  │            │                 │                          │
  │            │                 │              │            │
  ├─ View ─────►│                │              │            │
  │ Records     │  GetMyRecords() │              │            │
  │            │◄───────────────●              │            │
  │            │                │  GetConnection()          │
  │            │                ├──────────────►│            │
  │            │                │  ◄──OraConn───│            │
  │            │                │                │            │
  │            │                │   SELECT from HSBA        │
  │            │                ├────────────────────────────►│
  │            │                │   WHERE MABN = patient_id  │
  │            │                │   AND row-level security   │
  │            │                │                            │
  │            │                │   VPD Policy Applied      │
  │            │◄───── Filtered Results ─────────────────────│
  │            │              │                            │
  │            │              (Only patient's records)      │
  │            │                                             │
  │◄─ Display ─│                                             │
  │ Records    │                                             │
  
Legend: ● = Database access
        VPD = Virtual Private Database filtering (transparent)
```

## Deployment Architecture

```
┌─────────────────────────────────────────────────┐
│  Windows Client Machines                        │
│  ├─ Subsystem 1: OracleDBAdmin.exe              │
│  ├─ Subsystem 2: MedicalDataSystem.exe          │
│  └─ Required: .NET 10.0 Runtime                 │
└──────────────┬──────────────────────────────────┘
               │ Network Connection
               │ (TCP/IP Port 1521)
               │
┌──────────────▼──────────────────────────────────┐
│  Database Server                                │
│  ├─ Oracle Database 21c XE                      │
│  ├─ Port: 1521                                  │
│  ├─ Service: XE                                 │
│  │                                               │
│  ├─ User: project_admin                         │
│  ├─ Roles: COORDINATOR, DOCTOR, TECHNICIAN,    │
│  │          PATIENT                             │
│  │                                               │
│  ├─ Security:                                   │
│  │  ├─ RBAC (Database roles)                    │
│  │  ├─ VPD (Row-level filtering)                │
│  │  ├─ OLS (Label security)                     │
│  │  └─ Audit (Standard, Fine-grained, Unified)  │
│  │                                               │
│  └─ Tables:                                     │
│     ├─ BENHNHAN (100K+ patients)                │
│     ├─ NHANVIEN (170+ staff)                    │
│     ├─ HSBA (Medical records)                   │
│     ├─ HSBA_DV (Diagnostic services)            │
│     ├─ DONTHUOC (Prescriptions)                │
│     ├─ THONGBAO (Notifications)                 │
│     └─ AuditLog (Audit trails)                  │
└─────────────────────────────────────────────────┘
```

## Technology Stack Summary

| Layer | Technology | Version |
|-------|-----------|---------|
| **Presentation** | .NET WinForms | 10.0 |
| **Business Logic** | C# Services | .NET 10.0 |
| **Data Access** | ODP.NET Core | 23.26.100 |
| **Database** | Oracle XE | 21c (21.3.0) |
| **OS** | Windows 10/11 | Latest |
| **IDE** | Visual Studio | 2022+ |

## Security Checkpoint Summary

✓ **Authentication**: LoginForm → AuthenticationService
✓ **Authorization**: RBAC + Application-level checks  
✓ **Row-Level Security**: VPD policies (transparent)
✓ **Column Security**: OLS labels for notifications
✓ **Audit & Logging**: Standard, Fine-grained, Unified audit
✓ **Data Integrity**: Constraints, Foreign Keys, Cascading Rules
✓ **Encryption**: Passwords (hashed), Sensitive data (encrypted at storage)
