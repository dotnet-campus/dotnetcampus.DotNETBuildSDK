using System;
using System.ComponentModel;
using System.IO;
using dotnetCampus.Configurations;
using dotnetCampus.DotNETBuild.Context;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 对 msbuild 命令行工具的封装
    /// </summary>
    public class MSBuild : DotNetBuildTool
    {
        /// <summary>
        /// 对 msbuild 命令行工具的封装
        /// </summary>
        public MSBuild(IAppConfigurator appConfigurator) : base(appConfigurator)
        {
        }

        /// <summary>
        /// 执行编译
        /// </summary>
        public void Build(MSBuildCommandOptions options)
        {
            var configuration = options.MSBuildConfiguration;
            var slnPath = options.SlnPath;
            var msbuildPath = options.MSBuildPath;

            var command = $"/m:{options.MaxCpuCount}";

            command += $" /p:configuration={(configuration == MSBuildConfiguration.Release ? "release" : "debug")}";

            if (options.ShouldRestore)
            {
                command += " -restore";
            }

            if (string.IsNullOrEmpty(slnPath))
            {
                slnPath = CompileConfiguration.SlnPath;
            }

            command += $" {ProcessCommand.ToArgumentPath(slnPath)}";

            Build(command, msbuildPath);
        }

        /// <summary>
        /// 执行编译
        /// </summary>
        /// <param name="parallel"></param>
        /// <param name="configuration"></param>
        /// <param name="slnPath"></param>
        /// <param name="msbuildPath"></param>
        public void Build(bool parallel = true, MSBuildConfiguration configuration = MSBuildConfiguration.Release,
            string slnPath = "", string msbuildPath = "")
        {
            Build(new MSBuildCommandOptions()
            {
                MaxCpuCount = parallel ? 4 : 1,
                MSBuildConfiguration = configuration,
                SlnPath = slnPath,
                MSBuildPath = msbuildPath
            });
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
                msbuildPath = CompileConfiguration.MSBuildFile;
            }

            ExecuteCommand(msbuildPath, command);
        }

        /// <summary>
        /// 获取 msbuild 命令行工具路径
        /// </summary>
        /// <returns></returns>
        public string GetMSBuildFile()
        {
            if (!string.IsNullOrEmpty(CompileConfiguration.MSBuildFile))
            {
                return CompileConfiguration.MSBuildFile;
            }

            FindMSBuildFile();

            return CompileConfiguration.MSBuildFile;
        }

        internal void FindMSBuildFile()
        {
            var vs2019Community =
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe";

            if (File.Exists(vs2019Community))
            {
                CompileConfiguration.VS2019CommunityMSBuild = vs2019Community;
            }

            var vs2019Enterprise =
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe";

            if (File.Exists(vs2019Enterprise))
            {
                CompileConfiguration.VS2019EnterpriseMSBuild = vs2019Enterprise;
            }

            var vs2019Professional =
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe";

            if (File.Exists(vs2019Professional))
            {
                CompileConfiguration.VS2019ProfessionalMSBuild = vs2019Professional;
            }
        }

        /// <summary>
        /// 构建配置
        /// </summary>
        public CompileConfiguration CompileConfiguration => AppConfigurator.Of<CompileConfiguration>();

        /// <summary>
        /// 构建选项
        /// </summary>
        public enum MSBuildConfiguration
        {
            /// <summary>
            /// 发布版
            /// </summary>
            Release,
            /// <summary>
            /// 调试版
            /// </summary>
            Debug,
        }
    }

    /// <summary>
    /// 对 msbuild 命令行工具的封装
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("大小写命名错误，请使用 MSBuild 代替")]
    public class MsBuild : DotNetBuildTool
    {
        /// <inheritdoc />
        public MsBuild(IAppConfigurator appConfigurator) : base(appConfigurator)
        {
        }

        /// <summary>
        /// 执行编译
        /// </summary>
        public void Build(MsBuildCommandOptions options)
        {
            var configuration = options.MsBuildConfiguration;
            var slnPath = options.SlnPath;
            var msbuildPath = options.MsBuildPath;

            var command = $"/m:{options.MaxCpuCount}";

            command += $" /p:configuration={(configuration == MsBuildConfiguration.Release ? "release" : "debug")}";

            if (options.ShouldRestore)
            {
                command += " -restore";
            }

            if (string.IsNullOrEmpty(slnPath))
            {
                slnPath = CompileConfiguration.SlnPath;
            }

            command += $" {ProcessCommand.ToArgumentPath(slnPath)}";

            Build(command, msbuildPath);
        }

        /// <summary>
        /// 执行编译
        /// </summary>
        /// <param name="parallel"></param>
        /// <param name="configuration"></param>
        /// <param name="slnPath"></param>
        /// <param name="msbuildPath"></param>
        public void Build(bool parallel = true, MsBuildConfiguration configuration = MsBuildConfiguration.Release,
            string slnPath = "", string msbuildPath = "")
        {
            Build(new MsBuildCommandOptions()
            {
                MaxCpuCount = parallel ? 4 : 1,
                MsBuildConfiguration = configuration,
                SlnPath = slnPath,
                MsBuildPath = msbuildPath
            });
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
                msbuildPath = CompileConfiguration.MSBuildFile;
            }

            ExecuteCommand(msbuildPath, command);
        }

        /// <summary>
        /// 获取 msbuild 命令行工具路径
        /// </summary>
        /// <returns></returns>
        public string GetMsBuildFile()
        {
            if (!string.IsNullOrEmpty(CompileConfiguration.MSBuildFile))
            {
                return CompileConfiguration.MSBuildFile;
            }

            FindMsBuildFile();

            return CompileConfiguration.MSBuildFile;
        }

        internal void FindMsBuildFile()
        {
            var vs2019Community =
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe";

            if (File.Exists(vs2019Community))
            {
                CompileConfiguration.VS2019CommunityMSBuild = vs2019Community;
            }

            var vs2019Enterprise =
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe";

            if (File.Exists(vs2019Enterprise))
            {
                CompileConfiguration.VS2019EnterpriseMSBuild = vs2019Enterprise;
            }

            var vs2019Professional =
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe";

            if (File.Exists(vs2019Professional))
            {
                CompileConfiguration.VS2019ProfessionalMSBuild = vs2019Professional;
            }
        }

        /// <summary>
        /// 构建配置
        /// </summary>
        public CompileConfiguration CompileConfiguration => AppConfigurator.Of<CompileConfiguration>();

        /// <summary>
        /// 构建选项
        /// </summary>
        public enum MsBuildConfiguration
        {
            /// <summary>
            /// 发布版
            /// </summary>
            Release,
            /// <summary>
            /// 调试版
            /// </summary>
            Debug,
        }
    }

#nullable enable
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("大小写命名错误，请使用 MSBuildCommandOptions 代替")]
    public class MsBuildCommandOptions
    {
        public int MaxCpuCount { get; set; } = 1;

        public MsBuild.MsBuildConfiguration MsBuildConfiguration { get; set; } = MsBuild.MsBuildConfiguration.Release;

        public string? SlnPath { get; set; }

        public string? MsBuildPath { get; set; }

        public bool ShouldRestore { set; get; } = false;
    }

    public class MSBuildCommandOptions
    {
        public int MaxCpuCount { get; set; } = 1;

        public MSBuild.MSBuildConfiguration MSBuildConfiguration { get; set; } = MSBuild.MSBuildConfiguration.Release;

        public string? SlnPath { get; set; }

        public string? MSBuildPath { get; set; }

        public bool ShouldRestore { set; get; } = false;
    }
}