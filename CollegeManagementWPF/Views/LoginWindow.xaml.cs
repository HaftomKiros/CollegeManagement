using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class LoginWindow : FluentWindow
    {
        private static readonly string HostFile = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StMaryCollege", "lasthost.txt");

        public LoginWindow()
        {
            InitializeComponent();
            // Restore last used host
            try
            {
                if (System.IO.File.Exists(HostFile))
                {
                    string saved = System.IO.File.ReadAllText(HostFile).Trim();
                    var parts = saved.Split('.');
                    if (parts.Length == 4)
                    {
                        IP_a.Text = parts[0]; IP_b.Text = parts[1];
                        IP_c.Text = parts[2]; IP_d.Text = parts[3];
                    }
                }
            }
            catch { }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) DoLogin();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            DoLogin();
        }

        private void DoLogin()
        {
            ErrorBorder.Visibility = Visibility.Collapsed;

            string username = TxtUsername.Text.Trim();
            string password = TxtPassword.Password;

            // Empty field check
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter your username and password.");
                return;
            }

            // Build host from IP fields
            string host = "127.0.0.1";
            string a = IP_a.Text.Trim(), b = IP_b.Text.Trim(),
                   c = IP_c.Text.Trim(), d = IP_d.Text.Trim();
            if (!string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b) &&
                !string.IsNullOrEmpty(c) && !string.IsNullOrEmpty(d))
            {
                host = $"{a}.{b}.{c}.{d}";
                // Persist host for next launch
                try { System.IO.File.WriteAllText(HostFile, host); } catch { }
            }

            // Try DB login — catch ALL exceptions, not just MySqlException
            try
            {
                var db   = new DBConnect(host);
                var conn = db.GetConnection();

                int adminId = -1;
                int roleId  = -1;
                conn.Open();
                using (var cmd = new MySqlCommand(
                    "SELECT admin_id, COALESCE(priority,0) FROM ecc_dof_wukrostmarycollege.admins " +
                    "WHERE user_name=@user AND password=@pass LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    using var r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        adminId = Convert.ToInt32(r[0]);
                        int.TryParse(r[1]?.ToString(), out roleId);
                    }
                }
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();

                if (adminId >= 0)
                {
                    SessionUser.Load(username, adminId, roleId, db);
                    OpenHomePage();
                }
                else
                    ShowError("Invalid username or password.");
            }
            catch (Exception ex)
            {
                // DB is offline or unreachable — show error, do NOT allow login
                string msg = ex.Message.Contains("Unable to connect") || ex.Message.Contains("10061") || ex.Message.Contains("refused")
                    ? $"Cannot connect to database server at {host}.\nMake sure MySQL/XAMPP is running and the host is correct."
                    : $"Database error: {ex.Message.Split('\n')[0]}";
                ShowError(msg);
            }
        }

        private void OpenHomePage()
        {
            new HomePage().Show();
            this.Close();
        }

        private void ShowError(string msg)
        {
            TxtError.Text          = msg;
            ErrorBorder.Visibility = Visibility.Visible;
        }
    }
}
