using System.Text.Json.Serialization;

namespace Zse.Models;

public sealed class AppConfig
{
    [JsonPropertyName("isDarkMode")]
    public bool IsDarkMode { get; set; } = true;

    [JsonPropertyName("lastSaveDirectory")]
    public string LastSaveDirectory { get; set; } = string.Empty;

    [JsonPropertyName("createBackups")]
    public bool CreateBackups { get; set; } = true;
}

