using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Text.Json;

namespace CollegeManagementWPF
{
    /// <summary>
    /// Path configuration stored in the DB (app_config table).
    /// Three independent paths: photos, attachments, mark lists.
    /// Local JSON is a fallback when DB is unreachable.
    /// </summary>
    public class AppSettings
    {
        // ── Local fallback ────────────────────────────────────────────────────
        private static readonly string LocalPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StMaryCollege", "appsettings.json");

        // ── Default paths ─────────────────────────────────────────────────────
        private static readonly string DefaultPhotos =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StMaryCollege", "students", "photos");

        private static readonly string DefaultAttachments =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StMaryCollege", "students", "attachments");

        private static readonly string DefaultMarkLists =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StMaryCollege", "marklists");

        private static readonly string DefaultAssessments =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StMaryCollege", "assessments");

        // ── Properties (each stored independently in DB) ──────────────────────
        public string PhotosPath      { get; set; } = DefaultPhotos;
        public string AttachmentsPath { get; set; } = DefaultAttachments;
        public string MarkListsPath   { get; set; } = DefaultMarkLists;
        public string AssessmentsPath { get; set; } = DefaultAssessments;

        // ── Back-compat: StorageBasePath still usable where needed ────────────
        public string StorageBasePath
        {
            get => Path.GetDirectoryName(PhotosPath) ?? PhotosPath;
            set { /* ignored — paths are set individually now */ }
        }
        public string MarkListBasePath
        {
            get => MarkListsPath;
            set => MarkListsPath = value;
        }

        // ── Singleton ─────────────────────────────────────────────────────────
        private static AppSettings? _instance;
        public  static AppSettings  Current => _instance ??= Load();
        public  static void         Reload() => _instance = null;

        // ── Load: DB → local JSON → defaults ──────────────────────────────────
        public static AppSettings Load()
        {
            try
            {
                var db = new DBConnect();
                using var conn = db.GetConnection();
                conn.Open();
                EnsureTableExists(conn);

                var s = new AppSettings();
                using var cmd = new MySqlCommand(
                    "SELECT config_key, config_value FROM ecc_dof_wukrostmarycollege.path_config " +
                    "WHERE config_key IN ('photos_path','attachments_path','mark_list_path','assessments_path'," +
                    "'storage_base_path','mark_list_base_path')", conn);
                using var r = cmd.ExecuteReader();
                string? legacyBase = null, legacyMl = null;
                while (r.Read())
                {
                    string key = r["config_key"]?.ToString()   ?? "";
                    string val = r["config_value"]?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(val)) continue;
                    if (key == "photos_path")         s.PhotosPath      = val;
                    if (key == "attachments_path")    s.AttachmentsPath = val;
                    if (key == "mark_list_path")      s.MarkListsPath   = val;
                    if (key == "assessments_path")    s.AssessmentsPath = val;
                    if (key == "storage_base_path")   legacyBase        = val;
                    if (key == "mark_list_base_path") legacyMl          = val;
                }
                // Migrate from old single base path → derive individual paths
                if (legacyBase != null)
                {
                    if (s.PhotosPath      == DefaultPhotos)      s.PhotosPath      = Path.Combine(legacyBase, "photos");
                    if (s.AttachmentsPath == DefaultAttachments) s.AttachmentsPath = Path.Combine(legacyBase, "attachments");
                }
                if (legacyMl != null && s.MarkListsPath == DefaultMarkLists)
                    s.MarkListsPath = legacyMl;
                _instance = s;
                SaveLocal(s);
                return s;
            }
            catch { }

            // Fallback: local JSON
            try
            {
                if (File.Exists(LocalPath))
                {
                    var s = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(LocalPath));
                    if (s != null) { _instance = s; return s; }
                }
            }
            catch { }

            var def = new AppSettings();
            _instance = def;
            return def;
        }

        // ── Save: DB + local JSON ──────────────────────────────────────────────
        public void Save()
        {
            try
            {
                var db = new DBConnect();
                using var conn = db.GetConnection();
                conn.Open();
                EnsureTableExists(conn);
                Upsert(conn, "photos_path",      PhotosPath);
                Upsert(conn, "attachments_path", AttachmentsPath);
                Upsert(conn, "mark_list_path",   MarkListsPath);
                Upsert(conn, "assessments_path", AssessmentsPath);
            }
            catch { }

            SaveLocal(this);
            _instance = this;
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        public static void EnsureTableExists(MySqlConnection conn)
        {
            // Dedicated table for path configuration only
            new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS ecc_dof_wukrostmarycollege.path_config (" +
                "  config_key   VARCHAR(100) NOT NULL PRIMARY KEY," +
                "  config_value TEXT         NOT NULL," +
                "  updated_at   TIMESTAMP    DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;", conn).ExecuteNonQuery();
        }

        private static void Upsert(MySqlConnection conn, string key, string value)
        {
            var cmd = new MySqlCommand(
                "INSERT INTO ecc_dof_wukrostmarycollege.path_config (config_key, config_value) " +
                "VALUES (@k, @v) ON DUPLICATE KEY UPDATE config_value=@v", conn);
            cmd.Parameters.AddWithValue("@k", key);
            cmd.Parameters.AddWithValue("@v", value);
            cmd.ExecuteNonQuery();
        }

        private static void SaveLocal(AppSettings s)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LocalPath)!);
                File.WriteAllText(LocalPath,
                    JsonSerializer.Serialize(s, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }
    }
}
