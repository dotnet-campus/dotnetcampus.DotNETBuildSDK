using System;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;
using dotnetCampus.RunWithConfigValueTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace RunWithConfigValueTask.Tests
{
    [TestClass]
    public class CommandlineEngineTest
    {
        [ContractTestCase]
        public void FillCommandline()
        {
            "传入的配置包含默认值，将会在配置不存在时使用默认值".Test(() =>
            {
                var commandline = new string[] { "lindexi", "$(存在配置)??F1", "$(不存在配置)??F2" };
                var memoryConfigurationRepo = new MemoryConfigurationRepo();
                var defaultConfiguration = memoryConfigurationRepo.CreateAppConfigurator().Default;
                defaultConfiguration["存在配置"] = "doubi";

                var result = CommandlineEngine.FillCommandline(commandline, defaultConfiguration);

                var expectedResult = new string[] { "lindexi", "doubi", "F2" };

                Assert.AreEqual(3, result.Length);

                for (var i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expectedResult[i], result[i]);
                }
            });

            "传入配置里面存在的内容，将被替换为配置里面的内容".Test(() =>
            {
                var commandline = new string[] { "lindexi", "$(Foo)" };

                var memoryConfigurationRepo = new MemoryConfigurationRepo();
                var defaultConfiguration = memoryConfigurationRepo.CreateAppConfigurator().Default;
                defaultConfiguration["Foo"] = "doubi";

                var result = CommandlineEngine.FillCommandline(commandline, defaultConfiguration);

                var expectedResult = new string[] { "lindexi", "doubi" };

                Assert.AreEqual(2, result.Length);

                for (var i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(expectedResult[i], result[i]);
                }
            });

            "传入配置里面不存在的内容，将会抛出错误".Test(() =>
            {
                var commandline = new string[] { "lindexi", "$(Foo)" };

                Assert.ThrowsException<ArgumentException>(() =>
                {
                    var memoryConfigurationRepo = new MemoryConfigurationRepo();
                    var defaultConfiguration = memoryConfigurationRepo.CreateAppConfigurator().Default;
                    CommandlineEngine.FillCommandline(commandline, defaultConfiguration);
                });
            });

            "输入不包含需要替换的命令，输出和原有的命令相同".Test(() =>
            {
                var commandline = new string[] { "lindexi", "foo" };

                var result = CommandlineEngine.FillCommandline(commandline, new DefaultConfiguration());

                Assert.AreEqual(2, result.Length);

                for (var i = 0; i < result.Length; i++)
                {
                    Assert.AreEqual(commandline[i], result[i]);
                }
            });
        }
    }
}