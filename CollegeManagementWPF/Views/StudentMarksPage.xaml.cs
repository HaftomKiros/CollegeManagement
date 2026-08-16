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
        private const string Q = "SELECT student_id,level,module_code,employee_id,academic_year," +
            "score_of_knowledge_test,score_of_practical_test,competence " +
            "FROM ecc_dof_wukrostmarycollege.student_mark";

        public StudentMarksPage()
        {
            InitializeComponent();
            TxtKnow.PreviewTextInput += NumOnly;
            TxtPrac.PreviewTextInput += NumOnly;
            Loaded += async (s, e) => await Load(Q);
        }

        private void NumOnly(object s, System.Windows.Input.TextCompositionEventArgs e)
            => e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"\d");

        // Auto-calculate Competence
        private void ScoreChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(TxtKnow.Text, out int k) && int.TryParse(TxtPrac.Text, out int p))
                TxtCompetence.Text = (k >= 51 && k <= 100 && p >= 90 && p <= 100) ? "Competent" : "Not Competent";
            else
                TxtCompetence.Text = "";
        }

        private async Task Load(string q)
        {
            try
            {
                var t = await Task.Run(() => { var dt = new DataTable(); new MySqlDataAdapter(q, _db.GetConnection()).Fill(dt); dt.Columns.Add("_RowNo",typeof(int)); for(int i=0;i<dt.Rows.Count;i++) dt.Rows[i]["_RowNo"]=i+1; return dt; });
                Grid1.ItemsSource = t.DefaultView;
            }
            catch (Exception ex) { Msg("DB Error: " + ex.Message, false); }
        }

        private void Grid1_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (Grid1.SelectedItem is not DataRowView r) return;
            _selSid = r["student_id"]?.ToString() ?? "";
            _selLvl = r["level"]?.ToString() ?? "";
            _selMod = r["module_code"]?.ToString() ?? "";
            TxtStudID.Text   = _selSid;
            TxtModCode.Text  = _selMod;
            TxtEmpID.Text    = r["employee_id"]?.ToString() ?? "";
            TxtAcadYear.Text = r["academic_year"]?.ToString() ?? "";
            TxtKnow.Text     = r["score_of_knowledge_test"]?.ToString() ?? "";
            TxtPrac.Text     = r["score_of_practical_test"]?.ToString() ?? "";
            TxtCompetence.Text = r["competence"]?.ToString() ?? "";
            SetCombo(CmbLevel, r["level"]?.ToString() ?? "1");
        }

        private void SetCombo(ComboBox c, string v)
        { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }
        private string Cmb(ComboBox c) => (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtStudID.Text) || string.IsNullOrWhiteSpace(TxtModCode.Text) ||
                string.IsNullOrWhiteSpace(TxtEmpID.Text) || string.IsNullOrWhiteSpace(TxtAcadYear.Text) ||
                string.IsNullOrWhiteSpace(TxtKnow.Text) || string.IsNullOrWhiteSpace(TxtPrac.Text))
            { Msg("There is empty field(s). Please fill all fields!", false); return; }

            try
            {
                string sid = TxtStudID.Text.Trim(), lvl = Cmb(CmbLevel),
                       ay = TxtAcadYear.Text.Trim();

                bool dup = await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_mark WHERE student_id=@s AND level=@l AND academic_year=@y", c);
                    cmd.Parameters.AddWithValue("@s", sid); cmd.Parameters.AddWithValue("@l", lvl); cmd.Parameters.AddWithValue("@y", ay);
                    int n = Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n > 0;
                });
                if (dup) { Msg("There is already a mark record with the same ID!", false); return; }

                string mod = TxtModCode.Text.Trim(), emp = TxtEmpID.Text.Trim(),
                       kn = TxtKnow.Text.Trim(), pr = TxtPrac.Text.Trim(), comp = TxtCompetence.Text;
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand("INSERT INTO ecc_dof_wukrostmarycollege.student_mark (student_id,level,module_code,employee_id,academic_year,score_of_knowledge_test,score_of_practical_test,competence) VALUES(@s,@l,@m,@e,@y,@k,@p,@c)", c);
                    cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl); cmd.Parameters.AddWithValue("@m",mod);
                    cmd.Parameters.AddWithValue("@e",emp); cmd.Parameters.AddWithValue("@y",ay);
                    cmd.Parameters.AddWithValue("@k",kn); cmd.Parameters.AddWithValue("@p",pr); cmd.Parameters.AddWithValue("@c",comp);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            try
            {
                string emp = TxtEmpID.Text.Trim(), ay = TxtAcadYear.Text.Trim(),
                       kn = TxtKnow.Text.Trim(), pr = TxtPrac.Text.Trim(), comp = TxtCompetence.Text;
                string sid = _selSid, lvl = _selLvl, mod = _selMod;
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.student_mark SET employee_id=@e,academic_year=@y,score_of_knowledge_test=@k,score_of_practical_test=@p,competence=@c WHERE student_id=@s AND level=@l AND module_code=@m", c);
                    cmd.Parameters.AddWithValue("@e",emp); cmd.Parameters.AddWithValue("@y",ay);
                    cmd.Parameters.AddWithValue("@k",kn); cmd.Parameters.AddWithValue("@p",pr); cmd.Parameters.AddWithValue("@c",comp);
                    cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl); cmd.Parameters.AddWithValue("@m",mod);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Update successful!", true); await Load(Q);
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            var dlg = new ModernDialog($"Delete mark for {_selSid} Level {_selLvl}?", "Confirm", ModernDialog.DialogType.Warning) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;
            string sid = _selSid, lvl = _selLvl, mod = _selMod;
            try
            {
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.student_mark WHERE student_id=@s AND level=@l AND module_code=@m", c);
                    cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl); cmd.Parameters.AddWithValue("@m",mod);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Delete successful!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        private async void TxtFilter_Changed(object s, TextChangedEventArgs e)
        {
            string t = TxtFilter.Text.Trim();
            await Load(string.IsNullOrEmpty(t) ? Q : Q + $" WHERE student_id LIKE '%{t}%'");
        }

        private async void BtnReset_Click(object s, RoutedEventArgs e) { TxtFilter.Text = ""; await Load(Q); }
        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();

        private void Clear()
        {
            TxtStudID.Text = TxtModCode.Text = TxtEmpID.Text = TxtAcadYear.Text = TxtKnow.Text = TxtPrac.Text = TxtCompetence.Text = "";
            _selSid = _selLvl = _selMod = ""; MsgBorder.Visibility = Visibility.Collapsed;
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
