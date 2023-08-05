using System.Reflection;
#if NETCOREAPP3_1
using System.Runtime.InteropServices.WindowsRuntime;
#endif
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class AssemblyHelper
    {
        /// <summary>
        /// 填充程序集信息，包括版本号
        /// </summary>
        /// <returns></returns>
        public static IAppConfigurator FillAssemblyInfo(this IAppConfigurator appConfigurator, Assembly assembly)
        {
            var compileConfiguration = appConfigurator.Of<CompileConfiguration>();

            var assemblyInformationalVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (string.IsNullOrEmpty(compileConfiguration.AppVersion))
            {
                compileConfiguration.AppVersion = assemblyInformationalVersionAttribute!.InformationalVersion;
            }

            if (string.IsNullOrEmpty(compileConfiguration.AssemblyVersion))
            {
                compileConfiguration.AssemblyVersion = assemblyInformationalVersionAttribute!.InformationalVersion;
            }

            return appConfigurator;
        }
    }
}