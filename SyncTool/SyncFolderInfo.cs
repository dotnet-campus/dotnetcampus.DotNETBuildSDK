namespace SyncTool;
record SyncFolderInfo(ulong Version, List<SyncFileInfo> SyncFileList)
{
    public Dictionary<string /*RelativePath*/, SyncFileInfo> SyncFileDictionary
    {
        get
        {
            // 这里不怕多线程问题。多线程问题只会多创建一次对象，不会有其他影响
            return _syncFileDictionary ??= SyncFileList.ToDictionary(t => t.RelativePath);
        }
    }

    private Dictionary<string /*RelativePath*/, SyncFileInfo>? _syncFileDictionary;
}