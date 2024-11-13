using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Utils;

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
        /// 当前的 commit 是这个分支的第几次。手动调用 <see cref="GitHelper.FillGitInfo"/> 方法将被填充
        /// </summary>
        /// <remarks>将使用 git rev-list --count HEAD 命令获取</remarks>
        public int? GitCount
        {
            set => SetValue(value);
            get => GetInt32();
        }

        /// <summary>
        /// 当前的 commit 号。手动调用 <see cref="GitHelper.FillGitInfo"/> 方法将被填充
        /// </summary>
        public string CurrentCommit
        {
            set => SetValue(value);
            get => GetString();
        }
    }
}