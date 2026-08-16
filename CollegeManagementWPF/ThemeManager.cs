using System;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace CollegeManagementWPF
{
    public static class ThemeManager
    {
        private static bool _isDark = true;
        public static bool IsDark => _isDark;
        public static event Action? ThemeChanged;

        // Dark palette
        private static readonly Color DarkWin        = Color.FromRgb(0x0D, 0x1B, 0x3E);
        private static readonly Color DarkSidebar1   = Color.FromRgb(0x0A, 0x16, 0x28);
        private static readonly Color DarkSidebar2   = Color.FromRgb(0x07, 0x0E, 0x1C);
        private static readonly Color DarkTopBar1    = Color.FromRgb(0x0D, 0x1B, 0x3E);
        private static readonly Color DarkTopBar2    = Color.FromRgb(0x0A, 0x16, 0x28);
        private static readonly Color DarkContent    = Color.FromRgb(0x07, 0x0D, 0x1A);
        private static readonly Color DarkTopBorder  = Color.FromRgb(0x1A, 0x3A, 0x6B);

        // Light palette
        private static readonly Color LightWin       = Color.FromRgb(0xF1, 0xF5, 0xF9);
        private static readonly Color LightSidebar1  = Color.FromRgb(0x1E, 0x3A, 0x5F);
        private static readonly Color LightSidebar2  = Color.FromRgb(0x15, 0x2A, 0x4A);
        private static readonly Color LightTopBar1   = Color.FromRgb(0x1E, 0x40, 0x7C);
        private static readonly Color LightTopBar2   = Color.FromRgb(0x15, 0x30, 0x60);
        private static readonly Color LightContent   = Color.FromRgb(0xF8, 0xFA, 0xFC);
        private static readonly Color LightTopBorder = Color.FromRgb(0x93, 0xC5, 0xFD);

        public static void Toggle()
        {
            _isDark = !_isDark;
            Apply();
        }

        public static void Apply()
        {
            // Switch WpfUI theme engine
            ApplicationThemeManager.Apply(
                _isDark ? ApplicationTheme.Dark : ApplicationTheme.Light);

            // Update sub-button hover colors (replace, not mutate — XAML brushes are frozen)
            Application.Current.Resources["SubBtnHoverBg"] = new SolidColorBrush(
                _isDark ? Color.FromRgb(0x11,0x22,0x40) : Color.FromRgb(0x1E,0x3A,0x8A));
            Application.Current.Resources["SubBtnHoverFg"] = new SolidColorBrush(Colors.White);
            Application.Current.Resources["SectionBtnHoverBg"] = new SolidColorBrush(
                _isDark ? Color.FromRgb(0x14,0x30,0x5A) : Color.FromRgb(0x1E,0x3A,0x8A));

            ThemeChanged?.Invoke();
        }

        public static Color WinBg        => _isDark ? DarkWin       : LightWin;
        public static Color Sidebar1     => _isDark ? DarkSidebar1  : LightSidebar1;
        public static Color Sidebar2     => _isDark ? DarkSidebar2  : LightSidebar2;
        public static Color TopBar1      => _isDark ? DarkTopBar1   : LightTopBar1;
        public static Color TopBar2      => _isDark ? DarkTopBar2   : LightTopBar2;
        public static Color ContentBg    => _isDark ? DarkContent   : LightContent;
        public static Color TopBorder    => _isDark ? DarkTopBorder : LightTopBorder;
        public static string ToggleIcon  => _isDark ? "\uE708"      : "\uE706";  // moon : sun
        public static string ToggleTip   => _isDark ? "Switch to Light Mode" : "Switch to Dark Mode";
    }
}
