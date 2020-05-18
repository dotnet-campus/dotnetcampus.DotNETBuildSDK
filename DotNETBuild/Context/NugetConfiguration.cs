using System;
using System.Collections.Generic;
using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Context
{
    public class NugetConfiguration : Configuration
    {
        public string ApiKey
        {
            set => SetValue(value);
            get => GetString();
        }

        public string Source
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 附加的源，尝试推送但是忽略失败
        /// <para></para>
        /// 格式： 每行一个附加的源，按 Source ApiKey 的格式，其中 ApiKey 可以忽略
        /// </summary>
        public string[] AttachedSource
        {
            set => SetValue(string.Join("\n", value));
            get
            {
                string value = GetString();
                return value.Split('\n');
            }
        }
    }
}