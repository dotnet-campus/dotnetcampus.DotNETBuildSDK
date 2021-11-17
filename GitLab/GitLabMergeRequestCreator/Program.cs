using System;
using System.Threading.Tasks;

namespace dotnetCampus.GitLabMergeRequestCreator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = dotnetCampus.Cli.CommandLine.Parse(args).As<Options>();

            if (string.IsNullOrEmpty(options.GitLabUrl))
            {
                // CI_SERVER_URL 是 GitLab 的常量，大部分机器上的值格式如下
                // https://gitlab.sdlsj.net
                options.GitLabUrl = Environment.GetEnvironmentVariable("CI_SERVER_URL");
            }

            if (string.IsNullOrEmpty(options.GitLabToken))
            {
                // 这里的 Token 需要自己配置。可以在 CI/CD Settings 里面配置 Variables 的值
                options.GitLabToken = Environment.GetEnvironmentVariable("Token");
            }

            if (string.IsNullOrEmpty(options.ProjectId))
            {
                options.ProjectId = Environment.GetEnvironmentVariable("CI_PROJECT_ID");
            }

            if (string.IsNullOrEmpty(options.TargetBranch))
            {
                options.TargetBranch = Environment.GetEnvironmentVariable("CI_DEFAULT_BRANCH");
            }

            if (string.IsNullOrEmpty(options.SourceBranch))
            {
                options.SourceBranch = Environment.GetEnvironmentVariable("CI_COMMIT_BRANCH");
            }

            Console.WriteLine(options.ToString());

            await GitLabMergeRequestHelper.TryCreateMergeRequest(options);
        }
    }
}