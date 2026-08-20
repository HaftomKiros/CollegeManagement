@echo off
title College Management System - Build Installer
echo ============================================
echo  College Management System - Build Installer
echo ============================================
echo.

set PROJECT=CollegeManagementWPF\CollegeManagementWPF.csproj
set PUBLISH_DIR=publish\CollegeManagement
set ISS=publish\setup.iss
set ISCC="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
set OUTPUT=C:\Temp\CollegeSetup

echo [1/3] Publishing app (self-contained, win-x64)...
dotnet publish "%PROJECT%" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=false -o "%PUBLISH_DIR%"
if %ERRORLEVEL% neq 0 (
    echo ERROR: Publish failed!
    pause
    exit /b 1
)
echo     Done.
echo.

echo [2/3] Building installer...
mkdir "%OUTPUT%" 2>nul
del "%OUTPUT%\CollegeManagement_Setup_v1.0.0.exe" 2>nul
%ISCC% "%ISS%"
if %ERRORLEVEL% neq 0 (
    echo ERROR: Inno Setup failed!
    pause
    exit /b 1
)
echo     Done.
echo.

echo [3/3] Copying database import script...
copy /Y "import_db.bat" "%OUTPUT%\import_db.bat" >nul
copy /Y "C:\Users\IN-TECH\Desktop\welday\ecc_dof_clean_export.sql" "%OUTPUT%\ecc_dof_wukrostmarycollege.sql" >nul 2>nul
echo     Done.
echo.

echo ============================================
echo  BUILD COMPLETE!
echo  Installer: %OUTPUT%\CollegeManagement_Setup_v1.0.0.exe
echo  DB Script: %OUTPUT%\import_db.bat
echo ============================================
echo.
explorer "%OUTPUT%"
pause
