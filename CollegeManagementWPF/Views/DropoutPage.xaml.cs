using CollegeManagementWPF.Data;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
            "SELECT d.student_id, " +
            "CONCAT(TRIM(sp.first_name),' ',TRIM(sp.father_name),' ',TRIM(sp.grand_father_name)) AS full_name, " +
            "d.drop_out_date,d.level_number,d.drop_out_reason,d.remark " +
            "FROM ecc_dof_wukrostmarycollege.drop_out_students d " +
            "LEFT JOIN (SELECT TRIM(student_id) AS student_id, dept_id, stream_id, first_name, father_name, grand_father_name " +
            "FROM ecc_dof_wukrostmarycollege.student_profile GROUP BY TRIM(student_id)) sp " +
            "ON TRIM(d.student_id)=sp.student_id";

        public DropoutPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            Loaded += async (s, e) =>
            {
                await LoadDepartments();
                await Load(Q);
            };
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private async Task LoadDepartments()
        {
            try
            {
                var depts = await Task.Run(() =>
                {
                    var list = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT dept_id FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close(); return list;
                });
                CmbFDept.Items.Clear();
                foreach (var d in depts)
                    CmbFDept.Items.Add(new ComboBoxItem { Content = d });
            }
            catch { }
        }

        private async Task Load(string q)
        {
            try
            {
                if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
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

        private void SetCombo(ComboBox c, string v)
        { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }
        private string Cmb(ComboBox c) => (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        // ── SAVE ─────────────────────────────────────────────────────────────
        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            string sid=TxtStudID.Text.Trim(), lvl=Cmb(CmbLevel),
                   dt=TxtDate.Text.Trim(), rs=Cmb(CmbReason), rm=TxtRemark.Text.Trim();
            if (string.IsNullOrWhiteSpace(sid)||string.IsNullOrWhiteSpace(lvl)||
                string.IsNullOrWhiteSpace(dt)||string.IsNullOrWhiteSpace(rs))
            { Msg("Please fill all required fields!", false); return; }
            try
            {
                bool dup = await Task.Run(() =>
                {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.drop_out_students WHERE student_id=@s AND level_number=@l",c);
                    cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl);
                    int n=Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n>0;
                });
                if (dup) { Msg("There is already a dropout record for this student and level!", false); return; }
                await Task.Run(() =>
                {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.drop_out_students " +
                        "(student_id,drop_out_date,level_number,drop_out_reason,remark) VALUES(@s,@d,@l,@r,@m)",c);
                    cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@d",dt);
                    cmd.Parameters.AddWithValue("@l",lvl); cmd.Parameters.AddWithValue("@r",rs);
                    cmd.Parameters.AddWithValue("@m",rm); cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Connection failed! "+ex.Message, false); }
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            string sid=_selSid, lvl=_selLvl,
                   dt=TxtDate.Text.Trim(), rs=Cmb(CmbReason), rm=TxtRemark.Text.Trim();
            try
            {
                await Task.Run(() =>
                {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand(
                        "UPDATE ecc_dof_wukrostmarycollege.drop_out_students " +
                        "SET drop_out_date=@d,drop_out_reason=@r,remark=@m WHERE student_id=@s AND level_number=@l",c);
                    cmd.Parameters.AddWithValue("@d",dt); cmd.Parameters.AddWithValue("@r",rs);
                    cmd.Parameters.AddWithValue("@m",rm); cmd.Parameters.AddWithValue("@s",sid);
                    cmd.Parameters.AddWithValue("@l",lvl); cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Update successful!", true); await Load(Q);
            }
            catch (Exception ex) { Msg("Connection failed! "+ex.Message, false); }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            var dlg=new ModernDialog($"Delete dropout record for {_selSid} Level {_selLvl}?",
                "Confirm", ModernDialog.DialogType.Warning){Owner=Window.GetWindow(this)};
            if (dlg.ShowDialog()!=true) return;
            string sid=_selSid, lvl=_selLvl;
            try
            {
                await Task.Run(() =>
                {
                    var c=_db.GetConnection(); c.Open();
                    var cmd=new MySqlCommand(
                        "DELETE FROM ecc_dof_wukrostmarycollege.drop_out_students WHERE student_id=@s AND level_number=@l",c);
                    cmd.Parameters.AddWithValue("@s",sid); cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Delete successful!", true); await Load(Q); Clear();
            }
            catch (Exception ex) { Msg("Connection failed! "+ex.Message, false); }
        }

        // ── FILTER ────────────────────────────────────────────────────────────
        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string dept  = (CmbFDept.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? CmbFDept.Text?.Trim() ?? "";
            string year  = TxtFYear.Text.Trim();
            string level = Cmb(CmbFLevel);

            var conditions = new List<string>();
            if (!string.IsNullOrEmpty(dept))  conditions.Add($"sp.dept_id='{dept.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(year))  conditions.Add($"d.drop_out_date='{year.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(level)) conditions.Add($"d.level_number='{level.Replace("'","''")}'");

            await Load(conditions.Count > 0 ? Q + " WHERE " + string.Join(" AND ", conditions) : Q);
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            CmbFDept.SelectedIndex = -1; CmbFDept.Text = "";
            TxtFYear.Text = ""; CmbFLevel.SelectedIndex = 0;
            await Load(Q);
        }

        // ── QUICK FILTER ──────────────────────────────────────────────────────
        private async void TxtFilter_Changed(object s, TextChangedEventArgs e)
        {
            string t = TxtFilter.Text.Trim();
            if (string.IsNullOrEmpty(t)) { await Load(Q); return; }
            string safe = t.Replace("'","''");
            await Load(Q + $" WHERE TRIM(d.student_id) LIKE '%{safe}%' " +
                $"OR CONCAT(TRIM(sp.first_name),' ',TRIM(sp.father_name),' ',TRIM(sp.grand_father_name)) LIKE '%{safe}%'");
        }

        private async void BtnReset_Click(object s, RoutedEventArgs e) { TxtFilter.Text=""; await Load(Q); }
        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();
        private void Clear() { TxtStudID.Text=TxtDate.Text=TxtRemark.Text=""; _selSid=_selLvl=""; }

        // ── PDF EXPORT ────────────────────────────────────────────────────────
        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not DataView view || view.Count == 0)
            { Msg("No data to export.", false); return; }

            var dlg = new SaveFileDialog
            { FileName=$"Dropouts_{DateTime.Now:yyyyMMdd_HHmm}", DefaultExt=".pdf", Filter="PDF File|*.pdf" };
            if (dlg.ShowDialog()!=true) return;

            if (LoadingOverlay!=null) LoadingOverlay.Visibility=Visibility.Visible;
            await Task.Delay(50);
            try
            {
                string path=dlg.FileName;
                string[] fields  = { "student_id","full_name","level_number","drop_out_date","drop_out_reason","remark" };
                string[] headers = { "Student ID","Full Name","Level","Dropout Year","Reason","Remark" };

                await Task.Run(() =>
                {
                    var rows = new List<string[]>();
                    foreach (DataRowView drv in view)
                        rows.Add(Array.ConvertAll(fields, f => { try { return drv[f]?.ToString() ?? ""; } catch { return ""; } }));

                    var doc = new MigraDoc.DocumentObjectModel.Document();
                    var section = doc.AddSection();
                    section.PageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
                    section.PageSetup.PageFormat   = MigraDoc.DocumentObjectModel.PageFormat.A4;
                    section.PageSetup.TopMargin = section.PageSetup.BottomMargin =
                    section.PageSetup.LeftMargin = section.PageSetup.RightMargin = "1.5cm";

                    var title = section.AddParagraph("Wukro St. Mary College");
                    title.Format.Font.Size=16; title.Format.Font.Bold=true;
                    title.Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    section.AddParagraph("Dropout Students List").Format.Alignment=
                        MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    var date = section.AddParagraph($"Generated: {DateTime.Now:dd MMM yyyy  HH:mm}");
                    date.Format.Font.Size=8;
                    date.Format.Font.Color=MigraDoc.DocumentObjectModel.Colors.Gray;
                    date.Format.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    date.Format.SpaceAfter="6pt";

                    var table = section.AddTable();
                    table.Borders.Width=0.25;
                    table.Borders.Color=MigraDoc.DocumentObjectModel.Colors.LightGray;
                    double[] widths = { 3.5,5.0,1.2,2.2,3.5,3.5 };
                    foreach (var w in widths) table.AddColumn($"{w}cm");

                    var hRow=table.AddRow();
                    hRow.Shading.Color=new MigraDoc.DocumentObjectModel.Color(18,52,116);
                    for (int c=0;c<headers.Length;c++)
                    {
                        hRow.Cells[c].AddParagraph(headers[c]).Format.Font.Bold=true;
                        hRow.Cells[c].Format.Font.Color=MigraDoc.DocumentObjectModel.Colors.White;
                        hRow.Cells[c].Format.Font.Size=8;
                    }

                    bool alt=false;
                    foreach (var cols in rows)
                    {
                        var row=table.AddRow();
                        if (alt) row.Shading.Color=new MigraDoc.DocumentObjectModel.Color(245,247,250);
                        alt=!alt;
                        for (int c=0;c<cols.Length;c++) { row.Cells[c].AddParagraph(cols[c]).Format.Font.Size=8; }
                    }

                    var renderer=new MigraDoc.Rendering.PdfDocumentRenderer{Document=doc};
                    renderer.RenderDocument();
                    renderer.PdfDocument.Save(path);
                });
                Msg($"PDF saved to:\n{path}", true);
            }
            catch (Exception ex) { Msg("PDF export failed: "+ex.Message, false); }
            finally { if (LoadingOverlay!=null) LoadingOverlay.Visibility=Visibility.Collapsed; }
        }

        // ── EXCEL EXPORT ──────────────────────────────────────────────────────
        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not DataView view || view.Count == 0)
            { Msg("No data to export.", false); return; }

            var dlg = new SaveFileDialog
            { FileName=$"Dropouts_{DateTime.Now:yyyyMMdd_HHmm}", DefaultExt=".xlsx", Filter="Excel Workbook|*.xlsx" };
            if (dlg.ShowDialog()!=true) return;

            if (LoadingOverlay!=null) LoadingOverlay.Visibility=Visibility.Visible;
            try
            {
                string path=dlg.FileName;
                string[] fields  = { "student_id","full_name","level_number","drop_out_date","drop_out_reason","remark" };
                string[] headers = { "Student ID","Full Name","Level","Dropout Year","Reason","Remark" };
                await Task.Run(() =>
                {
                    using var wb=new ClosedXML.Excel.XLWorkbook();
                    var ws=wb.Worksheets.Add("Dropouts");
                    for (int c=0;c<headers.Length;c++)
                    {
                        var cell=ws.Cell(1,c+1); cell.Value=headers[c];
                        cell.Style.Font.Bold=true;
                        cell.Style.Fill.BackgroundColor=ClosedXML.Excel.XLColor.FromHtml("#1A3A6B");
                        cell.Style.Font.FontColor=ClosedXML.Excel.XLColor.White;
                    }
                    int row=2;
                    foreach (DataRowView drv in view)
                    {
                        for (int c=0;c<fields.Length;c++)
                        { try { ws.Cell(row,c+1).Value=drv[fields[c]]?.ToString()??""; } catch { } }
                        row++;
                    }
                    ws.Columns().AdjustToContents();
                    ws.SheetView.FreezeRows(1);
                    wb.SaveAs(path);
                });
                Msg($"Exported {view.Count} records to:\n{path}", true);
            }
            catch (Exception ex) { Msg("Export failed: "+ex.Message, false); }
            finally { if (LoadingOverlay!=null) LoadingOverlay.Visibility=Visibility.Collapsed; }
        }

        private void Msg(string m, bool ok)
        {
            var o=Window.GetWindow(this);
            if (ok) ModernDialog.Show(o,m,"Success",ModernDialog.DialogType.Success);
            else    ModernDialog.Show(o,m,"Error",  ModernDialog.DialogType.Error);
        }
    }
}
