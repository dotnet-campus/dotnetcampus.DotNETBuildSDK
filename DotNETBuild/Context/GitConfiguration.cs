using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Context
{
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

        public string CurrentCommit
        {
            set => SetValue(value);
            get => GetString();
        }
    }
}