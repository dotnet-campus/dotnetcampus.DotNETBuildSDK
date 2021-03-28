using dotnetCampus.Configurations;

namespace dotnetCampus.CopyAfterCompileTool
{
    class BinaryChopCompileConfiguration : Configuration
    {
        public BinaryChopCompileConfiguration() : base("")
        {
        }

        /// <summary>
        /// 是否在遇到之前已构建过的 commit 可以重新构建，默认是构建过就不覆盖
        /// </summary>
        public bool OverwriteCompiledCommit
        {
            set => SetValue(value);
            get => GetBoolean() ?? false;
        }

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