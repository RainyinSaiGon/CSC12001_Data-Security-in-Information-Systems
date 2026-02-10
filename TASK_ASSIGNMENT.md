# Task Assignment - Data Security Project

Team Members: 5 people
Project: CSC12001 Data Security in Information Systems
Start Date: February 10, 2026

---

## Team Structure and Responsibilities

### Person 1: Subsystem 1 - Oracle Database Administrator UI

Focus: Forms and User Interface Design
Estimated Hours: 20-25 hours
Priority Level: High

#### Forms to Implement

1. **Forms/MainForm.cs** [PRIMARY FORM]

   Description: Serve as the main application window and central navigation hub for the database administration application.

   Requirements:
   - Implement main application window with menu bar
   - Add buttons for accessing: User Management, Role Management, Permission Management, Privilege Viewer
   - Include status bar displaying currently connected user information
   - Implement proper window closing and application exit procedures
   - Add error handling for form initialization and navigation

   UI Components:
   - MenuStrip: File, Tools, Help menus
   - Buttons: User Management, Role Management, Permission Management, View Privileges
   - StatusStrip: Connection status, current user display, timestamp
   - Labels and panels for layout organization

2. **Forms/UserManagementForm.cs**

   Description: Enable administrators to perform CRUD operations on Oracle database users with full validation and error handling.

   Requirements:
   - Provide input fields for username and password
   - Implement Create User, Update User, and Delete User buttons
   - Display comprehensive DataGrid showing all users with columns: Username, CreatedDate, Status
   - Add search and filter functionality by username
   - Implement form-level validation before database operations

   UI Components:
   - TextBox: Username input
   - TextBox: Password input (masked)
   - Buttons: Create, Update, Delete, Clear, Refresh
   - DataGrid: User list display
   - SearchBox: Username filter
   - MessageBox: Operation status feedback

3. **Forms/RoleManagementForm.cs**

   Description: Enable management of Oracle database roles with comprehensive role information display.

   Requirements:
   - Provide input fields for role name and description
   - Implement Create Role and Delete Role buttons
   - Display DataGrid showing all roles with columns: RoleName, Description, CreatedDate
   - Add role filtering and search capabilities
   - Implement validation to prevent invalid role operations

   UI Components:
   - TextBox: Role name input
   - TextBox: Description input
   - Buttons: Create, Delete, Clear, Refresh
   - DataGrid: Role list with columns for name, description, creation date
   - SearchBox: Role name filter

4. **Forms/PermissionForm.cs**

   Description: Provide granular control over Oracle permissions including column-level security and grant options.

   Requirements:
   - Provide ComboBox for selecting user or role (dropdown selection)
   - Provide ComboBox for selecting Oracle objects (TABLE, VIEW, PROCEDURE, etc.)
   - Include CheckBoxes for individual permissions: SELECT, INSERT, UPDATE, DELETE
   - Include CheckBox for WITH GRANT OPTION
   - Implement Grant and Revoke buttons for permission management
   - Display DataGrid showing existing permissions with detailed information
   - Support column-level permission grants for SELECT and UPDATE operations

   UI Components:
   - ComboBox: User/Role selection
   - ComboBox: Object selection
   - CheckBox: SELECT, INSERT, UPDATE, DELETE permissions
   - CheckBox: WITH GRANT OPTION
   - Buttons: Grant, Revoke, Clear, Refresh
   - DataGrid: Existing permissions display with columns for grantee, object, permission type, columns, grant option
   - TextBox: Column list input for column-level security

5. **Forms/PrivilegeViewerForm.cs**

   Description: Display comprehensive privilege information for users and roles with filtering and search capabilities.

   Requirements:
   - Provide ComboBox to select user or role for privilege viewing
   - Display comprehensive DataGrid showing all privileges for selected user/role
   - Include columns: Object Name, Object Type, Permission Type, With Grant Option, Granted Date
   - Implement filtering options for privilege type (System vs Object privileges)
   - Add export functionality for privilege reports

   UI Components:
   - ComboBox: User or Role selection
   - DataGrid: Privilege list with comprehensive details
   - Buttons: Refresh, Export, Filter
   - RadioButtons: System Privileges vs Object Privileges filter
   - StatusBar: Display count of privileges

#### Required Services to Use

The following services must be properly integrated with these forms:

- UserService.CreateUser(), ListUsers(), DeleteUser(), ModifyUser()
- RoleService.CreateRole(), ListRoles(), DeleteRole()
- PermissionService.GrantPermission(), RevokePermission(), GrantColumnPermission()
- PrivilegeService.GetUserPrivileges(), GetRolePrivileges(), GetObjectPermissions()

#### Success Criteria and Acceptance Requirements

Implementation will be considered complete when the following criteria are met:

- All forms are fully functional and responsive
- Forms properly display data retrieved from underlying services
- Form inputs are validated before submission to prevent invalid data
- Clear and informative error messages are displayed for failed operations
- User interface is intuitive with clear labeling and logical workflow
- All CRUD operations complete successfully
- No unhandled exceptions or application crashes occur
- Forms properly handle empty result sets
- Performance is acceptable for typical database sizes

---

### Person 2: Subsystem 1 - Business Logic Services

Focus: Service Layer Implementation and Database Integration
Estimated Hours: 25-30 hours
Priority Level: Critical

#### Services to Implement

1. **Services/OracleConnectionService.cs** [CRITICAL - IMPLEMENT FIRST]

   Description: Serve as the foundational service managing all Oracle database connections with connection pooling and error handling. All other services depend on this implementation.

   Core Responsibilities:
   - Accept and store connection string from configuration
   - Manage OracleConnection lifecycle including creation, testing, and cleanup
   - Implement connection pooling for performance optimization
   - Provide methods for other services to retrieve database connections
   - Handle connection errors gracefully with detailed error logging

   Methods to Implement:
   - Constructor(string connectionString): Initialize service with connection details
   - TestConnection(): bool - Verify Oracle database connection using ODP.NET
   - GetConnection(): OracleConnection - Return new or pooled connection object
   - CloseConnection(OracleConnection): void - Properly close and dispose connections

   Error Handling Requirements:
   - Catch OracleException for connection failures
   - Log detailed error information for troubleshooting
   - Throw meaningful exceptions with descriptive messages
   - Handle timeout scenarios appropriately

   Technical Specifications:
   - Use Oracle.ManagedDataAccess.Core NuGet package
   - Implement connection pooling with configurable pool size
   - Support connection string from appsettings.json or environment variables
   - Never hardcode database credentials

2. **Services/ValidationService.cs**

   Description: Provide centralized input validation and data verification for all database operations.

   Methods to Implement:
   - ValidateUsername(string username): bool - Validate Oracle username format
     - Check length between 3-30 characters
     - Ensure alphanumeric characters with underscores allowed
     - Prevent reserved Oracle keywords
   - ValidatePassword(string password): bool - Validate password strength requirements
     - Enforce minimum 8 characters
     - Require mixed case (uppercase and lowercase)
     - Require at least one numeric character
     - Return false for weak passwords
   - CheckObjectExists(string objectName): bool - Verify Oracle object existence
     - Query DBA_OBJECTS or appropriate data dictionary
     - Return true if object exists, false otherwise
     - Handle exceptions gracefully

   Validation Rules:
   - All validators should return boolean values
   - Provide detailed error messages for validation failures
   - Handle null and empty string inputs safely
   - Log validation failures for security auditing

