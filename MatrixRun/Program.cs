using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using dotnetCampus.Cli;

namespace dotnetCampus.MatrixRun
{
    class Program
    {
        static int Main(string[] args)
        {
            var options = CommandLine.Parse(args).As<Options>();
            if (options.Matrix is null || options.Command is null
                || string.IsNullOrWhiteSpace(options.Matrix) || string.IsNullOrWhiteSpace(options.Command))
            {
                WriteUsage();
                return -1;
            }
            else
            {
                var matrixMatch = Regex.Match(options.Matrix, @"(\w+)=\[(.*)\]");
                if (matrixMatch.Success)
                {
                    var key = matrixMatch.Groups[1].Value;
                    var values = matrixMatch.Groups[2].Value
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrWhiteSpace(x));
                    foreach (var value in values)
                    {
                        var exitCode = ExecuteMatrix(options.Command, key, value);
                        if (exitCode != 0)
                        {
                            return exitCode;
                        }
                    }
                    return 0;
                }
                else
                {
                    Console.WriteLine($@"Matrix option {options.Matrix} is not recognized. The correct format for ""Matrix.Key"" should be:
  Key=[value1,value2]");
                    return -2;
                }
            }
        }

        private static int ExecuteMatrix(string command, string key, string value)
        {
            Console.WriteLine(@$"Execute Command with Environment Variables ""Matrix.{key}={value}"":
  {command}");
            Environment.SetEnvironmentVariable($"Matrix.{key}", value);
            var process = Process.Start("cmd", $"/c {command}");
            process.WaitForExit();
            return process.ExitCode;
        }

        private static void WriteUsage() => Console.WriteLine(@"Usage: MatrixRun [options]

Options:
  -m|--matrix   The matrix definetion. e.g. -m Key=[value1,value2]
  -c|--command  The command that will execute using environment variables like %Matrix.Key%.");
    }
}
