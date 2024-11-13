using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
                SetFullPath(value);
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
                SetFullPath(value);
            }
            get
            {
                // 如果有定义过采用哪个 MSBuild 文件，那就优先使用定义的
                // 否则使用本机安装的最高版本的 VS 作为返回值
                var msbuild = GetString();

                return GetMSBuildPath
                (
                    msbuild,
                    VS2022EnterpriseMSBuild,
                    VS2022ProfessionalMSBuild,
                    VS2022CommunityMSBuild,
                    Vs2019EnterpriseMsBuild,
                    Vs2019ProfessionalMsBuild,
                    Vs2019CommunityMsBuild
                );

                static string GetMSBuildPath(params string[] paths)
                {
                    return paths.FirstOrDefault(path => !string.IsNullOrEmpty(path));
                }
            }
        }

        /// <summary>
        /// VisualStudio 2019 专业版的 MSBuild.exe 文件所在路径
        /// </summary>
        public string VS2019ProfessionalMSBuild
        {
#pragma warning disable CS0618
            get => Vs2019ProfessionalMsBuild;
            set => Vs2019ProfessionalMsBuild = value;
#pragma warning restore CS0618
        }

        /// <summary>
        /// VisualStudio 2019 专业版的 MSBuild.exe 文件所在路径
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("大小写命名错误，请使用 VS2019ProfessionalMSBuild 代替")]
        public string Vs2019ProfessionalMsBuild
        {
            set => SetFullPath(value);
            get => GetString();
        }

        /// <summary>
        /// VisualStudio 2019 企业版的 MSBuild.exe 文件所在路径
        /// </summary>
        public string VS2019EnterpriseMSBuild
        {
#pragma warning disable CS0618
            get => Vs2019EnterpriseMsBuild;
            set => Vs2019EnterpriseMsBuild = value;
#pragma warning restore CS0618
        }

        /// <summary>
        /// VisualStudio 2019 企业版的 MSBuild.exe 文件所在路径
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("大小写命名错误，请使用 VS2019EnterpriseMSBuild 代替")]
        public string Vs2019EnterpriseMsBuild
        {
            set => SetFullPath(value);
            get => GetString();
        }

        /// <summary>
        /// VisualStudio 2019 社区版的 MSBuild.exe 文件所在路径
        /// </summary>
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

        /// <summary>
        /// VisualStudio 2019 社区版的 MSBuild.exe 文件所在路径
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("大小写命名错误，请使用 VS2019CommunityMSBuild 代替")]
        public string Vs2019CommunityMsBuild
        {
            set
            {
                SetFullPath(value);
            }
            get
            {
                return GetString();
            }
        }

        /// <summary>
        /// VisualStudio 2022 企业版的 MSBuild.exe 文件所在路径
        /// </summary>
        public string VS2022EnterpriseMSBuild
        {
            set => SetFullPath(value);
            get => GetString();
        }

        /// <summary>
        /// VisualStudio 2022 专业版的 MSBuild.exe 文件所在路径
        /// </summary>
        public string VS2022ProfessionalMSBuild
        {
            set => SetFullPath(value);
            get => GetString();
        }

        /// <summary>
        /// VisualStudio 2022 社区版的 MSBuild.exe 文件所在路径
        /// </summary>
        public string VS2022CommunityMSBuild
        {
            set => SetFullPath(value);
            get => GetString();
        }

        /// <summary>
        /// 获取 vstest.console.exe 文件路径
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("大小写命名错误，请使用 VSTestFile 代替")]
        public string VsTestFile
        {
            set => SetFullPath(value);
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

        /// <inheritdoc cref="GitConfiguration.CurrentCommit"/>
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
            set => SetFullPath(value);
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
        /// 设置或获取开始构建的时间
        /// </summary>
        public DateTime BuildStartTime
        {
            set => SetValue(value.ToString(CultureInfo.InvariantCulture));
            get
            {
                var time = GetString();
                if (time is null)
                {
                    var dateTime = DateTime.Now;
                    SetValue(dateTime.ToString(CultureInfo.InvariantCulture));
                    return dateTime;
                }
                else
                {
                    if (!DateTime.TryParse(time, out var dateTime))
                    {
                        dateTime = DateTime.Now;
                        SetValue(dateTime.ToString(CultureInfo.InvariantCulture));
                    }

                    return dateTime;
                }
            }
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
            set => SetFullPath(value);
            get => GetString();
        }

        /// <summary>
        /// 项目的 Build 配置文件夹，默认是当前工作路径下的 Build 文件夹
        /// </summary>
        /// \Build
        public string BuildConfigurationDirectory
        {
            set => SetFullPath(value);
            // 这里特意不相对于 CodeDirectory 路径，解决在仓库里面打包某个项目找错路径
            get => GetString() ?? Path.GetFullPath("Build");
        }

        /// <summary>
        /// 混淆配置文件夹，包含 saproj 文件，默认是 <value>build\obfuscation</value> 文件夹
        /// </summary>
        /// \Build\obfuscation
        public string ObfuscationConfigurationSaprojDirectory
        {
            set => SetFullPath(value);
            get => GetString() ?? Path.Combine(BuildConfigurationDirectory, "obfuscation");
        }

        /// <summary>
        /// 安装包配置文件夹，默认是 <value>\Build\Setup</value> 文件夹
        /// </summary>
        public string SetupConfigurationDirectory
        {
            set => SetFullPath(value);
            get => GetString() ?? Path.Combine(BuildConfigurationDirectory, "Setup");
        }

        /// <summary>
        /// 安装包工作文件夹 默认是 <value>build\working</value> 文件夹
        /// </summary>
        /// \Build\working
        public string InstallerWorkingDirectory
        {
            set => SetFullPath(value);
            get => GetString() ?? Path.Combine(BuildConfigurationDirectory, "working");
        }

        /// <summary>
        /// 安装包构建文件夹
        /// </summary>
        /// \Build\working\compiling
        public string InstallerCompilingDirectory
        {
            set => SetFullPath(value);
            get => GetString() ?? Path.Combine(InstallerWorkingDirectory, "compiling");
        }

        /// <summary>
        /// 安装包打包文件夹，这个文件夹内容是实际被安装包打包内容
        /// </summary>
        /// \Build\working\packing
        public string InstallerPackingDirectory
        {
            set => SetFullPath(value);
            get => GetString() ?? Path.Combine(InstallerWorkingDirectory, "packing");
        }

        private void SetFullPath(string path, [CallerMemberName] string key = "")
        {
            var value = string.IsNullOrEmpty(path) ? path : Path.GetFullPath(path);
            SetValue(value, key);
        }
    }
}