using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using WpfButton = System.Windows.Controls.Button;
using WpfTextBlock = System.Windows.Controls.TextBlock;

namespace CollegeManagementWPF.Views
{
    public partial class HomePage : FluentWindow
    {
        // Section map: button name → (panel, chevron)
        private readonly (string Panel, string Chevron)[] _sections =
        {
            ("PanStudents", "ChevStudents"),
            ("PanDepts",    "ChevDepts"),
            ("PanEmp",      "ChevEmp"),
            ("PanAlumni",   "ChevAlumni"),
            ("PanLib",      "ChevLib"),
            ("PanReports",  "ChevReports"),
            ("PanAdmins",   "ChevAdmins"),
        };

        private readonly System.Collections.Generic.Dictionary<string, string> _btnToPanel
            = new System.Collections.Generic.Dictionary<string, string>
        {
            {"BtnStudents", "PanStudents"}, {"BtnDepts", "PanDepts"},
            {"BtnEmp",      "PanEmp"},      {"BtnAlumni","PanAlumni"},
            {"BtnLib",      "PanLib"},      {"BtnReports","PanReports"},
            {"BtnAdmins",   "PanAdmins"}, {"BtnConfig",  "PanConfig"},
        };

        private readonly System.Collections.Generic.Dictionary<string, string> _btnToChev
            = new System.Collections.Generic.Dictionary<string, string>
        {
            {"BtnStudents","ChevStudents"},{"BtnDepts","ChevDepts"},
            {"BtnEmp","ChevEmp"},          {"BtnAlumni","ChevAlumni"},
            {"BtnLib","ChevLib"},          {"BtnReports","ChevReports"},
            {"BtnAdmins","ChevAdmins"}, {"BtnConfig","ChevConfig"},
        };

        public HomePage()
        {
            InitializeComponent();
            ContentFrame.Navigate(new DashboardPage());
            TxtPageSubTitle.Visibility = Visibility.Collapsed;
            ThemeManager.ThemeChanged += ApplyThemeToShell;
            ApplyThemeToShell();
        }

        private void ApplyThemeToShell()
        {
            bool dark = ThemeManager.IsDark;

            // ── Title bar buttons always white (title bar is always blue) ─────
            if (AppTitleBar != null)
            {
                AppTitleBar.Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Colors.White);
                AppTitleBar.ButtonsForeground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Colors.White);
            }
            // Title bar gradient
            if (FindName("TitleG1") is System.Windows.Media.GradientStop tg1)
                tg1.Color = dark
                    ? System.Windows.Media.Color.FromRgb(0x1A,0x1F,0x8C)
                    : System.Windows.Media.Color.FromRgb(0x1D,0x40,0xAF);
            if (FindName("TitleG2") is System.Windows.Media.GradientStop tg2)
                tg2.Color = dark
                    ? System.Windows.Media.Color.FromRgb(0x0F,0x34,0x60)
                    : System.Windows.Media.Color.FromRgb(0x1E,0x3A,0x8A);

            // Window background
            Background = new System.Windows.Media.SolidColorBrush(ThemeManager.WinBg);

            // ── Sidebar ──────────────────────────────────────────────────────
            if (FindName("SidebarBorder") is System.Windows.Controls.Border sb)
                sb.Background = new System.Windows.Media.SolidColorBrush(
                    dark ? System.Windows.Media.Color.FromRgb(0x0A,0x16,0x28)
                         : System.Windows.Media.Colors.White);

