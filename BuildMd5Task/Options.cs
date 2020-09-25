using dotnetCampus.Cli;

namespace dotnetCampus.BuildMd5Task
{
    public class Options
    {
        /// <summary>
        /// 需要用来创建校验的文件夹或文件，默认将会使用当前工作路径文件夹
        /// <para>
        /// 传入文件夹时，将会创建每个文件的校验到相同的输出文件
        /// </para>
        /// </summary>
        [Option('p', nameof(Path))]
        public string? Path { get; set; }

        /// <summary>
        /// 输出的校验文件，默认将会输出为当前工作路径的 <see cref="DefaultOutputFile"/> 文件
        /// </summary>
        [Option('o', "Output")]
        public string? OutputFile { get; set; }

        /// <summary>
        /// 匹配文件夹里所有符合此正则的文件名的文件，此属性仅在 <see cref="Path"/> 为文件夹时使用，默认不填将匹配所有文件
        /// </summary>
        [Option(longName: "Regex")]
        public string? Regex { get; set; }

        public const string DefaultOutputFile = "ChecksumMd5.txt";
    }
}