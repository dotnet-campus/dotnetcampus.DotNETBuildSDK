using System;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly LogConfiguration _logConfiguration;

        public FileLoggerProvider(LogConfiguration logConfiguration)
        {
            _logConfiguration = logConfiguration;

            _fileLog = FileLogProvider.GetFileLog(logConfiguration);
        }

        private readonly FileLog _fileLog;

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
            => new FileLogger(categoryName, this, _logConfiguration.LogLevel);

        /// <inheritdoc />
        public void Dispose()
        {
        }

        private void WriteLine(string str)
        {
            Console.WriteLine(str);
            _fileLog.WriteLine(str);
        }

        class FileLogger : ILogger
        {
            private readonly string _categoryName;
            private readonly FileLoggerProvider _fileLoggerProvider;

            private Microsoft.Extensions.Logging.LogLevel LogLevel { get; }

            public FileLogger(string categoryName, FileLoggerProvider fileLoggerProvider, Microsoft.Extensions.Logging.LogLevel logLevel)
            {
                _categoryName = categoryName;
                _fileLoggerProvider = fileLoggerProvider;
                LogLevel = logLevel;
            }

            /// <inheritdoc />
            public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state,
                Exception exception, Func<TState, Exception, string> formatter)
            {
                var categoryName = string.IsNullOrEmpty(_categoryName) ? string.Empty : $"[{_categoryName}]";
                var eventIdText = eventId.Id == 0 ? string.Empty : $" {eventId}";

                var str = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss,fff}][{logLevel}]{categoryName}{eventIdText} {formatter?.Invoke(state, exception)}";
                _fileLoggerProvider.WriteLine(str);
            }

            /// <inheritdoc />
            public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
                => logLevel <= LogLevel;


            /// <inheritdoc />
            public IDisposable BeginScope<TState>(TState state)
            {
                return new Empty();
            }

            private class Empty : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}