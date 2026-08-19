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

        // ── Permission groups — data permissions only, no nav_ keys ──────────
        // Each group maps directly to a section in the sidebar.
        // If ANY permission in a group is granted, that nav section becomes visible.
        public static readonly (string Group, string Icon, string Fg, List<(string Key, string Label)> Perms)[] AllPerms =
        {
            ("Students",           "\uE716", "#60A5FA", new List<(string,string)>{
                ("student_view",     "View Students"),
                ("student_register", "Register Student"),
                ("student_update",   "Update Student"),
                ("student_delete",   "Delete Student"),
                ("student_enroll",   "Enroll Student")}),

            ("Student Marks",      "\uE70A", "#4ADE80", new List<(string,string)>{
                ("marks_view",   "View Marks"),
                ("marks_add",    "Add Marks"),
                ("marks_update", "Update Marks"),
                ("marks_delete", "Delete Marks")}),

            ("Assessment Records", "\uE8A1", "#F472B6", new List<(string,string)>{
                ("assess_view",   "View Records"),
                ("assess_add",    "Add Records"),
                ("assess_update", "Update Records"),
                ("assess_delete", "Delete Records")}),

            ("Student Fees",       "\uE7C4", "#F87171", new List<(string,string)>{
                ("fees_view",   "View Fees"),
                ("fees_add",    "Add Fees"),
                ("fees_update", "Update Fees"),
                ("fees_delete", "Delete Fees")}),

            ("Dropout Students",   "\uE7BA", "#FB923C", new List<(string,string)>{
                ("dropout_view",   "View"),
                ("dropout_add",    "Add"),
                ("dropout_update", "Update"),
                ("dropout_delete", "Delete")}),

            ("COC Record",         "\uEA18", "#C084FC", new List<(string,string)>{
                ("coc_view",   "View"),
                ("coc_add",    "Add"),
                ("coc_update", "Update"),
                ("coc_delete", "Delete")}),

            ("Departments",        "\uE731", "#67E8F9", new List<(string,string)>{
                ("dept_view",   "View"),
                ("dept_add",    "Add"),
                ("dept_update", "Update"),
                ("dept_delete", "Delete")}),

            ("Streams",            "\uE8CE", "#38BDF8", new List<(string,string)>{
                ("stream_view",   "View"),
                ("stream_add",    "Add"),
                ("stream_update", "Update"),
                ("stream_delete", "Delete")}),

            ("Levels",             "\uE9D9", "#60A5FA", new List<(string,string)>{
                ("level_view",   "View"),
                ("level_add",    "Add"),
                ("level_update", "Update"),
                ("level_delete", "Delete")}),

            ("Courses",            "\uE736", "#A78BFA", new List<(string,string)>{
                ("course_view",   "View"),
                ("course_add",    "Add"),
                ("course_update", "Update"),
                ("course_delete", "Delete")}),

            ("Employees",          "\uE7EF", "#818CF8", new List<(string,string)>{
                ("emp_view",   "View"),
                ("emp_add",    "Add"),
                ("emp_update", "Update"),
                ("emp_delete", "Delete")}),

            ("Alumni",             "\uE7BB", "#38BDF8", new List<(string,string)>{
                ("alumni_view",   "View"),
                ("alumni_add",    "Add"),
                ("alumni_update", "Update"),
                ("alumni_delete", "Delete")}),

            ("Library",            "\uE736", "#A3E635", new List<(string,string)>{
                ("lib_view",   "View"),
                ("lib_add",    "Add"),
                ("lib_update", "Update"),
                ("lib_delete", "Delete")}),

            ("Reports",            "\uEA4B", "#FCD34D", new List<(string,string)>{
                ("report_tvet_transcript",  "TVET Transcript"),
                ("report_tvet_assessment",  "TVET Assessment Transcript"),
                ("report_marklist",         "Mark List"),
                ("report_assessment_ml",    "Assessment Mark List"),
                ("report_attendance",       "Attendance Sheet"),
                ("report_coc_list",         "COC List")}),

            ("Accounts",           "\uE728", "#E879F9", new List<(string,string)>{
                ("account_manage",       "Manage Accounts"),
                ("account_roles",        "Roles & Permissions")}),
        };

        public RolesPermissionsPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            _ = EnsureTablesAndLoadRoles();
        }

        private void ApplyTheme()
        {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is GradientStop g1)
                g1.Color = dark ? Color.FromRgb(0x0D,0x1B,0x3E) : Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is GradientStop g2)
                g2.Color = dark ? Color.FromRgb(0x07,0x10,0x1E) : Color.FromRgb(0xE2,0xE8,0xF0);
            if (_selectedRoleId >= 0) _ = ReloadCurrentRolePermissions();
        }

        private async Task ReloadCurrentRolePermissions()
        {
            var allowed = await LoadAllowedAsync(_selectedRoleId);
            BuildPermissionsUI(allowed);
        }

        private async Task<HashSet<string>> LoadAllowedAsync(int roleId)
        {
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var dt = new DataTable();
                var cmd = new MySqlCommand(
                    "SELECT permission_key FROM ecc_dof_wukrostmarycollege.role_permissions " +
                    "WHERE role_id=@r AND is_allowed=1", _db.GetConnection());
                cmd.Parameters.AddWithValue("@r", roleId);
                await Task.Run(() => new MySqlDataAdapter(cmd).Fill(dt));
                foreach (DataRow row in dt.Rows)
                    allowed.Add(row["permission_key"]?.ToString() ?? "");
            }
            catch { }
            return allowed;
        }

        // ── DB setup ─────────────────────────────────────────────────────────
        private async Task EnsureTablesAndLoadRoles()
        {
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    new MySqlCommand(
                        "CREATE TABLE IF NOT EXISTS ecc_dof_wukrostmarycollege.roles " +
                        "(role_id INT AUTO_INCREMENT PRIMARY KEY, role_name VARCHAR(100) NOT NULL UNIQUE)", conn)
                        .ExecuteNonQuery();
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
            catch (Exception ex) { MessageBox.Show("DB setup error: " + ex.Message); }
        }

        private async Task LoadRoles()
        {
            var dt = new DataTable();
            var cmd = new MySqlCommand(
                "SELECT role_id, role_name FROM ecc_dof_wukrostmarycollege.roles ORDER BY role_name",
                _db.GetConnection());
            await Task.Run(() => new MySqlDataAdapter(cmd).Fill(dt));
            LstRoles.ItemsSource = dt.DefaultView;
        }

        // ── Role CRUD ────────────────────────────────────────────────────────
        private async void BtnAddRole_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtRoleName.Text.Trim();
            if (string.IsNullOrEmpty(name)) { ModernDialog.Show(Window.GetWindow(this),"Enter a role name.","Info",ModernDialog.DialogType.Info); return; }
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    var cmd = new MySqlCommand("INSERT IGNORE INTO ecc_dof_wukrostmarycollege.roles (role_name) VALUES(@n)", conn);
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.ExecuteNonQuery(); conn.Close();
                });
                TxtRoleName.Text = "";
                await LoadRoles();
            }
            catch (Exception ex) { ModernDialog.Show(Window.GetWindow(this),"Error: "+ex.Message,"Error",ModernDialog.DialogType.Error); }
        }

        private async void BtnDeleteRole_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRoleId < 0) { ModernDialog.Show(Window.GetWindow(this),"Select a role first.","Info",ModernDialog.DialogType.Info); return; }
            try
            {
                int rid = _selectedRoleId;
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection(); conn.Open();
                    new MySqlCommand($"DELETE FROM ecc_dof_wukrostmarycollege.role_permissions WHERE role_id={rid}", conn).ExecuteNonQuery();
                    new MySqlCommand($"DELETE FROM ecc_dof_wukrostmarycollege.roles WHERE role_id={rid}", conn).ExecuteNonQuery();
                    conn.Close();
                });
                _selectedRoleId = -1;
                TxtSelectedRole.Text = "Select a role to edit permissions";
                PermissionsPanel.Children.Clear();
                await LoadRoles();
            }
            catch (Exception ex) { ModernDialog.Show(Window.GetWindow(this),"Error: "+ex.Message,"Error",ModernDialog.DialogType.Error); }
        }

        private async void LstRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstRoles.SelectedItem is not DataRowView r) return;
            _selectedRoleId = Convert.ToInt32(r["role_id"]);
            TxtSelectedRole.Text = $"Permissions for: {r["role_name"]}";
            var allowed = await LoadAllowedAsync(_selectedRoleId);
            BuildPermissionsUI(allowed);
            if (TxtPermHint != null) TxtPermHint.Visibility = Visibility.Collapsed;
        }

        // ── Permissions UI ───────────────────────────────────────────────────
        private void BuildPermissionsUI(HashSet<string> allowed)
        {
            PermissionsPanel.Children.Clear();

            bool dark = ThemeManager.IsDark;
            var cardBgs   = dark ? new[]{"#0D1F3C","#0A1628"} : new[]{"#FFFFFF","#F8FAFF"};
            var borderCol = dark ? "#1E3A6A" : "#CBD5E1";
            var divCol    = dark ? "#1E3A6A" : "#E2E8F0";
            var groupFg   = dark ? "#FFFFFF"  : "#0F172A";
            var cbFg      = dark ? Color.FromRgb(0xCC,0xDD,0xEE) : Color.FromRgb(0x33,0x41,0x55);

            var cols = new StackPanel[2];
            cols[0] = new StackPanel { Margin = new Thickness(0,0,6,0) };
            cols[1] = new StackPanel { Margin = new Thickness(6,0,0,0) };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) });
            Grid.SetColumn(cols[0],0); Grid.SetColumn(cols[1],1);
            grid.Children.Add(cols[0]); grid.Children.Add(cols[1]);

            for (int i = 0; i < AllPerms.Length; i++)
            {
                var (group, icon, fg, perms) = AllPerms[i];
                var card = new Border
                {
                    CornerRadius    = new CornerRadius(10),
                    Padding         = new Thickness(14,12,14,12),
                    Margin          = new Thickness(0,0,0,10),
                    BorderThickness = new Thickness(1),
                    BorderBrush     = new SolidColorBrush((Color)ColorConverter.ConvertFromString(borderCol)),
                    Background      = new SolidColorBrush((Color)ColorConverter.ConvertFromString(cardBgs[i%2]))
                };
                var sp = new StackPanel();

                // Group header
                var hdr = new StackPanel { Orientation=Orientation.Horizontal, Margin=new Thickness(0,0,0,8) };
                hdr.Children.Add(new TextBlock
                {
                    Text = icon, FontFamily = new FontFamily("Segoe MDL2 Assets"), FontSize = 14,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fg)),
                    VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0,0,8,0)
                });
                hdr.Children.Add(new TextBlock
                {
                    Text = group, FontSize = 12.5, FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(groupFg)),
                    FontFamily = new FontFamily("Segoe UI"), VerticalAlignment = VerticalAlignment.Center
                });
                sp.Children.Add(hdr);
                sp.Children.Add(new Border { Height=1, Margin=new Thickness(0,0,0,8),
                    Background=new SolidColorBrush((Color)ColorConverter.ConvertFromString(divCol)) });

                // Checkboxes
                var pg = new UniformGrid { Columns = 2 };
                foreach (var (key, label) in perms)
                    pg.Children.Add(new CheckBox
                    {
                        Content = label, Tag = key, IsChecked = allowed.Contains(key),
                        Foreground = new SolidColorBrush(cbFg),
                        FontFamily = new FontFamily("Segoe UI"), FontSize = 11.5,
                        Margin = new Thickness(0,3,0,3), Cursor = System.Windows.Input.Cursors.Hand
                    });
                sp.Children.Add(pg);
                card.Child = sp;
                cols[i%2].Children.Add(card);
            }
            PermissionsPanel.Children.Add(grid);
        }

        // ── Save ─────────────────────────────────────────────────────────────
        private async void BtnSavePermissions_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRoleId < 0) { ModernDialog.Show(Window.GetWindow(this),"Select a role first.","Info",ModernDialog.DialogType.Info); return; }

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
                            "INSERT INTO ecc_dof_wukrostmarycollege.role_permissions " +
                            "(role_id,permission_key,is_allowed) VALUES(@r,@k,@a) " +
                            "ON DUPLICATE KEY UPDATE is_allowed=@a", conn);
                        cmd.Parameters.AddWithValue("@r", rid);
                        cmd.Parameters.AddWithValue("@k", key);
                        cmd.Parameters.AddWithValue("@a", isAllowed ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                });
                ModernDialog.Show(Window.GetWindow(this),"Permissions saved!","Success",ModernDialog.DialogType.Success);
            }
            catch (Exception ex) { ModernDialog.Show(Window.GetWindow(this),"Error: "+ex.Message,"Error",ModernDialog.DialogType.Error); }
        }

        private void CollectCheckboxes(DependencyObject parent, List<(string,bool)> result)
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is CheckBox cb && cb.Tag is string key)
                    result.Add((key, cb.IsChecked == true));
                else
                    CollectCheckboxes(child, result);
            }
        }
    }
}
