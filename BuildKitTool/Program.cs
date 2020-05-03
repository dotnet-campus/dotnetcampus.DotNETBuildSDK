using CommandLine;
using dotnetCampus.DotNETBuild.Utils;
using System;
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
                    error => 1
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

            ConfigurationExtension.MergeConfiguration(option, configuration);

            CheckCommandInstall();

            var appConfigurator = AppConfigurator.GetAppConfigurator();
            // 写入当前能找到的各个文件的配置
            var fileSniff = new FileSniff(appConfigurator);
            fileSniff.Sniff();

            return 0;
        }

        /// <summary>
        /// 协助部署使用的，用于协助将所有的构建需要的命令更新
        /// </summary>
        private static void CheckCommandInstall()
        {
            
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