@echo off
"D:\xampp\mysql\bin\mysql.exe" -u root -h 127.0.0.1 -P 3306 --protocol=TCP ecc_dof_wukrostmarycollege -e "SELECT student_id, photo_path FROM student_profile WHERE photo_path IS NOT NULL AND photo_path <> '' LIMIT 10"
