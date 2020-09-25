using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    ///     文件夹辅助类
    /// </summary>
    public static class DirectoryInfoHelper
    {
        public const char MultiSearchPatternSeparator = '|';

        /// <summary>
        ///     通过正则匹配文件名获取文件
        /// </summary>
        public static IEnumerable<FileInfo> GetFilesWithRegexPattern(this DirectoryInfo directory, Regex regex)
        {
            foreach (var file in directory.EnumerateFiles("*", SearchOption.AllDirectories))
                if (regex.IsMatch(file.Name))
                    yield return file;
        }

        public static IEnumerable<FileInfo> GetFilesWithMultiSearchPattern(this DirectoryInfo directory,
            string multiSearchPattern)
        {
            var allMatchFileInfoList = new List<FileInfo>();
            foreach (var searchPattern in multiSearchPattern.Split(MultiSearchPatternSeparator))
            {
                var fileInfoList = directory.GetFiles(searchPattern, SearchOption.AllDirectories);
                allMatchFileInfoList.AddRange(fileInfoList);
            }

            // 去掉重复的文件
            return allMatchFileInfoList.Distinct();
        }
    }
}