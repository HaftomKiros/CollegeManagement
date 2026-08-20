@echo off
echo === student_profile columns ===
"D:\xampp\mysql\bin\mysql.exe" -u root -h 127.0.0.1 -P 3306 --protocol=TCP ecc_dof_wukrostmarycollege -e "SHOW COLUMNS FROM student_profile"

echo.
echo === BLOB sizes (photo + attachment) ===
"D:\xampp\mysql\bin\mysql.exe" -u root -h 127.0.0.1 -P 3306 --protocol=TCP ecc_dof_wukrostmarycollege -e "SELECT COUNT(*) as total, SUM(IF(photo IS NOT NULL AND LENGTH(photo)>0,1,0)) as with_photo, SUM(IF(attachment IS NOT NULL AND LENGTH(attachment)>0,1,0)) as with_attachment FROM student_profile"
