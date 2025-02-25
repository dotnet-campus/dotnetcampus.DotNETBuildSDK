using System;
using System.ComponentModel;
using System.Diagnostics;

using dotnetCampus.Configurations;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 表示构建过程使用的工具
    /// </summary>
    /// 设计上是构建过程的工具，输入和输出都是通过 <see cref="IAppConfigurator"/> 实现的，方便随时插入其他命令行调用的进程
    public abstract class DotNetBuildTool
    {
        /// <summary>
        /// 创建构建过程使用的工具
        /// </summary>
        /// <param name="appConfigurator"></param>
        /// <param name="logger"></param>
        protected DotNetBuildTool(IAppConfigurator appConfigurator, ILogger logger = null)
        {
            AppConfigurator = appConfigurator
                              ?? dotnetCampus.DotNETBuild.Context.AppConfigurator.GetAppConfigurator();

            Logger = logger ?? AppConfigurator.Of<LogConfiguration>().GetLogger();
        }

        /// <summary>
        /// 日志
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// 配置
        /// </summary>
        /// 设计上，所有的输入和输出都通过配置实现。但是配置不公开的原因是因为每个工具有自己私有的配置，不能给其他模块使用
        protected IAppConfigurator AppConfigurator { get; }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="exeName"></param>
        /// <param name="arguments"></param>
        /// <param name="workingDirectory">默认将使用当前进程工作路径</param>
        /// <param name="needAutoLogOutput">是否实时输出</param>
        /// <param name="processStartInfoFilter">用于对输入过程的过滤。如设置编码等</param>
        /// <returns></returns>
        protected ProcessResult ExecuteProcessCommand(string exeName, string arguments,
            string workingDirectory = "", bool needAutoLogOutput = true, Action<ProcessStartInfo>? processStartInfoFilter = null)
        {
            Logger.LogInformation($"{exeName} {arguments}");

            var result = ProcessRunner.ExecuteCommand(exeName, arguments, workingDirectory, processOutputInfo =>
            {
                var message = processOutputInfo.Message;
                if (needAutoLogOutput && !string.IsNullOrEmpty(message))
                {
                    if (processOutputInfo.OutputType == OutputType.StandardOutput)
                    {
                        Logger.LogInformation(message);
                    }
                    else if (processOutputInfo.OutputType == OutputType.StandardError)
                    {
                        Logger.LogWarning(message);
                    }
                }
            }, processStartInfoFilter);
            Logger.LogInformation($"ExitCode: {result.ExitCode}");

            return result;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// 这是给旧版本使用的，不能实时输出调用的进程输出内容，不推荐使用。推荐使用 <see cref="ExecuteProcessCommand"/> 方法
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected (bool success, string output) ExecuteCommand(string exeName, string arguments,
            string workingDirectory = "")
        {
            Logger.LogInformation($"执行命令 {exeName} {arguments}");
            (bool success, string output) = ProcessCommand.ExecuteCommand(exeName, arguments, workingDirectory);

            if (success)
            {
                Logger.LogInformation(output);
            }
            else
            {
                Logger.LogWarning(output);
            }

            return (success, output);
        }
    }
}