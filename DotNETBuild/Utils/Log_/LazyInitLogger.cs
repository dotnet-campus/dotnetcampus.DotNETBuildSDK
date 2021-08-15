#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                if (LogCacheList != null)
                {
                    LogCacheMessage();
                }

                ActualLogger.Log(logLevel, eventId, state, exception, formatter);
            }
            else
            {
                var message = formatter(state, exception);

                // 此时一定非空，因为还没有被清空调
                LogCacheList!.Add((logLevel, message));
            }
        }

        public void LogCacheMessage()
        {
            var logCacheList = LogCacheList;

            if (logCacheList == null)
            {
                return;
            }

            lock (logCacheList)
            {
                // 再次判断属性是否是空，如果是空，那么已被清空
                if (LogCacheList != null)
                {
                    foreach (var (level, message) in logCacheList)
                    {
                        ActualLogger.Log(level, message);
                    }

                    logCacheList.Clear();

                    LogCacheList = null;
                }
            }
        }

        public void CleanLogCache()
        {
            // 此时一定非在框架内
            Debug.Assert(_toActualLogger);

            // 清空的方法就是清空此日志缓存
            LogCacheList = null;
        }

        private List<(LogLevel logLevel, string message)>? LogCacheList { set; get; } = new List<(LogLevel logLevel, string message)>();
        //private List<(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)> LogCacheList { get; } = new List<(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)>();
    }
}