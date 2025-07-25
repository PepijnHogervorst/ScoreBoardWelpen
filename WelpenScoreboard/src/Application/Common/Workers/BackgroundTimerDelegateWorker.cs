using Microsoft.Extensions.Logging;
using WelpenScoreboard.Domain.Common.Interfaces;

namespace WelpenScoreboard.Application.Common.Workers;
/// <summary>
/// Runs a given task non stop in background with a given interval </br>
/// Optional initialization task can be provided
/// </summary>
public class BackgroundTimerDelegateWorker : BackgroundTimerWorker
{
    private readonly IBackgroundProgram _program;


    public BackgroundTimerDelegateWorker(IBackgroundTimerProgram program,
                                         ILogger logger)
        : base(program.Name, logger, program.Interval)
    {
        _program = program;
    }


    protected override Task InitializationTask(CancellationToken token)
    {
        if (_program.InitializationTask == null) return Task.CompletedTask;
        return _program.InitializationTask(token);
    }

    protected override Task LogError(Exception ex)
    {
        if (_program.LogTask == null) return Task.CompletedTask;
        return _program.LogTask(ex);
    }

    protected override Task StopTask()
    {
        if (_program.StopTask == null) return Task.CompletedTask;
        return _program.StopTask();
    }

    protected override Task PeriodicTask(CancellationToken token) => _program.BackgroundTask(token);
}
