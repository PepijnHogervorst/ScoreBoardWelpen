using Microsoft.Extensions.Logging;
using WelpenScoreboard.Domain.Common.Interfaces;

namespace WelpenScoreboard.Application.Common.Workers;
/// <summary>
/// Background worker base that periodically calls a given task
/// </summary>
public abstract class BackgroundTimerWorker : IBackgroundWorker
{
    protected readonly string _backgroundWorkerName;
    protected readonly ILogger _logger;
    protected readonly TimeSpan _interval;

    protected CancellationTokenSource _cancellationTokenSource = new();
    protected PeriodicTimer _timer = null!;
    protected Task? _timerTask;


    public bool IsRunning => _timerTask is not null;


    public BackgroundTimerWorker(string backgroundWorkerName, ILogger logger, TimeSpan interval = default)
    {
        _backgroundWorkerName = backgroundWorkerName;
        _logger = logger;
        _interval = GetInterval(interval);
    }

    public BackgroundTimerWorker(ILogger logger, TimeSpan interval = default)
    {
        _backgroundWorkerName = GetType().Name;
        _logger = logger;
        _interval = GetInterval(interval);
    }


    public virtual void Start()
    {
        if (_timerTask is not null)
        {
            if (_timerTask.Status == TaskStatus.Running) return;
            _cancellationTokenSource.Cancel();
        }

        _cancellationTokenSource = new();
        _timer = new(_interval);
        _timerTask = DoWorkAsync();
    }

    public virtual void Stop()
    {
        if (_timerTask is null) return;

        _cancellationTokenSource.Cancel();
        _timerTask = null;
    }

    public virtual async Task StopAsync()
    {
        if (_timerTask is null) return;

        _cancellationTokenSource.Cancel();
        await _timerTask;
        _cancellationTokenSource.Dispose();
        _timerTask = null;
    }


    /// <summary>
    /// The asynchronous task that is executed every given interval (default once every second)
    /// </summary>
    protected abstract Task PeriodicTask(CancellationToken token);

    /// <summary>
    /// Task that is executed when the worker is started. Will be executed just once. By default simple returns
    /// </summary>
    protected virtual Task InitializationTask(CancellationToken token) { return Task.CompletedTask; }

    /// <summary>
    /// Task that is called when the worker is stopped. By default does nothing but can be overwritten to do something specific
    /// </summary>
    protected virtual Task StopTask() { return Task.CompletedTask; }

    /// <summary>
    /// Task to log the error generated in the async task
    /// Can be overwritten
    /// </summary>
    protected virtual Task LogError(Exception ex)
    {
        _logger.LogError(ex, "{ClassName}: `{BackgroundWorkerName}` ran into a problem and terminated..", nameof(BackgroundWorker), _backgroundWorkerName);
        return Task.CompletedTask;
    }


    private async Task DoWorkAsync()
    {
        try
        {
            await InitializationTask(_cancellationTokenSource.Token).ConfigureAwait(false);

            while (await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token) &&
                   !_cancellationTokenSource.IsCancellationRequested)
            {
                await PeriodicTask(_cancellationTokenSource.Token).ConfigureAwait(true);
            }
        }
        catch (OperationCanceledException)
        {
            await StopTask();
        }
        catch (Exception ex)
        {
            await LogError(ex);
        }
    }

    private static TimeSpan GetInterval(TimeSpan interval)
    {
        if (interval == default) return TimeSpan.FromSeconds(1);
        return interval;
    }
}
