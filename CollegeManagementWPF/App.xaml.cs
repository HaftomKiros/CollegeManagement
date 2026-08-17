using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace CollegeManagementWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                File.WriteAllText("crash.log", ex.ExceptionObject.ToString());
                MessageBox.Show(ex.ExceptionObject.ToString(), "Crash", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                File.WriteAllText("crash.log", ex.Exception.ToString());
                MessageBox.Show(ex.Exception.ToString(), "Crash", MessageBoxButton.OK, MessageBoxImage.Error);
                ex.Handled = true;
            };

            base.OnStartup(e);
            ThemeManager.Apply();

            // One-time: drop legacy BLOB columns if they still exist
            Task.Run(() =>
            {
                try
                {
                    var db = new CollegeManagementWPF.Data.DBConnect();
                    using var conn = db.GetConnection();
                    conn.Open();
                    // Make photo/attachment nullable first (in case drop fails), then drop
                    foreach (var sql in new[]
                    {
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile MODIFY COLUMN photo LONGBLOB NULL",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile MODIFY COLUMN attachment LONGBLOB NULL",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile DROP COLUMN photo",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile DROP COLUMN attachment",
                    })
                    {
                        try { new MySql.Data.MySqlClient.MySqlCommand(sql, conn).ExecuteNonQuery(); }
                        catch { /* column may already be gone */ }
                    }
                    // Trim leading/trailing spaces from existing path values
                    foreach (var col in new[] { "photo_path", "attachment_path" })
                    {
                        try
                        {
                            new MySql.Data.MySqlClient.MySqlCommand(
                                $"UPDATE ecc_dof_wukrostmarycollege.student_profile SET {col} = TRIM({col}) WHERE {col} IS NOT NULL",
                                conn).ExecuteNonQuery();
                        }
                        catch { }
                    }
                    conn.Close();
                }
                catch { /* DB not available, skip */ }
            });
        }
    }
}
