using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace CollegeManagementWPF.Views
{
    public partial class AssignPathPage : Page
    {
        private string _photosPath    = "";
        private string _attachPath    = "";
        private string _mlPath        = "";
        private string _assessPath    = "";
        private string _empPhotosPath = "";

        public AssignPathPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            LoadCurrentSettings();
            _ = CheckDbStatusAsync();
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? Color.FromRgb(0x0D,0x1B,0x3E) : Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? Color.FromRgb(0x07,0x10,0x1E) : Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private void LoadCurrentSettings()
        {
            AppSettings.Reload();
            var s = AppSettings.Current;
            _photosPath    = s.PhotosPath;
            _attachPath    = s.AttachmentsPath;
            _mlPath        = s.MarkListsPath;
            _assessPath    = s.AssessmentsPath;
            _empPhotosPath = s.EmployeePhotosPath;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            TxtPhotosPath.Text  = _photosPath;
            TxtAttachPath.Text  = _attachPath;
            TxtMlPath.Text      = _mlPath;
            TxtAssessPath.Text  = _assessPath;
            TxtEmpPhotosPath.Text = _empPhotosPath;
        }

        private void BtnBrowsePhotos_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog { Title = "Select Photos folder", InitialDirectory = _photosPath };
            if (dlg.ShowDialog() == true) { _photosPath = dlg.FolderName; UpdateDisplay(); }
        }

        private void BtnBrowseAttach_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog { Title = "Select Attachments folder", InitialDirectory = _attachPath };
            if (dlg.ShowDialog() == true) { _attachPath = dlg.FolderName; UpdateDisplay(); }
        }

        private void BtnBrowseMl_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog { Title = "Select Mark List Documents folder", InitialDirectory = _mlPath };
            if (dlg.ShowDialog() == true) { _mlPath = dlg.FolderName; UpdateDisplay(); }
        }

        private void BtnBrowseAssess_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog { Title = "Select Assessment Documents folder", InitialDirectory = _assessPath };
            if (dlg.ShowDialog() == true) { _assessPath = dlg.FolderName; UpdateDisplay(); }
        }

        private void BtnBrowseEmpPhotos_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog { Title = "Select Employee Photos folder", InitialDirectory = _empPhotosPath };
            if (dlg.ShowDialog() == true) { _empPhotosPath = dlg.FolderName; UpdateDisplay(); }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_photosPath) || string.IsNullOrWhiteSpace(_attachPath))
            {
                ModernDialog.Show(Window.GetWindow(this),
                    "Please select folders for Photos and Attachments.", "Error", ModernDialog.DialogType.Error);
                return;
            }

            var s = AppSettings.Current;
            s.PhotosPath         = _photosPath;
            s.AttachmentsPath    = _attachPath;
            s.MarkListsPath      = _mlPath;
            s.AssessmentsPath    = _assessPath;
            s.EmployeePhotosPath = _empPhotosPath;
            s.Save();

            Directory.CreateDirectory(s.PhotosPath);
            Directory.CreateDirectory(s.AttachmentsPath);
            if (!string.IsNullOrWhiteSpace(s.MarkListsPath))   Directory.CreateDirectory(s.MarkListsPath);
            if (!string.IsNullOrWhiteSpace(s.AssessmentsPath)) Directory.CreateDirectory(s.AssessmentsPath);
            if (!string.IsNullOrWhiteSpace(s.EmployeePhotosPath)) Directory.CreateDirectory(s.EmployeePhotosPath);

            ModernDialog.Show(Window.GetWindow(this),
                $"Configuration saved!\n\nPhotos: {s.PhotosPath}\nAttachments: {s.AttachmentsPath}\nMark Lists: {s.MarkListsPath}\nAssessments: {s.AssessmentsPath}\nEmployee Photos: {s.EmployeePhotosPath}",
                "Saved", ModernDialog.DialogType.Success);

            _ = CheckDbStatusAsync();
        }

        private async Task CheckDbStatusAsync()
        {
            DbStatusIcon.Text       = "⏳";
            DbStatusText.Text       = "Checking database...";
            DbStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0x5E,0xAA,0xA8));
            DbStatusBorder.Background = new SolidColorBrush(Color.FromRgb(0x0A,0x1F,0x14));

            var result = await Task.Run(() =>
            {
                try
                {
                    var db = new DBConnect();
                    using var conn = db.GetConnection();
                    conn.Open();

                    bool tableExists = Convert.ToInt32(new MySqlCommand(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                        "WHERE TABLE_SCHEMA='ecc_dof_wukrostmarycollege' AND TABLE_NAME='path_config'", conn
                        ).ExecuteScalar()) > 0;

                    if (!tableExists) return (false, "", "", "", "", "");

                    string photos = "", attach = "", ml = "", assess = "", empPhotos = "";
                    using var r = new MySqlCommand(
                        "SELECT config_key, config_value FROM ecc_dof_wukrostmarycollege.path_config " +
                        "WHERE config_key IN ('photos_path','attachments_path','mark_list_path','assessments_path','employee_photos_path')", conn).ExecuteReader();
                    while (r.Read())
                    {
                        string k = r["config_key"]?.ToString()   ?? "";
                        string v = r["config_value"]?.ToString() ?? "";
                        if (k == "photos_path")          photos    = v;
                        if (k == "attachments_path")     attach    = v;
                        if (k == "mark_list_path")       ml        = v;
                        if (k == "assessments_path")     assess    = v;
                        if (k == "employee_photos_path") empPhotos = v;
                    }
                    return (true, photos, attach, ml, assess, empPhotos);
                }
                catch { return (false, "", "", "", "", ""); }
            });

            if (!result.Item1)
            {
                DbStatusIcon.Text         = "✗";
                DbStatusText.Text         = "path_config table not found — will be created on first Save.";
                DbStatusText.Foreground   = new SolidColorBrush(Color.FromRgb(0xF5,0x9E,0x0B));
                DbStatusBorder.Background = new SolidColorBrush(Color.FromRgb(0x1A,0x10,0x00));
            }
            else
            {
                string ph  = string.IsNullOrEmpty(result.Item2) ? "(not set)" : result.Item2;
                string at  = string.IsNullOrEmpty(result.Item3) ? "(not set)" : result.Item3;
                string ml  = string.IsNullOrEmpty(result.Item4) ? "(not set)" : result.Item4;
                string as2 = string.IsNullOrEmpty(result.Item5) ? "(not set)" : result.Item5;
                string ep  = string.IsNullOrEmpty(result.Item6) ? "(not set)" : result.Item6;
                DbStatusIcon.Text         = "✓";
                DbStatusText.Text         = $"DB ✓  photos: {ph}  |  attachments: {at}  |  mark lists: {ml}  |  assessments: {as2}  |  emp photos: {ep}";
                DbStatusText.Foreground   = new SolidColorBrush(Color.FromRgb(0x2D,0xD4,0xBF));
                DbStatusBorder.Background = new SolidColorBrush(Color.FromRgb(0x05,0x15,0x0F));
            }
        }
    }
}
