-- Run as SYS in XEPDB1 after reproducing an incident scenario.
-- Use suggested_restore_time in RMAN SET UNTIL TIME.

SELECT *
FROM (
    SELECT USERNAME,
           ACTION_NAME,
           OBJ_NAME,
           RETURNCODE,
           TO_CHAR(EXTENDED_TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') action_time
    FROM DBA_AUDIT_TRAIL
    WHERE OBJ_NAME IN ('BENHNHAN', 'HSBA', 'HSBA_DV', 'DONTHUOC')
    ORDER BY EXTENDED_TIMESTAMP DESC
)
WHERE ROWNUM <= 20;

SELECT *
FROM (
    SELECT DB_USER,
           OBJECT_SCHEMA,
           OBJECT_NAME,
           POLICY_NAME,
           STATEMENT_TYPE,
           TO_CHAR(TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') action_time,
           SQL_TEXT
    FROM DBA_FGA_AUDIT_TRAIL
    WHERE OBJECT_NAME IN ('BENHNHAN', 'HSBA', 'HSBA_DV', 'DONTHUOC')
    ORDER BY TIMESTAMP DESC
)
WHERE ROWNUM <= 20;

WITH candidate_events AS (
    SELECT EXTENDED_TIMESTAMP evt_time
    FROM DBA_AUDIT_TRAIL
    WHERE OBJ_NAME IN ('BENHNHAN', 'HSBA', 'HSBA_DV', 'DONTHUOC')
    UNION ALL
    SELECT CAST(TIMESTAMP AS TIMESTAMP) evt_time
    FROM DBA_FGA_AUDIT_TRAIL
    WHERE OBJECT_NAME IN ('BENHNHAN', 'HSBA', 'HSBA_DV', 'DONTHUOC')
), latest_event AS (
    SELECT MAX(evt_time) max_evt_time
    FROM candidate_events
)
SELECT TO_CHAR(max_evt_time, 'YYYY-MM-DD HH24:MI:SS') latest_incident_time,
       TO_CHAR(max_evt_time - NUMTODSINTERVAL(30, 'SECOND'), 'YYYY-MM-DD HH24:MI:SS') suggested_restore_time
FROM latest_event;
