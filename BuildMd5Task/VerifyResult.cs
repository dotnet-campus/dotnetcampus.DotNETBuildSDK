using System.Collections.Generic;

namespace dotnetCampus.BuildMd5Task
{
    public class VerifyResult
    {
        public bool IsAllMatch { set; get; }

        /// <summary>
        /// 没有匹配上的文件列表
        /// </summary>
        public List<NoMatchFileInfo> NoMatchFileInfoList { get; } = new List<NoMatchFileInfo>();
    }
}