namespace CollegeManagement
{
    partial class AttendanceSheet
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
            this.generateButton = new System.Windows.Forms.Button();
            this.admissionType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.level = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
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
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tVETTranscriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attendanceSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cOCListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adminsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageAdminsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deptID = new System.Windows.Forms.TextBox();
            this.streamID = new System.Windows.Forms.TextBox();
            this.cryRptAttendanceSheet = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // generateButton
            // 
            this.generateButton.Font = new System.Drawing.Font("Microsoft MHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.generateButton.Location = new System.Drawing.Point(654, 117);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(136, 62);
            this.generateButton.TabIndex = 238;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // admissionType
            // 
            this.admissionType.FormattingEnabled = true;
            this.admissionType.Items.AddRange(new object[] {
            "Regular",
            "Extension"});
            this.admissionType.Location = new System.Drawing.Point(279, 155);
            this.admissionType.Name = "admissionType";
            this.admissionType.Size = new System.Drawing.Size(118, 24);
            this.admissionType.TabIndex = 236;
            this.admissionType.Text = "Regular";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.DimGray;
            this.label9.Location = new System.Drawing.Point(143, 155);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(116, 20);
            this.label9.TabIndex = 235;
            this.label9.Text = "Admission Type";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.DimGray;
            this.label8.Location = new System.Drawing.Point(420, 117);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 20);
            this.label8.TabIndex = 229;
            this.label8.Text = "Stream ID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Silver;
            this.label1.Location = new System.Drawing.Point(8, 192);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1240, 17);
            this.label1.TabIndex = 228;
            this.label1.Text = "_________________________________________________________________________________" +
    "_________________________________________________________________________";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(452, 155);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 20);
            this.label2.TabIndex = 226;
            this.label2.Text = "Level";
            // 
            // level
            // 
            this.level.FormattingEnabled = true;
            this.level.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.level.Location = new System.Drawing.Point(516, 155);
            this.level.Name = "level";
            this.level.Size = new System.Drawing.Size(118, 24);
            this.level.TabIndex = 225;
            this.level.Text = "1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(149, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 20);
            this.label5.TabIndex = 223;
            this.label5.Text = "Department ID";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.Color.Silver;
            this.label20.Location = new System.Drawing.Point(14, 78);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(784, 17);
            this.label20.TabIndex = 222;
            this.label20.Text = "_________________________________________________________________________________" +
    "________________";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft MHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(12, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(247, 26);
            this.label4.TabIndex = 221;
            this.label4.Text = "Generate Attendance Sheet";
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
            this.reportsToolStripMenuItem,
            this.adminsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1260, 28);
            this.menuStrip1.TabIndex = 239;
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
            this.instructorsToolStripMenuItem2.Size = new System.Drawing.Size(87, 24);
            this.instructorsToolStripMenuItem2.Text = "Employee";
            // 
            // instructorsToolStripMenuItem3
            // 
            this.instructorsToolStripMenuItem3.Name = "instructorsToolStripMenuItem3";
            this.instructorsToolStripMenuItem3.Size = new System.Drawing.Size(150, 26);
            this.instructorsToolStripMenuItem3.Text = "Employee";
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
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tVETTranscriptToolStripMenuItem,
            this.markListToolStripMenuItem,
            this.attendanceSheetToolStripMenuItem,
            this.cOCListToolStripMenuItem});
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
            this.reportsToolStripMenuItem.Text = "Reports";
            // 
            // tVETTranscriptToolStripMenuItem
            // 
            this.tVETTranscriptToolStripMenuItem.Name = "tVETTranscriptToolStripMenuItem";
            this.tVETTranscriptToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.tVETTranscriptToolStripMenuItem.Text = "TVET Transcript";
            this.tVETTranscriptToolStripMenuItem.Click += new System.EventHandler(this.tVETTranscriptToolStripMenuItem_Click);
            // 
            // markListToolStripMenuItem
            // 
            this.markListToolStripMenuItem.Name = "markListToolStripMenuItem";
            this.markListToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.markListToolStripMenuItem.Text = "Mark List";
            this.markListToolStripMenuItem.Click += new System.EventHandler(this.markListToolStripMenuItem_Click);
            // 
            // attendanceSheetToolStripMenuItem
            // 
            this.attendanceSheetToolStripMenuItem.Name = "attendanceSheetToolStripMenuItem";
            this.attendanceSheetToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.attendanceSheetToolStripMenuItem.Text = "Attendance Sheet";
            this.attendanceSheetToolStripMenuItem.Click += new System.EventHandler(this.attendanceSheetToolStripMenuItem_Click);
            // 
            // cOCListToolStripMenuItem
            // 
            this.cOCListToolStripMenuItem.Name = "cOCListToolStripMenuItem";
            this.cOCListToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.cOCListToolStripMenuItem.Text = "COC List";
            this.cOCListToolStripMenuItem.Click += new System.EventHandler(this.cOCListToolStripMenuItem_Click);
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
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(116, 26);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // deptID
            // 
            this.deptID.Location = new System.Drawing.Point(279, 117);
            this.deptID.Name = "deptID";
            this.deptID.Size = new System.Drawing.Size(118, 22);
            this.deptID.TabIndex = 240;
            // 
            // streamID
            // 
            this.streamID.Location = new System.Drawing.Point(516, 117);
            this.streamID.Name = "streamID";
            this.streamID.Size = new System.Drawing.Size(118, 22);
            this.streamID.TabIndex = 241;
            // 
            // cryRptAttendanceSheet
            // 
            this.cryRptAttendanceSheet.ActiveViewIndex = -1;
            this.cryRptAttendanceSheet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cryRptAttendanceSheet.Cursor = System.Windows.Forms.Cursors.Default;
            this.cryRptAttendanceSheet.Location = new System.Drawing.Point(12, 212);
            this.cryRptAttendanceSheet.Name = "cryRptAttendanceSheet";
            this.cryRptAttendanceSheet.Size = new System.Drawing.Size(1236, 629);
            this.cryRptAttendanceSheet.TabIndex = 242;
            this.cryRptAttendanceSheet.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // AttendanceSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1260, 853);
            this.Controls.Add(this.cryRptAttendanceSheet);
            this.Controls.Add(this.streamID);
            this.Controls.Add(this.deptID);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.admissionType);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.level);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label4);
            this.MaximizeBox = false;
            this.Name = "AttendanceSheet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Attendance Sheet";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.ComboBox admissionType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox level;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label4;
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
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tVETTranscriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem markListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem attendanceSheetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cOCListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adminsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageAdminsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.TextBox deptID;
        private System.Windows.Forms.TextBox streamID;
        private System.Windows.Forms.ToolStripMenuItem libraryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem libraryToolStripMenuItem1;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer cryRptAttendanceSheet;
    }
}