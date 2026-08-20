@echo off
set M=C:\wamp\bin\mysql\mysql5.6.17\bin\mysql.exe
set CONN=-u root -h 127.0.0.1 -P 3306 --protocol=TCP

echo === student_profile columns ===
%M% %CONN% ecc_dof_wukrostmarycollege -e "SHOW COLUMNS FROM student_profile"

echo.
echo === Check BLOB columns exist? ===
%M% %CONN% ecc_dof_wukrostmarycollege -e "SELECT COUNT(*) as has_photo_col FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='ecc_dof_wukrostmarycollege' AND TABLE_NAME='student_profile' AND COLUMN_NAME IN ('photo','attachment')"

echo.
echo === Count students with photo BLOB data ===
%M% %CONN% ecc_dof_wukrostmarycollege -e "SELECT COUNT(*) as total, SUM(IF(photo IS NOT NULL AND LENGTH(photo)>100,1,0)) as with_photo_blob FROM student_profile" 2>nul || echo (no BLOB columns)
