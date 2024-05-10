using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Net.Sockets;

using dotnetCampus.Cli;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using SyncTool.Configurations;
using SyncTool.Context;

namespace SyncTool.Server;

/// <summary>
/// 服务端的参数
/// </summary>
[Verb("serve")]
internal class ServeOptions
{
    /// <summary>
    /// 开放监听的端口。不填将会自己随便找一个可用端口
    /// </summary>
    [Option('p', "Port")]
    public int? Port { set; get; }

    /// <summary>
    /// 同步的文件夹，不填将使用当前的工作路径
    /// </summary>
    [Option('f', "Folder")]
    public string? SyncFolder { set; get; }

    public async Task Run()
    {
        var syncFolder = SyncFolder;
        if (string.IsNullOrEmpty(syncFolder))
        {
            syncFolder = Environment.CurrentDirectory;
        }

        syncFolder = Path.GetFullPath(syncFolder);

        var syncFolderManager = new SyncFolderManager();
        syncFolderManager.Run(syncFolder);

        var port = Port ?? GetAvailablePort(IPAddress.Any);

        Console.WriteLine($"Listening on: http://0.0.0.0:{port}");
        try
        {
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var unicastIpAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    Console.WriteLine($"Listening on: http://{unicastIpAddressInformation.Address.ToString()}:{port}");
                }
            }
        }
        catch
        {
            // 忽略异常，只是为了方便开发者了解当前的网络信息，不用每次都去看自己内网地址
        }
        Console.WriteLine($"SyncFolder: {syncFolder}");

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls($"http://*:{port}");

        builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore"] = "Debug";
        builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore.Routing.EndpointMiddleware"] = "Warning";
        builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore.Server.Kestrel.Connections"] = "Warning";
        builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware"] = "Warning";
        builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore.Hosting.Diagnostics"] = "Warning";
        builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore.Routing.Matching.DfaMatcher"] = "Warning";
        builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware"] = "Warning";
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = false;
        });

        var webApplication = builder.Build();
        webApplication.MapGet("/", () => syncFolderManager.CurrentFolderInfo);
        webApplication.MapPost("/", async ([FromBody] QueryFileStatusRequest request, [FromServices] ILogger<ServeOptions> logger) =>
        {
            var taskCompletionSource = new TaskCompletionSource();
            syncFolderManager.CurrentFolderInfoChanged += OnCurrentFolderInfoChanged;

            try
            {
                // 如果没有更新，则进入等待，不立刻返回客户端
                if (syncFolderManager.CurrentFolderInfo == null ||
                    (syncFolderManager.CurrentFolderInfo.Version == request.CurrentVersion && !request.IsFirstQuery))
                {
                    // 防止客户端超过时间，设置为一半时间
                    var mainDelayTask = Task.Delay(ServerConfiguration.MaxFreeTime / 2);

                    while (!mainDelayTask.IsCompleted && !taskCompletionSource.Task.IsCompleted)
                    {
                        await Task.WhenAny(mainDelayTask, taskCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(1)));

                        if (syncFolderManager.CurrentFolderInfo?.Version != request.CurrentVersion)
                        {
                            break;
                        }
                    }
                }

                if (syncFolderManager.CurrentFolderInfo is not null)
                {
                    return new QueryFileStatusResponse(syncFolderManager.CurrentFolderInfo);
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                syncFolderManager.CurrentFolderInfoChanged -= OnCurrentFolderInfoChanged;
            }

            void OnCurrentFolderInfoChanged(object? sender, SyncFolderInfo e)
            {
                taskCompletionSource.TrySetResult();
            }
        });
        webApplication.MapPost("/Download", ([FromBody] DownloadFileRequest request, [FromServices] ILogger<ServeOptions> logger) =>
        {
            var currentFolderInfo = syncFolderManager.CurrentFolderInfo;
            if (currentFolderInfo == null)
            {
                return Results.NotFound();
            }

            if (currentFolderInfo.SyncFileDictionary.TryGetValue(request.RelativePath, out var value))
            {
                logger.LogInformation($"Download {request.RelativePath}");
                var file = Path.Join(syncFolder, value.RelativePath);
                return Results.File(file, MediaTypeNames.Application.Octet);
            }

            logger.LogInformation($"Download NotFound {request.RelativePath}");
            return Results.NotFound();
        });
        webApplication.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(syncFolder, ExclusionFilters.System),
            ContentTypeProvider = new ContentTypeProvider(),
            ServeUnknownFileTypes = true,
            RequestPath = StaticFileConfiguration.RequestPath,
            RedirectToAppendTrailingSlash = true,
            DefaultContentType = MediaTypeNames.Application.Octet,
        });
        await webApplication.RunAsync();
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