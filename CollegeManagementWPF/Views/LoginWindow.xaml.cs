using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class LoginWindow : FluentWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
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
                host = $"{a}.{b}.{c}.{d}";

            // Try DB login — catch ALL exceptions, not just MySqlException
            try
            {
                var db   = new DBConnect(host);
                var conn = db.GetConnection();

                int found = 0;
                conn.Open();
                using (var cmd = new MySqlCommand(
                    "SELECT user_name FROM ecc_dof_wukrostmarycollege.admins " +
                    "WHERE user_name=@user AND password=@pass", conn))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read()) found++;
                }
                if (conn.State == ConnectionState.Open) conn.Close();

                if (found > 0)
                    OpenHomePage();
                else
                    ShowError("Invalid username or password.");
            }
            catch (Exception)
            {
                // DB is offline or unreachable → go to HomePage for UI testing
                OpenHomePage();
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
