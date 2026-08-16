using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class AlumniPage : Page
    {
        private string _selKey = "";
        private DBConnect _db = new DBConnect();
        private const string Q = "SELECT alumni_id,student_id,graduated_year,employment_status,employed_office,home_address,mobile_number,current_educational_status FROM ecc_dof_wukrostmarycollege.alumni";

        public AlumniPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); Loaded += async (s,e) => await Load(Q); }

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
            _selKey = r["alumni_id"]?.ToString() ?? "";
            TxtAlumniID.Text = _selKey;
            TxtStudID.Text   = r["student_id"]?.ToString() ?? "";
            TxtGradYear.Text = r["graduated_year"]?.ToString() ?? "";
            TxtEmpStatus.Text= r["employment_status"]?.ToString() ?? "";
            TxtEmpOffice.Text= r["employed_office"]?.ToString() ?? "";
            TxtHomeAddr.Text = r["home_address"]?.ToString() ?? "";
            TxtMobile.Text   = r["mobile_number"]?.ToString() ?? "";
            TxtEduStatus.Text= r["current_educational_status"]?.ToString() ?? "";
        }

        private async void BtnSave_Click(object s, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TxtAlumniID.Text)||string.IsNullOrWhiteSpace(TxtStudID.Text)||string.IsNullOrWhiteSpace(TxtGradYear.Text)||string.IsNullOrWhiteSpace(TxtEmpStatus.Text)||string.IsNullOrWhiteSpace(TxtEmpOffice.Text)||string.IsNullOrWhiteSpace(TxtMobile.Text)||string.IsNullOrWhiteSpace(TxtHomeAddr.Text)||string.IsNullOrWhiteSpace(TxtEduStatus.Text))
            { Msg("There is empty field(s). Please fill all fields!",false); return; }
            try {
                bool dup = await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.alumni WHERE alumni_id=@k",c); cmd.Parameters.AddWithValue("@k",TxtAlumniID.Text.Trim()); int n=Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n>0; });
                if(dup){Msg("There is already a department with the same ID!",false);return;}
                await Task.Run(() => {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand("INSERT INTO ecc_dof_wukrostmarycollege.alumni (alumni_id,student_id,graduated_year,employment_status,employed_office,home_address,mobile_number,current_educational_status) VALUES(@a,@s,@g,@es,@eo,@ha,@m,@ed)",c);
                    cmd.Parameters.AddWithValue("@a",TxtAlumniID.Text.Trim()); cmd.Parameters.AddWithValue("@s",TxtStudID.Text.Trim());
                    cmd.Parameters.AddWithValue("@g",TxtGradYear.Text.Trim()); cmd.Parameters.AddWithValue("@es",TxtEmpStatus.Text.Trim());
                    cmd.Parameters.AddWithValue("@eo",TxtEmpOffice.Text.Trim()); cmd.Parameters.AddWithValue("@ha",TxtHomeAddr.Text.Trim());
                    cmd.Parameters.AddWithValue("@m",TxtMobile.Text.Trim()); cmd.Parameters.AddWithValue("@ed",TxtEduStatus.Text.Trim());
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            if(TxtAlumniID.Text.Trim()!=_selKey){Msg("Update attempt failed!",false);return;}
            try {
                string key=_selKey;
                await Task.Run(() => {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.alumni SET student_id=@s,graduated_year=@g,employment_status=@es,employed_office=@eo,home_address=@ha,mobile_number=@m,current_educational_status=@ed WHERE alumni_id=@k",c);
                    cmd.Parameters.AddWithValue("@s",TxtStudID.Text.Trim()); cmd.Parameters.AddWithValue("@g",TxtGradYear.Text.Trim());
                    cmd.Parameters.AddWithValue("@es",TxtEmpStatus.Text.Trim()); cmd.Parameters.AddWithValue("@eo",TxtEmpOffice.Text.Trim());
                    cmd.Parameters.AddWithValue("@ha",TxtHomeAddr.Text.Trim()); cmd.Parameters.AddWithValue("@m",TxtMobile.Text.Trim());
                    cmd.Parameters.AddWithValue("@ed",TxtEduStatus.Text.Trim()); cmd.Parameters.AddWithValue("@k",key);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Update successful!",true); await Load(Q);
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            var dlg=new ModernDialog($"Delete alumni '{_selKey}'?","Confirm",ModernDialog.DialogType.Warning){Owner=Window.GetWindow(this)};
            if(dlg.ShowDialog()!=true)return;
            string key=_selKey;
            try {
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.alumni WHERE alumni_id=@k",c); cmd.Parameters.AddWithValue("@k",key); cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Delete successful!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnFilter_Click(object sender, RoutedEventArgs e) {
            string aid=TxtFAlumniID.Text.Trim(), dept=TxtFDept.Text.Trim(), stream=TxtFStream.Text.Trim(), gy=TxtFGradYear.Text.Trim(), es=TxtFEmpStatus.Text.Trim();
            if(!string.IsNullOrEmpty(aid)&&string.IsNullOrEmpty(dept)&&string.IsNullOrEmpty(stream)&&string.IsNullOrEmpty(gy))
                await Load(Q+$" WHERE alumni_id='{aid}'");
            else if(string.IsNullOrEmpty(aid)&&!string.IsNullOrEmpty(dept)&&!string.IsNullOrEmpty(stream)&&!string.IsNullOrEmpty(gy)&&!string.IsNullOrEmpty(es))
                await Load("SELECT alumni.alumni_id,alumni.student_id,alumni.graduated_year,alumni.employment_status,alumni.employed_office,alumni.home_address,alumni.mobile_number,alumni.current_educational_status FROM ecc_dof_wukrostmarycollege.departments,ecc_dof_wukrostmarycollege.streams,ecc_dof_wukrostmarycollege.alumni WHERE departments.dept_id='"+dept+"' AND streams.stream_id='"+stream+"' AND alumni.graduated_year='"+gy+"' AND alumni.employment_status='"+es+"'");
            else Msg("Invalid filter parameters!",false);
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e) { TxtFAlumniID.Text=TxtFDept.Text=TxtFStream.Text=TxtFGradYear.Text=TxtFEmpStatus.Text=""; await Load(Q); }
        private async void TxtFilter_Changed(object s, TextChangedEventArgs e) { string t=TxtFilter.Text.Trim(); await Load(string.IsNullOrEmpty(t)?Q:Q+$" WHERE alumni_id LIKE '%{t}%'"); }
        private async void BtnReset_Click(object s, RoutedEventArgs e){TxtFilter.Text="";await Load(Q);}
        private void BtnClear_Click(object s, RoutedEventArgs e)=>Clear();
        private void Clear(){TxtAlumniID.Text=TxtStudID.Text=TxtGradYear.Text=TxtEmpStatus.Text=TxtEmpOffice.Text=TxtHomeAddr.Text=TxtMobile.Text=TxtEduStatus.Text="";_selKey="";}
        private void Msg(string m,bool ok){var o=Window.GetWindow(this);if(ok)ModernDialog.Show(o,m,"Success",ModernDialog.DialogType.Success);else ModernDialog.Show(o,m,"Error",ModernDialog.DialogType.Error);}
    }
}