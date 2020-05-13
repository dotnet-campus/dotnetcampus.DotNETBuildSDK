using System;
using System.Collections.Concurrent;
using dotnetCampus.DotNETBuild.Context;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class Log
    {
        public static LogLevel LogLevel { set; get; }

        public static void Debug(string message)
        {
            if (LogLevel < LogLevel.Debug)
            {
                return;
            }

            Console.WriteLine(message);
            FileLog?.WriteLine($"[Debug] {message}");
        }

        public static void Warning(string message)
        {
            if (LogLevel < LogLevel.Warning)
            {
                return;
            }

            Console.WriteLine(message);
            FileLog?.WriteLine($"[Warning] {message}");
        }

        public static void Info(string message)
        {
            if (LogLevel < LogLevel.Information)
            {
                return;
            }

            Console.WriteLine(message);
            FileLog?.WriteLine($"[Info] {message}");
        }

        public static void Error(string message)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = foregroundColor;
            FileLog?.WriteLine($"[Error] {message}");
        }

        public static FileLog FileLog { set; get; }

        public static void InitFileLog()
        {
            var appConfigurator = AppConfigurator.GetAppConfigurator();
            var logConfiguration = appConfigurator.Of<LogConfiguration>();

            FileLog = FileLogProvider.GetFileLog(logConfiguration);
        }
    }
}