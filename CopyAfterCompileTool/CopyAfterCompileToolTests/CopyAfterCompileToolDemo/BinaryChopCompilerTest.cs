using System;
using System.Diagnostics;
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
            "可以设置如果构建过的 commit 可以再次构建".Test(() =>
            {
                var testCodeFolder = GetTestCodeFolder();
                var codeDirectoryPath = Path.Combine(testCodeFolder.FullName, "Code");
                var targetFolder = Path.Combine(testCodeFolder.FullName, "TargetFolder");

                // 给其中两个 commit 添加上空白内容
                var commitFolder = Path.Combine(targetFolder, "7eb048730fbedd26b691271349088b5c5f6b39d3");
                Directory.CreateDirectory(commitFolder);
                // 写入一些垃圾
                var commitFile = Path.Combine(commitFolder, "垃圾.txt");
                File.WriteAllText(commitFile, "垃圾");
                Directory.CreateDirectory(Path.Combine(targetFolder, "693fec8423b3a6c5cc760d345dbd668a69c03c81"));

                var originBuildCoin = "build.coin";
                var originBuildCoinText = File.ReadAllText(originBuildCoin);
                var buildCoinText = originBuildCoinText.Replace("CodeDirectoryPath", codeDirectoryPath).Replace("TargetDirectoryPath", targetFolder);
                // 设置可以再次构建
                buildCoinText += "\n>\nOverwriteCompiledCommit\ntrue\n>";

                var buildCoinFile = Path.Combine(testCodeFolder.FullName, "Build.coin");
                File.WriteAllText(buildCoinFile, buildCoinText);

                var fileConfigurationRepo = ConfigurationFactory.FromFile(buildCoinFile);
                var appConfigurator = fileConfigurationRepo.CreateAppConfigurator();

                var binaryChopCompiler = BinaryChopCompiler.CreateInstance(appConfigurator);
                binaryChopCompiler.CompileAllCommitAndCopy();

                AssertTrue(File.Exists(Path.Combine(targetFolder, "7eb048730fbedd26b691271349088b5c5f6b39d3", "BuildLog.txt")));
                AssertTrue(File.Exists(Path.Combine(targetFolder, "693fec8423b3a6c5cc760d345dbd668a69c03c81", "BuildLog.txt")));
                AssertTrue(File.Exists(Path.Combine(targetFolder, "964568b1bbc042b1a5f4d3109103022d9d089b0d", "BuildLog.txt")));
            });

            "默认可以设置如果存在了 commit 对应的文件夹，那么就不再重复构建".Test(() =>
            {
                var testCodeFolder = GetTestCodeFolder();
                var codeDirectoryPath = Path.Combine(testCodeFolder.FullName, "Code");
                var targetFolder = Path.Combine(testCodeFolder.FullName, "TargetFolder");

                // 给其中两个 commit 添加上空白内容
                Directory.CreateDirectory(Path.Combine(targetFolder, "7eb048730fbedd26b691271349088b5c5f6b39d3"));
                Directory.CreateDirectory(Path.Combine(targetFolder, "693fec8423b3a6c5cc760d345dbd668a69c03c81"));

                var originBuildCoin = "build.coin";
                var originBuildCoinText = File.ReadAllText(originBuildCoin);
                var buildCoinText = originBuildCoinText.Replace("CodeDirectoryPath", codeDirectoryPath).Replace("TargetDirectoryPath", targetFolder);

                var buildCoinFile = Path.Combine(testCodeFolder.FullName, "Build.coin");
                File.WriteAllText(buildCoinFile, buildCoinText);

                var fileConfigurationRepo = ConfigurationFactory.FromFile(buildCoinFile);
                var appConfigurator = fileConfigurationRepo.CreateAppConfigurator();

                var binaryChopCompiler = BinaryChopCompiler.CreateInstance(appConfigurator);
                binaryChopCompiler.CompileAllCommitAndCopy();

                // 这两个都是不构建的
                AssertTrue(!File.Exists(Path.Combine(targetFolder, "7eb048730fbedd26b691271349088b5c5f6b39d3", "BuildLog.txt")));
                AssertTrue(!File.Exists(Path.Combine(targetFolder, "693fec8423b3a6c5cc760d345dbd668a69c03c81", "BuildLog.txt")));

                // 只构建一个
                AssertTrue(File.Exists(Path.Combine(targetFolder, "964568b1bbc042b1a5f4d3109103022d9d089b0d", "BuildLog.txt")));
            });

            "如果设置 LastCommit 为第二次，那么只构建出最后一个 commit 代码".Test(() =>
            {
                var testCodeFolder = GetTestCodeFolder();
                var codeDirectoryPath = Path.Combine(testCodeFolder.FullName, "Code");
                var targetFolder = Path.Combine(testCodeFolder.FullName, "TargetFolder");

                var originBuildCoin = "build.coin";
                var originBuildCoinText = File.ReadAllText(originBuildCoin);
                var buildCoinText = originBuildCoinText.Replace("CodeDirectoryPath", codeDirectoryPath).Replace("TargetDirectoryPath", targetFolder);
                buildCoinText += "\n>\nLastCommit\n693fec8423b3a6c5cc760d345dbd668a69c03c81\n>";

                var buildCoinFile = Path.Combine(testCodeFolder.FullName, "Build.coin");
                File.WriteAllText(buildCoinFile, buildCoinText);

                var fileConfigurationRepo = ConfigurationFactory.FromFile(buildCoinFile);
                var appConfigurator = fileConfigurationRepo.CreateAppConfigurator();

                var binaryChopCompiler = BinaryChopCompiler.CreateInstance(appConfigurator);
                binaryChopCompiler.CompileAllCommitAndCopy();

                // 这两个都是不构建的
                AssertTrue(!Directory.Exists(Path.Combine(targetFolder, "7eb048730fbedd26b691271349088b5c5f6b39d3")));
                AssertTrue(!Directory.Exists(Path.Combine(targetFolder, "693fec8423b3a6c5cc760d345dbd668a69c03c81")));

                // 只构建一个
                AssertTrue(Directory.Exists(Path.Combine(targetFolder, "964568b1bbc042b1a5f4d3109103022d9d089b0d")));
            });

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

                // 这里逻辑和本地环境相关，因此不适合使用 Assert 方法
                AssertTrue(Directory.Exists(Path.Combine(targetFolder, "7eb048730fbedd26b691271349088b5c5f6b39d3")));
                AssertTrue(Directory.Exists(Path.Combine(targetFolder, "693fec8423b3a6c5cc760d345dbd668a69c03c81")));
                AssertTrue(Directory.Exists(Path.Combine(targetFolder, "964568b1bbc042b1a5f4d3109103022d9d089b0d")));
            });
        }

        private void AssertTrue(bool value)
        {
            if (!value)
            {
                Debugger.Break();
            }
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
