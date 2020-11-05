using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PickTextValueTask.Tests
{
    [TestClass]
    public class PickTextValueTaskTest
    {
        [TestMethod]
        public void MainTest()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var inputFilePath = Path.Combine(folder, "input file.txt");
            var outputFilePath = Path.Combine(folder, "output file.txt");

            var inputText = "<AppVersion>1.2.5-adsfasd</AppVersion>";
            var outputText = "<git-hash>githash</git-hash>";

            //lang=regex
            var inputRegex = @"\d+\.\d+\.\d+-([\w\d]+)";
            var replaceRegex = @"(githash)";

            File.WriteAllText(inputFilePath, inputText);
            File.WriteAllText(outputFilePath, outputText);

            // 文件写入
            Thread.Sleep(100);

            PickTextValueTask.Program.Main(new[]
            {
                "-f",
                inputFilePath,
                "-r",
                inputRegex,
                "-o",
                outputFilePath,
                "-s",
                replaceRegex
            });

            // 文件写入
            Thread.Sleep(100);

            var replacedText = File.ReadAllText(outputFilePath);
            Assert.AreEqual("<git-hash>adsfasd</git-hash>", replacedText);
        }
    }
}
