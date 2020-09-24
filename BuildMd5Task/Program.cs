using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                Console.WriteLine($"Can not find OutputFile in command line. We will use the DefaultOutputFile={Options.DefaultOutputFile} as the OutputFile.");
                outputFile = Options.DefaultOutputFile;
            }

            outputFile = Path.GetFullPath(outputFile);


            Console.WriteLine($"Path={path}");
            Console.WriteLine($"OutputFile={outputFile}");

            if (Directory.Exists(path))
            {
                BuildFolderAllFilesMd5(new DirectoryInfo(path), outputFile);
            }
            else if (File.Exists(path))
            {
                BuildFileMd5(path, outputFile);
            }
            else
            {
                Console.WriteLine($"Can not find Path={path}");
                Environment.Exit(-1);
                return;
            }

            Console.WriteLine($"Finished build md5");
        }

        private static void BuildFolderAllFilesMd5(DirectoryInfo directory, string outputFile)
        {
            var fileMd5List = new List<FileMd5Info>();
            foreach (var file in directory.GetFiles("*.*", SearchOption.AllDirectories))
            {
                fileMd5List.Add(new FileMd5Info()
                {
                    File = file.FullName.Remove(0, directory.FullName.Length),
                    Md5 = GetMd5Hash(file)
                });
            }

            var json = JsonSerializer.Serialize(fileMd5List, new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true
            });

            Console.WriteLine(json);

            File.WriteAllText(outputFile, json);
        }

        private static void BuildFileMd5(string file, string outputFile)
        {
            var hash = GetMd5Hash(new FileInfo(file));
            Console.WriteLine($"Md5={hash}");
            File.WriteAllText(outputFile, hash);
        }

        /// <summary>
        /// 计算 <paramref name="file"/> 的MD5值。
        /// </summary>
        /// <returns>Md5值</returns>
        public static string GetMd5Hash(FileInfo file)
        {
            using var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return GetMd5HashFromStream(fileStream);
        }

        /// <summary>
        /// 计算 <param name="stream">指定流</param> 的MD5值。
        /// </summary>
        /// <returns>Md5值</returns>
        public static string GetMd5HashFromStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var hashBytes = md5.ComputeHash(stream);
                return string.Join(null, hashBytes.Select(temp => temp.ToString("x2")));
            }
        }
    }
}
