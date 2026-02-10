# Architecture & Design

Complete system architecture and technical design documentation for the Data Security in Information Systems project.

## System Overview Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                     Medical Hospital System                     │
│                    (Data Security Project)                      │
└─────────────────────────────────────────────────────────────────┘
                              │
                ┌─────────────┼────────────┐
                │             │            │
         ┌──────▼──────┐  ┌──▼─────────┐  ┌┴──────────────┐
         │ Subsystem 1 │  │ Subsystem 2│  │  Oracle 21c   │
         │ DB Admin    │  │ Medical    │  │  XE Database  │
         │ WinForm     │  │ WinForm    │  │  + Security   │
         └──────┬──────┘  └──┬─────────┘  └┬──────────────┘
                │            │             │
                ├────────────┼─────────────┤
                │            │             │
          .NET 10.0    .NET 10.0      RBAC, VPD
          ODP.NET      ODP.NET        OLS, Audit
```

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
- `Patient`: BỆNHNHÂN table mapping
- `Staff`: NHÂNVIÊN table mapping
- `MedicalRecord`: HSBA table mapping
- `DiagnosticService`: HSBA_DV table mapping
- `Prescription`: ĐƠNTHUỐC table mapping
- `Notification`: THÔNGBÁO table mapping

### 4. Database Layer (Oracle 21c XE)

Connection: `localhost:1521/XE`

## Entity Relationship Diagram (ERD)

```
BỆNHNHÂN (Patient)
├── MÃBN (PK)
├── TÊNBN
├── PHÁI
├── NGÀYSINH
├── CCCD (UNIQUE)
├── DiaChi
└── DiUng
    │
    └─── Has Multiple ──► HSBA (Medical Record)
                         ├── MÃHSBA (PK)
                         ├── MÃBN (FK to Patient)
                         ├── MÃNV (FK to Doctor)
                         ├── CHẨNĐOÁN
                         ├── ĐIỀUTRỊ
                         ├── KẾTLUẬN
                         └─── Related To ──► HSBA_DV (Diagnostic Service)
                                            ├── MÃDV (PK)
                                            ├── MÃHSBA (FK)
                                            ├── MÃNV_Technician (FK)
                                            ├── TenDichVu
                                            ├── Ngay
                                            └── KẾTQUẢ
                                            
                                 Related To ──► ĐƠNTHUỐC (Prescription)
                                                ├── MÃĐƠN (PK)
                                                ├── MÃHSBA (FK)
                                                ├── TÊNHÓA
                                                ├── LIỀU
                                                └── HƯỚNGDẪN

NHÂNVIÊN (Staff)
├── MÃNV (PK)
├── HỌTÊN
├── VAITRÒ [Coordinator|Doctor|Technician]
├── CHUYÊNKHOA
└── Email
    │
    └─── Assigned As ──► Doctor in HSBA
    └─── Assigned As ──► Technician in HSBA_DV
    └─── Can Create ──► HSBA (Medical Records)
    └─── Can Create ──► ĐƠNTHUỐC (Prescriptions)

THÔNGBÁO (Notification) - OLS Label Security
├── MÃTHÔNG (PK)
├── Title
├── Content
├── Ngày  
├── Department (Label Component 1)
├── Location (Label Component 2)
└── Classification (Label Component 3)
```

## Security Architecture

### Access Control Layers

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

Legend: [S1] = Subsystem 1  [S2] = Subsystem 2
```

### Role-Based Isolation

```
Coordinator Access:
├─ SELECT, INSERT, UPDATE on BỆNHNHÂN
├─ SELECT, INSERT, UPDATE on HSBA
├─ SELECT on NHÂNVIÊN
└─ VIEW: All Patients, Assignment Records

Doctor Access (VPD Filtered):
├─ SELECT on BỆNHNHÂN (only assigned patients)
├─ SELECT, INSERT, UPDATE on HSBA (own patients)
├─ INSERT, UPDATE on ĐƠNTHUỐC
├─ INSERT on HSBA_DV
└─ VIEW: Assigned Patients Only (transparent filtering)

Technician Access (VPD Filtered):
├─ SELECT on HSBA_DV (assigned services only)
├─ UPDATE on HSBA_DV
├─ SELECT on BỆNHNHÂN (for reference)
└─ VIEW: Assigned Services Only

Patient Access (Row-Level Security):
├─ SELECT on BỆNHNHÂN (self only)
├─ UPDATE on BỆNHNHÂN (contact info only)
├─ SELECT on HSBA (own records)
├─ SELECT on ĐƠNTHUỐC (own prescriptions)
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
        │   Query NHÂNVIÊN table
        │        │
        │        ├─► Valid? ───► Get VAITRÒ (Role)
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
        │   Executes: SELECT * FROM BỆNHNHÂN
        │            WHERE MÃNV = SYS_CONTEXT(...)
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
  │            │                │   WHERE MÃBN = patient_id  │
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
│     ├─ BỆNHNHÂN (100K+ patients)                │
│     ├─ NHÂNVIÊN (170+ staff)                    │
│     ├─ HSBA (Medical records)                   │
│     ├─ HSBA_DV (Diagnostic services)            │
│     ├─ ĐƠNTHUỐC (Prescriptions)                │
│     ├─ THÔNGBÁO (Notifications)                 │
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
