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

    /// <summary>
    /// 自定义的 DEBIAN\control 文件路径，将直接使用该文件作为 control 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求
    /// </summary>
    public string? DebControlFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 自定义的 opt\apps\${AppId}\info 文件路径，将直接使用该文件作为 info 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求
    /// </summary>
    public string? DebInfoFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 自定义的 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件路径，将直接使用该文件作为 desktop 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求
    /// </summary>
    public string? DebDesktopFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 应用的 AppId 值，用来组织应用的安装路径，同时也是应用的唯一标识。按照 UOS 的规范，请务必使用厂商的倒置域名+产品名作为应用包名，如 `com.example.demo` 格式，前半部分为厂商域名倒置，后半部分为产品名，如果使用非拥有者的域名作为前缀，可能会引起该域名拥有者进行申诉，导致软件被申诉下架或者删除，只允许小写字母。不写默认和 AssemblyName 属性相同
    /// </summary>
    public string? AppId
    {
        set => SetValue(value);
        get => GetString() ?? AssemblyName;
    }

    /// <summary>
    /// 应用的 AppId 值，用来组织应用的安装路径，同时也是应用的唯一标识。按照 UOS 的规范，请务必使用厂商的倒置域名+产品名作为应用包名，如 `com.example.demo` 格式，前半部分为厂商域名倒置，后半部分为产品名，如果使用非拥有者的域名作为前缀，可能会引起该域名拥有者进行申诉，导致软件被申诉下架或者删除，只允许小写字母。不写默认和 AppId 属性相同
    /// <para></para>
    /// 与 <see cref="AppId"/> 属性不同的是，该属性明确给制作 UOS 的包使用，不会和其他的逻辑的 AppId 混淆
    /// </summary>
    public string? UOSAppId
    {
        set => SetValue(value);
        get => GetString() ?? AppId;
    }

    /// <summary>
    /// 版本号，默认是 1.0.0 版本
    /// </summary>
    public string Version
    {
        set => SetValue(value);
        get => GetString() ?? "1.0.0";
    }

    /// <summary>
    /// 专门给制作 UOS 的包使用的版本号，不写将使用 <see cref="Version"/> 属性的值。可使用 a.b.c 格式，也可以比较复杂的语义版本号格式，如 `1.2.3-2+b1` 等格式
    /// </summary>
    public string UOSDebVersion
    {
        set => SetValue(value);
        get => GetString() ?? Version;
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Section 属性，可以选用 utils，admin, devel, doc, libs, net, 或者 unknown 等等，代表着该软件包在 Debian 仓库中将被归属到什么样的逻辑子分类中。默认是 utils 类型
    /// </summary>
    public string DebControlSection
    {
        set => SetValue(value);
        get => GetString() ?? "utils";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Priority 属性，可以选用 required, important, standard, optional, extra 等等，代表着该软件包在 Debian 仓库中的优先级，optional 优先级适用于与优先级为 required、important 或 standard 的软件包不冲突的新软件包。也可以做其它取值。若是不明了，请使用 optional。默认是 optional 类型
    /// </summary>
    public string DebControlPriority
    {
        set => SetValue(value);
        get => GetString() ?? "optional";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Architecture 属性，以及 opt\apps\${AppId}\info 文件的 arch 属性。可以选用 amd64, i386, arm64, armel, armhf, mips, mips64el, mipsel, ppc64el, s390x, 或者 all 等等，代表着该软件包在 Debian 仓库中的架构，amd64 代表着 64 位的 x86 架构，i386 代表着 32 位的 x86 架构，arm64 代表着 64 位的 ARM 架构，armel 代表着 32 位的 ARM 架构，armhf 代表着 32 位的 ARM 架构，mips 代表着 32 位的 MIPS 架构，mips64el 代表着 64 位的 MIPS 架构，mipsel 代表着 32 位的 MIPS 架构，ppc64el 代表着 64 位的 PowerPC 架构，s390x 代表着 64 位的 IBM S/390 架构，all 代表着所有架构。目前商店支持以下的 amd64, mips64el, arm64, sw_64, loongarch64 几种架构。默认是 amd64 类型
    /// </summary>
    public string Architecture
    {
        set => SetValue(value);
        get => GetString() ?? "amd64";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Multi-Arch 属性。默认是 foreign 类型
    /// </summary>
    public string DebControlMultiArch
    {
        set => SetValue(value);
        get => GetString() ?? "foreign";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Build-Depends 属性。默认是 debhelper (>=9) 类型
    /// </summary>
    public string DebControlBuildDepends
    {
        set => SetValue(value);
        get => GetString() ?? "debhelper (>=9)";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Standards-Version 属性。默认是 3.9.6 的值
    /// </summary>
    public string DebControlStandardsVersion
    {
        set => SetValue(value);
        get => GetString() ?? "3.9.6";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Maintainer 属性。如不填写，默认将会按照 Authors Author Company Publisher 的顺序，找到第一个不为空的值，作为 Maintainer 的值。如最终依然为空，可能导致打出来的安装包在用户端安装之后，不能在开始菜单中找到应用的图标
    /// </summary>
    public string? DebControlMaintainer
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Homepage 属性。如不填写，将尝试使用 PackageProjectUrl 属性，如依然为空则采用默认值。默认是 https://www.uniontech.com 的值
    /// </summary>
    public string DebControlHomepage
    {
        set => SetValue(value);
        get => GetString() ?? "https://www.uniontech.com";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Description 属性。如不填写，默认将使用 Description 属性的值
    /// </summary>
    public string? DebControlDescription
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 应用名，英文名。将作为 opt\apps\${AppId}\entries\applications\${AppId}.desktop 和 opt\apps\${AppId}\info 的 Name 属性的值，不写默认和 AssemblyName 属性相同
    /// </summary>
    public string? AppName
    {
        set => SetValue(value);
        get => GetString() ?? AssemblyName;
    }

    /// <summary>
    /// 应用名，中文名，可不写。将在开始菜单中显示
    /// </summary>
    public string? AppNameZhCN
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\info 文件的 permissions 属性，可不写，可开启的属性之间使用分号 ; 分割。可选值有：autostart, notification, trayicon, clipboard, account, bluetooth, camera, audio_record, installed_apps 等。默认为不开启任何权限
    /// </summary>
    public string? InfoPermissions
    {
        set => SetValue(value);
        get => GetString();
    }


    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Categories 属性，可选值有：AudioVideo, Audio, Video, Development, Education, Game, Graphics, Network, Office, Science, Settings, System, Utility, Other 等。默认为 Other 的值
    /// </summary>
    public string DesktopCategories
    {
        set => SetValue(value);
        get => GetString() ?? "Other";
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Keywords 属性，作为程序的通用关键搜索词，当在启动器中搜索该词而非程序名称时，即可索引出该程序的快捷方式。多个关键词之间使用分号 ; 分割，关键词使用英文。如需添加中文关键词，请设置 <see cref="DesktopKeywordsZhCN"/> 属性。默认为 deepin 的值
    /// </summary>
    public string DesktopKeywords
    {
        set => SetValue(value);
        get => GetString() ?? "deepin";
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Keywords[zh_CN] 属性，可不填，作为程序的通用关键搜索词，当在启动器中搜索该词而非程序名称时，即可索引出该程序的快捷方式。多个关键词之间使用分号 ; 分割，关键词使用中文
    /// </summary>
    public string? DesktopKeywordsZhCN
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Comment 属性，作为关于本程序的通用简述，在没有单独设置语言参数的情况下，默认显示该段内容。不填将使用 <see cref="UOSAppId"/> 属性的值
    /// </summary>
    public string DesktopComment
    {
        set => SetValue(value);
        get => GetString() ?? UOSAppId;
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Comment[zh_CN] 属性，作为关于本程序的通用中文简述，可不填
    /// </summary>
    public string? DesktopCommentZhCN
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Exec 属性，作为程序的启动命令，可不填，且推荐不填，除非有特殊需求。默认为 /opt/apps/${AppId}/files/bin/${AssemblyName} 的值
    /// </summary>
    public string? DesktopExec
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Icon 属性，作为程序的图标，可不填，且推荐不填，除非有特殊需求。默认为 <see cref="UOSAppId"/> 的值
    /// </summary>
    public string? DesktopIcon
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Type 属性，作为程序的类型，按照 UOS 的规范，必须为 Application 的值，推荐不更改，即不填
    /// </summary>
    public string DesktopType
    {
        set => SetValue(value);
        get => GetString() ?? "Application";
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Terminal 属性，用来决定程序是否以终端的形式运行，默认是 false 关闭状态
    /// </summary>
    public bool DesktopTerminal
    {
        set => SetValue(value);
        get => GetBoolean() ?? false;
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 StartupNotify 属性，用来决定程序是否允许桌面环境跟踪应用程序的启动，提供用户反馈和其他功能。例如鼠标的等待动画等，按照 UOS 规范建议，为保障应用使用体验，默认是 true 开启状态，推荐不更改，即不填
    /// </summary>
    public bool DesktopStartupNotify
    {
        set => SetValue(value);
        get => GetBoolean() ?? true;
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 MimeType 属性，用来配置程序支持的关联文件类型，根据实际需求来填写。如果没有需要支持关联文件，则不填。多个文件类型之间使用分号 ; 分割
    /// </summary>
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

    //public string? IconFolder
    //{
    //    set => SetValue(value);
    //    get => GetString();
    //}

    /// <summary>
    /// 应用图标文件，表示矢量图标文件。将被放入到 opt/apps/${AppId}/entries/icons/hicolor/scalable/apps/${appid}.svg 里面。矢量图标文件与 png 非矢量格式二选一，如果同时存在，优先使用矢量图标文件。
    /// 当 <see cref="IconFolder"/> 属性存在值时，本属性设置无效
    /// </summary>
    public string? SvgIconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 应用图标文件，表示 png 非矢量格式文件。将被放入到 opt/apps/${AppId}/entries/icons/hicolor/${分辨率}/apps/${appid}.png 里面。请确保实际图片分辨率正确，且是 png 格式。矢量图标文件与 png 非矢量格式二选一，如果同时存在，优先使用矢量图标文件。
    /// 当 <see cref="IconFolder"/> 属性存在值时，本属性设置无效
    /// </summary>
    public string? Png16x16IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    public string? Png24x24IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    public string? Png32x32IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    public string? Png48x48IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    public string? Png128x128IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    public string? Png256x256IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    public string? Png512x512IconFile
    {
        set => SetValue(value);
        get => GetString();
    }
}