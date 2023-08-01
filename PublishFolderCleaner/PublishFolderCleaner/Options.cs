#region

using dotnetCampus.Cli;

#endregion

namespace PublishFolderCleaner;

public class Options
{
    #region Properties

    [Option('p', "PublishFolder")] public string PublishFolder { set; get; } = null!;

    [Option('a', "ApplicationName")] public string ApplicationName { set; get; } = null!;

    [Option('s', "PublishSingleFile")]
    public bool PublishSingleFile { get; set; } = false;

    #endregion
}