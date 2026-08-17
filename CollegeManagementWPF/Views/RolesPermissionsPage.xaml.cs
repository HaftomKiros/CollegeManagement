using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CollegeManagementWPF.Views
{
    public partial class RolesPermissionsPage : Page
    {
        private DBConnect _db = new DBConnect();
        private int _selectedRoleId = -1;

        // All permissions grouped by module
        private static readonly (string Group, string Icon, string Fg, List<(string Key, string Label)> Perms)[] AllPerms =
        {
            ("Students",    "&#xE716;", "#60A5FA", new List<(string,string)>{
                ("student_register","Register Student"),("student_update","Update Student"),
                ("student_delete","Delete Student"),("student_enroll","Enroll Student"),
                ("student_view","View Students")}),
            ("Student Marks","&#xE70A;", "#4ADE80", new List<(string,string)>{
                ("marks_view","View Marks"),("marks_add","Add Marks"),
                ("marks_update","Update Marks"),("marks_delete","Delete Marks")}),
            ("Student Fees", "&#xE7C4;", "#F87171", new List<(string,string)>{
                ("fees_view","View Fees"),("fees_add","Add Fees"),
                ("fees_update","Update Fees"),("fees_delete","Delete Fees")}),
            ("Dropout",      "&#xE7BA;", "#FB923C", new List<(string,string)>{
                ("dropout_view","View Dropout"),("dropout_add","Add Dropout"),
                ("dropout_update","Update Dropout"),("dropout_delete","Delete Dropout")}),
            ("COC Record",   "&#xEA18;", "#C084FC", new List<(string,string)>{
                ("coc_view","View COC"),("coc_add","Add COC"),
                ("coc_update","Update COC"),("coc_delete","Delete COC")}),
            ("Departments",  "&#xE731;", "#67E8F9", new List<(string,string)>{
                ("dept_view","View"),("dept_add","Add"),("dept_update","Update"),("dept_delete","Delete")}),
            ("Employees",    "&#xE7EF;", "#818CF8", new List<(string,string)>{
                ("emp_view","View"),("emp_add","Add"),("emp_update","Update"),("emp_delete","Delete")}),
            ("Alumni",       "&#xE7BB;", "#38BDF8", new List<(string,string)>{
                ("alumni_view","View"),("alumni_add","Add"),("alumni_update","Update"),("alumni_delete","Delete")}),
            ("Library",      "&#xE736;", "#A3E635", new List<(string,string)>{
                ("lib_view","View"),("lib_add","Add"),("lib_update","Update"),("lib_delete","Delete")}),
            ("Reports",      "&#xEA4B;", "#FCD34D", new List<(string,string)>{
                ("report_view","View Reports"),("report_generate","Generate Reports")}),
            ("Accounts",     "&#xE728;", "#E879F9", new List<(string,string)>{
                ("account_view","View Accounts"),("account_add","Add Account"),
                ("account_update","Update Account"),("account_delete","Delete Account")}),
        };

        public RolesPermissionsPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            _ = EnsureTablesAndLoadRoles();
        }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? Color.FromRgb(0x0D,0x1B,0x3E) : Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? Color.FromRgb(0x07,0x10,0x1E) : Color.FromRgb(0xE2,0xE8,0xF0);

            // Rebuild permission cards with correct theme colors if a role is selected
            if (_selectedRoleId >= 0)
                _ = ReloadCurrentRolePermissions();
        }

        private async Task ReloadCurrentRolePermissions()
        {
            var allowed = new HashSet<string>();
            var dt = new DataTable();
            try
            {
                var cmd = new MySqlCommand(
                    $"SELECT permission_key FROM ecc_dof_wukrostmarycollege.role_permissions WHERE role_id={_selectedRoleId} AND is_allowed=1",
                    _db.GetConnection());
                await Task.Run(() => new MySqlDataAdapter(cmd).Fill(dt));
                foreach (DataRow row in dt.Rows) allowed.Add(row["permission_key"]?.ToString() ?? "");
            }
            catch { /* DB offline */ }
            BuildPermissionsUI(allowed);
        }

        // Ensure roles and role_permissions tables exist, then load roles
        private async Task EnsureTablesAndLoadRoles()
        {
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    // Create roles table
                    new MySqlCommand(
                        "CREATE TABLE IF NOT EXISTS ecc_dof_wukrostmarycollege.roles " +
                        "(role_id INT AUTO_INCREMENT PRIMARY KEY, role_name VARCHAR(100) NOT NULL UNIQUE)", conn)
                        .ExecuteNonQuery();
                    // Create role_permissions table
                    new MySqlCommand(
                        "CREATE TABLE IF NOT EXISTS ecc_dof_wukrostmarycollege.role_permissions " +
                        "(id INT AUTO_INCREMENT PRIMARY KEY, role_id INT NOT NULL, " +
                        "permission_key VARCHAR(100) NOT NULL, is_allowed TINYINT(1) DEFAULT 1, " +
                        "UNIQUE KEY uq_role_perm (role_id, permission_key))", conn)
                        .ExecuteNonQuery();
                    conn.Close();
                });
                await LoadRoles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB setup error: " + ex.Message);
            }
        }

        private async Task LoadRoles()
        {
            var dt = new DataTable();
            var cmd = new MySqlCommand("SELECT role_id, role_name FROM ecc_dof_wukrostmarycollege.roles ORDER BY role_name", _db.GetConnection());
            await Task.Run(() => new MySqlDataAdapter(cmd).Fill(dt));
            LstRoles.ItemsSource = dt.DefaultView;
        }

        private async void BtnAddRole_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtRoleName.Text.Trim();
            if (string.IsNullOrEmpty(name)) { MessageBox.Show("Enter a role name."); return; }
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand(
                        "INSERT IGNORE INTO ecc_dof_wukrostmarycollege.roles (role_name) VALUES(@n)", conn);
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                TxtRoleName.Text = "";
                await LoadRoles();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private async void BtnDeleteRole_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRoleId < 0) { MessageBox.Show("Select a role first."); return; }
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    new MySqlCommand($"DELETE FROM ecc_dof_wukrostmarycollege.role_permissions WHERE role_id={_selectedRoleId}", conn).ExecuteNonQuery();
                    new MySqlCommand($"DELETE FROM ecc_dof_wukrostmarycollege.roles WHERE role_id={_selectedRoleId}", conn).ExecuteNonQuery();
                    conn.Close();
                });
                _selectedRoleId = -1;
                TxtSelectedRole.Text = "Select a role to edit permissions";
                BuildPermissionsUI(new HashSet<string>());
                await LoadRoles();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private async void LstRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstRoles.SelectedItem is not DataRowView r) return;
            _selectedRoleId = Convert.ToInt32(r["role_id"]);
            string roleName = r["role_name"]?.ToString() ?? "";
            TxtSelectedRole.Text = $"Permissions for: {roleName}";

            // Load existing permissions for this role
            var allowed = new HashSet<string>();
            var dt = new DataTable();
            var cmd = new MySqlCommand(
                $"SELECT permission_key FROM ecc_dof_wukrostmarycollege.role_permissions WHERE role_id={_selectedRoleId} AND is_allowed=1",
                _db.GetConnection());
            await Task.Run(() => new MySqlDataAdapter(cmd).Fill(dt));
            foreach (DataRow row in dt.Rows) allowed.Add(row["permission_key"]?.ToString() ?? "");

            BuildPermissionsUI(allowed);
            if (TxtPermHint != null) TxtPermHint.Visibility = Visibility.Collapsed;
        }

        private void BuildPermissionsUI(HashSet<string> allowed)
        {
            PermissionsPanel.Children.Clear();

            var columns = new StackPanel[2];
            columns[0] = new StackPanel { Margin = new Thickness(0,0,10,0) };
            columns[1] = new StackPanel();
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Grid.SetColumn(columns[0], 0);
            Grid.SetColumn(columns[1], 1);
            grid.Children.Add(columns[0]);
            grid.Children.Add(columns[1]);

            bool dark = ThemeManager.IsDark;

            // Light/dark color sets
            var cardBgs = dark
                ? new[] { "#0D1F3C", "#0A1628" }
                : new[] { "#FFFFFF", "#F8FAFF" };
            var borderCol = dark ? "#1E3A6A" : "#CBD5E1";
            var dividerCol = dark ? "#1E3A6A" : "#E2E8F0";
            var groupTxtCol = dark ? "#FFFFFF" : "#0F172A";
            var cbFgCol = dark ? Color.FromRgb(0xCC,0xDD,0xEE) : Color.FromRgb(0x33,0x41,0x55);

            for (int i = 0; i < AllPerms.Length; i++)
            {
                var (group, icon, fg, perms) = AllPerms[i];
                var target = columns[i % 2];

                var card = new Border
                {
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(14,12,14,12),
                    Margin = new Thickness(0,0,0,10),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(borderCol)),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(cardBgs[i % 2]))
                };

                var sp = new StackPanel();

                // Group header
                var header = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0,0,0,10) };
                header.Children.Add(new TextBlock
                {
                    Text = System.Net.WebUtility.HtmlDecode(icon),
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    FontSize = 15,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fg)),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0,0,8,0)
                });
                header.Children.Add(new TextBlock
                {
                    Text = group,
                    FontSize = 13, FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(groupTxtCol)),
                    FontFamily = new FontFamily("Segoe UI"),
                    VerticalAlignment = VerticalAlignment.Center
                });
                sp.Children.Add(header);
                sp.Children.Add(new Border { Height=1, Background=new SolidColorBrush((Color)ColorConverter.ConvertFromString(dividerCol)), Margin=new Thickness(0,0,0,8) });

                // Checkboxes
                var permGrid = new UniformGrid { Rows = (int)Math.Ceiling(perms.Count / 2.0), Columns = 2 };
                foreach (var (key, label) in perms)
                {
                    permGrid.Children.Add(new CheckBox
                    {
                        Content = label,
                        Tag = key,
                        IsChecked = allowed.Contains(key),
                        Foreground = new SolidColorBrush(cbFgCol),
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 11.5,
                        Margin = new Thickness(0,3,0,3),
                        Cursor = System.Windows.Input.Cursors.Hand
                    });
                }
                sp.Children.Add(permGrid);
                card.Child = sp;
                target.Children.Add(card);
            }

            PermissionsPanel.Children.Add(grid);
        }

        private async void BtnSavePermissions_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRoleId < 0) { MessageBox.Show("Select a role first."); return; }

            // Collect all checkbox states
            var permsToSave = new List<(string key, bool allowed)>();
            CollectCheckboxes(PermissionsPanel, permsToSave);

            try
            {
                int rid = _selectedRoleId;
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    foreach (var (key, isAllowed) in permsToSave)
                    {
                        var cmd = new MySqlCommand(
                            "INSERT INTO ecc_dof_wukrostmarycollege.role_permissions (role_id,permission_key,is_allowed) " +
                            "VALUES(@r,@k,@a) ON DUPLICATE KEY UPDATE is_allowed=@a", conn);
                        cmd.Parameters.AddWithValue("@r", rid);
                        cmd.Parameters.AddWithValue("@k", key);
                        cmd.Parameters.AddWithValue("@a", isAllowed ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                });
                MessageBox.Show("Permissions saved successfully!");
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void CollectCheckboxes(DependencyObject parent, List<(string,bool)> result)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is CheckBox cb && cb.Tag is string key)
                    result.Add((key, cb.IsChecked == true));
                else
                    CollectCheckboxes(child, result);
            }
        }
    }
}
