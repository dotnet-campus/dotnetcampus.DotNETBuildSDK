using System;
using System.Threading.Tasks;

namespace DotNETBuildSDK.Demo
{
    class Program
    {
        //应用只是启动，没有设置日志等级，也没有输出任何日志，此时日志将会在 Main 结束输出
        static void Main(string[] args)
        {
            dotnetCampus.DotNETBuild.CommandLineDragonFruit
                .CommandLine
                .ExecuteAssemblyAsync(entryAssembly: typeof(Program).Assembly,
              args: args,
              "DotNETBuildSDK.Demo.FakeProgram1").Wait();
        }
    }

    class FakeProgram1
    {
        //应用只是启动，没有设置日志等级，也没有输出任何日志，此时日志将会在 Main 结束输出
        static void Main()
        {

        }
    }
}
