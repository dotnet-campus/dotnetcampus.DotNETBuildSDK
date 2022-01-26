using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Context
{
    /// <summary>
    /// 表示 Git 相关信息
    /// </summary>
    public class GitConfiguration : Configuration
    {
        /// <inheritdoc />
        public GitConfiguration() : base("")
        {
        }

        /// <summary>
        /// 当前的 commit 是这个分支的第几次
        /// </summary>
        public int? GitCount
        {
            set => SetValue(value);
            get => GetInt32();
        }

        /// <summary>
        /// 当前的 commit 号
        /// </summary>
        public string CurrentCommit
        {
            set => SetValue(value);
            get => GetString();
        }
    }
}