using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class StudentMarksPage : Page
    {
        private string _selSid = "", _selLvl = "", _selMod = "";
        private DBConnect _db = new DBConnect();
        private const string BASE =
            "SELECT student_id,level,module_code,employee_id,academic_year," +
            "score_of_knowledge_test,score_of_practical_test,competence " +
            "FROM ecc_dof_wukrostmarycollege.student_mark";

        public StudentMarksPage()
        {
            InitializeComponent();
            TxtKnow.PreviewTextInput += NumOnly;
            TxtPrac.PreviewTextInput += NumOnly;
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            // Open dropdown on click for module/dept combos
            CmbModCode.GotFocus  += (s,e) => ((ComboBox)s).IsDropDownOpen = true;
            CmbFDept.GotFocus    += (s,e) => ((ComboBox)s).IsDropDownOpen = true;
            CmbFModule.GotFocus  += (s,e) => ((ComboBox)s).IsDropDownOpen = true;
            Loaded += async (s, e) =>
            {
                await LoadModulesAsync();
                await Load(BASE);
            };
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

        // Load module codes from DB into CmbModCode
        private async Task LoadModulesAsync()
        {
            try
            {
                var modules = await Task.Run(() =>
                {
                    var list = new System.Collections.Generic.List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT DISTINCT module_code FROM ecc_dof_wukrostmarycollege.courses ORDER BY module_code", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close();
                    return list;
                });

                CmbModCode.Items.Clear();
                foreach (var m in modules)
                    CmbModCode.Items.Add(new ComboBoxItem { Content = m });
                if (CmbModCode.Items.Count > 0) CmbModCode.SelectedIndex = 0;

                // Filter module combo — starts empty (free-text allowed)
                CmbFModule.Items.Clear();
                foreach (var m in modules)
                    CmbFModule.Items.Add(new ComboBoxItem { Content = m });
                CmbFModule.Text = ""; // free-text, no default selection

                // Load departments into CmbFDept dropdown
                var depts = await Task.Run(() =>
                {
                    var list = new System.Collections.Generic.List<string>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT dept_id FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r[0]?.ToString() ?? "");
                    conn.Close();
                    return list;
                });

                CmbFDept.Items.Clear();
                foreach (var d in depts)
                    CmbFDept.Items.Add(new ComboBoxItem { Content = d });
                CmbFDept.Text = ""; // free-text default
            }
            catch { /* DB offline — skip */ }
        }

        private void NumOnly(object s, System.Windows.Input.TextCompositionEventArgs e)
            => e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"\d");

        private void ScoreChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(TxtKnow.Text, out int k) && int.TryParse(TxtPrac.Text, out int p))
                TxtCompetence.Text = (k >= 51 && k <= 100 && p >= 90 && p <= 100)
                    ? "Competent" : "Not Competent";
            else
                TxtCompetence.Text = "";
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
            _selLvl = r["level"]?.ToString() ?? "";
            _selMod = r["module_code"]?.ToString() ?? "";
            TxtStudID.Text     = _selSid;
            TxtEmpID.Text      = r["employee_id"]?.ToString() ?? "";
            TxtAcadYear.Text   = r["academic_year"]?.ToString() ?? "";
            TxtKnow.Text       = r["score_of_knowledge_test"]?.ToString() ?? "";
            TxtPrac.Text       = r["score_of_practical_test"]?.ToString() ?? "";
            TxtCompetence.Text = r["competence"]?.ToString() ?? "";
            SetCombo(CmbLevel,   r["level"]?.ToString() ?? "1");
            SetComboByContent(CmbModCode, _selMod);
        }

        private void SetCombo(ComboBox c, string v)
        { foreach (ComboBoxItem i in c.Items) if (i.Content?.ToString() == v) { c.SelectedItem = i; return; } }

        private void SetComboByContent(ComboBox c, string v)
        {
            foreach (ComboBoxItem i in c.Items)
                if (i.Content?.ToString() == v) { c.SelectedItem = i; return; }
            // If not found, add it dynamically
            var item = new ComboBoxItem { Content = v };
            c.Items.Add(item);
            c.SelectedItem = item;
        }

        private string CmbVal(ComboBox c)
        {
            // Always prefer typed text (covers both free-text and selected-from-list)
            var text = c.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(text) && text != "(All)") return text;
            return (c.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
        }

        // ── SAVE (exact original algorithm) ─────────────────────────────────
        private async void BtnSave_Click(object s, RoutedEventArgs e)
        {
            string sid = TxtStudID.Text.Trim(), lvl = CmbVal(CmbLevel),
                   mod = CmbVal(CmbModCode), emp = TxtEmpID.Text.Trim(),
                   ay  = TxtAcadYear.Text.Trim(),
                   kn  = TxtKnow.Text.Trim(), pr = TxtPrac.Text.Trim();

            // Validation: all fields required
            if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(lvl) ||
                string.IsNullOrWhiteSpace(mod) || string.IsNullOrWhiteSpace(emp) ||
                string.IsNullOrWhiteSpace(ay)  || string.IsNullOrWhiteSpace(kn)  ||
                string.IsNullOrWhiteSpace(pr))
            { Msg("Error. Please fill in all fields!", false); return; }

            if (!int.TryParse(kn, out int kVal) || !int.TryParse(pr, out int pVal))
            { Msg("Knowledge and Practical scores must be numbers!", false); return; }

            if (kVal < 0 || kVal > 100 || pVal < 0 || pVal > 100)
            { Msg("Scores must be between 0 and 100!", false); return; }

            try
            {
                // Duplicate check: student_id + level + academic_year (original logic)
                bool dup = await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_mark " +
                        "WHERE student_id=@s AND level=@l AND academic_year=@y", c);
                    cmd.Parameters.AddWithValue("@s", sid);
                    cmd.Parameters.AddWithValue("@l", lvl);
                    cmd.Parameters.AddWithValue("@y", ay);
                    int n = Convert.ToInt32(cmd.ExecuteScalar()); c.Close(); return n > 0;
                });
                if (dup) { Msg("Error. This mark list is already attached!", false); return; }

                // Competence logic (exact original)
                string comp = (kVal >= 51 && kVal <= 100 && pVal >= 90 && pVal <= 100)
                    ? "Competent" : "Not Competent";

                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.student_mark " +
                        "(student_id,level,module_code,employee_id,academic_year," +
                        "score_of_knowledge_test,score_of_practical_test,competence) " +
                        "VALUES(@s,@l,@m,@e,@y,@k,@p,@c)", c);
                    cmd.Parameters.AddWithValue("@s", sid); cmd.Parameters.AddWithValue("@l", lvl);
                    cmd.Parameters.AddWithValue("@m", mod); cmd.Parameters.AddWithValue("@e", emp);
                    cmd.Parameters.AddWithValue("@y", ay);  cmd.Parameters.AddWithValue("@k", kn);
                    cmd.Parameters.AddWithValue("@p", pr);  cmd.Parameters.AddWithValue("@c", comp);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!", true);
                await Load(BASE); Clear();
            }
            catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        private async void BtnUpdate_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            string emp = TxtEmpID.Text.Trim(), ay = TxtAcadYear.Text.Trim(),
                   kn  = TxtKnow.Text.Trim(),  pr = TxtPrac.Text.Trim(), comp = TxtCompetence.Text;
            try
            {
                string sid = _selSid, lvl = _selLvl, mod = _selMod;
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE ecc_dof_wukrostmarycollege.student_mark " +
                        "SET employee_id=@e,academic_year=@y," +
                        "score_of_knowledge_test=@k,score_of_practical_test=@p,competence=@c " +
                        "WHERE student_id=@s AND level=@l AND module_code=@m", c);
                    cmd.Parameters.AddWithValue("@e",emp); cmd.Parameters.AddWithValue("@y",ay);
                    cmd.Parameters.AddWithValue("@k",kn);  cmd.Parameters.AddWithValue("@p",pr);
                    cmd.Parameters.AddWithValue("@c",comp);
                    cmd.Parameters.AddWithValue("@s",sid);  cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.Parameters.AddWithValue("@m",mod);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Update successful!", true); await Load(BASE);
            }
            catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        private async void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selSid)) { Msg("Select a record first.", false); return; }
            var dlg = new ModernDialog(
                $"Delete mark for {_selSid} Level {_selLvl}?", "Confirm",
                ModernDialog.DialogType.Warning) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;
            string sid = _selSid, lvl = _selLvl, mod = _selMod;
            try
            {
                await Task.Run(() =>
                {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "DELETE FROM ecc_dof_wukrostmarycollege.student_mark " +
                        "WHERE student_id=@s AND level=@l AND module_code=@m", c);
                    cmd.Parameters.AddWithValue("@s",sid);
                    cmd.Parameters.AddWithValue("@l",lvl);
                    cmd.Parameters.AddWithValue("@m",mod);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Delete successful!", true); await Load(BASE); Clear();
            }
            catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── FILTER (exact original OR logic) ─────────────────────────────────
        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string studID  = TxtFStudID.Text.Trim();
            string deptID  = (CmbFDept.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            string year    = TxtFYear.Text.Trim();
            string level   = CmbVal(CmbFLevel);
            string modCod  = CmbFModule.Text?.Trim() ?? "";
            if (modCod == "(All)" || modCod == "") modCod = "";
            string admType = (CmbFAdmType.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            if (admType == "(All)") admType = "";

            if (!string.IsNullOrEmpty(studID))
            {
                // Student ID search — TRIM handles stored leading spaces
                await Load(BASE + $" WHERE TRIM(student_id)='{studID.Replace("'", "''")}'");
                return;
            }

            // OR branch — build conditions from only filled fields
            var conditions = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(year))    conditions.Add($"sm.academic_year='{year.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(level))   conditions.Add($"sm.level='{level.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(modCod))  conditions.Add($"sm.module_code='{modCod.Replace("'","''")}'");

            // Dept / admission_type filter goes through student_profile subquery
            var profileConds = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(deptID))  profileConds.Add($"dept_id='{deptID.Replace("'","''")}'");
            if (!string.IsNullOrEmpty(admType)) profileConds.Add($"admission_type='{admType.Replace("'","''")}'");

            if (profileConds.Count > 0)
                conditions.Add($"TRIM(sm.student_id) IN (SELECT TRIM(student_id) FROM ecc_dof_wukrostmarycollege.student_profile WHERE {string.Join(" AND ", profileConds)})");

            if (conditions.Count == 0)
            {
                await Load(BASE);
                return;
            }

            await Load(
                "SELECT sm.student_id,sm.level,sm.module_code,sm.employee_id,sm.academic_year," +
                "sm.score_of_knowledge_test,sm.score_of_practical_test,sm.competence " +
                "FROM ecc_dof_wukrostmarycollege.student_mark sm " +
                $"WHERE {string.Join(" AND ", conditions)}");
        }

        private async void BtnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            TxtFStudID.Text = TxtFYear.Text = "";
            CmbFDept.Text   = "";
            CmbFModule.Text = "";
            CmbFLevel.SelectedIndex   = 0;
            CmbFAdmType.SelectedIndex = 0;
            await Load(BASE);
        }

        // ── ATTACH MARK LIST ─────────────────────────────────────────────────
        private void BtnAttachMarkList_Click(object sender, RoutedEventArgs e)
        {
            var win = new AttachMarkListWindow { Owner = Window.GetWindow(this) };
            win.ShowDialog();
        }

        // ── QUICK FILTER (text box) ───────────────────────────────────────────
        private async void TxtFilter_Changed(object s, TextChangedEventArgs e)
        {
            string t = TxtFilter.Text.Trim();
            await Load(string.IsNullOrEmpty(t) ? BASE : BASE + $" WHERE student_id LIKE '%{t}%'");
        }

        private async void BtnReset_Click(object s, RoutedEventArgs e)
        { TxtFilter.Text = ""; await Load(BASE); }

        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not System.Data.DataView view || view.Count == 0)
            { Msg("No data to print.", false); return; }

            var pd = new System.Windows.Controls.PrintDialog();
            try
            {
                var server = new System.Printing.LocalPrintServer();
                foreach (System.Printing.PrintQueue q in server.GetPrintQueues())
                    if (q.Name.Contains("PDF", StringComparison.OrdinalIgnoreCase))
                    { pd.PrintQueue = q; break; }
            }
            catch { }
            if (pd.ShowDialog() != true) return;

            if (LoadingOverlay != null) { LoadingOverlay.Visibility = Visibility.Visible; }
            await Task.Delay(50);
            try
            {
                // Extract data on background thread
                string[][] rowData = await Task.Run(() =>
                {
                    string[] fields = { "student_id","level","module_code","employee_id",
                                        "academic_year","score_of_knowledge_test",
                                        "score_of_practical_test","competence" };
                    var items = new System.Data.DataRowView[view.Count];
                    for (int i = 0; i < view.Count; i++) items[i] = (System.Data.DataRowView)view[i];
                    var rows = new string[items.Length][];
                    System.Threading.Tasks.Parallel.For(0, items.Length,
                        new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = 4 },
                        i => { rows[i] = System.Array.ConvertAll(fields, f => items[i][f]?.ToString() ?? ""); });
                    return rows;
                });

                // Build FlowDocument on UI thread
                string[] headers = { "Student ID","Level","Module Code","Instructor ID",
                                     "Acad Year","Know Score","Prac Score","Competence" };
                var doc = new System.Windows.Documents.FlowDocument
                {
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"), FontSize = 9,
                    PagePadding = new Thickness(30), ColumnWidth = double.MaxValue,
                    Background = System.Windows.Media.Brushes.White, Foreground = System.Windows.Media.Brushes.Black
                };
                doc.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("Wukro St. Mary College"))
                    { FontSize = 16, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center, Margin = new Thickness(0,0,0,2) });
                doc.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("Student Marks List"))
                    { FontSize = 12, TextAlignment = TextAlignment.Center, Margin = new Thickness(0,0,0,2) });
                doc.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run($"Printed: {DateTime.Now:dd MMM yyyy  HH:mm}"))
                    { FontSize = 8, Foreground = System.Windows.Media.Brushes.Gray, TextAlignment = TextAlignment.Center, Margin = new Thickness(0,0,0,10) });

                var table = new System.Windows.Documents.Table { CellSpacing = 0 };
                foreach (var _ in headers) table.Columns.Add(new System.Windows.Documents.TableColumn());
                var rg = new System.Windows.Documents.TableRowGroup(); table.RowGroups.Add(rg);
                var hRow = new System.Windows.Documents.TableRow { Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(18,52,116)) };
                foreach (var h in headers)
                    hRow.Cells.Add(new System.Windows.Documents.TableCell(
                        new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(h)) { FontWeight = FontWeights.Bold, FontSize = 7.5 })
                    { Padding = new Thickness(2), Foreground = System.Windows.Media.Brushes.White });
                rg.Rows.Add(hRow);
                bool alt = false;
                var altBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(245,247,250));
                foreach (var cols in rowData)
                {
                    var row = new System.Windows.Documents.TableRow { Background = alt ? altBrush : System.Windows.Media.Brushes.White };
                    alt = !alt;
                    foreach (var val in cols)
                        row.Cells.Add(new System.Windows.Documents.TableCell(
                            new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(val)) { FontSize = 7.5 })
                        { Padding = new Thickness(2,1,2,1) });
                    rg.Rows.Add(row);
                }
                doc.Blocks.Add(table);

                var paginator = ((System.Windows.Documents.IDocumentPaginatorSource)doc).DocumentPaginator;
                paginator.PageSize = new System.Windows.Size(1122.5, 793.7);
                pd.PrintDocument(paginator, "Student Marks List");
                Msg($"Sent {view.Count} records to printer.", true);
            }
            catch (Exception ex) { Msg("Print failed: " + ex.Message, false); }
            finally { if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed; }
        }

        private async void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1.ItemsSource is not System.Data.DataView view || view.Count == 0)
            { Msg("No data to export.", false); return; }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName   = $"StudentMarks_{DateTime.Now:yyyyMMdd_HHmm}",
                DefaultExt = ".xlsx",
                Filter     = "Excel Workbook|*.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
            try
            {
                string path = dlg.FileName;
                string[] fields  = { "student_id","level","module_code","employee_id",
                                     "academic_year","score_of_knowledge_test",
                                     "score_of_practical_test","competence" };
                string[] headers = { "Student ID","Level","Module Code","Instructor ID",
                                     "Academic Year","Know Score","Prac Score","Competence" };

                await Task.Run(() =>
                {
                    using var wb = new ClosedXML.Excel.XLWorkbook();
                    var ws = wb.Worksheets.Add("Marks");
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
                            ws.Cell(row, c + 1).Value = drv[fields[c]]?.ToString() ?? "";
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

        private void Clear()
        {
            TxtStudID.Text = TxtEmpID.Text = TxtAcadYear.Text =
            TxtKnow.Text   = TxtPrac.Text  = TxtCompetence.Text = "";
            _selSid = _selLvl = _selMod = "";
            MsgBorder.Visibility = Visibility.Collapsed;
        }

        private void Msg(string m, bool ok)
        {
            var owner = Window.GetWindow(this);
            if (ok) ModernDialog.Show(owner, m, "Success", ModernDialog.DialogType.Success);
            else    ModernDialog.Show(owner, m, "Error",   ModernDialog.DialogType.Error);
            MsgBorder.Visibility = Visibility.Collapsed;
        }
    }
}
