using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 文件夹辅助类
    /// </summary>
    public static class DirectoryInfoHelper
    {
        /// <summary>
        /// 通过正则匹配文件名获取文件
        /// </summary>
        public static IEnumerable<FileInfo> GetFilesWithRegexPattern(this DirectoryInfo directory, Regex regex)
        {
            foreach (var file in directory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                if (regex.IsMatch(file.Name))
                {
                    yield return file;
                }
            }
        }
    }
}