using System;
using System.Configuration.Assemblies;
using System.Net;
using System.Reflection;

using dotnetCampus.Cli;

using SyncTool.Configurations;
using SyncTool.Context;
using SyncTool.Server;
using SyncTool.Utils;

namespace SyncTool.Client;

/// <summary>
/// 客户端的同步命令行参数
/// </summary>
[Verb("sync")]
internal class SyncOptions
{
    /// <summary>
    /// 同步服务的地址，如 http://127.0.0.1:56621
    /// </summary>
    [Option('a', "Address")]
    public string? Address { set; get; }

    /// <summary>
    /// 本地同步的文件夹，不填默认为工作路径
    /// </summary>
    [Option('f', "Folder")]
    public string? SyncFolder { set; get; }

    public async Task Run()
    {
        if (string.IsNullOrEmpty(Address))
        {
            Console.WriteLine($@"找不到同步地址，请确保传入正确参数。
参数列表：

- `-a` 或 `--Address` : 【必填】同步服务的地址，如 http://127.0.0.1:56621
- `-f` 或 `--Folder` : 【选填】本地同步的文件夹，不填默认为工作路径

参数例子： 
SyncTool -a http://127.0.0.1:56621 -f lindexi");
            return;
        }

        var syncFolder = SyncFolder;
        if (string.IsNullOrEmpty(syncFolder))
        {
            // 没有给明确的文件夹，使用工作文件夹
            syncFolder = Environment.CurrentDirectory;
        }

        syncFolder = Path.GetFullPath(syncFolder);
        Directory.CreateDirectory(syncFolder);

        Console.WriteLine($"开始执行文件夹同步。同步地址：{Address} 同步文件夹{syncFolder}");

        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Address);
        // 客户端允许等着服务端慢慢返回，不要不断发送请求
        httpClient.Timeout = ServerConfiguration.MaxFreeTime;

        // 记录本地的字典值。首次同步的时候需要用到
        Dictionary<string, SyncFileInfo> syncFileDictionary = InitLocalInfo(syncFolder);

        ulong currentVersion = 0;
        bool isFirstQuery = true;
        var clientName = Environment.MachineName;
        while (true)
        {
            try
            {
                var queryFileStatusRequest = new QueryFileStatusRequest(clientName, currentVersion, isFirstQuery);
                using var httpResponseMessage = await httpClient.PostAsJsonAsync("/", queryFileStatusRequest);
                if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    // 服务端是不是还没开启 是不是开启错版本了
                    var assemblyVersion =
                        GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
                            .InformationalVersion;
                    Console.WriteLine($"服务器返回 404 可能访问错误的服务，或 SyncTool 服务器版本过低。当前 SyncTool 客户端版本：{assemblyVersion}");

                    // 同步结束
                    return;
                }

                httpResponseMessage.EnsureSuccessStatusCode();

                var queryFileStatusResponse =
                    await httpResponseMessage.Content.ReadFromJsonAsync<QueryFileStatusResponse>();
                var syncFolderInfo = queryFileStatusResponse?.SyncFolderInfo;

                if (syncFolderInfo is null || syncFolderInfo.Version == currentVersion)
                {
                    // 这里不需要等待，继续不断发起请求就可以
                    // 为什么不怕发送太多，影响性能？服务端不会立刻返回
                    //await Task.Delay(TimeSpan.FromSeconds(1));
                    continue;
                }

                isFirstQuery = false;
                currentVersion = syncFolderInfo.Version;
                Console.WriteLine($"[{currentVersion}] 开始同步 - {DateTimeHelper.DateTimeNowToLogMessage()}");
                await SyncFolderAsync(syncFolderInfo.SyncFileList, syncFolderInfo.SyncFolderPathInfoList, currentVersion);

                Console.WriteLine($"[{currentVersion}] 同步完成 - {DateTimeHelper.DateTimeNowToLogMessage()}");
                Console.WriteLine($"同步地址：{Address} 同步文件夹{syncFolder}");
                Console.WriteLine("==========");

                // 更新本地字典信息
                syncFileDictionary.Clear();
                foreach (var syncFileInfo in syncFolderInfo.SyncFileList)
                {
                    syncFileDictionary[syncFileInfo.RelativePath] = syncFileInfo;
                }

                _ = ReportCompleted(currentVersion);
            }
            catch (HttpRequestException e)
            {
                if (e.HttpRequestError == HttpRequestError.ConnectionError)
                {
                    // 可能是服务器还没开启
                    Console.WriteLine($"【同步失败】连接服务器失败，同步地址：{Address} 同步文件夹{syncFolder}");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception e)
            {
                // 大不了下次再继续
                Console.WriteLine($"【同步失败】同步地址：{Address} 同步文件夹{syncFolder}\r\n{e}");
            }
        }

        async Task SyncFolderAsync(List<SyncFileInfo> remote, List<SyncFolderPathInfo> syncFolderPathInfoList, ulong version)
        {
            Dictionary<string/*RelativePath*/, SyncFileInfo> local = syncFileDictionary;

            foreach (var remoteSyncFileInfo in remote)
            {
                // ReSharper disable AccessToModifiedClosure
                if (version != currentVersion)
                {
                    return;
                }

                if (local.TryGetValue(remoteSyncFileInfo.RelativePath, out var localInfo))
                {
                    // 如果能拿到本地的记录，判断一下是否需要更新
                    var localFilePath = Path.Join(syncFolder, localInfo.RelativePath);
                    var localFile = new FileInfo(localFilePath);
                    if (localFile.Exists
                        // 时间不能取本地时间，因为必定存在时间差
                        && localInfo.LastWriteTimeUtc == remoteSyncFileInfo.LastWriteTimeUtc
                        && localFile.Length == remoteSyncFileInfo.FileSize)
                    {
                        // 如果本地的记录不需要更新，那就跳过
                        continue;
                    }
                }

                Console.WriteLine($"正在同步 {remoteSyncFileInfo.RelativePath}");

                // 先下载到一个新的文件，然后再重命名替换
                // 如果原本的文件正在被占用，那失败的只有重命名部分，而不会导致重复下载
                // 那如果下载失败呢？大概需要重新开始同步了

                var downloadFilePath = await DownloadFile(remoteSyncFileInfo);

                // 完成下载，移动下载的文件作为正式需要的文件
                while (true)
                {
                    if (version != currentVersion)
                    {
                        return;
                    }

                    try
                    {
                        var localFilePath = Path.Join(syncFolder, remoteSyncFileInfo.RelativePath);
                        File.Move(downloadFilePath, localFilePath, overwrite: true);
                        break;
                    }
                    catch
                    {
                        // 忽略
                        Console.WriteLine($"同步 {remoteSyncFileInfo.RelativePath} 失败，正在重试");
                    }

                    // 快速下载完成
                    //await Task.Delay(200);
                }
            }

            foreach (var folderPathInfo in syncFolderPathInfoList)
            {
                if (version != currentVersion)
                {
                    return;
                }

                // 如果文件夹不存在，则创建文件夹
                var localFilePath = Path.Join(syncFolder, folderPathInfo.RelativePath);
                Directory.CreateDirectory(localFilePath);
            }

            // 先删除多余的文件，再删除空文件夹，除非空文件夹是在记录里面的
            await RemoveRedundantFile(remote, version);
            await RemoveRedundantFolder(syncFolderPathInfoList, version);
        }

        async Task RemoveRedundantFile(List<SyncFileInfo> remote, ulong version)
        {
            // 删除多余的文件，也就是本地存在但是远程不存在的文件
            // 记录已经更新的 RelativePath 哈希，用来记录哪些存在
            var updatedList = new HashSet<string>(remote.Count);
            foreach (var syncFileInfo in remote)
            {
                updatedList.Add(syncFileInfo.RelativePath);
            }

            foreach (var file in Directory.GetFiles(syncFolder, "*", SearchOption.AllDirectories))
            {
                if (version != currentVersion)
                {
                    return;
                }

                var relativePath = Path.GetRelativePath(syncFolder, file);
                // 用来兼容 Linux 系统
                relativePath = relativePath.Replace('\\', '/');

                for (int i = 0; i < 1000; i++)
                {
                    try
                    {
                        if (updatedList.Contains(relativePath))
                        {
                            break;
                        }
                        else
                        {
                            // 本地存在，远端不存在，删除
                            File.Delete(file);
                            Console.WriteLine($"删除 {relativePath}");
                            if (!File.Exists(file))
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (i == 100)
                        {
                            Console.WriteLine($"第{i}次删除 {relativePath} 失败 {e}");
                        }

                        await Task.Delay(100);
                    }
                }
            }
        }

        async Task RemoveRedundantFolder(List<SyncFolderPathInfo> syncFolderPathInfoList, ulong version)
        {
            var updatedList = new HashSet<string>(syncFolderPathInfoList.Count);
            foreach (var syncFileInfo in syncFolderPathInfoList)
            {
                updatedList.Add(syncFileInfo.RelativePath);
            }

            foreach (var folder in Directory.GetDirectories(syncFolder, "*", SearchOption.AllDirectories))
            {
                if (version != currentVersion)
                {
                    return;
                }

                if (Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).Any())
                {
                    // 如果存在文件，则不是空文件夹，不能删除
                    continue;
                }

                // 没有任何文件的空文件夹，如果不在列表里面，则需要删除文件夹
                var relativePath = Path.GetRelativePath(syncFolder, folder);
                // 用来兼容 Linux 系统
                relativePath = relativePath.Replace('\\', '/');

                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        if (updatedList.Contains(relativePath))
                        {
                            break;
                        }
                        else
                        {
                            // 删除空文件夹的时候，可能遇到空文件夹里面还包含其他空文件夹，导致删除失败
                            // System.IO.IOException: Directory not empty
                            Directory.Delete(folder, true);
                            if (!Directory.Exists(folder))
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (i == 100 - 1)
                        {
                            Console.WriteLine($"第{i}次删除 {relativePath} 失败 {e}");
                        }

                        await Task.Delay(100);
                    }
                }
            }
        }

