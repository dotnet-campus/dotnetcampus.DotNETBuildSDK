using System;
using System.IO;
using System.Linq;
using dotnetCampus.Cli;

namespace PublishFolderCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = CommandLine.Parse(args).As<Options>();

            const string libFolderName = "lib";
            var publishFolder = options.PublishFolder.Trim();
            var libFolder = Path.GetFullPath(Path.Combine(publishFolder, libFolderName));

            Directory.CreateDirectory(libFolder);

            var excludeItems = options.Exclude
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e))
                .ToList();

            var entries = Directory.GetFileSystemEntries(publishFolder);
            foreach (var entry in entries)
            {
                var name = Path.GetFileName(entry);
                if (name.Equals(libFolderName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (ShouldExclude(name, excludeItems))
                {
                    continue;
                }

                var dest = Path.Combine(libFolder, name);
                if (File.Exists(entry))
                {
                    File.Move(entry, dest);
                }
                else if (Directory.Exists(entry))
                {
                    Directory.Move(entry, dest);
                }
            }

            var appHostFilePath = Path.Combine(libFolder, options.ApplicationName + ".exe");
            var newAppHostFilePath = Path.Combine(publishFolder, options.ApplicationName + ".exe");

            File.Move(appHostFilePath, newAppHostFilePath);

            AppHostPatcher.Patch(newAppHostFilePath, Path.Combine("lib", options.ApplicationName + ".dll"));
        }

        static bool ShouldExclude(string name, System.Collections.Generic.List<string> excludeItems)
        {
            return excludeItems.Any(e =>
                name.Equals(e, StringComparison.OrdinalIgnoreCase) ||
                name.Equals(e.TrimStart('.'), StringComparison.OrdinalIgnoreCase));
        }
    }
}
