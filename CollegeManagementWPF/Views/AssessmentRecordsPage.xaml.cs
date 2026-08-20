using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class AssessmentRecordsPage : Page
    {
        private readonly DBConnect _db = new DBConnect();
        private string _selSid = "", _selLvl = "", _selMod = "";
        private int _selId = -1;
        private List<(double Min, double Max, string Letter, double Points)> _grades = new();
        private bool _suppress = false;
        private CancellationTokenSource? _searchCts;
        private List<(string Code, string Title)> _streamModules     = new();
        private List<(string Id,   string Name)>  _moduleInstructors = new();

        private const string CREATE_SQL =
            "CREATE TABLE IF NOT EXISTS ecc_dof_wukrostmarycollege.student_assessment (" +
            "  id INT AUTO_INCREMENT PRIMARY KEY," +
            "  student_id VARCHAR(50) NOT NULL," +
            "  level VARCHAR(10) NOT NULL," +
            "  module_code VARCHAR(50) NOT NULL," +
            "  employee_id VARCHAR(50)," +
            "  academic_year VARCHAR(20)," +
            "  institutional_score DECIMAL(5,2)," +
            "  industry_score DECIMAL(5,2)," +
            "  total_score DECIMAL(5,2)," +
            "  letter_grade VARCHAR(10)," +
            "  grade_points DECIMAL(4,2)" +
            ") ENGINE=InnoDB;";

        private const string BASE =
            "SELECT id,student_id,level,module_code,employee_id,academic_year," +
            "institutional_score,industry_score,total_score,letter_grade,grade_points " +
            "FROM ecc_dof_wukrostmarycollege.student_assessment";

        public AssessmentRecordsPage()
        {
            InitializeComponent();
            TxtInstitutional.PreviewTextInput += NumOnly;
            TxtIndustry.PreviewTextInput      += NumOnly;
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            ApplyPermissions();
            Loaded += async (s, e) => { await EnsureTableAsync(); await LoadGradesAsync(); await LoadAllInstructorsAsync(); await Load(BASE); };
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
            Grid1.Visibility     = SessionUser.Has("assess_view")   ? Visibility.Visible : Visibility.Collapsed;
            BtnSave.Visibility   = SessionUser.Has("assess_add")    ? Visibility.Visible : Visibility.Collapsed;
            BtnUpdate.Visibility = SessionUser.Has("assess_update") ? Visibility.Visible : Visibility.Collapsed;
            BtnDelete.Visibility = SessionUser.Has("assess_delete") ? Visibility.Visible : Visibility.Collapsed;
            BtnImport.Visibility   = SessionUser.Has("assess_import")   ? Visibility.Visible : Visibility.Collapsed;
            BtnTemplate.Visibility = SessionUser.Has("assess_template") ? Visibility.Visible : Visibility.Collapsed;
            BtnRemoveDup.Visibility= SessionUser.Has("assess_remove_dup") ? Visibility.Visible : Visibility.Collapsed;
            BtnAttachAssessment.Visibility = (SessionUser.Has("assess_add") || SessionUser.Has("assess_view")) ? Visibility.Visible : Visibility.Collapsed;
            BtnClear.Visibility  = (SessionUser.Has("assess_add") || SessionUser.Has("assess_update")) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnAttachAssessment_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var win = new AttachAssessmentWindow { Owner = Window.GetWindow(this) };
            win.ShowDialog();
        }

        // Load all instructors from employee_profile into the dropdown on startup
        private async Task LoadAllInstructorsAsync()
        {
            try
            {
                _moduleInstructors = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT employee_id, " +
                        "TRIM(CONCAT_WS(' ', NULLIF(TRIM(first_name),''), NULLIF(TRIM(middle_name),''), NULLIF(TRIM(last_name),''))) AS full_name " +
                        "FROM ecc_dof_wukrostmarycollege.employee_profile ORDER BY employee_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString()?.Trim() ?? ""));
                    conn.Close(); return list;
                });
                _suppress = true;
                CmbEmpID.Items.Clear();
                foreach (var (id, name) in _moduleInstructors)
                    CmbEmpID.Items.Add(new ComboBoxItem { Content = string.IsNullOrEmpty(name) ? id : $"{id} — {name}", Tag = id });
                _suppress = false;
            }
            catch { }
        }

        private async Task EnsureTableAsync()
        { try { await Task.Run(() => { var c=_db.GetConnection(); c.Open(); new MySqlCommand(CREATE_SQL,c).ExecuteNonQuery(); c.Close(); }); } catch { } }

        private async Task LoadGradesAsync()
        {
            try
            {
                _grades = await Task.Run(() =>
                {
                    var list = new List<(double,double,string,double)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand("SELECT min_score,max_score,letter_grade,grade_points FROM ecc_dof_wukrostmarycollege.grade_config ORDER BY min_score DESC",conn);
                    using var r = cmd.ExecuteReader();
                    while(r.Read()) list.Add((Convert.ToDouble(r[0]),Convert.ToDouble(r[1]),r[2]?.ToString()??"",Convert.ToDouble(r[3])));
                    conn.Close(); return list;
                });
            }
            catch { }
        }

        // ── Student ID debounced search ────────────────────────────────────────
        private void CmbStudID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;
            string typed = CmbStudID.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(typed)) return;

            Task.Delay(300, token).ContinueWith(async t =>
            {
                if (t.IsCanceled) return;
                try
                {
                    var results = await Task.Run(() =>
                    {
                        var list = new List<string>();
                        var conn = _db.GetConnection(); conn.Open();
                        using var cmd = new MySqlCommand(
                            "SELECT DISTINCT TRIM(student_id) FROM ecc_dof_wukrostmarycollege.student_profile " +
                            "WHERE TRIM(student_id) LIKE @q ORDER BY student_id LIMIT 50", conn);
                        cmd.Parameters.AddWithValue("@q", $"%{typed}%");
                        using var r = cmd.ExecuteReader();
                        while (r.Read()) { var v = r[0]?.ToString()??""; if (!string.IsNullOrEmpty(v)) list.Add(v); }
                        conn.Close(); return list;
                    }, token);
                    if (token.IsCancellationRequested) return;
                    await Dispatcher.InvokeAsync(() =>
                    {
                        _suppress = true;
                        CmbStudID.Items.Clear();
                        foreach (var id in results) CmbStudID.Items.Add(new ComboBoxItem { Content = id });
                        _suppress = false;
                        if (CmbStudID.Items.Count > 0) CmbStudID.IsDropDownOpen = true;
                    });
                }
                catch { }
            }, TaskScheduler.Default);
        }

        private void CmbStudID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            if (CmbStudID.SelectedItem is ComboBoxItem sel)
            {
                string id = sel.Content?.ToString() ?? "";
                _suppress = true;
                CmbStudID.Text = id;   // force text to show selected value
                _suppress = false;
                CmbStudID.IsDropDownOpen = false;
            }
        }

        private async void CmbStudID_LostFocus(object sender, RoutedEventArgs e)
        {
            string sid = GetStudentId();
            if (string.IsNullOrEmpty(sid)) return;
            await LoadStudentNameAsync(sid);
            await LoadStreamModulesAsync(sid);
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

        // ── Module Code — cascades from student's stream + selected level ────────
        private async Task LoadStreamModulesAsync(string sid)
        {
            string lvl = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            await LoadStreamModulesForLevelAsync(sid, lvl);
        }

        private async void CmbLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string sid = GetStudentId();
            string lvl = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            if (!string.IsNullOrEmpty(sid) && !string.IsNullOrEmpty(lvl))
                await LoadStreamModulesForLevelAsync(sid, lvl);
        }

        private async Task LoadStreamModulesForLevelAsync(string sid, string lvl)
        {
            try
            {
                _streamModules = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    // Get modules for this student's stream filtered by the selected level
                    string sql =
                        "SELECT c.module_code, IFNULL(c.unit_of_competence_title,'') " +
                        "FROM ecc_dof_wukrostmarycollege.courses c " +
                        "JOIN ecc_dof_wukrostmarycollege.levels lv ON c.level_id=lv.level_id " +
                        "JOIN ecc_dof_wukrostmarycollege.student_profile sp ON lv.stream_id=sp.stream_id " +
                        "WHERE TRIM(sp.student_id)=@s";
                    if (!string.IsNullOrEmpty(lvl)) sql += " AND lv.level=@l";
                    sql += " GROUP BY c.module_code ORDER BY c.module_code";
                    using var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@s", sid);
                    if (!string.IsNullOrEmpty(lvl)) cmd.Parameters.AddWithValue("@l", lvl);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close(); return list;
                });
                _suppress = true;
                CmbModCode.Items.Clear();
                foreach (var (code, title) in _streamModules)
                    CmbModCode.Items.Add(new ComboBoxItem
                    { Content = string.IsNullOrEmpty(title) ? code : $"{code} — {title}", Tag = code });
                if (CmbModCode.Items.Count > 0) CmbModCode.SelectedIndex = 0;
                _suppress = false;
                string mc = GetModuleCode();
                if (!string.IsNullOrEmpty(mc)) await LoadModuleInstructorsAsync(mc);
            }
            catch { }
        }

        private async void CmbModCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string mc = GetModuleCode();
            if (!string.IsNullOrEmpty(mc)) await LoadModuleInstructorsAsync(mc);
        }

        private string GetModuleCode()
        {
            if (CmbModCode.SelectedItem is ComboBoxItem s && s.Tag != null) return s.Tag.ToString()!;
            string t = CmbModCode.Text?.Trim() ?? "";
            int d = t.IndexOf(" — "); return d >= 0 ? t[..d].Trim() : t;
        }

        // ── Instructor — load ALL employees from employee_profile ─────────────
        private async Task LoadModuleInstructorsAsync(string moduleCode)
        {
            try
            {
                _moduleInstructors = await Task.Run(() =>
                {
                    var list = new List<(string,string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT employee_id, " +
                        "TRIM(CONCAT_WS(' ', NULLIF(TRIM(first_name),''), NULLIF(TRIM(middle_name),''), NULLIF(TRIM(last_name),''))) AS full_name " +
                        "FROM ecc_dof_wukrostmarycollege.employee_profile " +
                        "ORDER BY employee_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString()??"", r[1]?.ToString()?.Trim()??""));
                    conn.Close(); return list;
                });
                _suppress = true;
                CmbEmpID.Items.Clear();
                foreach (var (id, name) in _moduleInstructors)
                    CmbEmpID.Items.Add(new ComboBoxItem { Content = string.IsNullOrEmpty(name) ? id : $"{id} — {name}", Tag = id });
                _suppress = false;
            }
            catch { }
        }

        private string GetInstructorId()
        {
            if (CmbEmpID.SelectedItem is ComboBoxItem s && s.Tag != null) return s.Tag.ToString()!;
            string t = CmbEmpID.Text?.Trim() ?? "";
            int d = t.IndexOf(" — "); return d >= 0 ? t[..d].Trim() : t;
        }

        private string GetStudentId()
        {
            if (CmbStudID.SelectedItem is ComboBoxItem sel) return sel.Content?.ToString()?.Trim() ?? "";
            return CmbStudID.Text?.Trim() ?? "";
        }

        // ── Score ──────────────────────────────────────────────────────────────
        private void ScoreChanged(object sender, TextChangedEventArgs e)
        {
            if (!double.TryParse(TxtInstitutional.Text, out double inst) || !double.TryParse(TxtIndustry.Text, out double ind))
            { TxtTotal.Text=TxtGrade.Text=TxtGradePoints.Text=""; TxtScoreError.Visibility=Visibility.Collapsed; if(BtnSave!=null)BtnSave.IsEnabled=true; if(BtnUpdate!=null)BtnUpdate.IsEnabled=true; return; }
            double total = Math.Round(inst+ind,2); TxtTotal.Text=total.ToString("F2");
            if (total>100) { TxtGrade.Text=TxtGradePoints.Text=""; TxtScoreError.Visibility=Visibility.Visible; if(BtnSave!=null)BtnSave.IsEnabled=false; if(BtnUpdate!=null)BtnUpdate.IsEnabled=false; }
            else { TxtScoreError.Visibility=Visibility.Collapsed; if(BtnSave!=null)BtnSave.IsEnabled=true; if(BtnUpdate!=null)BtnUpdate.IsEnabled=true; var(letter,pts)=GetGrade(total); TxtGrade.Text=letter; TxtGradePoints.Text=pts.ToString("F2"); }
        }

        private (string Letter, double Points) GetGrade(double score)
        { foreach(var(min,max,letter,pts) in _grades){bool m=max>100?score>=min&&score<=100:score>=min&&score<=max; if(m)return(letter,pts);} return("NYC",0); }

        private void NumOnly(object s, System.Windows.Input.TextCompositionEventArgs e)
            => e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"[\d\.]");

        // ── Load / Grid ────────────────────────────────────────────────────────
        private async Task Load(string q)
        {
            try { if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Visible; var dt=await Task.Run(()=>{var t=new DataTable();new MySqlDataAdapter(q,_db.GetConnection()).Fill(t);return t;}); Grid1.ItemsSource=dt.DefaultView; }
            catch(Exception ex){Msg("DB Error: "+ex.Message,false);}
            finally{if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Collapsed;}
        }

        private void Grid1_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if(Grid1.SelectedItem is not DataRowView r) return;
            _selId=r["id"]!=DBNull.Value?Convert.ToInt32(r["id"]):-1;
            _selSid=r["student_id"]?.ToString()??""; _selLvl=r["level"]?.ToString()??""; _selMod=r["module_code"]?.ToString()??"";
            _suppress=true; CmbStudID.Text=_selSid; _suppress=false;
            TxtAcadYear.Text=r["academic_year"]?.ToString()??"";
            TxtInstitutional.Text=r["institutional_score"]?.ToString()??"";
            TxtIndustry.Text=r["industry_score"]?.ToString()??"";
            TxtTotal.Text=r["total_score"]?.ToString()??"";
            TxtGrade.Text=r["letter_grade"]?.ToString()??"";
            TxtGradePoints.Text=r["grade_points"]?.ToString()??"";
            SetCombo(CmbLevel,_selLvl); SetComboByContent(CmbModCode,_selMod);
            _suppress=true; CmbEmpID.Text=r["employee_id"]?.ToString()??""; _suppress=false;
        }

        private void SetCombo(ComboBox c, string v)
        { foreach(ComboBoxItem i in c.Items) if(i.Content?.ToString()==v){c.SelectedItem=i;return;} }

        private void SetComboByContent(ComboBox c, string v)
        { foreach(ComboBoxItem i in c.Items) if((i.Tag?.ToString()??i.Content?.ToString())==v){c.SelectedItem=i;return;} var item=new ComboBoxItem{Content=v,Tag=v};c.Items.Add(item);c.SelectedItem=item; }

        private string CmbVal(ComboBox c)
        { var t=c.Text?.Trim()??""; if(!string.IsNullOrEmpty(t)&&t!="(All)")return t; return(c.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim()??""; }

        // ── CRUD ──────────────────────────────────────────────────────────────
        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            string sid=GetStudentId(),lvl=CmbVal(CmbLevel),mod=GetModuleCode(),emp=GetInstructorId(),ay=TxtAcadYear.Text.Trim();
            if(string.IsNullOrWhiteSpace(sid)||string.IsNullOrWhiteSpace(lvl)||string.IsNullOrWhiteSpace(mod)||string.IsNullOrWhiteSpace(TxtInstitutional.Text)||string.IsNullOrWhiteSpace(TxtIndustry.Text)){Msg("Please fill all required fields!",false);return;}
            if(!double.TryParse(TxtInstitutional.Text,out double inst)||!double.TryParse(TxtIndustry.Text,out double ind)){Msg("Scores must be numbers!",false);return;}
            double total=Math.Round(inst+ind,2); if(total>100){Msg("Total score cannot exceed 100!",false);return;}
            var(letter,pts)=GetGrade(total);
            try
            {
                await Task.Run(()=>{
                    var conn=_db.GetConnection();conn.Open();
                    // ── Duplicate check: same student+level+module+instructor (not year)
                    using(var chk=new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_assessment " +
                        "WHERE TRIM(student_id)=@s AND level=@l AND module_code=@m AND IFNULL(employee_id,'')=@e",conn))
                    {
                        chk.Parameters.AddWithValue("@s",sid);chk.Parameters.AddWithValue("@l",lvl);
                        chk.Parameters.AddWithValue("@m",mod);chk.Parameters.AddWithValue("@e",emp);
                        int dup=Convert.ToInt32(chk.ExecuteScalar());
                        if(dup>0) throw new InvalidOperationException($"Duplicate: A record for Student '{sid}', Level {lvl}, Module '{mod}', Instructor '{emp}' already exists.");
                    }
                    var cmd=new MySqlCommand("INSERT INTO ecc_dof_wukrostmarycollege.student_assessment (student_id,level,module_code,employee_id,academic_year,institutional_score,industry_score,total_score,letter_grade,grade_points) VALUES(@s,@l,@m,@e,@y,@i,@n,@t,@g,@p)",conn);
                    cmd.Parameters.AddWithValue("@s",sid);cmd.Parameters.AddWithValue("@l",lvl);cmd.Parameters.AddWithValue("@m",mod);cmd.Parameters.AddWithValue("@e",emp);cmd.Parameters.AddWithValue("@y",ay);cmd.Parameters.AddWithValue("@i",inst);cmd.Parameters.AddWithValue("@n",ind);cmd.Parameters.AddWithValue("@t",total);cmd.Parameters.AddWithValue("@g",letter);cmd.Parameters.AddWithValue("@p",pts);cmd.ExecuteNonQuery();conn.Close();
                });
                Msg("Saved successfully!",true); await Load(BASE); Clear();
            }
            catch(InvalidOperationException ex){Msg(ex.Message,false);}
            catch(Exception ex){Msg("Error: "+ex.Message,false);}
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if(_selId<0){Msg("Select a record first.",false);return;}
            if(!double.TryParse(TxtInstitutional.Text,out double inst)||!double.TryParse(TxtIndustry.Text,out double ind)){Msg("Scores must be numbers!",false);return;}
            double total=Math.Round(inst+ind,2); if(total>100){Msg("Total score cannot exceed 100!",false);return;}
            var(letter,pts)=GetGrade(total); int id=_selId; string emp=GetInstructorId(),ay=TxtAcadYear.Text.Trim();
            try
            {
                await Task.Run(()=>{var conn=_db.GetConnection();conn.Open();var cmd=new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.student_assessment SET employee_id=@e,academic_year=@y,institutional_score=@i,industry_score=@n,total_score=@t,letter_grade=@g,grade_points=@p WHERE id=@id",conn);cmd.Parameters.AddWithValue("@e",emp);cmd.Parameters.AddWithValue("@y",ay);cmd.Parameters.AddWithValue("@i",inst);cmd.Parameters.AddWithValue("@n",ind);cmd.Parameters.AddWithValue("@t",total);cmd.Parameters.AddWithValue("@g",letter);cmd.Parameters.AddWithValue("@p",pts);cmd.Parameters.AddWithValue("@id",id);cmd.ExecuteNonQuery();conn.Close();});
                Msg("Updated!",true); await Load(BASE);
            }
            catch(Exception ex){Msg("Error: "+ex.Message,false);}
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if(_selId<0){Msg("Select a record first.",false);return;}
            var dlg=new ModernDialog($"Delete assessment for {_selSid}?","Confirm",ModernDialog.DialogType.Warning){Owner=Window.GetWindow(this)};
            if(dlg.ShowDialog()!=true)return; int id=_selId;
            try
            {
                await Task.Run(()=>{var conn=_db.GetConnection();conn.Open();var cmd=new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.student_assessment WHERE id=@id",conn);cmd.Parameters.AddWithValue("@id",id);cmd.ExecuteNonQuery();conn.Close();});
                Msg("Deleted!",true); await Load(BASE); Clear();
            }
            catch(Exception ex){Msg("Error: "+ex.Message,false);}
        }

        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string sid=TxtFStudID.Text.Trim(),yr=TxtFYear.Text.Trim(),lvl=CmbVal(CmbFLevel);
            var conds=new List<string>();
            if(!string.IsNullOrEmpty(sid)) conds.Add($"TRIM(student_id)='{sid.Replace("'","''")}'");
            if(!string.IsNullOrEmpty(yr))  conds.Add($"academic_year='{yr.Replace("'","''")}'");
            if(!string.IsNullOrEmpty(lvl)&&lvl!="(All)") conds.Add($"level='{lvl.Replace("'","''")}'");
            await Load(conds.Count>0?BASE+" WHERE "+string.Join(" AND ",conds):BASE);
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        { TxtFStudID.Text=TxtFYear.Text=""; CmbFLevel.SelectedIndex=0; await Load(BASE); }

        private async void TxtFilter_Changed(object s, TextChangedEventArgs e)
        { string t=TxtFilter.Text.Trim(); await Load(string.IsNullOrEmpty(t)?BASE:BASE+$" WHERE student_id LIKE '%{t}%' OR module_code LIKE '%{t}%'"); }

        private async void BtnReset_Click(object s, RoutedEventArgs e)
        { TxtFilter.Text=""; await Load(BASE); }

        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();

        private void Clear()
        {
            _suppress=true; CmbStudID.Text=""; _suppress=false;
            TxtStudentName.Visibility=Visibility.Collapsed;
            _suppress=true; CmbEmpID.Text=""; _suppress=false;
            TxtAcadYear.Text=TxtInstitutional.Text=TxtIndustry.Text="";
            TxtTotal.Text=TxtGrade.Text=TxtGradePoints.Text="";
            TxtScoreError.Visibility=Visibility.Collapsed;
            _selSid=_selLvl=_selMod=""; _selId=-1;
            if(BtnSave!=null)BtnSave.IsEnabled=true; if(BtnUpdate!=null)BtnUpdate.IsEnabled=true;
        }

        // ── Remove Duplicate Records ──────────────────────────────────────────
        // Keeps the row with the LOWEST id for each (student_id+level+module_code+employee_id)
        // and deletes all others.
        private async void BtnRemoveDup_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ModernDialog(
                "This will delete duplicate records, keeping only the earliest entry for each Student+Level+Module+Instructor combination. Continue?",
                "Remove Duplicates", ModernDialog.DialogType.Warning) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;

            int deleted = 0;
            try
            {
                deleted = await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    // Delete rows where id is NOT the minimum id for that group
                    using var cmd = new MySqlCommand(
                        "DELETE sa FROM ecc_dof_wukrostmarycollege.student_assessment sa " +
                        "INNER JOIN (" +
                        "  SELECT MIN(id) AS keep_id, student_id, level, module_code, IFNULL(employee_id,'') AS employee_id " +
                        "  FROM ecc_dof_wukrostmarycollege.student_assessment " +
                        "  GROUP BY TRIM(student_id), level, module_code, IFNULL(employee_id,'') " +
                        "  HAVING COUNT(*) > 1" +
                        ") dup ON TRIM(sa.student_id)=dup.student_id AND sa.level=dup.level " +
                        "   AND sa.module_code=dup.module_code AND IFNULL(sa.employee_id,'')=dup.employee_id " +
                        "   AND sa.id != dup.keep_id", conn);
                    int n = cmd.ExecuteNonQuery();
                    conn.Close();
                    return n;
                });
                Msg($"Removed {deleted} duplicate record(s). One record kept per group.", deleted >= 0);
                await Load(BASE);
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        // ── Download Excel Template ───────────────────────────────────────────
        private void BtnTemplate_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save Import Template",
                FileName = "AssessmentRecords_Template",
                DefaultExt = ".xlsx",
                Filter = "Excel Workbook|*.xlsx"
            };
            if (dlg.ShowDialog() != true) return;
            try
            {
                using var wb = new ClosedXML.Excel.XLWorkbook();
                var ws = wb.Worksheets.Add("Assessment Records");

                // Headers
                string[] headers = { "student_id", "level", "module_code", "employee_id", "academic_year", "institutional_score", "industry_score" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = ws.Cell(1, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#1A55DD");
                    cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                    cell.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                }

                // Sample row
                ws.Cell(2, 1).Value = "ICT/R/17/051";
                ws.Cell(2, 2).Value = "1";
                ws.Cell(2, 3).Value = "EIS HNS1 M01";
                ws.Cell(2, 4).Value = "Inst029";
                ws.Cell(2, 5).Value = "2017";
                ws.Cell(2, 6).Value = 55;
                ws.Cell(2, 7).Value = 24;

                // Note row
                ws.Cell(4, 1).Value = "Note: student_id, level, module_code are required. institutional_score (max 70) + industry_score (max 30) = total (max 100).";
                ws.Cell(4, 1).Style.Font.Italic = true;
                ws.Cell(4, 1).Style.Font.FontColor = ClosedXML.Excel.XLColor.Gray;
                ws.Range(4, 1, 4, headers.Length).Merge();

                ws.Columns().AdjustToContents();
                wb.SaveAs(dlg.FileName);
                Msg("Template saved!", true);
            }
            catch (Exception ex) { Msg("Failed to save template: " + ex.Message, false); }
        }

        // ── Import from Excel (with cascade validation) ───────────────────────
        // Validates: student_id exists, level is valid for student's stream,
        //            module_code belongs to that level, academic_year is numeric.
        private async void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Select Excel file to import",
                Filter = "Excel Workbook|*.xlsx;*.xls",
                DefaultExt = ".xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            int inserted = 0, skipped = 0;
            var errors = new System.Text.StringBuilder();

            try
            {
                await Task.Run(() =>
                {
                    using var wb = new ClosedXML.Excel.XLWorkbook(dlg.FileName);
                    var ws = wb.Worksheets.Worksheet(1);

                    // Map headers (case-insensitive)
                    var colMap = new System.Collections.Generic.Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    foreach (var cell in ws.Row(1).CellsUsed())
                        colMap[cell.Value.ToString().Trim()] = cell.Address.ColumnNumber;

                    string[] required = { "student_id", "level", "module_code" };
                    foreach (var req in required)
                        if (!colMap.ContainsKey(req))
                            throw new Exception($"Missing required column: '{req}'");

                    int Get(string col) => colMap.TryGetValue(col, out int c) ? c : 0;
                    string Val(ClosedXML.Excel.IXLRow row, string col)
                    { int c = Get(col); return c > 0 ? row.Cell(c).Value.ToString().Trim() : ""; }

                    var conn = _db.GetConnection(); conn.Open();

                    // Pre-load validation sets
                    var validStudents = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (var cmd = new MySqlCommand("SELECT DISTINCT TRIM(student_id) FROM ecc_dof_wukrostmarycollege.student_profile", conn))
                    using (var r = cmd.ExecuteReader()) while (r.Read()) validStudents.Add(r[0]?.ToString() ?? "");

                    // student → stream_id mapping
                    var studentStream = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    using (var cmd = new MySqlCommand("SELECT DISTINCT TRIM(student_id), stream_id FROM ecc_dof_wukrostmarycollege.student_profile GROUP BY TRIM(student_id)", conn))
                    using (var r = cmd.ExecuteReader()) while (r.Read()) studentStream[r[0]?.ToString() ?? ""] = r[1]?.ToString() ?? "";

                    // stream+level → set of valid module_codes
                    var streamLevelModules = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<string>>(StringComparer.OrdinalIgnoreCase);
                    using (var cmd = new MySqlCommand(
                        "SELECT lv.stream_id, lv.level, c.module_code " +
                        "FROM ecc_dof_wukrostmarycollege.courses c " +
                        "JOIN ecc_dof_wukrostmarycollege.levels lv ON c.level_id=lv.level_id", conn))
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                        {
                            string key = $"{r[0]}|{r[1]}";
                            if (!streamLevelModules.ContainsKey(key))
                                streamLevelModules[key] = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            streamLevelModules[key].Add(r[2]?.ToString() ?? "");
                        }

                    int lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
                    for (int rn = 2; rn <= lastRow; rn++)
                    {
                        var row = ws.Row(rn);
                        if (row.IsEmpty()) continue;

                        string sid  = Val(row, "student_id");
                        string lvl  = Val(row, "level");
                        string mod  = Val(row, "module_code");
                        string emp  = Val(row, "employee_id");
                        string ay   = Val(row, "academic_year");
                        string inst = Val(row, "institutional_score");
                        string ind  = Val(row, "industry_score");

                        // ── Required fields
                        if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(lvl) || string.IsNullOrEmpty(mod))
                        { errors.AppendLine($"Row {rn}: student_id, level or module_code is empty — skipped."); skipped++; continue; }

                        // ── 1. Validate student exists
                        if (!validStudents.Contains(sid))
                        { errors.AppendLine($"Row {rn}: Student '{sid}' not found."); skipped++; continue; }

                        // ── 2. Validate level is numeric 1–4
                        if (!int.TryParse(lvl, out int lvlInt) || lvlInt < 1 || lvlInt > 4)
                        { errors.AppendLine($"Row {rn}: Level '{lvl}' is invalid (must be 1–4)."); skipped++; continue; }

                        // ── 3. Cascade — module must belong to student's stream + level
                        if (studentStream.TryGetValue(sid, out string? streamId))
                        {
                            string key = $"{streamId}|{lvl}";
                            if (streamLevelModules.TryGetValue(key, out var mods) && !mods.Contains(mod))
                            { errors.AppendLine($"Row {rn}: Module '{mod}' does not belong to level {lvl} of student '{sid}' stream."); skipped++; continue; }
                        }

                        // ── 4. Academic year — must be a 4-digit year if provided
                        if (!string.IsNullOrEmpty(ay) && (!int.TryParse(ay, out int ayInt) || ayInt < 1900 || ayInt > 2100))
                        { errors.AppendLine($"Row {rn}: Academic year '{ay}' is invalid."); skipped++; continue; }

                        // ── 5. Score validation
                        if (!double.TryParse(inst, out double instVal)) instVal = 0;
                        if (!double.TryParse(ind,  out double indVal))  indVal  = 0;
                        double total = Math.Round(instVal + indVal, 2);
                        if (total > 100) { errors.AppendLine($"Row {rn}: Total score {total} exceeds 100."); skipped++; continue; }

                        var (letter, pts) = GetGrade(total);

                        try
                        {
                            // ── Duplicate check before insert
                            using (var chk = new MySqlCommand(
                                "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_assessment " +
                                "WHERE TRIM(student_id)=@s AND level=@l AND module_code=@m AND IFNULL(employee_id,'')=@e", conn))
                            {
                                chk.Parameters.AddWithValue("@s", sid); chk.Parameters.AddWithValue("@l", lvl);
                                chk.Parameters.AddWithValue("@m", mod); chk.Parameters.AddWithValue("@e", emp);
                                int dup = Convert.ToInt32(chk.ExecuteScalar());
                                if (dup > 0)
                                { errors.AppendLine($"Row {rn}: Duplicate — Student '{sid}', Level {lvl}, Module '{mod}', Instructor '{emp}' already exists."); skipped++; continue; }
                            }
                            var cmd = new MySqlCommand(
                                "INSERT INTO ecc_dof_wukrostmarycollege.student_assessment " +
                                "(student_id,level,module_code,employee_id,academic_year," +
                                "institutional_score,industry_score,total_score,letter_grade,grade_points) " +
                                "VALUES(@s,@l,@m,@e,@y,@i,@n,@t,@g,@p)", conn);
                            cmd.Parameters.AddWithValue("@s", sid);
                            cmd.Parameters.AddWithValue("@l", lvl);
                            cmd.Parameters.AddWithValue("@m", mod);
                            cmd.Parameters.AddWithValue("@e", emp);
                            cmd.Parameters.AddWithValue("@y", ay);
                            cmd.Parameters.AddWithValue("@i", instVal);
                            cmd.Parameters.AddWithValue("@n", indVal);
                            cmd.Parameters.AddWithValue("@t", total);
                            cmd.Parameters.AddWithValue("@g", letter);
                            cmd.Parameters.AddWithValue("@p", pts);
                            cmd.ExecuteNonQuery();
                            inserted++;
                        }
                        catch (Exception ex2) { errors.AppendLine($"Row {rn}: {ex2.Message}"); skipped++; }
                    }
                    conn.Close();
                });

                string summary = $"Import complete.\nInserted: {inserted}  |  Skipped: {skipped}";
                if (errors.Length > 0) summary += $"\n\nValidation Errors:\n{errors}";
                Msg(summary, inserted > 0);
                await Load(BASE);
            }
            catch (Exception ex) { Msg("Import failed: " + ex.Message, false); }
        }

        private void Msg(string m, bool ok)
        { var o=Window.GetWindow(this); if(ok)ModernDialog.Show(o,m,"Success",ModernDialog.DialogType.Success); else ModernDialog.Show(o,m,"Error",ModernDialog.DialogType.Error); }
    }
}
