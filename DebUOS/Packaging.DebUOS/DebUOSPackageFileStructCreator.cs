using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using Packaging.DebUOS.Contexts;
using Packaging.DebUOS.Contexts.Configurations;
using Packaging.DebUOS.Exceptions;

using Walterlv.IO.PackageManagement;

namespace Packaging.DebUOS;

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

    /// <summary>
    /// 创建符合 UOS 安装包制作规范的打包文件夹，后续此文件夹完全支持 dpkg 工具直接打包
    /// </summary>
    /// <param name="configuration"></param>
    /// <exception cref="PackagingException"></exception>
    public void CreatePackagingFolder(DebUOSConfiguration configuration)
    {
        var projectPublishFolder = configuration.ProjectPublishFolder;
        if (!Directory.Exists(projectPublishFolder))
        {
            throw new PackagingException($"Project publish folder '{projectPublishFolder}' not exist");
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
            PackageDirectory.Delete(packingFolder);
        }

        Directory.CreateDirectory(packingFolder);
        configuration.PackingFolder = packingFolder;

        if (File.Exists(configuration.DebUOSOutputFilePath))
        {
            // 如果存在输出文件，则先删除，防止后续又将其加入压缩文件
            File.Delete(configuration.DebUOSOutputFilePath);
        }

        var appId = configuration.UOSAppId;
        if (string.IsNullOrEmpty(appId))
        {
            throw new PackagingException($"找不到 UOS 的 AppId 内容，请确保已经配置 UOSAppId 属性");
        }

        var match = Regex.Match(appId, @"[a-z\.]+");
        if (!match.Success || match.Value != appId)
        {
            throw new PackagingException($"UOS 的 AppId 内容不符合规范，请确保配置的 UOSAppId 属性符合规范。请务必使用厂商的倒置域名+产品名作为应用包名，如 `com.example.demo` 格式，且只允许使用小写字母。UOSAppId={appId}");
        }

        // opt\apps\AppId\
        // opt\apps\AppId\files
        var appIdFolder = Path.Join(packingFolder, "opt", "apps", appId);
        var filesFolder = Path.Join(appIdFolder, "files");
        Directory.CreateDirectory(filesFolder);
        var applicationBin = Path.Join(filesFolder, "bin");
        if (!FolderUtils.CreateSymbolLinkOrCopyFolder(projectPublishFolder, applicationBin))
        {
            throw new PackagingException($"将发布输出文件拷贝到安装包打包文件夹失败，从 '{projectPublishFolder}' 复制到 '{applicationBin}' 失败");
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
                .Append($"Categories={configuration.DesktopCategories.TrimEnd(',')};\n")
                .Append($"Name={configuration.AppName}\n")
                .Append($"Keywords={configuration.DesktopKeywords.TrimEnd(',')};\n")
                .Append($"Comment={configuration.DesktopComment}\n")
                .Append($"Type={configuration.DesktopType}\n")
                .Append($"Terminal={configuration.DesktopTerminal.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()}\n")
                .Append($"StartupNotify={configuration.DesktopStartupNotify.ToString().ToLowerInvariant()}\n");

            if (!string.IsNullOrEmpty(configuration.AppNameZhCN))
            {
                stringBuilder.Append($"Name[zh_CN]={configuration.AppNameZhCN}\n");
            }

            if (!string.IsNullOrEmpty(configuration.DesktopKeywordsZhCN))
            {
                stringBuilder.Append($"Keywords[zh_CN]={configuration.DesktopKeywordsZhCN.TrimEnd(';')};\n");
            }

            if (!string.IsNullOrEmpty(configuration.DesktopCommentZhCN))
            {
                stringBuilder.Append($"Comment[zh_CN]={configuration.DesktopCommentZhCN}\n");
            }

            if (configuration.DesktopNoDisplay is not null)
            {
                stringBuilder.Append($"NoDisplay={configuration.DesktopNoDisplay.Value.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()}\n");
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
                stringBuilder.Append($"MimeType={configuration.DesktopMimeType.TrimEnd(';')};\n");
            }

            File.WriteAllText(desktopFile, stringBuilder.ToString(), encoding);
        }

        if (configuration.CopyDesktopFileToUsrShareApplications)
        {
            var userShareApplicationsFolder = Path.Join(packingFolder, "usr", "share", "applications");
            Directory.CreateDirectory(userShareApplicationsFolder);
            var userShareDesktopFile = Path.Join(userShareApplicationsFolder, $"{appId}.desktop");
            File.Copy(desktopFile, userShareDesktopFile);
        }

        // opt\apps\AppId\entries\icons
        var iconsFolder = Path.Join(entriesFolder, "icons");
        if (!string.IsNullOrEmpty(configuration.UOSDebIconFolder))
        {
            // 如果开发者配置了自定义的图标文件夹，则使用开发者的文件夹
            if (!Directory.Exists(configuration.UOSDebIconFolder))
            {
                throw new PackagingException($"配置了 Icon 文件夹的 UOSDebIconFolder 属性，但文件夹不存在 UOSDebIconFolder={configuration.UOSDebIconFolder} FullPath={Path.GetFullPath(configuration.UOSDebIconFolder)}");
            }

            PackageDirectory.Copy(configuration.UOSDebIconFolder, iconsFolder);
        }
        else if (File.Exists(configuration.SvgIconFile))
        {
            // 如果开发者配置了自定义的矢量图标文件，则优先使用矢量图标
            var svgFile = Path.Join(iconsFolder, "hicolor", "scalable", "apps", $"{appId}.svg");
            Directory.CreateDirectory(Path.GetDirectoryName(svgFile)!);
            File.Copy(configuration.SvgIconFile, svgFile);
            if (configuration.CopyIconsToUsrShareIcons)
            {
                var userShareSvgFolder = Path.Join(packingFolder, "usr", "share", "icons", "hicolor", "scalable", "apps");
                if (!Directory.Exists(userShareSvgFolder))
                {
                    Directory.CreateDirectory(userShareSvgFolder);
                }
                File.Copy(configuration.SvgIconFile, Path.Join(userShareSvgFolder, $"{appId}.svg"));
            }
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
                         (configuration.Png64x64IconFile, "64x64"),
                         (configuration.Png128x128IconFile, "128x128"),
                         (configuration.Png256x256IconFile, "256x256"),
                         (configuration.Png512x512IconFile, "512x512"),
                     })
            {
                if (!string.IsNullOrEmpty(iconFile))
                {
                    if (File.Exists(iconFile))
                    {
                        var pngFile = Path.Join(iconsFolder, "hicolor", resolution, "apps", $"{appId}.png");
                        Directory.CreateDirectory(Path.GetDirectoryName(pngFile)!);
                        File.Copy(iconFile, pngFile);
                        anyIconFileExist = true;
                        if (configuration.CopyIconsToUsrShareIcons)
                        {
                            var userSharePngFolder = Path.Join(packingFolder, "usr", "share", "icons", "hicolor", resolution, "apps");
                            if (!Directory.Exists(userSharePngFolder))
                            {
                                Directory.CreateDirectory(userSharePngFolder);
                            }
                            File.Copy(iconFile, Path.Join(userSharePngFolder, $"{appId}.png"), true);
                        }
                    }
                    else
                    {
                        Logger.LogWarning($"配置了 {resolution} 的图标文件路径，但是找不到图标文件 图标文件={iconFile} 图标文件绝对路径={Path.GetFullPath(iconFile)}");
                    }
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
            var infoPermissions = configuration.InfoPermissions;

            var data = new ApplicationInfoFileData()
            {
                AppId = appId,
                ApplicationName = configuration.AppName,
                Version = configuration.UOSDebVersion,
                Architecture = configuration.Architecture.Split(';')
            };

            var permissions = infoPermissions?.Split(';');
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
                .Append($"Version: {configuration.UOSDebVersion}\n")
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

            if (!string.IsNullOrEmpty(configuration.DebControlDepends))
            {
                stringBuilder.Append($"Depends: {configuration.DebControlDepends}\n");
            }

            File.WriteAllText(controlFile, stringBuilder.ToString(), encoding);
        }

        if (!string.IsNullOrEmpty(configuration.DebPostinstFile))
        {
            if (File.Exists(configuration.DebPostinstFile))
            {
                var postinstFile = Path.Join(packingFolder, "DEBIAN", "postinst");
                File.Copy(configuration.DebPostinstFile, postinstFile);
            }
            else
            {
                ThrowFileNotFoundException(configuration.DebPostinstFile, nameof(configuration.DebPostinstFile));
            }
        }

        if (!string.IsNullOrEmpty(configuration.DebPrermFile))
        {
            if (File.Exists(configuration.DebPrermFile))
            {
                var prermFile = Path.Join(packingFolder, "DEBIAN", "prerm");
                File.Copy(configuration.DebPrermFile, prermFile);
            }
            else
            {
                ThrowFileNotFoundException(configuration.DebPrermFile, nameof(configuration.DebPrermFile));
            }
        }

        if (!string.IsNullOrEmpty(configuration.DebPostrmFile))
        {
            if (File.Exists(configuration.DebPostrmFile))
            {
                var postrmFile = Path.Join(packingFolder, "DEBIAN", "postrm");
                File.Copy(configuration.DebPostrmFile, postrmFile);
            }
            else
            {
                ThrowFileNotFoundException(configuration.DebPostrmFile, nameof(configuration.DebPostrmFile));
            }
        }

        if (!string.IsNullOrEmpty(configuration.DebPreinstFile))
        {
            if (File.Exists(configuration.DebPreinstFile))
            {
                var preinstFile = Path.Join(packingFolder, "DEBIAN", "preinst");
                File.Copy(configuration.DebPreinstFile, preinstFile);
            }
            else
            {
                ThrowFileNotFoundException(configuration.DebPreinstFile, nameof(configuration.DebPreinstFile));
            }
        }
     

        static void ThrowFileNotFoundException(string file, string propertyName)
        {
            throw new FileNotFoundException($"已配置 `{propertyName}` 属性，但找不到其配置的 '{file}' 文件", Path.GetFullPath(file));
        }
    }

    static class FolderUtils
    {
        public static bool CreateSymbolLinkOrCopyFolder(string sourceFolder, string destinationFolder)
        {
            try
            {
                try
                {
                    if (OperatingSystem.IsWindows())
                    {
                        // 在 Win 下可以创建 JunctionPoint 联接试试
                        // 这是不需要权限的，比 Directory.CreateSymbolicLink 更好
                        var ioResult = PackageDirectory.Link(destinationFolder, sourceFolder);

                        if (ioResult)
                        {
                            // 创建成功
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    // 失败了，继续尝试
                }

                try
                {
                    // 符号比拷贝速度快
                    var symbol = Directory.CreateSymbolicLink(destinationFolder, sourceFolder);
                    if (symbol.Exists)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    // 创建符号失败了，失败了就尝试拷贝一下吧
                }

                var result = PackageDirectory.Copy(sourceFolder, destinationFolder, true);
                return result;
            }
            catch (Exception e)
            {
                throw new PackagingException($"从 '{sourceFolder}' 复制到 '{destinationFolder}' 失败", e);
            }
        }
    }
}