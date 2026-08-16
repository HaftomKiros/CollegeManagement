using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class DepartmentsPage : Page
    {
        private string _selKey = "";
        private DBConnect _db = new DBConnect();
        private const string Q = "SELECT dept_id,dept_name,dept_program,dept_head FROM ecc_dof_wukrostmarycollege.departments";

        public DepartmentsPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); Loaded += async (s,e) => await Load(Q); }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1) g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2) g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private async Task Load(string q) {
            try {
                if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
                var t = await Task.Run(() => { var dt=new DataTable(); new MySqlDataAdapter(q,_db.GetConnection()).Fill(dt); dt.Columns.Add("_RowNo",typeof(int)); for(int i=0;i<dt.Rows.Count;i++) dt.Rows[i]["_RowNo"]=i+1; return dt; });
                Grid1.ItemsSource = t.DefaultView;
            } catch(Exception ex) { Msg("DB Error: "+ex.Message,false); }
            finally { if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed; }
        }

        private void Grid1_SelectionChanged(object s, SelectionChangedEventArgs e) {
            if (Grid1.SelectedItem is not DataRowView r) return;
            _selKey = r["dept_id"]?.ToString() ?? "";
            TxtDeptID.Text = r["dept_id"]?.ToString() ?? "";
            TxtDeptName.Text = r["dept_name"]?.ToString() ?? "";
            TxtProgram.Text = r["dept_program"]?.ToString() ?? "";
            TxtHead.Text = r["dept_head"]?.ToString() ?? "";
        }

        private async void BtnSave_Click(object s, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TxtDeptID.Text) ||
                string.IsNullOrWhiteSpace(TxtDeptName.Text) ||
                string.IsNullOrWhiteSpace(TxtProgram.Text) ||
                string.IsNullOrWhiteSpace(TxtHead.Text))
            { Msg("There is empty field(s). Please fill all fields!",false); return; }
            try {
                bool dup = await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.departments WHERE dept_id=@k",c); cmd.Parameters.AddWithValue("@k",TxtDeptID.Text.Trim()); int n=Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n>0; });
                if(dup){Msg("There is already a record with the same ID!",false);return;}
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("INSERT INTO ecc_dof_wukrostmarycollege.departments (dept_id,dept_name,dept_program,dept_head) VALUES(@d,@n,@p,@h)",c);
                    cmd.Parameters.AddWithValue("@d",TxtDeptID.Text.Trim());
                    cmd.Parameters.AddWithValue("@n",TxtDeptName.Text.Trim());
                    cmd.Parameters.AddWithValue("@p",TxtProgram.Text.Trim());
                    cmd.Parameters.AddWithValue("@h",TxtHead.Text.Trim());
                    cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Saved successfully!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            if(TxtDeptID.Text.Trim()!=_selKey){Msg("Update attempt failed!",false);return;}
            try {
                string key=_selKey;
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.departments SET dept_name=@n,dept_program=@p,dept_head=@h WHERE dept_id=@key",c);
                    cmd.Parameters.AddWithValue("@n",TxtDeptName.Text.Trim());
                    cmd.Parameters.AddWithValue("@p",TxtProgram.Text.Trim());
                    cmd.Parameters.AddWithValue("@h",TxtHead.Text.Trim());
                    cmd.Parameters.AddWithValue("@key",key); cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Update successful!",true); await Load(Q);
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            var dlg=new ModernDialog($"Delete record '{_selKey}'?","Confirm",ModernDialog.DialogType.Warning){Owner=Window.GetWindow(this)};
            if(dlg.ShowDialog()!=true)return;
            string key=_selKey;
            try {
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.departments WHERE dept_id=@k",c); cmd.Parameters.AddWithValue("@k",key); cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Delete successful!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void TxtFilter_Changed(object s, TextChangedEventArgs e) { string t=TxtFilter.Text.Trim(); await Load(string.IsNullOrEmpty(t)?Q:Q+$" WHERE dept_id LIKE '%{t}%'"); }
        private async void BtnReset_Click(object s, RoutedEventArgs e){TxtFilter.Text="";await Load(Q);}
        private void BtnClear_Click(object s, RoutedEventArgs e)=>Clear();
        private void Clear(){TxtDeptID.Text = TxtDeptName.Text = TxtProgram.Text = TxtHead.Text="";_selKey="";}
        private void Msg(string m,bool ok){var o=Window.GetWindow(this);if(ok)ModernDialog.Show(o,m,"Success",ModernDialog.DialogType.Success);else ModernDialog.Show(o,m,"Error",ModernDialog.DialogType.Error);}
    }
}