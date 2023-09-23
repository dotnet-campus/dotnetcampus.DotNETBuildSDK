using System.Diagnostics;
using SyncTool.Context;

namespace SyncTool.Server;

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
                    // 用来兼容 Linux 系统
                    relativePath = relativePath.Replace('\\', '/');
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