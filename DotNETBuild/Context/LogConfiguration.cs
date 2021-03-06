﻿using dotnetCampus.Configurations;
using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
    public class LogConfiguration : Configuration
    {
        /// <inheritdoc />
        public LogConfiguration() : base("")
        {
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

        public LogLevel LogLevel
        {
            set => SetValue(value.ToString());
            get
            {
                var configurationString = GetString();
                if (configurationString == null)
                {
                    // 默认输出等级
                    return LogLevel.Information;
                }
                else
                {
                    if (LogLevel.TryParse(configurationString, out LogLevel logLevel))
                    {
                        return logLevel;
                    }

                    // 默认输出等级
                    return LogLevel.Information;
                }
            }
        }
    }
}