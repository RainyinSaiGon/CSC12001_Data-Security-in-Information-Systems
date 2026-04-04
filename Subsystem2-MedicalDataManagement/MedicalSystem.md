# Medical Data Management System

## 1. Tổng quan hệ thống

Medical Data Management System là ứng dụng C# Windows Forms phục vụ quản lý dữ liệu khám chữa bệnh trên Oracle Database.

Mục tiêu chính:

- Quản lý bệnh nhân, hồ sơ bệnh án, đơn thuốc, dịch vụ cận lâm sàng.
- Hỗ trợ nhiều vai trò: điều phối viên, bác sĩ, kỹ thuật viên, bệnh nhân.
- Kết hợp cơ chế bảo mật dữ liệu theo vai trò (RBAC), theo dòng dữ liệu (VPD) và thông báo theo nhãn (OLS).

## 2. Cách tổ chức thư mục

Project được tách thành 3 nhóm chính:

- Forms
  - Tầng giao diện người dùng, xử lý tương tác UI.
- Models
  - Tầng đối tượng dữ liệu, định nghĩa cấu trúc entity.
- Services
  - Tầng nghiệp vụ và truy cập dữ liệu Oracle.

Tổ chức này giúp tách rõ UI, dữ liệu và logic xử lý để dễ bảo trì.

## 3. Mô tả chi tiết từng folder

### Forms

- Chứa các màn hình theo từng vai trò nghiệp vụ.
- Mỗi form có:
  - File `.cs`: code xử lý sự kiện, gọi service, cập nhật giao diện.
  - File `.Designer.cs`: code sinh tự động cho control/layout.

### Models

- Chứa class biểu diễn dữ liệu bảng/view Oracle.
- Dùng để map dữ liệu từ `OracleDataReader` và truyền dữ liệu giữa Form và Service.

### Services

- Chứa logic truy vấn/cập nhật dữ liệu, phân quyền và xác thực.
- Tập trung các thao tác CRUD, validation, authentication để Form không chứa SQL.

## 4. Mô tả chi tiết từng file

### Forms (CHI TIẾT TỪNG FILE)

#### CoordinatorForm

- Chức năng chính:
  - Dashboard cho điều phối viên: thêm bệnh nhân, tạo hồ sơ bệnh án, phân công bác sĩ, phân công kỹ thuật viên cho dịch vụ.
- Các control quan trọng (button, textbox, datagridview...):
  - `_patientsGrid` (`DataGridView`): danh sách bệnh nhân.
  - `_doctorComboBox`, `_technicianComboBox` (`ComboBox`): chọn nhân sự.
  - `_nameTextBox`, `_cccdTextBox`, `_addressTextBox`, `_medicalHistoryTextBox`, `_familyHistoryTextBox`, `_allergyTextBox`.
  - `_patientIdTextBox`, `_recordIdTextBox`, `_serviceTypeTextBox`, `_serviceDatePicker`.
  - Button: `Add patient`, `Refresh`, `Create record + assign doctor`, `Assign technician`, `Notifications`.
- Các sự kiện chính:
  - `addButton.Click` -> `AddPatient()`.
  - `refreshButton.Click` -> `RefreshPatients()`.
  - `assignDoctorButton.Click` -> `CreateRecord()`.
  - `assignTechnicianButton.Click` -> `AssignTechnician()`.
  - `notificationsButton.Click` -> mở `NotificationForm`.
- Các phương thức xử lý:
  - `BuildUi()`: dựng toàn bộ layout và gắn sự kiện.
  - `LoadReferenceData()`: nạp danh sách bác sĩ/kỹ thuật viên từ service.
  - `RefreshPatients()`: tải danh sách bệnh nhân.
  - `AddPatient()`: parse địa chỉ, tạo `Patient`, gọi `CoordinatorService.AddPatient`.
  - `CreateRecord()`: tạo hồ sơ bệnh án và gán bác sĩ.
  - `AssignTechnician()`: gán kỹ thuật viên cho dịch vụ trong hồ sơ.
- Luồng hoạt động:
  - Người dùng nhập thông tin bệnh nhân -> nhấn `Add patient` -> Form tạo object `Patient` -> gọi `CoordinatorService.AddPatient` -> insert vào `BENHNHAN` -> reload grid.
  - Người dùng nhập `Patient ID` + chọn bác sĩ -> nhấn tạo hồ sơ -> gọi `CreateMedicalRecord` -> insert `HSBA`.
  - Người dùng nhập `Record ID`, loại dịch vụ, ngày, chọn kỹ thuật viên -> nhấn `Assign technician` -> gọi `AssignTechnician` (MERGE `HSBA_DV`) -> hiện thông báo thành công/thất bại.
