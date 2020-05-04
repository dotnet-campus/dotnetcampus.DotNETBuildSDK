using System;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace MSBuildTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var msBuild = new MsBuild(AppConfigurator.GetAppConfigurator());
            msBuild.Build();
        }
    }
}
