using System;

namespace dotnetCampus.DotNETBuild.Utils
{
    static class Log
    {
        public static void Info(string message)
        {
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
    }

    
}