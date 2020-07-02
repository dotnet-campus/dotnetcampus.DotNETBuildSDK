using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using dotnetCampus.Cli;

namespace MatrixRun
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = CommandLine.Parse(args).As<Options>();
            if (options.Matrix is null || options.Command is null
                || string.IsNullOrWhiteSpace(options.Matrix) || string.IsNullOrWhiteSpace(options.Command))
            {
                WriteUsage();
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
                        ExecuteMatrix(options.Command, key, value);
                    }
                }
            }
        }

        private static void ExecuteMatrix(string command, string key, string value)
        {
            Environment.SetEnvironmentVariable($"Matrix.{key}", value);
            Process.Start("cmd", $"/c {command}");
        }

        private static void WriteUsage() => Console.WriteLine(@"Usage: MatrixRun [options]

Options:
  -m|--matrix   The matrix definetion. e.g. -m Key=[value1,value2]
  -c|--command  The command that will execute using environment variables like %Matrix.Key%.");
    }
}
