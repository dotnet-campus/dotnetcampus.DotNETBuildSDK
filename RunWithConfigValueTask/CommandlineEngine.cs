using System;
using System.Linq;
using System.Text.RegularExpressions;
using dotnetCampus.Configurations;

namespace dotnetCampus.RunWithConfigValueTask
{
    public static class CommandlineEngine
    {
        public static string[] FillCommandline(string[] runningCommand, DefaultConfiguration defaultConfiguration)
        {
            var regex = new Regex(@"\$\((\w+)\)");

            var commandlineWithDefaultValueRegex = new Regex(@"\$\((\w+)\)\?\?(\w*)");

            var result = runningCommand.ToArray();

            for (var i = 0; i < result.Length; i++)
            {
                var arg = result[i];

                var match = commandlineWithDefaultValueRegex.Match(arg);
                if (match.Success)
                {
                    var key = match.Groups[1].Value;
                    arg = defaultConfiguration[key] ?? match.Groups[2].Value;
                }
                else
                {
                    match = regex.Match(arg);
                    if (match.Success)
                    {
                        var key = match.Groups[1].Value;
                        arg = defaultConfiguration[key] ??
                              throw new ArgumentException($"Can not find {key} in configuration");
                    }
                }

                result[i] = arg;
            }

            return result;
        }
    }
}