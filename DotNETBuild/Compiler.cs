using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Versioning;
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
#pragma warning disable CS0618
            MsBuild = new MsBuild(AppConfigurator);
#pragma warning restore CS0618
            MSBuild = new MSBuild(AppConfigurator);
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("大小写命名错误，请使用 MSBuild 代替")]
        public MsBuild MsBuild { get; }

        public MSBuild MSBuild { get; }

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

            (bool success, string output) = ExecuteProcessCommand("dotnet", $"build {command}");
            return (success, output);
        }

        protected void WriteLog(string message)
        {
            Logger.LogInformation(message);
        }

        /// <summary>
        /// 执行命令，使用 cmd 执行命令
        /// </summary>
        /// <param name="str"></param>
        /// <param name="workingDirectory">工作路径默认为代码文件夹</param>
        /// <returns></returns>
#if NET6_0_OR_GREATER
        [SupportedOSPlatform("Windows")]
#endif
        protected (bool success, string output) Command(string str, string workingDirectory = "")
        {
            (bool success, string output) = ExecuteProcessCommand("cmd.exe", $"/c {str}", workingDirectory);
            return (success, output);
        }
    }
}