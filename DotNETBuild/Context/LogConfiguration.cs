using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Utils
{
    public class LogConfiguration : Configuration
    {
        public string BuildLogDirectory
        {
            set => SetValue(value);
            get => GetString() ?? "BuildLogs";
        }

        public string BuildLogFile
        {
            set => SetValue(value);
            get => GetString();
        }
    }
}