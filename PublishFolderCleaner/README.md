# PublishFolderCleaner

让 .NET Core 应用发布文件夹更简洁的工具

背景： 默认在 .NET Core 或 .NET 5 以上版本，发布文件夹将包含所有的引用输出，有很多文件夹。不利于开发者或用户找到入口可执行文件

功能： 本工具提供 NuGet 包，可以在安装完成之后，发布应用将让发布文件夹下只包含一个 exe 和一个 lib 文件夹。此 exe 即是入口可执行文件，而 lib 文件夹就是原本的发布文件夹下的其他的文件

使用方法：

在入口程序集上安装如下 NuGet 包

```xml
  <ItemGroup>
    <PackageReference Include="dotnetCampus.PublishFolderCleaner" Version="1.0.12" />
  </ItemGroup>
```