using System;
using System.Collections.Generic;
using System.Text;

using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Context
{
    /// <summary>
    /// 专门用来给 SDK 进行的配置逻辑
    /// </summary>
    public class DotNETBuildSDKConfiguration : Configuration
    {
        /// <summary>
        /// 是否需要设置写入默认的配置信息
        /// </summary>
        public bool EnableSetCommonConfiguration
        {
            set => SetValue(value);
            get => GetBoolean() ?? true;
        }
    }
}
