using System.Collections.Generic;

namespace dotnetCampus.BuildMd5Task
{
    public class DirectoryCheckingResult
    {
        public bool AreAllMatched { set; get; }

        /// <summary>
        /// 没有匹配上的文件列表
        /// </summary>
        public List<NotMatchedFileInfo> NoMatchedFileInfoList { get; } = new List<NotMatchedFileInfo>();
    }
}