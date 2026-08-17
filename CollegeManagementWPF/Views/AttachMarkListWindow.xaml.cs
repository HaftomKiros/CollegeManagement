using CollegeManagementWPF.Data;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class AttachMarkListWindow : Window
    {
        private readonly DBConnect _db = new DBConnect();
        private string _selDept="", _selStream="", _selLevel="", _selMod="", _selYear="", _selAdmType="";
        private bool _loading = false; // suppress cascade events during programmatic fill

        private const string BASE =
            "SELECT doc_dept_id,doc_stream_id,doc_level_id,doc_module_code,doc_academic_year,doc_admission_type " +
            "FROM ecc_dof_wukrostmarycollege.mark_list_docs";

        public AttachMarkListWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                await LoadDepartments();
                await LoadGrid(BASE);
            };
        }

        // ── Cascade loaders ───────────────────────────────────────────────────
        private async Task LoadDepartments()
        {
            var depts = await DbList("SELECT dept_id FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id");
            FillCombo(CmbDeptID, depts);
            FillCombo(TxtSDept,  depts);
        }

        private async Task LoadStreams(string deptId, ComboBox streamTarget)
        {
            var streams = await DbList(
                $"SELECT stream_id FROM ecc_dof_wukrostmarycollege.streams WHERE dept_id='{deptId.Replace("'","''")}' ORDER BY stream_id");
            FillCombo(streamTarget, streams);
        }

        private async Task LoadLevels(string streamId, ComboBox levelTarget)
        {
            // Load level_id from levels table for this stream — these match courses.level_id
            var levels = await DbList(
                $"SELECT level_id FROM ecc_dof_wukrostmarycollege.levels WHERE stream_id='{streamId.Replace("'","''")}' ORDER BY level_id");
            FillCombo(levelTarget, levels);
        }

        private async Task LoadModules(string deptId, string streamId, string levelId, ComboBox modTarget)
        {
            // Load module codes from courses for this level_id
            var mods = await DbList(
                $"SELECT module_code FROM ecc_dof_wukrostmarycollege.courses WHERE level_id='{levelId.Replace("'","''")}' ORDER BY module_code");
            FillCombo(modTarget, mods);
        }

        private async Task<List<string>> DbList(string sql)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var list = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(sql, conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close(); return list;
                });
            }
            catch { return new List<string>(); }
        }

        private static void FillCombo(ComboBox c, List<string> items)
        {
            c.Items.Clear();
            foreach (var v in items) c.Items.Add(new ComboBoxItem { Content = v });
        }

        // ── Cascade event handlers (form) ────────────────────────────────────
        private async void CmbDeptID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            string dept = GetCmb(CmbDeptID);
            if (string.IsNullOrEmpty(dept)) return;
            CmbStreamID.Items.Clear(); CmbLevelID.Items.Clear(); CmbModCode.Items.Clear();
            await LoadStreams(dept, CmbStreamID);
        }

        private async void CmbStreamID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            string stream = GetCmb(CmbStreamID);
            if (string.IsNullOrEmpty(stream)) return;
            CmbLevelID.Items.Clear(); CmbModCode.Items.Clear();
            await LoadLevels(stream, CmbLevelID);
        }

        private async void CmbLevelID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            string dept = GetCmb(CmbDeptID), stream = GetCmb(CmbStreamID), level = GetCmb(CmbLevelID);
            if (string.IsNullOrEmpty(level)) return;
            CmbModCode.Items.Clear();
            await LoadModules(dept, stream, level, CmbModCode);
        }

        // ── Cascade event handler (search panel) ─────────────────────────────
        private async void CmbSDept_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            string dept = GetCmb(TxtSDept);
            if (string.IsNullOrEmpty(dept)) return;
            TxtSStream.Items.Clear(); TxtSLevel.Items.Clear(); TxtSModule.Items.Clear();
            await LoadStreams(dept, TxtSStream);
        }

        // ── Helper: get combo text (typed or selected) ────────────────────────
        private static string GetCmb(ComboBox c)
        {
            string? text = c.Text?.Trim();
            if (!string.IsNullOrEmpty(text)) return text;
            return (c.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
        }

        // ── Grid load ────────────────────────────────────────────────────────
        private async Task LoadGrid(string q)
        {
            try
            {
                var t = await Task.Run(() =>
                {
                    var dt = new DataTable();
                    new MySqlDataAdapter(q, _db.GetConnection()).Fill(dt);
                    return dt;
                });
                GridDocs.ItemsSource = t.DefaultView;
            }
            catch (Exception ex) { ModernDialog.Show(this, "DB Error: " + ex.Message, "Error", ModernDialog.DialogType.Error); }
        }

        // ── Grid selection: fill form fields ─────────────────────────────────
        private async void GridDocs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridDocs.SelectedItem is not DataRowView r) return;
            _selDept    = r["doc_dept_id"]?.ToString()       ?? "";
            _selStream  = r["doc_stream_id"]?.ToString()     ?? "";
            _selLevel   = r["doc_level_id"]?.ToString()      ?? "";
            _selMod     = r["doc_module_code"]?.ToString()   ?? "";
            _selYear    = r["doc_academic_year"]?.ToString() ?? "";
            _selAdmType = r["doc_admission_type"]?.ToString()?? "";

            _loading = true;
            await LoadStreams(_selDept, CmbStreamID);
            await LoadLevels(_selStream, CmbLevelID);
            await LoadModules(_selDept, _selStream, _selLevel, CmbModCode);

            CmbDeptID.Text  = _selDept;
            CmbStreamID.Text= _selStream;
            CmbLevelID.Text = _selLevel;
            CmbModCode.Text = _selMod;
            TxtAcadYear.Text= _selYear;
            SetCombo(CmbAdmType, _selAdmType);
            _loading = false;

            // Check if a file is stored and show indicator
            TxtFilePath.Text = "[Checking file...]";
            try
            {
                bool hasFile = await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT LENGTH(doc_file) FROM ecc_dof_wukrostmarycollege.mark_list_docs " +
                        "WHERE doc_dept_id=@d AND doc_stream_id=@s AND doc_level_id=@l " +
                        "AND doc_module_code=@m AND doc_academic_year=@y AND doc_admission_type=@at", conn);
                    cmd.Parameters.AddWithValue("@d",_selDept);  cmd.Parameters.AddWithValue("@s",_selStream);
                    cmd.Parameters.AddWithValue("@l",_selLevel); cmd.Parameters.AddWithValue("@m",_selMod);
                    cmd.Parameters.AddWithValue("@y",_selYear);  cmd.Parameters.AddWithValue("@at",_selAdmType);
                    var result = cmd.ExecuteScalar();
                    conn.Close();
                    return result != null && result != DBNull.Value && Convert.ToInt64(result) > 0;
                });
                TxtFilePath.Text = hasFile
                    ? $"[File attached — {_selMod}_{_selYear}.pdf — click Browse to replace]"
                    : "[No file stored]";
            }
            catch { TxtFilePath.Text = "[Unable to check file]"; }
        }

        private void SetCombo(ComboBox c, string v)
        { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }
        private string CmbVal(ComboBox c) => (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? c.Text?.Trim() ?? "";

        // ── BROWSE ───────────────────────────────────────────────────────────
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            { Filter = "PDF Files(*.pdf)|*.pdf|Word Files(*.docx)|*.docx|All Files(*.*)|*.*" };
            if (dlg.ShowDialog() == true) TxtFilePath.Text = dlg.FileName;
        }

        // ── DOWNLOAD ─────────────────────────────────────────────────────────
        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selDept)) { ModernDialog.Show(this, "Select a record first.", "Info", ModernDialog.DialogType.Info); return; }
            var dlg = new SaveFileDialog
            { FileName = $"marklist_{_selDept}_{_selLevel}_{_selYear}", Filter = "PDF Files|*.pdf|Word Files|*.docx|All Files|*.*" };
            if (dlg.ShowDialog() != true) return;
            try
            {
                byte[]? data = await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT doc_file FROM ecc_dof_wukrostmarycollege.mark_list_docs " +
                        "WHERE doc_dept_id=@d AND doc_stream_id=@s AND doc_level_id=@l " +
                        "AND doc_module_code=@m AND doc_academic_year=@y AND doc_admission_type=@at", conn);
                    cmd.Parameters.AddWithValue("@d",_selDept);  cmd.Parameters.AddWithValue("@s",_selStream);
                    cmd.Parameters.AddWithValue("@l",_selLevel); cmd.Parameters.AddWithValue("@m",_selMod);
                    cmd.Parameters.AddWithValue("@y",_selYear);  cmd.Parameters.AddWithValue("@at",_selAdmType);
                    var bytes = cmd.ExecuteScalar() as byte[];
                    conn.Close(); return bytes;
                });
                if (data == null || data.Length == 0) { ModernDialog.Show(this, "No file found for this record.", "Info", ModernDialog.DialogType.Info); return; }
                await File.WriteAllBytesAsync(dlg.FileName, data);
                ModernDialog.Show(this, "Downloaded successfully!", "Success", ModernDialog.DialogType.Success);
            }
            catch (Exception ex) { ModernDialog.Show(this, "Connection failed! " + ex.Message, "Error", ModernDialog.DialogType.Error); }
        }

        // ── SAVE ─────────────────────────────────────────────────────────────
        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string dept   = GetCmb(CmbDeptID),  stream = GetCmb(CmbStreamID),
                   level  = GetCmb(CmbLevelID), mod    = GetCmb(CmbModCode),
                   year   = TxtAcadYear.Text.Trim(), adm = CmbVal(CmbAdmType), path = TxtFilePath.Text;

            if (string.IsNullOrEmpty(dept) || string.IsNullOrEmpty(stream) || string.IsNullOrEmpty(level) ||
                string.IsNullOrEmpty(mod)  || string.IsNullOrEmpty(year)   || string.IsNullOrEmpty(path))
            { ModernDialog.Show(this, "Error. Please fill in all fields!", "Error", ModernDialog.DialogType.Error); return; }

            try
            {
                bool dup = await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.mark_list_docs " +
                        "WHERE doc_dept_id=@d AND doc_stream_id=@s AND doc_level_id=@l " +
                        "AND doc_module_code=@m AND doc_academic_year=@y AND doc_admission_type=@at", conn);
                    cmd.Parameters.AddWithValue("@d",dept); cmd.Parameters.AddWithValue("@s",stream);
                    cmd.Parameters.AddWithValue("@l",level); cmd.Parameters.AddWithValue("@m",mod);
                    cmd.Parameters.AddWithValue("@y",year); cmd.Parameters.AddWithValue("@at",adm);
                    int n = Convert.ToInt32(cmd.ExecuteScalar()); conn.Close(); return n > 0;
                });
                if (dup) { ModernDialog.Show(this, "Error. This mark list is already attached!", "Error", ModernDialog.DialogType.Error); return; }

                byte[] fileBytes = await File.ReadAllBytesAsync(path);
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.mark_list_docs " +
                        "(doc_dept_id,doc_stream_id,doc_level_id,doc_module_code,doc_academic_year,doc_admission_type,doc_file) " +
                        "VALUES(@d,@s,@l,@m,@y,@at,@f)", conn);
                    cmd.Parameters.AddWithValue("@d",dept); cmd.Parameters.AddWithValue("@s",stream);
                    cmd.Parameters.AddWithValue("@l",level); cmd.Parameters.AddWithValue("@m",mod);
                    cmd.Parameters.AddWithValue("@y",year); cmd.Parameters.AddWithValue("@at",adm);
                    cmd.Parameters.AddWithValue("@f",fileBytes);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                ModernDialog.Show(this, "Saved successfully!", "Success", ModernDialog.DialogType.Success);
                await LoadGrid(BASE);
            }
            catch (Exception ex) { ModernDialog.Show(this, "Connection failed! " + ex.Message, "Error", ModernDialog.DialogType.Error); }
        }

        // ── UPDATE ───────────────────────────────────────────────────────────
        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selDept)) { ModernDialog.Show(this, "Select a record first.", "Info", ModernDialog.DialogType.Info); return; }
            if (string.IsNullOrEmpty(TxtFilePath.Text)) { ModernDialog.Show(this, "Error. Wrong update attempt!", "Error", ModernDialog.DialogType.Error); return; }

            if (GetCmb(CmbDeptID) != _selDept || GetCmb(CmbStreamID) != _selStream ||
                GetCmb(CmbLevelID) != _selLevel || GetCmb(CmbModCode) != _selMod   ||
                TxtAcadYear.Text.Trim() != _selYear || CmbVal(CmbAdmType) != _selAdmType)
            { ModernDialog.Show(this, "Error. Update attempt failed!", "Warning", ModernDialog.DialogType.Warning); return; }

            try
            {
                byte[] fileBytes = await File.ReadAllBytesAsync(TxtFilePath.Text);
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "UPDATE ecc_dof_wukrostmarycollege.mark_list_docs SET doc_file=@f " +
                        "WHERE doc_dept_id=@d AND doc_stream_id=@s AND doc_level_id=@l " +
                        "AND doc_module_code=@m AND doc_academic_year=@y AND doc_admission_type=@at", conn);
                    cmd.Parameters.AddWithValue("@f",fileBytes);
                    cmd.Parameters.AddWithValue("@d",_selDept); cmd.Parameters.AddWithValue("@s",_selStream);
                    cmd.Parameters.AddWithValue("@l",_selLevel); cmd.Parameters.AddWithValue("@m",_selMod);
                    cmd.Parameters.AddWithValue("@y",_selYear); cmd.Parameters.AddWithValue("@at",_selAdmType);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                ModernDialog.Show(this, "Update successful!", "Success", ModernDialog.DialogType.Success);
                await LoadGrid(BASE);
            }
            catch (Exception ex) { ModernDialog.Show(this, "Connection failed! " + ex.Message, "Error", ModernDialog.DialogType.Error); }
        }

        // ── DELETE ───────────────────────────────────────────────────────────
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selDept)) { ModernDialog.Show(this, "Select a record first.", "Info", ModernDialog.DialogType.Info); return; }
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "DELETE FROM ecc_dof_wukrostmarycollege.mark_list_docs " +
                        "WHERE doc_dept_id=@d AND doc_stream_id=@s AND doc_level_id=@l " +
                        "AND doc_module_code=@m AND doc_academic_year=@y AND doc_admission_type=@at", conn);
                    cmd.Parameters.AddWithValue("@d",_selDept); cmd.Parameters.AddWithValue("@s",_selStream);
                    cmd.Parameters.AddWithValue("@l",_selLevel); cmd.Parameters.AddWithValue("@m",_selMod);
                    cmd.Parameters.AddWithValue("@y",_selYear); cmd.Parameters.AddWithValue("@at",_selAdmType);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                ModernDialog.Show(this, "Delete successful!", "Success", ModernDialog.DialogType.Success);
                _selDept = _selStream = _selLevel = _selMod = _selYear = _selAdmType = "";
                await LoadGrid(BASE);
            }
            catch (Exception ex) { ModernDialog.Show(this, "Connection failed! " + ex.Message, "Error", ModernDialog.DialogType.Error); }
        }

        // ── FILTER ───────────────────────────────────────────────────────────
        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string d = GetCmb(TxtSDept), s = GetCmb(TxtSStream), l = GetCmb(TxtSLevel),
                   m = GetCmb(TxtSModule), y = TxtSYear.Text.Trim(), at = CmbVal(CmbSAdmType);

            var conditions = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(d))  conditions.Add($"doc_dept_id='{d.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(s))  conditions.Add($"doc_stream_id='{s.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(l))  conditions.Add($"doc_level_id='{l.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(m))  conditions.Add($"doc_module_code='{m.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(y))  conditions.Add($"doc_academic_year='{y.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(at)) conditions.Add($"doc_admission_type='{at.Replace("'","''")}'");

            string q = BASE + (conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "");
            await LoadGrid(q);
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            TxtSDept.Text = TxtSStream.Text = TxtSLevel.Text =
            TxtSModule.Text = TxtSYear.Text = "";
            CmbSAdmType.SelectedIndex = 0;
            await LoadGrid(BASE);
        }
    }
}
