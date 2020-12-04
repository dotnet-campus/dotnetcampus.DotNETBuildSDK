using System;
using System.IO;
using System.Linq;
using System.Text;
using dotnetCampus.BuildMd5Task;
using dotnetCampus.Cli;

namespace dotnetCampus.VerifyMd5Task
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Verify MD5");
            var commandLine = CommandLine.Parse(args);
            var options = commandLine.As<Options>();

            var path = options.Path;
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Path option is not set, we will use the working directory by default.");
                path = Environment.CurrentDirectory;
            }

            path = Path.GetFullPath(path);

            var checksumMd5FilePath = options.ChecksumMd5FilePath;
            if (string.IsNullOrEmpty(checksumMd5FilePath))
            {
                Console.WriteLine($"ChecksumMd5FilePath option is not set, we will use the {Options.DefaultChecksumFileName} by default.");
                checksumMd5FilePath = Options.DefaultChecksumFileName;
            }

            checksumMd5FilePath = Path.GetFullPath(checksumMd5FilePath);

            Console.WriteLine($"Path={path}");
            Console.WriteLine($"ChecksumMd5FilePath={checksumMd5FilePath}");

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Can not find path={path}");
            }

            if (!File.Exists(checksumMd5FilePath))
            {
                throw new FileNotFoundException($"Can not find ChecksumMd5FilePath={checksumMd5FilePath}");
            }

            var directoryCheckingResult = Md5Provider.VerifyFolderMd5(new DirectoryInfo(path), new FileInfo(checksumMd5FilePath));

            // 没有全成功，上报和记录日志哪些文件失败
            var noMatchFileInfoList = directoryCheckingResult.NoMatchedFileInfoList;

            var stringBuilder = new StringBuilder();

            // 文件丢失的列表
            var notFoundFileList = noMatchFileInfoList.Where(temp => temp.IsNotFound).ToList();
            if (notFoundFileList.Count > 0)
            {
                stringBuilder.AppendLine($"NotFoundFileList: {notFoundFileList.Count}");
                foreach (var temp in notFoundFileList)
                {
                    stringBuilder.Append($"File:{temp.RelativeFilePath}\r\nExpectedFileMd5:{temp.ExpectedFileMd5}\r\n");
                }
            }

            var damagedFileList = noMatchFileInfoList.Where(temp => !temp.IsNotFound).ToList();
            // 损坏的文件列表
            if (damagedFileList.Count > 0)
            {
                stringBuilder.AppendLine($"DamagedFileList: {damagedFileList.Count}");
                foreach (var temp in damagedFileList)
                {
                    stringBuilder.AppendLine($"File: {temp.RelativeFilePath}")
                        .AppendLine($"ActualFileMd5: {temp.ActualFileMd5}")
                        .AppendLine($"ActualFileLength: {temp.ActualFileLength}")
                        .AppendLine($"ExpectedFileMd5: {temp.ExpectedFileMd5}")
                        .AppendLine($"ExpectedFileLength: {temp.ExpectedFileLength}");
                }
            }

            Console.WriteLine(stringBuilder);

            if (!directoryCheckingResult.AreAllMatched)
            {
                Environment.Exit(-1);
            }
        }
    }

    public class Options
    {
        /// <summary>
        ///     默认校验文件名
        /// </summary>
        /// 为什么叫 ChecksumMd5 是因为后续也许有 ChecksumSha1 等，加上校验使用的算法可以方便理解
        public const string DefaultChecksumFileName = "ChecksumMd5.txt";

        /// <summary>
        ///     需要用来创建校验的文件夹或文件，默认将会使用当前工作路径文件夹
        ///     <para>
        ///         传入文件夹时，将会创建每个文件的校验到相同的输出文件
        ///     </para>
        /// </summary>
        [Option('p', "Path", LocalizableDescription = "需要用来校验的文件夹，默认将会使用当前工作路径文件夹")]
        public string? Path { get; set; }

        /// <summary>
        /// 校验文件所在路径
        /// </summary>
        [Option('f', "ChecksumMd5FilePath", LocalizableDescription = "校验文件所在路径")]
        public string? ChecksumMd5FilePath { get; set; }
    }

}
