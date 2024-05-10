namespace SyncTool.Configurations;

static class ServerConfiguration
{
    /// <summary>
    /// 如果服务端没有更新，最多会空挂机 1 分钟以内
    /// </summary>
    public static TimeSpan MaxFreeTime => TimeSpan.FromMinutes(1);
}