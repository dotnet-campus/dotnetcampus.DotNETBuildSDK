using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class LoggerProvider
    {
        public static ILogger GetLogger(this LogConfiguration logConfiguration) =>
            GetLoggerFactory(logConfiguration).CreateLogger("");

        public static ILoggerFactory GetLoggerFactory(this LogConfiguration logConfiguration)
        {
            lock (_loggerFactoryList)
            {
                if (_loggerFactoryList.TryGetValue(logConfiguration.BuildLogFile, out var weakReference))
                {
                    if (weakReference.TryGetTarget(out var value))
                    {
                        return value;
                    }
                }

                var fileLoggerProvider = new FileLoggerProvider(logConfiguration);

                var loggerFactory = new LoggerFactory(new[] { fileLoggerProvider });

                _loggerFactoryList[logConfiguration.BuildLogFile] = new WeakReference<ILoggerFactory>(loggerFactory);

                return loggerFactory;
            }
        }

        private readonly static Dictionary<string, WeakReference<ILoggerFactory>> _loggerFactoryList = new Dictionary<string, WeakReference<ILoggerFactory>>();
    }
}