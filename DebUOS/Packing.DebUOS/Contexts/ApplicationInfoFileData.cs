using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Packing.DebUOS.Contexts;

class ApplicationInfoFileData
{
    [JsonPropertyName("appid")]
    public string? AppId { init; get; }

    [JsonPropertyName("name")]
    public string? ApplicationName { init; get; }

    [JsonPropertyName("version")]
    public string? Version { init; get; }

    [JsonPropertyName("arch")]
    public IList<string>? Architecture { init; get; }

    [JsonPropertyName("permissions")]
    public ApplicationInfoPermissions? Permissions { set; get; }
}