using System;
using System.Collections.Generic;
using System.IO;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class FileLogProvider
    {
        public static FileLog GetFileLog(string file)
        {
            lock (_fileLogList)
            {
                if (_fileLogList.TryGetValue(file, out var weakReference))
                {
                    if (weakReference.TryGetTarget(out var fileLog))
                    {
                        return fileLog;
                    }
                }

                var log = new FileLog(new FileInfo(file));
                weakReference = new WeakReference<FileLog>(log);
                _fileLogList[file] = weakReference;
                return log;
            }
        }

        public static FileLog GetFileLog(LogConfiguration logConfiguration)
        {
            if (string.IsNullOrEmpty(logConfiguration.BuildLogFile))
            {
                if (string.IsNullOrEmpty(logConfiguration.BuildLogDirectory))
                {
                    logConfiguration.BuildLogDirectory = Path.GetFullPath("Log");
                }

                logConfiguration.BuildLogFile = Path.GetFullPath(Path.Combine(logConfiguration.BuildLogDirectory,
                    $"{DateTime.Now:yyMMddhhmmss}.txt"));
            }

            return GetFileLog(logConfiguration.BuildLogFile);
        }

        private readonly static Dictionary<string, WeakReference<FileLog>> _fileLogList =
            new Dictionary<string, WeakReference<FileLog>>();
    }
}