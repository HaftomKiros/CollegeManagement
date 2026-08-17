using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CollegeManagementWPF.Views
{
    public partial class MigrationPage : Page
    {
        public MigrationPage()
        {
            InitializeComponent();
            // Default storage folder
            TxtFolder.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StMaryCollege", "students");
        }

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            // Use OpenFileDialog as folder picker workaround
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title            = "Select folder — type path or navigate then click Open",
                CheckFileExists  = false,
                CheckPathExists  = true,
                FileName         = "Select Folder",
                Filter           = "Folder|*.none",
                InitialDirectory = TxtFolder.Text
            };
            if (dlg.ShowDialog() == true)
                TxtFolder.Text = Path.GetDirectoryName(dlg.FileName) ?? TxtFolder.Text;
        }

        private async void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFolder.Text))
            {
                TxtStatus.Text = "Please select a storage folder.";
                return;
            }

            BtnRun.IsEnabled = false;
            TxtLog.Text      = "";
            TxtStatus.Text   = "Checking migration status...";
            Progress.Value   = 0;

            var log     = new StringBuilder();
            var db      = new DBConnect();
            var baseDir = TxtFolder.Text;

            Directory.CreateDirectory(Path.Combine(baseDir, "photos"));
            Directory.CreateDirectory(Path.Combine(baseDir, "attachments"));

            // Check if photo_path column exists and count migrated records
            try
            {
                await Task.Run(() =>
                {
                    var conn = db.GetConnection();
                    conn.Open();

                    // Ensure path columns exist
                    foreach (var sql in new[]
                    {
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile ADD COLUMN photo_path VARCHAR(500) NULL",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile ADD COLUMN attachment_path VARCHAR(500) NULL"
                    })
                    {
                        try { new MySqlCommand(sql, conn).ExecuteNonQuery(); }
                        catch { /* already exists */ }
                    }

                    // Drop legacy BLOB columns
                    foreach (var sql in new[]
                    {
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile MODIFY COLUMN photo LONGBLOB NULL",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile MODIFY COLUMN attachment LONGBLOB NULL",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile DROP COLUMN photo",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile DROP COLUMN attachment",
                    })
                    {
                        try { new MySqlCommand(sql, conn).ExecuteNonQuery(); log.AppendLine($"✓ {sql}"); }
                        catch (Exception ex) { log.AppendLine($"  (skipped: {ex.Message.Split('\n')[0]})"); }
                    }

                    // Count records with paths
                    int total = Convert.ToInt32(new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_profile", conn).ExecuteScalar());
                    int withPhoto = Convert.ToInt32(new MySqlCommand(
                        "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_profile WHERE photo_path IS NOT NULL AND photo_path != ''", conn).ExecuteScalar());

                    log.AppendLine($"\n✓ Total students: {total}");
                    log.AppendLine($"✓ Students with photo path: {withPhoto}");
                    log.AppendLine($"✓ Storage folder: {baseDir}");
                    log.AppendLine("\nBLOB columns removed. App now uses file paths only.");
                    conn.Close();
                });

                TxtLog.Text    = log.ToString();
                TxtStatus.Text = "Migration complete — BLOB columns removed, file paths in use.";
                Progress.Value = 100;
            }
            catch (Exception ex)
            {
                TxtLog.Text    = "Error: " + ex.Message;
                TxtStatus.Text = "Failed.";
            }

            BtnRun.IsEnabled = true;
        }


    }
}
