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
    public partial class Streams : Form
    {
        string cellValue = null;
        int rowIndex;
        static string connString = "server=localhost;database=ecc_dof_wukrostmarycollege;uid=root;pwd=";
        MySqlConnection conn = new MySqlConnection(connString);
        MySqlCommand sqlCmd;
        MySqlDataReader dataReader;
        MySqlDataAdapter dataAdapter;
        public Streams()
        {
            InitializeComponent();
            //---Initialize dataGridStreams---
            string tableQuery = "Select dept_id, stream_id, stream_name, no_of_levels From ecc_dof_wukrostmarycollege.streams";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridStreams.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridStreams.RowTemplate.Height = 20;
            dataGridStreams.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridStreams.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            int checker = 0;
            string checkQuery = "Select stream_id From ecc_dof_wukrostmarycollege.streams where dept_id='" + this.streamID.Text + "'";
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
                if (deptID.Text != "" && streamID.Text != "" && streamName.Text != "" && noLevels.Text != "")
                {
                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.streams (dept_id, stream_id, stream_name, no_of_levels) values('" + this.deptID.Text + "', '" + this.streamID.Text + "', '" + this.streamName.Text + "', '" + this.noLevels.Text + "');";
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

            //---Updata dataGridStreams---
            string tableQuery = "Select dept_id, stream_id, stream_name, no_of_levels From ecc_dof_wukrostmarycollege.streams";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridStreams.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridStreams.RowTemplate.Height = 20;
            dataGridStreams.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridStreams.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if(streamID.Text == cellValue)
            {
                string updateQuery = "Update ecc_dof_wukrostmarycollege.streams Set dept_id = '" + this.deptID.Text + "', stream_name = '" + this.streamName.Text + "', no_of_levels = '" + this.noLevels.Text + "' Where stream_id = '" + cellValue + "';";
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

                //---Updata dataGridStreams---
                string tableQuery = "Select dept_id, stream_id, stream_name, no_of_levels From ecc_dof_wukrostmarycollege.streams";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridStreams.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridStreams.RowTemplate.Height = 20;
                dataGridStreams.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridStreams.DataSource = table;
            }
            else
            {
                MessageBox.Show("Update attempt failed!");
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.streams Where stream_id = '" + cellValue + "';";
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

            //---Updata dataGridStreams---
            string tableQuery = "Select dept_id, stream_id, stream_name, no_of_levels From ecc_dof_wukrostmarycollege.streams";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridStreams.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridStreams.RowTemplate.Height = 20;
            dataGridStreams.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridStreams.DataSource = table;
        }

        private void dataGridStreams_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridStreams.Rows[rowIndex];
            cellValue = row.Cells[1].Value.ToString();

            string depID, strmName, numLevels;
            string retrieveQuery = "Select dept_id, stream_name, no_of_levels From ecc_dof_wukrostmarycollege.streams Where stream_id = '" + cellValue + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    depID = dataReader["dept_id"].ToString();
                    strmName = dataReader["stream_name"].ToString();
                    numLevels = dataReader["no_of_levels"].ToString();

                    deptID.Text = depID;
                    streamID.Text = cellValue;
                    streamName.Text = strmName;
                    noLevels.Text = numLevels;
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
