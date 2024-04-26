#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace dotnetCampus.DotNETBuild.Utils
{
    /// <summary>
    /// 邮件帮助类
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// 同步的方法发送邮件
        /// </summary>
        /// <param name="emailConfiguration"></param>
        /// <param name="toList">收件人列表</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        /// <param name="displaySenderName">发送时显示的发件人名</param>
        /// <param name="attachmentFileList"></param>
        public static void SendEmail(EmailConfiguration emailConfiguration, IEnumerable<string> toList,
            string subject,
            string body,
            string? displaySenderName = null,
            IEnumerable<FileInfo>? attachmentFileList = null)
        {
            // 这里的发送邮箱配置是全局配置
            var smtpServer = emailConfiguration.SmtpServer;
            var port = emailConfiguration.SmtpServerPort ?? 25; // 默认邮件 25 端口
            var userName = emailConfiguration.UserName;
            var password = emailConfiguration.Password;
            var from = userName!;

            if (string.IsNullOrEmpty(smtpServer))
            {
                throw new ArgumentException($"配置里缺少 {nameof(emailConfiguration.SmtpServer)} 的配置",
                    nameof(emailConfiguration));
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException($"配置里缺少 {nameof(emailConfiguration.UserName)} 的配置",
                    nameof(emailConfiguration));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"配置里缺少 {nameof(emailConfiguration.Password)} 的配置",
                    nameof(emailConfiguration));
            }
            displaySenderName ??= userName;

            SendEmail(smtpServer, port, userName, password, from, displaySenderName, toList, subject, body,
                attachmentFileList);
        }

        /// <summary>
        /// 同步的方法发送邮件
        /// </summary>
        /// <param name="smtpServeHost"></param>
        /// <param name="serverPort"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="from"></param>
        /// <param name="senderName">发送时显示的发件人名</param>
        /// <param name="toList">收件人列表</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        /// <param name="attachmentFileList"></param>
        public static void SendEmail(string smtpServeHost,
            int serverPort, string userName,
            string password,
            string from,
            string senderName,
            IEnumerable<string> toList,
            string subject,
            string body,
            IEnumerable<FileInfo>? attachmentFileList = null)
        {
            var networkCredential = new NetworkCredential()
            {
                UserName = userName,
                Password = password,
            };

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(from, senderName),
                Sender = new MailAddress(from, senderName),
                Subject = subject,
                Body = body,
            };

            if (attachmentFileList != null)
            {
                foreach (var file in attachmentFileList)
                {
                    mailMessage.Attachments.Add(new Attachment(file.FullName));
                }
            }

            foreach (var to in toList)
            {
                mailMessage.To.Add(to);
            }

            var smtpClient = new SmtpClient(smtpServeHost, serverPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = networkCredential,
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };

            smtpClient.Send(mailMessage);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="smtpServeHost"></param>
        /// <param name="serverPort"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="from"></param>
        /// <param name="senderName"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendEmail(string smtpServeHost,
            int serverPort, string userName,
            string password,
            string from,
            string senderName,
            string to,
            string subject,
            string body) => SendEmail(smtpServeHost, serverPort, userName, password, from, senderName,
            new List<string>()
            {
                to
            }, subject, body);
    }
}