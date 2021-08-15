using System;
using System.Reflection;
using System.Threading.Tasks;

namespace dotnetCampus.DotNETBuild.CommandLineDragonFruit
{
    /// <summary>
    /// 命令行辅助类，从 DragonFruit 命令行抄的代码，将被 build\package.targets 里面生成的主函数调用
    /// </summary>
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

            return await SDK.Run(ExecuteAssemblyInnerAsync(entryAssembly, args, entryPointFullTypeName));
        }

        private static Func<Task<int>> ExecuteAssemblyInnerAsync(Assembly entryAssembly, string[] args, string entryPointFullTypeName)
        {
            return async () =>
            {
                // 下面代码调用实际上代码里面的
                var returnObj = 0;

                args = args ?? Array.Empty<string>();
                entryPointFullTypeName = entryPointFullTypeName?.Trim();

                MethodInfo entryMethod = EntryPointDiscoverer.FindStaticEntryMethod(entryAssembly, entryPointFullTypeName);

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

                return returnObj;
            };
        }
    }
}
