using System;
using System.IO;
using dotnetCampus.Cli;

namespace dotnetCampus.BuildMd5Task
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            Console.WriteLine("Start building MD5");
            var commandLine = CommandLine.Parse(args);
            var options = commandLine.As<Options>();

            var path = options.Path;
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Path option is not set, we will use the working directory by default.");
                path = Environment.CurrentDirectory;
            }

            path = Path.GetFullPath(path);

            var outputFile = options.OutputFile;
            if (string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine(
                    $"Output is not set, we will use '{Options.DefaultOutputFileName}' by default.");
                outputFile = Options.DefaultOutputFileName;
            }

            outputFile = Path.GetFullPath(outputFile);

            Console.WriteLine($"Path={path}");
            Console.WriteLine($"OutputFile={outputFile}");

            if (Directory.Exists(path))
            {
                // 只有文件夹下才需要使用通配符
                var searchPattern = options.SearchPattern;
                if (string.IsNullOrEmpty(searchPattern))
                {
                    Console.WriteLine($"SearchPattern is not set, we will use '*' by default.");
                }
                searchPattern ??= "*";
                Console.WriteLine($"SearchPattern={searchPattern}");
                var ignoreList = options.IgnoreList;
                Console.WriteLine($"SearchPattern={ignoreList ?? "<null>"}");

                Md5Provider.BuildFolderAllFilesMd5(new DirectoryInfo(path), outputFile, searchPattern, ignoreList, options.Overwrite);
            }
            else if (File.Exists(path))
            {
                Md5Provider.BuildFileMd5(new FileInfo(path), outputFile, options.Overwrite);
            }
            else
            {
                Console.WriteLine($"Can not find Path={path}");
                Environment.Exit(-1);
                return;
            }

            Console.WriteLine("Finished build md5");
        }
    }
}