using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Windows.Win32;
using Windows.Win32.Security;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace UsingHardLinkToZipNtfsDiskSize;

public class UsingHardLinkToZipNtfsDiskSizeProvider
{
    /// <summary>
    /// 开始将 <paramref name="workFolder"/> 文件夹里面重复的文件使用硬连接压缩磁盘空间
    /// </summary>
    /// <param name="workFolder"></param>
    /// <param name="sqliteFile"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public async Task Start(DirectoryInfo workFolder, FileInfo sqliteFile, ILogger logger)
    {
        await using (var fileStorageContext = new FileStorageContext(sqliteFile.FullName))
        {
            await fileStorageContext.Database.MigrateAsync();
        }

        var destination = new byte[1024];
        long saveSize = 0;
        long count = 0;
        foreach (var file in workFolder.EnumerateFiles("*", enumerationOptions: new EnumerationOptions()
        {
            RecurseSubdirectories = true,
            MaxRecursionDepth = 100,
        }))
        {
            logger.LogInformation($"Start 第 {count} 个文件 {file}");
            count++;

            try
            {
                await using var fileStorageContext = new FileStorageContext(sqliteFile.FullName);

                long fileLength = file.Length;

                var fileRecordModel = await fileStorageContext.FileRecordModel.FindAsync(file.FullName);
                if (fileRecordModel != null)
                {
                    if (fileRecordModel.FileLength == fileLength)
                    {
                        // 上次压缩过了，不要重复处理
                        continue;
                    }
                }

                string sha1;
                await using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var length = await SHA1.HashDataAsync(fileStream, destination);
                    sha1 = Convert.ToHexString(destination, 0, length);
                }

                fileStorageContext.FileRecordModel.Add(new FileRecordModel()
                {
                    FilePath = file.FullName,
                    FileLength = fileLength,
                    FileSha1Hash = sha1,
                });

                var fileStorageModel = await fileStorageContext.FileStorageModel.FindAsync(sha1);
                if (fileStorageModel != null)
                {
                    if (fileStorageModel.FileLength != fileLength)
                    {
                        logger.LogInformation($"SHA1={sha1} `fileStorageModel.FileLength != fileLength` fileStorageModel.FileLength={fileStorageModel.FileLength} fileLength={fileLength} fileStorageModel.OriginFilePath={fileStorageModel.OriginFilePath} file={file.FullName} 文件尺寸不匹配，不执行逻辑");
                        continue;
                    }

                    if (CreateHardLink(file.FullName, fileStorageModel.OriginFilePath, logger))
                    {
                        // 省的空间
                        saveSize += fileLength;
                        logger.LogInformation($"Exists Record SHA1={sha1} {file} FileSize={UnitConverter.ConvertSize(fileLength, separators: " ")} SaveSize：{UnitConverter.ConvertSize(saveSize, separators: " ")}");

                        fileStorageModel.ReferenceCount++;
                        fileStorageContext.FileStorageModel.Update(fileStorageModel);
                    }
                    else
                    {
                        // 拷贝失败的情况，可能是超过 Win32 限制数量
                        // > The maximum number of hard links that can be created with this function is 1023 per file. If more than 1023 links are created for a file, an error results.
                        // 此时换成新的文件记录即可修复此问题
                        fileStorageModel.OriginFilePath = file.FullName;
                        fileStorageContext.FileStorageModel.Update(fileStorageModel);
                    }

                    if (!File.Exists(file.FullName))
                    {
                        logger.LogInformation($"Error Break File {file.FullName} 文件损坏，文件找不到");
                    }
                }
                else
                {
                    fileStorageModel = new FileStorageModel()
                    {
                        FileLength = fileLength,
                        FileSha1Hash = sha1,
                        OriginFilePath = file.FullName,
                        ReferenceCount = 1
                    };
                    fileStorageContext.FileStorageModel.Add(fileStorageModel);

                    logger.LogInformation($"Not exists Record {file} SHA1={sha1}");
                }

                await fileStorageContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogWarning($"Hard link fail {file} {e}");
            }
        }

        logger.LogInformation($"Total save disk size: {UnitConverter.ConvertSize(saveSize, separators: " ")}");
    }

    /// <summary>
    /// 创建硬连接
    /// </summary>
    /// <param name="file"></param>
    /// <param name="originFilePath"></param>
    /// <param name="logger"></param>
    /// <returns>返回 false 表示创建失败</returns>
    public static bool CreateHardLink(string file, string originFilePath, ILogger logger)
    {
        if (file == originFilePath)
        {
            logger.LogInformation($"[CreateHardLink] 传入的原始文件相同，返回 false 啥都不做");
            return false;
        }

        if (!File.Exists(originFilePath))
        {
            logger.LogInformation($"[CreateHardLink] 传入的 originFilePath={originFilePath} 文件不存在");
            return false;
        }

        if (File.Exists(file))
        {
            logger.LogInformation($"[CreateHardLink] 传入的文件 file={file} 存在，正在删除");
            File.Delete(file);
        }

        var lpSecurityAttributes = new SECURITY_ATTRIBUTES();
        var result = PInvoke.CreateHardLink(file, originFilePath, ref lpSecurityAttributes);
        logger.LogInformation($"[CreateHardLink] PInvoke 结果={result == true} file={file} originFilePath={originFilePath}");

        if (!result)
        {
            // 以下三个都获取不正确错误号
            // 如 An attempt was made to create more links on a file than the file system supports.
            var lastWin32Error = Marshal.GetLastWin32Error();
            logger.LogInformation($"[CreateHardLink] PInvoke 结果={result == true} LastWin32Error={lastWin32Error} LastPInvokeError={Marshal.GetLastPInvokeError()} LastSystemError={Marshal.GetLastSystemError()}");
        }

        if (!File.Exists(file))
        {
            logger.LogInformation($"[CreateHardLink] 创建符号链接失败，只好复制文件。 Copy {originFilePath} to {file}");
            File.Copy(originFilePath, file);
        }

        return result;
    }
}