#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace dotnetCampus.DotNETBuild.Utils;

/// <summary>
/// 进程运行器
/// </summary>
public static class ProcessRunner
{
    /// <summary>
    /// 命令行执行命令
    /// </summary>
    /// <param name="exeName"></param>
    /// <param name="arguments"></param>
    /// <param name="workingDirectory"></param>
    /// <param name="onReceivedOutput">当收到进程输出触发</param>
    /// <returns></returns>
    public static ProcessResult ExecuteCommand(string exeName, string arguments,
        string workingDirectory = "", Action<ProcessOutputInfo>? onReceivedOutput = null)
    {
        var processStartInfo = new ProcessStartInfo
        {
            WorkingDirectory = workingDirectory,
            FileName = exeName,

            Arguments = arguments,
            // 设置为 true 那么不会将输出内容，输出内容到当前控制台
            // 设置为 false 且 RedirectStandardOutput 为 false 那么对方控制台内容将会输出到当前控制台
            CreateNoWindow = true,
            UseShellExecute = false,
            // 期望拿到输出信息，需要设置为 true 的值
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            // 编码？这就不设置了，如果还需要配置编码等，那就自己业务实现好了
            //StandardOutputEncoding = Encoding.UTF8
        };

        var processOutputInfoList = new List<ProcessOutputInfo>();

        var process = new Process();
        process.StartInfo = processStartInfo;
        // 等待调用 BeginOutputReadLine 方法，才能收到事件
        process.OutputDataReceived += (_, args) =>
        {
            var outputInfo = new ProcessOutputInfo(OutputType.StandardOutput, args.Data);
            onReceivedOutput?.Invoke(outputInfo);

            processOutputInfoList.Add(outputInfo);
        };
        process.ErrorDataReceived += (_, args) =>
        {
            var outputInfo = new ProcessOutputInfo(OutputType.StandardError, args.Data);
            onReceivedOutput?.Invoke(outputInfo);

            processOutputInfoList.Add(outputInfo);
        };

        //// 让 Exit 事件触发
        //process.EnableRaisingEvents = true;

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();
        var exitCode = 0;
        try
        {
            Debug.Assert(process.HasExited);
            exitCode = process.ExitCode;
        }
        catch (Exception)
        {
            // 也许有些进程拿不到
        }
        processOutputInfoList.TrimExcess();
        return new ProcessResult(exitCode, processOutputInfoList);
    }
}