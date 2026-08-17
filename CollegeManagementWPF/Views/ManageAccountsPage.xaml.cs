using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class ManageAccountsPage : Page
    {
        private string _selId = "";
        private DBConnect _db = new DBConnect();
        private const string Q =
            "SELECT a.admin_id, a.user_name, a.password, a.priority, " +
            "COALESCE(r.role_name, CASE a.priority WHEN '1' THEN 'Super Admin' WHEN '2' THEN 'Admin' ELSE 'Viewer' END) AS role_name " +
            "FROM ecc_dof_wukrostmarycollege.admins a " +
            "LEFT JOIN ecc_dof_wukrostmarycollege.roles r ON r.role_id = a.priority";

        public ManageAccountsPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            Loaded += async (s, e) => {
                await LoadRolesAsync();
                await Load(Q);
            };
        }

        private async Task LoadRolesAsync()
        {
            try
            {
                var roles = await Task.Run(() =>
                {
                    var list = new System.Collections.Generic.List<(int id, string name)>();
                    var conn = _db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT role_id, role_name FROM ecc_dof_wukrostmarycollege.roles ORDER BY role_id", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) list.Add((Convert.ToInt32(r["role_id"]), r["role_name"]?.ToString() ?? ""));
                    conn.Close();
                    return list;
                });

                CmbRole.Items.Clear();
                foreach (var (id, name) in roles)
                    CmbRole.Items.Add(new ComboBoxItem { Content = name, Tag = id.ToString() });

                // If no roles in DB yet, add defaults
                if (CmbRole.Items.Count == 0)
                {
                    CmbRole.Items.Add(new ComboBoxItem { Content = "Super Admin", Tag = "1" });
                    CmbRole.Items.Add(new ComboBoxItem { Content = "Admin",       Tag = "2" });
                    CmbRole.Items.Add(new ComboBoxItem { Content = "Viewer",      Tag = "3" });
                }
                if (CmbRole.Items.Count > 0) CmbRole.SelectedIndex = 0;
            }
            catch { /* DB offline — keep empty */ }
        }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);
        }

        private async Task Load(string q) {
            try {
                if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Visible;
                var t = await Task.Run(() => {
                    var dt = new DataTable();
                    new MySqlDataAdapter(q, _db.GetConnection()).Fill(dt);
                    dt.Columns.Add("_RowNo", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++) dt.Rows[i]["_RowNo"] = i + 1;
                    return dt;
                });
                Grid1.ItemsSource = t.DefaultView;
            } catch (Exception ex) { Msg("DB Error: " + ex.Message, false); }
            finally { if (LoadingOverlay != null) LoadingOverlay.Visibility = Visibility.Collapsed; }
        }

        private string GetPriority() {
            // Use the selected index + 1 as priority, or tag if available
            var item = CmbRole.SelectedItem as ComboBoxItem;
            if (item?.Tag is string tag && !string.IsNullOrEmpty(tag)) return tag;
            return (CmbRole.SelectedIndex + 1).ToString();
        }

        private void SetRole(string priority) {
            // Match by tag first, then by index
            foreach (ComboBoxItem item in CmbRole.Items)
                if (item.Tag?.ToString() == priority) { CmbRole.SelectedItem = item; return; }
            // fallback to index
            if (int.TryParse(priority, out int p) && p > 0 && p <= CmbRole.Items.Count)
                CmbRole.SelectedIndex = p - 1;
            else if (CmbRole.Items.Count > 0)
                CmbRole.SelectedIndex = 0;
        }

        private void Grid1_SelectionChanged(object s, SelectionChangedEventArgs e) {
            if (Grid1.SelectedItem is not DataRowView r) return;
            _selId = r["admin_id"]?.ToString() ?? "";
            TxtUserName.Text = r["user_name"]?.ToString() ?? "";
            SetRole(r["priority"]?.ToString() ?? "2");
            PwdPassword.Password   = r["password"]?.ToString() ?? "";
            PwdRePassword.Password = r["password"]?.ToString() ?? "";
        }

        // ── SAVE (original: validate all fields + password match, no duplicate check by username) ─
        private async void BtnSave_Click(object s, RoutedEventArgs e) {
            string un  = TxtUserName.Text.Trim();
            string pr = GetPriority();
            string pw  = PwdPassword.Password;
            string rpw = PwdRePassword.Password;

            if (string.IsNullOrWhiteSpace(un) || 
                string.IsNullOrWhiteSpace(pw) || string.IsNullOrWhiteSpace(rpw))
            { Msg("There is empty field(s). Please fill all fields!", false); return; }

            if (pw != rpw)
            { Msg("Please re-enter the same password!", false); return; }

            try {
                await Task.Run(() => {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO ecc_dof_wukrostmarycollege.admins (user_name,password,priority) VALUES(@u,@p,@pr)", c);
                    cmd.Parameters.AddWithValue("@u", un);
                    cmd.Parameters.AddWithValue("@p", pw);
                    cmd.Parameters.AddWithValue("@pr", pr);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Saved successfully!", true); await Load(Q); Clear();
            } catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── UPDATE (original: validates all fields + password match, updates by admin_id) ─
        private async void BtnUpdate_Click(object s, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(_selId)) { Msg("Select a record first.", false); return; }
            string un  = TxtUserName.Text.Trim();
            string pr = GetPriority();
            string pw  = PwdPassword.Password;
            string rpw = PwdRePassword.Password;

            if (string.IsNullOrWhiteSpace(un) || string.IsNullOrWhiteSpace(pw) ||
                string.IsNullOrWhiteSpace(rpw) )
            { Msg("There is empty field(s). Please fill all fields!", false); return; }

            if (pw != rpw)
            { Msg("Please re-enter the same password!", false); return; }

            try {
                string id = _selId;
                await Task.Run(() => {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE ecc_dof_wukrostmarycollege.admins SET user_name=@u,password=@p,priority=@pr WHERE admin_id=@id", c);
                    cmd.Parameters.AddWithValue("@u", un);
                    cmd.Parameters.AddWithValue("@p", pw);
                    cmd.Parameters.AddWithValue("@pr", pr);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Update successful!", true); await Load(Q);
            } catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        // ── DELETE ──────────────────────────────────────────────────────────
        private async void BtnDelete_Click(object s, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(_selId)) { Msg("Select a record first.", false); return; }
            var dlg = new ModernDialog($"Delete admin '{TxtUserName.Text}'?", "Confirm",
                ModernDialog.DialogType.Warning) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;
            string id = _selId;
            try {
                await Task.Run(() => {
                    var c = _db.GetConnection(); c.Open();
                    var cmd = new MySqlCommand(
                        "DELETE FROM ecc_dof_wukrostmarycollege.admins WHERE admin_id=@id", c);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery(); c.Close();
                });
                Msg("Delete successful!", true); await Load(Q); Clear();
            } catch (Exception ex) { Msg("Connection failed! " + ex.Message, false); }
        }

        private void BtnClear_Click(object s, RoutedEventArgs e) => Clear();

        private void Clear() {
            TxtUserName.Text = "";
            PwdPassword.Password = PwdRePassword.Password = "";
            CmbRole.SelectedIndex = 1; // default Admin
            _selId = "";
        }

        private void Msg(string m, bool ok) {
            var o = Window.GetWindow(this);
            if (ok) ModernDialog.Show(o, m, "Success", ModernDialog.DialogType.Success);
            else    ModernDialog.Show(o, m, "Error",   ModernDialog.DialogType.Error);
        }
    }
}