- Tương tác với:
  - Service: `CoordinatorService`, `OracleConnectionService` (gián tiếp qua `CoordinatorService`), `OLSService` (qua `NotificationForm`).
  - Model: `UserSession`, `Patient`, `Staff`.
  - Form khác: `NotificationForm`.

#### DoctorForm

- Chức năng chính:
  - Dashboard bác sĩ: xem bệnh nhân được phân công, cập nhật chẩn đoán/điều trị/kết luận, chỉ định dịch vụ, lưu đơn thuốc.
- Các control quan trọng (button, textbox, datagridview...):
  - `_patientsGrid`: danh sách bệnh nhân theo bác sĩ.
  - `_recordsGrid`: danh sách hồ sơ bệnh án theo bác sĩ.
  - `_recordIdTextBox`, `_diagnosisTextBox`, `_treatmentTextBox`, `_conclusionTextBox`.
  - `_serviceTypeTextBox`, `_serviceDatePicker`.
  - `_prescriptionNameTextBox`, `_prescriptionDoseTextBox`, `_prescriptionDatePicker`.
  - Button: `Update record`, `Order service`, `Save prescription`, `Notifications`.
- Các sự kiện chính:
  - `updateRecordButton.Click` -> `UpdateRecord()`.
  - `addServiceButton.Click` -> `AddService()`.
  - `savePrescriptionButton.Click` -> `SavePrescription()`.
  - `notificationsButton.Click` -> mở `NotificationForm`.
- Các phương thức xử lý:
  - `BuildUi()`: dựng layout bác sĩ.
  - `RefreshData()`: nạp bệnh nhân và hồ sơ theo `StaffId`.
  - `UpdateRecord()`: tạo `MedicalRecord`, gọi `DoctorService.UpdateMedicalRecord`.
  - `AddService()`: tạo `DiagnosticService`, gọi `DoctorService.OrderDiagnosticService`.
  - `SavePrescription()`: tạo `Prescription`, gọi `DoctorService.UpdatePrescription`.
- Luồng hoạt động:
  - Bác sĩ mở màn hình -> `RefreshData()` gọi `GetAssignedPatients` và `GetAssignedMedicalRecords` -> bind lên 2 grid.
  - Bác sĩ nhập `Record ID` và chẩn đoán -> nhấn cập nhật -> cập nhật `HSBA`.
  - Bác sĩ nhập dịch vụ cận lâm sàng -> nhấn `Order service` -> insert vào `HSBA_DV`.
  - Bác sĩ nhập thuốc/liều dùng/ngày -> `Save prescription` -> MERGE vào `DONTHUOC` (tạo mới hoặc cập nhật liều).
- Tương tác với:
  - Service: `DoctorService`, `VPDService` (khởi tạo kèm), `OracleConnectionService`, `OLSService` (qua `NotificationForm`).
  - Model: `UserSession`, `Patient`, `MedicalRecord`, `DiagnosticService`, `Prescription`.
  - Form khác: `NotificationForm`.

#### LoginForm

- Chức năng chính:
  - Đăng nhập Oracle user, kiểm tra input cơ bản, xác định role và chuyển sang form đúng vai trò.
- Các control quan trọng (button, textbox, datagridview...):
  - `_usernameTextBox`, `_passwordTextBox`, `_dataSourceTextBox`.
  - `_loginButton`, `_statusLabel`.
  - Layout chính: `TableLayoutPanel`.
- Các sự kiện chính:
  - `_loginButton.Click` -> `HandleLogin(...)`.
- Các phương thức xử lý:
  - `BuildUi()`: dựng form đăng nhập, nạp mặc định `ORACLE_DATA_SOURCE`.
  - `HandleLogin(...)`: validate input, gọi authenticate, điều hướng form theo role.
  - `ShowStatus(message)`: hiển thị lỗi/trạng thái đăng nhập.
- Luồng hoạt động:
  - Người dùng nhập username/password/data source -> bấm login.
  - Form gọi `ValidationService.ValidateUsername/ValidatePassword`.
  - Nếu hợp lệ -> gọi `AuthenticationService.Authenticate`.
  - Service truy vấn `V_SELF_NHANVIEN` hoặc `V_SELF_BENHNHAN` để tạo `UserSession`.
  - Form switch theo `session.Role` để mở `CoordinatorForm`/`DoctorForm`/`TechnicianForm`/`PatientForm`.
  - Form login ẩn đi, form mới đóng thì login đóng theo.
