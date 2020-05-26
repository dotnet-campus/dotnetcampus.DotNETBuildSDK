using System;
using System.Collections.Generic;
using System.Text;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace DotNETTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfigurator = AppConfigurator.GetAppConfigurator();
            var compileConfiguration = appConfigurator.Of<CompileConfiguration>();

            // 用于替换的列表
            var replaceDictionary = new Dictionary<string, string>()
            {
                { "$(AppVersion)", compileConfiguration.AppVersion }
            };

            // 替换一些变量，然后原原本本传入到 dotnet 命令里面
            var argsString = new StringBuilder();
            foreach (var arg in args)
            {
                var temp = arg;
                if (replaceDictionary.TryGetValue(temp, out var replaceValue))
                {
                    temp = replaceValue;
                }

                temp = ProcessCommand.ToArgumentPath(temp);

                argsString.Append(temp);
                argsString.Append(' ');
            }

            Log.Info($"dotnet {argsString}");
            var (success, output) = ProcessCommand.ExecuteCommand("dotnet", argsString.ToString());
            if (success)
            {
                Log.Info(output);
            }
            else
            {
                Log.Error(output);
            }
        }
    }
}