#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace dotnetCampus.DotNETBuild.Utils.BuildMd5Tools
{
    /// <summary>
    /// 提供 Md5 校验辅助方法
    /// </summary>
    public static class Md5Provider
    {
        /// <summary>
        /// 将 <paramref name="directory"/> 里所有文件，包括里层文件夹的文件，进行计算出 md5 值，作为 XML 格式，输出到 <paramref name="outputFile"/> 文件里面
        /// </summary>
        /// <param name="directory">遍历生成 md5 的文件夹</param>
        /// <param name="outputFile">输出的文件</param>
        /// <param name="multiSearchPattern">文件夹里的文件搜寻通配符匹配字符串，默认是 `*` 字符串</param>
        /// <param name="ignoreList">忽略文件列表，暂不支持通配符，需要使用相对路径，路径相对于 Path 路径。多个相对路径使用 `|` 字符分割，此属性仅在 Path 为文件夹时使用</param>
        /// <param name="overwrite">是否覆盖输出文件</param>
        public static void BuildFolderAllFilesMd5(DirectoryInfo directory, string outputFile,
            string? multiSearchPattern = null,
            string? ignoreList = null, bool overwrite = false)
        {
            multiSearchPattern ??= "*";
            var ignoreFileList = ParseIgnoreFileList(ignoreList);

            var fileMd5List = new List<FileMd5Info>();
            foreach (var file in directory.GetFilesWithMultiSearchPattern(multiSearchPattern))
            {
                var relativeFilePath = MakeRelativePath(directory.FullName, file.FullName);
                // 判断是否忽略列表
                StringComparison stringComparison = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    // 在 Windows 下忽略大小写
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal;
                if (ignoreFileList.Any(temp => string.Equals(relativeFilePath, temp, stringComparison)))
                {
                    // 这是被忽略的数据
                    continue;
                }

                fileMd5List.Add(new FileMd5Info
                {
                    RelativeFilePath = relativeFilePath,
                    FileSize = file.Length,
                    Md5 = GetMd5Hash(file)
                });
            }

            WriteToFile(fileMd5List, outputFile, overwrite);
        }

        /// <summary>
        /// 将 <paramref name="file"/> 计算出 md5 值，序列化为 XML 格式，输出到 <paramref name="outputFile"/> 文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="outputFile"></param>
        /// <param name="overwrite"></param>
        public static void BuildFileMd5(FileInfo file, string outputFile, bool overwrite)
        {
            var hash = GetMd5Hash(file);
            Console.WriteLine($"Md5={hash}");

            var fileMd5List = new List<FileMd5Info>
            {
                new FileMd5Info
                {
                    RelativeFilePath = file.Name,
                    FileSize = file.Length,
                    Md5 = hash
                }
            };

            WriteToFile(fileMd5List, outputFile, overwrite);
        }

        /// <summary>
        /// 判断 <paramref name="directory"/> 里的文件是否匹配 <paramref name="checksumFile"/> 里的值
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="checksumFile"></param>
        /// <returns></returns>
        public static DirectoryCheckingResult VerifyFolderMd5(DirectoryInfo directory, FileInfo checksumFile)
        {
            // 读取文件
            var xmlSerializer = new XmlSerializer(typeof(List<FileMd5Info>));
            using var fileStream = checksumFile.OpenRead();
            var fileMd5InfoList = (List<FileMd5Info>)xmlSerializer.Deserialize(fileStream);

            var verifyResult = new DirectoryCheckingResult();
            verifyResult.AreAllMatched = true;

            foreach (var fileMd5Info in fileMd5InfoList)
            {
                var file = new FileInfo(Path.Combine(directory.FullName, fileMd5Info.RelativeFilePath));
                if (file.Exists)
                {
                    // 先判断文件长度
                    var fileLength = file.Length;
                    if (fileMd5Info.FileSize != fileLength)
                    {
                        verifyResult.AreAllMatched = false;
                        verifyResult.NoMatchedFileInfoList.Add(new NotMatchedFileInfo(fileMd5Info, fileLength,
                            string.Empty));
                    }
                    else
                    {
                        var hash = GetMd5Hash(file);
                        if (!hash.Equals(fileMd5Info.Md5, StringComparison.OrdinalIgnoreCase))
                        {
                            // 哈希不相同
                            verifyResult.AreAllMatched = false;
                            verifyResult.NoMatchedFileInfoList.Add(
                                new NotMatchedFileInfo(fileMd5Info, fileLength, hash));
                        }
                    }
                }
                else
                {
                    // 找不到文件
                    verifyResult.AreAllMatched = false;
                    verifyResult.NoMatchedFileInfoList.Add(
                        NotMatchedFileInfo.GetFileNotFoundMatchFileInfo(fileMd5Info));
                }
            }

            return verifyResult;
        }

        private static List<string> ParseIgnoreFileList(string? ignoreList)
        {
            if (string.IsNullOrEmpty(ignoreList))
            {
                return new List<string>();
            }

            return ignoreList.Split('|').ToList();
        }

        private static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme)
                // 不是同一种路径，无法转换成相对路径。
                return toPath;

            if (fromUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase)
                && !fromPath.EndsWith("/", StringComparison.OrdinalIgnoreCase)
                && !fromPath.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
                // 如果是文件系统，则视来源路径为文件夹。
                fromUri = new Uri(fromPath + Path.DirectorySeparatorChar);

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            return relativePath;
        }

        private static void WriteToFile(List<FileMd5Info> fileMd5List, string outputFile, bool overwrite)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<FileMd5Info>));

            var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;
            using var fileStream = new FileStream(outputFile, fileMode, FileAccess.Write);

            xmlSerializer.Serialize(fileStream, fileMd5List);
        }

        /// <summary>
        ///     计算 <paramref name="file" /> 的MD5值。
        /// </summary>
        /// <returns>Md5值</returns>
        private static string GetMd5Hash(FileInfo file)
        {
            using var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return GetMd5HashFromStream(fileStream);
        }

        /// <summary>
        ///     计算
        ///     <param name="stream">指定流</param>
        ///     的MD5值。
        /// </summary>
        /// <returns>Md5值</returns>
        private static string GetMd5HashFromStream(Stream stream)
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