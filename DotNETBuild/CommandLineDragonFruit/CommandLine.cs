using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using dotnetCampus.Configurations.Core;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

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

            SDK.Init(LogLevel.Error);
            Log.LogLevel = LogLevel.Info;

            var returnObj = 0;
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

            var currentConfiguration = ConfigurationHelper.GetCurrentConfiguration();
            if (currentConfiguration is FileConfigurationRepo fileConfiguration)
            {
                await fileConfiguration.SaveAsync();
            }

            return returnObj;
        }
    }
}
