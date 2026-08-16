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
    public partial class Levels : Form
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
        public Levels()
        {
            InitializeComponent();
            //---Initialize dataGridLevels---
            db = new DBConnect();
            conn = db.getConnection();
            string tableQuery = "Select stream_id, level_id, level, occupational_name, no_of_courses From ecc_dof_wukrostmarycollege.levels";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridLevels.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridLevels.RowTemplate.Height = 20;
            dataGridLevels.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridLevels.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            int checker = 0;
            string checkQuery = "Select level_id From ecc_dof_wukrostmarycollege.levels where level_id='" + this.levelID.Text + "'";
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
                if (streamID.Text != "" && levelID.Text != "" && occName.Text != "" && noCourses.Text != "" && level.Text != "")
                {
                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.levels (stream_id, level_id, level, occupational_name, no_of_courses) values('" + this.streamID.Text + "', '" + this.levelID.Text + "', '" + this.level.Text + "', '"+ this.occName.Text+"' ,'" + this.noCourses.Text + "');";
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

            //---Initialize dataGridLevels---
            string tableQuery = "Select stream_id, level_id, level, occupational_name, no_of_courses From ecc_dof_wukrostmarycollege.levels";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridLevels.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridLevels.RowTemplate.Height = 20;
            dataGridLevels.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridLevels.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (levelID.Text == cellValue)
            {
                string updateQuery = "Update ecc_dof_wukrostmarycollege.levels Set stream_id = '" + this.streamID.Text + "', level = '" + this.level.Text + "', occupational_name = '" + this.occName.Text + "', no_of_courses = '"+ this.noCourses.Text+"' Where level_id = '" + cellValue + "';";
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

            //---Initialize dataGridLevels---
            string tableQuery = "Select stream_id, level_id, level, occupational_name, no_of_courses From ecc_dof_wukrostmarycollege.levels";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridLevels.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridLevels.RowTemplate.Height = 20;
            dataGridLevels.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridLevels.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.levels Where level_id = '" + cellValue + "';";
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

            //---Initialize dataGridLevels---
            string tableQuery = "Select stream_id, level_id, level, occupational_name, no_of_courses From ecc_dof_wukrostmarycollege.levels";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridLevels.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridLevels.RowTemplate.Height = 20;
            dataGridLevels.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridLevels.DataSource = table;
        }

        private void dataGridLevels_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridLevels.Rows[rowIndex];
            cellValue = row.Cells[1].Value.ToString();

            string strmID, occuName, lvl, numCourses;
            string retrieveQuery = "Select stream_id, level, occupational_name, no_of_courses From ecc_dof_wukrostmarycollege.levels Where level_id = '" + cellValue + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    strmID = dataReader["stream_id"].ToString();
                    lvl = dataReader["level"].ToString();
                    occuName = dataReader["occupational_name"].ToString();
                    numCourses = dataReader["no_of_courses"].ToString();

                    levelID.Text = cellValue;
                    streamID.Text = strmID;
                    level.Text = lvl;
                    occName.Text = occuName;
                    noCourses.Text = numCourses;
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
    }
}
