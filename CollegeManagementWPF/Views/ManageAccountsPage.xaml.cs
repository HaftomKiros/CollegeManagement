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
        private const string Q = "SELECT admin_id,user_name,password,priority FROM ecc_dof_wukrostmarycollege.admins";

        public ManageAccountsPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
            Loaded += async (s, e) => await Load(Q);
        }

        private void ApplyTheme() {
            bool dark = ThemeManager.IsDark;
            if (FindName("PageBg1") is System.Windows.Media.GradientStop g1)
                g1.Color = dark ? System.Windows.Media.Color.FromRgb(0x0D,0x1B,0x3E) : System.Windows.Media.Color.FromRgb(0xF1,0xF5,0xF9);
            if (FindName("PageBg2") is System.Windows.Media.GradientStop g2)
                g2.Color = dark ? System.Windows.Media.Color.FromRgb(0x07,0x10,0x1E) : System.Windows.Media.Color.FromRgb(0xE2,0xE8,0xF0);

            // Priority card backgrounds adapt to theme
            bool isDark = dark;
            SetCardBg("PriorityCard1", isDark ? "#2A0A4A" : "#F5F0FF");
            SetCardBg("PriorityCard2", isDark ? "#0A1E3A" : "#EFF6FF");
            SetCardBg("PriorityCard3", isDark ? "#0A2A1A" : "#F0FDF4");
        }

        private void SetCardBg(string name, string hex) {
            if (FindName(name) is System.Windows.Controls.Border b)
                b.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex));
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
            int idx = CmbRole.SelectedIndex;
            return idx switch { 0 => "1", 1 => "2", _ => "3" };
        }

        private void SetRole(string priority) {
            CmbRole.SelectedIndex = priority switch { "1" => 0, "2" => 1, _ => 2 };
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
