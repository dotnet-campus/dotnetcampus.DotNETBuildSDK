using System;
using dotnetCampus.Cli;
using dotnetCampus.DotNETBuild.Utils;
using dotnetCampus.GitCommand;
using GitLabApiClient;

namespace GitLabMergeRequestCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = dotnetCampus.Cli.CommandLine.Parse(args).As<Options>();
            if (string.IsNullOrEmpty(options.GitlabUrl)
                || string.IsNullOrEmpty(options.GitlabToken))
            {
                return;
            }

            var targetBranch = options.TargetBranch ?? "dev";

            var git = new Git();
            var (success, output) = git.ExecuteCommand($"diff {targetBranch} --name-only");

            var gitLabClient = new GitLabClient(options.GitlabUrl, options.GitlabToken);
        }
    }

    class Options
    {
        [Option("Gitlab")]
        public string GitlabUrl { set; get; }

        [Option("Token")]
        public string GitlabToken { set; get; }

        [Option("TargetBranch")]
        public string TargetBranch { set; get; }
    }
}
