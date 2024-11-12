// ReSharper disable InconsistentNaming

using System;
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
    /// <example>Assets\control</example>
    public string? DebControlFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 自定义的 DEBIAN\postinst 文件路径，将直接使用该文件作为 postinst 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求
    /// <para></para>
    /// postinst：软件安装时执行的脚本
    /// <para></para>
    /// 按照 UOS 的规范，除对本程序根目录文件进行必要的操作修改外，禁止使用deb的postinst等钩子对系统文件进行修改，包含这些脚本的软件包都无法上架
    /// </summary>
    /// <example>Assets\PostInstall.sh</example>
    public string? DebPostinstFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 自定义的 DEBIAN\prerm 文件路径，将直接使用该文件作为 prerm 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求
    /// <para></para>
    /// prerm：软件卸载前执行的脚本
    /// <para></para>
    /// 按照 UOS 的规范，除对本程序根目录文件进行必要的操作修改外，禁止使用deb的postinst等钩子对系统文件进行修改，包含这些脚本的软件包都无法上架
    /// </summary>
    /// <example>Assets\PreRm.sh</example>
    public string? DebPrermFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 自定义的 DEBIAN\postrm 文件路径，将直接使用该文件作为 postrm 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求
    /// <para></para>
    /// postrm：软件卸载后执行的脚本
    /// <para></para>
    /// 按照 UOS 的规范，除对本程序根目录文件进行必要的操作修改外，禁止使用deb的postinst等钩子对系统文件进行修改，包含这些脚本的软件包都无法上架
    /// </summary>
    /// <example>Assets\PostRm.sh</example>
    public string? DebPostrmFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 自定义的 DEBIAN\preinst 文件路径，将直接使用该文件作为 preinst 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求
    /// </summary>
    /// <example>Assets\preinst.sh</example>
    public string? DebPreinstFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 自定义的 opt\apps\${AppId}\info 文件路径，将直接使用该文件作为 info 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求
    /// </summary>
    /// <example>Assets\Info.json</example>
    public string? DebInfoFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 自定义的 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件路径，将直接使用该文件作为 desktop 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求
    /// </summary>
    /// <example>Assets\Demo.desktop</example>
    public string? DebDesktopFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 应用的 AppId 值，用来组织应用的安装路径，同时也是应用的唯一标识。按照 UOS 的规范，请务必使用厂商的倒置域名+产品名作为应用包名，如 `com.example.demo` 格式，前半部分为厂商域名倒置，后半部分为产品名，如果使用非拥有者的域名作为前缀，可能会引起该域名拥有者进行申诉，导致软件被申诉下架或者删除，只允许小写字母。不写默认和 AssemblyName 属性相同
    /// </summary>
    /// <example>com.example.demo</example>
    public string? AppId
    {
        set => SetValue(value);
        get => GetString() ?? AssemblyName?.ToLowerInvariant();
    }

    /// <summary>
    /// 应用的 AppId 值，用来组织应用的安装路径，同时也是应用的唯一标识。按照 UOS 的规范，请务必使用厂商的倒置域名+产品名作为应用包名，如 `com.example.demo` 格式，前半部分为厂商域名倒置，后半部分为产品名，如果使用非拥有者的域名作为前缀，可能会引起该域名拥有者进行申诉，导致软件被申诉下架或者删除，只允许小写字母。不写默认和 AppId 属性相同
    /// <para></para>
    /// 与 <see cref="AppId"/> 属性不同的是，该属性明确给制作 UOS 的包使用，不会和其他的逻辑的 AppId 混淆
    /// </summary>
    /// <example>com.example.demo</example>
    public string? UOSAppId
    {
        set => SetValue(value);
        get => GetString() ?? AppId;
    }

    /// <summary>
    /// 版本号，默认是 1.0.0 版本
    /// </summary>
    /// <example>1.2.3</example>
    public string Version
    {
        set => SetValue(value);
        get => GetString() ?? "1.0.0";
    }

    /// <summary>
    /// 专门给制作 UOS 的包使用的版本号，不写将使用 <see cref="Version"/> 属性的值。可使用 a.b.c 格式，也可以比较复杂的语义版本号格式，如 `1.2.3-2+b1` 等格式
    /// </summary>
    /// <example>1.2.3</example>
    public string UOSDebVersion
    {
        set => SetValue(value);
        get => GetString() ?? Version;
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Section 属性，可以选用 utils，admin, devel, doc, libs, net, 或者 unknown 等等，代表着该软件包在 Debian 仓库中将被归属到什么样的逻辑子分类中。默认是 utils 类型
    /// </summary>
    /// <example>utils</example>
    public string DebControlSection
    {
        set => SetValue(value);
        get => GetString() ?? "utils";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Priority 属性，可以选用 required, important, standard, optional, extra 等等，代表着该软件包在 Debian 仓库中的优先级，optional 优先级适用于与优先级为 required、important 或 standard 的软件包不冲突的新软件包。也可以做其它取值。若是不明了，请使用 optional。默认是 optional 类型
    /// </summary>
    /// <example>optional</example>
    public string DebControlPriority
    {
        set => SetValue(value);
        get => GetString() ?? "optional";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Architecture 属性，以及 opt\apps\${AppId}\info 文件的 arch 属性。可以选用 amd64, i386, arm64, armel, armhf, mips, mips64el, mipsel, ppc64el, s390x, 或者 all 等等，代表着该软件包在 Debian 仓库中的架构，amd64 代表着 64 位的 x86 架构，i386 代表着 32 位的 x86 架构，arm64 代表着 64 位的 ARM 架构，armel 代表着 32 位的 ARM 架构，armhf 代表着 32 位的 ARM 架构，mips 代表着 32 位的 MIPS 架构，mips64el 代表着 64 位的 MIPS 架构，mipsel 代表着 32 位的 MIPS 架构，ppc64el 代表着 64 位的 PowerPC 架构，s390x 代表着 64 位的 IBM S/390 架构，all 代表着所有架构。目前商店支持以下的 amd64, mips64el, arm64, sw_64, loongarch64 几种架构。默认将根据 <see cref="RuntimeIdentifier"/> 属性决定是 amd64 、arm64类型
    /// </summary>
    /// <example>amd64</example>
    public string Architecture
    {
        set => SetValue(value);
        get
        {
            var architecture = GetString();
            if (architecture == null)
            {
                if (RuntimeIdentifier == "linux-x64")
                {
                    architecture = "amd64";
                }
                else if (RuntimeIdentifier == "linux-arm64")
                {
                    architecture = "arm64";
                }
                else if (RuntimeIdentifier == "linux-loongarch64")
                {
                    architecture = "loongarch64";
                }
                //else if (RuntimeIdentifier == "linux-mips64el")
                //{
                //    // 这个似乎 dotnet 是没有支持的，且我也没设备，先不写了。有需要的话，开发者自己加上 Architecture 就可以了
                //    architecture = "mips64el";
                //}
                //else if (RuntimeIdentifier == "linux-sw_64")
                //{
                //    // 这个似乎 dotnet 是没有支持的，且我也没设备
                //    architecture = "sw_64";
                //}
            }
            return architecture ?? "amd64";
        }
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Multi-Arch 属性。默认是 foreign 类型
    /// </summary>
    /// <example>foreign</example>
    public string DebControlMultiArch
    {
        set => SetValue(value);
        get => GetString() ?? "foreign";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Build-Depends 属性。默认是 debhelper (>=9) 类型
    /// </summary>
    /// <example>debhelper (>=9)</example>
    public string DebControlBuildDepends
    {
        set => SetValue(value);
        get => GetString() ?? "debhelper (>=9)";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Standards-Version 属性。默认不写将会取 <see cref="UOSDebVersion"/> 属性的值，取不到则设置为 3.9.6 的值
    /// </summary>
    /// <example>3.9.6</example>
    public string DebControlStandardsVersion
    {
        set => SetValue(value);
        get
        {
            // 参考 https://www.debian.org/doc/debian-policy/ch-controlfields.html#s-f-standards-version
            // > 5.6.11. Standards-Version
            // > The most recent version of the standards (the policy manual and associated texts) with which the package complies. See Standards conformance.
            // > The version number has four components: major and minor version number and major and minor patch level. When the standards change in a way that requires every package to change the major number will be changed. Significant changes that will require work in many packages will be signaled by a change to the minor number. The major patch level will be changed for any change to the meaning of the standards, however small; the minor patch level will be changed when only cosmetic, typographical or other edits are made which neither change the meaning of the document nor affect the contents of packages.
            // 
            // > Thus only the first three components of the policy version are significant in the Standards-Version control field, and so either these three components or all four components may be specified. 5
            // 
            // > udebs and source packages that only produce udebs do not use Standards-Version.
            var result = GetString();

            if (result is not null)
            {
                return result;
            }

            // 尝试使用 UOSDebVersion 进行兼容，但是要求前提是这里的版本号是传统的非语义版本号
            if (System.Version.TryParse(UOSDebVersion, out var version))
            {
                string standardVersion;

                // 以下为兼容 UOSDebVersion 写的是 a.b.c.d 和 a.b.c 和 a.b 的写法
                var major = version.Major;
                var minor = version.Minor;
                var build = version.Build;
                var revision = version.Revision;

                major = Math.Max(0, major);
                minor = Math.Max(0, minor); // 如果没有版本号，则为 -1 的值，使用 Max 可确保不写默认为 0 而不是负数
                build = Math.Max(0, build);

                // 是否有最后一位，根据 Debian 维护者指导文档和 Debian 政策手册，要求使用 3-4 位的版本号
                if (revision >= 0)
                {
                    standardVersion = $"{major}.{minor}.{build}.{revision}";
                }
                else
                {
                    standardVersion = $"{major}.{minor}.{build}";
                }

                return standardVersion;
            }

            // 为什么默认返回 3.9.6 呢？因为是从 https://www.debian.org/doc/manuals/debmake-doc/ch09.en.html 里面抄的代码。且返回一个非 1.0.0 的版本号会比较方便大家想设置时，去找到是在哪里配置的
            return "3.9.6";
        }
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Maintainer 属性。如不填写，默认将会按照 Authors Author Company Publisher 的顺序，找到第一个不为空的值，作为 Maintainer 的值。如最终依然为空，可能导致打出来的安装包在用户端安装之后，不能在开始菜单中找到应用的图标
    /// </summary>
    /// <example>dotnet-campus</example>
    public string? DebControlMaintainer
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Homepage 属性。如不填写，将尝试使用 PackageProjectUrl 属性，如依然为空则采用默认值。默认是 https://www.uniontech.com 的值
    /// </summary>
    /// <example>https://github.com/dotnet-campus/dotnetcampus.DotNETBuildSDK</example>
    public string DebControlHomepage
    {
        set => SetValue(value);
        get => GetString() ?? "https://www.uniontech.com";
    }

    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Description 属性。如不填写，默认将使用 Description 属性的值
    /// </summary>
    /// <example>The file downloader.</example>
    public string? DebControlDescription
    {
        set => SetValue(value);
        get => GetString();
    }
    /// <summary>
    /// 配置放入到 DEBIAN\control 文件的 Depends 属性。如不填写，则忽略。用于配置软件依赖，比如填写入 vlc,libvlc-dev 即可在声明安装包依赖 vlc 组件
    /// </summary>
    /// <example>vlc</example>
    public string? DebControlDepends
    {
        set => SetValue(value);
        get => GetString();
    }
    
    /// <summary>
    /// 此字段若配置了，则会在 control 文件中写入 X-Package-System 属性，值为此字段的值
    /// </summary>
    public string? DebControlXPackageSystem
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 应用名，英文名。将作为 opt\apps\${AppId}\entries\applications\${AppId}.desktop 和 opt\apps\${AppId}\info 的 Name 属性的值，不写默认和 AssemblyName 属性相同
    /// </summary>
    /// <example>UnoFileDownloader</example>
    public string? AppName
    {
        set => SetValue(value);
        get => GetString() ?? AssemblyName;
    }

    /// <summary>
    /// 应用名，中文名，可不写。将在开始菜单中显示
    /// </summary>
    /// <example>下载器</example>
    public string? AppNameZhCN
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\info 文件的 permissions 属性，可不写，可开启的属性之间使用分号 ; 分割。可选值有：autostart, notification, trayicon, clipboard, account, bluetooth, camera, audio_record, installed_apps 等。默认为不开启任何权限
    /// </summary>
    /// <example>autostart;notification;trayicon;clipboard;account</example>
    public string? InfoPermissions
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Categories 属性，可选值有：AudioVideo, Audio, Video, Development, Education, Game, Graphics, Network, Office, Science, Settings, System, Utility, Other 等。默认为 Other 的值
    /// </summary>
    /// <example>Other</example>
    public string DesktopCategories
    {
        set => SetValue(value);
        get => GetString() ?? "Other";
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Keywords 属性，作为程序的通用关键搜索词，当在启动器中搜索该词而非程序名称时，即可索引出该程序的快捷方式。多个关键词之间使用分号 ; 分割，关键词使用英文。如需添加中文关键词，请设置 <see cref="DesktopKeywordsZhCN"/> 属性。默认为 deepin 的值
    /// </summary>
    /// <example>deepin;downloader</example>
    public string DesktopKeywords
    {
        set => SetValue(value);
        get => GetString() ?? "deepin";
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Keywords[zh_CN] 属性，可不填，作为程序的通用关键搜索词，当在启动器中搜索该词而非程序名称时，即可索引出该程序的快捷方式。多个关键词之间使用分号 ; 分割，关键词使用中文
    /// </summary>
    /// <example>工具;下载器</example>
    public string? DesktopKeywordsZhCN
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Comment 属性，作为关于本程序的通用简述，在没有单独设置语言参数的情况下，默认显示该段内容。不填将使用 <see cref="UOSAppId"/> 属性的值
    /// </summary>
    /// <example>The file downloader.</example>
    public string DesktopComment
    {
        set => SetValue(value);
        get => GetString() ?? UOSAppId;
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Comment[zh_CN] 属性，作为关于本程序的通用中文简述，可不填
    /// </summary>
    /// <example>这是一个下载器</example>
    public string? DesktopCommentZhCN
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 NoDisplay 属性，如果设置为 true 则表示当前的应用不放在开始菜单里面，即在开始菜单隐藏应用。一般用于一些不想让用户直接碰到的，直接运行的应用。可不填，默认是 false 的值
    /// </summary>
    /// <example>false</example>
    public bool? DesktopNoDisplay
    {
        set => SetValue(value);
        get => GetBoolean();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Exec 属性，作为程序的启动命令，可不填，且推荐不填，除非有特殊需求。默认为 /opt/apps/${AppId}/files/bin/${AssemblyName} 的值
    /// </summary>
    /// <example>/opt/apps/$(AppId)/files/bin/$(AssemblyName)</example>
    public string? DesktopExec
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Icon 属性，作为程序的图标，可不填，且推荐不填，除非有特殊需求。默认为 <see cref="UOSAppId"/> 的值
    /// </summary>
    /// <example>$(UOSAppId)</example>
    public string? DesktopIcon
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Type 属性，作为程序的类型，按照 UOS 的规范，必须为 Application 的值，推荐不更改，即不填
    /// </summary>
    /// <example>Application</example>
    public string DesktopType
    {
        set => SetValue(value);
        get => GetString() ?? "Application";
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Terminal 属性，用来决定程序是否以终端的形式运行，默认是 false 关闭状态
    /// </summary>
    /// <example>false</example>
    public bool DesktopTerminal
    {
        set => SetValue(value);
        get => GetBoolean() ?? false;
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 StartupNotify 属性，用来决定程序是否允许桌面环境跟踪应用程序的启动，提供用户反馈和其他功能。例如鼠标的等待动画等，按照 UOS 规范建议，为保障应用使用体验，默认是 true 开启状态，推荐不更改，即不填
    /// </summary>
    /// <example>true</example>
    public bool DesktopStartupNotify
    {
        set => SetValue(value);
        get => GetBoolean() ?? true;
    }

    /// <summary>
    /// 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 MimeType 属性，用来配置程序支持的关联文件类型，根据实际需求来填写。如果没有需要支持关联文件，则不填。多个文件类型之间使用分号 ; 分割
    /// </summary>
    /// <example>audio/aac;application/aac;</example>
    public string? DesktopMimeType
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 配置是否将 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件 Copy 到 /usr/share/applications 文件夹里面。默认是 true 开启状态
    /// Copy 到 /usr/share/applications 文件夹里面，是为了让应用在麒麟开始菜单里面显示，如果不需要在开始菜单里面显示，可以关闭此选项
    /// </summary>
    /// <remarks>这是用于在麒麟系统里的，在统信 UOS 不需要这么做，但看起来在统信 UOS 做了也没问题</remarks>
    /// <example>true</example>
    public bool CopyDesktopFileToUsrShareApplications
    {
        set => SetValue(value);
        get => GetBoolean() ?? true;
    }

    /// <summary>
    /// 配置是否将 opt\apps\${AppId}\entries\icons 文件夹 Copy 到 /usr/share/icons/hicolor 文件夹里面。默认是 true 开启状态
    /// Copy 到 /usr/share/icons/hicolor 文件夹里面，是为了让应用在开始菜单\桌面等地方能够正常显示图标，如果不需要显示图标，可以关闭此选项
    /// </summary>
    /// <remarks>这是用于在麒麟系统里的，在统信 UOS 不需要这么做，但看起来在统信 UOS 做了也没问题</remarks>
    /// <example>true</example>
    public bool CopyIconsToUsrShareIcons
    {
        set => SetValue(value);
        get => GetBoolean() ?? true;
    }

    /// <summary>
    /// 进行打包的文件夹，用来组织打包的文件。可不填，且推荐不填，将被打包工具自动填充
    /// </summary>
    /// <example>obj\DebUOSPacking\Packing\</example>
    public string? PackingFolder
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 工作文件夹，用来存放打包过程中的临时文件。可不填，且推荐不填，将被打包工具自动填充
    /// </summary>
    /// <example>obj\DebUOSPacking\</example>
    public string? WorkingFolder
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 项目的发布输出文件夹。可不填，且推荐不填，将被打包工具自动填充
    /// </summary>
    /// <example>$([MSBuild]::NormalizePath($(MSBuildProjectDirectory), $(PublishDir)))</example>
    public string? ProjectPublishFolder
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 打包输出文件路径。可不填，默认将放在发布文件夹里
    /// </summary>
    /// <example>bin\Foo.deb</example>
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

    /// <summary>
    /// 表示当前的构建平台信息，一般从 dotnet publish 命令的 -r 参数里读取。无需手动配置
    /// </summary>
    public string? RuntimeIdentifier
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 表示图标文件夹路径，文件夹里面按照 UOS 的 deb 规范组织图标文件，文件夹里面存放的内容将会被原原本本拷贝到 opt/apps/${AppId}/entries/icons/ 文件夹里面。此属性属于高级配置，一般不需要使用，可以用来满足更多的定制化需求。默认不填，且推荐在充分理解 UOS 的 deb 规范的情况下再进行使用。此属性存在值时，将会忽略 <see cref="SvgIconFile"/> 和 <see cref="Png16x16IconFile"/> 等属性的设置
    /// </summary>
    /// <remarks>
    /// 如果此属性配置不正确或图标文件夹的组织不正确，将会导致安装完成之后，无法从开始菜单中找到应用的图标
    /// </remarks>
    /// <example>Assets\Icons\</example>
    public string? UOSDebIconFolder
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 应用图标文件，表示矢量图标文件。将被放入到 opt/apps/${AppId}/entries/icons/hicolor/scalable/apps/${appid}.svg 里面。矢量图标文件与 png 非矢量格式二选一，如果同时存在，优先使用矢量图标文件。
    /// 当 <see cref="UOSDebIconFolder"/> 属性存在值时，本属性设置无效
    /// </summary>
    /// <example>Assets\Icons\Logo.svg</example>
    public string? SvgIconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <summary>
    /// 应用图标文件，表示 png 非矢量格式文件。将被放入到 opt/apps/${AppId}/entries/icons/hicolor/${分辨率}/apps/${appid}.png 里面。请确保实际图片分辨率正确，且是 png 格式。矢量图标文件与 png 非矢量格式二选一，如果同时存在，优先使用矢量图标文件。
    /// 当 <see cref="UOSDebIconFolder"/> 属性存在值时，本属性设置无效
    /// </summary>
    /// <example>Assets\Icons\Logo16x16.png</example>
    public string? Png16x16IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    /// <example>Assets\Icons\Logo24x24.png</example>
    public string? Png24x24IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    /// <example>Assets\Icons\Logo32x32.png</example>
    public string? Png32x32IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    /// <example>Assets\Icons\Logo48x48.png</example>
    public string? Png48x48IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    /// <example>Assets\Icons\Logo64x64.png</example>
    public string? Png64x64IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    /// <example>Assets\Icons\Logo128x128.png</example>
    public string? Png128x128IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    /// <example>Assets\Icons\Logo256x256.png</example>
    public string? Png256x256IconFile
    {
        set => SetValue(value);
        get => GetString();
    }

    /// <inheritdoc cref="Png16x16IconFile"/>
    /// <example>Assets\Icons\Logo512x512.png</example>
    public string? Png512x512IconFile
    {
        set => SetValue(value);
        get => GetString();
    }


    #region 打包控制相关

    /// <summary>
    /// 打包时应该有哪些后缀被排除，默认包括 .pdb .dbg .md 文件
    /// 如果有其他特殊规则，请自行编写 Target 在 CreateDebUOS 之前删除掉
    /// </summary>
    /// <example>.pdb;.dbg;.md</example>
    public string? ExcludePackingDebFileExtensions
    {
        set => SetValue(value);
        get => GetString() ?? ".pdb;.dbg;.md";
    }

    /// <summary>
    /// 打包时是否将 bin 文件夹替换为应用版本号，默认不替换。如设置为 True 将使用 <see cref="UOSDebVersion"/> 属性的版本号作为文件夹名替换 bin 文件夹名
    /// </summary>
    /// <remarks>有些应用期望里层带版本号，即打出 `/opt/apps/${AppId}/files/${UOSDebVersion}/${AssemblyName}`格式的路径，如 `/opt/apps/com.dotnetcampus.app/files/1.0.0/app` 的路径，可以方便用来做软件更新。默认为 false 打出来的是 /opt/apps/${AppId}/files/bin/${AssemblyName} 格式的路径</remarks>
    /// <example>False</example>
    public bool UsingAppVersionInsteadOfBinOnDebPacking
    {
        set => SetValue(value);
        get => GetBoolean() ?? false;
    }

    #endregion
}