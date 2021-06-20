using System.Collections.Generic;

namespace dotnetCampus.RunWithConfigValueTask
{
    public static class SubCommandParser
    {
        public static (List<string> ownerCommand, List<string> runningCommand) ParseCommandlineValue(string[] args)
        {
            // 如果存在 -- 那么 -- 之后的内容就是命令行了
            const string splitValue = "--";
            List<string> ownerCommand = new List<string>();

            foreach (var arg in args)
            {
                if (arg == splitValue)
                {
                    break;
                }

                ownerCommand.Add(arg);
            }

            if (ownerCommand.Count == args.Length)
            {
                // 如果长度相同，也就是说没有存在 -- 符号
                // 按照需求，全部的内容都作为执行命令
                var runningCommand = ownerCommand;
                return (new List<string>(0), runningCommand);
            }
            else
            {
                var runningCommand = new List<string>();
                for (var i = ownerCommand.Count + 1; i < args.Length; i++)
                {
                    runningCommand.Add(args[i]);
                }

                return (ownerCommand, runningCommand);
            }
        }
    }
}