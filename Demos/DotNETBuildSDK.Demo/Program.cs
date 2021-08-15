using dotnetCampus.DotNETBuild.Utils;
using System;
using System.Threading.Tasks;

namespace DotNETBuildSDK.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //// 应用只是启动，没有设置日志等级，也没有输出任何日志，此时日志将会在 Main 结束输出
            //dotnetCampus.DotNETBuild.CommandLineDragonFruit
            //    .CommandLine
            //    .ExecuteAssemblyAsync(entryAssembly: typeof(Program).Assembly,
            //  args: args,
            //  "DotNETBuildSDK.Demo.FakeProgram1").Wait();

            // 应用启动，输出第一条日志，将会在输出第一条日志之前，先输出框架日志
            dotnetCampus.DotNETBuild.CommandLineDragonFruit
                .CommandLine
                .ExecuteAssemblyAsync(entryAssembly: typeof(Program).Assembly,
              args: args,
              "DotNETBuildSDK.Demo.FakeProgram2").Wait();
        }
    }

    class FakeProgram1
    {
        // 应用只是启动，没有设置日志等级，也没有输出任何日志，此时日志将会在 Main 结束输出
        static void Main(string[] args)
        {

        }
    }

    class FakeProgram2
    {
        // 应用启动，输出第一条日志，将会在输出第一条日志之前，先输出框架日志
        static void Main(string[] args)
        {
            // 预期在输出 Foo 之前，先输出框架的日志
            Log.Info("Foo");
        }
    }
}
