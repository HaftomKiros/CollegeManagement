using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows;

namespace CollegeManagementWPF.Data
{
    public class DBConnect
    {
        private readonly string _connString;

        // Path where the last-used host is persisted (written at login)
        private static readonly string HostFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StMaryCollege", "lasthost.txt");
        /// <summary>
        /// Returns the saved host, falling back to 127.0.0.1 if none saved.
        /// </summary>
        public static string SavedHost
        {
            get
            {
                try
                {
                    if (File.Exists(HostFile))
                    {
                        string h = File.ReadAllText(HostFile).Trim();
                        if (!string.IsNullOrEmpty(h)) return h;
                    }
                }
                catch { }
                return "127.0.0.1";
            }
        }

        /// <summary>
        /// Creates a connection using the saved host (or localhost if none saved).
        /// All pages use new DBConnect() which automatically picks up the remote host.
        /// </summary>
        public DBConnect() : this(SavedHost) { }

        public DBConnect(string host)
        {
            _connString = $"Server={host};Database=ecc_dof_wukrostmarycollege;UserID=root;Password=;Connect Timeout=10;";
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
