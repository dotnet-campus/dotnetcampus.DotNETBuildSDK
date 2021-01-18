using System;
using dotnetCampus.Configurations;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
    public abstract class DotNetBuildTool
    {
        protected DotNetBuildTool(IAppConfigurator appConfigurator, ILogger logger = null)
        {
            AppConfigurator = appConfigurator
                              ?? dotnetCampus.DotNETBuild.Context
                                  .AppConfigurator.GetAppConfigurator(); ;

            Logger = logger ?? AppConfigurator.Of<LogConfiguration>().GetLogger();
        }

        protected ILogger Logger { get; }
        protected IAppConfigurator AppConfigurator { get; }

        protected (bool success, string output) ExecuteCommand(string exeName, string arguments,
            string workingDirectory = "")
        {
            Logger.LogInformation($"执行命令 {exeName} {arguments}");
            (bool success, string output) = ProcessCommand.ExecuteCommand(exeName, arguments, workingDirectory);

            if (success)
            {
                Logger.LogInformation(output);
            }
            else
            {
                Logger.LogWarning(output);
            }

            return (success, output);
        }
    }
}