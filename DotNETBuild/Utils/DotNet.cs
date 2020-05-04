using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Utils
{
    public class DotNet
    {
        private readonly IAppConfigurator _appConfigurator;

        public DotNet(IAppConfigurator appConfigurator)
        {
            _appConfigurator = appConfigurator;
        }

        /// <summary>
        /// 尝试安装 dotnet 工具，如果已经安装了，那么忽略
        /// </summary>
        /// <param name="packageNameList"></param>
        public void TryInstall(List<string> packageNameList)
        {
            foreach (var package in GetNotInstalledToolList(packageNameList))
            {
                InstallDotNETGloablTool(package);
            }
        }

        /// <summary>
        /// 安装全局 dotnet 工具
        /// </summary>
        /// <param name="packageName"></param>
        public void InstallDotNETGloablTool(string packageName) => InstallDotNETTool(packageName, isGlobalTool: true);

        /// <summary>
        /// 安装 dotnet 工具
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="isGlobalTool">如果是 true 就安装全局工具，否则安装本地工具，本地工具就是安装在某个项目的工具</param>
        public void InstallDotNETTool(string packageName, bool isGlobalTool)
        {
            // todo 判断本地工具是否添加了 tool-manifest 如果没有添加，需要先执行
            // dotnet new tool-manifest 创建清单
            ProcessCommand.ExecuteCommand("dotnet", $"tool install {(isGlobalTool ? "-g" : "")} {packageName}");
        }
         

        public IEnumerable<string> GetNotInstalledToolList(List<string> packageNameList)
        {
            // 先去掉重复的
            packageNameList = packageNameList.Distinct().ToList();

            var (success, output) = ProcessCommand.ExecuteCommand("dotnet","tool list -g");

            // 假定都是成功的
            // 只需要尝试寻找字符串是否匹配就可以判断是否已经安装了
            
            foreach (var package in packageNameList
                // 如果后面不加上一个空格，将可能没有安装的工具判断是安装
                // 也就是如果此时的 dotnetCampus.Foo 没有安装，但是 dotnetCampus.Foo2 安装了，那么判断 dotnetCampus.Foo 是安装的
                .Select(package=>package + " "))
            {
                if (!output.Contains(package, StringComparison.OrdinalIgnoreCase))
                {
                    yield return package;
                }
            }
        }
    }
}
