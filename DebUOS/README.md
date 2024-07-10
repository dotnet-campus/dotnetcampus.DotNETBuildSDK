# 制作符合 UOS 规范的 deb 安装包的工具

## 使用方法

有以下两个使用方式，任选其一即可

### 命令行工具方式

使用以下命令进行更新或安装工具：

```
dotnet tool update -g Packaging.DebUOS.Tool
```

将已经准备好的符合 UOS 安装包文件组织规范的文件夹打包为 deb 安装包：

```
dotnet dpkg-debuos -b C:\lindexi\DebPacking -o C:\lindexi\UOS\Foo.deb
```

以上的 `C:\lindexi\DebPacking` 为已准备好的符合 UOS 安装包文件组织规范的文件夹，以上的 `C:\lindexi\UOS\Foo.deb` 为打包输出的文件。其中 `-o` 指定打包输出文件参数可以忽略，如忽略此参数，则将会在打包文件夹输出 deb 安装包

使用命令行工具比较适合创建构建更为复杂的 deb 安装包，可以有更强的定制化，适合对 UOS 安装包规范较熟悉的开发者使用。或者是作为 `dpkg-deb` 工具在 Windows 上的替代品

### 配合 csproj 的 NuGet 包方式

请通过 NuGet 管理器或采用如下代码编辑 csproj 文件安装 `Packaging.DebUOS` 库

```xml
  <ItemGroup>
    <PackageReference Include="Packaging.DebUOS" Version="1.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
```

接着添加一些必要的配置参数，比如用来配置 UOS 的 AppId 的 `UOSAppId` 属性，如以下代码示例。更多的属性配置请参阅下文

```xml
  <PropertyGroup>
    <UOSAppId>com.xx.xxx</UOSAppId>
  </PropertyGroup>
```

通过如下命令行发布时，即可打出符合 UOS 规范的 deb 包

```
dotnet publish -t:CreateDebUOS -c release -r linux-x64 --self-contained
```

以上命令行与传统的发布命令最大的不同在于添加了 `-t:CreateDebUOS` 参数，通过此参数即可触发名为 `CreateDebUOS` 的 Target 进行创建 deb 包

如期望自动在发布之后输出符合 UOS 规范的 deb 包，期望不添加 `-t:CreateDebUOS` 参数，则可以通过配置 `<AutoCreateDebUOSAfterPublish>true</AutoCreateDebUOSAfterPublish>` 属性到 csproj 从而实现在发布之后，自动执行打包，如以下代码

```xml
  <PropertyGroup>
    <AutoCreateDebUOSAfterPublish>true</AutoCreateDebUOSAfterPublish>
  </PropertyGroup>
```

通过 NuGet 包配置的方法，可以很方便进行接入，自带大量的默认配置，从零开始接入的成本低，且不需要有许多额外的知识。可以完全复用原有的构建工具链，可以配合其他工具实现一次打包创建多个平台的安装包，可以将各项配置写入到 csproj 里面方便客制化定制以及接入更多自动化参数和加入代码管理

更多可配置属性请参阅 DebUOSConfiguration.cs 文件

调试方法：

- 可通过命令行参数输出了解打包参数的 `DebUOSPackingArgs.coin` 路径，从而了解打包所输入的参数。可以使用将此参数文件重新输入进行调试
- 可通过命令行参数输出了解文件组织的 `DebUOSPacking\Packing` 文件夹路径，通过此文件夹即可了解打出的 deb 的文件夹内容，也可以切换到 Linux 服务器上使用更稳定的 `dpkg-deb` 对此文件夹进行打包

