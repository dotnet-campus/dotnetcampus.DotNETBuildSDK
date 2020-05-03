using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace dotnetCampus.DotNETBuild
{
    public static class SDK
    {
        public static IAppConfigurator Init()
        {
            var appConfigurator = AppConfigurator.GetAppConfigurator();
            Log.InitFileLog();
            return appConfigurator;
        }
    }
}