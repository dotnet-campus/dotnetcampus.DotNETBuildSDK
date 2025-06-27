// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Text;
using DotNetCampus.Cli;
using DotNetCampus.Cli.Compiler;
using dotnetCampus.Configurations.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Packaging.DebUOS;
using Packaging.DebUOS.Contexts.Configurations;
using Packaging.DebUOS.Tool;

var options = CommandLine.Parse(args).As<Options>();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(loggerOptions => loggerOptions.FormatterName = MSBuildFormatter.FormatterName);
    builder.AddConsoleFormatter<MSBuildFormatter, ConsoleFormatterOptions>();
});

var logger = loggerFactory.CreateLogger("");

try
{
    if (!string.IsNullOrEmpty(options.BuildPath))
    {
        var packingFolder = new DirectoryInfo(options.BuildPath);
        var outputPath = options.OutputPath ?? Path.Join(packingFolder.FullName, $"{packingFolder.Name}.deb");
        var outputDebFile = new FileInfo(outputPath);

        var debUosPackageCreator = new DebUOSPackageCreator(logger);
        //var packingFolder = new DirectoryInfo(@"C:\lindexi\Work\");
        //var outputDebFile = new FileInfo(@"C:\lindexi\Work\Downloader.deb");
        debUosPackageCreator.PackageDeb(packingFolder, outputDebFile);
    }
    else if (!string.IsNullOrEmpty(options.PackageArgumentFilePath))
    {
        logger.LogInformation($"开始根据配置创建 UOS 的 deb 包。配置文件：{options.PackageArgumentFilePath}");
        if (!File.Exists(options.PackageArgumentFilePath))
        {
            logger.LogError($"配置文件 '{options.PackageArgumentFilePath}' 不存在");
            return;
        }

        var fileConfigurationRepo =
            ConfigurationFactory.FromFile(options.PackageArgumentFilePath, RepoSyncingBehavior.Static);
        var appConfigurator = fileConfigurationRepo.CreateAppConfigurator();
        var configuration = appConfigurator.Of<DebUOSConfiguration>();

        var fileStructCreator = new DebUOSPackageFileStructCreator(logger);
        fileStructCreator.CreatePackagingFolder(configuration);

        var packingFolder = new DirectoryInfo(configuration.PackingFolder!);
        var outputDebFile = new FileInfo(configuration.DebUOSOutputFilePath!);
        var workingFolder = new DirectoryInfo(configuration.WorkingFolder!);
        var excludePackingDebFileExtensionsPredicate = configuration.ToExcludePackingDebFileExtensionsPredicate();

        var debUosPackageCreator = new DebUOSPackageCreator(logger);
        debUosPackageCreator.PackageDeb(packingFolder, outputDebFile, workingFolder,
            optFileCanIncludePredicate: entry => /*取反，因为配置是不包括*/ !excludePackingDebFileExtensionsPredicate(entry));
    }
    else
    {
        // Show Help
        var stringBuilder = new StringBuilder()
            .AppendLine($"用法：[options] [arguments]");
        foreach (var propertyInfo in typeof(Options).GetProperties())
        {
            var optionAttribute = propertyInfo.GetCustomAttribute<OptionAttribute>();
            if (optionAttribute != null)
            {
                stringBuilder.AppendLine(
                    $"-{optionAttribute.ShortName}  {(optionAttribute.LongName ?? string.Empty).PadRight(10)} {optionAttribute.Description} {optionAttribute.LocalizableDescription}");
            }
        }

        Console.WriteLine(stringBuilder.ToString());
    }
}
catch (Exception e)
{
    logger.LogError(e, "Fail.");
}

class MSBuildFormatter : ConsoleFormatter
{
    public MSBuildFormatter() : base(FormatterName)
    {
    }

    public const string FormatterName = "MSBuild";

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var logLevel = logEntry.LogLevel;
        //var eventId = logEntry.EventId.Id;
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        var exception = logEntry.Exception;
        var logLevelString = logLevel switch
        {
            LogLevel.Trace => "debug: ",
            LogLevel.Debug => "debug: ",
            LogLevel.Information => "info: ",
            LogLevel.Warning => "warning: ",
            LogLevel.Error => "error: ",
            LogLevel.Critical => "error: ",
            _ => "",
        };
        textWriter.Write($"{logLevelString}[DebUOS] {message}");
        if (exception != null)
        {
            textWriter.WriteLine(exception);
        }
        else
        {
            textWriter.WriteLine();
        }
    }
}