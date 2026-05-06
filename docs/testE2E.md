## **0\. Setup Checks Before Demo**

- [X] **Verify Listener:** Ensure the Oracle listener is up and the service is localhost:1521/XEPDB1.  
- [X] **Initialize Admin:** Run Create\_HOSPITAL\_ADMIN.sql as SYS.  
- [X] **Execute Setup Scripts:** Run the following in order as HOSPITAL\_ADMIN: Reset.sql, schema/01\_CreateTables.sql, schema/02\_CreateIndexes.sql, schema/03\_InsertSampleData.sql, security/01\_RBAC\_Setup.sql, security/02\_VPD\_Setup.sql, security/03\_OLS\_Setup.sql (run twice), audit/01\_StandardAudit\_Setup.sql, and audit/02\_FGA\_Setup.sql.  
- [x] **Verify Accounts:** Confirm the sample Oracle users and roles exist.  
- [x] **Check Datasource:** Confirm the medical app login defaults to localhost:1521/XEPDB1.  
- [x] **Validate Admin Usage:** Confirm HOSPITAL\_ADMIN is only used as the schema/setup account, never as a normal business login.

## **1\. Requirement 1 — Access Control and UI**

### 1.1 Subsystem 1: Oracle Admin App

- [ ] **Create a user:** Create a new Oracle user. **Expected:** The user appears in Oracle and in the app list.  
- [ ] **Edit a user:** Change an existing user’s editable properties. **Expected:** The change is saved and visible afterward.  
- [ ] **Delete a user:** Remove an Oracle user. **Expected:** The user no longer appears in the app or Oracle list.  
- [ ] **Create a role:** Create a new Oracle role. **Expected:** The role appears in the role list.  
- [ ] **Edit a role:** Modify role grants or role settings. **Expected:** Updated role information is shown correctly.  
- [ ] **Delete a role:** Remove an Oracle role. **Expected:** The role disappears from the list.  
- [ ] **List all users and roles:** Open the account/role listing screens. **Expected:** All current Oracle users and roles are displayed.  
- [ ] **Grant privilege to a user:** Grant an object privilege directly to a user. **Expected:** The privilege becomes visible in the user’s privilege details.  
- [ ] **Grant privilege to a role:** Grant an object privilege to a role. **Expected:** The role privilege list updates correctly.  
- [ ] **Grant a role to a user:** Assign a role to a user. **Expected:** The user inherits the role.  
- [ ] **Grant with WITH GRANT OPTION:** Repeat a grant with and without the grant option. **Expected:** The app shows whether the recipient can re-grant it.  
- [ ] **Revoke privilege / revoke role:** Revoke the access from the user/role. **Expected:** The access disappears immediately.  
- [ ] **View privilege details:** Inspect user/role database object permissions. **Expected:** The app clearly shows the granted object permissions.  
- [ ] **Object-type coverage:** Test permissions on tables, views, stored procedures, and functions. **Expected:** The app handles each object type correctly.  
- [ ] **Column-level permission test:** Grant SELECT or UPDATE on specific columns only. **Expected:** Access is successfully limited to the chosen columns.  
- [ ] **Insert/Delete permission test:** Try INSERT and DELETE privilege flows. **Expected:** Handled correctly at the table level, not as column-level grants.

### 1.2 Subsystem 2: Medical App UI and Login Flow

- [x] **Open the login form:** **Expected:** The UI loads cleanly and the datasource shows localhost:1521/XEPDB1.  
- [x] **Login as coordinator:** Use the coordinator sample account. **Expected:** The coordinator dashboard opens.  
- [x] **Login as doctor:** Use the doctor sample account. **Expected:** The doctor dashboard opens.  
- [x] **Login as technician:** Use the technician sample account. **Expected:** The technician dashboard opens.  
- [x] **Login as patient:** Use the patient sample account. **Expected:** The patient dashboard opens.  
- [x] **Wrong credentials:** Use an invalid username/password. **Expected:** Login fails with an error message.  
- [x] **Logout:** Log out from any role screen. **Expected:** The app returns to the login form.  
- [x] **Do not use HOSPITAL\_ADMIN:** Attempt a business flow. **Expected:** Normal business flows use the mapped Oracle users, not the schema owner account.

### 1.3 TC\#1 — User Mapping and Self-Access

- [x] **Log in:** Access a staff member or patient account.  
- [x] **Verify mapping:** Ensure the app reads the matching row in NHANVIEN or BENHNHAN.  
- [x] **Verify isolation:** Check that the user only sees their own record.  
- [x] **Verify restriction:** Attempt to browse another person’s row freely.  
- [x] **Overall Expected:** Oracle account identity is tied to the correct business row, and access is strictly isolated to that person.

### 1.4 TC\#2 — Coordinator Workflow (Điều phối viên)

