// See https://aka.ms/new-console-template for more information

using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;

using Packing.DebUOS;

//var argsFilePath = args[0];
//var fileConfigurationRepo = ConfigurationFactory.FromFile(argsFilePath,RepoSyncingBehavior.Static);
//var appConfigurator = fileConfigurationRepo.CreateAppConfigurator();

var debUosPackageCreator = new DebUOSPackageCreator();
debUosPackageCreator.PackageDeb(new DirectoryInfo(@"C:\lindexi\Work\"),
    new FileInfo(@"C:\lindexi\Work\Downloader.deb"));

Console.Read();