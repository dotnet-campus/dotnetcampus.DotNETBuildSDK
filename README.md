# DotNETBuild

提供dotnet打包方法

![](https://github.com/dotnet-campus/dotnetcampus.DotNETBuildSDK/workflows/.NET%20Core/badge.svg)

每个工具之间没有任何依赖，每个工具都同时输出到控制台和写入到 build.coin 配置文件

每次构建推荐先调用 `dotnet buildkit init` 命令，此命令将会做初始化以及必备命令安装，读取机器配置和项目配置写入到构建时配置

## 配置文件等级

放在 `AppData\Roaming\dotnet campus\BuildKit\configuration.coin` 的文件为机器级配置

放在 `项目\build\build.coin` 是项目配置

默认在 `dotnet buildkit init` 命令，将会构建 `项目\build.coin` 构建时配置文件，此配置将包含机器配置和项目配置，其中项目配置内容相同项将会覆盖机器配置

## 配置列表

### Tool.DotNETToolList

需要安装的工具列表，多个工具之间使用分号分割

```
Tool.DotNETToolList
dotnetCampus.UpdateAllDotNetTools;dotnetCampus.UpdateAllDotNetTools
```

此配置项用来解决某些内部项目需要用到一些内部工具，这部分内部工具不适合开源，但是又期望在 dotnet buildkit init 的时候自动安装