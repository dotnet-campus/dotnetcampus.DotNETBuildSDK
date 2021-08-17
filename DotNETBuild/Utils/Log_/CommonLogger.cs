using System;
using dotnetCampus.DotNETBuild.Context;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
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
}