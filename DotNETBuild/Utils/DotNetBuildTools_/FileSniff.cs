using System;
using System.IO;
using System.Reflection;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 寻找文件，将找到的文件放在 AppConfigurator 配置
    /// </summary>
    public class FileSniff : DotNetBuildTool
    {
        /// <summary>
        /// 创建协助寻找文件
        /// </summary>
        /// <param name="appConfigurator"></param>
        public FileSniff(IAppConfigurator appConfigurator) : base(appConfigurator)
        {
        }

        /// <summary>
        /// 找到编译需要文件
        /// </summary>
        public void Sniff()
        {
            if (FileSniffConfiguration.ShouldFindNuGet)
            {
                FindNugetFile();

                Logger.LogInformation("寻找 nuget 文件完成");
            }

            if (FileSniffConfiguration.ShouldFindMSBuild)
            {
                var msBuild = new MSBuild(AppConfigurator);
                msBuild.FindMSBuildFile();

                Logger.LogInformation("寻找 msbuild 文件完成");
            }

            if (FileSniffConfiguration.ShouldFindCodeDirectory)
            {
                FindCodeDirectory();
            }

            if (FileSniffConfiguration.ShouldFindSlnFile)
            {
                FindSlnFile();
            }

            if (FileSniffConfiguration.ShouldFindVsTest)
            {
                var testHelper = new TestHelper(AppConfigurator);
                testHelper.GetVsTest();
            }
        }

        private void FindCodeDirectory()
        {
            var codeConfiguration = CompileConfiguration;
            if (!string.IsNullOrEmpty(codeConfiguration.CodeDirectory))
            {
                Logger.LogInformation($"代码文件夹 {codeConfiguration.CodeDirectory}");
                return;
            }

            Logger.LogInformation("查找代码文件夹，通过定位 .git 文件夹方式");

            var directory = Environment.CurrentDirectory;
            Logger.LogInformation($"从 {directory} 向上寻找");

            if (FindCodeDirectory(directory))
            {
                return;
            }

            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                directory = Path.GetDirectoryName(assembly.Location);
                Logger.LogInformation($"从 {directory} 向上寻找");

                if (FindCodeDirectory(directory))
                {
                    return;
                }
            }
        }

        private bool FindCodeDirectory(string directory)
        {
            var codeConfiguration = CompileConfiguration;
            while (directory != null)
            {
                if (CheckCodeDirectory(directory))
                {
                    Logger.LogInformation($"找到代码文件夹 {directory}");
                    codeConfiguration.CodeDirectory = directory;
                    return true;
                }

                directory = Directory.GetParent(directory)?.FullName;
            }

            return false;
        }

        /// <summary>
        /// 判断当前是否代码文件夹 通过定位 .git 文件夹方式
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private bool CheckCodeDirectory(string directory)
        {
            var gitDirectory = Path.Combine(directory, ".git");
            if (Directory.Exists(gitDirectory))
            {
                return true;
            }

            var gitFile = Path.Combine(directory, ".git");
            if (File.Exists(gitFile))
            {
                return true;
            }

            return false;
        }

        private FileInfo FindSlnOrCsprojFile(DirectoryInfo directory)
        {
            // 优先找 sln 文件
            // 如果找不到，找 csproj 文件
            // 如果找不到，返回空
            // 如果找到大于一个文件，异常

            var slnFileList = Directory.GetFiles(directory.FullName, "*.sln");
            if (slnFileList.Length > 1)
            {
                throw new ArgumentException(
                    $"在{directory}找到大于一个 sln 文件，找到的文件如下：{string.Join(';', slnFileList)}");
            }
            else if (slnFileList.Length == 1)
            {
                return new FileInfo(slnFileList[0]);
            }
            else
            {
                // 再试试找找 csproj 文件，有些项目是不存在 sln 文件的
                var csprojFileList = Directory.GetFiles(directory.FullName, "*.csproj");
                if (csprojFileList.Length > 1)
                {
                    throw new ArgumentException(
                        $"在{directory.FullName}找不到一个 sln 文件，但找到多个 csproj 文件，找到的文件如下：{string.Join(';', csprojFileList)}");
                }
                else if (csprojFileList.Length == 1)
                {
                    return new FileInfo(csprojFileList[0]);
                }
                else
                {
                    // 什么都没找到
                    return null;
                }
            }
        }

        private void FindSlnFile()
        {
            var codeConfiguration = CompileConfiguration;
            var slnFile = codeConfiguration.SlnPath;

            if (string.IsNullOrEmpty(slnFile))
            {
                Logger.LogInformation($"没有在配置文件找到 CodeConfiguration.SlnFile 配置项，将进行自动查找");

                var directory = Environment.CurrentDirectory;

                // 先在当前工作路径寻找，如果找不到，再去代码文件夹找
                // 解决 sln 没有放在代码文件夹
                var slnOrCsprojFile = FindSlnOrCsprojFile(new DirectoryInfo(directory));
                if (slnOrCsprojFile != null)
                {
                    codeConfiguration.SlnPath = slnOrCsprojFile.FullName;
                    Logger.LogInformation($"找到项目文件：{slnOrCsprojFile.FullName}");
                }
                else
                {
                    // 试试代码文件夹
                    directory = codeConfiguration.CodeDirectory;
                    if (string.IsNullOrEmpty(directory))
                    {
                        throw new ArgumentException($"没有在{Environment.CurrentDirectory}找到sln文件或 csproj 文件");
                    }
                    else
                    {
                        slnOrCsprojFile = FindSlnOrCsprojFile(new DirectoryInfo(directory));
                        if (slnOrCsprojFile != null)
                        {
                            codeConfiguration.SlnPath = slnOrCsprojFile.FullName;
                            Logger.LogInformation($"找到项目文件：{slnOrCsprojFile.FullName}");
                        }
                        else
                        {
                            throw new ArgumentException($"没有在{Environment.CurrentDirectory}和{directory}找到sln文件或 csproj 文件");
                        }
                    }
                }
            }
            else
            {
                slnFile = Path.GetFullPath(slnFile);
                if (!File.Exists(slnFile))
                {
                    throw new ArgumentException($"从配置项读取项目文件 {slnFile} 不存在");
                }

                codeConfiguration.SlnPath = slnFile;
            }
        }

        private void FindNugetFile()
        {
            var nuGet = new NuGet(AppConfigurator);
            nuGet.GetNugetFile();
        }

        /// <summary>
        /// 构建的配置
        /// </summary>
        public CompileConfiguration CompileConfiguration => AppConfigurator.Of<CompileConfiguration>();

        private FileSniffConfiguration FileSniffConfiguration => AppConfigurator.Of<FileSniffConfiguration>();
    }

    public class FileSniffConfiguration : Configuration
    {
        /// <summary>
        /// 是否应该寻在 NuGet.exe 所在的文件，将可能导致下载 nuget.exe 可执行文件
        /// </summary>
        public bool ShouldFindNuGet
        {
            set => SetValue(value);
            get => GetBoolean() ?? true;
        }

        public bool ShouldFindMSBuild
        {
            set => SetValue(value);
            get => GetBoolean() ?? true;
        }

        public bool ShouldFindCodeDirectory
        {
            set => SetValue(value);
            get => GetBoolean() ?? true;
        }

        public bool ShouldFindSlnFile
        {
            set => SetValue(value);
            get => GetBoolean() ?? true;
        }

        public bool ShouldFindVsTest
        {
            set => SetValue(value);
            get => GetBoolean() ?? true;
        }
    }
}