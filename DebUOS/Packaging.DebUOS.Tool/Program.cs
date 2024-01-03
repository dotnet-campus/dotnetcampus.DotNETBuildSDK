// See https://aka.ms/new-console-template for more information

using dotnetCampus.Cli;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Packaging.DebUOS;
using Packaging.DebUOS.Contexts.Configurations;
using Packaging.DebUOS.Tool;
using Packing.DebUOS;

var options = CommandLine.Parse(args).As<Options>();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(simpleConsoleFormatterOptions =>
    {
        simpleConsoleFormatterOptions.ColorBehavior = LoggerColorBehavior.Disabled;
        simpleConsoleFormatterOptions.SingleLine = true;
    });
});

var logger = loggerFactory.CreateLogger("");

if (!string.IsNullOrEmpty(options.BuildPath))
{
    var packingFolder = new DirectoryInfo(options.BuildPath);
    var outputPath = options.OutputPath ?? Path.Join(packingFolder.FullName,$"{packingFolder.Name}.deb");
    var outputDebFile = new FileInfo(outputPath);

    var debUosPackageCreator = new DebUOSPackageCreator(logger);
    //var packingFolder = new DirectoryInfo(@"C:\lindexi\Work\");
    //var outputDebFile = new FileInfo(@"C:\lindexi\Work\Downloader.deb");
    debUosPackageCreator.PackageDeb(packingFolder, outputDebFile);
}
else if (!string.IsNullOrEmpty(options.PackageArgumentFilePath))
{
    var fileConfigurationRepo = ConfigurationFactory.FromFile(options.PackageArgumentFilePath, RepoSyncingBehavior.Static);
    var appConfigurator = fileConfigurationRepo.CreateAppConfigurator();
    var configuration = appConfigurator.Of<DebUOSConfiguration>();

    var fileStructCreator = new DebUOSPackageFileStructCreator(logger);
    fileStructCreator.CreatePackagingFolder(configuration);

    var packingFolder = new DirectoryInfo(configuration.PackingFolder!);
    var outputDebFile = new FileInfo(configuration.DebUOSOutputFilePath!);
    var workingFolder = new DirectoryInfo(configuration.WorkingFolder!);

    var debUosPackageCreator = new DebUOSPackageCreator(logger);
    debUosPackageCreator.PackageDeb(packingFolder, outputDebFile,workingFolder);
}
else
{
    // Show Help
}
