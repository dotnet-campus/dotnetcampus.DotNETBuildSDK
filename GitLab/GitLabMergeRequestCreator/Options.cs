using dotnetCampus.Cli;
using System;

namespace dotnetCampus.GitLabMergeRequestCreator
{
    /// <summary>
    /// 命令行选项
    /// </summary>
    public class Options
    {
        /// <summary>
        /// 本机 GitLab 的地址
        /// </summary>
        [Option("GitLab")]
        public string GitLabUrl { set; get; }

        /// <summary>
        /// 拥有权限的 Token 值
        /// </summary>
        [Option("Token")]
        public string GitLabToken { set; get; }

        /// <summary>
        /// 对应的 GitLab 仓库。可选，默认将通过环境变量获取 GitLab 的 `$CI_PROJECT_ID` 常量
        /// </summary>
        [Option("ProjectId")]
        public string ProjectId { set; get; }

        /// <summary>
        /// 从 <see cref="SourceBranch"/> 合并到 <see cref="TargetBranch"/> 分支。可选，默认将通过环境变量获取 GitLab 的 `$CI_DEFAULT_BRANCH` 分支，也就是仓库的默认分支
        /// </summary>
        [Option("TargetBranch")]
        public string TargetBranch { set; get; }

        /// <summary>
        /// 从 <see cref="SourceBranch"/> 合并到 <see cref="TargetBranch"/> 分支。可选，默认将通过环境变量获取 GitLab 的 `$CI_COMMIT_BRANCH` 分支，也就是当前 CI 正在运行分支
        /// </summary>
        [Option("SourceBranch")]
        public string SourceBranch { get; set; }

        /// <summary>
        /// 提交 MergeRequest 的标题。可选，默认是默认标题
        /// </summary>
        [Option("Title")]
        public string Title { set; get; }

        public virtual void FixWithDefaultValue()
        {
            if (string.IsNullOrEmpty(GitLabUrl))
            {
                // CI_SERVER_URL 是 GitLab 的常量，大部分机器上的值格式如下
                // https://gitlab.sdlsj.net
                GitLabUrl = Environment.GetEnvironmentVariable("CI_SERVER_URL");
            }

            if (string.IsNullOrEmpty(GitLabToken))
            {
                // 这里的 Token 需要自己配置。可以在 CI/CD Settings 里面配置 Variables 的值
                GitLabToken = Environment.GetEnvironmentVariable("Token");
            }

            if (string.IsNullOrEmpty(ProjectId))
            {
                ProjectId = Environment.GetEnvironmentVariable("CI_PROJECT_ID");
            }

            if (string.IsNullOrEmpty(TargetBranch))
            {
                TargetBranch = Environment.GetEnvironmentVariable("CI_DEFAULT_BRANCH");
            }

            if (string.IsNullOrEmpty(SourceBranch))
            {
                SourceBranch = Environment.GetEnvironmentVariable("CI_COMMIT_BRANCH");
            }

            if (string.IsNullOrEmpty(Title))
            {
                Title = "[Bot] Automated PR to fix formatting errors";
            }
        }

        public override string ToString()
        {
            return @$"GitLabUrl:{GitLabUrl}
GitLabToken:{(string.IsNullOrEmpty(GitLabToken) ? "<NULL>" : "******")}
ProjectId:{ProjectId}
TargetBranch:{TargetBranch}
SourceBranch:{SourceBranch}
";
        }
    }
}