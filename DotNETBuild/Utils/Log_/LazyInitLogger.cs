#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 延迟初始化的日志
    /// </summary>
    /// 用于实现如下功能：
    /// 在框架初始化过程中，框架将会输出一些日志内容。此时不应该立刻将框架输出的日志内容进行输出，原因是有一些业务的控制台输出有特别的功能，如 GetAssemblyVersionTask 等。因此将会延迟日志的实际输出，延迟到上层业务开发者的第一句日志输出的时候、或者上层业务开发者没有写任何的日志，那就等待上层业务开发者的所有逻辑执行完成之后，再进行实际的输出
    /// 在框架中，将会在框架初始化完成之后，调用 <see cref="SwitchActualLogger"/> 方法切换为实际的日志输出
    /// 在业务代码中，可以通过重新设置 <see cref="Log.LogLevel"/> 来设置整个应用，包括框架初始化过程的日志的输出等级。默认的框架日志输出等级都是 <see cref="LogLevel.Information"/> 等级，如果在业务代码中，在第一句日志输出之前，设置了日志等级是警告等，那么框架的日志将会因为等级不够而没有输出
    /// 另外，在业务代码中，可以通过 <see cref="SDK.CleanSdkLog"/> 方法进行清理框架输出的日志内容
    class LazyInitLogger : ILogger, IDisposable
    {
        public ILogger ActualLogger { get => _actualLogger; set => _actualLogger = value ?? throw new ArgumentNullException(nameof(value)); }

        public void SwitchActualLogger(ILogger? actualLogger = null)
        {
            ActualLogger = actualLogger ?? new CommonLogger();
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

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (ActualLogger != null!)
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
            Debug.Assert(ActualLogger != null!);

            // 清空的方法就是清空此日志缓存
            LogCacheList = null;
        }

        private ILogger _actualLogger = null!;

        private List<(LogLevel logLevel, string message)>? LogCacheList { set; get; } = new List<(LogLevel logLevel, string message)>();
        //private List<(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)> LogCacheList { get; } = new List<(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)>();
    }
}