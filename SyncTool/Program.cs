using dotnetCampus.Cli;
using dotnetCampus.Cli.Standard;
using SyncTool.Client;
using SyncTool.Context;
using SyncTool.Server;

using System.Net.Http.Json;

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

var httpClient = new HttpClient();
var httpResponseMessage = await httpClient.GetAsync("http://www.baidu.com");
Console.WriteLine($"完成请求");
Console.WriteLine(httpResponseMessage);

var syncFolderInfo = await httpClient.GetFromJsonAsync<SyncFolderInfo>("http://www.baidu.com");

Console.WriteLine($"完成请求 SyncFolderInfo");

try
{
    CommandLine.Parse(args)
        .AddStandardHandlers()
        .AddHandler<ServeOptions>(o => o.Run())
        .AddHandler<SyncOptions>(o => o.Run())
        .Run();
}
catch (Exception e)
{
    Console.WriteLine(e);
}