namespace SyncTool.Context;

/// <summary>
/// 同步的文件夹信息
/// </summary>
/// <param name="Version">同步版本</param>
/// <param name="SyncFileList">同步的文件列表</param>
record SyncFolderInfo(ulong Version, List<SyncFileInfo> SyncFileList, List<SyncFolderPathInfo> SyncFolderPathInfoList)
{
    /// <summary>
    /// 同步的文件字典，用来给服务端快速获取文件对应
    /// </summary>
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