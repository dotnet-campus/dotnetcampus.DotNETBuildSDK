using DotNetCampus.Cli.Compiler;

namespace Packaging.DebUOS.Tool;

/// <summary>
/// 命令行参数
/// </summary>
public class Options
{
    /// <summary>
    /// 将给定路径文件夹打包为 UOS 的 deb 包
    /// </summary>
    /// 和 <see cref="PackageArgumentFilePath"/> 二选一，如果同时存在，优先使用 <see cref="BuildPath"/> 参数
    [Option('b', "Build", Description = "Build path", LocalizableDescription = "将符合 UOS 安装包组织规范的文件夹打包为 deb 包，和 -p 参数二选一")]
    public string? BuildPath { set; get; }

    /// <summary>
    /// 将根据给定的打包参数文件打包为 UOS 的 deb 包
    /// </summary>
    [Option('p', "Pack", Description = "Package argument file path", LocalizableDescription = "使用给定的 coin 格式参数文件制作 deb 包")]
    public string? PackageArgumentFilePath { set; get; }

    [Option('o', "Output", Description = "Output path", LocalizableDescription = "输出的 deb 文件路径")]
    public string? OutputPath { set; get; }
}
