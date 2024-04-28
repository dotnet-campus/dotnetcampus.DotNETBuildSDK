using System;
using System.IO;
using System.Linq;

namespace Packaging.DebUOS.Contexts.Configurations;

public static class DebUOSConfigurationExtension
{
    public static Predicate<string> ToExcludePackingDebFileExtensionsPredicate(this DebUOSConfiguration configuration)
    {
        var excludePackingDebFileExtensions = configuration.ExcludePackingDebFileExtensions;
        var extensions = excludePackingDebFileExtensions?.Split(';');
        if (extensions is null || extensions.Length == 0)
        {
            return static _ => true;
        }

        return entry =>
        {
            var file = entry;
            var extension = Path.GetExtension(file);
            return extensions.Contains(extension);
        };
    }
}