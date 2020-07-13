using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using dotnetCampus.Cli;

using Walterlv.Collections;

namespace dotnetCampus.MatrixRun
{
    class Program
    {
        static int Main(string[] args)
        {
            var options = CommandLine.Parse(args).As<Options>();
            if (options.Matrix is null || options.Command is null
                || options.Matrix is null || string.IsNullOrWhiteSpace(options.Command))
            {
                WriteUsage();
                return -1;
            }
            else
            {
                return Run(options.Matrix, options.Command);
            }
        }

        private static int Run(IReadOnlyDictionary<string, string> matrix, string command)
        {
            var combination = CartesianProduct.Enumerate(matrix.ToDictionary(
                    x => x.Key,
                    x => ExtractValues(x.Value),
                    StringComparer.Ordinal))
                .ToList();
            Console.Write($"[{AppNameVersion()}] Execute commands in {combination.Count} combinations of environment variables.");

            var index = 0;
            foreach (var dictionary in combination)
            {
                index++;

                // 输出环境变量组合。
                Console.WriteLine($@"
Execute the command under the environment variable combination {index}/{combination.Count}.
  - Environment Variables:
{string.Join(Environment.NewLine, dictionary.Select(x => $"    * Matrix.{x.Key}={x.Value}"))}
  - Command:
    > {command}
");

                // 执行命令
                var exitCode = ExecuteMatrix(command, dictionary);
                if (exitCode != 0)
                {
                    return exitCode;
                }
            }
            return 0;
        }

        private static int ExecuteMatrix(string command, IEnumerable<KeyValuePair<string, string>> sectionValues)
        {
            // 设置环境变量。
            foreach (var (key, value) in sectionValues)
            {
                Environment.SetEnvironmentVariable($"Matrix.{key}", value);
            }

            // 执行命令。
            var process = Process.Start("cmd", $"/c {command}");
            process.WaitForExit();
            return process.ExitCode;
        }

        /// <summary>
        /// 把字符串 [1, 2, 3] 拆成集合 { 1, 2, 3 }。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <returns>集合。</returns>
        private static IReadOnlyList<string> ExtractValues(string value) => value.Trim(' ', '[', ']')
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        private static void WriteUsage() => Console.WriteLine(@"Usage: MatrixRun [options]

Options:
  -m|--matrix   The matrix definetion. e.g. -m Key=[value1,value2]
  -c|--command  The command that will execute using environment variables like %Matrix.Key%.");

        private static string AppNameVersion()
        {
            return $"{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? ""} {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0"}";
        }
    }
}
