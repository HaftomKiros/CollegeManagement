@echo off
set M=C:\wamp\bin\mysql\mysql5.6.17\bin\mysql.exe
echo === Check BLOB columns and data ===
%M% -u root -h 127.0.0.1 -P 3306 --protocol=TCP ecc_dof_wukrostmarycollege -e "SELECT COUNT(*) as total_students, SUM(IF(LENGTH(photo)>100,1,0)) as has_photo, SUM(IF(LENGTH(attachment)>100,1,0)) as has_attachment FROM student_profile"
