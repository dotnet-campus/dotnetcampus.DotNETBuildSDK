// ReSharper disable InconsistentNaming

using System.IO;
using dotnetCampus.Configurations;

namespace Packaging.DebUOS.Contexts.Configurations;

public class DebUOSConfiguration : Configuration
{
    public DebUOSConfiguration() : base("")
    {
    }

    public string? AssemblyName
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? DebControlFile
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? DebInfoFile
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? DebDesktopFile
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? AppId
    {
        set => SetValue(value);
        get => GetString()??AssemblyName;
    }

    public string? UOSAppId
    {
        set => SetValue(value);
        get => GetString() ?? AppId;
    }

    public string Version
    {
        set => SetValue(value);
        get => GetString() ?? "1.0.0";
    }

    public string UOSDebVersion
    {
        set => SetValue(value);
        get => GetString() ?? Version;
    }

    public string DebControlSection
    {
        set => SetValue(value);
        get => GetString() ?? "utils";
    }

    public string DebControlPriority
    {
        set => SetValue(value);
        get => GetString() ?? "optional";
    }

    public string Architecture
    {
        set => SetValue(value);
        get => GetString() ?? "amd64";
    }

    public string DebControlMultiArch
    {
        set => SetValue(value);
        get => GetString() ?? "foreign";
    }

    public string DebControlBuildDepends
    {
        set => SetValue(value);
        get => GetString() ?? "debhelper (>=9)";
    }

    public string DebControlStandardsVersion
    {
        set => SetValue(value);
        get => GetString() ?? "3.9.6";
    }

    public string? DebControlMaintainer
    {
        set => SetValue(value);
        get => GetString();
    }

    public string DebControlHomepage
    {
        set => SetValue(value);
        get => GetString() ?? "https://www.uniontech.com";
    }

    public string? DebControlDescription
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? AppName
    {
        set => SetValue(value);
        get => GetString() ?? AssemblyName;
    }

    public string? InfoPermissions
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? AppNameZhCN
    {
        set => SetValue(value);
        get => GetString();
    }

    public string DesktopCategories
    {
        set => SetValue(value);
        get => GetString() ?? "Other";
    }

    public string DesktopKeywords
    {
        set => SetValue(value);
        get => GetString()?? "deepin";
    }

    public string? DesktopKeywordsZhCN
    {
        set => SetValue(value);
        get => GetString();
    }

    public string DesktopComment
    {
        set => SetValue(value);
        get => GetString() ?? UOSAppId;
    }

    public string? DesktopCommentZhCN
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? DesktopExec
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? DesktopIcon
    {
        set => SetValue(value);
        get => GetString();
    }

    public string DesktopType
    {
        set => SetValue(value);
        get => GetString() ?? "Application";
    }

    public bool DesktopTerminal
    {
        set => SetValue(value);
        get => GetBoolean() ?? false;
    }

    public bool DesktopStartupNotify
    {
        set => SetValue(value);
        get => GetBoolean() ?? true;
    }

    public string? DesktopMimeType
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 进行打包的文件夹，用来组织打包的文件
    /// </summary>
    public string? PackingFolder
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 工作文件夹，用来存放打包过程中的临时文件
    /// </summary>
    public string? WorkingFolder
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 项目的发布输出文件夹
    /// </summary>
    public string? ProjectPublishFolder
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 打包输出文件路径
    /// </summary>
    public string? DebUOSOutputFilePath
    {
        set => SetValue(value);
        get
        {
            var outputFilePath = GetString();
            if (outputFilePath is null)
            {
                if (!string.IsNullOrEmpty(ProjectPublishFolder))
                {
                    var name = UOSAppId;

                    if (string.IsNullOrEmpty(name))
                    {
                        name = Path.GetDirectoryName(ProjectPublishFolder);
                    }
                   
                    return Path.Join(ProjectPublishFolder, $"{name}.deb");
                }
            }

            return outputFilePath;
        }
    }

    public string? IconFolder
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? SvgIconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? Png16x16IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? Png24x24IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? Png32x32IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? Png48x48IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? Png128x128IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? Png256x256IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? Png512x512IconFile
    {
        set => SetValue(value);
        get => GetString();
    }
}
