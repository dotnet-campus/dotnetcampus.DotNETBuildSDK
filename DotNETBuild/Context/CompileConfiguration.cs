using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Context
{
    /// <summary>
    /// 编译配置
    /// </summary>
    public class CompileConfiguration : Configuration
    {
        /// <summary>
        /// 获取 sln 文件路径
        /// </summary>
        public string SlnPath
        {
            set
            {
                SetValue(value);
            }
            get { return GetString(); }
        }

        public string NugetFile
        {
            set
            {
                SetValue(value);
            }
            get { return GetString(); }
        }

        public string MsBuildFile
        {
            set
            {
                SetValue(value);
            }
            get
            {
                var msbuild= GetString();

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

        public string Vs2019ProfessionalMsBuild
        {
            set => SetValue(value);
            get => GetString();
        }

        public string Vs2019EnterpriseMsBuild
        {
            set => SetValue(value);
            get => GetString();
        }

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
        /// 获取 vstest.console.exe 文件
        /// </summary>
        public string VsTestFile
        {
            set => SetValue(value);
            get => GetString();
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

        public string BuildLogDirectory
        {
            set => SetValue(value);
            get => GetString() ?? "BuildLogs";
        }

        public string BuildLogFile
        {
            set => SetValue(value);
            get => GetString();
        }
    }
}