namespace WelpenScoreboard.Domain.Common.Interfaces;
/// <summary>
/// Background worker interface to start / stop a background task
/// </summary>
public interface IBackgroundWorker
{
    /// <summary>
    /// Signals if the background worker is (running) or not
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Start the background worker
    /// </summary>
    void Start();
    void Stop();

    /// <summary>
    /// Async stop, awaits the task to complete after canceling
    /// </summary>
    Task StopAsync();
}
