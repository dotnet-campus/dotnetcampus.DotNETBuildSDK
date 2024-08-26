// See https://aka.ms/new-console-template for more information

using dotnetCampus.Cli;

var option = CommandLine.Parse(args).As<Option>();

CopyFolder(Directory.CreateDirectory(option.Folder), Directory.CreateDirectory(option.OutputFolder),
    name => name.Replace(option.Content, option.ReplaceText));

static void CopyFolder(DirectoryInfo originFolder, DirectoryInfo desFolder, Rename rename)
{
    foreach (var directory in originFolder.EnumerateDirectories())
    {
        var originName = directory.Name;
        var newName = rename(originName);

        var newDirectory = desFolder.CreateSubdirectory(newName);
        CopyFolder(directory, newDirectory, rename);
    }

    foreach (var file in originFolder.EnumerateFiles())
    {
        var originName = file.Name;
        var newName = rename(originName);

        var destFileName = Path.Join(desFolder.FullName, newName);

        try
        {
            // 尝试读取文件重写一下
            var content = File.ReadAllText(file.FullName);
            var output = rename(content);
            File.WriteAllText(destFileName, output);
        }
        catch
        {
            file.CopyTo(destFileName, overwrite: true);
        }
    }
}

delegate string Rename(string name);

class Option
{
    [Option('f', nameof(Folder))]
    public string Folder { get; set; } = null!;

    [Option('o', "Output")]
    public string OutputFolder { get; set; } = null!;

    [Option('c', nameof(Content))]
    public string Content { get; set; } = null!;

    [Option('r', "Replace")]
    public string ReplaceText { get; set; } = null!;
}