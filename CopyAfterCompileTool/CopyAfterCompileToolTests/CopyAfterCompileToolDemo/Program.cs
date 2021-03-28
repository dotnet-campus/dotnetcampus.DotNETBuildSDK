using System;
using dotnetCampus.CopyAfterCompileTool;

namespace CopyAfterCompileToolDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var binaryChopCompiler = BinaryChopCompiler.CreateInstance();
            binaryChopCompiler.CompileAllCommitAndCopy();
        }
    }
}
