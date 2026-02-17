# Task 08: Subsystem 2 Database Security Setup (RBAC, VPD, OLS)

**Assigned to:** Ngọc, Vũ (Part B)
**Duration:** 10 hours
**Priority:** Critical
**Timeline:** Feb 17 - Feb 21, 2026

---

## 1. Objective

Configure the Oracle Database security layer for the Medical Data Management System (Subsystem 2) in adherence to the project specifications. This task involves establishing authentication, Role-Based Access Control (RBAC), Virtual Private Database (VPD) policies, and Oracle Label Security (OLS) to enforce data privacy and hierarchical access rules.

## 2. Scope of Work

### 2.1. User Account Provisioning (TC#1)

* **Strategy:** Create Oracle Database accounts corresponding to all records in the `NHANVIEN` and `BENHNHAN` tables.

* **Linking Mechanism:** Map Oracle accounts directly to data rows using the `USERNAME` column, strictly avoiding the creation of external user management tables.

* **Scale:**
  * **Staff:** Provision 170 accounts (20 Coordinators, 100 Doctors, 50 Technicians)
  * **Patients:** Implement a deferred creation strategy for ~100,000 patient accounts to optimize resource usage

### 2.2. Role-Based Access Control (RBAC) Implementation

* **Technician Role (TC#4):**
  * **Target Users:** 50 Staff members (Position: "Kỹ thuật viên")
  * **Permissions:** Grant `UPDATE` privileges solely on the `KETQUA` column of the `HSBA_DV` table
  * **Access Control:** Restrict visibility to assigned services only (`HSBA_DV` rows where `MAKTV` matches the user)
  * **Mechanism:** Implement via RBAC as specified in Requirement 1, Question 2

* **Patient Role (TC#5):**
  * **Target Users:** ~100,000 Users (Position: "Bệnh nhân")
  * **Permissions:** Grant view access to own records in `BENHNHAN`
  * **Update Restrictions:** Allow updates only to contact fields (`SONHA`, `TENDUONG`, `QUANHUYEN`, `TINHTP`). Strictly deny updates to identity fields (`MABN`, `TENBN`, `NGAYSINH`, `CCCD`)
  * **Mechanism:** Implement via RBAC as specified in Requirement 1, Question 2

### 2.3. Virtual Private Database (VPD) Implementation

* **Coordinator Role (TC#2):**
  * **Target Users:** 20 Staff members (Position: "Điều phối viên")
  * **Permissions:** Full access to view, add, and edit `BENHNHAN` records. Authorized to create `HSBA` and assign Doctors (`MABS`) or Technicians (`MAKTV`)
  * **Mechanism:** Implement via VPD as specified in Requirement 1, Question 3

* **Doctor Role (TC#3):**
  * **Target Users:** 100 Staff members (Position: "Bác sĩ/Y sĩ")
  * **Policy Logic:** Enforce row-level security to ensure doctors only view `HSBA` records where they are the assigned physician (`MABS`)
  * **Extended Permissions:**
    * View patient list associated with treated HSBAs
    * Update medical history fields (`TIENSUBENH`, `TIENSUBENHGD`, `DIUNGTHUOC`) for treated patients
    * Add/Delete diagnostic services in `HSBA_DV`
    * Manage prescriptions in `DONTHUOC`
    * Update `CHANDOAN`, `DIEUTRI`, `KETLUAN` (Audit required)

### 2.4. Oracle Label Security (OLS) Setup (Requirement 2)

* **Target Object:** `THONGBAO` table (Fields: `NOIDUNG`, `NGAYGIO`, `DIADIEM`)

* **Security Model:** Implement a 3-component label hierarchy:
  1. **Levels (Rank):** Ban Giám đốc (Director) > Lãnh đạo khoa (Head) > Nhân viên (Staff)
  2. **Compartments (Department):** Tim mạch, Tiêu hóa, Thần kinh
  3. **Groups (Location):** Hồ Chí Minh, Hải Phòng, Hà Nội

* **Label Policy:** Ensure data visibility strictly follows the label dominance rules (e.g., a "Cardiology Staff" in "HCM" cannot view notifications designated for "Directors" or "Neurology" departments)

## 3. Deliverables & Execution Order


1. **`01_RBAC_Setup.sql`** — Script to provision Oracle users linked to `NHANVIEN`/`BENHNHAN` and create Roles and Grants for Technicians and Patients
2. **`02_VPD_Setup.sql`** — Script to apply VPD policies for Coordinators and Doctors
3. **`03_OLS_Setup.sql`** — Script to configure OLS policies, levels, and labels for the `THONGBAO` table

## 4. Acceptance Criteria

* [ ] **User Linkage:** All 170 staff users and sample patient users can authenticate; `SYS_CONTEXT` correctly identifies their `MANV`/`MABN`
* [ ] **RBAC Verification:** Technicians cannot delete from `HSBA_DV`; Patients cannot modify their Date of Birth or Name
* [ ] **VPD Verification:** Doctors querying `HSBA` see *only* their assigned patients; Coordinators maintain oversight access
* [ ] **OLS Verification:** A user with the label `Tim mạch:Hồ Chí Minh:Nhân viên` cannot view notifications labeled for `Ban Giám đốc` or `Khoa thần kinh`

### 2.2. Role-Based Access Control (RBAC) Implementation

* **Technician Role (TC#4):**
  * **Target Users:** 50 Staff members (Position: "Kỹ thuật viên")
  * **Permissions:** Grant `UPDATE` privileges solely on the `KETQUA` column of the `HSBA_DV` table
  * **Access Control:** Restrict visibility to assigned services only (`HSBA_DV` rows where `MAKTV` matches the user)
  * **Mechanism:** Implement via RBAC as specified in Requirement 1, Question 2

* **Patient Role (TC#5):**
  * **Target Users:** ~100,000 Users (Position: "Bệnh nhân")
  * **Permissions:** Grant view access to own records in `BENHNHAN`
  * **Update Restrictions:** Allow updates only to contact fields (`SONHA`, `TENDUONG`, `QUANHUYEN`, `TINHTP`). Strictly deny updates to identity fields (`MABN`, `TENBN`, `NGAYSINH`, `CCCD`)
  * **Mechanism:** Implement via RBAC as specified in Requirement 1, Question 2

### 2.3. Virtual Private Database (VPD) Implementation

* **Coordinator Role (TC#2):**
  * **Target Users:** 20 Staff members (Position: "Điều phối viên")
  * **Permissions:** Full access to view, add, and edit `BENHNHAN` records. Authorized to create `HSBA` and assign Doctors (`MABS`) or Technicians (`MAKTV`)
  * **Mechanism:** Implement via VPD as specified in Requirement 1, Question 3

* **Doctor Role (TC#3):**
  * **Target Users:** 100 Staff members (Position: "Bác sĩ/Y sĩ")
  * **Policy Logic:** Enforce row-level security to ensure doctors only view `HSBA` records where they are the assigned physician (`MABS`)
  * **Extended Permissions:**
    * View patient list associated with treated HSBAs
    * Update medical history fields (`TIENSUBENH`, `TIENSUBENHGD`, `DIUNGTHUOC`) for treated patients
    * Add/Delete diagnostic services in `HSBA_DV`
    * Manage prescriptions in `DONTHUOC`
    * Update `CHANDOAN`, `DIEUTRI`, `KETLUAN` (Audit required)

### 2.4. Oracle Label Security (OLS) Setup (Requirement 2)

* **Target Object:** `THONGBAO` table (Fields: `NOIDUNG`, `NGAYGIO`, `DIADIEM`)

* **Security Model:** Implement a 3-component label hierarchy:
  1. **Levels (Rank):** Ban Giám đốc (Director) > Lãnh đạo khoa (Head) > Nhân viên (Staff)
  2. **Compartments (Department):** Tim mạch, Tiêu hóa, Thần kinh
  3. **Groups (Location):** Hồ Chí Minh, Hải Phòng, Hà Nội

* **Label Policy:** Ensure data visibility strictly follows the label dominance rules (e.g., a "Cardiology Staff" in "HCM" cannot view notifications designated for "Directors" or "Neurology" departments)

## 3. Deliverables & Execution Order

1. **`01_UsersCreation.sql`** — Script to provision Oracle users linked to `NHANVIEN`/`BENHNHAN`
2. **`02_RBAC_Setup.sql`** — Script to create Roles and Grants for Technicians and Patients
3. **`03_VPD_Setup.sql`** — Script to apply VPD policies for Coordinators and Doctors
4. **`04_OLS_Setup.sql`** — Script to configure OLS policies, levels, and labels for the `THONGBAO` table

## 4. Acceptance Criteria

* [ ] **User Linkage:** All 170 staff users and sample patient users can authenticate; `SYS_CONTEXT` correctly identifies their `MANV`/`MABN`
* [ ] **RBAC Verification:** Technicians cannot delete from `HSBA_DV`; Patients cannot modify their Date of Birth or Name
* [ ] **VPD Verification:** Doctors querying `HSBA` see *only* their assigned patients; Coordinators maintain oversight access
* [ ] **OLS Verification:** A user with the label `Tim mạch:Hồ Chí Minh:Nhân viên` cannot view notifications labeled for `Ban Giám đốc` or `Khoa thần kinh`
