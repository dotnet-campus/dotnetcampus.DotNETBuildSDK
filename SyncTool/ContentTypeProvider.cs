using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;

namespace SyncTool;

/// <summary>
/// 内容类型提供器，用于让所有的文件都可以被下载
/// </summary>
class ContentTypeProvider : IContentTypeProvider
{
    public bool TryGetContentType(string subpath, out string contentType)
    {
        contentType = MediaTypeNames.Application.Octet;
        return true;
    }
}