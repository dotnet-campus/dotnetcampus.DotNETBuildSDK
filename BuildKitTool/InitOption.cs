using CommandLine;

namespace BuildKitTool
{
    [Verb("init", HelpText = "初始化")]
    public class InitOption
    {
        [Option('c', "Configuration", Required = false, HelpText = "The configuration should be debug or release")]
        public string Configuration { set; get; }
    }
}