using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Zse.Models;

namespace Zse.Services;

public sealed class SaveManager
{
    private readonly SeagatePatcher _seagatePatcher = new();

    public string CurrentDirectory { get; private set; } = string.Empty;

    public SeagateSaveData? SeagateSave { get; set; }
    public SettingsSave? GameSettings { get; set; }
    public HashSet<string> FinishedMaps { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public EntitiesLog EntityLog { get; set; } = EntitiesLog.None;

    public AppConfig Config { get; private set; } = new();

    public SaveManager()
    {
        LoadAppConfig();
        CurrentDirectory = ResolveInitialSavePath();
    }

    public string ResolveInitialSavePath()
    {
        if (!string.IsNullOrWhiteSpace(Config.LastSaveDirectory) && Directory.Exists(Config.LastSaveDirectory))
        {
            return Config.LastSaveDirectory;
        }

        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string defaultZortPath = Path.Combine(userProfile, "AppData", "LocalLow", "Londer Software", "Zort");

        if (Directory.Exists(defaultZortPath))
        {
            return defaultZortPath;
        }

        return AppDomain.CurrentDomain.BaseDirectory;
    }

    public void SetSaveDirectory(string dir)
    {
        CurrentDirectory = dir;
        Config.LastSaveDirectory = dir;
        SaveAppConfig();
    }

    public void LoadAppConfig()
    {
        string cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        if (File.Exists(cfgPath))
        {
            try
            {
                string json = File.ReadAllText(cfgPath);
                Config = JsonSerializer.Deserialize(json, ZseJsonContext.Default.AppConfig) ?? new AppConfig();
            }
            catch
            {
                Config = new AppConfig();
            }
        }
    }

    public void SaveAppConfig()
    {
        string cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        try
        {
            string json = JsonSerializer.Serialize(Config, ZseJsonContext.Default.AppConfig);
            File.WriteAllText(cfgPath, json);
        }
        catch
        {
            // Ignore config write failures
        }
    }

    public void LoadAll()
    {
        if (!Directory.Exists(CurrentDirectory))
        {
            return;
        }

        // 1. Seagate (Ep 3)
        string seagatePath = Path.Combine(CurrentDirectory, "seagatesave.bin");
        if (File.Exists(seagatePath))
        {
            try
            {
                using var stream = File.OpenRead(seagatePath);
                SeagateSave = _seagatePatcher.Load(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Could not parse seagatesave.bin: {ex.Message}");
                SeagateSave = null;
            }
        }
        else
        {
            SeagateSave = null;
        }

        // 2. Game Settings
        string settingsPath = Path.Combine(CurrentDirectory, "settings.json");
        if (File.Exists(settingsPath))
        {
            try
            {
                string json = File.ReadAllText(settingsPath);
                GameSettings = JsonSerializer.Deserialize(json, ZseJsonContext.Default.SettingsSave);
            }
            catch
            {
                GameSettings = null;
            }
        }

        // 3. Finished Maps
        string finishedPath = Path.Combine(CurrentDirectory, "finished.json");
        if (File.Exists(finishedPath))
        {
            try
            {
                string json = File.ReadAllText(finishedPath);
                var set = JsonSerializer.Deserialize(json, ZseJsonContext.Default.HashSetString);
                FinishedMaps = set ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            catch
            {
                FinishedMaps.Clear();
            }
        }

        // 4. Entity Log (entitylog.txt or legacy enemiesseen.json)
        string txtPath = Path.Combine(CurrentDirectory, "entitylog.txt");
        if (File.Exists(txtPath))
        {
            try
            {
                string text = File.ReadAllText(txtPath).Trim();
                if (uint.TryParse(text, out uint val))
                {
                    EntityLog = (EntitiesLog)val;
                }
            }
            catch
            {
                EntityLog = EntitiesLog.None;
            }
        }
        else
        {
            string legacyPath = Path.Combine(CurrentDirectory, "enemiesseen.json");
            if (File.Exists(legacyPath))
            {
                try
                {
                    string json = File.ReadAllText(legacyPath);
                    var ids = JsonSerializer.Deserialize(json, ZseJsonContext.Default.ListInt32);
                    uint mask = 0;
                    if (ids != null)
                    {
                        foreach (int id in ids)
                        {
                            if (id >= 0 && id < 32)
                            {
                                mask |= (1u << id);
                            }
                        }
                    }
                    EntityLog = (EntitiesLog)mask;
                }
                catch
                {
                    EntityLog = EntitiesLog.None;
                }
            }
            else
            {
                EntityLog = EntitiesLog.None;
            }
        }
    }

    public void SaveAll()
    {
        if (!Directory.Exists(CurrentDirectory))
        {
            Directory.CreateDirectory(CurrentDirectory);
        }

        // 1. Seagate
        if (SeagateSave != null)
        {
            string seagatePath = Path.Combine(CurrentDirectory, "seagatesave.bin");
            CreateBackup(seagatePath);

            using var stream = File.Create(seagatePath);
            _seagatePatcher.Save(stream, SeagateSave);
        }

        // 2. Settings
        if (GameSettings != null)
        {
            string settingsPath = Path.Combine(CurrentDirectory, "settings.json");
            CreateBackup(settingsPath);
            string json = JsonSerializer.Serialize(GameSettings, ZseJsonContext.Default.SettingsSave);
            File.WriteAllText(settingsPath, json);
        }

        // 3. Finished Maps
        string finishedPath = Path.Combine(CurrentDirectory, "finished.json");
        CreateBackup(finishedPath);
        string finJson = JsonSerializer.Serialize(FinishedMaps, ZseJsonContext.Default.HashSetString);
        File.WriteAllText(finishedPath, finJson);

        // 4. Entity Log (entitylog.txt)
        string logPath = Path.Combine(CurrentDirectory, "entitylog.txt");
        CreateBackup(logPath);
        File.WriteAllText(logPath, ((uint)EntityLog).ToString());
    }

    private void CreateBackup(string filePath)
    {
        if (!Config.CreateBackups || !File.Exists(filePath)) return;

        try
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupPath = $"{filePath}.{timestamp}.bak";
            File.Copy(filePath, backupPath, overwrite: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] Backup creation failed for {filePath}: {ex.Message}");
        }
    }
}
