using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using dotnetCampus.Cli;
using dotnetCampus.DotNETBuild.Utils;
using dotnetCampus.GitCommand;

namespace dotnetCampus.GitLabCodeFormatBot
{
    class Program
    {
        static int Main(string[] args)
        {
            var options = dotnetCampus.Cli.CommandLine.Parse(args).As<Options>();
            if (string.IsNullOrEmpty(options.GitlabUrl)
                || string.IsNullOrEmpty(options.GitlabToken))
            {
                Console.WriteLine($"必须传入有效的 GitlabUrl 和 GitlabToken 值");
                return -1;
            }

            // 如果 TargetBranch 是空，那就是在当前分支基础上进行代码格式化
            var git = new Git();
            if (string.IsNullOrEmpty(options.TargetBranch))
            {
                // 如在 Dev 分支进行自动代码格式化，期望就是自动代码格式化完成之后，回到 Dev 版本
                options.TargetBranch = git.GetCurrentBranch();
            }

            var codeFormatBranch = options.CodeFormatBranch;
            if (string.IsNullOrEmpty(codeFormatBranch))
            {
                codeFormatBranch = "t/bot/FixCodeFormatting";
            }

            // 强行切到此分支，优势在于忘记合自动格式化分支的时候
            // 不会在仓库放入大量的垃圾分支
            git.CheckoutNewBranch(codeFormatBranch, force: true);

            FormatCode();



            return 0;
        }

        private static void FormatCode()
        {
            var (_, output) = ProcessCommand.ExecuteCommand("dotnet", "tool update -g dotnet-format");
            Console.WriteLine(output);

            // dotnet format
            (_, output) = ProcessCommand.ExecuteCommand("dotnet", "format");


        }
    }

    class Options : dotnetCampus.GitLabMergeRequestCreator.Options
    {
        /// <summary>
        /// 用于给格式化代码使用的分支，默认是 t/bot/FixCodeFormatting 分支
        /// </summary>
        [Option("CodeFormatBranch")]
        public string CodeFormatBranch { set; get; }
    }
}
