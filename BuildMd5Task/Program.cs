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
                BuildFolderAllFilesMd5(new DirectoryInfo(path), outputFile);
            }
            else if (File.Exists(path))
            {
                BuildFileMd5(new FileInfo(path), outputFile);
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
                    File = MakeRelativePath(directory.FullName, file.FullName),
                    FileSize = file.Length,
                    Md5 = GetMd5Hash(file)
                });
            }

            WriteAsJson(fileMd5List, outputFile);
        }

        private static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme)
            {
                // 不是同一种路径，无法转换成相对路径。
                return toPath;
            }

            if (fromUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase)
                && !fromPath.EndsWith("/", StringComparison.OrdinalIgnoreCase)
                && !fromPath.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                // 如果是文件系统，则视来源路径为文件夹。
                fromUri = new Uri(fromPath + Path.DirectorySeparatorChar);
            }

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private static void WriteAsJson(List<FileMd5Info> fileMd5List, string outputFile)
        {
            var json = JsonSerializer.Serialize(fileMd5List, new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true
            });

            Console.WriteLine(json);

            File.WriteAllText(outputFile, json);
        }

        private static void BuildFileMd5(FileInfo file, string outputFile)
        {
            var hash = GetMd5Hash(file);
            Console.WriteLine($"Md5={hash}");

            var fileMd5List = new List<FileMd5Info>()
            {
                new FileMd5Info()
                {
                    File = file.Name,
                    FileSize = file.Length,
                    Md5 = hash
                }
            };

            WriteAsJson(fileMd5List, outputFile);
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