3. **Services/UserService.cs**

   Description: Implement complete user management operations (Create, Read, Update, Delete) with proper validation and error handling.

   Methods to Implement:
   - CreateUser(User user): bool - Create new Oracle database user
     - Validate input using ValidationService
     - Execute CREATE USER statement: CREATE USER username IDENTIFIED BY password
     - Handle duplicate username errors
     - Return success/failure status
   - ModifyUser(User user): bool - Modify existing user properties
     - Execute ALTER USER statement to change password or other properties
     - Handle user-not-found errors
     - Maintain user role assignments
   - DeleteUser(string username): bool - Delete Oracle user
     - Validate username exists
     - Execute DROP USER statement
     - Handle cascade delete requirements
   - ListUsers(): List<User> - Retrieve all Oracle users
     - Query DBA_USERS to get all user information
     - Populate User model objects with complete details
     - Handle empty result sets
   - GrantRole(string username, string roleName): bool - Assign role to user
     - Execute GRANT role TO user statement
     - Verify both user and role exist
     - Handle duplicate role assignment

   Data Access:
   - Use OracleCommand with parameterized queries to prevent SQL injection
   - Use OracleDataReader for result processing
   - Implement proper connection management and cleanup

4. **Services/RoleService.cs**

   Description: Implement role management operations for Oracle database roles.

   Methods to Implement:
   - CreateRole(Role role): bool - Create new role
     - Execute CREATE ROLE statement
     - Store role metadata (name, description)
     - Validate role name before creation
   - DeleteRole(string roleName): bool - Delete existing role
     - Execute DROP ROLE statement
     - Handle role dependencies
     - Cascade revoke permissions if necessary
   - ListRoles(): List<Role> - Retrieve all roles from database
     - Query DBA_ROLES for role listing
     - Query DBA_ROLE_PRIVS for role privilege information
     - Return populated Role objects
   - GetRolePrivileges(string roleName): List<Permission> - Get all privileges granted to role
     - Query ROLE_TAB_PRIVS for table privileges
     - Query ROLE_SYS_PRIVS for system privileges
     - Return comprehensive Permission list

5. **Services/PermissionService.cs**
   
   Description: Manage Oracle permissions and privileges with support for column-level security and grant options.
   
   Methods to Implement:
   - GrantPermission(Permission permission): bool - Grant permission to user or role
     - Execute GRANT statement: GRANT permission ON object TO grantee [WITH GRANT OPTION]
     - Support all permission types: SELECT, INSERT, UPDATE, DELETE
     - Support WITH GRANT OPTION for permission delegation
     - Validate object existence before granting
     - Handle duplicate grant attempts
   - RevokePermission(Permission permission): bool - Revoke permission from user or role
     - Execute REVOKE statement
     - Handle permission-not-found scenarios
     - Log revocation activities for audit trail
   - GrantColumnPermission(string grantedTo, string tableName, List<string> columns, string permissionType): bool - Grant column-level permissions
     - Execute column-specific GRANT: GRANT SELECT(col1, col2) ON table TO user
     - Support SELECT and UPDATE for column-level grants
     - Validate column names against table schema
   - GetObjectPermissions(string objectName): List<Permission> - Retrieve all permissions on object
     - Query TABLE_PRIVS or appropriate data dictionary
     - Return all grants and grantees
   
   Validation Requirements:
   - Validate object existence before operations
   - Verify user/role exists before granting
   - Handle invalid permission types
   - Log all permission changes for audit compliance

6. **Services/PrivilegeService.cs**
   
   Description: Query and display privilege information for comprehensive access control review.
   
   Methods to Implement:
   - GetUserPrivileges(string username): List<Permission> - Get all privileges for user
     - Query USER_TAB_PRIVS for table privileges
     - Query USER_SYS_PRIVS for system privileges
     - Include both direct and role-based privileges
     - Return comprehensive list with all details
   - GetRolePrivileges(string roleName): List<Permission> - Get all privileges for role
     - Query ROLE_TAB_PRIVS for table privileges granted to role
     - Query ROLE_SYS_PRIVS for system privileges granted to role
     - Return all privileges with grantor information
   - GetObjectPermissions(string objectName): List<Permission> - Get all permissions on object
     - Query ALL_TAB_PRIVS where table_name equals objectName
     - Return all grants, grantees, and grant options
     - Include both user and role grants
   - HasPrivilege(string username, string objectName, string privilegeType): bool - Check single privilege
     - Query privilege data dictionary
     - Return true only if exact privilege exists
     - Used for access control validation

#### Required Models

- User.cs
- Role.cs  
- Permission.cs
- OracleObject.cs

#### Technology Stack and Dependencies

- Oracle.ManagedDataAccess.Core NuGet package (version 23.26.100 or higher)
- OracleConnection for connection management
- OracleCommand for SQL execution
- OracleDataReader for result processing
- System.Configuration for reading connection strings
- Microsoft.Extensions.Configuration for appsettings.json support

#### Success Criteria and Acceptance Requirements

Implementation will be considered complete when:

- All service methods are implemented and functional
- Oracle database queries execute successfully without errors
- Proper exception handling is implemented for connection failures
- Proper exception handling is implemented for query execution failures
- All services integrate successfully with Person 1's forms
- No credentials are hardcoded anywhere in code
- Connection strings are loaded from secure configuration sources
- All SQL statements use parameterized queries to prevent injection
- Services implement proper connection cleanup and resource disposal
- Services generate meaningful error messages for troubleshooting
- Performance is acceptable for normal operational loads

---

### Person 3: Subsystem 2 - Medical UI Forms (Role-Specific)

Focus: Role-Based User Interface Implementation and Data Display
Estimated Hours: 25-30 hours
Priority Level: High

#### Forms to Implement

1. **Forms/LoginForm.cs** [PRIMARY FORM - IMPLEMENT FIRST]
   
   Description: Serve as the application entry point providing secure user authentication and role determination for access control.
   
   Core Functionality:
   - Present professional login interface with username and password fields
   - Validate credentials through AuthenticationService
   - Determine user role after successful authentication
   - Open appropriate role-specific form based on login credentials
   - Properly handle failed login attempts
   - Close login form after successful authentication transition
   
   UI Components:
   - TextBox: Username input field
   - TextBox: Password input field (masked for security)
   - Button: Login
   - Button: Cancel/Exit
   - Label: Application title and version
   - Label: Error message display for failed logins
   - ProgressBar: Authentication processing indicator (optional)
   
   Implementation Details:
   - Call AuthenticationService.Login(username, password)
   - Receive user role from authentication service
   - Handle three authentication outcomes:
     a) Success: Determine role and open corresponding form
     b) Invalid credentials: Display error message and allow retry
     c) Database error: Display connection error and retry option
   - Clear sensitive data (password) from memory after validation
   - Prevent multiple simultaneous login attempts