一个简单的 csproj 配置示例

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>

    <Version>1.0.9</Version>
    
    <!-- 必要的属性配置 -->
    <!-- 用来配置 AppId 值 -->
    <UOSAppId>com.xxx.foo</UOSAppId>

    <!-- 可选的属性配置 -->
    <!-- 打包输出路径 -->
    <DebUOSOutputFilePath>C:\lindexi\Code\foo.deb</DebUOSOutputFilePath>

    <!-- 用来控制 .desktop 文件的内容 -->
    <!-- 配置使用控制台启动 -->
    <DesktopTerminal>true</DesktopTerminal>

    <!-- 矢量图和非矢量图二选一 -->
    <Png128x128IconFile>icon.png</Png128x128IconFile>
    <Png32x32IconFile>icon32.png</Png32x32IconFile>

    <SvgIconFile>icon.svg</SvgIconFile>

    <!-- 描述内容，可以在安装包双击看到 -->
    <Description>Test demo 也可以写中文</Description>
    <!-- 可以显示到开始菜单 -->
    <AppNameZhCN>测试中文名</AppNameZhCN>
    <DesktopKeywords>foo;icon</DesktopKeywords>
    <DesktopKeywordsZhCN>中文测试;foo</DesktopKeywordsZhCN>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Packaging.DebUOS" Version="1.2.1-alpha23">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

</Project>
```

更全面的配置请参阅本文末尾的《全部配置项》部分

## 开发

### 各项目作用

- Packaging.Targets ： 来源于 [https://github.com/quamotion/dotnet-packaging](https://github.com/quamotion/dotnet-packaging) 项目，用来提供基础的打包功能
- Packaging.DebUOS ： 打出符合 UOS 规范的 deb 包的核心项目，包括组织文件夹以及使用 Packaging.Targets 创建 deb 包两个功能
- Packaging.DebUOS.Tool ： 对 Packaging.DebUOS 封装 dotnet tool 命令行工具
- Packaging.DebUOS.NuGet ： 对 Packaging.DebUOS.Tool 工具进行封装为配合 csproj 的 NuGet 包，且添加大量的和 csproj 属性对接的中间属性

## 参考文档

- [一步步教你在 Windows 上构建 dotnet 系应用的 UOS 软件安装包](https://blog.lindexi.com/post/%E4%B8%80%E6%AD%A5%E6%AD%A5%E6%95%99%E4%BD%A0%E5%9C%A8-Windows-%E4%B8%8A%E6%9E%84%E5%BB%BA-dotnet-%E7%B3%BB%E5%BA%94%E7%94%A8%E7%9A%84-UOS-%E8%BD%AF%E4%BB%B6%E5%AE%89%E8%A3%85%E5%8C%85.html )

- [应用打包规范 文档中心-统信UOS生态社区](https://doc.chinauos.com/content/M7kCi3QB_uwzIp6HyF5J )

## 感谢

- [https://github.com/quamotion/dotnet-packaging](https://github.com/quamotion/dotnet-packaging)

## 全部配置项

大部分配置都是可选项，以下仅仅作为示例参考使用

```xml
<!-- 自定义的 DEBIAN\control 文件路径，将直接使用该文件作为 control 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求 -->
<DebControlFile>Assets\control</DebControlFile>

<!-- 自定义的 DEBIAN\postinst 文件路径，将直接使用该文件作为 postinst 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求

 postinst：软件安装时执行的脚本

 按照 UOS 的规范，除对本程序根目录文件进行必要的操作修改外，禁止使用deb的postinst等钩子对系统文件进行修改，包含这些脚本 的软件包都无法上架 -->
<DebPostinstFile>Assets\PostInstall.sh</DebPostinstFile>

<!-- 自定义的 DEBIAN\prerm 文件路径，将直接使用该文件作为 prerm 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求

 prerm：软件卸载前执行的脚本

 按照 UOS 的规范，除对本程序根目录文件进行必要的操作修改外，禁止使用deb的postinst等钩子对系统文件进行修改，包含这些脚本 的软件包都无法上架 -->
<DebPrermFile>Assets\PreRm.sh</DebPrermFile>

