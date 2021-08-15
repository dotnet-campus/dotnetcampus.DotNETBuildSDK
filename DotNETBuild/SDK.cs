using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace dotnetCampus.DotNETBuild
{
    internal static class SDK
    {
        public static IAppConfigurator Init(Assembly entryAssembly, LogLevel logLevel = LogLevel.Information)
        {
            Log.LogLevel = logLevel;
            var logger = Log.InitLazyLogger();

            LogApplicationInfo(entryAssembly, logger);

            var appConfigurator = AppConfigurator.InitAppConfigurator();
            logger.ActualLogger = new CommonLogger();

            return appConfigurator;
        }

        private static void LogApplicationInfo(Assembly entryAssembly, LazyInitLogger logger)
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