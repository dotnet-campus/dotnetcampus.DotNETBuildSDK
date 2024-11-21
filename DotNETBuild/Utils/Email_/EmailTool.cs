#nullable enable
using System;
using System.Collections.Generic;
using System.IO;

using dotnetCampus.Configurations;

using Microsoft.Extensions.Logging;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 发送邮件相关的工具
    /// </summary>
    public class EmailTool : DotNetBuildTool
    {
        public EmailTool(IAppConfigurator appConfigurator, ILogger? logger = null) : base(appConfigurator, logger)
        {
        }

        /// <summary>
        /// 同步的方法发送邮件
        /// </summary>
        /// <param name="toList">收件人列表</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        /// <param name="displaySenderName">发送时显示的发件人名</param>
        /// <param name="attachmentFileList"></param>
        public void SendEmail(IEnumerable<string> toList,
            string subject,
            string body,
            string? displaySenderName = null,
            IEnumerable<FileInfo>? attachmentFileList = null)
        {
            EmailHelper.SendEmail(AppConfigurator.Of<EmailConfiguration>(), toList, subject, body, displaySenderName, attachmentFileList);
        }
    }

    /// <summary>
    /// 发送邮件相关的工具的扩展方法
    /// </summary>
    public static class EmailToolExtensions
    {
        /// <summary>
        /// 发送邮件给 GitLab 的执行用户
        /// </summary>
        /// <param name="emailTool"></param>
        /// <param name="subject">主题</param>
        /// <param name="messageBody">内容</param>
        /// <param name="displaySenderName">发送时显示的发件人名</param>
        /// <param name="attachmentFileList"></param>
        /// <param name="shouldThrowExceptionWhenNotInGitLabCI">在非 GitLab CI 环境抛出异常。因为此时不知道发送给谁</param>
        public static void SendEmailToGitLabExecutiveUser(this EmailTool emailTool, string subject, string messageBody,
            string? displaySenderName = null,
            IEnumerable<FileInfo>? attachmentFileList = null,
            bool shouldThrowExceptionWhenNotInGitLabCI = true)
        {
            if (GitLabHelper.IsGitLabCI)
            {
                // 发送到真正触发构建的用户的邮件上，而不是此 commit 的作者的 CI_COMMIT_AUTHOR 的邮箱
                var to = GitLabHelper.ExecutiveUserEmail!;
                emailTool.SendEmail([to], subject, messageBody, displaySenderName, attachmentFileList);
            }
            else
            {
                if (shouldThrowExceptionWhenNotInGitLabCI)
                {
                    throw new InvalidOperationException("当前非 GitLab CI 环境，找不到真正触发构建的用户的邮件地址");
                }
            }
        }
    }
}