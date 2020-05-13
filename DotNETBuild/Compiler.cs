using System;
using System.IO;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild
{
    /// <summary>
    /// 编译器
    /// </summary>
    public class Compiler : DotNetBuildTool
    {
        /// <inheritdoc />
        public Compiler(IAppConfigurator appConfigurator = null, ILogger logger = null) : base(appConfigurator, logger)
        {
            _appConfigurator = appConfigurator;

            Init();

            Nuget = new NuGet(AppConfigurator);
            MsBuild = new MsBuild(AppConfigurator);
            TestHelper = new TestHelper(AppConfigurator);

            //_logger = logger ?? AppConfigurator.Of<LogConfiguration>().GetLogger();
        }

        //private ILogger _logger;

        ///// <summary>
        ///// 应用配置
        ///// </summary>
        //public IAppConfigurator AppConfigurator => _appConfigurator ?? Context.AppConfigurator.GetAppConfigurator();

        /// <summary>
        /// 编译配置
        /// </summary>
        public CompileConfiguration CompileConfiguration => AppConfigurator.Of<CompileConfiguration>();

        public virtual void Compile()
        {
            var slnFile = CompileConfiguration.SlnPath;

            slnFile = ProcessCommand.ToArgumentPath(slnFile);
            DotNetBuild($" {slnFile} -c release");
        }

        private readonly IAppConfigurator _appConfigurator;

        private void Init()
        {
            // todo 放在获取属性
            var fileSniff = new FileSniff(AppConfigurator);
            fileSniff.Sniff();
        }

        public NuGet Nuget { get; }

        public MsBuild MsBuild { get; }

        public TestHelper TestHelper { get; }

        protected (bool success, string output) NuGetRestore(string command = "")
        {
            var nuGet = new NuGet(AppConfigurator);
            return nuGet.Restore(command);
        }

        protected (bool success, string output) DotNetBuild(string command = "")
        {
            if (string.IsNullOrEmpty(command))
            {
                command = $" {ProcessCommand.ToArgumentPath(CompileConfiguration.SlnPath)}";
            }

            return ExecuteCommand("dotnet", $"build {command}");
        }

        protected void WriteLog(string message)
        {
            Logger.LogInformation(message);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="str"></param>
        /// <param name="workingDirectory">工作路径默认为代码文件夹</param>
        /// <returns></returns>
        protected (bool success, string output) Command(string str, string workingDirectory = "")
        {
            return ExecuteCommand("cmd.exe", $"/c {str}", workingDirectory);
        }
    }
}