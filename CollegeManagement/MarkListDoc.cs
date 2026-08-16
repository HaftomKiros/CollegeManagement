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
    public partial class MarkListDoc : Form
    {
        string cellValue = null, cellValue2 = null, cellValue3 = null, cellValue4 = null, cellValue5 = null, cellValue6 = null;
        int rowIndex;
        static string connString = "server=localhost;database=ecc_dof_wukrostmarycollege;uid=root;pwd=";
        MySqlConnection conn = new MySqlConnection(connString);
        MySqlCommand sqlCmd;
        MySqlDataReader dataReader;
        MySqlDataAdapter dataAdapter;
        public MarkListDoc()
        {
            InitializeComponent();
            //---Initializing dataGridMarks---
            string tableQuery = "Select doc_dept_id, doc_stream_id, doc_level_id, doc_module_code, doc_academic_year, doc_admission_type From ecc_dof_wukrostmarycollege.mark_list_docs";
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
            string checkQuery = "Select doc_dept_id, doc_stream_id, doc_level_id, doc_module_code, doc_academic_year, doc_admission_type From ecc_dof_wukrostmarycollege.mark_list_docs Where doc_dept_id='" + this.deptID.Text + "' and doc_stream_id='" + this.streamID.Text + "' and doc_level_id='" + this.levelID.Text + "' and doc_module_code = '"+this.modCod.Text+"' and doc_academic_year = '"+this.academicYear.Text+"' and doc_admission_type = '"+this.admissionType.Text+"'";
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

            if(checker == 0)
            {
                if (deptID.Text != "" && streamID.Text != "" && levelID.Text != "" && modCod.Text != "" && academicYear.Text != "" && admissionType.Text != "" && filePath.Text != "")
                {
                    byte[] attachByte = null;
                    FileStream fstream = new FileStream(this.filePath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr = new BinaryReader(fstream);
                    attachByte = binRdr.ReadBytes((int)fstream.Length);

                    string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.mark_list_docs (doc_dept_id, doc_stream_id, doc_level_id, doc_module_code, doc_academic_year, doc_admission_type, doc_file) values('" + this.deptID.Text + "', '" + this.streamID.Text + "','" + this.levelID.Text + "', '" + this.modCod.Text + "', '" + this.academicYear.Text + "', '"+this.admissionType.Text+"', @ATTCH);";
                    sqlCmd = new MySqlCommand(insertQuery, conn);
                    try
                    {
                        conn.Open();
                        sqlCmd.Parameters.Add(new MySqlParameter("@ATTCH", attachByte));
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
                    MessageBox.Show("Error. Please fill in all fields!");
                }
            }
            else
            {
                MessageBox.Show("Error. This mark list is already attached!");
            }

            //---Refreshing dataGridMarks---
            string tableQuery = "Select doc_dept_id, doc_stream_id, doc_level_id, doc_module_code, doc_academic_year, doc_admission_type From ecc_dof_wukrostmarycollege.mark_list_docs";
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
            if (deptID.Text == cellValue && streamID.Text == cellValue2 && levelID.Text == cellValue3 && modCod.Text == cellValue4 && academicYear.Text == cellValue5 && admissionType.Text == cellValue6)
            {
                if (filePath.Text != "")
                {
                    byte[] attachByte = null;
                    FileStream fstream = new FileStream(this.filePath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr = new BinaryReader(fstream);
                    attachByte = binRdr.ReadBytes((int)fstream.Length);

                    string updateQuery = "Update ecc_dof_wukrostmarycollege.mark_list_docs Set doc_file = @ATTCH  Where doc_dept_id = '" + cellValue + "' and doc_stream_id = '" + cellValue2 + "' and doc_level_id = '" + cellValue3 + "' and doc_module_code = '" + cellValue4 + "' and doc_academic_year = '" + cellValue5 + "' and doc_admission_type = '" + cellValue6 + "';";
                    sqlCmd = new MySqlCommand(updateQuery, conn);
                    try
                    {
                        conn.Open();
                        sqlCmd.Parameters.Add(new MySqlParameter("@ATTCH", attachByte));
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
                    MessageBox.Show("Error. Wrong update attempt!");
                }
            }
            else
            {
                MessageBox.Show("Error. Update attempt failed!");
            }

            //---Refreshing dataGridMarks---
            string tableQuery = "Select doc_dept_id, doc_stream_id, doc_level_id, doc_module_code, doc_academic_year, doc_admission_type From ecc_dof_wukrostmarycollege.mark_list_docs";
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
            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.mark_list_docs Where doc_dept_id = '" + cellValue + "' and doc_stream_id = '" + cellValue2 + "' and doc_level_id = '" + cellValue3 + "' and doc_module_code = '" + cellValue4 + "' and doc_academic_year = '" + cellValue5 + "' and doc_admission_type = '" + cellValue6 + "';";
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
            string tableQuery = "Select doc_dept_id, doc_stream_id, doc_level_id, doc_module_code, doc_academic_year, doc_admission_type From ecc_dof_wukrostmarycollege.mark_list_docs";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridMarks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridMarks.RowTemplate.Height = 20;
            dataGridMarks.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridMarks.DataSource = table;
        }

        //---Menu---
        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

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
            Dropout dOut = new Dropout();
            dOut.Show();
            this.Close();
        }

        private void cOCRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            COCRecord cRecord = new COCRecord();
            cRecord.Show();
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

        private void libraryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Library lb = new Library();
            lb.Show();
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
            Help hp = new Help();
            hp.Show();
            this.Close();
        }

        //---End Menu---
        private void dataGridMarks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridMarks.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();
            cellValue2 = row.Cells[1].Value.ToString();
            cellValue3 = row.Cells[2].Value.ToString();
            cellValue4 = row.Cells[3].Value.ToString();
            cellValue5 = row.Cells[4].Value.ToString();
            cellValue6 = row.Cells[5].Value.ToString();

            string dptID, strmID, lvlID, mdCod, acdYear, admnType;
            string retrieveQuery = "Select doc_dept_id, doc_stream_id, doc_level_id, doc_module_code, doc_academic_year, doc_admission_type From ecc_dof_wukrostmarycollege.mark_list_docs Where doc_dept_id = '" + cellValue + "' and doc_stream_id = '" + cellValue2 + "' and doc_level_id = '" + cellValue3 + "' and doc_module_code = '" + cellValue4 + "' and doc_academic_year = '" + cellValue5 + "' and doc_admission_type = '" + cellValue6 + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    deptID.Text = cellValue;
                    streamID.Text = cellValue2;
                    levelID.Text = cellValue3;
                    modCod.Text = cellValue4;
                    academicYear.Text = cellValue5;
                    admissionType.Text = cellValue6;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed!");
                conn.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "PDF Files(*.pdf)|*.pdf|Word Files(*.docx)|*.docx|All Files(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string attachPath = dlg.FileName.ToString();
                filePath.Text = attachPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void __filterButton_Click(object sender, EventArgs e)
        {
            if(deptID.Text != "" && streamID.Text != "" && levelID.Text != "" && modCod.Text != "" && academicYear.Text != "")
            {
                string tableQuery = "Select doc_dept_id, doc_stream_id, doc_level_id, doc_module_code, doc_academic_year, doc_admission_type From ecc_dof_wukrostmarycollege.mark_list_docs Where doc_dept_id = '"+this.__deptID.Text+ "' and doc_stream_id = '" + this.__streamID.Text + "' and doc_level_id = '" + this.__levelID.Text + "' and doc_module_code = '" + this.__modCod.Text + "' and doc_academic_year = '" + this.__academicYear.Text + "' and doc_admission_type = '" + this.__admissionType.Text + "'";
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
                MessageBox.Show("Error. Wrong filter parameters!");
            }
        }

        private void __printButton_Click(object sender, EventArgs e)
        {

        }
    }
}
