using System;
using System.Collections.Generic;
using System.IO;
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
        public static void MergeConfiguration(InitOption option, ConfigurationEnum configuration)
        {
            // 放在机器的配置
            var machineConfigurationFile = GetMachineConfigurationFile();

            if (!machineConfigurationFile.Exists)
            {
                Log.Debug("没有找到机器配置" + machineConfigurationFile.FullName);
                // 不存在机器配置
                return;
            }
        
            var currentConfiguration = GetBuildConfiguration();

            // 规则是如果当前存在的，那么就不从机器获取
            var machineConfiguration = CoinConfigurationSerializer.Deserialize(File.ReadAllText(machineConfigurationFile.FullName));

            foreach (var (key, value) in machineConfiguration)
            {
                if (!currentConfiguration.ContainsKey(key))
                {
                    currentConfiguration[key] = value;
                }
            }

            if (configuration != ConfigurationEnum.None)
            {
                currentConfiguration["Configuration"] = configuration.ToString();
            }

            currentConfiguration["AAA须知"] = "此文件为构建过程多个命令共享信息使用，请不要加入代码仓库";

            // 序列化写入
            var currentConfigurationFile = ConfigurationHelper.GetCurrentConfigurationFile();
            File.WriteAllText(currentConfigurationFile.FullName, CoinConfigurationSerializer.Serialize(currentConfiguration));
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
        public static FileInfo GetMachineConfigurationFile()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder = Path.Combine(folder, "dotnet campus", "BuildKit");
            var file = new FileInfo(Path.Combine(folder, "configuration.coin"));
            return file;
        }
    }
}