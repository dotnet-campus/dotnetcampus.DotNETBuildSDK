namespace dotnetCampus.BuildMd5Task
{
    /// <summary>
    ///     未匹配的文件信息
    /// </summary>
    public class NotMatchedFileInfo
    {
        public NotMatchedFileInfo(FileMd5Info fileMd5Info, long actualFileLength, string actualFileMd5)
            : this(fileMd5Info.RelativeFilePath, actualFileLength, fileMd5Info.FileSize, actualFileMd5, fileMd5Info.Md5)
        {
        }

        public NotMatchedFileInfo(string relativeFilePath, long actualFileLength, long expectedFileLength,
            string actualFileMd5,
            string expectedFileMd5)
        {
            RelativeFilePath = relativeFilePath;
            ActualFileLength = actualFileLength;
            ExpectedFileLength = expectedFileLength;
            ActualFileMd5 = actualFileMd5;
            ExpectedFileMd5 = expectedFileMd5;
        }

        private NotMatchedFileInfo(string relativeFilePath, long expectedFileLength, string expectedFileMd5)
        {
            RelativeFilePath = relativeFilePath;
            IsNotFound = true;
            ExpectedFileLength = expectedFileLength;
            ExpectedFileMd5 = expectedFileMd5;

            ActualFileMd5 = string.Empty;
            ActualFileLength = 0;
        }

        /// <summary>
        ///     文件的相对路径
        /// </summary>
        public string RelativeFilePath { get; }

        /// <summary>
        ///     是否没找到文件
        /// </summary>
        public bool IsNotFound { get; }

        public long ActualFileLength { get; }

        public long ExpectedFileLength { get; }

        public string ActualFileMd5 { get; }

        public string ExpectedFileMd5 { get; }

        public static NotMatchedFileInfo GetFileNotFoundMatchFileInfo(string file, long expectedFileLength,
            string expectedFileMd5)
        {
            return new NotMatchedFileInfo(file, expectedFileLength, expectedFileMd5);
        }

        public static NotMatchedFileInfo GetFileNotFoundMatchFileInfo(FileMd5Info fileMd5Info)
        {
            return new NotMatchedFileInfo(fileMd5Info.RelativeFilePath, fileMd5Info.FileSize, fileMd5Info.Md5);
        }
    }
}