namespace dotnetCampus.DotNETBuild.Context.Exceptions
{
    /// <summary>
    /// 使用 MSBuild 构建异常
    /// </summary>
    public class MSBuildCompileException : CompileException
    {
        /// <summary>
        /// 创建使用 MSBuild 构建异常
        /// </summary>
        public MSBuildCompileException(string? message) : base(message)
        {
        }
    }
}