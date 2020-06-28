using System;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace dotnetCampus.WriteAppVersionTask
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
                    throw new ArgumentException(
                        $"Can not find AssemblyInfoFile, try to input --AssemblyInfoFile value");
                }

                if (!Path.IsPathRooted(file))
                {
                    var codeDirectory = compileConfiguration.CodeDirectory;
                    file = Path.Combine(codeDirectory, file);
                    file = Path.GetFullPath(file);
                }

                appConfigurator.Default["AssemblyInfoFile"] = file;

                Log.Info($"assmebly info file: {file}");

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

                var appVersion = option.AppVersion;
                if (string.IsNullOrEmpty(appVersion))
                {
                    appVersion = compileConfiguration.AppVersion;
                }

                if (string.IsNullOrEmpty(appVersion))
                {
                    throw new ArgumentException("Can not find app version from command line and configuration file");
                }

                Log.Info($"app version: {appVersion}");

                // 通过 formatRegex 找到匹配的 Assembly 文件的版本号，然后修改版本号，替换为从命令行参数传入的或通过配置读取的版本号

                var content = File.ReadAllText(file);

                var match = Regex.Match(content, formatRegex);

                if (!match.Success)
                {
                    throw new ArgumentException(
                        $"Can not math VersionFormatRegex={formatRegex} in assmebly info file {file} \r\n The file content:\r\n{content}");
                }

                content = content.Replace(match.Value, match.Value.Replace(match.Groups[1].Value, appVersion));

                File.WriteAllText(file, content);

                Log.Info($"Wrote the app verion {appVersion} to assembly file {file}");
            });
        }

        public class AssmeblyOption
        {
            [Option('f', "AssemblyInfoFile", Required = true, HelpText = "The assmebly info file")]
            public string AssemblyInfoFile { set; get; }

            //version-format
            [Option('v', "AppVersion", Required = false,
                HelpText = "The version will write to assembly file. The default version will read from configurtion.")]
            public string AppVersion { set; get; }

            [Option('r', "VersionFormat", Required = false,
                HelpText = "The version format regex, default is Version = \\\"(\\d+.\\d+.\\d+)\\\";")]
            public string VersionFormatRegex { set; get; }
        }
    }
}