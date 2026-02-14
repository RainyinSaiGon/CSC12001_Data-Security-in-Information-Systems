/* ==========================================================================
    CREATE ACCOUNT FOR PATIENT/STAFF
   ========================================================================== */

ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

DECLARE
    v_user_name VARCHAR2(50);
    v_sql       VARCHAR2(200);
    v_count     NUMBER := 0;
    v_staff_count NUMBER := 0;
BEGIN
    DBMS_OUTPUT.PUT_LINE('>>> STARTING ACCOUNT CREATION...');

    -- 1. CREATE EMPLOYEES (NHANVIEN)
    FOR nv IN (SELECT USERNAME FROM NHANVIEN WHERE USERNAME IS NOT NULL) 
    LOOP
        v_user_name := nv.USERNAME;
        
        BEGIN
            -- Create User with password "123"
            v_sql := 'CREATE USER ' || v_user_name || ' IDENTIFIED BY "123"';
            EXECUTE IMMEDIATE v_sql;
            
            -- Grant Login Permission
            v_sql := 'GRANT CREATE SESSION TO ' || v_user_name;
            EXECUTE IMMEDIATE v_sql;
            
            v_staff_count := v_staff_count + 1;
            
        EXCEPTION
            WHEN OTHERS THEN
                -- If user already exists (ORA-01920), skip it
                IF SQLCODE != -01920 THEN 
                    DBMS_OUTPUT.PUT_LINE('Error creating ' || v_user_name || ': ' || SQLERRM);
                END IF;
        END;
    END LOOP;
    
    DBMS_OUTPUT.PUT_LINE('>>> FINISHED EMPLOYEES. Total: ' || v_staff_count);

    -- 2. CREATE PATIENTS (BENHNHAN)
    -- WARNING: This loop handles 100,000 users. It may take time.
    FOR bn IN (SELECT USERNAME FROM BENHNHAN WHERE USERNAME IS NOT NULL) 
    LOOP
        v_user_name := bn.USERNAME;
        
        BEGIN
            v_sql := 'CREATE USER ' || v_user_name || ' IDENTIFIED BY "123"';
            EXECUTE IMMEDIATE v_sql;
            
            v_sql := 'GRANT CREATE SESSION TO ' || v_user_name;
            EXECUTE IMMEDIATE v_sql;
            
            v_count := v_count + 1;
            
            -- Print progress every 5000 users so you know it's not frozen
            IF MOD(v_count, 5000) = 0 THEN
                DBMS_OUTPUT.PUT_LINE('Created ' || v_count || ' patient accounts...');
            END IF;
            
        EXCEPTION
            WHEN OTHERS THEN
                IF SQLCODE != -01920 THEN 
                    DBMS_OUTPUT.PUT_LINE('Error creating ' || v_user_name || ': ' || SQLERRM);
                END IF;
        END;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('>>> ALL ACCOUNTS CREATED SUCCESSFULLY!');
    
    -- =====================================================
    -- FINAL SUMMARY REPORT
    -- =====================================================
    DBMS_OUTPUT.PUT_LINE(' ');
    DBMS_OUTPUT.PUT_LINE('================== EXECUTION SUMMARY ==================');
    DBMS_OUTPUT.PUT_LINE('Staff Accounts Created    : ' || v_staff_count);
    DBMS_OUTPUT.PUT_LINE('Patient Accounts Created  : ' || v_count);
    DBMS_OUTPUT.PUT_LINE('Total Accounts Created    : ' || (v_staff_count + v_count));
    DBMS_OUTPUT.PUT_LINE('======================================================');
END;
/

-- =====================================================
-- Commit changes to persist user creation
-- =====================================================
COMMIT;

PROMPT ========================================
PROMPT User Account Creation Complete!
PROMPT ========================================
PROMPT Next step: Run 02_RBAC_Setup.sql to assign roles
PROMPT ========================================
