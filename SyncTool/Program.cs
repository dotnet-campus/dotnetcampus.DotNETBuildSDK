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
using dotnetCampus.FileDownloader;

#if DEBUG

var syncFolder = Path.Join(Path.GetTempPath(),$"SyncTool_{Path.GetRandomFileName()}");
var syncOptions = new SyncOptions()
{
    Address = "http://127.0.0.1:56621",
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

class ContentTypeProvider : IContentTypeProvider
{
    public bool TryGetContentType(string subpath, out string contentType)
    {
        contentType = MediaTypeNames.Application.Octet;
        return true;
    }
}

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

                await SyncFolderAsync(syncFolderInfo.SyncFileList);
            }
            catch (Exception e)
            {
                // 大不了下次再继续
            }
        }

        async Task SyncFolderAsync(List<SyncFileInfo> remote)
        {
            Dictionary<string, SyncFileInfo> local = syncFileDictionary;
            // 记录已经更新的 RelativePath 哈希，用来记录哪些已被删除
            var updatedList = new HashSet<string>();

            foreach (var remoteSyncFileInfo in remote)
            {
               
            }
        }
    }
}