using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using SyncTool.Client;
using SyncTool.Context;
using SyncTool.Utils;

namespace SyncTool.Server;

class SyncFolderManager
{
    [DisallowNull]
    public SyncFolderInfo? CurrentFolderInfo
    {
        get => _currentFolderInfo;
        private set
        {
            _currentFolderInfo = value;
            CurrentFolderInfoChanged?.Invoke(this, value);
        }
    }

    private FileSystemWatcher? _watcher;
    public event EventHandler<SyncFolderInfo>? CurrentFolderInfoChanged;

    public void Run(string watchFolder)
    {
        UpdateChange(watchFolder);

        var fileSystemWatcher = _watcher = new FileSystemWatcher(watchFolder)
        {
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                           NotifyFilters.FileName | NotifyFilters.LastWrite
        };

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
    private SyncFolderInfo? _currentFolderInfo;

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
                    // 用来兼容 Linux 系统
                    relativePath = relativePath.Replace('\\', '/');
                    var syncFileInfo = new SyncFileInfo(relativePath, fileInfo.Length, fileInfo.LastWriteTimeUtc);
                    syncFileList.Add(syncFileInfo);
                }

                var syncFolderPathInfoList = new List<SyncFolderPathInfo>();
                foreach (var folder in Directory.EnumerateDirectories(watchFolder, "*", SearchOption.AllDirectories))
                {
                    if (!Enable())
                    {
                        return;
                    }

                    var relativePath = Path.GetRelativePath(watchFolder, folder);
                    // 用来兼容 Linux 系统
                    relativePath = relativePath.Replace('\\', '/');

                    syncFolderPathInfoList.Add(new SyncFolderPathInfo(relativePath));
                }

                CurrentFolderInfo = new SyncFolderInfo(currentVersion, syncFileList, syncFolderPathInfoList);
            }
            catch (IOException e)
            {
                // 可以忽略，因为可以在读取文件时，文件被删掉
                Debug.WriteLine(e);
            }

            Console.WriteLine($"检测到更新 - {DateTimeHelper.DateTimeNowToLogMessage()}");
        });

        bool Enable() => Interlocked.Read(ref _currentVersion) == currentVersion;
    }
}