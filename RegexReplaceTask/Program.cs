using dotnetCampus.Cli;

namespace dotnetCampus.RegexReplaceTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = dotnetCampus.Cli.CommandLine.Parse(args).As<Options>();

        }

        class Options
        {
            [Option('v', "Value")]
            public string Value { set; get; }

            [Option('r', "Regex")]
            public string ReplaceRegex { set; get; }

            [Option('f', "File")]
            public string FilePath { set; get; }
        }
    }
}
