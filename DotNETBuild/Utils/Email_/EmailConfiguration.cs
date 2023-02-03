#nullable enable
using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Utils
{
    public class EmailConfiguration : Configuration
    {
        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string? SmtpServer
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        public int? SmtpServerPort
        {
            set => SetValue(value);
            get => GetInt32();
        }

        public string? UserName
        {
            set => SetValue(value);
            get => GetString();
        }

        public string? Password
        {
            set => SetValue(value);
            get => GetString();
        }

        /// <summary>
        /// 发送者显示的名字
        /// </summary>
        public string? SenderDisplayName
        {
            set => SetValue(value);
            get => GetString();
        }
    }
}