# CopyAfterCompileTool


使用方法

## 安装

先安装 dotnetCampus.CopyAfterCompileTool 工具，咱可以使用 update 命令代替 install 安装命令，使用 update 命令可以做到在没有安装的时候自动安装，在安装的时候自动更新到最新版本

```
dotnet tool update -g dotnetCampus.CopyAfterCompileTool
```

## 配置

接着在当前工作文件夹下存放 Build.coin 配置文件，配置文件里面包含的项如下：


```
> 代码文件夹
CodeDirectory
C:\lindexi\Code
> 应用输出文件夹
OutputDirectory
C:\lindexi\Code\Bin\release
> 保存构建完成的文件夹
TargetDirectory
C:\lindexi\App
>
OriginBranch
origin/master
>
```

## 运行

命令行输入下面命令即可执行

```
CopyAfterCompile
```