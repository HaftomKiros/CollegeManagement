﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace CollegeManagement
{
    public partial class StudRegistration : Form
    {
        string cellValue = null, cellValue2 = null;
        int rowIndex;
        static int IPa = 127, IPb = 0, IPc = 0, IPd = 1;
        static string connString = @"server=" + IPa + "." + IPb + "." + IPc + "." + IPd + ";port=3306;database=ecc_dof_wukrostmarycollege;uid=root;pwd=";
        MySqlConnection conn = new MySqlConnection(connString);
        MySqlCommand sqlCmd;
        MySqlDataReader dataReader;
        MySqlDataAdapter dataAdapter;
        public StudRegistration()
        {
            InitializeComponent();
            //---Initialize dataGridStudents---
            string tableQuery = "Select student_id, dept_id, stream_id, level, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1 From ecc_dof_wukrostmarycollege.student_profile";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridStudents.RowTemplate.Height = 20;
            dataGridStudents.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridStudents.DataSource = table;
            ApplyGridStyle();
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            int checker = 0;
            string checkQuery = "Select student_id, level From ecc_dof_wukrostmarycollege.student_profile where student_id='" + this.studID.Text + "' and level='"+this.level.Text+"'";
            sqlCmd = new MySqlCommand(checkQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    ++checker;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed!");
            }

            if (checker == 0)
            {
                if (fName.Text != "" && mName.Text != "" && lName.Text != "" && sex.Text != "" && gpa10.Text != "" && gpa12.Text != "" && admissionYear.Text != "" && programType.Text != "" && admissionType.Text != "" && wereda.Text != "" && kebele.Text != "" && phoneNumber.Text != "" && studID.Text != "" && level.Text != "" && deptID.Text != "" && streamID.Text != "" && photoPath.Text != "" && attachmentPath.Text != "")
                {
                    byte[] imageByte = null;
                    FileStream fstream = new FileStream(this.photoPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr = new BinaryReader(fstream);
                    imageByte = binRdr.ReadBytes((int)fstream.Length);

                    byte[] attachByte = null;
                    FileStream fstream2 = new FileStream(this.attachmentPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr2 = new BinaryReader(fstream2);
                    attachByte = binRdr2.ReadBytes((int)fstream2.Length);

                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.student_profile (student_id, dept_id, stream_id, level, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1, photo, attachment) values('" + this.studID.Text + "', '" + this.deptID.Text + "', '" + this.streamID.Text + "' ,'" + this.level.Text + "', '"+this.fName.Text+"', '"+this.mName.Text+"', '"+this.lName.Text+"', '"+this.sex.Text+"', '"+this.admissionYear.Text+"', '"+this.programType.Text+"', '"+this.admissionType.Text+"', '"+this.wereda.Text+"', '"+this.kebele.Text+"', '"+this.gpa10.Text+"', '"+this.gpa12.Text+"' ,'"+this.phoneNumber.Text+"', @IMG, @ATTCH);";
                    sqlCmd = new MySqlCommand(insertQuery, conn);
                    try
                    {
                        conn.Open();
                        sqlCmd.Parameters.Add(new MySqlParameter("@IMG", imageByte));
                        sqlCmd.Parameters.Add(new MySqlParameter("@ATTCH", attachByte));
                        dataReader = sqlCmd.ExecuteReader();
                        while (dataReader.Read())
                        {

                        }
                        MessageBox.Show("Saved successfully!");
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Connection failed!");
                        conn.Close();
                    }
                }
                else
                {
                    MessageBox.Show("There is empty field(s). Please fill all fields!");
                }
            }
            else
            {
                MessageBox.Show("There is already a department with the same ID!");
            }

            //---Refresh dataGridStudents---
            string tableQuery = "Select student_id, dept_id, stream_id, level, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1 From ecc_dof_wukrostmarycollege.student_profile";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridStudents.RowTemplate.Height = 20;
            dataGridStudents.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridStudents.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (studID.Text == cellValue && level.Text == cellValue2)
            {
                if(photoPath.Text != "" && attachmentPath.Text == "")
                {
                    byte[] imageByte = null;
                    FileStream fstream = new FileStream(this.photoPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr = new BinaryReader(fstream);
                    imageByte = binRdr.ReadBytes((int)fstream.Length);

                    string updateQuery = "Update ecc_dof_wukrostmarycollege.student_profile Set dept_id = '" + this.deptID.Text + "', stream_id = '" + this.streamID.Text + "', first_name = '" + this.fName.Text + "', father_name = '" + this.mName.Text + "', grand_father_name = '" + this.lName.Text + "', gender = '" + this.sex.Text + "', admission_date = '" + this.admissionYear.Text + "', program_type = '" + this.programType.Text + "', admission_type = '" + this.admissionType.Text + "', wereda = '" + this.wereda.Text + "', kebele = '" + this.kebele.Text + "', gpa_grade_10th = '" + this.gpa10.Text + "', gpa_grade_12th = '" + this.gpa12.Text + "', mobile_number1 = '" + this.phoneNumber.Text + "', photo = @IMG Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "';";
                    sqlCmd = new MySqlCommand(updateQuery, conn);
                    try
                    {
                        conn.Open();
                        sqlCmd.Parameters.Add(new MySqlParameter("@IMG", imageByte));
                        dataReader = sqlCmd.ExecuteReader();
                        while (dataReader.Read())
                        {

                        }
                        MessageBox.Show("Upate successful!");
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Connection failed!");
                        conn.Close();
                    }
                }
                else if(photoPath.Text == "" && attachmentPath.Text != "")
                {
                    byte[] attachByte = null;
                    FileStream fstream2 = new FileStream(this.attachmentPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr2 = new BinaryReader(fstream2);
                    attachByte = binRdr2.ReadBytes((int)fstream2.Length);

                    string updateQuery = "Update ecc_dof_wukrostmarycollege.student_profile Set dept_id = '" + this.deptID.Text + "', stream_id = '" + this.streamID.Text + "', first_name = '" + this.fName.Text + "', father_name = '" + this.mName.Text + "', grand_father_name = '" + this.lName.Text + "', gender = '" + this.sex.Text + "', admission_date = '" + this.admissionYear.Text + "', program_type = '" + this.programType.Text + "', admission_type = '" + this.admissionType.Text + "', wereda = '" + this.wereda.Text + "', kebele = '" + this.kebele.Text + "', gpa_grade_10th = '" + this.gpa10.Text + "', gpa_grade_12th = '" + this.gpa12.Text + "', mobile_number1 = '" + this.phoneNumber.Text + "', attachment = @ATTCH Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "';";
                    sqlCmd = new MySqlCommand(updateQuery, conn);
                    try
                    {
                        conn.Open();
                        sqlCmd.Parameters.Add(new MySqlParameter("@ATTCH", attachByte));
                        dataReader = sqlCmd.ExecuteReader();
                        while (dataReader.Read())
                        {

                        }
                        MessageBox.Show("Upate successful!");
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Connection failed!");
                        conn.Close();
                    }
                }
                else if(photoPath.Text != "" && attachmentPath.Text != "")
                {
                    byte[] imageByte = null;
                    FileStream fstream = new FileStream(this.photoPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr = new BinaryReader(fstream);
                    imageByte = binRdr.ReadBytes((int)fstream.Length);

                    byte[] attachByte = null;
                    FileStream fstream2 = new FileStream(this.attachmentPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr2 = new BinaryReader(fstream2);
                    attachByte = binRdr2.ReadBytes((int)fstream2.Length);

                    string updateQuery = "Update ecc_dof_wukrostmarycollege.student_profile Set dept_id = '" + this.deptID.Text + "', stream_id = '" + this.streamID.Text + "', first_name = '" + this.fName.Text + "', father_name = '" + this.mName.Text + "', grand_father_name = '" + this.lName.Text + "', gender = '" + this.sex.Text + "', admission_date = '" + this.admissionYear.Text + "', program_type = '" + this.programType.Text + "', admission_type = '" + this.admissionType.Text + "', wereda = '" + this.wereda.Text + "', kebele = '" + this.kebele.Text + "', gpa_grade_10th = '" + this.gpa10.Text + "', gpa_grade_12th = '" + this.gpa12.Text + "', mobile_number1 = '" + this.phoneNumber.Text + "', photo = @IMG, attachment = @ATTCH Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "';";
                    sqlCmd = new MySqlCommand(updateQuery, conn);
                    try
                    {
                        conn.Open();
                        sqlCmd.Parameters.Add(new MySqlParameter("@IMG", imageByte));
                        sqlCmd.Parameters.Add(new MySqlParameter("@ATTCH", attachByte));
                        dataReader = sqlCmd.ExecuteReader();
                        while (dataReader.Read())
                        {

                        }
                        MessageBox.Show("Upate successful!");
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Connection failed!");
                        conn.Close();
                    }
                }
                else if(photoPath.Text == "" && attachmentPath.Text == "")
                {
                    string updateQuery = "Update ecc_dof_wukrostmarycollege.student_profile Set dept_id = '" + this.deptID.Text + "', stream_id = '" + this.streamID.Text + "', first_name = '" + this.fName.Text + "', father_name = '" + this.mName.Text + "', grand_father_name = '" + this.lName.Text + "', gender = '" + this.sex.Text + "', admission_date = '" + this.admissionYear.Text + "', program_type = '" + this.programType.Text + "', admission_type = '" + this.admissionType.Text + "', wereda = '" + this.wereda.Text + "', kebele = '" + this.kebele.Text + "', gpa_grade_10th = '" + this.gpa10.Text + "', gpa_grade_12th = '" + this.gpa12.Text + "', mobile_number1 = '" + this.phoneNumber.Text + "' Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "';";
                    sqlCmd = new MySqlCommand(updateQuery, conn);
                    try
                    {
                        conn.Open();
                        dataReader = sqlCmd.ExecuteReader();
                        while (dataReader.Read())
                        {

                        }
                        MessageBox.Show("Upate successful!");
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Connection failed!");
                        conn.Close();
                    }
                }
                else
                {

                }
            }
            else
            {
                MessageBox.Show("Update attempt failed!");
            }

            //---Refresh dataGridStudents---
            string tableQuery = "Select student_id, dept_id, stream_id, level, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1 From ecc_dof_wukrostmarycollege.student_profile";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridStudents.RowTemplate.Height = 20;
            dataGridStudents.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridStudents.DataSource = table;
        }

        private void enrollButton_Click(object sender, EventArgs e)
        {
            int counter = 0;
            string countQuery = "Select student_id From ecc_dof_wukrostmarycollege.student_profile where student_id='" + this.searchID.Text + "'";
            sqlCmd = new MySqlCommand(countQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    ++counter;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed!");
            }
            if(counter != 0)
            {
                int level = counter + 1;
                string dptId = null, strmID = null, fNme = null, mNme = null, lNme = null, gndr = null, admssnDate = null, prgrmType = null, admssnType = null, wrda = null, kble = null, gpa10th = null, gpa12th = null, mobNmbr = null;
                string selectQuery = "Select dept_id, stream_id, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1 From ecc_dof_wukrostmarycollege.student_profile Where student_id = '" + this.searchID.Text + "' and level = '"+counter+"'";
                sqlCmd = new MySqlCommand(selectQuery, conn);
                try
                {
                    conn.Open();
                    dataReader = sqlCmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        dptId = dataReader["dept_id"].ToString();
                        strmID = dataReader["stream_id"].ToString();
                        fNme = dataReader["first_name"].ToString();
                        mNme = dataReader["father_name"].ToString();
                        lNme = dataReader["grand_father_name"].ToString();
                        gndr = dataReader["gender"].ToString();
                        admssnDate = dataReader["admission_date"].ToString();
                        prgrmType = dataReader["program_type"].ToString();
                        admssnType = dataReader["admission_type"].ToString();
                        wrda = dataReader["wereda"].ToString();
                        kble = dataReader["kebele"].ToString();
                        gpa10th = dataReader["gpa_grade_10th"].ToString();
                        gpa12th = dataReader["gpa_grade_12th"].ToString();
                        mobNmbr = dataReader["mobile_number1"].ToString();

                        byte[] img = (byte[])dataReader["photo"];
                        MemoryStream ms = new MemoryStream(img);
                        studPic.Image = Image.FromStream(ms);

                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed!");
                    conn.Close();
                }
                if (photoPath.Text != "" && attachmentPath.Text != "")
                {
                    byte[] imageByte = null;
                    FileStream fstream = new FileStream(this.photoPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr = new BinaryReader(fstream);
                    imageByte = binRdr.ReadBytes((int)fstream.Length);

                    byte[] attachByte = null;
                    FileStream fstream2 = new FileStream(this.attachmentPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr2 = new BinaryReader(fstream2);
                    attachByte = binRdr2.ReadBytes((int)fstream2.Length);

                    string enrollQuery = "Insert Into ecc_dof_wukrostmarycollege.student_profile (student_id, dept_id, stream_id, level, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1, photo, attachment) values('" + this.searchID.Text + "', '" + dptId + "', '" + strmID + "' ,'" + level + "', '" + fNme + "', '" + mNme + "', '" + lNme + "', '" + gndr + "', '" + admssnDate + "', '" + prgrmType + "', '" + admssnType + "', '" + wrda + "', '" + kble + "', '" + gpa10th + "', '" + gpa12th + "' ,'" + mobNmbr + "', @IMG, @ATTCH);";
                    sqlCmd = new MySqlCommand(enrollQuery, conn);
                    try
                    {
                        conn.Open();
                        dataReader = sqlCmd.ExecuteReader();
                        sqlCmd.Parameters.Add(new MySqlParameter("@IMG", imageByte));
                        sqlCmd.Parameters.Add(new MySqlParameter("@ATTCH", attachByte));
                        while (dataReader.Read())
                        {

                        }
                        MessageBox.Show("Saved successfully!");
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Connection failed!");
                        conn.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a photo and an attachment!");
                }
            }
            else
            {
                MessageBox.Show("No such student has been registered!");
            }
            
            //---Refresh dataGridStudents---
            string tableQuery = "Select student_id, dept_id, stream_id, level, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1 From ecc_dof_wukrostmarycollege.student_profile";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridStudents.RowTemplate.Height = 20;
            dataGridStudents.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridStudents.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.student_profile Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "';";
            sqlCmd = new MySqlCommand(deleteQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {

                }
                MessageBox.Show("Delete successful!");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed!");
                conn.Close();
            }

            //---Refresh dataGridStudents---
            string tableQuery = "Select student_id, dept_id, stream_id, level, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1 From ecc_dof_wukrostmarycollege.student_profile";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridStudents.RowTemplate.Height = 20;
            dataGridStudents.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridStudents.DataSource = table;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "JPG Files(*.jpg)|*.jpg|PNG Files(*.png)|*.png|All Files(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string picPath = dlg.FileName.ToString();
                photoPath.Text = picPath;
                studPic.ImageLocation = picPath;
            }
        }

        private void attachButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "PDF Files(*.pdf)|*.pdf|Word Files(*.docx)|*.docx|All Files(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string attachPath = dlg.FileName.ToString();
                attachmentPath.Text = attachPath;
            }
        }

        private void dataGridStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridStudents.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();
            cellValue2 = row.Cells[3].Value.ToString();

            string dptId = null, strmID = null, fNme = null, mNme = null, lNme = null, gndr = null, admssnDate = null, prgrmType = null, admssnType = null, wrda = null, kble = null, gpa10th = null, gpa12th = null, mobNmbr = null;
            string query = "Select dept_id, stream_id, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1, photo From ecc_dof_wukrostmarycollege.student_profile Where student_id = '" + cellValue + "' and level = '"+cellValue2+"'";
            sqlCmd = new MySqlCommand(query, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    dptId = dataReader["dept_id"].ToString();
                    strmID = dataReader["stream_id"].ToString();
                    fNme = dataReader["first_name"].ToString();
                    mNme = dataReader["father_name"].ToString();
                    lNme = dataReader["grand_father_name"].ToString();
                    gndr = dataReader["gender"].ToString();
                    admssnDate = dataReader["admission_date"].ToString();
                    prgrmType = dataReader["program_type"].ToString();
                    admssnType = dataReader["admission_type"].ToString();
                    wrda = dataReader["wereda"].ToString();
                    kble = dataReader["kebele"].ToString();
                    gpa10th = dataReader["gpa_grade_10th"].ToString();
                    gpa12th = dataReader["gpa_grade_12th"].ToString();
                    mobNmbr = dataReader["mobile_number1"].ToString();

                    byte[] img = (byte[])dataReader["photo"];
                    MemoryStream ms = new MemoryStream(img);
                    studPic.Image = Image.FromStream(ms);

                    studID.Text = cellValue;
                    deptID.Text = dptId;
                    streamID.Text = strmID;
                    level.Text = cellValue2;
                    fName.Text = fNme;
                    mName.Text = mNme;
                    lName.Text = lNme;
                    sex.Text = gndr;
                    admissionYear.Text = admssnDate;
                    programType.Text = prgrmType;
                    admissionType.Text = admssnType;
                    wereda.Text = wrda;
                    kebele.Text = kble;
                    gpa10.Text = gpa10th;
                    gpa12.Text = gpa12th;
                    phoneNumber.Text = mobNmbr;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed!");
                conn.Close();
            }

            string dept;
            string miniSelect = "Select dept_name From ecc_dof_wukrostmarycollege.departments Where dept_id = '" + dptId + "'";
            sqlCmd = new MySqlCommand(miniSelect, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    dept = dataReader["dept_name"].ToString();

                    fullName.Text = fNme + " " + mNme + " " + lNme;
                    dpName.Text = dept + " (" + dptId+ ")";
                    enrollement.Text = prgrmType + "  |  " + admssnType + "  |  " + admssnDate;
                    wer.Text = wrda;
                    keb.Text = kble;
                    mobile.Text = mobNmbr;

                    _name.Text = "Name";
                    _department.Text = "Department";
                    _enrollment.Text = "Enrollment";
                    _wereda.Text = "Wereda";
                    _kebele.Text = "Kebele";
                    _mobile.Text = "Mobile";
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed!");
                conn.Close();
            }
        }

        private void __filterButton_Click(object sender, EventArgs e)
        {
            if (__deptID.Text != "" && __level.Text != "" && __admissionYear.Text != "" && __admissionType.Text != "")
            {
                string tableQuery = "Select student_id, dept_id, stream_id, level, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1 From ecc_dof_wukrostmarycollege.student_profile Where dept_id = '" + this.__deptID.Text + "' and level = '" + this.__level.Text + "' and admission_date = '" + this.__admissionYear.Text + "' and admission_type = '" + this.__admissionType.Text + "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridStudents.RowTemplate.Height = 20;
                dataGridStudents.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridStudents.DataSource = table;
                ApplyGridStyle();
            }
            else
            {
                MessageBox.Show("Invalid filter parameters!");
            }
        }

        private void downloadPhotoButton_Click(object sender, EventArgs e)
        {
            //SqlConnection pubsConn = new SqlConnection("Data Source=localhost;Integrated Security=SSPI;Initial Catalog=pubs;");
            //SqlCommand logoCMD = new SqlCommand("SELECT pub_id, logo FROM pub_info", pubsConn);
            

            FileStream fs;                          // Writes the BLOB to a file (*.bmp).
            BinaryWriter bw;                        // Streams the BLOB to the FileStream object.

            int bufferSize = 300;                   // Size of the BLOB buffer.
            byte[] outbyte = new byte[bufferSize];  // The BLOB byte[] buffer to be filled by GetBytes.
            long retval;                            // The bytes returned from GetBytes.
            long startIndex = 0;                    // The starting position in the BLOB output.

            string pub_id = "";                     // The publisher id to use in the file name.
            string query = "Select photo From ecc_dof_wukrostmarycollege.student_profile Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "'";
            sqlCmd = new MySqlCommand(query, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader(CommandBehavior.SequentialAccess);
                while (dataReader.Read())
                {
                    // Get the publisher id, which must occur before getting the logo.
                    pub_id = dataReader.GetString(0);

                    // Create a file to hold the output.
                    fs = new FileStream("logo" + ".bmp", FileMode.OpenOrCreate, FileAccess.Write);
                    bw = new BinaryWriter(fs);

                    // Reset the starting byte for the new BLOB.
                    startIndex = 0;

                    // Read the bytes into outbyte[] and retain the number of bytes returned.
                    retval = dataReader.GetBytes(1, startIndex, outbyte, 0, bufferSize);

                    // Continue reading and writing while there are bytes beyond the size of the buffer.
                    while (retval == bufferSize)
                    {
                        bw.Write(outbyte);
                        bw.Flush();

                        // Reposition the start index to the end of the last buffer and fill the buffer.
                        startIndex += bufferSize;
                        retval = dataReader.GetBytes(1, startIndex, outbyte, 0, bufferSize);
                    }

                    // Write the remaining buffer.
                    bw.Write(outbyte, 0, (int)retval - 1);
                    bw.Flush();

                    // Close the output file.
                    bw.Close();
                    fs.Close();
                }

                // Close the reader and the connection.
                dataReader.Close();
                conn.Close();
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Connection failed! " + ex.Message);
                conn.Close();
            }
        }

        private void downloadAttachmentButton_Click(object sender, EventArgs e)
        {

        }

        //---Menu---
        private void studentMarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StudMark sm = new StudMark();
            sm.Show();
            this.Close();
        }

        private void studentFeesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StudFees sf = new StudFees();
            sf.Show();
            this.Close();
        }

        private void dropoutStudentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dropout dout = new Dropout();
            dout.Show();
            this.Close();
        }

        private void cOCRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            COCRecord cr = new COCRecord();
            cr.Show();
            this.Close();
        }

        private void instructorsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Departments dp = new Departments();
            dp.Show();
            this.Close();
        }

        private void streamsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Streams st = new Streams();
            st.Show();
            this.Close();
        }

        private void levelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Levels lv = new Levels();
            lv.Show();
            this.Close();
        }

        private void coursesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Courses cr = new Courses();
            cr.Show();
            this.Close();
        }

        private void instructorsToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Instructors ins = new Instructors();
            ins.Show();
            this.Close();
        }

        private void alumniToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Alumni al = new Alumni();
            al.Show();
            this.Close();
        }

        private void manageAdminsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Admins ad = new Admins();
            ad.Show();
            this.Close();
        }

        private void signOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Close();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Help hl = new Help();
            hl.Show();
            this.Close();
        }

        private void libraryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Library lb = new Library();
            lb.Show();
            this.Close();
        }

        //---End Menu---

        private void searchButton_Click(object sender, EventArgs e)
        {
            string tableQuery = "Select student_id, dept_id, stream_id, level, first_name, father_name, grand_father_name, gender, admission_date, program_type, admission_type, wereda, kebele, gpa_grade_10th, gpa_grade_12th, mobile_number1 From ecc_dof_wukrostmarycollege.student_profile Where student_id = '"+this.searchID.Text+"'";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridStudents.RowTemplate.Height = 20;
            dataGridStudents.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridStudents.DataSource = table;

            // Apply consistent row styling
            dataGridStudents.DefaultCellStyle.BackColor = Color.FromArgb(31, 56, 100);
            dataGridStudents.DefaultCellStyle.ForeColor = Color.White;
            dataGridStudents.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dataGridStudents.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridStudents.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(31, 56, 100);
            dataGridStudents.AlternatingRowsDefaultCellStyle.ForeColor = Color.White;
            dataGridStudents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 40, 80);
            dataGridStudents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridStudents.EnableHeadersVisualStyles = false;
            dataGridStudents.GridColor = Color.FromArgb(50, 80, 130);
        }
        private void ApplyGridStyle()
                {
                    dataGridStudents.DefaultCellStyle.BackColor = Color.FromArgb(31, 56, 100);
                    dataGridStudents.DefaultCellStyle.ForeColor = Color.White;
                    dataGridStudents.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
                    dataGridStudents.DefaultCellStyle.SelectionForeColor = Color.White;
                    dataGridStudents.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(31, 56, 100);
                    dataGridStudents.AlternatingRowsDefaultCellStyle.ForeColor = Color.White;
                    dataGridStudents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 40, 80);
                    dataGridStudents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dataGridStudents.EnableHeadersVisualStyles = false;
                    dataGridStudents.GridColor = Color.FromArgb(50, 80, 130);
                }

        private void ApplyGridStyle()
        {
            dataGridStudents.DefaultCellStyle.BackColor = Color.FromArgb(31, 56, 100);
            dataGridStudents.DefaultCellStyle.ForeColor = Color.White;
            dataGridStudents.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dataGridStudents.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridStudents.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(31, 56, 100);
            dataGridStudents.AlternatingRowsDefaultCellStyle.ForeColor = Color.White;
            dataGridStudents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 40, 80);
            dataGridStudents.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridStudents.EnableHeadersVisualStyles = false;
            dataGridStudents.GridColor = Color.FromArgb(50, 80, 130);
        }

        private void __printButton_Click(object sender, EventArgs e)
        {
            
        }

        //---Menu---
        private void homeToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            HomePage hp = new HomePage();
            hp.Show();
            this.Close();
        }

        private void studentRegistrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StudRegistration sr = new StudRegistration();
            sr.Show();
            this.Close();
        }

        //---End Menu---
    }
}

