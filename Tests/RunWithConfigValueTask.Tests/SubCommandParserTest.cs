using dotnetCampus.RunWithConfigValueTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace RunWithConfigValueTask.Tests
{
    [TestClass]
    public class SubCommandParserTest
    {
        [ContractTestCase]
        public void ParseCommandlineValue()
        {
            "在命令行中间包含 -- 的内容，可以分割命令行".Test(() =>
            {
                var commandlineArgValue = new string[] { "foo", "lindexi", "--", "f1", "doubi" };
                var (ownerCommand, runningCommand) = SubCommandParser.ParseCommandlineValue(commandlineArgValue);

                Assert.AreEqual(2, ownerCommand.Count);
                Assert.AreEqual(2, runningCommand.Count);
            });

            "读取在末尾包含 -- 的内容，可以完全视为主命令".Test(() =>
            {
                var commandlineArgValue = new string[] { "foo", "lindexi", "--" };
                var (ownerCommand, runningCommand) = SubCommandParser.ParseCommandlineValue(commandlineArgValue);

                Assert.AreEqual(2, ownerCommand.Count);
                Assert.AreEqual(0, runningCommand.Count);
            });

            "读取不包含 -- 的内容，可以完全视为具体执行的命令".Test(() =>
            {
                var commandlineArgValue = new string[] { "foo", "lindexi" };
                var (ownerCommand, runningCommand) = SubCommandParser.ParseCommandlineValue(commandlineArgValue);

                Assert.AreEqual(0, ownerCommand.Count);
                Assert.AreEqual(2, runningCommand.Count);
            });
        }
    }
}
