#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace dotnetCampus.DotNETBuild.Utils
{
    static class EmailHelper
    {
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