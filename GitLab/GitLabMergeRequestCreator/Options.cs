using dotnetCampus.Cli;

namespace dotnetCampus.GitLabMergeRequestCreator
{
    class Options
    {
        [Option("Gitlab")]
        public string GitlabUrl { set; get; }

        [Option("Token")]
        public string GitlabToken { set; get; }

        [Option("TargetBranch")]
        public string TargetBranch { set; get; }

        [Option("Title")]
        public string Title { set; get; }

        // $CI_PROJECT_ID
        [Option("ProjectId")]
        public string ProjectId { set; get; }
    }
}