<!-- 自定义的 DEBIAN\postrm 文件路径，将直接使用该文件作为 postrm 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求

 postrm：软件卸载后执行的脚本

 按照 UOS 的规范，除对本程序根目录文件进行必要的操作修改外，禁止使用deb的postinst等钩子对系统文件进行修改，包含这些脚本 的软件包都无法上架 -->
<DebPostrmFile>Assets\PostRm.sh</DebPostrmFile>

<!-- 自定义的 DEBIAN\preinst 文件路径，将直接使用该文件作为 preinst 文件，忽略其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求 -->
<DebPreinstFile>Assets\preinst.sh</DebPreinstFile>

<!-- 自定义的 opt\apps\${AppId}\info 文件路径，将直接使用该文件作为 info 文件，忽略其他配置。这是比较高级的配置，一般不 需要使用，可以用来满足更多的定制化需求 -->
<DebInfoFile>Assets\Info.json</DebInfoFile>

<!-- 自定义的 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件路径，将直接使用该文件作为 desktop 文件，忽略 其他配置。这是比较高级的配置，一般不需要使用，可以用来满足更多的定制化需求 -->
<DebDesktopFile>Assets\Demo.desktop</DebDesktopFile>

<!-- 应用的 AppId 值，用来组织应用的安装路径，同时也是应用的唯一标识。按照 UOS 的规范，请务必使用厂商的倒置域名+产品名作为应用包名，如 `com.example.demo` 格式，前半部分为厂商域名倒置，后半部分为产品名，如果使用非拥有者的域名作为前缀，可能会引起该域名拥有者进行申诉，导致软件被申诉下架或者删除，只允许小写字母。不写默认和 AssemblyName 属性相同 -->
<AppId>com.example.demo</AppId>

<!-- 应用的 AppId 值，用来组织应用的安装路径，同时也是应用的唯一标识。按照 UOS 的规范，请务必使用厂商的倒置域名+产品名作为应用包名，如 `com.example.demo` 格式，前半部分为厂商域名倒置，后半部分为产品名，如果使用非拥有者的域名作为前缀，可能会引起该域名拥有者进行申诉，导致软件被申诉下架或者删除，只允许小写字母。不写默认和 AppId 属性相同

 与 AppId 属性不同的是，该属性明确给制作 UOS 的包使用，不会和其他的逻辑的 AppId 混淆 -->
<UOSAppId>com.example.demo</UOSAppId>

<!-- 版本号，默认是 1.0.0 版本 -->
<Version>1.2.3</Version>

<!-- 专门给制作 UOS 的包使用的版本号，不写将使用 Version 属性的值。可使用 a.b.c 格式，也可以比较复杂的语义版本号格式，如 `1.2.3-2+b1` 等格式 -->
<UOSDebVersion>1.2.3</UOSDebVersion>

<!-- 配置放入到 DEBIAN\control 文件的 Section 属性，可以选用 utils，admin, devel, doc, libs, net, 或者 unknown 等等，代 表着该软件包在 Debian 仓库中将被归属到什么样的逻辑子分类中。默认是 utils 类型 -->
<DebControlSection>utils</DebControlSection>

<!-- 配置放入到 DEBIAN\control 文件的 Priority 属性，可以选用 required, important, standard, optional, extra 等等，代表 着该软件包在 Debian 仓库中的优先级，optional 优先级适用于与优先级为 required、important 或 standard 的软件包不冲突的新软件包。也可以做其它取值。若是不明了，请使用 optional。默认是 optional 类型 -->
<DebControlPriority>optional</DebControlPriority>

