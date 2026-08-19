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
    public partial class EmployeePage : Page
    {
        private string _selKey = "";
        private DBConnect _db = new DBConnect();
        private const string Q =
            "SELECT employee_id,department_id,first_name,middle_name,last_name,sex," +
            "birth_date,employee_date,level,qualification_title,mobile_number " +
            "FROM ecc_dof_wukrostmarycollege.employee_profile";

        public EmployeePage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); ApplyPermissions(); Loaded += async (s,e) => await Load(Q); }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1) g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2) g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private void ApplyPermissions() {
            if (SessionUser.IsSuperAdmin) return;
            Grid1.Visibility     = SessionUser.Has("emp_view")   ? Visibility.Visible : Visibility.Collapsed;
            BtnSave.Visibility   = SessionUser.Has("emp_add")    ? Visibility.Visible : Visibility.Collapsed;
            BtnUpdate.Visibility = SessionUser.Has("emp_update") ? Visibility.Visible : Visibility.Collapsed;
            BtnDelete.Visibility = SessionUser.Has("emp_delete") ? Visibility.Visible : Visibility.Collapsed;
            BtnClear.Visibility  = (SessionUser.Has("emp_add") || SessionUser.Has("emp_update")) ? Visibility.Visible : Visibility.Collapsed;
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
            _selKey = r["employee_id"]?.ToString() ?? "";
            TxtEmpID.Text       = _selKey;
            TxtDeptID.Text      = r["department_id"]?.ToString() ?? "";
            TxtFName.Text       = r["first_name"]?.ToString() ?? "";
            TxtMName.Text       = r["middle_name"]?.ToString() ?? "";
            TxtLName.Text       = r["last_name"]?.ToString() ?? "";
            TxtSex.Text         = r["sex"]?.ToString() ?? "";
            TxtBirthDate.Text   = r["birth_date"]?.ToString() ?? "";
            TxtEmpDate.Text     = r["employee_date"]?.ToString() ?? "";
            TxtLevel.Text       = r["level"]?.ToString() ?? "";
            TxtQualification.Text = r["qualification_title"]?.ToString() ?? "";
            TxtMobile.Text      = r["mobile_number"]?.ToString() ?? "";
            TxtPhoto.Text       = "";
        }

        private void BtnBrowsePhoto_Click(object sender, RoutedEventArgs e) {
            var dlg = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp" };
            if (dlg.ShowDialog() == true) TxtPhoto.Text = dlg.FileName;
        }

        private async void BtnSave_Click(object s, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TxtEmpID.Text)||string.IsNullOrWhiteSpace(TxtDeptID.Text)||
                string.IsNullOrWhiteSpace(TxtFName.Text)||string.IsNullOrWhiteSpace(TxtMName.Text)||
                string.IsNullOrWhiteSpace(TxtLName.Text)||string.IsNullOrWhiteSpace(TxtSex.Text)||
                string.IsNullOrWhiteSpace(TxtMobile.Text)||string.IsNullOrWhiteSpace(TxtBirthDate.Text)||
                string.IsNullOrWhiteSpace(TxtLevel.Text)||string.IsNullOrWhiteSpace(TxtQualification.Text))
            { Msg("There is empty field(s). Please fill all fields!",false); return; }
            try {
                string eid2=TxtEmpID.Text.Trim();
                bool dup = await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.employee_profile WHERE employee_id=@k",c); cmd.Parameters.AddWithValue("@k",eid2); int n=Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n>0; });
                if(dup){Msg("There is already an employee with the same ID!",false);return;}

                // Store file path (WPF uses file paths, not BLOB)
                string photoPath = TxtPhoto.Text.Trim();
                string eid=TxtEmpID.Text.Trim(), did=TxtDeptID.Text.Trim(), fn=TxtFName.Text.Trim(),
                       mn=TxtMName.Text.Trim(), ln=TxtLName.Text.Trim(), sx=TxtSex.Text.Trim(),
                       bd=TxtBirthDate.Text.Trim(), ed=TxtEmpDate.Text.Trim(), lv=TxtLevel.Text.Trim(),
                       qt=TxtQualification.Text.Trim(), mob=TxtMobile.Text.Trim();

                await Task.Run(() => {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand("INSERT INTO ecc_dof_wukrostmarycollege.employee_profile (employee_id,department_id,first_name,middle_name,last_name,sex,birth_date,employee_date,level,qualification_title,mobile_number) VALUES(@ei,@di,@fn,@mn,@ln,@sx,@bd,@ed,@lv,@qt,@mob)",c);
                    cmd.Parameters.AddWithValue("@ei",eid); cmd.Parameters.AddWithValue("@di",did);
                    cmd.Parameters.AddWithValue("@fn",fn);  cmd.Parameters.AddWithValue("@mn",mn);
                    cmd.Parameters.AddWithValue("@ln",ln);  cmd.Parameters.AddWithValue("@sx",sx);
                    cmd.Parameters.AddWithValue("@bd",bd);  cmd.Parameters.AddWithValue("@ed",ed);
                    cmd.Parameters.AddWithValue("@lv",lv);  cmd.Parameters.AddWithValue("@qt",qt);
                    cmd.Parameters.AddWithValue("@mob",mob);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            if(TxtEmpID.Text.Trim()!=_selKey){Msg("Update attempt failed!",false);return;}
            string key=_selKey, did=TxtDeptID.Text.Trim(), fn=TxtFName.Text.Trim(), mn=TxtMName.Text.Trim(),
                   ln=TxtLName.Text.Trim(), sx=TxtSex.Text.Trim(), bd=TxtBirthDate.Text.Trim(),
                   ed=TxtEmpDate.Text.Trim(), lv=TxtLevel.Text.Trim(), qt=TxtQualification.Text.Trim(), mob=TxtMobile.Text.Trim();
            try {
                await Task.Run(() => {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.employee_profile SET department_id=@di,first_name=@fn,middle_name=@mn,last_name=@ln,sex=@sx,birth_date=@bd,employee_date=@ed,level=@lv,qualification_title=@qt,mobile_number=@mob WHERE employee_id=@k",c);
                    cmd.Parameters.AddWithValue("@di",did); cmd.Parameters.AddWithValue("@fn",fn);
                    cmd.Parameters.AddWithValue("@mn",mn);  cmd.Parameters.AddWithValue("@ln",ln);
                    cmd.Parameters.AddWithValue("@sx",sx);  cmd.Parameters.AddWithValue("@bd",bd);
                    cmd.Parameters.AddWithValue("@ed",ed);  cmd.Parameters.AddWithValue("@lv",lv);
                    cmd.Parameters.AddWithValue("@qt",qt);  cmd.Parameters.AddWithValue("@mob",mob);
                    cmd.Parameters.AddWithValue("@k",key);  cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Update successful!",true); await Load(Q);
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            var dlg=new ModernDialog($"Delete employee '{_selKey}'?","Confirm",ModernDialog.DialogType.Warning){Owner=Window.GetWindow(this)};
            if(dlg.ShowDialog()!=true)return;
            string key=_selKey;
            try {
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.employee_profile WHERE employee_id=@k",c); cmd.Parameters.AddWithValue("@k",key); cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Delete successful!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string eid   = TxtFEmpID.Text.Trim();
            string fn    = TxtFFName.Text.Trim();
            string mn    = TxtFMName.Text.Trim();
            string ln    = TxtFLName.Text.Trim();
            string dept  = TxtFDept.Text.Trim();
            string lvl   = TxtFLevel.Text.Trim();

            // Employee ID takes priority alone
            if (!string.IsNullOrEmpty(eid))
            { await Load(Q + $" WHERE employee_id='{eid.Replace("'","''")}' "); return; }

            // Build OR-style conditions from name / dept / level
            var conds = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(fn))   conds.Add($"first_name LIKE '%{fn.Replace("'","''")}%'");
            if (!string.IsNullOrEmpty(mn))   conds.Add($"middle_name LIKE '%{mn.Replace("'","''")}%'");
            if (!string.IsNullOrEmpty(ln))   conds.Add($"last_name LIKE '%{ln.Replace("'","''")}%'");
            if (!string.IsNullOrEmpty(dept)) conds.Add($"department_id='{dept.Replace("'","''")}' ");
            if (!string.IsNullOrEmpty(lvl))  conds.Add($"level='{lvl.Replace("'","''")}' ");

            if (conds.Count == 0) { await Load(Q); return; }
            await Load(Q + " WHERE " + string.Join(" AND ", conds));
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            TxtFEmpID.Text = TxtFFName.Text = TxtFMName.Text = TxtFLName.Text =
            TxtFDept.Text  = TxtFLevel.Text = "";
            await Load(Q);
        }
        private void BtnClear_Click(object s, RoutedEventArgs e)=>Clear();
        private void Clear(){TxtEmpID.Text=TxtDeptID.Text=TxtFName.Text=TxtMName.Text=TxtLName.Text=TxtSex.Text=TxtBirthDate.Text=TxtEmpDate.Text=TxtLevel.Text=TxtQualification.Text=TxtMobile.Text=TxtPhoto.Text="";_selKey="";}
        private void Msg(string m,bool ok){var o=Window.GetWindow(this);if(ok)ModernDialog.Show(o,m,"Success",ModernDialog.DialogType.Success);else ModernDialog.Show(o,m,"Error",ModernDialog.DialogType.Error);}
    }
}
