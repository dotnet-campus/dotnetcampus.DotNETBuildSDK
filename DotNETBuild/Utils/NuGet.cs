using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;

namespace dotnetCampus.DotNETBuild.Utils
{
    public class NuGet
    {
        /// <inheritdoc />
        public NuGet(IAppConfigurator appConfigurator)
        {
            AppConfigurator = appConfigurator;
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

            return ProcessCommand.ExecuteCommand(GetNugetFile(), $"restore {command}");
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

            ProcessCommand.RunCommand(GetNugetFile(), command);
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
                Log.Info($"获取配置文件的 NuGet 文件 {toolConfiguration.NugetPath}");
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

            Log.Info($"找不到 {file} 文件，从资源拿到文件");

            Log.Info($"从服务器下载 nuget 文件");

            var directory = Path.GetDirectoryName(file);
            Directory.CreateDirectory(directory);

            var webClient = new WebClient();
            webClient.DownloadFile(new Uri("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"), file);

            if (File.Exists(file))
            {
                ToolConfiguration.NugetPath = file;
                Log.Info($"下载 {file} 完成");
            }
            else
            {
                throw new ArgumentException($"找不到 Nuget.exe 文件，在{file}没有找到文件，可以通过 ToolConfiguration.NugetPath 设置");
            }
        }

        public IAppConfigurator AppConfigurator { get; }

        public CompileConfiguration CompileConfiguration => AppConfigurator.Of<CompileConfiguration>();

        /// <summary>
        /// 将所有能找到的 nupkg 文件发布
        /// </summary>
        public void PublishNupkg(string version = "")
        {
            var fileList = GetNupkgFileList(version);

            foreach (var file in fileList)
            {
                Log.Info($"开始上传{file}");
                PublishNupkg(new FileInfo(file));
            }
        }

        private string[] GetNupkgFileList(string version = "")
        {
            var nupkgDirectory = FindNupkgDirectory();

            if (string.IsNullOrEmpty(nupkgDirectory))
            {
                Log.Info($"找不到上传 nuget 的文件夹，请确定 nuget 文件已经生成，将进行自动寻找");
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
        /// <param name="nupkgFile"></param>
        public void PublishNupkg(FileInfo nupkgFile)
        {
            if (!File.Exists(nupkgFile.FullName))
            {
                throw new FileNotFoundException($"找不到 {nupkgFile} 文件");
            }

            var toolConfiguration = AppConfigurator.Of<ToolConfiguration>();
            var nugetConfiguration = AppConfigurator.Of<NugetConfiguration>();
            var nugetPath = toolConfiguration.NugetPath;

            var temp = ProcessCommand.ToArgumentPath(nupkgFile.FullName);

            ProcessCommand.ExecuteCommand(nugetPath,
                $"push {temp} -Source {nugetConfiguration.Source} -ApiKey {nugetConfiguration.ApiKey}");
        }

        private string FindNupkgDirectory()
        {
            var nupkgDirectory = CompileConfiguration.NupkgDirectory;
            if (string.IsNullOrEmpty(nupkgDirectory))
            {
                Log.Info($"没有输出 nuget 包文件夹，自动查找");
                var directory = Path.Combine(CompileConfiguration.CodeDirectory, @"bin\Release");
                if (CheckNupkgDirectory(directory))
                {
                    CompileConfiguration.NupkgDirectory = directory;
                    Log.Info($"找到 nuget 包文件夹 {directory}");
                    return directory;
                }

                directory = Path.Combine(CompileConfiguration.CodeDirectory, @"bin\Debug");
                if (CheckNupkgDirectory(directory))
                {
                    CompileConfiguration.NupkgDirectory = directory;
                    Log.Info($"找到 nuget 包文件夹 {directory}");
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