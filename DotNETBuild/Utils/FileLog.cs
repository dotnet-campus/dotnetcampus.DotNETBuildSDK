using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using dotnetCampus.DotNETBuild.Context;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
    public class FileLog
    {
        public FileLog(FileInfo logFile)
        {
            LogFile = logFile;
            WriteToFile();
        }

        public FileInfo LogFile { get; }

        private void WriteToFile()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await _asyncAutoResetEvent.WaitOneAsync();

                    var count = Math.Max(_cache.Count, 8);

                    var cache = new List<string>(count);

                    while (_cache.TryDequeue(out var message))
                    {
                        cache.Add(message);
                    }


                    await File.AppendAllLinesAsync(LogFile.FullName, cache);
                }
            });
        }

        private readonly ConcurrentQueue<string> _cache = new ConcurrentQueue<string>();

        private readonly AsyncAutoResetEvent _asyncAutoResetEvent = new AsyncAutoResetEvent(false);

        public void WriteLine(string message)
        {
            var time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.ffffff");
            _cache.Enqueue($"{time} {message}");

            _asyncAutoResetEvent.Set();
        }
    }
}