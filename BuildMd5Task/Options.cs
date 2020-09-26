using dotnetCampus.Cli;

namespace dotnetCampus.BuildMd5Task
{
    public class Options
    {
        /// <summary>
        ///     默认校验文件名
        /// </summary>
        /// 为什么叫 ChecksumMd5 是因为后续也许有 ChecksumSha1 等，加上校验使用的算法可以方便理解
        public const string DefaultOutputFileName = "ChecksumMd5.txt";

        /// <summary>
        ///     需要用来创建校验的文件夹或文件，默认将会使用当前工作路径文件夹
        ///     <para>
        ///         传入文件夹时，将会创建每个文件的校验到相同的输出文件
        ///     </para>
        /// </summary>
        [Option('p', "Path", LocalizableDescription = "需要用来创建校验的文件夹或文件，默认将会使用当前工作路径文件夹")]
        public string? Path { get; set; }

        /// <summary>
        ///     输出的校验文件，默认将会输出为当前工作路径的 <see cref="DefaultOutputFileName" /> 文件
        /// </summary>
        [Option('o', "Output", LocalizableDescription = "输出的校验文件，默认将会输出为当前工作路径的 DefaultOutputFileName 文件")]
        public string? OutputFile { get; set; }

        /// <summary>
        ///     匹配文件夹里所有符合此通配符的文件名的文件，多个通配符使用 `|` 字符分割，此属性仅在 <see cref="Path" /> 为文件夹时使用，默认不填将匹配所有文件
        /// </summary>
        [Option("SearchPattern", LocalizableDescription = "匹配文件夹里所有符合此通配符的文件名的文件，多个通配符使用 `|` 字符分割，此属性仅在 Path 为文件夹时使用，默认不填将匹配所有文件")]
        public string? SearchPattern { get; set; }

        /// <summary>
        /// 忽略文件列表，暂不支持通配符，需要使用相对路径。多个相对路径使用 `|` 字符分割
        /// </summary>
        [Option("IgnoreList", LocalizableDescription = "忽略文件列表，暂不支持通配符，需要使用相对路径，路径相对于 Path 路径。多个相对路径使用 `|` 字符分割，此属性仅在 Path 为文件夹时使用")]
        public string? IgnoreList { set; get; }
    }
}