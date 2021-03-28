using dotnetCampus.Configurations;

namespace CopyAfterCompileTool
{
    class BinaryChopCompileConfiguration : Configuration
    {
        public string OriginBranch
        {
            set => SetValue(value);
            get => GetString() ?? "origin/dev";
        }

        /// <summary>
        /// 保存构建完成的文件夹
        /// </summary>
        public string TargetDirectory
        {
            set => SetValue(value);
            get => GetString();
        }

        public string LastCommit
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 用于做构建的 Bat 文件路径
        /// </summary>
        public string CompilerBatFile
        {
            set => SetValue(value);
            get => GetString();
        }
    }
}