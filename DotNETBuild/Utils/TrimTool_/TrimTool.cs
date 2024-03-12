#nullable enable
using dotnetCampus.Configurations;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning; // dotnet 6 使用
using dotnetCampus.DotNETBuild.Context;
using Walterlv.IO.PackageManagement;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 裁剪文件夹工具，可以辅助删除 Ref 文件夹，辅助删除 Runtimes 文件夹里面不需要的平台
    /// </summary>
    public class TrimTool : DotNetBuildTool
    {
        /// <summary>
        /// 创建裁剪文件夹工具
        /// </summary>
        /// <param name="appConfigurator"></param>
        /// <param name="logger"></param>
        public TrimTool(IAppConfigurator appConfigurator, ILogger? logger = null) : base(appConfigurator, logger)
        {
        }

        /// <summary>
        /// 删除 <see cref="CompileConfiguration.OutputDirectory"/> 里的 Ref 文件夹，此文件夹里面放的只是引用程序集
        /// </summary>
        public void TrimRefFolder()
        {
            var outputDirectory = AppConfigurator.Of<CompileConfiguration>().OutputDirectory;
            TrimRefFolder(outputDirectory);
        }

        /// <summary>
        /// 删除 <paramref name="folder"/> 里的 Ref 文件夹，此文件夹里面放的只是引用程序集
        /// </summary>
        public void TrimRefFolder(string folder)
        {
            var refFolder = Path.Combine(folder, "ref");
            if (!Directory.Exists(refFolder))
            {
                return;
            }

            Logger.LogInformation($"[TrimRefFolder] 删除 {refFolder}");
            PackageDirectory.Delete(refFolder);
        }

        /// <summary>
        /// 清理 Runtimes 文件夹里面，除了 <paramref name="platform"/> 平台之外的其他平台内容。例如在 Windows 平台上，可以删除 Linux 等平台的引用
        /// </summary>
        /// <param name="platform"></param>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
#endif
        public void TrimRuntimesFolder(Platform platform = Platform.X86, OSPlatform? osPlatform = null)
        {
            const string runtimesName = "runtimes";
            var outputDirectory = AppConfigurator.Of<CompileConfiguration>().OutputDirectory;
            var runtimesFolderPath = Path.Combine(outputDirectory, runtimesName);

            TrimRuntimesFolder(runtimesFolderPath, platform, Logger);
        }

        /// <summary>
        /// 清理 Runtimes 文件夹里面，除了 <paramref name="platform"/> 平台之外的其他平台内容。例如在 Windows 平台上，可以删除 Linux 等平台的引用
        /// </summary>
        /// <param name="runtimesFolderPath">传入要清理的 Runtimes 文件夹，例如 C:\Code\lindexi\bin\Debug\net6.0-windows\runtimes\</param>
        /// <param name="platform">指定当前的平台，除此平台之外，其他的将会被删除</param>
        /// <param name="osPlatform">指定当前的系统平台，除此平台之外，其他的将会被删除。现在只支持 Windows 平台</param>
        /// <param name="logger"></param>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
#endif
        public void TrimRuntimesFolder(string runtimesFolderPath, Platform platform = Platform.X86, ILogger? logger = null, OSPlatform? osPlatform = null)
        {
            logger ??= Logger;

            if (osPlatform is not null && osPlatform != OSPlatform.Windows)
            {
                throw new NotSupportedException("当前仅支持 Windows 平台");
            }

            if (!Directory.Exists(runtimesFolderPath))
            {
                return;
            }

            var winFolderName = platform switch
            {
                Platform.X86 => "win-x86",
                Platform.X64 => "win-x64",
                _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, null)
            };

            foreach (var directory in Directory.GetDirectories(runtimesFolderPath))
            {
                var directoryInfo = new DirectoryInfo(directory);
                if (directoryInfo.Name == "win" || directoryInfo.Name == winFolderName)
                {
                    // 这几个不删除
                }
                else
                {
                    logger.LogInformation($"[TrimRuntimesFolder] DeleteFolder: {directoryInfo.FullName}");
                    directoryInfo.Delete(true);
                }
            }
        }
    }
}
