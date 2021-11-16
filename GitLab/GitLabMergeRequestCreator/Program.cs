using System.Threading.Tasks;

namespace dotnetCampus.GitLabMergeRequestCreator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = dotnetCampus.Cli.CommandLine.Parse(args).As<Options>();
            await GitLabMergeRequestHelper.TryCreateMergeRequest(options);
        }
    }
}