2. **Forms/CoordinatorForm.cs** [RBAC-Based]
   
   Description: Provide comprehensive coordination interface for managing patients, medical records, and staff assignments with role-based access control.
   
   Core Responsibilities:
   - Display all patients in the medical system (RBAC-based access)
   - Enable CRUD operations on patient records
   - Facilitate doctor and technician assignment to patients and services
   - Track medical record status and workflow
   - Provide unified management interface for coordinator operations
   
   UI Components:
   
   Patient Management Section:
   - DataGrid: Display all patients (columns: PatientID, Name, Gender, DateOfBirth, Status)
   - Button: Add Patient
   - Button: Edit Patient
   - Button: Delete Patient
   - Button: View Patient Details
   - TextBox: Patient search filter
   - ComboBox: Filter by gender or status (optional)
   
   Record Management Section:
   - DataGrid: Display medical records (columns: RecordID, PatientID, Diagnosis, CreatedDate, Status)
   - Button: Assign Doctor
   - Button: Assign Technician
   - Button: View Record Details
   - ComboBox: Select doctor for assignment
   - ComboBox: Select technician for assignment
   
   Workflow Support:
   - Status bar showing current coordinator information
   - Refresh button for data refresh
   - Logout button for session termination
   
   Functionality Requirements:
   - Retrieve and display all patients using CoordinatorService.GetAllPatients()
   - Enable adding new patients with validation
   - Enable editing existing patient information
   - Display medical records associated with patients
   - Facilitate role assignments through service calls
   - Provide clear feedback for all operations