- Tương tác với:
  - Service: `ValidationService`, `AuthenticationService`.
  - Model: `UserSession`.
  - Form khác: `CoordinatorForm`, `DoctorForm`, `TechnicianForm`, `PatientForm`.

#### MainForm

- Chức năng chính:
  - Form khung chính, hiện tại đóng vai trò placeholder/dispatcher (chưa chứa logic nghiệp vụ cụ thể).
- Các control quan trọng (button, textbox, datagridview...):
  - Chưa có control nghiệp vụ riêng trong file `.cs`.
- Các sự kiện chính:
  - Chưa khai báo event xử lý cụ thể.
- Các phương thức xử lý:
  - Constructor `MainForm()` gọi `InitializeComponent()`.
- Luồng hoạt động:
  - Khi được mở, form chỉ khởi tạo giao diện theo phần Designer.
  - Luồng role-based chính đang chạy trực tiếp từ `LoginForm`, chưa đi qua `MainForm`.
- Tương tác với:
  - Chủ yếu tương tác với phần `Designer`; chưa gọi trực tiếp Service/Model trong code hiện tại.

#### NotificationForm

- Chức năng chính:
  - Hiển thị danh sách thông báo mà user hiện tại có thể truy cập.
- Các control quan trọng (button, textbox, datagridview...):
  - `_grid` (`DataGridView`): hiển thị danh sách `Notification`.
  - `Label` header: hiển thị tên user và role.
- Các sự kiện chính:
  - Không có button event; dữ liệu được nạp khi form khởi tạo.
- Các phương thức xử lý:
  - `BuildUi()`: dựng header + grid.
  - `LoadNotifications()`: gọi service lấy danh sách thông báo và bind grid.
- Luồng hoạt động:
  - Form mở từ các dashboard -> constructor gọi `LoadNotifications()`.
  - `OLSService.GetAccessibleNotificationsDetailed()` query bảng `THONGBAO`.
  - Trả về `List<Notification>` -> bind vào `_grid`.
  - Nếu lỗi Oracle/SQL -> hiển thị `MessageBox`.
- Tương tác với:
  - Service: `OLSService`, `OracleConnectionService`.
  - Model: `UserSession`, `Notification`.
  - Form gọi tới: `CoordinatorForm`, `DoctorForm`, `PatientForm`, `TechnicianForm`.

#### PatientForm

- Chức năng chính:
  - Cổng thông tin bệnh nhân: xem thông tin cá nhân, cập nhật địa chỉ/tiền sử/dị ứng, xem hồ sơ bệnh án và đơn thuốc của chính mình.
- Các control quan trọng (button, textbox, datagridview...):
  - Nhóm profile: `_sonhaTextBox`, `_tenduongTextBox`, `_quanhuyenTextBox`, `_tinhtpTextBox`, `_tiensuTextBox`, `_tiensuGiaDinhTextBox`, `_diungTextBox`, `_identityLabel`.
  - `_recordsGrid`: danh sách hồ sơ bệnh án.
  - `_prescriptionsGrid`: danh sách đơn thuốc.
  - `TabControl` với 2 tab `Medical Records` và `Prescriptions`.
  - Button: `Save profile`, `Notifications`.
- Các sự kiện chính:
  - `saveButton.Click` -> `SaveProfile()`.
  - `notificationsButton.Click` -> mở `NotificationForm`.
- Các phương thức xử lý:
  - `BuildUi()`: dựng layout profile + tab dữ liệu.
  - `LoadData()`: tải thông tin bệnh nhân, hồ sơ và đơn thuốc từ view bảo mật.
  - `SaveProfile()`: tạo object `Patient` từ input và gọi cập nhật.
- Luồng hoạt động:
  - Form mở với `UserSession.PatientId` -> `LoadData()`.
  - Gọi `PatientService.GetPatient` để lấy thông tin từ `V_SELF_BENHNHAN`.
  - Gọi `GetMyMedicalRecords` (`V_PATIENT_HSBA`) và `GetMyPrescriptions` (`V_PATIENT_DONTHUOC`) -> bind lên 2 grid.
  - Khi lưu profile -> gọi `UpdatePatientInfo` -> cập nhật view `V_SELF_BENHNHAN` -> reload dữ liệu.
