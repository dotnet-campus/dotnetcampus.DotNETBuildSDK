using System;
using System.Threading.Tasks;
using dotnetCampus.GitCommand;
using GitLabApiClient;
using GitLabApiClient.Models.MergeRequests.Requests;

namespace dotnetCampus.GitLabMergeRequestCreator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = dotnetCampus.Cli.CommandLine.Parse(args).As<Options>();
            if (string.IsNullOrEmpty(options.GitlabUrl)
                || string.IsNullOrEmpty(options.GitlabToken))
            {
                return;
            }

            // "[Bot] Automated PR to fix formatting errors"
            var title = options.Title?? "[Bot] Automated PR to fix formatting errors";

            var targetBranch = options.TargetBranch ?? "dev";

            var git = new Git();
            var currentBranch = git.GetCurrentBranch();

            var (success, output) = git.ExecuteCommand($"diff {targetBranch} --name-only");
            if (success is false || string.IsNullOrEmpty(output))
            {
                Console.WriteLine($"There is no difference CurrentBranch({currentBranch}) and TargetBranch({targetBranch}).");
                return;
            }

            var gitLabClient = new GitLabClient(options.GitlabUrl, options.GitlabToken);
           
            await gitLabClient.MergeRequests.CreateAsync(options.ProjectId,
                new CreateMergeRequest(currentBranch, options.TargetBranch, title));
        }
    }
}