- [x] **View patients:** **Expected:** All permitted patient rows are visible.  
- [x] **Add a patient:** **Expected:** A new BENHNHAN row is created.  
- [x] **Edit a patient:** **Expected:** Allowed patient fields can be changed successfully.  
- [x] **Create a medical record:** **Expected:** A new HSBA record is created for the patient.  
- [x] **Assign doctor to the record:** Update MABS / MAKHOA. **Expected:** The record reflects the assigned doctor and department.  
- [x] **Assign technician to a service:** Update MAKTV for HSBA\_DV. **Expected:** The service is officially assigned to the technician.

### 1.5 TC\#3 — Doctor Workflow (Bác sĩ/Y sĩ)

- [x] **View assigned medical records:** **Expected:** Only records specifically for that doctor appear.  
- [x] **View assigned patients:** **Expected:** Only patients linked to the doctor’s records appear.  
- [x] **Update details:** Change CHẨNĐOÁN, ĐIỀUTRỊ, KẾTLUẬN on owned records. **Expected:** Changes are saved and visible.  
- [x] **Update patient history:** Change TIỀNSỬBỆNH, TIỀNSỬBỆNHGD, DỊỨNGTHUỐC. **Expected:** The patient row updates only if the doctor has permission.  
- [x] **Add prescription:** **Expected:** A new ĐƠNTHUỐC row is created.  
- [x] **Delete prescription:** **Expected:** The ĐƠNTHUỐC row is completely removed.  
- [x] **Update prescription:** **Expected:** Allowed prescription fields update correctly.  
- [x] **Add diagnostic service:** **Expected:** A new HSBA\_DV service row is created.  
- [x] **Delete diagnostic service:** **Expected:** The HSBA\_DV service row is removed.

### 1.6 TC\#4 — Technician Workflow (Kỹ thuật viên)

- [] **View assigned services:** **Expected:** Only services assigned to that specific technician are visible.  
- [] **Update service result:** Modify KẾTQUẢ. **Expected:** The result saves successfully for the assigned rows.  
- [] **Try to update an unassigned service:** **Expected:** Access is blocked.  
- [] **Try to edit other columns:** **Expected:** Access is blocked by the UI or rejected directly by Oracle.

### 1.7 TC\#5 — Patient Workflow (Bệnh nhân)

- [] **View own personal information:** **Expected:** Only the patient’s personal row is visible.  
- [] **Update allowed profile fields:** Edit address/history/allergy fields only. **Expected:** Changes save successfully.  
- [] **Try to change restricted fields:** Attempt to modify Mã, Họ tên, Phái, Ngày sinh, or CCCD. **Expected:** Oracle rejects the change.  
- [] **View own medical records:** **Expected:** Only the patient’s own HSBA records appear.  
- [] **View own prescriptions:** **Expected:** Only the patient’s own ĐƠNTHUỐC rows appear.

## **2\. Requirement 2 — OLS Manual Tests**

Verify notification visibility using the seeded users and assuming standard Oracle label dominance rules.

| User | Seed Account | Label | Expected Visible Notifications |
| :---- | :---- | :---- | :---- |
| u1 | 990000000001 | L3\_GD:C\_TIEU,C\_THAN,C\_TIM:G\_HN,G\_HP,G\_HCM | t1, t2, t3, t4, t5, t6, t7 |
| u2 | 990000000090 | L2\_LD:C\_TIM:G\_HCM | t1, t3 |
| u3 | 990000000060 | L2\_LD:C\_THAN:G\_HN | t1, t3 |
| u4 | 990000000061 | L1\_NV:C\_THAN:G\_HCM | t1 |
| u5 | 990000000091 | L1\_NV:C\_TIM:G\_HCM | t1 |
| u6 | 990000000002 | L2\_LD:C\_TIM:G\_HCM | t1, t3 |
| u7 | 990000000003 | L2\_LD:C\_TIEU,C\_THAN,C\_TIM:G\_HN,G\_HP,G\_HCM | t1, t3, t4, t5, t6, t7 |
| u8 | 990000000030 | L1\_NV:C\_TIEU:G\_HN | t1, t6 |

### OLS Execution Steps

- [] **Verify Access Levels:** Log in as each user sequentially, open the notifications screen, and compare the visible rows against the table above.  
- [] **Verify Upward Restriction:** Confirm users cannot read rows above their label clearance.  
- [] **Verify Top-Level Access:** Confirm the top-level user can read all entries.  
- [] **Data-Insertion Test:** Create a labeled notification and check visibility for each user. **Expected:** Label rules filter the notification exactly as defined in the matrix.

## 

## 

## 

## 

## 

## 

## 

## 

## 

## 

## 

## 

## 

## **3\. Requirement 3 — Audit Manual Tests**

*Note: Execute 03\_ReadAuditLogs.sql after each scenario to verify results.*

### Standard Audit Tests

