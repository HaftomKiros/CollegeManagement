namespace CollegeManagement
{
    partial class Instructors
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sex = new System.Windows.Forms.ComboBox();
            this.lName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.dataGridInstructors = new System.Windows.Forms.DataGridView();
            this.label9 = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.empDate = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.mobNum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.deptID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.empID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.fName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.homeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.homeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.studentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.studentRegistrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.studentMarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.studentFeesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dropoutStudentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cOCRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instructorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instructorsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.streamsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.levelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coursesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instructorsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.instructorsToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.alumniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alumniToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.libraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.libraryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.adminsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageAdminsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label10 = new System.Windows.Forms.Label();
            this.qualficationTitle = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.level = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.birthDate = new System.Windows.Forms.TextBox();
            this.empPhotoPath = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.empDocPath = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.picBrowseButton = new System.Windows.Forms.Button();
            this.attachButton = new System.Windows.Forms.Button();
            this.picDownloadButton = new System.Windows.Forms.Button();
            this.attachmentDownloadButton = new System.Windows.Forms.Button();
            this.empPic = new System.Windows.Forms.PictureBox();
            this.label17 = new System.Windows.Forms.Label();
            this.@__empID = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.@__deptID = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.@__printButton = new System.Windows.Forms.Button();
            this.@__level = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.@__filterButton = new System.Windows.Forms.Button();
            this.nameTag = new System.Windows.Forms.Label();
            this.deptTag = new System.Windows.Forms.Label();
            this.levelTag = new System.Windows.Forms.Label();
            this.qualificationTag = new System.Windows.Forms.Label();
            this.mobNumTag = new System.Windows.Forms.Label();
            this.@__mobNumTag = new System.Windows.Forms.Label();
            this.@__levelTag = new System.Windows.Forms.Label();
            this.@__qualificationTag = new System.Windows.Forms.Label();
            this.@__deptTag = new System.Windows.Forms.Label();
            this.@__nameTag = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridInstructors)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.empPic)).BeginInit();
            this.SuspendLayout();
            // 
            // sex
            // 
            this.sex.FormattingEnabled = true;
            this.sex.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.sex.Location = new System.Drawing.Point(222, 211);
            this.sex.Name = "sex";
            this.sex.Size = new System.Drawing.Size(118, 24);
            this.sex.TabIndex = 188;
            this.sex.Text = "Male";
            // 
            // lName
            // 
            this.lName.Location = new System.Drawing.Point(802, 171);
            this.lName.Name = "lName";
            this.lName.Size = new System.Drawing.Size(118, 22);
            this.lName.TabIndex = 185;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.DimGray;
            this.label14.Location = new System.Drawing.Point(701, 171);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(80, 20);
            this.label14.TabIndex = 184;
            this.label14.Text = "Last Name";
            // 
            // dataGridInstructors
            // 
            this.dataGridInstructors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridInstructors.Location = new System.Drawing.Point(29, 467);
            this.dataGridInstructors.Name = "dataGridInstructors";
            this.dataGridInstructors.RowTemplate.Height = 24;
            this.dataGridInstructors.Size = new System.Drawing.Size(1266, 269);
            this.dataGridInstructors.TabIndex = 183;
            this.dataGridInstructors.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridInstructors_CellClick);
            this.dataGridInstructors.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridInstructors_CellContentClick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Silver;
            this.label9.Location = new System.Drawing.Point(26, 427);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(1272, 17);
            this.label9.TabIndex = 182;
            this.label9.Text = "_________________________________________________________________________________" +
    "_____________________________________________________________________________";
            // 
            // deleteButton
            // 
            this.deleteButton.Font = new System.Drawing.Font("Microsoft MHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.deleteButton.Location = new System.Drawing.Point(607, 380);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(109, 44);
            this.deleteButton.TabIndex = 181;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Font = new System.Drawing.Font("Microsoft MHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.updateButton.Location = new System.Drawing.Point(459, 380);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(109, 44);
            this.updateButton.TabIndex = 180;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Microsoft MHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.saveButton.Location = new System.Drawing.Point(301, 380);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(109, 44);
            this.saveButton.TabIndex = 178;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // empDate
            // 
            this.empDate.Location = new System.Drawing.Point(802, 124);
            this.empDate.Name = "empDate";
            this.empDate.Size = new System.Drawing.Size(118, 22);
            this.empDate.TabIndex = 177;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.DimGray;
            this.label8.Location = new System.Drawing.Point(650, 124);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(131, 20);
            this.label8.TabIndex = 176;
            this.label8.Text = "Employment Date";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.Color.Silver;
            this.label20.Location = new System.Drawing.Point(26, 77);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(904, 17);
            this.label20.TabIndex = 175;
            this.label20.Text = "_________________________________________________________________________________" +
    "_______________________________";
            // 
            // mobNum
            // 
            this.mobNum.Location = new System.Drawing.Point(505, 211);
            this.mobNum.Name = "mobNum";
            this.mobNum.Size = new System.Drawing.Size(118, 22);
            this.mobNum.TabIndex = 174;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(371, 211);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 20);
            this.label3.TabIndex = 173;
            this.label3.Text = "Mobile Number";
            // 
            // mName
            // 
            this.mName.Location = new System.Drawing.Point(505, 171);
            this.mName.Name = "mName";
            this.mName.Size = new System.Drawing.Size(118, 22);
            this.mName.TabIndex = 172;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.DimGray;
            this.label6.Location = new System.Drawing.Point(387, 171);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 20);
            this.label6.TabIndex = 171;
            this.label6.Text = "Middle Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.DimGray;
            this.label7.Location = new System.Drawing.Point(119, 171);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 20);
            this.label7.TabIndex = 170;
            this.label7.Text = "First Name";
            // 
            // deptID
            // 
            this.deptID.Location = new System.Drawing.Point(505, 126);
            this.deptID.Name = "deptID";
            this.deptID.Size = new System.Drawing.Size(118, 22);
            this.deptID.TabIndex = 169;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(379, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 20);
            this.label1.TabIndex = 168;
            this.label1.Text = "Department ID";
            // 
            // empID
            // 
            this.empID.Location = new System.Drawing.Point(222, 126);
            this.empID.Name = "empID";
            this.empID.Size = new System.Drawing.Size(118, 22);
            this.empID.TabIndex = 167;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(108, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 20);
            this.label5.TabIndex = 166;
            this.label5.Text = "Employee ID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft MHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(24, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(257, 26);
            this.label4.TabIndex = 165;
            this.label4.Text = "Employee Registration Panel";
            // 
            // fName
            // 
            this.fName.Location = new System.Drawing.Point(222, 171);
            this.fName.Name = "fName";
            this.fName.Size = new System.Drawing.Size(118, 22);
            this.fName.TabIndex = 190;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(169, 211);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 20);
            this.label2.TabIndex = 189;
            this.label2.Text = "Sex";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.homeToolStripMenuItem,
            this.studentsToolStripMenuItem,
            this.instructorsToolStripMenuItem,
            this.instructorsToolStripMenuItem2,
            this.alumniToolStripMenuItem,
            this.libraryToolStripMenuItem,
            this.adminsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1510, 28);
            this.menuStrip1.TabIndex = 191;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // homeToolStripMenuItem
            // 
            this.homeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.homeToolStripMenuItem1});
            this.homeToolStripMenuItem.Name = "homeToolStripMenuItem";
            this.homeToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.homeToolStripMenuItem.Text = "Home";
            // 
            // homeToolStripMenuItem1
            // 
            this.homeToolStripMenuItem1.Name = "homeToolStripMenuItem1";
            this.homeToolStripMenuItem1.Size = new System.Drawing.Size(125, 26);
            this.homeToolStripMenuItem1.Text = "Home";
            this.homeToolStripMenuItem1.Click += new System.EventHandler(this.homeToolStripMenuItem1_Click);
            // 
            // studentsToolStripMenuItem
            // 
            this.studentsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.studentRegistrationToolStripMenuItem,
            this.studentMarksToolStripMenuItem,
            this.studentFeesToolStripMenuItem,
            this.dropoutStudentsToolStripMenuItem,
            this.cOCRecordToolStripMenuItem});
            this.studentsToolStripMenuItem.Name = "studentsToolStripMenuItem";
            this.studentsToolStripMenuItem.Size = new System.Drawing.Size(78, 24);
            this.studentsToolStripMenuItem.Text = "Students";
            // 
            // studentRegistrationToolStripMenuItem
            // 
            this.studentRegistrationToolStripMenuItem.Name = "studentRegistrationToolStripMenuItem";
            this.studentRegistrationToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.studentRegistrationToolStripMenuItem.Text = "Student Registration";
            this.studentRegistrationToolStripMenuItem.Click += new System.EventHandler(this.studentRegistrationToolStripMenuItem_Click);
            // 
            // studentMarksToolStripMenuItem
            // 
            this.studentMarksToolStripMenuItem.Name = "studentMarksToolStripMenuItem";
            this.studentMarksToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.studentMarksToolStripMenuItem.Text = "Student Marks";
            this.studentMarksToolStripMenuItem.Click += new System.EventHandler(this.studentMarksToolStripMenuItem_Click);
            // 
            // studentFeesToolStripMenuItem
            // 
            this.studentFeesToolStripMenuItem.Name = "studentFeesToolStripMenuItem";
            this.studentFeesToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.studentFeesToolStripMenuItem.Text = "Student Fees";
            this.studentFeesToolStripMenuItem.Click += new System.EventHandler(this.studentFeesToolStripMenuItem_Click);
            // 
            // dropoutStudentsToolStripMenuItem
            // 
            this.dropoutStudentsToolStripMenuItem.Name = "dropoutStudentsToolStripMenuItem";
            this.dropoutStudentsToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.dropoutStudentsToolStripMenuItem.Text = "Dropout Students";
            this.dropoutStudentsToolStripMenuItem.Click += new System.EventHandler(this.dropoutStudentsToolStripMenuItem_Click);
            // 
            // cOCRecordToolStripMenuItem
            // 
            this.cOCRecordToolStripMenuItem.Name = "cOCRecordToolStripMenuItem";
            this.cOCRecordToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.cOCRecordToolStripMenuItem.Text = "COC Record";
            this.cOCRecordToolStripMenuItem.Click += new System.EventHandler(this.cOCRecordToolStripMenuItem_Click);
            // 
            // instructorsToolStripMenuItem
            // 
            this.instructorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.instructorsToolStripMenuItem1,
            this.streamsToolStripMenuItem,
            this.levelsToolStripMenuItem,
            this.coursesToolStripMenuItem});
            this.instructorsToolStripMenuItem.Name = "instructorsToolStripMenuItem";
            this.instructorsToolStripMenuItem.Size = new System.Drawing.Size(107, 24);
            this.instructorsToolStripMenuItem.Text = "Departments";
            // 
            // instructorsToolStripMenuItem1
            // 
            this.instructorsToolStripMenuItem1.Name = "instructorsToolStripMenuItem1";
            this.instructorsToolStripMenuItem1.Size = new System.Drawing.Size(170, 26);
            this.instructorsToolStripMenuItem1.Text = "Departments";
            this.instructorsToolStripMenuItem1.Click += new System.EventHandler(this.instructorsToolStripMenuItem1_Click);
            // 
            // streamsToolStripMenuItem
            // 
            this.streamsToolStripMenuItem.Name = "streamsToolStripMenuItem";
            this.streamsToolStripMenuItem.Size = new System.Drawing.Size(170, 26);
            this.streamsToolStripMenuItem.Text = "Streams";
            this.streamsToolStripMenuItem.Click += new System.EventHandler(this.streamsToolStripMenuItem_Click);
            // 
            // levelsToolStripMenuItem
            // 
            this.levelsToolStripMenuItem.Name = "levelsToolStripMenuItem";
            this.levelsToolStripMenuItem.Size = new System.Drawing.Size(170, 26);
            this.levelsToolStripMenuItem.Text = "Levels";
            this.levelsToolStripMenuItem.Click += new System.EventHandler(this.levelsToolStripMenuItem_Click);
            // 
            // coursesToolStripMenuItem
            // 
            this.coursesToolStripMenuItem.Name = "coursesToolStripMenuItem";
            this.coursesToolStripMenuItem.Size = new System.Drawing.Size(170, 26);
            this.coursesToolStripMenuItem.Text = "Courses";
            this.coursesToolStripMenuItem.Click += new System.EventHandler(this.coursesToolStripMenuItem_Click);
            // 
            // instructorsToolStripMenuItem2
            // 
            this.instructorsToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.instructorsToolStripMenuItem3});
            this.instructorsToolStripMenuItem2.Name = "instructorsToolStripMenuItem2";
            this.instructorsToolStripMenuItem2.Size = new System.Drawing.Size(93, 24);
            this.instructorsToolStripMenuItem2.Text = "Employees";
            // 
            // instructorsToolStripMenuItem3
            // 
            this.instructorsToolStripMenuItem3.Name = "instructorsToolStripMenuItem3";
            this.instructorsToolStripMenuItem3.Size = new System.Drawing.Size(156, 26);
            this.instructorsToolStripMenuItem3.Text = "Employees";
            this.instructorsToolStripMenuItem3.Click += new System.EventHandler(this.instructorsToolStripMenuItem3_Click);
            // 
            // alumniToolStripMenuItem
            // 
            this.alumniToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alumniToolStripMenuItem1});
            this.alumniToolStripMenuItem.Name = "alumniToolStripMenuItem";
            this.alumniToolStripMenuItem.Size = new System.Drawing.Size(68, 24);
            this.alumniToolStripMenuItem.Text = "Alumni";
            // 
            // alumniToolStripMenuItem1
            // 
            this.alumniToolStripMenuItem1.Name = "alumniToolStripMenuItem1";
            this.alumniToolStripMenuItem1.Size = new System.Drawing.Size(131, 26);
            this.alumniToolStripMenuItem1.Text = "Alumni";
            this.alumniToolStripMenuItem1.Click += new System.EventHandler(this.alumniToolStripMenuItem1_Click);
            // 
            // libraryToolStripMenuItem
            // 
            this.libraryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.libraryToolStripMenuItem1});
            this.libraryToolStripMenuItem.Name = "libraryToolStripMenuItem";
            this.libraryToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.libraryToolStripMenuItem.Text = "Library";
            // 
            // libraryToolStripMenuItem1
            // 
            this.libraryToolStripMenuItem1.Name = "libraryToolStripMenuItem1";
            this.libraryToolStripMenuItem1.Size = new System.Drawing.Size(129, 26);
            this.libraryToolStripMenuItem1.Text = "Library";
            this.libraryToolStripMenuItem1.Click += new System.EventHandler(this.libraryToolStripMenuItem1_Click);
            // 
            // adminsToolStripMenuItem
            // 
            this.adminsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageAdminsToolStripMenuItem,
            this.signOutToolStripMenuItem});
            this.adminsToolStripMenuItem.Name = "adminsToolStripMenuItem";
            this.adminsToolStripMenuItem.Size = new System.Drawing.Size(71, 24);
            this.adminsToolStripMenuItem.Text = "Admins";
            // 
            // manageAdminsToolStripMenuItem
            // 
            this.manageAdminsToolStripMenuItem.Name = "manageAdminsToolStripMenuItem";
            this.manageAdminsToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.manageAdminsToolStripMenuItem.Text = "Manage Admins";
            this.manageAdminsToolStripMenuItem.Click += new System.EventHandler(this.manageAdminsToolStripMenuItem_Click);
            // 
            // signOutToolStripMenuItem
            // 
            this.signOutToolStripMenuItem.Name = "signOutToolStripMenuItem";
            this.signOutToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.signOutToolStripMenuItem.Text = "Sign Out";
            this.signOutToolStripMenuItem.Click += new System.EventHandler(this.signOutToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(116, 26);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.DimGray;
            this.label10.Location = new System.Drawing.Point(685, 211);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(96, 20);
            this.label10.TabIndex = 197;
            this.label10.Text = "Date of Birth";
            // 
            // qualficationTitle
            // 
            this.qualficationTitle.Location = new System.Drawing.Point(802, 250);
            this.qualficationTitle.Name = "qualficationTitle";
            this.qualficationTitle.Size = new System.Drawing.Size(118, 22);
            this.qualficationTitle.TabIndex = 195;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.DimGray;
            this.label11.Location = new System.Drawing.Point(652, 250);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(129, 20);
            this.label11.TabIndex = 194;
            this.label11.Text = "Qualification Title";
            // 
            // level
            // 
            this.level.Location = new System.Drawing.Point(505, 250);
            this.level.Name = "level";
            this.level.Size = new System.Drawing.Size(118, 22);
            this.level.TabIndex = 193;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.DimGray;
            this.label12.Location = new System.Drawing.Point(445, 250);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(44, 20);
            this.label12.TabIndex = 192;
            this.label12.Text = "Level";
            // 
            // birthDate
            // 
            this.birthDate.Location = new System.Drawing.Point(801, 211);
            this.birthDate.Name = "birthDate";
            this.birthDate.Size = new System.Drawing.Size(118, 22);
            this.birthDate.TabIndex = 198;
            // 
            // empPhotoPath
            // 
            this.empPhotoPath.Location = new System.Drawing.Point(802, 288);
            this.empPhotoPath.Name = "empPhotoPath";
            this.empPhotoPath.Size = new System.Drawing.Size(118, 22);
            this.empPhotoPath.TabIndex = 200;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.DimGray;
            this.label13.Location = new System.Drawing.Point(660, 288);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(121, 20);
            this.label13.TabIndex = 199;
            this.label13.Text = "Employee Photo";
            // 
            // empDocPath
            // 
            this.empDocPath.Location = new System.Drawing.Point(802, 328);
            this.empDocPath.Name = "empDocPath";
            this.empDocPath.Size = new System.Drawing.Size(118, 22);
            this.empDocPath.TabIndex = 202;
            this.empDocPath.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.DimGray;
            this.label16.Location = new System.Drawing.Point(630, 328);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(151, 20);
            this.label16.TabIndex = 201;
            this.label16.Text = "Employee Document";
            this.label16.Click += new System.EventHandler(this.label16_Click);
            // 
            // picBrowseButton
            // 
            this.picBrowseButton.Font = new System.Drawing.Font("Microsoft MHei", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.picBrowseButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.picBrowseButton.Location = new System.Drawing.Point(946, 281);
            this.picBrowseButton.Name = "picBrowseButton";
            this.picBrowseButton.Size = new System.Drawing.Size(87, 34);
            this.picBrowseButton.TabIndex = 203;
            this.picBrowseButton.Text = "Browse";
            this.picBrowseButton.UseVisualStyleBackColor = true;
            this.picBrowseButton.Click += new System.EventHandler(this.picBrowseButton_Click);
            // 
            // attachButton
            // 
            this.attachButton.Font = new System.Drawing.Font("Microsoft MHei", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.attachButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.attachButton.Location = new System.Drawing.Point(946, 323);
            this.attachButton.Name = "attachButton";
            this.attachButton.Size = new System.Drawing.Size(87, 30);
            this.attachButton.TabIndex = 204;
            this.attachButton.Text = "Attach";
            this.attachButton.UseVisualStyleBackColor = true;
            this.attachButton.Click += new System.EventHandler(this.attachButton_Click);
            // 
            // picDownloadButton
            // 
            this.picDownloadButton.Font = new System.Drawing.Font("Microsoft MHei", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.picDownloadButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.picDownloadButton.Location = new System.Drawing.Point(1055, 281);
            this.picDownloadButton.Name = "picDownloadButton";
            this.picDownloadButton.Size = new System.Drawing.Size(105, 34);
            this.picDownloadButton.TabIndex = 205;
            this.picDownloadButton.Text = "Download...";
            this.picDownloadButton.UseVisualStyleBackColor = true;
            // 
            // attachmentDownloadButton
            // 
            this.attachmentDownloadButton.Font = new System.Drawing.Font("Microsoft MHei", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.attachmentDownloadButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.attachmentDownloadButton.Location = new System.Drawing.Point(1055, 321);
            this.attachmentDownloadButton.Name = "attachmentDownloadButton";
            this.attachmentDownloadButton.Size = new System.Drawing.Size(105, 34);
            this.attachmentDownloadButton.TabIndex = 206;
            this.attachmentDownloadButton.Text = "Download...";
            this.attachmentDownloadButton.UseVisualStyleBackColor = true;
            // 
            // empPic
            // 
            this.empPic.Location = new System.Drawing.Point(946, 30);
            this.empPic.Name = "empPic";
            this.empPic.Size = new System.Drawing.Size(214, 240);
            this.empPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.empPic.TabIndex = 207;
            this.empPic.TabStop = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft MHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.CadetBlue;
            this.label17.Location = new System.Drawing.Point(1324, 512);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(39, 27);
            this.label17.TabIndex = 223;
            this.label17.Text = "OR";
            // 
            // __empID
            // 
            this.@__empID.Location = new System.Drawing.Point(1329, 477);
            this.@__empID.Name = "__empID";
            this.@__empID.Size = new System.Drawing.Size(136, 22);
            this.@__empID.TabIndex = 222;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.Color.DimGray;
            this.label18.Location = new System.Drawing.Point(1325, 454);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(58, 20);
            this.label18.TabIndex = 221;
            this.label18.Text = "Emp ID";
            // 
            // __deptID
            // 
            this.@__deptID.Location = new System.Drawing.Point(1329, 575);
            this.@__deptID.Name = "__deptID";
            this.@__deptID.Size = new System.Drawing.Size(136, 22);
            this.@__deptID.TabIndex = 220;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ForeColor = System.Drawing.Color.Silver;
            this.label19.Location = new System.Drawing.Point(1301, 718);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(168, 17);
            this.label19.TabIndex = 219;
            this.label19.Text = "____________________";
            this.label19.Click += new System.EventHandler(this.label19_Click);
            // 
            // __printButton
            // 
            this.@__printButton.Font = new System.Drawing.Font("Microsoft MHei", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.@__printButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.@__printButton.Location = new System.Drawing.Point(1409, 683);
            this.@__printButton.Name = "__printButton";
            this.@__printButton.Size = new System.Drawing.Size(57, 31);
            this.@__printButton.TabIndex = 218;
            this.@__printButton.Text = "Print";
            this.@__printButton.UseVisualStyleBackColor = true;
            this.@__printButton.Click += new System.EventHandler(this.@__printButton_Click);
            // 
            // __level
            // 
            this.@__level.FormattingEnabled = true;
            this.@__level.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.@__level.Location = new System.Drawing.Point(1329, 633);
            this.@__level.Name = "__level";
            this.@__level.Size = new System.Drawing.Size(136, 24);
            this.@__level.TabIndex = 214;
            this.@__level.Text = "1";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.DimGray;
            this.label22.Location = new System.Drawing.Point(1325, 610);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(44, 20);
            this.label22.TabIndex = 213;
            this.label22.Text = "Level";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.ForeColor = System.Drawing.Color.Silver;
            this.label27.Location = new System.Drawing.Point(1301, 427);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(168, 17);
            this.label27.TabIndex = 210;
            this.label27.Text = "____________________";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ForeColor = System.Drawing.Color.DimGray;
            this.label24.Location = new System.Drawing.Point(1325, 552);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(61, 20);
            this.label24.TabIndex = 209;
            this.label24.Text = "Dept ID";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft MHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.Gray;
            this.label25.Location = new System.Drawing.Point(1299, 398);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(87, 26);
            this.label25.TabIndex = 208;
            this.label25.Text = "Filter By:";
            // 
            // __filterButton
            // 
            this.@__filterButton.Font = new System.Drawing.Font("Microsoft MHei", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.@__filterButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.@__filterButton.Location = new System.Drawing.Point(1329, 683);
            this.@__filterButton.Name = "__filterButton";
            this.@__filterButton.Size = new System.Drawing.Size(57, 31);
            this.@__filterButton.TabIndex = 217;
            this.@__filterButton.Text = "Filter";
            this.@__filterButton.UseVisualStyleBackColor = true;
            this.@__filterButton.Click += new System.EventHandler(this.@__filterButton_Click);
            // 
            // nameTag
            // 
            this.nameTag.Font = new System.Drawing.Font("Microsoft NeoGothic", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameTag.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.nameTag.Location = new System.Drawing.Point(1212, 77);
            this.nameTag.Name = "nameTag";
            this.nameTag.Size = new System.Drawing.Size(51, 23);
            this.nameTag.TabIndex = 224;
            // 
            // deptTag
            // 
            this.deptTag.Font = new System.Drawing.Font("Microsoft NeoGothic", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deptTag.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.deptTag.Location = new System.Drawing.Point(1166, 112);
            this.deptTag.Name = "deptTag";
            this.deptTag.Size = new System.Drawing.Size(97, 29);
            this.deptTag.TabIndex = 225;
            // 
            // levelTag
            // 
            this.levelTag.Font = new System.Drawing.Font("Microsoft NeoGothic", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.levelTag.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.levelTag.Location = new System.Drawing.Point(1216, 182);
            this.levelTag.Name = "levelTag";
            this.levelTag.Size = new System.Drawing.Size(46, 23);
            this.levelTag.TabIndex = 227;
            // 
            // qualificationTag
            // 
            this.qualificationTag.Font = new System.Drawing.Font("Microsoft NeoGothic", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qualificationTag.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.qualificationTag.Location = new System.Drawing.Point(1167, 147);
            this.qualificationTag.Name = "qualificationTag";
            this.qualificationTag.Size = new System.Drawing.Size(96, 23);
            this.qualificationTag.TabIndex = 226;
            // 
            // mobNumTag
            // 
            this.mobNumTag.Font = new System.Drawing.Font("Microsoft NeoGothic", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mobNumTag.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.mobNumTag.Location = new System.Drawing.Point(1203, 221);
            this.mobNumTag.Name = "mobNumTag";
            this.mobNumTag.Size = new System.Drawing.Size(59, 23);
            this.mobNumTag.TabIndex = 228;
            // 
            // __mobNumTag
            // 
            this.@__mobNumTag.Font = new System.Drawing.Font("Microsoft NeoGothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.@__mobNumTag.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.@__mobNumTag.Location = new System.Drawing.Point(1284, 221);
            this.@__mobNumTag.Name = "__mobNumTag";
            this.@__mobNumTag.Size = new System.Drawing.Size(203, 23);
            this.@__mobNumTag.TabIndex = 233;
            // 
            // __levelTag
            // 
            this.@__levelTag.Font = new System.Drawing.Font("Microsoft NeoGothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.@__levelTag.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.@__levelTag.Location = new System.Drawing.Point(1284, 182);
            this.@__levelTag.Name = "__levelTag";
            this.@__levelTag.Size = new System.Drawing.Size(203, 23);
            this.@__levelTag.TabIndex = 232;
            // 
            // __qualificationTag
            // 
            this.@__qualificationTag.Font = new System.Drawing.Font("Microsoft NeoGothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.@__qualificationTag.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.@__qualificationTag.Location = new System.Drawing.Point(1284, 147);
            this.@__qualificationTag.Name = "__qualificationTag";
            this.@__qualificationTag.Size = new System.Drawing.Size(203, 23);
            this.@__qualificationTag.TabIndex = 231;
            // 
            // __deptTag
            // 
            this.@__deptTag.Font = new System.Drawing.Font("Microsoft NeoGothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.@__deptTag.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.@__deptTag.Location = new System.Drawing.Point(1284, 112);
            this.@__deptTag.Name = "__deptTag";
            this.@__deptTag.Size = new System.Drawing.Size(203, 23);
            this.@__deptTag.TabIndex = 230;
            // 
            // __nameTag
            // 
            this.@__nameTag.Font = new System.Drawing.Font("Microsoft NeoGothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.@__nameTag.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.@__nameTag.Location = new System.Drawing.Point(1284, 77);
            this.@__nameTag.Name = "__nameTag";
            this.@__nameTag.Size = new System.Drawing.Size(203, 23);
            this.@__nameTag.TabIndex = 229;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.ForeColor = System.Drawing.Color.Silver;
            this.label29.Location = new System.Drawing.Point(1209, 255);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(288, 17);
            this.label29.TabIndex = 234;
            this.label29.Text = "___________________________________";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.ForeColor = System.Drawing.Color.Silver;
            this.label30.Location = new System.Drawing.Point(1209, 51);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(288, 17);
            this.label30.TabIndex = 235;
            this.label30.Text = "___________________________________";
            // 
            // Instructors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1510, 748);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.@__mobNumTag);
            this.Controls.Add(this.@__levelTag);
            this.Controls.Add(this.@__qualificationTag);
            this.Controls.Add(this.@__deptTag);
            this.Controls.Add(this.@__nameTag);
            this.Controls.Add(this.mobNumTag);
            this.Controls.Add(this.levelTag);
            this.Controls.Add(this.qualificationTag);
            this.Controls.Add(this.deptTag);
            this.Controls.Add(this.nameTag);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.@__empID);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.@__deptID);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.@__printButton);
            this.Controls.Add(this.@__filterButton);
            this.Controls.Add(this.@__level);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.empPic);
            this.Controls.Add(this.attachmentDownloadButton);
            this.Controls.Add(this.picDownloadButton);
            this.Controls.Add(this.attachButton);
            this.Controls.Add(this.picBrowseButton);
            this.Controls.Add(this.empDocPath);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.empPhotoPath);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.birthDate);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.qualficationTitle);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.level);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.fName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sex);
            this.Controls.Add(this.lName);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.dataGridInstructors);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.empDate);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.mobNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.deptID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.empID);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.MaximizeBox = false;
            this.Name = "Instructors";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Employee Profile";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridInstructors)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.empPic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox sex;
        private System.Windows.Forms.TextBox lName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DataGridView dataGridInstructors;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox empDate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox mobNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox deptID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox empID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox fName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem homeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem homeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem studentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem studentRegistrationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem studentMarksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem studentFeesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dropoutStudentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cOCRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem instructorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem instructorsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem streamsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem levelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem coursesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem instructorsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem instructorsToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem alumniToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alumniToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem adminsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageAdminsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox qualficationTitle;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox level;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox birthDate;
        private System.Windows.Forms.TextBox empPhotoPath;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox empDocPath;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button picBrowseButton;
        private System.Windows.Forms.Button attachButton;
        private System.Windows.Forms.Button picDownloadButton;
        private System.Windows.Forms.Button attachmentDownloadButton;
        private System.Windows.Forms.PictureBox empPic;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox __empID;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox __deptID;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button __printButton;
        private System.Windows.Forms.ComboBox __level;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button __filterButton;
        private System.Windows.Forms.Label nameTag;
        private System.Windows.Forms.Label deptTag;
        private System.Windows.Forms.Label levelTag;
        private System.Windows.Forms.Label qualificationTag;
        private System.Windows.Forms.Label mobNumTag;
        private System.Windows.Forms.Label __mobNumTag;
        private System.Windows.Forms.Label __levelTag;
        private System.Windows.Forms.Label __qualificationTag;
        private System.Windows.Forms.Label __deptTag;
        private System.Windows.Forms.Label __nameTag;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.ToolStripMenuItem libraryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem libraryToolStripMenuItem1;
    }
}