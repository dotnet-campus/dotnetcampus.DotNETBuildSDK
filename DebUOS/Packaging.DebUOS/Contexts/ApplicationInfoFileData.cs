using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Packaging.DebUOS.Contexts;

/// <summary>
/// 用于写入到 opt\apps\${AppId}\info 文件的数据内容
/// </summary>
/// 将使用 json 格式写入
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