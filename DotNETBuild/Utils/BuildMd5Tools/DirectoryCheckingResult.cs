using System.Collections.Generic;

namespace dotnetCampus.DotNETBuild.Utils.BuildMd5Tools
{
    public class DirectoryCheckingResult
    {
        public bool AreAllMatched { set; get; }

        /// <summary>
        ///     没有匹配上的文件列表
        /// </summary>
        public List<NotMatchedFileInfo> NoMatchedFileInfoList { get; } = new List<NotMatchedFileInfo>();
    }
}