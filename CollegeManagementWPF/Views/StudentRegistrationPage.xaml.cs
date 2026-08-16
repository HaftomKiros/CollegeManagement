using CollegeManagementWPF.Data;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CollegeManagementWPF.Views
{
    // Converts AlternationIndex (0-based) to Serial No (1-based)
    public class AddOneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is int i ? (i + 1).ToString() : "";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public partial class StudentRegistrationPage : Page
    {
        private string _selectedCell  = "";
        private string _selectedLevel = "";
        private DBConnect _db = new DBConnect();

        private const string BASE_QUERY =
            "SELECT student_id,dept_id,stream_id,level,first_name,father_name," +
            "grand_father_name,gender,admission_date,program_type,admission_type," +
            "wereda,kebele,gpa_grade_10th,gpa_grade_12th,mobile_number1 " +
            "FROM ecc_dof_wukrostmarycollege.student_profile";

        public StudentRegistrationPage()
        {
            InitializeComponent();
            TxtGpa10.PreviewTextInput += GpaField_PreviewTextInput;
            TxtGpa12.PreviewTextInput += GpaField_PreviewTextInput;
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            Loaded += async (s, e) =>
            {
                await LoadDepartmentsAsync();
                await LoadGridAsync(BASE_QUERY);
            };
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;

            // Page background
            SetGrad("PageBg1", dark ? "#0A1526" : "#EEF2F7");
            SetGrad("PageBg2", dark ? "#060E1C" : "#E2E8F0");

            // Form card
            SetBrush("FormCardBg",     dark ? "#070F1E" : "#FFFFFF");
            SetBrush("FormCardBorder", dark ? "#1E3A6A" : "#CBD5E1");

            // Photo card
            SetBrush("PhotoCardBg",     dark ? "#050B16" : "#F8FAFC");
            SetBrush("PhotoCardBorder", dark ? "#1E3A6A" : "#CBD5E1");

            // Table card
            SetBrush("TableCardBg",     dark ? "#050B16" : "#FFFFFF");
            SetBrush("TableCardBorder", dark ? "#1E3A6A" : "#CBD5E1");
        }

        private void SetBrush(string name, string hex)
        {
            if (FindName(name) is System.Windows.Media.SolidColorBrush b)
                b.Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
        }

        private void SetGrad(string name, string hex)
        {
            if (FindName(name) is System.Windows.Media.GradientStop g)
                g.Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
        }

        // Load departments into CmbDeptID and CmbFDept
        private async Task LoadDepartmentsAsync()
        {
            try
            {
                var depts = await Task.Run(() =>
                {
                    var list = new System.Collections.Generic.List<(string id, string name)>();
                    var conn = _db.GetConnection();
                    conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT dept_id, dept_name FROM ecc_dof_wukrostmarycollege.departments ORDER BY dept_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                        list.Add((r["dept_id"].ToString()!, r["dept_name"].ToString()!));
                    conn.Close();
                    return list;
                });

                CmbDeptID.Items.Clear();
                CmbFDept.Items.Clear();
                foreach (var (id, name) in depts)
                {
                    CmbDeptID.Items.Add(new System.Windows.Controls.ComboBoxItem
                        { Content = id, Tag = name });
                    CmbFDept.Items.Add(new System.Windows.Controls.ComboBoxItem
                        { Content = id, Tag = name });
                }
            }
            catch { /* DB offline — skip */ }
        }

        // Cascade: load streams for selected department
        private async void CmbDeptID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbDeptID.SelectedItem is not System.Windows.Controls.ComboBoxItem item) return;
            string deptId = item.Content?.ToString() ?? "";
            await LoadStreamsAsync(CmbStreamID, deptId);
        }

        private void CmbFDept_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbFDept.SelectedItem is not System.Windows.Controls.ComboBoxItem item) return;
            string deptId = item.Content?.ToString() ?? "";
            // Filter streams for filter bar if needed — here just capture dept
        }

        private async Task LoadStreamsAsync(ComboBox target, string deptId)
        {
            try
            {
                var streams = await Task.Run(() =>
                {
                    var list = new System.Collections.Generic.List<string>();
                    var conn = _db.GetConnection();
                    conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT stream_id FROM ecc_dof_wukrostmarycollege.streams WHERE dept_id=@did ORDER BY stream_id", conn);
                    cmd.Parameters.AddWithValue("@did", deptId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add(r["stream_id"].ToString()!);
                    conn.Close();
                    return list;
                });

                target.Items.Clear();
                foreach (var s in streams)
                    target.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = s });
                if (target.Items.Count > 0) target.SelectedIndex = 0;
            }
            catch { }
        }

        private string GetDeptID() =>
            (CmbDeptID.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "";

        private string GetStreamID() =>
            (CmbStreamID.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "";

        // Only allow digits and one decimal point in GPA fields
        private void GpaField_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var tb = (System.Windows.Controls.TextBox)sender;
            string proposed = tb.Text + e.Text;
            // Allow digits and single dot
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"[\d\.]") ||
                        (e.Text == "." && tb.Text.Contains("."));
        }

        // ── Async grid loader (never blocks UI) ─────────────────────────────
        private async Task LoadGridAsync(string query)
        {
            try
            {
                if (RegLoadingOverlay != null) RegLoadingOverlay.Visibility = Visibility.Visible;
                var table = await Task.Run(() =>
                {
                    var conn    = _db.GetConnection();
                    var adapter = new MySqlDataAdapter(new MySqlCommand(query, conn));
                    var dt      = new DataTable();
                    adapter.Fill(dt);
                    // Add sequential row number column
                    dt.Columns.Add("_RowNo", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++)
                        dt.Rows[i]["_RowNo"] = i + 1;
                    return dt;
                });
                GridStudents.ItemsSource = table.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error:\n\n" + ex.Message,
                    "DB Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally { if (RegLoadingOverlay != null) RegLoadingOverlay.Visibility = Visibility.Collapsed; }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e) { }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) { }

        // Called from HomePage top bar search
        public void ExternalSearch(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                _ = LoadGridAsync(BASE_QUERY);
            else
                _ = LoadGridAsync(BASE_QUERY +
                    $" WHERE student_id LIKE '%{term}%' OR first_name LIKE '%{term}%'");
        }

        // ── Row click → fill form (async photo load) ─────────────────────────
        private async void GridStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridStudents.SelectedItem is not DataRowView row) return;
            _selectedCell  = row["student_id"]?.ToString() ?? "";
            _selectedLevel = row["level"]?.ToString()      ?? "";

            TxtStudID.Text   = _selectedCell;
            TxtFName.Text    = row["first_name"]?.ToString()        ?? "";
            TxtMName.Text    = row["father_name"]?.ToString()       ?? "";
            TxtLName.Text    = row["grand_father_name"]?.ToString() ?? "";
            // Set Dept combo
            SetComboByContent(CmbDeptID, row["dept_id"]?.ToString() ?? "");
            // Cascade streams then set stream
            _ = LoadStreamsAsync(CmbStreamID, row["dept_id"]?.ToString() ?? "").ContinueWith(_ =>
                Dispatcher.Invoke(() => SetComboByContent(CmbStreamID, row["stream_id"]?.ToString() ?? "")));
            TxtAdmYear.Text  = row["admission_date"]?.ToString()    ?? "";
            TxtWereda.Text   = row["wereda"]?.ToString()            ?? "";
            TxtKebele.Text   = row["kebele"]?.ToString()            ?? "";
            TxtGpa10.Text    = row["gpa_grade_10th"]?.ToString()    ?? "";
            TxtGpa12.Text    = row["gpa_grade_12th"]?.ToString()    ?? "";
            TxtPhone.Text    = row["mobile_number1"]?.ToString()    ?? "";

            SetCombo(CmbLevel,   row["level"]?.ToString()           ?? "1");
            SetCombo(CmbSex,     row["gender"]?.ToString()          ?? "Male");
            SetCombo(CmbProgram, row["program_type"]?.ToString()    ?? "TVET");
            SetCombo(CmbAdmType, row["admission_type"]?.ToString()  ?? "Regular");

            // Update info panel
            if (InfoName != null)
            {
                InfoName.Text   = $"{TxtFName.Text} {TxtMName.Text} {TxtLName.Text}".Trim();
                InfoDept.Text   = GetDeptID();
                InfoEnroll.Text = $"{GetCombo(CmbProgram)} | {GetCombo(CmbAdmType)} | {TxtAdmYear.Text}";
                InfoWereda.Text = TxtWereda.Text;
                InfoKebele.Text = TxtKebele.Text;
                InfoMobile.Text = TxtPhone.Text;
            }

            // Show DB indicator in photo/attachment fields (will be updated after photo loads)
            TxtPhoto.Text  = "[Loading...]";
            TxtAttach.Text = "[Loading...]";
            try
            {
                var sid = _selectedCell;
                var lvl = _selectedLevel;
                // Load paths from DB and display photo from file
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT photo_path, attachment_path FROM ecc_dof_wukrostmarycollege.student_profile " +
                        "WHERE student_id=@id AND level=@lvl", conn);
                    cmd.Parameters.AddWithValue("@id",  sid);
                    cmd.Parameters.AddWithValue("@lvl", lvl);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string pp = reader["photo_path"]?.ToString()      ?? "";
                        string ap = reader["attachment_path"]?.ToString() ?? "";
                        Dispatcher.Invoke(() =>
                        {
                            TxtPhoto.Text  = !string.IsNullOrEmpty(pp) ? pp : "[No photo stored]";
                            TxtAttach.Text = !string.IsNullOrEmpty(ap) ? ap : "[No attachment stored]";

                            // Load photo from file path
                            if (!string.IsNullOrEmpty(pp) && File.Exists(pp))
                            {
                                try
                                {
                                    var bmp = new BitmapImage(new Uri(pp));
                                    ImgPreview.Source           = bmp;
                                    ImgPreview.Visibility       = Visibility.Visible;
                                    PhotoPlaceholder.Visibility = Visibility.Collapsed;
                                }
                                catch { }
                            }
                            else
                            {
                                ImgPreview.Visibility       = Visibility.Collapsed;
                                PhotoPlaceholder.Visibility = Visibility.Visible;
                            }
                        });
                    }
                    conn.Close();
                });
            }
            catch { /* photo load failure is non-critical */ }
        }

        private void SetCombo(ComboBox c, string value)
        {
            foreach (ComboBoxItem item in c.Items)
                if (item.Content?.ToString() == value) { c.SelectedItem = item; return; }
        }

        private void SetComboByContent(ComboBox c, string value)
        {
            foreach (System.Windows.Controls.ComboBoxItem item in c.Items)
                if (item.Content?.ToString() == value) { c.SelectedItem = item; return; }
        }

        // ── Download photo from DB ────────────────────────────────────────────
        private async void DownloadPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedCell))
            { ShowMsg("Select a student from the list first.", false); return; }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"photo_{_selectedCell}",
                Filter   = "Image Files|*.jpg;*.png;*.bmp|All Files|*.*",
                DefaultExt = ".jpg"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                string sid = _selectedCell, lvl = _selectedLevel;
                byte[]? data = await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT photo_path FROM ecc_dof_wukrostmarycollege.student_profile WHERE student_id=@id AND level=@lvl", conn);
                    cmd.Parameters.AddWithValue("@id",  sid);
                    cmd.Parameters.AddWithValue("@lvl", lvl);
                    using var r = cmd.ExecuteReader();
                    string? path = null;
                    if (r.Read()) path = r["photo_path"]?.ToString();
                    conn.Close();
                    return path != null && File.Exists(path) ? File.ReadAllBytes(path) : null;
                });

                if (data == null || data.Length == 0)
                { ShowMsg("No photo found for this student.", false); return; }

                await File.WriteAllBytesAsync(dlg.FileName, data);
                ShowMsg("Photo downloaded successfully!", true);
            }
            catch (Exception ex) { ShowMsg("Download failed: " + ex.Message, false); }
        }

        // ── Download attachment from DB ───────────────────────────────────────
        private async void DownloadAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedCell))
            { ShowMsg("Select a student from the list first.", false); return; }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName   = $"attachment_{_selectedCell}",
                Filter     = "PDF Files|*.pdf|Word Files|*.docx|All Files|*.*",
                DefaultExt = ".pdf"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                string sid = _selectedCell, lvl = _selectedLevel;
                byte[]? data = await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT attachment_path FROM ecc_dof_wukrostmarycollege.student_profile WHERE student_id=@id AND level=@lvl", conn);
                    cmd.Parameters.AddWithValue("@id",  sid);
                    cmd.Parameters.AddWithValue("@lvl", lvl);
                    using var r = cmd.ExecuteReader();
                    string? path = null;
                    if (r.Read()) path = r["attachment_path"]?.ToString();
                    conn.Close();
                    return path != null && File.Exists(path) ? File.ReadAllBytes(path) : null;
                });

                if (data == null || data.Length == 0)
                { ShowMsg("No attachment found for this student.", false); return; }

                await File.WriteAllBytesAsync(dlg.FileName, data);
                ShowMsg("Attachment downloaded successfully!", true);
            }
            catch (Exception ex) { ShowMsg("Download failed: " + ex.Message, false); }
        }

        // ── Browse photo ─────────────────────────────────────────────────────
        private void BrowsePhoto_Click(object sender, RoutedEventArgs e)        {
            var dlg = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp" };
            if (dlg.ShowDialog() != true) return;
            TxtPhoto.Text = dlg.FileName;
            try
            {
                var bmp = new BitmapImage(new Uri(dlg.FileName));
                ImgPreview.Source           = bmp;
                ImgPreview.Visibility       = Visibility.Visible;
                PhotoPlaceholder.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        private void BrowseAttach_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Documents|*.pdf;*.docx;*.doc;*.*" };
            if (dlg.ShowDialog() == true) TxtAttach.Text = dlg.FileName;
        }

        // ── REGISTER ─────────────────────────────────────────────────────────
        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Validation 1: All 18 required fields must be filled
            if (string.IsNullOrWhiteSpace(TxtFName.Text)    || string.IsNullOrWhiteSpace(TxtMName.Text)   ||
                string.IsNullOrWhiteSpace(TxtLName.Text)    || string.IsNullOrWhiteSpace(GetCombo(CmbSex)) ||
                string.IsNullOrWhiteSpace(TxtGpa10.Text)    || string.IsNullOrWhiteSpace(TxtGpa12.Text)   ||
                string.IsNullOrWhiteSpace(TxtAdmYear.Text)  || string.IsNullOrWhiteSpace(GetCombo(CmbProgram)) ||
                string.IsNullOrWhiteSpace(GetCombo(CmbAdmType)) || string.IsNullOrWhiteSpace(TxtWereda.Text) ||
                string.IsNullOrWhiteSpace(TxtKebele.Text)   || string.IsNullOrWhiteSpace(TxtPhone.Text)   ||
                string.IsNullOrWhiteSpace(TxtStudID.Text)   || string.IsNullOrWhiteSpace(GetCombo(CmbLevel)) ||
                string.IsNullOrWhiteSpace(GetDeptID())      || string.IsNullOrWhiteSpace(GetStreamID())      ||
                string.IsNullOrWhiteSpace(TxtPhoto.Text)    || string.IsNullOrWhiteSpace(TxtAttach.Text))
            {
                ShowMsg("There is empty field(s). Please fill all fields!", false);
                return;
            }

            // Validation 3: GPA must be a valid decimal number
            if (!decimal.TryParse(TxtGpa10.Text.Trim(), out decimal gpa10Val) || gpa10Val < 0)
            {
                ShowMsg("GPA Grade 10 must be a valid number (e.g. 3.50).", false);
                return;
            }
            if (!decimal.TryParse(TxtGpa12.Text.Trim(), out decimal gpa12Val) || gpa12Val < 0)
            {
                ShowMsg("GPA Grade 12 must be a valid number (e.g. 3.50).", false);
                return;
            }

            // Validation 2: Duplicate check — same student_id + level
            bool exists = await ExistsAsync(TxtStudID.Text.Trim(), GetCombo(CmbLevel));
            if (exists)
            {
                ShowMsg("There is already a student with the same ID and Level!", false);
                return;
            }

            try
            {
                // Validate file paths exist
                if (!File.Exists(TxtPhoto.Text))
                { ShowMsg("Photo file not found. Please browse and select a valid file.", false); return; }
                if (!File.Exists(TxtAttach.Text))
                { ShowMsg("Attachment file not found. Please attach a valid file.", false); return; }

                // Copy files to app storage folder
                string storageBase = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "StMaryCollege", "students");
                Directory.CreateDirectory(Path.Combine(storageBase, "photos"));
                Directory.CreateDirectory(Path.Combine(storageBase, "attachments"));

                string sid=TxtStudID.Text.Trim(), did=GetDeptID(),
                       stid=GetStreamID(), lvl=GetCombo(CmbLevel),
                       fn=TxtFName.Text.Trim(), mn=TxtMName.Text.Trim(),
                       ln=TxtLName.Text.Trim(), sex=GetCombo(CmbSex),
                       ay=TxtAdmYear.Text.Trim(), pt=GetCombo(CmbProgram),
                       at=GetCombo(CmbAdmType), wr=TxtWereda.Text.Trim(),
                       kb=TxtKebele.Text.Trim(), g10=TxtGpa10.Text.Trim(),
                       g12=TxtGpa12.Text.Trim(), ph=TxtPhone.Text.Trim();

                string photoExt  = Path.GetExtension(TxtPhoto.Text);
                string attachExt = Path.GetExtension(TxtAttach.Text);
                string photoPath  = Path.Combine(storageBase, "photos",      $"{sid}_L{lvl}{photoExt}");
                string attachPath = Path.Combine(storageBase, "attachments", $"{sid}_L{lvl}{attachExt}");

                File.Copy(TxtPhoto.Text,  photoPath,  overwrite: true);
                File.Copy(TxtAttach.Text, attachPath, overwrite: true);

                await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    conn.Open();
                    using var cmd = new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.student_profile " +
                        "(student_id,dept_id,stream_id,level,first_name,father_name,grand_father_name," +
                        "gender,admission_date,program_type,admission_type,wereda,kebele," +
                        "gpa_grade_10th,gpa_grade_12th,mobile_number1,photo_path,attachment_path) " +
                        "VALUES(@sid,@did,@stid,@lvl,@fn,@mn,@ln,@sex,@ay,@pt,@at,@wr,@kb,@g10,@g12,@ph,@pp,@ap)", conn);
                    cmd.Parameters.AddWithValue("@sid",sid);  cmd.Parameters.AddWithValue("@did",did);
                    cmd.Parameters.AddWithValue("@stid",stid);cmd.Parameters.AddWithValue("@lvl",lvl);
                    cmd.Parameters.AddWithValue("@fn",fn);    cmd.Parameters.AddWithValue("@mn",mn);
                    cmd.Parameters.AddWithValue("@ln",ln);    cmd.Parameters.AddWithValue("@sex",sex);
                    cmd.Parameters.AddWithValue("@ay",ay);    cmd.Parameters.AddWithValue("@pt",pt);
                    cmd.Parameters.AddWithValue("@at",at);    cmd.Parameters.AddWithValue("@wr",wr);
                    cmd.Parameters.AddWithValue("@kb",kb);    cmd.Parameters.AddWithValue("@g10",g10);
                    cmd.Parameters.AddWithValue("@g12",g12);  cmd.Parameters.AddWithValue("@ph",ph);
                    cmd.Parameters.AddWithValue("@pp", photoPath);
                    cmd.Parameters.AddWithValue("@ap", attachPath);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                });

                ShowMsg("Saved successfully!", true);
                await LoadGridAsync(BASE_QUERY);
                ClearForm();
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Contains("foreign key")
                    ? "Registration failed: The Department ID or Stream ID does not exist. Please check and try again."
                    : "Connection failed: " + ex.Message;
                ShowMsg(msg, false);
            }
        }

        // ── UPDATE ───────────────────────────────────────────────────────────
        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedCell))
            { ShowMsg("Select a student from the list first.", false); return; }

            // Validate required fields (excluding photo/attachment — optional on update)
            if (string.IsNullOrWhiteSpace(TxtFName.Text)   || string.IsNullOrWhiteSpace(TxtMName.Text)  ||
                string.IsNullOrWhiteSpace(TxtLName.Text)   || string.IsNullOrWhiteSpace(GetDeptID()) ||
                string.IsNullOrWhiteSpace(GetStreamID()))
            { ShowMsg("There is empty field(s). Please fill all fields!", false); return; }

            try
            {
                byte[]? imgBytes    = TxtPhoto.Text  != "" ? File.ReadAllBytes(TxtPhoto.Text)  : null;
                byte[]? attachBytes = TxtAttach.Text != "" ? File.ReadAllBytes(TxtAttach.Text) : null;

                string sid=_selectedCell, lvl=_selectedLevel, did=GetDeptID(),
                       stid=GetStreamID(), fn=TxtFName.Text, mn=TxtMName.Text,
                       ln=TxtLName.Text, sex=GetCombo(CmbSex), ay=TxtAdmYear.Text,
                       pt=GetCombo(CmbProgram), at=GetCombo(CmbAdmType),
                       wr=TxtWereda.Text, kb=TxtKebele.Text,
                       g10=TxtGpa10.Text, g12=TxtGpa12.Text, ph=TxtPhone.Text;

                await Task.Run(() =>
                {
                    string sql = "UPDATE ecc_dof_wukrostmarycollege.student_profile SET " +
                        "dept_id=@did,stream_id=@stid,first_name=@fn,father_name=@mn," +
                        "grand_father_name=@ln,gender=@sex,admission_date=@ay," +
                        "program_type=@pt,admission_type=@at,wereda=@wr,kebele=@kb," +
                        "gpa_grade_10th=@g10,gpa_grade_12th=@g12,mobile_number1=@ph" +
                        (imgBytes    != null ? ",photo=@img"      : "") +
                        (attachBytes != null ? ",attachment=@att" : "") +
                        " WHERE student_id=@sid AND level=@lvl";

                    var conn = _db.GetConnection();
                    conn.Open();
                    using var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@sid", sid);  cmd.Parameters.AddWithValue("@lvl",  lvl);
                    cmd.Parameters.AddWithValue("@did", did);  cmd.Parameters.AddWithValue("@stid", stid);
                    cmd.Parameters.AddWithValue("@fn",  fn);   cmd.Parameters.AddWithValue("@mn",   mn);
                    cmd.Parameters.AddWithValue("@ln",  ln);   cmd.Parameters.AddWithValue("@sex",  sex);
                    cmd.Parameters.AddWithValue("@ay",  ay);   cmd.Parameters.AddWithValue("@pt",   pt);
                    cmd.Parameters.AddWithValue("@at",  at);   cmd.Parameters.AddWithValue("@wr",   wr);
                    cmd.Parameters.AddWithValue("@kb",  kb);   cmd.Parameters.AddWithValue("@g10",  g10);
                    cmd.Parameters.AddWithValue("@g12", g12);  cmd.Parameters.AddWithValue("@ph",   ph);
                    if (imgBytes    != null) cmd.Parameters.AddWithValue("@img", imgBytes);
                    if (attachBytes != null) cmd.Parameters.AddWithValue("@att", attachBytes);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                });

                ShowMsg("Update successful!", true);
                await LoadGridAsync(BASE_QUERY);
            }
            catch (Exception ex) { ShowMsg("Error: " + ex.Message, false); }
        }

        // ── DELETE ───────────────────────────────────────────────────────────
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
                {
                    if (string.IsNullOrEmpty(_selectedCell))
                    { ShowMsg("Select a student from the list first.", false); return; }

                    // Modern confirm dialog
                    var owner = Window.GetWindow(this);
                    var confirm = new ModernDialog(
                        $"Are you sure you want to delete student '{_selectedCell}' (Level {_selectedLevel})?\n\nThis will also delete all related marks, fees and records.",
                        "Confirm Delete", ModernDialog.DialogType.Warning)
                    { Owner = owner };
                    if (confirm.ShowDialog() != true) return;

                    try
                    {
                        string sid = _selectedCell, lvl = _selectedLevel;
                        await Task.Run(() =>
                        {
                            var conn = _db.GetConnection();
                            conn.Open();

                            // Delete child records first to satisfy FK constraints
                            string[] childSql =
                            {
                                "DELETE FROM ecc_dof_wukrostmarycollege.student_mark   WHERE student_id=@id AND level=@lvl",
                                "DELETE FROM ecc_dof_wukrostmarycollege.student_fee    WHERE student_id=@id AND level=@lvl",
                                "DELETE FROM ecc_dof_wukrostmarycollege.attendance     WHERE student_id=@id AND level=@lvl",
                                "DELETE FROM ecc_dof_wukrostmarycollege.dropout        WHERE student_id=@id AND level=@lvl",
                                "DELETE FROM ecc_dof_wukrostmarycollege.coc_record     WHERE student_id=@id AND level=@lvl",
                            };
                            foreach (var sql in childSql)
                            {
                                try
                                {
                                    using var d = new MySqlCommand(sql, conn);
                                    d.Parameters.AddWithValue("@id",  sid);
                                    d.Parameters.AddWithValue("@lvl", lvl);
                                    d.ExecuteNonQuery();
                                }
                                catch { /* table may not exist or no rows */ }
                            }

                            // Now delete the student profile
                            using var cmd = new MySqlCommand(
                                "DELETE FROM ecc_dof_wukrostmarycollege.student_profile WHERE student_id=@id AND level=@lvl", conn);
                            cmd.Parameters.AddWithValue("@id",  sid);
                            cmd.Parameters.AddWithValue("@lvl", lvl);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        });

                        ShowMsg("Delete successful!", true);
                        await LoadGridAsync(BASE_QUERY);
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message.Contains("foreign key")
                            ? "Cannot delete: this student still has related records that could not be removed. Please remove them first."
                            : "Connection failed: " + ex.Message;
                        ShowMsg(msg, false);
                    }
                }


        // ── ENROLL ───────────────────────────────────────────────────────────
        private async void BtnEnroll_Click(object sender, RoutedEventArgs e)
        {
            string sid = TxtStudID.Text.Trim();
            if (string.IsNullOrEmpty(sid)) { ShowMsg("Enter a Student ID to enroll.", false); return; }

            // Validation: both files required
            if (string.IsNullOrEmpty(TxtPhoto.Text) || string.IsNullOrEmpty(TxtAttach.Text))
            {
                ShowMsg("Please select a photo and an attachment!", false);
                return;
            }

            try
            {
                byte[] imgBytes    = File.ReadAllBytes(TxtPhoto.Text);
                byte[] attachBytes = File.ReadAllBytes(TxtAttach.Text);

                string? errorMsg = await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    conn.Open();
                    using var countCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_profile WHERE student_id=@id", conn);
                    countCmd.Parameters.AddWithValue("@id", sid);
                    int count = Convert.ToInt32(countCmd.ExecuteScalar());
                    conn.Close();

                    if (count == 0) return "No such student has been registered!";

                    int nextLvl = count + 1;
                    conn.Open();
                    using var selCmd = new MySqlCommand(
                        "SELECT dept_id,stream_id,first_name,father_name,grand_father_name,gender," +
                        "admission_date,program_type,admission_type,wereda,kebele," +
                        "gpa_grade_10th,gpa_grade_12th,mobile_number1 " +
                        "FROM ecc_dof_wukrostmarycollege.student_profile WHERE student_id=@id AND level=@lvl", conn);
                    selCmd.Parameters.AddWithValue("@id",  sid);
                    selCmd.Parameters.AddWithValue("@lvl", count.ToString());
                    using var r = selCmd.ExecuteReader();
                    if (!r.Read()) { conn.Close(); return "Could not find previous level data."; }

                    string did=r["dept_id"].ToString()!, stid=r["stream_id"].ToString()!,
                           fn=r["first_name"].ToString()!, mn=r["father_name"].ToString()!,
                           ln=r["grand_father_name"].ToString()!, sx=r["gender"].ToString()!,
                           ay=r["admission_date"].ToString()!, pt=r["program_type"].ToString()!,
                           at=r["admission_type"].ToString()!, wr=r["wereda"].ToString()!,
                           kb=r["kebele"].ToString()!, g10=r["gpa_grade_10th"].ToString()!,
                           g12=r["gpa_grade_12th"].ToString()!, ph=r["mobile_number1"].ToString()!;
                    r.Close(); conn.Close();

                    conn.Open();
                    using var insCmd = new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.student_profile " +
                        "(student_id,dept_id,stream_id,level,first_name,father_name,grand_father_name," +
                        "gender,admission_date,program_type,admission_type,wereda,kebele," +
                        "gpa_grade_10th,gpa_grade_12th,mobile_number1,photo,attachment) " +
                        "VALUES(@sid,@did,@stid,@lvl,@fn,@mn,@ln,@sx,@ay,@pt,@at,@wr,@kb,@g10,@g12,@ph,@img,@att)", conn);
                    insCmd.Parameters.AddWithValue("@sid",  sid);   insCmd.Parameters.AddWithValue("@did",  did);
                    insCmd.Parameters.AddWithValue("@stid", stid);  insCmd.Parameters.AddWithValue("@lvl",  nextLvl.ToString());
                    insCmd.Parameters.AddWithValue("@fn",   fn);    insCmd.Parameters.AddWithValue("@mn",   mn);
                    insCmd.Parameters.AddWithValue("@ln",   ln);    insCmd.Parameters.AddWithValue("@sx",   sx);
                    insCmd.Parameters.AddWithValue("@ay",   ay);    insCmd.Parameters.AddWithValue("@pt",   pt);
                    insCmd.Parameters.AddWithValue("@at",   at);    insCmd.Parameters.AddWithValue("@wr",   wr);
                    insCmd.Parameters.AddWithValue("@kb",   kb);    insCmd.Parameters.AddWithValue("@g10",  g10);
                    insCmd.Parameters.AddWithValue("@g12",  g12);   insCmd.Parameters.AddWithValue("@ph",   ph);
                    insCmd.Parameters.AddWithValue("@img",  imgBytes);
                    insCmd.Parameters.AddWithValue("@att",  attachBytes);
                    insCmd.ExecuteNonQuery();
                    conn.Close();
                    return null;
                });

                if (errorMsg != null) { ShowMsg(errorMsg, false); return; }
                ShowMsg("Enrolled to next level!", true);
                await LoadGridAsync(BASE_QUERY);
            }
            catch (Exception ex) { ShowMsg("Error: " + ex.Message, false); }
        }

        // ── FILTER ───────────────────────────────────────────────────────────
        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            string dept = (CmbFDept.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "";
            string lvl  = GetCombo(CmbFLevel);
            string at   = GetCombo(CmbFAdmType);

            if (string.IsNullOrEmpty(dept) || string.IsNullOrEmpty(lvl) || string.IsNullOrEmpty(at))
            {
                ShowMsg("Invalid filter parameters!", false);
                return;
            }
            await LoadGridAsync($"{BASE_QUERY} WHERE dept_id='{dept}' AND level='{lvl}' AND admission_type='{at}'");
        }

        private async void BtnResetFilter_Click(object sender, RoutedEventArgs e)
            => await LoadGridAsync(BASE_QUERY);

        private void BtnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        // ── PRINT — generates PDF via Windows Print Dialog ───────────────────
        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            var view = GridStudents.ItemsSource as System.Data.DataView;
            if (view == null || view.Count == 0)
            { ShowMsg("No data to print.", false); return; }

            // Show "Printing..." blocking modal on separate thread, close after done
            var owner = Window.GetWindow(this);

            // Build document first
            var doc = BuildPrintDocument(view);
            var paginator = ((System.Windows.Documents.IDocumentPaginatorSource)doc).DocumentPaginator;
            paginator.PageSize = new System.Windows.Size(1122.5, 793.7); // A4 landscape 96dpi

            // Show print dialog
            var pd = new System.Windows.Controls.PrintDialog();

            // Try to pre-select Microsoft Print to PDF
            try
            {
                var server = new System.Printing.LocalPrintServer();
                foreach (System.Printing.PrintQueue q in server.GetPrintQueues())
                {
                    if (q.Name.Contains("PDF", StringComparison.OrdinalIgnoreCase))
                    { pd.PrintQueue = q; break; }
                }
            }
            catch { }

            if (pd.ShowDialog() != true) return;

            // Show "Printing..." progress modal
            var printingModal = new ModernDialog(
                "Generating document, please wait...",
                "Printing", ModernDialog.DialogType.Info)
            { Owner = owner };

            // Print on background so UI stays responsive; close modal when done
            Task.Run(() =>
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        pd.PrintDocument(paginator, "Student Registration List");
                        printingModal.Close();
                        ShowMsg($"Sent {view.Count} records to printer.", true);
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        printingModal.Close();
                        ShowMsg("Print failed: " + ex.Message, false);
                    });
                }
            });

            printingModal.ShowDialog(); // blocks UI — closes when print done
        }

        private System.Windows.Documents.FlowDocument BuildPrintDocument(System.Data.DataView? view)
        {
            var doc = new System.Windows.Documents.FlowDocument
            {
                FontFamily  = new System.Windows.Media.FontFamily("Segoe UI"),
                FontSize    = 9,
                PagePadding = new Thickness(30),
                ColumnWidth = double.MaxValue,
                Background  = System.Windows.Media.Brushes.White,
                Foreground  = System.Windows.Media.Brushes.Black
            };

            doc.Blocks.Add(new System.Windows.Documents.Paragraph(
                new System.Windows.Documents.Run("Wukro St. Mary College"))
            { FontSize = 16, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 2) });
            doc.Blocks.Add(new System.Windows.Documents.Paragraph(
                new System.Windows.Documents.Run("Student Registration List"))
            { FontSize = 12, TextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 2) });
            doc.Blocks.Add(new System.Windows.Documents.Paragraph(
                new System.Windows.Documents.Run($"Printed: {DateTime.Now:dd MMM yyyy  HH:mm}"))
            { FontSize = 8, Foreground = System.Windows.Media.Brushes.Gray, TextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 10) });

            var table = new System.Windows.Documents.Table { CellSpacing = 0 };
            string[] headers = { "Student ID", "Dept", "Stream", "Lvl", "First Name", "Father Name", "G/Father", "Gender", "Adm Date", "Program", "Adm Type", "Wereda", "Kebele", "GPA10", "GPA12", "Phone" };
            string[] fields  = { "student_id", "dept_id", "stream_id", "level", "first_name", "father_name", "grand_father_name", "gender", "admission_date", "program_type", "admission_type", "wereda", "kebele", "gpa_grade_10th", "gpa_grade_12th", "mobile_number1" };

            foreach (var _ in headers)
                table.Columns.Add(new System.Windows.Documents.TableColumn());

            var rg = new System.Windows.Documents.TableRowGroup();
            table.RowGroups.Add(rg);

            var hRow = new System.Windows.Documents.TableRow
            { Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(18, 52, 116)) };
            foreach (var h in headers)
                hRow.Cells.Add(new System.Windows.Documents.TableCell(
                    new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(h))
                    { FontWeight = FontWeights.Bold, FontSize = 7.5 })
                { Padding = new Thickness(2, 2, 2, 2), Foreground = System.Windows.Media.Brushes.White });
            rg.Rows.Add(hRow);

            if (view != null)
            {
                bool alt = false;
                foreach (System.Data.DataRowView drv in view)
                {
                    var row = new System.Windows.Documents.TableRow
                    { Background = alt ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 247, 250)) : System.Windows.Media.Brushes.White };
                    alt = !alt;
                    foreach (var f in fields)
                        row.Cells.Add(new System.Windows.Documents.TableCell(
                            new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(drv[f]?.ToString() ?? ""))
                            { FontSize = 7.5 })
                        { Padding = new Thickness(2, 1, 2, 1) });
                    rg.Rows.Add(row);
                }
            }
            doc.Blocks.Add(table);
            return doc;
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private async Task<bool> ExistsAsync(string sid, string lvl)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_profile WHERE student_id=@id AND level=@lvl", conn);
                    cmd.Parameters.AddWithValue("@id",  sid);
                    cmd.Parameters.AddWithValue("@lvl", lvl);
                    int c = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                    return c > 0;
                });
            }
            catch { return false; }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(TxtStudID.Text) || string.IsNullOrWhiteSpace(TxtFName.Text) ||
                string.IsNullOrWhiteSpace(TxtMName.Text)  || string.IsNullOrWhiteSpace(TxtLName.Text) ||
                string.IsNullOrWhiteSpace(GetDeptID()) || string.IsNullOrWhiteSpace(GetStreamID()))
            { ShowMsg("Please fill all required fields (ID, Names, Dept, Stream).", false); return false; }
            return true;
        }

        private string GetCombo(ComboBox c) =>
            (c.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        private void ClearForm()
        {
            TxtStudID.Text=""; TxtFName.Text=""; TxtMName.Text=""; TxtLName.Text="";
            CmbDeptID.SelectedIndex = -1; CmbStreamID.Items.Clear(); TxtAdmYear.Text="";
            TxtWereda.Text=""; TxtKebele.Text=""; TxtGpa10.Text=""; TxtGpa12.Text="";
            TxtPhone.Text=""; TxtPhoto.Text=""; TxtAttach.Text="";
            ImgPreview.Visibility       = Visibility.Collapsed;
            PhotoPlaceholder.Visibility = Visibility.Visible;
            _selectedCell = ""; _selectedLevel = "";
            MsgBorder.Visibility = Visibility.Collapsed;
        }

        private void ShowMsg(string msg, bool success)
        {
            var owner = Window.GetWindow(this);
            if (success)
                ModernDialog.Show(owner, msg, "Success", ModernDialog.DialogType.Success);
            else
                ModernDialog.Show(owner, msg, "Error", ModernDialog.DialogType.Error);

            // Keep inline banner hidden — modal is enough
            MsgBorder.Visibility = Visibility.Collapsed;
        }
    }
}
