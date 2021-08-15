using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.CommandLineDragonFruit
{
    public static class CommandLine
    {
        /// <summary>
        /// Finds and executes 'Program.Main', but with strong types.
        /// </summary>
        /// <param name="entryAssembly">The entry assembly</param>
        /// <param name="args">The string arguments.</param>
        /// <param name="entryPointFullTypeName">Explicitly defined entry point</param>
        /// <param name="xmlDocsFilePath">Explicitly defined path to xml file containing XML Docs</param>
        /// <returns>The exit code.</returns>
        public static async Task<int> ExecuteAssemblyAsync(
            Assembly entryAssembly,
            string[] args,
            string entryPointFullTypeName,
            string xmlDocsFilePath = null)
        {
            if (entryAssembly == null)
            {
                throw new ArgumentNullException(nameof(entryAssembly));
            }

            args = args ?? Array.Empty<string>();
            entryPointFullTypeName = entryPointFullTypeName?.Trim();

            MethodInfo entryMethod = EntryPointDiscoverer.FindStaticEntryMethod(entryAssembly, entryPointFullTypeName);

            ////TODO The xml docs file name and location can be customized using <DocumentationFile> project property.
            //return await InvokeMethodAsync(args, entryMethod, xmlDocsFilePath, null, console);

            SDK.Init();
            var currentConfiguration = ConfigurationHelper.GetCurrentConfiguration();
            // 全局可以配置日志输出
            var appConfigurator = currentConfiguration.CreateAppConfigurator();
            Log.LogLevel = appConfigurator.Of<LogConfiguration>().LogLevel;

            SetCommonConfiguration(appConfigurator);

            // 完成框架，重新设置一下日志
            Log.Logger.SwitchActualLogger();

            // 下面代码调用实际上代码里面的
            var returnObj = 0;

            try
            {
                var obj = entryMethod.Invoke(null, new[] { args });
                if (obj is Task task)
                {
                    await task;
                }
                else if (obj is Task<int> taskObj)
                {
                    await taskObj;
                    returnObj = taskObj.Result;
                }
                else if (obj is int n)
                {
                    returnObj = n;
                }
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

            return returnObj;
        }

        private static void SetCommonConfiguration(IAppConfigurator appConfigurator)
        {
            var compileConfiguration = appConfigurator.Of<CompileConfiguration>();

            compileConfiguration.SetCommonConfiguration();
        }
    }
}
