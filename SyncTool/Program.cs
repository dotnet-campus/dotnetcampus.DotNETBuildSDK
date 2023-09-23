// See https://aka.ms/new-console-template for more information

using System.Buffers;
using System.Diagnostics;
using dotnetCampus.Cli;
using dotnetCampus.Cli.Standard;

using System.Net.Sockets;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using dotnetCampus.FileDownloader;

#if DEBUG
// 调试代码，用来配置客户端同步到本地的路径
var syncFolder = Path.Join(Path.GetTempPath(),$"SyncTool_{Path.GetRandomFileName()}");
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
    .AddHandler<SyncOptions>(o =>  o.Run())
    .Run();

Console.WriteLine("Hello, World!");

/// <summary>
/// 服务端的参数
/// </summary>
[Verb("serve")]
internal class ServeOptions
{
    [Option('p', "Port")]
    public int? Port { set; get; }

    /// <summary>
    /// 同步的文件夹，不填将使用当前的工作路径
    /// </summary>
    [Option('f', "Folder")]
    public string? SyncFolder { set; get; }

    public void Run()
    {
        var syncFolder = SyncFolder;
        if (string.IsNullOrEmpty(syncFolder))
        {
            syncFolder = Environment.CurrentDirectory;
        }

        var syncFolderManager = new SyncFolderManager();
        syncFolderManager.Run(syncFolder);

        var port = Port ?? GetAvailablePort(IPAddress.Any);

        Console.WriteLine($"Listening on: http://0.0.0.0:{port} SyncFolder: {syncFolder}");

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

        builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore"] = "Debug";
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = false;
        });

        var webApplication = builder.Build();
        webApplication.MapGet("/", () => syncFolderManager.CurrentFolderInfo);
        webApplication.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(syncFolder, ExclusionFilters.System),
            ContentTypeProvider = new ContentTypeProvider(),
            ServeUnknownFileTypes = true,
            RequestPath = StaticFileConfiguration.RequestPath,
            RedirectToAppendTrailingSlash = true,
            DefaultContentType = MediaTypeNames.Application.Octet,
        });
        webApplication.Run();
    }

    public static int GetAvailablePort(IPAddress ip)
    {
        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(ip, 0));
        socket.Listen(1);
        var ipEndPoint = (IPEndPoint) socket.LocalEndPoint!;
        var port = ipEndPoint.Port;
        return port;
    }
}

/// <summary>
/// 内容类型提供器，用于让所有的文件都可以被下载
/// </summary>
class ContentTypeProvider : IContentTypeProvider
{
    public bool TryGetContentType(string subpath, out string contentType)
    {
        contentType = MediaTypeNames.Application.Octet;
        return true;
    }
}

/// <summary>
/// 静态文件配置
/// </summary>
static class StaticFileConfiguration
{
    public const string RequestPath = "/File";
}

record SyncFolderInfo(ulong Version, List<SyncFileInfo> SyncFileList)
{

}

record SyncFileInfo(string RelativePath, long FileSize, DateTime LastWriteTimeUtc)
{

}

class SyncFolderManager
{
    public SyncFolderInfo? CurrentFolderInfo { get; private set; }

    public void Run(string watchFolder)
    {
        UpdateChange(watchFolder);

        var fileSystemWatcher = new FileSystemWatcher(watchFolder, "*");
        fileSystemWatcher.EnableRaisingEvents = true;
        fileSystemWatcher.Changed += (sender, args) =>
        {
            UpdateChangeInner();
        };
        fileSystemWatcher.Created += (sender, args) =>
        {
            UpdateChangeInner();
        };
        fileSystemWatcher.Deleted += (sender, args) =>
        {
            UpdateChangeInner();
        };
        fileSystemWatcher.Renamed += (sender, args) =>
        {
            UpdateChangeInner();
        };

        void UpdateChangeInner()
        {
            UpdateChange(watchFolder);
        }
    }

    private ulong _currentVersion;

