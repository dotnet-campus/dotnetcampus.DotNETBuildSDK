using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Microsoft.Extensions.Logging;
using Path = System.IO.Path;
using Microsoft.EntityFrameworkCore;

namespace UsingHardLinkToZipNtfsDiskSize;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Grid_OnDragEnter(object sender, DragEventArgs e)
    {
        var data = e.Data.GetData(DataFormats.FileDrop) as string[];
        if (data != null && data.Length > 0)
        {
            var folder = data[0];
            if (Directory.Exists(folder))
            {
                _ = StartUsingHardLinkToZipNtfsDiskSize(folder);
            }
        }
    }

    private async ValueTask StartUsingHardLinkToZipNtfsDiskSize(string folder)
    {
        // 转换为日志存储文件夹
        var hexString = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(folder)));

        var logFolder = Path.GetFullPath(Path.Join(hexString, $"Log_{DateTime.Now:yyyy.MM.dd}"));
        Directory.CreateDirectory(logFolder);

        var logFileStringLoggerWriter = new LogFileStringLoggerWriter(new DirectoryInfo(logFolder));
        var dispatcherStringLoggerWriter = new DispatcherStringLoggerWriter(LogTextBlock);
        using var channelLoggerProvider = new ChannelLoggerProvider(logFileStringLoggerWriter, dispatcherStringLoggerWriter);
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "yyyy.MM.dd HH:mm:ss ";
            });
            // ReSharper disable once AccessToDisposedClosure
            builder.AddProvider(channelLoggerProvider);
        });

        var sqliteFile = Path.Join(hexString, "FileManger.db");
        await using (var fileStorageContext = new FileStorageContext(sqliteFile))
        {
            await fileStorageContext.Database.MigrateAsync();
        }

        var logger = loggerFactory.CreateLogger("");

        logger.LogInformation($"Start zip {folder} folder. LogFolder={logFolder}");

        await Task.Run(async () =>
        {
            await using var fileStorageContext = new FileStorageContext(sqliteFile);

            var provider = new UsingHardLinkToZipNtfsDiskSizeProvider();
            await provider.Start(new DirectoryInfo(folder), fileStorageContext, logger);
        });
    }

    /// <summary>
    /// 损伤修复措施
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="logger"></param>
    /// <param name="logFolder"></param>
    /// <param name="sqliteFile"></param>
    /// <returns></returns>
    /// 暂时不需要调用，因为损伤是代码逻辑写错，后续修复代码逻辑就用不到损伤修复。但是代码放着，也许后面依然有其他诡异的问题，可以继续修复
    private static async Task FixBreakFile(string folder, ILogger logger, string logFolder, string sqliteFile)
    {
        await Task.Run(async () =>
        {
            logger.LogInformation($"开始损伤修复 {folder} 文件夹  LogFolder={logFolder}");

            try
            {
                await using (var fileStorageContext = new FileStorageContext(sqliteFile))
                {
                    logger.LogInformation($"开始打开数据库文件 {sqliteFile}");

                    // 查找所有的记录文件
                    await foreach (var fileRecordModel in fileStorageContext.FileRecordModel)
                    {
                        var filePath = fileRecordModel.FilePath;
                        logger.LogInformation($"开始 {filePath}");
                        var fileExists = File.Exists(filePath);
                        logger.LogInformation($"判断 {filePath} 文件存在：{fileExists}");

                        if (!fileExists)
                        {
                            // 文件误删
                            logger.LogInformation($"文件误删 {filePath} 尝试执行修复逻辑");

                            var success = false;

                            foreach (var recordModel in fileStorageContext.FileRecordModel.Where(t =>
                                         t.FileSha1Hash == fileRecordModel.FileSha1Hash))
                            {
                                var fixFileExists = File.Exists(recordModel.FilePath);

                                logger.LogInformation(
                                    $"SHA1={fileRecordModel.FileSha1Hash} 找到相近文件 {recordModel.FilePath} 修复的文件存在：{fixFileExists}");

                                if (fixFileExists)
                                {
                                    logger.LogInformation($"准备拷贝文件修复 {recordModel.FilePath} 到 {filePath}");
                                    var result = UsingHardLinkToZipNtfsDiskSizeProvider.CreateHardLink(filePath,
                                        recordModel.FilePath, logger);

                                    logger.LogInformation($"完成拷贝文件修复 结果={result} {recordModel.FilePath} 到 {filePath}");

                                    success = File.Exists(filePath);

                                    if (!success)
                                    {
                                        logger.LogInformation($"修复失败！拷贝之后依然不存在文件");
                                    }

                                    break;
                                }
                            }

                            if (success)
                            {
                                logger.LogInformation($"文件误删 {filePath} 修复成功");
                            }
                            else
                            {
                                logger.LogInformation($"文件误删 {filePath} 修复失败，没有找到相似且存在的文件");
                            }

                            //Dispatcher.Invoke(() => MessageBox.Show($"修复 {filePath}"));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogInformation($"执行失败 {e}");
            }

            logger.LogInformation("执行完成");
        });
    }
}