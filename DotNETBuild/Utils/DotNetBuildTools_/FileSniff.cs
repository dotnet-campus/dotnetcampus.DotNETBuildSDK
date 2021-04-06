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
            FindNugetFile();

            Logger.LogInformation("寻找 nuget 文件完成");

            var msBuild = new MsBuild(AppConfigurator);
            msBuild.FindMsBuildFile();

            Logger.LogInformation("寻找 msbuild 文件完成");

            FindCodeDirectory();

            FindSlnFile();

            var testHelper = new TestHelper(AppConfigurator);
            testHelper.GetVsTest();
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

        private void FindSlnFile()
        {
            var codeConfiguration = CompileConfiguration;
            var slnFile = codeConfiguration.SlnPath;

            if (string.IsNullOrEmpty(slnFile))
            {
                Logger.LogInformation($"没有在配置文件找到 CodeConfiguration.SlnFile 配置项，将进行自动查找");

                var directory = Environment.CurrentDirectory;

                if (!string.IsNullOrEmpty(codeConfiguration.CodeDirectory))
                {
                    directory = codeConfiguration.CodeDirectory;
                }

                var slnFileList = Directory.GetFiles(directory, "*.sln");

                if (slnFileList.Length > 1)
                {
                    throw new ArgumentException(
                        $"在{Environment.CurrentDirectory}找到大于一个 sln 文件，找到的文件如下：{string.Join(';', slnFileList)}");
                }
                else if (slnFileList.Length == 1)
                {
                    slnFile = slnFileList[0];
                    codeConfiguration.SlnPath = slnFile;
                    Logger.LogInformation($"找到sln文件 {slnFile}");
                }
                else
                {
                    // 再试试找找 csproj 文件，有些项目是不存在 sln 文件的
                    var csprojFileList = Directory.GetFiles(directory, "*.csproj");
                    if (csprojFileList.Length > 1)
                    {
                        throw new ArgumentException(
                            $"在{Environment.CurrentDirectory}找不到一个 sln 文件，但找到多个 csproj 文件，找到的文件如下：{string.Join(';', csprojFileList)}");
                    }
                    else if (csprojFileList.Length == 1)
                    {
                        slnFile = csprojFileList[0];
                        codeConfiguration.SlnPath = slnFile;
                        Logger.LogInformation($"找不到sln文件，但找到一个 csproj 文件 {slnFile}");
                    }
                    else
                    {
                        throw new ArgumentException($"没有在{Environment.CurrentDirectory}找到sln文件或 csproj 文件");
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
    }
}