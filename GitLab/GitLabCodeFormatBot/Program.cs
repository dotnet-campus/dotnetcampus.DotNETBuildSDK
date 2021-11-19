using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using dotnetCampus.Cli;
using dotnetCampus.DotNETBuild.Utils;
using dotnetCampus.GitCommand;
using dotnetCampus.GitLabMergeRequestCreator;

namespace dotnetCampus.GitLabCodeFormatBot
{
    class Program
    {
        static int Main(string[] args)
        {
            var options = dotnetCampus.Cli.CommandLine.Parse(args).As<Options>();
            options.FixWithDefaultValue();

            // 如果 TargetBranch 是空，那就是在当前分支基础上进行代码格式化
            var git = new Git();

            // 强行切到此分支，优势在于忘记合自动格式化分支的时候
            // 不会在仓库放入大量的垃圾分支
            git.CheckoutNewBranch(options.CodeFormatBranch, force: true);

            FormatCode();

            git.AddAll();
            git.Commit(options.Title);
            git.Push(options.GitLabPushUrl, options.CodeFormatBranch, force: true);

            GitLabMergeRequestHelper.TryCreateMergeRequest(options)
                // 这里没有 UI 线程，不怕 wait 方法
                .Wait();

            return 0;
        }

        private static void FormatCode()
        {
            var (_, output) = ProcessCommand.ExecuteCommand("dotnet", "tool update -g dotnet-format");
            Console.WriteLine(output);

            // dotnet format
            (_, output) = ProcessCommand.ExecuteCommand("dotnet", "format");
            Console.WriteLine(output);
        }
    }

    class Options : dotnetCampus.GitLabMergeRequestCreator.Options
    {
        /// <summary>
        /// 用于给格式化代码使用的分支，默认是 t/bot/FixCodeFormatting 分支
        /// </summary>
        [Option("CodeFormatBranch")]
        public string CodeFormatBranch { set; get; }

        /// <summary>
        /// 用于上传代码的 GitLab 地址，格式如 git@gitlab.sdlsj.net:lindexi/foo.git 地址。可选，默认将通过环境变量拼接 git@$CI_SERVER_URL:$CI_PROJECT_PATH.git 地址
        /// </summary>
        [Option("GitLabPushUrl")]
        public string GitLabPushUrl { set; get; }

        public override void FixWithDefaultValue()
        {
            if (string.IsNullOrEmpty(CodeFormatBranch))
            {
                CodeFormatBranch = "t/bot/FixCodeFormatting";
            }

            if (string.IsNullOrEmpty(GitLabPushUrl))
            {
                GitLabPushUrl =
                    $"git@{Environment.GetEnvironmentVariable("CI_SERVER_URL")}:{Environment.GetEnvironmentVariable("CI_PROJECT_PATH")}.git";
            }

            // 设置上传分支为代码分支
            SourceBranch = CodeFormatBranch;
            base.FixWithDefaultValue();
        }
    }
}
