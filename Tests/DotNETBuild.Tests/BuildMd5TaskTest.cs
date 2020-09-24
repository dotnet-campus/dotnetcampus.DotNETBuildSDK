using System.IO;
using System.Threading;
using dotnetCampus.BuildMd5Task;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace PickTextValueTask.Tests
{
    [TestClass]
    public class BuildMd5TaskTest
    {
        [ContractTestCase]
        public void BuildFolderMd5Test()
        {
            "将输出文件夹的所有文件创建校验文件，可以重新校验成功".Test(() =>
            {
                // 使用当前文件夹
                var directory = new DirectoryInfo(".");
                var outputFile = Options.DefaultOutputFile;

                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }

                Md5Provider.BuildFolderAllFilesMd5(directory, outputFile);

                // 等待校验文件写入
                Thread.Sleep(1000);

                // 读取校验文件，然后测试是否文件完全相同
                var verifyResult = Md5Provider.VerifyFolderMd5(directory, new FileInfo(outputFile));
                // 预期是所有都相同
                Assert.AreEqual(true, verifyResult.IsAllMatch);
            });

            "将输出文件夹的所有文件创建校验文件然后修改某个文件，可以输出被更改的文件".Test(() =>
            {
                // 使用当前文件夹
                var directory = new DirectoryInfo(".");
                var outputFile = Options.DefaultOutputFile;

                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }

                // 加入一个将被更改的文件
                var testFile = "test.txt";
                File.WriteAllText(testFile, "逗比");

                Md5Provider.BuildFolderAllFilesMd5(directory, outputFile);

                // 修改测试文件
                File.WriteAllText(testFile, "林德熙是逗比");

                // 等待校验文件写入
                Thread.Sleep(1000);

                // 读取校验文件，然后测试是否文件完全相同
                var verifyResult = Md5Provider.VerifyFolderMd5(directory, new FileInfo(outputFile));

                // 预期是找到修改的文件
                Assert.AreEqual(false, verifyResult.IsAllMatch);
                Assert.AreEqual(testFile, verifyResult.NoMatchFileInfoList[0].File);
            });

            "将输出文件夹的所有文件创建校验文件然后删除某个文件，可以输出被删除的文件".Test(() =>
            {
                // 使用当前文件夹
                var directory = new DirectoryInfo(".");
                var outputFile = Options.DefaultOutputFile;

                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }

                // 加入一个将被更改的文件
                var testFile = "test.txt";
                File.WriteAllText(testFile, "逗比");

                Md5Provider.BuildFolderAllFilesMd5(directory, outputFile);

                // 删除测试文件
                File.Delete(testFile);

                // 等待校验文件写入
                Thread.Sleep(1000);

                // 读取校验文件，然后测试是否文件完全相同
                var verifyResult = Md5Provider.VerifyFolderMd5(directory, new FileInfo(outputFile));

                // 预期是找到修改的文件
                Assert.AreEqual(false, verifyResult.IsAllMatch);
                Assert.AreEqual(testFile, verifyResult.NoMatchFileInfoList[0].File);
                Assert.AreEqual(true, verifyResult.NoMatchFileInfoList[0].IsNotFound);
            });
        }
    }
}