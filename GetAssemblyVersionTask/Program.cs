using System;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace GetAssemblyVersionTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfigurator = AppConfigurator.GetAppConfigurator();
            var compileConfiguration = appConfigurator.Of<CompileConfiguration>();
            Parser.Default.ParseArguments<AssmeblyOption>(args).WithParsed(option =>
            {
                var file = option.AssemblyInfoFile;
                file = Path.GetFullPath(file);

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

                Log.Info($"VersionFormatRegex={formatRegex}");

                var content = File.ReadAllText(file);
                var match = Regex.Match(content,formatRegex);
                if (match.Success)
                {
                    var assemblyVersion = match.Groups[1].Value;

                    Log.Info($"assembly version: {assemblyVersion}");

                    var lastVersion = 0;
                    var gitConfiguration = appConfigurator.Of<GitConfiguration>();
                    if (gitConfiguration.GitCount != null)
                    {
                        Log.Info($"GitCount: {gitConfiguration.GitCount}");
                        lastVersion = gitConfiguration.GitCount.Value;
                    }

                    var appVersion = $"{assemblyVersion}.{lastVersion}";
                    Log.Info($"app version: {appVersion}");
                    compileConfiguration.AppVersion = appVersion;
                }
                else
                {
                    throw new ArgumentException($"Can not math VersionFormatRegex={formatRegex} in assmebly info file {file} \r\n The file content:\r\n{content}");
                }
            });

        }

        /// <summary>
        /// 编译配置
        /// </summary>
        public class CompileConfiguration : Configuration
        {
            /// <inheritdoc />
            public CompileConfiguration() : base("")
            {
            }

            /// <summary>
            /// 版本号
            /// </summary>
            public string AppVersion
            {
                set => SetValue(value);
                get => GetString();
            }
        }
    }

    public class AssmeblyOption
    {
        [Option('f', "AssemblyInfoFile", Required = true, HelpText = "The assmebly info file")]
        public string AssemblyInfoFile { set; get; }

        //version-format
        [Option('r', "VersionFormat", Required = false, HelpText = "The version format regex, default is Version = \\\"(\\d+.\\d+.\\d+)\\\";")]
        public string VersionFormatRegex { set; get; }
    }
}
