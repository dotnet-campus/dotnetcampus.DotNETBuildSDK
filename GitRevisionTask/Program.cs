using System;
using System.Threading;
using dotnetCampus.DotNETBuild;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.GitRevisionTask
{
    class Program
    {
        static void Main(string[] args)
        {
            // 这个命令可以提供给其他命令作为控制台输出使用，此时需要减少输出内容
            Log.LogLevel = LogLevel.Error;

            var git = GitHelper.GetGitRepo();
            var gitCommitRevisionCount = git.GetGitCommitRevisionCount();

            Console.WriteLine(gitCommitRevisionCount);

            var gitConfiguration = AppConfigurator.GetAppConfigurator().Of<GitConfiguration>();
            gitConfiguration.GitCount = gitCommitRevisionCount;

            gitConfiguration.CurrentCommit = git.GetCurrentCommit();
        }
    }
}
