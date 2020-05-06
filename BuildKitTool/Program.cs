using CommandLine;
using dotnetCampus.DotNETBuild.Utils;
using System;
using System.Collections.Generic;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;

namespace BuildKitTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<InitOption>(args)
                .MapResult
                (
                    (InitOption option) => RunInit(option),
                    error =>
                    {
                        Console.WriteLine("少年郎，是不是忘了输入 init 执行初始化了");
                        return 1;
                    }
                );
        }

        private static int RunInit(InitOption option)
        {
            Log.Info("BuildKit Tool 开始初始化，详细命令请看 https://github.com/dotnet-campus/dotnetcampus.DotNETBuildSDK");

            if (Enum.TryParse(option.Configuration, ignoreCase: true, out ConfigurationEnum configuration))
            {
            }
            else
            {
                configuration = ConfigurationEnum.None;
            }

            InitLog(configuration);

            var fileConfiguration = ConfigurationExtension.MergeConfiguration(option, configuration);
            var appConfigurator = fileConfiguration.CreateAppConfigurator();

            CheckCommandInstall(appConfigurator);

            // 写入当前能找到的各个文件的配置
            var fileSniff = new FileSniff(appConfigurator);
            fileSniff.Sniff();

            appConfigurator.Of<CompileConfiguration>().SetCommonConfiguration();

            fileConfiguration.SaveAsync().Wait();

            return 0;
        }

        /// <summary>
        /// 协助部署使用的，用于协助将所有的构建需要的命令更新
        /// </summary>
        /// <param name="appConfigurator"></param>
        private static void CheckCommandInstall(IAppConfigurator appConfigurator)
        {
            var localToolList = new List<string>
            {
                "dotnetCampus.GitRevisionTask",
                "dotnetCampus.NuGetTask",

                // https://github.com/dotnet-campus/dotnetCampus.TagToVersion
                "dotnetCampus.TagToVersion",
                // https://github.com/dotnet-campus/dotnetCampus.UpdateAllDotNetTools
                "dotnetCampus.UpdateAllDotNetTools",
            };

            // 读取全局配置的工具
            var configToolList = appConfigurator.Of<ToolConfiguration>().DotNETToolList;

            Log.Debug("开始确认工具准备完成");
            localToolList.AddRange(configToolList);
            var dotNetTool = new DotNet(appConfigurator);
            dotNetTool.TryInstall(localToolList);
        }


        static void InitLog(ConfigurationEnum configurationEnum)
        {
            switch (configurationEnum)
            {
                case ConfigurationEnum.Debug:
                {
                    Log.LogLevel = LogLevel.Debug;
                    Log.Debug("开始配置日志输出等级");
                    Log.Debug("进入 Debug 模式，更多信息将会详细输出");
                }
                    break;
                case ConfigurationEnum.Release:
                    // 保持原因的信息级输出
                    break;
                case ConfigurationEnum.None:
                default:
                {
                    Log.LogLevel = LogLevel.Debug;
                    Log.Debug("开始配置日志输出等级");
                    Log.Debug($"看不懂你的配置是什么意思，就按照 Debug 模式输出");
                    break;
                }
            }
        }
    }
}