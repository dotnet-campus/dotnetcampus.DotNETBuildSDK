using System;
using System.Text;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace dotnetCampus.RunWithConfigValueTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfigurator = AppConfigurator.GetAppConfigurator();
            var appConfiguratorDefault = appConfigurator.Default;

            var (ownerCommand, runningCommand) = SubCommandParser.ParseCommandlineValue(args);

            if (runningCommand.Count == 0)
            {
                return;
            }

            // 其中的 OwnerCommand 先忽略
            // 判断 runningCommand 是否存在需要转换的参数
            var actualCommandline = CommandlineEngine.FillCommandline(runningCommand.ToArray(), appConfiguratorDefault);

            var commandlineArgString = new StringBuilder();
            for (var i = 1; i < actualCommandline.Length; i++)
            {
                var arg = actualCommandline[i];
                Console.WriteLine($"[{i}] = {arg}");
                commandlineArgString.Append(ProcessCommand.ToArgumentPath(arg));
                commandlineArgString.Append(' ');
            }

            var arguments = commandlineArgString.ToString();
            Console.WriteLine($"Command = {actualCommandline[0]} {arguments}");
            var (success, output) = ProcessCommand.ExecuteCommand(actualCommandline[0], arguments);
            Console.WriteLine(output);
        }
    }
}
