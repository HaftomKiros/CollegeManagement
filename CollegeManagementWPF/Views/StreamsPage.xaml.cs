using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class StreamsPage : Page
    {
        private string _selKey = "";
        private DBConnect _db = new DBConnect();
        private const string Q = "SELECT dept_id,stream_id,stream_name,no_of_levels FROM ecc_dof_wukrostmarycollege.streams";

        public StreamsPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); Loaded += async (s,e) => { await LoadDepts(); await Load(Q); }; }

        private async Task LoadDepts()
        {
            try {
                var list = await Task.Run(() => { var r=new System.Collections.Generic.List<string>(); var c=_db.GetConnection(); c.Open(); using var cmd=new MySqlCommand("SELECT dept_id FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id",c); using var rd=cmd.ExecuteReader(); while(rd.Read()) r.Add(rd[0]?.ToString()??""); c.Close(); return r; });
                TxtDeptID.Items.Clear();
                foreach (var d in list) TxtDeptID.Items.Add(new System.Windows.Controls.ComboBoxItem{Content=d});
            } catch {}
        }

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
            _selKey = r["stream_id"]?.ToString() ?? "";
            TxtDeptID.Text = r["dept_id"]?.ToString() ?? "";
            TxtStreamID.Text = r["stream_id"]?.ToString() ?? "";
            TxtStreamName.Text = r["stream_name"]?.ToString() ?? "";
            TxtNoLevels.Text = r["no_of_levels"]?.ToString() ?? "";
        }

        private async void BtnSave_Click(object s, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TxtDeptID.Text) ||
                string.IsNullOrWhiteSpace(TxtStreamID.Text) ||
                string.IsNullOrWhiteSpace(TxtStreamName.Text) ||
                string.IsNullOrWhiteSpace(TxtNoLevels.Text))
            { Msg("There is empty field(s). Please fill all fields!",false); return; }
            try {
                string did=TxtDeptID.Text.Trim(), sid2=TxtStreamID.Text.Trim(), sname=TxtStreamName.Text.Trim(), nlvl=TxtNoLevels.Text.Trim();
                bool dup = await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.streams WHERE stream_id=@k",c); cmd.Parameters.AddWithValue("@k",sid2); int n=Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n>0; });
                if(dup){Msg("There is already a record with the same ID!",false);return;}
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("INSERT INTO ecc_dof_wukrostmarycollege.streams (dept_id,stream_id,stream_name,no_of_levels) VALUES(@d,@s,@n,@l)",c);
                    cmd.Parameters.AddWithValue("@d",did); cmd.Parameters.AddWithValue("@s",sid2);
                    cmd.Parameters.AddWithValue("@n",sname); cmd.Parameters.AddWithValue("@l",nlvl);
                    cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Saved successfully!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            if(TxtStreamID.Text.Trim()!=_selKey){Msg("Update attempt failed!",false);return;}
            try {
                string key=_selKey, did=TxtDeptID.Text.Trim(), sname=TxtStreamName.Text.Trim(), nlvl=TxtNoLevels.Text.Trim();
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("UPDATE ecc_dof_wukrostmarycollege.streams SET dept_id=@d,stream_name=@n,no_of_levels=@l WHERE stream_id=@key",c);
                    cmd.Parameters.AddWithValue("@d",did); cmd.Parameters.AddWithValue("@n",sname);
                    cmd.Parameters.AddWithValue("@l",nlvl); cmd.Parameters.AddWithValue("@key",key); cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Update successful!",true); await Load(Q);
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(_selKey)){Msg("Select a record first.",false);return;}
            var dlg=new ModernDialog($"Delete record '{_selKey}'?","Confirm",ModernDialog.DialogType.Warning){Owner=Window.GetWindow(this)};
            if(dlg.ShowDialog()!=true)return;
            string key=_selKey;
            try {
                await Task.Run(() => { var c=_db.GetConnection(); c.Open(); var cmd=new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.streams WHERE stream_id=@k",c); cmd.Parameters.AddWithValue("@k",key); cmd.ExecuteNonQuery(); c.Close(); });
                Msg("Delete successful!",true); await Load(Q); Clear();
            } catch(Exception ex){Msg("Connection failed! "+ex.Message,false);}
        }

        private async void TxtFilter_Changed(object s, TextChangedEventArgs e) { string t=TxtFilter.Text.Trim(); await Load(string.IsNullOrEmpty(t)?Q:Q+$" WHERE stream_id LIKE '%{t}%' OR stream_name LIKE '%{t}%'"); }
        private async void BtnReset_Click(object s, RoutedEventArgs e){TxtFilter.Text="";await Load(Q);}
        private void BtnClear_Click(object s, RoutedEventArgs e)=>Clear();
        private void Clear(){TxtDeptID.Text = TxtStreamID.Text = TxtStreamName.Text = TxtNoLevels.Text="";_selKey="";}
        private void Msg(string m,bool ok){var o=Window.GetWindow(this);if(ok)ModernDialog.Show(o,m,"Success",ModernDialog.DialogType.Success);else ModernDialog.Show(o,m,"Error",ModernDialog.DialogType.Error);}

        private async void BtnExportPdf_Click(object s, RoutedEventArgs e) => await ExportPdf(new[]{"dept_id","stream_id","stream_name","no_of_levels"}, new[]{"Dept ID","Stream ID","Name","No. Levels"}, "Streams");
        private async void BtnExportExcel_Click(object s, RoutedEventArgs e) => await ExportExcel(new[]{"dept_id","stream_id","stream_name","no_of_levels"}, new[]{"Dept ID","Stream ID","Name","No. Levels"}, "Streams");

        private async Task ExportPdf(string[] fields, string[] headers, string title)
        {
            if (Grid1.ItemsSource is not System.Data.DataView view || view.Count==0){Msg("No data.",false);return;}
            var dlg=new Microsoft.Win32.SaveFileDialog{FileName=$"{title}_{DateTime.Now:yyyyMMdd}",DefaultExt=".pdf",Filter="PDF|*.pdf"};
            if(dlg.ShowDialog()!=true)return;
            if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Visible;
            await Task.Delay(50);
            try{
                string path=dlg.FileName;
                var rows=new System.Collections.Generic.List<string[]>();
                foreach(System.Data.DataRowView drv in view)rows.Add(System.Array.ConvertAll(fields,f=>{try{return drv[f]?.ToString()??""; }catch{return "";}}));
                await Task.Run(()=>{
                    var doc=new MigraDoc.DocumentObjectModel.Document(); var sec=doc.AddSection();
                    sec.PageSetup.Orientation=MigraDoc.DocumentObjectModel.Orientation.Landscape; sec.PageSetup.PageFormat=MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.TopMargin=sec.PageSetup.BottomMargin=sec.PageSetup.LeftMargin=sec.PageSetup.RightMargin="1.5cm";
                    var tp=sec.AddParagraph("Wukro St. Mary College"); tp.Format.Font.Bold=true; tp.Format.Font.Size=14; tp.Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    sec.AddParagraph(title+" List").Format.Font.Size=11;
                    var tbl=sec.AddTable(); tbl.Borders.Width=0.25; tbl.Borders.Color=MigraDoc.DocumentObjectModel.Colors.LightGray;
                    foreach(var _ in headers)tbl.AddColumn("4.5cm");
                    var hRow=tbl.AddRow(); hRow.Shading.Color=new MigraDoc.DocumentObjectModel.Color(18,52,116);
                    for(int c=0;c<headers.Length;c++){hRow.Cells[c].AddParagraph(headers[c]).Format.Font.Bold=true;hRow.Cells[c].Format.Font.Color=MigraDoc.DocumentObjectModel.Colors.White;hRow.Cells[c].Format.Font.Size=9;}
                    bool alt=false;
                    foreach(var cols in rows){var row=tbl.AddRow();if(alt)row.Shading.Color=new MigraDoc.DocumentObjectModel.Color(245,247,250);alt=!alt;for(int c=0;c<cols.Length;c++)row.Cells[c].AddParagraph(cols[c]).Format.Font.Size=9;}
                    var r=new MigraDoc.Rendering.PdfDocumentRenderer{Document=doc};r.RenderDocument();r.PdfDocument.Save(path);
                });
                Msg("PDF saved!",true);
            }catch(Exception ex){Msg("PDF failed: "+ex.Message,false);}
            finally{if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Collapsed;}
        }

        private async Task ExportExcel(string[] fields, string[] headers, string sheetName)
        {
            if(Grid1.ItemsSource is not System.Data.DataView view||view.Count==0){Msg("No data.",false);return;}
            var dlg=new Microsoft.Win32.SaveFileDialog{FileName=$"{sheetName}_{DateTime.Now:yyyyMMdd}",DefaultExt=".xlsx",Filter="Excel|*.xlsx"};
            if(dlg.ShowDialog()!=true)return;
            if(LoadingOverlay!=null)LoadingOverlay.Visibility=Visibility.Visible;
            try{
                string path=dlg.FileName;
                var snap=new System.Collections.Generic.List<string[]>();
                foreach(System.Data.DataRowView drv in view) snap.Add(System.Array.ConvertAll(fields,f2=>{try{return drv[f2]?.ToString()??"";}catch{return "";}}));
                await Task.Run(()=>{
                    using var wb=new ClosedXML.Excel.XLWorkbook(); var ws=wb.Worksheets.Add(sheetName);
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


