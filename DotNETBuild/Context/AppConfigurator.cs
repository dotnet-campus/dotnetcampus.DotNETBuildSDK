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
                throw new InvalidOperationException("必须在调用 GetAppConfigurator 方法之前设置配置文件路径，请将设置路径的代码放在程序运行最前");
            }

            _appConfigurator = ConfigurationFactory.FromFile(file.FullName).CreateAppConfigurator();
        }

        internal static void SetAppConfigurator(IAppConfigurator appConfigurator)
        {
            if (_appConfigurator != null)
            {
                throw new InvalidOperationException("必须在调用 GetAppConfigurator 方法之前设置配置，请将设置配置的代码放在程序运行最前");
            }

            _appConfigurator = appConfigurator;
        }

        /// <summary>
        /// 获取应用配置
        /// </summary>
        /// <returns></returns>
        public static IAppConfigurator GetAppConfigurator()
        {
            if (_appConfigurator is null)
            {
                Log.Info("没有设置应用配置，自动寻找应用配置");

                var fileConfigurationRepo = ConfigurationHelper.GetCurrentConfiguration();
                _appConfigurator = fileConfigurationRepo.CreateAppConfigurator();
            }

            return _appConfigurator;
        }

        internal static IAppConfigurator InitAppConfigurator()
        {
            if (_appConfigurator is null)
            {
                var fileConfigurationRepo = ConfigurationHelper.GetCurrentConfiguration();
                _appConfigurator = fileConfigurationRepo.CreateAppConfigurator();
            }

            return _appConfigurator;
        }

        private static IAppConfigurator _appConfigurator;
    }
}