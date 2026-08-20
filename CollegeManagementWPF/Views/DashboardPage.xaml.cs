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
            ApplyPermissions();
            Loaded += async (s, e) => await LoadStatsAsync();
        }

        /// <summary>
        /// 1. Update welcome banner with logged-in user name.
        /// 2. Hide Quick Access buttons the user has no permission for.
        /// </summary>
        private void ApplyPermissions()
        {
            // Welcome banner — show real username
            if (FindName("TxtWelcome") is System.Windows.Controls.TextBlock wb)
                wb.Text = $"Welcome back, {(string.IsNullOrEmpty(SessionUser.Username) ? "Administrator" : SessionUser.Username)}";

            if (SessionUser.IsSuperAdmin) return;

            // Quick Access: hide buttons whose tag maps to a permission the user lacks
            var permKeys = new System.Collections.Generic.Dictionary<string, string[]>
            {
                { "StudentRegistration", new[]{ "student_view","student_register","student_update","student_delete","student_enroll" } },
                { "StudentMarks",        new[]{ "marks_view","marks_add","marks_update","marks_delete","marks_attach" } },
                { "AttendanceSheet",     new[]{ "report_attendance" } },
                { "StudentFees",         new[]{ "fees_view","fees_add","fees_update","fees_delete" } },
                { "Courses",             new[]{ "course_view","course_add","course_update","course_delete" } },
                { "COCRecord",           new[]{ "coc_view","coc_add","coc_update","coc_delete" } },
            };

            if (QAGrid == null) return;
            foreach (UIElement child in QAGrid.Children)
            {
                if (child is not System.Windows.Controls.Button btn) continue;
                string tag = btn.Tag?.ToString() ?? "";
                if (permKeys.TryGetValue(tag, out string[]? keys))
                {
                    bool allowed = false;
                    foreach (var k in keys) if (SessionUser.Has(k)) { allowed = true; break; }
                    btn.Visibility = allowed ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        // ── Theme ──────────────────────────────────────────────────────────────
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
            ApplyStatCard("C1", dark,
                dG1:"#0D1F4A", dG2:"#0A1635", dBrd:"#1E3A8A", dLbl:"#6B9ADA", dSub:"#3A6AAA", dIcBg:"#1A3A6A", dIc:"#60A5FA",
                lG1:"#EFF6FF", lG2:"#FFFFFF",  lBrd:"#BFDBFE", lLbl:"#1D4ED8", lSub:"#3B82F6", lIcBg:"#DBEAFE", lIc:"#1D4ED8",
                numTb: TxtStudents);
            ApplyStatCard("C2", dark,
                dG1:"#0D2A1A", dG2:"#071A10", dBrd:"#166534", dLbl:"#22AA66", dSub:"#166534", dIcBg:"#14532D", dIc:"#4ADE80",
                lG1:"#F0FDF4", lG2:"#FFFFFF",  lBrd:"#86EFAC", lLbl:"#15803D", lSub:"#16A34A", lIcBg:"#DCFCE7", lIc:"#15803D",
                numTb: TxtDepartments);
            ApplyStatCard("C3", dark,
                dG1:"#2A1E08", dG2:"#1A1205", dBrd:"#92400E", dLbl:"#D97706", dSub:"#92400E", dIcBg:"#78350F", dIc:"#FCD34D",
                lG1:"#FFFBEB", lG2:"#FFFFFF",  lBrd:"#FDE68A", lLbl:"#B45309", lSub:"#D97706", lIcBg:"#FEF3C7", lIc:"#92400E",
                numTb: TxtStreams);
            ApplyStatCard("C4", dark,
                dG1:"#2A0D0D", dG2:"#1A0707", dBrd:"#991B1B", dLbl:"#EF4444", dSub:"#991B1B", dIcBg:"#7F1D1D", dIc:"#F87171",
                lG1:"#FFF1F2", lG2:"#FFFFFF",  lBrd:"#FECACA", lLbl:"#DC2626", lSub:"#EF4444", lIcBg:"#FEE2E2", lIc:"#DC2626",
                numTb: TxtEmployees);

            ApplyQACards(dark);
        }

        private void ApplyStatCard(string prefix, bool dark,
            string dG1, string dG2, string dBrd, string dLbl, string dSub, string dIcBg, string dIc,
            string lG1, string lG2, string lBrd, string lLbl, string lSub, string lIcBg, string lIc,
            TextBlock? numTb)
        {
            SetGradient(prefix + "G1",    dark ? dG1   : lG1);
            SetGradient(prefix + "G2",    dark ? dG2   : lG2);
            SetBrush   (prefix + "Border",dark ? dBrd  : lBrd);
            SetBrush   (prefix + "Label", dark ? dLbl  : lLbl);
            SetBrush   (prefix + "Sub",   dark ? dSub  : lSub);
            SetBrush   (prefix + "Sub2",  dark ? dSub  : lSub);
            SetBrush   (prefix + "IconBg",dark ? dIcBg : lIcBg);
            SetBrush   (prefix + "Icon",  dark ? dIc   : lIc);
            if (numTb != null)
                numTb.Foreground = new SolidColorBrush(dark ? Colors.White : Color.FromRgb(0x0F,0x17,0x2A));
        }

        private void ApplyQACards(bool dark)
        {
            if (QAGrid == null) return;

            var cards = new (string dBg1, string dBg2, string dBrd, string dIcBg,
                             string lBg1, string lBg2, string lBrd, string lIcBg)[]
            {
                ("#1A1A3A","#0D1128","#2E2E5A","#1A2A5A",  "#EFF6FF","#FFFFFF","#BFDBFE","#DBEAFE"),
                ("#0D2A1A","#071A10","#1A4A2A","#0D3A1A",  "#F0FDF4","#FFFFFF","#86EFAC","#DCFCE7"),
                ("#2A1E08","#1A1205","#4A3A0A","#3A2A08",  "#FFFBEB","#FFFFFF","#FDE68A","#FEF3C7"),
                ("#2A0D0D","#1A0707","#4A1A1A","#3A0D0D",  "#FFF1F2","#FFFFFF","#FECACA","#FEE2E2"),
                ("#0D1E2A","#07121A","#1A3A4A","#0D2A3A",  "#F0F9FF","#FFFFFF","#BAE6FD","#E0F2FE"),
                ("#1A0D2A","#10071A","#3A1A4A","#2A0D3A",  "#FAF5FF","#FFFFFF","#DDD6FE","#EDE9FE"),
            };

            var titleFg = dark ? Colors.White              : Color.FromRgb(0x0F,0x17,0x2A);
            var subFg   = dark ? Color.FromRgb(0x4A,0x6A,0x9A) : Color.FromRgb(0x64,0x74,0x8B);
            var arrowFg = dark ? Color.FromRgb(0x2A,0x4A,0x7A) : Color.FromRgb(0x94,0xA3,0xB8);

            int idx = 0;
            foreach (UIElement child in QAGrid.Children)
            {
                if (child is not Button btn) continue;
                if (idx >= cards.Length) break;

                var (dBg1,dBg2,dBrd,dIcBg,lBg1,lBg2,lBrd,lIcBg) = cards[idx++];

                btn.Background = new LinearGradientBrush(
                    (Color)ColorConverter.ConvertFromString(dark ? dBg1 : lBg1),
                    (Color)ColorConverter.ConvertFromString(dark ? dBg2 : lBg2), 45);
                btn.BorderBrush = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(dark ? dBrd : lBrd));

                if (btn.Content is Grid g)
                    foreach (UIElement el in g.Children)
                    {
                        if (el is Border ib && ib.Child is TextBlock)
                            ib.Background = new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString(dark ? dIcBg : lIcBg));
                        else if (el is StackPanel sp)
                            foreach (UIElement spEl in sp.Children)
                                if (spEl is TextBlock tb2)
                                    tb2.Foreground = new SolidColorBrush(tb2.FontSize >= 12 ? titleFg : subFg);
                        else if (el is TextBlock atb)
                            atb.Foreground = new SolidColorBrush(arrowFg);
                    }
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

        private async Task LoadStatsAsync()
        {
            // Each stat card has its own dashboard permission
            bool canSeeStudents  = SessionUser.IsSuperAdmin || SessionUser.Has("dashboard_students");
            bool canSeeDepts     = SessionUser.IsSuperAdmin || SessionUser.Has("dashboard_departments");
            bool canSeeStreams    = SessionUser.IsSuperAdmin || SessionUser.Has("dashboard_streams");
            bool canSeeEmployees = SessionUser.IsSuperAdmin || SessionUser.Has("dashboard_employees");

            Dispatcher.Invoke(() => {
                TxtStudents.Text    = canSeeStudents  ? "..." : "****";
                TxtDepartments.Text = canSeeDepts     ? "..." : "****";
                TxtStreams.Text     = canSeeStreams    ? "..." : "****";
                TxtEmployees.Text   = canSeeEmployees ? "..." : "****";
            });

            if (!canSeeStudents && !canSeeDepts && !canSeeStreams && !canSeeEmployees) return;

            try
            {
                var result = await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    if (conn == null) throw new Exception("DB connection returned null");
                    conn.Open();
                    int students    = canSeeStudents  ? Count(conn, "SELECT COUNT(DISTINCT student_id) FROM ecc_dof_wukrostmarycollege.student_profile") : -1;
                    int departments = canSeeDepts     ? Count(conn, "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.departments") : -1;
                    int streams     = canSeeStreams    ? Count(conn, "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.streams") : -1;
                    int employees   = canSeeEmployees ? Count(conn, "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.employee_profile") : -1;
                    conn.Close();
                    return (students, departments, streams, employees);
                });

                TxtStudents.Text    = result.students    >= 0 ? result.students.ToString("N0")    : "****";
                TxtDepartments.Text = result.departments >= 0 ? result.departments.ToString("N0") : "****";
                TxtStreams.Text     = result.streams      >= 0 ? result.streams.ToString("N0")     : "****";
                TxtEmployees.Text   = result.employees   >= 0 ? result.employees.ToString("N0")   : "****";
            }
            catch (Exception ex)
            {
                TxtStudents.Text = TxtDepartments.Text = TxtStreams.Text = TxtEmployees.Text = "—";
                System.IO.File.WriteAllText("dashboard_error.log", ex.ToString());
            }
        }

        private static int Count(MySqlConnection conn, string sql)
        {
            using var cmd = new MySqlCommand(sql, conn);
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        // ── Quick Access navigation ─────────────────────────────────────────────
        private void QA_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            string tag = btn.Tag?.ToString() ?? "";
            if (Window.GetWindow(this) is not HomePage home) return;
            home.NavigateTo(tag);
        }
    }
}
