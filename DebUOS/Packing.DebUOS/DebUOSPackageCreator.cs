using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.MSBuildUtils;

using Packaging.Targets;
using Packaging.Targets.IO;

namespace Packing.DebUOS;

public class DebUOSPackageCreator
{
    public void PackageDeb(DirectoryInfo packingFolder, FileInfo outputDebFile, DirectoryInfo? workingFolder = null)
    {
        ArchiveBuilder archiveBuilder = new ArchiveBuilder()
        {
        };

        workingFolder ??= packingFolder;
        var debTarFilePath = Path.Combine(workingFolder.FullName, "deb.tar");
        var debTarXzPath = Path.Combine(workingFolder.FullName, "deb.tar.xz");

        var optFolder = Path.Combine(packingFolder.FullName, "opt");

        var archiveEntries = archiveBuilder.FromDirectory(
            optFolder,
            null,
            "/opt");

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
    }

    private void WriteControl(DirectoryInfo packingFolder, Stream targetStream)
    {
        var controlTar = new MemoryStream();
        WriteControlEntry(controlTar, "./");

        var controlFile = Path.Combine(packingFolder.FullName, "DEBIAN", "control");
        var controlFileText = File.ReadAllText(controlFile);

        WriteControlEntry(controlTar, "./control", controlFileText);
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
            FileSize = (uint) s.Length,
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
