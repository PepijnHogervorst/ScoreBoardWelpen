using Microsoft.Extensions.Logging;
using WelpenScoreboard.Domain.Common.Interfaces;

namespace WelpenScoreboard.Application.Common.Workers;
/// <summary>
/// Background worker base that runs a given task non stop
/// </summary>
public abstract class BackgroundWorker : IBackgroundWorker
{
    protected readonly string _backgroundWorkerName;
    protected readonly ILogger _logger;

    protected CancellationTokenSource _cancellationTokenSource = new();
    protected Task? _task;


    public bool IsRunning => _task is not null;


    public BackgroundWorker(string backgroundWorkerName, ILogger logger)
    {
        _backgroundWorkerName = backgroundWorkerName;
        _logger = logger;
    }

    public BackgroundWorker(ILogger logger)
    {
        _backgroundWorkerName = GetType().Name;
        _logger = logger;
    }


    public virtual void Start()
    {
        if (IsRunning) return;

        _cancellationTokenSource = new();
        _task = DoWorkAsync();
    }

    public virtual void Stop()
    {
        if (_task is null) return;

        _cancellationTokenSource.Cancel();
        _task = null;
    }

    public virtual async Task StopAsync()
    {
        if (_task is null) return;

        _cancellationTokenSource.Cancel();
        await _task;
        _cancellationTokenSource.Dispose();
        _task = null;
    }


    /// <summary>
    /// The asynchronous task that is executed continuously
    /// </summary>
    protected abstract Task TaskToRun(CancellationToken token);

    /// <summary>
    /// Task that is executed when the worker is started. Will be executed just once. By default simple returns
    /// </summary>
    protected virtual Task InitializationTask(CancellationToken token) { return Task.CompletedTask; }

    /// <summary>
    /// Task that is called when the worker is stopped. By default does nothing but can be overwritten to do something specific
    /// </summary>
    protected virtual Task StopTask() { return Task.CompletedTask; }

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

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await TaskToRun(_cancellationTokenSource.Token).ConfigureAwait(false);
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
}
