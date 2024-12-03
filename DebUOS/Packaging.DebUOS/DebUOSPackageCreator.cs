using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Packaging.Targets;
using Packaging.Targets.IO;

namespace Packaging.DebUOS;

/// <summary>
/// 打出 UOS 的 deb 包
/// </summary>
/// 打包细节请参阅 [一步步教你在 Windows 上构建 dotnet 系应用的 UOS 软件安装包](https://blog.lindexi.com/post/%E4%B8%80%E6%AD%A5%E6%AD%A5%E6%95%99%E4%BD%A0%E5%9C%A8-Windows-%E4%B8%8A%E6%9E%84%E5%BB%BA-dotnet-%E7%B3%BB%E5%BA%94%E7%94%A8%E7%9A%84-UOS-%E8%BD%AF%E4%BB%B6%E5%AE%89%E8%A3%85%E5%8C%85.html )
// ReSharper disable once InconsistentNaming
public class DebUOSPackageCreator
{
    public DebUOSPackageCreator(ILogger logger)
    {
        Logger = logger;
    }

    public ILogger Logger { get; }

    public void PackageDeb(DirectoryInfo packingFolder, FileInfo outputDebFile, DirectoryInfo? workingFolder = null,
        Predicate<string>? optFileCanIncludePredicate = null)
    {
        Logger.LogInformation($"开始打包。Start packaging UOS deb from '{packingFolder.FullName}' to '{outputDebFile.FullName}'");

        ArchiveBuilder archiveBuilder = new ArchiveBuilder();

        workingFolder ??= packingFolder;
        var debTarFilePath = Path.Combine(workingFolder.FullName, "deb.tar");
        var debTarXzPath = Path.Combine(workingFolder.FullName, "deb.tar.xz");

        var archiveEntries = new List<ArchiveEntry>();
        foreach (var subFolder in packingFolder.EnumerateDirectories())
        {
            if (subFolder.Name == "DEBIAN")
            {
                // 这里面存放的包控制信息，不需要加入压缩
                continue;
            }

            archiveBuilder.AddDirectory(subFolder.FullName, string.Empty, $"/{subFolder.Name}", archiveEntries,
                optFileCanIncludePredicate);
        }

        EnsureDirectories(archiveEntries);

        archiveEntries = archiveEntries
            .OrderBy(e => e.TargetPathWithFinalSlash, StringComparer.Ordinal)
            .ToList();

        using (var targetStream = outputDebFile.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        using (var tarStream = File.Open(debTarFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
            TarFileCreator.FromArchiveEntries(archiveEntries, tarStream);
            tarStream.Position = 0;

            // 由于 XZOutputStream 类质量低劣（连当前位置都不知道，需要 Dispose 才能完成压缩等等）
            // 因此需要先 Dispose 再重新打开
            // XZOutputStream class has low quality (doesn't even know it's current position,
            // needs to be disposed to finish compression, etc),
            // So we are doing compression in a separate step
            using (var tarXzStream = File.Open(debTarXzPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            using (var xzStream = new XZOutputStream(tarXzStream))
            {
                tarStream.CopyTo(xzStream);
            }

            // 重新打开
            using (var tarXzStream = File.Open(debTarXzPath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var pkgPackageFormatVersion = new Version(2, 0);

                ArFileCreator.WriteMagic(targetStream);
                ArFileCreator.WriteEntry(targetStream, "debian-binary", ArFileMode, pkgPackageFormatVersion + "\n");
                WriteControl(packingFolder, targetStream);
                ArFileCreator.WriteEntry(targetStream, "data.tar.xz", ArFileMode, tarXzStream);
            }
        }

        Logger.LogInformation($"打包完成 '{outputDebFile.FullName}'");
    }

    private void WriteControl(DirectoryInfo packingFolder, Stream targetStream)
    {
        var controlTar = new MemoryStream();
        WriteControlEntry(controlTar, "./");
        foreach (var file in new[] { "control", "preinst", "postinst", "prerm", "postrm" })
        {
            var filePath = Path.Combine(packingFolder.FullName, "DEBIAN", file);
            if (File.Exists(filePath))
            {
                var fileText = File.ReadAllText(filePath);
                var mode = file == "control"
                    ? LinuxFileMode.S_IRUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IROTH
                    : LinuxFileMode.S_IRUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IXUSR | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IXGRP | LinuxFileMode.S_IROTH | LinuxFileMode.S_IXOTH;
                WriteControlEntry(controlTar, $"./{file}", fileText, mode);
            }
        }

        TarFileCreator.WriteTrailer(controlTar);
        controlTar.Seek(0, SeekOrigin.Begin);

        var controlTarGz = new MemoryStream();
        using (var gzStream = new GZipStream(controlTarGz, CompressionMode.Compress, true))
        {
            controlTar.CopyTo(gzStream);
        }

        controlTarGz.Seek(0, SeekOrigin.Begin);
        ArFileCreator.WriteEntry(targetStream, "control.tar.gz", ArFileMode, controlTarGz);
    }

    private static void WriteControlEntry(Stream tar, string name, string data = null, LinuxFileMode? fileMode = null)
    {
        var s = (data != null) ? new MemoryStream(Encoding.UTF8.GetBytes(data)) : new MemoryStream();
        var mode = fileMode ?? LinuxFileMode.S_IRUSR | LinuxFileMode.S_IWUSR |
            LinuxFileMode.S_IRGRP | LinuxFileMode.S_IROTH;
        mode |= data == null
            ? LinuxFileMode.S_IFDIR | LinuxFileMode.S_IXUSR | LinuxFileMode.S_IXGRP | LinuxFileMode.S_IXOTH
            : LinuxFileMode.S_IFREG;
        var hdr = new TarHeader
        {
            FileMode = mode,
            FileName = name,
            FileSize = (uint)s.Length,
            GroupName = "root",
            UserName = "root",
            LastModified = DateTimeOffset.UtcNow,
            Magic = "ustar",
            TypeFlag = data == null ? TarTypeFlag.DirType : TarTypeFlag.RegType,
        };
        TarFileCreator.WriteEntry(tar, hdr, s);
    }

    internal static void EnsureDirectories(List<ArchiveEntry> entries, bool includeRoot = true)
    {
        var dirs = new HashSet<string>(entries.Where(x => x.Mode.HasFlag(LinuxFileMode.S_IFDIR))
            .Select(d => d.TargetPathWithoutFinalSlash));

        var toAdd = new List<ArchiveEntry>();

        string GetDirPath(string path)
        {
            path = path.TrimEnd('/');
            if (path == string.Empty)
            {
                return "/";
            }

            if (!path.Contains("/"))
            {
                return string.Empty;
            }

            return path.Substring(0, path.LastIndexOf('/'));
        }

        void EnsureDir(string dirPath)
        {
            if (dirPath == string.Empty || dirPath == ".")
            {
                return;
            }

            if (!dirs.Contains(dirPath))
            {
                if (dirPath != "/")
                {
                    EnsureDir(GetDirPath(dirPath));
                }

                dirs.Add(dirPath);
                toAdd.Add(new ArchiveEntry()
                {
                    Mode = LinuxFileMode.S_IXOTH | LinuxFileMode.S_IROTH | LinuxFileMode.S_IXGRP |
                           LinuxFileMode.S_IRGRP | LinuxFileMode.S_IXUSR | LinuxFileMode.S_IWUSR |
                           LinuxFileMode.S_IRUSR | LinuxFileMode.S_IFDIR,
                    Modified = DateTime.Now,
                    Group = "root",
                    Owner = "root",
                    TargetPath = dirPath,
                    LinkTo = string.Empty,
                });
            }
        }

        foreach (var entry in entries)
        {
            EnsureDir(GetDirPath(entry.TargetPathWithFinalSlash));
        }

        if (includeRoot)
        {
            EnsureDir("/");
        }

        entries.AddRange(toAdd);
    }

    private const LinuxFileMode ArFileMode = LinuxFileMode.S_IRUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IRGRP |
                                             LinuxFileMode.S_IROTH | LinuxFileMode.S_IFREG;
}