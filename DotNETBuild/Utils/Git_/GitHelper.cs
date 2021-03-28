using System;
using System.IO;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.GitCommand;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class GitHelper
    {
        public static Git GetGitRepo()
        {
            var appConfigurator = AppConfigurator.GetAppConfigurator();
            return GetGitRepo(appConfigurator);
        }

        public static Git GetGitRepo(IAppConfigurator appConfigurator)
        {
            // 先尝试从项目配置获取
            var compileConfiguration = appConfigurator.Of<CompileConfiguration>();
            var codeDirectory = compileConfiguration.CodeDirectory;
            if (string.IsNullOrEmpty(codeDirectory))
            {
                // 也就是现在还没找过这个文件夹
                var fileSniff = new FileSniff(appConfigurator);
                fileSniff.Sniff();
            }
            codeDirectory = compileConfiguration.CodeDirectory;
            if (string.IsNullOrEmpty(codeDirectory))
            {
                throw new ArgumentException("没有找到 git 仓库文件夹，请确定当前项目被 Git 管理 当前工作路径:" + Environment.CurrentDirectory);
            }

            var git = new Git(new DirectoryInfo(codeDirectory))
            {
                NeedWriteLog = false,
            };
            return git;
        }

        public static void FillGitInfo(IAppConfigurator appConfigurator = null)
        {
            appConfigurator ??= AppConfigurator.GetAppConfigurator();
            var git = GitHelper.GetGitRepo(appConfigurator);
            var gitCommitRevisionCount = git.GetGitCommitRevisionCount();
            var gitConfiguration = appConfigurator.Of<GitConfiguration>();
            gitConfiguration.GitCount = gitCommitRevisionCount;

            gitConfiguration.CurrentCommit = git.GetCurrentCommit();

            var compileConfiguration = appConfigurator.Of<CompileConfiguration>();
            if (string.IsNullOrEmpty(compileConfiguration.CurrentCommit))
            {
                compileConfiguration.CurrentCommit = gitConfiguration.CurrentCommit;
            }
        }
    }
}