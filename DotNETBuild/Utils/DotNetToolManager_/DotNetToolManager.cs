#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using dotnetCampus.Configurations;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils;

public class DotNetToolManager : DotNetBuildTool
{
    public DotNetToolManager(IAppConfigurator appConfigurator, ILogger? logger = null) : base(appConfigurator, logger)
    {
    }

    public void EnsureGlobalToolInstalled(params string[] packageIds)
    {
        var dotNetToolInfoList = GetInstalledGlobalTools();
        var needInstallList = packageIds.Where(packageId => !dotNetToolInfoList.Any(t => string.Equals(t.PackageId, packageId, StringComparison.OrdinalIgnoreCase))).ToList();

        foreach (var packageId in needInstallList)
        {
            InstallOrUpdateGlobalTool(packageId);
        }
    }

    public (bool success, string output) InstallOrUpdateGlobalTool(string packageId)
    {
        Logger.LogInformation($"[InstallGlobalTool] Installing {packageId}");
        var (success, output) = ProcessCommand.ExecuteCommand("dotnet", $"tool update -g {packageId}");
        Logger.LogInformation($"[InstallGlobalTool] Success={success} Output={output}");
        return (success, output);
    }

    public bool CheckGlobalToolInstalled(string packageId)
    {
        var dotNetToolInfoList = GetInstalledGlobalTools();
        return dotNetToolInfoList.Any(t => string.Equals(t.PackageId, packageId, StringComparison.OrdinalIgnoreCase));
    }

    public List<DotNetToolInfo> GetInstalledGlobalTools()
    {
        var (success, output) = ProcessCommand.ExecuteCommand("dotnet", "tool list -g");

        var dotNetToolInfoList = new List<DotNetToolInfo>();

        if (success)
        {
            var stringReader = new StringReader(output);
            stringReader.ReadLine();
            stringReader.ReadLine();

            string? line;
            while ((line = stringReader.ReadLine()) != null)
            {
                var match = Regex.Match(line, @"([\S\.]+)\s+([\S\.]+)\s+([\S\.]+)");
                if (match.Success)
                {
                    var packageId = match.Groups[1].Value;
                    var version = match.Groups[2].Value;
                    var commands = match.Groups[3].Value;

                    dotNetToolInfoList.Add(new DotNetToolInfo(packageId, version, commands));
                }
                else
                {
                    Logger.LogWarning($"[GetInstalledGlobalTools] Can not parse installed tool. Text={line}");
                }
            }
        }
        else
        {
            Logger.LogWarning($"[GetInstalledGlobalTools] Fail. {output}");
        }

        return dotNetToolInfoList;
    }
}