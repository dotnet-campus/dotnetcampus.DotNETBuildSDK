// See https://aka.ms/new-console-template for more information

using dotnetCampus.Cli;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;

using Packing.DebUOS;
using Packing.DebUOS.Tool;

var options = CommandLine.Parse(args).As<Options>();

if (!string.IsNullOrEmpty(options.BuildPath))
{
    var packingFolder = new DirectoryInfo(options.BuildPath);
    var outputPath = options.OutputPath ?? Path.Join(packingFolder.FullName,$"{packingFolder.Name}.deb");
    var outputDebFile = new FileInfo(outputPath);

    var debUosPackageCreator = new DebUOSPackageCreator();
    //var packingFolder = new DirectoryInfo(@"C:\lindexi\Work\");
    //var outputDebFile = new FileInfo(@"C:\lindexi\Work\Downloader.deb");
    debUosPackageCreator.PackageDeb(packingFolder, outputDebFile);
}
else if (!string.IsNullOrEmpty(options.PackageArgumentFilePath))
{
    var fileConfigurationRepo = ConfigurationFactory.FromFile(options.PackageArgumentFilePath, RepoSyncingBehavior.Static);
    var appConfigurator = fileConfigurationRepo.CreateAppConfigurator();
    var configuration = appConfigurator.Of<DebUOSConfiguration>();

    var debUosPackageCreator = new DebUOSPackageCreator();
    debUosPackageCreator.CreatePackageFolder(configuration);

    var outputFilePath = configuration.DebUOSOutputFilePath;

    var packingFolder = new DirectoryInfo(configuration.PackingFolder);
    var outputDebFile = new FileInfo(outputFilePath);

    debUosPackageCreator.PackageDeb(packingFolder, outputDebFile);
}
else
{
    // Show Help
}
