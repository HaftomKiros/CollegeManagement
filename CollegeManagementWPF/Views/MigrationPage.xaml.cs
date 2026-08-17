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
            TxtFolder.Text   = AppSettings.Current.StorageBasePath;
            TxtMlFolder.Text = AppSettings.Current.MarkListBasePath;
        }

        // ── Tab 1: Student Profile BLOB migration ─────────────────────────────
        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select storage folder", CheckFileExists = false, CheckPathExists = true,
                FileName = "Select Folder", Filter = "Folder|*.none", InitialDirectory = TxtFolder.Text
            };
            if (dlg.ShowDialog() == true)
                TxtFolder.Text = Path.GetDirectoryName(dlg.FileName) ?? TxtFolder.Text;
        }

        private async void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFolder.Text)) { TxtStatus.Text = "Please select a storage folder."; return; }
            BtnRun.IsEnabled = false;
            TxtLog.Text = ""; TxtStatus.Text = "Checking migration status..."; Progress.Value = 0;

            var log = new StringBuilder();
            var db  = new DBConnect();
            var dir = string.IsNullOrWhiteSpace(TxtFolder.Text)
                ? AppSettings.Current.StorageBasePath
                : TxtFolder.Text;
            Directory.CreateDirectory(Path.Combine(dir, "photos"));
            Directory.CreateDirectory(Path.Combine(dir, "attachments"));

            try
            {
                await Task.Run(() =>
                {
                    var conn = db.GetConnection(); conn.Open();
                    foreach (var sql in new[]
                    {
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile ADD COLUMN photo_path VARCHAR(500) NULL",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile ADD COLUMN attachment_path VARCHAR(500) NULL"
                    })
                    { try { new MySqlCommand(sql, conn).ExecuteNonQuery(); } catch { } }

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

                    int total     = Convert.ToInt32(new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_profile", conn).ExecuteScalar());
                    int withPhoto = Convert.ToInt32(new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_profile WHERE photo_path IS NOT NULL AND photo_path != ''", conn).ExecuteScalar());
                    log.AppendLine($"\n✓ Total students: {total}");
                    log.AppendLine($"✓ With photo path: {withPhoto}");
                    log.AppendLine($"✓ Storage: {dir}");
                    log.AppendLine("\nBLOB columns removed. File paths in use.");
                    conn.Close();
                });
                TxtLog.Text = log.ToString(); TxtStatus.Text = "Complete."; Progress.Value = 100;
            }
            catch (Exception ex) { TxtLog.Text = "Error: " + ex.Message; TxtStatus.Text = "Failed."; }
            BtnRun.IsEnabled = true;
        }

        // ── Tab 2: Mark List Documents BLOB migration ─────────────────────────
        private void BrowseMlFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select storage folder for mark lists", CheckFileExists = false, CheckPathExists = true,
                FileName = "Select Folder", Filter = "Folder|*.none", InitialDirectory = TxtMlFolder.Text
            };
            if (dlg.ShowDialog() == true)
                TxtMlFolder.Text = Path.GetDirectoryName(dlg.FileName) ?? TxtMlFolder.Text;
        }

        private async void BtnMlRun_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtMlFolder.Text)) { TxtMlStatus.Text = "Please select a storage folder."; return; }
            BtnMlRun.IsEnabled = false;
            TxtMlLog.Text = ""; TxtMlStatus.Text = "Starting mark list migration..."; MlProgress.Value = 0;

            var log = new StringBuilder();
            var db  = new DBConnect();
            var dir = string.IsNullOrWhiteSpace(TxtMlFolder.Text)
                ? AppSettings.Current.MarkListsPath
                : TxtMlFolder.Text;
            Directory.CreateDirectory(dir);

            try
            {
                // Step 1: Add doc_file_path column if not exists
                await Task.Run(() =>
                {
                    var conn = db.GetConnection(); conn.Open();
                    try { new MySqlCommand("ALTER TABLE ecc_dof_wukrostmarycollege.mark_list_docs ADD COLUMN doc_file_path VARCHAR(500) NULL", conn).ExecuteNonQuery(); log.AppendLine("✓ doc_file_path column added."); }
                    catch { log.AppendLine("  (doc_file_path already exists — skipped)"); }
                    conn.Close();
                });

                // Step 2: Read all BLOB records
                var rows = new System.Collections.Generic.List<(string dept, string stream, string level, string mod, string year, string adm, byte[]? file)>();
                await Task.Run(() =>
                {
                    var conn = db.GetConnection(); conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT doc_dept_id,doc_stream_id,doc_level_id,doc_module_code,doc_academic_year,doc_admission_type,doc_file " +
                        "FROM ecc_dof_wukrostmarycollege.mark_list_docs", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        byte[]? blob = r["doc_file"] != DBNull.Value ? (byte[])r["doc_file"] : null;
                        rows.Add((
                            r["doc_dept_id"]?.ToString()!,
                            r["doc_stream_id"]?.ToString()!,
                            r["doc_level_id"]?.ToString()!,
                            r["doc_module_code"]?.ToString()!,
                            r["doc_academic_year"]?.ToString()!,
                            r["doc_admission_type"]?.ToString()!,
                            blob));
                    }
                    conn.Close();
                });
                log.AppendLine($"✓ Found {rows.Count} mark list records.");
                TxtMlStatus.Text = $"Exporting {rows.Count} files...";

                // Step 3: Export files and update paths
                int done = 0, errors = 0;
                foreach (var (dept, stream, level, mod, year, adm, blob) in rows)
                {
                    try
                    {
                        string? fp = null;
                        if (blob != null && blob.Length > 0)
                        {
                            // Sanitize filename
                            string safe(string s) => s.Replace("/","_").Replace("\\","_").Replace(":","_")
                                                       .Replace("*","_").Replace("?","_").Replace("\"","_")
                                                       .Replace("<","_").Replace(">","_").Replace("|","_")
                                                       .Replace(" ","_");
                            string fname = $"{safe(dept)}_{safe(stream)}_{safe(level)}_{safe(mod)}_{safe(year)}_{safe(adm)}.pdf";
                            fp = Path.Combine(dir, fname);
                            await File.WriteAllBytesAsync(fp, blob);
                        }

                        await Task.Run(() =>
                        {
                            var conn = db.GetConnection(); conn.Open();
                            using var upd = new MySqlCommand(
                                "UPDATE ecc_dof_wukrostmarycollege.mark_list_docs SET doc_file_path=@p " +
                                "WHERE doc_dept_id=@d AND doc_stream_id=@s AND doc_level_id=@l " +
                                "AND doc_module_code=@m AND doc_academic_year=@y AND doc_admission_type=@at", conn);
                            upd.Parameters.AddWithValue("@p", (object?)fp ?? DBNull.Value);
                            upd.Parameters.AddWithValue("@d", dept);  upd.Parameters.AddWithValue("@s", stream);
                            upd.Parameters.AddWithValue("@l", level); upd.Parameters.AddWithValue("@m", mod);
                            upd.Parameters.AddWithValue("@y", year);  upd.Parameters.AddWithValue("@at", adm);
                            upd.ExecuteNonQuery(); conn.Close();
                        });
                        done++;
                        log.AppendLine($"  ✓ {dept}/{mod}/{year} → {Path.GetFileName(fp ?? "none")}");
                    }
                    catch (Exception ex) { errors++; log.AppendLine($"  ✗ {dept}/{mod}: {ex.Message}"); }

                    if ((done + errors) % 3 == 0 || (done + errors) == rows.Count)
                    {
                        double pct = rows.Count > 0 ? (done + errors) * 100.0 / rows.Count : 100;
                        string snap = log.ToString();
                        Dispatcher.Invoke(() => { MlProgress.Value = pct; TxtMlStatus.Text = $"Exported {done}/{rows.Count}..."; TxtMlLog.Text = snap; });
                        await Task.Delay(1);
                    }
                }

                // Step 4: Drop the BLOB column
                log.AppendLine($"\n✓ Done — {done} exported, {errors} errors.");
                await Task.Run(() =>
                {
                    var conn = db.GetConnection(); conn.Open();
                    foreach (var sql in new[]
                    {
                        "ALTER TABLE ecc_dof_wukrostmarycollege.mark_list_docs MODIFY COLUMN doc_file LONGBLOB NULL",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.mark_list_docs DROP COLUMN doc_file",
                    })
                    {
                        try { new MySqlCommand(sql, conn).ExecuteNonQuery(); log.AppendLine($"✓ {sql}"); }
                        catch (Exception ex) { log.AppendLine($"  (skipped: {ex.Message.Split('\n')[0]})"); }
                    }
                    conn.Close();
                });

                log.AppendLine("\ndoc_file BLOB column removed. App now uses doc_file_path.");
                TxtMlLog.Text = log.ToString(); TxtMlStatus.Text = $"Complete — {done} files exported."; MlProgress.Value = 100;
            }
            catch (Exception ex) { TxtMlLog.Text = "Error: " + ex.Message; TxtMlStatus.Text = "Failed."; }
            BtnMlRun.IsEnabled = true;
        }
    }
}