<!-- 配置放入到 DEBIAN\control 文件的 Architecture 属性，以及 opt\apps\${AppId}\info 文件的 arch 属性。可以选用 amd64, i386, arm64, armel, armhf, mips, mips64el, mipsel, ppc64el, s390x, 或者 all 等等，代表着该软件包在 Debian 仓库中的架构，amd64 代表着 64 位的 x86 架构，i386 代表着 32 位的 x86 架构，arm64 代表着 64 位的 ARM 架构，armel 代表着 32 位的 ARM 架构，armhf 代表着 32 位的 ARM 架构，mips 代表着 32 位的 MIPS 架构，mips64el 代表着 64 位的 MIPS 架构，mipsel 代表着 32 位的 MIPS 架构，ppc64el 代表着 64 位的 PowerPC 架构，s390x 代表着 64 位的 IBM S/390 架构，all 代表着所有架构。目前商店支持以下的 amd64, mips64el, arm64, sw_64, loongarch64 几种架构。默认将根据 RuntimeIdentifier 属性决定是 amd64 、arm64类型 -->
<Architecture>amd64</Architecture>

<!-- 配置放入到 DEBIAN\control 文件的 Multi-Arch 属性。默认是 foreign 类型 -->
<DebControlMultiArch>foreign</DebControlMultiArch>

<!-- 配置放入到 DEBIAN\control 文件的 Build-Depends 属性。默认是 debhelper (>=9) 类型 -->
<DebControlBuildDepends>debhelper (>=9)</DebControlBuildDepends>

<!-- 配置放入到 DEBIAN\control 文件的 Standards-Version 属性。默认是 3.9.6 的值 -->
<DebControlStandardsVersion>3.9.6</DebControlStandardsVersion>

<!-- 配置放入到 DEBIAN\control 文件的 Maintainer 属性。如不填写，默认将会按照 Authors Author Company Publisher 的顺序， 找到第一个不为空的值，作为 Maintainer 的值。如最终依然为空，可能导致打出来的安装包在用户端安装之后，不能在开始菜单中找到应用的图标 -->
<DebControlMaintainer>dotnet-campus</DebControlMaintainer>

<!-- 配置放入到 DEBIAN\control 文件的 Homepage 属性。如不填写，将尝试使用 PackageProjectUrl 属性，如依然为空则采用默认值。默认是 https://www.uniontech.com 的值 -->
<DebControlHomepage>https://github.com/dotnet-campus/dotnetcampus.DotNETBuildSDK</DebControlHomepage>

<!-- 配置放入到 DEBIAN\control 文件的 Description 属性。如不填写，默认将使用 Description 属性的值 -->
<DebControlDescription>The file downloader.</DebControlDescription>

<!-- 应用名，英文名。将作为 opt\apps\${AppId}\entries\applications\${AppId}.desktop 和 opt\apps\${AppId}\info 的 Name 属性的值，不写默认和 AssemblyName 属性相同 -->
<AppName>UnoFileDownloader</AppName>

<!-- 应用名，中文名，可不写。将在开始菜单中显示 -->
<AppNameZhCN>下载器</AppNameZhCN>

