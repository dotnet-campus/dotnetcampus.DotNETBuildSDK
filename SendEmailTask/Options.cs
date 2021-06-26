using dotnetCampus.Cli;

namespace dotnetCampus.SendEmailTask
{
    internal class Options
    {
        /// <summary>
        /// 使用 ; 分割多个不同的命令
        /// </summary>
        [Option(ToCommand, nameof(To))]
        public string? To { get; set; }

        public const char ToCommand = 't';
        public const char SubjectCommand = 's';
        public const char BodyCommand = 'b';

        /// <summary>
        /// 邮件标题
        /// </summary>
        [Option(SubjectCommand, nameof(Subject))]
        public string? Subject { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        [Option(BodyCommand, nameof(Body))]
        public string? Body { get; set; }

        /// <summary>
        /// 邮件服务器
        /// </summary>
        [Option(nameof(SmtpServer))]
        public string? SmtpServer { get; set; }

        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        [Option(nameof(SmtpServerPort))]
        public int? SmtpServerPort { get; set; }

        [Option(nameof(UserName))]
        public string? UserName { get; set; }

        [Option(nameof(Password))]
        public string? Password { get; set; }

        /// <summary>
        /// 附加的文件列表，多个文件之间使用 `|` 符号分割。如 C:\lindexi.txt|C:\doubi.txt 表示两个文件
        /// </summary>
        [Option(nameof(Files))]
        public string? Files { get; set; }

        /// <summary>
        /// 发送者显示的名字
        /// </summary>
        [Option(nameof(SenderDisplayName))]
        public string? SenderDisplayName { get; set; }
    }
}