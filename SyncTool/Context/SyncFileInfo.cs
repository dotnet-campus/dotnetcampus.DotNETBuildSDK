namespace SyncTool.Context;

/// <summary>
/// 同步文件信息
/// </summary>
/// <param name="RelativePath">文件的相对路径。这里为了兼容 Linux 系统，采用的是 / 字符</param>
/// <param name="FileSize"></param>
/// <param name="LastWriteTimeUtc"></param>
record SyncFileInfo(string RelativePath, long FileSize, DateTime LastWriteTimeUtc);