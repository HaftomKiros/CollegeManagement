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
    public partial class Admins : Form
    {
        string cellValue = null;
        int rowIndex;
        private DBConnect db;
        MySqlConnection conn = null;
        MySqlCommand sqlCmd;
        MySqlDataReader dataReader;
        MySqlDataAdapter dataAdapter;

        //string cellValue = null;
        //int rowIndex;
        //static string connString = "server=localhost;database=ecc_dof_wukrostmarycollege;uid=root;pwd=";
        //MySqlConnection conn = new MySqlConnection(connString);
        //MySqlCommand sqlCmd;
        //MySqlDataReader dataReader;
        //MySqlDataAdapter dataAdapter;
        public Admins()
        {
            InitializeComponent();
            //---Initialize dataGridAdmins---
            db = new DBConnect();
            conn = db.getConnection();
            string tableQuery = "Select admin_id, user_name, password, priority From ecc_dof_wukrostmarycollege.admins";            
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridAdmins.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridAdmins.RowTemplate.Height = 20;
            dataGridAdmins.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridAdmins.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (uName.Text != "" && priority.Text != "" && password.Text != "" && rePassword.Text != "")
            {
                if(password.Text == rePassword.Text)
                {
                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.admins (user_name, password, priority) values('" + this.uName.Text + "', '" + this.password.Text + "', '" + this.priority.Text + "');";
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
                    MessageBox.Show("Please re-enter the same password!");
                }
            }
            else
            {
                MessageBox.Show("There is empty field(s). Please fill all fields!");
            }

            //---Refreshing dataGridAdmins---
            string tableQuery = "Select admin_id, user_name, password, priority From ecc_dof_wukrostmarycollege.admins";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridAdmins.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridAdmins.RowTemplate.Height = 20;
            dataGridAdmins.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridAdmins.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();
            if (uName.Text != "" && password.Text != "" && rePassword.Text != "" && priority.Text != "")
            {
                if (password.Text == rePassword.Text)
                {
                    string updateQuery = "Update ecc_dof_wukrostmarycollege.admins Set user_name = '" + this.uName.Text + "', password = '" + this.password.Text + "', priority = '" + this.priority.Text + "' Where admin_id = '" + cellValue + "';";
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
                    MessageBox.Show("Update failed!");
                }
            }
            else
            {
                MessageBox.Show("Empty field!");
            }

            //---Refreshing dataGridAdmins---
            string tableQuery = "Select admin_id, user_name, password, priority From ecc_dof_wukrostmarycollege.admins";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridAdmins.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridAdmins.RowTemplate.Height = 20;
            dataGridAdmins.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridAdmins.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();
            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.admins Where admin_id = '" + cellValue + "';";
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

            //---Refreshing dataGridAdmins---
            string tableQuery = "Select admin_id, user_name, password, priority From ecc_dof_wukrostmarycollege.admins";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridAdmins.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridAdmins.RowTemplate.Height = 20;
            dataGridAdmins.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridAdmins.DataSource = table;
        }

        private void dataGridAdmins_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridAdmins.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();

            string uNme, prty, psswd;
            string retrieveQuery = "Select user_name, password, priority From ecc_dof_wukrostmarycollege.admins Where admin_id = '" + cellValue + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    uNme = dataReader["user_name"].ToString();
                    psswd = dataReader["password"].ToString();
                    prty = dataReader["priority"].ToString();

                    uName.Text = uNme;
                    password.Text = psswd;
                    rePassword.Text = psswd;
                    priority.Text = prty;
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
