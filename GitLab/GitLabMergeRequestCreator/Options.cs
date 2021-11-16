using dotnetCampus.Cli;

namespace dotnetCampus.GitLabMergeRequestCreator
{
    public class Options
    {
        /// <summary>
        /// 本机 GitLab 的地址
        /// </summary>
        [Option("Gitlab")]
        public string GitlabUrl { set; get; }

        /// <summary>
        /// 拥有权限的 Token 值
        /// </summary>
        [Option("Token")]
        public string GitlabToken { set; get; }

        /// <summary>
        /// 从 <see cref="SourceBranch"/> 合并到 <see cref="TargetBranch"/> 分支。可选，默认是 dev 分支
        /// </summary>
        [Option("TargetBranch")]
        public string TargetBranch { set; get; }

        /// <summary>
        /// 从 <see cref="SourceBranch"/> 合并到 <see cref="TargetBranch"/> 分支。可选，默认是当前分支
        /// </summary>
        [Option("SourceBranch")]
        public string SourceBranch { get; set; }

        /// <summary>
        /// 提交 MergeRequest 的标题
        /// </summary>
        [Option("Title")]
        public string Title { set; get; }

        /// <summary>
        /// 对应的 GitLab 仓库，可在 GitLab 的 .gitlab-ci.yml 上使用 `$CI_PROJECT_ID` 常量传入
        /// </summary>
        [Option("ProjectId")]
        public string ProjectId { set; get; }
    }
}