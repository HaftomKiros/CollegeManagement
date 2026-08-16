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
    public partial class StudFees : Form
    {
        string cellValue = null, cellValue2 = null, cellValue3 = null, cellValue4 = null;
        int rowIndex;
        static string connString = "server=localhost;database=ecc_dof_wukrostmarycollege;uid=root;pwd=";
        MySqlConnection conn = new MySqlConnection(connString);
        MySqlCommand sqlCmd;
        MySqlDataReader dataReader;
        MySqlDataAdapter dataAdapter;
        public StudFees()
        {
            InitializeComponent();
            //---Initialize dataGridFees---
            string tableQuery = "Select student_id, level, academic_year, month, amount, cash_receipt_voucher, remark From ecc_dof_wukrostmarycollege.student_registration_fee";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridFees.RowTemplate.Height = 20;
            dataGridFees.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridFees.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            int checker = 0;
            string checkQuery = "Select student_id, level, academic_year, month From ecc_dof_wukrostmarycollege.student_registration_fee Where student_id='" + this.studID.Text + "' and level='"+this.level.Text+"' and academic_year='"+this.academicYear.Text+"' and month='"+this.month.Text+"'";
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
                if (studID.Text != "" && level.Text != "" && academicYear.Text != "" && month.Text != "" && amount.Text != "" && crv.Text != "" && remark.Text != "")
                {
                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.student_registration_fee (student_id, level, academic_year, month, amount, cash_receipt_voucher, remark) values('" + this.studID.Text + "', '" + this.level.Text + "', '" + this.academicYear.Text + "' ,'" + this.month.Text + "', '"+this.amount.Text+"', '"+this.crv.Text+"', '"+this.remark.Text+"');";
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

            //---Refreshing dataGridFees---
            string tableQuery = "Select student_id, level, academic_year, month, amount, cash_receipt_voucher, remark From ecc_dof_wukrostmarycollege.student_registration_fee";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridFees.RowTemplate.Height = 20;
            dataGridFees.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridFees.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (studID.Text == cellValue && level.Text == cellValue2 && academicYear.Text == cellValue3 && month.Text == cellValue4)
            {
                string updateQuery = "Update ecc_dof_wukrostmarycollege.student_registration_fee Set amount = '" + this.amount.Text + "', cash_receipt_voucher = '" + this.crv.Text + "', remark = '" + this.remark.Text + "' Where student_id = '" + cellValue + "' and level = '"+cellValue2+"' and academic_year = '"+cellValue3+"' and month = '"+cellValue4+"';";
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

            //---Initialize dataGridFees---
            string tableQuery = "Select student_id, level, academic_year, month, amount, cash_receipt_voucher, remark From ecc_dof_wukrostmarycollege.student_registration_fee";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridFees.RowTemplate.Height = 20;
            dataGridFees.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridFees.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.student_registration_fee Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "' and academic_year = '" + cellValue3 + "' and month = '" + cellValue4 + "';";
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

            //---Refreshing dataGridFees---
            string tableQuery = "Select student_id, level, academic_year, month, amount, cash_receipt_voucher, remark From ecc_dof_wukrostmarycollege.student_registration_fee";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridFees.RowTemplate.Height = 20;
            dataGridFees.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridFees.DataSource = table;
        }

        //---Menu
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
        private void dataGridFees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridFees.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();
            cellValue2 = row.Cells[1].Value.ToString();
            cellValue3 = row.Cells[2].Value.ToString();
            cellValue4 = row.Cells[3].Value.ToString();

            string amnt, receipt, rmrk;
            string retrieveQuery = "Select amount, cash_receipt_voucher, remark From ecc_dof_wukrostmarycollege.student_registration_fee Where student_id = '" + cellValue + "' and level = '"+cellValue2+"' and academic_year = '"+cellValue3+"' and month = '"+cellValue4+"'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    amnt = dataReader["amount"].ToString();
                    receipt = dataReader["cash_receipt_voucher"].ToString();
                    rmrk = dataReader["remark"].ToString();

                    studID.Text = cellValue;
                    level.Text = cellValue2;
                    academicYear.Text = cellValue3;
                    month.Text = cellValue4;
                    amount.Text = amnt;
                    crv.Text = receipt;
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
            if (__studID.Text != "")
            {
                string tableQuery = "Select student_registration_fee.student_id, student_registration_fee.level, student_registration_fee.academic_year, student_registration_fee.month, student_registration_fee.amount, student_registration_fee.cash_receipt_voucher, student_registration_fee.remark From ecc_dof_wukrostmarycollege.student_registration_fee Where student_registration_fee.student_id = '" + this.__studID.Text + "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridFees.RowTemplate.Height = 20;
                dataGridFees.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridFees.DataSource = table;
            }
            else if (__deptID.Text != "" && __academicYear.Text == "" && __level.Text == "" && __month.Text == "")
            {
                string tableQuery = "Select student_registration_fee.student_id, student_registration_fee.level, student_registration_fee.academic_year, student_registration_fee.month, student_registration_fee.amount, student_registration_fee.cash_receipt_voucher, student_registration_fee.remark From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.student_registration_fee Where departments.dept_id = '" + this.__deptID.Text + "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridFees.RowTemplate.Height = 20;
                dataGridFees.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridFees.DataSource = table;
            }
            else if (__deptID.Text != "" && __academicYear.Text != "" && __level.Text == "" && __month.Text == "")
            {
                string tableQuery = "Select student_registration_fee.student_id, student_registration_fee.level, student_registration_fee.academic_year, student_registration_fee.month, student_registration_fee.amount, student_registration_fee.cash_receipt_voucher, student_registration_fee.remark From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.student_registration_fee Where departments.dept_id = '" + this.__deptID.Text + "' and student_registration_fee = '"+this.__academicYear.Text+"'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridFees.RowTemplate.Height = 20;
                dataGridFees.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridFees.DataSource = table;
            }
            else if (__deptID.Text != "" && __academicYear.Text != "" && __level.Text != "" && __month.Text == "")
            {
                string tableQuery = "Select student_registration_fee.student_id, student_registration_fee.level, student_registration_fee.academic_year, student_registration_fee.month, student_registration_fee.amount, student_registration_fee.cash_receipt_voucher, student_registration_fee.remark From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.student_registration_fee Where departments.dept_id = '" + this.__deptID.Text + "' and student_registration_fee.academic_year = '" + this.__academicYear.Text + "' and student_registration_fee.level = '"+this.__level.Text+ "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridFees.RowTemplate.Height = 20;
                dataGridFees.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridFees.DataSource = table;
            }
            else if (__deptID.Text != "" && __academicYear.Text != "" && __level.Text != "" && __month.Text != "")
            {
                string tableQuery = "Select student_registration_fee.student_id, student_registration_fee.level, student_registration_fee.academic_year, student_registration_fee.month, student_registration_fee.amount, student_registration_fee.cash_receipt_voucher, student_registration_fee.remark From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.student_registration_fee Where departments.dept_id = '" + this.__deptID.Text + "' and student_registration_fee.academic_year = '" + this.__academicYear.Text + "' and student_registration_fee.level = '" + this.__level.Text + "' and student_registration_fee.month = '"+this.__month.Text+"'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridFees.RowTemplate.Height = 20;
                dataGridFees.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridFees.DataSource = table;
            }
            else
            {
                MessageBox.Show("Invalid filter parameters!");
            }
        }

        private void __printButton_Click(object sender, EventArgs e)
        {

        }
    }
}
