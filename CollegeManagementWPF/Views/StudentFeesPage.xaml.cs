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
            "SELECT sf.student_id, " +
            "CONCAT(TRIM(sp.first_name),' ',TRIM(sp.father_name),' ',TRIM(sp.grand_father_name)) AS full_name, " +
            "sf.level,sf.academic_year,sf.month,sf.amount,sf.cash_receipt_voucher,sf.remark " +
            "FROM ecc_dof_wukrostmarycollege.student_fee sf " +
            "LEFT JOIN (SELECT TRIM(student_id) AS student_id, dept_id, first_name, father_name, grand_father_name " +
            "FROM ecc_dof_wukrostmarycollege.student_profile GROUP BY TRIM(student_id)) sp " +
            "ON TRIM(sf.student_id)=sp.student_id";

        public StudentFeesPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            ApplyPermissions();
            Loaded += async (s, e) =>
            {
                await LoadDepartments();
                await Load(Q);
            };
        }

        private async Task LoadDepartments()
        {
            try
            {
                var depts = await Task.Run(() =>
                {
                    var list = new System.Collections.Generic.List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT dept_id FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close(); return list;
                });
                CmbFDeptID.Items.Clear();
                foreach (var d in depts)
                    CmbFDeptID.Items.Add(new ComboBoxItem { Content = d });
            }
            catch { }
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

        private void ApplyPermissions()
        {
            if (SessionUser.IsSuperAdmin) return;
            Grid1.Visibility     = SessionUser.Has("fees_view")   ? Visibility.Visible : Visibility.Collapsed;
            BtnSave.Visibility   = SessionUser.Has("fees_add")    ? Visibility.Visible : Visibility.Collapsed;
            BtnUpdate.Visibility = SessionUser.Has("fees_update") ? Visibility.Visible : Visibility.Collapsed;
            BtnDelete.Visibility = SessionUser.Has("fees_delete") ? Visibility.Visible : Visibility.Collapsed;
            BtnClear.Visibility  = (SessionUser.Has("fees_add") || SessionUser.Has("fees_update")) ? Visibility.Visible : Visibility.Collapsed;
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
            await Load(string.IsNullOrEmpty(t) ? Q : Q + $" WHERE TRIM(sf.student_id) LIKE '%{t.Replace("'","''")}%' OR CONCAT(TRIM(sp.first_name),' ',TRIM(sp.father_name),' ',TRIM(sp.grand_father_name)) LIKE '%{t.Replace("'","''")}%'");
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
            string sid   = TxtFStudID.Text.Trim();
            string dept  = (CmbFDeptID.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? CmbFDeptID.Text?.Trim() ?? "";
            string lvl   = (CmbFLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string mo    = (CmbFMonth.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string ay    = TxtFYear.Text.Trim();

            if (!string.IsNullOrEmpty(sid))
            {
                await Load(Q + $" WHERE TRIM(sf.student_id)='{sid.Replace("'","''")}'");
            }
            else
            {
                var conditions = new System.Collections.Generic.List<string>();
                if (!string.IsNullOrEmpty(dept)) conditions.Add($"sp.dept_id='{dept.Replace("'","''")}'");
                if (!string.IsNullOrEmpty(ay))   conditions.Add($"sf.academic_year='{ay.Replace("'","''")}'");
                if (!string.IsNullOrEmpty(lvl))  conditions.Add($"sf.level='{lvl.Replace("'","''")}'");
                if (!string.IsNullOrEmpty(mo))   conditions.Add($"sf.month='{mo.Replace("'","''")}'");

                if (conditions.Count == 0)
                    await Load(Q);
                else
                    await Load(Q + " WHERE " + string.Join(" AND ", conditions));
            }
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            TxtFStudID.Text = TxtFYear.Text = "";
            CmbFDeptID.SelectedIndex = -1; CmbFDeptID.Text = "";
            CmbFLevel.SelectedIndex = 0;
            CmbFMonth.SelectedIndex = 0;
            await Load(Q);
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not System.Data.DataView view || view.Count == 0)
            { Msg("No data to export.", false); return; }

            var saveDlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"StudentFees_{DateTime.Now:yyyyMMdd_HHmm}",
                DefaultExt = ".pdf", Filter = "PDF File|*.pdf"
            };
            if (saveDlg.ShowDialog() != true) return;

            if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
            await Task.Delay(50);
            try
            {
                string path = saveDlg.FileName;
                string[] fields  = { "student_id","full_name","level","academic_year","month","amount","cash_receipt_voucher","remark" };
                string[] headers = { "Student ID","Full Name","Level","Acad Year","Month","Amount","CRV","Remark" };

                await Task.Run(() =>
                {
                    // Snapshot data
                    var rows = new System.Collections.Generic.List<string[]>();
                    foreach (System.Data.DataRowView drv in view)
                        rows.Add(System.Array.ConvertAll(fields, f => { try { return drv[f]?.ToString() ?? ""; } catch { return ""; } }));

                    // Build MigraDoc document
                    var doc = new MigraDoc.DocumentObjectModel.Document();
                    doc.Info.Title = "Student Fees List";

                    var section = doc.AddSection();
                    section.PageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
                    section.PageSetup.PageFormat   = MigraDoc.DocumentObjectModel.PageFormat.A4;
                    section.PageSetup.TopMargin    = "1.5cm";
                    section.PageSetup.BottomMargin = "1.5cm";
                    section.PageSetup.LeftMargin   = "1.5cm";
                    section.PageSetup.RightMargin  = "1.5cm";

                    // Title
                    var title = section.AddParagraph("Wukro St. Mary College");
                    title.Format.Font.Size = 16; title.Format.Font.Bold = true;
                    title.Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    var sub = section.AddParagraph("Student Fees List");
                    sub.Format.Font.Size = 12;
                    sub.Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    var date = section.AddParagraph($"Generated: {DateTime.Now:dd MMM yyyy  HH:mm}");
                    date.Format.Font.Size = 8; date.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                    date.Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    date.Format.SpaceAfter = "6pt";

                    // Table
                    var table = section.AddTable();
                    table.Borders.Width = 0.25;
                    table.Borders.Color = MigraDoc.DocumentObjectModel.Colors.LightGray;

                    double[] widths = { 3.0, 4.5, 1.0, 2.0, 2.0, 2.0, 2.5, 3.0 };
                    foreach (var w in widths) { var col = table.AddColumn($"{w}cm"); col.Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Left; }

                    // Header row
                    var hRow = table.AddRow();
                    hRow.Shading.Color = new MigraDoc.DocumentObjectModel.Color(18, 52, 116);
                    for (int c = 0; c < headers.Length; c++)
                    {
                        hRow.Cells[c].AddParagraph(headers[c]).Format.Font.Bold = true;
                        hRow.Cells[c].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        hRow.Cells[c].Format.Font.Size = 8;
                        hRow.Cells[c].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    }

                    // Data rows
                    bool alt = false;
                    foreach (var cols in rows)
                    {
                        var row = table.AddRow();
                        if (alt) row.Shading.Color = new MigraDoc.DocumentObjectModel.Color(245, 247, 250);
                        alt = !alt;
                        for (int c = 0; c < cols.Length; c++)
                        {
                            row.Cells[c].AddParagraph(cols[c]).Format.Font.Size = 8;
                            row.Cells[c].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        }
                    }

                    // Render to PDF
                    var renderer = new MigraDoc.Rendering.PdfDocumentRenderer { Document = doc };
                    renderer.RenderDocument();
                    renderer.PdfDocument.Save(path);
                });

                Msg($"PDF saved to:\n{path}", true);
            }
            catch (Exception ex) { Msg("PDF export failed: " + ex.Message, false); }
            finally { if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed; }
        }

        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not System.Data.DataView view || view.Count == 0)
            { Msg("No data to export.", false); return; }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"StudentFees_{DateTime.Now:yyyyMMdd_HHmm}",
                DefaultExt = ".xlsx", Filter = "Excel Workbook|*.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
            try
            {
                string path = dlg.FileName;
                string[] fields  = { "student_id","full_name","level","academic_year","month","amount","cash_receipt_voucher","remark" };
                string[] headers = { "Student ID","Full Name","Level","Academic Year","Month","Amount","CRV","Remark" };
                await Task.Run(() =>
                {
                    using var wb = new ClosedXML.Excel.XLWorkbook();
                    var ws = wb.Worksheets.Add("Fees");
                    for (int c = 0; c < headers.Length; c++)
                    {
                        var cell = ws.Cell(1, c + 1);
                        cell.Value = headers[c];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#1A3A6B");
                        cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                    }
                    int row = 2;
                    foreach (System.Data.DataRowView drv in view)
                    {
                        for (int c = 0; c < fields.Length; c++)
                        { try { ws.Cell(row, c + 1).Value = drv[fields[c]]?.ToString() ?? ""; } catch { } }
                        row++;
                    }
                    ws.Columns().AdjustToContents();
                    ws.SheetView.FreezeRows(1);
                    wb.SaveAs(path);
                });
                Msg($"Exported {view.Count} records to:\n{path}", true);
            }
            catch (Exception ex) { Msg("Export failed: " + ex.Message, false); }
            finally { if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed; }
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
