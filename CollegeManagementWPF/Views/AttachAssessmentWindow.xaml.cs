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
    public partial class AttachAssessmentWindow : Window
    {
        private readonly DBConnect _db = new DBConnect();
        private int    _selId = -1;
        private string _selDept="", _selStream="", _selLevel="", _selMod="", _selYear="", _selAdmType="";
        private string _existingFilePath = "";
        private bool   _loading = false;

        private const string TABLE = "ecc_dof_wukrostmarycollege.assessment_docs";
        private const string BASE  =
            "SELECT id,doc_dept_id,doc_stream_id,doc_level_id,doc_module_code,doc_academic_year,doc_admission_type " +
            "FROM ecc_dof_wukrostmarycollege.assessment_docs";

        public AttachAssessmentWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                await EnsureTableAsync();
                await LoadDepartments();
                await LoadGrid(BASE);
            };
        }

        private async Task EnsureTableAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    new MySqlCommand(
                        "CREATE TABLE IF NOT EXISTS ecc_dof_wukrostmarycollege.assessment_docs (" +
                        "  id INT AUTO_INCREMENT PRIMARY KEY," +
                        "  doc_dept_id VARCHAR(50)," +
                        "  doc_stream_id VARCHAR(50)," +
                        "  doc_level_id VARCHAR(50)," +
                        "  doc_module_code VARCHAR(100)," +
                        "  doc_academic_year VARCHAR(20)," +
                        "  doc_admission_type VARCHAR(30)," +
                        "  doc_file_path VARCHAR(500)" +
                        ") ENGINE=InnoDB", conn).ExecuteNonQuery();
                    // Drop student_id column if it exists from old version
                    try { new MySqlCommand("ALTER TABLE ecc_dof_wukrostmarycollege.assessment_docs DROP COLUMN student_id", conn).ExecuteNonQuery(); } catch { }
                    conn.Close();
                });
            }
            catch { }
        }

        private async Task LoadDepartments()
        {
            var list = await DbList("SELECT dept_id FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id");
            FillCombo(CmbDeptID, list);
            FillCombo(TxtSDept, list);
        }

        private async Task LoadStreams(string deptId, ComboBox target)
        {
            var list = await DbList(
                "SELECT stream_id FROM ecc_dof_wukrostmarycollege.streams WHERE dept_id=@p ORDER BY stream_id",
                deptId);
            FillCombo(target, list);
        }

        private async Task LoadLevels(string streamId, ComboBox target)
        {
            var list = await DbList(
                "SELECT level_id FROM ecc_dof_wukrostmarycollege.levels WHERE stream_id=@p ORDER BY level_id",
                streamId);
            FillCombo(target, list);
        }

        private async Task LoadModules(string levelId, ComboBox target)
        {
            var list = await DbList(
                "SELECT module_code FROM ecc_dof_wukrostmarycollege.courses WHERE level_id=@p ORDER BY module_code",
                levelId);
            FillCombo(target, list);
        }

        private async Task<List<string>> DbList(string sql, string? param = null)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var list = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(sql, conn);
                    if (param != null) cmd.Parameters.AddWithValue("@p", param);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close();
                    return list;
                });
            }
            catch { return new List<string>(); }
        }

        private static void FillCombo(ComboBox c, List<string> items)
        { c.Items.Clear(); foreach (var v in items) c.Items.Add(new ComboBoxItem { Content = v }); }

        private static string GetCmb(ComboBox c)
        {
            string? t = c.Text?.Trim();
            if (!string.IsNullOrEmpty(t)) return t;
            return (c.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
        }

        private string CmbVal(ComboBox c) =>
            (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? c.Text?.Trim() ?? "";

        // ── Cascade: form ─────────────────────────────────────────────────────
        private async void CmbDeptID_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            string d = GetCmb(CmbDeptID);
            if (string.IsNullOrEmpty(d)) return;
            CmbStreamID.Items.Clear(); CmbLevelID.Items.Clear(); CmbModCode.Items.Clear();
            await LoadStreams(d, CmbStreamID);
        }

        private async void CmbStreamID_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            string st = GetCmb(CmbStreamID);
            if (string.IsNullOrEmpty(st)) return;
            CmbLevelID.Items.Clear(); CmbModCode.Items.Clear();
            await LoadLevels(st, CmbLevelID);
        }

        private async void CmbLevelID_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            string lv = GetCmb(CmbLevelID);
            if (string.IsNullOrEmpty(lv)) return;
            CmbModCode.Items.Clear();
            await LoadModules(lv, CmbModCode);
        }

        // ── Cascade: search ───────────────────────────────────────────────────
        private async void CmbSDept_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            string d = GetCmb(TxtSDept);
            if (string.IsNullOrEmpty(d)) return;
            TxtSStream.Items.Clear(); TxtSLevel.Items.Clear(); TxtSModule.Items.Clear();
            await LoadStreams(d, TxtSStream);
        }

        private async void CmbSStream_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            string st = GetCmb(TxtSStream);
            if (string.IsNullOrEmpty(st)) return;
            TxtSLevel.Items.Clear(); TxtSModule.Items.Clear();
            await LoadLevels(st, TxtSLevel);
        }

        private async void CmbSLevel_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (_loading) return;
            string lv = GetCmb(TxtSLevel);
            if (string.IsNullOrEmpty(lv)) return;
            TxtSModule.Items.Clear();
            await LoadModules(lv, TxtSModule);
        }

        // ── Grid ──────────────────────────────────────────────────────────────
        private async Task LoadGrid(string q)
        {
            try
            {
                var t = await Task.Run(() => { var dt = new DataTable(); new MySqlDataAdapter(q, _db.GetConnection()).Fill(dt); return dt; });
                GridDocs.ItemsSource = t.DefaultView;
            }
            catch (Exception ex) { ModernDialog.Show(this, "DB Error: " + ex.Message, "Error", ModernDialog.DialogType.Error); }
        }

        private async void GridDocs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridDocs.SelectedItem is not DataRowView r) return;
            _selId      = r["id"] != DBNull.Value ? Convert.ToInt32(r["id"]) : -1;
            _selDept    = r["doc_dept_id"]?.ToString()       ?? "";
            _selStream  = r["doc_stream_id"]?.ToString()     ?? "";
            _selLevel   = r["doc_level_id"]?.ToString()      ?? "";
            _selMod     = r["doc_module_code"]?.ToString()   ?? "";
            _selYear    = r["doc_academic_year"]?.ToString() ?? "";
            _selAdmType = r["doc_admission_type"]?.ToString()?? "";

            _loading = true;
            await LoadStreams(_selDept, CmbStreamID);
            await LoadLevels(_selStream, CmbLevelID);
            await LoadModules(_selLevel, CmbModCode);
            CmbDeptID.Text   = _selDept;
            CmbStreamID.Text = _selStream;
            CmbLevelID.Text  = _selLevel;
            CmbModCode.Text  = _selMod;
            TxtAcadYear.Text = _selYear;
            SetCombo(CmbAdmType, _selAdmType);
            _loading = false;

            _existingFilePath = "";
            TxtFilePath.Text = "[Checking...]";
            try
            {
                string fp = await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand("SELECT doc_file_path FROM " + TABLE + " WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _selId);
                    string? v = cmd.ExecuteScalar()?.ToString()?.Trim();
                    conn.Close();
                    return v ?? "";
                });
                _existingFilePath = fp;
                TxtFilePath.Text = string.IsNullOrEmpty(fp) ? "[No file stored]" : Path.GetFileName(fp);
            }
            catch { TxtFilePath.Text = "[No file stored]"; }
        }

        private void SetCombo(ComboBox c, string v)
        { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "PDF Files(*.pdf)|*.pdf|Word Files(*.docx)|*.docx|All Files(*.*)|*.*" };
            if (dlg.ShowDialog() == true) TxtFilePath.Text = dlg.FileName;
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (_selId < 0) { ModernDialog.Show(this, "Select a record first.", "Info", ModernDialog.DialogType.Info); return; }
            if (string.IsNullOrEmpty(_existingFilePath) || !File.Exists(_existingFilePath))
            { ModernDialog.Show(this, "No file found for this record.", "Info", ModernDialog.DialogType.Info); return; }
            var dlg = new SaveFileDialog { FileName = Path.GetFileName(_existingFilePath), Filter = "All Files|*.*" };
            if (dlg.ShowDialog() != true) return;
            try { File.Copy(_existingFilePath, dlg.FileName, true); ModernDialog.Show(this, "Downloaded!", "Success", ModernDialog.DialogType.Success); }
            catch (Exception ex) { ModernDialog.Show(this, ex.Message, "Error", ModernDialog.DialogType.Error); }
            await Task.CompletedTask;
        }

        private string SafeName(string s)
            => s.Replace("/","_").Replace("\\","_").Replace(":","_").Replace(" ","_").Replace("*","_").Replace("?","_");

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string dept = GetCmb(CmbDeptID), stream = GetCmb(CmbStreamID),
                   lv   = GetCmb(CmbLevelID), mod = GetCmb(CmbModCode),
                   year = TxtAcadYear.Text.Trim(), adm = CmbVal(CmbAdmType),
                   path = TxtFilePath.Text.Trim();

            if (string.IsNullOrEmpty(dept) || string.IsNullOrEmpty(mod) || string.IsNullOrEmpty(year) || string.IsNullOrEmpty(path))
            { ModernDialog.Show(this, "Please fill all fields and browse a file.", "Error", ModernDialog.DialogType.Error); return; }
            if (!File.Exists(path)) { ModernDialog.Show(this, "File not found. Please browse again.", "Error", ModernDialog.DialogType.Error); return; }

            try
            {
                string mlDir = Path.Combine(AppSettings.Current.MarkListsPath, "assessments");
                Directory.CreateDirectory(mlDir);
                string fname = SafeName(dept) + "_" + SafeName(lv) + "_" + SafeName(mod) + "_" + SafeName(year) + "_" + SafeName(adm) + Path.GetExtension(path);
                string dest  = Path.Combine(mlDir, fname);
                byte[] bytes = await File.ReadAllBytesAsync(path);
                await File.WriteAllBytesAsync(dest, bytes);

                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO " + TABLE + " (doc_dept_id,doc_stream_id,doc_level_id,doc_module_code,doc_academic_year,doc_admission_type,doc_file_path) " +
                        "VALUES(@d,@s,@l,@m,@y,@at,@fp)", conn);
                    cmd.Parameters.AddWithValue("@d",  dept);
                    cmd.Parameters.AddWithValue("@s",  stream);
                    cmd.Parameters.AddWithValue("@l",  lv);
                    cmd.Parameters.AddWithValue("@m",  mod);
                    cmd.Parameters.AddWithValue("@y",  year);
                    cmd.Parameters.AddWithValue("@at", adm);
                    cmd.Parameters.AddWithValue("@fp", dest);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                ModernDialog.Show(this, "Saved successfully!", "Success", ModernDialog.DialogType.Success);
                await LoadGrid(BASE);
            }
            catch (Exception ex) { ModernDialog.Show(this, ex.Message, "Error", ModernDialog.DialogType.Error); }
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selId < 0) { ModernDialog.Show(this, "Select a record first.", "Info", ModernDialog.DialogType.Info); return; }
            string path = TxtFilePath.Text.Trim();
            bool isNewFile = !string.IsNullOrEmpty(path) && File.Exists(path) && path != _existingFilePath;
            string destPath = _existingFilePath;

            if (isNewFile)
            {
                try
                {
                    string mlDir = Path.Combine(AppSettings.Current.MarkListsPath, "assessments");
                    Directory.CreateDirectory(mlDir);
                    string fname = SafeName(_selDept) + "_" + SafeName(_selLevel) + "_" + SafeName(_selMod) + "_" + SafeName(_selYear) + "_" + SafeName(_selAdmType) + Path.GetExtension(path);
                    destPath = Path.Combine(mlDir, fname);
                    byte[] bytes = await File.ReadAllBytesAsync(path);
                    await File.WriteAllBytesAsync(destPath, bytes);
                }
                catch (Exception ex) { ModernDialog.Show(this, ex.Message, "Error", ModernDialog.DialogType.Error); return; }
            }

            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand("UPDATE " + TABLE + " SET doc_file_path=@fp WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@fp", destPath);
                    cmd.Parameters.AddWithValue("@id", _selId);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                _existingFilePath = destPath;
                TxtFilePath.Text = Path.GetFileName(destPath);
                ModernDialog.Show(this, "Updated!", "Success", ModernDialog.DialogType.Success);
                await LoadGrid(BASE);
            }
            catch (Exception ex) { ModernDialog.Show(this, ex.Message, "Error", ModernDialog.DialogType.Error); }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selId < 0) { ModernDialog.Show(this, "Select a record first.", "Info", ModernDialog.DialogType.Info); return; }
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM " + TABLE + " WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _selId);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                ModernDialog.Show(this, "Deleted!", "Success", ModernDialog.DialogType.Success);
                _selId = -1; _selDept = _selStream = _selLevel = _selMod = _selYear = _selAdmType = "";
                await LoadGrid(BASE);
            }
            catch (Exception ex) { ModernDialog.Show(this, ex.Message, "Error", ModernDialog.DialogType.Error); }
        }

        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            var c = new List<string>();
            string d  = GetCmb(TxtSDept), st = GetCmb(TxtSStream), lv = GetCmb(TxtSLevel),
                   m  = GetCmb(TxtSModule), y = TxtSYear.Text.Trim(), at = CmbVal(CmbSAdmType);
            if (!string.IsNullOrEmpty(d))  c.Add("doc_dept_id='"   + d.Replace("'","''")  + "'");
            if (!string.IsNullOrEmpty(st)) c.Add("doc_stream_id='"  + st.Replace("'","''") + "'");
            if (!string.IsNullOrEmpty(lv)) c.Add("doc_level_id='"   + lv.Replace("'","''") + "'");
            if (!string.IsNullOrEmpty(m))  c.Add("doc_module_code='" + m.Replace("'","''") + "'");
            if (!string.IsNullOrEmpty(y))  c.Add("doc_academic_year='" + y.Replace("'","''") + "'");
            if (!string.IsNullOrEmpty(at)) c.Add("doc_admission_type='" + at.Replace("'","''") + "'");
            await LoadGrid(BASE + (c.Count > 0 ? " WHERE " + string.Join(" AND ", c) : ""));
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            _loading = true;
            _selId = -1; _selDept = _selStream = _selLevel = _selMod = _selYear = _selAdmType = ""; _existingFilePath = "";
            CmbDeptID.Text = ""; CmbStreamID.Items.Clear(); CmbStreamID.Text = "";
            CmbLevelID.Items.Clear(); CmbLevelID.Text = ""; CmbModCode.Items.Clear(); CmbModCode.Text = "";
            TxtAcadYear.Text = ""; TxtFilePath.Text = ""; CmbAdmType.SelectedIndex = 0;
            _loading = false; GridDocs.SelectedItem = null;
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            TxtSDept.Text = TxtSStream.Text = TxtSLevel.Text = TxtSModule.Text = TxtSYear.Text = "";
            CmbSAdmType.SelectedIndex = 0;
            await LoadGrid(BASE);
        }
    }
}
