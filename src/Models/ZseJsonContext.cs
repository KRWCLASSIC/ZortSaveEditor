using System.Collections.Generic;
using System.Text.Json.Serialization;
using Zse.Models;

namespace Zse.Services;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(AppConfig))]
[JsonSerializable(typeof(SettingsSave))]
[JsonSerializable(typeof(HashSet<string>))]
[JsonSerializable(typeof(List<int>))]
internal sealed partial class ZseJsonContext : JsonSerializerContext
{
}

