using Microsoft.Extensions.Logging;

using Packing.DebUOS.Contexts;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using Packing.DebUOS.Contexts.Configurations;

namespace Packing.DebUOS;

/// <summary>
/// 创建符合 UOS 安装包制作规范的打包文件夹
/// </summary>
/// 打包细节请参阅 [一步步教你在 Windows 上构建 dotnet 系应用的 UOS 软件安装包](https://blog.lindexi.com/post/%E4%B8%80%E6%AD%A5%E6%AD%A5%E6%95%99%E4%BD%A0%E5%9C%A8-Windows-%E4%B8%8A%E6%9E%84%E5%BB%BA-dotnet-%E7%B3%BB%E5%BA%94%E7%94%A8%E7%9A%84-UOS-%E8%BD%AF%E4%BB%B6%E5%AE%89%E8%A3%85%E5%8C%85.html )
// ReSharper disable once InconsistentNaming
public class DebUOSPackageFileStructCreator
{
    public DebUOSPackageFileStructCreator(ILogger logger)
    {
        Logger = logger;
    }

    public ILogger Logger { get; }

    public void CreatePackagingFolder(DebUOSConfiguration configuration)
    {
        var projectPublishFolder = configuration.ProjectPublishFolder;
        if (!Directory.Exists(projectPublishFolder))
        {
            Logger.LogError($"Project publish folder '{projectPublishFolder}' not exist");
            return;
        }

        var workingFolder = configuration.WorkingFolder;
        if (string.IsNullOrEmpty(workingFolder))
        {
            workingFolder = Path.Join(Path.GetTempPath(), "DebUOSPacking",
                $"{configuration.AssemblyName}_{Path.GetRandomFileName()}");
        }

        Directory.CreateDirectory(workingFolder);
        configuration.WorkingFolder = workingFolder;

        var packingFolder = configuration.PackingFolder;
        if (string.IsNullOrEmpty(packingFolder))
        {
            packingFolder = Path.Join(workingFolder, "Packing");
        }

        // 删除旧的文件夹，防止打包使用到旧文件
        if (Directory.Exists(packingFolder))
        {
            Directory.Delete(packingFolder, true);
        }

        Directory.CreateDirectory(packingFolder);
        configuration.PackingFolder = packingFolder;

        var appId = configuration.UOSAppId;
        if (string.IsNullOrEmpty(appId))
        {
            Logger.LogError($"找不到 UOS 的 AppId 内容，请确保已经配置 UOSAppId 属性");
            return;
        }

        var match = Regex.Match(appId, @"[a-z\.]+");
        if (!match.Success || match.Value != appId)
        {
            Logger.LogError(
                $"UOS 的 AppId 内容不符合规范，请确保配置的 UOSAppId 属性符合规范。请务必使用厂商的倒置域名+产品名作为应用包名，如 `com.example.demo` 格式，且只允许使用小写字母");
            return;
        }

        // opt\apps\AppId\
        // opt\apps\AppId\files
        var appIdFolder = Path.Join(packingFolder, "opt", "apps", appId);
        var filesFolder = Path.Join(appIdFolder, "files");
        Directory.CreateDirectory(filesFolder);
        var applicationBin = Path.Join(filesFolder, "bin");
        // 符号比拷贝速度快
        var symbol = Directory.CreateSymbolicLink(applicationBin, projectPublishFolder);
        if (!symbol.Exists)
        {
            Logger.LogError($"创建符号链接失败，从 '{projectPublishFolder}' 到 '{applicationBin}'");
            return;
        }

        // opt\apps\AppId\entries
        // opt\apps\AppId\entries\applications
        var entriesFolder = Path.Join(appIdFolder, "entries");
        var applicationsFolder = Path.Join(entriesFolder, "applications");
        var desktopFile = Path.Join(applicationsFolder, $"{appId}.desktop");
        Directory.CreateDirectory(applicationsFolder);

        // 不能使用 Encoding.UTF8 编码，因为默认会写入 BOM 导致 deb 打包失败
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        if (File.Exists(configuration.DebDesktopFile))
        {
            // 开发者配置了自定义的文件，则使用开发者的文件
            File.Copy(configuration.DebDesktopFile, desktopFile);
        }
        else
        {
            var stringBuilder = new StringBuilder();
            // 这里不能使用 AppendLine 方法，保持换行使用 \n 字符
            stringBuilder
                .Append("[Desktop Entry]\n")
                .Append($"Categories={configuration.DesktopCategories}\n")
                .Append($"Name={configuration.AppName}\n")
                .Append($"Keywords={configuration.DesktopKeywords}\n")
                .Append($"Comment={configuration.DesktopComment}\n")
                .Append($"Type={configuration.DesktopType}\n")
                .Append($"Terminal={configuration.DesktopTerminal.ToString().ToLowerInvariant()}\n")
                .Append($"StartupNotify={configuration.DesktopStartupNotify.ToString().ToLowerInvariant()}\n");

            if (!string.IsNullOrEmpty(configuration.AppNameZhCN))
            {
                stringBuilder.Append($"Name[zh_CN]={configuration.AppNameZhCN}\n");
            }

            if (!string.IsNullOrEmpty(configuration.DesktopKeywordsZhCN))
            {
                stringBuilder.Append($"Keywords[zh_CN]={configuration.DesktopKeywordsZhCN}\n");
            }

            if (!string.IsNullOrEmpty(configuration.DesktopCommentZhCN))
            {
                stringBuilder.Append($"Comment[zh_CN]={configuration.DesktopCommentZhCN}\n");
            }

            if (!string.IsNullOrEmpty(configuration.DesktopExec))
            {
                stringBuilder.Append($"Exec={configuration.DesktopExec}\n");
            }
            else
            {
                // 这里不能使用 Path.Join 方法，因为如果在 Windows 上进行打包，会将 \ 替换为 /，导致打包失败
                //var exec = Path.Join("/opt/apps", appId, "files", "bin", configuration.AssemblyName);
                var exec = $"/opt/apps/{appId}/files/bin/{configuration.AssemblyName}";
                stringBuilder.Append($"Exec={exec}\n");
            }

            if (!string.IsNullOrEmpty(configuration.DesktopIcon))
            {
                stringBuilder.Append($"Icon={configuration.DesktopIcon}\n");
            }
            else
            {
                stringBuilder.Append($"Icon={appId}\n");
            }

            if (!string.IsNullOrEmpty(configuration.DesktopMimeType))
            {
                stringBuilder.Append($"MimeType={configuration.DesktopMimeType}\n");
            }

            File.WriteAllText(desktopFile, stringBuilder.ToString(), encoding);
        }

        // opt\apps\AppId\entries\icons
        if (File.Exists(configuration.SvgIconFile))
        {
            var svgFile = Path.Join(entriesFolder, "icons", "hicolor", "scalable", "apps", $"{appId}.svg");
            Directory.CreateDirectory(Path.GetDirectoryName(svgFile)!);
            File.Copy(configuration.SvgIconFile, svgFile);
        }
        else
        {
            bool anyIconFileExist = false;
            foreach (var (iconFile, resolution) in new (string? iconFile, string resolution)[]
                     {
                         (configuration.Png16x16IconFile, "16x16"),
                         (configuration.Png24x24IconFile, "24x24"),
                         (configuration.Png32x32IconFile, "32x32"),
                         (configuration.Png48x48IconFile, "48x48"),
                         (configuration.Png128x128IconFile, "128x128"),
                         (configuration.Png256x256IconFile, "256x256"),
                         (configuration.Png512x512IconFile, "512x512"),
                     })
            {
                if (File.Exists(iconFile))
                {
                    var pngFile = Path.Join(entriesFolder, "icons", "hicolor", resolution, "apps", $"{appId}.png");
                    Directory.CreateDirectory(Path.GetDirectoryName(pngFile)!);
                    File.Copy(iconFile, pngFile);

                    anyIconFileExist = true;
                }
            }

            if (!anyIconFileExist)
            {
                Logger.LogWarning("找不到任何的图标文件，将导致应用无法在开始菜单显示。可通过 SvgIconFile 配置矢量图，可通过 Png16x16IconFile 等属性配置不同分辨率的图标");
            }
        }

        // opt\apps\AppId\info
        var infoJsonFile = Path.Join(appIdFolder, "info");
        if (File.Exists(configuration.DebInfoFile))
        {
            File.Copy(configuration.DebInfoFile, infoJsonFile);
        }
        else
        {
            var data = new ApplicationInfoFileData()
            {
                AppId = appId,
                ApplicationName = configuration.AppName,
                Version = configuration.Version,
                Architecture = configuration.Architecture.Split(';')
            };

            var permissions = configuration.InfoPermissions?.Split(';');
            if (permissions != null && permissions.Length > 0)
            {
                var applicationInfoPermissions = new ApplicationInfoPermissions();
                foreach (var permission in permissions)
                {
                    switch (permission)
                    {
                        case "autostart":
                            applicationInfoPermissions.Autostart = true;
                            break;
                        case "notification":
                            applicationInfoPermissions.Notification = true;
                            break;
                        case "trayicon":
                            applicationInfoPermissions.TrayIcon = true;
                            break;
                        case "clipboard":
                            applicationInfoPermissions.Clipboard = true;
                            break;
                        case "account":
                            applicationInfoPermissions.Account = true;
                            break;
                        case "bluetooth":
                            applicationInfoPermissions.Bluetooth = true;
                            break;
                        case "camera":
                            applicationInfoPermissions.Camera = true;
                            break;
                        case "audio_record":
                            applicationInfoPermissions.AudioRecord = true;
                            break;
                        case "installed_apps":
                            applicationInfoPermissions.InstalledApps = true;
                            break;
                    }
                }

                data.Permissions = applicationInfoPermissions;
            }

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions()
            {
                WriteIndented = true,
            }).Replace("\r\n", "\n");
            File.WriteAllText(infoJsonFile, json, encoding);
        }

