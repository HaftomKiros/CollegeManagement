@echo off
echo === student_profile columns ===
"D:\xampp\mysql\bin\mysql.exe" -u root -h 127.0.0.1 -P 3306 --protocol=TCP ecc_dof_wukrostmarycollege -e "SHOW COLUMNS FROM student_profile"

echo.
echo === Count with photo_path set ===
"D:\xampp\mysql\bin\mysql.exe" -u root -h 127.0.0.1 -P 3306 --protocol=TCP ecc_dof_wukrostmarycollege -e "SELECT COUNT(*) as with_photo FROM student_profile WHERE photo_path IS NOT NULL AND photo_path <> ''"

echo.
echo === Sample photo_path values ===
"D:\xampp\mysql\bin\mysql.exe" -u root -h 127.0.0.1 -P 3306 --protocol=TCP ecc_dof_wukrostmarycollege -e "SELECT student_id, photo_path FROM student_profile WHERE photo_path IS NOT NULL LIMIT 5"
pause
