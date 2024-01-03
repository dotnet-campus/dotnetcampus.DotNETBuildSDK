using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Cli;

namespace Packing.DebUOS.Tool;

/// <summary>
/// 命令行参数
/// </summary>
public class Options
{
    /// <summary>
    /// 将给定路径文件夹打包为 UOS 的 deb 包
    /// </summary>
    /// 和 <see cref="PackageArgumentFilePath"/> 二选一，如果同时存在，优先使用 <see cref="BuildPath"/> 参数
    [Option('b', "Build", Description = "Build path")]
    public string? BuildPath { set; get; }

    /// <summary>
    /// 将根据给定的打包参数文件打包为 UOS 的 deb 包
    /// </summary>
    [Option('p', "Pack", Description = "Package argument file path")]
    public string? PackageArgumentFilePath { set; get; }

    [Option('o', "Output", Description = "Output path")]
    public string? OutputPath { set; get; }
}
