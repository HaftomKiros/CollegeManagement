@echo off
set M=C:\wamp\bin\mysql\mysql5.6.17\bin
set CONN=-u root -h 127.0.0.1 -P 3306 --protocol=TCP
set OUT=C:\Users\IN-TECH\Desktop\welday\ecc_dof_wamp_export.sql

echo Exporting database with BLOBs from WAMP...
%M%\mysqldump.exe %CONN% --single-transaction --routines --triggers --add-drop-table ecc_dof_wukrostmarycollege > "%OUT%"
echo Export exit: %ERRORLEVEL%
echo File: %OUT%
pause
