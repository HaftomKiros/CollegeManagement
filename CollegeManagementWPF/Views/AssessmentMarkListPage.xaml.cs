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
        private List<(string Id, string Name)> _allDepts = new();
        private List<(string Id, string Name)> _allStreams = new();
        private List<(string LevelId, string Num)> _streamLevels = new();
        private List<(string Code, string Title, string Hours)> _levelModules = new();
        private List<(string Id, string Name)> _levelInstructors = new();

        public AssessmentMarkListPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            ApplyPermissions();
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

        private void ApplyPermissions()
        {
            if (SessionUser.IsSuperAdmin) return;
            bool can = SessionUser.Has("report_assessment_ml");
            BtnGenerate.Visibility    = can ? Visibility.Visible : Visibility.Collapsed;
            BtnPrint.Visibility       = can ? Visibility.Visible : Visibility.Collapsed;
            BtnExportExcel.Visibility = can ? Visibility.Visible : Visibility.Collapsed;
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
                RefreshDeptDrop("");
            }
            catch { }
        }

        private void RefreshDeptDrop(string f)
        {
            _suppress = true; CmbDept.Items.Clear();
            foreach (var (id, name) in _allDepts)
                if (string.IsNullOrEmpty(f) || id.Contains(f, StringComparison.OrdinalIgnoreCase) || name.Contains(f, StringComparison.OrdinalIgnoreCase))
                    CmbDept.Items.Add(new ComboBoxItem { Content = string.IsNullOrEmpty(name) ? id : $"{id} - {name}", Tag = id });
            _suppress = false;
        }

        private void CmbDept_TextChanged(object s, TextChangedEventArgs e) { if (_suppress) return; RefreshDeptDrop(CmbDept.Text?.Trim() ?? ""); if (CmbDept.Items.Count > 0 && !string.IsNullOrEmpty(CmbDept.Text)) CmbDept.IsDropDownOpen = true; }
        private async void CmbDept_SelectionChanged(object s, SelectionChangedEventArgs e) { if (_suppress) return; string did = GT(CmbDept); if (string.IsNullOrEmpty(did)) return; CB(CmbStream); await LoadStreamsAsync(did); }

        private string GT(ComboBox c) { if (c.SelectedItem is ComboBoxItem s && s.Tag != null) return s.Tag.ToString()!; string t = c.Text?.Trim() ?? ""; int d = t.IndexOf(" - "); return d >= 0 ? t[..d].Trim() : t; }

        private async Task LoadStreamsAsync(string did)
        {
            try
            {
                _allStreams = await Task.Run(() => { var list = new List<(string, string)>(); var conn = _db.GetConnection(); conn.Open(); using var cmd = new MySqlCommand("SELECT stream_id,IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE dept_id=@d ORDER BY stream_id", conn); cmd.Parameters.AddWithValue("@d", did); using var r = cmd.ExecuteReader(); while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? "")); conn.Close(); return list; });
                _suppress = true; CmbStream.Items.Clear();
                foreach (var (id, name) in _allStreams) CmbStream.Items.Add(new ComboBoxItem { Content = string.IsNullOrEmpty(name) ? id : $"{id} - {name}", Tag = id });
                if (CmbStream.Items.Count > 0) CmbStream.SelectedIndex = 0; _suppress = false;
            }
            catch { }
        }

        private void CmbStream_TextChanged(object s, TextChangedEventArgs e) { if (_suppress) return; if (CmbStream.Items.Count > 0) CmbStream.IsDropDownOpen = true; }
        private async void CmbStream_SelectionChanged(object s, SelectionChangedEventArgs e) { if (_suppress) return; string sid = GT(CmbStream); if (string.IsNullOrEmpty(sid)) return; CB(CmbLevel); await LoadLevelsAsync(sid); }

        private async Task LoadLevelsAsync(string sid)
        {
            try
            {
                _streamLevels = await Task.Run(() => { var list = new List<(string, string)>(); var conn = _db.GetConnection(); conn.Open(); using var cmd = new MySqlCommand("SELECT level_id,level FROM ecc_dof_wukrostmarycollege.levels WHERE stream_id=@s ORDER BY level", conn); cmd.Parameters.AddWithValue("@s", sid); using var r = cmd.ExecuteReader(); while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? "")); conn.Close(); return list; });
                _suppress = true; CmbLevel.Items.Clear();
                foreach (var (lid, lnum) in _streamLevels) CmbLevel.Items.Add(new ComboBoxItem { Content = $"{lid} - {lnum}", Tag = lid });
                if (CmbLevel.Items.Count > 0) CmbLevel.SelectedIndex = 0; _suppress = false;
                string lvlId = GT(CmbLevel); if (!string.IsNullOrEmpty(lvlId)) await LoadModulesAsync(lvlId);
            }
            catch { }
        }

        private void CmbLevel_TextChanged(object s, TextChangedEventArgs e) { if (_suppress) return; if (CmbLevel.Items.Count > 0) CmbLevel.IsDropDownOpen = true; }
        private async void CmbLevel_SelectionChanged(object s, SelectionChangedEventArgs e) { if (_suppress) return; string lvlId = GT(CmbLevel); if (string.IsNullOrEmpty(lvlId)) return; CB(CmbModCode); await LoadModulesAsync(lvlId); }

        private string GetLevelNum() { string d = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? ""; int i = d.IndexOf(" - "); return i >= 0 ? d[(i + 3)..].Trim() : d; }

        private async Task LoadModulesAsync(string levelId)
        {
            try
            {
                _levelModules = await Task.Run(() => { var list = new List<(string, string, string)>(); var conn = _db.GetConnection(); conn.Open(); using var cmd = new MySqlCommand("SELECT module_code,IFNULL(unit_of_competence_title,''),IFNULL(total_hours,'0') FROM ecc_dof_wukrostmarycollege.courses WHERE level_id=@l ORDER BY module_code", conn); cmd.Parameters.AddWithValue("@l", levelId); using var r = cmd.ExecuteReader(); while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? "", r[2]?.ToString() ?? "0")); conn.Close(); return list; });
                _suppress = true; CmbModCode.Items.Clear();
                foreach (var (code, title, _) in _levelModules) CmbModCode.Items.Add(new ComboBoxItem { Content = string.IsNullOrEmpty(title) ? code : $"{code} - {title}", Tag = code });
                if (CmbModCode.Items.Count > 0) CmbModCode.SelectedIndex = 0; _suppress = false;
                string mc = GT(CmbModCode); if (!string.IsNullOrEmpty(mc)) await LoadInsYearsAsync(mc);
            }
            catch { }
        }

        private void CmbModCode_TextChanged(object s, TextChangedEventArgs e) { if (_suppress) return; if (CmbModCode.Items.Count > 0) CmbModCode.IsDropDownOpen = true; }
        private async void CmbModCode_SelectionChanged(object s, SelectionChangedEventArgs e) { if (_suppress) return; string mc = GT(CmbModCode); if (string.IsNullOrEmpty(mc)) return; CB(CmbInstructor); await LoadInsYearsAsync(mc); }

        private async Task LoadInsYearsAsync(string mc)
        {
            try
            {
                var (il, yl) = await Task.Run(() =>
                {
                    var ilist = new List<(string, string)>(); var ylist = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using (var cmd = new MySqlCommand("SELECT DISTINCT sa.employee_id,IFNULL(CONCAT(TRIM(ep.first_name),' ',TRIM(ep.middle_name),' ',TRIM(ep.last_name)),sa.employee_id) FROM ecc_dof_wukrostmarycollege.student_assessment sa LEFT JOIN ecc_dof_wukrostmarycollege.employee_profile ep ON sa.employee_id=ep.employee_id WHERE sa.module_code=@m AND sa.employee_id IS NOT NULL ORDER BY sa.employee_id", conn)) { cmd.Parameters.AddWithValue("@m", mc); using var r = cmd.ExecuteReader(); while (r.Read()) ilist.Add((r[0]?.ToString() ?? "", r[1]?.ToString()?.Trim() ?? "")); }
                    using (var cmd = new MySqlCommand("SELECT DISTINCT academic_year FROM ecc_dof_wukrostmarycollege.student_assessment WHERE module_code=@m AND academic_year IS NOT NULL ORDER BY academic_year", conn)) { cmd.Parameters.AddWithValue("@m", mc); using var r = cmd.ExecuteReader(); while (r.Read()) ylist.Add(r[0]?.ToString() ?? ""); }
                    conn.Close(); return (ilist, ylist);
                });
                _levelInstructors = il; _suppress = true;
                CmbInstructor.Items.Clear();
                foreach (var (id, name) in il) CmbInstructor.Items.Add(new ComboBoxItem { Content = $"{id} - {name}", Tag = id });
                CmbAcadYear.Items.Clear(); CmbAcadYear.Items.Add(new ComboBoxItem { Content = "" });
                foreach (var y in yl) CmbAcadYear.Items.Add(new ComboBoxItem { Content = y });
                if (CmbAcadYear.Items.Count > 1) CmbAcadYear.SelectedIndex = 1; _suppress = false;
            }
            catch { }
        }

        private void CmbInstructor_TextChanged(object s, TextChangedEventArgs e) { if (_suppress) return; if (CmbInstructor.Items.Count > 0) CmbInstructor.IsDropDownOpen = true; }

        private void CB(ComboBox start)
        {
            _suppress = true; bool clear = false;
            foreach (var c in new[] { CmbStream, CmbLevel, CmbModCode, CmbInstructor, CmbAcadYear }) { if (c == start) clear = true; if (clear) c.Items.Clear(); }
            _suppress = false;
        }

        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            string deptId=GT(CmbDept),streamId=GT(CmbStream),mc=GT(CmbModCode);
            if (string.IsNullOrEmpty(deptId)||string.IsNullOrEmpty(mc)) { ModernDialog.Show(Window.GetWindow(this),"Department and Module Code required!","Error",ModernDialog.DialogType.Error); return; }
            string lvlNum=GetLevelNum(),insId=GT(CmbInstructor);
            string ay=(CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim()??"";
            string at=(CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString()??"Regular";
            try
            {
                var dt=new DataTable();
                string sql="SELECT DISTINCT sa.student_id,CONCAT(TRIM(sp.first_name),' ',TRIM(sp.father_name),' ',TRIM(sp.grand_father_name)) AS full_name,sp.gender,sa.institutional_score,sa.industry_score,sa.total_score,sa.letter_grade,sa.grade_points FROM ecc_dof_wukrostmarycollege.student_assessment sa JOIN (SELECT TRIM(student_id) AS student_id,dept_id,stream_id,admission_type,first_name,father_name,grand_father_name,gender FROM ecc_dof_wukrostmarycollege.student_profile GROUP BY TRIM(student_id)) sp ON TRIM(sa.student_id)=sp.student_id WHERE sp.dept_id=@d AND sp.stream_id=@s AND sa.module_code=@m AND sp.admission_type=@at";
                if (!string.IsNullOrEmpty(lvlNum)) sql+=" AND sa.level=@l";
                if (!string.IsNullOrEmpty(insId))  sql+=" AND sa.employee_id=@ins";
                if (!string.IsNullOrEmpty(ay))     sql+=" AND sa.academic_year=@y";
                sql+=" ORDER BY sp.first_name,sp.father_name";
                await Task.Run(()=>{ var conn=_db.GetConnection(); var cmd=new MySqlCommand(sql,conn); cmd.Parameters.AddWithValue("@d",deptId); cmd.Parameters.AddWithValue("@s",streamId); cmd.Parameters.AddWithValue("@m",mc); cmd.Parameters.AddWithValue("@at",at); if(!string.IsNullOrEmpty(lvlNum))cmd.Parameters.AddWithValue("@l",lvlNum); if(!string.IsNullOrEmpty(insId))cmd.Parameters.AddWithValue("@ins",insId); if(!string.IsNullOrEmpty(ay))cmd.Parameters.AddWithValue("@y",ay); new MySqlDataAdapter(cmd).Fill(dt); });
                Grid1.ItemsSource=dt.DefaultView; PreviewCard.Visibility=Visibility.Visible;
                TxtPreviewInfo.Text=$"Dept: {deptId} | Module: {mc} | Level: {lvlNum} | {dt.Rows.Count} students";
            }
            catch(Exception ex){ModernDialog.Show(Window.GetWindow(this),"Error: "+ex.Message,"DB Error",ModernDialog.DialogType.Error);}
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not DataView view||view.Count==0) { ModernDialog.Show(Window.GetWindow(this),"Generate first.","Info",ModernDialog.DialogType.Info); return; }
            var dlg=new Microsoft.Win32.SaveFileDialog{FileName=$"AssessmentMarkList_{DateTime.Now:yyyyMMdd}",DefaultExt=".pdf",Filter="PDF|*.pdf"};
            if (dlg.ShowDialog()!=true) return;
            string path=dlg.FileName,deptId=GT(CmbDept),streamId=GT(CmbStream),mc=GT(CmbModCode),insId=GT(CmbInstructor),lvlNum=GetLevelNum();
            string ay=(CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim()??"";
            string at=(CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString()??"";
            string unitTitle="",nomHours="0";
            foreach(var(code,title,hours) in _levelModules) if(code==mc){unitTitle=title;nomHours=hours;break;}
            var rows=new List<(string Name,string Sex,string Id,string Inst,string Ind,string Total,string Grade,string Pts)>();
            foreach(DataRowView drv in view) rows.Add((TG(drv,"full_name"),TG(drv,"gender"),TG(drv,"student_id"),TG(drv,"institutional_score"),TG(drv,"industry_score"),TG(drv,"total_score"),TG(drv,"letter_grade"),TG(drv,"grade_points")));
            try
            {
                await Task.Run(()=>{
                    var conn=_db.GetConnection(); conn.Open();
                    string dn="",sectorName="",occupationName="",iname=insId;
                    string entryYear=ay, trainingYear=ay;
                    using(var cmd=new MySqlCommand("SELECT IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments WHERE dept_id=@d LIMIT 1",conn)){cmd.Parameters.AddWithValue("@d",deptId);dn=cmd.ExecuteScalar()?.ToString()??deptId;}
                    // sector = dept_name, occupation = stream_name (streams table has no sector column)
                    sectorName=dn;
                    try{using var cmd=new MySqlCommand("SELECT IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE stream_id=@s LIMIT 1",conn);cmd.Parameters.AddWithValue("@s",streamId);occupationName=cmd.ExecuteScalar()?.ToString()??streamId;}catch{occupationName=streamId;}
                    if(!string.IsNullOrEmpty(insId)){using var cmd=new MySqlCommand("SELECT TRIM(CONCAT_WS(' ', NULLIF(TRIM(first_name),''), NULLIF(TRIM(middle_name),''), NULLIF(TRIM(last_name),''))) FROM ecc_dof_wukrostmarycollege.employee_profile WHERE employee_id=@i LIMIT 1",conn);cmd.Parameters.AddWithValue("@i",insId);iname=cmd.ExecuteScalar()?.ToString()?.Trim()??insId;}
                    try{using var cmd=new MySqlCommand("SELECT IFNULL(MIN(entry_year),@ay) FROM ecc_dof_wukrostmarycollege.student_profile sp JOIN ecc_dof_wukrostmarycollege.student_assessment sa ON TRIM(sp.student_id)=TRIM(sa.student_id) WHERE sa.module_code=@m AND sp.dept_id=@d AND sp.stream_id=@s LIMIT 1",conn);cmd.Parameters.AddWithValue("@ay",ay);cmd.Parameters.AddWithValue("@m",mc);cmd.Parameters.AddWithValue("@d",deptId);cmd.Parameters.AddWithValue("@s",streamId);var ev=cmd.ExecuteScalar()?.ToString();if(!string.IsNullOrEmpty(ev))entryYear=ev;}catch{}
                    conn.Close();
                    string lvlR=lvlNum switch{"1"=>"I","2"=>"II","3"=>"III","4"=>"IV",_=>lvlNum};
                    var doc=new MigraDoc.DocumentObjectModel.Document();
                    if(doc.Styles["Normal"] is {} ns){ns.Font.Name="Times New Roman";ns.Font.Size=10;}
                    var sec=doc.AddSection();
                    sec.PageSetup.PageFormat=MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.Orientation=MigraDoc.DocumentObjectModel.Orientation.Portrait;
                    sec.PageSetup.TopMargin="1.5cm";sec.PageSetup.BottomMargin="1.8cm";
                    sec.PageSetup.LeftMargin="1.5cm";sec.PageSetup.RightMargin="1.5cm";
                    void AddCentered(string t,double sz,bool ul=false,bool bold=true){var p=sec.AddParagraph(t);p.Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;p.Format.Font.Bold=bold;p.Format.Font.Size=sz;p.Format.Font.Name="Times New Roman";if(ul)p.Format.Font.Underline=MigraDoc.DocumentObjectModel.Underline.Single;p.Format.SpaceBefore="0mm";p.Format.SpaceAfter="1mm";}
                    AddCentered("ECC-DoA  WUKRO ST.MARY COLLEGE",13);
                    AddCentered("REGISTRAR'S OFFICE",12);
                    AddCentered("Assessment Results Summary Report to the Registrar",11,ul:true);
                    sec.AddParagraph().Format.SpaceAfter="2mm";

                    // ── INFO TABLE: NO BORDERS, full page width, grey/yellow fill only ──
                    // 6 columns totaling 18.4cm (A4 minus margins)
                    var info=sec.AddTable();
                    info.Borders.Visible=false;
                    info.AddColumn("3.0cm"); // 0: left label
                    info.AddColumn("5.8cm"); // 1: left value
                    info.AddColumn("0.6cm"); // 2: gap (white)
                    info.AddColumn("3.5cm"); // 3: right label
                    info.AddColumn("1.5cm"); // 4: right mid (Level label / Program value)
                    info.AddColumn("4.0cm"); // 5: right value (underlined)
                    info.TopPadding="1.2mm";info.BottomPadding="1.2mm";
                    info.Format.Font.Size=9.5;info.Format.Font.Name="Times New Roman";
                    var grey=new MigraDoc.DocumentObjectModel.Color(230,230,230);
                    var yellow=new MigraDoc.DocumentObjectModel.Color(255,255,0);

                    MigraDoc.DocumentObjectModel.Tables.Row NR(){var r=info.AddRow();r.Borders.Visible=false;return r;}
                    void Spacer(){var r=NR();r.Height="2.5mm";r.Cells[0].MergeRight=5;}

                    // ROW 1: Department: [bold val] (grey) | gap | Instructor ID: (grey) | [bold underlined val] (grey)
                    {var r=NR();
                     var p=r.Cells[0].AddParagraph();p.AddFormattedText("Department:  ",MigraDoc.DocumentObjectModel.TextFormat.NotBold);p.AddFormattedText(dn,MigraDoc.DocumentObjectModel.TextFormat.Bold);
                     r.Cells[0].MergeRight=1;r.Cells[0].Shading.Color=grey;
                     r.Cells[3].AddParagraph("Instructor ID:  ");r.Cells[3].Shading.Color=grey;r.Cells[3].MergeRight=1;
                     var pv=r.Cells[5].AddParagraph();var fv=pv.AddFormattedText(insId,MigraDoc.DocumentObjectModel.TextFormat.Bold);fv.Underline=MigraDoc.DocumentObjectModel.Underline.Single;r.Cells[5].Shading.Color=grey;}

                    // ROW 2: [right] Entry Year: (grey) | [bold ul val] (grey) | gap | Training Year: (grey) | [bold ul val] (grey)
                    {var r=NR();
                     r.Cells[0].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                     r.Cells[0].AddParagraph("Entry Year :  ");r.Cells[0].Shading.Color=grey;
                     var p1=r.Cells[1].AddParagraph();var f1=p1.AddFormattedText(entryYear,MigraDoc.DocumentObjectModel.TextFormat.Bold);f1.Underline=MigraDoc.DocumentObjectModel.Underline.Single;r.Cells[1].Shading.Color=grey;
                     r.Cells[3].AddParagraph("Training Year:  ");r.Cells[3].Shading.Color=grey;r.Cells[3].MergeRight=1;
                     var p2=r.Cells[5].AddParagraph();var f2=p2.AddFormattedText(trainingYear,MigraDoc.DocumentObjectModel.TextFormat.Bold);f2.Underline=MigraDoc.DocumentObjectModel.Underline.Single;r.Cells[5].Shading.Color=grey;}

                    Spacer();

                    // ROW 3: Sector: [bold] (grey) | gap | Occupation: (grey) | [large bold] (grey)
                    {var r=NR();
                     var p=r.Cells[0].AddParagraph();p.AddFormattedText("Sector :  ",MigraDoc.DocumentObjectModel.TextFormat.NotBold);p.AddFormattedText(sectorName,MigraDoc.DocumentObjectModel.TextFormat.Bold);
                     r.Cells[0].MergeRight=1;r.Cells[0].Shading.Color=grey;
                     r.Cells[3].AddParagraph("Occupation :  ");r.Cells[3].Shading.Color=grey;r.Cells[3].MergeRight=1;
                     var pv=r.Cells[5].AddParagraph();var ft=pv.AddFormattedText(occupationName,MigraDoc.DocumentObjectModel.TextFormat.Bold);ft.Size=13;r.Cells[5].Shading.Color=grey;}

                    Spacer();

                    // ROW 4: Unit of Competence:- [bold ul title] (grey) | gap | Module Code: (grey) | [bold ul mc] (grey)
                    {var r=NR();
                     var p1=r.Cells[0].AddParagraph();p1.AddFormattedText("Unit of Competence:- ",MigraDoc.DocumentObjectModel.TextFormat.NotBold);
                     var v1=p1.AddFormattedText(unitTitle,MigraDoc.DocumentObjectModel.TextFormat.Bold);v1.Underline=MigraDoc.DocumentObjectModel.Underline.Single;
                     r.Cells[0].MergeRight=1;r.Cells[0].Shading.Color=grey;
                     r.Cells[3].AddParagraph("Module Code:  ");r.Cells[3].Shading.Color=grey;r.Cells[3].MergeRight=1;
                     var p2=r.Cells[5].AddParagraph();var v2=p2.AddFormattedText(mc,MigraDoc.DocumentObjectModel.TextFormat.Bold);v2.Underline=MigraDoc.DocumentObjectModel.Underline.Single;r.Cells[5].Shading.Color=grey;}

                    Spacer();

                    // ROW 5: [right] Nominal Duration: (grey) | [YELLOW: hours] | gap | Program: [bold at] (grey) | Level (grey) | [bold ul lvlR] (grey)
                    {var r=NR();
                     r.Cells[0].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                     r.Cells[0].AddParagraph("Nominal Duration :  ");r.Cells[0].Shading.Color=grey;
                     r.Cells[1].AddParagraph(nomHours);r.Cells[1].Format.Font.Bold=true;r.Cells[1].Shading.Color=yellow;
                     // gap col 2 — white, no fill
                     var p3=r.Cells[3].AddParagraph();p3.AddFormattedText("Program:  ",MigraDoc.DocumentObjectModel.TextFormat.NotBold);p3.AddFormattedText(at,MigraDoc.DocumentObjectModel.TextFormat.Bold);r.Cells[3].Shading.Color=grey;
                     r.Cells[4].AddParagraph("Level  ");r.Cells[4].Shading.Color=grey;
                     var p5=r.Cells[5].AddParagraph();var fL=p5.AddFormattedText(lvlR,MigraDoc.DocumentObjectModel.TextFormat.Bold);fL.Underline=MigraDoc.DocumentObjectModel.Underline.Single;r.Cells[5].Shading.Color=grey;}

                    Spacer();

                    sec.AddParagraph().Format.SpaceAfter="2mm";                    var tbl=sec.AddTable();tbl.Borders.Width=0.5;tbl.Borders.Color=MigraDoc.DocumentObjectModel.Colors.Black;tbl.Format.Font.Size=9;tbl.Format.Font.Name="Times New Roman";tbl.TopPadding="1mm";tbl.BottomPadding="1mm";
                    tbl.AddColumn("0.8cm");tbl.AddColumn("4.5cm");tbl.AddColumn("0.8cm");tbl.AddColumn("2.8cm");tbl.AddColumn("2.0cm");tbl.AddColumn("2.0cm");tbl.AddColumn("1.8cm");tbl.AddColumn("1.4cm");tbl.AddColumn("1.8cm");
                    var hdr=tbl.AddRow();hdr.HeadingFormat=true;hdr.Format.Font.Bold=true;hdr.VerticalAlignment=MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;hdr.Shading.Color=new MigraDoc.DocumentObjectModel.Color(220,220,220);
                    string[] hdrs={"No.","Name of the trainees","sex","ID.NO.","Institutional\nassessment\n(70%)","Industry\nAssessment\n(30%)","Total\nResult\n(100%)","Grade\nin\nLetter","Grade\nin\npoint"};
                    for(int ci=0;ci<hdrs.Length;ci++){hdr.Cells[ci].AddParagraph(hdrs[ci]);hdr.Cells[ci].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;}
                    for(int i=0;i<rows.Count;i++){
                        var(nm,sx,id,inst,ind,tot,grd,pts)=rows[i];
                        // Grade in point = nominal_hours × grade_points (GPA scale)
                        double.TryParse(pts,out double gpVal);double.TryParse(nomHours,out double nh2);
                        string gipStr=gpVal>0&&nh2>0?Math.Round(gpVal*nh2,0).ToString("0"):pts;
                        var row=tbl.AddRow();row.VerticalAlignment=MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row.Cells[0].AddParagraph((i+1).ToString());row.Cells[0].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[1].AddParagraph(nm);row.Cells[1].Format.Font.Bold=true;
                        row.Cells[2].AddParagraph(sx.Length>0?sx[0].ToString().ToUpper():"");row.Cells[2].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[3].AddParagraph(id);
                        row.Cells[4].AddParagraph(inst);row.Cells[4].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                        row.Cells[5].AddParagraph(ind);row.Cells[5].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                        row.Cells[6].AddParagraph(tot);row.Cells[6].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                        row.Cells[7].AddParagraph(grd);row.Cells[7].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[8].AddParagraph(gipStr);row.Cells[8].Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                    }
                    sec.AddParagraph().Format.SpaceAfter="6mm";

                    // Footer: 2 rows × 3 columns each
                    // Row 1: Trainer Name _____ | Signature _____ | Date _____
                    // Row 2: Department Head ___ | Signature _____ | Date _____
                    var st=sec.AddTable();st.Borders.Visible=false;
                    st.AddColumn("5.5cm");st.AddColumn("0.5cm");st.AddColumn("5.5cm");st.AddColumn("0.5cm");st.AddColumn("5.4cm");
                    st.TopPadding="2mm";st.BottomPadding="2mm";
                    st.Format.Font.Size=9.5;st.Format.Font.Name="Times New Roman";

                    void FRow(string c1,string c3,string c5){
                        var row=st.AddRow();
                        void FC(MigraDoc.DocumentObjectModel.Tables.Cell cell,string txt){
                            var p=cell.AddParagraph(txt);
                            cell.Borders.Bottom.Visible=true;cell.Borders.Bottom.Width=0.5;cell.Borders.Bottom.Color=MigraDoc.DocumentObjectModel.Colors.Black;
                        }
                        FC(row.Cells[0],c1);row.Cells[1].Borders.Visible=false;
                        FC(row.Cells[2],c3);row.Cells[3].Borders.Visible=false;
                        FC(row.Cells[4],c5);
                    }
                    // spacer between rows
                    void FSpacer(){var r=st.AddRow();r.Height="4mm";r.Borders.Visible=false;r.Cells[0].MergeRight=4;}

                    FRow("Trainer Name :-","Signature","Date");
                    FSpacer();
                    FRow("Department Head","Signature","Date");
                    var ren=new MigraDoc.Rendering.PdfDocumentRenderer{Document=doc};ren.RenderDocument();ren.PdfDocument.Save(path);
                });
                ModernDialog.Show(Window.GetWindow(this),"PDF saved!","Success",ModernDialog.DialogType.Success);
            }
            catch(Exception ex){ModernDialog.Show(Window.GetWindow(this),"PDF failed: "+ex.Message,"Error",ModernDialog.DialogType.Error);}
        }

        private async void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not DataView view||view.Count==0){ModernDialog.Show(Window.GetWindow(this),"Generate first.","Info",ModernDialog.DialogType.Info);return;}
            var dlg=new Microsoft.Win32.SaveFileDialog{FileName=$"AssessmentMarkList_{DateTime.Now:yyyyMMdd}",DefaultExt=".xlsx",Filter="Excel Workbook|*.xlsx"};
            if(dlg.ShowDialog()!=true)return;
            string path=dlg.FileName,deptId=GT(CmbDept),streamId=GT(CmbStream),mc=GT(CmbModCode),insId=GT(CmbInstructor),lvlNum=GetLevelNum();
            string ay=(CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim()??"";
            string at=(CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString()??"";
            string ut="",nh="0"; foreach(var(code,title,hours) in _levelModules) if(code==mc){ut=title;nh=hours;break;}
            var rows=new List<(string,string,string,string,string,string,string,string)>();
            foreach(DataRowView drv in view) rows.Add((TG(drv,"full_name"),TG(drv,"gender"),TG(drv,"student_id"),TG(drv,"institutional_score"),TG(drv,"industry_score"),TG(drv,"total_score"),TG(drv,"letter_grade"),TG(drv,"grade_points")));
            try
            {
                await Task.Run(()=>{
                    var conn=_db.GetConnection();conn.Open();
                    string dn="",sectorName="",occupationName="",iname=insId;
                    string entryYear=ay,trainingYear=ay;
                    using(var c=new MySqlCommand("SELECT IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments WHERE dept_id=@d LIMIT 1",conn)){c.Parameters.AddWithValue("@d",deptId);dn=c.ExecuteScalar()?.ToString()??deptId;}
                    // sector = dept_name, occupation = stream_name
                    sectorName=dn;
                    try{using var c=new MySqlCommand("SELECT IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE stream_id=@s LIMIT 1",conn);c.Parameters.AddWithValue("@s",streamId);occupationName=c.ExecuteScalar()?.ToString()??streamId;}catch{occupationName=streamId;}
                    if(!string.IsNullOrEmpty(insId)){using var c=new MySqlCommand("SELECT TRIM(CONCAT_WS(' ', NULLIF(TRIM(first_name),''), NULLIF(TRIM(middle_name),''), NULLIF(TRIM(last_name),''))) FROM ecc_dof_wukrostmarycollege.employee_profile WHERE employee_id=@i LIMIT 1",conn);c.Parameters.AddWithValue("@i",insId);iname=c.ExecuteScalar()?.ToString()?.Trim()??insId;}
                    try{using var c=new MySqlCommand("SELECT IFNULL(MIN(entry_year),@ay) FROM ecc_dof_wukrostmarycollege.student_profile sp JOIN ecc_dof_wukrostmarycollege.student_assessment sa ON TRIM(sp.student_id)=TRIM(sa.student_id) WHERE sa.module_code=@m AND sp.dept_id=@d AND sp.stream_id=@s LIMIT 1",conn);c.Parameters.AddWithValue("@ay",ay);c.Parameters.AddWithValue("@m",mc);c.Parameters.AddWithValue("@d",deptId);c.Parameters.AddWithValue("@s",streamId);var ev=c.ExecuteScalar()?.ToString();if(!string.IsNullOrEmpty(ev))entryYear=ev;}catch{}
                    conn.Close();
                    string lvlR=lvlNum switch{"1"=>"I","2"=>"II","3"=>"III","4"=>"IV",_=>lvlNum};
                    int cols=9;
                    using var wb=new ClosedXML.Excel.XLWorkbook();var ws=wb.Worksheets.Add("MarkList");
                    int rn=1;
                    // Helper: merge+center across all cols
                    void MgAll(int row,string val,bool bold=false,int fs=10,bool ul=false){
                        ws.Range(row,1,row,cols).Merge();ws.Cell(row,1).Value=val;
                        ws.Cell(row,1).Style.Font.Bold=bold;ws.Cell(row,1).Style.Font.FontSize=fs;
                        ws.Cell(row,1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        if(ul)ws.Cell(row,1).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;
                    }
                    // Title rows
                    MgAll(rn,"ECC-DoA  WUKRO ST.MARY COLLEGE",true,13);rn++;
                    MgAll(rn,"REGISTRAR'S OFFICE",true,12);rn++;
                    MgAll(rn,"Assessment Results Summary Report to the Registrar",true,11,ul:true);rn++;
                    rn++; // blank row

                    // Info rows — NO borders, grey fill on cells, full-width
                    // Left block: cols 1-4 (merged), Right block: cols 5-9 (merged)
                    // For proper label/value layout we use individual cell styling
                    // Row: Dept+val | gap | InsLbl | InsVal
                    // Cols: 1=leftLbl, 2-3=leftVal, 4=gap, 5-6=rightLbl, 7-9=rightVal
                    var xlGrey=ClosedXML.Excel.XLColor.FromHtml("#E6E6E6");
                    var xlYellow=ClosedXML.Excel.XLColor.FromHtml("#FFFF00");
                    void XInfoRow(int row,string leftText,bool leftBold,bool leftUl,string rightLbl,string rightVal,bool rightBold,bool rightUl,int rightFs=10){
                        ws.Range(row,1,row,4).Merge();ws.Cell(row,1).Value=leftText;
                        ws.Cell(row,1).Style.Font.Bold=leftBold;ws.Cell(row,1).Style.Font.FontSize=10;
                        if(leftUl)ws.Cell(row,1).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;
                        ws.Cell(row,1).Style.Fill.BackgroundColor=xlGrey;
                        ws.Range(row,5,row,7).Merge();ws.Cell(row,5).Value=rightLbl;
                        ws.Cell(row,5).Style.Font.FontSize=10;ws.Cell(row,5).Style.Fill.BackgroundColor=xlGrey;
                        ws.Range(row,8,row,cols).Merge();ws.Cell(row,8).Value=rightVal;
                        ws.Cell(row,8).Style.Font.Bold=rightBold;ws.Cell(row,8).Style.Font.FontSize=rightFs;
                        if(rightUl)ws.Cell(row,8).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;
                        ws.Cell(row,8).Style.Fill.BackgroundColor=xlGrey;
                    }
                    XInfoRow(rn,$"Department:  {dn}",true,false,"Instructor ID:  ",insId,true,true);rn++;
                    // Entry Year row: right-aligned label, underlined value
                    ws.Cell(rn,1).Value="Entry Year :  ";ws.Cell(rn,1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;ws.Range(rn,1,rn,2).Merge();ws.Cell(rn,1).Style.Fill.BackgroundColor=xlGrey;
                    ws.Cell(rn,3).Value=entryYear;ws.Cell(rn,3).Style.Font.Bold=true;ws.Cell(rn,3).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;ws.Range(rn,3,rn,4).Merge();ws.Cell(rn,3).Style.Fill.BackgroundColor=xlGrey;
                    ws.Cell(rn,5).Value="Training Year:  ";ws.Range(rn,5,rn,7).Merge();ws.Cell(rn,5).Style.Fill.BackgroundColor=xlGrey;
                    ws.Cell(rn,8).Value=trainingYear;ws.Cell(rn,8).Style.Font.Bold=true;ws.Cell(rn,8).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;ws.Range(rn,8,rn,cols).Merge();ws.Cell(rn,8).Style.Fill.BackgroundColor=xlGrey;rn++;
                    rn++; // spacer
                    XInfoRow(rn,$"Sector :  {sectorName}",true,false,"Occupation :  ",occupationName,true,false,12);rn++;
                    rn++; // spacer
                    XInfoRow(rn,$"Unit of Competence:-  {ut}",true,true,"Module Code:  ",mc,true,true);rn++;
                    rn++; // spacer
                    // Nominal Duration row: left=label(grey), left-val=yellow, right=Program bold + Level + Roman
                    ws.Cell(rn,1).Value="Nominal Duration :  ";ws.Cell(rn,1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;ws.Range(rn,1,rn,2).Merge();ws.Cell(rn,1).Style.Fill.BackgroundColor=xlGrey;
                    ws.Cell(rn,3).Value=nh;ws.Cell(rn,3).Style.Font.Bold=true;ws.Range(rn,3,rn,4).Merge();ws.Cell(rn,3).Style.Fill.BackgroundColor=xlYellow;
                    ws.Cell(rn,5).Value=$"Program:  {at}";ws.Range(rn,5,rn,6).Merge();ws.Cell(rn,5).Style.Font.Bold=true;ws.Cell(rn,5).Style.Fill.BackgroundColor=xlGrey;
                    ws.Cell(rn,7).Value="Level  ";ws.Cell(rn,7).Style.Fill.BackgroundColor=xlGrey;
                    ws.Cell(rn,8).Value=lvlR;ws.Cell(rn,8).Style.Font.Bold=true;ws.Cell(rn,8).Style.Font.Underline=ClosedXML.Excel.XLFontUnderlineValues.Single;ws.Range(rn,8,rn,cols).Merge();ws.Cell(rn,8).Style.Fill.BackgroundColor=xlGrey;rn++;
                    rn++; // blank before table

                    // Column headers
                    string[] hdrs={"No.","Name of the trainees","sex","ID.NO.","Institutional\nassessment(70%)","Industry\nAssessment(30%)","Total Result\n(100%)","Grade in\nLetter","Grade in\npoint"};
                    for(int ci=0;ci<hdrs.Length;ci++){
                        ws.Cell(rn,ci+1).Value=hdrs[ci];
                        ws.Cell(rn,ci+1).Style.Font.Bold=true;
                        ws.Cell(rn,ci+1).Style.Fill.BackgroundColor=ClosedXML.Excel.XLColor.FromHtml("#D9E1F2");
                        ws.Cell(rn,ci+1).Style.Border.OutsideBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;
                        ws.Cell(rn,ci+1).Style.Alignment.WrapText=true;
                        ws.Cell(rn,ci+1).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        ws.Cell(rn,ci+1).Style.Alignment.Vertical=ClosedXML.Excel.XLAlignmentVerticalValues.Center;
                    }
                    ws.Row(rn).Height=42;rn++;
                    // Data rows
                    for(int i=0;i<rows.Count;i++){
                        var(nm,sx,id,inst,ind,tot,grd,pts)=rows[i];
                        // Grade in point = nominal_hours × grade_points (GPA scale)
                        double.TryParse(pts,out double gpVal);double.TryParse(nh,out double nhVal);
                        double gipNum=gpVal>0&&nhVal>0?Math.Round(gpVal*nhVal,0):0;
                        ws.Cell(rn,1).Value=i+1;
                        ws.Cell(rn,2).Value=nm;ws.Cell(rn,2).Style.Font.Bold=true;
                        ws.Cell(rn,3).Value=sx.Length>0?sx[0].ToString().ToUpper():"";
                        ws.Cell(rn,4).Value=id;
                        ws.Cell(rn,5).Value=inst;ws.Cell(rn,5).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                        ws.Cell(rn,6).Value=ind;ws.Cell(rn,6).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                        ws.Cell(rn,7).Value=tot;ws.Cell(rn,7).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                        ws.Cell(rn,8).Value=grd;ws.Cell(rn,8).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        if(gipNum>0)ws.Cell(rn,9).Value=gipNum;else ws.Cell(rn,9).Value=pts;
                        ws.Cell(rn,9).Style.Alignment.Horizontal=ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                        for(int ci=1;ci<=9;ci++)ws.Cell(rn,ci).Style.Border.OutsideBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;
                        rn++;
                    }
                    rn++; // blank before footer
                    // Footer: Trainer Name ___ | Signature ___ | Date ___
                    //         Department Head _ | Signature ___ | Date ___
                    // 5 cols: label | spacer | signature | spacer | date
                    void FooterRow(int row,string c1,string c3,string c5){
                        ws.Cell(row,1).Value=c1;ws.Range(row,1,row,3).Merge();
                        ws.Cell(row,1).Style.Border.BottomBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;
                        ws.Cell(row,4).Value=c3;ws.Range(row,4,row,6).Merge();
                        ws.Cell(row,4).Style.Border.BottomBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;
                        ws.Cell(row,7).Value=c5;ws.Range(row,7,row,cols).Merge();
                        ws.Cell(row,7).Style.Border.BottomBorder=ClosedXML.Excel.XLBorderStyleValues.Thin;
                    }
                    FooterRow(rn,"Trainer Name :-","Signature","Date");rn++;
                    rn++; // spacer
                    FooterRow(rn,"Department Head","Signature","Date");rn++;
                    // Column widths
                    ws.Column(1).Width=5;ws.Column(2).Width=28;ws.Column(3).Width=5;ws.Column(4).Width=15;
                    ws.Column(5).Width=13;ws.Column(6).Width=13;ws.Column(7).Width=11;ws.Column(8).Width=9;ws.Column(9).Width=11;
                    wb.SaveAs(path);
                });
                ModernDialog.Show(Window.GetWindow(this),"Excel saved!","Success",ModernDialog.DialogType.Success);
            }
            catch(Exception ex){ModernDialog.Show(Window.GetWindow(this),"Export failed: "+ex.Message,"Error",ModernDialog.DialogType.Error);}
        }

        private static string TG(DataRowView drv, string col) { try { return drv[col]?.ToString()?.Trim() ?? ""; } catch { return ""; } }
    }
}
