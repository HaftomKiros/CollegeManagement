using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class StudentMarksPage : Page
    {
        private string _selSid = "", _selLvl = "", _selMod = "";
        private DBConnect _db = new DBConnect();
        private const string BASE =
            "SELECT student_id,level,module_code,employee_id,academic_year," +
            "score_of_knowledge_test,score_of_practical_test,competence " +
            "FROM ecc_dof_wukrostmarycollege.student_mark";

        public StudentMarksPage()
        {
            InitializeComponent();
            TxtKnow.PreviewTextInput += NumOnly;
            TxtPrac.PreviewTextInput += NumOnly;
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            // Open dropdown on click for module/dept combos
            CmbModCode.GotFocus  += (s,e) => ((ComboBox)s).IsDropDownOpen = true;
            CmbFDept.GotFocus    += (s,e) => ((ComboBox)s).IsDropDownOpen = true;
            CmbFModule.GotFocus  += (s,e) => ((ComboBox)s).IsDropDownOpen = true;
            Loaded += async (s, e) =>
            {
                await LoadModulesAsync();
                await Load(BASE);
            };
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark
                    ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E)
                    : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark
                    ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E)
                    : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        // Load module codes from DB into CmbModCode
        private async Task LoadModulesAsync()
        {
            try
            {
                var modules = await Task.Run(() =>
                {
                    var list = new System.Collections.Generic.List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT DISTINCT module_code FROM ecc_dof_wukrostmarycollege.courses ORDER BY module_code", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close();
                    return list;
                });

                CmbModCode.Items.Clear();
                foreach (var m in modules)
                    CmbModCode.Items.Add(new ComboBoxItem { Content = m });
                if (CmbModCode.Items.Count > 0) CmbModCode.SelectedIndex = 0;

                // Filter module combo — starts empty (free-text allowed)
                CmbFModule.Items.Clear();
                foreach (var m in modules)
                    CmbFModule.Items.Add(new ComboBoxItem { Content = m });
                CmbFModule.Text = ""; // free-text, no default selection

                // Load departments into CmbFDept dropdown
                var depts = await Task.Run(() =>
                {
                    var list = new System.Collections.Generic.List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT dept_id FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close();
                    return list;
                });

                CmbFDept.Items.Clear();
                foreach (var d in depts)
                    CmbFDept.Items.Add(new ComboBoxItem { Content = d });
                CmbFDept.Text = ""; // free-text default
            }
            catch { /* DB offline — skip */ }
        }

        private void NumOnly(object s, System.Windows.Input.TextCompositionEventArgs e)
            => e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"\d");

        private void ScoreChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(TxtKnow.Text, out int k) && int.TryParse(TxtPrac.Text, out int p))
                TxtCompetence.Text = (k >= 51 && k <= 100 && p >= 90 && p <= 100)
                    ? "Competent" : "Not Competent";
            else
                TxtCompetence.Text = "";
        }

        private async Task Load(string q)
        {
            try
            {
                if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
                var t = await Task.Run(() =>
                {
                    var dt = new DataTable();
                    new MySqlDataAdapter(q, _db.GetConnection()).Fill(dt);
                    dt.Columns.Add("_RowNo", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++) dt.Rows[i]["_RowNo"] = i + 1;
                    return dt;
                });
                Grid1.ItemsSource = t.DefaultView;
            }
            catch (Exception ex) { Msg("DB Error: " + ex.Message, false); }
            finally { if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed; }
        }

        private void Grid1_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (Grid1.SelectedItem is not DataRowView r) return;
            _selSid = r["student_id"]?.ToString() ?? "";
            _selLvl = r["level"]?.ToString() ?? "";
            _selMod = r["module_code"]?.ToString() ?? "";
            TxtStudID.Text     = _selSid;
            TxtEmpID.Text      = r["employee_id"]?.ToString() ?? "";
            TxtAcadYear.Text   = r["academic_year"]?.ToString() ?? "";
            TxtKnow.Text       = r["score_of_knowledge_test"]?.ToString() ?? "";
            TxtPrac.Text       = r["score_of_practical_test"]?.ToString() ?? "";
            TxtCompetence.Text = r["competence"]?.ToString() ?? "";
            SetCombo(CmbLevel,   r["level"]?.ToString() ?? "1");
            SetComboByContent(CmbModCode, _selMod);
        }

        private void SetCombo(ComboBox c, string v)
        { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }

        private void SetComboByContent(ComboBox c, string v)
        {
            foreach (ComboBoxItem i in c.Items)
                if (i.Content?.ToString() == v) { c.SelectedItem = i; return; }
            // If not found, add it dynamically
            var item = new ComboBoxItem { Content = v };
            c.Items.Add(item);
            c.SelectedItem = item;
        }

        private string CmbVal(ComboBox c)
        {
            // Always prefer typed text (covers both free-text and selected-from-list)
            var text = c.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(text) && text != "(All)") return text;
            return (c.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
        }

        // ── SAVE (exact original algorithm) ─────────────────────────────────
        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            string sid = TxtStudID.Text.Trim(), lvl = CmbVal(CmbLevel),
                   mod = CmbVal(CmbModCode), emp = TxtEmpID.Text.Trim(),
                   ay  = TxtAcadYear.Text.Trim(),
                   kn  = TxtKnow.Text.Trim(), pr = TxtPrac.Text.Trim();

            // Validation: all fields required
            if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(lvl) ||
                string.IsNullOrWhiteSpace(mod) || string.IsNullOrWhiteSpace(emp) ||
                string.IsNullOrWhiteSpace(ay)  || string.IsNullOrWhiteSpace(kn)  ||
                string.IsNullOrWhiteSpace(pr))
            { Msg("Error. Please fill in all fields!", false); return; }

            if (!int.TryParse(kn, out int kVal) || !int.TryParse(pr, out int pVal))
            { Msg("Knowledge and Practical scores must be numbers!", false); return; }

            if (kVal < 0 || kVal > 100 || pVal < 0 || pVal > 100)
            { Msg("Scores must be between 0 and 100!", false); return; }

            try
            {
                // Duplicate check: student_id + level + academic_year (original logic)
                bool dup = await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_mark " +
                        "WHERE student_id=@s AND level=@l AND academic_year=@y", c);
                    cmd.Parameters.AddWithValue("@s", sid);
                    cmd.Parameters.AddWithValue("@l", lvl);
                    cmd.Parameters.AddWithValue("@y", ay);
                    int n = Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n > 0;
                });
                if (dup) { Msg("Error. This mark list is already attached!", false); return; }

                // Competence logic (exact original)
                string comp = (kVal >= 51 && kVal <= 100 && pVal >= 90 && pVal <= 100)
                    ? "Competent" : "Not Competent";

                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.student_mark " +
                        "(student_id,level,module_code,employee_id,academic_year," +
                        "score_of_knowledge_test,score_of_practical_test,competence) " +
                        "VALUES(@s,@l,@m,@e,@y,@k,@p,@c)", c);
                    cmd.Parameters.AddWithValue("@s", sid); cmd.Parameters.AddWithValue("@l", lvl);
                    cmd.Parameters.AddWithValue("@m", mod); cmd.Parameters.AddWithValue("@e", emp);
                    cmd.Parameters.AddWithValue("@y", ay);  cmd.Parameters.AddWithValue("@k", kn);
                    cmd.Parameters.AddWithValue("@p", pr);  cmd.Parameters.AddWithValue("@c", comp);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!", true);
                await Load(BASE); Clear();
            }
            catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            string emp = TxtEmpID.Text.Trim(), ay = TxtAcadYear.Text.Trim(),
                   kn  = TxtKnow.Text.Trim(),  pr = TxtPrac.Text.Trim(), comp = TxtCompetence.Text;
            try
            {
                string sid = _selSid, lvl = _selLvl, mod = _selMod;
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE ecc_dof_wukrostmarycollege.student_mark " +
                        "SET employee_id=@e,academic_year=@y," +
                        "score_of_knowledge_test=@k,score_of_practical_test=@p,competence=@c " +
                        "WHERE student_id=@s AND level=@l AND module_code=@m", c);
                    cmd.Parameters.AddWithValue("@e",emp); cmd.Parameters.AddWithValue("@y",ay);
                    cmd.Parameters.AddWithValue("@k",kn);  cmd.Parameters.AddWithValue("@p",pr);
                    cmd.Parameters.AddWithValue("@c",comp);
                    cmd.Parameters.AddWithValue("@s",sid);  cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.Parameters.AddWithValue("@m",mod);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Update successful!", true); await Load(BASE);
            }
            catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            var dlg = new ModernDialog(
                $"Delete mark for {_selSid} Level {_selLvl}?", "Confirm",
                ModernDialog.DialogType.Warning) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;
            string sid = _selSid, lvl = _selLvl, mod = _selMod;
            try
            {
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "DELETE FROM ecc_dof_wukrostmarycollege.student_mark " +
                        "WHERE student_id=@s AND level=@l AND module_code=@m", c);
                    cmd.Parameters.AddWithValue("@s",sid);
                    cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.Parameters.AddWithValue("@m",mod);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Delete successful!", true); await Load(BASE); Clear();
            }
            catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── FILTER (exact original OR logic) ─────────────────────────────────
        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string studID = TxtFStudID.Text.Trim();
            string deptID = (CmbFDept.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            string year   = TxtFYear.Text.Trim();
            string level  = CmbVal(CmbFLevel);
            // Module: IsEditable combo — use Text property
            string modCod = CmbFModule.Text?.Trim() ?? "";
            if (modCod == "(All)" || modCod == "") modCod = "";

            if (!string.IsNullOrEmpty(studID))
            {
                // Filter by Student ID only (original algorithm)
                await Load(BASE + $" WHERE student_id='{studID}'");
            }
            else if (!string.IsNullOrEmpty(deptID) && !string.IsNullOrEmpty(year) &&
                     !string.IsNullOrEmpty(level)  && !string.IsNullOrEmpty(modCod))
            {
                // Filter by Dept + Year + Level + Module (original algorithm)
                // Original joins departments table to validate dept exists, filters student_mark
                await Load(
                    "SELECT student_id,level,module_code,employee_id,academic_year," +
                    "score_of_knowledge_test,score_of_practical_test,competence " +
                    "FROM ecc_dof_wukrostmarycollege.student_mark " +
                    $"WHERE academic_year='{year}' " +
                    $"AND level='{level}' " +
                    $"AND module_code='{modCod}' " +
                    $"AND student_id IN (" +
                    $"SELECT student_id FROM ecc_dof_wukrostmarycollege.student_profile " +
                    $"WHERE dept_id='{deptID}')");
            }
            else
            {
                Msg("Invalid filter parameters!\nEnter Student ID alone,\nor fill Dept ID + Year + Level + Module Code.", false);
            }
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            TxtFStudID.Text = TxtFYear.Text = "";
            CmbFDept.Text   = "";
            CmbFModule.Text = "";
            CmbFLevel.SelectedIndex   = 0;
            CmbFAdmType.SelectedIndex = 0;
            await Load(BASE);
        }

        // ── ATTACH MARK LIST ─────────────────────────────────────────────────
        private void BtnAttachMarkList_Click(object sender, RoutedEventArgs e)
        {
            var win = new AttachMarkListWindow { Owner = Window.GetWindow(this) };
            win.ShowDialog();
        }

        // ── QUICK FILTER (text box) ───────────────────────────────────────────
        private async void TxtFilter_Changed(object s, TextChangedEventArgs e)
        {
            string t = TxtFilter.Text.Trim();
            await Load(string.IsNullOrEmpty(t) ? BASE : BASE + $" WHERE student_id LIKE '%{t}%'");
        }

        private async void BtnReset_Click(object s, RoutedEventArgs e)
        { TxtFilter.Text = ""; await Load(BASE); }

        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this);
            ModernDialog.Show(owner,
                "Print Mark List requires the Crystal Reports integration from the original application.\nPlease use the MarkList report from the Reports menu.",
                "Print Mark List", ModernDialog.DialogType.Info);
        }

        private void Clear()
        {
            TxtStudID.Text = TxtEmpID.Text = TxtAcadYear.Text =
            TxtKnow.Text   = TxtPrac.Text  = TxtCompetence.Text = "";
            _selSid = _selLvl = _selMod = "";
            MsgBorder.Visibility = Visibility.Collapsed;
        }

        private void Msg(string m, bool ok)
        {
            var owner = Window.GetWindow(this);
            if (ok) ModernDialog.Show(owner, m, "Success", ModernDialog.DialogType.Success);
            else    ModernDialog.Show(owner, m, "Error",   ModernDialog.DialogType.Error);
            MsgBorder.Visibility = Visibility.Collapsed;
        }
    }
}
