using System;
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

namespace CollegeManagement
{
    public partial class Instructors : Form
    {
        string cellValue = null;
        int rowIndex;
        private DBConnect db;
        MySqlConnection conn = null;
        MySqlCommand sqlCmd;
        MySqlDataReader dataReader;
        MySqlDataAdapter dataAdapter;

        //static string connString = "server=localhost;database=ecc_dof_wukrostmarycollege;uid=root;pwd=";
        //MySqlConnection conn = new MySqlConnection(connString);
        //MySqlCommand sqlCmd;
        //MySqlDataReader dataReader;
        //MySqlDataAdapter dataAdapter;
        public Instructors()
        {
            InitializeComponent();
            //---Initialize dataGridInstructors---
            db = new DBConnect();
            conn = db.getConnection();
            string tableQuery = "Select employee_id, department_id, first_name, middle_name, last_name, sex, birth_date, employee_date, level, qualification_title, mobile_number From ecc_dof_wukrostmarycollege.employee_profile";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridInstructors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridInstructors.RowTemplate.Height = 20;
            dataGridInstructors.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridInstructors.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            int checker = 0;
            string checkQuery = "Select employee_id From ecc_dof_wukrostmarycollege.employee_profile where employee_id = '" + this.empID.Text + "'";
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
                if (empID.Text != "" && deptID.Text != "" && empDate.Text != "" && fName.Text != "" && mName.Text != "" && lName.Text != "" && sex.Text != "" && mobNum.Text != "" && birthDate.Text != "" && level.Text != "" && qualficationTitle.Text != "" && empPhotoPath.Text != "" && empDocPath.Text != "")
                {
                    byte[] imageByte = null;
                    FileStream fstream = new FileStream(this.empPhotoPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr = new BinaryReader(fstream);
                    imageByte = binRdr.ReadBytes((int)fstream.Length);

                    byte[] attachByte = null;
                    FileStream fstream2 = new FileStream(this.empDocPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr2 = new BinaryReader(fstream2);
                    attachByte = binRdr2.ReadBytes((int)fstream2.Length);

                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.employee_profile (employee_id, department_id, first_name, middle_name, last_name, sex, birth_date, employee_date, level, qualification_title, mobile_number, photo, attachment) values('" + this.empID.Text + "', '" + this.deptID.Text + "','" + this.fName.Text + "', '"+this.mName.Text+"', '"+this.lName.Text+"', '"+this.sex.Text+"', '"+this.birthDate.Text+"','"+this.empDate.Text+"', '"+this.level.Text+"', '"+this.qualficationTitle.Text+"', '"+this.mobNum.Text+"', @IMG, @ATTCH);";
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
                MessageBox.Show("There is already an employee with the same ID!");
            }

            //---Refreshing dataGridInstructors---
            string tableQuery = "Select employee_id, department_id, first_name, middle_name, last_name, sex, birth_date, employee_date, level, qualification_title, mobile_number From ecc_dof_wukrostmarycollege.employee_profile";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridInstructors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridInstructors.RowTemplate.Height = 20;
            dataGridInstructors.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridInstructors.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (empID.Text == cellValue)
            {
                if (empPhotoPath.Text != "" && empDocPath.Text == "")
                {
                    byte[] imageByte = null;
                    FileStream fstream = new FileStream(this.empPhotoPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr = new BinaryReader(fstream);
                    imageByte = binRdr.ReadBytes((int)fstream.Length);

                    string updateQuery = "Update ecc_dof_wukrostmarycollege.employee_profile Set department_id = '" + this.deptID.Text + "', first_name = '" + this.fName.Text + "', middle_name = '" + this.mName.Text + "', last_name = '" + this.lName.Text + "', sex = '" + this.sex.Text + "', birth_date = '" + this.birthDate.Text + "', employee_date = '" + this.empDate.Text + "', level = '" + this.level.Text + "', qualification_title = '" + this.qualficationTitle.Text + "', mobile_number = '" + this.mobNum.Text + "', photo = @IMG Where employee_id = '" + cellValue + "';";
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
                else if (empPhotoPath.Text == "" && empDocPath.Text != "")
                {
                    byte[] attachByte = null;
                    FileStream fstream2 = new FileStream(this.empDocPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr2 = new BinaryReader(fstream2);
                    attachByte = binRdr2.ReadBytes((int)fstream2.Length);

                    string updateQuery = "Update ecc_dof_wukrostmarycollege.employee_profile Set department_id = '" + this.deptID.Text + "', first_name = '" + this.fName.Text + "', middle_name = '" + this.mName.Text + "', last_name = '" + this.lName.Text + "', sex = '" + this.sex.Text + "', birth_date = '" + this.birthDate.Text + "', employee_date = '" + this.empDate.Text + "', level = '" + this.level.Text + "', qualification_title = '" + this.qualficationTitle.Text + "', mobile_number = '" + this.mobNum.Text + "', attachment = @ATTCH Where employee_id = '" + cellValue + "';";
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
                else if (empPhotoPath.Text != "" && empDocPath.Text != "")
                {
                    byte[] imageByte = null;
                    FileStream fstream = new FileStream(this.empPhotoPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr = new BinaryReader(fstream);
                    imageByte = binRdr.ReadBytes((int)fstream.Length);

                    byte[] attachByte = null;
                    FileStream fstream2 = new FileStream(this.empDocPath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr2 = new BinaryReader(fstream2);
                    attachByte = binRdr2.ReadBytes((int)fstream2.Length);

                    string updateQuery = "Update ecc_dof_wukrostmarycollege.employee_profile Set department_id = '" + this.deptID.Text + "', first_name = '" + this.fName.Text + "', middle_name = '" + this.mName.Text + "', last_name = '" + this.lName.Text + "', sex = '" + this.sex.Text + "', birth_date = '" + this.birthDate.Text + "', employee_date = '" + this.empDate.Text + "', level = '" + this.level.Text + "', qualification_title = '" + this.qualficationTitle.Text + "', mobile_number = '" + this.mobNum.Text + "', attachment = @ATTCH Where employee_id = '" + cellValue + "';";
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
                else if (empPhotoPath.Text == "" && empDocPath.Text == "")
                {
                    string updateQuery = "Update ecc_dof_wukrostmarycollege.employee_profile Set department_id = '" + this.deptID.Text + "', first_name = '" + this.fName.Text + "', middle_name = '" + this.mName.Text + "', last_name = '" + this.lName.Text + "', sex = '" + this.sex.Text + "', birth_date = '" + this.birthDate.Text + "', employee_date = '" + this.empDate.Text + "', level = '" + this.level.Text + "', qualification_title = '" + this.qualficationTitle.Text + "', mobile_number = '" + this.mobNum.Text + "' Where employee_id = '" + cellValue + "';";
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
                    MessageBox.Show("Error!");
                }
            }
            else
            {
                MessageBox.Show("Update attempt failed!");
            }

            //---Refreshing dataGridInstructors---
            string tableQuery = "Select employee_id, department_id, first_name, middle_name, last_name, sex, birth_date, employee_date, level, qualification_title, mobile_number From ecc_dof_wukrostmarycollege.employee_profile";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridInstructors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridInstructors.RowTemplate.Height = 20;
            dataGridInstructors.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridInstructors.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.employee_profile Where employee_id = '" + cellValue + "';";
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

            //---Refreshing dataGridInstructors---
            string tableQuery = "Select employee_id, department_id, first_name, middle_name, last_name, sex, birth_date, employee_date, level, qualification_title, mobile_number From ecc_dof_wukrostmarycollege.employee_profile";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridInstructors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridInstructors.RowTemplate.Height = 20;
            dataGridInstructors.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridInstructors.DataSource = table;
        }

        private void dataGridInstructors_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridInstructors.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();

            string dptID = null, fNme = null, mNme = null, lNme = null, sx = null, birthDte = null, empDte = null, lvl = null, qualificationTtle = null, mobNmbr = null;
            string retrieveQuery = "Select department_id, first_name, middle_name, last_name, sex, birth_date, employee_date, level, qualification_title, mobile_number, photo From ecc_dof_wukrostmarycollege.employee_profile Where employee_id = '" + cellValue + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    dptID = dataReader["department_id"].ToString();
                    fNme = dataReader["first_name"].ToString();
                    mNme = dataReader["middle_name"].ToString();
                    lNme = dataReader["last_name"].ToString();
                    sx = dataReader["sex"].ToString();
                    birthDte = dataReader["birth_date"].ToString();
                    empDte = dataReader["employee_date"].ToString();
                    lvl = dataReader["level"].ToString();
                    qualificationTtle = dataReader["qualification_title"].ToString();
                    mobNmbr = dataReader["mobile_number"].ToString();

                    byte[] img = (byte[])dataReader["photo"];
                    MemoryStream ms = new MemoryStream(img);
                    empPic.Image = Image.FromStream(ms);

                    empID.Text = cellValue;
                    deptID.Text = dptID;
                    fName.Text = fNme;
                    mName.Text = mNme;
                    lName.Text = lNme;
                    sex.Text = sx;
                    birthDate.Text = birthDte;
                    empDate.Text = empDte;
                    level.Text = lvl;
                    qualficationTitle.Text = qualificationTtle;
                    mobNum.Text = mobNmbr;
                }
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Connection failed! " + ex.Message);
                conn.Close();
            }

            string dept;
            string miniSelect = "Select dept_name From ecc_dof_wukrostmarycollege.departments Where dept_id = '" + dptID + "'";
            sqlCmd = new MySqlCommand(miniSelect, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    dept = dataReader["dept_name"].ToString();

                    nameTag.Text = "Name";
                    deptTag.Text = "Department";
                    qualificationTag.Text = "Qualification";
                    levelTag.Text = "Level";
                    mobNumTag.Text = "Mobile";

                    __nameTag.Text = fNme + " " + mNme + " " + lNme;
                    __deptTag.Text = dept;
                    __qualificationTag.Text = qualificationTtle;
                    __levelTag.Text = lvl;
                    __mobNumTag.Text = mobNmbr;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed!");
                conn.Close();
            }
        }

        //---Menu---
        private void homeToolStripMenuItem1_Click(object sender, EventArgs e)
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

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help hl = new Help();
            hl.Show();
            this.Close();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void __printButton_Click(object sender, EventArgs e)
        {

        }

        private void __filterButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (__empID.Text != "" && __deptID.Text == "")
            {
                string tableQuery = "Select employee_id, department_id, first_name, middle_name, last_name, sex, birth_date, employee_date, level, qualification_title, mobile_number From ecc_dof_wukrostmarycollege.employee_profile Where employee_id = '"+this.__empID.Text+"'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridInstructors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridInstructors.RowTemplate.Height = 20;
                dataGridInstructors.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridInstructors.DataSource = table;
            }
            else if (__empID.Text == "" && __deptID.Text != "")
            {
                string tableQuery = "Select employee_id, department_id, first_name, middle_name, last_name, sex, birth_date, employee_date, level, qualification_title, mobile_number From ecc_dof_wukrostmarycollege.employee_profile Where department_id = '" + this.__deptID.Text + "' and level = '"+this.__level.Text+"'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridInstructors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridInstructors.RowTemplate.Height = 20;
                dataGridInstructors.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridInstructors.DataSource = table;
            }
            else
            {
                MessageBox.Show("Invalid filter parameters!");
            }
        }

        private void __month_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void dataGridInstructors_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void picBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "JPG Files(*.jpg)|*.jpg|PNG Files(*.png)|*.png|All Files(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string picPath = dlg.FileName.ToString();
                empPhotoPath.Text = picPath;
                empPic.ImageLocation = picPath;
            }
        }

        private void attachButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "PDF Files(*.pdf)|*.pdf|Word Files(*.docx)|*.docx|All Files(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string attachPath = dlg.FileName.ToString();
                empDocPath.Text = attachPath;
            }
        }

        private void libraryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Library lb = new Library();
            lb.Show();
            this.Close();
        }

        //---End Menu---
    }
}
