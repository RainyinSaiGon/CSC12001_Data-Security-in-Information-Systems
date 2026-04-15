SET SERVEROUTPUT ON;

PROMPT === Requirement 4 / Automatic physical incremental backup ===
PROMPT Prerequisite:
PROMPT 1. Create an OS credential for DBMS_SCHEDULER if your Oracle installation requires it.
PROMPT 2. Adjust RMAN_EXE and backup paths below if needed.

BEGIN
    BEGIN
        DBMS_SCHEDULER.DROP_JOB(
            job_name => 'MED_AUTO_PHY_INC_BACKUP_JOB',
            force    => TRUE
        );
    EXCEPTION
        WHEN OTHERS THEN
            NULL;
    END;

    DBMS_SCHEDULER.CREATE_JOB(
        job_name        => 'MED_AUTO_PHY_INC_BACKUP_JOB',
        job_type        => 'EXECUTABLE',
        job_action      => 'C:\Windows\System32\cmd.exe',
        number_of_arguments => 2,
        start_date      => SYSTIMESTAMP,
        repeat_interval => 'FREQ=DAILY;BYHOUR=1;BYMINUTE=0;BYSECOND=0',
        enabled         => FALSE,
        auto_drop       => FALSE,
        comments        => 'Daily RMAN incremental physical backup'
    );

    DBMS_SCHEDULER.SET_JOB_ARGUMENT_VALUE(
        job_name => 'MED_AUTO_PHY_INC_BACKUP_JOB',
        argument_position => 1,
        argument_value => '/c'
    );

    DBMS_SCHEDULER.SET_JOB_ARGUMENT_VALUE(
        job_name => 'MED_AUTO_PHY_INC_BACKUP_JOB',
        argument_position => 2,
        argument_value => '"C:\app\LENOVO\product\21c\dbhomeXE\bin\rman.exe" target / cmdfile=D:\atbmhttt\lab\CSC12001_Data-Security-in-Information-Systems\Database\Subsystem2-MedicalDB\recovery\01_Manual_Physical_Incremental_Backup.rman log=D:\oracle-backup\physical\logs\MED_AUTO_PHY_INC_BACKUP.log'
    );

    -- Uncomment and adjust this line after creating the credential:
    -- DBMS_SCHEDULER.SET_ATTRIBUTE('MED_AUTO_PHY_INC_BACKUP_JOB', 'credential_name', 'MED_RMAN_OS_CRED');

    DBMS_OUTPUT.PUT_LINE('Created job MED_AUTO_PHY_INC_BACKUP_JOB. Enable it after configuring credential/path settings.');
END;
/

BEGIN
    DBMS_SCHEDULER.ENABLE('MED_AUTO_PHY_INC_BACKUP_JOB');
END;
/

select job_name, enabled, state, repeat_interval
from dba_scheduler_jobs
where job_name = 'MED_AUTO_PHY_INC_BACKUP_JOB';

begin
DBMS_SCHEDULER.RUN_JOB('MED_AUTO_PHY_INC_BACKUP_JOB', use_current_session => FALSE);
end;
/

select status, actual_start_date, run_duration, additional_info
from dba_scheduler_job_run_details
where job_name = 'MED_AUTO_PHY_INC_BACKUP_JOB'
order by log_date desc;