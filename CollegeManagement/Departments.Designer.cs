namespace CollegeManagement
{
    partial class Departments
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
            this.deptName = new System.Windows.Forms.TextBox();
            this.dataGridDepts = new System.Windows.Forms.DataGridView();
            this.label9 = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.deptHead = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.deptProgram = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.deptID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
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
            this.adminsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageAdminsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDepts)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // deptName
            // 
            this.deptName.Location = new System.Drawing.Point(238, 153);
            this.deptName.Name = "deptName";
            this.deptName.Size = new System.Drawing.Size(118, 22);
            this.deptName.TabIndex = 180;
            // 
            // dataGridDepts
            // 
            this.dataGridDepts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridDepts.Location = new System.Drawing.Point(57, 299);
            this.dataGridDepts.Name = "dataGridDepts";
            this.dataGridDepts.RowTemplate.Height = 24;
            this.dataGridDepts.Size = new System.Drawing.Size(693, 334);
            this.dataGridDepts.TabIndex = 179;
            this.dataGridDepts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridDepts_CellClick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Silver;
            this.label9.Location = new System.Drawing.Point(54, 267);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(696, 17);
            this.label9.TabIndex = 178;
            this.label9.Text = "_________________________________________________________________________________" +
    "_____";
            // 
            // deleteButton
            // 
            this.deleteButton.Font = new System.Drawing.Font("Microsoft MHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.deleteButton.Location = new System.Drawing.Point(581, 222);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(109, 44);
            this.deleteButton.TabIndex = 177;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Font = new System.Drawing.Font("Microsoft MHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.updateButton.Location = new System.Drawing.Point(411, 222);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(109, 44);
            this.updateButton.TabIndex = 176;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Microsoft MHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.saveButton.Location = new System.Drawing.Point(238, 222);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(109, 44);
            this.saveButton.TabIndex = 174;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.Color.Silver;
            this.label20.Location = new System.Drawing.Point(54, 70);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(696, 17);
            this.label20.TabIndex = 173;
            this.label20.Text = "_________________________________________________________________________________" +
    "_____";
            // 
            // deptHead
            // 
            this.deptHead.Location = new System.Drawing.Point(573, 153);
            this.deptHead.Name = "deptHead";
            this.deptHead.Size = new System.Drawing.Size(118, 22);
            this.deptHead.TabIndex = 171;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.DimGray;
            this.label6.Location = new System.Drawing.Point(419, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 20);
            this.label6.TabIndex = 170;
            this.label6.Text = "Department Head";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.DimGray;
            this.label7.Location = new System.Drawing.Point(82, 153);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(136, 20);
            this.label7.TabIndex = 169;
            this.label7.Text = "Department Name";
            // 
            // deptProgram
            // 
            this.deptProgram.Location = new System.Drawing.Point(573, 108);
            this.deptProgram.Name = "deptProgram";
            this.deptProgram.Size = new System.Drawing.Size(118, 22);
            this.deptProgram.TabIndex = 166;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(395, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 20);
            this.label1.TabIndex = 165;
            this.label1.Text = "Department Program";
            // 
            // deptID
            // 
            this.deptID.Location = new System.Drawing.Point(238, 108);
            this.deptID.Name = "deptID";
            this.deptID.Size = new System.Drawing.Size(118, 22);
            this.deptID.TabIndex = 164;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft MHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(108, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 20);
            this.label5.TabIndex = 163;
            this.label5.Text = "Department ID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft MHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(52, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 26);
            this.label4.TabIndex = 162;
            this.label4.Text = "Department Entry";
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
            this.menuStrip1.Size = new System.Drawing.Size(815, 28);
            this.menuStrip1.TabIndex = 181;
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
            // Departments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 647);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.deptName);
            this.Controls.Add(this.dataGridDepts);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.deptHead);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.deptProgram);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.deptID);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.MaximizeBox = false;
            this.Name = "Departments";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Departments";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDepts)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox deptName;
        private System.Windows.Forms.DataGridView dataGridDepts;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox deptHead;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox deptProgram;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox deptID;
        private System.Windows.Forms.Label label5;
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
        private System.Windows.Forms.ToolStripMenuItem adminsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageAdminsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem libraryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem libraryToolStripMenuItem1;
    }
}