using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace dotnetCampus.BuildMd5Task
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Start build md5");
            var commandLine = dotnetCampus.Cli.CommandLine.Parse(args);
            var options = commandLine.As<Options>();

            var path = options.Path;
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Can not find Path in command line. We will use the WorkingDirectory as the path.");
                path = Environment.CurrentDirectory;
            }

            path = Path.GetFullPath(path);

            var outputFile = options.OutputFile;
            if (string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine(
                    $"Can not find OutputFile in command line. We will use the DefaultOutputFile={Options.DefaultOutputFile} as the OutputFile.");
                outputFile = Options.DefaultOutputFile;
            }

            outputFile = Path.GetFullPath(outputFile);

            Console.WriteLine($"Path={path}");
            Console.WriteLine($"OutputFile={outputFile}");

            if (Directory.Exists(path))
            {
                Md5Provider.BuildFolderAllFilesMd5(new DirectoryInfo(path), outputFile);
            }
            else if (File.Exists(path))
            {
                Md5Provider.BuildFileMd5(new FileInfo(path), outputFile);
            }
            else
            {
                Console.WriteLine($"Can not find Path={path}");
                Environment.Exit(-1);
                return;
            }

            Console.WriteLine($"Finished build md5");
        }
    }
}