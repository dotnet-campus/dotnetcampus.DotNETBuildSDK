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
    }
}