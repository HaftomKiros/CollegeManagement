using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class COCListPage : Page
    {
        private DBConnect _db = new DBConnect();
        public COCListPage() { InitializeComponent(); ThemeManager.ThemeChanged += ApplyTheme; ApplyTheme(); Loaded += async (s,e) => await LoadDepts(); }

        private async Task LoadDepts()
        {
            try {
                var list = await Task.Run(() => { var r=new System.Collections.Generic.List<string>(); var c=_db.GetConnection(); c.Open(); using var cmd=new MySqlCommand("SELECT dept_id FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id",c); using var rd=cmd.ExecuteReader(); while(rd.Read()) r.Add(rd[0]?.ToString()??""); c.Close(); return r; });
                TxtDeptID.Items.Clear(); foreach(var d in list) TxtDeptID.Items.Add(new ComboBoxItem{Content=d});
            } catch {}
        }

        private async void CmbDeptID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string dept = TxtDeptID.Text?.Trim() ?? (TxtDeptID.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            if (string.IsNullOrEmpty(dept)) return;
            try {
                var list = await Task.Run(() => { var r=new System.Collections.Generic.List<string>(); var c=_db.GetConnection(); c.Open(); using var cmd=new MySqlCommand("SELECT stream_id FROM ecc_dof_wukrostmarycollege.streams WHERE dept_id=@d ORDER BY stream_id",c); cmd.Parameters.AddWithValue("@d",dept); using var rd=cmd.ExecuteReader(); while(rd.Read()) r.Add(rd[0]?.ToString()??""); c.Close(); return r; });
                TxtStreamID.Items.Clear(); foreach(var s in list) TxtStreamID.Items.Add(new ComboBoxItem{Content=s});
            } catch {}
        }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtDeptID.Text) || string.IsNullOrWhiteSpace(TxtStreamID.Text))
            {
                ModernDialog.Show(Window.GetWindow(this), "Department ID and Stream ID are required!", "Error", ModernDialog.DialogType.Error);
                return;
            }

            string di  = TxtDeptID.Text.Trim();
            string si  = TxtStreamID.Text.Trim();
            string at  = (CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Regular";
            string lv  = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "1";
            string ad  = TxtAssessDate.Text.Trim();
            string ay  = TxtAcadYear.Text.Trim();

            try
            {
                var dt = new DataTable();
                // Filter coc by level + assessment_date; join student_profile for name/gender/mobile
                // dept/stream filter goes through student_profile (admission_type also there)
                string sql =
                    "SELECT c.student_id, " +
                    "IFNULL(CONCAT(TRIM(sp.first_name),' ',TRIM(sp.father_name),' ',TRIM(sp.grand_father_name)),'') AS full_name, " +
                    "IFNULL(sp.gender,'') AS gender, IFNULL(sp.mobile_number1,'') AS mobile_number1, " +
                    "c.level, c.assessment_date, c.assessor_name, c.supervisor_name, c.competence, c.coc_level_id " +
                    "FROM ecc_dof_wukrostmarycollege.coc c " +
                    "LEFT JOIN ecc_dof_wukrostmarycollege.student_profile sp " +
                    "ON TRIM(c.student_id)=TRIM(sp.student_id) AND c.level=sp.level " +
                    "WHERE c.level=@l";

                // dept/stream filters are optional — only apply if student_profile join finds them
                if (!string.IsNullOrEmpty(di)) sql += " AND (sp.dept_id=@d OR sp.dept_id IS NULL)";
                if (!string.IsNullOrEmpty(si)) sql += " AND (sp.stream_id=@s OR sp.stream_id IS NULL)";
                if (!string.IsNullOrEmpty(at) && at != "(All)") sql += " AND (sp.admission_type=@at OR sp.admission_type IS NULL)";
                if (!string.IsNullOrEmpty(ad)) sql += " AND c.assessment_date LIKE @ad";
                if (!string.IsNullOrEmpty(ay)) sql += " AND (sp.admission_date LIKE @ay OR sp.admission_date IS NULL)";
                sql += " ORDER BY full_name";

                var cmd = new MySqlCommand(sql, _db.GetConnection());
                cmd.Parameters.AddWithValue("@l", lv);
                if (!string.IsNullOrEmpty(di)) cmd.Parameters.AddWithValue("@d",  di);
                if (!string.IsNullOrEmpty(si)) cmd.Parameters.AddWithValue("@s",  si);
                if (!string.IsNullOrEmpty(at) && at != "(All)") cmd.Parameters.AddWithValue("@at", at);
                if (!string.IsNullOrEmpty(ad)) cmd.Parameters.AddWithValue("@ad", $"%{ad}%");
                if (!string.IsNullOrEmpty(ay)) cmd.Parameters.AddWithValue("@ay", $"%{ay}%");

                await Task.Run(() => new MySqlDataAdapter(cmd).Fill(dt));

                Grid1.ItemsSource = dt.DefaultView;
                PreviewCard.Visibility = Visibility.Visible;
                TxtPreviewInfo.Text = $"COC List — Dept={di} | Stream={si} | Level={lv} | Adm={at}" +
                    (string.IsNullOrEmpty(ad) ? "" : $" | Assessment={ad}") +
                    (string.IsNullOrEmpty(ay) ? "" : $" | Year={ay}") +
                    $" — {dt.Rows.Count} records";
            }
            catch (Exception ex)
            {
                ModernDialog.Show(Window.GetWindow(this), "Error: " + ex.Message, "DB Error", ModernDialog.DialogType.Error);
            }
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not System.Data.DataView view || view.Count == 0)
            { ModernDialog.Show(Window.GetWindow(this), "Generate first.", "Info", ModernDialog.DialogType.Info); return; }

            var dlg = new Microsoft.Win32.SaveFileDialog
            { FileName = $"COCList_{DateTime.Now:yyyyMMdd}", DefaultExt = ".pdf", Filter = "PDF|*.pdf" };
            if (dlg.ShowDialog() != true) return;

            // Read all UI values on UI thread
            string path     = dlg.FileName;
            string deptId   = TxtDeptID.Text.Trim();
            string streamId = TxtStreamID.Text.Trim();
            string admType  = (CmbAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string level    = (CmbLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string assessDate = TxtAssessDate.Text.Trim();

            var rows = new System.Collections.Generic.List<(string Name, string Sex, string Mobile)>();
            foreach (System.Data.DataRowView drv in view)
            {
                string name = "";
                try { name = drv["full_name"]?.ToString()?.Trim() ?? ""; } catch { }
                if (string.IsNullOrEmpty(name)) try { name = drv["student_id"]?.ToString() ?? ""; } catch { }
                string sex    = ""; try { sex    = drv["gender"]?.ToString()?.Trim() ?? ""; } catch { }
                string mobile = ""; try { mobile = drv["mobile_number1"]?.ToString()?.Trim() ?? ""; } catch { }
                rows.Add((name, sex, mobile));
            }

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
                    doc.Styles["Normal"].Font.Size = 10;

                    var sec = doc.AddSection();
                    sec.PageSetup.PageFormat   = MigraDoc.DocumentObjectModel.PageFormat.A4;
                    sec.PageSetup.Orientation  = MigraDoc.DocumentObjectModel.Orientation.Portrait;
                    sec.PageSetup.TopMargin    = "2.0cm";
                    sec.PageSetup.BottomMargin = "2.0cm";
                    sec.PageSetup.LeftMargin   = "2.0cm";
                    sec.PageSetup.RightMargin  = "2.0cm";

                    // ── Title ─────────────────────────────────────────────────
                    void CentBold(string text, double size)
                    {
                        var p = sec.AddParagraph(text);
                        p.Format.Alignment   = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        p.Format.Font.Bold   = true;
                        p.Format.Font.Size   = size;
                        p.Format.Font.Name   = "Times New Roman";
                        p.Format.SpaceBefore = "0mm";
                        p.Format.SpaceAfter  = "1mm";
                    }

                    CentBold("ECC-DoA ST.MARY COLLEGE", 14);
                    CentBold("TIGRAY, ETHIOPIA", 12);
                    CentBold("COC CANDIDATES LIST", 11);
                    sec.AddParagraph().Format.SpaceAfter = "4mm";

                    // ── Info block ────────────────────────────────────────────
                    void InfoLine(string label, string value)
                    {
                        var p = sec.AddParagraph();
                        p.Format.Font.Size   = 10.5;
                        p.Format.Font.Name   = "Times New Roman";
                        p.Format.SpaceAfter  = "0.5mm";
                        p.AddFormattedText(label, MigraDoc.DocumentObjectModel.TextFormat.NotBold);
                        p.AddFormattedText(value, MigraDoc.DocumentObjectModel.TextFormat.Bold);
                    }

                    InfoLine("Department:  ", deptName);
                    InfoLine("Occupational Title:  ", streamName);
                    InfoLine("Admission Type:  ", admType);
                    InfoLine("Schedule of Assessment Date:  ", string.IsNullOrEmpty(assessDate) ? "_______________" : assessDate);

                    sec.AddParagraph().Format.SpaceAfter = "5mm";

                    // ── Candidate table ───────────────────────────────────────
                    // Portrait A4: 21cm - 2×2cm = 17cm usable
                    // 0.8 + 7.0 + 1.0 + 2.5 + 3.2 + 2.5 = 17.0cm
                    var tbl = sec.AddTable();
                    tbl.Borders.Width = 0.5;
                    tbl.Borders.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    tbl.Format.Font.Size = 10;
                    tbl.Format.Font.Name = "Times New Roman";
                    tbl.TopPadding    = "1.5mm";
                    tbl.BottomPadding = "1.5mm";

                    tbl.AddColumn("0.8cm");   // No
                    tbl.AddColumn("7.0cm");   // Full Name
                    tbl.AddColumn("1.0cm");   // Sex
                    tbl.AddColumn("2.5cm");   // COC FEE
                    tbl.AddColumn("3.2cm");   // Mobile Number
                    tbl.AddColumn("2.5cm");   // Signature

                    var hdr = tbl.AddRow();
                    hdr.HeadingFormat = true;
                    hdr.Format.Font.Bold = true;
                    hdr.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    string[] hdrs = { "No", "Full Name", "Sex", "COC FEE", "Mobile Number", "Signature" };
                    for (int c = 0; c < hdrs.Length; c++)
                    {
                        hdr.Cells[c].AddParagraph(hdrs[c]);
                        hdr.Cells[c].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Left;
                    }

                    for (int i = 0; i < rows.Count; i++)
                    {
                        var (name, sex, mobile) = rows[i];
                        var row = tbl.AddRow();
                        row.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row.Cells[0].AddParagraph((i + 1).ToString());
                        row.Cells[0].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[1].AddParagraph(name);
                        row.Cells[2].AddParagraph(sex.Length > 0 ? sex[0].ToString().ToUpper() : "");
                        row.Cells[2].Format.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                        row.Cells[3].AddParagraph("");  // COC FEE — blank for manual fill
                        row.Cells[4].AddParagraph(mobile);
                        row.Cells[5].AddParagraph("");  // Signature — blank
                    }

                    // ── Assessor / Supervisor block ───────────────────────────
                    sec.AddParagraph().Format.SpaceAfter = "8mm";

                    void AssignLine(string text)
                    {
                        var p = sec.AddParagraph(text);
                        p.Format.Font.Size  = 10.5;
                        p.Format.Font.Name  = "Times New Roman";
                        p.Format.SpaceAfter = "3mm";
                    }

                    AssignLine("Assigned Assessor:____________________");
                    AssignLine("Assigned Supervisors 1)______________________");
                    AssignLine("                              2)______________________");

                    // ── Footer: page number ───────────────────────────────────
                    var footer = sec.Footers.Primary;
                    var fp = footer.AddParagraph();
                    fp.Format.Alignment   = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    fp.Format.Font.Size   = 8;
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
            catch (Exception ex)
            {
                ModernDialog.Show(Window.GetWindow(this), "PDF failed: " + ex.Message, "Error", ModernDialog.DialogType.Error);
            }
        }
    }
}
