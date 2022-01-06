using System.Collections.Generic;

namespace dotnetCampus.DotNETBuild.Utils.BuildMd5Tools
{
    /// <summary>
    ///     文件夹校验结果
    /// </summary>
    public class DirectoryCheckingResult
    {
        /// <summary>
        ///     是否所有的文件都符合预期，全都匹配上校验值
        /// </summary>
        public bool AreAllMatched { set; get; }

        /// <summary>
        ///     没有匹配上的文件列表
        /// </summary>
        public List<NotMatchedFileInfo> NoMatchedFileInfoList { get; } = new List<NotMatchedFileInfo>();
    }
}