#nullable enable

using dotnetCampus.Configurations;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 调用 Crossgen2 的辅助工具
    /// </summary>
    public class Crossgen2Tool : DotNetBuildTool
    {
        /// <summary>
        /// 创建 Crossgen2 辅助工具
        /// </summary>
        /// <param name="appConfigurator"></param>
        /// <param name="logger"></param>
        public Crossgen2Tool(IAppConfigurator appConfigurator, ILogger? logger = null) : base(appConfigurator, logger)
        {
        }

        /// <summary>
        /// 构建 <paramref name="inputFileList"/> 的 ReadyToRun 版本
        /// </summary>
        /// <param name="platform">指定平台，使用 ReadyToRun 需要根据具体使用进程所使用的平台，决定如何优化。如果有某个 DLL 同时给多个平台的进程共用，那不推荐对此 DLL 进行 ReadyToRun 优化</param>
        /// <param name="inputFileList">将进行 ReadyToRun 的文件列表</param>
        /// <param name="searchDirectoryInfoCollection">输入的 <paramref name="inputFileList"/> 所引用的程序集搜索路径。因为在进行 ReadyToRun 优化时，需要找到 DLL 所引用的所有程序集才能优化对应的调用</param>
        /// <param name="workingFolder">进行 ReadyToRun 的工作文件夹，将在这个文件夹释放配置等中间文件，如为空将使用 Temp 文件夹</param>
        /// <param name="crossgen2ToolFile">配置 Crossgen2 工具的路径，如为空将自行寻找或安装</param>
        public void BuildReadyToRun(Platform platform, IReadOnlyCollection<FileInfo> inputFileList,
          IReadOnlyCollection<DirectoryInfo> searchDirectoryInfoCollection, DirectoryInfo? workingFolder = null, FileInfo? crossgen2ToolFile = null)
        {
            crossgen2ToolFile ??= FindCrossgen2ToolFile();
            var crossgen2ToolPath = crossgen2ToolFile.FullName;

            workingFolder ??= new DirectoryInfo(Path.GetTempPath());
            workingFolder = Directory.CreateDirectory(Path.Combine(workingFolder.FullName, "Crossgen2"));

            Logger.LogInformation(
           $"[Crossgen2Tool][Start] Crossgen2ToolPath=<{crossgen2ToolFile}>;WorkingFolder=<{workingFolder}>;Platform={platform};SearchDirectoryCollection=<{(string.Join(";", searchDirectoryInfoCollection))}>");
            foreach (var inputFile in inputFileList)
            {
                Logger.LogInformation($"[Crossgen2Tool][Start] {inputFile}");

                var result = BuildReadyToRunInner(inputFile, withPdbFile: true);

                if (!result)
                {
                    Logger.LogInformation($"[Crossgen2Tool][重试] {inputFile}");
                    // 如果加上 PDB 的失败了，那就尝试不加 PDB 的
                    result = BuildReadyToRunInner(inputFile, withPdbFile: false);
                }

                Logger.LogInformation($"[Crossgen2Tool]{(result ? "[Success]" : "[Fail]")} {inputFile}");
            }

            Logger.LogInformation($"[Crossgen2Tool][Finish]");

            bool BuildReadyToRunInner(FileInfo inputFile, bool withPdbFile)
            {
                var outputFile = new FileInfo(Path.Combine(workingFolder.FullName, inputFile.Name));
                var rspFile = Path.Combine(workingFolder.FullName,
                    Path.GetFileNameWithoutExtension(inputFile.Name) + ".rsp");

                var rspFileContent =
                    BuildRspFileContent(platform, inputFile, outputFile, searchDirectoryInfoCollection, withPdbFile);
                File.WriteAllText(rspFile, rspFileContent);

                var stopwatch = Stopwatch.StartNew();
                // 执行 Crossgen2Tool 命令，执行完成之后，可以拿到 outputFile 输出的文件，替换掉原有的文件
                var (success, message) = ProcessRunner.ExecuteCommand(crossgen2ToolPath,
                    ProcessCommand.ToArgumentPath($"@{rspFile}"), onReceivedOutput:
                    info =>
                    {
                        if (!string.IsNullOrEmpty(info.Message))
                        {
                            Logger.LogInformation($"[Crossgen2Tool][BuildReadyToRun] {info.Message}");
                        }
                    });
                if (!success)
                {
                    Logger.LogWarning($"[Crossgen2Tool][BuildReadyToRun][Fail] <{inputFile}> {message}");
                    return false;
                }
                else
                {
                    stopwatch.Stop();
                    Debug.WriteLine(stopwatch.ElapsedMilliseconds);

                    File.Move(outputFile.FullName, inputFile.FullName, true);

                    // 如果能找到 Xxx.ni.pdb 文件，那将 ni.pdb 文件也放在打包路径下，方便备份
                    var niPdbFile = new FileInfo(Path.Combine(workingFolder.FullName,
                        Path.GetFileNameWithoutExtension(inputFile.FullName) + ".ni.pdb"));
                    if (niPdbFile.Exists)
                    {
                        File.Move(niPdbFile.FullName, Path.Combine(inputFile.DirectoryName!, niPdbFile.Name), true);
                    }

                    return true;
                }
            }
        }

        private string BuildRspFileContent(Platform platform, FileInfo inputFile, FileInfo outputFile,
           IReadOnlyCollection<DirectoryInfo> searchDirectoryInfoCollection, bool withPdbFile)
        {
            string targetPlatform = platform switch
            {
                Platform.X86 => "x86",
                Platform.X64 => "x64",
                _ => throw new NotSupportedException($"Not Supported the {platform}.")
            };

            var content = new StringBuilder();
            content.AppendLine("--targetos:windows")
                .AppendLine($"--targetarch:{targetPlatform}");
            if (withPdbFile)
            {
                content.AppendLine($"--pdb");
            }

            content.AppendLine("-O"); // 优化

            var referenceFileHashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var searchDirectory in searchDirectoryInfoCollection.Concat(new[] { inputFile.Directory! }))
            {
                var referenceFileList = GetReferenceFileList(searchDirectory);
                foreach (var referenceFile in referenceFileList)
                {
                    if (referenceFileHashSet.Add(referenceFile.FullName))
                    {
                        content.AppendLine($"-r:\"{referenceFile.FullName}\"");
                    }
                }
            }

            content.AppendLine($"--out:\"{outputFile.FullName}\"")
                .AppendLine(inputFile.FullName);
            return content.ToString();
        }

        private FileInfo[] GetReferenceFileList(DirectoryInfo directory)
        {
            return directory.GetFiles("*.dll");
        }

        private FileInfo FindCrossgen2ToolFile()
        {
            // 先定位到 NuGet 缓存文件夹
            var nugetCacheDirectory = GetNuGetCacheDirectory();
            // 尝试找到 microsoft.netcore.app.crossgen2.win-x64 文件夹
            var crossgen2NuGetFolder = Path.Combine(nugetCacheDirectory.FullName, "microsoft.netcore.app.crossgen2.win-x64");
            // 预期是能找到的，这是默认有安装的，没有找到的话，那就先炸掉吧
            if (!Directory.Exists(crossgen2NuGetFolder))
            {
                throw new DirectoryNotFoundException($"找不到 Crossgen2 工具文件夹 {crossgen2NuGetFolder} 请尝试将任意项目执行 dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true 再重试");
            }

            // 找到任意版本
            var versionFolder = Directory.EnumerateDirectories(crossgen2NuGetFolder).FirstOrDefault();
            if (!Directory.Exists(versionFolder))
            {
                // 理论上不会存在这个情况，因为默认是有一个版本的
                throw new DirectoryNotFoundException($"找不到 Crossgen2 工具任何版本 {crossgen2NuGetFolder} 请尝试将任意项目执行 dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true 再重试");
            }

            var filePath = Path.Combine(versionFolder, @"tools\crossgen2.exe");
            return new FileInfo(filePath);
        }

        private DirectoryInfo GetNuGetCacheDirectory()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var nugetCacheFolder = Path.Combine(appDataFolder, @"..\..\.nuget\packages");
            return new DirectoryInfo(nugetCacheFolder);
        }
    }

    /// <summary>
    /// 平台，如 x86 或 x64 等平台
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// X86 平台
        /// </summary>
        X86,
        /// <summary>
        /// X64 平台
        /// </summary>
        X64,
        // 其他平台没测试过，先不写
    }
}
