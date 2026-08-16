using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class StudentFeesPage : Page
    {
        private string _selSid = "", _selLvl = "", _selAy = "", _selMo = "";
        private DBConnect _db = new DBConnect();
        private const string Q =
            "SELECT student_id,level,academic_year,month,amount,cash_receipt_voucher,remark " +
            "FROM ecc_dof_wukrostmarycollege.student_fee";

        public StudentFeesPage()
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
                g1.Color = dark
                    ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E)
                    : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark
                    ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E)
                    : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private async Task Load(string q)
        {
            try
            {
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
        }

        private void Grid1_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (Grid1.SelectedItem is not DataRowView r) return;
            _selSid = r["student_id"]?.ToString() ?? "";
            _selLvl = r["level"]?.ToString() ?? "";
            _selAy  = r["academic_year"]?.ToString() ?? "";
            _selMo  = r["month"]?.ToString() ?? "";
            TxtStudID.Text   = _selSid;
            TxtAcadYear.Text = _selAy;
            SetCombo(CmbMonth, _selMo);
            TxtAmount.Text   = r["amount"]?.ToString() ?? "";
            TxtCRV.Text      = r["cash_receipt_voucher"]?.ToString() ?? "";
            TxtRemark.Text   = r["remark"]?.ToString() ?? "";
            SetCombo(CmbLevel, _selLvl);
        }

        private void SetCombo(ComboBox c, string v)
        { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }
        private string Cmb(ComboBox c) => (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        // ── SAVE ─────────────────────────────────────────────────────────────
        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            string sid = TxtStudID.Text.Trim(), lvl = Cmb(CmbLevel),
                   ay  = TxtAcadYear.Text.Trim(), mo = Cmb(CmbMonth),
                   amt = TxtAmount.Text.Trim(), crv = TxtCRV.Text.Trim(),
                   rem = TxtRemark.Text.Trim();

            if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(lvl) ||
                string.IsNullOrWhiteSpace(ay)  || string.IsNullOrWhiteSpace(mo)  ||
                string.IsNullOrWhiteSpace(amt))
            { Msg("There is empty field(s). Please fill all fields!", false); return; }

            try
            {
                bool dup = await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_fee " +
                        "WHERE student_id=@s AND level=@l AND academic_year=@y AND month=@m", c);
                    cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.Parameters.AddWithValue("@y",ay);  cmd.Parameters.AddWithValue("@m",mo);
                    int n = Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n > 0;
                });
                if (dup) { Msg("There is already a fee record for this student/level/year/month!", false); return; }

                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.student_fee " +
                        "(student_id,level,academic_year,month,amount,cash_receipt_voucher,remark) " +
                        "VALUES(@s,@l,@y,@m,@a,@c,@r)", c);
                    cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.Parameters.AddWithValue("@y",ay);  cmd.Parameters.AddWithValue("@m",mo);
                    cmd.Parameters.AddWithValue("@a",amt); cmd.Parameters.AddWithValue("@c",crv);
                    cmd.Parameters.AddWithValue("@r",rem);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            string amt = TxtAmount.Text.Trim(), crv = TxtCRV.Text.Trim(), rem = TxtRemark.Text.Trim();
            try
            {
                string sid=_selSid, lvl=_selLvl, ay=_selAy, mo=_selMo;
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE ecc_dof_wukrostmarycollege.student_fee " +
                        "SET amount=@a,cash_receipt_voucher=@c,remark=@r " +
                        "WHERE student_id=@s AND level=@l AND academic_year=@y AND month=@m", c);
                    cmd.Parameters.AddWithValue("@a",amt); cmd.Parameters.AddWithValue("@c",crv);
                    cmd.Parameters.AddWithValue("@r",rem); cmd.Parameters.AddWithValue("@s",sid);
                    cmd.Parameters.AddWithValue("@l",lvl); cmd.Parameters.AddWithValue("@y",ay);
                    cmd.Parameters.AddWithValue("@m",mo);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Update successful!", true); await Load(Q);
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            var dlg = new ModernDialog(
                $"Delete fee for {_selSid} Level {_selLvl}?", "Confirm",
                ModernDialog.DialogType.Warning) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;
            string sid=_selSid, lvl=_selLvl, ay=_selAy, mo=_selMo;
            try
            {
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "DELETE FROM ecc_dof_wukrostmarycollege.student_fee " +
                        "WHERE student_id=@s AND level=@l AND academic_year=@y AND month=@m", c);
                    cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.Parameters.AddWithValue("@y",ay);  cmd.Parameters.AddWithValue("@m",mo);
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
            TxtStudID.Text = TxtAcadYear.Text =
            TxtAmount.Text = TxtCRV.Text = TxtRemark.Text = "";
            _selSid = _selLvl = _selAy = _selMo = "";
            MsgBorder.Visibility = Visibility.Collapsed;
        }

        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string sid = TxtFStudID.Text.Trim();
            string lvl = (CmbFLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string mo  = (CmbFMonth.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string ay  = TxtFYear.Text.Trim();

            if (!string.IsNullOrEmpty(sid))
                await Load(Q + $" WHERE student_id='{sid}'");
            else if (!string.IsNullOrEmpty(lvl) && !string.IsNullOrEmpty(ay))
                await Load(Q + $" WHERE level='{lvl}' AND academic_year='{ay}'" +
                    (string.IsNullOrEmpty(mo) ? "" : $" AND month='{mo}'"));
            else
                Msg("Enter Student ID, or Level + Year to filter.", false);
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            TxtFStudID.Text = TxtFYear.Text = "";
            CmbFLevel.SelectedIndex = 0;
            CmbFMonth.SelectedIndex = 0;
            await Load(Q);
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this);
            ModernDialog.Show(owner, "Print not implemented.", "Info", ModernDialog.DialogType.Info);
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
