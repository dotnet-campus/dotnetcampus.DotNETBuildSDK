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
    public void Execute()
    {
        ArchiveBuilder archiveBuilder = new ArchiveBuilder()
        {
        };

        var optFolder = Path.Combine(WorkFolder, "opt");

        var archiveEntries = archiveBuilder.FromDirectory(
            optFolder,
            null,
            "/opt");
        archiveEntries = archiveEntries
            .OrderBy(e => e.TargetPathWithFinalSlash, StringComparer.Ordinal)
            .ToList();

        using (var targetStream = File.Open(this.DebPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        using (var tarStream = File.Open(this.DebTarPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
            TarFileCreator.FromArchiveEntries(archiveEntries, tarStream);
            tarStream.Position = 0;

            var debTarXzPath = Path.Combine(WorkFolder, "deb.tar.xz");

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
                var pkgPackageFormatVersion  = new Version(2, 0);

                ArFileCreator.WriteMagic(targetStream);
                ArFileCreator.WriteEntry(targetStream, "debian-binary", ArFileMode, pkgPackageFormatVersion + "\n");
                WriteControl(targetStream);
                ArFileCreator.WriteEntry(targetStream, "data.tar.xz", ArFileMode, tarXzStream);
            }
        }
    }

    private void WriteControl(Stream targetStream)
    {
        var controlTar = new MemoryStream();
        WriteControlEntry(controlTar, "./");

        var controlFile = Path.Combine(WorkFolder, "DEBIAN", "control");
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


    public string WorkFolder { set; get; } = @"C:\lindexi\Work\";
    public string DebPath { get; init; } = "DebPath.deb";
    public string DebTarPath { get; init; } = "DebTarPath.tar";

    private const LinuxFileMode ArFileMode = LinuxFileMode.S_IRUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IRGRP |
                                             LinuxFileMode.S_IROTH | LinuxFileMode.S_IFREG;
}
