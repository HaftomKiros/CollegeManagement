using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class DropoutPage : Page
    {
        private string _selSid = "", _selLvl = "";
        private DBConnect _db = new DBConnect();
        private const string Q = "SELECT student_id,drop_out_date,level_number,drop_out_reason,remark FROM ecc_dof_wukrostmarycollege.drop_out_students";

        public DropoutPage() { InitializeComponent(); Loaded += async (s, e) => await Load(Q); }

        private async Task Load(string q)
        {
            try { var t = await Task.Run(() => { var dt = new DataTable(); new MySqlDataAdapter(q, _db.GetConnection()).Fill(dt); dt.Columns.Add("_RowNo",typeof(int)); for(int i=0;i<dt.Rows.Count;i++) dt.Rows[i]["_RowNo"]=i+1; return dt; }); Grid1.ItemsSource = t.DefaultView; }
            catch (Exception ex) { Msg("DB Error: " + ex.Message, false); }
        }

        private void Grid1_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (Grid1.SelectedItem is not DataRowView r) return;
            _selSid = r["student_id"]?.ToString() ?? "";
            _selLvl = r["level_number"]?.ToString() ?? "";
            TxtStudID.Text = _selSid;
            TxtDate.Text   = r["drop_out_date"]?.ToString() ?? "";
            TxtReason.Text = r["drop_out_reason"]?.ToString() ?? "";
            TxtRemark.Text = r["remark"]?.ToString() ?? "";
            SetCombo(CmbLevel, _selLvl);
        }

        private void SetCombo(ComboBox c, string v) { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }
        private string Cmb(ComboBox c) => (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtStudID.Text) || string.IsNullOrWhiteSpace(TxtDate.Text) ||
                string.IsNullOrWhiteSpace(TxtReason.Text) || string.IsNullOrWhiteSpace(TxtRemark.Text))
            { Msg("There is empty field(s). Please fill all fields!", false); return; }

            string sid = TxtStudID.Text.Trim(), lvl = Cmb(CmbLevel);
            bool dup = await Task.Run(() =>
            {
                var c = _db.GetConnection(); c.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.drop_out_students WHERE student_id=@s AND level_number=@l", c);
                cmd.Parameters.AddWithValue("@s", sid); cmd.Parameters.AddWithValue("@l", lvl);
                int n = Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n > 0;
            });
            if (dup) { Msg("Student already recorded as dropout!", false); return; }

            string dt = TxtDate.Text.Trim(), rs = TxtReason.Text.Trim(), rm = TxtRemark.Text.Trim();
            try
            {
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand("INSERT INTO ecc_dof_wukrostmarycollege.drop_out_students (student_id,drop_out_date,level_number,drop_out_reason,remark) VALUES(@s,@d,@l,@r,@m)", c);
                    cmd.Parameters.AddWithValue("@s", sid); cmd.Parameters.AddWithValue("@d", dt);
                    cmd.Parameters.AddWithValue("@l", lvl); cmd.Parameters.AddWithValue("@r", rs); cmd.Parameters.AddWithValue("@m", rm);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            string sid = _selSid, lvl = _selLvl, dt = TxtDate.Text.Trim(), rs = TxtReason.Text.Trim(), rm = TxtRemark.Text.Trim();
            try
            {
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.drop_out_students SET drop_out_date=@d,drop_out_reason=@r,remark=@m WHERE student_id=@s AND level_number=@l", c);
                    cmd.Parameters.AddWithValue("@d", dt); cmd.Parameters.AddWithValue("@r", rs); cmd.Parameters.AddWithValue("@m", rm);
                    cmd.Parameters.AddWithValue("@s", sid); cmd.Parameters.AddWithValue("@l", lvl);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Update successful!", true); await Load(Q);
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            var dlg = new ModernDialog($"Delete dropout record for {_selSid}?", "Confirm", ModernDialog.DialogType.Warning) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;
            string sid = _selSid, lvl = _selLvl;
            try
            {
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.drop_out_students WHERE student_id=@s AND level_number=@l", c);
                    cmd.Parameters.AddWithValue("@s", sid); cmd.Parameters.AddWithValue("@l", lvl);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Delete successful!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        private async void TxtFilter_Changed(object s, TextChangedEventArgs e)
        { string t = TxtFilter.Text.Trim(); await Load(string.IsNullOrEmpty(t) ? Q : Q + $" WHERE student_id LIKE '%{t}%'"); }

        private async void BtnReset_Click(object s, RoutedEventArgs e) { TxtFilter.Text = ""; await Load(Q); }
        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();

        private void Clear()
        {
            TxtStudID.Text = TxtDate.Text = TxtReason.Text = TxtRemark.Text = "";
            _selSid = _selLvl = ""; MsgBorder.Visibility = Visibility.Collapsed;
        }

        private void Msg(string m, bool ok)
        {
            var o = Window.GetWindow(this);
            if (ok) ModernDialog.Show(o, m, "Success", ModernDialog.DialogType.Success);
            else    ModernDialog.Show(o, m, "Error",   ModernDialog.DialogType.Error);
            MsgBorder.Visibility = Visibility.Collapsed;
        }
    }
}
