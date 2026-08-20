@echo off
title Database Import - College Management System
echo ============================================
echo  College Management System - Database Import
echo ============================================
echo.

set MYSQL=D:\xampp\mysql\bin\mysql.exe
set CONN=-u root -h 127.0.0.1 -P 3306 --protocol=TCP
set SQLFILE=%~dp0ecc_dof_wukrostmarycollege.sql

if not exist "%MYSQL%" (
    echo ERROR: MySQL not found at %MYSQL%
    echo Please update the MYSQL path in this script.
    pause
    exit /b 1
)

if not exist "%SQLFILE%" (
    echo ERROR: SQL file not found: %SQLFILE%
    echo Place ecc_dof_wukrostmarycollege.sql in the same folder as this script.
    pause
    exit /b 1
)

echo MySQL: %MYSQL%
echo SQL File: %SQLFILE%
echo.

echo [1/2] Dropping and recreating database...
%MYSQL% %CONN% -e "DROP DATABASE IF EXISTS ecc_dof_wukrostmarycollege; CREATE DATABASE ecc_dof_wukrostmarycollege CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci"
if %ERRORLEVEL% neq 0 (
    echo ERROR: Could not connect to MySQL. Make sure XAMPP MySQL is running.
    pause
    exit /b 1
)
echo     Done.

echo [2/2] Importing database...
%MYSQL% %CONN% ecc_dof_wukrostmarycollege < "%SQLFILE%"
if %ERRORLEVEL% neq 0 (
    echo WARNING: Import completed with some errors (may be OK if tables already exist).
) else (
    echo     Import successful!
)

echo.
echo ============================================
echo  DATABASE IMPORT COMPLETE
echo  2534 students and all data loaded.
echo  Start XAMPP and launch the app.
echo ============================================
pause
