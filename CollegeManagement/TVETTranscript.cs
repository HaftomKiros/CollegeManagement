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
using CrystalDecisions.Windows.Forms;

namespace CollegeManagement
{
    public partial class TVETTranscript : Form
    {
        public TVETTranscript()
        {
            InitializeComponent();
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            ReportDocument cryRpt = new ReportDocument();
            string startupPath = Application.StartupPath;
            string reportPath = Path.GetFullPath(Path.Combine(startupPath, @"..\..\"));
            cryRpt.Load(reportPath + "TranscriptReport.rpt");

            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinitions crParameterFieldDefinitions2;
            ParameterFieldDefinitions crParameterFieldDefinitions3;
            ParameterFieldDefinition crParameterFieldDefinition;
            ParameterFieldDefinition crParameterFieldDefinition2;
            ParameterFieldDefinition crParameterFieldDefinition3;
            ParameterValues crParameterValues = new ParameterValues();
            ParameterValues crParameterValues2 = new ParameterValues();
            ParameterValues crParameterValues3 = new ParameterValues();
            ParameterDiscreteValue crParameterDiscreteValue = new ParameterDiscreteValue();
            ParameterDiscreteValue crParameterDiscreteValue2 = new ParameterDiscreteValue();
            ParameterDiscreteValue crParameterDiscreteValue3 = new ParameterDiscreteValue();

            crParameterDiscreteValue.Value = studID.Text;
            crParameterFieldDefinitions = cryRpt.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["studid"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;

            crParameterDiscreteValue2.Value = level.Text;
            crParameterFieldDefinitions2 = cryRpt.DataDefinition.ParameterFields;
            crParameterFieldDefinition2 = crParameterFieldDefinitions2["level"];
            crParameterValues2 = crParameterFieldDefinition2.CurrentValues;

            crParameterDiscreteValue3.Value = academicYear.Text;
            crParameterFieldDefinitions3 = cryRpt.DataDefinition.ParameterFields;
            crParameterFieldDefinition3 = crParameterFieldDefinitions3["academicyear"];
            crParameterValues3 = crParameterFieldDefinition3.CurrentValues;

            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crParameterValues2.Clear();
            crParameterValues2.Add(crParameterDiscreteValue2);
            crParameterFieldDefinition2.ApplyCurrentValues(crParameterValues2);

            crParameterValues3.Clear();
            crParameterValues3.Add(crParameterDiscreteValue3);
            crParameterFieldDefinition3.ApplyCurrentValues(crParameterValues3);

            cryRptTranscript.ReportSource = cryRpt;
            cryRptTranscript.Refresh();

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

        //---End Menu---
    }
}
