namespace WelpenScoreboard.Domain.Common.Interfaces;
/// <summary>
/// Interface for a background program that runs a task non stop
/// </summary>
public interface IBackgroundProgram
{
    string Name { get; }

    Task BackgroundTask(CancellationToken token);
    Task InitializationTask(CancellationToken token);
    Task LogTask(Exception ex);
    Task StopTask();
}
