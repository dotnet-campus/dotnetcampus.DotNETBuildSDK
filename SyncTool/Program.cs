// See https://aka.ms/new-console-template for more information

using System.Buffers;
using dotnetCampus.Cli;
using dotnetCampus.Cli.Standard;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using dotnetCampus.FileDownloader;
using Microsoft.AspNetCore.Mvc.Routing;
using SyncTool;

#if DEBUG
// 调试代码，用来配置客户端同步到本地的路径
var syncFolder = Path.Join(Path.GetTempPath(), $"SyncTool_{Path.GetRandomFileName()}");
var syncOptions = new SyncOptions()
{
    Address = "http://127.0.0.1:56621", // 在调试启动参数固定了 56621 当成了端口
    SyncFolder = syncFolder,
};
syncOptions.Run();
#endif

CommandLine.Parse(args)
    .AddStandardHandlers()
    .AddHandler<ServeOptions>(o => o.Run())
    .AddHandler<SyncOptions>(o => o.Run())
    .Run();

Console.WriteLine("Hello, World!");