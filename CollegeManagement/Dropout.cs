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
    public partial class Dropout : Form
    {
        string cellValue = null, cellValue2 = null;
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
        public Dropout()
        {
            InitializeComponent();
            //---Initialize dataGridDrop---
            db = new DBConnect();
            conn = db.getConnection();
            string tableQuery = "Select student_id, drop_out_date, level_number, drop_out_reason, remark From ecc_dof_wukrostmarycollege.drop_out_students";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridDrop.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridDrop.RowTemplate.Height = 20;
            dataGridDrop.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridDrop.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            int checker = 0;
            string checkQuery = "Select student_id, level_number From ecc_dof_wukrostmarycollege.drop_out_students Where student_id='" + this.studID.Text + "' and level_number='" + this.level.Text + "'";
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
                if (studID.Text != "" && level.Text != "" && dropYear.Text != "" && dropReason.Text != "" && remark.Text != "")
                {
                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.drop_out_students (student_id, drop_out_date, level_number, drop_out_reason, remark) values('" + this.studID.Text + "', '" + this.dropYear.Text + "', '" + this.level.Text + "' ,'" + this.dropReason.Text + "', '" + this.remark.Text + "');";
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

            //---Initialize dataGridDrop---
            string tableQuery = "Select student_id, drop_out_date, level_number, drop_out_reason, remark From ecc_dof_wukrostmarycollege.drop_out_students";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridDrop.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridDrop.RowTemplate.Height = 20;
            dataGridDrop.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridDrop.DataSource = table;
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (studID.Text == cellValue && level.Text == cellValue2)
            {
                string updateQuery = "Update ecc_dof_wukrostmarycollege.drop_out_students Set drop_out_date = '" + this.dropYear.Text + "', drop_out_reason = '" + this.dropReason.Text + "', remark = '" + this.remark.Text + "' Where student_id = '" + cellValue + "' and level_number = '" + cellValue2 + "';";
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

            //---Initialize dataGridDrop---
            string tableQuery = "Select student_id, drop_out_date, level_number, drop_out_reason, remark From ecc_dof_wukrostmarycollege.drop_out_students";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridDrop.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridDrop.RowTemplate.Height = 20;
            dataGridDrop.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridDrop.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.drop_out_students Where student_id = '" + cellValue + "' and level_number = '" + cellValue2 + "';";
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

            //---Initialize dataGridDrop---
            string tableQuery = "Select student_id, drop_out_date, level_number, drop_out_reason, remark From ecc_dof_wukrostmarycollege.drop_out_students";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridDrop.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridDrop.RowTemplate.Height = 20;
            dataGridDrop.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridDrop.DataSource = table;
        }

        private void dataGridDrop_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridDrop.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();
            cellValue2 = row.Cells[2].Value.ToString();


            string drpYr, drpRsn, rmrk;
            string retrieveQuery = "Select drop_out_date, drop_out_reason, remark From ecc_dof_wukrostmarycollege.drop_out_students Where student_id = '" + cellValue + "' and level_number = '" + cellValue2 + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    drpYr = dataReader["drop_out_date"].ToString();
                    drpRsn = dataReader["drop_out_reason"].ToString();
                    rmrk = dataReader["remark"].ToString();

                    studID.Text = cellValue;
                    level.Text = cellValue2;
                    dropYear.Text = drpYr;
                    dropReason.Text = drpRsn;
                    remark.Text = rmrk;
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

            if (__deptID.Text != "" && __streamID.Text != "" && __dropYear.Text != "" && __level.Text != "")
            {
                string tableQuery = "Select drop_out_students.student_id, drop_out_students.drop_out_date, drop_out_students.level_number, drop_out_students.drop_out_reason, drop_out_students.remark From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.streams, ecc_dof_wukrostmarycollege.drop_out_students Where departments.dept_id = '" + this.__deptID.Text + "' and streams.stream_id = '"+this.__streamID.Text+"' and drop_out_students.drop_out_date = '"+this.__dropYear.Text+"' and drop_out_students.level_number = '"+this.__level.Text+"'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridDrop.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridDrop.RowTemplate.Height = 20;
                dataGridDrop.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridDrop.DataSource = table;
            }
            else
            {
                MessageBox.Show("Invalid filter parameters!");
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

        private void streamsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Streams st = new Streams();
            st.Show();
            this.Close();
        }

        private void instructorsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Departments dp = new Departments();
            dp.Show();
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
        private void __printButton_Click(object sender, EventArgs e)
        {

        }
    }
}
