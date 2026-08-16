using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CollegeManagementWPF.Views
{
    public partial class DashboardPage : Page
    {
        private readonly DBConnect _db = new DBConnect();

        public DashboardPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            Loaded += async (s, e) => await LoadStatsAsync();
        }

        // ── Theme ─────────────────────────────────────────────────────────────
        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;

            // Page background
            if (FindName("PageBg") is SolidColorBrush bg)
                bg.Color = dark ? Color.FromRgb(0x07,0x10,0x1E) : Color.FromRgb(0xF1,0xF5,0xF9);

            // Welcome banner
            SetGradient("BannerG1", dark ? "#1A2F6E" : "#1E40AF");
            SetGradient("BannerG2", dark ? "#0D0B3A" : "#1E3A8A");
            SetBrush("BannerBorderBrush", dark ? "#3E2A7A" : "#3B82F6");
            SetBrush("BannerSubFg",       dark ? "#9988BB" : "#BFDBFE");

            // QA section label
            SetBrush("QALabelFg", dark ? "White" : "#0F172A");

            // Stat cards
            ApplyStatCard("C1", dark, "#0D1F4A","#0A1635","#1E3A8A","#6B9ADA","#3A6AAA","#1A3A6A","#60A5FA",
                                      "#EFF6FF","#FFFFFF","#BFDBFE","#1D4ED8","#3B82F6","#DBEAFE","#1D4ED8", TxtStudents, dark);
            ApplyStatCard("C2", dark, "#0D2A1A","#071A10","#166534","#22AA66","#166534","#14532D","#4ADE80",
                                      "#F0FDF4","#FFFFFF","#86EFAC","#15803D","#16A34A","#DCFCE7","#15803D", TxtCourses, dark);
            ApplyStatCard("C3", dark, "#2A1E08","#1A1205","#92400E","#D97706","#92400E","#78350F","#FCD34D",
                                      "#FFFBEB","#FFFFFF","#FDE68A","#B45309","#D97706","#FEF3C7","#92400E", TxtInstructors, dark);
            ApplyStatCard("C4", dark, "#2A0D0D","#1A0707","#991B1B","#EF4444","#991B1B","#7F1D1D","#F87171",
                                      "#FFF1F2","#FFFFFF","#FECACA","#DC2626","#EF4444","#FEE2E2","#DC2626", TxtFees, dark);

            // Quick access cards
            ApplyQACards(dark);
        }

        private void ApplyStatCard(string prefix, bool dark,
            string dG1, string dG2, string dBrd, string dLbl, string dSub, string dIcBg, string dIc,
            string lG1, string lG2, string lBrd, string lLbl, string lSub, string lIcBg, string lIc,
            TextBlock? numTb, bool isDark)
        {
            SetGradient(prefix+"G1",   isDark ? dG1   : lG1);
            SetGradient(prefix+"G2",   isDark ? dG2   : lG2);
            SetBrush(prefix+"Border",  isDark ? dBrd  : lBrd);
            SetBrush(prefix+"Label",   isDark ? dLbl  : lLbl);
            SetBrush(prefix+"Sub",     isDark ? dSub  : lSub);
            SetBrush(prefix+"Sub2",    isDark ? dSub  : lSub);
            SetBrush(prefix+"IconBg",  isDark ? dIcBg : lIcBg);
            SetBrush(prefix+"Icon",    isDark ? dIc   : lIc);
            if (numTb != null)
                numTb.Foreground = new SolidColorBrush(isDark ? Colors.White : Color.FromRgb(0x0F,0x17,0x2A));
        }

        private void ApplyQACards(bool dark)
        {
            if (QAGrid == null) return;

            // (bg1dark, bg2dark, borddark, icBgDark,  bg1light, bg2light, bordlight, icBgLight)
            var cards = new (string dBg1,string dBg2,string dBrd,string dIcBg,
                             string lBg1,string lBg2,string lBrd,string lIcBg)[]
            {
                ("#1A1A3A","#0D1128","#2E2E5A","#1A2A5A",  "#EFF6FF","#FFFFFF","#BFDBFE","#DBEAFE"),
                ("#0D2A1A","#071A10","#1A4A2A","#0D3A1A",  "#F0FDF4","#FFFFFF","#86EFAC","#DCFCE7"),
                ("#2A1E08","#1A1205","#4A3A0A","#3A2A08",  "#FFFBEB","#FFFFFF","#FDE68A","#FEF3C7"),
                ("#2A0D0D","#1A0707","#4A1A1A","#3A0D0D",  "#FFF1F2","#FFFFFF","#FECACA","#FEE2E2"),
                ("#0D1E2A","#07121A","#1A3A4A","#0D2A3A",  "#F0F9FF","#FFFFFF","#BAE6FD","#E0F2FE"),
                ("#1A0D2A","#10071A","#3A1A4A","#2A0D3A",  "#FAF5FF","#FFFFFF","#DDD6FE","#EDE9FE"),
            };

            var titleFg = dark ? Colors.White : Color.FromRgb(0x0F,0x17,0x2A);
            var subFg   = dark ? Color.FromRgb(0x4A,0x6A,0x9A) : Color.FromRgb(0x64,0x74,0x8B);
            var arrowFg = dark ? Color.FromRgb(0x2A,0x4A,0x7A) : Color.FromRgb(0x94,0xA3,0xB8);

            int idx = 0;
            foreach (UIElement child in QAGrid.Children)
            {
                if (child is not Button btn || idx >= cards.Length) { if (child is Button) idx++; continue; }
                var (dBg1,dBg2,dBrd,dIcBg,lBg1,lBg2,lBrd,lIcBg) = cards[idx];

                btn.Background = new LinearGradientBrush(
                    (Color)ColorConverter.ConvertFromString(dark ? dBg1 : lBg1),
                    (Color)ColorConverter.ConvertFromString(dark ? dBg2 : lBg2), 45);
                btn.BorderBrush = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(dark ? dBrd : lBrd));

                // Update inner grid children
                if (btn.Content is Grid g)
                    foreach (UIElement el in g.Children)
                    {
                        if (el is Border ib && ib.Child is TextBlock)
                            ib.Background = new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString(dark ? dIcBg : lIcBg));
                        else if (el is StackPanel sp)
                            foreach (UIElement spEl in sp.Children)
                                if (spEl is TextBlock tb2)
                                    tb2.Foreground = new SolidColorBrush(
                                        tb2.FontSize >= 12 ? titleFg : subFg);
                        else if (el is TextBlock atb)
                            atb.Foreground = new SolidColorBrush(arrowFg);
                    }
                idx++;
            }
        }

        private void SetBrush(string name, string hex)
        {
            if (FindName(name) is SolidColorBrush b)
                b.Color = (Color)ColorConverter.ConvertFromString(hex);
        }

        private void SetGradient(string name, string hex)
        {
            if (FindName(name) is GradientStop g)
                g.Color = (Color)ColorConverter.ConvertFromString(hex);
        }

        // ── Load stats ─────────────────────────────────────────────────────────
        private async Task LoadStatsAsync()
        {
            try
            {
                var (students, courses, instructors, fees) = await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    conn.Open();
                    int s = Count(conn, "SELECT COUNT(DISTINCT student_id) FROM ecc_dof_wukrostmarycollege.student_profile");
                    int c = Count(conn, "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.courses");
                    int i = Count(conn, "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.employees");
                    int f = Count(conn, "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_fee");
                    conn.Close();
                    return (s, c, i, f);
                });
                TxtStudents.Text    = students.ToString("N0");
                TxtCourses.Text     = courses.ToString("N0");
                TxtInstructors.Text = instructors.ToString("N0");
                TxtFees.Text        = fees.ToString("N0");
            }
            catch
            {
                TxtStudents.Text = TxtCourses.Text = TxtInstructors.Text = TxtFees.Text = "—";
            }
        }

        private static int Count(MySqlConnection conn, string sql)
        {
            using var cmd = new MySqlCommand(sql, conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ── Quick Access navigation ─────────────────────────────────────────────
        private void QA_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            string tag = btn.Tag?.ToString() ?? "";
            var home = Window.GetWindow(this) as HomePage;
            if (home == null) return;
            Page page = tag switch
            {
                "StudentRegistration" => new StudentRegistrationPage(),
                "StudentMarks"        => new StudentMarksPage(),
                "StudentFees"         => new StudentFeesPage(),
                "AttendanceSheet"     => new PlaceholderPage("Attendance Sheet"),
                "Courses"             => new PlaceholderPage("Courses"),
                "COCRecord"           => new PlaceholderPage("COC Record"),
                _                     => new PlaceholderPage(tag)
            };
            home.NavigateTo(tag, page);
        }
    }
}
