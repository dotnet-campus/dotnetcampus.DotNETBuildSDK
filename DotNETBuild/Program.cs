using System;
using System.IO;
using System.Linq;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace dotnetCampus.DotNETBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var config = "build.fkv";
                Console.WriteLine($"命令行 " + Environment.CommandLine);

                config = Path.GetFullPath(config);
                Console.WriteLine($"配置文件 {config}");

                AppConfigurator.SetConfigurationFile(new FileInfo(config));

                var fileConfigurationRepo = ConfigurationFactory.FromFile(config);
                IAppConfigurator appConfigurator = fileConfigurationRepo.CreateAppConfigurator();

                var compiler = new Compiler(appConfigurator);
                compiler.Compile();

                if (args.Any(temp => temp == "publish"))
                {
                    compiler.Nuget.PublishNupkg();
                }
            }
            catch (Exception e)
            {
                // 可能日志出现异常，因此就不通过日志库输出
                Console.WriteLine(e);
                Log.Error(e.ToString());

                Environment.Exit(-1);
            }
        }
    }
}
