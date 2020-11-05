using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using dotnetCampus.Configurations.Core;
using dotnetCampus.DotNETBuild.Context;

namespace dotnetCampus.DotNETBuild.Utils
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// 获取当前配置
        /// <para></para>
        /// </summary>
        public static FileConfigurationRepo GetCurrentConfiguration()
        {
            var file = GetCurrentConfigurationFile();

            FileConfigurationRepo fileConfigurationRepo = ConfigurationFactory.FromFile(file.FullName);

            return fileConfigurationRepo;
        }

        public static FileInfo GetCurrentConfigurationFile()
        {
            Log.Debug("获取当前配置文件路径");
            Log.Debug($"工作路径 " + Environment.CurrentDirectory);
            string config = "build.coin";
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

        /// <summary>
        /// 设置通用的配置
        /// </summary>
        /// <param name="compile"></param>
        public static void SetCommonConfiguration(this CompileConfiguration compile)
        {
            // 属性等于属性的写法作用是用来在配置文件里面写入默认配置
            // 如果配置不存在，那么将被写入默认配置
            // 作用：先在项目根路径调用命令，此时写入了默认配置，默认配置的路径将是对的相对路径
            // 第二个命令在项目其他路径调用，此时尝试获取配置，拿到的配置是第一个任务写入的绝对路径
            // 如果没有此方法，那么第二个命令将不知道哪个是项目的根路径，无法解决我在仓库里面给某个子项目打包。如有仓库是 `lindexi` 里面包含三个不同的项目 `shi` `dou` `bi` 此时如果想要打包 `shi` 和 `dou` 两个项目，每个项目打包都是独立的，那么将无法找对项目
            compile.BuildConfigurationDirectory = compile.BuildConfigurationDirectory;
            compile.ObfuscationConfigurationSaprojDirectory = compile.ObfuscationConfigurationSaprojDirectory;
            compile.SetupConfigurationDirectory = compile.SetupConfigurationDirectory;
            compile.InstallerWorkingDirectory = compile.InstallerWorkingDirectory;
            compile.InstallerCompilingDirectory = compile.InstallerCompilingDirectory;
            compile.InstallerPackingDirectory = compile.InstallerPackingDirectory;
        }
    }
}