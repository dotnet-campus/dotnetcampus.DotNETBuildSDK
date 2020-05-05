using System;
using System.IO;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace NuGetPublishTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfigurator = AppConfigurator.GetAppConfigurator();
            var nuGet = new NuGet(appConfigurator);

            Log.Info($"当前命令行 {Environment.CommandLine}");

            if (args.Length == 1)
            {
                // 可以传入命令行或文件
                if (Directory.Exists(args[0]))
                {
                    Log.Info($"传入 NuGet 文件所在文件夹 {args[0]}");
                    appConfigurator.Of<CompileConfiguration>().NupkgDirectory = args[0];
                    nuGet.PublishNupkg();
                }
                else if (File.Exists(args[0]))
                {
                    Log.Info($"传入 NuGet 文件 {args[0]}");
                    nuGet.PublishNupkg(new FileInfo(args[0]));
                }
                else if (args[0] == "-h" || args[0] == "--help")
                {
                    Console.WriteLine(@"此命令用于将 NuGet 包发布到配置的默认源，默认将会取 Compile.NupkgDirectory 文件夹内的 nupkg 文件上传到 Nuget.Source 服务器
命令可不填写参数，不填写时将会使用配置的 Compile.NupkgDirectory 文件夹内的所有 nupkg 文件上传
命令可选参数是 nupkg 文件路径或文件所在文件夹

NuGetPublishTask [nupkg file path | nupkg folder]

NuGetPublishTask [nupkg 文件路径 | nupkg 文件所在文件夹]

如需指定非配置里面的 NuGet 服务器等，请直接使用 nuget 命令");
                    return;
                }
                else
                {
                    Log.Error($"未能解析传入内容为 NuGet 文件或所在文件夹");
                    Environment.Exit(-1);
                    return;
                }
            }
            else if (args.Length == 0)
            {
                nuGet.PublishNupkg();
            }
            else
            {
                Log.Error("此命令仅接受传入当前 NuGet 包文件路径或所在文件夹，此命令用于将 NuGet 包发布到配置的默认源");
                Environment.Exit(-1);
                return;
            }
        }
    }
}
