#nullable enable
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
}