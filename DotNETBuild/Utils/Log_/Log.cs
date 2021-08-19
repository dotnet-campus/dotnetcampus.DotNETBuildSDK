#nullable enable
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 工具使用的静态日志，默认将会输出到控制台和日志文件
    /// </summary>
    public static class Log
    {
        private static ILogger? _logger;

        internal static ILogger Logger
        {
            private set => _logger = value;
            get
            {
                if (_logger is null)
                {
                    var logLevel = LogLevel;
                    // 暂时禁用日志，解决堆栈炸掉问题
                    LogLevel = LogLevel.None;

                    // 这是在非 SDK 下执行的逻辑，此时需要分配默认的日志记录。为了让日志逻辑保持相同，因此采用相同的延迟日志
                    var lazyInitLogger = new LazyInitLogger();
                    // 自动切换为实际的日志
                    lazyInitLogger.SwitchActualLogger();
                    _logger = lazyInitLogger;

                    LogLevel = logLevel;
                }
                return _logger;
            }
        }

        /// <summary>
        /// 日志等级
        /// </summary>
        public static LogLevel LogLevel { set; get; }

        /// <summary>
        /// 替换当前的日志
        /// </summary>
        /// <param name="logger"></param>
        public static void SetLogger(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (Logger is LazyInitLogger lazyInitLogger)
            {
                lazyInitLogger.ActualLogger = logger;
            }
        }

        /// <summary>
        /// 初始化日志
        /// </summary>
        /// <remarks>
        /// 默认框架初始化的日志将会是 <see cref="LazyInitLogger"/> 类型，之后在框架初始化完成之后，才会切换为实际的日志输出。也就是说在框架初始化中的框架输出日志，此时还不会输出。将会直到上层业务开发者的第一条日志输出之后、或者上层业务开发者的逻辑运行完成之后，才会进行输出。详细请看 <see cref="LazyInitLogger"/> 的注释
        /// </remarks>
        /// <returns></returns>
        /// Call by SDK
        internal static LazyInitLogger InitLazyLogger()
        {
            if (_logger != null)
            {
#if DEBUG
                if (Debugger.IsAttached)
                {
                    throw new InvalidOperationException("有逻辑在初始化日志之前，进行了日志记录，还请查阅日志干掉此逻辑。或者已重复调用初始化日志。仅调试下抛出");
                }
#endif
            }

            var lazyLogger = new LazyInitLogger();
            Logger = lazyLogger;
            return lazyLogger;
        }

        /// <summary>
        /// 输出调试等级日志
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            if (LogLevel > LogLevel.Debug)
            {
                return;
            }

            Logger.LogDebug(message);
        }

        /// <summary>
        /// 输出警告等级日志
        /// </summary>
        /// <param name="message"></param>
        public static void Warning(string message)
        {
            if (LogLevel > LogLevel.Warning)
            {
                return;
            }

            Logger.LogWarning(message);
        }

        /// <summary>
        /// 输出信息等级日志
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            if (LogLevel > LogLevel.Information)
            {
                return;
            }

            Logger.LogInformation(message);
        }

        /// <summary>
        /// 输出错误等级日志
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            Logger.LogError(message);
        }
    }
}