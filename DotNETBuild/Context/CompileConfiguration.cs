using System;
using System.ComponentModel;
using System.IO;

using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Context
{
    /// <summary>
    /// 编译配置
    /// </summary>
    public class CompileConfiguration : Configuration
    {
        /// <inheritdoc />
        public CompileConfiguration() : base("")
        {
        }

        /// <summary>
        /// 获取或设置 sln 文件路径
        /// </summary>
        public string SlnPath
        {
            set
            {
                SetValue(value);
            }
            get { return GetString(); }
        }

        /// <summary>
        /// 获取或设置 MSBuild 文件路径，可自动寻找本机的 MSBuild 路径
        /// </summary>
        public string MSBuildFile
        {
#pragma warning disable CS0618
            get => MsBuildFile;
            set => MsBuildFile = value;
#pragma warning restore CS0618
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("大小写命名错误，请使用 MSBuildFile 代替")]
        public string MsBuildFile
        {
            set
            {
                SetValue(value);
            }
            get
            {
                var msbuild = GetString();

                if (!string.IsNullOrEmpty(msbuild))
                {
                    return msbuild;
                }

                if (!string.IsNullOrEmpty(Vs2019EnterpriseMsBuild))
                {
                    return Vs2019EnterpriseMsBuild;
                }

                if (!string.IsNullOrEmpty(Vs2019ProfessionalMsBuild))
                {
                    return Vs2019ProfessionalMsBuild;
                }

                if (!string.IsNullOrEmpty(Vs2019CommunityMsBuild))
                {
                    return Vs2019CommunityMsBuild;
                }

                return msbuild;
            }
        }

        public string VS2019ProfessionalMSBuild
        {
#pragma warning disable CS0618
            get => Vs2019ProfessionalMsBuild;
            set => Vs2019ProfessionalMsBuild = value;
#pragma warning restore CS0618
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("大小写命名错误，请使用 Vs2019ProfessionalMsBuild 代替")]
        public string Vs2019ProfessionalMsBuild
        {
            set => SetValue(value);
            get => GetString();
        }

        public string VS2019EnterpriseMSBuild
        {
#pragma warning disable CS0618
            get => Vs2019EnterpriseMsBuild;
            set => Vs2019EnterpriseMsBuild = value;
#pragma warning restore CS0618
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("大小写命名错误，请使用 VS2019EnterpriseMSBuild 代替")]
        public string Vs2019EnterpriseMsBuild
        {
            set => SetValue(value);
            get => GetString();
        }

        public string VS2019CommunityMSBuild
        {
            set
            {
#pragma warning disable CS0618
                Vs2019CommunityMsBuild = value;
#pragma warning restore CS0618
            }
            get
            {
#pragma warning disable CS0618
                return Vs2019CommunityMsBuild;
#pragma warning restore CS0618
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("大小写命名错误，请使用 VS2019CommunityMSBuild 代替")]
        public string Vs2019CommunityMsBuild
        {
            set
            {
                SetValue(value);
            }
            get
            {
                return GetString();
            }
        }

        /// <summary>
        /// 获取 vstest.console.exe 文件路径
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("大小写命名错误，请使用 VSTestFile 代替")]
        public string VsTestFile
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 获取 vstest.console.exe 文件路径
        /// </summary>
        public string VSTestFile
        {
#pragma warning disable CS0618
            get => VsTestFile;
            set => VsTestFile = value;
#pragma warning restore CS0618
        }

        public string CurrentCommit
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 输出文件夹
        /// </summary>
        public string OutputDirectory
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 打包输出文件夹
        /// </summary>
        public string NupkgDirectory
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 程序集记录的版本号
        /// </summary>
        public string AssemblyVersion
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 应用的版本号
        /// </summary>
        public string AppVersion
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 构建次数
        /// </summary>
        public string BuildVersion
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 代码文件夹
        /// </summary>
        public string CodeDirectory
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 项目的 Build 配置
        /// </summary>
        /// \Build
        public string BuildConfigurationDirectory
        {
            set => SetValue(value);
            // 这里特意不相对于 CodeDirectory 路径，解决在仓库里面打包某个项目找错路径
            get => GetString() ?? Path.GetFullPath("Build");
        }

        /// <summary>
        /// 混淆配置文件夹，包含 saproj 文件
        /// </summary>
        /// \Build\obfuscation
        public string ObfuscationConfigurationSaprojDirectory
        {
            set => SetValue(value);
            get => GetString() ?? Path.Combine(BuildConfigurationDirectory, "obfuscation");
        }

        /// <summary>
        /// 安装包配置文件夹
        /// </summary>
        /// \Build\Setup
        public string SetupConfigurationDirectory
        {
            set => SetValue(value);
            get => GetString() ?? Path.Combine(BuildConfigurationDirectory, "Setup");
        }

        /// <summary>
        /// 安装包工作文件夹
        /// </summary>
        /// \Build\working
        public string InstallerWorkingDirectory
        {
            set => SetValue(value);
            get => GetString() ?? Path.Combine(BuildConfigurationDirectory, "working");
        }

        /// <summary>
        /// 安装包构建文件夹
        /// </summary>
        /// \Build\working\compiling
        public string InstallerCompilingDirectory
        {
            set => SetValue(value);
            get => GetString() ?? Path.Combine(InstallerWorkingDirectory, "compiling");
        }

        /// <summary>
        /// 安装包打包文件夹，这个文件夹内容是实际被安装包打包内容
        /// </summary>
        /// \Build\working\packing
        public string InstallerPackingDirectory
        {
            set => SetValue(value);
            get => GetString() ?? Path.Combine(InstallerWorkingDirectory, "packing");
        }
    }
}