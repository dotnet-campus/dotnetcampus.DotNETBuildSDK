using System.IO;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;

namespace dotnetCampus.DotNETBuild.Utils
{
    public class MsBuild
    {
        /// <inheritdoc />
        public MsBuild(IAppConfigurator appConfigurator)
        {
            AppConfigurator = appConfigurator;
        }

        /// <summary>
        /// 执行编译
        /// </summary>
        /// <param name="parallel"></param>
        /// <param name="configuration"></param>
        /// <param name="slnPath"></param>
        /// <param name="msbuildPath"></param>
        public void Build(bool parallel = true, MsBuildConfiguration configuration = MsBuildConfiguration.Release, string slnPath = "", string msbuildPath = "")
        {
            var command = "";
            if (parallel)
            {
                command = "/m";
            }

            command += $" /p:configuration={(configuration == MsBuildConfiguration.Release ? "release" : "debug")}";

            if (string.IsNullOrEmpty(slnPath))
            {
                slnPath = CompileConfiguration.SlnPath;
            }

            command += $" {ProcessCommand.ToArgumentPath(slnPath)}";

            Build(command, msbuildPath);
        }

        /// <summary>
        /// 执行 msbuild 命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="msbuildPath">默认使用最新版本</param>
        public void Build(string command, string msbuildPath = "")
        {
            if (string.IsNullOrEmpty(msbuildPath))
            {
                msbuildPath = CompileConfiguration.MsBuildFile;
            }

            ProcessCommand.RunCommand(msbuildPath, command);
        }


        public string GetMsBuildFile()
        {
            if (!string.IsNullOrEmpty(CompileConfiguration.MsBuildFile))
            {
                return CompileConfiguration.MsBuildFile;
            }

            FindMsBuildFile();

            return CompileConfiguration.MsBuildFile;
        }

        internal void FindMsBuildFile()
        {
            var vs2019Community =
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe";

            if (File.Exists(vs2019Community))
            {
                CompileConfiguration.Vs2019CommunityMsBuild = vs2019Community;
            }

            var vs2019Enterprise =
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe";

            if (File.Exists(vs2019Enterprise))
            {
                CompileConfiguration.Vs2019EnterpriseMsBuild = vs2019Enterprise;
            }

            var vs2019Professional =
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe";

            if (File.Exists(vs2019Professional))
            {
                CompileConfiguration.Vs2019ProfessionalMsBuild = vs2019Professional;
            }


        }

        public IAppConfigurator AppConfigurator { get; }

        public CompileConfiguration CompileConfiguration => AppConfigurator.Of<CompileConfiguration>();

        public enum MsBuildConfiguration
        {
            Release,
            Debug,
        }
    }
}