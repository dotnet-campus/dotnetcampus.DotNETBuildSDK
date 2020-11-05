using System;
using System.Collections.Generic;
using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Context
{
    /// <summary>
    /// 工具配置
    /// </summary>
    public class ToolConfiguration : Configuration
    {
        /// <inheritdoc />
        public ToolConfiguration() : base("")
        {
        }

        /// <summary>
        /// Nuget路径
        /// </summary>
        public string NugetPath
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 存放私有安装的工具
        /// </summary>
        public string[] DotNETToolList
        {
            set => SetValue(string.Join(';', value));
            get
            {
                var configurationString = GetString();
                if (configurationString == null)
                {
                    return Array.Empty<string>();
                }
                else
                {
                    return ((string)configurationString).Split(';');
                }
            }
        }
    }
}
