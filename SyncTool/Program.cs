using dotnetCampus.Cli;
using dotnetCampus.Cli.Standard;
using SyncTool.Client;
using SyncTool.Server;

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

try
{
    CommandLine.Parse(args)
        .AddStandardHandlers()
        .AddHandler<ServeOptions>(o => o.Run())
        .AddHandler<SyncOptions>(o => o.Run())
        .Run();

    Console.Read();
}
catch (Exception e)
{
    Console.WriteLine(e);
}