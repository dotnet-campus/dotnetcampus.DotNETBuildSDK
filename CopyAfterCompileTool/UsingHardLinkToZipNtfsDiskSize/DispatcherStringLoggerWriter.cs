using System.Windows.Controls;

namespace UsingHardLinkToZipNtfsDiskSize;

class DispatcherStringLoggerWriter : IStringLoggerWriter
{
    public DispatcherStringLoggerWriter(TextBlock logTextBlock)
    {
        _logTextBlock = logTextBlock;
    }

    private readonly TextBlock _logTextBlock;

    private string _lastMessage = string.Empty;
    private bool _isInvalidate = false;

    public ValueTask WriteAsync(string message)
    {
        _lastMessage = message;

        if (!_isInvalidate)
        {
            _isInvalidate = true;

            _logTextBlock.Dispatcher.InvokeAsync(() =>
            {
                _logTextBlock.Text = _lastMessage;
                _isInvalidate = false;
            });
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}