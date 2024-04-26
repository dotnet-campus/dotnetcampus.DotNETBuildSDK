#nullable enable

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class ZipArchiveExtension
    {
        /// <summary>
        /// 追加文件夹到压缩文件里面
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="sourceDirectoryName"></param>
        /// <param name="zipRelativePath">在压缩包里面的相对路径</param>
        /// <param name="compressionLevel"></param>
        /// <param name="fileCanAddedPredicate"></param>
        public static void AppendDirectoryToZipArchive(this ZipArchive archive, string sourceDirectoryName, string zipRelativePath, CompressionLevel compressionLevel = CompressionLevel.Fastest, Predicate<string>? fileCanAddedPredicate = null)
        {
            var folders = new Stack<string>();

            folders.Push(sourceDirectoryName);

            while (folders.Count > 0)
            {
                var currentFolder = folders.Pop();

                foreach (var item in Directory.EnumerateFiles(currentFolder))
                {
                    if (fileCanAddedPredicate != null && !fileCanAddedPredicate(item))
                    {
                        continue;
                    }

                    archive.CreateEntryFromFile(item, Path.Join(zipRelativePath, Path.GetRelativePath(sourceDirectoryName, item)), compressionLevel);
                }

                foreach (var item in Directory.EnumerateDirectories(currentFolder))
                {
                    folders.Push(item);
                }
            }
        }
    }
}

