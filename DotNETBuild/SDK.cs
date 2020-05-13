using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild
{
    public static class SDK
    {
        public static IAppConfigurator Init(LogLevel logLevel = LogLevel.Information)
        {
            Log.LogLevel = logLevel;
            var appConfigurator = AppConfigurator.GetAppConfigurator();
            Log.InitFileLog();

            return appConfigurator;
        }
    }
}