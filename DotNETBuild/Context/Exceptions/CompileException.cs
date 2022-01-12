#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetCampus.DotNETBuild.Context.Exceptions
{
    /// <summary>
    /// 表示构建时的异常
    /// </summary>
    public class CompileException : Exception
    {
        /// <summary>
        /// 创建构建时的异常
        /// </summary>
        public CompileException()
        {
        }

        /// <summary>
        /// 创建构建时的异常
        /// </summary>
        public CompileException(string? message) : base(message)
        {
        }

        /// <summary>
        /// 创建构建时的异常
        /// </summary>
        public CompileException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}