- [ ] **Successful login/logout:** Log in and log out as a normal user. **Expected:** Session audit row appears with a success code.  
- [ ] **Failed SELECT on BENHNHAN:** Use an unauthorized user. **Expected:** Failed audit entry is recorded.  
- [ ] **Failed DML on HSBA\_DV:** Try an unauthorized insert/update/delete. **Expected:** Failed audit entry is recorded.  
- [ ] **DML on HSBA:** Perform a valid insert/update/delete. **Expected:** Standard audit row is created.  
- [ ] **DML on DONTHUOC:** Perform a valid prescription insert/update/delete. **Expected:** Standard audit row is created.  
- [ ] **VPD function execution:** Run a query that triggers VPD\_HSBA\_FN. **Expected:** Audit entry for the function execution appears.

### Fine-Grained Auditing (FGA) Tests

- [ ] **Update DONTHUOC after creation:** Modify an existing prescription. **Expected:** FGA\_DONTHUOC\_AFTER\_CREATE fires.  
- [ ] **Valid doctor update on HSBA:** Doctor updates their own record fields. **Expected:** FGA\_HSBA\_VALID\_UPDATE fires.  
- [ ] **Invalid doctor update on HSBA:** User attempts to update an unowned record. **Expected:** FGA\_HSBA\_INVALID\_UPDATE fires.  
- [ ] **Illegal HSBA\_DV DML:** Unauthorized insert/update/delete on HSBA\_DV. **Expected:** FGA\_HSBA\_DV\_ILLEGAL\_DML fires.

### Audit Readback Test

- [ ] **Verify Logs:** After completing the scenarios, run 03\_ReadAuditLogs.sql and confirm relevant rows appear in DBA\_AUDIT\_SESSION, DBA\_AUDIT\_TRAIL, and DBA\_FGA\_AUDIT\_TRAIL.

      ## **4\. Requirement 4 — Backup and Recovery Manual Tests**

- [ ] **Create backup directory object:** Run Create\_Medical\_Backup\_Directory.sql. **Expected:** MEDICAL\_BACKUP\_DIR is created and writable.  
- [ ] **Manual physical full backup:** Run 00\_Manual\_Physical\_Full\_Backup.rman. **Expected:** A full backup piece is created successfully.  
- [ ] **Manual physical incremental backup:** Run 01\_Manual\_Physical\_Incremental\_Backup.rman. **Expected:** An incremental backup piece is created successfully.  
- [ ] **Automatic physical incremental backup job:** Run 02\_Auto\_Physical\_Incremental\_Backup.sql. **Expected:** Scheduler job exists and is enabled.  
- [ ] **Manual logical backup:** Run 03\_Manual\_Logical\_Backup.sql. **Expected:** A .dmp export file is created successfully.  
- [ ] **Automatic logical backup job:** Run 04\_Auto\_Logical\_Backup.sql. **Expected:** Scheduler job exists and is enabled.  
- [ ] **Validate logical dump file:** Check the .dmp file. **Expected:** Dump metadata is readable and the file size is non-zero.  
- [ ] **Prepare logical recovery test:** Run 05\_Prepare\_Logical\_Recovery\_Test.sql. **Expected:** Snapshot table is created.  
- [ ] **Verify logical recovery:** Import the dump and run 06\_Verify\_Logical\_Recovery\_Test.sql. **Expected:** Object counts match the pre-restore snapshot or are explainably close.  
- [ ] **Read recovery audit anchor:** Run 07\_Recovery\_Audit\_Timestamp\_Anchor.sql. **Expected:** A suggested restore timestamp is printed.  
- [ ] **Manual physical PITR:** Run 08\_Manual\_Physical\_Recovery\_PITR.rman in a test environment. **Expected:** The database recovers to the target point in time.  
- [ ] **Compare methods:** Review 09\_Method\_Comparison\_And\_Conclusion.md. **Expected:** Clear pros/cons of RMAN, Data Pump, and flashback recovery are documented.  
- [ ] **Optional Flashback Test:** Enable Flashback Database and execute a test. **Expected:** Database restores to a specific point prior to an incident.

## **5\. Express Demo Sequence**

If you are running a timed video demo or live presentation, follow this strict sequence:

- [ ] Confirm all setup scripts run successfully.  
- [ ] Demonstrate the login flow for Coordinator, Doctor, Technician, and Patient.  
- [ ] Have the Coordinator create and assign new records.  
- [ ] Have the Doctor update diagnosis, treatment, and issue prescriptions.  
- [ ] Have the Technician update service results.  
- [ ] Have the Patient successfully edit allowed personal fields (and fail on restricted ones).  
- [ ] Show OLS notification visibility switching dynamically between two different users.  
- [ ] Query and display the Audit logs populated by the previous actions.  
- [ ] Walk through a single Backup/Recovery process.
