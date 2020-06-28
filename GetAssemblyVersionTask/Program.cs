using System;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace dotnetCampus.GetAssemblyVersionTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AssmeblyOption>(args).WithParsed(option =>
            {
                var appConfigurator = AppConfigurator.GetAppConfigurator();
                var compileConfiguration = appConfigurator.Of<CompileConfiguration>();

#if DEBUG
                var fileSniff = new FileSniff(appConfigurator);
                fileSniff.Sniff();
#endif

                var file = option.AssemblyInfoFile;

                if (string.IsNullOrEmpty(file))
                {
                    file = appConfigurator.Default["AssemblyInfoFile"];
                }

                if (string.IsNullOrEmpty(file))
                {
                    throw new ArgumentException($"Can not find AssemblyInfoFile, try to input --AssemblyInfoFile value");
                }

                if (!Path.IsPathRooted(file))
                {
                    var codeDirectory = compileConfiguration.CodeDirectory;
                    file = Path.Combine(codeDirectory, file);
                    file = Path.GetFullPath(file);
                }

                appConfigurator.Default["AssemblyInfoFile"] = file;

                Log.Info($"Start read assmebly info file {file}");

                if (!File.Exists(file))
                {
                    throw new ArgumentException($"The assmebly info file {file} can not be found.");
                }

                var formatRegex = option.VersionFormatRegex;
                if (string.IsNullOrEmpty(formatRegex))
                {
                    formatRegex = "Version = \\\"(\\d+.\\d+.\\d+)\\\";";
                }

                Log.Info($"VersionFormatRegex: {formatRegex}");

                var content = File.ReadAllText(file);
                var match = Regex.Match(content, formatRegex);
                if (match.Success)
                {
                    var assemblyVersion = match.Groups[1].Value;
                    var fieldCount = GetVersionFieldCount(assemblyVersion);

                    Log.Info($"assembly version: {assemblyVersion}");
                    appConfigurator.Default["AssemblyVersion"] = assemblyVersion;

                    var lastVersion = 0;
                    var gitConfiguration = appConfigurator.Of<GitConfiguration>();
                    if (fieldCount == 3 && gitConfiguration.GitCount != null)
                    {
                        Log.Info($"GitCount: {gitConfiguration.GitCount}");
                        lastVersion = gitConfiguration.GitCount.Value;
                    }

                    var appVersion = fieldCount == 3
                        ? $"{assemblyVersion}.{lastVersion}"
                        : assemblyVersion;
                    Log.Info($"app version: {appVersion}");
                    compileConfiguration.AppVersion = appVersion;
                }
                else
                {
                    throw new ArgumentException($"Can not math VersionFormatRegex={formatRegex} in assmebly info file {file} \r\n The file content:\r\n{content}");
                }
            });
        }

        private static int GetVersionFieldCount(string originalString)
        {
            if (Version.TryParse(originalString, out var version))
            {
                try
                {
                    version.ToString(4);
                    return 4;
                }
                catch (Exception)
                {
                    try
                    {
                        version.ToString(3);
                        return 3;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            version.ToString(2);
                            return 2;
                        }
                        catch (Exception)
                        {
                            return 1;
                        }
                    }
                }
            }
            else
            {
                return 0;
            }
        }
    }

    public class AssmeblyOption
    {
        [Option('f', "AssemblyInfoFile", Required = false, HelpText = "The assmebly info file")]
        public string AssemblyInfoFile { set; get; }

        [Option('r', "VersionFormat", Required = false, HelpText = "The version format regex, default is Version = \\\"(\\d+.\\d+.\\d+)\\\";")]
        public string VersionFormatRegex { set; get; }
    }
}
