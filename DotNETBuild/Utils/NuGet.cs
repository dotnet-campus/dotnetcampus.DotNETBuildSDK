using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
    public class NuGet : DotNetBuildTool
    {
        /// <inheritdoc />
        public NuGet(IAppConfigurator appConfigurator) : base(appConfigurator)
        {
        }

        /// <summary>
        /// 执行 nuget restore [command] 这里的[command]为传入参数，默认值是 sln 文件
        /// </summary>
        /// <param name="command"></param>
        public (bool success, string output) Restore(string command = "")
        {
            if (string.IsNullOrEmpty(command))
            {
                command = ProcessCommand.ToArgumentPath(CompileConfiguration.SlnPath);
            }

            return ExecuteCommand(GetNugetFile(), $"restore {command}");
        }

        /// <summary>
        /// 还原 NuGet 库，如果执行失败会抛出异常
        /// </summary>
        /// <param name="slnPath"></param>
        /// <param name="msbuildPath"></param>
        public void Restore(FileInfo slnPath, string msbuildPath)
        {
            var command =
                $"restore {ProcessCommand.ToArgumentPath(slnPath.FullName)} -MSBuildPath {ProcessCommand.ToArgumentPath(msbuildPath)}";

            ExecuteCommand(GetNugetFile(), command);
        }

        public string GetNugetFile()
        {
            FindNugetFile();

            var toolConfiguration = ToolConfiguration;
            return toolConfiguration.NugetPath;
        }

        private ToolConfiguration ToolConfiguration => AppConfigurator.Of<ToolConfiguration>();

        private void FindNugetFile()
        {
            var toolConfiguration = ToolConfiguration;

            if (!string.IsNullOrEmpty(toolConfiguration.NugetPath))
            {
                Logger.LogInformation($"获取配置文件的 NuGet 文件 {toolConfiguration.NugetPath}");
                return;
            }

            // 尝试寻找 NuGet 文件
            //var path = Environment.GetEnvironmentVariable("Path");

            //var file = Path.Combine(CurrentDirectory.FullName, "nuget.exe");

            //if (File.Exists(file))
            //{
            //    CompileConfiguration.NugetFile = file;
            //    return;
            //}

            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Combine(folder, "tools", "nuget.exe");

            if (File.Exists(file))
            {
                Log.Info($"找到 NuGet 文件 {file}");
                toolConfiguration.NugetPath = file;
                return;
            }

            Logger.LogInformation($"找不到 {file} 文件，从资源拿到文件");

            Logger.LogInformation($"从服务器下载 nuget 文件");

            var directory = Path.GetDirectoryName(file);
            Directory.CreateDirectory(directory);

            var webClient = new WebClient();
            webClient.DownloadFile(new Uri("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"), file);

            if (File.Exists(file))
            {
                ToolConfiguration.NugetPath = file;
                Logger.LogInformation($"下载 {file} 完成");
            }
            else
            {
                throw new ArgumentException($"找不到 Nuget.exe 文件，在{file}没有找到文件，可以通过 ToolConfiguration.NugetPath 设置");
            }
        }

        public CompileConfiguration CompileConfiguration => AppConfigurator.Of<CompileConfiguration>();

        /// <summary>
        /// 将所有能找到的 nupkg 文件发布
        /// </summary>
        public void PublishNupkg(string version = "")
        {
            var fileList = GetNupkgFileList(version);

            foreach (var file in fileList)
            {
                Logger.LogInformation($"开始上传{file}");
                PublishNupkg(new FileInfo(file));
            }
        }

        private string[] GetNupkgFileList(string version = "")
        {
            var nupkgDirectory = FindNupkgDirectory();

            if (string.IsNullOrEmpty(nupkgDirectory))
            {
                Logger.LogInformation($"找不到上传 nuget 的文件夹，请确定 nuget 文件已经生成，将进行自动寻找");
                var fileList = Directory.GetFiles(CompileConfiguration.CodeDirectory, "*.nupkg", SearchOption.AllDirectories);
                if (fileList.Length == 0)
                {
                    throw new ArgumentException("找不到nupkg文件");
                }

                return fileList;
            }
            else
            {
                var fileList = Directory.GetFiles(nupkgDirectory, $"*{version}.nupkg");

                if (fileList.Length == 0)
                {
                    throw new ArgumentException($"在{nupkgDirectory}没有找到一个可以上传的文件");
                }

                return fileList;
            }
        }

        /// <summary>
        /// 发布指定的文件
        /// </summary>
        public void PublishNupkg(FileInfo nupkgFile, bool skipDuplicate = true)
        {
            if (!File.Exists(nupkgFile.FullName))
            {
                throw new FileNotFoundException($"找不到 {nupkgFile} 文件");
            }

            var toolConfiguration = AppConfigurator.Of<ToolConfiguration>();
            var nugetConfiguration = AppConfigurator.Of<NugetConfiguration>();
            var nugetPath = toolConfiguration.NugetPath;

            Logger.LogInformation($"开始发布主要的源 {nugetConfiguration.Source}");
            RunPublishNupkg(nugetPath, nupkgFile.FullName, nugetConfiguration.Source,nugetConfiguration.ApiKey, skipDuplicate);

            // 推送额外的源，推送失败自动忽略
            var attachedSource = nugetConfiguration.AttachedSource;
            if (attachedSource.Length > 0)
            {
                Logger.LogInformation($"额外推送的源 NugetConfiguration.AttachedSource 有 {attachedSource.Length} 个");
                for (var i = 0; i < attachedSource.Length; i++)
                {
                    try
                    {
                        // 依据 Url 里面不包含空格，可以使用空格分开 APIKey 和源
                        var splitSource = attachedSource[i].Split(" ");
                        if (splitSource.Length>0)
                        {
                            string source = splitSource[0];
                            string apiKey = null;
                            if (splitSource.Length > 1)
                            {
                                apiKey = splitSource[1];
                            }

                            if (!string.IsNullOrEmpty(source))
                            {
                                Logger.LogInformation($"正在推送第 {i}/{attachedSource.Length} 个额外 NuGet 源 {source}");
                                RunPublishNupkg(nugetPath, nupkgFile.FullName, source, apiKey, skipDuplicate);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning(e,"推送额外 NuGet 源");
                    }
                }
            }
        }

        public void RunPublishNupkg(string nugetToolPath, string nupkgFile, string source, string apiKey = "",
            bool skipDuplicate = true)
        {
            var temp = ProcessCommand.ToArgumentPath(nupkgFile);
            ExecuteCommand(nugetToolPath,
                $"push {temp} -Source {source} {(string.IsNullOrEmpty(apiKey) ? "" : $"-ApiKey {apiKey}")} {(skipDuplicate ? "-SkipDuplicate" : "")}");
        }


        private string FindNupkgDirectory()
        {
            var nupkgDirectory = CompileConfiguration.NupkgDirectory;
            if (string.IsNullOrEmpty(nupkgDirectory))
            {
                Logger.LogInformation($"没有输出 nuget 包文件夹，自动查找");
                var directory = Path.Combine(CompileConfiguration.CodeDirectory, @"bin\Release");
                if (CheckNupkgDirectory(directory))
                {
                    CompileConfiguration.NupkgDirectory = directory;
                    Logger.LogInformation($"找到 nuget 包文件夹 {directory}");
                    return directory;
                }

                directory = Path.Combine(CompileConfiguration.CodeDirectory, @"bin\Debug");
                if (CheckNupkgDirectory(directory))
                {
                    CompileConfiguration.NupkgDirectory = directory;
                    Logger.LogInformation($"找到 nuget 包文件夹 {directory}");
                    return directory;
                }
            }

            return nupkgDirectory;

            bool CheckNupkgDirectory(string directory)
            {
                return Directory.GetFiles(directory, "*.nupkg").Any();
            }
        }
    }
}