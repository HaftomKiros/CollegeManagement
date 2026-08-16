using MySql.Data.MySqlClient;
using System.Windows;

namespace CollegeManagementWPF.Data
{
    public class DBConnect
    {
        private readonly string _connString;

        public DBConnect(string host = "127.0.0.1")
        {
            _connString = $"Server={host};Database=ecc_dof_wukrostmarycollege;UserID=root;Password=;";
        }

        public MySqlConnection GetConnection()
        {
            try
            {
                return new MySqlConnection(_connString);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "DB Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null!;
            }
        }
    }
}