        // 创建 control 文件
        var controlFile = Path.Join(packingFolder, "DEBIAN", "control");
        Directory.CreateDirectory(Path.GetDirectoryName(controlFile)!);
        if (File.Exists(configuration.DebControlFile))
        {
            File.Copy(configuration.DebControlFile, controlFile);
        }
        else
        {
            var stringBuilder = new StringBuilder();
            stringBuilder
                .Append($"Package: {appId}\n")
                .Append($"Version: {configuration.Version}\n")
                .Append($"Section: {configuration.DebControlSection}\n")
                .Append($"Priority: {configuration.DebControlPriority}\n")
                .Append($"Architecture: {configuration.Architecture}\n")
                .Append($"Multi-Arch: {configuration.DebControlMultiArch}\n")
                .Append($"Build-Depends: {configuration.DebControlBuildDepends}\n")
                .Append($"Standards-Version: {configuration.DebControlStandardsVersion}\n")
                .Append($"Homepage: {configuration.DebControlHomepage}\n")
                ;
            if (!string.IsNullOrEmpty(configuration.DebControlMaintainer))
            {
                stringBuilder.Append($"Maintainer: {configuration.DebControlMaintainer}\n");
            }
            else
            {
                Logger.LogWarning(
                    $"没有找到 DebControlMaintainer 属性配置。安装完成可能在开始菜单找不到应用。请配置 deb 包的维护者，如 <DebControlMaintainer>lindexi</DebControlMaintainer> 格式");
            }

            if (!string.IsNullOrEmpty(configuration.DebControlDescription))
            {
                stringBuilder.Append($"Description: {configuration.DebControlDescription}\n");
            }
            else
            {
                Logger.LogWarning($"没有找到 DebControlDescription 属性配置。安装完成可能在开始菜单找不到应用。请配置 deb 包的描述，描述可使用中文");
            }

            File.WriteAllText(controlFile, stringBuilder.ToString(), encoding);
        }
    }
}