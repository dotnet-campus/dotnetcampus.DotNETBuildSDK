using dotnetCampus.Cli;

namespace dotnetCampus.VerifyMd5Task
{
    public class Options
    {
        /// <summary>
        ///     默认校验文件名
        /// </summary>
        /// 为什么叫 ChecksumMd5 是因为后续也许有 ChecksumSha1 等，加上校验使用的算法可以方便理解
        public const string DefaultChecksumFileName = "ChecksumMd5.txt";

        /// <summary>
        ///     需要用来创建校验的文件夹或文件，默认将会使用当前工作路径文件夹
        ///     <para>
        ///         传入文件夹时，将会创建每个文件的校验到相同的输出文件
        ///     </para>
        /// </summary>
        [Option('p', "Path", LocalizableDescription = "需要用来校验的文件夹，默认将会使用当前工作路径文件夹")]
        public string? Path { get; set; }

        /// <summary>
        /// 校验文件所在路径
        /// </summary>
        [Option('f', "ChecksumMd5FilePath", LocalizableDescription = "校验文件所在路径")]
        public string? ChecksumMd5FilePath { get; set; }
    }
}