# DotNETBuild

提供dotnet打包方法

![](https://github.com/dotnet-campus/dotnetcampus.DotNETBuildSDK/workflows/.NET%20Core/badge.svg)

每个工具之间没有任何依赖，每个工具都同时输出到控制台和写入到 build.coin 配置文件

每次构建推荐先调用 `dotnet buildkit init` 命令，此命令将会做初始化以及必备命令安装，读取机器配置和项目配置写入到构建时配置

## 配置文件等级

放在 `AppData\Roaming\dotnet campus\BuildKit\configuration.coin` 的文件为机器级配置

放在 `项目\build\build.coin` 是项目配置

默认在 `dotnet buildkit init` 命令，将会构建 `项目\build.coin` 构建时配置文件，此配置将包含机器配置和项目配置，其中项目配置内容相同项将会覆盖机器配置