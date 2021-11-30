using System;
using System.Threading.Tasks;

namespace dotnetCampus.GitLabMergeRequestCreator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = dotnetCampus.Cli.CommandLine.Parse(args).As<Options>();

            options.FixWithDefaultValue();

            Console.WriteLine(options.ToString());

            await GitLabMergeRequestHelper.TryCreateMergeRequest(options);
        }
    }
}