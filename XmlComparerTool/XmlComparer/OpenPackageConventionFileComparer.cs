using System;
using System.IO;
using System.IO.Compression;

namespace dotnetCampus.Comparison
{
    /// <summary>
    /// 对比 OPC Open Package Convention 文件的工具
    /// </summary>
    public static class OpenPackageConventionFileComparer
    {
        /// <summary>
        /// 判断两个 OPC 格式文件的 XML 部分是否相等
        /// </summary>
        public static void VerifyOpcFileXmlEquals(FileInfo file1, FileInfo file2,
            OpenPackageConventionFileComparerSettings? settings = null)
        {
            settings ??= new OpenPackageConventionFileComparerSettings();

            var workingDirectory = settings.WorkingDirectory;
            workingDirectory ??= new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            workingDirectory.Create();

            var file1UnZipFolder = workingDirectory.CreateSubdirectory("File1");
            var file2UnZipFolder = workingDirectory.CreateSubdirectory("File2");

            ZipFile.ExtractToDirectory(file1.FullName, file1UnZipFolder.FullName);
            ZipFile.ExtractToDirectory(file2.FullName, file2UnZipFolder.FullName);

            foreach (var xmlFile in Directory.EnumerateFiles(file1UnZipFolder.FullName,
                "*.xml"))
            {
                var fileName = MakeRelativePath(file1UnZipFolder.FullName, xmlFile);
                CompareFile(fileName, file1UnZipFolder, file2UnZipFolder, file1.Name, settings);
            }
        }

        /// <summary>
        /// 对比文件
        /// </summary>
        /// <param name="xmlFileName">需要对比的 XML 文件</param>
        /// <param name="file1UnZipFolder"></param>
        /// <param name="file2UnZipFolder"></param>
        /// <param name="compareOpcFileName">传入的待对比的 OPC 文件</param>
        /// <param name="settings"></param>
        private static void CompareFile(string xmlFileName,
            DirectoryInfo file1UnZipFolder,
            DirectoryInfo file2UnZipFolder, string compareOpcFileName,
            OpenPackageConventionFileComparerSettings settings)
        {
            var file1 = Path.Combine(file1UnZipFolder.FullName, xmlFileName);
            var file2 = Path.Combine(file2UnZipFolder.FullName, xmlFileName);

            if (!File.Exists(file2))
            {
                throw new FileNotFoundException($"在新导入的 {compareOpcFileName} 文档，找不到 {xmlFileName} 文件");
            }

            XmlComparer.VerifyXmlEquals(new FileInfo(file1), new FileInfo(file2), settings.XmlComparerSettings);
        }

        private static string MakeRelativePath(string fromPath, string toPath)
        {
#if NETFRAMEWORK
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
#else
            return Path.GetRelativePath(fromPath, toPath);
#endif
        }
    }
}