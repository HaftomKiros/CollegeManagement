using CollegeManagementWPF.Data;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
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

        private const string BASE =
            "SELECT doc_dept_id,doc_stream_id,doc_level_id,doc_module_code,doc_academic_year,doc_admission_type " +
            "FROM ecc_dof_wukrostmarycollege.mark_list_docs";

        public AttachMarkListWindow()
        {
            InitializeComponent();
            _ = LoadGrid(BASE);
        }

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
            catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
        }

        private void GridDocs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridDocs.SelectedItem is not DataRowView r) return;
            _selDept    = r["doc_dept_id"]?.ToString()       ?? "";
            _selStream  = r["doc_stream_id"]?.ToString()     ?? "";
            _selLevel   = r["doc_level_id"]?.ToString()      ?? "";
            _selMod     = r["doc_module_code"]?.ToString()   ?? "";
            _selYear    = r["doc_academic_year"]?.ToString() ?? "";
            _selAdmType = r["doc_admission_type"]?.ToString()?? "";

            TxtDeptID.Text   = _selDept;
            TxtStreamID.Text = _selStream;
            TxtLevelID.Text  = _selLevel;
            TxtModCode.Text  = _selMod;
            TxtAcadYear.Text = _selYear;
            SetCombo(CmbAdmType, _selAdmType);
            TxtFilePath.Text = "";
        }

        private void SetCombo(ComboBox c, string v)
        { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }
        private string CmbVal(ComboBox c) => (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        // ── BROWSE ────────────────────────────────────────────────────────────
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            { Filter = "PDF Files(*.pdf)|*.pdf|Word Files(*.docx)|*.docx|All Files(*.*)|*.*" };
            if (dlg.ShowDialog() == true) TxtFilePath.Text = dlg.FileName;
        }

        // ── DOWNLOAD ─────────────────────────────────────────────────────────
        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selDept)) { MessageBox.Show("Select a record first."); return; }
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
                    cmd.Parameters.AddWithValue("@d", _selDept);   cmd.Parameters.AddWithValue("@s", _selStream);
                    cmd.Parameters.AddWithValue("@l", _selLevel);  cmd.Parameters.AddWithValue("@m", _selMod);
                    cmd.Parameters.AddWithValue("@y", _selYear);   cmd.Parameters.AddWithValue("@at", _selAdmType);
                    var bytes = cmd.ExecuteScalar() as byte[];
                    conn.Close(); return bytes;
                });
                if (data == null || data.Length == 0) { MessageBox.Show("No file found for this record."); return; }
                await File.WriteAllBytesAsync(dlg.FileName, data);
                MessageBox.Show("Downloaded successfully!");
            }
            catch (Exception ex) { MessageBox.Show("Connection failed! " + ex.Message); }
        }

        // ── SAVE (exact original algorithm) ───────────────────────────────────
        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string dept = TxtDeptID.Text.Trim(), stream = TxtStreamID.Text.Trim(),
                   level = TxtLevelID.Text.Trim(), mod = TxtModCode.Text.Trim(),
                   year = TxtAcadYear.Text.Trim(), adm = CmbVal(CmbAdmType), path = TxtFilePath.Text;

            if (string.IsNullOrEmpty(dept) || string.IsNullOrEmpty(stream) || string.IsNullOrEmpty(level) ||
                string.IsNullOrEmpty(mod)  || string.IsNullOrEmpty(year)  || string.IsNullOrEmpty(path))
            { MessageBox.Show("Error. Please fill in all fields!"); return; }

            try
            {
                // Duplicate check (original algorithm)
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
                if (dup) { MessageBox.Show("Error. This mark list is already attached!"); return; }

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
                MessageBox.Show("Saved successfully!");
                await LoadGrid(BASE);
            }
            catch (Exception ex) { MessageBox.Show("Connection failed! " + ex.Message); }
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selDept)) { MessageBox.Show("Select a record first."); return; }
            if (string.IsNullOrEmpty(TxtFilePath.Text)) { MessageBox.Show("Error. Wrong update attempt!"); return; }

            // Must match selected record (original validation)
            if (TxtDeptID.Text.Trim() != _selDept   || TxtStreamID.Text.Trim() != _selStream ||
                TxtLevelID.Text.Trim() != _selLevel  || TxtModCode.Text.Trim()  != _selMod   ||
                TxtAcadYear.Text.Trim() != _selYear  || CmbVal(CmbAdmType)      != _selAdmType)
            { MessageBox.Show("Error. Update attempt failed!"); return; }

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
                MessageBox.Show("Update successful!");
                await LoadGrid(BASE);
            }
            catch (Exception ex) { MessageBox.Show("Connection failed! " + ex.Message); }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selDept)) { MessageBox.Show("Select a record first."); return; }
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
                MessageBox.Show("Delete successful!");
                _selDept = _selStream = _selLevel = _selMod = _selYear = _selAdmType = "";
                await LoadGrid(BASE);
            }
            catch (Exception ex) { MessageBox.Show("Connection failed! " + ex.Message); }
        }

        // ── FILTER (original: all 6 fields required) ──────────────────────────
        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string d=TxtSDept.Text.Trim(), s=TxtSStream.Text.Trim(), l=TxtSLevel.Text.Trim(),
                   m=TxtSModule.Text.Trim(), y=TxtSYear.Text.Trim(), at=CmbVal(CmbSAdmType);

            if (string.IsNullOrEmpty(d) || string.IsNullOrEmpty(s) || string.IsNullOrEmpty(l) ||
                string.IsNullOrEmpty(m) || string.IsNullOrEmpty(y))
            { MessageBox.Show("Error. Wrong filter parameters!"); return; }

            await LoadGrid(
                "SELECT doc_dept_id,doc_stream_id,doc_level_id,doc_module_code,doc_academic_year,doc_admission_type " +
                "FROM ecc_dof_wukrostmarycollege.mark_list_docs " +
                $"WHERE doc_dept_id=@d AND doc_stream_id=@s AND doc_level_id=@l " +
                $"AND doc_module_code=@m AND doc_academic_year=@y AND doc_admission_type=@at"
                    .Replace("@d",$"'{d}'").Replace("@s",$"'{s}'").Replace("@l",$"'{l}'")
                    .Replace("@m",$"'{m}'").Replace("@y",$"'{y}'").Replace("@at",$"'{at}'"));
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
