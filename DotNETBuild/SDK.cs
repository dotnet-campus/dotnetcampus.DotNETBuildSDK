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
    /// <summary>
    /// 提供构建的开发包
    /// </summary>
    public static class SDK
    {
        /// <summary>
        /// 在框架内执行实际的逻辑，框架将自动初始化日志和配置等功能
        /// </summary>
        public static Task<int> Run(Action action, Assembly entryAssembly = null)
        {
            return Run(() =>
            {
                action();
                return Task.FromResult(0);
            }, entryAssembly);
        }

        /// <summary>
        /// 在框架内执行实际的逻辑，框架将自动初始化日志和配置等功能
        /// </summary>
        /// <param name="action"></param>
        /// <param name="entryAssembly"></param>
        /// <returns></returns>
        public static async Task<int> Run(Func<Task<int>> action, Assembly entryAssembly = null)
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

            LogApplicationInfo(entryAssembly);

            SetCommonConfiguration(appConfigurator);

            // 获取开始构建的时间，获取时将自动设置开始构建的时间
            _ = appConfigurator.Of<CompileConfiguration>().BuildStartTime;

            // 完成框架，重新设置一下日志
            logger.SwitchActualLogger();

            try
            {
                return await action();
            }
            finally
            {
                // 清空日志缓存
                logger.LogCacheMessage();

                if (currentConfiguration is {} fileConfiguration)
                {
                    await fileConfiguration.SaveAsync();
                }
            }
        }

        /// <summary>
        /// 清空框架的日志内容，此函数需要在业务代码的第一句通过 <see cref="Log"/> 日志输出之前调用，否则框架的日志将因为在业务日志输出之前输出而依然输出
        /// </summary>
        public static void CleanSdkLog()
        {
            var logger = Log.Logger;
            if (logger is LazyInitLogger lazyInitLogger)
            {
                lazyInitLogger.CleanLogCache();
            }
        }

        private static void SetCommonConfiguration(IAppConfigurator appConfigurator)
        {
            var sdkConfiguration = appConfigurator.Of<DotNETBuildSDKConfiguration>();
            if (sdkConfiguration.EnableSetCommonConfiguration)
            {
                var compileConfiguration = appConfigurator.Of<CompileConfiguration>();

                compileConfiguration.SetCommonConfiguration();
            }
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