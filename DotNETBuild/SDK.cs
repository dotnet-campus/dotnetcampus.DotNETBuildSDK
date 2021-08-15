using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace dotnetCampus.DotNETBuild
{
    internal static class SDK
    {
        public static async Task<int> Run(Func<Task<int>> action, Assembly entryAssembly=null)
        {
            entryAssembly ??= Assembly.GetEntryAssembly();

            // 预先设置框架内的日志输出，后续再次设置为项目配置的输出。开始是放在 Lazy 里面，因此基本不伤。框架内的调试输出是给 SDK 框架开发者了解的，上层业务开发者可以无须了解，因此框架的调试输出内容，不会输出给到上层业务开发者
            Log.LogLevel = LogLevel.Information;
            var logger = Log.InitLazyLogger();

            // 全局可以配置日志输出
            var currentConfiguration = ConfigurationHelper.GetCurrentConfiguration();
            var appConfigurator = currentConfiguration.CreateAppConfigurator();
            Log.LogLevel = appConfigurator.Of<LogConfiguration>().LogLevel;
            AppConfigurator.SetAppConfigurator(appConfigurator);

            // 在配置完成之后，才可以设置实际的日志
            logger.ActualLogger = new CommonLogger();

            LogApplicationInfo(entryAssembly);

            SetCommonConfiguration(appConfigurator);

            // 完成框架，重新设置一下日志
            Log.Logger.SwitchActualLogger();

            try
            {
                return await action();
            }
            finally
            {
                // 清空日志缓存
                Log.Logger.LogCacheMessage();

                if (currentConfiguration is FileConfigurationRepo fileConfiguration)
                {
                    await fileConfiguration.SaveAsync();
                }
            }
        }

        private static void SetCommonConfiguration(IAppConfigurator appConfigurator)
        {
            var compileConfiguration = appConfigurator.Of<CompileConfiguration>();

            compileConfiguration.SetCommonConfiguration();
        }

      

        private static void LogApplicationInfo(Assembly entryAssembly)
        {
            var assemblyTitleAttribute = entryAssembly.GetCustomAttribute<AssemblyTitleAttribute>();
            if (assemblyTitleAttribute != null)
            {               
                Log.Info("ToolName: " + assemblyTitleAttribute.Title);
            }

            var assemblyVersionAttribute = entryAssembly.GetCustomAttribute<AssemblyVersionAttribute>();
            if (assemblyVersionAttribute != null)
            {
                Log.Info("ToolVersion: " + assemblyVersionAttribute.Version);
            }
            else
            {
                var assemblyFileVersionAttribute = entryAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                if (assemblyFileVersionAttribute != null)
                {
                    Log.Info("ToolVersions: " + assemblyFileVersionAttribute.Version);
                }
            }
        }
    }
}