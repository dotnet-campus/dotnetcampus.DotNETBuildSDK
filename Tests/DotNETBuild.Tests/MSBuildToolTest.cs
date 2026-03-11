using dotnetCampus.DotNETBuild.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using dotnetCampus.Configurations.Core;

namespace DotNETBuild.Tests;

//[TestClass]
public class MSBuildToolTest
{
    // 别的机器上不一定有，可能只有 dotnet 哦
    //[TestMethod]
    public void TestFindMSBuild()
    {
        var memoryConfigurationRepo = new MemoryConfigurationRepo();
        var appConfigurator = memoryConfigurationRepo.CreateAppConfigurator();
        var msBuild = new MSBuild(appConfigurator);
        var msBuildFile = msBuild.GetMSBuildFile();
        Assert.IsTrue(File.Exists(msBuildFile));
    }
}
