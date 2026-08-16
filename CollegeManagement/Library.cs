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
    public partial class Library : Form
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
        public Library()
        {
            InitializeComponent();
            //Initializing dataGridBooks...
            db = new DBConnect();
            conn = db.getConnection();
            string tableQuery = "Select book_id, book_type, book_title, book_dept_id, book_stream_id, book_level_id, book_module_code From ecc_dof_wukrostmarycollege.library";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridBooks.RowTemplate.Height = 20;
            dataGridBooks.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridBooks.DataSource = table;
        }

        private void Library_Load(object sender, EventArgs e)
        {

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            int checker = 0;
            string checkQuery = "Select book_id From ecc_dof_wukrostmarycollege.library where book_id = '" + this.bookID.Text + "'";
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
                byte[] attachByte = null;
                FileStream fstream = new FileStream(this.bookFilePath.Text, FileMode.Open, FileAccess.Read);
                BinaryReader binRdr = new BinaryReader(fstream);
                attachByte = binRdr.ReadBytes((int)fstream.Length);

                if (bookType.Text == "Ref. Book")
                {
                    if (bookID.Text != "" && bookTitle.Text != "" && bookDeptID.Text != "" && bookStreamID.Text != "" && bookFilePath.Text != "")
                    {
                        string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.library (book_id, book_type, book_title, book_dept_id, book_stream_id, book_level_id, book_module_code, book_file) values('" + this.bookID.Text + "', '" + this.bookType.Text + "','" + this.bookTitle.Text + "', '" + this.bookDeptID.Text + "', '" + this.bookStreamID.Text + "', '"+this.bookLevelID.Text+"', '"+this.bookModCode.Text+"', @ATTCH);";
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
                            MessageBox.Show("Connection failed!" + ex.Message);
                            conn.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("There is empty field(s). Please fill all fields!");
                    }
                }
                else if(bookType.Text == "Instructor Handout")
                {
                    if (bookID.Text != "" && bookTitle.Text != "" && bookDeptID.Text != "" && bookStreamID.Text != "" && bookLevelID.Text != "" && bookModCode.Text != "" && bookFilePath.Text != "")
                    {
                        string insertQuery = "Insert Into ecc_dof_wukrostmarycollege.library (book_id, book_type, book_title, book_dept_id, book_stream_id, book_level_id, book_module_code, book_file) values('" + this.bookID.Text + "', '" + this.bookType.Text + "','" + this.bookTitle.Text + "', '" + this.bookDeptID.Text + "', '" + this.bookStreamID.Text + "', '" + this.bookLevelID.Text + "', '" + this.bookModCode.Text + "', @ATTCH);";
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
                            MessageBox.Show("Connection failed!" + ex.Message);
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
                    MessageBox.Show("Error!");
                }
            }
            else
            {
                MessageBox.Show("There is already an employee with the same ID!");
            }

            //Refreshing dataGridBooks...
            string tableQuery = "Select book_id, book_type, book_title, book_dept_id, book_stream_id, book_level_id, book_module_code From ecc_dof_wukrostmarycollege.library";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridBooks.RowTemplate.Height = 20;
            dataGridBooks.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridBooks.DataSource = table;
        }

        private void bookBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "PDF Files(*.pdf)|*.pdf|Word Files(*.docx)|*.docx|All Files(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string attachPath = dlg.FileName.ToString();
                bookFilePath.Text = attachPath;
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (bookID.Text == cellValue)
            {
                if (bookFilePath.Text != "")
                {
                    byte[] attachByte = null;
                    FileStream fstream = new FileStream(this.bookFilePath.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader binRdr = new BinaryReader(fstream);
                    attachByte = binRdr.ReadBytes((int)fstream.Length);

                    if(bookType.Text == "Ref. Book")
                    {
                        string updateQuery = "Update ecc_dof_wukrostmarycollege.library Set book_type = '" + this.bookType.Text + "', book_title = '" + this.bookTitle.Text + "', book_dept_id = '" + this.bookDeptID.Text + "', book_stream_id = '" + this.bookStreamID.Text + "', book_level_id = '"+this.bookLevelID.Text+"', book_module_code = '"+this.bookModCode.Text+"', book_file = @ATTCH Where book_id = '" + cellValue + "';";
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
                        if (bookLevelID.Text != "" && bookModCode.Text != "")
                        {
                            string updateQuery = "Update ecc_dof_wukrostmarycollege.library Set book_type = '" + this.bookType.Text + "', book_title = '" + this.bookTitle.Text + "', book_dept_id = '" + this.bookDeptID.Text + "', book_stream_id = '" + this.bookStreamID.Text + "', book_level_id = '"+this.bookLevelID.Text+"', book_module_code = '"+this.bookModCode.Text+"', book_file = @ATTCH Where book_id = '" + cellValue + "';";
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
                            MessageBox.Show("Please fill all fields!");
                        }
                    }
                }
                else
                {
                    if (bookType.Text == "Ref. Book")
                    {
                        string updateQuery = "Update ecc_dof_wukrostmarycollege.library Set book_type = '" + this.bookType.Text + "', book_title = '" + this.bookTitle.Text + "', book_dept_id = '" + this.bookDeptID.Text + "', book_stream_id = '" + this.bookStreamID.Text + "' Where book_id = '" + cellValue + "';";
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
                        if (bookLevelID.Text != "" && bookModCode.Text != "")
                        {
                            string updateQuery = "Update ecc_dof_wukrostmarycollege.library Set book_type = '" + this.bookType.Text + "', book_title = '" + this.bookTitle.Text + "', book_dept_id = '" + this.bookDeptID.Text + "', book_stream_id = '" + this.bookStreamID.Text + "', book_level_id = '" + this.bookLevelID.Text + "', book_module_code = '" + this.bookModCode.Text + "' Where book_id = '" + cellValue + "';";
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
                            MessageBox.Show("Please fill all fields!");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Update failed!");
            }

            //Refreshing dataGridBooks...
            string tableQuery = "Select book_id, book_type, book_title, book_dept_id, book_stream_id, book_level_id, book_module_code From ecc_dof_wukrostmarycollege.library";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridBooks.RowTemplate.Height = 20;
            dataGridBooks.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridBooks.DataSource = table;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            string deleteQuery = "Delete From ecc_dof_wukrostmarycollege.library Where book_id = '" + cellValue + "';";
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

            //Refreshing dataGridBooks...
            string tableQuery = "Select book_id, book_type, book_title, book_dept_id, book_stream_id, book_level_id, book_module_code From ecc_dof_wukrostmarycollege.library";
            sqlCmd = new MySqlCommand(tableQuery, conn);
            dataAdapter = new MySqlDataAdapter(sqlCmd);
            DataTable table = new DataTable();
            dataGridBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridBooks.RowTemplate.Height = 20;
            dataGridBooks.AllowUserToAddRows = false;
            dataAdapter.Fill(table);
            dataGridBooks.DataSource = table;
        }

        private void dataGridBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            rowIndex = e.RowIndex;
            DataGridViewRow row = dataGridBooks.Rows[rowIndex];
            cellValue = row.Cells[0].Value.ToString();

            string bookTyp = null, bookTtl = null, bookDpt = null, bookStrm = null, bookLvl = null, bookMod = null;
            string retrieveQuery = "Select book_type, book_title, book_dept_id, book_stream_id, book_level_id, book_module_code From ecc_dof_wukrostmarycollege.library Where book_id = '" + cellValue + "'";
            sqlCmd = new MySqlCommand(retrieveQuery, conn);
            try
            {
                conn.Open();
                dataReader = sqlCmd.ExecuteReader();
                while (dataReader.Read())
                {
                    bookTyp = dataReader["book_type"].ToString();
                    bookTtl = dataReader["book_title"].ToString();
                    bookDpt = dataReader["book_dept_id"].ToString();
                    bookStrm = dataReader["book_stream_id"].ToString();
                    bookLvl = dataReader["book_level_id"].ToString();
                    bookMod = dataReader["book_module_code"].ToString();

                    bookID.Text = cellValue;
                    bookType.Text = bookTyp;
                    bookTitle.Text = bookTtl;
                    bookDeptID.Text = bookDpt;
                    bookStreamID.Text = bookStrm;
                    bookLevelID.Text = bookLvl;
                    bookModCode.Text = bookMod;
                }
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Connection failed! " + ex.Message);
                conn.Close();
            }
        }

        private void __filterButton_Click(object sender, EventArgs e)
        {
            db = new DBConnect();
            conn = db.getConnection();

            if (__bookID.Text != "" && __bookTitle.Text == "" && __bookDeptID.Text == "" && __bookStreamID.Text == "")
            {
                string tableQuery = "Select book_id, book_type, book_title, book_dept_id, book_stream_id, book_level_id, book_module_code From ecc_dof_wukrostmarycollege.library Where book_id = '"+this.__bookID.Text+"'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridBooks.RowTemplate.Height = 20;
                dataGridBooks.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridBooks.DataSource = table;
            }
            else if (__bookID.Text == "" && __bookTitle.Text != "" && __bookDeptID.Text == "" && __bookStreamID.Text == "")
            {
                string tableQuery = "Select book_id, book_type, book_title, book_dept_id, book_stream_id, book_level_id, book_module_code From ecc_dof_wukrostmarycollege.library Where book_title = '" + this.__bookTitle.Text + "'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridBooks.RowTemplate.Height = 20;
                dataGridBooks.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridBooks.DataSource = table;
            }
            else if (__bookID.Text == "" && __bookTitle.Text == "" && __bookDeptID.Text != "" && __bookStreamID.Text != "")
            {
                string tableQuery = "Select book_id, book_type, book_title, book_dept_id, book_stream_id, book_level_id, book_module_code From ecc_dof_wukrostmarycollege.library Where book_dept_id = '" + this.__bookDeptID.Text + "' and book_stream_id = '"+this.__bookStreamID.Text+"'";
                sqlCmd = new MySqlCommand(tableQuery, conn);
                dataAdapter = new MySqlDataAdapter(sqlCmd);
                DataTable table = new DataTable();
                dataGridBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridBooks.RowTemplate.Height = 20;
                dataGridBooks.AllowUserToAddRows = false;
                dataAdapter.Fill(table);
                dataGridBooks.DataSource = table;
            }
            else
            {
                MessageBox.Show("Invalid search parameters!");
            }
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
            Dropout dout = new Dropout();
            dout.Show();
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
            Departments dpt = new Departments();
            dpt.Show();
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
            Help hl = new Help();
            hl.Show();
            this.Close();
        }

        private void bookDownloadButton_Click(object sender, EventArgs e)
        {
            //UInt32 FileSize;
            //byte[] rawData;
            //FileStream fs;
            //string fileName = Path.GetTempFileName() + ".pdf";

            //string downloadQuery = "SELECT * FROM ecc_dof_wukrostmarycollege.library where book_id='" + cellValue + "'";
            //sqlCmd = new MySqlCommand(downloadQuery, conn);
            //try
            //{
            //    conn.Open();

            //    dataReader = sqlCmd.ExecuteReader();

            //    if (!dataReader.HasRows)
            //        throw new Exception("There are no BLOBs to save");

            //    dataReader.Read();

            //    FileSize = dataReader.GetUInt32(dataReader.GetOrdinal("filesize"));
            //    rawData = new byte[FileSize];

            //    dataReader.GetBytes(dataReader.GetOrdinal("file"), 0, rawData, 0, (int)FileSize);

            //    fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            //    fs.Write(rawData, 0, (int)FileSize);
            //    fs.Close();

            //    MessageBox.Show("File successfully written to disk!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //    //Process prc = new Process();
            //    //prc.StartInfo.FileName = fileName;
            //    //prc.Start();
            //    dataReader.Close();
            //    conn.Close();
            //}
            //catch (MySql.Data.MySqlClient.MySqlException ex)
            //{
            //    MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
    }
}
