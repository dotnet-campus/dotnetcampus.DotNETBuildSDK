using System;
using System.Reflection;

namespace CopyAfterCompileTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                $"这个工具可以用来 CodeDirectory 的代码从 OriginBranch 的最新版每一个 commit 进行构建，将构建输出文件夹 OutputDirectory 的内容保存到 TargetDirectory 文件夹");

            var binaryChopCompiler = BinaryChopCompiler.CreateInstance();
            binaryChopCompiler.CompileAllCommitAndCopy();
        }
    }
}
