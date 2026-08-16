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
            {"BtnAdmins",   "PanAdmins"},
        };

        private readonly System.Collections.Generic.Dictionary<string, string> _btnToChev
            = new System.Collections.Generic.Dictionary<string, string>
        {
            {"BtnStudents","ChevStudents"},{"BtnDepts","ChevDepts"},
            {"BtnEmp","ChevEmp"},          {"BtnAlumni","ChevAlumni"},
            {"BtnLib","ChevLib"},          {"BtnReports","ChevReports"},
            {"BtnAdmins","ChevAdmins"},
        };

        public HomePage()
        {
            InitializeComponent();
            ContentFrame.Navigate(new DashboardPage());
            TxtPageSubTitle.Visibility = Visibility.Collapsed;
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
                "ManageAdmins"        => "Add, Update, Delete Admin Accounts",
                "Dashboard"           => "",
                _                     => ""
            };
            TxtPageSubTitle.Text       = sub;
            TxtPageSubTitle.Visibility = string.IsNullOrEmpty(sub)
                ? Visibility.Collapsed : Visibility.Visible;

            // Show search bar only for list pages
            bool hasSearch = tag is "StudentRegistration" or "StudentMarks" or "StudentFees"
                or "DropoutStudents" or "COCRecord" or "Departments" or "Streams"
                or "Levels" or "Courses" or "RegisterEmployee" or "RegisterAlumni"
                or "Library" or "ManageAdmins";
            SearchPanel.Visibility = hasSearch ? Visibility.Visible : Visibility.Collapsed;
            if (TopSearchBox != null) TopSearchBox.Text = "";

            ContentFrame.Navigate(tag switch
            {
                "Dashboard"           => (object)new DashboardPage(),
                "StudentRegistration" => new StudentRegistrationPage(),
                "StudentMarks"        => new StudentMarksPage(),
                "StudentFees"         => new StudentFeesPage(),
                "DropoutStudents"     => new DropoutPage(),
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
    }
}
