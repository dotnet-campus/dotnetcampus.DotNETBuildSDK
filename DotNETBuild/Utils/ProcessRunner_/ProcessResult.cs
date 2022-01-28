using System;
using System.Collections.Generic;
using System.Linq;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 运行进程拿到的结果
    /// </summary>
    public readonly struct ProcessResult
    {
        /// <summary>
        /// 创建进程拿到的结果
        /// </summary>
        /// <param name="exitCode"></param>
        /// <param name="processOutputInfoList"></param>
        public ProcessResult(int exitCode, IReadOnlyList<ProcessOutputInfo> processOutputInfoList)
        {
            ExitCode = exitCode;
            ProcessOutputInfoList = processOutputInfoList;
        }
        /// <summary>
        /// 是否成功，进程退出返回值是 0 的值
        /// </summary>
        public bool Success => ExitCode == 0;

        /// <summary>
        /// 进程退出返回值
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// 进程输出信息
        /// </summary>
        public string OutputMessage
        {
            get
            {
                return string.Join(Environment.NewLine,
                    ProcessOutputInfoList.Select(processOutputInfo => processOutputInfo.Message));
            }
        }

        /// <summary>
        /// 进程输出信息
        /// </summary>
        public IReadOnlyList<ProcessOutputInfo> ProcessOutputInfoList { get; }

        /// <summary>
        /// 解构
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public void Deconstruct(out bool success, out string message)
        {
            success = Success;
            message = OutputMessage;
        }
    }
}