<!-- 配置放入到 opt\apps\${AppId}\info 文件的 permissions 属性，可不写，可开启的属性之间使用分号 ; 分割。可选值有：autostart, notification, trayicon, clipboard, account, bluetooth, camera, audio_record, installed_apps 等。默认为不开启任何权限 -->
<InfoPermissions>autostart;notification;trayicon;clipboard;account</InfoPermissions>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Categories 属性，可选值有：AudioVideo, Audio, Video, Development, Education, Game, Graphics, Network, Office, Science, Settings, System, Utility, Other 等。默认 为 Other 的值 -->
<DesktopCategories>Other</DesktopCategories>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Keywords 属性，作为程序的通用关键搜索词，当在启动器中搜索该词而非程序名称时，即可索引出该程序的快捷方式。多个关键词之间使用分号 ; 分割，关键词使用英文。如需添加 中文关键词，请设置 DesktopKeywordsZhCN 属性。默认为 deepin 的值 -->
<DesktopKeywords>deepin;downloader</DesktopKeywords>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Keywords[zh_CN] 属性，可不填，作为程序的 通用关键搜索词，当在启动器中搜索该词而非程序名称时，即可索引出该程序的快捷方式。多个关键词之间使用分号 ; 分割，关键词使 用中文 -->
<DesktopKeywordsZhCN>工具;下载器</DesktopKeywordsZhCN>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Comment 属性，作为关于本程序的通用简述， 在没有单独设置语言参数的情况下，默认显示该段内容。不填将使用 UOSAppId 属性的值 -->
<DesktopComment>The file downloader.</DesktopComment>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Comment[zh_CN] 属性，作为关于本程序的通用中文简述，可不填 -->
<DesktopCommentZhCN>这是一个下载器</DesktopCommentZhCN>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 NoDisplay 属性，如果设置为 true 则表示当 前的应用不放在开始菜单里面，即在开始菜单隐藏应用。一般用于一些不想让用户直接碰到的，直接运行的应用。可不填，默认是 false 的值 -->
<DesktopNoDisplay>false</DesktopNoDisplay>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Exec 属性，作为程序的启动命令，可不填，且推荐不填，除非有特殊需求。默认为 /opt/apps/${AppId}/files/bin/${AssemblyName} 的值 -->
<DesktopExec>/opt/apps/$(AppId)/files/bin/$(AssemblyName)</DesktopExec>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Icon 属性，作为程序的图标，可不填，且推荐不填，除非有特殊需求。默认为 UOSAppId 的值 -->
<DesktopIcon>$(UOSAppId)</DesktopIcon>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Type 属性，作为程序的类型，按照 UOS 的规 范，必须为 Application 的值，推荐不更改，即不填 -->
<DesktopType>Application</DesktopType>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 Terminal 属性，用来决定程序是否以终端的形式运行，默认是 false 关闭状态 -->
<DesktopTerminal>false</DesktopTerminal>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 StartupNotify 属性，用来决定程序是否允许 桌面环境跟踪应用程序的启动，提供用户反馈和其他功能。例如鼠标的等待动画等，按照 UOS 规范建议，为保障应用使用体验，默认是 true 开启状态，推荐不更改，即不填 -->
<DesktopStartupNotify>true</DesktopStartupNotify>

<!-- 配置放入到 opt\apps\${AppId}\entries\applications\${AppId}.desktop 文件的 MimeType 属性，用来配置程序支持的关联文件类型，根据实际需求来填写。如果没有需要支持关联文件，则不填。多个文件类型之间使用分号 ; 分割 -->
<DesktopMimeType>audio/aac;application/aac;</DesktopMimeType>

<!-- 进行打包的文件夹，用来组织打包的文件。可不填，且推荐不填，将被打包工具自动填充 -->
<PackingFolder>obj\DebUOSPacking\Packing\</PackingFolder>

<!-- 工作文件夹，用来存放打包过程中的临时文件。可不填，且推荐不填，将被打包工具自动填充 -->
<WorkingFolder>obj\DebUOSPacking\</WorkingFolder>

<!-- 项目的发布输出文件夹。可不填，且推荐不填，将被打包工具自动填充 -->
<ProjectPublishFolder>$([MSBuild]::NormalizePath($(MSBuildProjectDirectory), $(PublishDir)))</ProjectPublishFolder>

<!-- 打包输出文件路径。可不填，默认将放在发布文件夹里 -->
<DebUOSOutputFilePath>bin\Foo.deb</DebUOSOutputFilePath>

<!-- 表示图标文件夹路径，文件夹里面按照 UOS 的 deb 规范组织图标文件，文件夹里面存放的内容将会被原原本本拷贝到 opt/apps/${AppId}/entries/icons/ 文件夹里面。此属性属于高级配置，一般不需要使用，可以用来满足更多的定制化需求。默认不填，且推荐在 充分理解 UOS 的 deb 规范的情况下再进行使用。此属性存在值时，将会忽略 SvgIconFile 和 Png16x16IconFile 等属性的设置 -->
<UOSDebIconFolder>Assets\Icons\</UOSDebIconFolder>