- Tương tác với:
  - Service: `PatientService`, `OracleConnectionService`, `OLSService` (qua `NotificationForm`).
  - Model: `UserSession`, `Patient`, `MedicalRecord`, `Prescription`.
  - Form khác: `NotificationForm`.

#### TechnicianForm

- Chức năng chính:
  - Dashboard kỹ thuật viên: xem danh sách dịch vụ được phân công và nhập kết quả thực hiện.
- Các control quan trọng (button, textbox, datagridview...):
  - `_servicesGrid`: danh sách dịch vụ từ `V_TECHNICIAN_HSBA_DV`.
  - `_recordIdTextBox`, `_serviceTypeTextBox`, `_serviceDatePicker`, `_resultTextBox`.
  - Button: `Save result`, `Notifications`.
- Các sự kiện chính:
  - `saveButton.Click` -> `SaveResult()`.
  - `notificationsButton.Click` -> mở `NotificationForm`.
- Các phương thức xử lý:
  - `BuildUi()`: dựng form.
  - `RefreshData()`: tải dịch vụ theo kỹ thuật viên.
  - `SaveResult()`: gọi `TechnicianService.UpdateServiceResult(...)`.
- Luồng hoạt động:
  - Form mở -> `RefreshData()` lấy danh sách dịch vụ đã gán cho `StaffId` hiện tại.
  - KTV nhập khóa bản ghi (recordId + serviceType + date) và kết quả.
  - `SaveResult()` cập nhật `HSBA_DV.KETQUA`.
  - Thành công thì reload grid; lỗi thì báo MessageBox.
- Tương tác với:
  - Service: `TechnicianService`, `VPDService`, `OracleConnectionService`, `OLSService` (qua `NotificationForm`).
  - Model: `UserSession`, `DiagnosticService`.
  - Form khác: `NotificationForm`.

### Models (CHI TIẾT TỪNG FILE)

#### Patient.cs

- Ý nghĩa:
  - Entity đại diện bảng/thông tin bệnh nhân, được dùng xuyên suốt các nghiệp vụ tiếp nhận, khám và cập nhật hồ sơ cá nhân.
- Các thuộc tính chi tiết (ví dụ giả định hợp lý):
  - `MABN`: mã bệnh nhân (khóa chính).
  - `TENBN`: họ tên bệnh nhân.
  - `PHAI`: giới tính.
  - `NGAYSINH`: ngày sinh.
  - `CCCD`: căn cước.
  - `SONHA`, `TENDUONG`, `QUANHUYEN`, `TINHTP`: thông tin địa chỉ.
  - `TIENSUBENH`: tiền sử bệnh cá nhân.
  - `TIENSUBENHGD`: tiền sử bệnh gia đình.
  - `DIUNGTHUOC`: thông tin dị ứng thuốc.
  - `USERNAME`: Oracle username map với bệnh nhân.
- Vai trò trong hệ thống:
  - Là object truyền dữ liệu giữa UI và DB trong các ca thêm/sửa/xem bệnh nhân.
- Được sử dụng ở đâu (Form/Service nào):
  - Forms: `CoordinatorForm`, `DoctorForm`, `PatientForm`.
  - Services: `CoordinatorService`, `DoctorService`, `PatientService`, `VPDService`.

#### Staff.cs

- Ý nghĩa:
  - Entity nhân viên y tế dùng cho danh sách bác sĩ, kỹ thuật viên và thông tin vai trò nội bộ.
- Các thuộc tính chi tiết (ví dụ giả định hợp lý):
  - `MANV`: mã nhân viên.
  - `HOTEN`: họ tên.
  - `PHAI`, `NGAYSINH`, `CMND`, `QUEQUAN`, `SODT`: thông tin cá nhân.
  - `VAITRO`: vai trò nghiệp vụ (điều phối viên/bác sĩ/kỹ thuật viên...).
  - `CHUYENKHOA`: chuyên khoa/phòng ban.
  - `USERNAME`: Oracle username của nhân viên.
- Vai trò trong hệ thống:
  - Là dữ liệu nguồn cho combobox phân công bác sĩ/kỹ thuật viên và cho logic phân quyền theo vai trò.
- Được sử dụng ở đâu (Form/Service nào):
  - Forms: `CoordinatorForm` (combo doctor/technician).
  - Services: `CoordinatorService` (GetDoctors/GetTechnicians), `AuthenticationService`, `RBACService`, `OLSService`.

