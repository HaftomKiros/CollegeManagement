using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace CollegeManagementWPF.Views
{
    public partial class AssignPathPage : Page
    {
        private string _selectedPath   = "";
        private string _selectedMlPath = "";

        public AssignPathPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            LoadCurrentSettings();
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
            if (FindName("PreviewBg") is System.Windows.Media.SolidColorBrush pb)
                pb.Color = dark ? System.Windows.Media.Color.FromRgb(0x05,0x0B,0x16) : System.Windows.Media.Color.FromRgb(0xF1,0xF9,0xF8);
        }

        private void LoadCurrentSettings()
        {
            var s = AppSettings.Current;
            _selectedPath   = s.StorageBasePath;
            _selectedMlPath = s.MarkListBasePath;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            TxtBasePath.Text   = _selectedPath;
            TxtPhotosPath.Text = Path.Combine(_selectedPath, "photos");
            TxtAttachPath.Text = Path.Combine(_selectedPath, "attachments");
            TxtMlBasePath.Text = _selectedMlPath;
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog
            {
                Title = "Select base storage folder for student files",
                InitialDirectory = _selectedPath
            };
            if (dlg.ShowDialog() == true) { _selectedPath = dlg.FolderName; UpdateDisplay(); }
        }

        private void BtnBrowseMl_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog
            {
                Title = "Select storage folder for mark list documents",
                InitialDirectory = _selectedMlPath
            };
            if (dlg.ShowDialog() == true) { _selectedMlPath = dlg.FolderName; UpdateDisplay(); }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedPath))
            {
                ModernDialog.Show(Window.GetWindow(this), "Please select a student files folder.", "Error", ModernDialog.DialogType.Error);
                return;
            }

            var s = AppSettings.Current;
            s.StorageBasePath   = _selectedPath;
            s.MarkListBasePath  = _selectedMlPath;
            s.Save();

            Directory.CreateDirectory(s.PhotosPath);
            Directory.CreateDirectory(s.AttachmentsPath);
            if (!string.IsNullOrWhiteSpace(_selectedMlPath))
                Directory.CreateDirectory(s.MarkListsPath);

            ModernDialog.Show(Window.GetWindow(this),
                $"Configuration saved!\n\nPhotos: {s.PhotosPath}\nAttachments: {s.AttachmentsPath}\nMark Lists: {s.MarkListsPath}",
                "Saved", ModernDialog.DialogType.Success);
        }
    }
}
