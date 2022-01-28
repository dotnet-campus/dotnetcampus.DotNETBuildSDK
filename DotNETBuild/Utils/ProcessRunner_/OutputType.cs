namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 输出类型
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// 标准输出，对应 StandardOutput 内容
        /// </summary>
        StandardOutput,

        /// <summary>
        /// 输出错误信息，对应 StandardError 内容
        /// </summary>
        StandardError,
    }
}