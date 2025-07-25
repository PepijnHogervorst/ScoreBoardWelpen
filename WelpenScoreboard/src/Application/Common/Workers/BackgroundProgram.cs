using Microsoft.Extensions.Logging;
using WelpenScoreboard.Domain.Common.Interfaces;

namespace WelpenScoreboard.Application.Common.Workers;
public abstract class BackgroundProgram : IBackgroundProgram
{
    protected readonly ILogger _logger;

    public string Name { get; }

    protected BackgroundProgram(ILogger logger, string? name = null)
    {
        _logger = logger;
        Name = name ?? $"BackgroundProgram.{GetType().Name}.{Guid.NewGuid()}";
    }


    public abstract Task BackgroundTask(CancellationToken token);

    public virtual Task InitializationTask(CancellationToken token) => Task.CompletedTask;

    public virtual Task LogTask(Exception ex)
    {
        _logger.LogError(ex, "Background program '{Name}' crash..", Name);
        return Task.CompletedTask;
    }

    public virtual Task StopTask() => Task.CompletedTask;
}
