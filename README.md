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

本项目仅提供一系列的工具，如果某个工具不符合你的需求，或者你的需求没有工具实现。那么请你自行实现，如果说自己实现要做的东西太多了，没关系，边边角角的内容就用我的工具，核心的功能你自己写去。如果写的通用，欢迎发布

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

```
dotnet tool install -g dotnetCampus.MatrixRun
```

![image](https://user-images.githubusercontent.com/9959623/87214539-12541b00-c360-11ea-9521-8c310516bec6.png)

```
MatrixRun -m:S1=[A,B,C];S2=[1,2];S3=[x,y,z] -c "echo echo Matrix.S1=%Matrix.S1%, Matrix.S2=%Matrix.S2%, Matrix.S3=%Matrix.S3%"
```

通过 `-m` 传入矩阵，可以通过在批处理中使用 `Matrix.Xx` 环境变量获取运行时的值

使用 `-c` 传入执行命令，支持批处理文件

矩阵格式是 `属性名=可选参数` 需要将可选参数放在 `[]` 内，多个不同的参数使用 `,` 分割。多个不同的属性使用 `;` 分割。而 `-c` 后的命令，将会取决于矩阵的全排列次数被调用相应的次数

### GitRevisionTask

获取当前 Git 的 commit 次数以及 hash 的值，用于创建版本号等

```
dotnet tool install -g dotnetCampus.GitRevisionTask
```

用法

```
GitRevisionTask
```

将会通过控制台输出当前的 Git 的 commit 次数，同时写入到配置文件的 `GitCount` 和 `CurrentCommit` 属性

```
> 当前的 commit 是这个分支的第几次
GitCount
623
> 当前的 commit 的 hash 的值
CurrentCommit
9a1be330450b10b51230a17390b5cf1a7f8af1d8
```

### SendEmailTask

发送邮件

```
dotnet tool install -g dotnetCampus.SendEmailTask
```

用法

```
SendEmail 参数
```

必选参数如下

- `-t` 或 `--To` 接收方的邮件地址使用 ; 分割多个不同的命令
- `-s` 或 `--Subject` 邮件标题
- `-b` 或 `--Body` 邮件内容，将 `\\r\\n` 替换为换行

可选参数如下

- `--SmtpServer` 邮件服务器地址，对应配置文件 `Email.SmtpServer` 内容
- `--SmtpServerPort` 邮件服务器端口，对应配置文件 `Email.SmtpServerPort` 内容
- `--UserName` 发送邮件的登录用户名，对应配置文件 `Email.UserName` 内容
- `--Password` 发送邮件的登录密码，对应配置文件 `Email.Password` 内容
- `--SenderDisplayName` 发送邮件显示的发送者名字，对应配置文件 `Email.SenderDisplayName` 内容，默认和 `Email.UserName` 相同

例子

```
SendEmail -t lindexi_gd@outlook.com --subject 测试 --SmtpServer smtp.yandex.com --SmtpServerPort 587 --UserName lindexi@yandex.com --Password miBN8dFLxdUs9d3
```

在 GitHub Action 中，可以将用户名密码等存放在凭据管理器里面，详细请看 [Creating and storing encrypted secrets - GitHub Docs](https://docs.github.com/en/actions/configuring-and-managing-workflows/creating-and-storing-encrypted-secrets )

另外在统一配置下，推荐统一邮箱账号密码存放到机器级配置里，更多关于机器级配置请看上文

### BuildMd5Task

将对输入的文件或文件夹创建 md5 校验文件，传入文件夹时，将会创建每个文件的校验到相同的输出文件

```
dotnet tool install -g dotnetCampus.BuildMd5Task
```

用法

```
BuildMd5 [参数]
```

如不添加参数，将对当前工作路径的文件夹创建校验文件，将校验文件存放在当前文件夹的 ChecksumMd5.txt 文件

可选参数

```
- `-p` 或 `--path` 需要用来创建校验的文件夹或文件，默认将会使用当前工作路径文件夹
- `-o` 或 `--output` 输出的校验文件，默认将会输出为当前工作路径的 ChecksumMd5.txt 文件
- `--search-pattern` 匹配文件夹里所有符合此通配符的文件名的文件，多个通配符使用 `|` 字符分割，如同时匹配 exe 和 dll 文件可以使用 `--search-pattern "*.dll|*.exe"` 这句命令。此属性仅在 Path 为文件夹时使用，默认不填将匹配所有文件
- `--ignore-list` 忽略文件列表，暂不支持通配符，需要使用相对路径，路径相对于 Path 路径。多个相对路径使用 `|` 字符分割，此属性仅在 Path 为文件夹时使用。如忽略 `file\lindexi.exe` 文件可以使用 `--ignore-list "file\lindexi.exe"` 这句命令
- `--overwrite [true|false]` 如果校验文件存在，那么将会被覆盖重写。默认值是 false 也就是说校验文件存在将会失败
```

### RunWithConfigValueTask

提供命令自动从配置获取替换符的任务方法，通过此命令行工具执行的命令，将会在执行命令时，将命令行传入的内容自动替换为配置文件里面的对应配置。同时支持在配置不存在时使用默认值的方式

安装方法如下

```
dotnet tool install -g dotnetCampus.RunWithConfigValueTask
```

使用方法如下，假定配置文件里面已存在配置是 `Foo=doubi` 而调用命令行方式如下

```
RunWithConfigValueTask dotnetcampus.exe f1 $(Foo) $(F2)??lindexi
```

那么实际执行命令行将会是如下

```
dotnetcampus.exe f1 doubi lindexi
```

在传入 RunWithConfigValueTask 的命令，使用格式为 `$(配置的Key值)` 或者 `$(配置的Key值)??默认值` 的方式，即可在 RunWithConfigValueTask 替换为配置中对应的参数

## 相似的项目

[dotnetcore/FlubuCore: A cross platform build and deployment automation system for building projects and executing deployment scripts using C# code.](https://github.com/dotnetcore/FlubuCore )