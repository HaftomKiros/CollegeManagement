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
    public partial class Departments : Form
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
        public Departments()
        {
            InitializeComponent();
            //---Initialize dataGridDepts---
            db = new DBConnect();
            conn = db.getConnection();
            string tableQuery = "Select dept_id, dept_name, dept_program, dept_head From ecc_dof_wukrostmarycollege.departments";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridDepts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridDepts.RowTemplate.Height = 20;
            dataGridDepts.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridDepts.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            int checker = 0;
            string checkQuery = "Select dept_id From ecc_dof_wukrostmarycollege.departments where dept_id='" + this.deptID.Text + "'";
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
                if (deptID.Text != "" && deptName.Text != "" && deptProgram.Text != "" && deptHead.Text != "")
                {
                    string insertQuery = "Insert into ecc_dof_wukrostmarycollege.departments (dept_id, dept_name, dept_program, dept_head) values('" + this.deptID.Text + "', '" + this.deptName.Text + "', '" + this.deptProgram.Text + "', '" + this.deptHead.Text + "');";
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

            //---Updata dataGridDepts---
            string tableQuery = "Select dept_id, dept_name, dept_program, dept_head From ecc_dof_wukrostmarycollege.departments";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridDepts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridDepts.RowTemplate.Height = 20;
            dataGridDepts.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridDepts.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (deptID.Text == cellValue)
            {
                string updateQuery = "Update ecc_dof_wukrostmarycollege.departments Set dept_name = '" + this.deptName.Text + "', dept_program = '" + this.deptProgram.Text + "', dept_head = '" + this.deptHead.Text + "' Where dept_id = '" + cellValue + "';";
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

            //---Updata dataGridDepts---
            string tableQuery = "Select dept_id, dept_name, dept_program, dept_head From ecc_dof_wukrostmarycollege.departments";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridDepts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridDepts.RowTemplate.Height = 20;
            dataGridDepts.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridDepts.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.departments Where dept_id = '"+ cellValue+"';";
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

            //---Updata dataGridDepts---
            string tableQuery = "Select dept_id, dept_name, dept_program, dept_head From ecc_dof_wukrostmarycollege.departments";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridDepts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridDepts.RowTemplate.Height = 20;
            dataGridDepts.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridDepts.DataSource = table;
        }

        private void dataGridDepts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridDepts.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();

            string depProgram, depName, depHead;
            string retrieveQuery = "Select dept_id, dept_name, dept_program, dept_head From ecc_dof_wukrostmarycollege.departments Where dept_id = '" + cellValue + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    depName = dataReader["dept_name"].ToString();
                    depProgram = dataReader["dept_program"].ToString();
                    depHead = dataReader["dept_head"].ToString();

                    deptID.Text = cellValue;
                    deptName.Text = depName;
                    deptProgram.Text = depProgram;
                    deptHead.Text = depHead;
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
