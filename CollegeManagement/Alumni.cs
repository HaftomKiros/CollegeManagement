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
    public partial class Alumni : Form
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
        public Alumni()
        {
            InitializeComponent();
            //---Initialize dataGridAlumni---
            db = new DBConnect();
            conn = db.getConnection();
            string tableQuery = "Select alumni_id, student_id, graduated_year, employment_status, employed_office, home_address, mobile_number, current_educational_status From ecc_dof_wukrostmarycollege.alumni";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridAlumni.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridAlumni.RowTemplate.Height = 20;
            dataGridAlumni.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridAlumni.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            int checker = 0;
            string checkQuery = "Select alumni_id From ecc_dof_wukrostmarycollege.alumni where alumni_id='" + this.alumniID.Text + "'";
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
                if (alumniID.Text != "" && studID.Text != "" && gradYear.Text != "" && empStatus.Text != "" && empOffice.Text != "" && mobNum.Text != "" && homeAddress.Text != "" && eduStatus.Text != "")
                {
                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.alumni (alumni_id, student_id, graduated_year, employment_status, employed_office, home_address, mobile_number, current_educational_status) values('" + this.alumniID.Text + "', '" + this.studID.Text + "', '" + this.gradYear.Text + "', '" + this.empStatus.Text + "', '" + this.empOffice.Text + "', '" + this.homeAddress.Text + "', '" + this.mobNum.Text + "', '" + this.eduStatus.Text + "');";
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

            //---Refreshing dataGridAlumni---
            string tableQuery = "Select alumni_id, student_id, graduated_year, employment_status, employed_office, home_address, mobile_number, current_educational_status From ecc_dof_wukrostmarycollege.alumni";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridAlumni.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridAlumni.RowTemplate.Height = 20;
            dataGridAlumni.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridAlumni.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (alumniID.Text == cellValue)
            {
                string updateQuery = "Update ecc_dof_wukrostmarycollege.alumni Set student_id = '" + this.studID.Text + "', graduated_year = '" + this.gradYear.Text + "', employment_status = '" + this.empStatus.Text + "', employed_office = '"+this.empOffice.Text+"', home_address = '"+this.homeAddress.Text+"', mobile_number = '"+this.mobNum.Text+"', current_educational_status = '"+this.eduStatus.Text+"' Where alumni_id = '" + cellValue + "';";
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

            //---Refreshing dataGridAlumni---
            string tableQuery = "Select alumni_id, student_id, graduated_year, employment_status, employed_office, home_address, mobile_number, current_educational_status From ecc_dof_wukrostmarycollege.alumni";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridAlumni.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridAlumni.RowTemplate.Height = 20;
            dataGridAlumni.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridAlumni.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.alumni Where alumni_id = '" + cellValue + "';";
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

            //---Refreshing dataGridAlumni---
            string tableQuery = "Select alumni_id, student_id, graduated_year, employment_status, employed_office, home_address, mobile_number, current_educational_status From ecc_dof_wukrostmarycollege.alumni";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridAlumni.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridAlumni.RowTemplate.Height = 20;
            dataGridAlumni.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridAlumni.DataSource = table;
        }

        private void dataGridAlumni_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridAlumni.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();

            string stdID, gradYr, empStts, empOffc, homeAddrs, mobNumbr, eduStts;
            string retrieveQuery = "Select student_id, graduated_year, employment_status, employed_office, home_address, mobile_number, current_educational_status From ecc_dof_wukrostmarycollege.alumni Where alumni_id = '" + cellValue + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    stdID = dataReader["student_id"].ToString();
                    gradYr = dataReader["graduated_year"].ToString();
                    empStts = dataReader["employment_status"].ToString();
                    empOffc = dataReader["employed_office"].ToString();
                    homeAddrs = dataReader["home_address"].ToString();
                    mobNumbr = dataReader["mobile_number"].ToString();
                    eduStts = dataReader["current_educational_status"].ToString();

                    alumniID.Text = cellValue;
                    studID.Text = stdID;
                    gradYear.Text = gradYr;
                    empStatus.Text = empStts;
                    empOffice.Text = empOffc;
                    homeAddress.Text = homeAddrs;
                    mobNum.Text = mobNumbr;
                    eduStatus.Text = eduStts;
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

            if (__alumniID.Text != "" && __deptID.Text == "" && __streamID.Text == "" && __gradYear.Text == "" && __empStatus.Text != "")
            {
                //---Filtering dataGridAlumni---
                string tableQuery = "Select alumni_id, student_id, graduated_year, employment_status, employed_office, home_address, mobile_number, current_educational_status From ecc_dof_wukrostmarycollege.alumni Where alumni_id = '"+this.__alumniID.Text+"'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridAlumni.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridAlumni.RowTemplate.Height = 20;
                dataGridAlumni.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridAlumni.DataSource = table;
            }
            else if (__alumniID.Text == "" && __deptID.Text != "" && __streamID.Text != "" && __gradYear.Text != "" && __empStatus.Text != "")
            {
                //---Filtering dataGridAlumni---
                string tableQuery = "Select alumni.alumni_id, alumni.student_id, alumni.graduated_year, alumni.employment_status, alumni.employed_office, alumni.home_address, alumni.mobile_number, alumni.current_educational_status From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.streams, ecc_dof_wukrostmarycollege.alumni Where departments.dept_id = '"+this.__deptID.Text+"' and streams.stream_id = '"+this.__streamID.Text+"' and alumni.graduated_year = '" + this.__gradYear.Text + "' and alumni.employment_status = '"+this.__empStatus.Text+"'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridAlumni.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridAlumni.RowTemplate.Height = 20;
                dataGridAlumni.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridAlumni.DataSource = table;
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