            // Sidebar header (logo area)
            if (FindName("SidebarHeaderBorder") is System.Windows.Controls.Border shb)
                shb.Background = new System.Windows.Media.SolidColorBrush(
                    dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1F,0x3C)
                         : System.Windows.Media.Color.FromRgb(0xF8,0xFA,0xFF));

            // College name text
            if (FindName("TxtCollegeName") is WpfTextBlock cn)
                cn.Foreground = new System.Windows.Media.SolidColorBrush(
                    dark ? System.Windows.Media.Colors.White
                         : System.Windows.Media.Color.FromRgb(0x0F,0x17,0x2A));

            // College subtitle text
            if (FindName("TxtCollegeSub") is WpfTextBlock cs)
                cs.Foreground = new System.Windows.Media.SolidColorBrush(
                    dark ? System.Windows.Media.Color.FromRgb(0x2A,0x4A,0x7A)
                         : System.Windows.Media.Color.FromRgb(0x64,0x74,0x8B));

            // Section button text + background
            var sectionBg  = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1F,0x3C)
                                  : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            var sectionFg  = dark ? System.Windows.Media.Color.FromRgb(0xC8,0xDC,0xF0)
                                  : System.Windows.Media.Color.FromRgb(0x0F,0x17,0x2A);
            var sectionHov = dark ? System.Windows.Media.Color.FromRgb(0x14,0x30,0x5A)
                                  : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);

            foreach (var name in new[]{"BtnStudents","BtnDepts","BtnEmp","BtnAlumni","BtnLib","BtnReports","BtnAdmins","BtnConfig"})
                if (FindName(name) is WpfButton bb)
                {
                    bb.Background = new System.Windows.Media.SolidColorBrush(sectionBg);
                    bb.Foreground = new System.Windows.Media.SolidColorBrush(sectionFg);
                }

            // Chevron text blocks
            var chevFg = dark ? System.Windows.Media.Color.FromRgb(0x2A,0x4A,0x7A)
                              : System.Windows.Media.Color.FromRgb(0x94,0xA3,0xB8);
            foreach (var name in new[]{"ChevStudents","ChevDepts","ChevEmp","ChevAlumni","ChevLib","ChevReports","ChevAdmins"})
                if (FindName(name) is WpfTextBlock ct)
                    ct.Foreground = new System.Windows.Media.SolidColorBrush(chevFg);

            // Sub-button text
            var subFgColor = dark ? System.Windows.Media.Color.FromRgb(0x6B,0x8C,0xAE)
                                  : System.Windows.Media.Color.FromRgb(0x33,0x41,0x55);
            foreach (var panelName in new[]{"PanStudents","PanDepts","PanEmp","PanAlumni","PanLib","PanReports","PanAdmins","PanConfig"})
                if (FindName(panelName) is StackPanel pan)
                    foreach (var child in pan.Children)
                        if (child is WpfButton subBtn)
                        {
                            subBtn.Foreground = new System.Windows.Media.SolidColorBrush(subFgColor);
                            subBtn.Background = new System.Windows.Media.SolidColorBrush(
                                dark ? System.Windows.Media.Colors.Transparent
                                     : System.Windows.Media.Colors.Transparent);
                        }

            // Separator lines
            var sepColor = dark
                ? System.Windows.Media.Color.FromRgb(0x1A,0x3A,0x5A)
                : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
            foreach (var n in new[]{"SepBrush1","SepBrush2","SepBrush3","SepBrush4","SepBrush5","SepBrush6","SepBrush7"})
                if (FindName(n) is System.Windows.Media.SolidColorBrush sb2) sb2.Color = sepColor;

            // ── Top content bar ──────────────────────────────────────────────
            if (FindName("TopBarBorder") is System.Windows.Controls.Border tb)
            {
                if (dark)
                    tb.Background = new System.Windows.Media.LinearGradientBrush(
                        System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E),
                        System.Windows.Media.Color.FromRgb(0x0A,0x16,0x28), 0);
                else
                    tb.Background = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(0xF8,0xFA,0xFF));
            }

            if (TxtPageTitle != null)
                TxtPageTitle.Foreground = new System.Windows.Media.SolidColorBrush(
                    dark ? System.Windows.Media.Colors.White
                         : System.Windows.Media.Color.FromRgb(0x0F,0x17,0x2A));

            // ── Content frame ────────────────────────────────────────────────
            if (ContentFrame != null)
                ContentFrame.Background = new System.Windows.Media.SolidColorBrush(
                    dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E)
                         : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9));

            // ── Search box ────────────────────────────────────────────────────
            if (FindName("SearchBorderColor") is System.Windows.Media.SolidColorBrush sbc)
                sbc.Color = dark ? System.Windows.Media.Color.FromRgb(0x1E,0x3A,0x6A)
                                 : System.Windows.Media.Color.FromRgb(0x93,0xC5,0xFD);
            if (FindName("SearchHintFg") is System.Windows.Media.SolidColorBrush shf)
                shf.Color = dark ? System.Windows.Media.Color.FromRgb(0x2A,0x4A,0x7A)
                                 : System.Windows.Media.Color.FromRgb(0x94,0xA3,0xB8);
            if (FindName("SearchFg") is System.Windows.Media.SolidColorBrush sf)
                sf.Color = dark ? System.Windows.Media.Colors.White
                                : System.Windows.Media.Color.FromRgb(0x0F,0x17,0x2A);

            // ── Toggle button ────────────────────────────────────────────────
            if (BtnThemeToggle != null)
            {
                BtnThemeToggle.ToolTip = ThemeManager.ToggleTip;
                if (BtnThemeToggle.Template?.FindName("ThemeIcon", BtnThemeToggle)
                        is System.Windows.Controls.TextBlock ic)
                    ic.Text = ThemeManager.ToggleIcon;
                // Update toggle button border/bg for light mode
                if (BtnThemeToggle.Template?.FindName("bd", BtnThemeToggle)
                        is System.Windows.Controls.Border tbBd)
                {
                    tbBd.Background = new System.Windows.Media.SolidColorBrush(
                        dark ? System.Windows.Media.Color.FromRgb(0x0A,0x18,0x30)
                             : System.Windows.Media.Color.FromRgb(0xEF,0xF6,0xFF));
                    tbBd.BorderBrush = new System.Windows.Media.SolidColorBrush(
                        dark ? System.Windows.Media.Color.FromRgb(0x1E,0x3A,0x6A)
                             : System.Windows.Media.Color.FromRgb(0x93,0xC5,0xFD));
                }
            }

            // ── Administrator badge ───────────────────────────────────────────
            if (FindName("AdminBadgeBg") is System.Windows.Media.SolidColorBrush abBg)
                abBg.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E)
                                  : System.Windows.Media.Color.FromRgb(0xEF,0xF6,0xFF);
            if (FindName("AdminBadgeBorder") is System.Windows.Media.SolidColorBrush abBrd)
                abBrd.Color = dark ? System.Windows.Media.Color.FromRgb(0x1A,0x3A,0x6B)
                                   : System.Windows.Media.Color.FromRgb(0x93,0xC5,0xFD);
            if (FindName("AdminLabelFg") is System.Windows.Media.SolidColorBrush alf)
                alf.Color = dark ? System.Windows.Media.Color.FromRgb(0x88,0xAA,0xCC)
                                 : System.Windows.Media.Color.FromRgb(0x1E,0x40,0xAF);

            // ── Reload current page ──────────────────────────────────────────
            if (ContentFrame?.Content != null)
                ContentFrame.Navigate(ContentFrame.Content switch
                {
                    DashboardPage           => (object)new DashboardPage(),
                    StudentRegistrationPage => new StudentRegistrationPage(),
                    StudentMarksPage        => new StudentMarksPage(),
                    StudentFeesPage         => new StudentFeesPage(),
                    DropoutPage             => new DropoutPage(),
                    COCRecordPage           => new COCRecordPage(),
                    DepartmentsPage         => new DepartmentsPage(),
                    StreamsPage             => new StreamsPage(),
                    LevelsPage              => new LevelsPage(),
                    CoursesPage             => new CoursesPage(),
                    AlumniPage              => new AlumniPage(),
                    EmployeePage            => new EmployeePage(),
                    LibraryPage             => new LibraryPage(),
                    AssignPathPage       => new AssignPathPage(),
                    MigrationPage           => new MigrationPage(),
                    _                       => ContentFrame.Content
                });
        }

        private void BtnThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.Toggle();
        }

        // Toggle section expand/collapse — accordion: close others when one opens
        private void Section_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as WpfButton;
            if (btn == null) return;
            string btnName = btn.Name;
            if (!_btnToPanel.ContainsKey(btnName)) return;

            var panel = FindName(_btnToPanel[btnName]) as StackPanel;
            var chev  = FindName(_btnToChev[btnName]) as WpfTextBlock;
            if (panel == null) return;

            bool isCurrentlyOpen = panel.Visibility == Visibility.Visible;

            // Close ALL sections first
            foreach (var entry in _btnToPanel)
            {
                var p = FindName(entry.Value) as StackPanel;
                var c = FindName(_btnToChev[entry.Key]) as WpfTextBlock;
                if (p != null) p.Visibility = Visibility.Collapsed;
                if (c != null) c.Text = "⌄";
            }

            // If it was closed, open it now
            if (!isCurrentlyOpen)
            {
                panel.Visibility = Visibility.Visible;
                if (chev != null) chev.Text = "⌃";
            }
        }

        // Navigate on sub-item click
        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as WpfButton;
            if (btn == null) return;
            string tag = btn.Tag?.ToString() ?? "";
            TxtPageTitle.Text = tag switch
            {
                "StudentRegistration" => "Student Registration",
                "StudentMarks"        => "Student Marks",
                "StudentFees"         => "Student Fees",
                "DropoutStudents"     => "Dropout Students",
                "COCRecord"           => "COC Record",
                _                     => tag
            };

            // Set subtitle
            string sub = tag switch
            {
                "StudentRegistration" => "Register, Update, Enroll, Delete Student",
                "StudentMarks"        => "Add, Update, Delete Student Marks",
                "StudentFees"         => "Record, Update, Delete Student Fees",
                "DropoutStudents"     => "Manage Dropout Student Records",
                "COCRecord"           => "Manage COC Records",
                "Departments"         => "Add, Update, Delete Departments",
                "Streams"             => "Add, Update, Delete Streams",
                "Levels"              => "Add, Update, Delete Levels",
                "Courses"             => "Add, Update, Delete Courses",
                "RegisterEmployee"    => "Register, Update, Delete Employees",
                "RegisterAlumni"      => "Register, Update, Delete Alumni",
                "Library"             => "Manage Library Records",
                "ManageAccounts"  => "Manage Accounts", "RolesPermissions" => "Roles & Permissions",
                "Dashboard"           => "",
                _                     => ""
            };
            TxtPageSubTitle.Text       = sub;
            TxtPageSubTitle.Visibility = string.IsNullOrEmpty(sub)
                ? Visibility.Collapsed : Visibility.Visible;

            // Show search bar only for Student Registration
            bool hasSearch = tag == "StudentRegistration";
            SearchPanel.Visibility = hasSearch ? Visibility.Visible : Visibility.Collapsed;
            if (TopSearchBox != null) TopSearchBox.Text = "";

            ContentFrame.Navigate(tag switch
            {
                "TVETTranscript"       => new TVETTranscriptPage(),
                "MarkList"            => new MarkListPage(),
                "AttendanceSheet"     => new AttendanceSheetPage(),
                "COCList"             => new COCListPage(),
                "Dashboard"           => (object)new DashboardPage(),
                "StudentRegistration" => new StudentRegistrationPage(),
                "StudentMarks"        => new StudentMarksPage(),
                "StudentFees"         => new StudentFeesPage(),
                "DropoutStudents"     => new DropoutPage(),
                "COCRecord"           => new COCRecordPage(),
                "Departments"         => new DepartmentsPage(),
                "Streams"             => new StreamsPage(),
                "Levels"              => new LevelsPage(),
                "Courses"             => new CoursesPage(),
                "RegisterEmployee"    => new EmployeePage(),
                "RegisterAlumni"      => new AlumniPage(),
                "Library"             => new LibraryPage(),
                "ManageAccounts"      => new ManageAccountsPage(),
                "RolesPermissions"    => new RolesPermissionsPage(),
                "AssignPath"         => new AssignPathPage(),
                "Migration"           => new MigrationPage(),
                _                     => new PlaceholderPage(TxtPageTitle.Text)
            });
        }

        private void TopSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (TopSearchHint != null)
                TopSearchHint.Visibility = string.IsNullOrEmpty(TopSearchBox.Text)
                    ? Visibility.Visible : Visibility.Collapsed;

            // Pass search text to current page if it supports it
            if (ContentFrame.Content is StudentRegistrationPage srp)
                srp.ExternalSearch(TopSearchBox.Text);
        }

        private void TopSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content is StudentRegistrationPage srp)
                srp.ExternalSearch(TopSearchBox.Text);
        }

        private void BtnSignOut_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        // Called by DashboardPage quick access buttons — reuses same nav logic
        public void NavigateTo(string tag)
        {
            // Simulate a NavButton_Click by delegating to the same handler logic
            var fakeBtn = new System.Windows.Controls.Button { Tag = tag };
            NavButton_Click(fakeBtn, new RoutedEventArgs());
        }
    }
}
