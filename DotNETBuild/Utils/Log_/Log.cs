using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using dotnetCampus.DotNETBuild.Context;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class Log
    {
        internal static LazyInitLogger Logger { set; get; }

        public static LogLevel LogLevel { set; get; }

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

            //Console.WriteLine(message);
            //FileLog?.WriteLine($"[Debug] {message}");
        }

        public static void Warning(string message)
        {
            if (LogLevel > LogLevel.Warning)
            {
                return;
            }

            Logger.LogWarning(message);
            //Console.WriteLine(message);
            //FileLog?.WriteLine($"[Warning] {message}");
        }

        public static void Info(string message)
        {
            if (LogLevel > LogLevel.Information)
            {
                return;
            }

            Logger.LogInformation(message);

            //Console.WriteLine(message);
            //FileLog?.WriteLine($"[Info] {message}");
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

        //public static FileLog FileLog { set; get; }

        //public static void InitFileLog()
        //{
        //    var appConfigurator = AppConfigurator.GetAppConfigurator();
        //    var logConfiguration = appConfigurator.Of<LogConfiguration>();

        //    FileLog = FileLogProvider.GetFileLog(logConfiguration);
        //}
    }

    class CommonLogger : ILogger, IDisposable
    {
        public CommonLogger()
        {
            var appConfigurator = AppConfigurator.GetAppConfigurator();
            var logConfiguration = appConfigurator.Of<LogConfiguration>();

            FileLog = FileLogProvider.GetFileLog(logConfiguration);
        }

        private FileLog FileLog { get; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // 通过 Log.LogLevel 决定是否记录
            if (dotnetCampus.DotNETBuild.Utils.Log.LogLevel > logLevel)
            {
                return;
            }

            var message = formatter(state, exception);
            Console.WriteLine(message);
            FileLog.WriteLine(message);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
    }

    class LazyInitLogger : ILogger, IDisposable
    {
        public void SwitchActualLogger()
        {
            _toActualLogger = true;
        }

        private bool _toActualLogger;

        public ILogger ActualLogger { set; get; }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_toActualLogger)
            {
                // 如果当前切换到实际的日志模式了，而且还有缓存的话，那么调用记录到实际的日志里面
                if (LogCacheList.Any())
                {
                    LogCacheMessage();
                }

                ActualLogger.Log(logLevel, eventId, state, exception, formatter);
            }
            else
            {
                var message = formatter(state, exception);

                LogCacheList.Add((logLevel, message));
            }
        }

        public void LogCacheMessage()
        {
            foreach (var (level, message) in LogCacheList)
            {
                ActualLogger.Log(level, message);
            }

            LogCacheList.Clear();
        }

        private List<(LogLevel logLevel, string message)> LogCacheList { get; } = new List<(LogLevel logLevel, string message)>();
        //private List<(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)> LogCacheList { get; } = new List<(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)>();
    }
}