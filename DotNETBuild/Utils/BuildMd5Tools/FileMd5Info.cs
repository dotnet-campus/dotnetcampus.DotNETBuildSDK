namespace dotnetCampus.DotNETBuild.Utils.BuildMd5Tools
{
    /// <summary>
    /// 文件的 Md5 校验信息
    /// </summary>
    public class FileMd5Info
    {
        /// <summary>
        /// 相对的文件路径
        /// </summary>
        public string RelativeFilePath { set; get; } = null!;

        /// <summary>
        /// 文件的大小
        /// </summary>
        public long FileSize { set; get; }

        /// <summary>
        /// 文件的 Md5 值
        /// </summary>
        public string Md5 { set; get; } = null!;
    }
}