3. **Forms/DoctorForm.cs** [VPD-Based Filtering]
   
   Description: Provide medical professional interface for managing patient care with virtual private database filtering applied automatically to patient data.
   
   Core Responsibilities:
   - Display only assigned patients (VPD automatically filters to doctor's assigned patients)
   - Enable viewing comprehensive patient history and medical information
   - Support diagnosis creation and management
   - Enable prescription management and creation
   - Facilitate ordering of diagnostic services
   - Display patient's medical records and allergies
   
   UI Components:
   
   Patient List Section:
   - DataGrid: List assigned patients (columns: PatientID, Name, Age, Status) - VPD filtered
   - Label: Count of assigned patients
   - Button: Refresh data
   
   Patient Details Section:
   - TextBox (read-only): Patient name, ID number
   - TextBox (read-only): Medical history summary
   - TextBox (read-only): Drug allergies
   - TextBox (read-only): Contact information
   - Button: View full patient history
   
   Clinical Operations Section:
   - Button: Create Diagnosis
   - Button: Update Diagnosis
   - Button: Create/Update Prescription
   - Button: Order Diagnostic Service
   - DataGrid: Display patient's medical records
   - DataGrid: Display patient's prescriptions
   
   VPD Security:
   - All patient data automatically filtered by database VPD policies
   - Doctor can only see patients they are assigned to
   - VPD filtering occurs at database level (transparent to form)
   
   Functionality Requirements:
   - Retrieve assigned patients using DoctorService.GetAssignedPatients()
   - Display comprehensive patient information
   - Create diagnosis records with audit logging
   - Manage prescriptions with dosage and instructions
   - Order diagnostic services for patients
   - Support viewing full patient medical history

4. **Forms/TechnicianForm.cs** [VPD-Based Filtering]
   
   Description: Provide interface for diagnostic service management with role-specific access to assigned services only.
   
   Core Responsibilities:
   - Display diagnostic services assigned to technician (VPD filtered)
   - Enable updating service results with detailed findings
   - Support marking services as complete
   - Display required patient and service information
   - Maintain comprehensive service workflow
   
   UI Components:
   
   Service List Section:
   - DataGrid: List assigned services (columns: ServiceID, PatientID, ServiceType, Status) - VPD filtered
   - Label: Count of assigned services
   - ComboBox: Filter by service status (Pending, In Progress, Complete)
   
   Service Details Section:
   - Label: Selected service details
   - Label: Patient information
   - Label: Test type/service description
   - TextBox: Service date and time
   
   Results Management Section:
   - TextBox (multiline): Service results/findings input
   - DateTimePicker: Result date/time
   - Button: Update Results
   - Button: Mark Complete
   - TextBox (read-only): Previous attempts/history
   
   VPD Security:
   - Only services assigned to technician are displayed
   - VPD policies enforce at database level
   
   Functionality Requirements:
   - Retrieve assigned services using TechnicianService.GetAssignedServices()
   - Display comprehensive service information
   - Update service results with validation
   - Mark services as complete
   - Track service workflow status
   - Provide clear status feedback

5. **Forms/PatientForm.cs** [Row-Level Security]
   
   Description: Provide patient self-service portal for accessing personal medical information with row-level security ensuring patients see only their own records.
   
   Core Responsibilities:
   - Display authenticated patient's own information only
   - Enable viewing personal medical records
   - Enable viewing prescriptions
   - Enable viewing appointment history
   - Support updating contact information
   - Enforce read-only access for sensitive medical data
   
   UI Components:
   
   Patient Information Section:
   - Label: Patient name (display only)
   - Label: Patient ID (display only)
   - TextBox: Phone number (editable)
   - TextBox: Email address (editable)
   - TextBox: Address (editable)
   - Button: Update Contact Info
   
   Medical Records Section:
   - TabControl: Organize different record types
   
   Tab 1: Medical Records
     - DataGrid: Medical records (columns: RecordID, Date, Diagnosis, Treatment) - read-only
     - Button: View Full Record Details
   
   Tab 2: Prescriptions
     - DataGrid: Prescriptions (columns: PrescriptionID, Drug, Dosage, Instructions, Date) - read-only
     - Button: Print Prescription
   
   Tab 3: Appointment History
     - DataGrid: Appointments (columns: AppointmentID, Doctor, Date, Status) - read-only
   
   Row-Level Security:
   - Patient can only see their own records
   - All data is read-only (no editing of medical records)
   - System verification ensures patient viewing own data only
   
   Functionality Requirements:
   - Display authenticated patient's information
   - Retrieve personal medical records using PatientService.GetMyMedicalRecords()
   - Retrieve personal prescriptions using PatientService.GetMyPrescriptions()
   - Support editing own contact information
   - Display comprehensive appointment history
   - Enforce read-only access for all medical data

6. **Forms/NotificationForm.cs** [OLS Label-Based Access]
   
   Description: Display notifications with Oracle Label Security filtering ensuring only accessible notifications are displayed based on user's label hierarchy.
   
   Core Responsibilities:
   - Display notifications filtered by user's OLS labels
   - Implement label-based access control
   - Provide clear notification information
   - Support notification navigation and viewing
   - Enforce label hierarchy in filtering
   
   UI Components:
   
   Notification List Section:
   - DataGrid: Notifications (columns: NotificationID, Title, Department, Location, Date) - OLS filtered
   - Label: Current user's labels (Department, Location, Classification)
   - Label: Count of accessible notifications
   
   Notification Details Section:
   - TextBox (multiline): Notification title (display)
   - TextBox (multiline): Notification content (display)
   - Label: Department
   - Label: Location
   - Label: Classification
   - Label: Created date
   
   Navigation:
   - Buttons: Previous, Next for notification navigation
   - Button: Refresh notifications
   - Button: Close/Return
   
   OLS Security:
   - Only notifications matching user's labels are displayed
   - Label hierarchy enforced (user label >= notification label required)
   - System queries return pre-filtered notifications
   
   Functionality Requirements:
   - Retrieve user's OLS labels using OLSService.GetUserLabels()
   - Retrieve accessible notifications using OLSService.GetAccessibleNotifications()
   - Display only notifications matching label requirements
   - Show comprehensive notification details
   - Provide read-only notification viewing
   - Display user's current label levels

#### Required Services to Use

These services must be properly integrated for forms to function correctly:
- AuthenticationService.Login()
- RBACService.CheckUserRole()
- VPDService.GetVisiblePatients(), GetVisibleRecords(), GetVisibleServices()
- OLSService.GetUserLabels(), GetAccessibleNotifications()
- PatientService.GetPatient(), GetMyMedicalRecords(), UpdatePatientInfo()
- DoctorService.GetAssignedPatients(), CreateDiagnosis(), UpdatePrescription(), OrderDiagnosticService()
- CoordinatorService.GetAllPatients(), AddPatient(), EditPatient(), AssignDoctorToPatient(), AssignTechnicianToService()
- TechnicianService.GetAssignedServices(), UpdateServiceResult(), CompleteService()

#### Required Models

Forms will use the following model classes for data structures:
- Patient.cs
- Staff.cs
- MedicalRecord.cs
- DiagnosticService.cs
- Prescription.cs
- Notification.cs

#### Success Criteria and Acceptance Requirements

Implementation will be considered complete when:
- LoginForm successfully authenticates users and opens correct role forms
- Each role-specific form displays appropriate data for that role
- Data displays correctly and reflects current database state
- VPD filtering works correctly (doctors see only assigned patients)
- OLS label filtering works correctly (notifications restricted by labels)
- All editable fields validate input before submission
- Forms provide clear user feedback for all operations
- Patient forms are read-only for medical data (cannot edit diagnoses or prescriptions)
- Logout functionality properly terminates session and returns to login
- No unhandled exceptions occur during normal operations
- Forms handle empty result sets gracefully
- User interface is intuitive and professional in appearance

---

### Person 4: Subsystem 2 - Security Services (RBAC, VPD, OLS)
Focus: Security Mechanisms and Access Control Implementation
Estimated Hours: 30-35 hours
Priority Level: Critical

#### Services to Implement

1. **Services/AuthenticationService.cs** [CRITICAL - IMPLEMENT FIRST]
   
   Description: Provide centralized user authentication and role determination. This service is the foundation for all access control mechanisms and must be implemented first.
   
   Core Responsibilities:
   - Authenticate users against Oracle database with secure credential verification
   - Determine and return user's role for access control decisions
   - Manage user sessions and authentication state
   - Handle authentication failures gracefully
   - Log authentication attempts for security auditing
   
   Methods to Implement:
   - Login(string username, string password): string - Authenticate user and return role
     - Connect to Oracle database using provided credentials
     - Query NHÂNVIÊN table to locate user
     - Verify password matches stored value (or use Oracle authentication)
     - Determine user's VAITRÒ (role): Coordinator, Doctor, Technician, or Patient
     - Return role as string (null if authentication fails)
     - Log authentication attempt regardless of success/failure
   
   - ValidateUserRole(string username, string expectedRole): bool - Verify user has specific role
     - Query database for user's current role
     - Compare against expected role
     - Return true only if roles match
     - Used for authorization verification
   
   - Logout(string username): void - Terminate user session
     - Clear session data from memory
     - Log logout event for audit trail
     - Clean up any open resources
     - Remove session tokens/identifiers
   
   Authentication Requirements:
   - Support all four role types: Coordinator, Doctor/Nurse, Technician, Patient
   - Handle invalid username (user does not exist)
   - Handle invalid password (incorrect password)
   - Handle database connection failures
   - Implement account lockout after multiple failed attempts (optional security enhancement)
   - Never transmit passwords in plain text logging
   - Use secure password comparison (avoid timing attacks)
   
   Error Handling:
   - Return null for failed authentication attempts
   - Throw exception only for database connection errors
   - Provide meaningful error messages for logging
   - Log all authentication events for compliance

2. **Services/OracleConnectionService.cs**
   
   Description: Connection service for Subsystem 2 (can be implemented identically to Person 2's version for Subsystem 1 or create duplicate implementation).
   
   Core Functionality:
   - Manage Oracle database connections
   - Implement connection pooling
   - Provide connection testing capability
   - Handle connection errors gracefully
   
   Methods to Implement:
   - Constructor(string connectionString)
   - TestConnection(): bool
   - GetConnection(): OracleConnection
   
   See Person 2's detailed requirements for full specification.

3. **Services/RBACService.cs** (Role-Based Access Control)
   
   Description: Implement role-based access control determining what actions each role can perform in the system.
   
   Core Responsibilities:
   - Query and determine user's role from database
   - Verify user has permission for requested action
   - Return list of available actions for user's role
   - Support four distinct roles with different capabilities
   
   Role Definitions:
   
   Coordinator (20 staff members):
     Available Actions: ViewAllPatients, AddPatient, EditPatient, DeletePatient, CreateMedicalRecord, 
                       AssignDoctor, AssignTechnician, ViewAllRecords, ViewAllStaff
   
   Doctor/Nurse (100 staff members):
     Available Actions: ViewAssignedPatients, ViewOwnPatientHistory, CreateDiagnosis, UpdateTreatment,
                       CreatePrescription, UpdatePrescription, OrderDiagnosticService, ViewPatientAllergies
   
   Technician (50 staff members):
     Available Actions: ViewAssignedServices, UpdateServiceResults, MarkServiceComplete, 
                       ViewRelatedPatientInfo
   
   Patient (100,000+ users):
     Available Actions: ViewOwnRecords, ViewOwnPrescriptions, ViewAppointmentHistory, UpdateOwnContactInfo
   
   Methods to Implement:
   - CheckUserRole(string username): string - Query user's role from database
     - Execute SELECT query to find user in NHÂNVIÊN table
     - Return VAITRÒ (role) value
     - Handle user-not-found scenario
     - Return null if user not found
   
   - CheckPermission(string username, string action): bool - Verify user's permission
     - Get user's role using CheckUserRole()
     - Cross-reference action against role's available actions list
     - Return true only if role has explicit permission for action
     - Used for access control enforcement throughout application
   
   - GetAvailableActions(string username): List<string> - Return all valid actions for user
     - Determine user's role
     - Return complete list of actions available for that role
     - Used for UI menu/button enablement
     - Prevents users from attempting unauthorized operations
   
   Implementation Requirements:
   - Role definitions must match database security setup from Person 5
   - All role-action mappings must be explicit (whitelist approach)
   - Method calls must be fast (consider caching role definitions)
   - Support for adding new actions in future without code recompilation

4. **Services/VPDService.cs** (Virtual Private Database)
   
   Description: Implement row-level security filtering ensuring users see only data they are authorized to access.
   
   Core Responsibilities:
   - Apply automatic row-level security filtering at database level
   - Ensure doctors see only their assigned patients
   - Ensure coordinators see only assigned records
   - Ensure technicians see only assigned services
   - Implement using Oracle VPD policies with DBMS_RLS package
   
   Methods to Implement:
   - GetVisiblePatients(string doctorId): List<string> - Return patients assigned to doctor
     - Query database for patients assigned to specified doctor
     - VPD policy automatically restricts results to doctor's patients
     - Return list of patientIDs visible to doctor
     - Implementation: SELECT MÃBN FROM BỆNHNHÂN WHERE doctor_id = ? (VPD enforced)
   
   - GetVisibleRecords(string staffId, string role): List<string> - Return records visible to staff
     - Query medical records based on staff role
     - For Coordinator: Return records assigned to coordinator
     - For Doctor: Return records for assigned patients
     - For Technician: Return records related to assigned services
     - VPD policies automatically enforce at database level
   
   - GetVisibleServices(string technicianId): List<string> - Return services assigned to technician
     - Query diagnostic services assigned to technician
     - VPD policy restricts to assigned services only
     - Return serviceIDs visible to technician
   
   Oracle VPD Implementation:
   - Use DBMS_RLS.ADD_POLICY to create VPD policies
   - Define policy functions returning WHERE clause conditions
   - Policies must be attached to tables: HSBA, HSBA_DV, BỆNHNHÂN
   - Policies automatically filter rows based on SYS_CONTEXT values
   - VPD should be configured at database level by Person 5
   - Service methods query data that is already pre-filtered by database
   
   Security Assurance:
   - VPD filtering occurs at database level (mandatory)
   - No way to bypass VPD even with direct database access
   - Service provides transparent filtering to application
   - All queries return only authorized data

5. **Services/OLSService.cs** (Oracle Label Security)
   
   Description: Implement label-based access control using Oracle Label Security with 3-level label hierarchy.
   
   Core Responsibilities:
   - Query user's OLS labels from database
   - Verify label compatibility for accessing protected data
   - Return only accessible notifications based on label hierarchy
   - Support 3-level label hierarchy: Department, Location, Classification
   
   Label Hierarchy Structure:
   
   Level 1 - Department (Hierarchical):
     - Cardiology
     - Gastroenterology
     - Neurology
   
   Level 2 - Location (Hierarchical):
     - Hồ Chí Minh
     - Hải Phòng
     - Hà Nội
   
   Level 3 - Classification (Hierarchical):
     - Staff (least privileged)
     - Department Head (medium privilege)
     - Director (highest privilege)
   
   Methods to Implement:
   - GetUserLabels(string userId): (string Department, string Location, string Classification) - Retrieve user's labels
     - Query OLS user label table
     - Return tuple containing all three label levels
     - Handle user-not-found scenario
   
   - CanAccessNotification(string userId, string notificationDept, string notificationLoc, 
                          string notificationClass): bool - Check label compatibility
     - Get user's labels using GetUserLabels()
     - Compare user labels against notification labels using hierarchy rules
     - AccessRule: User can access if user_level >= notification_level in ALL dimensions
     - Return true only if all three dimensions satisfy access rules
   
   - GetAccessibleNotifications(string userId): List<int> - Return notification IDs user can access
     - Query all notifications from database
     - Filter using CanAccessNotification() for each notification
     - Return complete list of accessible notification IDs
     - Used to populate NotificationForm with proper data
   
   Label Comparison Rules:
   - Labels use hierarchical comparison (higher level can access lower level)
   - Classification: Director >= Department Head >= Staff
   - All three dimensions must satisfy access rules simultaneously
   - Labels form a cartesian product (department x location x classification)
   
   Database Prerequisites:
   - OLS policies must be configured at database level (Person 5)
   - User labels must be assigned in database OLS tables
   - Notification labels must be stored in Notification table
   - DBMS_MACADM package provides OLS management functions

6. **Services/ValidationService.cs**
   
   Description: Provide medical system input validation for patient data and record information.
   
   Methods to Implement:
   - ValidateUsername(string username): bool - Validate username format
     - Check non-null and non-empty
     - Validate format (alphanumeric, underscores allowed)
     - Return false for invalid usernames
   
   - ValidatePassword(string password): bool - Validate password strength
     - Enforce minimum 8 characters
     - Require mixed case (upper and lower)
     - Require at least one number
     - Reject common weak passwords
   
   - ValidatePatientId(string patientId): bool - Validate patient identifier format
     - Check format matches MÃBN structure
     - Verify patient exists in database (optional)
     - Return validation result
   
   - ValidateMedicalRecord(string diagnosis, string treatment, string conclusion): bool - Validate record data
     - Ensure all required fields are non-empty
     - Check length constraints (if applicable)
     - Validate for special characters or injection attempts
     - Return true only if all fields valid

#### Oracle Database Prerequisites

The following database configurations must be completed before authenticationservice can function:
- Security roles must be created: COORDINATOR, DOCTOR, TECHNICIAN, PATIENT
- Users must be assigned to appropriate roles in database
- VPD policies must be configured on HSBA, HSBA_DV tables
- OLS labels must be configured with 3-level hierarchy
- User label assignments must be completed in OLS tables
- Column-level security must be configured on sensitive fields in BỆNHNHÂN table

#### Success Criteria and Acceptance Requirements

Implementation will be considered complete when:
- AuthenticationService successfully authenticates all user types (Coordinator, Doctor, Technician, Patient)
- RBAC correctly restricts actions per role (users cannot perform unauthorized operations)
- VPD filtering works correctly (Doctors see only assigned patients; Coordinators see assigned records; Technicians see assigned services)
- OLS label filtering works correctly (Notifications displayed only if user labels match requirements)
- Services work seamlessly with Person 3's role-specific forms
- All exceptions are handled with meaningful error messages
- Security is enforced at database level (not application level alone)
- Services provide transparent security enforcement (forms call business logic, security enforced automatically)
- Performance is acceptable with minimal overhead from security checks
- All services implement proper logging for security auditing
- Session management is secure (no session information exposed)

---

### Person 5: Subsystem 2 - Business Services and Database Setup
Focus: Database Administration and Business Logic Service Implementation
Estimated Hours: 35-40 hours
Priority Level: Critical

#### Part A: Business Services Implementation (20 hours)

1. **Services/PatientService.cs**
   
   Description: Implement patient data access operations with row-level security ensuring patients see only their own information.
   
   Methods to Implement:
   - GetPatient(string patientId): Patient - Retrieve specific patient with row-level security
     - Query BỆNHNHÂN table by MÃBN
     - Row-level security ensures only authorized access
     - Return populated Patient model object
     - Handle patient-not-found scenario
   
   - UpdatePatientInfo(Patient patient): bool - Update patient contact information
     - Update address, phone, email in BỆNHNHÂN table
     - Only patient can update own information (enforced by row-level security)
     - Validate required fields before update
     - Log update for audit trail
   
   - GetMyMedicalRecords(string patientId): List<MedicalRecord> - Retrieve authenticated patient's records
     - Query HSBA where MÃBN = authenticated patient
     - Return only authenticated patient's records (row-level security enforced)
     - Include all record details: diagnosis, treatment, conclusion, dates
     - Handle empty result set gracefully
   
   - GetMyPrescriptions(string patientId): List<Prescription> - Retrieve patient's prescriptions
     - Query ĐƠNTHUỐC for patient's associated records
     - Return only authenticated patient's prescriptions
     - Include drug name, dosage, instructions
     - Sort by date descending (most recent first)

2. **Services/DoctorService.cs**
   
   Description: Implement clinical operations for doctors with VPD filtering ensuring access only to assigned patients.
   
   Methods to Implement:
   - GetAssignedPatients(string doctorId): List<Patient> - Retrieve doctor's assigned patients
     - Query patients assigned to doctor
     - VPD policy enforces doctor sees only assigned patients
     - Return complete Patient list
     - Query assignment relationship table (create if needed)
   
   - CreateDiagnosis(MedicalRecord record): bool - Create new diagnosis record
     - Insert new record into HSBA table
     - Validate doctor has access to patient (use VPD check)
     - Set record creation timestamp
     - Log action in AuditService
     - Return success/failure status
   
   - UpdatePrescription(Prescription prescription): bool - Update existing prescription
     - Update ĐƠNTHUỐC record
     - Validate doctor has access to patient's records
     - Log change in audit trail
     - Maintain history of prescription changes
   
   - OrderDiagnosticService(DiagnosticService service): bool - Create diagnostic service order
     - Insert into HSBA_DV table
     - Link to appropriate medical record
     - Set pending status
     - Notify technician of new service order
     - Log action in audit trail

3. **Services/CoordinatorService.cs**
   
   Description: Implement comprehensive patient and record management for coordinator role.
   
   Methods to Implement:
   - GetAllPatients(): List<Patient> - Retrieve all patients in system
     - Query complete BỆNHNHÂN table
     - Coordinators can access all patients (no filtering)
     - Return populated Patient list
   
   - AddPatient(Patient patient): bool - Create new patient record
     - Validate all required fields (name, ID, birthdate, etc.)
     - Generate unique patient ID
     - Insert into BỆNHNHÂN table
     - Log action in audit trail
   
   - EditPatient(Patient patient): bool - Update patient information
     - Update BỆNHNHÂN record with new information
     - Validate required fields
     - Log changes for audit trail
   
   - AssignDoctorToPatient(string doctorId, string patientId): bool - Create doctor-patient relationship
     - Create or update assignment record
     - Verify doctor and patient exist
     - Enable VPD policy to filter records for this doctor
   
   - AssignTechnicianToService(string technicianId, string serviceId): bool - Assign diagnostic service
     - Create assignment linking technician to service
     - Enable technician to view and work on service
     - Log assignment action
   
   - GetRecordStatus(string recordId): string - Track medical record workflow status
     - Query HSBA record status
     - Return current status (pending, in progress, completed, archived)

4. **Services/TechnicianService.cs**
   
   Description: Implement diagnostic service management for technicians with assignment-based filtering.
   
   Methods to Implement:
   - GetAssignedServices(string technicianId): List<DiagnosticService> - Retrieve assigned services
     - Query HSBA_DV for services assigned to technician
     - VPD policy restricts to assigned services
     - Return complete service details
   
   - UpdateServiceResult(string serviceId, string result): bool - Record test results
     - Update KẾTQUẢ field in HSBA_DV
     - Record test date and time
     - Log results entry in audit trail
     - Mark service as in-progress
   
   - CompleteService(string serviceId): bool - Mark service complete
     - Update status to completed in HSBA_DV
     - Record completion timestamp
     - Log completion event
     - Notify relevant parties of completion

5. **Services/AuditService.cs** [CRITICAL - IMPORTANT FOR COMPLIANCE]
   
   Description: Implement comprehensive audit logging for security and compliance verification.
   
   Methods to Implement:
   - LogUserAction(string userId, string action, string details): bool - Log user actions
     - Insert audit record into AuditLog table
     - Include: userId, timestamp, action, details, IP address
     - Log all significant operations for compliance
     - Return success/failure status
   
   - GetAuditLogs(DateTime startDate, DateTime endDate, string specificUser = null): List<AuditLogEntry> - Query audit records
     - Query AuditLog table with date range filter
     - Filter by specific user if provided
     - Return complete audit trail for reporting
     - Used for compliance verification and investigations
   
   - LogSensitiveAccess(string userId, string dataType, string recordId): bool - Log sensitive data access
     - Log access to medical records, prescriptions, diagnoses
     - Record what data was accessed and when
     - Used to verify HIPAA compliance
     - Track data access patterns for security monitoring

#### Part B: Database Setup Implementation (20 hours)

This section requires creating SQL scripts that are executed against Oracle database. All scripts must:
- Include detailed comments explaining each command
- Handle pre-existing objects gracefully
- Include appropriate error handling
- Use proper Oracle syntax and conventions
- Never hardcode usernames or passwords
- Include transaction control (COMMIT/ROLLBACK)

1. **Database/Schema/01_CreateTables.sql** [CRITICAL - IMPLEMENT FIRST]
   
   Purpose: Create the foundation table structures for the medical management system.
   
   Tables to Create:
   
   BỆNHNHÂN (Patient) Table
   - MÃBN: VARCHAR2(10) - Primary Key - Patient ID
   - TÊNBN: VARCHAR2(100) - Patient Name
   - PHÁI: CHAR(1) - Gender (M/F)
   - NGÀYSINH: DATE - Date of Birth
   - CCCD: VARCHAR2(20) - National ID Number
   - DiaChi: VARCHAR2(255) - Address
   - DiUng: VARCHAR2(255) - Drug Allergies
   - SoDienThoai: VARCHAR2(20) - Phone Number
   - Email: VARCHAR2(100) - Email Address
   - CreatedDate: TIMESTAMP - Record creation timestamp
   - Constraints: Primary key on MÃBN, unique constraint on CCCD
   
   NHÂNVIÊN (Staff) Table
   - MÃNV: VARCHAR2(10) - Primary Key - Staff ID
   - HỌTÊN: VARCHAR2(100) - Full Name
   - VAITRÒ: VARCHAR2(20) - Role (Coordinator/Doctor/Technician)
   - CHUYÊNKHOA: VARCHAR2(50) - Specialty
   - SoDienThoai: VARCHAR2(20) - Contact number
   - Email: VARCHAR2(100) - Email
   - CreatedDate: TIMESTAMP - Hire date/record creation
   - Constraints: Primary key on MÃNV
   
   HSBA (Medical Record) Table
   - MÃHSBA: VARCHAR2(15) - Primary Key - Record ID
   - MÃBN: VARCHAR2(10) - Foreign Key to BỆNHNHÂN
   - MÃNV: VARCHAR2(10) - Foreign Key to NHÂNVIÊN (doctor)
   - CHẨNĐOÁN: VARCHAR2(500) - Diagnosis
   - ĐIỀUTRỊ: VARCHAR2(1000) - Treatment
   - KẾTLUẬN: VARCHAR2(500) - Conclusion
   - TaoBanGhi: TIMESTAMP - Record creation date
   - UpdateDate: TIMESTAMP - Last update
   - Status: VARCHAR2(20) - Status (Pending/Active/Completed/Archived)
   - Constraints: Primary key on MÃHSBA, foreign keys on MÃBN and MÃNV
   
   HSBA_DV (Diagnostic Service) Table
   - MÃDV: VARCHAR2(15) - Primary Key - Service ID
   - MÃHSBA: VARCHAR2(15) - Foreign Key to HSBA
   - MÃNV: VARCHAR2(10) - Foreign Key to NHÂNVIÊN (technician)
   - TenDichVu: VARCHAR2(100) - Service/Test Name
   - Ngay: TIMESTAMP - Service date
   - KẾTQUẢ: VARCHAR2(1000) - Test results
   - HoanThanh: CHAR(1) - Completion flag (Y/N)
   - Status: VARCHAR2(20) - Status (Pending/InProgress/Completed)
   - Constraints: Primary key on MÃDV, foreign keys on MÃHSBA and MÃNV
   
   ĐƠNTHUỐC (Prescription) Table
   - MÃĐƠN: VARCHAR2(15) - Primary Key - Prescription ID
   - MÃHSBA: VARCHAR2(15) - Foreign Key to HSBA
   - TÊNHÓA: VARCHAR2(100) - Drug Name
   - LIỀU: VARCHAR2(100) - Dosage
   - HƯỚNGDẪN: VARCHAR2(500) - Instructions
   - NgayDangKy: TIMESTAMP - Registration date
   - NgayHetHan: DATE - Expiration date
   - Constraints: Primary key on MÃĐƠN, foreign key on MÃHSBA
   
   Notification Table
   - NotificationId: NUMBER - Primary Key
   - Title: VARCHAR2(200) - Notification title
   - Content: VARCHAR2(2000) - Notification content
   - Department: VARCHAR2(50) - Department label for OLS
   - Location: VARCHAR2(50) - Location label for OLS
   - Classification: VARCHAR2(50) - Classification label for OLS
   - CreatedDate: TIMESTAMP - Creation date
   - Constraints: Primary key on NotificationId
   
   AuditLog Table
   - AuditId: NUMBER - Primary Key (auto-increment)
   - UserId: VARCHAR2(20) - User who performed action
   - ActionTime: TIMESTAMP - When action occurred
   - Action: VARCHAR2(100) - Action type
   - TableName: VARCHAR2(30) - Affected table
   - RecordId: VARCHAR2(30) - Affected record
   - Details: VARCHAR2(1000) - Additional details
   - IPAddress: VARCHAR2(15) - User's IP address
   - Constraints: Primary key on AuditId, index on UserId and ActionTime

2. **Database/Schema/02_CreateIndexes.sql**
   
   Purpose: Create indexes for performance optimization on frequently queried columns.
   
   Indexes to Create:
   - Index on BỆNHNHÂN(MÃBN) - For patient lookups
   - Index on BỆNHNHÂN(CCCD) - For ID-based searches
   - Index on NHÂNVIÊN(MÃNV) - For staff lookups
   - Index on HSBA(MÃBN) - For patient's medical records queries
   - Index on HSBA(MÃHSBA) - For record lookups
   - Index on HSBA_DV(MÃHSBA) - For services linked to records
   - Index on ĐƠNTHUỐC(MÃHSBA) - For prescriptions linked to records
   - Index on AuditLog(UserId) - For audit log queries by user
   - Index on AuditLog(ActionTime) - For audit log queries by date
   - Composite index on AuditLog(UserId, ActionTime) - For combined queries
   - Index on Notification(Department, Location, Classification) - For OLS filtering
   
   Index creation should consider:
   - Column selectivity
   - Query patterns
   - Write performance impact
   - Storage requirements

3. **Database/Schema/03_InsertSampleData.sql**
   
   Purpose: Insert test data for development, testing, and demonstration.
   
   Sample Data Requirements:
   - 100 sample patients with realistic information
     - Varied names (Vietnamese)
     - Realistic birthdates and ages
     - Diverse IDs and addresses
   
   - 170 staff members distributed:
     - 20 Coordinators
     - 100 Doctors/Nurses
     - 50 Technicians
   
   - 20 sample medical records with:
     - Patient-doctor associations
     - Realistic diagnoses and treatments
     - Varied dates (some historical records)
   
   - 50 sample prescriptions with:
     - Links to medical records
     - Realistic drug names and dosages
     - Instructions and expiration dates
   
   - 10 diagnostic services with:
     - Links to medical records
     - Service types (X-ray, Lab test, Ultrasound, etc.)
     - Some completed with results, some pending
   
   - 15 sample notifications with:
     - Varied departments and locations
     - Realistic content
     - OLS labels for testing label-based filtering
   
   Data should be realistic and representative of actual medical system operations.

4. **Database/Security/01_RBAC_Setup.sql**
   
   Purpose: Configure role-based access control with four distinct roles.
   
   Roles to Create:
   
   COORDINATOR Role
   - Grant: SELECT, INSERT, UPDATE on BỆNHNHÂN
   - Grant: SELECT, INSERT, UPDATE on HSBA
   - Grant: SELECT on NHÂNVIÊN
   - Grant: SELECT on HSBA_DV
   - Purpose: Manage patients and medical records, assign doctors and technicians
   
   DOCTOR Role
   - Grant: SELECT on BỆNHNHÂN (through VPD)
   - Grant: SELECT, INSERT, UPDATE on HSBA (through VPD)
   - Grant: INSERT, UPDATE on ĐƠNTHUỐC
   - Grant: INSERT on HSBA_DV
   - Grant: SELECT on NHÂNVIÊN
   - Purpose: Manage patient care, create diagnoses, prescriptions, order services
   
   TECHNICIAN Role
   - Grant: SELECT on HSBA_DV (through VPD)
   - Grant: UPDATE on HSBA_DV (through VPD)
   - Grant: SELECT on BỆNHNHÂN (for patient info)
   - Grant: SELECT on HSBA
   - Purpose: Update diagnostic service results, mark services complete
   
   PATIENT Role
   - Grant: SELECT on BỆNHNHÂN (single row - own record)
   - Grant: UPDATE on BỆNHNHÂN (own contact info only)
   - Grant: SELECT on HSBA (own records through row-level security)
   - Grant: SELECT on ĐƠNTHUỐC (own prescriptions)
   - Purpose: View own medical records and update contact information
   
   All roles should be created with CREATE ROLE statements followed by GRANT statements.

5. **Database/Security/02_VPD_Setup.sql**
   
   Purpose: Implement Virtual Private Database policies for row-level security.
   
   VPD Policies to Create:
   
   HSBA Table Policy (Medical Records Filtering)
   - Policy name: HSBA_VPD_Policy
   - Policy function: Returns WHERE clause condition
   - Logic:
     - Doctors: WHERE MÃNV = SYS_CONTEXT('USERENV','SESSION_USER')
     - Coordinators: WHERE assigned_coordinator_id = current_user
     - Technicians: No direct access to HSBA (access through HSBA_DV)
   
   HSBA_DV Table Policy (Service Filtering)
   - Policy name: HSBA_DV_VPD_Policy
   - Policy function: Returns WHERE condition
   - Logic:
     - Technicians: WHERE MÃNV = SYS_CONTEXT('USERENV','SESSION_USER')
   
   Implementation Requirements:
   - Use DBMS_RLS package
   - Create policy functions that return dynamic WHERE clauses
   - Policies must be transparently applied to all queries
   - Performance should not be significantly impacted
   - Test queries to verify VPD enforcement

6. **Database/Security/03_OLS_Setup.sql**
   
   Purpose: Configure Oracle Label Security with 3-level hierarchy for notifications.
   
   OLS Label Configuration:
   
   Label Hierarchy:
   - Level 1: DEPARTMENTS (Cardiology, Gastroenterology, Neurology)
   - Level 2: LOCATIONS (Hồ Chí Minh, Hải Phòng, Hà Nội)
   - Level 3: CLASSIFICATIONS (Staff, DepartmentHead, Director)
   
   Policy Creation:
   - Create OLS policy using DBMS_MACADM
   - Define level component values
   - Create level components
   - Set policy properties
   - Apply policy to Notification table
   
   Label Assignment:
   - Labels must be assigned to database users
   - Users can access only notifications with labels <= their labels in hierarchy
   - Labels form logical groupings for data organization

7. **Database/Security/04_Users_Creation.sql**
   
   Purpose: Create database users for testing with appropriate role assignments and OLS labels.
   
   Test Users to Create:
   
   2 Directors (All labels - highest access):
   - user_dir_001: Coordinator role
   - user_dir_002: Coordinator role
   
   2 Department Heads (Department + Location specific):
   - user_dh_001: Doctor role, Cardiology, Hồ Chí Minh
   - user_dh_002: Technician role, Gastroenterology, Hải Phòng
   
   4 Staff (Single location/department):
   - user_staff_001: Doctor role, Cardiology, Hồ Chí Minh
   - user_staff_002: Doctor role, Neurology, Hà Nội
   - user_staff_003: Technician role, Gastroenterology, Hải Phòng
   - user_staff_004: Patient role
   
   User Creation Requirements:
   - Create users with CREATE USER statements
   - Grant appropriate roles
   - Assign OLS labels
   - Set default tablespace
   - Enable user account
   - Never hardcode passwords in script (use placeholder or read from input)

8. **Database/Audit/01_StandardAudit_Setup.sql**
   
   Purpose: Enable Oracle standard auditing for basic security compliance.
   
   Audit Configuration:
   - AUDIT ALL STATEMENTS by users - Log all user activity
   - AUDIT CONNECT - Log user logins
   - AUDIT DISCONNECT - Log logouts
   - AUDIT SELECT, INSERT, UPDATE, DELETE ON HSBA - Monitor medical records
   - AUDIT INSERT, UPDATE, DELETE ON BỆNHNHÂN - Monitor patient data modifications
   - AUDIT INSERT, UPDATE, DELETE ON ĐƠNTHUỐC - Monitor prescriptions
   
   Storage:
   - By default, audit trail stored in DBA_AUDIT_TRAIL
   - Can be redirected to external OS files
   - Regular cleanup of old audit records necessary

9. **Database/Audit/02_FineGrainedAudit_Setup.sql**
   
   Purpose: Implement fine-grained auditing on sensitive data columns.
   
   Fine-Grained Audit Policies:
   - Policy on HSBA: Monitor INSERT, UPDATE, DELETE operations
   - Policy on ĐƠNTHUỐC: Monitor all operations on prescriptions
   - Policy on BỆNHNHÂN: Monitor SELECT operations on sensitive columns (phone, address)
   
   Using DBMS_FGA package:
   - Create FGA policies using DBMS_FGA.ADD_POLICY
   - Define audit conditions
   - Specify columns to monitor
   - Enable policy
   - Results stored in FGA_LOG$ table

10. **Database/Audit/03_UnifiedAudit_Setup.sql**
    
    Purpose: Implement modern Oracle Unified Auditing for comprehensive compliance.
    
    Unified Audit Configuration:
    - CREATE AUDIT POLICY for RBAC violations
    - CREATE AUDIT POLICY for VPD policy violations
    - CREATE AUDIT POLICY for sensitive data access
    - Enable audit policies
    - Results stored in UNIFIED_AUDIT_TRAIL
    
    This provides comprehensive auditing for regulatory compliance.

11. **Database/Audit/ReadAuditLogs.sql**
    
    Purpose: Provide queries for reading and analyzing audit logs.
    
    Sample Queries:
    - Query DBA_AUDIT_TRAIL for standard audit records
    - Query FGA_LOG$ for fine-grained audit records
    - Query UNIFIED_AUDIT_TRAIL for unified audit records
    - Queries with filtering by: date range, user, object, operation type
    - Aggregate queries for trend analysis
    
    These queries are used by AuditService.GetAuditLogs() method.

12. **Database/BackupRestore/Backup_Recovery_Documentation.md**
    
    Purpose: Document backup strategies and recovery procedures.
    
    Content Required:
    - Backup Strategy 1: RMAN (Recovery Manager)
      - Configuration
      - Incremental backup strategy
      - Retention policies
      - Recovery procedures
    
    - Backup Strategy 2: Export/Data Pump
      - Schema export procedures
      - Data export procedures
      - Import procedures
    
    - Recovery Procedures:
      - Point-in-time recovery
      - Full database recovery
      - Selective object recovery
      - Testing recovery procedures
    
    - Testing and Validation:
      - How to test backup completeness
      - How to verify recovery procedures
      - Recovery time objectives (RTO)
      - Recovery point objectives (RPO)

#### Success Criteria and Acceptance Requirements

Implementation will be considered complete when:
- All business service methods are implemented and functional
- All database tables are created with proper constraints and data types
- Index creation improves query performance
- Sample data is sufficient for testing all functionality
- All roles are created with appropriate permissions
- VPD policies enforce row-level security at database level
- OLS labels are configured and functional
- All audit mechanisms are operational and recording events
- Services integrate seamlessly with Person 4's security services
- All SQL scripts execute successfully without errors
- No credentials are hardcoded in any scripts
- Database supports all four access control mechanisms (RBAC, VPD, OLS, Audit)
- Backup strategy is documented and recovery procedures are tested
- Audit logs can be successfully queried and analyzed
- Performance is acceptable for the intended user base
- All data modifications are recorded for compliance

---

## Implementation Sequence and Critical Dependencies

**See [TRACEABILITY_MATRIX.md](TRACEABILITY_MATRIX.md) for detailed test execution timeline and complete dependency mapping.**

**CRITICAL: Person 5's Week 1 database completion (Fri 2/14) blocks all other team members. No delays acceptable.**

## Implementation Sequence and Critical Dependencies

### Week 1: Foundation Phase (Feb 10-14)

Person 5 (Database Setup)
- `01_CreateTables.sql` - 7 tables with constraints
- `02_CreateIndexes.sql` - Performance indexes
- `03_InsertSampleData.sql` - 100 patients, 170 staff, sample records
- Test ODP.NET connection
- **Deadline: Fri 2/14** (blocks everyone if late)

### Week 2: Security & Services (Feb 17-21)

Person 5 (Database Security)
- Wed 2/19: `04_Users_Creation.sql`, `05_RBAC_Setup.sql`
- Thu 2/20: `06_VPD_Setup.sql`, `07_OLS_Setup.sql`
- Fri 2/21: `08_StandardAudit_Setup.sql`, `09_FineGrainedAudit_Setup.sql`
- All business services implemented (PatientService, DoctorService, etc.)

Person 4 (Security Services)
- AuthenticationService & RBACService (Wed 2/19)
- VPDService & OLSService (Thu 2/20)
- Integration testing (Fri 2/21)

Person 2 (Business Services for Subsystem 1)
- Implement all services by Fri 2/21
- UserService, RoleService, PermissionService, PrivilegeService

### Week 3: UI Implementation (Feb 24-28)

Person 1 (Subsystem 1 Forms)
- Implement: 5 forms (MainForm, UserManagementForm, RoleManagementForm, PermissionForm, PrivilegeViewerForm)
- Test integration with services
- **Deadline: Fri 2/28**

Person 3 (Subsystem 2 Forms)
- Implement: 7 forms (LoginForm, CoordinatorForm, DoctorForm, TechnicianForm, PatientForm, NotificationForm)
- Verify VPD and OLS filtering work correctly
- **Deadline: Fri 2/28**

### Week 4: Integration Testing (Mar 3-7)

All team members:
- **Mon-Tue (3/3-4):** End-to-end testing of both subsystems
- **Tue-Wed (3/4-5):** Security verification (RBAC, VPD, OLS)
- **Wed-Thu (3/5-6):** Audit & backup testing, performance benchmarking
- **Fri (3/7):** Bug fixes, final documentation, GitHub commit

## Critical Blockers and Dependencies

**CRITICAL:** Person 5's database completion is the single blocking item. See [TRACEABILITY_MATRIX.md](TRACEABILITY_MATRIX.md) for detailed dependency chains and timeline.

**Key Dates:**
- **Fri 2/14:** Database tables, indexes, sample data (blocks everyone)
- **Wed 2/19:** Roles, users (blocks Person 4 authentication)
- **Thu 2/20:** VPD, OLS (blocks Person 4 services, Person 3 forms)
- **Fri 2/21:** Audit, business services (unblocks all forms)
- **Fri 2/28:** All forms complete
- **Fri 3/7:** All testing complete

## Team Communication Requirements

Weekly Sync Meetings (Mandatory):
- Monday 9 AM: Status review and planning
- Wednesday 2 PM: Technical discussion and problem-solving
- Friday 4 PM: Demo and planning

Communication Channels:
- GitHub issues for bug tracking
- Git commits with descriptive messages
- Code review before merging to main branch

---

**For detailed test execution timeline, dependencies, and pass criteria, see [TRACEABILITY_MATRIX.md](TRACEABILITY_MATRIX.md).**
