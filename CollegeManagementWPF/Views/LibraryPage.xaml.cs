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

        public LibraryPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); ApplyPermissions(); Loaded += async (s,e) => await Load(Q); }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1) g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2) g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private void ApplyPermissions() {
            if (SessionUser.IsSuperAdmin) return;
            Grid1.Visibility     = SessionUser.Has("lib_view")   ? Visibility.Visible : Visibility.Collapsed;
            BtnSave.Visibility   = SessionUser.Has("lib_add")    ? Visibility.Visible : Visibility.Collapsed;
            BtnUpdate.Visibility = SessionUser.Has("lib_update") ? Visibility.Visible : Visibility.Collapsed;
            BtnDelete.Visibility = SessionUser.Has("lib_delete") ? Visibility.Visible : Visibility.Collapsed;
            BtnClear.Visibility  = (SessionUser.Has("lib_add") || SessionUser.Has("lib_update")) ? Visibility.Visible : Visibility.Collapsed;
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

        // Filter: flexible - any combination
        private async void BtnFilter_Click(object sender, RoutedEventArgs e) {
            string bid=TxtFBookID.Text.Trim(), ti=TxtFTitle.Text.Trim(),
                   di=TxtFDept.Text.Trim(), si=TxtFStream.Text.Trim();
            var conditions = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(bid)) conditions.Add($"book_id='{bid.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(ti))  conditions.Add($"book_title LIKE '%{ti.Replace("'","''")}%'");
            if (!string.IsNullOrEmpty(di))  conditions.Add($"book_dept_id='{di.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(si))  conditions.Add($"book_stream_id='{si.Replace("'","''")}'");
            await Load(conditions.Count > 0 ? Q + " WHERE " + string.Join(" AND ", conditions) : Q);
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e) { TxtFBookID.Text=TxtFTitle.Text=TxtFDept.Text=TxtFStream.Text=""; await Load(Q); }
        private void BtnClear_Click(object s, RoutedEventArgs e)=>Clear();
        private void Clear(){TxtBookID.Text=TxtTitle.Text=TxtDeptID.Text=TxtStreamID.Text=TxtLevelID.Text=TxtModCode.Text=TxtFilePath.Text="";_selKey="";}
        private void Msg(string m,bool ok){var o=Window.GetWindow(this);if(ok)ModernDialog.Show(o,m,"Success",ModernDialog.DialogType.Success);else ModernDialog.Show(o,m,"Error",ModernDialog.DialogType.Error);}

        private async void BtnExportPdf_Click(object s, RoutedEventArgs e) => await ExportPdf();
        private async void BtnExportExcel_Click(object s, RoutedEventArgs e) => await ExportExcel();

        private async Task ExportPdf()
        {
            if(Grid1.ItemsSource is not System.Data.DataView view||view.Count==0){Msg("No data.",false);return;}
            var dlg=new SaveFileDialog{FileName=$"Library_{DateTime.Now:yyyyMMdd}",DefaultExt=".pdf",Filter="PDF|*.pdf"};
            if(dlg.ShowDialog()!=true)return;
            if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Visible;
            await Task.Delay(50);
            try{
                string path=dlg.FileName;
                string[] fields={"book_id","book_type","book_title","book_dept_id","book_stream_id","book_level_id","book_module_code"};
                string[] headers={"Book ID","Type","Title","Dept","Stream","Level","Module"};
                var rows=new System.Collections.Generic.List<string[]>();
                foreach(System.Data.DataRowView drv in view)rows.Add(System.Array.ConvertAll(fields,f=>{try{return drv[f]?.ToString()??""; }catch{return "";}}));
                await Task.Run(()=>{
                    var doc=new MigraDoc.DocumentObjectModel.Document();var sec=doc.AddSection();
                    sec.PageSetup.Orientation=MigraDoc.DocumentObjectModel.Orientation.Landscape;sec.PageSetup.PageFormat=MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.TopMargin=sec.PageSetup.BottomMargin=sec.PageSetup.LeftMargin=sec.PageSetup.RightMargin="1.5cm";
                    var tp=sec.AddParagraph("Wukro St. Mary College");tp.Format.Font.Bold=true;tp.Format.Font.Size=14;tp.Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    sec.AddParagraph("Library List").Format.Font.Size=11;
                    var tbl=sec.AddTable();tbl.Borders.Width=0.25;tbl.Borders.Color=MigraDoc.DocumentObjectModel.Colors.LightGray;
                    double[] ws2={2.5,3.0,7.0,2.0,2.5,2.0,3.0};foreach(var w in ws2)tbl.AddColumn($"{w}cm");
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
            var dlg=new SaveFileDialog{FileName=$"Library_{DateTime.Now:yyyyMMdd}",DefaultExt=".xlsx",Filter="Excel|*.xlsx"};
            if(dlg.ShowDialog()!=true)return;
            if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Visible;
            try{
                string path=dlg.FileName;
                string[] fields={"book_id","book_type","book_title","book_dept_id","book_stream_id","book_level_id","book_module_code"};
                string[] headers={"Book ID","Type","Title","Dept","Stream","Level","Module"};
                var snap=new System.Collections.Generic.List<string[]>();
                foreach(System.Data.DataRowView drv in view)snap.Add(System.Array.ConvertAll(fields,f2=>{try{return drv[f2]?.ToString()??"";}catch{return "";}}));
                await Task.Run(()=>{
                    using var wb=new ClosedXML.Excel.XLWorkbook();var ws=wb.Worksheets.Add("Library");
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
