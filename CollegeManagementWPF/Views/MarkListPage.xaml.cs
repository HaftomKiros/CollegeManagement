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
    public partial class MarkListPage : Page
    {
        private readonly DBConnect _db = new DBConnect();
        private bool _suppress = false;

        // Master lists — loaded once, never change
        private List<(string Id, string Name)>           _allDepts   = new();
        private List<(string Id, string Name)>           _allStreams  = new();
        private List<(string LevelId, string LevelNum)>  _streamLevels = new(); // per stream
        private List<(string Code, string Title)>        _allModules = new();

        public MarkListPage()
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

        // ── 1. DEPARTMENT — load on startup ──────────────────────────────────
        private async Task LoadDeptsAsync()
        {
            try
            {
                _allDepts = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT dept_id, IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close();
                    return list;
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
            {
                if (string.IsNullOrEmpty(filter) ||
                    id.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    CmbDept.Items.Add(new ComboBoxItem
                    {
                        Content = string.IsNullOrEmpty(name) ? id : $"{id} — {name}",
                        Tag = id
                    });
                }
            }
            _suppress = false;
        }

        private void CmbDept_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            RefreshDeptDropdown(CmbDept.Text?.Trim() ?? "");
            if (CmbDept.Items.Count > 0 && !string.IsNullOrEmpty(CmbDept.Text))
                CmbDept.IsDropDownOpen = true;
        }

        private async void CmbDept_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string deptId = GetDeptId();
            if (string.IsNullOrEmpty(deptId)) return;
            // Clear downstream
            ClearFrom(CmbStream);
            await LoadStreamsAsync(deptId);
        }

        private string GetDeptId()
        {
            if (CmbDept.SelectedItem is ComboBoxItem s && s.Tag != null) return s.Tag.ToString()!;
            string t = CmbDept.Text?.Trim() ?? "";
            int d = t.IndexOf(" — "); return d >= 0 ? t[..d].Trim() : t;
        }

        // ── 2. STREAM — searchable by ID or name, cascades from dept ─────────
        private async Task LoadStreamsAsync(string deptId)
        {
            try
            {
                _allStreams = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT stream_id, IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams " +
                        "WHERE dept_id=@d ORDER BY stream_id", conn);
                    cmd.Parameters.AddWithValue("@d", deptId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close();
                    return list;
                });
                RefreshStreamDropdown("");
                if (CmbStream.Items.Count > 0) CmbStream.SelectedIndex = 0;
            }
            catch { }
        }

        private void RefreshStreamDropdown(string filter)
        {
            _suppress = true;
            CmbStream.Items.Clear();
            foreach (var (id, name) in _allStreams)
            {
                if (string.IsNullOrEmpty(filter) ||
                    id.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    CmbStream.Items.Add(new ComboBoxItem
                    {
                        Content = string.IsNullOrEmpty(name) ? id : $"{id} — {name}",
                        Tag = id
                    });
                }
            }
            _suppress = false;
        }

        private void CmbStream_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            RefreshStreamDropdown(CmbStream.Text?.Trim() ?? "");
            if (CmbStream.Items.Count > 0 && !string.IsNullOrEmpty(CmbStream.Text))
                CmbStream.IsDropDownOpen = true;
        }

        private async void CmbStream_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string streamId = GetStreamId();
            if (string.IsNullOrEmpty(streamId)) return;
            ClearFrom(CmbLevel);
            await LoadLevelsAsync(streamId);
        }

        private string GetStreamId()
        {
            if (CmbStream.SelectedItem is ComboBoxItem s && s.Tag != null) return s.Tag.ToString()!;
            string t = CmbStream.Text?.Trim() ?? "";
            int d = t.IndexOf(" — "); return d >= 0 ? t[..d].Trim() : t;
        }

        // ── 3. LEVEL — searchable by level_id or numeric level number ──────────
        private async Task LoadLevelsAsync(string streamId)
        {
            try
            {
                _streamLevels = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT level_id, level FROM ecc_dof_wukrostmarycollege.levels " +
                        "WHERE stream_id=@s ORDER BY level", conn);
                    cmd.Parameters.AddWithValue("@s", streamId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close();
                    return list;
                });

                RefreshLevelDropdown("");
                if (CmbLevel.Items.Count > 0) CmbLevel.SelectedIndex = 0;

                string lvlId  = (CmbLevel.SelectedItem as ComboBoxItem)?.Tag?.ToString()     ?? "";
                string lvlNum = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                if (!string.IsNullOrEmpty(lvlId))
                    await Task.WhenAll(LoadModulesAsync(lvlId), LoadInstructorsAsync(GetDeptId(), lvlNum));
            }
            catch { }
        }

        private void RefreshLevelDropdown(string filter)
        {
            _suppress = true;
            CmbLevel.Items.Clear();
            foreach (var (lid, lnum) in _streamLevels)
            {
                if (string.IsNullOrEmpty(filter) ||
                    lid.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    lnum.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    CmbLevel.Items.Add(new ComboBoxItem
                    {
                        Content = $"{lid} — {lnum}",  // e.g. "EIS HNS1 — 1"
                        Tag     = lid                  // level_id for module lookup
                    });
                }
            }
            _suppress = false;
        }

        private void CmbLevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            RefreshLevelDropdown(CmbLevel.Text?.Trim() ?? "");
            if (CmbLevel.Items.Count > 0 && !string.IsNullOrEmpty(CmbLevel.Text))
                CmbLevel.IsDropDownOpen = true;
        }

        private async void CmbLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string lvlId  = (CmbLevel.SelectedItem as ComboBoxItem)?.Tag?.ToString()     ?? "";
            string lvlNum = GetLevelNum();
            if (string.IsNullOrEmpty(lvlId)) return;
            ClearFrom(CmbModCode);
            ClearFrom(CmbInstructor);
            await Task.WhenAll(LoadModulesAsync(lvlId), LoadInstructorsAsync(GetDeptId(), lvlNum));
        }

        // ── 4. MODULE CODE — filtered by level ────────────────────────────────
        private List<(string Code, string Title)> _levelModules = new();

        private async Task LoadModulesAsync(string level)
        {
            try
            {
                _levelModules = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT module_code, IFNULL(unit_of_competence_title,'') " +
                        "FROM ecc_dof_wukrostmarycollege.courses " +
                        "WHERE level_id=@l ORDER BY module_code", conn);
                    cmd.Parameters.AddWithValue("@l", level);                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close();
                    return list;
                });
                RefreshModuleDropdown("");
            }
            catch { }
        }

        private void RefreshModuleDropdown(string filter)
        {
            _suppress = true;
            CmbModCode.Items.Clear();
            foreach (var (code, title) in _levelModules)
            {
                if (string.IsNullOrEmpty(filter) ||
                    code.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    title.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    CmbModCode.Items.Add(new ComboBoxItem
                    {
                        Content = string.IsNullOrEmpty(title) ? code : $"{code} — {title}",
                        Tag = code
                    });
                }
            }
            _suppress = false;
        }

        private void CmbModCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            RefreshModuleDropdown(CmbModCode.Text?.Trim() ?? "");
            if (CmbModCode.Items.Count > 0 && !string.IsNullOrEmpty(CmbModCode.Text))
                CmbModCode.IsDropDownOpen = true;
        }

        private async void CmbModCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string mc = GetModuleCode();
            // Reload instructors scoped to this module (or fall back to dept+level if cleared)
            string lvlNum = GetLevelNum();
            ClearFrom(CmbInstructor);
            await LoadInstructorsAsync(GetDeptId(), lvlNum, mc);
        }

        private string GetModuleCode()
        {
            if (CmbModCode.SelectedItem is ComboBoxItem s && s.Tag != null) return s.Tag.ToString()!;
            string t = CmbModCode.Text?.Trim() ?? "";
            int d = t.IndexOf(" — "); return d >= 0 ? t[..d].Trim() : t;
        }

        // Extract numeric level from "EIS HNS3 — 3" → "3"
        private string GetLevelNum()
        {
            string display = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            int dash = display.IndexOf(" — ");
            return dash >= 0 ? display[(dash + 3)..].Trim() : display;
        }

        // ── 5. INSTRUCTOR — filtered by dept + level ──────────────────────────
        private List<(string Id, string Name)> _levelInstructors = new();

        private async Task LoadInstructorsAsync(string deptId, string level, string moduleCode = "")
        {
            try
            {
                _levelInstructors = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();

                    string sql;
                    MySqlCommand cmd;

                    if (!string.IsNullOrEmpty(moduleCode))
                    {
                        // When a module is selected: get instructors directly from student_mark
                        // for that module — no dept/level filter needed, most accurate
                        sql =
                            "SELECT DISTINCT sm.employee_id, " +
                            "IFNULL(CONCAT(TRIM(ep.first_name),' ',TRIM(ep.middle_name),' ',TRIM(ep.last_name)), sm.employee_id) AS full_name " +
                            "FROM ecc_dof_wukrostmarycollege.student_mark sm " +
                            "LEFT JOIN ecc_dof_wukrostmarycollege.employee_profile ep ON sm.employee_id=ep.employee_id " +
                            "WHERE sm.module_code=@mc AND sm.employee_id IS NOT NULL " +
                            "ORDER BY sm.employee_id";
                        cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@mc", moduleCode);
                    }
                    else
                    {
                        // No module selected: get instructors by dept + level
                        sql =
                            "SELECT DISTINCT sm.employee_id, " +
                            "IFNULL(CONCAT(TRIM(ep.first_name),' ',TRIM(ep.middle_name),' ',TRIM(ep.last_name)), sm.employee_id) AS full_name " +
                            "FROM ecc_dof_wukrostmarycollege.student_mark sm " +
                            "JOIN ecc_dof_wukrostmarycollege.student_profile sp " +
                            "ON TRIM(sm.student_id)=TRIM(sp.student_id) AND sm.level=sp.level " +
                            "LEFT JOIN ecc_dof_wukrostmarycollege.employee_profile ep ON sm.employee_id=ep.employee_id " +
                            "WHERE sp.dept_id=@d AND sm.level=@l AND sm.employee_id IS NOT NULL " +
                            "ORDER BY sm.employee_id";
                        cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@d", deptId);
                        cmd.Parameters.AddWithValue("@l", level);
                    }

                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString()?.Trim() ?? ""));
                    conn.Close();
                    return list;
                });
                RefreshInstructorDropdown("");
            }
            catch { }
        }

        private void RefreshInstructorDropdown(string filter)
        {
            _suppress = true;
            CmbInstructor.Items.Clear();
            foreach (var (id, name) in _levelInstructors)
            {
                if (string.IsNullOrEmpty(filter) ||
                    id.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    CmbInstructor.Items.Add(new ComboBoxItem { Content = $"{id} — {name}", Tag = id });
                }
            }
            _suppress = false;
        }

        private void CmbInstructor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            RefreshInstructorDropdown(CmbInstructor.Text?.Trim() ?? "");
            if (CmbInstructor.Items.Count > 0 && !string.IsNullOrEmpty(CmbInstructor.Text))
                CmbInstructor.IsDropDownOpen = true;
        }

        private async void CmbInstructor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string insId = GetInstructorId();
            string mc    = GetModuleCode();
            await LoadAcadYearsAsync(insId, mc);
        }

        private string GetInstructorId()
        {
            if (CmbInstructor.SelectedItem is ComboBoxItem s && s.Tag != null) return s.Tag.ToString()!;
            string t = CmbInstructor.Text?.Trim() ?? "";
            int d = t.IndexOf(" — "); return d >= 0 ? t[..d].Trim() : t;
        }

        // ── 6. ACADEMIC YEAR — cascades from instructor + module ─────────────
        private async Task LoadAcadYearsAsync(string insId, string moduleCode)
        {
            try
            {
                var years = await Task.Run(() =>
                {
                    var list = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();

                    string sql = "SELECT DISTINCT academic_year FROM ecc_dof_wukrostmarycollege.student_mark " +
                                 "WHERE employee_id IS NOT NULL AND academic_year IS NOT NULL";
                    if (!string.IsNullOrEmpty(insId))    sql += " AND employee_id=@ins";
                    if (!string.IsNullOrEmpty(moduleCode)) sql += " AND module_code=@mc";
                    sql += " ORDER BY academic_year";

                    using var cmd = new MySqlCommand(sql, conn);
                    if (!string.IsNullOrEmpty(insId))    cmd.Parameters.AddWithValue("@ins", insId);
                    if (!string.IsNullOrEmpty(moduleCode)) cmd.Parameters.AddWithValue("@mc", moduleCode);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        var v = r[0]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(v)) list.Add(v);
                    }
                    conn.Close();
                    return list;
                });

                CmbAcadYear.Items.Clear();
                // Add a blank "All years" option at top
                CmbAcadYear.Items.Add(new ComboBoxItem { Content = "" });
                foreach (var y in years)
                    CmbAcadYear.Items.Add(new ComboBoxItem { Content = y });
                if (CmbAcadYear.Items.Count > 1) CmbAcadYear.SelectedIndex = 1;
            }
            catch { }
        }

        // ── Helper: clear a combo and all combos after it ─────────────────────
        private void ClearFrom(ComboBox start)
        {
            _suppress = true;
            bool clear = false;
            foreach (var cmb in new[] { CmbStream, CmbLevel, CmbModCode, CmbInstructor, CmbAcadYear })
            {
                if (cmb == start) clear = true;
                if (clear) cmb.Items.Clear();
            }
            _suppress = false;
        }

        // ── Generate ─────────────────────────────────────────────────────────
        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            string deptId   = GetDeptId();
            string streamId = GetStreamId();

            if (string.IsNullOrEmpty(deptId) || string.IsNullOrEmpty(streamId))
            {
                ModernDialog.Show(Window.GetWindow(this), "Department and Stream are required!", "Error", ModernDialog.DialogType.Error);
                return;
            }

            // Extract the numeric level stored in student_mark (the number after " — ")
            string lvlNum = "";
            if (CmbLevel.SelectedItem is ComboBoxItem lvlItem)
            {
                string display = lvlItem.Content?.ToString() ?? "";
                int dash = display.IndexOf(" — ");
                lvlNum = dash >= 0 ? display[(dash + 3)..].Trim() : display;
            }

            string at    = (CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Regular";
            string mc    = GetModuleCode();   // raw module_code from Tag
            string insId = GetInstructorId(); // raw employee_id from Tag
            string ay    = (CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";

            try
            {
                var dt  = new DataTable();
                string sql =
                    "SELECT sm.student_id, sm.level, sm.module_code, sm.employee_id, " +
                    "CONCAT(TRIM(ep.first_name),' ',TRIM(ep.middle_name)) AS instructor_name, " +
                    "sm.academic_year, sm.score_of_knowledge_test, sm.score_of_practical_test, sm.competence, " +
                    "CONCAT(TRIM(sp.first_name),' ',TRIM(sp.father_name),' ',TRIM(sp.grand_father_name)) AS student_name, " +
                    "sp.gender " +
                    "FROM ecc_dof_wukrostmarycollege.student_mark sm " +
                    "JOIN ecc_dof_wukrostmarycollege.student_profile sp " +
                    "ON TRIM(sm.student_id)=TRIM(sp.student_id) AND sm.level=sp.level " +
                    "LEFT JOIN ecc_dof_wukrostmarycollege.employee_profile ep ON sm.employee_id=ep.employee_id " +
                    "WHERE sp.dept_id=@d AND sp.stream_id=@s AND sp.admission_type=@at";

                if (!string.IsNullOrEmpty(lvlNum)) sql += " AND sm.level=@l";
                if (!string.IsNullOrEmpty(mc))     sql += " AND sm.module_code=@mc";
                if (!string.IsNullOrEmpty(insId))  sql += " AND sm.employee_id=@ins";
                if (!string.IsNullOrEmpty(ay))     sql += " AND sm.academic_year=@ay";
                sql += " ORDER BY sm.student_id, sm.module_code";

                await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    var cmd  = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@d",  deptId);
                    cmd.Parameters.AddWithValue("@s",  streamId);
                    cmd.Parameters.AddWithValue("@at", at);
                    if (!string.IsNullOrEmpty(lvlNum)) cmd.Parameters.AddWithValue("@l",   lvlNum);
                    if (!string.IsNullOrEmpty(mc))     cmd.Parameters.AddWithValue("@mc",  mc);
                    if (!string.IsNullOrEmpty(insId))  cmd.Parameters.AddWithValue("@ins", insId);
                    if (!string.IsNullOrEmpty(ay))     cmd.Parameters.AddWithValue("@ay",  ay);
                    new MySqlDataAdapter(cmd).Fill(dt);
                });

                Grid1.ItemsSource      = dt.DefaultView;
                PreviewCard.Visibility = Visibility.Visible;
                TxtPreviewInfo.Text    =
                    $"Dept: {deptId} | Stream: {streamId} | Level: {lvlNum} | Adm: {at}" +
                    (string.IsNullOrEmpty(mc)    ? "" : $" | Module: {mc}") +
                    (string.IsNullOrEmpty(ay)    ? "" : $" | Year: {ay}") +
                    $" — {dt.Rows.Count} records";
            }
            catch (Exception ex)
            {
                ModernDialog.Show(Window.GetWindow(this), "Error: " + ex.Message, "DB Error", ModernDialog.DialogType.Error);
            }
        }

        // ── Print to PDF — Official Competency Assessment Report format ─────────
        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not DataView view || view.Count == 0)
            { ModernDialog.Show(Window.GetWindow(this), "Generate the report first.", "Info", ModernDialog.DialogType.Info); return; }

            var dlg = new Microsoft.Win32.SaveFileDialog
            { FileName = $"MarkList_{DateTime.Now:yyyyMMdd}", DefaultExt = ".pdf", Filter = "PDF|*.pdf" };
            if (dlg.ShowDialog() != true) return;

            try
            {
                string path = dlg.FileName;

                // ── Read ALL UI values on the UI thread BEFORE Task.Run ───────
                string deptId  = GetDeptId();
                string streamId = GetStreamId();
                string lvlNum  = GetLevelNum();
                string mc      = GetModuleCode();
                string insId   = GetInstructorId();
                string ay      = (CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
                string admType = (CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

                // Snapshot the DataView rows on the UI thread
                var rows = new List<(string Name, string Sex, string Id, string KT, string PT, string Comp)>();
                foreach (DataRowView drv in view)
                    rows.Add((
                        TryGet(drv, "student_name"),
                        TryGet(drv, "gender"),
                        TryGet(drv, "student_id"),
                        TryGet(drv, "score_of_knowledge_test"),
                        TryGet(drv, "score_of_practical_test"),
                        TryGet(drv, "competence")
                    ));

                // ── All DB + PDF work on background thread ────────────────────
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();

                    var cmd = new MySqlCommand(
                        "SELECT IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments WHERE dept_id=@d LIMIT 1", conn);
                    cmd.Parameters.AddWithValue("@d", deptId);
                    string deptName = cmd.ExecuteScalar()?.ToString() ?? deptId;

                    cmd = new MySqlCommand(
                        "SELECT IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE stream_id=@s LIMIT 1", conn);
                    cmd.Parameters.AddWithValue("@s", streamId);
                    string streamName = cmd.ExecuteScalar()?.ToString() ?? streamId;

                    string insName = insId;
                    if (!string.IsNullOrEmpty(insId))
                    {
                        cmd = new MySqlCommand(
                            "SELECT CONCAT(TRIM(first_name),' ',TRIM(middle_name),' ',TRIM(last_name)) " +
                            "FROM ecc_dof_wukrostmarycollege.employee_profile WHERE employee_id=@i LIMIT 1", conn);
                        cmd.Parameters.AddWithValue("@i", insId);
                        insName = cmd.ExecuteScalar()?.ToString()?.Trim() ?? insId;
                    }

                    string unitTitle = "";
                    if (!string.IsNullOrEmpty(mc))
                    {
                        cmd = new MySqlCommand(
                            "SELECT IFNULL(unit_of_competence_title,'') FROM ecc_dof_wukrostmarycollege.courses WHERE module_code=@m LIMIT 1", conn);
                        cmd.Parameters.AddWithValue("@m", mc);
                        unitTitle = cmd.ExecuteScalar()?.ToString() ?? "";
                    }
                    conn.Close();

                    // ── Build PDF ─────────────────────────────────────────────
                    var doc = new MigraDoc.DocumentObjectModel.Document();
                    doc.Styles["Normal"].Font.Name = "Times New Roman";
                    doc.Styles["Normal"].Font.Size = 10;

                    var sec = doc.AddSection();
                    sec.PageSetup.PageFormat   = MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.Orientation  = MigraDoc.DocumentObjectModel.Orientation.Portrait;
                    sec.PageSetup.TopMargin    = "1.8cm";
                    sec.PageSetup.BottomMargin = "2.0cm";
                    sec.PageSetup.LeftMargin   = "1.8cm";
                    sec.PageSetup.RightMargin  = "1.8cm";

                    // ── Title block (body, page 1) ────────────────────────────
                    void CentBold(string text, double size)
                    {
                        var p = sec.AddParagraph(text);
                        p.Format.Alignment   = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        p.Format.Font.Bold   = true;
                        p.Format.Font.Size   = size;
                        p.Format.Font.Name   = "Times New Roman";
                        p.Format.SpaceBefore = "0mm";
                        p.Format.SpaceAfter  = "1mm";
                    }

                    CentBold("ECC-DoA ST.MARY COLLEGE", 14);
                    CentBold("REGISTRAR'S OFFICE", 13);
                    CentBold("OFFICIAL COMPETENCY ASSESSMENT REPORTS SUMMARY", 11);

                    sec.AddParagraph().Format.SpaceAfter = "3mm";

                    // ── Info block (body, page 1) ─────────────────────────────
                    var info = sec.AddTable();
                    info.Borders.Width = 0;
                    info.AddColumn("9.1cm");
                    info.AddColumn("9.1cm");
                    info.TopPadding    = "0.8mm";
                    info.BottomPadding = "0.8mm";

                    void InfoRow(string label1, string val1, string label2, string val2)
                    {
                        var r = info.AddRow();
                        // Left cell
                        var p1 = r.Cells[0].AddParagraph();
                        p1.AddFormattedText(label1, MigraDoc.DocumentObjectModel.TextFormat.NotBold);
                        p1.AddFormattedText(val1,   MigraDoc.DocumentObjectModel.TextFormat.Bold);
                        // Right cell
                        var p2 = r.Cells[1].AddParagraph();
                        p2.AddFormattedText(label2, MigraDoc.DocumentObjectModel.TextFormat.NotBold);
                        p2.AddFormattedText(val2,   MigraDoc.DocumentObjectModel.TextFormat.Bold);
                    }

                    InfoRow("Instructor's Name: ", insName,    "Department: ",        deptName);
                    InfoRow("Occupational Title: ", streamName, "Unit of Competence: ", unitTitle);
                    InfoRow("Module Code: ",       mc,          "Semester: ",          "_______________");
                    InfoRow("Class Year: ",        "_______________", "Academic Year: ", ay);
                    InfoRow("Admission Type: ",    admType,     "Signature: ",         "_______________");

                    // Separator line
                    var sep = sec.AddParagraph();
                    sep.Format.SpaceBefore = "3mm";
                    sep.Format.SpaceAfter  = "4mm";
                    sep.Format.Borders.Bottom.Width = 0.75;
                    sep.Format.Borders.Bottom.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                    // ── Student table ─────────────────────────────────────────
                    var tbl = sec.AddTable();
                    tbl.Borders.Width = 0.5;
                    tbl.Borders.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    tbl.Format.Font.Size = 10;
                    tbl.Format.Font.Name = "Times New Roman";
                    tbl.TopPadding    = "1.5mm";
                    tbl.BottomPadding = "1.5mm";

                    tbl.AddColumn("1.0cm");
                    tbl.AddColumn("5.3cm");
                    tbl.AddColumn("1.0cm");
                    tbl.AddColumn("3.5cm");
                    tbl.AddColumn("1.9cm");
                    tbl.AddColumn("1.9cm");
                    tbl.AddColumn("3.6cm");

                    // Column header row — HeadingFormat repeats it on every page
                    var hdrRow = tbl.AddRow();
                    hdrRow.HeadingFormat = true;
                    hdrRow.Format.Font.Bold = true;
                    hdrRow.Shading.Color = new MigraDoc.DocumentObjectModel.Color(220, 220, 220);
                    hdrRow.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    string[] hdrs = { "No", "Name of Students", "Sex", "ID.NO",
                                      "Score of\nKT(100%)", "Score of\nPT(100%)", "Competency" };
                    for (int c = 0; c < hdrs.Length; c++)
                    {
                        hdrRow.Cells[c].AddParagraph(hdrs[c]);
                        hdrRow.Cells[c].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    }

                    // Data rows
                    for (int i = 0; i < rows.Count; i++)
                    {
                        var (name, sex, id, kt, pt, comp) = rows[i];
                        var row = tbl.AddRow();
                        row.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        if (i % 2 == 1)
                            row.Shading.Color = new MigraDoc.DocumentObjectModel.Color(248, 248, 248);

                        row.Cells[0].AddParagraph((i + 1).ToString());
                        row.Cells[0].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[1].AddParagraph(name);
                        row.Cells[2].AddParagraph(sex.Length > 0 ? sex[0].ToString().ToUpper() : "");
                        row.Cells[2].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[3].AddParagraph(id);
                        row.Cells[4].AddParagraph(kt);
                        row.Cells[4].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[5].AddParagraph(pt);
                        row.Cells[5].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[6].AddParagraph(comp);
                        row.Cells[6].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    }

                    // ── Footer: page number ───────────────────────────────────
                    var footer = sec.Footers.Primary;
                    var footPara = footer.AddParagraph();
                    footPara.Format.Alignment   = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    footPara.Format.Font.Size   = 8;
                    footPara.Format.Font.Name   = "Times New Roman";
                    footPara.Format.Borders.Top.Width = 0.5;
                    footPara.Format.Borders.Top.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    footPara.Format.SpaceBefore = "1mm";
                    footPara.AddText("Page ");
                    footPara.AddPageField();
                    footPara.AddText(" of ");
                    footPara.AddNumPagesField();

                    // ── Signature block ───────────────────────────────────────
                    sec.AddParagraph().Format.SpaceAfter = "10mm";

                    var sigTbl = sec.AddTable();
                    sigTbl.Borders.Width = 0;
                    sigTbl.AddColumn("6.5cm");
                    sigTbl.AddColumn("4.2cm");
                    sigTbl.AddColumn("3.5cm");

                    void SigRow(string label)
                    {
                        var row = sigTbl.AddRow();
                        row.Format.Font.Size   = 10;
                        row.Format.Font.Name   = "Times New Roman";
                        row.Format.SpaceBefore = "2mm";
                        row.Cells[0].AddParagraph(label);
                        row.Cells[1].AddParagraph("Signature___________");
                        row.Cells[2].AddParagraph("Date__________");
                    }

                    SigRow("Department Head Name________________");
                    SigRow("Dean of College Name_________________");
                    SigRow("Registrar Office Name________________");

                    var renderer = new MigraDoc.Rendering.PdfDocumentRenderer { Document = doc };
                    renderer.RenderDocument();
                    renderer.PdfDocument.Save(path);
                });

                ModernDialog.Show(Window.GetWindow(this), "PDF saved!", "Success", ModernDialog.DialogType.Success);
            }
            catch (Exception ex)
            {
                ModernDialog.Show(Window.GetWindow(this), "PDF failed: " + ex.Message, "Error", ModernDialog.DialogType.Error);
            }
        }

        private static string TryGet(DataRowView drv, string col)
        {
            try { return drv[col]?.ToString()?.Trim() ?? ""; } catch { return ""; }
        }
    }
}
