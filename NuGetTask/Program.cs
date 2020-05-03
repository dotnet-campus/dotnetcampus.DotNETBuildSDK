using System;
using dotnetCampus.DotNETBuild.Context;
using dotnetCampus.DotNETBuild.Utils;

namespace dotnetCampus.NuGetTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var nuGet = new NuGet(AppConfigurator.GetAppConfigurator());
            nuGet.Restore();

            // todo 支持更多命令
        }
    }
}
