using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace CollegeManagementWPF.Views
{
    public partial class AssignPathPage : Page
    {
        private string _selectedPath = "";

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
            var settings = AppSettings.Current;
            _selectedPath = settings.StorageBasePath;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            TxtBasePath.Text   = _selectedPath;
            TxtPhotosPath.Text = Path.Combine(_selectedPath, "photos");
            TxtAttachPath.Text = Path.Combine(_selectedPath, "attachments");
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Use OpenFolderDialog (WPF .NET 6+)
            var dlg = new OpenFolderDialog
            {
                Title        = "Select base storage folder for student files",
                InitialDirectory = _selectedPath
            };
            if (dlg.ShowDialog() == true)
            {
                _selectedPath = dlg.FolderName;
                UpdateDisplay();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedPath))
            {
                ModernDialog.Show(Window.GetWindow(this), "Please select a folder first.", "Error", ModernDialog.DialogType.Error);
                return;
            }

            var settings = AppSettings.Current;
            settings.StorageBasePath = _selectedPath;
            settings.Save();

            Directory.CreateDirectory(settings.PhotosPath);
            Directory.CreateDirectory(settings.AttachmentsPath);

            ModernDialog.Show(Window.GetWindow(this),
                $"Storage path saved!\n\nPhotos: {settings.PhotosPath}\nAttachments: {settings.AttachmentsPath}",
                "Saved", ModernDialog.DialogType.Success);
        }
    }
}