#### Prescription.cs

- Ý nghĩa:
  - Entity đơn thuốc trong hồ sơ bệnh án.
- Các thuộc tính chi tiết (ví dụ giả định hợp lý):
  - `MAHSBA`: mã hồ sơ bệnh án.
  - `NGAYDT`: ngày kê đơn/điều trị.
  - `TENTHUOC`: tên thuốc.
  - `LIEUDUNG`: liều dùng/hướng dẫn sử dụng.
- Vai trò trong hệ thống:
  - Đóng gói dữ liệu đơn thuốc khi bác sĩ lưu đơn và bệnh nhân xem đơn.
- Được sử dụng ở đâu (Form/Service nào):
  - Forms: `DoctorForm`, `PatientForm`.
  - Services: `DoctorService` (Update/Delete), `PatientService` (GetMyPrescriptions).

#### Notification.cs

- Ý nghĩa:
  - Entity thông báo nội bộ hiển thị trong popup thông báo.
- Các thuộc tính chi tiết (ví dụ giả định hợp lý):
  - `MATHONGBAO`: mã thông báo.
  - `NOIDUNG`: nội dung thông báo.
  - `NGAYGIO`: thời điểm phát hành.
  - `DIADIEM`: địa điểm hoặc phạm vi áp dụng.
- Vai trò trong hệ thống:
  - Là dữ liệu đầu ra cho màn hình `NotificationForm`.
- Được sử dụng ở đâu (Form/Service nào):
  - Forms: `NotificationForm`.
  - Services: `OLSService` (GetAccessibleNotificationsDetailed).

#### UserSession.cs

- Ý nghĩa:
  - Đối tượng phiên làm việc sau khi đăng nhập thành công, dùng để mang ngữ cảnh user xuyên suốt app.
- Các thuộc tính chi tiết (ví dụ giả định hợp lý):
  - `Username`: Oracle user hiện tại.
  - `FullName`: tên hiển thị.
  - `Role`: role ứng dụng (`COORDINATOR`, `DOCTOR`, `TECHNICIAN`, `PATIENT`).
  - `StaffId`/`PatientId`: id định danh theo loại tài khoản.
  - `DepartmentCode`: mã khoa/phòng ban (nếu có).
  - `ConnectionString`, `DataSource`: thông tin kết nối Oracle cho session.
- Vai trò trong hệ thống:
  - Là nguồn dữ liệu để chọn form theo role và tạo service theo đúng kết nối của user.
- Được sử dụng ở đâu (Form/Service nào):
  - Forms: tất cả form theo vai trò (`CoordinatorForm`, `DoctorForm`, `TechnicianForm`, `PatientForm`, `NotificationForm`).
  - Services: khởi tạo `OracleConnectionService` theo connection string session.

### Services (VIẾT SÂU)

- Nhóm kết nối và nền tảng:
  - `OracleConnectionService`
    - Mục đích: tạo connection Oracle, set schema `HOSPITAL_ADMIN`, cung cấp hàm `Execute<T>()` để chuẩn hóa pattern truy cập DB.
    - Method chính:
      - `BuildConnectionString(dataSource, userId, password)`.
      - `GetConnection()`.
      - `Execute<T>(Func<OracleConnection,T>)` và `Execute(Action<OracleConnection>)`.

- Nhóm authentication/authorization/validation:
  - `AuthenticationService`
    - `Authenticate(username, password, dataSource)`:
      - Tạo kết nối theo user Oracle.
      - Đọc `V_SELF_NHANVIEN` hoặc `V_SELF_BENHNHAN`.
      - Trả về `UserSession` với role tương ứng.
    - Có thêm `Login`, `ValidateUserRole`, `Logout` (phục vụ mở rộng).
  - `ValidationService`
    - `ValidateUsername`, `ValidatePassword`, `ValidatePatientId`, `ValidateMedicalRecord`.
    - Được `LoginForm` dùng để chặn input sai trước khi vào DB.
  - `RBACService`
    - Quản lý ma trận quyền qua `RoleActions`.
    - `CheckUserRole`, `CheckPermission`, `GetAvailableActions`.

