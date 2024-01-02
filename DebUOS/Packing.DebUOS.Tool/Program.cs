// See https://aka.ms/new-console-template for more information

using dotnetCampus.Cli;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;

using Packing.DebUOS;
using Packing.DebUOS.Tool;

var options = CommandLine.Parse(args).As<Options>();

if (!string.IsNullOrEmpty(options.BuildPath))
{

}
else if (!string.IsNullOrEmpty(options.PackageArgumentFilePath))
{

}
else
{
    // Show Help
}

//var argsFilePath = args[0];
//var fileConfigurationRepo = ConfigurationFactory.FromFile(argsFilePath,RepoSyncingBehavior.Static);
//var appConfigurator = fileConfigurationRepo.CreateAppConfigurator();

var debUosPackageCreator = new DebUOSPackageCreator();
debUosPackageCreator.PackageDeb(new DirectoryInfo(@"C:\lindexi\Work\"),
    new FileInfo(@"C:\lindexi\Work\Downloader.deb"));

Console.Read();