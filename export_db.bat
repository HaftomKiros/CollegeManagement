@echo off
set DUMP=D:\xampp\mysql\bin\mysqldump.exe
set CONN=-u root -h 127.0.0.1 -P 3306 --protocol=TCP
set OUT=C:\Users\IN-TECH\Desktop\welday\ecc_dof_clean_export.sql

echo Exporting database...
%DUMP% %CONN% --single-transaction --routines --triggers --add-drop-table ecc_dof_wukrostmarycollege > "%OUT%"
echo Export exit: %ERRORLEVEL%
echo File saved to: %OUT%
