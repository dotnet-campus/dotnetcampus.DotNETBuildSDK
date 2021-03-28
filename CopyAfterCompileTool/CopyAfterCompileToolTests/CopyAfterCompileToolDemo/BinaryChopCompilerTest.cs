using System;
using System.IO;
using System.IO.Compression;
using dotnetCampus.Configurations.Core;
using dotnetCampus.CopyAfterCompileTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace CopyAfterCompileToolDemo
{
    [TestClass]
    public class BinaryChopCompilerTest
    {
        [ContractTestCase]
        public void CompileAllCommitAndCopy()
        {
            "全新构建一次，可以将测试文件的三个commit都构建出来".Test(() =>
            {
                var testCodeFolder = GetTestCodeFolder();
                var codeDirectoryPath = Path.Combine(testCodeFolder.FullName, "Code");
                var targetFolder = Path.Combine(testCodeFolder.FullName, "TargetFolder");

                var originBuildCoin = "build.coin";
                var originBuildCoinText = File.ReadAllText(originBuildCoin);
                var buildCoinText = originBuildCoinText.Replace("CodeDirectoryPath", codeDirectoryPath).Replace("TargetDirectoryPath", targetFolder);

                var buildCoinFile = Path.Combine(testCodeFolder.FullName, "Build.coin");
                File.WriteAllText(buildCoinFile, buildCoinText);

                var fileConfigurationRepo = ConfigurationFactory.FromFile(buildCoinFile);
                var appConfigurator = fileConfigurationRepo.CreateAppConfigurator();

                var binaryChopCompiler = BinaryChopCompiler.CreateInstance(appConfigurator);
                binaryChopCompiler.CompileAllCommitAndCopy();
            });
        }

        private static DirectoryInfo GetTestCodeFolder()
        {
            // 找到任意文件夹用来解压缩
            var folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(folder);

            ZipFile.ExtractToDirectory("CopyAfterCompileToolTest.zip", folder);

            // 在 Windows 下，会自动清理
            return new DirectoryInfo(folder);
        }
    }
}
