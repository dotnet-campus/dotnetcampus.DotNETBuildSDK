using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using dotnetCampus.BuildMd5Task;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace PickTextValueTask.Tests
{
    [TestClass]
    public class BuildMd5TaskTest
    {
        [ContractTestCase]
        public void BuildFolderMd5WithMultiSearchPatternTest()
        {
            "传入通配符，将会校验符合通配符的文件".Test(() =>
            {
                // 使用当前文件夹
                var directory = new DirectoryInfo(".");
                var outputFile = Options.DefaultOutputFileName;

                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }

                var multiSearchPattern = @"*.dll|*.exe";

                Md5Provider.BuildFolderAllFilesMd5(directory, outputFile, multiSearchPattern);
                // 等待校验文件写入
                Thread.Sleep(1000);

                var checksumFile = new FileInfo(outputFile);
                var xmlSerializer = new XmlSerializer(typeof(List<FileMd5Info>));
                using var fileStream = checksumFile.OpenRead();
                var fileMd5InfoList = (List<FileMd5Info>)xmlSerializer.Deserialize(fileStream);

                // 默认存在 exe 和 dll 文件
                var existExe = fileMd5InfoList.Any(temp =>
                    Path.GetExtension(temp.RelativeFilePath).Equals(".exe", StringComparison.OrdinalIgnoreCase));
                var existDll = fileMd5InfoList.Any(temp =>
                    Path.GetExtension(temp.RelativeFilePath).Equals(".dll", StringComparison.OrdinalIgnoreCase));
                var existPdb = fileMd5InfoList.Any(temp =>
                    Path.GetExtension(temp.RelativeFilePath).Equals(".pdb", StringComparison.OrdinalIgnoreCase));

                // 上面通配符写了 exe 和 dll 文件，不包含 pdb 文件
                Assert.AreEqual(true, existExe && existDll);
                Assert.AreEqual(false, existPdb);
            });

            "默认不传入通配符，将会校验所有文件".Test(() =>
            {
                // 使用当前文件夹
                var directory = new DirectoryInfo(".");
                var outputFile = Options.DefaultOutputFileName;

                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }

                Md5Provider.BuildFolderAllFilesMd5(directory, outputFile);
                // 等待校验文件写入
                Thread.Sleep(1000);

                var checksumFile = new FileInfo(outputFile);
                var xmlSerializer = new XmlSerializer(typeof(List<FileMd5Info>));
                using var fileStream = checksumFile.OpenRead();
                var fileMd5InfoList = (List<FileMd5Info>)xmlSerializer.Deserialize(fileStream);

                // 默认存在 exe 和 dll 文件
                var existExe = fileMd5InfoList.Any(temp =>
                    Path.GetExtension(temp.RelativeFilePath).Equals(".exe", StringComparison.OrdinalIgnoreCase));
                var existDll = fileMd5InfoList.Any(temp =>
                     Path.GetExtension(temp.RelativeFilePath).Equals(".dll", StringComparison.OrdinalIgnoreCase));
                var existPdb = fileMd5InfoList.Any(temp =>
                    Path.GetExtension(temp.RelativeFilePath).Equals(".pdb", StringComparison.OrdinalIgnoreCase));

                Assert.AreEqual(true, existExe && existDll && existPdb);
            });
        }

        [ContractTestCase]
        public void BuildFolderMd5Test()
        {
            "将输出文件夹的所有文件创建校验文件，可以重新校验成功".Test(() =>
            {
                // 使用当前文件夹
                var directory = new DirectoryInfo(".");
                var outputFile = Options.DefaultOutputFileName;

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
                Assert.AreEqual(true, verifyResult.AreAllMatched);
            });

            "将输出文件夹的所有文件创建校验文件然后修改某个文件，可以输出被更改的文件".Test(() =>
            {
                // 使用当前文件夹
                var directory = new DirectoryInfo(".");
                var outputFile = Options.DefaultOutputFileName;

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
                Assert.AreEqual(false, verifyResult.AreAllMatched);
                Assert.AreEqual(testFile, verifyResult.NoMatchedFileInfoList[0].RelativeFilePath);
            });

            "将输出文件夹的所有文件创建校验文件然后删除某个文件，可以输出被删除的文件".Test(() =>
            {
                // 使用当前文件夹
                var directory = new DirectoryInfo(".");
                var outputFile = Options.DefaultOutputFileName;

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
                Assert.AreEqual(false, verifyResult.AreAllMatched);
                Assert.AreEqual(testFile, verifyResult.NoMatchedFileInfoList[0].RelativeFilePath);
                Assert.AreEqual(true, verifyResult.NoMatchedFileInfoList[0].IsNotFound);
            });
        }
    }
}