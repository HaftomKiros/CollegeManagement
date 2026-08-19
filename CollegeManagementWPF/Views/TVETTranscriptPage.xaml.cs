using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class TVETTranscriptPage : Page
    {
        private readonly DBConnect _db = new DBConnect();

        // All student IDs loaded once on startup
        private List<string> _allStudentIds = new();

        // Suppress cascading re-entrancy
        private bool _suppress = false;

        public TVETTranscriptPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadStudentIdsAsync();
        }

        // ── Load all student IDs into the searchable dropdown ────────────────
        private async Task LoadStudentIdsAsync()
        {
            try
            {
                _allStudentIds = await Task.Run(() =>
                {
                    var list = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT DISTINCT TRIM(student_id) FROM ecc_dof_wukrostmarycollege.student_mark " +
                        "ORDER BY TRIM(student_id)", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        var v = r[0]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(v)) list.Add(v);
                    }
                    conn.Close();
                    return list;
                });

                RefreshStudentDropdown("");
            }
            catch { /* DB offline — skip */ }
        }

        // Populate CmbStudID items filtered by typed text
        private void RefreshStudentDropdown(string filter)
        {
            _suppress = true;
            CmbStudID.Items.Clear();
            foreach (var id in _allStudentIds)
                if (string.IsNullOrEmpty(filter) || id.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    CmbStudID.Items.Add(new ComboBoxItem { Content = id });
            _suppress = false;
        }

        // ── Student ID text changed — filter dropdown list ───────────────────
        private void CmbStudID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            string typed = CmbStudID.Text?.Trim() ?? "";
            RefreshStudentDropdown(typed);

            // Open dropdown to show matches if there's something typed
            if (!string.IsNullOrEmpty(typed) && CmbStudID.Items.Count > 0)
                CmbStudID.IsDropDownOpen = true;

            // Clear cascades and name until a valid ID is committed
            CmbLevel.Items.Clear();
            CmbAcadYear.Items.Clear();
            TxtStudentName.Visibility = Visibility.Collapsed;
        }

        // ── Focus-out: resolve student ID, show name, cascade Level ──────────
        private async void CmbStudID_LostFocus(object sender, RoutedEventArgs e)
        {
            string sid = GetStudentId();
            if (string.IsNullOrEmpty(sid)) return;

            await LoadStudentNameAsync(sid);
            await LoadLevelsAsync(sid);
        }

        // ── Level changed — reload Academic Years for this student + level ───
        private async void CmbLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string sid = GetStudentId();
            string lvl = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(lvl)) return;
            await LoadAcadYearsAsync(sid, lvl);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private string GetStudentId()
        {
            // Prefer selected item; fall back to typed text
            if (CmbStudID.SelectedItem is ComboBoxItem sel)
                return sel.Content?.ToString()?.Trim() ?? "";
            return CmbStudID.Text?.Trim() ?? "";
        }

        private async Task LoadStudentNameAsync(string sid)
        {
            try
            {
                string name = await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT TRIM(CONCAT(IFNULL(first_name,''), ' ', " +
                        "IFNULL(father_name,''), ' ', IFNULL(grand_father_name,''))) " +
                        "FROM ecc_dof_wukrostmarycollege.student_profile " +
                        "WHERE TRIM(student_id)=@s LIMIT 1", conn);
                    cmd.Parameters.AddWithValue("@s", sid);
                    var result = cmd.ExecuteScalar()?.ToString()?.Trim() ?? "";
                    conn.Close();
                    return result;
                });

                if (!string.IsNullOrEmpty(name))
                {
                    TxtStudentName.Text = name;
                    TxtStudentName.Visibility = Visibility.Visible;
                }
                else
                {
                    TxtStudentName.Visibility = Visibility.Collapsed;
                }
            }
            catch { TxtStudentName.Visibility = Visibility.Collapsed; }
        }

        private async Task LoadLevelsAsync(string sid)
        {
            try
            {
                var levels = await Task.Run(() =>
                {
                    var list = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT DISTINCT level FROM ecc_dof_wukrostmarycollege.student_mark " +
                        "WHERE TRIM(student_id)=@s ORDER BY level", conn);
                    cmd.Parameters.AddWithValue("@s", sid);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close();
                    return list;
                });

                _suppress = true;
                CmbLevel.Items.Clear();
                foreach (var l in levels)
                    CmbLevel.Items.Add(new ComboBoxItem { Content = l });
                if (CmbLevel.Items.Count > 0) CmbLevel.SelectedIndex = 0;
                _suppress = false;

                // Trigger year load for the auto-selected level
                string lvl = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                if (!string.IsNullOrEmpty(lvl))
                    await LoadAcadYearsAsync(sid, lvl);
            }
            catch { /* skip */ }
        }

        private async Task LoadAcadYearsAsync(string sid, string level)
        {
            try
            {
                var years = await Task.Run(() =>
                {
                    var list = new List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT DISTINCT academic_year FROM ecc_dof_wukrostmarycollege.student_mark " +
                        "WHERE TRIM(student_id)=@s AND level=@l ORDER BY academic_year", conn);
                    cmd.Parameters.AddWithValue("@s", sid);
                    cmd.Parameters.AddWithValue("@l", level);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close();
                    return list;
                });

                CmbAcadYear.Items.Clear();
                foreach (var y in years)
                    CmbAcadYear.Items.Add(new ComboBoxItem { Content = y });
                if (CmbAcadYear.Items.Count > 0) CmbAcadYear.SelectedIndex = 0;
            }
            catch { /* skip */ }
        }

        // ── Generate ─────────────────────────────────────────────────────────
        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            string sid = GetStudentId();
            if (string.IsNullOrEmpty(sid))
            {
                ModernDialog.Show(Window.GetWindow(this), "Please select a Student ID!", "Error", ModernDialog.DialogType.Error);
                return;
            }

            string lvl = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string ay  = (CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            try
            {
                var dt = new DataTable();
                string sql =
                    "SELECT student_id,level,module_code,employee_id,academic_year," +
                    "score_of_knowledge_test,score_of_practical_test,competence " +
                    "FROM ecc_dof_wukrostmarycollege.student_mark " +
                    "WHERE TRIM(student_id)=@s";
                if (!string.IsNullOrEmpty(lvl)) sql += " AND level=@l";
                if (!string.IsNullOrEmpty(ay))  sql += " AND academic_year=@y";

                await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    var cmd  = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@s", sid);
                    if (!string.IsNullOrEmpty(lvl)) cmd.Parameters.AddWithValue("@l", lvl);
                    if (!string.IsNullOrEmpty(ay))  cmd.Parameters.AddWithValue("@y", ay);
                    new MySqlDataAdapter(cmd).Fill(dt);
                });

                Grid1.ItemsSource    = dt.DefaultView;
                PreviewCard.Visibility = Visibility.Visible;
                TxtPreviewInfo.Text  =
                    $"Transcript: {sid}" +
                    (string.IsNullOrEmpty(lvl) ? "" : $" | Level {lvl}") +
                    (string.IsNullOrEmpty(ay)  ? "" : $" | Year {ay}") +
                    $" — {dt.Rows.Count} record(s)";
            }
            catch (Exception ex)
            {
                ModernDialog.Show(Window.GetWindow(this), "Error: " + ex.Message, "DB Error", ModernDialog.DialogType.Error);
            }
        }

        // ── Print to PDF — matches the official transcript layout ────────────
        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            string sid = GetStudentId();
            string lvl = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string ay  = (CmbAcadYear.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            if (string.IsNullOrEmpty(sid))
            { ModernDialog.Show(Window.GetWindow(this), "Please select a Student ID first.", "Info", ModernDialog.DialogType.Info); return; }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName   = $"Transcript_{sid.Replace("/","_").Replace("\\","_")}",
                DefaultExt = ".pdf",
                Filter     = "PDF|*.pdf"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                string path = dlg.FileName;

                // ── Fetch student profile ────────────────────────────────────
                string fName = "", fatherName = "", gfName = "", gender = "",
                       deptId = "", deptName = "", streamId = "", admType = "", admDate = "";
                string streamName = "";

                // ── Fetch mark rows with unit_of_competence_title from courses
                var markRows = new List<(string ModCode, string UnitTitle, string KT, string PT, string Comp)>();

                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();

                    // Student profile
                    using (var cmd = new MySqlCommand(
                        "SELECT first_name,father_name,grand_father_name,gender," +
                        "dept_id,stream_id,admission_type,admission_date " +
                        "FROM ecc_dof_wukrostmarycollege.student_profile " +
                        "WHERE TRIM(student_id)=@s AND level=@l LIMIT 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@s", sid);
                        cmd.Parameters.AddWithValue("@l", lvl);
                        using var r = cmd.ExecuteReader();
                        if (r.Read())
                        {
                            fName      = r["first_name"]?.ToString()?.Trim()       ?? "";
                            fatherName = r["father_name"]?.ToString()?.Trim()      ?? "";
                            gfName     = r["grand_father_name"]?.ToString()?.Trim()    ?? "";
                            gender     = r["gender"]?.ToString()?.Trim()           ?? "";
                            deptId     = r["dept_id"]?.ToString()?.Trim()          ?? "";
                            streamId   = r["stream_id"]?.ToString()?.Trim()        ?? "";
                            admType    = r["admission_type"]?.ToString()?.Trim()   ?? "";
                            admDate    = r["admission_date"]?.ToString()?.Trim()   ?? "";
                        }
                    }

                    // Stream name (occupational title) + dept name
                    if (!string.IsNullOrEmpty(streamId))
                    {
                        using var cmd2 = new MySqlCommand(
                            "SELECT stream_name FROM ecc_dof_wukrostmarycollege.streams " +
                            "WHERE stream_id=@s LIMIT 1", conn);
                        cmd2.Parameters.AddWithValue("@s", streamId);
                        streamName = cmd2.ExecuteScalar()?.ToString()?.Trim() ?? streamId;
                    }

                    if (!string.IsNullOrEmpty(deptId))
                    {
                        using var cmd4 = new MySqlCommand(
                            "SELECT dept_name FROM ecc_dof_wukrostmarycollege.departments " +
                            "WHERE dept_id=@d LIMIT 1", conn);
                        cmd4.Parameters.AddWithValue("@d", deptId);
                        deptName = cmd4.ExecuteScalar()?.ToString()?.Trim() ?? deptId;
                    }

                    // Marks joined with courses for unit title
                    string sql =
                        "SELECT sm.module_code, IFNULL(c.unit_of_competence_title,'') AS unit_title," +
                        "sm.score_of_knowledge_test, sm.score_of_practical_test, sm.competence " +
                        "FROM ecc_dof_wukrostmarycollege.student_mark sm " +
                        "LEFT JOIN ecc_dof_wukrostmarycollege.courses c ON sm.module_code=c.module_code " +
                        "WHERE TRIM(sm.student_id)=@s AND sm.level=@l";
                    if (!string.IsNullOrEmpty(ay)) sql += " AND sm.academic_year=@y";
                    sql += " ORDER BY sm.module_code";

                    using var cmd3 = new MySqlCommand(sql, conn);
                    cmd3.Parameters.AddWithValue("@s", sid);
                    cmd3.Parameters.AddWithValue("@l", lvl);
                    if (!string.IsNullOrEmpty(ay)) cmd3.Parameters.AddWithValue("@y", ay);
                    using var r3 = cmd3.ExecuteReader();
                    while (r3.Read())
                        markRows.Add((
                            r3["module_code"]?.ToString()?.Trim()              ?? "",
                            r3["unit_title"]?.ToString()?.Trim()               ?? "",
                            r3["score_of_knowledge_test"]?.ToString()?.Trim()  ?? "",
                            r3["score_of_practical_test"]?.ToString()?.Trim()  ?? "",
                            r3["competence"]?.ToString()?.Trim()               ?? ""
                        ));

                    conn.Close();
                });

                if (markRows.Count == 0)
                { ModernDialog.Show(Window.GetWindow(this), "No mark records found for this selection.", "Info", ModernDialog.DialogType.Info); return; }

                string fullName = $"{fName} {fatherName} {gfName}".Trim();
                double ktTotal = 0, ptTotal = 0;
                foreach (var row in markRows)
                { if (double.TryParse(row.KT, out double k)) ktTotal += k; if (double.TryParse(row.PT, out double p)) ptTotal += p; }
                double ktAvg = markRows.Count > 0 ? ktTotal / markRows.Count : 0;
                double ptAvg = markRows.Count > 0 ? ptTotal / markRows.Count : 0;

                await Task.Run(() =>
                {
                    var doc = new MigraDoc.DocumentObjectModel.Document();
                    // Set Times New Roman as the document-wide default font
                    if (doc.Styles["Normal"]   is { } ns2) ns2.Font.Name = "Times New Roman";
                    if (doc.Styles["Heading1"] is { } ns3) ns3.Font.Name = "Times New Roman";
                    var sec = doc.AddSection();
                    sec.PageSetup.PageFormat  = MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait;
                    sec.PageSetup.TopMargin   = "1.8cm";
                    sec.PageSetup.BottomMargin = "1.8cm";
                    sec.PageSetup.LeftMargin  = "1.8cm";
                    sec.PageSetup.RightMargin = "1.8cm";

                    void CentBold(string text, double size)
                    {
                        var p = sec.AddParagraph(text);
                        p.Format.Alignment   = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        p.Format.Font.Bold   = true;
                        p.Format.Font.Size   = size;
                        p.Format.Font.Name   = "Times New Roman";
                        p.Format.SpaceBefore = "0.5mm";
                        p.Format.SpaceAfter  = "0.5mm";
                    }

                    // ── Header ───────────────────────────────────────────────
                    CentBold("ETHIOPIAN CATHOLIC CHURCH DIOCESE OF ADIGRAT", 13);
                    CentBold("WUKRO ST. MARY'S COLLEGE", 13);
                    CentBold("REGISTRAR'S OFFICE", 13);
                    CentBold("STUDENT RECORDS", 13);
                    CentBold("ATTENDED TRAINING SUCCESSFULLY IN THE FOLLOWING COMPETENCES", 12);

                    sec.AddParagraph().Format.SpaceAfter = "2mm";

                    // ── Student info block ────────────────────────────────────
                    void InfoLine(string label, string value, bool bold = false)
                    {
                        var p = sec.AddParagraph();
                        p.Format.Font.Size   = 10.5;
                        p.Format.Font.Name   = "Times New Roman";
                        p.Format.SpaceBefore = "0.3mm";
                        p.Format.SpaceAfter  = "0.3mm";
                        p.AddFormattedText(label, MigraDoc.DocumentObjectModel.TextFormat.NotBold);
                        var v = p.AddFormattedText(value, bold ? MigraDoc.DocumentObjectModel.TextFormat.Bold : MigraDoc.DocumentObjectModel.TextFormat.NotBold);
                        if (bold) v.Underline = MigraDoc.DocumentObjectModel.Underline.Single;
                    }

                    // Convert numeric level to Roman numeral
                    string lvlRoman = lvl switch { "1" => "I", "2" => "II", "3" => "III", "4" => "IV", "5" => "V", _ => lvl };

                    InfoLine("Occupational Title: ", streamName, true);
                    InfoLine("Level: ", lvlRoman, true);
                    InfoLine("Name of Trainee: ", fullName, true);
                    InfoLine("Department: ", deptName);
                    InfoLine("Student ID No.: ", sid);
                    InfoLine("Gender: ", gender);
                    InfoLine("Admission Type: ", admType);
                    InfoLine("Academic Year: ", (string.IsNullOrEmpty(ay) ? admDate : ay) + " E.C.");

                    sec.AddParagraph().Format.SpaceAfter = "3mm";

                    // ── Table ─────────────────────────────────────────────────
                    var tbl = sec.AddTable();
                    tbl.Borders.Width = 0.5;
                    tbl.Borders.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    tbl.Format.Font.Size = 9;
                    tbl.Format.Font.Name = "Times New Roman";

                    // Column widths to fill A4 portrait (~17cm content width)
                    tbl.AddColumn("1.0cm");  // No.
                    tbl.AddColumn("3.2cm");  // Module Code
                    tbl.AddColumn("7.0cm");  // Unit of Competency
                    tbl.AddColumn("2.0cm");  // KT Score
                    tbl.AddColumn("2.0cm");  // PT Score
                    tbl.AddColumn("2.3cm");  // Competency

                    // Header row
                    var hdr = tbl.AddRow();
                    hdr.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                    hdr.Format.Font.Bold = true;
                    string[] hdrs = { "No.", "Module Code", "Unit of Competency", "Score of\nKT(100%)", "Score of\nPT(100%)", "Competency" };
                    for (int c = 0; c < hdrs.Length; c++)
                    {
                        hdr.Cells[c].AddParagraph(hdrs[c]);
                        hdr.Cells[c].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        hdr.Cells[c].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    }

                    // Data rows
                    for (int i = 0; i < markRows.Count; i++)
                    {
                        var (mc, unit, kt, pt, comp) = markRows[i];
                        var row = tbl.AddRow();
                        row.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row.Cells[0].AddParagraph((i + 1).ToString());
                        row.Cells[0].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[1].AddParagraph(mc);
                        row.Cells[2].AddParagraph(unit);
                        row.Cells[3].AddParagraph(kt);
                        row.Cells[3].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[4].AddParagraph(pt);
                        row.Cells[4].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[5].AddParagraph(comp);
                        row.Cells[5].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    }

                    sec.AddParagraph().Format.SpaceAfter = "3mm";

                    // ── Totals / Averages ─────────────────────────────────────
                    var totals = sec.AddParagraph();
                    totals.Format.Font.Size = 9;
                    totals.AddText($"Score of KT Total = {ktTotal}");
                    totals.AddTab(); totals.AddTab(); totals.AddTab();
                    totals.AddText($"Score of PT Total = {ptTotal}");

                    var avgs = sec.AddParagraph();
                    avgs.Format.Font.Size = 9;
                    avgs.Format.FirstLineIndent = "3cm";
                    avgs.AddText($"Average = {ktAvg:F5}");
                    avgs.AddTab(); avgs.AddTab(); avgs.AddTab();
                    avgs.AddText($"Average = {ptAvg:F5}");

                    sec.AddParagraph().Format.SpaceAfter = "4mm";

                    var note = sec.AddParagraph("Note: KT = Knowledge Test     PT = Practical Test");
                    note.Format.Font.Size = 9;

                    // Spacer
                    sec.AddParagraph().Format.SpaceAfter = "20mm";

                    // ── Signature line ────────────────────────────────────────
                    var sigTbl = sec.AddTable();
                    sigTbl.Borders.Width = 0;
                    sigTbl.AddColumn("9cm");
                    sigTbl.AddColumn("8.5cm");
                    var sigRow = sigTbl.AddRow();
                    sigRow.Cells[0].AddParagraph("Registrar Signature: _______________");
                    sigRow.Cells[0].Format.Font.Size = 9;
                    sigRow.Cells[1].AddParagraph("Date of Issue: _______________");
                    sigRow.Cells[1].Format.Font.Size = 9;
                    sigRow.Cells[1].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;

                    var renderer = new MigraDoc.Rendering.PdfDocumentRenderer { Document = doc };
                    renderer.RenderDocument();
                    renderer.PdfDocument.Save(path);
                });

                ModernDialog.Show(Window.GetWindow(this), "PDF saved!", "Success", ModernDialog.DialogType.Success);
            }
            catch (Exception ex)
            {
                ModernDialog.Show(Window.GetWindow(this), "PDF failed: " + ex.Message, "Error", ModernDialog.DialogType.Error);
            }
        }
    }
}
