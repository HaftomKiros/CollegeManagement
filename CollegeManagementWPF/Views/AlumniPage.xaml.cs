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

        public AlumniPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); ApplyPermissions(); Loaded += async (s,e) => await Load(Q); }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1) g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2) g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private void ApplyPermissions() {
            if (SessionUser.IsSuperAdmin) return;
            Grid1.Visibility     = SessionUser.Has("alumni_view")   ? Visibility.Visible : Visibility.Collapsed;
            BtnSave.Visibility   = SessionUser.Has("alumni_add")    ? Visibility.Visible : Visibility.Collapsed;
            BtnUpdate.Visibility = SessionUser.Has("alumni_update") ? Visibility.Visible : Visibility.Collapsed;
            BtnDelete.Visibility = SessionUser.Has("alumni_delete") ? Visibility.Visible : Visibility.Collapsed;
            BtnClear.Visibility  = (SessionUser.Has("alumni_add") || SessionUser.Has("alumni_update")) ? Visibility.Visible : Visibility.Collapsed;
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
                string ai=TxtAlumniID.Text.Trim(),si=TxtStudID.Text.Trim(),gy=TxtGradYear.Text.Trim(),es=TxtEmpStatus.Text.Trim(),eo=TxtEmpOffice.Text.Trim(),ha=TxtHomeAddr.Text.Trim(),mo=TxtMobile.Text.Trim(),ed=TxtEduStatus.Text.Trim();
                bool dup = await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.alumni WHERE alumni_id=@k",c); cmd.Parameters.AddWithValue("@k",ai); int n=Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n>0; });
                if(dup){Msg("There is already a department with the same ID!",false);return;}
                await Task.Run(() => {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand("INSERT INTO ecc_dof_wukrostmarycollege.alumni (alumni_id,student_id,graduated_year,employment_status,employed_office,home_address,mobile_number,current_educational_status) VALUES(@a,@s,@g,@es,@eo,@ha,@m,@ed)",c);
                    cmd.Parameters.AddWithValue("@a",ai); cmd.Parameters.AddWithValue("@s",si);
                    cmd.Parameters.AddWithValue("@g",gy); cmd.Parameters.AddWithValue("@es",es);
                    cmd.Parameters.AddWithValue("@eo",eo); cmd.Parameters.AddWithValue("@ha",ha);
                    cmd.Parameters.AddWithValue("@m",mo); cmd.Parameters.AddWithValue("@ed",ed);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            if(TxtAlumniID.Text.Trim()!=_selKey){Msg("Update attempt failed!",false);return;}
            try {
                string key=_selKey,si=TxtStudID.Text.Trim(),gy=TxtGradYear.Text.Trim(),es=TxtEmpStatus.Text.Trim(),eo=TxtEmpOffice.Text.Trim(),ha=TxtHomeAddr.Text.Trim(),mo=TxtMobile.Text.Trim(),ed=TxtEduStatus.Text.Trim();
                await Task.Run(() => {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.alumni SET student_id=@s,graduated_year=@g,employment_status=@es,employed_office=@eo,home_address=@ha,mobile_number=@m,current_educational_status=@ed WHERE alumni_id=@k",c);
                    cmd.Parameters.AddWithValue("@s",si); cmd.Parameters.AddWithValue("@g",gy);
                    cmd.Parameters.AddWithValue("@es",es); cmd.Parameters.AddWithValue("@eo",eo);
                    cmd.Parameters.AddWithValue("@ha",ha); cmd.Parameters.AddWithValue("@m",mo);
                    cmd.Parameters.AddWithValue("@ed",ed); cmd.Parameters.AddWithValue("@k",key);
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
            string aid=TxtFAlumniID.Text.Trim(), dept=TxtFDept.Text.Trim(),
                   stream=TxtFStream.Text.Trim(), gy=TxtFGradYear.Text.Trim(), es=TxtFEmpStatus.Text.Trim();

            if (!string.IsNullOrEmpty(aid)) {
                await Load(Q + $" WHERE TRIM(alumni_id)='{aid.Replace("'","''")}' OR TRIM(student_id)='{aid.Replace("'","''")}'");
                return;
            }
            var conditions = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(dept))   conditions.Add($"alumni.student_id IN (SELECT TRIM(student_id) FROM ecc_dof_wukrostmarycollege.student_profile WHERE dept_id='{dept.Replace("'","''")}')");
            if (!string.IsNullOrEmpty(stream)) conditions.Add($"alumni.student_id IN (SELECT TRIM(student_id) FROM ecc_dof_wukrostmarycollege.student_profile WHERE stream_id='{stream.Replace("'","''")}')");
            if (!string.IsNullOrEmpty(gy))     conditions.Add($"graduated_year='{gy.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(es))     conditions.Add($"employment_status='{es.Replace("'","''")}'");
            await Load(conditions.Count > 0 ? Q + " WHERE " + string.Join(" AND ", conditions) : Q);
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e) { TxtFAlumniID.Text=TxtFDept.Text=TxtFStream.Text=TxtFGradYear.Text=TxtFEmpStatus.Text=""; await Load(Q); }
        private async void TxtFilter_Changed(object s, TextChangedEventArgs e) {
            string t=TxtFilter.Text.Trim();
            if (string.IsNullOrEmpty(t)) { await Load(Q); return; }
            string safe=t.Replace("'","''");
            await Load(Q+$" WHERE TRIM(alumni_id) LIKE '%{safe}%' OR TRIM(student_id) LIKE '%{safe}%' OR employed_office LIKE '%{safe}%'");
        }
        private async void BtnReset_Click(object s, RoutedEventArgs e){TxtFilter.Text="";await Load(Q);}
        private void BtnClear_Click(object s, RoutedEventArgs e)=>Clear();
        private void Clear(){TxtAlumniID.Text=TxtStudID.Text=TxtGradYear.Text=TxtEmpStatus.Text=TxtEmpOffice.Text=TxtHomeAddr.Text=TxtMobile.Text=TxtEduStatus.Text="";_selKey="";}
        private void Msg(string m,bool ok){var o=Window.GetWindow(this);if(ok)ModernDialog.Show(o,m,"Success",ModernDialog.DialogType.Success);else ModernDialog.Show(o,m,"Error",ModernDialog.DialogType.Error);}

        private async void BtnExportPdf_Click(object s, RoutedEventArgs e) => await ExportPdf();
        private async void BtnExportExcel_Click(object s, RoutedEventArgs e) => await ExportExcel();

        private async Task ExportPdf()
        {
            if (Grid1.ItemsSource is not System.Data.DataView view||view.Count==0){Msg("No data.",false);return;}
            var dlg=new Microsoft.Win32.SaveFileDialog{FileName=$"Alumni_{DateTime.Now:yyyyMMdd}",DefaultExt=".pdf",Filter="PDF|*.pdf"};
            if(dlg.ShowDialog()!=true)return;
            if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Visible;
            await Task.Delay(50);
            try{
                string path=dlg.FileName;
                string[] fields={"alumni_id","student_id","graduated_year","employment_status","employed_office","home_address","mobile_number","current_educational_status"};
                string[] headers={"Alumni ID","Student ID","Grad Year","Emp Status","Office","Home Address","Mobile","Edu Status"};
                var rows=new System.Collections.Generic.List<string[]>();
                foreach(System.Data.DataRowView drv in view)rows.Add(System.Array.ConvertAll(fields,f=>{try{return drv[f]?.ToString()??""; }catch{return "";}}));
                await Task.Run(()=>{
                    var doc=new MigraDoc.DocumentObjectModel.Document();var sec=doc.AddSection();
                    sec.PageSetup.Orientation=MigraDoc.DocumentObjectModel.Orientation.Landscape;sec.PageSetup.PageFormat=MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.TopMargin=sec.PageSetup.BottomMargin=sec.PageSetup.LeftMargin=sec.PageSetup.RightMargin="1.5cm";
                    var tp=sec.AddParagraph("Wukro St. Mary College");tp.Format.Font.Bold=true;tp.Format.Font.Size=14;tp.Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    sec.AddParagraph("Alumni List").Format.Font.Size=11;
                    var tbl=sec.AddTable();tbl.Borders.Width=0.25;tbl.Borders.Color=MigraDoc.DocumentObjectModel.Colors.LightGray;
                    foreach(var _ in headers)tbl.AddColumn("2.8cm");
                    var hRow=tbl.AddRow();hRow.Shading.Color=new MigraDoc.DocumentObjectModel.Color(18,52,116);
                    for(int c=0;c<headers.Length;c++){hRow.Cells[c].AddParagraph(headers[c]).Format.Font.Bold=true;hRow.Cells[c].Format.Font.Color=MigraDoc.DocumentObjectModel.Colors.White;hRow.Cells[c].Format.Font.Size=8;}
                    bool alt=false;
                    foreach(var cols in rows){var row=tbl.AddRow();if(alt)row.Shading.Color=new MigraDoc.DocumentObjectModel.Color(245,247,250);alt=!alt;for(int c=0;c<cols.Length;c++)row.Cells[c].AddParagraph(cols[c]).Format.Font.Size=8;}
                    var r=new MigraDoc.Rendering.PdfDocumentRenderer{Document=doc};r.RenderDocument();r.PdfDocument.Save(path);
                });
                Msg("PDF saved!",true);
            }catch(Exception ex){Msg("PDF failed: "+ex.Message,false);}
            finally{if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Collapsed;}
        }

        private async Task ExportExcel()
        {
            if(Grid1.ItemsSource is not System.Data.DataView view||view.Count==0){Msg("No data.",false);return;}
            var dlg=new Microsoft.Win32.SaveFileDialog{FileName=$"Alumni_{DateTime.Now:yyyyMMdd}",DefaultExt=".xlsx",Filter="Excel|*.xlsx"};
            if(dlg.ShowDialog()!=true)return;
            if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Visible;
            try{
                string path=dlg.FileName;
                string[] fields={"alumni_id","student_id","graduated_year","employment_status","employed_office","home_address","mobile_number","current_educational_status"};
                string[] headers={"Alumni ID","Student ID","Grad Year","Emp Status","Office","Home Address","Mobile","Edu Status"};
                var snap=new System.Collections.Generic.List<string[]>();
                foreach(System.Data.DataRowView drv in view)snap.Add(System.Array.ConvertAll(fields,f2=>{try{return drv[f2]?.ToString()??"";}catch{return "";}}));
                await Task.Run(()=>{
                    using var wb=new ClosedXML.Excel.XLWorkbook();var ws=wb.Worksheets.Add("Alumni");
                    for(int c=0;c<headers.Length;c++){var cell=ws.Cell(1,c+1);cell.Value=headers[c];cell.Style.Font.Bold=true;cell.Style.Fill.BackgroundColor=ClosedXML.Excel.XLColor.FromHtml("#1A3A6B");cell.Style.Font.FontColor=ClosedXML.Excel.XLColor.White;}
                    int row=2;foreach(var srow in snap){for(int c=0;c<srow.Length;c++)ws.Cell(row,c+1).Value=srow[c];row++;}
                    ws.Columns().AdjustToContents();ws.SheetView.FreezeRows(1);wb.SaveAs(path);
                });
                Msg($"Exported {view.Count} records!",true);
            }catch(Exception ex){Msg("Export failed: "+ex.Message,false);}
            finally{if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Collapsed;}
        }
    }
}