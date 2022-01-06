using System.IO;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 单元测试帮助类
    /// </summary>
    public class TestHelper
    {
        /// <inheritdoc />
        public TestHelper(IAppConfigurator appConfigurator)
        {
            AppConfigurator = appConfigurator;
        }

        /// <summary>
        /// 运行单元测试文件，通过 vstest.console.exe 运行
        /// </summary>
        /// <param name="file"></param>
        public void RunVsTestFile(FileInfo file)
        {
            ProcessCommand.RunCommand(GetVsTest(), ProcessCommand.ToArgumentPath(file.FullName));
        }

        /// <summary>
        /// 获取 vstest.console.exe 文件
        /// </summary>
        /// <returns></returns>
        public string GetVsTest()
        {
            if (!string.IsNullOrEmpty(CompileConfiguration.VSTestFile))
            {
                return CompileConfiguration.VSTestFile;
            }

            var msBuildFile = CompileConfiguration.MSBuildFile;
            if (string.IsNullOrEmpty(msBuildFile))
            {
                return msBuildFile;
            }

            var file = Path.Combine(msBuildFile,
                @"..\..\..\..\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe");
            file = Path.GetFullPath(file);

            CompileConfiguration.VSTestFile = file;

            return file;
        }

        public IAppConfigurator AppConfigurator { get; }

        public CompileConfiguration CompileConfiguration => AppConfigurator.Of<CompileConfiguration>();
    }
}