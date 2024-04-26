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

            // 优先从 AssemblyVersionAttribute 取出，解决 [dotnet 8 破坏性改动 在 AssemblyInformationalVersionAttribute 添加上 git 的 commit 号](https://blog.lindexi.com/post/dotnet-8-%E7%A0%B4%E5%9D%8F%E6%80%A7%E6%94%B9%E5%8A%A8-%E5%9C%A8-AssemblyInformationalVersionAttribute-%E6%B7%BB%E5%8A%A0%E4%B8%8A-git-%E7%9A%84-commit-%E5%8F%B7.html )
            // 
            if (string.IsNullOrEmpty(compileConfiguration.AppVersion))
            {
                var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyVersionAttribute>();
                if (assemblyVersionAttribute != null)
                {
                    compileConfiguration.AppVersion = assemblyVersionAttribute.Version;
                }
                else
                {
                    var assemblyFileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                    if (assemblyFileVersionAttribute != null)
                    {
                        compileConfiguration.AppVersion = assemblyFileVersionAttribute.Version;
                    }
                }
            }

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