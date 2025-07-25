using Microsoft.Extensions.Logging;
using WelpenScoreboard.Domain.Common.Interfaces;

namespace WelpenScoreboard.Application.Common.Workers;
/// <summary>
/// Runs a given task non stop</br>
/// Optionally runs an initialization task once before the main task
/// </summary>
public class BackgroundDelegateWorker : BackgroundWorker
{
    private readonly IBackgroundProgram _program;


    public BackgroundDelegateWorker(IBackgroundProgram program,
                                    ILogger logger)
        : base(program.Name, logger)
    {
        _program = program;
    }


    protected override Task InitializationTask(CancellationToken token)
    {
        if (_program.InitializationTask == null) return Task.CompletedTask;
        return _program.InitializationTask(token);
    }

    protected override Task StopTask()
    {
        if (_program.StopTask == null) return Task.CompletedTask;
        return _program.StopTask();
    }

    protected override Task LogError(Exception ex)
    {
        if (_program.LogTask == null) return Task.CompletedTask;
        return _program.LogTask(ex);
    }

    protected override Task TaskToRun(CancellationToken token) => _program.BackgroundTask(token);
}
