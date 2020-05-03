using System;
using System.Threading;
using dotnetCampus.DotNETBuild;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace dotnetCampus.GitRevisionTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var git = GitHelper.GetGitRepo();
            var gitCommitRevisionCount = git.GetGitCommitRevisionCount();

            Console.WriteLine(gitCommitRevisionCount);

            var gitConfiguration = AppConfigurator.GetAppConfigurator().Of<GitConfiguration>();
            gitConfiguration.GitCount = gitCommitRevisionCount;

            gitConfiguration.CurrentCommit = git.GetCurrentCommit();
        }
    }
}
