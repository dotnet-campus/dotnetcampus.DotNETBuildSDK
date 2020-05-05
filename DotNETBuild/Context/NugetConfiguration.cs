using dotnetCampus.Configurations;

namespace dotnetCampus.DotNETBuild.Context
{
    public class NugetConfiguration : Configuration
    {
        public string ApiKey
        {
            set => SetValue(value);
            get => GetString();
        }

        public string Source
        {
            set => SetValue(value);
            get => GetString();
        }

    }
}