using System;
using System.Linq;
using dotnetCampus.DotNETBuild.Context;

namespace dotnetCampus.SendEmailTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandLine = dotnetCampus.Cli.CommandLine.Parse(args);

            var dictionary = commandLine.ToDictionary();

            // 获取设备配置，要求执行 dotnet buildkit init 命令，执行之后才会将设备的配置和项目的配置 Merge 因此单独执行这个应用，也许会找不到用户名等
            var appConfigurator = AppConfigurator.GetAppConfigurator();

            appConfigurator.Merge(dictionary, "Email");

            var emailConfiguration = appConfigurator.Of<EmailConfiguration>();

            var smtpServer = emailConfiguration.SmtpServer;
            if (string.IsNullOrEmpty(smtpServer))
            {
                throw new ArgumentException($"找不到邮件 smtp 服务器地址，是否忘记调用 dotnet buildkit init 读取设备的配置。可在命令行传入 --{nameof(Options.SmtpServer)} 指定邮件服务器地址");
            }

            // 忽略了……
            var port = emailConfiguration.SmtpServerPort;
            // 忽略对用户名和密码的判断
            var userName = emailConfiguration.UserName;
            var password = emailConfiguration.Password;
            var displayName = emailConfiguration.SenderDisplayName;
            var from = userName;

            if (string.IsNullOrEmpty(displayName))
            {
                displayName = userName;
            }

            var options = commandLine.As<Options>();
            var to = options.To;
            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException($"必须通过 -t 或 --to 指定邮件接收方，多个接收方可采用 ; 分割");
            }

            Console.WriteLine($"通过 {smtpServer}:{port} 发送 {options.Subject} 到 {to}");

            if (string.IsNullOrEmpty(options.Subject))
            {
                throw new ArgumentException($"邮件主题不能为空，请使用 -{Options.SubjectCommand} 或 --{nameof(Options.Subject)}指定邮件主题");
            }

            var toList = to.Split(';')
                // 使用 Trim()，因为经常写 ` ;` 或者 `; ` 的字符串
                .Select(temp => temp.Trim());

            EmailHelper.SendEmail(smtpServer, port!.Value, userName!, password!,
                from!,
                displayName!,
                toList,
                options.Subject!,
                options.Body ?? string.Empty);
        }
    }
}