using System.Collections.Generic;
using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 命令行参数到配置的扩展
    /// </summary>
    public static class CommandLineArgsToConfiguration
    {
        /// <summary>
        /// 添加命令行参数到配置
        /// </summary>
        /// <param name="appConfigurator"></param>
        /// <param name="args"></param>
        /// <param name="switchMapping">命令行参数到配置的映射表</param>
        public static void AddCommandLine(this IAppConfigurator appConfigurator, string[] args,
            Dictionary<string, string> switchMapping)
        {
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.Length <= 1) continue;
                var argName = TryGetArgName(switchMapping, arg);

                if (string.IsNullOrEmpty(argName)) continue;

                var argValue = "true";

                if (i < args.Length - 1)
                {
                    var nextArg = args[i + 1];
                    if (nextArg.Length <= 1 || nextArg[0] != '-')
                    {
                        argValue = nextArg;
                    }
                }

                appConfigurator.Default[argName] = argValue;
            }
        }

        private static string TryGetArgName(Dictionary<string, string> switchMapping, string arg)
        {
            if (arg[0] != '-') return string.Empty;

            if (arg[1] == '-')
            {
                return arg.Substring(2);
            }

            if (switchMapping.TryGetValue(arg, out var map))
            {
                if (map.StartsWith("--"))
                {
                    return map.Substring(2);
                }

                return map;
            }

            return string.Empty;
        }
    }
}