    private void UpdateChange(string watchFolder)
    {
        var currentVersion = Interlocked.Increment(ref _currentVersion);

        Task.Run(async () =>
        {
            // 等待一段时间，防止连续变更导致不断刷新
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            if (!Enable())
            {
                return;
            }

            try
            {
                var syncFileList = new List<SyncFileInfo>();

                foreach (var file in Directory.EnumerateFiles(watchFolder, "*", SearchOption.AllDirectories))
                {
                    if (!Enable())
                    {
                        return;
                    }

                    var fileInfo = new FileInfo(file);
                    var relativePath = Path.GetRelativePath(watchFolder, file);
                    var syncFileInfo = new SyncFileInfo(relativePath, fileInfo.Length, fileInfo.LastWriteTimeUtc);
                    syncFileList.Add(syncFileInfo);
                }

                CurrentFolderInfo = new SyncFolderInfo(currentVersion, syncFileList);
            }
            catch (IOException e)
            {
                // 可以忽略，因为可以在读取文件时，文件被删掉
                Debug.WriteLine(e);
            }
        });

        bool Enable() => Interlocked.Read(ref _currentVersion) == currentVersion;
    }
}

/// <summary>
/// 客户端的同步命令行参数
/// </summary>

[Verb("sync")]
internal class SyncOptions
{
    [Option('a', "Address")]
    public string? Address { set; get; }

    [Option('f', "Folder")]
    public string? SyncFolder { set; get; }

    public async void Run()
    {
        if (string.IsNullOrEmpty(Address))
        {
            Console.WriteLine($"找不到同步地址，请确保传入 Address 参数");
            return;
        }
        
        var syncFolder = SyncFolder;
        if (string.IsNullOrEmpty(syncFolder))
        {
            syncFolder = Environment.CurrentDirectory;
        }

        Directory.CreateDirectory(syncFolder);

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Address);

        // 记录本地的字典值
        var syncFileDictionary = new Dictionary<string/*RelativePath*/, SyncFileInfo>();
        foreach (var file in Directory.EnumerateFiles(syncFolder, "*", SearchOption.AllDirectories))
        {
            var fileInfo = new FileInfo(file);
            var relativePath = Path.GetRelativePath(syncFolder, file);
            var syncFileInfo = new SyncFileInfo(relativePath, fileInfo.Length, fileInfo.LastWriteTimeUtc);
            syncFileDictionary[relativePath] = syncFileInfo;
        }

        ulong currentVersion = 0;
        while (true)
        {
            try
            {
                var syncFolderInfo = await httpClient.GetFromJsonAsync<SyncFolderInfo>("/");
                if (syncFolderInfo is null || syncFolderInfo.Version == currentVersion)
                {
                    continue;
                }
                currentVersion = syncFolderInfo.Version;
                await SyncFolderAsync(syncFolderInfo.SyncFileList, currentVersion);
            }
            catch (Exception e)
            {
                // 大不了下次再继续
            }
        }

        async Task SyncFolderAsync(List<SyncFileInfo> remote, ulong version)
        {
            Dictionary<string, SyncFileInfo> local = syncFileDictionary;
            // 记录已经更新的 RelativePath 哈希，用来记录哪些已被删除
            var updatedList = new HashSet<string>();

            foreach (var remoteSyncFileInfo in remote)
            {
                // ReSharper disable AccessToModifiedClosure
                if (version != currentVersion)
                {
                    return;
                }

                var relativePath = remoteSyncFileInfo.RelativePath;
                relativePath = relativePath.Replace('\\', '/');
                var encode = UrlEncoder.Create().Encode(relativePath);
                var downloadUrl = $"{StaticFileConfiguration.RequestPath}/{encode}";

                // 先下载到一个新的文件，然后再重命名替换
                // 如果原本的文件正在被占用，那失败的只有重命名部分，而不会导致重复下载
                // 那如果下载失败呢？大概需要重新开始同步了
                var downloadFilePath = Path.Join(syncFolder,$"{remoteSyncFileInfo.RelativePath}_{Path.GetRandomFileName()}");
                await DownloadFile(downloadFilePath, downloadUrl);

                if (File.Exists(downloadFilePath))
                {
                    // 下载失败了？理论上不会进入此分支
                    // 重新等待下一次
                    return;
                }

                while(true)
                {
                    if (version != currentVersion)
                    {
                        return;
                    }

                    try
                    {
                       
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }

        async Task DownloadFile(string downloadFilePath, string downloadUrl)
        {
            await using var stream = await httpClient.GetStreamAsync(downloadUrl);
            await using var fileStream = File.Create(downloadFilePath);
            await stream.CopyToAsync(fileStream);
        }
    }
}