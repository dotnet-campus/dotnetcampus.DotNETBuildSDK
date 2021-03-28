using System;
using System.IO;
using System.Linq;
using System.Threading;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;
using dotnetCampus.GitCommand;
using Microsoft.Extensions.Logging;
using Walterlv.IO.PackageManagement;

namespace dotnetCampus.CopyAfterCompileTool
{
    /// <summary>
    /// 对二分查找做准备，编译每个提交
    /// </summary>
    public class BinaryChopCompiler
    {
        public static BinaryChopCompiler CreateInstance(IAppConfigurator? appConfigurator = null)
        {
            appConfigurator ??= dotnetCampus.DotNETBuild.Context.AppConfigurator.GetAppConfigurator();

            var compileConfiguration = appConfigurator.Of<CompileConfiguration>();

            Console.WriteLine($"代码仓库文件夹 {compileConfiguration.CodeDirectory}");
            Console.WriteLine($"代码构建输出文件夹 {compileConfiguration.OutputDirectory}");

            var binaryChopCompileConfiguration = appConfigurator.Of<BinaryChopCompileConfiguration>();

            Console.WriteLine($"保存的文件夹 {binaryChopCompileConfiguration.TargetDirectory}");
            Console.WriteLine($"构建代码分支 {binaryChopCompileConfiguration.OriginBranch}");

            var codeDirectory = new DirectoryInfo(compileConfiguration.CodeDirectory);
            var outputDirectory = new DirectoryInfo(compileConfiguration.OutputDirectory);
            var targetDirectory = new DirectoryInfo(binaryChopCompileConfiguration.TargetDirectory);
            var originBranch = binaryChopCompileConfiguration.OriginBranch;

            if (!codeDirectory.Exists)
            {
                throw new ArgumentException("代码仓库文件夹 CodeDirectory 找不到");
            }

            Directory.CreateDirectory(targetDirectory.FullName);

            return new BinaryChopCompiler(appConfigurator, codeDirectory, targetDirectory, outputDirectory, originBranch);
        }

        public BinaryChopCompiler(IAppConfigurator appConfigurator,
            DirectoryInfo codeDirectory,
            DirectoryInfo targetDirectory,
            DirectoryInfo? outputDirectory = null,
            string? originBranch = null,
            //ICompiler compiler = null,
            ILogger? logger = null)
        {
            AppConfigurator = appConfigurator;

            CodeDirectory = codeDirectory;
            TargetDirectory = targetDirectory;

            if (logger is null)
            {
                var logConfiguration = appConfigurator.Of<LogConfiguration>();

                logger = logConfiguration.GetLogger();
            }

            Logger = logger;

            if (!string.IsNullOrEmpty(originBranch))
            {
                OriginBranch = originBranch;
            }

            var git = new Git(codeDirectory)
            {
                DefaultCommandTimeout = TimeSpan.FromMinutes(3)
            };

            _git = git;
            //Compiler = compiler ?? new MsBuildCompiler();

            if (outputDirectory is null)
            {
                outputDirectory = new DirectoryInfo(Path.Combine(codeDirectory.FullName, "bin"));
            }

            OutputDirectory = outputDirectory;
        }

        private ILogger Logger { get; }

        private void Log(string str) => Logger?.LogInformation(str);

        private readonly Git _git;

        public string OriginBranch { get; } = "dev";

        /// <summary>
        /// 移动到的文件夹，编译完成将输出移动到这个文件夹
        /// </summary>
        public DirectoryInfo TargetDirectory { get; }

        public DirectoryInfo CodeDirectory { get; }

        /// <summary>
        /// 输出文件夹
        /// </summary>
        public DirectoryInfo OutputDirectory { get; }

