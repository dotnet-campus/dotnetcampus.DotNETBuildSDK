using dotnetCampus.Cli;

namespace dotnetCampus.BuildMd5Task
{
    internal class Options
    {
        /// <summary>
        /// 需要用来创建校验的文件夹或文件，默认将会使用当前工作路径文件夹
        /// <para>
        /// 传入文件夹时，将会创建每个文件的校验到相同的输出文件
        /// </para>
        /// </summary>
        [Option('P', nameof(Path))]
        public string? Path { get; set; }

        /// <summary>
        /// 输出的校验文件，默认将会输出为当前工作路径的 <see cref="DefaultOutputFile"/> 文件
        /// </summary>
        [Option('O', "Output")]
        public string? OutputFile { get; set; }

        public const string DefaultOutputFile = "ChecksumMd5.txt";
    }
}