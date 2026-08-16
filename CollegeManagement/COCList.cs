using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;

namespace CollegeManagement
{
    public partial class COCList : Form
    {
        public COCList()
        {
            InitializeComponent();
        }

        private void generateButton_Click(object sender, EventArgs e)
        {

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

        private void tVETTranscriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TVETTranscript tvet = new TVETTranscript();
            tvet.Show();
            this.Close();
        }

        private void markListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkList ml = new MarkList();
            ml.Show();
            this.Close();
        }

        private void attendanceSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AttendanceSheet att = new AttendanceSheet();
            att.Show();
            this.Close();
        }

        private void cOCListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            COCList cl = new COCList();
            cl.Show();
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

        private void COCList_Load(object sender, EventArgs e)
        {

        }

        private void generateButton_Click_1(object sender, EventArgs e)
        {
            ReportDocument cryRpt = new ReportDocument();
            string startupPath = Application.StartupPath;
            string reportPath = Path.GetFullPath(Path.Combine(startupPath, @"..\..\"));
            cryRpt.Load(reportPath + "COCReport.rpt");

            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinitions crParameterFieldDefinitions2;
            ParameterFieldDefinitions crParameterFieldDefinitions3;
            ParameterFieldDefinitions crParameterFieldDefinitions4;
            ParameterFieldDefinitions crParameterFieldDefinitions5;
            ParameterFieldDefinition crParameterFieldDefinition;
            ParameterFieldDefinition crParameterFieldDefinition2;
            ParameterFieldDefinition crParameterFieldDefinition3;
            ParameterFieldDefinition crParameterFieldDefinition4;
            ParameterFieldDefinition crParameterFieldDefinition5;
            ParameterValues crParameterValues = new ParameterValues();
            ParameterValues crParameterValues2 = new ParameterValues();
            ParameterValues crParameterValues3 = new ParameterValues();
            ParameterValues crParameterValues4 = new ParameterValues();
            ParameterValues crParameterValues5 = new ParameterValues();
            ParameterDiscreteValue crParameterDiscreteValue = new ParameterDiscreteValue();
            ParameterDiscreteValue crParameterDiscreteValue2 = new ParameterDiscreteValue();
            ParameterDiscreteValue crParameterDiscreteValue3 = new ParameterDiscreteValue();
            ParameterDiscreteValue crParameterDiscreteValue4 = new ParameterDiscreteValue();
            ParameterDiscreteValue crParameterDiscreteValue5 = new ParameterDiscreteValue();

            crParameterDiscreteValue.Value = deptID.Text;
            crParameterFieldDefinitions = cryRpt.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["deptid"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;

            crParameterDiscreteValue2.Value = streamID.Text;
            crParameterFieldDefinitions2 = cryRpt.DataDefinition.ParameterFields;
            crParameterFieldDefinition2 = crParameterFieldDefinitions2["streamid"];
            crParameterValues2 = crParameterFieldDefinition2.CurrentValues;

            crParameterDiscreteValue3.Value = level.Text;
            crParameterFieldDefinitions3 = cryRpt.DataDefinition.ParameterFields;
            crParameterFieldDefinition3 = crParameterFieldDefinitions3["level"];
            crParameterValues3 = crParameterFieldDefinition3.CurrentValues;
            
            crParameterDiscreteValue4.Value = admissionType.Text;
            crParameterFieldDefinitions4 = cryRpt.DataDefinition.ParameterFields;
            crParameterFieldDefinition4 = crParameterFieldDefinitions4["admissiontype"];
            crParameterValues4 = crParameterFieldDefinition4.CurrentValues;

            crParameterDiscreteValue5.Value = academicYear.Text;
            crParameterFieldDefinitions5 = cryRpt.DataDefinition.ParameterFields;
            crParameterFieldDefinition5 = crParameterFieldDefinitions5["admissiondate"];
            crParameterValues5 = crParameterFieldDefinition5.CurrentValues;

            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crParameterValues2.Clear();
            crParameterValues2.Add(crParameterDiscreteValue2);
            crParameterFieldDefinition2.ApplyCurrentValues(crParameterValues2);

            crParameterValues3.Clear();
            crParameterValues3.Add(crParameterDiscreteValue3);
            crParameterFieldDefinition3.ApplyCurrentValues(crParameterValues3);

            crParameterValues4.Clear();
            crParameterValues4.Add(crParameterDiscreteValue4);
            crParameterFieldDefinition4.ApplyCurrentValues(crParameterValues4);

            crParameterValues5.Clear();
            crParameterValues5.Add(crParameterDiscreteValue5);
            crParameterFieldDefinition5.ApplyCurrentValues(crParameterValues5);

            cryRptCOCList.ReportSource = cryRpt;
            cryRptCOCList.Refresh();
        }

        //---End Menu---
    }
}
