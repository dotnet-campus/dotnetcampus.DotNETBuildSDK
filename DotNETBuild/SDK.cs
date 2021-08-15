using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild
{
    internal static class SDK
    {
        public static IAppConfigurator Init(LogLevel logLevel = LogLevel.Information)
        {
            Log.LogLevel = logLevel;
            var logger = Log.InitLazyLogger();

            var appConfigurator = AppConfigurator.InitAppConfigurator();
            logger.ActualLogger = new CommonLogger();

            return appConfigurator;
        }
    }
}