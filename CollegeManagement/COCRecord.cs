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
    public partial class COCRecord : Form
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
        public COCRecord()
        {
            InitializeComponent();
            //---Initialize dataGridCOCRecord---
            db = new DBConnect();
            conn = db.getConnection();
            string tableQuery = "Select student_id, level, assessment_date, assessor_name, supervisor_name, competence, coc_level_id From ecc_dof_wukrostmarycollege.coc";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridCOCRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridCOCRecord.RowTemplate.Height = 20;
            dataGridCOCRecord.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridCOCRecord.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            int checker = 0;
            string checkQuery = "Select student_id, level From ecc_dof_wukrostmarycollege.coc Where student_id='" + this.studID.Text + "' and level='" + this.level.Text + "'";
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
                if (studID.Text != "" && level.Text != "" && assDate.Text != "" && assName.Text != "" && SupName.Text != "" && competence.Text != "")
                {
                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.coc (student_id, level, assessment_date, assessor_name, supervisor_name, competence, coc_level_id) values('" + this.studID.Text + "', '" + this.level.Text + "', '" + this.assDate.Text + "' ,'" + this.assName.Text + "', '" + this.SupName.Text + "', '" + this.competence.Text + "', '"+this.cocID.Text+"');";
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

            //---Refreshing dataGridCOCRecord---
            string tableQuery = "Select student_id, level, assessment_date, assessor_name, supervisor_name, competence, coc_level_id From ecc_dof_wukrostmarycollege.coc";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridCOCRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridCOCRecord.RowTemplate.Height = 20;
            dataGridCOCRecord.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridCOCRecord.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (studID.Text == cellValue && level.Text == cellValue2)
            {
                string updateQuery = "Update ecc_dof_wukrostmarycollege.coc Set assessment_date = '" + this.assDate.Text + "', assessor_name = '" + this.assName.Text + "', supervisor_name = '" + this.SupName.Text + "', competence = '"+this.competence.Text+"', coc_level_id = '"+this.cocID.Text+"' Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "';";
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

            //---Refreshing dataGridCOCRecord---
            string tableQuery = "Select student_id, level, assessment_date, assessor_name, supervisor_name, competence, coc_level_id From ecc_dof_wukrostmarycollege.coc";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridCOCRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridCOCRecord.RowTemplate.Height = 20;
            dataGridCOCRecord.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridCOCRecord.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.coc Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "';";
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

            //---Refreshing dataGridCOCRecord---
            string tableQuery = "Select student_id, level, assessment_date, assessor_name, supervisor_name, competence, coc_level_id From ecc_dof_wukrostmarycollege.coc";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridCOCRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridCOCRecord.RowTemplate.Height = 20;
            dataGridCOCRecord.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridCOCRecord.DataSource = table;
        }

        private void dataGridCOCRecord_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridCOCRecord.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();
            cellValue2 = row.Cells[1].Value.ToString();


            string assDte, assNme, supNme, cmptnc, ccID;
            string retrieveQuery = "Select assessment_date, assessor_name, supervisor_name, competence, coc_level_id From ecc_dof_wukrostmarycollege.coc Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    assDte = dataReader["assessment_date"].ToString();
                    assNme = dataReader["assessor_name"].ToString();
                    supNme = dataReader["supervisor_name"].ToString();
                    cmptnc = dataReader["competence"].ToString();
                    ccID = dataReader["coc_level_id"].ToString();

                    studID.Text = cellValue;
                    level.Text = cellValue2;
                    assDate.Text = assDte;
                    assName.Text = assNme;
                    SupName.Text = supNme;
                    competence.Text = cmptnc;
                    cocID.Text = ccID;
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

            if (__studID.Text != "" && __deptID.Text == "" && __streamID.Text == "" && __level.Text == "" && __competence.Text != "")
            {
                string tableQuery = "Select coc.student_id, coc.level, coc.assessment_date, coc.assessor_name, coc.supervisor_name, coc.competence, coc.coc_level_id From ecc_dof_wukrostmarycollege.coc Where coc.student_id = '" + this.__studID.Text + "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridCOCRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridCOCRecord.RowTemplate.Height = 20;
                dataGridCOCRecord.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridCOCRecord.DataSource = table;
            }
            else if (__studID.Text == "" && __deptID.Text != "" && __streamID.Text != "" && __level.Text != "" && __competence.Text != "")
            {
                string tableQuery = "Select coc.student_id, coc.level, coc.assessment_date, coc.assessor_name, coc.supervisor_name, coc.competence, coc.coc_level_id From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.streams, ecc_dof_wukrostmarycollege.coc Where departments.dept_id = '" + this.__deptID.Text + "' and streams.stream_id = '"+this.__streamID.Text+"' and coc.level = '" + this.__level.Text + "' and coc.competence = '" + this.__competence.Text + "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridCOCRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridCOCRecord.RowTemplate.Height = 20;
                dataGridCOCRecord.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridCOCRecord.DataSource = table;
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
        private void __printButton_Click(object sender, EventArgs e)
        {

        }
    }
}
