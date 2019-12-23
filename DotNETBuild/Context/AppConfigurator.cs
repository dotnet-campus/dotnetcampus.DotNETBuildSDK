using System;
using System.IO;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;
using dotnetCampus.DotNETBuild.Utils;

namespace dotnetCampus.DotNETBuild.Context
{
    /// <summary>
    /// 应用配置
    /// </summary>
    public static class AppConfigurator
    {
        /// <summary>
        /// 设置配置文件路径
        /// </summary>
        /// <param name="file"></param>
        public static void SetConfigurationFile(FileInfo file)
        {
            if (_appConfigurator != null)
            {
                throw new Exception("必须在调用 GetAppConfigurator 方法之前设置配置文件路径，请将设置路径的代码放在程序运行最前");
            }

            ConfigurationFile = file;
        }

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static FileInfo ConfigurationFile { get; private set; }

        public static IAppConfigurator GetAppConfigurator()
        {
            if (_appConfigurator is null)
            {
                Log.Info("没有设置应用配置，自动寻找应用配置");

                Log.Info($"工作路径 " + Environment.CurrentDirectory);
                string config = "build.fkv";
                config = Path.GetFullPath(config);
                Log.Info($"配置文件路径" + config);

                ConfigurationFile = new FileInfo(config);

                var fileConfigurationRepo = new FileConfigurationRepo(ConfigurationFile.FullName);
                _appConfigurator = fileConfigurationRepo.CreateAppConfigurator();
            }

            return _appConfigurator;
        }

        private static IAppConfigurator _appConfigurator;
    }
}