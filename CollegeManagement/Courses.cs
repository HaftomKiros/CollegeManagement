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
    public partial class Courses : Form
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
        public Courses()
        {
            InitializeComponent();
            //---Initialize dataGridCourses---
            db = new DBConnect();
            conn = db.getConnection();
            string tableQuery = "Select level_id, module_code, unit_of_competence_title, total_hours From ecc_dof_wukrostmarycollege.courses";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridCourses.RowTemplate.Height = 20;
            dataGridCourses.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridCourses.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            int checker = 0;
            string checkQuery = "Select module_code From ecc_dof_wukrostmarycollege.courses where module_code='" + this.moduleCode.Text + "'";
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
                if (levelID.Text != "" && moduleCode.Text != "" && unitCompetence.Text != "" && fuckers.Text != "")
                {
                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.courses (level_id, module_code, unit_of_competence_title, total_hours) values('" + this.levelID.Text + "', '" + this.moduleCode.Text + "', '" + this.unitCompetence.Text + "' ,'" + this.totalHours.Text + "');";
                    sqlCmd = new MySqlCommand(insertQuery, conn);
                    try
                    {
                        conn.Open();
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

            //---Initialize dataGridCourses---
            string tableQuery = "Select level_id, module_code, unit_of_competence_title, total_hours From ecc_dof_wukrostmarycollege.courses";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridCourses.RowTemplate.Height = 20;
            dataGridCourses.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridCourses.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (moduleCode.Text == cellValue)
            {
                string updateQuery = "Update ecc_dof_wukrostmarycollege.courses Set level_id = '" + this.levelID.Text + "', unit_of_competence_title = '" + this.unitCompetence.Text + "', total_hours = '"+ this.totalHours.Text+"' Where module_code = '" + cellValue + "';";
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
                MessageBox.Show("Update attempt failed!");
            }

            //---Initialize dataGridCourses---
            string tableQuery = "Select level_id, module_code, unit_of_competence_title, total_hours From ecc_dof_wukrostmarycollege.courses";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridCourses.RowTemplate.Height = 20;
            dataGridCourses.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridCourses.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.courses Where module_code = '" + cellValue + "';";
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

            //---Initialize dataGridCourses---
            string tableQuery = "Select level_id, module_code, unit_of_competence_title, total_hours From ecc_dof_wukrostmarycollege.courses";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridCourses.RowTemplate.Height = 20;
            dataGridCourses.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridCourses.DataSource = table;
        }

        private void dataGridCourses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridCourses.Rows[rowIndex];
            cellValue = row.Cells[1].Value.ToString();

            string lvlID, unitCmptnc, totalHrs;
            string retrieveQuery = "Select level_id, unit_of_competence_title, total_hours From ecc_dof_wukrostmarycollege.courses Where module_code = '" + cellValue + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    lvlID = dataReader["level_id"].ToString();
                    unitCmptnc = dataReader["unit_of_competence_title"].ToString();
                    totalHrs = dataReader["total_hours"].ToString();

                    moduleCode.Text = cellValue;
                    levelID.Text = lvlID;
                    unitCompetence.Text = unitCmptnc;
                    totalHours.Text = totalHrs;
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
            db = new DBConnect();
            conn = db.getConnection();

            if (__deptID.Text != "" && __streamID.Text == "" && __levelID.Text == "")
            {
                string tableQuery = "Select courses.level_id, courses.module_code, courses.unit_of_competence_title, courses.total_hours From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.courses Where departments.dept_id = '" + this.__deptID.Text + "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridCourses.RowTemplate.Height = 20;
                dataGridCourses.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridCourses.DataSource = table;
            }
            else if (__deptID.Text != "" && __streamID.Text != "" && __levelID.Text == "")
            {
                string tableQuery = "Select courses.level_id, courses.module_code, courses.unit_of_competence_title, courses.total_hours From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.streams, ecc_dof_wukrostmarycollege.courses Where departments.dept_id = '" + this.__deptID.Text + "' and streams.stream_id = '" + this.__streamID.Text + "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridCourses.RowTemplate.Height = 20;
                dataGridCourses.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridCourses.DataSource = table;
            }
            else if (__deptID.Text != "" && __streamID.Text != "" && __levelID.Text != "")
            {
                string tableQuery = "Select courses.level_id, courses.module_code, courses.unit_of_competence_title, courses.total_hours From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.streams, ecc_dof_wukrostmarycollege.courses Where departments.dept_id = '" + this.__deptID.Text + "' and streams.stream_id = '" + this.__streamID.Text + "' and courses.level_id = '" + this.__levelID.Text + "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridCourses.RowTemplate.Height = 20;
                dataGridCourses.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridCourses.DataSource = table;
            }
            else
            {
                MessageBox.Show("Invalid filter parameters!");
            }
        }

        private void __printButton_Click(object sender, EventArgs e)
        {

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
    }
}