- Nhóm CRUD theo vai trò nghiệp vụ:
  - `CoordinatorService`
    - CRUD bệnh nhân: `GetAllPatients`, `AddPatient`, `EditPatient`.
    - Điều phối hồ sơ: `CreateMedicalRecord`, `AssignDoctorToPatient`, `AssignTechnician`, `AssignTechnicianToService`.
    - Dữ liệu tham chiếu: `GetDoctors`, `GetTechnicians`, `GetRecordStatus`.
    - SQL đáng chú ý: `MERGE INTO HSBA_DV` khi gán kỹ thuật viên.
  - `DoctorService`
    - Lấy dữ liệu bác sĩ phụ trách: `GetAssignedPatients`, `GetAssignedMedicalRecords`.
    - Cập nhật bệnh án: `UpdateMedicalRecord`/`CreateDiagnosis`.
    - Quản lý thuốc: `UpdatePrescription` (MERGE), `DeletePrescription`.
    - Dịch vụ cận lâm sàng: `OrderDiagnosticService`, `DeleteDiagnosticService`.
  - `PatientService`
    - Hồ sơ cá nhân: `GetPatient`, `UpdatePatientInfo`.
    - Dữ liệu bệnh án/đơn thuốc của chính bệnh nhân: `GetMyMedicalRecords`, `GetMyPrescriptions`.
    - Truy vấn qua các view tự thân: `V_SELF_BENHNHAN`, `V_PATIENT_HSBA`, `V_PATIENT_DONTHUOC`.
  - `TechnicianService`
    - Dịch vụ được phân công: `GetAssignedServices`.
    - Cập nhật kết quả dịch vụ: `UpdateServiceResult(...)`.
    - Có overload nhận `serviceId` dạng chuỗi và parse thành khóa đầy đủ.

- Nhóm bảo mật dữ liệu và audit:
  - `VPDService`
    - Cung cấp danh sách dữ liệu nhìn thấy theo vai trò (`GetVisiblePatients`, `GetVisibleRecords`, `GetVisibleServices`) dựa trên service nghiệp vụ.
  - `OLSService`
    - Lấy nhãn user (`GetUserLabels`) và danh sách thông báo truy cập được (`GetAccessibleNotificationsDetailed`).
    - Dùng trực tiếp tại `NotificationForm`.
  - `AuditService`
    - `GetAuditLogs(startDate, endDate, specificUser)` đọc từ `UNIFIED_AUDIT_TRAIL` hoặc fallback `DBA_AUDIT_TRAIL`.
    - `LogUserAction`, `LogSensitiveAccess` hiện là stub (placeholder) để mở rộng ghi log nghiệp vụ ở tầng ứng dụng.

- Ví dụ method cụ thể (giả định bám code hiện tại):
  - `Login(username, password)` -> kiểm tra role user.
  - `GetPatients()` tương ứng `CoordinatorService.GetAllPatients()`.
  - `CreatePrescription()` tương ứng `DoctorService.UpdatePrescription(prescription)` (MERGE tạo/cập nhật).

- Luồng xử lý chuẩn (Form -> Service -> Model -> Form):
  - Ví dụ cập nhật hồ sơ bệnh nhân cá nhân:
    - `PatientForm` thu input textbox -> tạo `Patient`.
    - Gọi `PatientService.UpdatePatientInfo(patient)`.
    - Service chạy SQL update qua `OracleConnectionService.Execute`.
    - Form reload bằng `LoadData()` để lấy bản ghi mới và hiển thị lại.
  - Ví dụ bác sĩ lưu đơn thuốc:
    - `DoctorForm` tạo `Prescription` từ UI.
    - Gọi `DoctorService.UpdatePrescription(prescription)`.
    - Service MERGE vào `DONTHUOC`.
    - Form thông báo thành công và dữ liệu sẽ xuất hiện ở portal bệnh nhân.

## 5. Tổng quan kiến trúc

Luồng tương tác chính:

- Người dùng đăng nhập tại `LoginForm` -> hệ thống tạo `UserSession`.
- Form theo vai trò gọi service tương ứng để lấy/cập nhật dữ liệu.
- Service thao tác với Oracle qua `OracleConnectionService`, map kết quả về model.
- Form bind model lên control (`DataGridView`, `TextBox`) để hiển thị.

Nguyên tắc tách biệt (separation of concerns):

- Forms chỉ xử lý UI + event.
- Models chỉ chứa dữ liệu.
- Services xử lý nghiệp vụ + SQL + bảo mật truy cập.

Kết quả là code dễ theo dõi luồng, dễ mở rộng thêm chức năng và thuận lợi cho nhóm tiếp tục phát triển.
