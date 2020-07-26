# DotNETBuild

提供dotnet打包方法

| Build | NuGet |
|--|--|
|![](https://github.com/dotnet-campus/dotnetcampus.DotNETBuildSDK/workflows/.NET%20Core/badge.svg) |[![](https://img.shields.io/nuget/v/dotnetCampus.DotNETBuildSDK.svg)](https://www.nuget.org/packages/dotnetCampus.DotNETBuildSDK)|


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

### DotNETToolList

需要安装的工具列表，多个工具之间使用分号分割

```
DotNETToolList
dotnetCampus.UpdateAllDotNetTools;dotnetCampus.UpdateAllDotNetTools
```

此配置项用来解决某些内部项目需要用到一些内部工具，这部分内部工具不适合开源，但是又期望在 dotnet buildkit init 的时候自动安装

### AppVersion

应用的版本，将会在 GetAssemblyVersionTask 读取时写入

## 更多工具

另一个比较完善的构建辅助项目 [nuke-build/nuke: The AKEless Build System for C#/.NET](https://github.com/nuke-build/nuke )

[dotnet/source-build: A repository to track efforts to produce a source tarball of the .NET Core SDK and all its components](https://github.com/dotnet/source-build )


## 工具列表

### dotnetCampus.BuildKitTool

这是主工具

```
dotnet tool install -g dotnetCampus.BuildKitTool
```

使用方法：

```
dotnet buildkit init [-c debug|release ]

-c --Configuration 可选 表示当前输出等级是 debug 或 release 等级，以及当前默认构建等级。默认是 Debug 等级

执行此命令将会初始化构建环境，包括寻找各个文件路径，以及安装更新必要工具
```

### PickTextValueTask

从一个文件用正则读取内容，将读取到的内容写入到另一个文件的正则匹配项

```
dotnet tool install -g dotnetCampus.PickTextValueTask
```

使用方法

```
PickTextValueTask -f 输入文件路径 -r 输入文件读取的正则 -o 输出文件路径 -s 输出文件替换的正则

  -f, --InputFile       Required.

  -r, --InputRegex      Required.

  -o, --OutputFile      Required.

  -s, --ReplaceRegex    Required.
```

此工具将会使用 InputRegex 去读取 InputFile 的内容，找到第一项匹配 `match.Groups[1].Value` 作为输入的值

然后通过 ReplaceRegex 去读取 OutputFile 的内容，将第一项匹配替换为输入的值，然后再写回 OutputFile 文件

```csharp
            var inputFilePath = "input file.txt";
            var outputFilePath = "output file.txt";

            var inputText = "<AppVersion>1.2.5-adsfasd</AppVersion>";
            var outputText = "<git-hash>githash</git-hash>";

            var inputRegex = @"\d+\.\d+\.\d+-([\w\d]+)";
            var replaceRegex = @"(githash)";

            File.WriteAllText(inputFilePath, inputText);
            File.WriteAllText(outputFilePath, outputText);

            PickTextValueTask.Program.Main(new[]
            {
                "-f",
                inputFilePath,
                "-r",
                inputRegex,
                "-o",
                outputFilePath,
                "-s",
                replaceRegex
            });

            var replacedText = File.ReadAllText(outputFilePath);
            Assert.AreEqual("<git-hash>adsfasd</git-hash>", replacedText);
```

### MatrixRun

矩阵命令执行工具

![image](https://user-images.githubusercontent.com/9959623/87214539-12541b00-c360-11ea-9521-8c310516bec6.png)

```
MatrixRun -m:S1=[A,B,C];S2=[1,2];S3=[x,y,z] -c "echo echo Matrix.S1=%Matrix.S1%, Matrix.S2=%Matrix.S2%, Matrix.S3=%Matrix.S3%"
```

通过 `-m` 传入矩阵，可以通过在批处理中使用 `Matrix.Xx` 环境变量获取运行时的值

使用 `-c` 传入执行命令，支持批处理文件

矩阵格式是 `属性名=可选参数` 需要将可选参数放在 `[]` 内，多个不同的参数使用 `,` 分割。多个不同的属性使用 `;` 分割。而 `-c` 后的命令，将会取决于矩阵的全排列次数被调用相应的次数