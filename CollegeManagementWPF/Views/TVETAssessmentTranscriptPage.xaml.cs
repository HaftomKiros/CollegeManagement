using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class TVETAssessmentTranscriptPage : Page
    {
        private readonly DBConnect _db = new DBConnect();
        private List<string> _allStudentIds = new();
        private bool _suppress = false;
        private const string LOGO_PATH = @"C:\Users\IN-TECH\Desktop\welday\C# source code\2018 CollegeManagement\transcript logo.png";

        public TVETAssessmentTranscriptPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            Loaded += async (s, e) => await LoadStudentIdsAsync();
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private async Task LoadStudentIdsAsync()
        {
            try
            {
                _allStudentIds = await Task.Run(() =>
                {
                    var list = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT DISTINCT TRIM(student_id) FROM ecc_dof_wukrostmarycollege.student_assessment ORDER BY student_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) { var v = r[0]?.ToString() ?? ""; if (!string.IsNullOrEmpty(v)) list.Add(v); }
                    conn.Close(); return list;
                });
                RefreshStudentDropdown("");
            }
            catch { }
        }

        private void RefreshStudentDropdown(string filter)
        {
            _suppress = true;
            CmbStudID.Items.Clear();
            foreach (var id in _allStudentIds)
                if (string.IsNullOrEmpty(filter) || id.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    CmbStudID.Items.Add(new ComboBoxItem { Content = id });
            _suppress = false;
        }

        private void CmbStudID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            RefreshStudentDropdown(CmbStudID.Text?.Trim() ?? "");
            if (!string.IsNullOrEmpty(CmbStudID.Text) && CmbStudID.Items.Count > 0)
                CmbStudID.IsDropDownOpen = true;
        }

        private async void CmbStudID_LostFocus(object sender, RoutedEventArgs e)
        {
            string sid = GetStudentId();
            if (string.IsNullOrEmpty(sid)) return;
            await LoadStudentNameAsync(sid);
            await LoadLevelsAsync(sid);
        }

        private async Task LoadStudentNameAsync(string sid)
        {
            try
            {
                string name = await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT TRIM(CONCAT(IFNULL(first_name,''),' ',IFNULL(father_name,''),' ',IFNULL(grand_father_name,''))) " +
                        "FROM ecc_dof_wukrostmarycollege.student_profile WHERE TRIM(student_id)=@s LIMIT 1", conn);
                    cmd.Parameters.AddWithValue("@s", sid);
                    var n = cmd.ExecuteScalar()?.ToString()?.Trim() ?? "";
                    conn.Close(); return n;
                });
                TxtStudentName.Text = name;
                TxtStudentName.Visibility = string.IsNullOrEmpty(name) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch { TxtStudentName.Visibility = Visibility.Collapsed; }
        }

        private async Task LoadLevelsAsync(string sid)
        {
            try
            {
                var levels = await Task.Run(() =>
                {
                    var list = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT DISTINCT level FROM ecc_dof_wukrostmarycollege.student_assessment WHERE TRIM(student_id)=@s ORDER BY level", conn);
                    cmd.Parameters.AddWithValue("@s", sid);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close(); return list;
                });
                _suppress = true;
                CmbLevel.Items.Clear();
                foreach (var l in levels) CmbLevel.Items.Add(new ComboBoxItem { Content = l });
                if (CmbLevel.Items.Count > 0) CmbLevel.SelectedIndex = 0;
                _suppress = false;
                string lv = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                if (!string.IsNullOrEmpty(lv)) await LoadAcadYearsAsync(sid, lv);
            }
            catch { }
        }

        private async void CmbLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string sid = GetStudentId();
            string lv = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            if (!string.IsNullOrEmpty(sid) && !string.IsNullOrEmpty(lv))
                await LoadAcadYearsAsync(sid, lv);
        }

        private async Task LoadAcadYearsAsync(string sid, string level)
        {
            try
            {
                var years = await Task.Run(() =>
                {
                    var list = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT DISTINCT academic_year FROM ecc_dof_wukrostmarycollege.student_assessment WHERE TRIM(student_id)=@s AND level=@l ORDER BY academic_year", conn);
                    cmd.Parameters.AddWithValue("@s", sid);
                    cmd.Parameters.AddWithValue("@l", level);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close(); return list;
                });
                CmbAcadYear.Items.Clear();
                foreach (var y in years) CmbAcadYear.Items.Add(new ComboBoxItem { Content = y });
                if (CmbAcadYear.Items.Count > 0) CmbAcadYear.SelectedIndex = 0;
            }
            catch { }
        }

        private string GetStudentId()
        {
            if (CmbStudID.SelectedItem is ComboBoxItem sel) return sel.Content?.ToString()?.Trim() ?? "";
            return CmbStudID.Text?.Trim() ?? "";
        }

        private static string TryGet(DataRowView drv, string col)
        {
            try { return drv[col]?.ToString()?.Trim() ?? ""; } catch { return ""; }
        }

        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            string sid = GetStudentId();
            if (string.IsNullOrEmpty(sid))
            { ModernDialog.Show(Window.GetWindow(this), "Please select a Student ID!", "Error", ModernDialog.DialogType.Error); return; }
            string lvl = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string ay  = (CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            try
            {
                var dt = new DataTable();
                string sql =
                    "SELECT sa.student_id,sa.level,sa.module_code," +
                    "IFNULL(c.unit_of_competence_title,'') AS unit_title," +
                    "IFNULL(c.total_hours,0) AS total_hours," +
                    "sa.employee_id,sa.academic_year,sa.institutional_score,sa.industry_score," +
                    "sa.total_score,sa.letter_grade,sa.grade_points " +
                    "FROM ecc_dof_wukrostmarycollege.student_assessment sa " +
                    "LEFT JOIN ecc_dof_wukrostmarycollege.courses c ON sa.module_code=c.module_code " +
                    "WHERE TRIM(sa.student_id)=@s";
                if (!string.IsNullOrEmpty(lvl)) sql += " AND sa.level=@l";
                if (!string.IsNullOrEmpty(ay))  sql += " AND sa.academic_year=@y";
                sql += " ORDER BY sa.module_code";
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    var cmd  = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@s", sid);
                    if (!string.IsNullOrEmpty(lvl)) cmd.Parameters.AddWithValue("@l", lvl);
                    if (!string.IsNullOrEmpty(ay))  cmd.Parameters.AddWithValue("@y", ay);
                    new MySqlDataAdapter(cmd).Fill(dt);
                });
                Grid1.ItemsSource = dt.DefaultView;
                PreviewCard.Visibility = Visibility.Visible;
                TxtPreviewInfo.Text = $"Assessment Transcript: {sid}" +
                    (string.IsNullOrEmpty(lvl) ? "" : $" | Level {lvl}") +
                    (string.IsNullOrEmpty(ay)  ? "" : $" | Year {ay}") +
                    $" — {dt.Rows.Count} record(s)";
            }
            catch (Exception ex)
            { ModernDialog.Show(Window.GetWindow(this), "Error: " + ex.Message, "DB Error", ModernDialog.DialogType.Error); }
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not DataView view || view.Count == 0)
            { ModernDialog.Show(Window.GetWindow(this), "Generate first.", "Info", ModernDialog.DialogType.Info); return; }
            var dlg = new Microsoft.Win32.SaveFileDialog
            { FileName = $"AssessmentTranscript_{GetStudentId().Replace("/","_")}", DefaultExt = ".pdf", Filter = "PDF|*.pdf" };
            if (dlg.ShowDialog() != true) return;

            string path = dlg.FileName, sid = GetStudentId();
            string lvl = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string ay  = (CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            var rows = new List<(string Mod, string Unit, string Hours, string Score, string Grade, string Points)>();
            foreach (DataRowView drv in view)
                rows.Add((TryGet(drv,"module_code"), TryGet(drv,"unit_title"), TryGet(drv,"total_hours"),
                          TryGet(drv,"total_score"), TryGet(drv,"letter_grade"), TryGet(drv,"grade_points")));

            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    string fullName="",deptName="",streamName="",admType="",gender="";
                    using (var cmd = new MySqlCommand(
                        "SELECT TRIM(CONCAT(IFNULL(first_name,''),' ',IFNULL(father_name,''),' ',IFNULL(grand_father_name,''))),dept_id,stream_id,admission_type,gender " +
                        "FROM ecc_dof_wukrostmarycollege.student_profile WHERE TRIM(student_id)=@s LIMIT 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@s", sid);
                        using var r = cmd.ExecuteReader();
                        if (r.Read()) { fullName=r[0]?.ToString()?.Trim()??""; string di=r[1]?.ToString()?.Trim()??"",si=r[2]?.ToString()?.Trim()??""; admType=r[3]?.ToString()?.Trim()??""; gender=r[4]?.ToString()?.Trim()??""; r.Close(); using var c2=new MySqlCommand("SELECT IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments WHERE dept_id=@d LIMIT 1",conn); c2.Parameters.AddWithValue("@d",di); deptName=c2.ExecuteScalar()?.ToString()??di; using var c3=new MySqlCommand("SELECT IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE stream_id=@s LIMIT 1",conn); c3.Parameters.AddWithValue("@s",si); streamName=c3.ExecuteScalar()?.ToString()??si; }
                    }
                    var gradeConfig = new List<(double,double,string,double)>();
                    using (var cmd = new MySqlCommand("SELECT min_score,max_score,letter_grade,grade_points FROM ecc_dof_wukrostmarycollege.grade_config ORDER BY min_score DESC",conn))
                    using (var r = cmd.ExecuteReader())
                        while(r.Read()) gradeConfig.Add((Convert.ToDouble(r[0]),Convert.ToDouble(r[1]),r[2]?.ToString()??"",Convert.ToDouble(r[3])));

                    // Cumulative GPA: all levels <= current level for this student
                    double cumulativeGPA = 0;
                    using (var cmd = new MySqlCommand(
                        "SELECT SUM(IFNULL(c.total_hours,0)*sa.grade_points)/NULLIF(SUM(IFNULL(c.total_hours,0)),0) " +
                        "FROM ecc_dof_wukrostmarycollege.student_assessment sa " +
                        "LEFT JOIN ecc_dof_wukrostmarycollege.courses c ON sa.module_code=c.module_code " +
                        "WHERE TRIM(sa.student_id)=@s AND sa.level<=@l", conn))
                    {
                        cmd.Parameters.AddWithValue("@s", sid);
                        cmd.Parameters.AddWithValue("@l", string.IsNullOrEmpty(lvl) ? "1" : lvl);
                        var res = cmd.ExecuteScalar();
                        if (res != DBNull.Value && res != null) cumulativeGPA = Math.Round(Convert.ToDouble(res), 3);
                    }
                    conn.Close();

                    double totalHours=0, totalGIP=0;
                    foreach(var(_,_,hours,_,_,pts) in rows) { if(!double.TryParse(hours,out double h)) h=0; if(!double.TryParse(pts,out double p)) p=0; totalHours+=h; totalGIP+=h*p; }
                    double gpa = totalHours>0 ? Math.Round(totalGIP/totalHours,3) : 0;
                    string lvlR = lvl switch {"1"=>"I","2"=>"II","3"=>"III","4"=>"IV",_=>lvl};

                    var doc = new MigraDoc.DocumentObjectModel.Document();
                    if (doc.Styles["Normal"] is {} ns) { ns.Font.Name="Times New Roman"; ns.Font.Size=10; }
                    var sec = doc.AddSection();
                    sec.PageSetup.PageFormat=MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.Orientation=MigraDoc.DocumentObjectModel.Orientation.Portrait;
                    sec.PageSetup.TopMargin="1.5cm"; sec.PageSetup.BottomMargin="1.8cm";
                    sec.PageSetup.LeftMargin="1.5cm"; sec.PageSetup.RightMargin="1.5cm";

                    bool logoExists = System.IO.File.Exists(LOGO_PATH);
                    var ht = sec.AddTable(); ht.Borders.Width=0; ht.AddColumn("2.5cm"); ht.AddColumn("12.7cm"); ht.AddColumn("2.5cm");
                    var hr = ht.AddRow(); hr.VerticalAlignment=MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    if (logoExists) { var i1=hr.Cells[0].AddImage(LOGO_PATH); i1.Width="2.2cm"; i1.Height="2.2cm"; hr.Cells[0].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center; }
                    void HL(string t, double sz) { var p=hr.Cells[1].AddParagraph(t); p.Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center; p.Format.Font.Bold=true; p.Format.Font.Size=sz; p.Format.Font.Name="Times New Roman"; p.Format.SpaceBefore="0mm"; p.Format.SpaceAfter="0.5mm"; }
                    HL("ETHIOPIAN CATHOLIC CHURCH DIOCESE OF ADIGRAT",11); HL("WUKRO ST. MARY'S COLLEGE",11); HL("REGISTRAR'S OFFICE",11); HL("TRAINEES COPY",11);
                    if (logoExists) { var i2=hr.Cells[2].AddImage(LOGO_PATH); i2.Width="2.2cm"; i2.Height="2.2cm"; hr.Cells[2].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center; }

                    var cp=sec.AddParagraph("P.O.Box 12  Website: www.wstmci.edu.et  E.Mail: wstmarycc@gmail.com  FB: StMarycc Wukro");
                    cp.Format.Font.Size=8.5; cp.Format.Font.Name="Times New Roman"; cp.Format.SpaceBefore="1mm"; cp.Format.SpaceAfter="2mm";
                    cp.Format.Borders.Bottom.Width=0.5; cp.Format.Borders.Bottom.Color=MigraDoc.DocumentObjectModel.Colors.Black;

                    void IL(string label, string val) { var p=sec.AddParagraph(); p.Format.Font.Size=10.5; p.Format.Font.Name="Times New Roman"; p.Format.SpaceAfter="0.5mm"; p.AddFormattedText(label,MigraDoc.DocumentObjectModel.TextFormat.NotBold); var v=p.AddFormattedText(val,MigraDoc.DocumentObjectModel.TextFormat.Bold); v.Underline=MigraDoc.DocumentObjectModel.Underline.Single; }
                    IL("Occupational Title: ",$"{streamName} Level {lvlR}"); IL("Name of Trainee: ",fullName); IL("Department: ",deptName); IL("Student ID No.: ",sid); IL("Gender: ",gender); IL("Admission Type: ",admType); IL("Academic Year: ",(string.IsNullOrEmpty(ay)?"":ay)+" E.C.");
                    sec.AddParagraph().Format.SpaceAfter="3mm";

                    var tbl=sec.AddTable(); tbl.Borders.Width=0.5; tbl.Borders.Color=MigraDoc.DocumentObjectModel.Colors.Black; tbl.Format.Font.Size=9; tbl.Format.Font.Name="Times New Roman"; tbl.TopPadding="1mm"; tbl.BottomPadding="1mm";
                    tbl.AddColumn("0.7cm"); tbl.AddColumn("2.8cm"); tbl.AddColumn("5.5cm"); tbl.AddColumn("1.2cm"); tbl.AddColumn("1.2cm"); tbl.AddColumn("1.2cm"); tbl.AddColumn("1.4cm"); tbl.AddColumn("1.7cm");
                    var hdrR=tbl.AddRow(); hdrR.HeadingFormat=true; hdrR.Format.Font.Bold=true; hdrR.VerticalAlignment=MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    string[] hdrs={"No","Module Code","Unit of competence Title","Total\nHours","Grade\nIn\nPoint","Grade\nIn\nLetter","Grade\npoints\nValue","Grade\nIn\nPoint"};
                    for(int ci=0;ci<hdrs.Length;ci++){hdrR.Cells[ci].AddParagraph(hdrs[ci]);hdrR.Cells[ci].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;}

                    for(int i=0;i<rows.Count;i++){
                        var(mod,unit,hours,score,grade,pts)=rows[i];
                        double.TryParse(hours,out double hh); double.TryParse(pts,out double pp); double gip=Math.Round(hh*pp,1);
                        var row=tbl.AddRow(); row.VerticalAlignment=MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row.Cells[0].AddParagraph((i+1).ToString()); row.Cells[0].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[1].AddParagraph(mod);
                        row.Cells[2].AddParagraph(unit).Format.Font.Bold=true;
                        row.Cells[3].AddParagraph(hh>0?hh.ToString("0"):hours); row.Cells[3].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[4].AddParagraph(score); row.Cells[4].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[5].AddParagraph(grade); row.Cells[5].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[6].AddParagraph(pp>0?pp.ToString("0.##"):pts); row.Cells[6].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[7].AddParagraph(gip>0?gip.ToString("0.#"):""); row.Cells[7].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    }
                    var totR=tbl.AddRow(); totR.Format.Font.Bold=true; totR.Cells[0].MergeRight=2; totR.Cells[0].AddParagraph("Total Numbers of Hours & Grade In Point"); totR.Cells[0].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center; totR.Cells[3].AddParagraph(totalHours.ToString("0")); totR.Cells[3].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center; totR.Cells[7].AddParagraph(totalGIP.ToString("0.#")); totR.Cells[7].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    var gpaR=tbl.AddRow(); gpaR.Format.Font.Bold=true; gpaR.Cells[0].MergeRight=6; gpaR.Cells[0].AddParagraph($"Grade Point Average of Level {lvlR}"); gpaR.Cells[0].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center; gpaR.Cells[7].AddParagraph(gpa.ToString("0.###")); gpaR.Cells[7].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;

                    // Cumulative GPA row — only show if student has previous levels
                    if (cumulativeGPA > 0 && !string.IsNullOrEmpty(lvl) && lvl != "1")
                    {
                        // Build roman numeral range e.g. I-II, I-III
                        string cumLabel = $"Cumulative Grade Point Average of Level I-{lvlR}";
                        var cumR = tbl.AddRow();
                        cumR.Format.Font.Bold = true;
                        cumR.Cells[0].MergeRight = 6;
                        cumR.Cells[0].AddParagraph(cumLabel);
                        cumR.Cells[0].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        cumR.Cells[7].AddParagraph(cumulativeGPA.ToString("0.###"));
                        cumR.Cells[7].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    }

                    sec.AddParagraph().Format.SpaceAfter="3mm";
                    var nb=sec.AddParagraph(); nb.Format.Font.Size=8.5; nb.Format.Font.Name="Times New Roman";
                    nb.AddFormattedText("NB: Grading System Criteria Referenced Based on\n",MigraDoc.DocumentObjectModel.TextFormat.Bold);
                    var sb=new System.Text.StringBuilder();
                    foreach(var(mn,mx,lt,pt2) in gradeConfig){ if(mn<=0) sb.Append($"<={mx:0}= (F=NYC)  "); else if(mx>=100) sb.Append($"{lt} = {mn:0} - 100%,({lt}={pt2:0.##})  "); else sb.Append($"{lt} = {mn:0} - {mx:0}%,({lt}={pt2:0.##})  "); }
                    nb.AddText(sb.ToString());

                    sec.AddParagraph().Format.SpaceAfter="12mm";
                    var st=sec.AddTable(); st.Borders.Width=0; st.AddColumn("9cm"); st.AddColumn("9cm");
                    var sr1=st.AddRow(); sr1.Format.Font.Size=10; sr1.Format.Font.Name="Times New Roman"; sr1.Cells[0].AddParagraph("___________________"); sr1.Cells[1].AddParagraph("____________________");
                    var sr2=st.AddRow(); sr2.Format.Font.Size=10; sr2.Format.Font.Name="Times New Roman"; sr2.Cells[0].AddParagraph("Head of the Registrar"); sr2.Cells[1].AddParagraph("Dean of College");
                    var sr3=st.AddRow(); sr3.Format.SpaceBefore="5mm"; sr3.Cells[0].MergeRight=1; var dp=sr3.Cells[0].AddParagraph(); dp.Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center; dp.Format.Font.Bold=true; dp.Format.Font.Size=10.5; dp.Format.Font.Name="Times New Roman"; dp.AddText($"Date of Issue:      {DateTime.Now:MMMM dd,yyyy} G.C");

                    var ren=new MigraDoc.Rendering.PdfDocumentRenderer{Document=doc}; ren.RenderDocument(); ren.PdfDocument.Save(path);
                });
                ModernDialog.Show(Window.GetWindow(this),"PDF saved!","Success",ModernDialog.DialogType.Success);
            }
            catch(Exception ex){ModernDialog.Show(Window.GetWindow(this),"PDF failed: "+ex.Message,"Error",ModernDialog.DialogType.Error);}
        }

        private async void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not DataView view || view.Count == 0)
            { ModernDialog.Show(Window.GetWindow(this), "Generate first.", "Info", ModernDialog.DialogType.Info); return; }
            var dlg = new Microsoft.Win32.SaveFileDialog
            { FileName = $"AssessmentTranscript_{GetStudentId().Replace("/","_")}_{DateTime.Now:yyyyMMdd}", DefaultExt = ".xlsx", Filter = "Excel Workbook|*.xlsx" };
            if (dlg.ShowDialog() != true) return;

            string path = dlg.FileName, sid = GetStudentId();
            string lvl = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string ay  = (CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            var rows = new List<(string Mod, string Unit, string Hours, string Score, string Grade, string Points)>();
            foreach (DataRowView drv in view)
                rows.Add((TryGet(drv,"module_code"), TryGet(drv,"unit_title"), TryGet(drv,"total_hours"),
                          TryGet(drv,"total_score"), TryGet(drv,"letter_grade"), TryGet(drv,"grade_points")));
            try
            {
                await Task.Run(() =>
                {
                    double totalHours=0, totalGIP=0;
                    foreach(var(_,_,hours,_,_,pts) in rows) { if(!double.TryParse(hours,out double h)) h=0; if(!double.TryParse(pts,out double p)) p=0; totalHours+=h; totalGIP+=h*p; }
                    double gpa = totalHours>0 ? Math.Round(totalGIP/totalHours,3) : 0;
                    string lvlR = lvl switch {"1"=>"I","2"=>"II","3"=>"III","4"=>"IV",_=>lvl};

                    string fullName="",deptName="",streamName="",admType="",gender="";
                    double cumulativeGPA=0;
                    var gradeConfig = new List<(double,double,string,double)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using (var cmd = new MySqlCommand("SELECT TRIM(CONCAT(IFNULL(first_name,''),' ',IFNULL(father_name,''),' ',IFNULL(grand_father_name,''))),dept_id,stream_id,admission_type,gender FROM ecc_dof_wukrostmarycollege.student_profile WHERE TRIM(student_id)=@s LIMIT 1",conn))
                    { cmd.Parameters.AddWithValue("@s",sid); using var r=cmd.ExecuteReader(); if(r.Read()){fullName=r[0]?.ToString()?.Trim()??"";string di=r[1]?.ToString()?.Trim()??"",si=r[2]?.ToString()?.Trim()??"";admType=r[3]?.ToString()?.Trim()??"";gender=r[4]?.ToString()?.Trim()??"";r.Close();using var c2=new MySqlCommand("SELECT IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments WHERE dept_id=@d LIMIT 1",conn);c2.Parameters.AddWithValue("@d",di);deptName=c2.ExecuteScalar()?.ToString()??di;using var c3=new MySqlCommand("SELECT IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE stream_id=@s LIMIT 1",conn);c3.Parameters.AddWithValue("@s",si);streamName=c3.ExecuteScalar()?.ToString()??si;} }
                    using (var cmd=new MySqlCommand(
                        "SELECT SUM(IFNULL(c.total_hours,0)*sa.grade_points)/NULLIF(SUM(IFNULL(c.total_hours,0)),0) " +
                        "FROM ecc_dof_wukrostmarycollege.student_assessment sa " +
                        "LEFT JOIN ecc_dof_wukrostmarycollege.courses c ON sa.module_code=c.module_code " +
                        "WHERE TRIM(sa.student_id)=@s AND sa.level<=@l",conn))
                    { cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",string.IsNullOrEmpty(lvl)?"1":lvl); var res=cmd.ExecuteScalar(); if(res!=DBNull.Value&&res!=null) cumulativeGPA=Math.Round(Convert.ToDouble(res),3); }
                    using (var cmd=new MySqlCommand("SELECT min_score,max_score,letter_grade,grade_points FROM ecc_dof_wukrostmarycollege.grade_config ORDER BY min_score DESC",conn)) using(var r=cmd.ExecuteReader()) while(r.Read()) gradeConfig.Add((Convert.ToDouble(r[0]),Convert.ToDouble(r[1]),r[2]?.ToString()??"",Convert.ToDouble(r[3])));
                    conn.Close();

                    using var wb = new ClosedXML.Excel.XLWorkbook();
                    var ws = wb.Worksheets.Add("Transcript");
                    int totalCols=8;
                    void MergeAll(int row,string val,bool bold=false,int fontSize=10){ws.Range(row,1,row,totalCols).Merge();ws.Cell(row,1).Value=val;ws.Cell(row,1).Style.Font.FontSize=fontSize;ws.Cell(row,1).Style.Font.Bold=bold;ws.Cell(row,1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;}
                    void InfoRow(int row,string label,string val){ws.Range(row,1,row,totalCols).Merge();ws.Cell(row,1).Value=label+val;ws.Cell(row,1).Style.Font.Bold=true;ws.Cell(row,1).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;}

                    int r2=1;
                    MergeAll(r2,"ETHIOPIAN CATHOLIC CHURCH DIOCESE OF ADIGRAT",true,12);r2++;
                    MergeAll(r2,"WUKRO ST. MARY'S COLLEGE",true,12);r2++;
                    MergeAll(r2,"REGISTRAR'S OFFICE",true,11);r2++;
                    MergeAll(r2,"TRAINEES COPY",true,11);r2++;
                    MergeAll(r2,"P.O.Box 12  Website: www.wstmci.edu.et  E.Mail: wstmarycc@gmail.com  FB: StMarycc Wukro",false,9);
                    ws.Row(r2).Style.Border.BottomBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;r2++;

                    InfoRow(r2,"Occupational Title: ",$"{streamName} Level {lvlR}");r2++;
                    InfoRow(r2,"Name of Trainee: ",fullName);r2++;
                    InfoRow(r2,"Department: ",deptName);r2++;
                    InfoRow(r2,"Student ID No.: ",sid);r2++;
                    InfoRow(r2,"Gender: ",gender);r2++;
                    InfoRow(r2,"Admission Type: ",admType);r2++;
                    InfoRow(r2,"Academic Year: ",(string.IsNullOrEmpty(ay)?"":ay)+" E.C.");r2++;
                    r2++;

                    string[] headers={"No","Module Code","Unit of competence Title","Total\nHours","Grade\nIn\nPoint","Grade\nIn\nLetter","Grade\npoints\nValue","Grade\nIn\nPoint"};
                    for(int c=0;c<headers.Length;c++){var cell=ws.Cell(r2,c+1);cell.Value=headers[c];cell.Style.Font.Bold=true;cell.Style.Font.FontSize=9;cell.Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;cell.Style.Alignment.Vertical=ClosedXML.Excel.XLAlignmentVerticalValues.Center;cell.Style.Alignment.WrapText=true;cell.Style.Border.OutsideBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;cell.Style.Fill.BackgroundColor=ClosedXML.Excel.XLColor.FromHtml("#D9E1F2");}
                    ws.Row(r2).Height=36;r2++;

                    for(int i=0;i<rows.Count;i++){
                        var(mod,unit,hours,score,grade,pts)=rows[i];
                        double.TryParse(hours,out double hh);double.TryParse(pts,out double pp);double gip=Math.Round(hh*pp,1);
                        ws.Cell(r2,1).Value=i+1;ws.Cell(r2,1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                        ws.Cell(r2,2).Value=mod;
                        ws.Cell(r2,3).Value=unit;ws.Cell(r2,3).Style.Font.Bold=true;
                        ws.Cell(r2,4).Value=hh>0?hh:0;ws.Cell(r2,4).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                        ws.Cell(r2,5).Value=score;ws.Cell(r2,5).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                        ws.Cell(r2,6).Value=grade;ws.Cell(r2,6).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        ws.Cell(r2,7).Value=pp>0?pp:0;ws.Cell(r2,7).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                        ws.Cell(r2,8).Value=gip>0?gip:0;ws.Cell(r2,8).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                        for(int c=1;c<=8;c++) ws.Cell(r2,c).Style.Border.OutsideBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;
                        r2++;
                    }

                    ws.Range(r2,1,r2,3).Merge();ws.Cell(r2,1).Value="Total Numbers of Hours & Grade In Point";ws.Cell(r2,1).Style.Font.Bold=true;ws.Cell(r2,1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    ws.Cell(r2,4).Value=totalHours;ws.Cell(r2,4).Style.Font.Bold=true;ws.Cell(r2,4).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;ws.Cell(r2,4).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                    ws.Cell(r2,8).Value=totalGIP;ws.Cell(r2,8).Style.Font.Bold=true;ws.Cell(r2,8).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;ws.Cell(r2,8).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                    for(int c=1;c<=8;c++) ws.Cell(r2,c).Style.Border.OutsideBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;r2++;

                    ws.Range(r2,1,r2,7).Merge();ws.Cell(r2,1).Value=$"Grade Point Average of Level {lvlR}";ws.Cell(r2,1).Style.Font.Bold=true;ws.Cell(r2,1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    ws.Cell(r2,8).Value=gpa;ws.Cell(r2,8).Style.Font.Bold=true;ws.Cell(r2,8).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;ws.Cell(r2,8).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                    for(int c=1;c<=8;c++) ws.Cell(r2,c).Style.Border.OutsideBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;r2++;

                    // Cumulative row — only when student has previous levels
                    if (cumulativeGPA > 0 && !string.IsNullOrEmpty(lvl) && lvl != "1")
                    {
                        ws.Range(r2,1,r2,7).Merge();ws.Cell(r2,1).Value=$"Cumulative Grade Point Average of Level I-{lvlR}";ws.Cell(r2,1).Style.Font.Bold=true;ws.Cell(r2,1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        ws.Cell(r2,8).Value=cumulativeGPA;ws.Cell(r2,8).Style.Font.Bold=true;ws.Cell(r2,8).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;ws.Cell(r2,8).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                        for(int c=1;c<=8;c++) ws.Cell(r2,c).Style.Border.OutsideBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;r2++;
                    }
                    r2++;

                    var sb2=new System.Text.StringBuilder("NB: Grading System Criteria Referenced Based on\n");
                    foreach(var(mn,mx,lt,pt2) in gradeConfig){if(mn<=0) sb2.Append($"<={mx:0}= (F=NYC)  ");else if(mx>=100) sb2.Append($"{lt} = {mn:0} - 100%,({lt}={pt2:0.##})  ");else sb2.Append($"{lt} = {mn:0} - {mx:0}%,({lt}={pt2:0.##})  ");}
                    ws.Range(r2,1,r2,totalCols).Merge();ws.Cell(r2,1).Value=sb2.ToString();ws.Cell(r2,1).Style.Font.FontSize=8.5;ws.Cell(r2,1).Style.Alignment.WrapText=true;ws.Row(r2).Height=40;

                    ws.Column(1).Width=5;ws.Column(2).Width=18;ws.Column(3).Width=38;ws.Column(4).Width=8;ws.Column(5).Width=8;ws.Column(6).Width=8;ws.Column(7).Width=9;ws.Column(8).Width=10;

                    if (System.IO.File.Exists(LOGO_PATH))
                    {
                        var pic1=ws.AddPicture(LOGO_PATH); pic1.MoveTo(ws.Cell(1,1)).WithSize(60,60);
                        var pic2=ws.AddPicture(LOGO_PATH); pic2.MoveTo(ws.Cell(1,8)).WithSize(60,60);
                    }
                    wb.SaveAs(path);
                });
                ModernDialog.Show(Window.GetWindow(this),"Excel saved!","Success",ModernDialog.DialogType.Success);
            }
            catch(Exception ex){ModernDialog.Show(Window.GetWindow(this),"Export failed: "+ex.Message,"Error",ModernDialog.DialogType.Error);}
        }
    }
}
