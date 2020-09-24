namespace dotnetCampus.BuildMd5Task
{
    public class NoMatchFileInfo
    {
        public NoMatchFileInfo(FileMd5Info fileMd5Info, long actualFileLength, string actualFileMd5)
            : this(fileMd5Info.File, actualFileLength, fileMd5Info.FileSize, actualFileMd5, fileMd5Info.Md5)
        {
        }

        public NoMatchFileInfo(string file, long actualFileLength, long expectedFileLength, string actualFileMd5,
            string expectedFileMd5)
        {
            File = file;
            ActualFileLength = actualFileLength;
            ExpectedFileLength = expectedFileLength;
            ActualFileMd5 = actualFileMd5;
            ExpectedFileMd5 = expectedFileMd5;
        }

        private NoMatchFileInfo(string file, long expectedFileLength, string expectedFileMd5)
        {
            File = file;
            IsNotFound = false;
            ExpectedFileLength = expectedFileLength;
            ExpectedFileMd5 = expectedFileMd5;

            ActualFileMd5 = string.Empty;
            ActualFileLength = 0;
        }

        public static NoMatchFileInfo GetFileNotFoundMatchFileInfo(string file, long expectedFileLength,
            string expectedFileMd5)
            => new NoMatchFileInfo(file, expectedFileLength, expectedFileMd5);

        public static NoMatchFileInfo GetFileNotFoundMatchFileInfo(FileMd5Info fileMd5Info)
            => new NoMatchFileInfo(fileMd5Info.File, fileMd5Info.FileSize, fileMd5Info.Md5);

        /// <summary>
        /// 文件的相对路径
        /// </summary>
        public string File { get; }

        /// <summary>
        /// 是否没找到文件
        /// </summary>
        public bool IsNotFound { get; }

        public long ActualFileLength { get; }

        public long ExpectedFileLength { get; }

        public string ActualFileMd5 { get; }

        public string ExpectedFileMd5 { get; }
    }
}