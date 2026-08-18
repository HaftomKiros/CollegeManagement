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
    public partial class AttendanceSheetPage : Page
    {
        private readonly DBConnect _db = new DBConnect();
        private bool _suppress = false;
        private List<(string Id, string Name)>     _allDepts    = new();
        private List<(string Id, string Name)>     _allStreams   = new();
        private List<(string LevelId, string Num)> _streamLevels = new();

        public AttendanceSheetPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            Loaded += async (s, e) => await LoadDeptsAsync();
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private async Task LoadDeptsAsync()
        {
            try
            {
                _allDepts = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT dept_id, IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close(); return list;
                });
                RefreshDeptDropdown("");
            }
            catch { }
        }

        private void RefreshDeptDropdown(string filter)
        {
            _suppress = true;
            CmbDept.Items.Clear();
            foreach (var (id, name) in _allDepts)
                if (string.IsNullOrEmpty(filter) ||
                    id.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    CmbDept.Items.Add(new ComboBoxItem { Content = string.IsNullOrEmpty(name) ? id : $"{id} — {name}", Tag = id });
            _suppress = false;
        }

        private void CmbDept_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            RefreshDeptDropdown(CmbDept.Text?.Trim() ?? "");
            if (CmbDept.Items.Count > 0 && !string.IsNullOrEmpty(CmbDept.Text)) CmbDept.IsDropDownOpen = true;
        }

        private async void CmbDept_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string deptId = GetDeptId();
            if (string.IsNullOrEmpty(deptId)) return;
            ClearBelow(CmbStream);
            await LoadStreamsAsync(deptId);
        }

        private string GetDeptId()
        {
            if (CmbDept.SelectedItem is ComboBoxItem s && s.Tag != null) return s.Tag.ToString()!;
            string t = CmbDept.Text?.Trim() ?? ""; int d = t.IndexOf(" — ");
            return d >= 0 ? t[..d].Trim() : t;
        }

        private async Task LoadStreamsAsync(string deptId)
        {
            try
            {
                _allStreams = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT stream_id, IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE dept_id=@d ORDER BY stream_id", conn);
                    cmd.Parameters.AddWithValue("@d", deptId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close(); return list;
                });
                RefreshStreamDropdown("");
                if (CmbStream.Items.Count > 0) CmbStream.SelectedIndex = 0;
            }
            catch { }
        }

        private void RefreshStreamDropdown(string filter)
        {
            _suppress = true;
            CmbStream.Items.Clear();
            foreach (var (id, name) in _allStreams)
                if (string.IsNullOrEmpty(filter) ||
                    id.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    CmbStream.Items.Add(new ComboBoxItem { Content = string.IsNullOrEmpty(name) ? id : $"{id} — {name}", Tag = id });
            _suppress = false;
        }

        private void CmbStream_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppress) return;
            RefreshStreamDropdown(CmbStream.Text?.Trim() ?? "");
            if (CmbStream.Items.Count > 0 && !string.IsNullOrEmpty(CmbStream.Text)) CmbStream.IsDropDownOpen = true;
        }

        private async void CmbStream_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppress) return;
            string sid = GetStreamId();
            if (string.IsNullOrEmpty(sid)) return;
            ClearBelow(CmbLevel);
            await LoadLevelsAsync(sid);
        }

        private string GetStreamId()
        {
            if (CmbStream.SelectedItem is ComboBoxItem s && s.Tag != null) return s.Tag.ToString()!;
            string t = CmbStream.Text?.Trim() ?? ""; int d = t.IndexOf(" — ");
            return d >= 0 ? t[..d].Trim() : t;
        }

        private async Task LoadLevelsAsync(string streamId)
        {
            try
            {
                _streamLevels = await Task.Run(() =>
                {
                    var list = new List<(string, string)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT level_id, level FROM ecc_dof_wukrostmarycollege.levels WHERE stream_id=@s ORDER BY level", conn);
                    cmd.Parameters.AddWithValue("@s", streamId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((r[0]?.ToString() ?? "", r[1]?.ToString() ?? ""));
                    conn.Close(); return list;
                });
                _suppress = true;
                CmbLevel.Items.Clear();
                foreach (var (lid, lnum) in _streamLevels)
                    CmbLevel.Items.Add(new ComboBoxItem { Content = $"{lid} — {lnum}", Tag = lid });
                if (CmbLevel.Items.Count > 0) CmbLevel.SelectedIndex = 0;
                _suppress = false;
            }
            catch { }
        }

        private string GetLevelNum()
        {
            string display = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            int dash = display.IndexOf(" — ");
            return dash >= 0 ? display[(dash + 3)..].Trim() : display;
        }

        private void ClearBelow(ComboBox start)
        {
            _suppress = true;
            bool clear = false;
            foreach (var cmb in new[] { CmbStream, CmbLevel })
            { if (cmb == start) clear = true; if (clear) cmb.Items.Clear(); }
            _suppress = false;
        }

        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            string deptId   = GetDeptId();
            string streamId = GetStreamId();
            if (string.IsNullOrEmpty(deptId) || string.IsNullOrEmpty(streamId))
            { ModernDialog.Show(Window.GetWindow(this), "Department and Stream are required!", "Error", ModernDialog.DialogType.Error); return; }

            string lvlNum = GetLevelNum();
            string at     = (CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Regular";
            string ay     = TxtAcadYear.Text.Trim();

            try
            {
                var dt = new DataTable();
                string sql =
                    "SELECT sp.student_id, " +
                    "CONCAT(TRIM(sp.first_name),' ',TRIM(sp.father_name),' ',TRIM(sp.grand_father_name)) AS full_name, " +
                    "sp.gender, sp.level, sp.admission_type " +
                    "FROM ecc_dof_wukrostmarycollege.student_profile sp " +
                    "WHERE sp.dept_id=@d AND sp.stream_id=@s AND sp.admission_type=@at";
                if (!string.IsNullOrEmpty(lvlNum)) sql += " AND sp.level=@l";
                sql += " ORDER BY sp.first_name, sp.father_name";

                await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    var cmd  = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@d",  deptId);
                    cmd.Parameters.AddWithValue("@s",  streamId);
                    cmd.Parameters.AddWithValue("@at", at);
                    if (!string.IsNullOrEmpty(lvlNum)) cmd.Parameters.AddWithValue("@l", lvlNum);
                    new MySqlDataAdapter(cmd).Fill(dt);
                });

                Grid1.ItemsSource      = dt.DefaultView;
                PreviewCard.Visibility = Visibility.Visible;
                TxtPreviewInfo.Text    = $"Dept: {deptId} | Stream: {streamId} | Level: {lvlNum} | Adm: {at}" +
                                         (string.IsNullOrEmpty(ay) ? "" : $" | Year: {ay}") +
                                         $" — {dt.Rows.Count} students";
            }
            catch (Exception ex) { ModernDialog.Show(Window.GetWindow(this), "Error: " + ex.Message, "DB Error", ModernDialog.DialogType.Error); }
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not DataView view || view.Count == 0)
            { ModernDialog.Show(Window.GetWindow(this), "Generate first.", "Info", ModernDialog.DialogType.Info); return; }

            var dlg = new Microsoft.Win32.SaveFileDialog
            { FileName = $"Attendance_{DateTime.Now:yyyyMMdd}", DefaultExt = ".pdf", Filter = "PDF|*.pdf" };
            if (dlg.ShowDialog() != true) return;

            string path      = dlg.FileName;
            string deptId    = GetDeptId();
            string streamId  = GetStreamId();
            string lvlNum    = GetLevelNum();
            string at        = (CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string ay        = TxtAcadYear.Text.Trim();
            string semester  = TxtSemester.Text.Trim();
            string classYear = TxtClassYear.Text.Trim();

            var rows = new List<(string Name, string Sex, string Id)>();
            foreach (DataRowView drv in view)
                rows.Add((TryGet(drv, "full_name"), TryGet(drv, "gender"), TryGet(drv, "student_id")));

            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand("SELECT IFNULL(dept_name,'') FROM ecc_dof_wukrostmarycollege.departments WHERE dept_id=@d LIMIT 1", conn);
                    cmd.Parameters.AddWithValue("@d", deptId);
                    string deptName = cmd.ExecuteScalar()?.ToString() ?? deptId;

                    cmd = new MySqlCommand("SELECT IFNULL(stream_name,'') FROM ecc_dof_wukrostmarycollege.streams WHERE stream_id=@s LIMIT 1", conn);
                    cmd.Parameters.AddWithValue("@s", streamId);
                    string streamName = cmd.ExecuteScalar()?.ToString() ?? streamId;
                    conn.Close();

                    var doc = new MigraDoc.DocumentObjectModel.Document();
                    doc.Styles["Normal"].Font.Name = "Times New Roman";
                    doc.Styles["Normal"].Font.Size = 9;

                    var sec = doc.AddSection();
                    sec.PageSetup.PageFormat   = MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.Orientation  = MigraDoc.DocumentObjectModel.Orientation.Landscape;
                    sec.PageSetup.TopMargin    = "1.5cm";
                    sec.PageSetup.BottomMargin = "1.8cm";
                    sec.PageSetup.LeftMargin   = "1.2cm";
                    sec.PageSetup.RightMargin  = "1.2cm";
                    // ── Title ─────────────────────────────────────────────────
                    void CentBold(string text, double size)
                    {
                        var p = sec.AddParagraph(text);
                        p.Format.Alignment   = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        p.Format.Font.Bold   = true;
                        p.Format.Font.Size   = size;
                        p.Format.Font.Name   = "Times New Roman";
                        p.Format.SpaceBefore = "0mm";
                        p.Format.SpaceAfter  = "0.8mm";
                    }

                    CentBold("ECC-DoA ST.MARY COLLEGE", 13);
                    CentBold("REGISTRAR'S OFFICE", 11.5);
                    CentBold("ATTENDANCE SHEET", 10);
                    sec.AddParagraph().Format.SpaceAfter = "2mm";

                    // ── Info block (3 rows, 2 cols) ───────────────────────────
                    var info = sec.AddTable();
                    info.Borders.Width = 0;
                    info.AddColumn("13.65cm");
                    info.AddColumn("13.65cm");
                    info.TopPadding    = "0.5mm";
                    info.BottomPadding = "0.5mm";
                    info.Format.Font.Size = 9.5;
                    info.Format.Font.Name = "Times New Roman";

                    void InfoRow(string l1, string v1, string l2, string v2)
                    {
                        var r = info.AddRow();
                        var p1 = r.Cells[0].AddParagraph();
                        p1.AddFormattedText(l1, MigraDoc.DocumentObjectModel.TextFormat.NotBold);
                        p1.AddFormattedText(v1, MigraDoc.DocumentObjectModel.TextFormat.Bold);
                        var p2 = r.Cells[1].AddParagraph();
                        p2.AddFormattedText(l2, MigraDoc.DocumentObjectModel.TextFormat.NotBold);
                        p2.AddFormattedText(v2, MigraDoc.DocumentObjectModel.TextFormat.Bold);
                    }

                    InfoRow("Department: ", deptName,  "Occupational Title: ", streamName);
                    InfoRow("Class Year: ", string.IsNullOrEmpty(classYear) ? "_______________" : classYear,
                            "Academic Year: ", string.IsNullOrEmpty(ay) ? "_______________" : ay);
                    InfoRow("Admission Type: ", at, "Semester: ", string.IsNullOrEmpty(semester) ? "_______________" : semester);

                    sec.AddParagraph().Format.SpaceAfter = "3mm";

                    // ── Attendance table ──────────────────────────────────────
                    // Time slots repeated across the page: 2:00-4:00 | 4:15-6:00 | 8:00-11:00 (x5 sets)
                    string[] slots = { "2:00-\n4:00", "4:15-\n6:00", "8:00-\n11:00" };
                    int sets = 5;   // 5 repetitions of the 3 slots = 15 date columns
                    int dateCols = sets * slots.Length;  // 15

                    var tbl = sec.AddTable();
                    tbl.Borders.Width = 0.4;
                    tbl.Borders.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    tbl.Format.Font.Size = 8;
                    tbl.Format.Font.Name = "Times New Roman";
                    tbl.TopPadding    = "0.8mm";
                    tbl.BottomPadding = "0.8mm";

                    // Column widths — A4 landscape, 1.2cm margins each side = 29.7 - 2.4 = 27.3cm usable
                    // 0.7 + 4.8 + 0.6 + 15×1.34 + 1.1 = 27.3cm
                    tbl.AddColumn("0.7cm");
                    tbl.AddColumn("4.8cm");
                    tbl.AddColumn("0.6cm");
                    for (int i = 0; i < dateCols; i++) tbl.AddColumn("1.34cm");
                    tbl.AddColumn("1.1cm");

                    // Header row 1: merged "No", "Full Name", "Sex", then slots, "Total no of Absence"
                    var hdr = tbl.AddRow();
                    hdr.HeadingFormat = true;
                    hdr.Format.Font.Bold = true;
                    hdr.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    hdr.Shading.Color = new MigraDoc.DocumentObjectModel.Color(220, 220, 220);

                    hdr.Cells[0].AddParagraph("No");
                    hdr.Cells[0].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    hdr.Cells[1].AddParagraph("Full Name");
                    hdr.Cells[2].AddParagraph("Sex");
                    hdr.Cells[2].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;

                    // Slot headers
                    for (int i = 0; i < dateCols; i++)
                    {
                        hdr.Cells[3 + i].AddParagraph(slots[i % slots.Length]);
                        hdr.Cells[3 + i].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    }

                    hdr.Cells[3 + dateCols].AddParagraph("Total no of Absence");
                    hdr.Cells[3 + dateCols].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;

                    for (int i = 0; i < rows.Count; i++)
                    {
                        var (name, sex, _) = rows[i];
                        var row = tbl.AddRow();
                        row.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        if (i % 2 == 1)
                            row.Shading.Color = new MigraDoc.DocumentObjectModel.Color(250, 250, 250);

                        row.Cells[0].AddParagraph((i + 1).ToString());
                        row.Cells[0].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;

                        // Name — no wrap, shrink font if needed
                        var namePara = row.Cells[1].AddParagraph(name);
                        namePara.Format.Font.Size = 7.5;

                        row.Cells[2].AddParagraph(sex.Length > 0 ? sex[0].ToString().ToUpper() : "");
                        row.Cells[2].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        // leave date + total cells blank for manual filling
                    }

                    // ── Approval block ────────────────────────────────────────
                    sec.AddParagraph().Format.SpaceAfter = "8mm";

                    var sig = sec.AddTable();
                    sig.Borders.Width = 0;
                    sig.AddColumn("9.1cm");
                    sig.AddColumn("9.1cm");
                    sig.AddColumn("9.1cm");
                    sig.Format.Font.Size = 9.5;
                    sig.Format.Font.Name = "Times New Roman";

                    void SigRow(string label)
                    {
                        var r = sig.AddRow();
                        r.Format.SpaceBefore = "1.5mm";
                        r.Cells[0].AddParagraph($"{label}:-________________");
                        r.Cells[1].AddParagraph("Sign._____________");
                        r.Cells[2].AddParagraph("");
                    }

                    SigRow("Prepared by");
                    SigRow("Checked by");
                    SigRow("Approved by");

                    // ── Footer ────────────────────────────────────────────────
                    var footer = sec.Footers.Primary;
                    var fp = footer.AddParagraph();
                    fp.Format.Alignment   = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    fp.Format.Font.Size   = 7.5;
                    fp.Format.Font.Name   = "Times New Roman";
                    fp.Format.Borders.Top.Width = 0.4;
                    fp.Format.Borders.Top.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    fp.Format.SpaceBefore = "1mm";
                    fp.AddText("Page "); fp.AddPageField(); fp.AddText(" of "); fp.AddNumPagesField();

                    var renderer = new MigraDoc.Rendering.PdfDocumentRenderer { Document = doc };
                    renderer.RenderDocument();
                    renderer.PdfDocument.Save(path);
                });

                ModernDialog.Show(Window.GetWindow(this), "PDF saved!", "Success", ModernDialog.DialogType.Success);
            }
            catch (Exception ex) { ModernDialog.Show(Window.GetWindow(this), "PDF failed: " + ex.Message, "Error", ModernDialog.DialogType.Error); }
        }

        private static string TryGet(DataRowView drv, string col)
        { try { return drv[col]?.ToString()?.Trim() ?? ""; } catch { return ""; } }
    }
}
