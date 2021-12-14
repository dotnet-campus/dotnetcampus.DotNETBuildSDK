using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTest.Extensions.Contracts;

namespace RegexReplaceTask.Tests
{
    [TestClass]
    public class RegexReplaceTest
    {
        [ContractTestCase]
        public void RegexReplace()
        {
            "传入文件和正则替换内容，可以将内容进行替换".Test(() =>
            {
                var content = "1.2.$GitRevision$";
                var file = Path.GetTempFileName();
                File.WriteAllText(file, content);

                var argList = new[]
                {
                    "-v",
                    "Foo",
                    "-r",
                    @"\d+\.\d+\.(\$GitRevision\$)",
                    "-f",
                    file
                };

                dotnetCampus.RegexReplaceTask.Program.Main(argList);

                content = File.ReadAllText(file);
                Assert.AreEqual("1.2.Foo", content);
            });
        }
    }
}
