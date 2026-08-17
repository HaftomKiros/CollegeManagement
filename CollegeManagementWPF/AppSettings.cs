using System;
using System.IO;
using System.Text.Json;

namespace CollegeManagementWPF
{
    public class AppSettings
    {
        private static readonly string SettingsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StMaryCollege", "appsettings.json");

        public string StorageBasePath { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StMaryCollege", "students");

        public string MarkListBasePath { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StMaryCollege", "marklists");

        private static AppSettings? _instance;
        public static AppSettings Current => _instance ??= Load();

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch { }
            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
                File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
                _instance = this;
            }
            catch { }
        }

        public string PhotosPath      => Path.Combine(StorageBasePath, "photos");
        public string AttachmentsPath => Path.Combine(StorageBasePath, "attachments");
        public string MarkListsPath   => MarkListBasePath;
    }
}
