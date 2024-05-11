namespace SyncTool.Context;

/// <summary>
/// 查询文件状态的请求
/// </summary>
record QueryFileStatusRequest(string ClientName, ulong CurrentVersion, bool IsFirstQuery);

/// <summary>
/// 查询文件状态的响应
/// </summary>
record QueryFileStatusResponse(SyncFolderInfo SyncFolderInfo);