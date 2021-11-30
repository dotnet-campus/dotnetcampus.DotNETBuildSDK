using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotnetCampus.DotNETBuild.Utils;

namespace dotnetCampus.GitCommand
{
    /// <summary>
    /// 表示 Git 对象，用于命令行调用 Git 命令
    /// </summary>
    public class Git : Lindexi.Src.GitCommand.Git
    {
        /// <summary>
        /// 创建 Git 对象，使用当前工作路径作为 Git 仓库
        /// </summary>
        public Git() : base(new DirectoryInfo("."))
        {
        }

        /// <summary>
        /// 创建 Git 对象
        /// </summary>
        /// <param name="repo">仓库</param>
        public Git(DirectoryInfo repo) : base(repo)
        {
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public (bool success, string output) ExecuteCommand(string args)
        {
            return ProcessCommand.ExecuteCommand("git", args, Repo.FullName);
        }

        /// <summary>
        /// 创建新分支，使用 checkout -b <paramref name="branchName"/> 命令
        /// </summary>
        public void CheckoutNewBranch(string branchName, bool force)
        {
            ExecuteCommand($"checkout -{(force ? "B" : "b")} {branchName}");
        }

        /// <summary>
        /// 调用 git add . 命令
        /// </summary>
        public void AddAll()
        {
            ExecuteCommand("add .");
        }

        /// <summary>
        /// 调用 git commit -m message 命令
        /// </summary>
        /// <param name="message"></param>
        public void Commit(string message)
        {
            ExecuteCommand($"commit -m \"{message}\"");
        }

        /// <summary>
        /// 推送代码到仓库
        /// </summary>
        public (bool success, string output) Push(string repository, string branchOrTag, bool force = false)
        {
            return ExecuteCommand($"push \"{repository}\" \"{branchOrTag}\" {(force ? "-f" : "")}");
        }

        /// <summary>
        /// 获取当前分支名
        /// </summary>
        /// <returns></returns>
        public string GetCurrentBranch()
        {
            // git rev-parse --abbrev-ref HEAD
            // git branch --show-current （Git 2.22）
            var (success, output) = ExecuteCommand("branch --show-current");
            if (success)
            {
                return output.Trim('\n');
            }
            else
            {
                return string.Empty;
            }
        }


    }
}