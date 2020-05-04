# DotNETBuild

提供dotnet打包方法

![](https://github.com/dotnet-campus/dotnetcampus.DotNETBuildSDK/workflows/.NET%20Core/badge.svg)

每个工具之间没有任何依赖，每个工具都同时输出到控制台和写入到 build.coin 配置文件

## 使用方法

每次构建推荐先调用 `dotnet buildkit init` 命令，此命令将会做初始化以及必备命令安装，读取机器配置和项目配置写入到构建时配置

后续根据需要调用对应的命令执行构建命令

## 为什么要创建这个项目

原本团队内部有一个自动构建平台，但是这个平台设计的时候，多个不同的任务之间是通过内存的属性关联的。换句话说是多个不同的任务是不能拿出来单独执行的，必须放在自动构建这个应用程序里面才能跑起来

这带来的问题就是每次有新需求扩展都需要改这个自动构建平台项目

此项目让多个不同的任务放在不同的进程里面，调用的是不同的应用，多个应用之间通过构建时配置文件 `build.coin` 进行关联，此时方便小伙伴定制和添加自己的构建命令

## 原则

本项目仅提供一系列的工具，如果某个工具不如何你的需求，或者你的需求没有工具实现。那么请你自行实现，如果说自己实现要做的东西太多了，没关系，边边角角的内容就用我的工具，核心的功能你自己写去。如果写的通用，欢迎发布

本项目原则上减少自己造轮子，尽可能复用现有的工具，通过命令行包装一层的方法，可以快速将大量的工具集成

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