<!-- 应用图标文件，表示矢量图标文件。将被放入到 opt/apps/${AppId}/entries/icons/hicolor/scalable/apps/${appid}.svg 里面 。矢量图标文件与 png 非矢量格式二选一，如果同时存在，优先使用矢量图标文件。
 当 UOSDebIconFolder 属性存在值时，本属性设置无效 -->
<SvgIconFile>Assets\Icons\Logo.svg</SvgIconFile>

<!-- 应用图标文件，表示 png 非矢量格式文件。将被放入到 opt/apps/${AppId}/entries/icons/hicolor/${分辨率}/apps/${appid}.png 里面。请确保实际图片分辨率正确，且是 png 格式。矢量图标文件与 png 非矢量格式二选一，如果同时存在，优先使用矢量图标文 件。
 当 UOSDebIconFolder 属性存在值时，本属性设置无效 -->
<Png16x16IconFile>Assets\Icons\Logo16x16.png</Png16x16IconFile>

<!-- 打包时应该有哪些后缀被排除，默认包括 .pdb .dbg .md 文件
 如果有其他特殊规则，请自行编写 Target 在 CreateDebUOS 之前删除掉 -->
<ExcludePackingDebFileExtensions>.pdb;.dbg;.md</ExcludePackingDebFileExtensions>
```

## FAQ

### 如何在 deb 包里面添加符号 pdb 文件

添加 ExcludePackingDebFileExtensions 属性配置，重新指定打包时应该有哪些后缀被排除。因为默认的 ExcludePackingDebFileExtensions 属性包含了 .pdb .dbg .md 文件，因此符号 pdb 文件将被排除

```xml
    <PropertyGroup>
      <ExcludePackingDebFileExtensions>.dbg;.md</ExcludePackingDebFileExtensions>
    </PropertyGroup>
```

### 如何添加更多文件加入 deb 打包里

一般情况下，能够输出到发布路径的，就能加入到 deb 包里面。比如在 csproj 配置某些文件如果较新则拷贝等

如果需要动态编写构建逻辑，则可在 Publish 之后，在 CreateDebUOS 之前，进行动态加入文件。如以下例子，添加的是构建信息 Version.txt 文件到打包的 deb 里面

```xml
  <Target Name="_BuildVersionInfoTarget" BeforeTargets="CreateDebUOS" DependsOnTargets="Publish">
    <PropertyGroup>
      <BuildVersionInfoFile>$([System.IO.Path]::Combine($(PublishDir), "Version.txt"))</BuildVersionInfoFile>
      <BuildTimeInfo>$([System.DateTimeOffset]::get_Now().ToString())</BuildTimeInfo>
    </PropertyGroup>
    <ItemGroup>
      <BuildVersionInfoWriteArgLine Include="&gt;" />
      <BuildVersionInfoWriteArgLine Include="GitCommit" />
      <BuildVersionInfoWriteArgLine Include="$(GitCommit)" />
      <BuildVersionInfoWriteArgLine Include="&gt;" />

      <BuildVersionInfoWriteArgLine Include="BuildTime" />
      <BuildVersionInfoWriteArgLine Include="$(BuildTimeInfo)" />
      <BuildVersionInfoWriteArgLine Include="&gt;" />
    </ItemGroup>

    <WriteLinesToFile File="$(BuildVersionInfoFile)" Lines="@(BuildVersionInfoWriteArgLine)" Overwrite="true" />
  </Target>

  <Target Name="_GitCommit" Returns="$(GitCommit)" BeforeTargets="_BuildVersionInfoTarget" Condition="'$(GitCommit)' == ''">
    <Exec Command="git rev-parse HEAD" EchoOff="true" StandardErrorImportance="low" StandardOutputImportance="low" ConsoleToMSBuild="true" ContinueOnError="true" StdOutEncoding="utf-8">
      <Output TaskParameter="ConsoleOutput" PropertyName="GitCommit" />
      <Output TaskParameter="ExitCode" PropertyName="MSBuildLastExitCode" />
    </Exec>
  </Target>
```

