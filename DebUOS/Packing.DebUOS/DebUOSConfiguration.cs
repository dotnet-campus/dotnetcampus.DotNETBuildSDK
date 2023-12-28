using dotnetCampus.Configurations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packing.DebUOS;

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
        get => GetString();
    }

    public string? Version
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? DebControlSection
    {
        set => SetValue(value);
        get => GetString();
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

    public string? DebControlHomepage
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? DebControlDescription
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? AppName
    {
        set => SetValue(value);
        get => GetString();
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

    public string? DesktopCategories
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? DesktopKeywords
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? DesktopKeywordsZhCN
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? DesktopComment
    {
        set => SetValue(value);
        get => GetString();
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

    public string? PackingFolder
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? WorkingFolder
    {
        set => SetValue(value);
        get => GetString();
    }

    public string? ProjectPublishFolder
    {
        set => SetValue(value);
        get => GetString();
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
