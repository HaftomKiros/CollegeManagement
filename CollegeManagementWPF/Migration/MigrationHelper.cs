using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CollegeManagementWPF.Migration
{
    /// <summary>
    /// Migrates student_profile BLOB columns to file paths.
    /// Safe: adds new columns, exports files, updates paths.
    /// BLOB columns are NOT dropped automatically — review then drop manually.
    /// </summary>
    public class MigrationHelper
    {
        private readonly DBConnect _db;
        private readonly string _baseFolder;

        public MigrationHelper(string baseFolder)
        {
            _db = new DBConnect();
            // e.g. C:\CollegeFiles\
            _baseFolder = baseFolder;
            Directory.CreateDirectory(Path.Combine(_baseFolder, "photos"));
            Directory.CreateDirectory(Path.Combine(_baseFolder, "attachments"));
        }

        public async Task<(int migrated, int errors, string log)> RunAsync(
            IProgress<string>? progress = null)
        {
            int migrated = 0, errors = 0;
            var log = new System.Text.StringBuilder();

            // Step 1: Add path columns if they don't exist
            try
            {
                await Task.Run(() =>
                {
                    var conn = _db.GetConnection();
                    conn.Open();
                    // Add columns safely (ignore if already exist)
                    foreach (var sql in new[]
                    {
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile ADD COLUMN photo_path VARCHAR(500) NULL",
                        "ALTER TABLE ecc_dof_wukrostmarycollege.student_profile ADD COLUMN attachment_path VARCHAR(500) NULL"
                    })
                    {
                        try { new MySqlCommand(sql, conn).ExecuteNonQuery(); }
                        catch { /* column already exists */ }
                    }
                    conn.Close();
                });
                log.AppendLine("✓ Path columns added (or already exist).");
            }
            catch (Exception ex)
            {
                log.AppendLine($"✗ Failed to add columns: {ex.Message}");
                return (0, 1, log.ToString());
            }

            // Step 2: Read all students and export BLOBs
            try
            {
                var rows = await Task.Run(() =>
                {
                    var list = new System.Collections.Generic.List<(string id, string lvl, byte[]? photo, byte[]? attach)>();
                    var conn = _db.GetConnection();
                    conn.Open();
                    using var cmd = new MySqlCommand(
                        "SELECT student_id, level, photo, attachment FROM ecc_dof_wukrostmarycollege.student_profile", conn);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        byte[]? photo = r["photo"] != DBNull.Value ? (byte[])r["photo"] : null;
                        byte[]? attach = r["attachment"] != DBNull.Value ? (byte[])r["attachment"] : null;
                        list.Add((r["student_id"].ToString()!, r["level"].ToString()!, photo, attach));
                    }
                    conn.Close();
                    return list;
                });

                log.AppendLine($"✓ Found {rows.Count} student records to migrate.");
                progress?.Report($"Found {rows.Count} records...");

                foreach (var (id, lvl, photo, attach) in rows)
                {
                    try
                    {
                        string? photoPath = null;
                        string? attachPath = null;

                        // Save photo
                        if (photo != null && photo.Length > 0)
                        {
                            photoPath = Path.Combine(_baseFolder, "photos", $"{id}_L{lvl}.jpg");
                            await File.WriteAllBytesAsync(photoPath, photo);
                        }

                        // Save attachment
                        if (attach != null && attach.Length > 0)
                        {
                            attachPath = Path.Combine(_baseFolder, "attachments", $"{id}_L{lvl}.pdf");
                            await File.WriteAllBytesAsync(attachPath, attach);
                        }

                        // Update path columns in DB
                        await Task.Run(() =>
                        {
                            var conn = _db.GetConnection();
                            conn.Open();
                            using var upd = new MySqlCommand(
                                "UPDATE ecc_dof_wukrostmarycollege.student_profile SET photo_path=@pp, attachment_path=@ap " +
                                "WHERE student_id=@id AND level=@lvl", conn);
                            upd.Parameters.AddWithValue("@pp",  (object?)photoPath  ?? DBNull.Value);
                            upd.Parameters.AddWithValue("@ap",  (object?)attachPath ?? DBNull.Value);
                            upd.Parameters.AddWithValue("@id",  id);
                            upd.Parameters.AddWithValue("@lvl", lvl);
                            upd.ExecuteNonQuery();
                            conn.Close();
                        });

                        migrated++;
                        progress?.Report($"Migrated: {id} L{lvl}");
                        log.AppendLine($"  ✓ {id} L{lvl} → photo:{photoPath ?? "none"}, attach:{attachPath ?? "none"}");
                    }
                    catch (Exception ex)
                    {
                        errors++;
                        log.AppendLine($"  ✗ {id} L{lvl}: {ex.Message}");
                    }
                }

                log.AppendLine($"\n✓ Migration complete: {migrated} migrated, {errors} errors.");
                log.AppendLine($"\nFiles saved to: {_baseFolder}");
                log.AppendLine("\nNEXT STEP: Verify files are correct, then run this SQL to drop BLOB columns:");
                log.AppendLine("  ALTER TABLE ecc_dof_wukrostmarycollege.student_profile");
                log.AppendLine("      DROP COLUMN photo, DROP COLUMN attachment;");
            }
            catch (Exception ex)
            {
                log.AppendLine($"✗ Migration failed: {ex.Message}");
            }

            return (migrated, errors, log.ToString());
        }
    }
}