        public void CompileAllCommitAndCopy()
        {
            _git.FetchAll();

            var commitList = GetCommitList().Reverse().ToList();

            for (var i = 0; i < commitList.Count; i++)
            {
                var commit = commitList[i];
                try
                {
                    Log($"开始 {commit} 二分，本次任务第{i+1}次构建，总共{commitList.Count}次构建");
                    CleanDirectory(commit);

                    // 如果没有指定使用 bat 脚本构建，那么执行通用构建

                    var compilerBatFile = BinaryChopCompileConfiguration.CompilerBatFile;
                    if (string.IsNullOrEmpty(compilerBatFile) || !File.Exists(compilerBatFile))
                    {
                        Log($"找不到指定的 bat 构建脚本文件 {compilerBatFile} 将使用默认的方式构建");

                        // 这里是代码里面自己带的构建配置文件
                        var appConfigurator = GetCurrentBuildConfiguration();

                        var currentBuildLogFile1 = GetCurrentBuildLogFile(appConfigurator);

                        var msBuildCompiler = new MsBuild(appConfigurator);
                        msBuildCompiler.Build(new MsBuildCommandOptions()
                        {
                            ShouldRestore = true
                        });
                        var currentBuildLogFile = currentBuildLogFile1;

                        MoveFile(commit, currentBuildLogFile);
                    }
                    else
                    {
                        Log($"开始执行 {compilerBatFile} 构建脚本文件");

                        var (success, output) = ProcessCommand.ExecuteCommand(compilerBatFile, null);
                        // 将输出写入到文件里面
                        var logFile = Path.GetTempFileName();
                        File.WriteAllText(logFile, output);
                        MoveFile(commit, new FileInfo(logFile));
                    }

                    LastCommit = commit;
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }
            }
        }

        private static FileInfo GetCurrentBuildLogFile(IAppConfigurator appConfigurator)
        {
            var currentBuildLogFile = new FileInfo(Path.GetTempFileName());
            var logConfiguration = appConfigurator.Of<LogConfiguration>();
            logConfiguration.BuildLogFile = currentBuildLogFile.FullName;
            return currentBuildLogFile;
        }

        /// <summary>
        /// 获取当前构建的配置
        /// </summary>
        /// <returns></returns>
        private IAppConfigurator GetCurrentBuildConfiguration()
        {
            // 这是在每次构建的时候，放在代码仓库的构建代码
            var currentBuildConfiguration = Path.Combine(CompileConfiguration.CodeDirectory, "Build.coin");
            var fileConfigurationRepo = ConfigurationFactory.FromFile(currentBuildConfiguration);
            var appConfigurator = fileConfigurationRepo.CreateAppConfigurator();
            var compileConfiguration = appConfigurator.Of<CompileConfiguration>();
            compileConfiguration.CodeDirectory = CompileConfiguration.CodeDirectory;

            var toolConfiguration = appConfigurator.Of<ToolConfiguration>();
            var nugetFile = new FileInfo("nuget.exe");
            if (nugetFile.Exists)
            {
                toolConfiguration.NugetPath = nugetFile.FullName;
            }

            return appConfigurator;
        }

        private void MoveFile(string commit, FileInfo? buildLogFile)
        {
            var outputDirectory = new DirectoryInfo(OutputDirectory.FullName);

            var moveDirectory = Path.Combine(TargetDirectory.FullName, commit);
            Log($"将{outputDirectory.FullName}移动到{moveDirectory}");

            PackageDirectory.Move(outputDirectory, new DirectoryInfo(moveDirectory));

            if (buildLogFile is not null && File.Exists(buildLogFile.FullName))
            {
                try
                {
                    // 等待日志写完
                    Thread.Sleep(3000);

                    Directory.CreateDirectory(moveDirectory);
                    var logFile = Path.Combine(moveDirectory, "BuildLog.txt");
                    buildLogFile.CopyTo(logFile);
                    File.Delete(buildLogFile.FullName);
                }
                catch (Exception)
                {

                }
            }
        }

        private void CleanDirectory(string commit)
        {
            Log($"开始清空仓库");

            var git = _git;
            git.Clean();
            git.Checkout(commit, true);
        }

        private string[] GetCommitList()
        {
            var git = _git;
            if (string.IsNullOrEmpty(LastCommit))
            {
                return git.GetLogCommit();
            }
            else
            {
                return git.GetLogCommit(LastCommit, OriginBranch);
            }
        }

        private string LastCommit
        {
            set => BinaryChopCompileConfiguration.LastCommit = value;
            get => BinaryChopCompileConfiguration.LastCommit;
        }

        private IAppConfigurator AppConfigurator { get; }

        private CompileConfiguration CompileConfiguration => AppConfigurator.Of<CompileConfiguration>();

        private BinaryChopCompileConfiguration BinaryChopCompileConfiguration =>
            AppConfigurator.Of<BinaryChopCompileConfiguration>();
    }
}