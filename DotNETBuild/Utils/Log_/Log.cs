using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class Log
    {
        internal static LazyInitLogger Logger { set; get; }

        public static LogLevel LogLevel { set; get; }

        /// <summary>
        /// 替换当前的日志
        /// </summary>
        /// <param name="logger"></param>
        public static void SetLogger(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            Logger.ActualLogger = logger;
        }

        // Call by SDK
        internal static LazyInitLogger InitLazyLogger()
        {
            var lazyLogger = new LazyInitLogger();
            Logger = lazyLogger;
            return lazyLogger;
        }

        public static void Debug(string message)
        {
            if (LogLevel > LogLevel.Debug)
            {
                return;
            }

            Logger.LogDebug(message);
        }

        public static void Warning(string message)
        {
            if (LogLevel > LogLevel.Warning)
            {
                return;
            }

            Logger.LogWarning(message);
        }

        public static void Info(string message)
        {
            if (LogLevel > LogLevel.Information)
            {
                return;
            }

            Logger.LogInformation(message);
        }

        public static void Error(string message)
        {
            Logger.LogError(message);

            //var foregroundColor = Console.ForegroundColor;
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine(message);
            //Console.ForegroundColor = foregroundColor;
            //FileLog?.WriteLine($"[Error] {message}");
        }
    }
}