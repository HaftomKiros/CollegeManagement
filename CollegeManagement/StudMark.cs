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
    public partial class StudMark : Form
    {
        string cellValue = null, cellValue2 = null, cellValue3 = null;
        int rowIndex;
        static string connString = "server=localhost;database=ecc_dof_wukrostmarycollege;uid=root;pwd=";
        MySqlConnection conn = new MySqlConnection(connString);
        MySqlCommand sqlCmd;
        MySqlDataReader dataReader;
        MySqlDataAdapter dataAdapter;
        public StudMark()
        {
            InitializeComponent();
            //---Initializing dataGridMarks---
            string tableQuery = "Select student_id, level, module_code, employee_id, academic_year, score_of_knowledge_test, score_of_practical_test, competence From ecc_dof_wukrostmarycollege.student_mark";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridMarks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridMarks.RowTemplate.Height = 20;
            dataGridMarks.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridMarks.DataSource = table;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            int checker = 0;
            string checkQuery = "Select student_id, level, module_code From ecc_dof_wukrostmarycollege.student_mark Where student_id='" + this.studID.Text + "' and level='" + this.level.Text + "' and academic_year='" + this.academicYear.Text + "'";
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
                MessageBox.Show("Connection failed!" + ex.Message);
            }

            if (checker == 0)
            {
                if (studID.Text != "" && level.Text != "" && academicYear.Text != "" && instID.Text != "" && modCod.Text != "" && pracTest.Text != "" && knowTest.Text != "")
                {
                    int _knowTest = Int32.Parse(knowTest.Text);
                    int _pracTest = Int32.Parse(pracTest.Text);
                    if(_knowTest >= 51 && _knowTest <= 100 && _pracTest >= 90 && _pracTest <= 100)
                    {
                        string competence = "Competent";
                        string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.student_mark (student_id, level, module_code, employee_id, academic_year, score_of_knowledge_test, score_of_practical_test, competence) values('" + this.studID.Text + "', '" + this.level.Text + "', '" + this.modCod.Text + "' ,'" + this.instID.Text + "', '" + this.academicYear.Text + "', '" + this.knowTest.Text + "', '" + this.pracTest.Text + "', '"+competence+"');";
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
                            MessageBox.Show("Connection failed!" + ex.Message);
                            conn.Close();
                        }
                    }
                    else
                    {
                        string competence = "Not competent";
                        string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.student_mark (student_id, level, module_code, employee_id, academic_year, score_of_knowledge_test, score_of_practical_test, competence) values('" + this.studID.Text + "', '" + this.level.Text + "', '" + this.modCod.Text + "' ,'" + this.instID.Text + "', '" + this.academicYear.Text + "', '" + this.knowTest.Text + "', '" + this.pracTest.Text + "', '"+competence+"');";
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
                            MessageBox.Show("Connection failed!" + ex.Message);
                            conn.Close();
                        }
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

            //---Refreshing dataGridMarks---
            string tableQuery = "Select student_id, level, module_code, employee_id, academic_year, score_of_knowledge_test, score_of_practical_test, competence From ecc_dof_wukrostmarycollege.student_mark";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridMarks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridMarks.RowTemplate.Height = 20;
            dataGridMarks.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridMarks.DataSource = table;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (studID.Text == cellValue && level.Text == cellValue2 && modCod.Text == cellValue3)
            {
                int _knowTest = Int32.Parse(knowTest.Text);
                int _pracTest = Int32.Parse(pracTest.Text);
                if (_knowTest >= 51 && _knowTest <= 100 && _pracTest >= 90 && _pracTest <= 100)
                {
                    string competence = "Competent";
                    string updateQuery = "Update ecc_dof_wukrostmarycollege.student_mark Set employee_id = '" + this.instID.Text + "', academic_year = '" + this.academicYear.Text + "', score_of_knowledge_test = '" + this.knowTest.Text + "', score_of_practical_test = '" + this.pracTest.Text + "', competence = '"+competence+"' Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "' and module_code = '" + cellValue3 + "';";
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
                        MessageBox.Show("Connection failed!" + ex.Message);
                        conn.Close();
                    }
                }
                else
                {
                    string competence = "Not competent";
                    string updateQuery = "Update ecc_dof_wukrostmarycollege.student_mark Set employee_id = '" + this.instID.Text + "', academic_year = '" + this.academicYear.Text + "', score_of_knowledge_test = '" + this.knowTest.Text + "', score_of_practical_test = '" + this.pracTest.Text + "', competence = '" + competence + "' Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "' and module_code = '" + cellValue3 + "';";
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
            }
            else
            {
                MessageBox.Show("Update attempt failed!");
            }

            //---Refreshing dataGridMarks---
            string tableQuery = "Select student_id, level, module_code, employee_id, academic_year, score_of_knowledge_test, score_of_practical_test, competence From ecc_dof_wukrostmarycollege.student_mark";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridMarks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridMarks.RowTemplate.Height = 20;
            dataGridMarks.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridMarks.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.student_mark Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "' and module_code = '" + cellValue3 + "';";
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

            //---Refreshing dataGridMarks---
            string tableQuery = "Select student_id, level, module_code, employee_id, academic_year, score_of_knowledge_test, score_of_practical_test, competence From ecc_dof_wukrostmarycollege.student_mark";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridMarks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridMarks.RowTemplate.Height = 20;
            dataGridMarks.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridMarks.DataSource = table;
        }

        private void dataGridMarks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridMarks.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();
            cellValue2 = row.Cells[1].Value.ToString();
            cellValue3 = row.Cells[2].Value.ToString();

            string insID, acdYr, knwTest, prcTest, cmptnc;
            string retrieveQuery = "Select employee_id, academic_year, score_of_knowledge_test, score_of_practical_test From ecc_dof_wukrostmarycollege.student_mark Where student_id = '" + cellValue + "' and level = '" + cellValue2 + "' and module_code = '" + cellValue3 + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    insID = dataReader["employee_id"].ToString();
                    acdYr = dataReader["academic_year"].ToString();
                    knwTest = dataReader["score_of_knowledge_test"].ToString();
                    prcTest = dataReader["score_of_practical_test"].ToString();

                    studID.Text = cellValue;
                    level.Text = cellValue2;
                    modCod.Text = cellValue3;
                    instID.Text = insID;
                    academicYear.Text = acdYr;
                    knowTest.Text = knwTest;
                    pracTest.Text = prcTest;
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

        private void attachMarkListButton_Click(object sender, EventArgs e)
        {
            MarkListDoc sm = new MarkListDoc();
            sm.Show();
            this.Close();
        }

        //---End---

        private void __filterButton_Click(object sender, EventArgs e)
        {
            if(__studID.Text != "")
            {
                string tableQuery = "Select student_mark.student_id, student_mark.level, student_mark.module_code, student_mark.employee_id, student_mark.academic_year, student_mark.score_of_knowledge_test, student_mark.score_of_practical_test, student_mark.competence From ecc_dof_wukrostmarycollege.student_mark Where student_mark.student_id = '" + this.__studID.Text + "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridMarks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridMarks.RowTemplate.Height = 20;
                dataGridMarks.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridMarks.DataSource = table;
            }
            else if (__deptID.Text != "" && __year.Text != "" && __level.Text != "" && __modCod.Text != "")
            {
                string tableQuery = "Select student_mark.student_id, student_mark.level, student_mark.module_code, student_mark.employee_id, student_mark.academic_year, student_mark.score_of_knowledge_test, student_mark.score_of_practical_test, student_mark.competence From ecc_dof_wukrostmarycollege.departments, ecc_dof_wukrostmarycollege.student_mark Where departments.dept_id = '" + this.__deptID.Text + "' and student_mark.academic_year = '" + this.__year.Text+"' and student_mark.level = '"+this.__level.Text+"' and student_mark.module_code = '"+this.__modCod.Text+"'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridMarks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridMarks.RowTemplate.Height = 20;
                dataGridMarks.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridMarks.DataSource = table;
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
