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
            TxtStatus.Text   = "Starting migration...";
            Progress.Value   = 0;

            var log     = new StringBuilder();
            var db      = new DBConnect();
            var baseDir = TxtFolder.Text;

            Directory.CreateDirectory(Path.Combine(baseDir, "photos"));
            Directory.CreateDirectory(Path.Combine(baseDir, "attachments"));

            // Step 1: Ensure path columns exist
            try
            {
                await Task.Run(() =>
                {
                    var conn = db.GetConnection();
                    conn.Open();
                    foreach (var sql in new[]
                    {
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile ADD COLUMN photo_path VARCHAR(500) NULL",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile ADD COLUMN attachment_path VARCHAR(500) NULL"
                    })
                    {
                        try { new MySqlCommand(sql, conn).ExecuteNonQuery(); }
                        catch { /* already exists */ }
                    }
                    conn.Close();
                });
                log.AppendLine("✓ Path columns ready.");
            }
            catch (Exception ex)
            {
                TxtLog.Text  = $"✗ Failed to add columns: {ex.Message}";
                BtnRun.IsEnabled = true;
                return;
            }

            // Step 2: Read all students with BLOBs
            var rows = new System.Collections.Generic.List<(string id, string lvl, byte[]? photo, byte[]? attach)>();
            try
            {
                await Task.Run(() =>
                {
                    var conn = db.GetConnection();
                    conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT student_id, level, photo, attachment FROM ecc_dof_wukrostmarycollege.student_profile", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        byte[]? ph = r["photo"]      != DBNull.Value ? (byte[])r["photo"]      : null;
                        byte[]? at = r["attachment"] != DBNull.Value ? (byte[])r["attachment"] : null;
                        rows.Add((r["student_id"].ToString()!, r["level"].ToString()!, ph, at));
                    }
                    conn.Close();
                });
                log.AppendLine($"✓ Found {rows.Count} student records.");
                TxtStatus.Text = $"Migrating {rows.Count} records...";
            }
            catch (Exception ex)
            {
                TxtLog.Text = $"✗ Failed to read students: {ex.Message}";
                BtnRun.IsEnabled = true;
                return;
            }

            // Step 3: Export files and update paths
            int done = 0, errors = 0;
            foreach (var (id, lvl, photo, attach) in rows)
            {
                try
                {
                    string? pp = null, ap = null;

                    if (photo != null && photo.Length > 0)
                    {
                        // Sanitize student ID — replace path-unsafe chars with underscore
                        string safeId = id.Replace("/", "_").Replace("\\", "_")
                                          .Replace(":", "_").Replace("*", "_")
                                          .Replace("?", "_").Replace("\"", "_")
                                          .Replace("<", "_").Replace(">", "_")
                                          .Replace("|", "_");
                        pp = Path.Combine(baseDir, "photos", $"{safeId}_L{lvl}.jpg");
                        await File.WriteAllBytesAsync(pp, photo);
                    }

                    if (attach != null && attach.Length > 0)
                    {
                        string safeId = id.Replace("/", "_").Replace("\\", "_")
                                          .Replace(":", "_").Replace("*", "_")
                                          .Replace("?", "_").Replace("\"", "_")
                                          .Replace("<", "_").Replace(">", "_")
                                          .Replace("|", "_");
                        ap = Path.Combine(baseDir, "attachments", $"{safeId}_L{lvl}.pdf");
                        await File.WriteAllBytesAsync(ap, attach);
                    }

                    // Update path columns in DB (BLOBs untouched)
                    await Task.Run(() =>
                    {
                        var conn = db.GetConnection();
                        conn.Open();
                        using var upd = new MySqlCommand(
                            "UPDATE ecc_dof_wukrostmarycollege.student_profile " +
                            "SET photo_path=@pp, attachment_path=@ap " +
                            "WHERE student_id=@id AND level=@lvl", conn);
                        upd.Parameters.AddWithValue("@pp",  (object?)pp ?? DBNull.Value);
                        upd.Parameters.AddWithValue("@ap",  (object?)ap ?? DBNull.Value);
                        upd.Parameters.AddWithValue("@id",  id);
                        upd.Parameters.AddWithValue("@lvl", lvl);
                        upd.ExecuteNonQuery();
                        conn.Close();
                    });

                    done++;
                    log.AppendLine($"  ✓ {id} L{lvl}  photo:{pp ?? "none"}  attach:{ap ?? "none"}");
                }
                catch (Exception ex)
                {
                    errors++;
                    log.AppendLine($"  ✗ {id} L{lvl}: {ex.Message}");
                }

                // Update UI progress every 5 records to prevent freeze
                if ((done + errors) % 5 == 0 || (done + errors) == rows.Count)
                {
                    double pct = rows.Count > 0 ? (done + errors) * 100.0 / rows.Count : 100;
                    string logSnapshot = log.ToString();
                    Dispatcher.Invoke(() =>
                    {
                        Progress.Value = pct;
                        TxtStatus.Text = $"Migrated {done}/{rows.Count}...";
                        TxtLog.Text    = logSnapshot;
                    });
                    await Task.Delay(1); // yield to UI thread
                }
            }

            log.AppendLine($"\n✓ Done — {done} migrated, {errors} errors.");
            log.AppendLine($"Files saved to: {baseDir}");
            log.AppendLine("\nBLOB columns (photo, attachment) are UNCHANGED — no data was deleted.");

            TxtLog.Text      = log.ToString();
            TxtStatus.Text   = $"Complete — {done} migrated, {errors} errors.";
            Progress.Value   = 100;
            BtnRun.IsEnabled = true;
        }
    }
}
