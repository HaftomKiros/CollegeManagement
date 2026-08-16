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
        private const string Q =
            "SELECT student_id,drop_out_date,level_number,drop_out_reason,remark " +
            "FROM ecc_dof_wukrostmarycollege.drop_out_students";

        public DropoutPage()
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
                var t = await Task.Run(() => {
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
            _selLvl = r["level_number"]?.ToString() ?? "";
            TxtStudID.Text = _selSid;
            TxtDate.Text   = r["drop_out_date"]?.ToString() ?? "";
            TxtRemark.Text = r["remark"]?.ToString() ?? "";
            SetCombo(CmbLevel,  _selLvl);
            SetCombo(CmbReason, r["drop_out_reason"]?.ToString() ?? "");
        }

        private void SetCombo(ComboBox c, string v) { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }
        private string Cmb(ComboBox c) => (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            string sid=TxtStudID.Text.Trim(), lvl=Cmb(CmbLevel), dt=TxtDate.Text.Trim(), rs=Cmb(CmbReason), rm=TxtRemark.Text.Trim();
            if (string.IsNullOrWhiteSpace(sid)||string.IsNullOrWhiteSpace(lvl)||string.IsNullOrWhiteSpace(dt)||string.IsNullOrWhiteSpace(rs)||string.IsNullOrWhiteSpace(rm))
            { Msg("There is empty field(s). Please fill all fields!", false); return; }
            try
            {
                bool dup = await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.drop_out_students WHERE student_id=@s AND level_number=@l",c); cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl); int n=Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n>0; });
                if (dup) { Msg("There is already a student with the same ID!", false); return; }
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("INSERT INTO ecc_dof_wukrostmarycollege.drop_out_students (student_id,drop_out_date,level_number,drop_out_reason,remark) VALUES(@s,@d,@l,@r,@m)",c); cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@d",dt); cmd.Parameters.AddWithValue("@l",lvl); cmd.Parameters.AddWithValue("@r",rs); cmd.Parameters.AddWithValue("@m",rm); cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Saved successfully!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Connection failed! "+ex.Message, false); }
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            if (TxtStudID.Text.Trim()!=_selSid || Cmb(CmbLevel)!=_selLvl) { Msg("Update attempt failed! Student ID and Level cannot be changed.", false); return; }
            string sid=_selSid, lvl=_selLvl, dt=TxtDate.Text.Trim(), rs=Cmb(CmbReason), rm=TxtRemark.Text.Trim();
            try
            {
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.drop_out_students SET drop_out_date=@d,drop_out_reason=@r,remark=@m WHERE student_id=@s AND level_number=@l",c); cmd.Parameters.AddWithValue("@d",dt); cmd.Parameters.AddWithValue("@r",rs); cmd.Parameters.AddWithValue("@m",rm); cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl); cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Update successful!", true); await Load(Q);
            }
            catch (Exception ex) { Msg("Connection failed! "+ex.Message, false); }
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            var dlg=new ModernDialog($"Delete dropout record for {_selSid} Level {_selLvl}?","Confirm",ModernDialog.DialogType.Warning){Owner=Window.GetWindow(this)};
            if (dlg.ShowDialog()!=true) return;
            string sid=_selSid, lvl=_selLvl;
            try
            {
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.drop_out_students WHERE student_id=@s AND level_number=@l",c); cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl); cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Delete successful!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Connection failed! "+ex.Message, false); }
        }

        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string dept=TxtFDept.Text.Trim(), stream=TxtFStream.Text.Trim(), year=TxtFYear.Text.Trim(), level=Cmb(CmbFLevel);
            if (string.IsNullOrEmpty(dept)||string.IsNullOrEmpty(stream)||string.IsNullOrEmpty(year)||string.IsNullOrEmpty(level))
            { Msg("Please fill all filter fields: Dept ID, Stream ID, Dropout Year and Level.", false); return; }
            await Load("SELECT d.student_id,d.drop_out_date,d.level_number,d.drop_out_reason,d.remark FROM ecc_dof_wukrostmarycollege.departments dp,ecc_dof_wukrostmarycollege.streams st,ecc_dof_wukrostmarycollege.drop_out_students d WHERE dp.dept_id='"+dept+"' AND st.stream_id='"+stream+"' AND d.drop_out_date='"+year+"' AND d.level_number='"+level+"'");
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e) { TxtFDept.Text=TxtFStream.Text=TxtFYear.Text=""; CmbFLevel.SelectedIndex=0; await Load(Q); }
        private void BtnPrint_Click(object sender, RoutedEventArgs e) { ModernDialog.Show(Window.GetWindow(this),"Print not implemented.","Info",ModernDialog.DialogType.Info); }
        private async void TxtFilter_Changed(object s, TextChangedEventArgs e) { string t=TxtFilter.Text.Trim(); await Load(string.IsNullOrEmpty(t)?Q:Q+$" WHERE student_id LIKE '%{t}%'"); }
        private async void BtnReset_Click(object s, RoutedEventArgs e) { TxtFilter.Text=""; await Load(Q); }
        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();
        private void Clear() { TxtStudID.Text=TxtDate.Text=TxtRemark.Text=""; _selSid=_selLvl=""; }
        private void Msg(string m, bool ok) { var o=Window.GetWindow(this); if(ok) ModernDialog.Show(o,m,"Success",ModernDialog.DialogType.Success); else ModernDialog.Show(o,m,"Error",ModernDialog.DialogType.Error); }
    }
}