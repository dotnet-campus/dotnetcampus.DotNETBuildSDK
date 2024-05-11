namespace SyncTool.Utils;

static class DateTimeHelper
{
    public static string DateTimeNowToLogMessage()
    {
        return ToLogMessage(DateTime.Now);
    }

    public static string ToLogMessage(DateTime time)
    {
        return time.ToString(DefaultLogTimeFormat);
    }

    public const string DefaultLogTimeFormat = "yyyy-MM-dd HH:mm:ss,fff";
}