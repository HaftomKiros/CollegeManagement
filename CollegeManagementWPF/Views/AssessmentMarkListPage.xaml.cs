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
    public partial class AssessmentMarkListPage : Page
    {
        private readonly DBConnect _db = new DBConnect();
        private bool _suppress = false;
        private List<(string Id, string Name)>     _allDepts      = new();
        private List<(string Id, string Name)>     _allStreams     = new();
        private List<(string LevelId, string Num)> _streamLevels  = new();
        private List<(string Code, string Title, string Hours)> _levelModules = new();
        private List<(string Id, string Name)>     _levelInstructors = new();

        public AssessmentMarkListPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            Loaded += async (s, e) => await LoadDeptsAsync();
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private async Task LoadDeptsAsync()
        {
            try
            {
                _allDepts = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand("SELECT dept_id,IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close(); return list;
                });
                RefreshDeptDropdown("");
            }
            catch { }
        }

        private void RefreshDeptDropdown(string filter)
        {
            _suppress = true;
            CmbDept.Items.Clear();
            foreach (var (id, name) in _allDepts)
                if (string.IsNullOrEmpty(filter) || id.Contains(filter, StringComparison.OrdinalIgnoreCase) || name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    CmbDept.Items.Add(new ComboBoxItem { Content = string.IsNullOrEmpty(name) ? id : $"{id} -- {name}", Tag = id });
            _suppress = false;
        }

        private void CmbDept_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            RefreshDeptDropdown(CmbDept.Text?.Trim() ?? "");
            if (CmbDept.Items.Count > 0 && !string.IsNullOrEmpty(CmbDept.Text)) CmbDept.IsDropDownOpen = true;
        }

        private async void CmbDept_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string did = GetTag(CmbDept);
            if (string.IsNullOrEmpty(did)) return;
            ClearBelow(CmbStream);
            await LoadStreamsAsync(did);
        }

        private string GetTag(ComboBox c)
        {
            if (c.SelectedItem is ComboBoxItem s && s.Tag != null) return s.Tag.ToString()!;
            string t = c.Text?.Trim() ?? ""; int d = t.IndexOf(" -- ");
            return d >= 0 ? t[..d].Trim() : t;
        }

        private async Task LoadStreamsAsync(string deptId)
        {
            try
            {
                _allStreams = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand("SELECT stream_id,IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE dept_id=@d ORDER BY stream_id", conn);
                    cmd.Parameters.AddWithValue("@d", deptId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close(); return list;
                });
                _suppress = true;
                CmbStream.Items.Clear();
                foreach (var (id, name) in _allStreams)
                    CmbStream.Items.Add(new ComboBoxItem { Content = string.IsNullOrEmpty(name) ? id : $"{id} -- {name}", Tag = id });
                if (CmbStream.Items.Count > 0) CmbStream.SelectedIndex = 0;
                _suppress = false;
            }
            catch { }
        }

        private void CmbStream_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            if (CmbStream.Items.Count > 0 && !string.IsNullOrEmpty(CmbStream.Text)) CmbStream.IsDropDownOpen = true;
        }

        private async void CmbStream_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string sid = GetTag(CmbStream);
            if (string.IsNullOrEmpty(sid)) return;
            ClearBelow(CmbLevel);
            await LoadLevelsAsync(sid);
        }

        private async Task LoadLevelsAsync(string streamId)
        {
            try
            {
                _streamLevels = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand("SELECT level_id,level FROM ecc_dof_wukrostmarycollege.levels WHERE stream_id=@s ORDER BY level", conn);
                    cmd.Parameters.AddWithValue("@s", streamId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close(); return list;
                });
                _suppress = true;
                CmbLevel.Items.Clear();
                foreach (var (lid, lnum) in _streamLevels)
                    CmbLevel.Items.Add(new ComboBoxItem { Content = $"{lid} -- {lnum}", Tag = lid });
                if (CmbLevel.Items.Count > 0) CmbLevel.SelectedIndex = 0;
                _suppress = false;
                string lvlId = GetTag(CmbLevel);
                if (!string.IsNullOrEmpty(lvlId)) await LoadModulesAsync(lvlId);
            }
            catch { }
        }

        private void CmbLevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            if (CmbLevel.Items.Count > 0 && !string.IsNullOrEmpty(CmbLevel.Text)) CmbLevel.IsDropDownOpen = true;
        }

        private async void CmbLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string lvlId = GetTag(CmbLevel);
            if (string.IsNullOrEmpty(lvlId)) return;
            ClearBelow(CmbModCode);
            await LoadModulesAsync(lvlId);
        }

        private string GetLevelNum()
        {
            string display = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            int dash = display.IndexOf(" -- "); return dash >= 0 ? display[(dash + 4)..].Trim() : display;
        }

        private async Task LoadModulesAsync(string levelId)
        {
            try
            {
                _levelModules = await Task.Run(() =>
                {
                    var list = new List<(string, string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand("SELECT module_code,IFNULL(unit_of_competence_title,''),IFNULL(total_hours,'0') FROM ecc_dof_wukrostmarycollege.courses WHERE level_id=@l ORDER BY module_code", conn);
                    cmd.Parameters.AddWithValue("@l", levelId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? "", r[2]?.ToString() ?? "0"));
                    conn.Close(); return list;
                });
                _suppress = true;
                CmbModCode.Items.Clear();
                foreach (var (code, title, _) in _levelModules)
                    CmbModCode.Items.Add(new ComboBoxItem { Content = string.IsNullOrEmpty(title) ? code : $"{code} -- {title}", Tag = code });
                if (CmbModCode.Items.Count > 0) CmbModCode.SelectedIndex = 0;
                _suppress = false;
                string mc = GetTag(CmbModCode);
                if (!string.IsNullOrEmpty(mc)) await LoadInstructorsAndYearsAsync(mc);
            }
            catch { }
        }

        private void CmbModCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            if (CmbModCode.Items.Count > 0 && !string.IsNullOrEmpty(CmbModCode.Text)) CmbModCode.IsDropDownOpen = true;
        }

        private async void CmbModCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string mc = GetTag(CmbModCode);
            if (string.IsNullOrEmpty(mc)) return;
            ClearBelow(CmbInstructor);
            await LoadInstructorsAndYearsAsync(mc);
        }

        private async Task LoadInstructorsAndYearsAsync(string moduleCode)
        {
            try
            {
                var (instructors, years) = await Task.Run(() =>
                {
                    var ilist = new List<(string, string)>();
                    var ylist = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT DISTINCT sa.employee_id, IFNULL(CONCAT(TRIM(ep.first_name),' ',TRIM(ep.middle_name),' ',TRIM(ep.last_name)),sa.employee_id) " +
                        "FROM ecc_dof_wukrostmarycollege.student_assessment sa " +
                        "LEFT JOIN ecc_dof_wukrostmarycollege.employee_profile ep ON sa.employee_id=ep.employee_id " +
                        "WHERE sa.module_code=@m AND sa.employee_id IS NOT NULL ORDER BY sa.employee_id", conn))
                    { cmd.Parameters.AddWithValue("@m", moduleCode); using var r=cmd.ExecuteReader(); while(r.Read()) ilist.Add((r[0]?.ToString()??"", r[1]?.ToString()?.Trim()??"")); }
                    using (var cmd = new MySqlCommand("SELECT DISTINCT academic_year FROM ecc_dof_wukrostmarycollege.student_assessment WHERE module_code=@m AND academic_year IS NOT NULL ORDER BY academic_year", conn))
                    { cmd.Parameters.AddWithValue("@m", moduleCode); using var r=cmd.ExecuteReader(); while(r.Read()) ylist.Add(r[0]?.ToString()??""); }
                    conn.Close(); return (ilist, ylist);
                });
                _levelInstructors = instructors;
                _suppress = true;
                CmbInstructor.Items.Clear();
                foreach (var (id, name) in instructors)
                    CmbInstructor.Items.Add(new ComboBoxItem { Content = $"{id} -- {name}", Tag = id });
                CmbAcadYear.Items.Clear();
                CmbAcadYear.Items.Add(new ComboBoxItem { Content = "" });
                foreach (var y in years) CmbAcadYear.Items.Add(new ComboBoxItem { Content = y });
                if (CmbAcadYear.Items.Count > 1) CmbAcadYear.SelectedIndex = 1;
                _suppress = false;
            }
            catch { }
        }

        private void CmbInstructor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            if (CmbInstructor.Items.Count > 0 && !string.IsNullOrEmpty(CmbInstructor.Text)) CmbInstructor.IsDropDownOpen = true;
        }

        private void ClearBelow(ComboBox start)
        {
            _suppress = true;
            bool clear = false;
            foreach (var cmb in new[] { CmbStream, CmbLevel, CmbModCode, CmbInstructor, CmbAcadYear })
            { if (cmb == start) clear = true; if (clear) cmb.Items.Clear(); }
            _suppress = false;
        }

        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            string deptId = GetTag(CmbDept), streamId = GetTag(CmbStream), mc = GetTag(CmbModCode);
            if (string.IsNullOrEmpty(deptId) || string.IsNullOrEmpty(mc))
            { ModernDialog.Show(Window.GetWindow(this), "Department and Module Code are required!", "Error", ModernDialog.DialogType.Error); return; }
            string lvlNum = GetLevelNum(), insId = GetTag(CmbInstructor);
            string ay = (CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            string at = (CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Regular";
            try
            {
                var dt = new DataTable();
                string sql =
                    "SELECT sa.student_id, CONCAT(TRIM(sp.first_name),' ',TRIM(sp.father_name),' ',TRIM(sp.grand_father_name)) AS full_name, " +
                    "sp.gender, sa.institutional_score, sa.industry_score, sa.total_score, sa.letter_grade, sa.grade_points " +
                    "FROM ecc_dof_wukrostmarycollege.student_assessment sa " +
                    "JOIN ecc_dof_wukrostmarycollege.student_profile sp ON TRIM(sa.student_id)=TRIM(sp.student_id) " +
                    "WHERE sp.dept_id=@d AND sp.stream_id=@s AND sa.module_code=@m AND sp.admission_type=@at";
                if (!string.IsNullOrEmpty(lvlNum)) sql += " AND sa.level=@l";
                if (!string.IsNullOrEmpty(insId))  sql += " AND sa.employee_id=@ins";
                if (!string.IsNullOrEmpty(ay))     sql += " AND sa.academic_year=@y";
                sql += " ORDER BY sp.first_name, sp.father_name";
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@d", deptId); cmd.Parameters.AddWithValue("@s", streamId);
                    cmd.Parameters.AddWithValue("@m", mc); cmd.Parameters.AddWithValue("@at", at);
                    if (!string.IsNullOrEmpty(lvlNum)) cmd.Parameters.AddWithValue("@l", lvlNum);
                    if (!string.IsNullOrEmpty(insId))  cmd.Parameters.AddWithValue("@ins", insId);
                    if (!string.IsNullOrEmpty(ay))     cmd.Parameters.AddWithValue("@y", ay);
                    new MySqlDataAdapter(cmd).Fill(dt);
                });
                Grid1.ItemsSource = dt.DefaultView; PreviewCard.Visibility = Visibility.Visible;
                TxtPreviewInfo.Text = $"Dept: {deptId} | Module: {mc} | Level: {lvlNum} | {dt.Rows.Count} students";
            }
            catch (Exception ex) { ModernDialog.Show(Window.GetWindow(this), "Error: " + ex.Message, "DB Error", ModernDialog.DialogType.Error); }
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not DataView view || view.Count == 0)
            { ModernDialog.Show(Window.GetWindow(this), "Generate first.", "Info", ModernDialog.DialogType.Info); return; }
            var dlg = new Microsoft.Win32.SaveFileDialog { FileName = $"AssessmentMarkList_{DateTime.Now:yyyyMMdd}", DefaultExt = ".pdf", Filter = "PDF|*.pdf" };
            if (dlg.ShowDialog() != true) return;
            string path = dlg.FileName, deptId = GetTag(CmbDept), streamId = GetTag(CmbStream);
            string mc = GetTag(CmbModCode), insId = GetTag(CmbInstructor), lvlNum = GetLevelNum();
            string ay = (CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            string at = (CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string unitTitle = "", nomHours = "0";
            foreach (var (code, title, hours) in _levelModules) if (code == mc) { unitTitle = title; nomHours = hours; break; }
            var rows = new List<(string Name, string Sex, string Id, string Inst, string Ind, string Total, string Grade, string Points)>();
            foreach (DataRowView drv in view)
                rows.Add((TryGet(drv,"full_name"),TryGet(drv,"gender"),TryGet(drv,"student_id"),
                          TryGet(drv,"institutional_score"),TryGet(drv,"industry_score"),
                          TryGet(drv,"total_score"),TryGet(drv,"letter_grade"),TryGet(drv,"grade_points")));
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    string deptName="",streamName="",insName=insId;
                    using(var cmd=new MySqlCommand("SELECT IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments WHERE dept_id=@d LIMIT 1",conn)){cmd.Parameters.AddWithValue("@d",deptId);deptName=cmd.ExecuteScalar()?.ToString()??deptId;}
                    using(var cmd=new MySqlCommand("SELECT IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE stream_id=@s LIMIT 1",conn)){cmd.Parameters.AddWithValue("@s",streamId);streamName=cmd.ExecuteScalar()?.ToString()??streamId;}
                    if(!string.IsNullOrEmpty(insId)){using var cmd=new MySqlCommand("SELECT CONCAT(TRIM(first_name),' ',TRIM(middle_name),' ',TRIM(last_name)) FROM ecc_dof_wukrostmarycollege.employee_profile WHERE employee_id=@i LIMIT 1",conn);cmd.Parameters.AddWithValue("@i",insId);insName=cmd.ExecuteScalar()?.ToString()?.Trim()??insId;}
                    conn.Close();
                    string lvlR = lvlNum switch {"1"=>"I","2"=>"II","3"=>"III","4"=>"IV",_=>lvlNum};
                    var doc = new MigraDoc.DocumentObjectModel.Document();
                    if(doc.Styles["Normal"] is {} ns){ns.Font.Name="Times New Roman";ns.Font.Size=10;}
                    var sec=doc.AddSection();
                    sec.PageSetup.PageFormat=MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.Orientation=MigraDoc.DocumentObjectModel.Orientation.Portrait;
                    sec.PageSetup.TopMargin="1.5cm";sec.PageSetup.BottomMargin="1.8cm";
                    sec.PageSetup.LeftMargin="1.5cm";sec.PageSetup.RightMargin="1.5cm";
                    void CB(string text,double size,bool ul=false){var p=sec.AddParagraph(text);p.Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;p.Format.Font.Bold=true;p.Format.Font.Size=size;p.Format.Font.Name="Times New Roman";if(ul)p.Format.Font.Underline=MigraDoc.DocumentObjectModel.Underline.Single;p.Format.SpaceBefore="0mm";p.Format.SpaceAfter="1mm";}
                    CB("ECC-DoA  WUKRO ST.MARY COLLEGE",13);CB("REGISTRAR'S OFFICE",12);CB("Assessment Results Summary Report to the Registrar",11,true);
                    sec.AddParagraph().Format.SpaceAfter="2mm";
                    var info=sec.AddTable();info.Borders.Width=0.4;info.Borders.Color=MigraDoc.DocumentObjectModel.Colors.Black;
                    info.AddColumn("4.5cm");info.AddColumn("4.5cm");info.AddColumn("4.5cm");info.AddColumn("4.5cm");
                    info.TopPadding="0.8mm";info.BottomPadding="0.8mm";info.Format.Font.Size=9.5;info.Format.Font.Name="Times New Roman";
                    void IR(string l1,string v1,string l2,string v2){var r=info.AddRow();var p1=r.Cells[0].AddParagraph();p1.AddFormattedText(l1,MigraDoc.DocumentObjectModel.TextFormat.NotBold);p1.AddFormattedText(v1,MigraDoc.DocumentObjectModel.TextFormat.Bold);r.Cells[0].MergeRight=1;var p2=r.Cells[2].AddParagraph();p2.AddFormattedText(l2,MigraDoc.DocumentObjectModel.TextFormat.NotBold);p2.AddFormattedText(v2,MigraDoc.DocumentObjectModel.TextFormat.Bold);r.Cells[2].MergeRight=1;}
                    IR("Department: ",deptName,"Instructor ID:  ",insId);
                    IR("Entry Year :  ",ay,"Training Year:  ",ay);
                    {var br=info.AddRow();br.Cells[0].MergeRight=3;}
                    IR("Sector : ",deptName,"Occupation :   ",streamName);
                    {var br=info.AddRow();br.Cells[0].MergeRight=3;}
                    {var r=info.AddRow();var p1=r.Cells[0].AddParagraph();p1.AddFormattedText("Unit of Competence:- ",MigraDoc.DocumentObjectModel.TextFormat.NotBold);var v1=p1.AddFormattedText(unitTitle,MigraDoc.DocumentObjectModel.TextFormat.Bold);v1.Underline=MigraDoc.DocumentObjectModel.Underline.Single;r.Cells[0].MergeRight=1;var p2=r.Cells[2].AddParagraph();p2.AddFormattedText("Module Code:  ",MigraDoc.DocumentObjectModel.TextFormat.NotBold);var v2=p2.AddFormattedText(mc,MigraDoc.DocumentObjectModel.TextFormat.Bold);v2.Underline=MigraDoc.DocumentObjectModel.Underline.Single;r.Cells[2].MergeRight=1;}
                    {var br=info.AddRow();br.Cells[0].MergeRight=3;}
                    {var r=info.AddRow();var p1=r.Cells[0].AddParagraph();p1.AddFormattedText("Nominal Duration :  ",MigraDoc.DocumentObjectModel.TextFormat.NotBold);p1.AddFormattedText(nomHours,MigraDoc.DocumentObjectModel.TextFormat.Bold);r.Cells[0].MergeRight=1;var p2=r.Cells[2].AddParagraph();p2.AddFormattedText("Program:  ",MigraDoc.DocumentObjectModel.TextFormat.NotBold);p2.AddFormattedText(at+"   Level   "+lvlR,MigraDoc.DocumentObjectModel.TextFormat.Bold);r.Cells[2].MergeRight=1;}
                    sec.AddParagraph().Format.SpaceAfter="3mm";
                    var tbl=sec.AddTable();tbl.Borders.Width=0.5;tbl.Borders.Color=MigraDoc.DocumentObjectModel.Colors.Black;
                    tbl.Format.Font.Size=9;tbl.Format.Font.Name="Times New Roman";tbl.TopPadding="1mm";tbl.BottomPadding="1mm";
                    tbl.AddColumn("0.8cm");tbl.AddColumn("4.5cm");tbl.AddColumn("0.8cm");tbl.AddColumn("3.0cm");
                    tbl.AddColumn("2.0cm");tbl.AddColumn("2.0cm");tbl.AddColumn("1.8cm");tbl.AddColumn("1.5cm");tbl.AddColumn("1.8cm");
                    var hdr=tbl.AddRow();hdr.HeadingFormat=true;hdr.Format.Font.Bold=true;hdr.VerticalAlignment=MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    hdr.Shading.Color=new MigraDoc.DocumentObjectModel.Color(220,220,220);
                    string[] hdrs={"No.","Name of the trainees","sex","ID.NO.","Institutional\nassessment\n(70%)","Industry\nAssessment\n(30%)","Total\nResult\n(100%)","Grade\nin\nLetter","Grade\nin\npoint"};
                    for(int ci=0;ci<hdrs.Length;ci++){hdr.Cells[ci].AddParagraph(hdrs[ci]);hdr.Cells[ci].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;}
                    for(int i=0;i<rows.Count;i++){
                        var(name,sex,id,inst,ind,total,grade,pts)=rows[i];
                        double.TryParse(pts,out double p);double.TryParse(total,out double tot);double gip=tot>0&&p>0?Math.Round(tot*p,0):0;
                        var row=tbl.AddRow();row.VerticalAlignment=MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row.Cells[0].AddParagraph((i+1).ToString());row.Cells[0].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[1].AddParagraph(name);row.Cells[1].Format.Font.Bold=true;
                        row.Cells[2].AddParagraph(sex.Length>0?sex[0].ToString().ToUpper():"");row.Cells[2].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[3].AddParagraph(id);
                        row.Cells[4].AddParagraph(inst);row.Cells[4].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                        row.Cells[5].AddParagraph(ind);row.Cells[5].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                        row.Cells[6].AddParagraph(total);row.Cells[6].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                        row.Cells[7].AddParagraph(grade);row.Cells[7].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[8].AddParagraph(gip>0?gip.ToString("0"):pts);row.Cells[8].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                    }
                    sec.AddParagraph().Format.SpaceAfter="8mm";
                    var st=sec.AddTable();st.Borders.Width=0;st.AddColumn("9.2cm");st.AddColumn("9.2cm");
                    void SR(string l,string r2){var row=st.AddRow();row.Format.Font.Size=9.5;row.Format.Font.Name="Times New Roman";row.Format.SpaceBefore="1mm";row.Cells[0].AddParagraph(l);row.Cells[1].AddParagraph(r2);}
                    SR($"Trainer Name :- {insName}","Department Head");SR("Signature","Signature");SR("Date","Date");
                    var ren=new MigraDoc.Rendering.PdfDocumentRenderer{Document=doc};ren.RenderDocument();ren.PdfDocument.Save(path);
                });
                ModernDialog.Show(Window.GetWindow(this),"PDF saved!","Success",ModernDialog.DialogType.Success);
            }
            catch(Exception ex){ModernDialog.Show(Window.GetWindow(this),"PDF failed: "+ex.Message,"Error",ModernDialog.DialogType.Error);}
        }

        private async void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not DataView view || view.Count == 0)
            { ModernDialog.Show(Window.GetWindow(this), "Generate first.", "Info", ModernDialog.DialogType.Info); return; }
            var dlg = new Microsoft.Win32.SaveFileDialog { FileName = $"AssessmentMarkList_{DateTime.Now:yyyyMMdd}", DefaultExt = ".xlsx", Filter = "Excel Workbook|*.xlsx" };
            if (dlg.ShowDialog() != true) return;
            string path=dlg.FileName,deptId=GetTag(CmbDept),streamId=GetTag(CmbStream),mc=GetTag(CmbModCode),insId=GetTag(CmbInstructor),lvlNum=GetLevelNum();
            string ay=(CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim()??"";
            string at=(CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString()??"";
            string unitTitle="",nomHours="0";
            foreach(var(code,title,hours) in _levelModules) if(code==mc){unitTitle=title;nomHours=hours;break;}
            var rows=new List<(string Name,string Sex,string Id,string Inst,string Ind,string Total,string Grade,string Points)>();
            foreach(DataRowView drv in view)
                rows.Add((TryGet(drv,"full_name"),TryGet(drv,"gender"),TryGet(drv,"student_id"),TryGet(drv,"institutional_score"),TryGet(drv,"industry_score"),TryGet(drv,"total_score"),TryGet(drv,"letter_grade"),TryGet(drv,"grade_points")));
            try
            {
                await Task.Run(()=>{
                    var conn=_db.GetConnection();conn.Open();
                    string deptName="",streamName="",insName=insId;
                    using(var cmd=new MySqlCommand("SELECT IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments WHERE dept_id=@d LIMIT 1",conn)){cmd.Parameters.AddWithValue("@d",deptId);deptName=cmd.ExecuteScalar()?.ToString()??deptId;}
                    using(var cmd=new MySqlCommand("SELECT IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE stream_id=@s LIMIT 1",conn)){cmd.Parameters.AddWithValue("@s",streamId);streamName=cmd.ExecuteScalar()?.ToString()??streamId;}
                    if(!string.IsNullOrEmpty(insId)){using var cmd=new MySqlCommand("SELECT CONCAT(TRIM(first_name),' ',TRIM(middle_name),' ',TRIM(last_name)) FROM ecc_dof_wukrostmarycollege.employee_profile WHERE employee_id=@i LIMIT 1",conn);cmd.Parameters.AddWithValue("@i",insId);insName=cmd.ExecuteScalar()?.ToString()?.Trim()??insId;}
                    conn.Close();
                    string lvlR=lvlNum switch{"1"=>"I","2"=>"II","3"=>"III","4"=>"IV",_=>lvlNum};
                    using var wb=new ClosedXML.Excel.XLWorkbook();var ws=wb.Worksheets.Add("MarkList");int cols=9;
                    void Mg(int row,string val,bool bold=false,int fs=10){ws.Range(row,1,row,cols).Merge();ws.Cell(row,1).Value=val;ws.Cell(row,1).Style.Font.Bold=bold;ws.Cell(row,1).Style.Font.FontSize=fs;ws.Cell(row,1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;}
                    int r2=1;
                    Mg(r2,"ECC-DoA  WUKRO ST.MARY COLLEGE",true,13);r2++;Mg(r2,"REGISTRAR'S OFFICE",true,12);r2++;
                    Mg(r2,"Assessment Results Summary Report to the Registrar",true,11);ws.Cell(r2,1).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;r2++;r2++;
                    ws.Cell(r2,1).Value=$"Department: {deptName}";ws.Cell(r2,1).Style.Font.Bold=true;ws.Range(r2,1,r2,4).Merge();
                    ws.Cell(r2,5).Value=$"Instructor ID:  {insId}";ws.Cell(r2,5).Style.Font.Bold=true;ws.Range(r2,5,r2,cols).Merge();r2++;
                    ws.Cell(r2,1).Value=$"Entry Year :  {ay}";ws.Range(r2,1,r2,4).Merge();ws.Cell(r2,5).Value=$"Training Year:  {ay}";ws.Range(r2,5,r2,cols).Merge();r2++;r2++;
                    ws.Cell(r2,1).Value=$"Sector : {deptName}";ws.Range(r2,1,r2,4).Merge();ws.Cell(r2,5).Value=$"Occupation :   {streamName}";ws.Cell(r2,5).Style.Font.Bold=true;ws.Cell(r2,5).Style.Font.FontSize=12;ws.Range(r2,5,r2,cols).Merge();r2++;r2++;
                    ws.Cell(r2,1).Value=$"Unit of Competence:- {unitTitle}";ws.Cell(r2,1).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;ws.Range(r2,1,r2,4).Merge();
                    ws.Cell(r2,5).Value=$"Module Code:  {mc}";ws.Cell(r2,5).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;ws.Range(r2,5,r2,cols).Merge();r2++;r2++;
                    ws.Cell(r2,1).Value=$"Nominal Duration :  {nomHours}";ws.Cell(r2,1).Style.Fill.BackgroundColor=ClosedXML.Excel.XLColor.Yellow;ws.Range(r2,1,r2,4).Merge();
                    ws.Cell(r2,5).Value=$"Program:  {at}   Level   {lvlR}";ws.Cell(r2,5).Style.Font.Bold=true;ws.Range(r2,5,r2,cols).Merge();r2++;r2++;
                    string[] hdrs={"No.","Name of the trainees","sex","ID.NO.","Institutional\nassessment(70%)","Industry\nAssessment(30%)","Total Result\n(100%)","Grade in\nLetter","Grade in\npoint"};
                    for(int c=0;c<hdrs.Length;c++){ws.Cell(r2,c+1).Value=hdrs[c];ws.Cell(r2,c+1).Style.Font.Bold=true;ws.Cell(r2,c+1).Style.Fill.BackgroundColor=ClosedXML.Excel.XLColor.FromHtml("#D9E1F2");ws.Cell(r2,c+1).Style.Border.OutsideBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;ws.Cell(r2,c+1).Style.Alignment.WrapText=true;ws.Cell(r2,c+1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;}
                    ws.Row(r2).Height=40;r2++;
                    for(int i=0;i<rows.Count;i++){
                        var(name,sex,id,inst,ind,total,grade,pts)=rows[i];
                        double.TryParse(pts,out double p);double.TryParse(total,out double tot);double gip=tot>0&&p>0?Math.Round(tot*p,0):0;
                        ws.Cell(r2,1).Value=i+1;ws.Cell(r2,2).Value=name;ws.Cell(r2,2).Style.Font.Bold=true;
                        ws.Cell(r2,3).Value=sex.Length>0?sex[0].ToString().ToUpper():"";ws.Cell(r2,4).Value=id;
                        ws.Cell(r2,5).Value=inst;ws.Cell(r2,6).Value=ind;ws.Cell(r2,7).Value=total;ws.Cell(r2,8).Value=grade;ws.Cell(r2,9).Value=gip>0?gip:0;
                        for(int c=1;c<=9;c++)ws.Cell(r2,c).Style.Border.OutsideBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;r2++;
                    }
                    r2++;ws.Cell(r2,1).Value=$"Trainer Name :- {insName}";ws.Range(r2,1,r2,4).Merge();r2++;
                    ws.Cell(r2,1).Value="Signature";ws.Range(r2,1,r2,4).Merge();ws.Cell(r2,5).Value="Department Head";ws.Range(r2,5,r2,cols).Merge();r2++;
                    ws.Cell(r2,1).Value="Signature";ws.Range(r2,1,r2,4).Merge();ws.Cell(r2,5).Value="Signature";ws.Range(r2,5,r2,cols).Merge();r2++;
                    ws.Cell(r2,1).Value="Date";ws.Range(r2,1,r2,4).Merge();ws.Cell(r2,5).Value="Date";ws.Range(r2,5,r2,cols).Merge();
                    ws.Column(1).Width=5;ws.Column(2).Width=28;ws.Column(3).Width=5;ws.Column(4).Width=15;
                    ws.Column(5).Width=12;ws.Column(6).Width=12;ws.Column(7).Width=10;ws.Column(8).Width=8;ws.Column(9).Width=10;
                    wb.SaveAs(path);
                });
                ModernDialog.Show(Window.GetWindow(this),"Excel saved!","Success",ModernDialog.DialogType.Success);
            }
            catch(Exception ex){ModernDialog.Show(Window.GetWindow(this),"Export failed: "+ex.Message,"Error",ModernDialog.DialogType.Error);}
        }

        private static string TryGet(DataRowView drv, string col)
        { try { return drv[col]?.ToString()?.Trim() ?? ""; } catch { return ""; } }
    }
}
