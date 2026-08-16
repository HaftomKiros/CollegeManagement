using System.Windows;
using System.Windows.Media;

namespace CollegeManagementWPF.Views
{
    public partial class ModernDialog : Window
    {
        public enum DialogType { Success, Error, Warning, Info }

        public ModernDialog(string message, string title, DialogType type = DialogType.Info)
        {
            InitializeComponent();
            MessageText.Text = message;
            TitleText.Text   = title;

            switch (type)
            {
                case DialogType.Success:
                    AccentBar.Background  = new SolidColorBrush(Color.FromRgb(34, 197, 94));
                    IconBadge.Background  = new SolidColorBrush(Color.FromArgb(40, 34, 197, 94));
                    IconText.Text         = "✓";
                    IconText.Foreground   = new SolidColorBrush(Color.FromRgb(74, 222, 128));
                    OkButton.Background   = new SolidColorBrush(Color.FromRgb(22, 163, 74));
                    break;

                case DialogType.Error:
                    AccentBar.Background  = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                    IconBadge.Background  = new SolidColorBrush(Color.FromArgb(40, 239, 68, 68));
                    IconText.Text         = "✕";
                    IconText.Foreground   = new SolidColorBrush(Color.FromRgb(248, 113, 113));
                    OkButton.Background   = new SolidColorBrush(Color.FromRgb(185, 28, 28));
                    break;

                case DialogType.Warning:
                    AccentBar.Background  = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                    IconBadge.Background  = new SolidColorBrush(Color.FromArgb(40, 245, 158, 11));
                    IconText.Text         = "⚠";
                    IconText.Foreground   = new SolidColorBrush(Color.FromRgb(252, 211, 77));
                    OkButton.Background   = new SolidColorBrush(Color.FromRgb(180, 83, 9));
                    break;

                default: // Info
                    AccentBar.Background  = new SolidColorBrush(Color.FromRgb(59, 130, 246));
                    IconBadge.Background  = new SolidColorBrush(Color.FromArgb(40, 59, 130, 246));
                    IconText.Text         = "ℹ";
                    IconText.Foreground   = new SolidColorBrush(Color.FromRgb(147, 197, 253));
                    OkButton.Background   = new SolidColorBrush(Color.FromRgb(29, 78, 216));
                    break;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        // Static helper — call from anywhere
        public static void Show(Window owner, string message, string title,
                                DialogType type = DialogType.Info)
        {
            var dlg = new ModernDialog(message, title, type) { Owner = owner };
            dlg.ShowDialog();
        }
    }
}
