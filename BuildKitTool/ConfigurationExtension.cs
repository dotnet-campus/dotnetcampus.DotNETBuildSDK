using System;
using System.Collections.Generic;
using System.IO;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;
using dotnetCampus.DotNETBuild.Utils;

namespace BuildKitTool
{
    static class ConfigurationExtension
    {
        /// <summary>
        /// 合并配置文件
        /// </summary>
        /// <param name="option"></param>
        /// <param name="configuration"></param>
        public static FileConfigurationRepo MergeConfiguration(InitOption option, ConfigurationEnum configuration)
        {
            // 放在机器的配置
            var globalConfigurationFile = GetGlobalConfigurationFile();

            Dictionary<string, string> machineConfiguration=new Dictionary<string, string>();
            if (!globalConfigurationFile.Exists)
            {
                Log.Debug("没有找到机器配置" + globalConfigurationFile.FullName);
                // 不存在机器配置
            }
            else
            {
                // 规则是如果当前存在的，那么就不从机器获取
                machineConfiguration = CoinConfigurationSerializer.Deserialize(File.ReadAllText(globalConfigurationFile.FullName));
            }

            var currentConfiguration = GetBuildConfiguration();
            foreach (var (key, value) in machineConfiguration)
            {
                if (!currentConfiguration.ContainsKey(key))
                {
                    currentConfiguration[key] = value;
                }
            }

            SetLogLevel(configuration, currentConfiguration);

            // 下面代码只是让配置文件里面可以告诉小伙伴这个文件是做什么
            currentConfiguration["AAA须知"] = "此文件为构建过程多个命令共享信息使用，请不要加入代码仓库";

            // 序列化写入
            var fileConfiguration = ConfigurationHelper.GetCurrentConfiguration();
            IConfigurationRepo configurationRepo = fileConfiguration;
            
            foreach (var (key, value) in currentConfiguration)
            {
                configurationRepo.SetValue(key, value);
            }

            return fileConfiguration;
        }

        private static void SetLogLevel(ConfigurationEnum configuration, Dictionary<string, string> currentConfiguration)
        {
            // 如果全局配置和项目配置里面没有写入日志默认配置
            var logLevel = "Log.LogLevel";
            if (!currentConfiguration.ContainsKey(logLevel))
            {
                // 根据配置进行写入
                if (configuration == ConfigurationEnum.Release)
                {
                    currentConfiguration[logLevel] = LogLevel.Info.ToString();
                }
                else
                {
                    currentConfiguration[logLevel] = LogLevel.Debug.ToString();
                }
            }
        }

        static Dictionary<string, string> GetBuildConfiguration()
        {
            var buildConfigurationFile = GetBuildConfigurationFile();
            if (buildConfigurationFile.Exists)
            {
                return CoinConfigurationSerializer.Deserialize(File.ReadAllText(buildConfigurationFile.FullName));
            }

            return new Dictionary<string, string>();
        }

        /// <summary>
        /// 存放在当前项目的 Build 文件夹的 build.coin 文件
        /// </summary>
        /// <returns></returns>
        static FileInfo GetBuildConfigurationFile()
        {
            Log.Debug("获取构建配置文件");
            Log.Debug($"工作路径 " + Environment.CurrentDirectory);
            string config = "build.coin";
            config = Path.Combine("Build", config);
            config = Path.GetFullPath(config);
            Log.Debug($"配置文件路径" + config);
            return new FileInfo(config);
        }


        /// <summary>
        /// 获取机器级配置文件
        /// </summary>
        public static FileInfo GetGlobalConfigurationFile()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder = Path.Combine(folder, "dotnet campus", "BuildKit");
            var file = new FileInfo(Path.Combine(folder, "configuration.coin"));
            return file;
        }
    }
}