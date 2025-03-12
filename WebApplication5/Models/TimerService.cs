using System;
using System.Threading;
using System.Threading.Tasks;

public class TimerService : IDisposable
{
    private Timer _timer;
    private readonly TimeSpan _interval;
    private readonly Func<Task> _callback;
    private CancellationTokenSource _cancellationTokenSource;

    public TimerService(TimeSpan interval, Func<Task> callback)
    {
        _interval = interval;
        _callback = callback;
    }

    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;
        _timer = new Timer(async _ => await TimerCallback(cancellationToken), null, TimeSpan.Zero, _interval);
    }

    private async Task TimerCallback(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        try
        {
            await _callback();
        }
        catch (OperationCanceledException)
        {
            // Ignore the cancellation exception
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _timer?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}
