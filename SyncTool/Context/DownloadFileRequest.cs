namespace SyncTool.Context;

/// <summary>
/// 下载文件请求
/// </summary>
/// <param name="RelativePath">相对路径</param>
/// 原本是使用 Get 的方式请求，然而可能存在编码问题，比如 `#` 字符等问题
record DownloadFileRequest(string RelativePath);