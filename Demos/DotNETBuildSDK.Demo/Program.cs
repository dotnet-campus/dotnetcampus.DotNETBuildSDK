using dotnetCampus.DotNETBuild;
using dotnetCampus.DotNETBuild.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotNETBuildSDK.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            dotnetCampus.DotNETBuild.CommandLineDragonFruit
                .CommandLine
                .ExecuteAssemblyAsync(entryAssembly: typeof(Program).Assembly,
              args: args,
            // 应用只是启动，没有设置日志等级，也没有输出任何日志，此时日志将会在 Main 结束输出
            //"DotNETBuildSDK.Demo.FakeProgram1").Wait();

            // 应用启动，输出第一条日志，将会在输出第一条日志之前，先输出框架日志
            //"DotNETBuildSDK.Demo.FakeProgram2").Wait();

            // 应用启动，设置日志等级，可以让框架输出的日志设置等级
            // - 只是设置日志等级，不做任何输出
            // - 日志等级是最低，可以输出所有框架的日志
            //"DotNETBuildSDK.Demo.FakeProgram3").Wait();

            // 应用启动，设置日志等级，可以让框架输出的日志设置等级
            // - 只是设置日志等级，不做任何输出
            // - 日志等级是最高，所有框架的日志没有可以输出的
            //"DotNETBuildSDK.Demo.FakeProgram4").Wait();

            // 应用启动，设置日志等级，可以让框架输出的日志设置等级
            // - 设置日志等级，同时输出日志
            // - 日志等级是最高，所有框架的日志没有可以输出的
            //"DotNETBuildSDK.Demo.FakeProgram5").Wait();

            // 应用启动，替换实际的日志为自己的日志
            //"DotNETBuildSDK.Demo.FakeProgram6").Wait();

            // 应用启动，清理框架先写入的日志，可以让框架先写入的日志不再输出
            "DotNETBuildSDK.Demo.FakeProgram7").Wait();
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

    class FakeProgram3
    {
        // 应用启动，设置日志等级，可以让框架输出的日志设置等级
        // - 只是设置日志等级，不做任何输出
        // - 日志等级是最低，可以输出所有框架的日志
        static void Main(string[] args)
        {
            Log.LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug;
        }
    }

    class FakeProgram4
    {
        // 应用启动，设置日志等级，可以让框架输出的日志设置等级
        // - 只是设置日志等级，不做任何输出
        // - 日志等级是最高，所有框架的日志没有可以输出的
        static void Main(string[] args)
        {
            Log.LogLevel = Microsoft.Extensions.Logging.LogLevel.Error;
        }
    }

    class FakeProgram5
    {
        // 应用启动，设置日志等级，可以让框架输出的日志设置等级
        // - 设置日志等级，同时输出日志
        // - 日志等级是最高，所有框架的日志没有可以输出的
        static void Main(string[] args)
        {
            Log.LogLevel = Microsoft.Extensions.Logging.LogLevel.Error;
            Log.Error("Foo");
        }
    }

    class FakeProgram6
    {
        // 应用启动，替换实际的日志为自己的日志
        static void Main(string[] args)
        {
            Log.SetLogger(new FakeLogger());
            Log.Info("Foo");
        }

        class FakeLogger : ILogger, IDisposable
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return this;
            }

            public void Dispose()
            {
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var message = formatter(state, exception);
                Debug.WriteLine(message);
            }
        }
    }

    class FakeProgram7
    {
        // 应用启动，清理框架先写入的日志，可以让框架先写入的日志不再输出
        static void Main(string[] args)
        {
            SDK.CleanSdkLog();
            Log.Info("Foo");
        }
    }
}
