using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Context
{
    /// <summary>
    /// 工具配置
    /// </summary>
    public class ToolConfiguration : Configuration
    {
        /// <summary>
        /// Nuget路径
        /// </summary>
        public string NugetPath
        {
            set => SetValue(value);
            get => GetString();
        }
    }
}
