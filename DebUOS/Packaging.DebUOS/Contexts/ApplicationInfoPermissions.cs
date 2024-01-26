using System.Text.Json.Serialization;

namespace Packaging.DebUOS.Contexts;

class ApplicationInfoPermissions
{
    [JsonPropertyName("autostart")]
    public bool Autostart { get; set; }
    [JsonPropertyName("notification")]
    public bool Notification { get; set; }
    [JsonPropertyName("trayicon")]
    public bool TrayIcon { get; set; }
    [JsonPropertyName("clipboard")]
    public bool Clipboard { get; set; }
    [JsonPropertyName("account")]
    public bool Account { get; set; }
    [JsonPropertyName("bluetooth")]
    public bool Bluetooth { get; set; }
    [JsonPropertyName("camera")]
    public bool Camera { get; set; }
    [JsonPropertyName("audio_record")]
    public bool AudioRecord { get; set; }
    [JsonPropertyName("installed_apps")]
    public bool InstalledApps { get; set; }
}