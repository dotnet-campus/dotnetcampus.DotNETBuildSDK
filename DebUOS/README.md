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
