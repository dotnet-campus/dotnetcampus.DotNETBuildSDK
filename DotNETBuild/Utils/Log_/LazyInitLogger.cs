using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
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