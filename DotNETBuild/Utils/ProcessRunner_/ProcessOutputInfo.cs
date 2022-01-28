#nullable enable
using System;

namespace dotnetCampus.DotNETBuild.Utils;

/// <summary>
/// 进程输出消息
/// </summary>
public readonly struct ProcessOutputInfo
{
    /// <summary>
    /// 创建进程输出消息
    /// </summary>
    /// <param name="outputType"></param>
    /// <param name="message"></param>
    public ProcessOutputInfo(OutputType outputType, string? message)
    {
        OutputType = outputType;
        Message = message;

        OutputTime = DateTime.Now;
    }

    /// <summary>
    /// 输出类型
    /// </summary>
    public OutputType OutputType { get; }

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// 何时输出
    /// </summary>
    public DateTimeOffset OutputTime { get; }
}