        async Task<string> DownloadFile(SyncFileInfo remoteSyncFileInfo)
        {
            // 发起请求，使用 Post 的方式，解决 GetURL 的字符不支持
            var request = new DownloadFileRequest(remoteSyncFileInfo.RelativePath);
            var response = await httpClient.PostAsJsonAsync("/Download", request);
            await using var stream = await response.Content.ReadAsStreamAsync();

            var downloadFilePath = Path.Join(syncFolder, $"{remoteSyncFileInfo.RelativePath}_{Path.GetRandomFileName()}");
            // 下载之前先确保文件夹存在，防止下载炸了
            Directory.CreateDirectory(Path.GetDirectoryName(downloadFilePath)!);

            await using var fileStream = File.Create(downloadFilePath);
            await stream.CopyToAsync(fileStream);

            return downloadFilePath;
        }

        async Task ReportCompleted(ulong version)
        {
            try
            {
                var syncCompletedRequest = new SyncCompletedRequest(clientName, version);
                await httpClient.PostAsJsonAsync("/SyncCompleted", syncCompletedRequest);
            }
            catch
            {
                // 只是报告而已，失败就失败
            }
        }
    }

    /// <summary>
    /// 初始化本地文件的信息
    /// </summary>
    /// <param name="syncFolder"></param>
    /// <returns></returns>
    private static Dictionary<string /*RelativePath*/, SyncFileInfo> InitLocalInfo(string syncFolder)
    {
        var syncFileDictionary =
            new Dictionary<string /*RelativePath*/, SyncFileInfo>();
        foreach (var file in Directory.EnumerateFiles(syncFolder, "*", SearchOption.AllDirectories))
        {
            var fileInfo = new FileInfo(file);
            var relativePath = Path.GetRelativePath(syncFolder, file);
            var syncFileInfo = new SyncFileInfo(relativePath, fileInfo.Length, fileInfo.LastWriteTimeUtc);
            syncFileDictionary[relativePath] = syncFileInfo;
        }
        return syncFileDictionary;
    }
}