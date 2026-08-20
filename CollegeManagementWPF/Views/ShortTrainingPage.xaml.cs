using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class ShortTrainingPage : Page
    {
        private readonly DBConnect _db = new DBConnect();
        private int _selId = -1;

        private const string CREATE_SQL =
            "CREATE TABLE IF NOT EXISTS ecc_dof_wukrostmarycollege.short_training (" +
            "  id INT AUTO_INCREMENT PRIMARY KEY," +
            "  student_id VARCHAR(50)," +
            "  full_name VARCHAR(150) NOT NULL," +
            "  sex VARCHAR(10)," +
            "  occupational_title VARCHAR(150)," +
            "  entry_year VARCHAR(20)," +
            "  training_round VARCHAR(50)," +
            "  admission_type VARCHAR(30)," +
            "  duration VARCHAR(50)," +
            "  mobile_number VARCHAR(20)" +
            ") ENGINE=InnoDB;";

        private const string BASE =
            "SELECT id, student_id, full_name, sex, occupational_title, entry_year, " +
            "training_round, admission_type, duration, mobile_number " +
            "FROM ecc_dof_wukrostmarycollege.short_training";

        public ShortTrainingPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            ApplyPermissions();
            Loaded += async (s, e) => { await EnsureTableAsync(); await Load(BASE); };
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private void ApplyPermissions()
        {
            if (SessionUser.IsSuperAdmin) return;
            Grid1.Visibility          = SessionUser.Has("short_view")   ? Visibility.Visible : Visibility.Collapsed;
            BtnSave.Visibility        = SessionUser.Has("short_add")    ? Visibility.Visible : Visibility.Collapsed;
            BtnUpdate.Visibility      = SessionUser.Has("short_update") ? Visibility.Visible : Visibility.Collapsed;
            BtnDelete.Visibility      = SessionUser.Has("short_delete") ? Visibility.Visible : Visibility.Collapsed;
            BtnClear.Visibility       = (SessionUser.Has("short_add") || SessionUser.Has("short_update")) ? Visibility.Visible : Visibility.Collapsed;
            BtnPrintReport.Visibility = SessionUser.Has("short_report") ? Visibility.Visible : Visibility.Collapsed;
            BtnExcelReport.Visibility = SessionUser.Has("short_report") ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task EnsureTableAsync()
        {
            try { await Task.Run(() => {
                var c=_db.GetConnection(); c.Open();
                new MySqlCommand(CREATE_SQL,c).ExecuteNonQuery();
                // Add student_id column if it doesn't exist (upgrade existing tables)
                try { new MySqlCommand("ALTER TABLE ecc_dof_wukrostmarycollege.short_training ADD COLUMN student_id VARCHAR(50) AFTER id", c).ExecuteNonQuery(); } catch { }
                c.Close();
            }); }
            catch { }
        }

        private async Task Load(string q)
        {
            try
            {
                if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
                var dt = await Task.Run(() =>
                {
                    var t = new DataTable();
                    new MySqlDataAdapter(q, _db.GetConnection()).Fill(t);
                    t.Columns.Add("_RowNo", typeof(int));
                    for (int i = 0; i < t.Rows.Count; i++) t.Rows[i]["_RowNo"] = i + 1;
                    return t;
                });
                Grid1.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex) { Msg("DB Error: " + ex.Message, false); }
            finally { if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed; }
        }

        private void Grid1_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (Grid1.SelectedItem is not DataRowView r) return;
            _selId = r["id"] != DBNull.Value ? Convert.ToInt32(r["id"]) : -1;
            TxtStudentId.Text    = r["student_id"]?.ToString() ?? "";
            TxtFullName.Text     = r["full_name"]?.ToString() ?? "";
            SetCombo(CmbSex,     r["sex"]?.ToString() ?? "");
            TxtOccupation.Text   = r["occupational_title"]?.ToString() ?? "";
            TxtEntryYear.Text    = r["entry_year"]?.ToString() ?? "";
            TxtTrainingRound.Text= r["training_round"]?.ToString() ?? "";
            SetCombo(CmbAdmType, r["admission_type"]?.ToString() ?? "");
            TxtDuration.Text     = r["duration"]?.ToString() ?? "";
            TxtMobile.Text       = r["mobile_number"]?.ToString() ?? "";
        }

        private void SetCombo(ComboBox c, string v)
        { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }

        private string CmbVal(ComboBox c)
            => (c.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? c.Text?.Trim() ?? "";

        // ── CRUD ──────────────────────────────────────────────────────────────
        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            string sid  = TxtStudentId.Text.Trim();
            string name = TxtFullName.Text.Trim();
            string sex  = CmbVal(CmbSex);
            string occ  = TxtOccupation.Text.Trim();
            string ey   = TxtEntryYear.Text.Trim();
            string tr   = TxtTrainingRound.Text.Trim();
            string at   = CmbVal(CmbAdmType);
            string dur  = TxtDuration.Text.Trim();
            string mob  = TxtMobile.Text.Trim();

            if (string.IsNullOrWhiteSpace(name)) { Msg("Full Name is required.", false); return; }
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.short_training " +
                        "(student_id,full_name,sex,occupational_title,entry_year,training_round,admission_type,duration,mobile_number) " +
                        "VALUES(@si,@n,@sx,@oc,@ey,@tr,@at,@du,@mo)", conn);
                    cmd.Parameters.AddWithValue("@si", sid);
                    cmd.Parameters.AddWithValue("@n",  name);
                    cmd.Parameters.AddWithValue("@sx", sex);
                    cmd.Parameters.AddWithValue("@oc", occ);
                    cmd.Parameters.AddWithValue("@ey", ey);
                    cmd.Parameters.AddWithValue("@tr", tr);
                    cmd.Parameters.AddWithValue("@at", at);
                    cmd.Parameters.AddWithValue("@du", dur);
                    cmd.Parameters.AddWithValue("@mo", mob);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                Msg("Saved!", true); await Load(BASE); Clear();
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if (_selId < 0) { Msg("Select a record first.", false); return; }
            int    id  = _selId;
            string sid = TxtStudentId.Text.Trim();
            string name= TxtFullName.Text.Trim();
            string sex = CmbVal(CmbSex);
            string occ = TxtOccupation.Text.Trim();
            string ey  = TxtEntryYear.Text.Trim();
            string tr  = TxtTrainingRound.Text.Trim();
            string at  = CmbVal(CmbAdmType);
            string dur = TxtDuration.Text.Trim();
            string mob = TxtMobile.Text.Trim();
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE ecc_dof_wukrostmarycollege.short_training SET " +
                        "student_id=@si,full_name=@n,sex=@sx,occupational_title=@oc,entry_year=@ey," +
                        "training_round=@tr,admission_type=@at,duration=@du,mobile_number=@mo " +
                        "WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@si", sid);
                    cmd.Parameters.AddWithValue("@n",  name);
                    cmd.Parameters.AddWithValue("@sx", sex);
                    cmd.Parameters.AddWithValue("@oc", occ);
                    cmd.Parameters.AddWithValue("@ey", ey);
                    cmd.Parameters.AddWithValue("@tr", tr);
                    cmd.Parameters.AddWithValue("@at", at);
                    cmd.Parameters.AddWithValue("@du", dur);
                    cmd.Parameters.AddWithValue("@mo", mob);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                Msg("Updated!", true); await Load(BASE);
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (_selId < 0) { Msg("Select a record first.", false); return; }
            string displayName = TxtFullName.Text;
            var dlg = new ModernDialog($"Delete '{displayName}'?", "Confirm", ModernDialog.DialogType.Warning) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;
            int id = _selId;
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM ecc_dof_wukrostmarycollege.short_training WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                Msg("Deleted!", true); await Load(BASE); Clear();
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, false); }
        }

        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            var conds = new System.Collections.Generic.List<string>();
            string ey   = TxtFYear.Text.Trim();
            string rnd  = TxtFRound.Text.Trim();
            string dur  = TxtFDuration.Text.Trim();
            string occ  = TxtFOccupation.Text.Trim();
            string sex  = (CmbFSex.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string adm  = (CmbFAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string name = TxtFName.Text.Trim();

            if (!string.IsNullOrEmpty(ey))   conds.Add($"entry_year='{ey.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(rnd))  conds.Add($"training_round LIKE '%{rnd.Replace("'","''")}%'");
            if (!string.IsNullOrEmpty(dur))  conds.Add($"duration LIKE '%{dur.Replace("'","''")}%'");
            if (!string.IsNullOrEmpty(occ))  conds.Add($"occupational_title LIKE '%{occ.Replace("'","''")}%'");
            if (!string.IsNullOrEmpty(sex) && sex != "(All)")  conds.Add($"sex='{sex.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(adm) && adm != "(All)") conds.Add($"admission_type='{adm.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(name)) conds.Add($"full_name LIKE '%{name.Replace("'","''")}%'");

            await Load(conds.Count > 0 ? BASE + " WHERE " + string.Join(" AND ", conds) : BASE);
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            TxtFYear.Text = TxtFRound.Text = TxtFDuration.Text = TxtFOccupation.Text = TxtFName.Text = "";
            CmbFSex.SelectedIndex = 0;
            CmbFAdmType.SelectedIndex = 0;
            await Load(BASE);
        }

        private async void TxtFilter_Changed(object s, TextChangedEventArgs e)
        {
            string t = TxtFilter.Text.Trim();
            await Load(string.IsNullOrEmpty(t) ? BASE :
                BASE + $" WHERE full_name LIKE '%{t}%' OR occupational_title LIKE '%{t}%'");
        }

        private async void BtnReset_Click(object s, RoutedEventArgs e)
        { TxtFilter.Text = ""; await Load(BASE); }

        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();

        private void Clear()
        {
            TxtStudentId.Text = TxtFullName.Text = TxtOccupation.Text = TxtEntryYear.Text = "";
            TxtTrainingRound.Text = TxtDuration.Text = TxtMobile.Text = "";
            CmbSex.SelectedIndex = -1; CmbAdmType.SelectedIndex = 0;
            _selId = -1;
        }

        private void Msg(string m, bool ok)
        {
            var o = Window.GetWindow(this);
            if (ok) ModernDialog.Show(o, m, "Success", ModernDialog.DialogType.Success);
            else    ModernDialog.Show(o, m, "Error",   ModernDialog.DialogType.Error);
        }

        // ── Report helpers ────────────────────────────────────────────────────
        private System.Data.DataView? GetCurrentView()
            => Grid1.ItemsSource as System.Data.DataView;

        private (string occ, string ey, string rnd, string adm, string dur) GetFilterSummary()
        {
            // Pull summary values from the current filter fields first,
            // then fall back to actual data if filter fields are blank
            string occ = TxtFOccupation.Text.Trim();
            string ey  = TxtFYear.Text.Trim();
            string rnd = TxtFRound.Text.Trim();
            string adm = (CmbFAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            if (adm == "(All)") adm = "";
            string dur = TxtFDuration.Text.Trim();

            // Fill blanks from actual grid data
            if (GetCurrentView() is { } v && v.Count > 0)
            {
                if (string.IsNullOrEmpty(occ)) occ = v[0]["occupational_title"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(ey))  ey  = v[0]["entry_year"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(rnd)) rnd = v[0]["training_round"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(adm)) adm = v[0]["admission_type"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(dur)) dur = v[0]["duration"]?.ToString() ?? "";
            }
            return (occ, ey, rnd, adm, dur);
        }

        // ── PDF Export ────────────────────────────────────────────────────────
        private async void BtnPrintReport_Click(object sender, RoutedEventArgs e)
        {
            var view = GetCurrentView();
            if (view == null || view.Count == 0) { Msg("Generate/filter data first.", false); return; }
            var dlg = new Microsoft.Win32.SaveFileDialog
            { FileName = $"ShortTrainingReport_{DateTime.Now:yyyyMMdd}", DefaultExt = ".pdf", Filter = "PDF|*.pdf" };
            if (dlg.ShowDialog() != true) return;

            string path = dlg.FileName;
            var (occ, ey, rnd, adm, dur) = GetFilterSummary();
            var rows = new System.Collections.Generic.List<string[]>();
            foreach (System.Data.DataRowView r in view)
                rows.Add(new[]{
                    r["full_name"]?.ToString()??"", r["sex"]?.ToString()??"",
                    r["occupational_title"]?.ToString()??"", r["entry_year"]?.ToString()??"",
                    r["training_round"]?.ToString()??"", r["admission_type"]?.ToString()??"",
                    r["duration"]?.ToString()??"", r["mobile_number"]?.ToString()??""
                });

            try
            {
                await Task.Run(() =>
                {
                    var doc = new MigraDoc.DocumentObjectModel.Document();
                    if (doc.Styles["Normal"] is { } ns) { ns.Font.Name = "Times New Roman"; ns.Font.Size = 10; }
                    var sec = doc.AddSection();
                    sec.PageSetup.PageFormat = MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
                    sec.PageSetup.TopMargin = "1.5cm"; sec.PageSetup.BottomMargin = "1.5cm";
                    sec.PageSetup.LeftMargin = "1.5cm"; sec.PageSetup.RightMargin = "1.5cm";

                    void Hdr(string t, double sz, bool ul = false)
                    {
                        var p = sec.AddParagraph(t);
                        p.Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        p.Format.Font.Bold = true; p.Format.Font.Size = sz;
                        p.Format.Font.Name = "Times New Roman";
                        if (ul) p.Format.Font.Underline = MigraDoc.DocumentObjectModel.Underline.Single;
                        p.Format.SpaceBefore = "0mm"; p.Format.SpaceAfter = "1mm";
                    }
                    Hdr("ECC-DoA  WUKRO ST.MARY COLLEGE", 13);
                    Hdr("REGISTRAR'S OFFICE", 12);
                    Hdr("SUMMARY REPORT AND  LIST NAME  FOR SHORT TRAINING", 11, true);
                    sec.AddParagraph().Format.SpaceAfter = "2mm";

                    // Summary info lines — full width split 50/50
                    void InfoLine(string l1, string v1, string l2, string v2)
                    {
                        var t2 = sec.AddTable(); t2.Borders.Visible = false;
                        t2.AddColumn("12.7cm"); t2.AddColumn("12.7cm");
                        t2.TopPadding = "0.5mm"; t2.BottomPadding = "0.5mm";
                        t2.Format.Font.Size = 10; t2.Format.Font.Name = "Times New Roman";
                        var r2 = t2.AddRow();
                        r2.Cells[0].AddParagraph($"{l1}: __{v1}__");
                        r2.Cells[1].AddParagraph($"{l2}: __{v2}__");
                    }
                    InfoLine("Occupational Title", occ, "Entry Year", ey);
                    InfoLine("Training Round Number", rnd, "Admission Type", adm.Length > 0 ? adm + "  Short Training" : "Short Training");
                    var t3 = sec.AddTable(); t3.Borders.Visible = false; t3.AddColumn("25.4cm");
                    t3.TopPadding = "0.5mm"; t3.BottomPadding = "0.5mm"; t3.Format.Font.Size = 10; t3.Format.Font.Name = "Times New Roman";
                    t3.AddRow().Cells[0].AddParagraph($"Durations of Training: __{dur}__");
                    sec.AddParagraph().Format.SpaceAfter = "3mm";

                    // Data table — full A4 landscape width (~25.4cm usable)
                    var tbl = sec.AddTable();
                    tbl.Borders.Width = 0.5; tbl.Borders.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    tbl.Format.Font.Size = 9; tbl.Format.Font.Name = "Times New Roman";
                    tbl.TopPadding = "1mm"; tbl.BottomPadding = "1mm";
                    // Total width = 25.4cm  (A4 landscape 297mm - 15mm*2 margins)
                    tbl.AddColumn("1.0cm");  // NO
                    tbl.AddColumn("5.8cm");  // Name
                    tbl.AddColumn("1.0cm");  // Sex
                    tbl.AddColumn("4.5cm");  // Occupational Title
                    tbl.AddColumn("2.0cm");  // Entry Year
                    tbl.AddColumn("2.2cm");  // Training Round
                    tbl.AddColumn("2.4cm");  // Admission Type
                    tbl.AddColumn("3.0cm");  // Duration
                    tbl.AddColumn("3.5cm");  // Mobile

                    var hdr = tbl.AddRow(); hdr.HeadingFormat = true;
                    hdr.Format.Font.Bold = true; hdr.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    hdr.Shading.Color = new MigraDoc.DocumentObjectModel.Color(220, 220, 220);
                    string[] hdrs = { "NO", "Name of Students", "Sex", "Occupational Title", "Entry year", "Training\nRound No.", "Admission\nType", "Durations\nof Training", "Mobile\nNumber" };
                    for (int ci = 0; ci < hdrs.Length; ci++)
                    { hdr.Cells[ci].AddParagraph(hdrs[ci]); hdr.Cells[ci].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center; }

                    for (int i = 0; i < rows.Count; i++)
                    {
                        var row = tbl.AddRow(); row.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row.Cells[0].AddParagraph((i + 1).ToString()); row.Cells[0].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[1].AddParagraph(rows[i][0]); row.Cells[1].Format.Font.Bold = true;
                        row.Cells[2].AddParagraph(rows[i][1].Length > 0 ? rows[i][1][0].ToString().ToUpper() : ""); row.Cells[2].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        for (int ci = 2; ci < 8; ci++) row.Cells[ci + 1].AddParagraph(rows[i][ci]);
                    }

                    var ren = new MigraDoc.Rendering.PdfDocumentRenderer { Document = doc };
                    ren.RenderDocument(); ren.PdfDocument.Save(path);
                });
                Msg("PDF report saved!", true);
            }
            catch (Exception ex) { Msg("PDF failed: " + ex.Message, false); }
        }

        // ── Excel Export ──────────────────────────────────────────────────────
        private async void BtnExcelReport_Click(object sender, RoutedEventArgs e)
        {
            var view = GetCurrentView();
            if (view == null || view.Count == 0) { Msg("Generate/filter data first.", false); return; }
            var dlg = new Microsoft.Win32.SaveFileDialog
            { FileName = $"ShortTrainingReport_{DateTime.Now:yyyyMMdd}", DefaultExt = ".xlsx", Filter = "Excel Workbook|*.xlsx" };
            if (dlg.ShowDialog() != true) return;

            string path = dlg.FileName;
            var (occ, ey, rnd, adm, dur) = GetFilterSummary();
            var rows = new System.Collections.Generic.List<string[]>();
            foreach (System.Data.DataRowView r in view)
                rows.Add(new[]{
                    r["full_name"]?.ToString()??"", r["sex"]?.ToString()??"",
                    r["occupational_title"]?.ToString()??"", r["entry_year"]?.ToString()??"",
                    r["training_round"]?.ToString()??"", r["admission_type"]?.ToString()??"",
                    r["duration"]?.ToString()??"", r["mobile_number"]?.ToString()??""
                });

            try
            {
                await Task.Run(() =>
                {
                    using var wb = new ClosedXML.Excel.XLWorkbook();
                    var ws = wb.Worksheets.Add("Short Training Report");
                    int cols = 9;

                    void Mg(int row, string val, bool bold = false, int fs = 11)
                    {
                        ws.Range(row, 1, row, cols).Merge();
                        ws.Cell(row, 1).Value = val;
                        ws.Cell(row, 1).Style.Font.Bold = bold;
                        ws.Cell(row, 1).Style.Font.FontSize = fs;
                        ws.Cell(row, 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    }

                    int rn = 1;
                    Mg(rn, "ECC-DoA  WUKRO ST.MARY COLLEGE", true, 13); rn++;
                    Mg(rn, "REGISTRAR'S OFFICE", true, 12); rn++;
                    Mg(rn, "SUMMARY REPORT AND  LIST NAME  FOR SHORT TRAINING", true, 11);
                    ws.Cell(rn, 1).Style.Font.Underline = ClosedXML.Excel.XLFontUnderlineValues.Single; rn++; rn++;

                    // Info rows
                    ws.Cell(rn, 1).Value = $"Occupational Title: {occ}"; ws.Range(rn, 1, rn, 5).Merge();
                    ws.Cell(rn, 6).Value = $"Entry Year: {ey}"; ws.Range(rn, 6, rn, cols).Merge(); rn++;
                    ws.Cell(rn, 1).Value = $"Training Round Number: {rnd}"; ws.Range(rn, 1, rn, 5).Merge();
                    ws.Cell(rn, 6).Value = $"Admission Type: {(adm.Length > 0 ? adm + "  Short Training" : "Short Training")}"; ws.Range(rn, 6, rn, cols).Merge(); rn++;
                    ws.Cell(rn, 1).Value = $"Durations of Training: {dur}"; ws.Range(rn, 1, rn, cols).Merge(); rn++; rn++;

                    // Table header
                    string[] hdrs = { "NO", "Name of Students", "Sex", "Occupational Title", "Entry year", "Training Round No.", "Admission Type", "Durations of Training", "Mobile Number" };
                    for (int ci = 0; ci < hdrs.Length; ci++)
                    {
                        ws.Cell(rn, ci + 1).Value = hdrs[ci];
                        ws.Cell(rn, ci + 1).Style.Font.Bold = true;
                        ws.Cell(rn, ci + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#D9E1F2");
                        ws.Cell(rn, ci + 1).Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        ws.Cell(rn, ci + 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        ws.Cell(rn, ci + 1).Style.Alignment.WrapText = true;
                    }
                    ws.Row(rn).Height = 38; rn++;

                    // Data rows
                    for (int i = 0; i < rows.Count; i++)
                    {
                        ws.Cell(rn, 1).Value = i + 1;
                        ws.Cell(rn, 2).Value = rows[i][0]; ws.Cell(rn, 2).Style.Font.Bold = true;
                        ws.Cell(rn, 3).Value = rows[i][1].Length > 0 ? rows[i][1][0].ToString().ToUpper() : "";
                        ws.Cell(rn, 4).Value = rows[i][2]; ws.Cell(rn, 5).Value = rows[i][3];
                        ws.Cell(rn, 6).Value = rows[i][4]; ws.Cell(rn, 7).Value = rows[i][5];
                        ws.Cell(rn, 8).Value = rows[i][6]; ws.Cell(rn, 9).Value = rows[i][7];
                        for (int ci = 1; ci <= cols; ci++)
                            ws.Cell(rn, ci).Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        rn++;
                    }

                    ws.Column(1).Width = 5; ws.Column(2).Width = 25; ws.Column(3).Width = 5;
                    ws.Column(4).Width = 20; ws.Column(5).Width = 10; ws.Column(6).Width = 12;
                    ws.Column(7).Width = 14; ws.Column(8).Width = 14; ws.Column(9).Width = 14;
                    wb.SaveAs(path);
                });
                Msg("Excel report saved!", true);
            }
            catch (Exception ex) { Msg("Excel failed: " + ex.Message, false); }
        }
    }
}
