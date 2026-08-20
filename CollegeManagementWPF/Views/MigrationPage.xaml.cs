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
                Title            = "Select storage folder",
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
            TxtStatus.Text   = "Running migration...";
            Progress.Value   = 0;

            var log       = new StringBuilder();
            var db        = new DBConnect();
            var dir       = TxtFolder.Text.Trim();
            var photosDir = Path.Combine(dir, "photos");
            var attDir    = Path.Combine(dir, "attachments");

            Directory.CreateDirectory(photosDir);
            Directory.CreateDirectory(attDir);
            AppSettings.Current.StorageBasePath = dir;
            AppSettings.Current.Save();

            MySqlConnection NewConn()
            {
                var c = db.GetConnection();
                if (c == null) throw new Exception("Cannot connect to MySQL. Make sure WAMP/MySQL is running.");
                c.Open();
                try { new MySqlCommand("SET SESSION max_allowed_packet=268435456", c).ExecuteNonQuery(); } catch { }
                try { new MySqlCommand("SET SESSION net_read_timeout=600",         c).ExecuteNonQuery(); } catch { }
                try { new MySqlCommand("SET SESSION net_write_timeout=600",        c).ExecuteNonQuery(); } catch { }
                return c;
            }

            try
            {
                await Task.Run(() =>
                {
                    // ── Step 1: Add path columns (idempotent, verified) ──
                    foreach (var col in new[] { "photo_path", "attachment_path" })
                    {
                        // Check first via INFORMATION_SCHEMA so we don't swallow real errors
                        using var cChk = NewConn();
                        var exists = Convert.ToInt32(new MySqlCommand(
                            $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                            $"WHERE TABLE_SCHEMA='ecc_dof_wukrostmarycollege' " +
                            $"AND TABLE_NAME='student_profile' AND COLUMN_NAME='{col}'", cChk).ExecuteScalar()) > 0;

                        if (exists)
                        {
                            log.AppendLine($"  ({col} already exists — OK)");
                        }
                        else
                        {
                            using var cAdd = NewConn();
                            new MySqlCommand(
                                $"ALTER TABLE ecc_dof_wukrostmarycollege.student_profile ADD COLUMN {col} VARCHAR(500) NULL",
                                cAdd).ExecuteNonQuery();
                            log.AppendLine($"✓ {col} column added.");
                        }
                    }

                    // ── Step 2: Detect BLOB columns ──
                    bool photoExists = false, attExists = false;
                    using (var c2 = NewConn())
                    {
                        try
                        {
                            var cmd = new MySqlCommand(
                                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                                "WHERE TABLE_SCHEMA='ecc_dof_wukrostmarycollege' " +
                                "AND TABLE_NAME='student_profile' AND COLUMN_NAME='photo'", c2);
                            photoExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                            log.AppendLine(photoExists ? "✓ photo BLOB column detected." : "  (photo column not found)");
                        }
                        catch (Exception ex) { log.AppendLine($"  (photo check error: {ex.Message.Split('\n')[0]})"); }

                        try
                        {
                            var cmd = new MySqlCommand(
                                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                                "WHERE TABLE_SCHEMA='ecc_dof_wukrostmarycollege' " +
                                "AND TABLE_NAME='student_profile' AND COLUMN_NAME='attachment'", c2);
                            attExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                            log.AppendLine(attExists ? "✓ attachment BLOB column detected." : "  (attachment column not found)");
                        }
                        catch (Exception ex) { log.AppendLine($"  (attachment check error: {ex.Message.Split('\n')[0]})"); }
                    }

                    // ── Step 3: Extract BLOBs → files → update paths ──
                    int extracted = 0, extractErrors = 0;

                    if (photoExists || attExists)
                    {
                        log.AppendLine($"Extracting BLOBs (photo={photoExists}, attachment={attExists})...");
                        var blobRows = new System.Collections.Generic.List<(string sid, byte[]? ph, byte[]? at2)>();

                        using (var c3 = NewConn())
                        {
                            try
                            {
                                // Include level — needed for the UPDATE WHERE clause
                                string cols   = "student_id,level" + (photoExists ? ",photo" : "") + (attExists ? ",attachment" : "");
                                string where2 = photoExists && attExists ? "WHERE photo IS NOT NULL OR attachment IS NOT NULL"
                                              : photoExists              ? "WHERE photo IS NOT NULL"
                                                                         : "WHERE attachment IS NOT NULL";
                                var cmd = new MySqlCommand(
                                    $"SELECT {cols} FROM ecc_dof_wukrostmarycollege.student_profile {where2}", c3);
                                cmd.CommandTimeout = 600;

                                using var r = cmd.ExecuteReader();
                                while (r.Read())
                                {
                                    string  sid = r["student_id"]?.ToString() ?? "";
                                    string  lvl = r["level"]?.ToString()      ?? "";
                                    byte[]? ph  = photoExists && r["photo"]      != DBNull.Value ? (byte[])r["photo"]      : null;
                                    byte[]? at2 = attExists   && r["attachment"] != DBNull.Value ? (byte[])r["attachment"] : null;
                                    if (ph != null || at2 != null) blobRows.Add((sid + "|" + lvl, ph, at2));
                                }
                                log.AppendLine($"✓ Read {blobRows.Count} rows with BLOB data.");
                            }
                            catch (Exception ex) { log.AppendLine($"  BLOB read error: {ex.Message.Split('\n')[0]}"); }
                        }

                        if (blobRows.Count > 0)
                        {
                            // Track already-written files so all levels of a student share one file
                            var writtenPhotos  = new System.Collections.Generic.HashSet<string>();
                            var writtenAttachs = new System.Collections.Generic.HashSet<string>();

                            using var c4 = NewConn();
                            var updCmd = new MySqlCommand(
                                "UPDATE ecc_dof_wukrostmarycollege.student_profile " +
                                "SET photo_path=@pp, attachment_path=@ap WHERE student_id=@s AND level=@lvl", c4);

                            foreach (var (sidLvl, ph, at2) in blobRows)
                            {
                                var parts  = sidLvl.Split('|');
                                string sid = parts[0];
                                string lvl = parts.Length > 1 ? parts[1] : "";
                                string safeSid = sid.Replace("/","_").Replace("\\","_").Replace(":","_")
                                                    .Replace("*","_").Replace("?","_").Replace("\"","_")
                                                    .Replace("<","_").Replace(">","_").Replace("|","_");
                                try
                                {
                                    string? ppFile = null, apFile = null;

                                    if (ph != null && ph.Length > 0)
                                    {
                                        ppFile = safeSid + ".jpg";
                                        if (!writtenPhotos.Contains(ppFile))
                                        {
                                            File.WriteAllBytes(Path.Combine(photosDir, ppFile), ph);
                                            writtenPhotos.Add(ppFile);
                                        }
                                    }
                                    if (at2 != null && at2.Length > 0)
                                    {
                                        apFile = safeSid + ".pdf";
                                        if (!writtenAttachs.Contains(apFile))
                                        {
                                            File.WriteAllBytes(Path.Combine(attDir, apFile), at2);
                                            writtenAttachs.Add(apFile);
                                        }
                                    }

                                    updCmd.Parameters.Clear();
                                    updCmd.Parameters.AddWithValue("@pp",  (object?)ppFile ?? DBNull.Value);
                                    updCmd.Parameters.AddWithValue("@ap",  (object?)apFile ?? DBNull.Value);
                                    updCmd.Parameters.AddWithValue("@s",   sid);
                                    updCmd.Parameters.AddWithValue("@lvl", lvl);
                                    updCmd.ExecuteNonQuery();
                                    extracted++;
                                }
                                catch (Exception ex2) { extractErrors++; log.AppendLine($"  ✗ {sid} L{lvl}: {ex2.Message.Split('\n')[0]}"); }
                            }
                            log.AppendLine($"✓ Extracted {extracted} rows. Unique photo files: {writtenPhotos.Count}, attachment files: {writtenAttachs.Count}. Errors: {extractErrors}");
                        }

                        // Drop BLOB columns only if everything succeeded
                        if (extractErrors == 0)
                        {
                            foreach (var col in new[] { "photo", "attachment" })
                            {
                                // Verify column still exists before attempting DROP
                                bool colExists;
                                using (var cChk = NewConn())
                                {
                                    colExists = Convert.ToInt32(new MySqlCommand(
                                        $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                                        $"WHERE TABLE_SCHEMA='ecc_dof_wukrostmarycollege' " +
                                        $"AND TABLE_NAME='student_profile' AND COLUMN_NAME='{col}'", cChk).ExecuteScalar()) > 0;
                                }

                                if (!colExists)
                                {
                                    log.AppendLine($"  ({col} BLOB column already gone — OK)");
                                    continue;
                                }

                                // Use a fresh connection for each DDL statement
                                using var cDrop = NewConn();
                                try
                                {
                                    new MySqlCommand(
                                        $"ALTER TABLE ecc_dof_wukrostmarycollege.student_profile DROP COLUMN {col}",
                                        cDrop).ExecuteNonQuery();
                                    log.AppendLine($"✓ {col} BLOB column removed.");
                                }
                                catch (Exception ex) { log.AppendLine($"  (skip DROP {col}: {ex.Message.Split('\n')[0]})"); }
                            }
                        }
                        else
                        {
                            log.AppendLine($"  ⚠ {extractErrors} errors — BLOB columns kept to preserve data.");
                        }
                    }
                    else
                    {
                        log.AppendLine("  (no BLOB columns — database already migrated)");

                        // Re-point existing paths to new folder if user changed the base dir
                        using var cR = NewConn();
                        int repointed = 0;
                        try
                        {
                            var cmdR = new MySqlCommand(
                                "SELECT DISTINCT student_id,photo_path,attachment_path " +
                                "FROM ecc_dof_wukrostmarycollege.student_profile " +
                                "WHERE (photo_path IS NOT NULL AND photo_path!='') " +
                                "OR (attachment_path IS NOT NULL AND attachment_path!='')", cR);
                            using var rdr  = cmdR.ExecuteReader();
                            var list = new System.Collections.Generic.List<(string, string?, string?)>();
                            while (rdr.Read())
                                list.Add((rdr["student_id"]?.ToString() ?? "", rdr["photo_path"]?.ToString(), rdr["attachment_path"]?.ToString()));
                            rdr.Close();

                            if (list.Count == 0)
                            {
                                // Count total students to distinguish fresh DB vs lost-BLOB scenario
                                int totalStu = Convert.ToInt32(new MySqlCommand(
                                    "SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_profile", cR).ExecuteScalar());
                                if (totalStu > 0)
                                    log.AppendLine($"  ⚠ {totalStu} student records found but photo_path is NULL for all.");
                                log.AppendLine("  → BLOB columns are gone and no file paths exist.");
                                log.AppendLine("  → Photos must be uploaded individually via Student Registration.");
                            }
                            else
                            {
                                foreach (var (sid, oldPp, oldAp) in list)
                                {
                                    // Keep only filename — full path resolved at runtime via AppSettings
                                    string? newPp = string.IsNullOrEmpty(oldPp) ? null : Path.GetFileName(oldPp);
                                    string? newAp = string.IsNullOrEmpty(oldAp) ? null : Path.GetFileName(oldAp);
                                    var upd = new MySqlCommand(
                                        "UPDATE ecc_dof_wukrostmarycollege.student_profile " +
                                        "SET photo_path=@pp, attachment_path=@ap WHERE student_id=@s", cR);
                                    upd.Parameters.AddWithValue("@pp", (object?)newPp ?? DBNull.Value);
                                    upd.Parameters.AddWithValue("@ap", (object?)newAp ?? DBNull.Value);
                                    upd.Parameters.AddWithValue("@s",  sid);
                                    upd.ExecuteNonQuery();
                                    repointed++;
                                }
                                log.AppendLine($"✓ Re-pointed {repointed} paths to filenames only.");
                            }
                        }
                        catch (Exception ex) { log.AppendLine($"  (re-point: {ex.Message.Split('\n')[0]})"); }
                    }

                    // ── Final stats ──
                    using var cFinal = NewConn();
                    int total     = Convert.ToInt32(new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_profile", cFinal).ExecuteScalar());
                    int withPhoto = Convert.ToInt32(new MySqlCommand("SELECT COUNT(*) FROM ecc_dof_wukrostmarycollege.student_profile WHERE photo_path IS NOT NULL AND photo_path!=''", cFinal).ExecuteScalar());
                    log.AppendLine($"\n✓ Total students: {total}");
                    log.AppendLine($"✓ With photo path: {withPhoto}");
                    log.AppendLine($"✓ Storage folder: {dir}");
                    log.AppendLine("\n✓ Migration complete. Database is ready to use.");
                });

                TxtLog.Text    = log.ToString();
                TxtStatus.Text = "Complete.";
                Progress.Value = 100;
            }
            catch (Exception ex)
            {
                // Unwrap inner exception to get the real MySQL error
                var inner = ex.InnerException ?? ex;
                TxtLog.Text    = $"Error: {ex.Message}\n\nDetail: {inner.Message}";
                TxtStatus.Text = "Failed.";
            }
            finally
            {
                BtnRun.IsEnabled = true;
            }
        }

        // ── Tab 2: Mark List Documents BLOB migration ─────────────────────────

        private void BrowseMlFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title            = "Select storage folder for mark lists",
                CheckFileExists  = false,
                CheckPathExists  = true,
                FileName         = "Select Folder",
                Filter           = "Folder|*.none",
                InitialDirectory = TxtMlFolder.Text
            };
            if (dlg.ShowDialog() == true)
                TxtMlFolder.Text = Path.GetDirectoryName(dlg.FileName) ?? TxtMlFolder.Text;
        }

        private async void BtnMlRun_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtMlFolder.Text))
            {
                TxtMlStatus.Text = "Please select a storage folder.";
                return;
            }

            BtnMlRun.IsEnabled = false;
            TxtMlLog.Text      = "";
            TxtMlStatus.Text   = "Starting mark list migration...";
            MlProgress.Value   = 0;

            var log = new StringBuilder();
            var db  = new DBConnect();
            var dir = TxtMlFolder.Text.Trim();
            Directory.CreateDirectory(dir);
            AppSettings.Current.MarkListBasePath = dir;
            AppSettings.Current.Save();

            try
            {
                // ── Step 1: Add doc_file_path column (idempotent) ──
                await Task.Run(() =>
                {
                    using var conn = db.GetConnection();
                    conn.Open();
                    try
                    {
                        new MySqlCommand(
                            "ALTER TABLE ecc_dof_wukrostmarycollege.mark_list_docs ADD COLUMN doc_file_path VARCHAR(500) NULL",
                            conn).ExecuteNonQuery();
                        log.AppendLine("✓ doc_file_path column added.");
                    }
                    catch { log.AppendLine("  (doc_file_path already exists — skipped)"); }
                });

                // ── Step 2: Detect whether doc_file BLOB column still exists ──
                bool blobColExists = false;
                await Task.Run(() =>
                {
                    using var conn = db.GetConnection();
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                        "WHERE TABLE_SCHEMA='ecc_dof_wukrostmarycollege' " +
                        "AND TABLE_NAME='mark_list_docs' AND COLUMN_NAME='doc_file'", conn);
                    blobColExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    log.AppendLine(blobColExists
                        ? "✓ doc_file BLOB column detected."
                        : "  (doc_file column not found — already migrated)");
                });

                if (!blobColExists)
                {
                    log.AppendLine("\n✓ Nothing to migrate. Mark list docs already on file paths.");
                    TxtMlLog.Text    = log.ToString();
                    TxtMlStatus.Text = "Already migrated.";
                    MlProgress.Value = 100;
                    BtnMlRun.IsEnabled = true;
                    return;
                }

                // ── Step 3: Read all BLOB records ──
                var rows = new System.Collections.Generic.List<(string dept, string stream, string level, string mod, string year, string adm, byte[]? file)>();
                await Task.Run(() =>
                {
                    using var conn = db.GetConnection();
                    conn.Open();
                    try { new MySqlCommand("SET SESSION max_allowed_packet=268435456", conn).ExecuteNonQuery(); } catch { }
                    try { new MySqlCommand("SET SESSION net_read_timeout=600",         conn).ExecuteNonQuery(); } catch { }
                    try { new MySqlCommand("SET SESSION net_write_timeout=600",        conn).ExecuteNonQuery(); } catch { }

                    using var cmd = new MySqlCommand(
                        "SELECT doc_dept_id, doc_stream_id, doc_level_id, doc_module_code, " +
                        "doc_academic_year, doc_admission_type, doc_file " +
                        "FROM ecc_dof_wukrostmarycollege.mark_list_docs", conn);
                    cmd.CommandTimeout = 600;
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        byte[]? blob = r["doc_file"] != DBNull.Value ? (byte[])r["doc_file"] : null;
                        rows.Add((
                            r["doc_dept_id"]?.ToString()      ?? "",
                            r["doc_stream_id"]?.ToString()    ?? "",
                            r["doc_level_id"]?.ToString()     ?? "",
                            r["doc_module_code"]?.ToString()  ?? "",
                            r["doc_academic_year"]?.ToString() ?? "",
                            r["doc_admission_type"]?.ToString() ?? "",
                            blob));
                    }
                });

                log.AppendLine($"✓ Found {rows.Count} mark list records.");
                TxtMlLog.Text    = log.ToString();
                TxtMlStatus.Text = $"Exporting {rows.Count} files...";

                // ── Step 4: Export files and update paths ──
                int done = 0, errors = 0;

                string Sanitize(string s) =>
                    s.Replace("/","_").Replace("\\","_").Replace(":","_")
                     .Replace("*","_").Replace("?","_").Replace("\"","_")
                     .Replace("<","_").Replace(">","_").Replace("|","_")
                     .Replace(" ","_");

                foreach (var (dept, stream, level, mod, year, adm, blob) in rows)
                {
                    try
                    {
                        string? fp = null;
                        if (blob != null && blob.Length > 0)
                        {
                            string fname = $"{Sanitize(dept)}_{Sanitize(stream)}_{Sanitize(level)}_{Sanitize(mod)}_{Sanitize(year)}_{Sanitize(adm)}.pdf";
                            fp = Path.Combine(dir, fname);
                            await File.WriteAllBytesAsync(fp, blob);
                        }

                        await Task.Run(() =>
                        {
                            using var conn = db.GetConnection();
                            conn.Open();
                            using var upd = new MySqlCommand(
                                "UPDATE ecc_dof_wukrostmarycollege.mark_list_docs SET doc_file_path=@p " +
                                "WHERE doc_dept_id=@d AND doc_stream_id=@s AND doc_level_id=@l " +
                                "AND doc_module_code=@m AND doc_academic_year=@y AND doc_admission_type=@at", conn);
                            upd.Parameters.AddWithValue("@p",  (object?)fp ?? DBNull.Value);
                            upd.Parameters.AddWithValue("@d",  dept);
                            upd.Parameters.AddWithValue("@s",  stream);
                            upd.Parameters.AddWithValue("@l",  level);
                            upd.Parameters.AddWithValue("@m",  mod);
                            upd.Parameters.AddWithValue("@y",  year);
                            upd.Parameters.AddWithValue("@at", adm);
                            upd.ExecuteNonQuery();
                        });

                        done++;
                        log.AppendLine($"  ✓ {dept}/{mod}/{year} → {Path.GetFileName(fp ?? "none")}");
                    }
                    catch (Exception ex)
                    {
                        errors++;
                        log.AppendLine($"  ✗ {dept}/{mod}: {ex.Message.Split('\n')[0]}");
                    }

                    // Update UI periodically
                    if ((done + errors) % 5 == 0 || (done + errors) == rows.Count)
                    {
                        double pct  = rows.Count > 0 ? (done + errors) * 100.0 / rows.Count : 100;
                        string snap = log.ToString();
                        Dispatcher.Invoke(() =>
                        {
                            MlProgress.Value = pct;
                            TxtMlStatus.Text = $"Exported {done}/{rows.Count}...";
                            TxtMlLog.Text    = snap;
                        });
                        await Task.Delay(1);
                    }
                }

                log.AppendLine($"\n✓ Done — {done} exported, {errors} errors.");

                // ── Step 5: Drop the BLOB column (only if no errors) ──
                if (errors == 0)
                {
                    await Task.Run(() =>
                    {
                        using var conn = db.GetConnection();
                        conn.Open();
                        foreach (var sql in new[]
                        {
                            "ALTER TABLE ecc_dof_wukrostmarycollege.mark_list_docs MODIFY COLUMN doc_file LONGBLOB NULL",
                            "ALTER TABLE ecc_dof_wukrostmarycollege.mark_list_docs DROP COLUMN doc_file"
                        })
                        {
                            try   { new MySqlCommand(sql, conn).ExecuteNonQuery(); log.AppendLine("✓ doc_file BLOB column removed."); }
                            catch (Exception ex) { log.AppendLine($"  (skip: {ex.Message.Split('\n')[0]})"); }
                        }
                    });
                }
                else
                {
                    log.AppendLine($"  ⚠ {errors} errors — doc_file BLOB column kept to preserve data.");
                }

                TxtMlLog.Text    = log.ToString();
                TxtMlStatus.Text = errors == 0 ? "Complete." : $"Done with {errors} errors.";
                MlProgress.Value = 100;
            }
            catch (Exception ex)
            {
                TxtMlLog.Text    = log.ToString() + "\nError: " + ex.Message;
                TxtMlStatus.Text = "Failed.";
            }
            finally
            {
                BtnMlRun.IsEnabled = true;
            }
        }
    }
}
