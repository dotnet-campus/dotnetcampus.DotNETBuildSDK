using System.Collections.Generic;
using dotnetCampus.Configurations;

namespace dotnetCampus.SendEmailTask
{
    static class AppConfiguratorMerger
    {
        public static void Merge(this IAppConfigurator appConfigurator, Dictionary<string, string> dictionary,
            string configurationSectionName = "")
        {
            var configuration = appConfigurator.Default;
            if (!string.IsNullOrEmpty(configurationSectionName))
            {
                if (!configurationSectionName.EndsWith("."))
                {
                    configurationSectionName += ".";
                }
            }

            foreach (var (key, value) in dictionary)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    configuration[$"{configurationSectionName}{key}"] = value;
                }
            }
        }
    }
}