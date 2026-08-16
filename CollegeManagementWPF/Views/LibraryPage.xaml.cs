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
    public partial class LibraryPage : Page
    {
        private string _selKey = "";
        private DBConnect _db = new DBConnect();
        private const string Q =
            "SELECT book_id,book_type,book_title,book_dept_id,book_stream_id,book_level_id,book_module_code " +
            "FROM ecc_dof_wukrostmarycollege.library";

        public LibraryPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); Loaded += async (s,e) => await Load(Q); }

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
            _selKey = r["book_id"]?.ToString() ?? "";
            TxtBookID.Text   = _selKey;
            TxtTitle.Text    = r["book_title"]?.ToString() ?? "";
            TxtDeptID.Text   = r["book_dept_id"]?.ToString() ?? "";
            TxtStreamID.Text = r["book_stream_id"]?.ToString() ?? "";
            TxtLevelID.Text  = r["book_level_id"]?.ToString() ?? "";
            TxtModCode.Text  = r["book_module_code"]?.ToString() ?? "";
            TxtFilePath.Text = "";
            SetCombo(CmbBookType, r["book_type"]?.ToString() ?? "Ref. Book");
        }

        private void SetCombo(ComboBox c, string v) { foreach(ComboBoxItem i in c.Items) if(i.Content?.ToString()==v){c.SelectedItem=i;return;} }
        private string Cmb(ComboBox c) => (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        private void BtnBrowse_Click(object sender, RoutedEventArgs e) {
            var dlg = new OpenFileDialog { Filter = "PDF Files|*.pdf|Word Files|*.docx|All Files|*.*" };
            if (dlg.ShowDialog() == true) TxtFilePath.Text = dlg.FileName;
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a book first.",false);return;}
            var dlg = new SaveFileDialog { FileName=$"book_{_selKey}", Filter="PDF Files|*.pdf|Word Files|*.docx|All Files|*.*" };
            if(dlg.ShowDialog()!=true)return;
            try {
                byte[]? data = await Task.Run(() => {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand("SELECT book_file FROM ecc_dof_wukrostmarycollege.library WHERE book_id=@k",c);
                    cmd.Parameters.AddWithValue("@k",_selKey);
                    var bytes = cmd.ExecuteScalar() as byte[];
                    c.Close(); return bytes;
                });
                if(data==null||data.Length==0){Msg("No file stored for this book.",false);return;}
                await File.WriteAllBytesAsync(dlg.FileName, data);
                Msg("Downloaded successfully!",true);
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnSave_Click(object s, RoutedEventArgs e) {
            string bid=TxtBookID.Text.Trim(), bt=Cmb(CmbBookType), ti=TxtTitle.Text.Trim(),
                   di=TxtDeptID.Text.Trim(), si=TxtStreamID.Text.Trim(), li=TxtLevelID.Text.Trim(),
                   mc=TxtModCode.Text.Trim(), fp=TxtFilePath.Text;
            if(string.IsNullOrWhiteSpace(bid)||string.IsNullOrWhiteSpace(ti)||string.IsNullOrWhiteSpace(di)||
               string.IsNullOrWhiteSpace(si)||string.IsNullOrWhiteSpace(fp))
            { Msg("There is empty field(s). Please fill all fields!",false); return; }
            if(bt=="Instructor Handout"&&(string.IsNullOrWhiteSpace(li)||string.IsNullOrWhiteSpace(mc)))
            { Msg("Level ID and Module Code are required for Instructor Handout!",false); return; }
            try {
                bool dup = await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.library WHERE book_id=@k",c); cmd.Parameters.AddWithValue("@k",bid); int n=Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n>0; });
                if(dup){Msg("There is already an employee with the same ID!",false);return;}
                byte[] fileBytes = await File.ReadAllBytesAsync(fp);
                await Task.Run(() => {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand("INSERT INTO ecc_dof_wukrostmarycollege.library (book_id,book_type,book_title,book_dept_id,book_stream_id,book_level_id,book_module_code,book_file) VALUES(@bi,@bt,@ti,@di,@si,@li,@mc,@f)",c);
                    cmd.Parameters.AddWithValue("@bi",bid); cmd.Parameters.AddWithValue("@bt",bt);
                    cmd.Parameters.AddWithValue("@ti",ti);  cmd.Parameters.AddWithValue("@di",di);
                    cmd.Parameters.AddWithValue("@si",si);  cmd.Parameters.AddWithValue("@li",li);
                    cmd.Parameters.AddWithValue("@mc",mc);  cmd.Parameters.AddWithValue("@f",fileBytes);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            if(TxtBookID.Text.Trim()!=_selKey){Msg("Update failed!",false);return;}
            string key=_selKey, bt=Cmb(CmbBookType), ti=TxtTitle.Text.Trim(), di=TxtDeptID.Text.Trim(),
                   si=TxtStreamID.Text.Trim(), li=TxtLevelID.Text.Trim(), mc=TxtModCode.Text.Trim(), fp=TxtFilePath.Text;
            try {
                if(!string.IsNullOrEmpty(fp)) {
                    byte[] fileBytes = await File.ReadAllBytesAsync(fp);
                    await Task.Run(() => {
                        var c=_db.GetConnection(); c.Open();
                        var cmd=new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.library SET book_type=@bt,book_title=@ti,book_dept_id=@di,book_stream_id=@si,book_level_id=@li,book_module_code=@mc,book_file=@f WHERE book_id=@k",c);
                        cmd.Parameters.AddWithValue("@bt",bt); cmd.Parameters.AddWithValue("@ti",ti);
                        cmd.Parameters.AddWithValue("@di",di); cmd.Parameters.AddWithValue("@si",si);
                        cmd.Parameters.AddWithValue("@li",li); cmd.Parameters.AddWithValue("@mc",mc);
                        cmd.Parameters.AddWithValue("@f",fileBytes); cmd.Parameters.AddWithValue("@k",key);
                        cmd.ExecuteNonQuery(); c.Close();
                    });
                } else {
                    await Task.Run(() => {
                        var c=_db.GetConnection(); c.Open();
                        var cmd=new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.library SET book_type=@bt,book_title=@ti,book_dept_id=@di,book_stream_id=@si,book_level_id=@li,book_module_code=@mc WHERE book_id=@k",c);
                        cmd.Parameters.AddWithValue("@bt",bt); cmd.Parameters.AddWithValue("@ti",ti);
                        cmd.Parameters.AddWithValue("@di",di); cmd.Parameters.AddWithValue("@si",si);
                        cmd.Parameters.AddWithValue("@li",li); cmd.Parameters.AddWithValue("@mc",mc);
                        cmd.Parameters.AddWithValue("@k",key); cmd.ExecuteNonQuery(); c.Close();
                    });
                }
                Msg("Update successful!",true); await Load(Q);
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            var dlg=new ModernDialog($"Delete book '{_selKey}'?","Confirm",ModernDialog.DialogType.Warning){Owner=Window.GetWindow(this)};
            if(dlg.ShowDialog()!=true)return;
            string key=_selKey;
            try {
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.library WHERE book_id=@k",c); cmd.Parameters.AddWithValue("@k",key); cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Delete successful!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        // Filter: by book_id alone OR title alone OR dept+stream (original algorithm)
        private async void BtnFilter_Click(object sender, RoutedEventArgs e) {
            string bid=TxtFBookID.Text.Trim(), ti=TxtFTitle.Text.Trim(), di=TxtFDept.Text.Trim(), si=TxtFStream.Text.Trim();
            if(!string.IsNullOrEmpty(bid)&&string.IsNullOrEmpty(ti)&&string.IsNullOrEmpty(di))
                await Load(Q+$" WHERE book_id='{bid}'");
            else if(string.IsNullOrEmpty(bid)&&!string.IsNullOrEmpty(ti)&&string.IsNullOrEmpty(di))
                await Load(Q+$" WHERE book_title='{ti}'");
            else if(string.IsNullOrEmpty(bid)&&string.IsNullOrEmpty(ti)&&!string.IsNullOrEmpty(di)&&!string.IsNullOrEmpty(si))
                await Load(Q+$" WHERE book_dept_id='{di}' AND book_stream_id='{si}'");
            else Msg("Invalid search parameters!",false);
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e) { TxtFBookID.Text=TxtFTitle.Text=TxtFDept.Text=TxtFStream.Text=""; await Load(Q); }
        private void BtnClear_Click(object s, RoutedEventArgs e)=>Clear();
        private void Clear(){TxtBookID.Text=TxtTitle.Text=TxtDeptID.Text=TxtStreamID.Text=TxtLevelID.Text=TxtModCode.Text=TxtFilePath.Text="";_selKey="";}
        private void Msg(string m,bool ok){var o=Window.GetWindow(this);if(ok)ModernDialog.Show(o,m,"Success",ModernDialog.DialogType.Success);else ModernDialog.Show(o,m,"Error",ModernDialog.DialogType.Error);}
    }
}
