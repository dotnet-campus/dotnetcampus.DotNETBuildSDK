using System.Text;

namespace dotnetCampus.DotNETBuild.Utils;

public class DotNetToolInfo
{
    public DotNetToolInfo(string packageId, string version, string commands)
    {
        PackageId = packageId;
        Version = version;
        Commands = commands;
    }

    public string PackageId { get; }
    public string Version { get; }
    public string Commands { get; }
}