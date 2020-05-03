using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

            //MethodInfo entryMethod = EntryPointDiscoverer.FindStaticEntryMethod(entryAssembly, entryPointFullTypeName);

            ////TODO The xml docs file name and location can be customized using <DocumentationFile> project property.
            //return await InvokeMethodAsync(args, entryMethod, xmlDocsFilePath, null, console);
            await Task.Delay(0);
            return 0;
        }
    }
}
