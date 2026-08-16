using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class COCRecordPage : Page
    {
        private string _selSid = "", _selLvl = "";
        private DBConnect _db = new DBConnect();
        private const string Q =
            "SELECT student_id,level,assessment_date,assessor_name,supervisor_name,competence,coc_level_id " +
            "FROM ecc_dof_wukrostmarycollege.coc";

        public COCRecordPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            Loaded += async (s, e) => await Load(Q);
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
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
            TxtStudID.Text  = _selSid;
            TxtAssDate.Text = r["assessment_date"]?.ToString() ?? "";
            TxtAssName.Text = r["assessor_name"]?.ToString() ?? "";
            TxtSupName.Text = r["supervisor_name"]?.ToString() ?? "";
            TxtCocID.Text   = r["coc_level_id"]?.ToString() ?? "";
            SetCombo(CmbLevel,       _selLvl);
            SetCombo(CmbCompetence,  r["competence"]?.ToString() ?? "");
        }

        private void SetCombo(ComboBox c, string v)
        { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }
        private string Cmb(ComboBox c) => (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        // ── SAVE (original: duplicate check student_id+level, all fields required) ─
        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            string sid = TxtStudID.Text.Trim(), lvl = Cmb(CmbLevel),
                   dt  = TxtAssDate.Text.Trim(), an  = TxtAssName.Text.Trim(),
                   sn  = TxtSupName.Text.Trim(), comp = Cmb(CmbCompetence),
                   cid = TxtCocID.Text.Trim();

            if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(lvl) ||
                string.IsNullOrWhiteSpace(dt)  || string.IsNullOrWhiteSpace(an)  ||
                string.IsNullOrWhiteSpace(sn)  || string.IsNullOrWhiteSpace(comp))
            { Msg("There is empty field(s). Please fill all fields!", false); return; }

            try
            {
                bool dup = await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.coc WHERE student_id=@s AND level=@l", c);
                    cmd.Parameters.AddWithValue("@s", sid); cmd.Parameters.AddWithValue("@l", lvl);
                    int n = Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n > 0;
                });
                if (dup) { Msg("There is already a student with the same ID!", false); return; }

                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.coc " +
                        "(student_id,level,assessment_date,assessor_name,supervisor_name,competence,coc_level_id) " +
                        "VALUES(@s,@l,@d,@a,@sn,@c,@ci)", c);
                    cmd.Parameters.AddWithValue("@s",sid);  cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.Parameters.AddWithValue("@d",dt);   cmd.Parameters.AddWithValue("@a",an);
                    cmd.Parameters.AddWithValue("@sn",sn);  cmd.Parameters.AddWithValue("@c",comp);
                    cmd.Parameters.AddWithValue("@ci",cid);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── UPDATE (original: student_id+level must match selected record) ───
        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            if (TxtStudID.Text.Trim() != _selSid || Cmb(CmbLevel) != _selLvl)
            { Msg("Update attempt failed! Student ID and Level cannot be changed.", false); return; }

            string sid=_selSid, lvl=_selLvl,
                   dt=TxtAssDate.Text.Trim(), an=TxtAssName.Text.Trim(),
                   sn=TxtSupName.Text.Trim(), comp=Cmb(CmbCompetence), cid=TxtCocID.Text.Trim();
            try
            {
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE ecc_dof_wukrostmarycollege.coc " +
                        "SET assessment_date=@d,assessor_name=@a,supervisor_name=@sn,competence=@c,coc_level_id=@ci " +
                        "WHERE student_id=@s AND level=@l", c);
                    cmd.Parameters.AddWithValue("@d",dt);  cmd.Parameters.AddWithValue("@a",an);
                    cmd.Parameters.AddWithValue("@sn",sn); cmd.Parameters.AddWithValue("@c",comp);
                    cmd.Parameters.AddWithValue("@ci",cid);cmd.Parameters.AddWithValue("@s",sid);
                    cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Update successful!", true); await Load(Q);
            }
            catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            var dlg = new ModernDialog($"Delete COC record for {_selSid} Level {_selLvl}?",
                "Confirm", ModernDialog.DialogType.Warning) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;
            string sid=_selSid, lvl=_selLvl;
            try
            {
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "DELETE FROM ecc_dof_wukrostmarycollege.coc WHERE student_id=@s AND level=@l", c);
                    cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Delete successful!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── FILTER (exact original OR logic) ─────────────────────────────────
        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string sid    = TxtFStudID.Text.Trim();
            string dept   = TxtFDept.Text.Trim();
            string stream = TxtFStream.Text.Trim();
            string level  = Cmb(CmbFLevel);
            string comp   = Cmb(CmbFCompetence);

            // Mode A: Student ID only (original: studID filled, rest empty)
            if (!string.IsNullOrEmpty(sid) && string.IsNullOrEmpty(dept) &&
                string.IsNullOrEmpty(stream))
            {
                await Load(
                    "SELECT coc.student_id,coc.level,coc.assessment_date,coc.assessor_name," +
                    "coc.supervisor_name,coc.competence,coc.coc_level_id " +
                    $"FROM ecc_dof_wukrostmarycollege.coc WHERE coc.student_id='{sid}'");
            }
            // Mode B: Dept+Stream+Level+Competence (original: studID empty, all others filled)
            else if (string.IsNullOrEmpty(sid) && !string.IsNullOrEmpty(dept) &&
                     !string.IsNullOrEmpty(stream) && !string.IsNullOrEmpty(level) &&
                     !string.IsNullOrEmpty(comp))
            {
                await Load(
                    "SELECT coc.student_id,coc.level,coc.assessment_date,coc.assessor_name," +
                    "coc.supervisor_name,coc.competence,coc.coc_level_id " +
                    "FROM ecc_dof_wukrostmarycollege.departments," +
                    "ecc_dof_wukrostmarycollege.streams," +
                    "ecc_dof_wukrostmarycollege.coc " +
                    $"WHERE departments.dept_id='{dept}' " +
                    $"AND streams.stream_id='{stream}' " +
                    $"AND coc.level='{level}' " +
                    $"AND coc.competence='{comp}'");
            }
            else
            {
                Msg("Invalid filter parameters!\nUse Student ID alone,\nor fill Dept ID + Stream ID + Level + Competence.", false);
            }
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            TxtFStudID.Text = TxtFDept.Text = TxtFStream.Text = "";
            CmbFLevel.SelectedIndex = 0; CmbFCompetence.SelectedIndex = 0;
            await Load(Q);
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
            => ModernDialog.Show(Window.GetWindow(this), "Print not implemented.", "Info", ModernDialog.DialogType.Info);

        private async void TxtFilter_Changed(object s, TextChangedEventArgs e)
        {
            string t = TxtFilter.Text.Trim();
            await Load(string.IsNullOrEmpty(t) ? Q : Q + $" WHERE student_id LIKE '%{t}%'");
        }

        private async void BtnReset_Click(object s, RoutedEventArgs e) { TxtFilter.Text = ""; await Load(Q); }
        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();

        private void Clear()
        {
            TxtStudID.Text = TxtAssDate.Text = TxtAssName.Text = TxtSupName.Text = TxtCocID.Text = "";
            _selSid = _selLvl = "";
        }

        private void Msg(string m, bool ok)
        {
            var o = Window.GetWindow(this);
            if (ok) ModernDialog.Show(o, m, "Success", ModernDialog.DialogType.Success);
            else    ModernDialog.Show(o, m, "Error",   ModernDialog.DialogType.Error);
        }
    }
}
