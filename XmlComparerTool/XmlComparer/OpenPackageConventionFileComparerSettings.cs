using System.IO;

namespace dotnetCampus.Comparison
{
    /// <summary>
    /// 对比 OPC Open Package Convention 文件的工具配置
    /// </summary>
    public class OpenPackageConventionFileComparerSettings
    {
        /// <summary>
        /// 配置 XML 比较
        /// </summary>
        public XmlComparerSettings? XmlComparerSettings { set; get; }

        /// <summary>
        /// 工作用的文件夹
        /// </summary>
        public DirectoryInfo? WorkingDirectory { set; get; }
    }
}